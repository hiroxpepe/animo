// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Affect + force_reset — numerical verification (spec Q-S5/Q-S13).</summary>
    [TestFixture]
    public class ForceResetTests {

        [Test] public void Case01_AffectAddsToNeed() {
            var p = new Persona { agent_id="a", needs=NeedsOf(("fear",30f)),
                actions=new List<Action>{ ActionOf("Flee","fear",2) }};
            var e = new Engine(p);
            e.Affect("fear", +20f);
            Assert.That(e.GetBaseNeed("fear"), Is.EqualTo(50f).Within(0.001f),
                "Affect must add delta to need immediately.");
        }

        [Test] public void Case02_AffectClampsAt100() {
            var p = new Persona { agent_id="a", needs=NeedsOf(("fear",90f)),
                actions=new List<Action>{ ActionOf("Flee","fear",2) }};
            var e = new Engine(p);
            e.Affect("fear", +50f);
            Assert.That(e.GetBaseNeed("fear"), Is.EqualTo(100f).Within(0.001f),
                "Affect must clamp at 100.");
        }

        [Test] public void Case03_AffectClampsAt0() {
            var p = new Persona { agent_id="a", needs=NeedsOf(("fear",10f)),
                actions=new List<Action>{ ActionOf("Flee","fear",2) }};
            var e = new Engine(p);
            e.Affect("fear", -50f);
            Assert.That(e.GetBaseNeed("fear"), Is.EqualTo(0f).Within(0.001f),
                "Affect must clamp at 0.");
        }

        [Test] public void Case04_ForceResetTrue_SkipsCommitmentBonus_ForOneFrame() {
            // Q-S5: force_reset strips commitment bonus for one frame.
            // Scenario:
            //   Flee(fear=70) wins first frame (no bonus yet).
            //   force_reset=true + Affect(idle,+40) → idle=75, fear=70
            //   On next frame: Flee has NO bonus (force_reset consumed) → Flee=70 vs Idle=75 → Idle wins.
            var p = new Persona { agent_id="a",
                needs   = NeedsOf(("idle",35f),("fear",70f)),
                actions = new List<Action>{
                    ActionOf("Flee","fear",2,1.0f), ActionOf("Idle","idle",5,1.0f) },
                commitment = new Commitment { bonus = 30f }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);  // Flee(70) > Idle(35+bonus=65) → Flee wins
            Assert.That(e.Behavior, Is.EqualTo("Flee"), "pre: Flee wins");
            // force_reset + raise Idle above Flee: Flee without bonus=70, Idle=75 → Idle wins
            e.Affect("idle", +40f, force_reset: true);  // idle=75
            e.Live(dt: 0.016f);  // Flee has no bonus: 70 vs Idle: 75 → Idle wins
            Assert.That(e.Behavior, Is.EqualTo("Idle"),
                "Q-S5: force_reset must strip commitment bonus; Idle(75) beats Flee(70) without bonus.");
        }

        [Test] public void Case05_ForceResetLatchSurvivesLock_HardMode() {
            // Q-S13: force_reset latch survives Hard Lock
            var p = new Persona { agent_id="a",
                needs   = NeedsOf(("idle",80f),("fear",30f)),
                actions = new List<Action>{
                    ActionOf("Idle","idle",5,1.0f), ActionOf("Flee","fear",2,1.0f) },
                commitment = new Commitment { bonus = 30f }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);  // Idle wins
            Assert.That(e.Behavior, Is.EqualTo("Idle"), "pre");
            e.Lock(2.0f, LockMode.Hard);
            e.Affect("fear", +60f, force_reset: true);  // fear=90, latch set
            e.Live(dt: 0.016f);  // Hard lock: behavior frozen
            Assert.That(e.Behavior, Is.EqualTo("Idle"), "Hard lock: behavior frozen");
            e.Live(dt: 2.1f);  // unlock
            Assert.That(e.IsLocked, Is.False, "unlocked");
            // Post-unlock: force_reset survived → Idle has no bonus: 80 vs Flee: 90 → Flee wins
            Assert.That(e.Behavior, Is.EqualTo("Flee"),
                "Q-S13: force_reset latch must survive Hard Lock and be honored on unlock.");
        }
    }
}
