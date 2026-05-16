// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Decision-table tests for Q-S11: when `reset_threshold` is OMITTED in
    /// the JSON (null in the deserialized field), Composer fills with
    /// `Math.Max(0.0, trigger_threshold - 5.0)` (spec §7.3, §12.3.4). The
    /// `Math.Max` floor at 0 prevents an unreachable-reset deadlock when
    /// `trigger_threshold &lt; 5.0` — without it, a Threshold could fire
    /// once and stay trapped in `Above` forever (Need clamp [0, 100], spec §9.9).
    ///
    /// EXPLICIT negative values are NOT floored — they are rejected by
    /// Validator A034 (see A034_ResetThresholdNegativeTests). The two rules
    /// are complementary: omit-default safety vs. typed-negative typo
    /// surfacing.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ResetThresholdFloorTests {

        [Test] public void Case01_OmittedResetThreshold_LowTrigger_FilledAtZeroFloor() {
            // Pre: trigger_threshold = 3.0, reset_threshold omitted.
            // Naïve default would be 3.0 - 5.0 = -2.0; Need clamp [0, 100]
            // makes -2.0 unreachable → permanent Above trap.
            // Post: Composer floors to Max(0, -2.0) = 0.0.
            Persona p = new Persona {
                agent_id = "a",
                kind_ids = new List<string>(),
                actions  = new List<Animo.Model.Action> { ActionOf(id: "X", need: "fear", tier: 2) },
                needs    = NeedsOf(("fear", 30f)),
                binding  = new Binding {
                    on_action_change = "animo_{agent_id}_{behavior}",
                    thresholds = new List<Threshold> {
                        ThresholdOf(need: "fear", trigger: 3.0f, trigger_event: "animo_{agent_id}_fear_low", reset: null)
                    }
                }
            };
            Root r = new Root { schema_version = "1.5", personas = new List<Persona> { p } };
            Persona composed = Composer.Compose(persona: p, root: r);
            Assert.That(composed.binding, Is.Not.Null);
            Assert.That(composed.binding!.thresholds, Is.Not.Null);
            Assert.That(composed.binding!.thresholds!.Count, Is.EqualTo(expected: 1));
            Threshold filled = composed.binding!.thresholds![0];
            Assert.That(filled.reset_threshold, Is.EqualTo(expected: 0.0f).Within(0.0001f),
                "Q-S11: omit-default with low trigger must floor at 0.0 (spec §12.3.4)");
        }

        [Test] public void Case02_OmittedResetThreshold_HighTrigger_FilledAtTriggerMinusFive() {
            // Pre: trigger_threshold = 80.0, reset_threshold omitted.
            // Default 80.0 - 5.0 = 75.0 is well above 0 → no floor needed.
            // Post: Composer fills with 75.0 (the floor does not interfere).
            Persona p = new Persona {
                agent_id = "a",
                kind_ids = new List<string>(),
                actions  = new List<Animo.Model.Action> { ActionOf(id: "X", need: "fear", tier: 2) },
                needs    = NeedsOf(("fear", 50f)),
                binding  = new Binding {
                    on_action_change = "animo_{agent_id}_{behavior}",
                    thresholds = new List<Threshold> {
                        ThresholdOf(need: "fear", trigger: 80.0f, trigger_event: "animo_{agent_id}_fear_critical", reset: null)
                    }
                }
            };
            Root r = new Root { schema_version = "1.5", personas = new List<Persona> { p } };
            Persona composed = Composer.Compose(persona: p, root: r);
            Threshold filled = composed.binding!.thresholds![0];
            Assert.That(filled.reset_threshold, Is.EqualTo(expected: 75.0f).Within(0.0001f),
                "Q-S11: omit-default with high trigger must compute trigger - 5.0 (spec §7.3)");
        }
    }
}
