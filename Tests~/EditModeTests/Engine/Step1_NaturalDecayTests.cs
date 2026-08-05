// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Step 1: natural decay — numerical value verification (spec §9.2).</summary>
    [TestFixture]
    public class Step1_NaturalDecayTests {

        Engine MakeEngine(float hunger_init, float hunger_rate) {
            Persona p = new Persona {
                agent_id = "a",
                needs  = NeedsOf(("hunger", hunger_init), ("idle", 50f)),
                rates  = RatesOf(("hunger", hunger_rate)),
                actions = new List<Action> { ActionOf("Idle", "idle", 5) }
            };
            return new Engine(p);
        }

        [Test] public void Case01_PositiveRateRaisesNeed() {
            // hunger_init=50, rate=+10, delta_time=1.0 → expected=60
            Engine e = MakeEngine(hunger_init: 50f, hunger_rate: +10f);
            e.Live(delta_time: 1.0f);
            Assert.That(e.GetBaseNeed("hunger"), Is.EqualTo(60f).Within(0.01f),
                "Step 1: positive rate must raise need by rate*delta_time per frame.");
        }

        [Test] public void Case02_NegativeRateLowersNeed() {
            // hunger_init=50, rate=-10, delta_time=1.0 → expected=40
            Engine e = MakeEngine(hunger_init: 50f, hunger_rate: -10f);
            e.Live(delta_time: 1.0f);
            Assert.That(e.GetBaseNeed("hunger"), Is.EqualTo(40f).Within(0.01f),
                "Step 1: negative rate must lower need by |rate|*delta_time per frame.");
        }

        [Test] public void Case03_RateZero_NoChange() {
            Engine e = MakeEngine(hunger_init: 50f, hunger_rate: 0f);
            e.Live(delta_time: 1.0f);
            Assert.That(e.GetBaseNeed("hunger"), Is.EqualTo(50f).Within(0.001f),
                "Step 1: zero rate must not change need.");
        }

        [Test] public void Case04_ClampUpperBoundAt100() {
            // hunger_init=99, rate=+100, delta_time=1.0 → would be 199, clamps to 100
            Engine e = MakeEngine(hunger_init: 99f, hunger_rate: +100f);
            e.Live(delta_time: 1.0f);
            Assert.That(e.GetBaseNeed("hunger"), Is.EqualTo(100f).Within(0.001f),
                "Step 1: need must clamp to 100.");
        }

        [Test] public void Case05_ClampLowerBoundAtZero() {
            // hunger_init=1, rate=-100, delta_time=1.0 → would be -99, clamps to 0
            Engine e = MakeEngine(hunger_init: 1f, hunger_rate: -100f);
            e.Live(delta_time: 1.0f);
            Assert.That(e.GetBaseNeed("hunger"), Is.EqualTo(0f).Within(0.001f),
                "Step 1: need must clamp to 0.");
        }

        [Test] public void Case06_DtScalesDecay() {
            // hunger_init=50, rate=+10, delta_time=0.5 → expected=55
            Engine e = MakeEngine(hunger_init: 50f, hunger_rate: +10f);
            e.Live(delta_time: 0.5f);
            Assert.That(e.GetBaseNeed("hunger"), Is.EqualTo(55f).Within(0.01f),
                "Step 1: decay must scale by delta_time (rate*delta_time applied per frame).");
        }
    }
}
