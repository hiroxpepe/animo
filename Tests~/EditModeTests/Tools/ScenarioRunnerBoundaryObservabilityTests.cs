// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Decision-table test for Q-S40 (v0.1.5): ScenarioRunner.Run with
    /// an event scheduled at exactly time == duration must record the
    /// boundary event in TraceResult.frames. Q-S35's post-loop sweep
    /// consumed the event but ran no Live + no RecordTraceFrame, so
    /// the Affect's effect was an observability black hole.
    ///
    /// Phase 3 contract: when the post-loop sweep consumes >= 1 event,
    /// run engine.Live(dt: 0.0f) + RecordTraceFrame(time: duration) so
    /// the boundary event's effect on Needs/scores is visible in the
    /// returned TraceResult.frames.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerBoundaryObservabilityTests {
        [Test] public void Case01_EventAtDuration_AppearsInFinalTraceFrame() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "ScenarioRunner.Run(duration: 10.0f, dt: 0.1f, events: [{ time: 10.0f, ... }]) " +
                "must produce a TraceResult whose final TraceFrame at time = 10.0 reflects " +
                "the post-Affect Need value. Q-S35 + Q-S40 final form: post-loop sweep + " +
                "Live(dt: 0.0f) + RecordTraceFrame(time: duration) when sweep consumed >= 1.");
        }
    }
}
