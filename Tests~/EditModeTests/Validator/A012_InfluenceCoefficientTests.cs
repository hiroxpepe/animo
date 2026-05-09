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
    /// <summary>Decision-table tests for Validator rule A012: influences[].coefficient is -1.0 to 1.0.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A012_InfluenceCoefficientTests {
        [Test] public void Case01_CoefficientAbove1_FailsA012() {
            Root root = MinimalRoot();
            root.personas[0].influences = new List<Influence> { InfluenceOf(source: "a", target: "b", coefficient: 2f) };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A012");
        }
        [Test] public void Case02_CoefficientBelowMinus1_FailsA012() {
            Root root = MinimalRoot();
            root.personas[0].influences = new List<Influence> { InfluenceOf(source: "a", target: "b", coefficient: -2f) };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A012");
        }
        [Test] public void Case03_CoefficientBoundary_Passes() {
            Root root = MinimalRoot();
            root.personas[0].influences = new List<Influence> { InfluenceOf(source: "a", target: "b", coefficient: 1f) };
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
