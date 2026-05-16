// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Spec-content test for Q-S116 (v0.1.5): §9.6.5 hot-path
    /// pseudocode uses System.Math.Clamp (BCL), not Mathf.Clamp
    /// (UnityEngine), to honor §5 + asmdef noEngineReferences:true
    /// for Animo.Core.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class HotPathSystemMathClampTests {
        [Test] public void Case01_SpecEN_HotPathUsesSystemMathClamp() {
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
            Assert.That(path, Is.Not.Null, "Q-S116: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("System.Math.Clamp(eff.Get(inf.target)"),
                "Q-S116: §9.6.5 cascade pseudocode must use System.Math.Clamp.");
        }
    }
}
