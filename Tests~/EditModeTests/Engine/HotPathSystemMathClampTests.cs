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
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_HotPathUsesSystemMathClamp() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S116: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("System.Math.Clamp"),
                "Q-S116: §9.6.5 cascade pseudocode must use System.Math.Clamp.");
        }
    }
}
