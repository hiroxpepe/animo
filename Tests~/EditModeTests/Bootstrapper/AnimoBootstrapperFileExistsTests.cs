// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.BootstrapperTests {
    /// <summary>
    /// File-existence test for Q-S97 (v0.1.5): Scripts/AnimoBootstrapper.cs
    /// is physically present in the repository. Pre-Q-S97 §11.6.5 had the
    /// class as spec text but no .cs file — same gap pattern as Q-S83.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AnimoBootstrapperFileExistsTests {
        [Test] public void Case01_AnimoBootstrapperCs_ExistsInScriptsDir() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? found = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "Scripts", "AnimoBootstrapper.cs");
                if (File.Exists(p)) { found = p; break; }
            }
            Assert.That(found, Is.Not.Null,
                "Q-S97: Scripts/AnimoBootstrapper.cs must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("class AnimoBootstrapper"),
                "Q-S97: AnimoBootstrapper.cs must declare class AnimoBootstrapper.");
            Assert.That(text, Does.Contain("UNITY_5_3_OR_NEWER"),
                "Q-S97: AnimoBootstrapper.cs must be bracketed in #if UNITY_5_3_OR_NEWER.");
            Assert.That(text, Does.Contain("DefaultExecutionOrder(-1000)"),
                "Q-S97: AnimoBootstrapper.cs must have DefaultExecutionOrder(-1000).");
        }
    }
}
