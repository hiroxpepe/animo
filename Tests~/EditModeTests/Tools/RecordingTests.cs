// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using Animo.Tools;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.Tools {

    /// <summary>
    /// A Recording keeps the snapshot of each frame of a run, so a run can be
    /// saved and looked at again. It grows as the loop ticks, and on play-back it
    /// hands the saved frames out in order and lets a reader seek to any one.
    /// This is the Stage 3 "record and play again" piece.
    /// </summary>
    [TestFixture]
    public class RecordingTests {

        static MonitorLoop MakeLoop() {
            var persona = new Persona {
                agent_id = "scout",
                needs = NeedsOf(("hunger", 30f), ("fear", 10f), ("idle", 70f)),
                rates = RatesOf(("hunger", +0.5f)),
                actions = new List<Animo.Model.Action> {
                    ActionOf("Eat", "hunger", 1, 1.0f),
                    ActionOf("Flee", "fear", 1, 1.0f),
                },
            };
            return new MonitorLoop(new Animo.Core.Engine(persona), delta_time: 0.5f);
        }

        [Test]
        public void NewRecording_HasNoFrames() {
            var rec = new Recording();
            Assert.That(rec.Count, Is.Zero);
        }

        [Test]
        public void Add_GrowsTheFrameCount() {
            var loop = MakeLoop();
            var rec = new Recording();
            rec.Add(loop.Tick());
            rec.Add(loop.Tick());
            Assert.That(rec.Count, Is.EqualTo(2));
        }

        [Test]
        public void Frame_ReadsTheSavedSnapshotAtAnIndex() {
            var loop = MakeLoop();
            var rec = new Recording();
            var first = loop.Tick();
            rec.Add(first);
            Assert.That(rec.Frame(0).base_needs["hunger"], Is.EqualTo(first.base_needs["hunger"]));
        }

        [Test]
        public void Frame_KeepsEachFrameAsItWas() {
            var loop = MakeLoop();
            var rec = new Recording();
            rec.Add(loop.Tick());
            rec.Add(loop.Tick());
            // The two frames are different points in the run, not the same object.
            Assert.That(rec.Frame(0).base_needs["hunger"],
                Is.Not.EqualTo(rec.Frame(1).base_needs["hunger"]));
        }

        [Test]
        public void Frame_ClampsAnIndexOutsideTheRun() {
            var loop = MakeLoop();
            var rec = new Recording();
            rec.Add(loop.Tick());
            rec.Add(loop.Tick());
            // Seeking past the end gives the last frame, not a crash.
            Assert.That(rec.Frame(99), Is.SameAs(rec.Frame(1)));
            Assert.That(rec.Frame(-5), Is.SameAs(rec.Frame(0)));
        }

        [Test]
        public void Clear_EmptiesTheRecording() {
            var loop = MakeLoop();
            var rec = new Recording();
            rec.Add(loop.Tick());
            rec.Clear();
            Assert.That(rec.Count, Is.Zero);
        }
    }
}
