// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Decision-table test for Q-S84 (v0.1.5): ScenarioRunner.Run drives
    /// exactly `floor(duration / dt)` Live(dt) calls without IEEE-754
    /// float drift. Phase 3 contract: integer step counter.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerIntegerStepTests {
        [Test] public void Case01_Run_ExecutesExactlyFloorDurationOverDtLiveCalls() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "ScenarioRunner.Run(duration=10.0f, dt=0.1f) must execute exactly 100 " +
                "Live(dt) calls (the floor(10.0/0.1) value), even after thousands of " +
                "iterations where naive `current_time += dt` would drift. Q-S84 fix: " +
                "use `for (int i = 0; i < total_steps; i++)`.");
        }
    }
}
