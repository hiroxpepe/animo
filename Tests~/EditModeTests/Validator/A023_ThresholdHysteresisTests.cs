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
    /// <summary>Decision-table tests for Validator rule A023: trigger_threshold > reset_threshold (v0.1.1).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A023_ThresholdHysteresisTests {
        [Test] public void Case01_TriggerEqualsReset_FailsA023() {
            Root root = MinimalRoot();
            root.personas[0].binding = new Binding { thresholds = new List<Threshold> { ThresholdOf(need: "fear", trigger: 70f, trigger_event: "x", reset: 70f) } };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A023");
        }
        [Test] public void Case02_TriggerBelowReset_FailsA023() {
            Root root = MinimalRoot();
            root.personas[0].binding = new Binding { thresholds = new List<Threshold> { ThresholdOf(need: "fear", trigger: 60f, trigger_event: "x", reset: 70f) } };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A023");
        }
        [Test] public void Case03_TriggerAboveReset_Passes() {
            Root root = MinimalRoot();
            root.personas[0].binding = new Binding { thresholds = new List<Threshold> { ThresholdOf(need: "fear", trigger: 80f, trigger_event: "x", reset: 70f) } };
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
