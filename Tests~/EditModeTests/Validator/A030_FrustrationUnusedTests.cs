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
    /// <summary>Decision-table tests for Validator rule A030: no actions/influences use frustration (Warning, v0.1.4).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A030_FrustrationUnusedTests {
        [Test] public void Case01_FrustrationUnused_WarnsA030() {
            Root root = MinimalRoot();
            // No mention of frustration anywhere.
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasWarning(r, rule_id: "A030");
        }
        [Test] public void Case02_FrustrationUsedInActions_Passes() {
            Root root = MinimalRoot();
            root.personas[0].actions = new List<Action> {
                ActionOf(id: "Idle", need: "idle", tier: 5),
                ActionOf(id: "Sulk", need: "frustration", tier: 2) };
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.HasRule(rule_id: "A030"), Is.False);
        }
    }
}
