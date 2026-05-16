// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// File-content test for Q-S95 (v0.1.5): A019_TypoNeedsKeyTests.cs
    /// calls Validator.ValidateStage2(composed), not Validator.Validate(root).
    /// Pre-Q-S95 the test file called Stage 1 entry but Q-S39 had moved
    /// A019 to Stage 2 — would stay Red FOREVER.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A019Stage2CallTests {
        [Test] public void Case01_A019Test_CallsValidateStage2NotValidate() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? found = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "Tests~", "EditModeTests", "Validator",
                    "A019_TypoNeedsKeyTests.cs");
                if (File.Exists(p)) { found = p; break; }
            }
            Assert.That(found, Is.Not.Null, "Q-S95: A019_TypoNeedsKeyTests.cs must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("Validator.ValidateStage2"),
                "Q-S95: A019 test must call Validator.ValidateStage2 (Q-S39 moved A019 to Stage 2).");
            Assert.That(text, Does.Not.Contain("Validator.Validate(root: root)"),
                "Q-S95: A019 test must NOT call Stage 1 entry Validator.Validate(root).");
        }
    }
}
