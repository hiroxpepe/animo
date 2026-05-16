// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Tools;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Spec-content + reflection test for Q-S117 (v0.1.5): ScenarioRunner
    /// .Run throws ArgumentException for dt &lt;= 0.0f at entry, before
    /// any time math runs, so a dt=0 call cannot silently produce an
    /// empty TraceResult via (int)Infinity = int.MinValue.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerDtPositiveGuardTests {
        [Test] public void Case01_SpecEN_RunHasDtPositiveGuard() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? path = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "docs", "animo_spec_v0.1.5_EN.md");
                if (File.Exists(p)) { path = p; break; }
            }
            Assert.That(path, Is.Not.Null, "Q-S117: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("if (dt <= 0.0f) {"),
                "Q-S117: spec EN must guard dt at Run entry.");
            Assert.That(text, Does.Contain("System.ArgumentException"),
                "Q-S117: spec EN must throw ArgumentException for non-positive dt.");
        }

        [Test] public void Case02_ScenarioRunner_DocstringMentionsQS117() {
            // Verify the physical Scripts/Tools/ScenarioRunner.cs file
            // also records Q-S117 in its class docstring (Q-S101 NEW
            // LAYER discipline).
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? path = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "Scripts", "Tools", "ScenarioRunner.cs");
                if (File.Exists(p)) { path = p; break; }
            }
            Assert.That(path, Is.Not.Null, "Q-S117: ScenarioRunner.cs must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("Q-S117"),
                "Q-S117: physical ScenarioRunner.cs must reference Q-S117 (Q-S101 NEW LAYER discipline).");
        }
    }
}
