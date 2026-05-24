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
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_PhysicalAnimoBootstrapper_HasEditorOnlyGuardComment() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "Scripts", "AnimoBootstrapper.cs"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S118: AnimoBootstrapper.cs must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("Application.isEditor"),
                "Q-S118: AnimoBootstrapper.cs must document Application.isEditor guard.");
            Assert.That(text, Does.Contain("Q-S118"),
                "Q-S118: AnimoBootstrapper.cs must reference Q-S118.");
        }

        [Test] public void Case02_SpecEN_AnimoBootstrapperOnDestroyHasGuard() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec_v0.1.5_EN.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S118: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("if (!Application.isEditor || Application.isPlaying) return;"),
                "Q-S118: spec EN must show the editor-only guard before Q-S58 cleanup pair.");
        }
    }
}
