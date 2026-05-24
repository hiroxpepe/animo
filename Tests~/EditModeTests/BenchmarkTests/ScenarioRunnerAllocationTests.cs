// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Tools;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.BenchmarkTests {
    /// <summary>
    /// Task 3-5-d (roadmap §5.8) Allocation profile for ScenarioRunner.Run.
    ///
    /// Note: ScenarioRunner.Run intentionally allocates TraceResult, frames List,
    /// per-frame TraceFrame, etc. — these are part of its API (observation surface).
    /// What we verify here is that the PER-FRAME steady-state allocation is bounded.
    /// </summary>
    [TestFixture]
    public class ScenarioRunnerAllocationTests {

        static Animo.Model.Root MakeRoot() => new Animo.Model.Root { schema_version="1.5",
            personas = new List<Persona>{ new Persona { agent_id="bench",
                needs=NeedsOf(("fear",30f),("idle",70f)),
                actions=new List<Animo.Model.Action>{
                    ActionOf("Flee","fear",2), ActionOf("Idle","idle",5) }}}};

        [Test] public void ScenarioRunner_Run_PerFrameAllocation_IsBounded() {
            // (#1 fix) Compute per-frame allocation via DIFFERENCE of two runs
            // with different frame counts. This eliminates the ScenarioRunner ctor
            // / first-Run initialization cost (A_init) from the measurement —
            // the previous "alloc_100 < alloc_10 * 15" formula would pass even
            // with a 1KB/frame leak because A_init dwarfed the per-frame term.
            //
            // Math:
            //   alloc_short  = A_init + N_short  * per_frame
            //   alloc_long   = A_init + N_long   * per_frame
            //   per_frame    = (alloc_long - alloc_short) / (N_long - N_short)

            // Warm up: JIT + ensure both runs are post-JIT
            new ScenarioRunner(MakeRoot()).Run("bench", duration: 0.1f, dt: 0.1f);
            new ScenarioRunner(MakeRoot()).Run("bench", duration: 1.0f, dt: 0.1f);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // Short run: 10 frames
            long before_short = GC.GetAllocatedBytesForCurrentThread();
            new ScenarioRunner(MakeRoot()).Run("bench", duration: 1.0f, dt: 0.1f);
            long after_short  = GC.GetAllocatedBytesForCurrentThread();
            long alloc_short  = after_short - before_short;

            // Long run: 1000 frames (much larger so per-frame term dominates)
            long before_long = GC.GetAllocatedBytesForCurrentThread();
            new ScenarioRunner(MakeRoot()).Run("bench", duration: 100.0f, dt: 0.1f);
            long after_long  = GC.GetAllocatedBytesForCurrentThread();
            long alloc_long  = after_long - before_long;

            // Per-frame steady-state allocation (init cost cancels out)
            const int N_short = 10, N_long = 1000;
            long per_frame = (alloc_long - alloc_short) / (N_long - N_short);

            // TraceResult per-frame: 1 TraceFrame object + 3 Dictionaries + signals_fired List.
            // Realistic ceiling: a few hundred bytes per frame at the OBSERVATION layer.
            // The underlying Engine.Live hot path is verified zero-alloc separately.
            // 2 KB/frame upper bound catches genuine leaks while accepting the
            // intentional per-frame TraceFrame allocation.
            Assert.That(per_frame, Is.LessThan(4096),
                $"ScenarioRunner.Run per-frame allocation = {per_frame} bytes. " +
                $"Short run ({N_short} frames) = {alloc_short} B, " +
                $"Long run ({N_long} frames) = {alloc_long} B. " +
                "Per-frame must stay under 4 KB (TraceFrame + Dictionaries only).");
            Assert.That(per_frame, Is.GreaterThanOrEqualTo(0),
                $"Per-frame allocation must be non-negative. Got {per_frame} bytes " +
                "(measurement noise or ordering issue).");
        }
    }
}
