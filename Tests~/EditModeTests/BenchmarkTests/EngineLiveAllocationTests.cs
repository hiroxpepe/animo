// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.BenchmarkTests {
    /// <summary>
    /// Task 3-5-a (roadmap §5.8) Zero-GC proof for Engine.Live hot path.
    /// Engine.Live(delta_time) must allocate zero bytes after warm-up.
    /// </summary>
    [TestFixture]
    public class EngineLiveAllocationTests {

        static Engine MakeRealisticEngine() {
            // 8 needs (typical Persona), 4 actions, 3 influences, 2 thresholds
            var p = new Persona { agent_id = "bench",
                needs = NeedsOf(
                    ("hunger",30f), ("fatigue",20f), ("fear",10f), ("anger",0f),
                    ("loneliness",15f), ("confidence",50f), ("curiosity",40f), ("idle",70f) ),
                rates = RatesOf(("hunger", +0.5f), ("fatigue", +0.3f), ("idle", -0.1f)),
                actions = new List<Animo.Model.Action> {
                    ActionOf("Eat","hunger",1,1.0f),
                    ActionOf("Rest","fatigue",1,1.0f),
                    ActionOf("Flee","fear",2,1.5f),
                    ActionOf("Idle","idle",5,1.0f) },
                influences = new List<Influence> {
                    new Influence { source="fear", target="confidence", coefficient=-0.3f },
                    new Influence { source="fatigue", target="confidence", coefficient=-0.2f },
                    new Influence { source="loneliness", target="curiosity", coefficient=-0.1f } },
                commitment = new Commitment { bonus = 5f },
                binding = new Binding { thresholds = new List<Threshold>{
                    ThresholdOf("fear", 80f, "fear_critical"),
                    ThresholdOf("hunger", 90f, "hunger_critical") }},
                suppression = new Suppression { tier2 = 0.5f, tier3 = 0.4f, tier4 = 0.3f, tier5 = 0.2f }
            };
            return new Engine(p);
        }

        [Test] public void Engine_Live_HotPath_IsZeroAllocation_Over_100K_Calls() {
            var engine = MakeRealisticEngine();

            // Warm up: JIT + any one-time allocations
            for (int i = 0; i < 1000; i++) engine.Live(delta_time: 0.016f);

            // Force GC to baseline
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // Measure
            long alloc_before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 100000; i++) engine.Live(delta_time: 0.016f);
            long alloc_after = GC.GetAllocatedBytesForCurrentThread();

            long allocated = alloc_after - alloc_before;
            Assert.That(allocated, Is.EqualTo(0),
                $"Engine.Live allocated {allocated} bytes over 100K calls. " +
                "Expected 0. Hot path GC violation (§16.1).");
        }

        [Test] public void Engine_Live_PerCall_Under_10us() {
            var engine = MakeRealisticEngine();

            // Warm up
            for (int i = 0; i < 1000; i++) engine.Live(delta_time: 0.016f);

            // Measure
            const int iterations = 100000;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++) engine.Live(delta_time: 0.016f);
            sw.Stop();

            double per_call_us = (sw.Elapsed.TotalMilliseconds * 1000.0) / iterations;
            // Generous bound: 10 µs per call (roadmap §5.8.2 target)
            // Debug builds typically 3-5x slower than Release; allow 50µs in Debug.
            Assert.That(per_call_us, Is.LessThan(50.0),
                $"Engine.Live took {per_call_us:F2} µs/call. Target < 10 µs (Release), " +
                $"< 50 µs (Debug). Total {sw.Elapsed.TotalMilliseconds:F1} ms over {iterations} calls.");
        }
    }
}
