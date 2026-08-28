// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// File-content test for Q-S94 (v0.1.5): spec narrative and package.json
    /// use the same com.meowtoon.* namespace. UPM cannot resolve
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

        [Test] public void Case01_PackageJson_UsesMeowtoonPrefix() {
            string? found = null;
            { var p = Path.Combine(RepoRoot(), "package.json"); if (File.Exists(p)) found = p; }
            Assert.That(found, Is.Not.Null, "Q-S94: package.json must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("com.meowtoon.animo"),
                "Q-S94: package.json must declare name as com.meowtoon.animo.");
            Assert.That(text, Does.Contain("com.meowtoon.germio"),
                "Q-S94: package.json must depend on com.meowtoon.germio.");
        }

        [Test] public void Case02_SpecEN_UsesMeowtoonNamespaceConsistently() {
            string? found = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec.md"); if (File.Exists(p)) found = p; }
            Assert.That(found, Is.Not.Null, "Q-S94: spec EN must exist.");
            var text = File.ReadAllText(found!);
            // Q-S94: com.meowtoon.germio is the true UPM name germio's own package.json
            // holds — checked live, 2026-08-29. A spec that ever names the wrong prefix
            // (com.studiomeowtoon.*) would mislead a reader trying to wire a dependency.
            Assert.That(text, Does.Not.Contain("com.studiomeowtoon."),
                "Q-S94: spec EN must never name the wrong prefix — com.meowtoon.* is true.");
        }
    }
}
