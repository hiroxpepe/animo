// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Animo.Model {

    /// <summary>JSON root: schema_version + kinds + personas.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [Serializable]
    public class Root {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public string schema_version { get; set; } = "";
        public List<Kind> kinds { get; set; } = new();
        public List<Persona> personas { get; set; } = new();
    }

    /// <summary>Type definition. Cascades into Personas via kind_ids.</summary>
    [Serializable]
    public class Kind {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public string kind_id { get; set; } = "";
        public Rates? rates { get; set; }
        public Suppression? suppression { get; set; }
        public List<Influence>? influences { get; set; }
        public List<Action>? actions { get; set; }
        public Commitment? commitment { get; set; }
        public Binding? binding { get; set; }
        // v0.1.5 (Q-S30): optional per-Need metadata. Currently the only
        // populated field is `tier` for genre-custom Needs joining
        // Maslow suppression. Keyed by Need name.
        public Dictionary<string, NeedMeta>? needs_meta { get; set; }
    }

    /// <summary>Individual agent definition. Inherits via kind_ids.</summary>
    [Serializable]
    public class Persona {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public string agent_id { get; set; } = "";
        public string? persona_name { get; set; }
        public List<string>? kind_ids { get; set; }
        public Needs? needs { get; set; }
        public Rates? rates { get; set; }
        public Suppression? suppression { get; set; }
        public List<Influence>? influences { get; set; }
        public List<Action>? actions { get; set; }
        public Commitment? commitment { get; set; }
        public Binding? binding { get; set; }
        public Dictionary<string, NeedMeta>? needs_meta { get; set; }

        /// <summary>
        /// (§16.3.4 Pre-cache Principle) Topo-sorted edge order produced by
        /// Composer.topologicalSortInfluences (cold path, once per composed Persona).
        /// Engine Step 2 iterates this int[] with zero allocation per frame.
        /// null = not yet composed (direct construction); Engine falls back to
        /// declaration order (safe, possibly non-topo for cyclic graphs).
        /// </summary>
        [JsonIgnore]
        public int[]? sorted_influence_order { get; set; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// (v0.1.5, Q-S64) Deep-clone the composed Persona so each
        /// `Animo.Agent` (MonoBehaviour) holds its own mutable copy.
        /// Pre-Q-S64 the spec sample code `_composed_persona =
        /// template.DeepCopy()` referenced an undeclared method —
        /// confirmed compile error. The PersonaCache returns a shared
        /// composed template (§11.6); without DeepCopy, two Agents
        /// spawned from the same template id would share `Needs`,
        /// `actions[]`, `binding.thresholds[].expanded_trigger`, etc.,
        /// and one Agent's runtime mutation (e.g. Q-S28's agent_id
        /// override) would corrupt every sibling. DeepCopy is the
        /// per-Agent isolation barrier.
        ///
        /// Phase 3 implements deep copy of all reference-type fields:
        /// Needs.values, Rates.values, Suppression (already value-typed
        /// fields), each Influence in influences[], each Action in
        /// actions[] (including action-internal collections), Commitment,
        /// Binding (including binding.thresholds[] with their
        /// expanded_trigger strings), needs_meta dictionary entries.
        /// Stub returns NotImplementedException; Red baseline test
        /// `PersonaDeepCopyIsolationTests` asserts isolation.
        /// </summary>
        public Persona DeepCopy() {
            // (v0.1.5, Q-S64 + Q-S141) Deep copy all reference-type fields.
            // PersonaCache returns a shared composed template; each Agent.Awake
            // must work on its own isolated copy so one Agent's runtime
            // mutations (e.g. Q-S28 agent_id override) cannot corrupt siblings.
            var copy = new Persona {
                agent_id     = this.agent_id,
                persona_name = this.persona_name,
                kind_ids     = this.kind_ids != null ? new List<string>(this.kind_ids) : null,
            };

            // Needs: copy the values dictionary
            if (this.needs != null) {
                copy.needs = new Needs();
                foreach (var entry in this.needs.values) copy.needs.values[entry.Key] = entry.Value;
            }

            // Rates: copy the values dictionary
            if (this.rates != null) {
                copy.rates = new Rates();
                foreach (var entry in this.rates.values) copy.rates.values[entry.Key] = entry.Value;
            }

            // Suppression: struct-like, just field-copy
            if (this.suppression != null) {
                copy.suppression = new Suppression {
                    tier2 = this.suppression.tier2,
                    tier3 = this.suppression.tier3,
                    tier4 = this.suppression.tier4,
                    tier5 = this.suppression.tier5
                };
            }

            // Influences: deep copy each element (Q-S141)
            if (this.influences != null) {
                copy.influences = new List<Influence>(capacity: this.influences.Count);
                foreach (var influence in this.influences) copy.influences.Add(influence.DeepCopy());
            }

            // Actions: deep copy each element (Q-S141)
            if (this.actions != null) {
                copy.actions = new List<Action>(capacity: this.actions.Count);
                foreach (var act in this.actions) copy.actions.Add(act.DeepCopy());
            }

            // Commitment: deep copy (Q-S141)
            if (this.commitment != null) copy.commitment = this.commitment.DeepCopy();

            // Binding: copy on_action_change + deep copy each Threshold (Q-S141)
            if (this.binding != null) {
                copy.binding = new Binding { on_action_change = this.binding.on_action_change };
                foreach (var threshold in this.binding.thresholds) copy.binding.thresholds.Add(threshold.DeepCopy());
            }

            // needs_meta: deep copy each NeedMeta entry (Q-S134 + Q-S141)
            if (this.needs_meta != null) {
                copy.needs_meta = new Dictionary<string, NeedMeta>(capacity: this.needs_meta.Count);
                foreach (var entry in this.needs_meta) copy.needs_meta[entry.Key] = entry.Value.DeepCopy();
            }
            // (§16.3.4) Copy pre-sorted order array (int[] is immutable after Compose; safe to share).
            copy.sorted_influence_order = this.sorted_influence_order;

            return copy;
        }
    }

    /// <summary>
    /// v0.1.5 (Q-S30): per-Need metadata. Currently carries only `tier`
    /// for genre-custom Needs (oxygen, thirst, jealousy) that should
    /// participate in Maslow tier suppression — without this metadata
    /// non-standard Needs would be excluded from §9.3.4
    /// `max_lower_tier_intensity` (Q-S16's safe default).
    ///
    /// `tier` ∈ [1, 5] — Validator A038 enforces the range.
    /// </summary>
    [Serializable]
    public class NeedMeta {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public int   tier            { get; set; }
        /// <summary>
        /// (v0.1.5, Q-S48) Per-Need decay rate multiplier applied in Engine
        /// PHASE C via applyNonTierMetadata. 1.0 = no change (default).
        /// Values &lt; 1.0 slow decay; values &gt; 1.0 accelerate decay.
        /// Used to give different Needs different "drain speeds" without
        /// requiring explicit rates[] overrides in every Persona.
        /// Phase 3 Engine.applyNonTierMetadata multiplies the base rates[]
        /// value by this factor before storing it in a per-Need rate cache.
        /// </summary>
        public float decay_multiplier { get; set; } = 1.0f;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// (v0.1.5, Q-S56) Per-Need default NeedMeta. Used by Engine ctor
        /// PHASE C "Step 3" when iterating ALL composed Needs and the
        /// author has not declared an explicit `needs_meta` entry for
        /// this Need. v0.1.5 returns tier per §3.5 for standard Needs
        /// or a sentinel `0` for non-standard Needs (which always have
        /// an explicit needs_meta if they reach Engine ctor — A019 +
        /// A038 enforce). v0.2 / v0.3 adds per-Need defaults for
        /// future fields (decay_multiplier, etc.).
        /// </summary>
        public static NeedMeta DefaultFor(string need_name) {
            // Standard Needs: tier per §3.5 (lookup via Const).
            // Non-standard reaching here without explicit meta is
            // a Validator-prevented contradiction; sentinel 0 is
            // safe-by-construction.
            int tier = 0;
            if (Animo.Const.NEED_TIER_BY_NAME.TryGetValue(need_name, out var threshold)) {
                tier = threshold;
            }
            return new NeedMeta { tier = tier };
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// (v0.1.5, Q-S134) Shallow-safe copy of this NeedMeta.
        /// v0.1.5 carries only `tier` (value type int), so a field-by-field
        /// copy is identical to a deep copy. Declared here as an explicit
        /// contract so that v0.2 / v0.3 NeedMeta field additions (e.g.
        /// `decay_multiplier`, `label`) are automatically caught by the
        /// compiler: the implementer must add the new field to this method
        /// or the build fails. Pre-Q-S134 `Persona.DeepCopy()` would have
        /// silently omitted future NeedMeta fields — a copy-leakage bug
        /// guaranteed on first NeedMeta extension. Phase 3 implements
        /// `Persona.DeepCopy()` by calling `meta.DeepCopy()` per entry.
        /// </summary>
        public NeedMeta DeepCopy() {
            // v0.1.5: NeedMeta carries only `tier` (value type).
            // Add every new field introduced in future versions here.
            return new NeedMeta { tier = this.tier, decay_multiplier = this.decay_multiplier };
        }
    }

    /// <summary>Need value set [0, 100]. Float dictionary backed.</summary>
    ///
    /// (v0.1.5, Q-S151) JSON deserialization contract for Phase 3:
    /// The JSON shape for `needs` / `rates` is a FLAT object —
    /// <c>{"hunger": 40, "fatigue": 20}</c> — not a wrapper object
    /// <c>{"values": {"hunger": 40}}</c>. Newtonsoft.Json's default
    /// <c>DeserializeObject&lt;Needs&gt;</c> looks for properties named
    /// after each JSON key directly on the class, fails to find them,
    /// and silently produces a Needs with <c>values.Count == 0</c> —
    /// every Agent would spawn with no Needs at all. Empirically
    /// verified: <c>JsonConvert.DeserializeObject&lt;Needs&gt;("{\"hunger\":40}").values.Count</c>
    /// returns 0 with the bare Dictionary-backed shape.
    ///
    /// Phase 3 implementation pattern (one of):
    ///   Option A (RECOMMENDED — minimal disruption, preserves `.values`
    ///   convention used in Q-S65 §3.5.2 PHASE A and 8 existing tests):
    ///     - Add private `Dictionary&lt;string, JToken&gt; _raw` annotated
    ///       with <c>[JsonExtensionData]</c>; Newtonsoft routes all
    ///       unmapped top-level properties into it.
    ///     - `values` becomes a read-only projection: foreach entry in _raw,
    ///       parse as float, populate Dictionary&lt;string, float&gt;.
    ///     - Existing call sites (`_persona.needs?.values`) continue to work.
    ///   Option B (deeper change — defer to v0.2 if Option A blocks):
    ///     - Replace the `Needs` class with `Dictionary&lt;string, float&gt;?`
    ///       directly on Persona/Kind. Requires updating Q-S65 spec
    ///       pseudocode, §11.4.1 examples, and 8 test files.
    ///
    /// Q-S151 chooses Option A; the v0.1.5 stub keeps the simple
    /// Dictionary-backed shape because Phase 3 wires up the converter.
    /// The contract is documented here so Phase 3 cannot regress.
    [Serializable]
    public class Needs {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public Dictionary<string, float> values { get; set; } = new();
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public float Get(string need) =>
            values.TryGetValue(need, out var v) ? v : 0f;
        public float Normalized(string need) =>
            values.TryGetValue(need, out var v) ? v / 100f : 0f;
        // (v0.1.5, Q-S63) `Clamp()` removed. Hot path uses flat float[]
        // and Mathf.Clamp directly per §16.2; the instance method was
        // dead code that would only have surfaced as a confusing
        // NotImplementedException for tool authors. Hot-path-zero-alloc
        // (§16.1) leaves the Needs class as a JSON-bridge shape only.
    }

    /// <summary>Need change rate per second. Negative pulls toward 0; positive pushes toward 100.</summary>
    ///
    /// (v0.1.5, Q-S151) Same JSON-bridge contract as Needs above —
    /// JSON shape is FLAT <c>{"hunger": -0.5, "fatigue": -0.3}</c>,
    /// not a wrapper. Phase 3 implements the same [JsonExtensionData]
    /// projection pattern.
    [Serializable]
    public class Rates {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public Dictionary<string, float> values { get; set; } = new();
    }

    /// <summary>Tier suppression factors [0, 1]. Only tier2..tier5 are valid.</summary>
    [Serializable]
    public class Suppression {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public float tier2 { get; set; } = 0f;
        public float tier3 { get; set; } = 0f;
        public float tier4 { get; set; } = 0f;
        public float tier5 { get; set; } = 0f;
    }

    /// <summary>Directed need-to-need effect. Coefficient in [-1, 1].</summary>
    [Serializable]
    public class Influence {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public string source { get; set; } = "";
        public string target { get; set; } = "";
        public float coefficient { get; set; } = 0f;

        /// <summary>
        /// (v0.1.5, §16.3.4 Pre-cache Principle) Baked by Engine ctor PHASE B
        /// from _need_index. Hot-path Step 2 uses these int indices directly
        /// instead of string lookups. -1 = not yet baked.
        /// </summary>
        public int source_index { get; set; } = -1;
        public int target_index { get; set; } = -1;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// (v0.1.5, Q-S141) Q-S134 pattern extended to all reference-type model
        /// classes that are deep-copied inside Persona.DeepCopy(). Influence
        /// carries only value-type and immutable-string fields in v0.1.5, so
        /// a field-by-field copy is currently equivalent to a deep copy.
        /// Declared explicitly so future field additions (e.g. a weight List)
        /// trigger a compiler error here if DeepCopy() is not extended.
        /// Phase 3 implements Persona.DeepCopy() by calling DeepCopy() on each entry.
        /// </summary>
        public Influence DeepCopy() {
            return new Influence {
                source       = this.source,
                target       = this.target,
                coefficient  = this.coefficient,
                source_index = this.source_index,
                target_index = this.target_index
            };
        }
    }

    /// <summary>Action definition. need is required since v0.1.1.</summary>
    [Serializable]
    public class Action {
        // need_index cache is internal in spec; tests use the public API only.
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // internal Fields

        internal int need_index;
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public string id { get; set; } = "";
        public string need { get; set; } = "";
        public int tier { get; set; } = 1;
        public float exponent { get; set; } = 1.0f;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// (v0.1.5, Q-S141) Q-S134 pattern: explicit DeepCopy() so future
        /// fields trigger compiler error at this site. v0.1.5 Action fields
        /// are value types + immutable strings. Commitment sub-object is
        /// value-type only; Persona.DeepCopy() reconstructs it via
        /// Commitment.DeepCopy(). Phase 3 responsibility: include need_index
        /// and any future cached state.
        /// </summary>
        public Action DeepCopy() {
            return new Action {
                id = this.id,
                need = this.need,
                tier = this.tier,
                exponent = this.exponent
                // need_index: Phase 3 re-derives from Engine ctor — not copied.
            };
        }
    }

    /// <summary>Action continuation bonus. v0.1.3 dropped 'decay' field.</summary>
    [Serializable]
    public class Commitment {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public float bonus { get; set; } = 0f;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>(v0.1.5, Q-S141) See Action.DeepCopy() rationale.</summary>
        public Commitment DeepCopy() {
            return new Commitment { bonus = this.bonus };
        }
    }

    /// <summary>Germio integration binding.</summary>
    [Serializable]
    public class Binding {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public string? on_action_change { get; set; }
        // v0.1.5 (Q-S12): non-nullable with empty-list default. Awake-time
        // foreach over `thresholds` is branch-free; null cannot bypass
        // Composer's default-fill (Q-S7) and crash Agent.Awake.
        public List<Threshold> thresholds { get; set; } = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// (v0.1.5, Q-S141) Binding contains a List of Thresholds — explicit
        /// DeepCopy() ensures each Threshold is deep-copied, not shared.
        /// on_action_change is an immutable string (shallow copy is safe).
        /// </summary>
        public Binding DeepCopy() {
            var copy = new Binding { on_action_change = this.on_action_change };
            foreach (var threshold in thresholds) copy.thresholds.Add(item: threshold.DeepCopy());
            return copy;
        }
    }

    /// <summary>Two-stage hysteresis threshold trigger (v0.1.1).</summary>
    [Serializable]
    public class Threshold {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // internal Fields

        internal int need_index;
        // v0.1.5 (Q-S14): per-Threshold pre-expanded trigger string.
        // Replaces the old `_cached_threshold_triggers[threshold.need]` dictionary
        // which collapsed multiple thresholds on the same Need (e.g.
        // fear=50 → "alerted", fear=80 → "panic") into a single overwriting
        // entry. With this field, each Threshold carries its own resolved
        // string and Awake walks the list once without keying by Need.
        internal string expanded_trigger = "";
        // v0.1.5 (Q-S25): hysteresis state. The §12.3.2 state machine has
        // two states (Below / Above) and is the very mechanism that makes
        // `reset_threshold` meaningful — without state, `prev < trigger
        // && curr >= trigger` cross-detection chatters around `trigger`
        // even when `reset_threshold` is set, because the value would
        // never need to drop below `reset_threshold` to re-arm. Engine
        // ctor seeds this from the spawn-time `_effective_needs` (Q-S8
        // + Q-S23 + Q-S25): Persona spawned with the Need already above
        // `trigger_threshold` starts in `Above` and does NOT fire on
        // first Live(delta_time). Step 3 fire branch transitions Below → Above;
        // Step 3 reset branch transitions Above → Below.
        internal bool is_above;
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public string need { get; set; } = "";
        public float trigger_threshold { get; set; } = 0f;
        public float? reset_threshold { get; set; }
        public string trigger { get; set; } = "";

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// (v0.1.5, Q-S141) Threshold carries internal state fields
        /// (need_index, expanded_trigger, is_above) that must be re-derived
        /// by Engine/Composer for each Agent instance — they are NOT copied
        /// as they are instance-specific runtime cache. The public JSON-facing
        /// fields (need, trigger_threshold, reset_threshold, trigger) are copied.
        /// Phase 3 Persona.DeepCopy() must call this and then let the Engine
        /// ctor re-populate the internal fields per-instance.
        /// </summary>
        public Threshold DeepCopy() {
            return new Threshold {
                need = this.need,
                trigger_threshold = this.trigger_threshold,
                reset_threshold = this.reset_threshold,
                trigger = this.trigger
                // need_index, expanded_trigger, is_above: re-derived by Engine ctor.
            };
        }
    }
}
