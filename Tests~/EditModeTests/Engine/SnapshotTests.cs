// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {

    /// <summary>
    /// Snapshot is the one call the live monitor reads each frame. It must return
    /// the whole visible state of the engine in one object: the chosen behavior,
    /// the lock state, every need as both a base and an effective value, and the
    /// action scores. These tests hold that shape.
    /// </summary>
    [TestFixture]
    public class SnapshotTests {

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
        public void Snapshot_CarriesTheChosenBehavior() {
            var engine = MakeEngine();
            engine.Live(0.5f);
            var snap = engine.Snapshot();
            Assert.That(snap.behavior, Is.EqualTo(engine.Behavior));
        }

        [Test]
        public void Snapshot_CarriesTheLockState() {
            var engine = MakeEngine();
            engine.Live(0.5f);
            engine.Lock(2f);
            var snap = engine.Snapshot();
            Assert.That(snap.is_locked, Is.True);
            Assert.That(snap.locked_behavior, Is.EqualTo(engine.LockedBehavior));
        }

        [Test]
        public void Snapshot_CarriesEveryNeedAsBaseAndEffective() {
            var engine = MakeEngine();
            engine.Live(0.5f);
            var snap = engine.Snapshot();
            // The engine keeps the full standard set of needs, so the snapshot
            // carries them all, each as a base and an effective value.
            Assert.That(snap.base_needs.Keys, Is.EquivalentTo(snap.effective_needs.Keys));
            Assert.That(snap.base_needs.ContainsKey("hunger"), Is.True);
            Assert.That(snap.base_needs.ContainsKey("fear"), Is.True);
            Assert.That(snap.base_needs["hunger"], Is.EqualTo(engine.GetBaseNeed("hunger")));
            Assert.That(snap.effective_needs["hunger"], Is.EqualTo(engine.GetNeed("hunger")));
        }

        [Test]
        public void Snapshot_CarriesAnActionScoreForEveryAction() {
            var engine = MakeEngine();
            engine.Live(0.5f);
            var snap = engine.Snapshot();
            Assert.That(snap.action_scores.Keys, Is.EquivalentTo(new[] { "Eat", "Flee", "Idle" }));
        }

        [Test]
        public void Snapshot_ReadsTheStateAtTheMomentItIsCalled() {
            var engine = MakeEngine();
            engine.Live(0.5f);
            var first = engine.Snapshot();
            engine.Affect("fear", 50f);
            engine.Live(0.5f);
            var second = engine.Snapshot();
            Assert.That(second.base_needs["fear"], Is.Not.EqualTo(first.base_needs["fear"]));
        }
    }
}
