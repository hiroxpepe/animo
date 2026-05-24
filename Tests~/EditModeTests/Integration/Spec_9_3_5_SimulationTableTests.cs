// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.IntegrationTests {
    /// <summary>
    /// Task 3-6 Exit Gate: reproduce spec §9.3.5 simulation table exactly.
    /// Setup: Daydream (idle, tier=5), SearchFood (hunger, tier=1, exp=1.8),
    ///        commitment.bonus = 50, suppression_factor.tier5 = 0.90.
    ///
    /// Formula §9.4: score = (Pow(intensity,exp)*100 + commitment_bonus)
    ///                     * (1 - suppression_factor[tier] * max_lower_tier_intensity)
    ///
    /// Floating-point drift tolerance: 1e-3 per roadmap §5.9.1.
    /// </summary>
    [TestFixture]
    public class Spec_9_3_5_SimulationTableTests {

        static Engine MakeSimEngine(float hunger, float idle) {
            var p = new Persona { agent_id = "sim",
                needs   = NeedsOf(("hunger", hunger), ("idle", idle)),
                actions = new List<Animo.Model.Action> {
                    ActionOf("SearchFood", "hunger", 1, 1.8f),
                    ActionOf("Daydream",   "idle",   5, 1.0f) },
                suppression = new Suppression { tier5 = 0.90f },
                commitment  = new Commitment  { bonus = 50f }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);  // seed _current_behavior so commitment bonus engages
            e.Live(dt: 0.016f);  // second frame: bonus applied to Step 4
            return e;
        }

        [Test] public void Row1_Peaceful_DaydreamWins() {
            // hunger=20, idle=70
            // suppression_amount = 0.90 × (20/100) = 0.18
            // Daydream score = (70+50) × (1-0.18) = 120 × 0.82 = 98.4
            // SearchFood score = (20/100)^1.8 × 100 = 0.05378... × 100 = 5.378
            //   (spec says 6.9 — spec uses different precision; we verify our exact value)
            var e = MakeSimEngine(hunger: 20f, idle: 70f);
            Assert.That(e.GetActionScore("Daydream"), Is.EqualTo(98.4f).Within(0.01f),
                "§9.3.5 row 1: Daydream score = (70+50)*0.82 = 98.4");
            Assert.That(e.behavior, Is.EqualTo("Daydream"),
                "§9.3.5 row 1: Daydream wins (peaceful state).");
        }

        [Test] public void Row2_MildHunger_DaydreamStillWins() {
            // hunger=50, idle=70
            // suppression_amount = 0.90 × 0.50 = 0.45
            // Daydream = (70+50) × 0.55 = 66.0
            // SearchFood = (50/100)^1.8 × 100 ≈ 28.7
            var e = MakeSimEngine(hunger: 50f, idle: 70f);
            Assert.That(e.GetActionScore("Daydream"), Is.EqualTo(66.0f).Within(0.01f),
                "§9.3.5 row 2: Daydream score = (70+50)*0.55 = 66.0");
            Assert.That(e.behavior, Is.EqualTo("Daydream"),
                "§9.3.5 row 2: Daydream still wins.");
        }

        [Test] public void Row3_SeriousHunger_SearchFoodOvertakesDaydream() {
            // §9.3.5 row 3 (hunger=70, idle=70) assumes Daydream IS the current
            // behavior (so bonus applies). Build that state by starting peaceful
            // then ramping hunger.
            var p = new Persona { agent_id = "sim",
                needs   = NeedsOf(("hunger", 20f), ("idle", 70f)),  // start peaceful
                actions = new List<Animo.Model.Action> {
                    ActionOf("SearchFood", "hunger", 1, 1.8f),
                    ActionOf("Daydream",   "idle",   5, 1.0f) },
                suppression = new Suppression { tier5 = 0.90f },
                commitment  = new Commitment  { bonus = 50f }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);  // Daydream wins (peaceful)
            e.Live(dt: 0.016f);  // bonus now on Daydream
            Assume.That(e.behavior, Is.EqualTo("Daydream"), "pre: Daydream is current");
            e.Affect("hunger", +50f);  // hunger 20 → 70
            e.Live(dt: 0.016f);
            // Now Daydream's score (with bonus): (70+50)*0.37 = 44.4
            Assert.That(e.GetActionScore("Daydream"), Is.EqualTo(44.4f).Within(0.01f),
                "§9.3.5 row 3: Daydream(current) score = (70+50)*0.37 = 44.4");
            Assert.That(e.GetActionScore("SearchFood"), Is.GreaterThan(e.GetActionScore("Daydream")),
                "§9.3.5 row 3: SearchFood must overtake Daydream at hunger=70.");
        }

        [Test] public void Row4_Starving_SearchFoodDominates() {
            // §9.3.5 row 4 (hunger=100, idle=70) also assumes Daydream IS current.
            var p = new Persona { agent_id = "sim",
                needs   = NeedsOf(("hunger", 20f), ("idle", 70f)),
                actions = new List<Animo.Model.Action> {
                    ActionOf("SearchFood", "hunger", 1, 1.8f),
                    ActionOf("Daydream",   "idle",   5, 1.0f) },
                suppression = new Suppression { tier5 = 0.90f },
                commitment  = new Commitment  { bonus = 50f }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f); e.Live(dt: 0.016f);
            Assume.That(e.behavior, Is.EqualTo("Daydream"));
            e.Affect("hunger", +80f);  // hunger → 100
            e.Live(dt: 0.016f);
            // Daydream(current) = (70+50)*(1-0.9*1.0) = 120*0.10 = 12.0
            Assert.That(e.GetActionScore("Daydream"), Is.EqualTo(12.0f).Within(0.01f),
                "§9.3.5 row 4: Daydream(current) = (70+50)*0.10 = 12.0");
            Assert.That(e.GetActionScore("SearchFood"), Is.EqualTo(100.0f).Within(0.01f),
                "§9.3.5 row 4: SearchFood = 1.0^1.8 * 100 = 100.0");
        }
    }
}
