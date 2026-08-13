// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Spec-content test for Q-S115 (v0.1.5): §11.4.1 documents an
    /// ITimeProvider DI receiving point so Phase 3 can substitute
    /// MockTime under EditMode tests instead of UnityEngine.Time.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ITimeProviderDIPointTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_DocumentsITimeProvider() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S115: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("ITimeProvider"),
                "Q-S115: spec EN must document the ITimeProvider DI seam.");
            Assert.That(text, Does.Contain("Q-S115"),
                "Q-S115: spec EN must reference Q-S115 in the documentation.");
        }
    }
}
