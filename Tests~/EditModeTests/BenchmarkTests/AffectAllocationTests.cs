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
    /// Task 3-5-b (roadmap §5.8) Zero-GC proof for Engine.Affect.
    /// Engine.Affect must allocate zero bytes after warm-up.
    /// </summary>
    [TestFixture]
    public class AffectAllocationTests {

        static Engine MakeEngine() {
            var p = new Persona { agent_id = "bench",
                needs   = NeedsOf(("fear",30f), ("idle",70f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Flee","fear",2), ActionOf("Idle","idle",5) }
            };
            return new Engine(p);
        }

        [Test] public void Engine_Affect_IsZeroAllocation_Over_100K_Calls() {
            var engine = MakeEngine();

            // Warm up
            for (int i = 0; i < 1000; i++) engine.Affect("fear", (i & 1) == 0 ? +1f : -1f);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long alloc_before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 100000; i++) engine.Affect("fear", (i & 1) == 0 ? +1f : -1f);
            long alloc_after = GC.GetAllocatedBytesForCurrentThread();

            long allocated = alloc_after - alloc_before;
            Assert.That(allocated, Is.EqualTo(0),
                $"Engine.Affect allocated {allocated} bytes over 100K calls. " +
                "Expected 0. Hot path GC violation (§16.1).");
        }

        [Test] public void Engine_Affect_WithForceReset_IsZeroAllocation() {
            var engine = MakeEngine();

            for (int i = 0; i < 1000; i++) engine.Affect("fear", +0.1f, force_reset: true);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long alloc_before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 100000; i++) engine.Affect("fear", +0.1f, force_reset: true);
            long alloc_after = GC.GetAllocatedBytesForCurrentThread();

            long allocated = alloc_after - alloc_before;
            Assert.That(allocated, Is.EqualTo(0),
                $"Engine.Affect(force_reset:true) allocated {allocated} bytes. Expected 0.");
        }
    }
}
