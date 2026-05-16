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
        [Test] public void Case01_SpecEN_Q_S42NarrativeUsesAgentIdNotTemplateId() {
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
            Assert.That(path, Is.Not.Null, "Q-S109: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("${agent_id}_run_${_seq++}"),
                "Q-S109: spec EN must use the in-scope ${agent_id}_run_${_seq++} form.");
        }
    }
}
