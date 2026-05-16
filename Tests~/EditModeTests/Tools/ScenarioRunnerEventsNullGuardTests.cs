// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Spec-content test for Q-S104 (v0.1.5): §26.3.1 Run loop
    /// normalizes `events` once at entry to Array.Empty, so Run()
    /// called with default events=null does not NRE.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerEventsNullGuardTests {
        [Test] public void Case01_SpecEN_RunNormalizesEventsToEmptyArray() {
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
            Assert.That(path, Is.Not.Null, "Q-S104: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("events ??= System.Array.Empty<TimedAffectEvent>()"),
                "Q-S104: spec EN must normalize events to empty array before the Run loops.");
        }
    }
}
