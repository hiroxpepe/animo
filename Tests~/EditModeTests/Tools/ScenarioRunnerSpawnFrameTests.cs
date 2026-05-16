// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Decision-table test for Q-S51 (v0.1.5): ScenarioRunner.Run records
    /// a spawn frame at time = 0.0 BEFORE the main loop begins. Pre-Q-S51
    /// the first frame was at time = dt, leaving the spawn moment
    /// (initial Need values, Q-S9 tie-break initial behavior) invisible.
    ///
    /// Phase 3 contract: TraceResult.frames[0].time == 0.0f. Total frames =
    /// 1 (spawn) + floor(duration/dt) (loop) + 0/1 (Q-S40 boundary).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerSpawnFrameTests {
        [Test] public void Case01_FirstFrameTimeIsZero_NotDt() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "ScenarioRunner.Run with duration=10.0, dt=0.1 must produce a TraceResult " +
                "whose frames[0].time == 0.0 (spawn-state frame from Q-S51 pre-loop " +
                "Live(0.0f) + RecordTraceFrame(0.0f)). Pre-Q-S51 frames[0].time was 0.1.");
        }
    }
}
