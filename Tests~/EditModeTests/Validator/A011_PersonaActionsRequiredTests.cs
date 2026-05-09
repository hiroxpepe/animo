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
    /// <summary>Decision-table tests for Validator rule A011: If no kind_ids, the Persona must have at least one action (A011a).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A011_PersonaActionsRequiredTests {
        [Test] public void Case01_NoKindIdsNoActions_FailsA011() {
            Root root = new Root {
                schema_version = "1.4",
                personas = new List<Persona> { new Persona { agent_id = "a" } }
            };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A011");
        }
        [Test] public void Case02_WithKindIdsNoOwnActions_Passes() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k", actions = new List<Action> { ActionOf(id: "A", need: "idle", tier: 5) } } },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "k" } } }
            };
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case03_NoKindIdsWithActions_Passes() {
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
