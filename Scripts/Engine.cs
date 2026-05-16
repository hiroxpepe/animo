// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using Animo.Model;

namespace Animo.Core {
    /// <summary>Lock mode for behavior locking (v0.1.4). See spec §24.2.1.</summary>
    public enum LockMode {
        Hard,
        Soft
    }

    /// <summary>
    /// Animo AI calculation engine. Runs the 5-step Live(dt) per frame:
    /// natural decay → effective needs → threshold check → score → switch.
    /// See spec §9.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Engine {

        readonly Persona _persona;

        // (v0.1.5, Q-S70) Lock countdown timer (§24). Decremented by
        // `dt` at the start of every Live(dt) — the T0 timer phase
        // per §9.2. Reaching ≤0 triggers Unlock. Pre-Q-S70 the spec
        // referenced this field in §9.2 mermaid pseudocode and §24.3
        // narrative but never declared it in §16.6 or in this file.
        // Phase 3 implementation reads/writes during Live(dt) T0 and
        // public Lock/Unlock methods. CS0414 suppressed because the
        // field is intentionally unused in v0.1.5 stub (T0 phase
        // implemented in Phase 3).
        #pragma warning disable CS0414
        float _lock_remaining = 0.0f;
        #pragma warning restore CS0414

        // (v0.1.5, Q-S142) Cached index of the locked action in the
        // _action_scores float[] array. Pre-Q-S142 spec §24 and §3.4
        // referenced `_action_scores[locked_behavior_index]` (e.g. spec
        // line 237, 5421) but the field was never declared in §16.6 or
        // in this file — Phase 3 implementer writing Step 4 / Step 5
        // under Lock would hit a compile error.
        //
        // Cache rationale (Pre-cache Principle §16.1): Lock(duration)
        // is called on the cold path; `_engine.behavior` at lock time is
        // already resolved to an array index via `_need_index`. Storing the
        // integer index avoids a Dictionary<string, int> lookup on every
        // Hot Path Step 4 / Step 5 frame while locked. The field is -1
        // (sentinel "not locked") when no Lock is active; Lock() sets it
        // to the index of `behavior` at lock time; Unlock() resets to -1.
        // CS0414 suppressed for v0.1.5 stub.
        #pragma warning disable CS0414
        int _locked_behavior_index = -1;
        #pragma warning restore CS0414

        // (v0.1.5, Q-S110) Previous-frame behavior tracker for the
        // Q-S31 silent-first-transition contract (§16.6). Pre-Q-S110
        // the §16.6 fields table listed `_previous_behavior` but the
        // physical Engine.cs file declared only `_persona` and
        // `_lock_remaining` — the Q-S70 fix for `_lock_remaining`
        // closed the analogous gap, but the same fix was not applied
        // to `_previous_behavior`. Phase 3 implementer writing Step 5
        // (`if (_previous_behavior != new_behavior) RaiseSignal(...);
        //  _previous_behavior = new_behavior;`) would hit "the name
        // `_previous_behavior` does not exist" compile error.
        //
        // Initial value `""` (empty string) is the Q-S31 sentinel:
        // the only frame where `_previous_behavior == ""` is the very
        // first Step 5 of the Engine's life, which is exactly when
        // the silent-first-transition contract should suppress
        // OnSignal. After Step 5 writes a real behavior id once,
        // the sentinel can never reappear (snake_case action ids
        // are non-empty by A009). CS0414 suppressed for v0.1.5 stub.
        #pragma warning disable CS0414
        string _previous_behavior = "";
        #pragma warning restore CS0414

        public Engine(Persona persona) {
            _persona = persona;
        }

        // v0.1.5 (Q-S26): output channel for fire / behavior-change
        // signals. Engine is a pure C# library (§12.1) and does NOT
        // hold a reference to Germio.Bus — that reference belongs to
        // Animo.Agent (the MonoBehaviour). Engine raises this event
        // when a Threshold fires (Step 3) or when `behavior` actually
        // changes (Step 4 / Step 5); Agent subscribes once in Awake
        // and forwards the payload to `Bus.Publish(signal_id)`.
        // Pre-Q-S26 the only path described in §16.5 was a fictional
        // `_bus.Publish(...)` call inside Engine — architecturally
        // impossible because Engine has no Bus reference. OnSignal
        // is the missing wire.
        public event Action<string>? OnSignal;

        /// <summary>Current chosen action id. Empty before the first Live().</summary>
        /// <remarks>
        /// (v0.1.5, Q-S34) After Agent.Awake calls Live(dt: 0.0f) once
        /// to seed the initial behavior decision, this property carries
        /// the spawn-time chosen Action (typically actions[0] via Q-S9
        /// tie-break on equal scores). Hosts read this property to set
        /// their Animator/View state directly — Q-S31's silent-first-
        /// transition contract means OnSignal is NOT raised for the
        /// "" → actions[0] transition, so reading this property is
        /// the only way to know what to play. After spawn the property
        /// stays in sync with the most recent Step 5 decision and
        /// hosts that subscribe to OnSignal need not re-read it.
        /// </remarks>
        public string behavior => throw new NotImplementedException();

        /// <summary>Whether the engine is in Lock state.</summary>
        public bool is_locked => throw new NotImplementedException();

        /// <summary>The action id locked when Lock() was called. Empty if not locked.</summary>
        public string locked_behavior => throw new NotImplementedException();

        /// <summary>Advance the engine by dt seconds (5-step process).</summary>
        public void Live(float dt) {
            throw new NotImplementedException();
        }

        /// <summary>External stimulus. Add delta to the named Need; clamp to [0,100].</summary>
        public void Affect(string need, float delta, bool force_reset = false) {
            throw new NotImplementedException();
        }

        // (v0.1.5, Q-S86) Phase 3 implementation contract: Step3_Thresholds
        // hot path reads `t.reset_threshold!.Value` directly, NOT
        // `t.reset_threshold ?? Math.Max(...)`. The non-null guarantee
        // comes from Composer.Compose (Q-S11 contract): every Threshold
        // has its `reset_threshold` filled with `Math.Max(0f,
        // trigger_threshold - 5f)` if the author omitted it, BEFORE
        // returning the composed Persona. Engine.ctor receives only
        // composed Personas (via PersonaCache.GetComposed). The
        // null-forgiving operator (`!`) is therefore safe; a contract
        // violation would surface as NullReferenceException on the
        // FIRST frame's Step3, not silently as the wrong reset value.
        // This eliminates dead code in the §16.1 zero-overhead Hot Path.

        /// <summary>Lock the current behavior for duration seconds.</summary>
        public void Lock(float duration, LockMode mode = LockMode.Hard) {
            throw new NotImplementedException();
        }

        /// <summary>Manually release the lock (emergency only; auto-release is preferred).</summary>
        public void Unlock() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read the **effective** (post-Influence-cascade per Q-S23)
        /// value of the named Need. Returns 0.0 for unknown needs after
        /// a Warning. Read-only debug API (v0.1.5; semantics pinned to
        /// effective by Q-S54); not for the hot path. Hot-path code
        /// should use the cached EffectiveNeeds buffer (spec §16.4).
        /// </summary>
        public float GetNeed(string need) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// (v0.1.5, Q-S54) Read the **base** (pre-cascade) value of the
        /// named Need. Companion to GetNeed for inspector tools that
        /// display both layers. Returns 0.0 for unknown needs after a
        /// Warning. Read-only debug API; not for the hot path.
        /// </summary>
        public float GetBaseNeed(string need) {
            throw new NotImplementedException();
        }

        // v0.1.5 (Q-S32): internal debug accessors for Animo.Tools
        // (ScenarioRunner). Pre-Q-S32 §26.3 declared `TraceFrame` with
        // `effective_needs`, `action_scores` Dictionaries, but Engine's
        // public API only exposed `GetNeed(string)` — there was no way
        // for ScenarioRunner to populate the trace. These accessors are
        // `internal` (visible to Animo.Tools via InternalsVisibleTo)
        // and explicitly NOT for the hot path; they allocate or copy.
        // The hot path inside Engine uses direct float[] index access.

        /// <summary>(Q-S32) Read Effective Need value — for ScenarioRunner / debug.</summary>
        internal float GetEffectiveNeed(string need) {
            throw new NotImplementedException();
        }

        /// <summary>(Q-S32) Read computed Action score — for ScenarioRunner / debug.</summary>
        internal float GetActionScore(string action_id) {
            throw new NotImplementedException();
        }

        /// <summary>(Q-S32) Snapshot all Need names (incl. non-standard) — for ScenarioRunner.</summary>
        internal IReadOnlyList<string> GetAllNeedNames() {
            throw new NotImplementedException();
        }

        /// <summary>(Q-S32) Snapshot all Action ids — for ScenarioRunner.</summary>
        internal IReadOnlyList<string> GetAllActionIds() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// (Q-S44) Cold-path accessor: returns the `expanded_action_change`
        /// string for the given Action id (i.e. `binding.on_action_change`
        /// template expanded with this Engine's runtime-unique agent_id).
        /// Used by `Agent.Awake` step (6) to push the initial behavior to
        /// the host's Animator through the SAME template format the Bus
        /// path uses — keeps the host's state-name namespace consistent
        /// between frame 1 (silent first transition, Q-S31) and all later
        /// frames (Bus-routed via Q-S26).
        /// Falls back to `behavior` (raw id) if the template was not
        /// configured in `binding.on_action_change`.
        /// Internal access — visible to Animo.Tools and Animo.Agent
        /// callers via InternalsVisibleTo (Q-S32 + Q-S44).
        /// </summary>
        internal string GetExpandedActionTrigger(string behavior) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// (Q-S45 + Q-S48) Hook for non-tier `NeedMeta` fields applied
        /// during Engine ctor PHASE C (§3.5.2). Called for both standard
        /// and non-standard Needs — only the tier-assignment side of
        /// PHASE C skips standard Needs. v0.1.5 has no non-tier NeedMeta
        /// fields, so this method is a no-op (Phase 3 implements as an
        /// empty body); v0.2 / v0.3 may add fields like
        /// `decay_multiplier` or `label` and implement them here.
        ///
        /// Q-S48 fix: pre-Q-S48 the Q-S45 narrow-skip code in §3.5.2
        /// PHASE C called this method but no declaration existed in
        /// Engine.cs — confirmed compile error. This declaration closes
        /// the spec-vs-code gap so the Q-S45 path is buildable.
        /// </summary>
        /// <param name="need_index">Resolved index (per Q-S37 PHASE B)</param>
        /// <param name="meta">The NeedMeta entry (tier-only in v0.1.5)</param>
        private void ApplyNonTierMetadata(int need_index, NeedMeta meta) {
            // v0.1.5: no-op. NeedMeta currently carries only `tier`,
            // which PHASE C handles directly. Future fields apply here.
        }

        // v0.1.5 (Q-S26): protected raise helper for subclass test
        // harnesses; Engine implementation in Phase 3 invokes this from
        // Step 3 (Threshold fire) and Step 4 / Step 5 (behavior change).
        // Helper exists so tests with derived stubs can simulate fires
        // without re-implementing the whole 5-step loop.
        protected void RaiseSignal(string signal_id) {
            OnSignal?.Invoke(obj: signal_id);
        }
    }
}
