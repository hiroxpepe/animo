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
    /// <summary>Decision-table tests for Validator rule A028: commitment.bonus > 30 may cause action lock-in (Warning).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A028_CommitmentBonusWarnTests {
        [Test] public void Case01_BonusBelowThreshold_Passes() {
            Root root = MinimalRoot(); root.personas[0].commitment = new Commitment { bonus = 20f };
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.HasRule(rule_id: "A028"), Is.False);
        }
        [Test] public void Case02_BonusAtThreshold_Passes() {
            Root root = MinimalRoot(); root.personas[0].commitment = new Commitment { bonus = 30f };
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.HasRule(rule_id: "A028"), Is.False);
        }
        [Test] public void Case03_BonusAboveThreshold_WarnsA028() {
            Root root = MinimalRoot(); root.personas[0].commitment = new Commitment { bonus = 50f };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasWarning(r, rule_id: "A028");
        }
    }
}
