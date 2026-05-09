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
    /// <summary>Decision-table tests for Validator rule A007: actions[].tier is 1 to 5.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A007_ActionTierTests {
        [Test] public void Case01_TierZero_FailsA007() {
            Root root = MinimalRoot(); root.personas[0].actions![0].tier = 0;
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A007");
        }
        [Test] public void Case02_TierSix_FailsA007() {
            Root root = MinimalRoot(); root.personas[0].actions![0].tier = 6;
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A007");
        }
        [Test] public void Case03_TierFive_Passes() {
            Root root = MinimalRoot(); root.personas[0].actions![0].tier = 5;
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case04_TierOne_Passes() {
            Root root = MinimalRoot(); root.personas[0].actions![0].tier = 1;
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
