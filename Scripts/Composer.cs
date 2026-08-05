// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using Animo.Model;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
            foreach (var kind_id in resolveKindIds(persona, root)) {
                var kind = findKind(root, kind_id);
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
            topologicalSortInfluences(composed);

            return composed;
        }

        ///////////////////////////////////////////////////////////////////////
        // Kind-id resolution

        // (Q-S47) Compound-key match with EPSILON on trigger_threshold.
        internal static bool ThresholdsMatch(Threshold first, Threshold second) =>
            first.need == second.need &&
            Math.Abs(first.trigger_threshold - second.trigger_threshold) < THRESHOLD_KEY_EPSILON;

        static IEnumerable<string> resolveKindIds(Persona persona, Root root) {
            if (persona.kind_ids == null || persona.kind_ids.Count == 0)
                yield break;
            // (Q-S33) Deduplicate kind_ids: last-wins — keep last occurrence.
            var seen  = new HashSet<string>();
            var deduplicated = new List<string>();
            for (int i = persona.kind_ids.Count - 1; i >= 0; i--) {
                if (seen.Add(persona.kind_ids[i])) deduplicated.Add(persona.kind_ids[i]);
            }
            deduplicated.Reverse();
            foreach (var k in deduplicated) yield return k;
        }

        static Kind? findKind(Root root, string kind_id) {
            foreach (var k in root.kinds) if (k.kind_id == kind_id) return k;
            return null;
        }

        ///////////////////////////////////////////////////////////////////////
        // Kind merge (builds up the base; persona overrides come after)

        static void mergeKind(Persona composed, Kind kind) {
            if (kind.rates       != null) mergeRates(composed, kind.rates);
            if (kind.suppression != null) mergeSuppression(composed, kind.suppression);
            if (kind.influences  != null) mergeInfluences(composed, kind.influences);
            if (kind.actions     != null) mergeActions(composed, kind.actions);
            if (kind.commitment  != null) mergeCommitment(composed, kind.commitment);
            if (kind.binding     != null) mergeBinding(composed, kind.binding);
            if (kind.needs_meta  != null) mergeNeedsMeta(composed, kind.needs_meta);
        }

        ///////////////////////////////////////////////////////////////////////
        // Persona own-field merge (persona wins)

        static void mergePersonaOwn(Persona composed, Persona source) {
            if (source.needs      != null) mergeNeeds(composed, source.needs);
            if (source.rates      != null) mergeRates(composed, source.rates);
            if (source.suppression!= null) mergeSuppression(composed, source.suppression);

            // (Q-S19/Q-S20) Persona-first ordering: persona entries lead,
            // then Kind entries not already present by key.
            if (source.influences != null) mergeInfluencesPersonaFirst(composed, source.influences);
            if (source.actions    != null) mergeActionsPersonaFirst(composed, source.actions);

            if (source.commitment != null) mergeCommitment(composed, source.commitment);
            if (source.binding    != null) mergeBinding(composed, source.binding);
            if (source.needs_meta != null) mergeNeedsMeta(composed, source.needs_meta);
        }

        ///////////////////////////////////////////////////////////////////////
        // Field-level merges

        static void mergeNeeds(Persona composed, Needs source) {
            if (composed.needs == null) composed.needs = new Needs();
            foreach (var entry in source.values) composed.needs.values[entry.Key] = entry.Value;
        }

        static void mergeRates(Persona composed, Rates source) {
            if (composed.rates == null) composed.rates = new Rates();
            foreach (var entry in source.values) composed.rates.values[entry.Key] = entry.Value;
        }

        static void mergeSuppression(Persona composed, Suppression source) {
            if (composed.suppression == null) composed.suppression = new Suppression();
            // §8.3 deep-merge per field (last-wins per field, not whole-object replace).
            if (source.tier2 != 0f) composed.suppression.tier2 = source.tier2;
            if (source.tier3 != 0f) composed.suppression.tier3 = source.tier3;
            if (source.tier4 != 0f) composed.suppression.tier4 = source.tier4;
            if (source.tier5 != 0f) composed.suppression.tier5 = source.tier5;
        }

        // Kind-cascade: simple last-wins by (source, target) key.
        static void mergeInfluences(Persona composed, List<Influence> source) {
            if (composed.influences == null) composed.influences = new List<Influence>();
            foreach (var influence in source) {
                bool found = false;
                for (int i = 0; i < composed.influences.Count; i++) {
                    if (composed.influences[i].source == influence.source &&
                        composed.influences[i].target == influence.target) {
                        composed.influences[i] = influence.DeepCopy(); found = true; break;
                    }
                }
                if (!found) composed.influences.Add(influence.DeepCopy());
            }
        }

        // (Q-S20) Persona-first: persona entries first, then unmatched Kind entries.
        static void mergeInfluencesPersonaFirst(Persona composed, List<Influence> persona_influences) {
            var kind_influences = composed.influences ?? new List<Influence>();
            var result    = new List<Influence>();
            foreach (var influence in persona_influences) result.Add(influence.DeepCopy());
            foreach (var k in kind_influences) {
                bool is_duplicate = false;
                foreach (var existing in result) {
                    if (existing.source == k.source && existing.target == k.target) { is_duplicate = true; break; }
                }
                if (!is_duplicate) result.Add(k.DeepCopy());
            }
            composed.influences = result;
        }

        // Kind-cascade: simple last-wins by id key.
        static void mergeActions(Persona composed, List<Animo.Model.Action> source) {
            if (composed.actions == null) composed.actions = new List<Animo.Model.Action>();
            foreach (var act in source) {
                bool found = false;
                for (int i = 0; i < composed.actions.Count; i++) {
                    if (composed.actions[i].id == act.id) {
                        composed.actions[i] = act.DeepCopy(); found = true; break;
                    }
                }
                if (!found) composed.actions.Add(act.DeepCopy());
            }
        }

        // (Q-S19) Persona-first + (Q-S61) additive-only (never removes Kind actions).
        static void mergeActionsPersonaFirst(Persona composed, List<Animo.Model.Action> persona_acts) {
            var kind_acts = composed.actions ?? new List<Animo.Model.Action>();
            var result    = new List<Animo.Model.Action>();
            foreach (var action in persona_acts) result.Add(action.DeepCopy());
            foreach (var k in kind_acts) {
                bool is_duplicate = false;
                foreach (var existing in result) { if (existing.id == k.id) { is_duplicate = true; break; } }
                if (!is_duplicate) result.Add(k.DeepCopy());
            }
            composed.actions = result;
        }

        static void mergeCommitment(Persona composed, Commitment source) {
            if (composed.commitment == null) composed.commitment = new Commitment();
            // last-wins per field
            if (source.bonus != 0f) composed.commitment.bonus = source.bonus;
        }

        static void mergeBinding(Persona composed, Binding source) {
            if (composed.binding == null) composed.binding = new Binding();
            if (source.on_action_change != null)
                composed.binding.on_action_change = source.on_action_change;
            mergeThresholds(composed.binding.thresholds, source.thresholds);
        }

        // (Q-S14 + Q-S43 + Q-S47 + Q-S85) first-occurrence-wins, EPSILON compound key.
        static void mergeThresholds(List<Threshold> merged, List<Threshold> incoming) {
            foreach (var threshold in incoming) {
                int found = -1;
                for (int i = 0; i < merged.Count; i++) {
                    if (ThresholdsMatch(merged[i], threshold)) { found = i; break; }
                }
                if (found >= 0) merged[found] = threshold.DeepCopy();
                else            merged.Add(threshold.DeepCopy());
            }
        }

        static void mergeNeedsMeta(Persona composed, Dictionary<string, NeedMeta> source) {
            if (composed.needs_meta == null) composed.needs_meta = new Dictionary<string, NeedMeta>();
            foreach (var entry in source) composed.needs_meta[entry.Key] = entry.Value.DeepCopy();
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
        static void topologicalSortInfluences(Persona composed) {
            var influences = composed.influences;
            if (influences == null || influences.Count == 0) {
                composed.sorted_influence_order = System.Array.Empty<int>();
                return;
            }
            int n = influences.Count;
            var in_degree = new int[n];
            var adjacency    = new System.Collections.Generic.List<int>[n];
            for (int i = 0; i < n; i++) adjacency[i] = new System.Collections.Generic.List<int>();

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (i != j && influences[i].target == influences[j].source) {
                        adjacency[i].Add(j); in_degree[j]++;
                    }

            var queue  = new System.Collections.Generic.List<int>();
            for (int i = 0; i < n; i++) if (in_degree[i] == 0) queue.Add(i);
            var sorted = new int[n];
            int index    = 0;
            while (queue.Count > 0) {
                int min_index = 0;
                for (int k = 1; k < queue.Count; k++) if (queue[k] < queue[min_index]) min_index = k;
                int vertex = queue[min_index]; queue.RemoveAt(min_index); sorted[index++] = vertex;
                foreach (var neighbor in adjacency[vertex]) if (--in_degree[neighbor] == 0) queue.Add(neighbor);
            }
            // (§8: cycle safety) If index < n, a cycle exists (A025 catches at validate time).
            // Resize to index entries so Engine never iterates 0-filled trailing slots.
            if (index < n) {
                var trimmed = new int[index];
                System.Array.Copy(sorted, trimmed, index);
                composed.sorted_influence_order = trimmed;
            } else {
                composed.sorted_influence_order = sorted;
            }
        }

        static void fillResetThresholds(Persona composed) {
            if (composed.binding == null) return;
            foreach (var threshold in composed.binding.thresholds)
                if (threshold.reset_threshold == null)
                    threshold.reset_threshold = Math.Max(0f, threshold.trigger_threshold - 5f);
        }

        // (Q-S7) Fill 0.0f for every Need key referenced but not yet in needs.values.
        static void fillMissingNeeds(Persona composed) {
            if (composed.needs == null) composed.needs = new Needs();
            var referenced = new HashSet<string>();
            if (composed.actions    != null) foreach (var action   in composed.actions)     referenced.Add(action.need);
            if (composed.rates      != null) foreach (var entry  in composed.rates.values) referenced.Add(entry.Key);
            if (composed.influences != null) foreach (var influence in composed.influences) {
                referenced.Add(influence.source); referenced.Add(influence.target);
            }
            // (#10) Q-S7 guarantees composed.binding is non-null at this point —
            // Compose() materializes an empty Binding{thresholds=new List<>()} when
            // input had none. No null guard needed.
            foreach (var threshold in composed.binding!.thresholds) referenced.Add(threshold.need);
            foreach (var need in referenced)
                if (!composed.needs.values.ContainsKey(need)) composed.needs.values[need] = 0.0f;
        }
    }
}
