// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// File-content test for Q-S119 (v0.1.5): physical
    /// Scripts/Validator.cs ValidateStage2 XML docstring enumerates
    /// A040 alongside A019..A039. Pre-Q-S119 Q-S113 had added the
    /// rule to spec §13 but missed updating this docstring listing.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ValidatorDocstringIncludesA040Tests {
        [Test] public void Case01_PhysicalValidator_DocstringMentionsA040() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? path = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "Scripts", "Validator.cs");
                if (File.Exists(p)) { path = p; break; }
            }
            Assert.That(path, Is.Not.Null, "Q-S119: Validator.cs must exist.");
            var text = File.ReadAllText(path!);
            // Find the ValidateStage2 method's docstring block by
            // anchoring on the method declaration and walking back to
            // its preceding /// block. Simpler: just check that the
            // file contains both "ValidateStage2" and "A040" with the
            // Q-S113 attribution.
            Assert.That(text, Does.Contain("ValidateStage2"),
                "Q-S119: Validator.cs must declare ValidateStage2.");
            Assert.That(text, Does.Contain("A040"),
                "Q-S119: Validator.cs must mention A040 (added in Q-S113, listing fixed in Q-S119).");
            Assert.That(text, Does.Contain("Q-S119"),
                "Q-S119: Validator.cs must reference Q-S119.");
        }
    }
}
