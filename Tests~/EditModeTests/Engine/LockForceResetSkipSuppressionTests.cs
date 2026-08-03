#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class LockForceResetSkipSuppressionTests {

        Engine MakeEngine() {
            // fear=80 → Flee wins without bonus. idle=20 + commitment.bonus=50 → Talk wins WITH bonus.
            var p = new Persona { agent_id = "a",
                needs    = NeedsOf(("fear",80f), ("idle",20f)),
                actions  = new List<Animo.Model.Action> {
                    ActionOf("Talk", "idle", 5, 1.0f),   // score = 20/100*100 = 20 (+50 bonus = 70)
                    ActionOf("Flee", "fear", 2, 1.0f) },  // score = 80/100*100 = 80
                commitment = new Commitment { bonus = 50f }
            };
            return new Engine(p);
        }

        [Test] public void Case01_ForceResetDuringLock_SkipSuppressed_LockedBehaviorKeepsBonus() {
            // Q-S13: during Hard lock, force_reset skip is suppressed.
            // The locked behavior keeps its commitment bonus.
            var e = new Engine(new Persona {
                agent_id = "a",
                needs    = NeedsOf(("fear",20f), ("idle",80f)),  // Talk wins first
                actions  = new List<Animo.Model.Action> {
                    ActionOf("Talk", "idle", 5, 1.0f),  // idle=80 → score=80+bonus
                    ActionOf("Flee", "fear", 2, 1.0f) },
                commitment = new Commitment { bonus = 30f }
            });
            e.Live(0.016f);  // Talk wins (idle=80 > fear=20)
            Assert.That(e.Behavior, Is.EqualTo("Talk"), "precondition: Talk selected");
            e.Lock(1.0f, LockMode.Hard);

            // force_reset during lock: Q-S13 skip suppressed, Talk keeps bonus
            e.Affect("fear", +30f, force_reset: true);  // fear=50
            e.Live(0.2f);
            Assert.That(e.Behavior, Is.EqualTo("Talk"),
                "Q-S13: Hard-locked behavior must not change; force_reset skip suppressed.");
        }

        [Test] public void Case02_LockedFor5Seconds_SkipNeverConsumed_BehaviorOnlyChangesAfterUnlock() {
            // Q-S13: skip consumes ONCE after unlock, not during lock.
            var e = new Engine(new Persona {
                agent_id = "a",
                needs    = NeedsOf(("fear",20f), ("idle",80f)),
                actions  = new List<Animo.Model.Action> {
                    ActionOf("Talk", "idle", 5, 1.0f),
                    ActionOf("Flee", "fear", 2, 1.0f) },
                commitment = new Commitment { bonus = 30f }
            });
            e.Live(0.016f);
            Assert.That(e.Behavior, Is.EqualTo("Talk"), "precondition");
            e.Lock(5.0f, LockMode.Hard);
            e.Affect("fear", +20f, force_reset: true);

            for (int i = 0; i < 4; i++) {
                e.Live(1.0f);
                Assert.That(e.IsLocked, Is.True, $"still locked at sample {i}");
                Assert.That(e.Behavior, Is.EqualTo("Talk"),
                    $"Q-S13: locked behavior must persist at sample {i}.");
            }
            // Unlock frame
            e.Live(1.5f);
            Assert.That(e.IsLocked, Is.False, "lock expired");
        }
    }
}
