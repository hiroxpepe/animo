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
    /// <summary>Decision-table tests for Validator rule A000: schema_version exists and is not empty.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A000_SchemaVersionTests {
        [Test] public void Case01_MissingSchemaVersion_FailsA000() {
            Root root = new Root { personas = new List<Persona> { PersonaOf(agent_id: "a") } };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A000");
        }
        [Test] public void Case02_EmptyStringSchemaVersion_FailsA000() {
            Root root = MinimalRoot(); root.schema_version = "";
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A000");
        }
        [Test] public void Case03_ValidSchemaVersion_Passes() {
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
