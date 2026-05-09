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
    /// <summary>Decision-table tests for Validator rule A015: thresholds[].trigger placeholders only {agent_id}.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A015_TriggerPlaceholdersTests {
        [Test] public void Case01_DisallowedPlaceholder_FailsA015() {
            Root root = MinimalRoot();
            root.personas[0].binding = new Binding { thresholds = new List<Threshold> {
                ThresholdOf(need: "fear", trigger: 80f, trigger_event: "x_{behavior}_y", reset: 70f) } };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A015");
        }
        [Test] public void Case02_AllowedPlaceholder_Passes() {
            Root root = MinimalRoot();
            root.personas[0].binding = new Binding { thresholds = new List<Threshold> {
                ThresholdOf(need: "fear", trigger: 80f, trigger_event: "x_{agent_id}_y", reset: 70f) } };
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
