// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.BootstrapperTests {
    /// <summary>
    /// File-content test for Q-S118 (v0.1.5): physical
    /// Scripts/AnimoBootstrapper.cs records the editor-only guard so
    /// production scene transitions do not wipe Store entries that
    /// DontDestroyOnLoad Agents depend on.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AnimoBootstrapperEditorOnlyGuardTests {
        [Test] public void Case01_PhysicalAnimoBootstrapper_HasEditorOnlyGuardComment() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? path = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "Scripts", "AnimoBootstrapper.cs");
                if (File.Exists(p)) { path = p; break; }
            }
            Assert.That(path, Is.Not.Null, "Q-S118: AnimoBootstrapper.cs must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("Application.isEditor"),
                "Q-S118: AnimoBootstrapper.cs must document Application.isEditor guard.");
            Assert.That(text, Does.Contain("Q-S118"),
                "Q-S118: AnimoBootstrapper.cs must reference Q-S118.");
        }

        [Test] public void Case02_SpecEN_AnimoBootstrapperOnDestroyHasGuard() {
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
            Assert.That(path, Is.Not.Null, "Q-S118: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("if (!Application.isEditor || Application.isPlaying) return;"),
                "Q-S118: spec EN must show the editor-only guard before Q-S58 cleanup pair.");
        }
    }
}
