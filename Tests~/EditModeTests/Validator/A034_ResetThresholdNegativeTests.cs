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
    /// Decision-table tests for Validator rule A034 (v0.1.5, Q-S11):
    /// `binding.thresholds[].reset_threshold &lt; 0` (explicit user value) is
    /// an Error. The companion Composer floor in §12.3.4 handles the
    /// OMITTED case by flooring at 0; A034 fires only on explicit negatives,
    /// surfacing authoring typos rather than silently correcting them.
    ///
    /// Symmetric with A028 (commitment.bonus negative is also Error).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A034_ResetThresholdNegativeTests {

        [Test] public void Case01_ExplicitNegativeResetThreshold_FailsA034() {
            // Pre: user typed reset_threshold: -1.0 in the JSON.
            // Post: A034 Error. The Composer's floor does NOT silently fix it.
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
                                ThresholdOf(need: "fear", trigger: 80.0f, trigger_event: "animo_{agent_id}_fear_critical", reset: -1.0f)
                            }
                        }
                    }
                }
            };
            ValidationResult r = Validator.Validate(root: root);
            AssertResult.HasError(r, rule_id: "A034");
        }

        [Test] public void Case02_OmittedResetThreshold_DoesNotFailA034() {
            // Pre: reset_threshold omitted (null). Composer fills the default;
            // the user did NOT type a negative. A034 must NOT fire here.
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
                                ThresholdOf(need: "fear", trigger: 3.0f, trigger_event: "animo_{agent_id}_fear_low", reset: null)
                            }
                        }
                    }
                }
            };
            ValidationResult r = Validator.Validate(root: root);
            Assert.That(r.HasRule(rule_id: "A034"), Is.False,
                "Q-S11: A034 must only fire on EXPLICIT negative reset_threshold, not on omission");
        }
    }
}
