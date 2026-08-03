// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;
using Animo;
using Animo.Tools;

namespace Animo.Tests.EditMode.IntegrationTests {
    /// <summary>
    /// Task 3-6 Exit Gate: ScenarioRunner runs the goblin_scout example end-to-end.
    /// Loads examples/goblin_scout.json from disk, parses via Animo.JSON,
    /// runs ScenarioRunner.Run for 10 seconds @ 60Hz, verifies output integrity.
    /// </summary>
    [TestFixture]
    public class GoblinScoutEndToEndTests {

        static string RepoRoot() {
            string? dir = Directory.GetCurrentDirectory();
            while (dir != null && !File.Exists(Path.Combine(dir, "Scripts", "Const.cs")))
                dir = Directory.GetParent(dir)?.FullName;
            if (dir == null)
                throw new DirectoryNotFoundException("Could not locate repo root from " + Directory.GetCurrentDirectory());
            return dir;
        }

        [Test] public void GoblinScout_LoadsAndRuns_10Seconds() {
            string path = Path.Combine(RepoRoot(), "examples", "goblin_scout.json");
            Assume.That(File.Exists(path), $"goblin_scout.json must exist at {path}");
            string text = File.ReadAllText(path);

            var root   = JSON.Parse(text);
            var runner = new ScenarioRunner(root);
            var result = runner.Run(agent_id: "goblin_scout_01", duration: 10.0f, dt: 1.0f / 60.0f);

            Assert.That(result.frames.Count, Is.GreaterThan(500),
                "Phase 3 Exit Gate: 10-second run at 60Hz must record 500+ frames.");
            Assert.That(result.frames[0].behavior, Is.Not.Empty,
                "Phase 3 Exit Gate: spawn frame must have a non-empty behavior.");
            Assert.That(result.behavior_count.Count, Is.GreaterThan(0),
                "Phase 3 Exit Gate: BuildAnalysis must populate behavior_count.");
        }

        [Test] public void GoblinScout_ToCsv_RoundTripsBasic() {
            string path = Path.Combine(RepoRoot(), "examples", "goblin_scout.json");
            Assume.That(File.Exists(path));
            var root   = JSON.Parse(File.ReadAllText(path));
            var runner = new ScenarioRunner(root);
            var result = runner.Run(agent_id: "goblin_scout_01", duration: 1.0f, dt: 0.1f);

            string csv = result.ToCSV();
            Assert.That(csv, Is.Not.Empty, "Phase 3 Exit Gate: CSV must not be empty.");
            Assert.That(csv, Does.Contain("time,behavior"),
                "Phase 3 Exit Gate: CSV header must include time and behavior columns.");
            int lines = csv.Split('\n').Length;
            Assert.That(lines, Is.GreaterThan(10),
                $"Phase 3 Exit Gate: CSV must have at least 11 lines. Got {lines}.");
        }
    }
}
