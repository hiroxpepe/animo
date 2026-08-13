// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// File-content test for Q-S94 (v0.1.5): spec narrative and package.json
    /// use the same com.studiomeowtoon.* namespace. UPM cannot resolve
    /// dependencies if they disagree.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PackageNamespaceUnifiedTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_PackageJson_UsesStudiomeowtoonPrefix() {
            string? found = null;
            { var p = Path.Combine(RepoRoot(), "package.json"); if (File.Exists(p)) found = p; }
            Assert.That(found, Is.Not.Null, "Q-S94: package.json must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("com.studiomeowtoon.animo"),
                "Q-S94: package.json must declare name as com.studiomeowtoon.animo.");
            Assert.That(text, Does.Contain("com.studiomeowtoon.germio"),
                "Q-S94: package.json must depend on com.studiomeowtoon.germio.");
        }

        [Test] public void Case02_SpecEN_NoMeowtoonOutsideHistoricalCitation() {
            string? found = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec.md"); if (File.Exists(p)) found = p; }
            Assert.That(found, Is.Not.Null, "Q-S94: spec EN must exist.");
            var text = File.ReadAllText(found!);
            // Q-S94 ヘッダー Theme + §3.1 paragraph contain `com.meowtoon.*` as historical citation.
            // All other appearances should be eliminated. Count check: <= 4 historical mentions.
            int meowtoonHits = (text.Length - text.Replace("com.meowtoon.", "").Length) / "com.meowtoon.".Length;
            Assert.That(meowtoonHits, Is.LessThanOrEqualTo(4),
                $"Q-S94: spec EN should have <= 4 com.meowtoon.* mentions (historical citations only); found {meowtoonHits}.");
        }
    }
}
