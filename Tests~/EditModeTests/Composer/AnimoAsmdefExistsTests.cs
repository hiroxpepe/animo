// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ComposerTests {
    [TestFixture]
    public class AnimoAsmdefExistsTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_AnimoAsmdef_ExistsWithGermioReference() {
            string? foundAsmdef = null;
            string? foundPkg    = null;
            { var p = System.IO.Path.Combine(RepoRoot(), "Scripts", "Animo.asmdef"); if (System.IO.File.Exists(p)) foundAsmdef = p; }
            { var p = System.IO.Path.Combine(RepoRoot(), "package.json"); if (System.IO.File.Exists(p)) foundPkg = p; }
            Assert.That(foundAsmdef, Is.Not.Null, "Q-S77: Scripts/Animo.asmdef must exist with Germio reference.");
            Assert.That(foundPkg,    Is.Not.Null, "Q-S77: package.json must exist with com.studiomeowtoon.germio dependency.");
            var asmdefText = System.IO.File.ReadAllText(foundAsmdef!);
            Assert.That(asmdefText, Does.Contain("\"Germio\""), "Q-S77: Animo.asmdef must reference Germio.");
            var pkgText = System.IO.File.ReadAllText(foundPkg!);
            Assert.That(pkgText, Does.Contain("com.studiomeowtoon.germio"), "Q-S77: package.json must include germio dependency.");
        }
    }
}
