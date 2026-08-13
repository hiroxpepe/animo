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
    /// .Run throws ArgumentException for delta_time &lt;= 0.0f at entry, before
    /// any time math runs, so a delta_time=0 call cannot silently produce an
    /// empty TraceResult via (int)Infinity = int.MinValue.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerDtPositiveGuardTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_RunHasDtPositiveGuard() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S117: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("Should `delta_time <= 0.0f`, throw `ArgumentException`"),
                "Q-S117: spec EN must guard delta_time at Run entry.");
            Assert.That(text, Does.Contain("ArgumentException"),
                "Q-S117: spec EN must throw ArgumentException for non-positive delta_time.");
        }

        [Test] public void Case02_ScenarioRunner_DocstringMentionsQS117() {
            // Verify the physical Scripts/Tools/ScenarioRunner.cs file
            // also records Q-S117 in its class docstring (Q-S101 NEW
            // LAYER discipline).
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "Scripts", "Tools", "ScenarioRunner.cs"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S117: ScenarioRunner.cs must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("Q-S117"),
                "Q-S117: physical ScenarioRunner.cs must reference Q-S117 (Q-S101 NEW LAYER discipline).");
        }
    }
}
