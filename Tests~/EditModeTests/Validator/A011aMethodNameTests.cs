// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// File-content test for Q-S129 (v0.1.5): the
    /// A011_PersonaActionsRequiredTests file declares its method as
    /// `Case01_NoKindIdsNoActions_FailsA011a`, matching the assertion
    /// string `"A011a"`. Pre-Q-S129 (post-Q-S100) the method name still
    /// read `FailsA011` while the assertion was `"A011a"` — cosmetic
    /// mismatch from Q-S100's sed touching only the assertion string.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A011aMethodNameTests {
        [Test] public void Case01_TestFile_MethodNameMatchesAssertion() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? path = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "Tests~", "EditModeTests", "Validator",
                    "A011_PersonaActionsRequiredTests.cs");
                if (File.Exists(p)) { path = p; break; }
            }
            Assert.That(path, Is.Not.Null,
                "Q-S129: A011_PersonaActionsRequiredTests.cs must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("Case01_NoKindIdsNoActions_FailsA011a"),
                "Q-S129: method name must end with FailsA011a, matching the A011a assertion.");
        }
    }
}
