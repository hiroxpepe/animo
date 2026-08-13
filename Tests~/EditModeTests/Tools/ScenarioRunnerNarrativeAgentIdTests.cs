// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Spec-content test for Q-S109 (v0.1.5): Q-S42 narrative uses
    /// the parameter name `agent_id` (matching the actual Run
    /// signature), not the out-of-scope `template_id`.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerNarrativeAgentIdTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_Q_S42NarrativeUsesAgentIdNotTemplateId() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S109: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("${agent_id}_run_${_sequence++}"),
                "Q-S109: spec EN must use the in-scope ${agent_id}_run_${_sequence++} form.");
        }
    }
}
