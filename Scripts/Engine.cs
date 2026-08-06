// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using Animo.Model;

namespace Animo.Core {
    /// <summary>
    /// (v0.1.5, Q-S115) Time abstraction for Engine.Live delta_time injection.
    /// Phase 3: Agent.Update calls _engine.Live(delta_time: _time_provider.deltaTime).
    /// MockTime implements this for deterministic headless tests.
    /// </summary>
    public interface ITimeProvider {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective]

        float deltaTime { get; }
    }

    public enum LockMode { Hard, Soft }

    public class Engine {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        readonly Persona _persona;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Pre-allocated hot-path arrays (§16.4)

        float[] _needs;
        float[] _effective_needs;
        // (#4) _previous_effective_needs removed: was a Q-S23 patch residue. After Q-S25
        //      introduced Threshold.is_above state machine, no logic reads the previous
        //      effective snapshot — only writes remained (alloc + seed + per-frame copy).
        float[] _action_scores;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Index maps (built once in ctor; cold path only)

        readonly Dictionary<string, int> _need_index;
        readonly Dictionary<string, int> _action_id_to_index;
        readonly Dictionary<int, int[]>  _need_tier_indices;  // per-Persona (Q-S30 + Q-S69)

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // String cache (§16.5)

        readonly Dictionary<string, string> _cached_action_triggers;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // State fields

        float  _lock_remaining       = 0.0f;       // Q-S70
        // (Q-S48) Per-Need decay multiplier cache. Built in PHASE C via
        // applyNonTierMetadata. Applied in Step 1 to rates[] values.
        // Index parallel to _needs[]. Default 1.0f (no change).
        float[] _decay_rates = Array.Empty<float>();
        // (§16.3.4 Pre-cache Principle) Flat parallel array for Step 1 (natural decay).
        // Index = _need_index slot. Built once in PHASE B.
        // Eliminates foreach-over-Dictionary + string TryGetValue in hot-path Step 1.
        float[] _rates_flat  = Array.Empty<float>();
        int    _locked_behavior_index = -1;         // Q-S142
        string _previous_behavior    = "";          // Q-S110 / Q-S31 sentinel
        string _current_behavior     = "";
        bool   _force_reset_pending  = false;       // Q-S5

        LockMode _lock_mode          = LockMode.Hard;

        // (#2 Zero-GC) Pre-cached non-null List<Threshold> reference avoids per-frame
        // IReadOnlyList cast boxing in Step 3 hot path. Set in ctor.
        List<Threshold> _thresholds = null!;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Engine(Persona persona) {
            _persona = persona;

            // ── PHASE A (Q-S27): build _need_index ────────────────────────
            _need_index = new Dictionary<string, int>();
            for (int i = 0; i < Const.STANDARD_NEEDS.Count; i++)
                _need_index[Const.STANDARD_NEEDS[i]] = i;
            int next_index = Const.STANDARD_NEEDS.Count;
            // (Q-S65) iterate _persona.needs?.values
            foreach (var entry in _persona.needs?.values ?? new Dictionary<string, float>())
                if (!_need_index.ContainsKey(entry.Key))
                    _need_index[entry.Key] = next_index++;

            // PHASE A.2: needs_meta-only slots
            if (_persona.needs_meta != null)
                foreach (var entry in _persona.needs_meta)
                    if (!_need_index.ContainsKey(entry.Key))
                        _need_index[entry.Key] = next_index++;

            int n = next_index;
            _needs                    = new float[n];
            _effective_needs          = new float[n];
            _decay_rates              = new float[n];
            for (int i = 0; i < n; i++) _decay_rates[i] = 1.0f;  // default: no multiplier

            // Seed _needs from spawn values (Q-S65)
            foreach (var entry in _persona.needs?.values ?? new Dictionary<string, float>())
                _needs[_need_index[entry.Key]] = entry.Value;

            // ── PHASE B (Q-S37): bake need_index into Action/Threshold ────
            foreach (var act in _persona.actions ?? new List<Animo.Model.Action>())
                act.need_index = _need_index[act.need];
            // (#2 Zero-GC) Cache thresholds List directly; avoid per-frame cast in hot path.
            _thresholds = _persona.binding?.thresholds ?? new List<Threshold>();
            foreach (var threshold in _thresholds)
                threshold.need_index = _need_index[threshold.need];

            // (§16.3.4) Bake source_index / target_index into each Influence.
            foreach (var influence in _persona.influences ?? new List<Influence>()) {
                influence.source_index = _need_index.TryGetValue(influence.source, out var source_index) ? source_index : -1;
                influence.target_index = _need_index.TryGetValue(influence.target, out var target_index) ? target_index : -1;
            }

            // (§16.3.4) Build _rates_flat: parallel to _needs[]. Step 1 uses this
            // flat float[] instead of foreach-over-Dictionary.
            _rates_flat = new float[n];
            if (_persona.rates != null)
                foreach (var entry in _persona.rates.values)
                    if (_need_index.TryGetValue(entry.Key, out var ri))
                        _rates_flat[ri] = entry.Value;

            // ── PHASE C (Q-S30 + Q-S69): build _need_tier_indices ─────────
            var scratch = new Dictionary<int, List<int>>();
            foreach (var entry in Const.NEED_INDICES_BY_TIER)
                scratch[entry.Key] = new List<int>(entry.Value);
            if (_persona.needs_meta != null)
                foreach (var entry in _persona.needs_meta) {
                    bool is_standard = false;
                    foreach (var standard_need in Const.STANDARD_NEEDS) if (standard_need == entry.Key) { is_standard = true; break; }
                    if (is_standard) continue;
                    int tier = entry.Value.tier;
                    if (!scratch.ContainsKey(tier)) scratch[tier] = new List<int>();
                    scratch[tier].Add(_need_index[entry.Key]);
                }
            _need_tier_indices = new Dictionary<int, int[]>();
            foreach (var entry in scratch) _need_tier_indices[entry.Key] = entry.Value.ToArray();

            // PHASE C Step 3: applyNonTierMetadata for all needs
            foreach (var entry in _need_index) {
                NeedMeta meta = (_persona.needs_meta != null &&
                                 _persona.needs_meta.TryGetValue(entry.Key, out var em))
                                ? em : NeedMeta.DefaultFor(entry.Key);
                applyNonTierMetadata(entry.Value, meta);
            }

            // ── Action score array ─────────────────────────────────────────
            _action_scores     = new float[(_persona.actions?.Count) ?? 0];
            _action_id_to_index = new Dictionary<string, int>();
            for (int i = 0; i < (_persona.actions?.Count ?? 0); i++)
                _action_id_to_index[_persona.actions![i].id] = i;

            // ── String cache (§16.5, Q-S46 + Q-S53) ──────────────────────
            _cached_action_triggers = new Dictionary<string, string>();
            string template = _persona.binding?.on_action_change ?? Const.DEFAULT_ON_ACTION_CHANGE;
            foreach (var act in _persona.actions ?? new List<Animo.Model.Action>())
                _cached_action_triggers[act.id] = template
                    .Replace("{agent_id}", _persona.agent_id)
                    .Replace("{behavior}",  act.id);
            foreach (var threshold in _thresholds)
                threshold.expanded_trigger = threshold.trigger.Replace("{agent_id}", _persona.agent_id);

            // ── PHASE D (Q-S8 + Q-S23 + Q-S25): seed previous_eff + is_above
            step2EffectiveNeeds();
            foreach (var threshold in _thresholds)
                threshold.is_above = _effective_needs[threshold.need_index] >= threshold.trigger_threshold;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Events [verb, verb phrase]

        public event Action<string>? OnSignal;      // Q-S26

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public string AgentID => _persona.agent_id ?? "";

        public string Behavior   => _current_behavior;
        public bool   IsLocked   => _lock_remaining > 0f;  // Q-S126: computed property
        public string LockedBehavior =>
            (_locked_behavior_index >= 0 && _persona.actions != null &&
             _locked_behavior_index < _persona.actions.Count)
            ? _persona.actions[_locked_behavior_index].id : "";

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Live(delta_time) — 5 steps + T0

        public void Live(float delta_time) {
            // (Q-S117) Validate delta_time before any time-based math.
            if (float.IsNaN(delta_time))
                throw new ArgumentException("delta_time is NaN — would corrupt all Needs via decay.", nameof(delta_time));
            if (delta_time < 0f)
                throw new ArgumentException($"delta_time must be >= 0. Got {delta_time}.", nameof(delta_time));
            // T0: Lock timer (Q-S3)
            if (IsLocked) {
                _lock_remaining -= delta_time;
                if (_lock_remaining <= 0f) {
                    _lock_remaining = 0f;
                    _locked_behavior_index = -1;
                    // Unlock raises OnSignal for behavior change in Step 5 below
                }
            }

            // Step 1: natural decay (rates)
            // (§16.3.4 Pre-cache Principle) Use _rates_flat float[] — zero string lookup,
            // zero Dictionary boxing. Skip slots where rate == 0 (default: no decay).
            for (int i = 0; i < _rates_flat.Length; i++) {
                if (_rates_flat[i] == 0f) continue;
                // (Q-S48) Apply per-Need decay_multiplier from NeedMeta.
                float effective_rate = _rates_flat[i] * _decay_rates[i];
                _needs[i] = (float)System.Math.Clamp(_needs[i] + effective_rate * delta_time, 0f, 100f);
            }

            // Step 2: EffectiveNeeds cascade
            step2EffectiveNeeds();

            // Step 3: Threshold check
            step3Thresholds();

            // Step 4: Action score calc
            step4ScoreActions();

            // Step 5: switch decision.
            // (Q-S2, spec §24 line 5525, DECISION LOG Q-S2 line 334)
            // BOTH Hard and Soft lock skip Step 5 — behavior is frozen in both modes.
            // Soft Lock's "inner state keeps moving" refers to Steps 1-4 (decay, cascade,
            // threshold, score), NOT to Step 5 (switch). ROADMAP §5.6.1 3-3-k
            // "Step 5 runs but output is frozen" was an outdated description superseded
            // by the Q-S2 decision and spec §24 table.
            if (!IsLocked)
                step5Switch();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Public API

        public void Affect(string need, float delta, bool force_reset = false) {
            if (need == null) throw new ArgumentNullException(nameof(need));
            if (string.IsNullOrEmpty(need)) throw new ArgumentException("need cannot be empty.", nameof(need));
            if (float.IsNaN(delta)) throw new ArgumentException("delta is NaN.", nameof(delta));
            if (!_need_index.TryGetValue(need, out var index)) {
                AnimoLog.Warning($"Engine.Affect: need '{need}' is unknown (no-op).");
                return;
            }
            float new_value;
            if (float.IsInfinity(delta)) {
                new_value = delta > 0 ? 100f : 0f;
            } else {
                new_value = (float)System.Math.Clamp(_needs[index] + delta, 0f, 100f);
            }
            _needs[index] = new_value;
            // Also update effective_needs immediately so GetNeed() reflects the change
            // before the next Live(delta_time) runs Step2 (Q-S54 semantics: GetNeed reads effective).
            _effective_needs[index] = new_value;
            _force_reset_pending |= force_reset;  // Q-S5: OR-latch
        }

        public void Lock(float duration, LockMode mode = LockMode.Hard) {
            // (#4) NaN guard: float.NaN > MAX → false, < 0 → false, would slip past
            // all comparisons and write _lock_remaining = NaN, then NaN <= 0 → false
            // every frame → permanent freeze. Fail-loud is the correct response.
            if (float.IsNaN(duration))
                throw new ArgumentException("Lock duration must not be NaN.", nameof(duration));
            if (duration < 0f)
                throw new ArgumentException($"Lock duration must be >= 0. Got {duration}.", nameof(duration));
            // (§24.6.1) Hard cap at LOCK_DURATION_MAX before any other check.
            if (duration > Const.LOCK_DURATION_MAX)
                duration = Const.LOCK_DURATION_MAX;
            // (A031) Runtime warning when duration exceeds LOCK_DURATION_WARN_THRESHOLD (30s).
            if (duration > Const.LOCK_DURATION_WARN_THRESHOLD)
                AnimoLog.Warning(
                    $"[A031] Engine.Lock: duration {duration}s exceeds " +
                    $"LOCK_DURATION_WARN_THRESHOLD ({Const.LOCK_DURATION_WARN_THRESHOLD}s). " +
                    "Runaway Lock state risk.");
            _lock_remaining        = duration;
            _lock_mode             = mode;
            if (IsLocked && !string.IsNullOrEmpty(_current_behavior) &&
                _action_id_to_index.TryGetValue(_current_behavior, out var index))
                _locked_behavior_index = index;
            else if (!IsLocked)
                _locked_behavior_index = -1;
        }

        public void Unlock() {
            _lock_remaining        = 0f;
            _locked_behavior_index = -1;
        }

        public float GetNeed(string need) {
            if (need == null) throw new ArgumentNullException(nameof(need));
            if (string.IsNullOrEmpty(need)) throw new ArgumentException("need cannot be empty.", nameof(need));
            if (!_need_index.TryGetValue(need, out var index)) {
                AnimoLog.Warning($"Engine.GetNeed: '{need}' unknown."); return 0f;
            }
            return _effective_needs[index];
        }

        public float GetBaseNeed(string need) {
            if (need == null) throw new ArgumentNullException(nameof(need));
            if (string.IsNullOrEmpty(need)) throw new ArgumentException("need cannot be empty.", nameof(need));
            if (!_need_index.TryGetValue(need, out var index)) {
                AnimoLog.Warning($"Engine.GetBaseNeed: '{need}' unknown."); return 0f;
            }
            return _needs[index];
        }

        /// <summary>
        /// The whole visible state of the engine at this moment, in one object.
        /// This is the call the live monitor reads each frame: the chosen
        /// behavior, the lock state, every need as a base and an effective
        /// value, and the action scores. It reads, it does not change anything.
        /// </summary>
        public EngineSnapshot Snapshot() {
            var base_needs = new Dictionary<string, float>();
            var effective_needs = new Dictionary<string, float>();
            foreach (var name in GetAllNeedNames()) {
                base_needs[name] = GetBaseNeed(name);
                effective_needs[name] = GetNeed(name);
            }
            var action_scores = new Dictionary<string, float>();
            foreach (var id in GetAllActionIds())
                action_scores[id] = GetActionScore(id);
            return new EngineSnapshot(
                behavior: Behavior,
                is_locked: IsLocked,
                locked_behavior: LockedBehavior,
                base_needs: base_needs,
                effective_needs: effective_needs,
                action_scores: action_scores);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // internal Methods [verb]

        internal float GetEffectiveNeed(string need) =>
            _need_index.TryGetValue(need, out var i) ? _effective_needs[i] : 0f;

        internal float GetActionScore(string action_id) =>
            _action_id_to_index.TryGetValue(action_id, out var i) ? _action_scores[i] : 0f;

        internal IReadOnlyList<string> GetAllNeedNames() {
            var names = new string[_need_index.Count];
            foreach (var entry in _need_index) names[entry.Value] = entry.Key;
            return names;
        }

        internal IReadOnlyList<string> GetAllActionIds() =>
            _persona.actions?.ConvertAll(action => action.id) ?? new List<string>();

        internal string GetExpandedActionTrigger(string behavior) =>
            _cached_action_triggers.TryGetValue(behavior, out var threshold) ? threshold : behavior;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        protected void RaiseSignal(string signal_id) => OnSignal?.Invoke(signal_id);

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Step 2: EffectiveNeeds

        void step2EffectiveNeeds() {
            // Start from base needs
            Array.Copy(_needs, _effective_needs, _needs.Length);

            if (_persona.influences == null || _persona.influences.Count == 0) return;

            // (§16.3.4 Pre-cache Principle) Use pre-sorted order from Composer.
            // Zero allocation: iterate int[] built once at compose time.
            // If sorted_influence_order is null (directly-constructed Persona),
            // fall back to declaration order (safe; A025 catches cycles at validate time).
            var edges = _persona.influences;
            var order = _persona.sorted_influence_order;

            if (order != null) {
                for (int order_index = 0; order_index < order.Length; order_index++) {
                    var influence = edges[order[order_index]];
                    int source_index  = influence.source_index;
                    int target_index  = influence.target_index;
                    if (source_index < 0 || target_index < 0) continue;
                    float intensity = _effective_needs[source_index] / 100f;
                    float delta     = influence.coefficient * intensity * _effective_needs[source_index];
                    _effective_needs[target_index] = (float)System.Math.Clamp(_effective_needs[target_index] + delta, 0f, 100f);
                }
            } else {
                // Cold fallback: declaration order (direct Persona construction without Composer)
                for (int i = 0; i < edges.Count; i++) {
                    var influence = edges[i];
                    if (!_need_index.TryGetValue(influence.source, out var source_index)) continue;
                    if (!_need_index.TryGetValue(influence.target, out var target_index)) continue;
                    float intensity = _effective_needs[source_index] / 100f;
                    float delta     = influence.coefficient * intensity * _effective_needs[source_index];
                    _effective_needs[target_index] = (float)System.Math.Clamp(_effective_needs[target_index] + delta, 0f, 100f);
                }
            }
        }

        void step3Thresholds() {
            // foreach over concrete List<T> uses struct-enumerator (no alloc).
            foreach (var threshold in _thresholds) {
                float current  = _effective_needs[threshold.need_index];
                // (Q-S86) Composer always fills reset_threshold (Q-S11 contract).
                // Use !.Value — NRE on first frame is the correct fail-loud signal
                // if contract is violated (preferable to silent wrong-value fallback).
                float reset = threshold.reset_threshold!.Value;
                if (!threshold.is_above) {
                    if (current >= threshold.trigger_threshold) {
                        threshold.is_above = true;
                        RaiseSignal(threshold.expanded_trigger);
                    }
                } else {
                    if (current <= reset) threshold.is_above = false;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Step 4: Score

        void step4ScoreActions() {
            if (_persona.actions == null) return;
            for (int i = 0; i < _persona.actions.Count; i++) {
                var act     = _persona.actions[i];
                float effective   = _effective_needs[act.need_index];
                float intensity = effective / 100f;
                float score = (float)System.Math.Pow(intensity, act.exponent) * 100f;

                // (Q-S13) While locked, the bonus-skip is suppressed.
                // The latch (_force_reset_pending) survives the lock but does NOT
                // skip the bonus mid-lock — it is consumed on the first post-unlock Step 4.
                bool is_current = (act.id == _current_behavior);
                bool locked_act = (_locked_behavior_index >= 0 && _locked_behavior_index == i);
                bool skip_bonus = _force_reset_pending && !IsLocked;
                if ((is_current || locked_act) && !skip_bonus) {
                    float bonus = _persona.commitment?.bonus ?? 0f;
                    score += bonus;
                }

                // Apply Maslow dynamic suppression (§9.3.4)
                // score × (1 - suppression_factor[act.tier] × max_lower_tier_intensity)
                // suppression_factor is keyed on ACT.TIER (one value), not on t2 (loop var).
                float applied_suppression = 0f;
                if (_persona.suppression != null && act.tier > 1) {
                    // Determine the suppression coefficient for THIS action's tier.
                    float suppression_factor = act.tier == 2 ? _persona.suppression.tier2 :
                               act.tier == 3 ? _persona.suppression.tier3 :
                               act.tier == 4 ? _persona.suppression.tier4 :
                                               _persona.suppression.tier5;
                    if (suppression_factor > 0f) {
                        // Accumulate max need intensity from ALL lower tiers.
                        float max_lower = 0f;
                        for (int tier_index = 1; tier_index < act.tier; tier_index++) {
                            if (!_need_tier_indices.TryGetValue(tier_index, out var indices)) continue;
                            foreach (var need_index in indices) {
                                float normalized = _effective_needs[need_index] / 100f;
                                if (normalized > max_lower) max_lower = normalized;
                            }
                        }
                        applied_suppression = suppression_factor * max_lower;
                    }
                }
                score *= (1f - applied_suppression);

                _action_scores[i] = score;
            }

            // Clear force_reset if not locked (Q-S13)
            if (!IsLocked) _force_reset_pending = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Step 5: Switch

        void step5Switch() {
            if (_persona.actions == null || _persona.actions.Count == 0) return;

            // Pick best score (tie-break: declaration order / lowest index, Q-S9)
            int   best_index   = 0;
            float best_score = _action_scores[0];
            for (int i = 1; i < _action_scores.Length; i++)
                if (_action_scores[i] > best_score) { best_score = _action_scores[i]; best_index = i; }

            string new_behavior = _persona.actions[best_index].id;
            if (new_behavior != _current_behavior) {
                string previous = _current_behavior;
                _current_behavior = new_behavior;
                onBehaviorChanged(previous, new_behavior);
            }
            _previous_behavior = _current_behavior;
        }

        void onBehaviorChanged(string previous, string next_behavior) {
            if (previous == "") return;  // Q-S31: silent first transition
            if (_cached_action_triggers.TryGetValue(next_behavior, out var sig))
                RaiseSignal(sig);
        }


        void applyNonTierMetadata(int need_index, NeedMeta meta) {
            // (Q-S45 + Q-S48) Apply non-tier NeedMeta fields.
            // decay_multiplier: scales the rates[] value for this Need.
            // 1.0 = no change (default). Applied to _decay_rates[] cache;
            // Step 1 multiplies by this factor when updating _needs[].
            if (need_index >= 0 && need_index < _decay_rates.Length)
                _decay_rates[need_index] = meta.decay_multiplier;
        }

    }

    /// <summary>
    /// A read-only picture of the engine at one moment, made by Engine.Snapshot.
    /// The live monitor sends one of these to the dashboard each frame.
    /// </summary>
    [System.Serializable]
    public sealed class EngineSnapshot {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public EngineSnapshot(
            string behavior,
            bool is_locked,
            string locked_behavior,
            IReadOnlyDictionary<string, float> base_needs,
            IReadOnlyDictionary<string, float> effective_needs,
            IReadOnlyDictionary<string, float> action_scores) {
            this.behavior = behavior;
            this.is_locked = is_locked;
            this.locked_behavior = locked_behavior;
            this.base_needs = base_needs;
            this.effective_needs = effective_needs;
            this.action_scores = action_scores;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public string behavior { get; }
        public bool is_locked { get; }
        public string locked_behavior { get; }
        public IReadOnlyDictionary<string, float> base_needs { get; }
        public IReadOnlyDictionary<string, float> effective_needs { get; }
        public IReadOnlyDictionary<string, float> action_scores { get; }
    }

}
