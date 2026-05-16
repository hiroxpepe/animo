// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// File-existence test for Q-S77 (v0.1.5): `Scripts/Animo.asmdef`
    /// and `package.json` exist with Germio reference. Pre-Q-S77 the
    /// asmdef did not exist; Phase 3 Unity build would fail to resolve
    /// the Germio.Bus reference in Agent.cs.
    ///
    /// In headless dotnet test environments (CI, this test runner) the
    /// files exist but aren't actively consumed; the test asserts only
    /// presence + minimal correctness. Unity's editor consumes them
    /// during compilation when Phase 3 work begins.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AnimoAsmdefExistsTests {
        [Test] public void Case01_AnimoAsmdef_ExistsWithGermioReference() {
            // Resolve repo root by walking up from this assembly's location.
            var asmdefRel = "Scripts/Animo.asmdef";
            var pkgRel    = "package.json";
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? foundAsmdef = null;
            string? foundPkg    = null;
            foreach (var r in roots) {
                var a = Path.Combine(r, asmdefRel);
                var p = Path.Combine(r, pkgRel);
                if (File.Exists(a) && foundAsmdef == null) foundAsmdef = a;
                if (File.Exists(p) && foundPkg == null)    foundPkg = p;
            }
            Assert.That(foundAsmdef, Is.Not.Null,
                "Q-S77: Scripts/Animo.asmdef must exist with Germio reference.");
            Assert.That(foundPkg, Is.Not.Null,
                "Q-S77: package.json must exist with com.studiomeowtoon.germio dependency.");
            var asmdefText = File.ReadAllText(foundAsmdef!);
            Assert.That(asmdefText, Does.Contain("\"Germio\""),
                "Q-S77: Animo.asmdef references must include \"Germio\".");
            var pkgText = File.ReadAllText(foundPkg!);
            Assert.That(pkgText, Does.Contain("com.studiomeowtoon.germio"),
                "Q-S77: package.json dependencies must include com.studiomeowtoon.germio.");
        }
    }
}
