// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// File-content test for Q-S91 (v0.1.5): EditMode asmdef references
    /// Animo.Tools so Unity Editor can compile Tools tests.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class EditModeAsmdefAnimoToolsRefTests {
        [Test] public void Case01_EditModeAsmdef_ReferencesAnimoTools() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? found = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "Tests~", "EditModeTests", "Animo.Tests.EditMode.asmdef");
                if (File.Exists(p)) { found = p; break; }
            }
            Assert.That(found, Is.Not.Null, "Q-S91: EditMode asmdef must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("\"Animo.Tools\""),
                "Q-S91: EditMode asmdef references must include \"Animo.Tools\".");
        }
    }
}
