// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Spec-content test for Q-S122 (v0.1.5): §13 A039 pseudocode
    /// uses `&lt;= 1.0f` (inclusive) so the boundary case (78.0 and
    /// 79.0, diff exactly 1.0) fires the Warning per the existing
    /// test's intent.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A039InclusiveBoundaryTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_A039PseudocodeInclusive() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S122: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("(next.trigger_threshold - prev.trigger_threshold) <= 1.0f"),
                "Q-S122: A039 pseudocode must use <= (inclusive) so 78/79 boundary fires.");
        }
    }
}
