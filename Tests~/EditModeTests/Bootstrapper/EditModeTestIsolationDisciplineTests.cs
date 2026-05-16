// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.BootstrapperTests {
    /// <summary>
    /// Spec-content test for Q-S130 (v0.1.5): §11.6.5 documents the
    /// EditMode test isolation discipline. Q-S118's editor-only guard
    /// is correct for production, but NUnit EditMode tests run with
    /// `(isEditor=true, isPlaying=false)` — the same state where
    /// cleanup fires. The spec must explicitly delegate isolation
    /// responsibility to test-side `[SetUp]`.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class EditModeTestIsolationDisciplineTests {
        [Test] public void Case01_SpecEN_DocsTestSideDiscipline() {
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
            Assert.That(path, Is.Not.Null, "Q-S130: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("EditMode test isolation discipline"),
                "Q-S130: spec EN must declare test isolation discipline.");
            Assert.That(text, Does.Contain("Q-S130"),
                "Q-S130: spec EN must reference Q-S130.");
            Assert.That(text, Does.Contain("`Animo.Store.ResetForTesting()` in `[SetUp]`"),
                "Q-S130: spec EN must require Store reset in SetUp.");
        }
    }
}
