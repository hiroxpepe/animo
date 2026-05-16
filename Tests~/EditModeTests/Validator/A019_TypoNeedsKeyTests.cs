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
    /// Decision-table tests for Validator rule A019: unknown needs key
    /// looks like a typo (Warning, v0.1.4 8 needs).
    ///
    /// (v0.1.5, Q-S39) A019 was moved to Stage 2 so Persona-level
    /// `needs_meta` can suppress false-positives. (v0.1.5, Q-S95) The
    /// test file's call sites were not updated when Q-S39 moved the
    /// rule — every case still called `Validator.Validate(root)` which
    /// is Stage 1 ONLY per the Q-S71 split. Q-S90 caught and fixed
    /// this for A025/A035/A036/A037 but missed A019. Q-S95 closes the
    /// gap: 3 cases all rewrite to `Composer.Compose(persona, root)`
    /// then `Validator.ValidateStage2(composed)` so Phase 3's correct
    /// Stage 2 implementation will actually surface the Warning.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A019_TypoNeedsKeyTests {
        [Test] public void Case01_TypoOfHunger_WarnsA019() {
            Root root = MinimalRoot();
            root.personas[0].needs = NeedsOf(("hungrr", 50f));
            // (v0.1.5, Q-S95) Stage 2 testing — A019 moved to Stage 2 in Q-S39.
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            ValidationResult r = Validator.ValidateStage2(composed: composed);
            AssertResult.HasWarning(r, rule_id: "A019");
        }
        [Test] public void Case02_ExactStandardNeed_Passes() {
            Root root = MinimalRoot();
            root.personas[0].needs = NeedsOf(("hunger", 50f));
            // (v0.1.5, Q-S95) Stage 2 testing.
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            ValidationResult r = Validator.ValidateStage2(composed: composed);
            AssertResult.IsClean(r);
        }
        [Test] public void Case03_GenuineCustomNeed_Passes() {
            Root root = MinimalRoot();
            root.personas[0].needs = NeedsOf(("longing", 50f));
            // (v0.1.5, Q-S95) Stage 2 testing.
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            ValidationResult r = Validator.ValidateStage2(composed: composed);
            Assert.That(r.HasRule(rule_id: "A019"), Is.False);
        }
    }
}
