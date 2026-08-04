// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using Animo.Tools;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.Tools {

    /// <summary>
    /// MonitorSet holds many live agents at once. Each tick it advances every
    /// agent, and the dashboard picks one agent to watch and to step into. A
    /// step-in names its agent, so a poke lands on the right one only. This is
    /// the Stage 3 "many agents" piece, and it is tested without any socket.
    /// </summary>
    [TestFixture]
    public class MonitorSetTests {

        static Animo.Core.Engine MakeEngine(string id) {
            var persona = new Persona {
                agent_id = id,
                needs = NeedsOf(("hunger", 30f), ("fear", 10f), ("idle", 70f)),
                rates = RatesOf(("hunger", +0.5f)),
                actions = new List<Animo.Model.Action> {
                    ActionOf("Eat", "hunger", 1, 1.0f),
                    ActionOf("Flee", "fear", 1, 1.0f),
                },
            };
            return new Animo.Core.Engine(persona);
        }

        static MonitorSet MakeSet() {
            var set = new MonitorSet(dt: 0.5f);
            set.Add("scout_1", MakeEngine("scout_1"));
            set.Add("scout_2", MakeEngine("scout_2"));
            return set;
        }

        [Test]
        public void Ids_ListsEveryAgent() {
            var set = MakeSet();
            Assert.That(set.Ids, Is.EquivalentTo(new[] { "scout_1", "scout_2" }));
        }

        [Test]
        public void TickAll_AdvancesEveryAgent() {
            var set = MakeSet();
            var before_1 = set.Loop("scout_1").Engine.GetBaseNeed("hunger");
            var before_2 = set.Loop("scout_2").Engine.GetBaseNeed("hunger");
            var snaps = set.TickAll();
            Assert.That(set.Loop("scout_1").Engine.GetBaseNeed("hunger"), Is.Not.EqualTo(before_1));
            Assert.That(set.Loop("scout_2").Engine.GetBaseNeed("hunger"), Is.Not.EqualTo(before_2));
            Assert.That(snaps.Keys, Is.EquivalentTo(new[] { "scout_1", "scout_2" }));
        }

        [Test]
        public void StepIn_LandsOnTheNamedAgentOnly() {
            var set = MakeSet();
            set.TickAll();
            var fear_1_before = set.Loop("scout_1").Engine.GetBaseNeed("fear");
            var fear_2_before = set.Loop("scout_2").Engine.GetBaseNeed("fear");
            set.Loop("scout_1").QueueAffect("fear", 50f);
            set.TickAll();
            Assert.That(set.Loop("scout_1").Engine.GetBaseNeed("fear"), Is.GreaterThan(fear_1_before));
            // scout_2 only felt its own decay, not the poke meant for scout_1.
            Assert.That(set.Loop("scout_2").Engine.GetBaseNeed("fear"),
                Is.LessThanOrEqualTo(fear_2_before + 1f));
        }

        [Test]
        public void Watched_DefaultsToTheFirstAgent() {
            var set = MakeSet();
            Assert.That(set.Watched, Is.EqualTo("scout_1"));
        }

        [Test]
        public void Watch_ChangesTheWatchedAgent() {
            var set = MakeSet();
            set.Watch("scout_2");
            Assert.That(set.Watched, Is.EqualTo("scout_2"));
        }

        [Test]
        public void Watch_IgnoresAnUnknownAgent() {
            var set = MakeSet();
            set.Watch("nobody");
            Assert.That(set.Watched, Is.EqualTo("scout_1"));
        }

        [Test]
        public void Loop_ThrowsAKindErrorForAnUnknownAgent() {
            var set = MakeSet();
            Assert.That(() => set.Loop("nobody"),
                Throws.TypeOf<KeyNotFoundException>());
        }

        // ── Recording (Stage 3, record and play again) ─────────────────────

        [Test]
        public void TickAll_RecordsAFramePerAgent() {
            var set = MakeSet();
            set.TickAll();
            set.TickAll();
            Assert.That(set.Recording("scout_1").Count, Is.EqualTo(2));
            Assert.That(set.Recording("scout_2").Count, Is.EqualTo(2));
        }

        [Test]
        public void Recording_KeepsEachAgentSeparate() {
            var set = MakeSet();
            set.TickAll();
            set.Watch("scout_1");
            set.Loop("scout_1").QueueAffect("fear", 60f);
            set.TickAll();
            // scout_1's recorded fear rose from the poke; scout_2's did not.
            var last_1 = set.Recording("scout_1").Frame(1).base_needs["fear"];
            var last_2 = set.Recording("scout_2").Frame(1).base_needs["fear"];
            Assert.That(last_1, Is.GreaterThan(last_2));
        }

        [Test]
        public void Recording_ForAnUnknownAgentThrows() {
            var set = MakeSet();
            Assert.That(() => set.Recording("nobody"),
                Throws.TypeOf<KeyNotFoundException>());
        }
    }
}
