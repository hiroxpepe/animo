// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Decision-table test for Q-S55 (v0.1.5): ScenarioRunner sweeps
    /// `events[next].time <= 0.0f` BEFORE the spawn Live(0.0f) +
    /// RecordTraceFrame(0.0f). A t=0 event modifies Need values
    /// observably in the trace's first frame.
    ///
    /// Phase 3 contract: events = [{ time: 0.0, ev: Affect("fear", +50) }]
    /// produces TraceResult.frames[0].time == 0.0 with fear == 50
    /// (or whatever spawn-+-50 evaluates to).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerT0EventVisibilityTests {
        [Test] public void Case01_T0Event_VisibleInFrameAtTimeZero() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "ScenarioRunner.Run(events: [{ time: 0.0, ev: Affect(\"fear\", +50) }], ...) " +
                "must produce a TraceResult whose frames[0].time == 0.0 and shows fear with " +
                "the +50 already applied. Pre-Q-S55 the t=0 event was deferred to first " +
                "loop iteration, leaving frames[0] showing pre-Affect spawn state.");
        }
    }
}
