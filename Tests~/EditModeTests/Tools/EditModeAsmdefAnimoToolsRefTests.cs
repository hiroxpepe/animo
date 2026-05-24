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
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_EditModeAsmdef_ReferencesAnimoTools() {
            string? found = null;
            { var p = Path.Combine(RepoRoot(), "Tests~", "EditModeTests", "Animo.Tests.EditMode.asmdef"); if (File.Exists(p)) found = p; }
            Assert.That(found, Is.Not.Null, "Q-S91: EditMode asmdef must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("\"Animo.Tools\""),
                "Q-S91: EditMode asmdef references must include \"Animo.Tools\".");
        }
    }
}
