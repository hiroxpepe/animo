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
    /// Decision-table test for Q-S17: A025 cycle detection must run in
    /// stage 2 against the COMPOSED `influences` graph, not just stage 1
    /// against raw arrays. A "ghost cycle" synthesized only by Kind ×
    /// Persona overlay (Kind: fear→confidence; Persona: confidence→fear)
    /// passes stage 1 unscathed because neither array contains a cycle in
    /// isolation. Stage 2 rebuilds the merged graph and rejects it with
    /// the same A025 Error.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A025_GhostCycleStage2Tests {

        [Test] public void Case01_KindFearToConf_PersonaConfToFear_CycleOnlyInComposed_FailsA025Stage2() {
            // Pre: Kind defines fear→confidence; Persona defines
            //      confidence→fear. Neither array has a cycle alone.
            //      Composed influences graph has both edges → cycle.
            // Pre-Q-S17: stage 1 A025 sees no cycle → Pass; stage 2 had
            //            no A025 → Pass; Engine.Live Step 2 explodes.
            // Post-Q-S17: stage 2 rebuilds composed graph, fires A025.
            Root root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> {
                    new Kind {
                        kind_id = "scared_creature",
                        actions = new List<Animo.Model.Action> { ActionOf(id: "X", need: "fear", tier: 2) },
                        influences = new List<Influence> {
                            InfluenceOf(source: "fear", target: "confidence", coefficient: -0.5f)
                        }
                    }
                },
                personas = new List<Persona> {
                    new Persona {
                        agent_id = "a",
                        kind_ids = new List<string> { "scared_creature" },
                        needs    = NeedsOf(("fear", 50f), ("confidence", 50f)),
                        influences = new List<Influence> {
                            InfluenceOf(source: "confidence", target: "fear", coefficient: -0.3f)
                        }
                    }
                }
            };
            // (v0.1.5, Q-S90) Stage 2 testing: previously called
            // Validator.Validate(root) which is Stage 1 ONLY (per the
            // Q-S71 split). A025 ghost-cycle is a Stage 2 rule (runs
            // against the composed influences graph), so the proper
            // call site is Validator.ValidateStage2(composed). Without
            // Q-S90 the test would have been permanently Red even
            // when Phase 3 implemented Stage 2 correctly — the test
            // never invoked the Stage 2 entry point. Phase 3 contract:
            // ValidateStage2(composed) finds A025 here.
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            ValidationResult r = Validator.ValidateStage2(composed: composed);
            AssertResult.HasError(r, rule_id: "A025");
        }
    }
}
