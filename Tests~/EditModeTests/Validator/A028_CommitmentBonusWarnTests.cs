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
    /// <summary>
    /// Decision-table tests for Validator rule A028: commitment.bonus range [0, 50].
    /// Negative is Error (v0.1.5, Q8); 0..30 Pass; >30..50 Warning (lock-in risk).
    /// </summary>
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
        [Test] public void Case03_BonusAboveWarnThreshold_WarnsA028() {
            Root root = MinimalRoot(); root.personas[0].commitment = new Commitment { bonus = 40f };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasWarning(r, rule_id: "A028");
        }
        [Test] public void Case04_BonusNegative_FailsA028(){
            // Q8 (v0.1.5): negative bonus would make the *current* action lose score every
            // frame — pathological "anti-commitment". Always a typo or a sign-flip.
            Root root = MinimalRoot(); root.personas[0].commitment = new Commitment { bonus = -5f };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A028");
        }
        [Test] public void Case05_BonusAboveCeiling_FailsA028() {
            // Q8 (v0.1.5): ceiling 50 = WARN_THRESHOLD (30) + safety margin.
            Root root = MinimalRoot(); root.personas[0].commitment = new Commitment { bonus = 100f };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A028");
        }
        [Test] public void Case06_BonusAtCeiling_WarnsButNotError() {
            Root root = MinimalRoot(); root.personas[0].commitment = new Commitment { bonus = 50f };
            ValidationResult r = Validator.Validate(root: root);
            AssertResult.HasWarning(r, rule_id: "A028");
            Assert.That(r.has_errors, Is.False);
        }
        [Test] public void Case07_BonusZero_Passes() {
            Root root = MinimalRoot(); root.personas[0].commitment = new Commitment { bonus = 0f };
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.HasRule(rule_id: "A028"), Is.False);
        }
    }
}
