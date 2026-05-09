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
    /// <summary>Decision-table tests for Validator rule A021: schema_version must be 1.3 or 1.4.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A021_SupportedVersionTests {
        [Test] public void Case01_Version10_FailsA021() {
            Root root = MinimalRoot(); root.schema_version = "1.0";
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A021");
        }
        [Test] public void Case02_Version13_Passes() {
            Root root = MinimalRoot(); root.schema_version = "1.3";
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case03_Version14_Passes() {
            Root root = MinimalRoot(); root.schema_version = "1.4";
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case04_Version15Future_FailsA021() {
            Root root = MinimalRoot(); root.schema_version = "1.5";
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A021");
        }
    }
}
