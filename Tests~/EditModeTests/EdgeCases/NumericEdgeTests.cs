// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using Animo.Tests.EditMode.Helpers;
using static Animo.Tests.EditMode.Helpers.Fixture;
using Action = Animo.Model.Action;

namespace Animo.Tests.EditMode.EdgeCases {
    /// <summary>Numeric edges: NaN / +Inf / -Inf / +0 / -0 / max / min / denormals (spec §4.6.3).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class NumericEdgeTests {
        [Test] public void Case01_NeedNaN_FailsValidator() {
            Root root = MinimalRoot(); root.personas[0].needs = NeedsOf(("fear", float.NaN));
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.has_errors, Is.True);
        }
        [Test] public void Case02_NeedPositiveInfinity_FailsValidator() {
            Root root = MinimalRoot(); root.personas[0].needs = NeedsOf(("fear", float.PositiveInfinity));
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.has_errors, Is.True);
        }
        [Test] public void Case03_NeedNegativeInfinity_FailsValidator() {
            Root root = MinimalRoot(); root.personas[0].needs = NeedsOf(("fear", float.NegativeInfinity));
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.has_errors, Is.True);
        }
        [Test] public void Case04_NeedNegativeZero_TreatedAsZero() {
            Root root = MinimalRoot(); root.personas[0].needs = NeedsOf(("fear", -0f));
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case05_NeedFloatMaxValue_FailsValidator() {
            Root root = MinimalRoot(); root.personas[0].needs = NeedsOf(("fear", float.MaxValue));
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.has_errors, Is.True);
        }
        [Test] public void Case06_ExponentJustAboveMax_FailsA008() {
            Root root = MinimalRoot(); root.personas[0].actions![0].exponent = 5.0001f;
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A008");
        }
        [Test] public void Case07_CoefficientNaN_FailsA012() {
            Root root = MinimalRoot();
            root.personas[0].influences = new List<Influence> { InfluenceOf(source: "a", target: "b", coefficient: float.NaN) };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A012");
        }
    }
}
