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
    /// The monitor calls Snapshot every frame, right after Live. Snapshot builds
    /// small dictionaries — that is by design, since each recorded frame must be
    /// its own object — but it must not push that cost into Live's hot path. This
    /// holds that Live itself stays zero-allocation even when a monitor is reading
    /// a snapshot each frame.
    /// </summary>
    [TestFixture]
    public class SnapshotAllocationTests {

        static Animo.Core.Engine MakeEngine() {
            var persona = new Persona {
                agent_id = "bench",
                needs = NeedsOf(
                    ("hunger", 30f), ("fatigue", 20f), ("fear", 10f), ("anger", 0f),
                    ("loneliness", 15f), ("confidence", 50f), ("curiosity", 40f), ("idle", 70f)),
                rates = RatesOf(("hunger", +0.5f), ("fatigue", +0.3f), ("idle", -0.1f)),
                actions = new List<Animo.Model.Action> {
                    ActionOf("Eat", "hunger", 1, 1.0f),
                    ActionOf("Rest", "fatigue", 1, 1.0f),
                    ActionOf("Flee", "fear", 1, 1.0f),
                },
            };
            return new Animo.Core.Engine(persona);
        }

        [Test]
        public void Live_StaysZeroAllocation_WhenSnapshotIsReadEachFrame() {
            var engine = MakeEngine();
            // Warm up both paths.
            for (int i = 0; i < 1000; i++) { engine.Live(0.016f); engine.Snapshot(); }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // Measure Live alone across many frames, with Snapshot NOT counted.
            long worst = 0;
            for (int i = 0; i < 10000; i++) {
                long before = GC.GetAllocatedBytesForCurrentThread();
                engine.Live(0.016f);
                long after = GC.GetAllocatedBytesForCurrentThread();
                worst = Math.Max(worst, after - before);
                engine.Snapshot(); // read a snapshot each frame, outside the measure
            }
            Assert.That(worst, Is.EqualTo(0),
                "Live's hot path must stay zero-allocation even while a monitor reads snapshots.");
        }
    }
}
