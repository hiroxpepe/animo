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
    /// Decision-table tests for Validator rule A035 (v0.1.5, Q-S15).
    /// A035 is a POST-COMPOSITION check (§13.2 stage 2): after Composer
    /// fills omitted reset_threshold defaults via Q-S11, the resulting
    /// (trigger_threshold, reset_threshold) pair must still satisfy
    /// `trigger > reset` strictly. Catches the residual case where
    /// trigger=0 + omitted reset escapes A010 + A023 + A034 to land as
    /// (0, 0) and chatter at the Need clamp's lower bound.
    ///
    /// Note: Case01's input has trigger=0 which Phase_2_4_7 also rejects
    /// via A010 (Q-S15 tightening). The intent here is that BOTH rules
    /// fire on this input, providing defense in depth — A010 from the
    /// stage-1 boundary side, A035 from the stage-2 post-fill side. The
    /// test asserts A035 specifically.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A035_PostComposeTriggerGtResetTests {

        [Test] public void Case01_TriggerZero_OmittedReset_PostComposePairZeroZero_FailsA035() {
            // Pre: trigger_threshold = 0.0 (boundary), reset_threshold omitted.
            // Composer's Q-S11 floor fills reset_threshold = Max(0, 0-5) = 0.0.
            // Post-Compose pair is (0.0, 0.0) — A035 fails strictly.
            Root root = new Root {
                schema_version = "1.5",
                personas = new List<Persona> {
                    new Persona {
                        agent_id = "a",
                        actions  = new List<Animo.Model.Action> { ActionOf(id: "X", need: "fear", tier: 2) },
                        needs    = NeedsOf(("fear", 0f)),
                        binding  = new Binding {
                            on_action_change = "animo_{agent_id}_{behavior}",
                            thresholds = new List<Threshold> {
                                ThresholdOf(need: "fear", trigger: 0.0f, trigger_event: "animo_{agent_id}_fear_chatter", reset: null)
                            }
                        }
                    }
                }
            };
            // (v0.1.5, Q-S90) Stage 2 testing — see A025 file for full
            // rationale. A035 is a Stage 2 rule per §13.2 (post-Compose
            // trigger > reset check); must call ValidateStage2 to fire.
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            ValidationResult r = Validator.ValidateStage2(composed: composed);
            AssertResult.HasError(r, rule_id: "A035");
        }

        [Test] public void Case02_TriggerFive_OmittedReset_PostComposePairFiveZero_PassesA035() {
            // Pre: trigger_threshold = 5.0, reset_threshold omitted.
            // Composer fills reset = Max(0, 5-5) = 0.0. Pair is (5.0, 0.0)
            // — strictly trigger > reset, so A035 must NOT fire.
            Root root = new Root {
                schema_version = "1.5",
                personas = new List<Persona> {
                    new Persona {
                        agent_id = "a",
                        actions  = new List<Animo.Model.Action> { ActionOf(id: "X", need: "fear", tier: 2) },
                        needs    = NeedsOf(("fear", 30f)),
                        binding  = new Binding {
                            on_action_change = "animo_{agent_id}_{behavior}",
                            thresholds = new List<Threshold> {
                                ThresholdOf(need: "fear", trigger: 5.0f, trigger_event: "animo_{agent_id}_fear_low", reset: null)
                            }
                        }
                    }
                }
            };
            // (v0.1.5, Q-S90) Stage 2 testing.
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            ValidationResult r = Validator.ValidateStage2(composed: composed);
            Assert.That(r.HasRule(rule_id: "A035"), Is.False,
                "Q-S15: A035 must NOT fire when post-Compose trigger > reset strictly (5.0 > 0.0)");
        }
    }
}
