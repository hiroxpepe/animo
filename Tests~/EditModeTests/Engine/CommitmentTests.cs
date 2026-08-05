// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Step 4: commitment.bonus — numerical verification (spec §9.4, Q-S1).</summary>
    [TestFixture]
    public class CommitmentTests {

        [Test] public void Case01_CommitmentBonusHoldsCurrentActionOnTie() {
            // fear=50 (Flee exp=1.0 → score=50), idle=50 (Idle exp=1.0 → score=50)
            // Tie → declaration order: Flee wins first. Next frame: bonus=20 keeps Flee.
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("fear",50f),("idle",50f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Flee","fear",2,1.0f), ActionOf("Idle","idle",5,1.0f) },
                commitment = new Commitment { bonus = 20f }
            };
            var e = new Engine(p);
            e.Live(delta_time: 0.016f);   // Flee wins tie by declaration order
            string first = e.Behavior;
            e.Live(delta_time: 0.016f);   // bonus=20 on Flee: 70 vs 50 → Flee stays
            Assert.That(e.Behavior, Is.EqualTo(first),
                "Commitment: bonus must hold the current action against a tie.");
        }

        [Test] public void Case02_CommitmentBonusValue_AddedToCurrentAction() {
            // fear=60 (Flee), idle=50 (Idle), bonus=20
            // Frame 1: _current="" → no bonus → Flee(60)>Idle(50) → Flee wins
            // Frame 2: _current="Flee" → Flee gets bonus: 60+20=80 (verified via GetActionScore)
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("fear",60f),("idle",50f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Flee","fear",2,1.0f), ActionOf("Idle","idle",5,1.0f) },
                commitment = new Commitment { bonus = 20f }
            };
            var e = new Engine(p);
            e.Live(delta_time: 0.016f);  // seeds behavior="Flee"
            Assert.That(e.Behavior, Is.EqualTo("Flee"), "pre: Flee wins");
            e.Live(delta_time: 0.016f);  // now _current="Flee" → Step4 adds bonus
            float flee_score = e.GetActionScore("Flee");
            Assert.That(flee_score, Is.EqualTo(80f).Within(0.1f),
                "Commitment: on 2nd frame, score of current action must include bonus (60+20=80).");
        }

        [Test] public void Case03_ForceReset_RemovesBonus_ForOneFrame() {
            // Q-S5: force_reset=true skips bonus for that frame
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("fear",60f),("idle",65f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Flee","fear",2,1.0f), ActionOf("Idle","idle",5,1.0f) },
                commitment = new Commitment { bonus = 20f }
            };
            var e = new Engine(p);
            e.Live(delta_time: 0.016f);  // Idle wins (65 vs 60)
            Assert.That(e.Behavior, Is.EqualTo("Idle"), "pre");
            // force_reset: Idle bonus removed → Idle=65, Flee=60 → still Idle unless bonus pushed it
            e.Affect("fear", +10f, force_reset: true);  // fear=70, Idle has no bonus → Flee(70)>Idle(65)
            e.Live(delta_time: 0.016f);
            Assert.That(e.Behavior, Is.EqualTo("Flee"),
                "Q-S5: force_reset must remove commitment bonus, allowing switch.");
        }

        [Test] public void Case04_BonusDoesNotDecayOverTime() {
            // spec: commitment.bonus is constant (no decay)
            // After 2nd frame, bonus is applied → score stabilizes at 80. Must stay at 80 forever.
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("fear",60f),("idle",50f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Flee","fear",2,1.0f), ActionOf("Idle","idle",5,1.0f) },
                commitment = new Commitment { bonus = 20f }
            };
            var e = new Engine(p);
            e.Live(delta_time: 0.016f);  // seeds behavior
            e.Live(delta_time: 0.016f);  // bonus now applied
            float score_first = e.GetActionScore("Flee");  // 80
            for (int i = 0; i < 100; i++) e.Live(delta_time: 0.016f);
            float score_after = e.GetActionScore("Flee");
            Assert.That(score_after, Is.EqualTo(score_first).Within(0.01f),
                "Commitment: bonus must not decay over time (stable at 80).");
        }
    }
}
