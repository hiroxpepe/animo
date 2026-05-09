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
    /// <summary>Decision-table tests for Validator rule A024: Action using idle should be tier 5 (Warning).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A024_IdleActionTierTests {
        [Test] public void Case01_IdleAtTier1_WarnsA024() {
            Root root = MinimalRoot(); root.personas[0].actions![0].need = "idle"; root.personas[0].actions![0].tier = 1;
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasWarning(r, rule_id: "A024");
        }
        [Test] public void Case02_IdleAtTier5_Passes() {
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.HasRule(rule_id: "A024"), Is.False);
        }
    }
}
