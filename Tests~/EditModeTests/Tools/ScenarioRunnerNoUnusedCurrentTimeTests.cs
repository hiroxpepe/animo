// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Spec-content test for Q-S123 (v0.1.5): §26.3.1 does NOT
    /// declare `float current_time = total_steps * delta_time;` as a live
    /// code-block statement because no downstream code reads it
    /// (CS0219 "variable assigned but never used").
    /// 
    /// The check excludes references inside backtick-quoted prose
    /// (header Theme rows, §3.1 paragraphs, decision log narrative)
    /// where the dead-line text is merely cited in describing the
    /// removal. Phase 3 source-of-truth is the live pseudocode
    /// block at line ~5895 of EN spec.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerNoUnusedCurrentTimeTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_NoUnusedCurrentTimeDeclaration() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec_v0.1.5_EN.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S123: spec EN must exist.");
            var lines = File.ReadAllLines(path!);
            // Live code-block lines are unindented C#-like and follow a
            // `csharp fence. We scan for lines that START with "float
            // current_time" (no leading "//" comment, no backtick prose
            // quoting). The pre-Q-S123 dead line was exactly:
            //   float current_time = total_steps * delta_time;   // for the post-loop sweep boundary check below
            // Post-Q-S123 there should be ZERO such lines.
            int violation_count = 0;
            foreach (var line in lines) {
                var trimmed = line.TrimStart();
                // Skip backtick-quoted prose references (e.g. "...declared
                // `float current_time = total_steps * delta_time;` here...")
                if (trimmed.Contains("`float current_time = total_steps")) continue;
                // Skip comment-only lines (e.g. "// `float current_time...
                // = total_steps * delta_time;` here — but no")
                if (trimmed.StartsWith("//")) continue;
                // Real code-block declaration: line begins with the type
                if (trimmed.StartsWith("float current_time = total_steps")) {
                    violation_count++;
                }
            }
            Assert.That(violation_count, Is.EqualTo(expected: 0),
                "Q-S123: spec EN must NOT carry a live `float current_time = total_steps * delta_time;` " +
                $"declaration in code blocks; found {violation_count} live declarations.");
        }
    }
}
