// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// File-content test for Q-S90 (v0.1.5): the four Stage 2 test files
    /// (A025/A035/A036/A037) call Validator.ValidateStage2(composed),
    /// not Validator.Validate(root). Pre-Q-S90 they all called Stage 1
    /// — tests would stay Red FOREVER even when Phase 3 implements
    /// Stage 2 correctly.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Stage2TestsCallValidateStage2Tests {
        static string? FindFile(string relativePath) {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            foreach (var r in roots) {
                var p = Path.Combine(r, relativePath);
                if (File.Exists(p)) return p;
            }
            return null;
        }

        [Test, TestCase("A025_GhostCycleStage2Tests.cs"),
                TestCase("A035_PostComposeTriggerGtResetTests.cs"),
                TestCase("A036_ComposedActionsEmptyTests.cs"),
                TestCase("A037_MultiEdgeSameTargetTests.cs")]
        public void Case_AllFourFiles_CallValidateStage2NotValidate(string filename) {
            var path = FindFile(Path.Combine("Tests~", "EditModeTests", "Validator", filename));
            Assert.That(path, Is.Not.Null, $"Q-S90: {filename} must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("Validator.ValidateStage2"),
                $"Q-S90: {filename} must call Validator.ValidateStage2.");
            Assert.That(text, Does.Not.Contain("Validator.Validate(root: root)"),
                $"Q-S90: {filename} must NOT call Stage 1 entry Validator.Validate(root).");
        }
    }
}
