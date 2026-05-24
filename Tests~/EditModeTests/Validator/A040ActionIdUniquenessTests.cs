// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Spec-content test for Q-S113 (v0.1.5): rule A040 is defined
    /// in spec §13 ruleset for composed actions[].id uniqueness.
    /// Pre-Q-S113 A009 protected non-empty but uniqueness was an
    /// unverified assumption — _cached_action_triggers Dictionary
    /// silently overwrote duplicates.
    ///
    /// Phase 3 contract: Validator.ValidateStage2 emits A040 Error
    /// when composed.actions has any duplicate id.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A040ActionIdUniquenessTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_A040RuleIsDefined() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec_v0.1.5_EN.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S113: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("**A040**"),
                "Q-S113: spec EN must declare A040 in the §13 ruleset table.");
            Assert.That(text, Does.Contain("Q-S113"),
                "Q-S113: spec EN must reference Q-S113 in the A040 row.");
        }

        [Test] public void Case02_LayoutAnnotation_UpdatedToA000_A040() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec_v0.1.5_EN.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S113: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("A000-A040"),
                "Q-S113: §17 Layout must annotate Validator.cs with A000-A040.");
        }
    }
}
