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
    /// <summary>Decision-table tests for Validator rule A008: actions[].exponent is 0.1 to 5.0.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A008_ActionExponentTests {
        [Test] public void Case01_ExponentBelowMin_FailsA008() {
            Root root = MinimalRoot(); root.personas[0].actions![0].exponent = 0.05f;
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A008");
        }
        [Test] public void Case02_ExponentAboveMax_FailsA008() {
            Root root = MinimalRoot(); root.personas[0].actions![0].exponent = 5.1f;
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A008");
        }
        [Test] public void Case03_ExponentBoundary_Passes() {
            Root root = MinimalRoot(); root.personas[0].actions![0].exponent = 5.0f;
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
