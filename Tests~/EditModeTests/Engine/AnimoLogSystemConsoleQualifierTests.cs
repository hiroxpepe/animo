// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// File-content test for Q-S127 (v0.1.5): physical
    /// Scripts/AnimoLog.cs comments use the fully qualified
    /// `System.Console.Error.WriteLine` form so a Phase 3
    /// implementer copy-pasting the comment doesn't hit CS0103
    /// (no `using System;` in the file).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AnimoLogSystemConsoleQualifierTests {
        [Test] public void Case01_PhysicalAnimoLog_UsesSystemConsoleQualifier() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? path = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "Scripts", "AnimoLog.cs");
                if (File.Exists(p)) { path = p; break; }
            }
            Assert.That(path, Is.Not.Null, "Q-S127: AnimoLog.cs must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("System.Console.Error.WriteLine"),
                "Q-S127: AnimoLog.cs must use the fully qualified form.");
            Assert.That(text, Does.Contain("Q-S127"),
                "Q-S127: AnimoLog.cs must reference Q-S127.");
        }
    }
}
