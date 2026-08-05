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
    /// MonitorLoop drives a live engine for the monitor: each tick it advances
    /// the engine by delta_time and hands out a snapshot, and a step-in queued from the
    /// dashboard lands at the head of the next tick, never in the middle of one.
    /// It knows nothing of sockets, so the loop can be tested on its own.
    /// </summary>
    [TestFixture]
    public class MonitorLoopTests {

        static Animo.Core.Engine MakeEngine() {
            var persona = new Persona {
                agent_id = "scout",
                needs = NeedsOf(("hunger", 30f), ("fear", 10f), ("idle", 70f)),
                rates = RatesOf(("hunger", +0.5f)),
                actions = new List<Animo.Model.Action> {
                    ActionOf("Eat", "hunger", 1, 1.0f),
                    ActionOf("Flee", "fear", 1, 1.0f),
                    ActionOf("Idle", "idle", 1, 1.0f),
                },
            };
            return new Animo.Core.Engine(persona);
        }

        [Test]
        public void Tick_AdvancesTheEngineAndReturnsASnapshot() {
            var loop = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            var before = loop.Engine.GetBaseNeed("hunger");
            var snap = loop.Tick();
            Assert.That(snap, Is.Not.Null);
            Assert.That(loop.Engine.GetBaseNeed("hunger"), Is.Not.EqualTo(before));
            Assert.That(snap.base_needs["hunger"], Is.EqualTo(loop.Engine.GetBaseNeed("hunger")));
        }

        [Test]
        public void StepIn_LandsAtTheHeadOfTheNextTick() {
            var loop = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            loop.Tick();
            var fear_before = loop.Engine.GetBaseNeed("fear");
            // Queue a step-in; it must NOT change the engine until the next tick.
            loop.QueueAffect("fear", 50f);
            Assert.That(loop.Engine.GetBaseNeed("fear"), Is.EqualTo(fear_before),
                "the step-in must not touch the engine before the next tick");
            // The next tick applies it at the head, then advances.
            loop.Tick();
            Assert.That(loop.Engine.GetBaseNeed("fear"), Is.GreaterThan(fear_before));
        }

        [Test]
        public void StepIn_AppliesInOrderAtTheNextTick() {
            var loop = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            loop.Tick();
            var fear_before = loop.Engine.GetBaseNeed("fear");
            loop.QueueAffect("fear", 10f);
            loop.QueueAffect("fear", 20f);
            loop.Tick();
            // Both step-ins landed this tick: +30 total, before the decay of one delta_time.
            Assert.That(loop.Engine.GetBaseNeed("fear"), Is.GreaterThanOrEqualTo(fear_before + 30f - 1f));
        }

        [Test]
        public void Pause_HoldsTheEngineStill() {
            var loop = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            loop.Tick();
            loop.Pause();
            var hunger_before = loop.Engine.GetBaseNeed("hunger");
            var snap = loop.Tick();
            // Paused: the engine does not advance, but a snapshot still comes out.
            Assert.That(loop.Engine.GetBaseNeed("hunger"), Is.EqualTo(hunger_before));
            Assert.That(snap, Is.Not.Null);
        }

        [Test]
        public void Resume_LetsTheEngineAdvanceAgain() {
            var loop = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            loop.Pause();
            loop.Tick();
            loop.Resume();
            var hunger_before = loop.Engine.GetBaseNeed("hunger");
            loop.Tick();
            Assert.That(loop.Engine.GetBaseNeed("hunger"), Is.Not.EqualTo(hunger_before));
        }

        [Test]
        public void QueueLock_LocksAtTheNextTick() {
            var loop = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            loop.Tick();
            loop.QueueLock(2f, Animo.Core.LockMode.Hard);
            // Queued, not yet applied.
            Assert.That(loop.Engine.IsLocked, Is.False,
                "the lock must not touch the engine before the next tick");
            loop.Tick();
            Assert.That(loop.Engine.IsLocked, Is.True);
        }

        [Test]
        public void QueueUnlock_UnlocksAtTheNextTick() {
            var loop = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            loop.Tick();
            loop.QueueLock(5f, Animo.Core.LockMode.Hard);
            loop.Tick();
            Assert.That(loop.Engine.IsLocked, Is.True);
            loop.QueueUnlock();
            loop.Tick();
            Assert.That(loop.Engine.IsLocked, Is.False);
        }

        [Test]
        public void Step_AdvancesOneFrameWhilePaused() {
            var loop = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            loop.Pause();
            var hunger_before = loop.Engine.GetBaseNeed("hunger");
            var snap = loop.Step();
            // Step moves exactly one frame even though the loop is paused,
            // and the loop stays paused afterwards.
            Assert.That(loop.Engine.GetBaseNeed("hunger"), Is.Not.EqualTo(hunger_before));
            Assert.That(loop.IsPaused, Is.True);
            Assert.That(snap, Is.Not.Null);
        }

        [Test]
        public void Step_AppliesQueuedStepInsAtItsHead() {
            var loop = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            loop.Pause();
            loop.Tick();
            var fear_before = loop.Engine.GetBaseNeed("fear");
            loop.QueueAffect("fear", 50f);
            loop.Step();
            Assert.That(loop.Engine.GetBaseNeed("fear"), Is.GreaterThan(fear_before));
        }

        [Test]
        public void Dt_ClampsToASaneRange() {
            var loop = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            // A wild value from the dashboard must not run the engine off a cliff.
            loop.DeltaTime = 9999f;
            Assert.That(loop.DeltaTime, Is.LessThanOrEqualTo(MonitorLoop.DT_MAX));
            loop.DeltaTime = -3f;
            Assert.That(loop.DeltaTime, Is.GreaterThanOrEqualTo(MonitorLoop.DT_MIN));
            loop.DeltaTime = 0f;
            Assert.That(loop.DeltaTime, Is.GreaterThanOrEqualTo(MonitorLoop.DT_MIN));
        }

        [Test]
        public void Step_MovesExactlyOneFrameWhenNotPaused() {
            var loop = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            // Step is an explicit one-frame move; running it while not paused
            // advances one frame, the same as a tick, not two.
            var by_step = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            var by_tick = new MonitorLoop(MakeEngine(), delta_time: 0.5f);
            by_step.Step();
            by_tick.Tick();
            Assert.That(by_step.Engine.GetBaseNeed("hunger"),
                Is.EqualTo(by_tick.Engine.GetBaseNeed("hunger")).Within(0.0001f));
        }
    }
}
