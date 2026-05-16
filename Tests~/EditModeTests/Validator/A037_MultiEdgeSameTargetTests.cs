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
    /// Decision-table tests for Validator rule A037 (v0.1.5, Q-S20).
    /// A037 is a Warning that fires when two or more `influences[]`
    /// entries write to the same target Need. With mid-cascade Clamp
    /// (§9.6.3), the apply order of those edges affects the result —
    /// the order is fixed deterministically by the composed
    /// `influences[]` sequence (Q-S19/S20 Persona-first), but the LLM
    /// author may not realize that authoring order changes outputs.
    /// A037 surfaces this so authors can either reorder deliberately or
    /// restructure to avoid the dependency on order.
    ///
    /// Note: A037 is a NUDGE rule — the configuration is legal and
    /// deterministic under Q-S20. Warning, not Error.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A037_MultiEdgeSameTargetTests {

        [Test] public void Case01_TwoEdgesSameTarget_WarnsA037() {
            // Pre: Two independent edges (hunger→fear, fatigue→fear) both
            //      write to "fear". Q-S20 makes the apply order
            //      deterministic via stable topo sort, but A037 nudges
            //      the LLM to notice the order matters here.
            Root root = new Root {
                schema_version = "1.5",
                personas = new List<Persona> {
                    new Persona {
                        agent_id = "a",
                        actions  = new List<Animo.Model.Action> { ActionOf(id: "X", need: "fear", tier: 2) },
                        needs    = NeedsOf(("hunger", 50f), ("fatigue", 50f), ("fear", 50f)),
                        influences = new List<Influence> {
                            InfluenceOf(source: "hunger",  target: "fear", coefficient: +0.5f),
                            InfluenceOf(source: "fatigue", target: "fear", coefficient: -0.5f)
                        }
                    }
                }
            };
            // (v0.1.5, Q-S90) Stage 2 testing.
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            ValidationResult r = Validator.ValidateStage2(composed: composed);
            AssertResult.HasWarning(r, rule_id: "A037");
        }

        [Test] public void Case02_OneEdgePerTarget_NoA037() {
            // Pre: Each target Need is written to by exactly one edge.
            // No order ambiguity → A037 must NOT fire.
            Root root = new Root {
                schema_version = "1.5",
                personas = new List<Persona> {
                    new Persona {
                        agent_id = "a",
                        actions  = new List<Animo.Model.Action> { ActionOf(id: "X", need: "fear", tier: 2) },
                        needs    = NeedsOf(("hunger", 50f), ("fatigue", 50f), ("fear", 50f), ("confidence", 50f)),
                        influences = new List<Influence> {
                            InfluenceOf(source: "hunger",  target: "fear",       coefficient: +0.5f),
                            InfluenceOf(source: "fatigue", target: "confidence", coefficient: -0.3f)
                        }
                    }
                }
            };
            // (v0.1.5, Q-S90) Stage 2 testing.
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            ValidationResult r = Validator.ValidateStage2(composed: composed);
            Assert.That(r.HasRule(rule_id: "A037"), Is.False,
                "Q-S20: A037 must NOT fire when each target Need has at most one inbound edge");
        }
    }
}
