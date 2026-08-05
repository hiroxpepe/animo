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
    /// Decision-table tests for Validator rule A036 (v0.1.5, Q-S18).
    /// A036 is a POST-COMPOSITION check (§13.2 stage 2) that the per-
    /// Persona `actions[]` list is non-empty after Composer cascade.
    ///
    /// Q6's decision log claimed "A011a covers the post-composition case
    /// too" — but A011a runs in stage 1 only, where A011b explicitly
    /// allows omitting `actions` when `kind_ids` is non-empty. A Persona
    /// that omits `actions` and references a Kind with empty `actions[]`
    /// formerly passed both A011a and A011b at stage 1, then reached the
    /// Engine with `actions = []`, where Q-S9's tie-break
    /// (`actions.First(...)`) would throw `InvalidOperationException` on
    /// the first `Live(delta_time)`. A036 closes this architectural gap.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A036_ComposedActionsEmptyTests {

        [Test] public void Case01_PersonaOmitsActions_KindActionsEmpty_ComposedEmpty_FailsA036() {
            // Pre: Kind has empty actions; Persona omits actions and
            // references the Kind. Stage 1: A011b allows the omission
            // (kind_ids is non-empty). Composer merges → composed.actions
            // = []. Stage 2: A036 must fire.
            Root root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> {
                    new Kind {
                        kind_id = "empty_kind",
                        actions = new List<Animo.Model.Action>()   // explicitly empty
                    }
                },
                personas = new List<Persona> {
                    new Persona {
                        agent_id = "a",
                        kind_ids = new List<string> { "empty_kind" }
                        // actions intentionally omitted (A011b allows this)
                    }
                }
            };
            // (v0.1.5, Q-S90) Stage 2 testing.
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            ValidationResult r = Validator.ValidateStage2(composed: composed);
            AssertResult.HasError(r, rule_id: "A036");
        }

        [Test] public void Case02_PersonaHasActions_KindActionsEmpty_ComposedNonEmpty_PassesA036() {
            // Pre: Kind has empty actions but Persona supplies actions.
            // Composer merges → composed.actions has the Persona's items.
            // A036 must NOT fire (the post-composition list is non-empty).
            Root root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> {
                    new Kind {
                        kind_id = "empty_kind",
                        actions = new List<Animo.Model.Action>()
                    }
                },
                personas = new List<Persona> {
                    new Persona {
                        agent_id = "a",
                        kind_ids = new List<string> { "empty_kind" },
                        actions  = new List<Animo.Model.Action> {
                            ActionOf(id: "Idle", need: "idle", tier: 5)
                        }
                    }
                }
            };
            // (v0.1.5, Q-S90) Stage 2 testing.
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            ValidationResult r = Validator.ValidateStage2(composed: composed);
            Assert.That(r.HasRule(rule_id: "A036"), Is.False,
                "Q-S18: A036 must NOT fire when composed actions[] is non-empty");
        }
    }
}
