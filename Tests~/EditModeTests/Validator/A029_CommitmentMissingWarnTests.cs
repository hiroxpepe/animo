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
    /// <summary>Decision-table tests for Validator rule A029: commitment omitted but actions has 2+ items (Warning).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A029_CommitmentMissingWarnTests {
        [Test] public void Case01_OmittedWithMultipleActions_WarnsA029() {
            Root root = MinimalRoot();
            root.personas[0].actions = new List<Action> {
                ActionOf(id: "A", need: "fear", tier: 2),
                ActionOf(id: "B", need: "idle", tier: 5) };
            root.personas[0].commitment = null;
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasWarning(r, rule_id: "A029");
        }
        [Test] public void Case02_OmittedWithSingleAction_Passes() {
            Root root = MinimalRoot(); root.personas[0].commitment = null;
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.HasRule(rule_id: "A029"), Is.False);
        }
    }
}
