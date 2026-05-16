// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Spec-content test for Q-S114 (v0.1.5): C# code blocks use the
    /// C# string-interpolation form `$"{agent_id}_run_{_seq++}"`, not
    /// the Bash/JS template-literal form `${agent_id}_run_${_seq++}`
    /// that Q-S109's sed accidentally left in code blocks.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AgentIdRunStringInterpolationCSharpFormTests {
        static string? FindSpec() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            foreach (var r in roots) {
                var p = Path.Combine(r, "docs", "animo_spec_v0.1.5_EN.md");
                if (File.Exists(p)) return p;
            }
            return null;
        }
        [Test] public void Case01_SpecEN_CodeBlockHasCSharpForm() {
            var path = FindSpec();
            Assert.That(path, Is.Not.Null, "Q-S114: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("$\"{agent_id}_run_{_seq++}\""),
                "Q-S114: C# code blocks must use the C# interp form.");
        }
    }
}
