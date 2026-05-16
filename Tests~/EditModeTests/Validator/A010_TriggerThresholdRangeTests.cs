// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using Animo.Tests.EditMode.Helpers;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Decision-table tests for A010 tightened in v0.1.5 Q-S15:
    /// `trigger_threshold` range is now `(0.0, 100.0]` strictly positive.
    /// `trigger_threshold == 0` has no semantic meaning at the Need
    /// `[0, 100]` clamp's lower bound — a 0-trigger fires every frame the
    /// Need stays at 0, regardless of reset_threshold's floor (Q-S11).
    /// Companion measure to A035's post-composition check.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A010_TriggerThresholdRangeTests {

        [Test] public void Case01_TriggerThresholdZero_FailsA010_PostQS15() {
            // Pre: trigger_threshold = 0.0. Pre-Q-S15 this passed A010
            // (range was [0.0, 100.0]). Post-Q-S15 it fails because the
            // range tightened to (0.0, 100.0].
            Root root = new Root {
                schema_version = "1.5",
                personas = new List<Persona> {
                    new Persona {
                        agent_id = "a",
                        actions  = new List<Animo.Model.Action> { ActionOf(id: "X", need: "fear", tier: 2) },
                        needs    = NeedsOf(("fear", 50f)),
                        binding  = new Binding {
                            on_action_change = "animo_{agent_id}_{behavior}",
                            thresholds = new List<Threshold> {
                                ThresholdOf(need: "fear", trigger: 0.0f, trigger_event: "animo_{agent_id}_fear_zero")
                            }
                        }
                    }
                }
            };
            ValidationResult r = Validator.Validate(root: root);
            AssertResult.HasError(r, rule_id: "A010");
        }
    }
}
