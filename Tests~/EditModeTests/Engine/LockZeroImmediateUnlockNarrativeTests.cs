// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Spec-content test for Q-S126 (v0.1.5): §9.2 narrative explains
    /// `Lock(0)` is immediately observable as `is_locked == false` via
    /// the property semantics (`_lock_remaining > 0`), no special path
    /// needed in `Lock`.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class LockZeroImmediateUnlockNarrativeTests {
        [Test] public void Case01_SpecEN_LockZeroNarrativeMentionsPropertySemantics() {
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
            Assert.That(path, Is.Not.Null, "Q-S126: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("Q-S126"),
                "Q-S126: spec EN must reference Q-S126 in the Lock(0) clarification.");
            Assert.That(text, Does.Contain("no special path inside `Lock` is required"),
                "Q-S126: spec EN must explicitly state no special path in Lock is needed.");
        }
    }
}
