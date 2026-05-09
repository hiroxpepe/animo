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
    /// <summary>Decision-table tests for Validator rule A010: thresholds[].trigger_threshold is 0.0 to 100.0.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A010_ThresholdRangeTests {
        [Test] public void Case01_TriggerAbove100_FailsA010() {
            Root root = MinimalRoot();
            root.personas[0].binding = new Binding { thresholds = new List<Threshold> { ThresholdOf(need: "fear", trigger: 200f, trigger_event: "x") } };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A010");
        }
        [Test] public void Case02_TriggerBelowZero_FailsA010() {
            Root root = MinimalRoot();
            root.personas[0].binding = new Binding { thresholds = new List<Threshold> { ThresholdOf(need: "fear", trigger: -10f, trigger_event: "x") } };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A010");
        }
        [Test] public void Case03_TriggerBoundary_Passes() {
            Root root = MinimalRoot();
            root.personas[0].binding = new Binding { thresholds = new List<Threshold> { ThresholdOf(need: "fear", trigger: 100f, trigger_event: "x", reset: 90f) } };
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
