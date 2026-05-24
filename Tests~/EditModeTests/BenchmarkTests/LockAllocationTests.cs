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
    /// Task 3-5-c (roadmap §5.8) Zero-GC proof for Engine.Lock / Unlock.
    /// Lock state changes are once, not per frame, but must still be zero-alloc.
    /// </summary>
    [TestFixture]
    public class LockAllocationTests {

        static Engine MakeEngine() {
            var p = new Persona { agent_id = "bench",
                needs   = NeedsOf(("fear",30f), ("idle",70f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Flee","fear",2), ActionOf("Idle","idle",5) }
            };
            return new Engine(p);
        }

        [Test] public void Engine_Lock_Unlock_IsZeroAllocation_Over_10K_Calls() {
            var engine = MakeEngine();
            engine.Live(dt: 0.016f);  // seed behavior

            // Warm up
            for (int i = 0; i < 1000; i++) { engine.Lock(0.5f); engine.Unlock(); }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long alloc_before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 10000; i++) { engine.Lock(0.5f); engine.Unlock(); }
            long alloc_after = GC.GetAllocatedBytesForCurrentThread();

            long allocated = alloc_after - alloc_before;
            Assert.That(allocated, Is.EqualTo(0),
                $"Engine.Lock+Unlock allocated {allocated} bytes over 10K cycles. " +
                "Expected 0. Hot path GC violation (§16.1).");
        }

        [Test] public void Engine_Lock_LiveDuringLock_IsZeroAllocation() {
            // Lock once, then 100K Live calls inside lock — still zero-alloc
            var engine = MakeEngine();
            engine.Live(dt: 0.016f);

            // Warm up
            for (int i = 0; i < 1000; i++) engine.Live(dt: 0.016f);
            engine.Lock(duration: 1e6f);  // effectively perma-lock (capped at MAX, but no unlock)

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long alloc_before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 100000; i++) engine.Live(dt: 0.016f);
            long alloc_after = GC.GetAllocatedBytesForCurrentThread();

            long allocated = alloc_after - alloc_before;
            Assert.That(allocated, Is.EqualTo(0),
                $"Live() during lock allocated {allocated} bytes over 100K calls. Expected 0.");
        }
    }
}
