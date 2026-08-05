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
    /// StepInReader turns a step-in message from the dashboard (a small JSON
    /// text) into an action on the loop. The socket layer only passes the text
    /// through, so this reader is where the step-in is understood, and it can be
    /// tested on its own without any socket.
    /// </summary>
    [TestFixture]
    public class StepInReaderTests {

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
        public void Read_AffectQueuesTheStepIn() {
            var loop = MakeLoop();
            loop.Tick();
            var fear_before = loop.Engine.GetBaseNeed("fear");
            StepInReader.Read(loop, "{\"kind\":\"affect\",\"need\":\"fear\",\"delta\":50}");
            // Queued, not yet applied.
            Assert.That(loop.Engine.GetBaseNeed("fear"), Is.EqualTo(fear_before));
            loop.Tick();
            Assert.That(loop.Engine.GetBaseNeed("fear"), Is.GreaterThan(fear_before));
        }

        [Test]
        public void Read_PausePausesTheLoop() {
            var loop = MakeLoop();
            StepInReader.Read(loop, "{\"kind\":\"pause\"}");
            Assert.That(loop.IsPaused, Is.True);
        }

        [Test]
        public void Read_ResumeResumesTheLoop() {
            var loop = MakeLoop();
            loop.Pause();
            StepInReader.Read(loop, "{\"kind\":\"resume\"}");
            Assert.That(loop.IsPaused, Is.False);
        }

        [Test]
        public void Read_ChangeDtSetsTheStep() {
            var loop = MakeLoop();
            StepInReader.Read(loop, "{\"kind\":\"delta_time\",\"delta_time\":0.1}");
            Assert.That(loop.DeltaTime, Is.EqualTo(0.1f).Within(0.0001f));
        }

        [Test]
        public void Read_LockQueuesALock() {
            var loop = MakeLoop();
            loop.Tick();
            StepInReader.Read(loop, "{\"kind\":\"lock\",\"duration\":2,\"mode\":\"hard\"}");
            loop.Tick();
            Assert.That(loop.Engine.IsLocked, Is.True);
        }

        [Test]
        public void Read_LockDefaultsToHardWhenNoMode() {
            var loop = MakeLoop();
            loop.Tick();
            StepInReader.Read(loop, "{\"kind\":\"lock\",\"duration\":2}");
            loop.Tick();
            Assert.That(loop.Engine.IsLocked, Is.True);
        }

        [Test]
        public void Read_UnlockQueuesAnUnlock() {
            var loop = MakeLoop();
            loop.Tick();
            loop.QueueLock(5f);
            loop.Tick();
            StepInReader.Read(loop, "{\"kind\":\"unlock\"}");
            loop.Tick();
            Assert.That(loop.Engine.IsLocked, Is.False);
        }

        [Test]
        public void Read_StepMovesOneFrameWhilePaused() {
            var loop = MakeLoop();
            loop.Pause();
            var hunger_before = loop.Engine.GetBaseNeed("hunger");
            StepInReader.Read(loop, "{\"kind\":\"step\"}");
            Assert.That(loop.Engine.GetBaseNeed("hunger"), Is.Not.EqualTo(hunger_before));
            Assert.That(loop.IsPaused, Is.True);
        }

        [Test]
        public void Read_UnknownKindIsIgnored() {
            var loop = MakeLoop();
            Assert.DoesNotThrow(() => StepInReader.Read(loop, "{\"kind\":\"nonsense\"}"));
            Assert.That(loop.IsPaused, Is.False);
        }

        [Test]
        public void Read_BadTextIsIgnored() {
            var loop = MakeLoop();
            Assert.DoesNotThrow(() => StepInReader.Read(loop, "not json at all"));
        }

        // ── MonitorSet (Stage 3, many agents) ──────────────────────────────

        static MonitorSet MakeSet() {
            var set = new MonitorSet(delta_time: 0.5f);
            foreach (var id in new[] { "scout_1", "scout_2" }) {
                var persona = new Persona {
                    agent_id = id,
                    needs = NeedsOf(("hunger", 30f), ("fear", 10f), ("idle", 70f)),
                    rates = RatesOf(("hunger", +0.5f)),
                    actions = new List<Animo.Model.Action> {
                        ActionOf("Eat", "hunger", 1, 1.0f),
                        ActionOf("Flee", "fear", 1, 1.0f),
                    },
                };
                set.Add(id, new Animo.Core.Engine(persona));
            }
            return set;
        }

        [Test]
        public void ReadSet_WatchChangesTheWatchedAgent() {
            var set = MakeSet();
            StepInReader.Read(set, "{\"kind\":\"watch\",\"agent\":\"scout_2\"}");
            Assert.That(set.Watched, Is.EqualTo("scout_2"));
        }

        [Test]
        public void ReadSet_StepInGoesToTheWatchedAgent() {
            var set = MakeSet();
            set.TickAll();
            set.Watch("scout_2");
            var fear_2_before = set.Loop("scout_2").Engine.GetBaseNeed("fear");
            StepInReader.Read(set, "{\"kind\":\"affect\",\"need\":\"fear\",\"delta\":50}");
            set.TickAll();
            Assert.That(set.Loop("scout_2").Engine.GetBaseNeed("fear"), Is.GreaterThan(fear_2_before));
        }

        [Test]
        public void ReadSet_BadTextIsIgnored() {
            var set = MakeSet();
            Assert.DoesNotThrow(() => StepInReader.Read(set, "not json"));
        }
    }
}
