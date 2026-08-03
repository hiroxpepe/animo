// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using Animo.Model;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Animo.Model;

[assembly: InternalsVisibleTo("Animo.Tests.EditMode")]

namespace Animo.Core {
    /// <summary>
    /// Composes a final Persona by deep-copying the kind chain and applying
    /// persona-level overrides. See spec §10.
    ///
    /// (v0.1.5, Q-S85) When merging binding.thresholds[], uses
    /// first-occurrence-wins semantics with EPSILON-tolerant compound key.
    /// (v0.1.5, Q-S11 + Q-S86 contract) Fills every reset_threshold.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    internal static class Composer {

        // (Q-S47) 0.01f covers IEEE-754 round-trip drift (~1e-7) at [0,100] scale.
        internal const float THRESHOLD_KEY_EPSILON = 0.01f;

        internal static Persona Compose(Persona persona, Root root) {
            var composed = new Persona {
                agent_id     = persona.agent_id,
                persona_name = persona.persona_name,
                kind_ids     = persona.kind_ids != null ? new List<string>(persona.kind_ids) : null
            };

            // §10 steps 2-3: merge Kinds in cascade order.
            foreach (var kid in resolveKindIds(persona, root)) {
                var kind = findKind(root, kid);
                if (kind != null) mergeKind(composed, kind);
            }

            // §10 step 4: persona's own fields win (applied last).
            mergePersonaOwn(composed, persona);

            // (Q-S7) Ensure binding is always non-null after Compose.
            if (composed.binding == null) {
                composed.binding = new Binding {
                    on_action_change = Const.DEFAULT_ON_ACTION_CHANGE
                };
            }

            // (Q-S11 + Q-S86) Fill reset_threshold.
            fillResetThresholds(composed);

            // (Q-S7) Fill missing referenced Need keys with 0.0f.
            fillMissingNeeds(composed);

            // (§16.3.4 Pre-cache Principle) Topo-sort influences[] once at compose time
            // so Engine Step 2 can iterate a pre-ordered int[] with zero allocation.
            topoSortInfluences(composed);

            return composed;
        }

        ///////////////////////////////////////////////////////////////////////
        // Kind-id resolution

        // (Q-S47) Compound-key match with EPSILON on trigger_threshold.
        internal static bool ThresholdsMatch(Threshold a, Threshold b) =>
            a.need == b.need &&
            Math.Abs(a.trigger_threshold - b.trigger_threshold) < THRESHOLD_KEY_EPSILON;

        static IEnumerable<string> resolveKindIds(Persona persona, Root root) {
            if (persona.kind_ids == null || persona.kind_ids.Count == 0)
                yield break;
            // (Q-S33) Deduplicate kind_ids: last-wins — keep last occurrence.
            var seen  = new HashSet<string>();
            var dedup = new List<string>();
            for (int i = persona.kind_ids.Count - 1; i >= 0; i--) {
                if (seen.Add(persona.kind_ids[i])) dedup.Add(persona.kind_ids[i]);
            }
            dedup.Reverse();
            foreach (var k in dedup) yield return k;
        }

        static Kind? findKind(Root root, string kind_id) {
            foreach (var k in root.kinds) if (k.kind_id == kind_id) return k;
            return null;
        }

        ///////////////////////////////////////////////////////////////////////
        // Kind merge (builds up the base; persona overrides come after)

        static void mergeKind(Persona c, Kind kind) {
            if (kind.rates       != null) mergeRates(c, kind.rates);
            if (kind.suppression != null) mergeSuppression(c, kind.suppression);
            if (kind.influences  != null) mergeInfluences(c, kind.influences);
            if (kind.actions     != null) mergeActions(c, kind.actions);
            if (kind.commitment  != null) mergeCommitment(c, kind.commitment);
            if (kind.binding     != null) mergeBinding(c, kind.binding);
            if (kind.needs_meta  != null) mergeNeedsMeta(c, kind.needs_meta);
        }

        ///////////////////////////////////////////////////////////////////////
        // Persona own-field merge (persona wins)

        static void mergePersonaOwn(Persona c, Persona p) {
            if (p.needs      != null) mergeNeeds(c, p.needs);
            if (p.rates      != null) mergeRates(c, p.rates);
            if (p.suppression!= null) mergeSuppression(c, p.suppression);

            // (Q-S19/Q-S20) Persona-first ordering: persona entries lead,
            // then Kind entries not already present by key.
            if (p.influences != null) mergeInfluencesPersonaFirst(c, p.influences);
            if (p.actions    != null) mergeActionsPersonaFirst(c, p.actions);

            if (p.commitment != null) mergeCommitment(c, p.commitment);
            if (p.binding    != null) mergeBinding(c, p.binding);
            if (p.needs_meta != null) mergeNeedsMeta(c, p.needs_meta);
        }

        ///////////////////////////////////////////////////////////////////////
        // Field-level merges

        static void mergeNeeds(Persona c, Needs src) {
            if (c.needs == null) c.needs = new Needs();
            foreach (var kv in src.values) c.needs.values[kv.Key] = kv.Value;
        }

        static void mergeRates(Persona c, Rates src) {
            if (c.rates == null) c.rates = new Rates();
            foreach (var kv in src.values) c.rates.values[kv.Key] = kv.Value;
        }

        static void mergeSuppression(Persona c, Suppression src) {
            if (c.suppression == null) c.suppression = new Suppression();
            // §8.3 deep-merge per field (last-wins per field, not whole-object replace).
            if (src.tier2 != 0f) c.suppression.tier2 = src.tier2;
            if (src.tier3 != 0f) c.suppression.tier3 = src.tier3;
            if (src.tier4 != 0f) c.suppression.tier4 = src.tier4;
            if (src.tier5 != 0f) c.suppression.tier5 = src.tier5;
        }

        // Kind-cascade: simple last-wins by (source, target) key.
        static void mergeInfluences(Persona c, List<Influence> src) {
            if (c.influences == null) c.influences = new List<Influence>();
            foreach (var inf in src) {
                bool found = false;
                for (int i = 0; i < c.influences.Count; i++) {
                    if (c.influences[i].source == inf.source &&
                        c.influences[i].target == inf.target) {
                        c.influences[i] = inf.DeepCopy(); found = true; break;
                    }
                }
                if (!found) c.influences.Add(inf.DeepCopy());
            }
        }

        // (Q-S20) Persona-first: persona entries first, then unmatched Kind entries.
        static void mergeInfluencesPersonaFirst(Persona c, List<Influence> persona_infs) {
            var kind_infs = c.influences ?? new List<Influence>();
            var result    = new List<Influence>();
            foreach (var p in persona_infs) result.Add(p.DeepCopy());
            foreach (var k in kind_infs) {
                bool dup = false;
                foreach (var r in result) {
                    if (r.source == k.source && r.target == k.target) { dup = true; break; }
                }
                if (!dup) result.Add(k.DeepCopy());
            }
            c.influences = result;
        }

        // Kind-cascade: simple last-wins by id key.
        static void mergeActions(Persona c, List<Animo.Model.Action> src) {
            if (c.actions == null) c.actions = new List<Animo.Model.Action>();
            foreach (var act in src) {
                bool found = false;
                for (int i = 0; i < c.actions.Count; i++) {
                    if (c.actions[i].id == act.id) {
                        c.actions[i] = act.DeepCopy(); found = true; break;
                    }
                }
                if (!found) c.actions.Add(act.DeepCopy());
            }
        }

        // (Q-S19) Persona-first + (Q-S61) additive-only (never removes Kind actions).
        static void mergeActionsPersonaFirst(Persona c, List<Animo.Model.Action> persona_acts) {
            var kind_acts = c.actions ?? new List<Animo.Model.Action>();
            var result    = new List<Animo.Model.Action>();
            foreach (var p in persona_acts) result.Add(p.DeepCopy());
            foreach (var k in kind_acts) {
                bool dup = false;
                foreach (var r in result) { if (r.id == k.id) { dup = true; break; } }
                if (!dup) result.Add(k.DeepCopy());
            }
            c.actions = result;
        }

        static void mergeCommitment(Persona c, Commitment src) {
            if (c.commitment == null) c.commitment = new Commitment();
            // last-wins per field
            if (src.bonus != 0f) c.commitment.bonus = src.bonus;
        }

        static void mergeBinding(Persona c, Binding src) {
            if (c.binding == null) c.binding = new Binding();
            if (src.on_action_change != null)
                c.binding.on_action_change = src.on_action_change;
            mergeThresholds(c.binding.thresholds, src.thresholds);
        }

        // (Q-S14 + Q-S43 + Q-S47 + Q-S85) first-occurrence-wins, EPSILON compound key.
        static void mergeThresholds(List<Threshold> merged, List<Threshold> incoming) {
            foreach (var t in incoming) {
                int found = -1;
                for (int i = 0; i < merged.Count; i++) {
                    if (ThresholdsMatch(merged[i], t)) { found = i; break; }
                }
                if (found >= 0) merged[found] = t.DeepCopy();
                else            merged.Add(t.DeepCopy());
            }
        }

        static void mergeNeedsMeta(Persona c, Dictionary<string, NeedMeta> src) {
            if (c.needs_meta == null) c.needs_meta = new Dictionary<string, NeedMeta>();
            foreach (var kv in src) c.needs_meta[kv.Key] = kv.Value.DeepCopy();
        }

        ///////////////////////////////////////////////////////////////////////
        // Post-merge fill passes

        // (Q-S11 + Q-S86) Every Threshold must have a numeric reset_threshold.
        /// <summary>
        /// (§16.3.4 Pre-cache Principle) Stable Kahn's topo-sort of influences[]
        /// at compose time (cold path). Stores sorted order in
        /// Persona.sorted_influence_order int[]. Engine Step 2 iterates this
        /// array with zero allocation per frame.
        /// If a cycle is detected (A025), order is left unsorted (Engine Step 2
        /// skips the cyclic portion; A025 reports the Error at validate time).
        /// </summary>
        static void topoSortInfluences(Persona c) {
            var infs = c.influences;
            if (infs == null || infs.Count == 0) {
                c.sorted_influence_order = System.Array.Empty<int>();
                return;
            }
            int n = infs.Count;
            var in_deg = new int[n];
            var adj    = new System.Collections.Generic.List<int>[n];
            for (int i = 0; i < n; i++) adj[i] = new System.Collections.Generic.List<int>();

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (i != j && infs[i].target == infs[j].source) {
                        adj[i].Add(j); in_deg[j]++;
                    }

            var queue  = new System.Collections.Generic.List<int>();
            for (int i = 0; i < n; i++) if (in_deg[i] == 0) queue.Add(i);
            var sorted = new int[n];
            int idx    = 0;
            while (queue.Count > 0) {
                int vi = 0;
                for (int k = 1; k < queue.Count; k++) if (queue[k] < queue[vi]) vi = k;
                int v = queue[vi]; queue.RemoveAt(vi); sorted[idx++] = v;
                foreach (var w in adj[v]) if (--in_deg[w] == 0) queue.Add(w);
            }
            // (§8: cycle safety) If idx < n, a cycle exists (A025 catches at validate time).
            // Resize to idx entries so Engine never iterates 0-filled trailing slots.
            if (idx < n) {
                var trimmed = new int[idx];
                System.Array.Copy(sorted, trimmed, idx);
                c.sorted_influence_order = trimmed;
            } else {
                c.sorted_influence_order = sorted;
            }
        }

        static void fillResetThresholds(Persona c) {
            if (c.binding == null) return;
            foreach (var t in c.binding.thresholds)
                if (t.reset_threshold == null)
                    t.reset_threshold = Math.Max(0f, t.trigger_threshold - 5f);
        }

        // (Q-S7) Fill 0.0f for every Need key referenced but not yet in needs.values.
        static void fillMissingNeeds(Persona c) {
            if (c.needs == null) c.needs = new Needs();
            var referenced = new HashSet<string>();
            if (c.actions    != null) foreach (var a   in c.actions)     referenced.Add(a.need);
            if (c.rates      != null) foreach (var kv  in c.rates.values) referenced.Add(kv.Key);
            if (c.influences != null) foreach (var inf in c.influences) {
                referenced.Add(inf.source); referenced.Add(inf.target);
            }
            // (#10) Q-S7 guarantees composed.binding is non-null at this point —
            // Compose() materializes an empty Binding{thresholds=new List<>()} when
            // input had none. No null guard needed.
            foreach (var t in c.binding!.thresholds) referenced.Add(t.need);
            foreach (var need in referenced)
                if (!c.needs.values.ContainsKey(need)) c.needs.values[need] = 0.0f;
        }
    }
}
