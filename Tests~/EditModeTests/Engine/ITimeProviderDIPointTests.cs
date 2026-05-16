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
        [Test] public void Case01_SpecEN_DocumentsITimeProvider() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? path = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "docs", "animo_spec_v0.1.5_EN.md");
                if (File.Exists(p)) { path = p; break; }
            }
            Assert.That(path, Is.Not.Null, "Q-S115: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("ITimeProvider"),
                "Q-S115: spec EN must document the ITimeProvider DI seam.");
            Assert.That(text, Does.Contain("Q-S115"),
                "Q-S115: spec EN must reference Q-S115 in the documentation.");
        }
    }
}
