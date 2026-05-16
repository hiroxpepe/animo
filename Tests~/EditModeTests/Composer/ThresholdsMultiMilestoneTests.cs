// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Decision-table tests for Q-S14: multiple Thresholds on the same Need
    /// must be allowed to coexist after Composer cascade and after Awake
    /// caching. Pre-Q-S14 §8.3 merged thresholds "by need, last-wins" and
    /// §16.5 cached them in `Dictionary<string, string>` keyed by `t.need`,
    /// so `fear=50 → "alerted"` plus `fear=80 → "panic"` collapsed into
    /// whichever came last. Now §8.3 uses the compound key
    /// `(need, trigger_threshold)` and the cache moved to per-Threshold
    /// `internal string expanded_trigger`.
    ///
    /// These tests pin the data-shape contract: post-Compose, both
    /// thresholds exist with their own trigger_threshold values.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ThresholdsMultiMilestoneTests {

        [Test] public void Case01_TwoThresholdsOnSameNeed_DifferentTriggers_BothSurviveCompose() {
            // Pre: persona has two thresholds on "fear" — one at 50 (alerted),
            // one at 80 (panic) — defined together in the same persona.binding.
            // Pre-Q-S14, the by-need compound key would have collapsed them.
            // Post: composed Persona has BOTH thresholds, with their original
            // trigger_threshold values intact.
            Persona p = new Persona {
                agent_id = "a",
                kind_ids = new List<string>(),
                actions  = new List<Animo.Model.Action> { ActionOf(id: "X", need: "fear", tier: 2) },
                needs    = NeedsOf(("fear", 30f)),
                binding  = new Binding {
                    on_action_change = "animo_{agent_id}_{behavior}",
                    thresholds = new List<Threshold> {
                        ThresholdOf(need: "fear", trigger: 50.0f, trigger_event: "animo_{agent_id}_fear_alerted"),
                        ThresholdOf(need: "fear", trigger: 80.0f, trigger_event: "animo_{agent_id}_fear_panic")
                    }
                }
            };
            Root r = new Root { schema_version = "1.5", personas = new List<Persona> { p } };
            Persona composed = Composer.Compose(persona: p, root: r);
            Assert.That(composed.binding, Is.Not.Null);
            Assert.That(composed.binding!.thresholds.Count, Is.EqualTo(expected: 2),
                "Q-S14: both thresholds on same Need (different triggers) must survive Compose");
            float[] triggers = composed.binding!.thresholds.Select(t => t.trigger_threshold).OrderBy(x => x).ToArray();
            Assert.That(triggers[0], Is.EqualTo(expected: 50.0f).Within(0.001f));
            Assert.That(triggers[1], Is.EqualTo(expected: 80.0f).Within(0.001f));
        }

        [Test] public void Case02_KindFearLow_PersonaFearHigh_BothSurviveCascade() {
            // Pre: a Kind defines fear=30 (mild), a Persona inherits the Kind
            // and adds fear=80 (panic). Pre-Q-S14, the by-need merge would
            // have erased the Kind's mild threshold during cascade. Post-Q-S14
            // the compound key (need, trigger) keeps them as siblings.
            Kind k = new Kind {
                kind_id = "scared_creature",
                actions = new List<Animo.Model.Action> { ActionOf(id: "X", need: "fear", tier: 2) },
                binding = new Binding {
                    on_action_change = "animo_{agent_id}_{behavior}",
                    thresholds = new List<Threshold> {
                        ThresholdOf(need: "fear", trigger: 30.0f, trigger_event: "animo_{agent_id}_fear_mild")
                    }
                }
            };
            Persona p = new Persona {
                agent_id = "a",
                kind_ids = new List<string> { "scared_creature" },
                needs    = NeedsOf(("fear", 50f)),
                binding  = new Binding {
                    on_action_change = "animo_{agent_id}_{behavior}",
                    thresholds = new List<Threshold> {
                        ThresholdOf(need: "fear", trigger: 80.0f, trigger_event: "animo_{agent_id}_fear_panic")
                    }
                }
            };
            Root r = new Root {
                schema_version = "1.5",
                kinds          = new List<Kind> { k },
                personas       = new List<Persona> { p }
            };
            Persona composed = Composer.Compose(persona: p, root: r);
            Assert.That(composed.binding!.thresholds.Count, Is.EqualTo(expected: 2),
                "Q-S14: Kind's fear=30 + Persona's fear=80 must coexist in cascade");
            float[] triggers = composed.binding!.thresholds.Select(t => t.trigger_threshold).OrderBy(x => x).ToArray();
            Assert.That(triggers[0], Is.EqualTo(expected: 30.0f).Within(0.001f),
                "Q-S14: Kind's mild threshold must NOT be wiped by Persona's panic threshold (different triggers)");
            Assert.That(triggers[1], Is.EqualTo(expected: 80.0f).Within(0.001f));
        }
    }
}
