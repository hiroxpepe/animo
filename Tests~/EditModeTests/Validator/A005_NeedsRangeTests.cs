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
    /// <summary>Decision-table tests for Validator rule A005: All needs values are in 0.0 to 100.0.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A005_NeedsRangeTests {
        [Test] public void Case01_NeedAbove100_FailsA005() {
            Root root = MinimalRoot(); root.personas[0].needs = NeedsOf(("fear", 150f));
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A005");
        }
        [Test] public void Case02_NeedBelowZero_FailsA005() {
            Root root = MinimalRoot(); root.personas[0].needs = NeedsOf(("fear", -1f));
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A005");
        }
        [Test] public void Case03_NeedAtBoundary100_Passes() {
            Root root = MinimalRoot(); root.personas[0].needs = NeedsOf(("fear", 100f));
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case04_NeedAtBoundary0_Passes() {
            Root root = MinimalRoot(); root.personas[0].needs = NeedsOf(("fear", 0f));
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
