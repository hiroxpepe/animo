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
    /// <summary>Decision-table tests for Validator rule A001: personas exists and is not empty.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A001_PersonasExistsTests {
        [Test] public void Case01_MissingPersonas_FailsA001() {
            Root root = new Root { schema_version = "1.4" };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A001");
        }
        [Test] public void Case02_EmptyPersonas_FailsA001() {
            Root root = new Root { schema_version = "1.4", personas = new List<Persona>() };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A001");
        }
        [Test] public void Case03_OnePersona_Passes() {
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
