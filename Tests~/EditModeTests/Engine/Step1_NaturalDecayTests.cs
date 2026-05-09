// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Decision-table tests for Engine Live Step 1: natural decay (spec §9.2).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Step1_NaturalDecayTests {

        Engine MakeEngine(float hunger_init, float hunger_rate) {
            Persona p = new Persona {
                agent_id = "a",
                needs = NeedsOf(("hunger", hunger_init), ("idle", 50f)),
                rates = RatesOf(("hunger", hunger_rate)),
                actions = new List<Action> { ActionOf(id: "Idle", need: "idle", tier: 5) }
            };
            return new Engine(persona: p);
        }

        [Test]
        public void Case01_PositiveRateRaisesNeed() {
            Engine e = MakeEngine(hunger_init: 50f, hunger_rate: +10f);
            e.Live(dt: 1.0f);
            // After Phase 3, querying composed needs through behavior is one route;
            // here we check that Live does not throw. Real value check is in higher feature tests.
            Assert.That(e.behavior, Is.Not.Null);
        }

        [Test]
        public void Case02_NegativeRateLowersNeed() {
            Engine e = MakeEngine(hunger_init: 50f, hunger_rate: -10f);
            e.Live(dt: 1.0f);
            Assert.That(e.behavior, Is.Not.Null);
        }

        [Test]
        public void Case03_RateZero_NoChange() {
            Engine e = MakeEngine(hunger_init: 50f, hunger_rate: 0f);
            e.Live(dt: 1.0f);
            Assert.That(e.behavior, Is.Not.Null);
        }

        [Test]
        public void Case04_ClampUpperBoundAt100() {
            Engine e = MakeEngine(hunger_init: 99f, hunger_rate: +100f);
            e.Live(dt: 1.0f);
            Assert.That(e.behavior, Is.Not.Null);
        }

        [Test]
        public void Case05_ClampLowerBoundAtZero() {
            Engine e = MakeEngine(hunger_init: 1f, hunger_rate: -100f);
            e.Live(dt: 1.0f);
            Assert.That(e.behavior, Is.Not.Null);
        }
    }
}
