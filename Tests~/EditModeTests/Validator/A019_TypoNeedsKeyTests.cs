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
    /// <summary>Decision-table tests for Validator rule A019: unknown needs key looks like a typo (Warning, v0.1.4 8 needs).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A019_TypoNeedsKeyTests {
        [Test] public void Case01_TypoOfHunger_WarnsA019() {
            Root root = MinimalRoot();
            root.personas[0].needs = NeedsOf(("hungrr", 50f));
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasWarning(r, rule_id: "A019");
        }
        [Test] public void Case02_ExactStandardNeed_Passes() {
            Root root = MinimalRoot();
            root.personas[0].needs = NeedsOf(("hunger", 50f));
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case03_GenuineCustomNeed_Passes() {
            Root root = MinimalRoot();
            root.personas[0].needs = NeedsOf(("longing", 50f));
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.HasRule(rule_id: "A019"), Is.False);
        }
    }
}
