// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Spec-content test for Q-S124 (v0.1.5): §13 A019 row notes
    /// the Need-name collection covers the same union as A038's
    /// "in use" check (needs[] + actions[].need + influences[].source/
    /// target + binding.thresholds[].need + rates.keys()).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A019CoversThresholdsAndRatesTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_A019RowMentionsThresholdsAndRates() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec_v0.1.5_EN.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S124: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("Q-S124"),
                "Q-S124: spec EN must reference Q-S124 in the A019 row.");
            // The A019 row should explicitly enumerate the union
            Assert.That(text, Does.Contain("`binding.thresholds[].need` ∪ `rates.keys()`"),
                "Q-S124: spec EN A019 row must enumerate thresholds and rates in the coverage union.");
        }
    }
}
