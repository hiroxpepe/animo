// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Spec-content test for Q-S98 (v0.1.5): §26.3.1 ScenarioRunner Run
    /// loop uses Math.Round((double)duration / (double)dt), NOT
    /// Math.Floor(duration / dt). Pre-Q-S98 Q-S84's Floor + float
    /// division systematically under-shot by 1 step (float32
    /// 10.0f/0.1f = 99.999... → Floor = 99).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerMathRoundTests {
        [Test] public void Case01_SpecEN_RunLoopUsesMathRoundDoubleCast() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? found = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "docs", "animo_spec_v0.1.5_EN.md");
                if (File.Exists(p)) { found = p; break; }
            }
            Assert.That(found, Is.Not.Null, "Q-S98: spec EN must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("Math.Round((double)duration / (double)dt)"),
                "Q-S98: spec EN must use Math.Round + double cast for total_steps.");
        }

        [Test] public void Case02_IEEE754_FloorWouldUnderShoot_RoundCorrects() {
            // Sanity check: verify the IEEE-754 behavior that motivated Q-S98.
            float duration = 10.0f;
            float dt = 0.1f;
            // Floor on float division SHOULD under-shoot.
            int floorResult = (int)System.Math.Floor((double)(duration / dt));
            // Round on double division SHOULD give 100.
            int roundResult = (int)System.Math.Round((double)duration / (double)dt);
            Assert.That(roundResult, Is.EqualTo(100),
                "Q-S98: Math.Round + double cast must give the correct 100 steps.");
            Assert.That(floorResult, Is.LessThan(roundResult),
                "Q-S98: Math.Floor on float division would under-shoot " +
                $"(got {floorResult} instead of {roundResult}) — that's the bug Q-S98 fixes.");
        }
    }
}
