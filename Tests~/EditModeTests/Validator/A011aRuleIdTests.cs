// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// File-content test for Q-S100 (v0.1.5, CENTENNIAL): Test files
    /// asserting the no-kind_ids-no-actions rule must use rule_id
    /// "A011a" (matching spec §13.1's v0.1.5 split into A011a/A011b),
    /// not "A011".
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A011aRuleIdTests {
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

        [Test] public void Case01_A011PersonaActionsRequiredTests_AssertsA011a() {
            var path = FindFile(Path.Combine("Tests~", "EditModeTests", "Validator",
                "A011_PersonaActionsRequiredTests.cs"));
            Assert.That(path, Is.Not.Null, "Q-S100: A011_PersonaActionsRequiredTests.cs must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("rule_id: \"A011a\""),
                "Q-S100: A011_PersonaActionsRequiredTests.cs must assert rule_id A011a.");
        }

        [Test] public void Case02_EmptyAndNullTests_AssertsA011a() {
            var path = FindFile(Path.Combine("Tests~", "EditModeTests", "EdgeCases",
                "EmptyAndNullTests.cs"));
            Assert.That(path, Is.Not.Null, "Q-S100: EmptyAndNullTests.cs must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("rule_id: \"A011a\""),
                "Q-S100: EmptyAndNullTests.cs must assert rule_id A011a.");
        }
    }
}
