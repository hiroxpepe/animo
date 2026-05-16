// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table tests for Q-S13: while locked, the commitment-bonus
    /// SKIP must also be suppressed (not just the latch clear). Phase_2_4_6's
    /// Q-S10 layout had `LockGate` downstream of `Skip`, so the skip ran
    /// every locked frame — turning a one-frame interrupt into a multi-frame
    /// debuff. Q-S13 moves `LockGate` upstream of `Skip`: while locked,
    /// neither the skip nor the clear runs; the latch survives untouched
    /// and is honored by the first post-unlock Step 4.
    ///
    /// These tests pin the *observable* contract:
    ///   - During Lock, locked_behavior continues to receive its normal
    ///     commitment.bonus even after Affect(force_reset: true) was raised.
    ///   - Only on the first post-unlock frame does the skip happen — once.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class LockForceResetSkipSuppressionTests {

        Engine MakeEngine() {
            // Two-action persona where, with full commitment.bonus on Talk,
            // Talk would beat Flee at moderate fear. Without bonus → Flee wins.
            Persona p = new Persona {
                agent_id = "npc_a",
                needs    = NeedsOf(("fear", 60f), ("idle", 30f)),
                actions  = new List<Animo.Model.Action> {
                    ActionOf(id: "Talk", need: "idle", tier: 5, exponent: 1.0f),
                    ActionOf(id: "Flee", need: "fear", tier: 2, exponent: 2.0f)
                },
                commitment = new Commitment { bonus = 30f }
            };
            return new Engine(persona: p);
        }

        [Test] public void Case01_ForceResetDuringLock_SkipSuppressed_LockedBehaviorKeepsBonus() {
            // Pre: Soft-locked on Talk. Player attacks → Affect(force_reset).
            // While Step 4 still runs in the lock (Q-S2), Q-S13 says the
            // commitment_bonus skip MUST NOT apply. Talk keeps its full
            // cushion through the locked frames; only post-unlock Step 4
            // applies the skip exactly once. Step 5 is locked → behavior
            // never changes mid-lock.
            //
            // Observable: behavior == "Talk" while locked, regardless of
            // force_reset latch state.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            e.Lock(duration: 1.0f, mode: LockMode.Soft);
            Assert.That(e.is_locked, Is.True, "precondition: soft locked");

            e.Affect(need: "fear", delta: +10f, force_reset: true);
            e.Live(dt: 0.2f);
            Assert.That(e.is_locked, Is.True, "still locked");
            Assert.That(e.behavior, Is.EqualTo(expected: "Talk"),
                "Q-S13: locked_behavior must keep its commitment cushion mid-lock; skip is suppressed");

            e.Live(dt: 0.2f);
            Assert.That(e.behavior, Is.EqualTo(expected: "Talk"),
                "Q-S13: still locked, still no skip applied");
        }

        [Test] public void Case02_LockedFor5Seconds_SkipNeverConsumed_BehaviorOnlyChangesAfterUnlock() {
            // Pre: 5-second Soft Lock at simulated 60 fps. With
            // Phase_2_4_6's flawed layout, the skip would run on every one
            // of ~300 frames inside the lock — a multi-frame debuff. Q-S13
            // requires that skip happens at most ONCE, and only after
            // unlock. We sample several mid-lock frames; behavior must stay
            // on the locked action throughout.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            e.Lock(duration: 5.0f, mode: LockMode.Soft);

            e.Affect(need: "fear", delta: +20f, force_reset: true);

            // Sample 4 mid-lock frames at 1-second intervals
            for (int i = 0; i < 4; i++) {
                e.Live(dt: 1.0f);
                Assert.That(e.is_locked, Is.True, $"still locked at sample {i}");
                Assert.That(e.behavior, Is.EqualTo(expected: "Talk"),
                    $"Q-S13: at sample {i}, locked_behavior must NOT have lost its cushion. " +
                    "Phase_2_4_6's flaw was running skip every locked frame.");
            }

            // Unlock + first post-unlock frame: skip + clear consumed exactly once
            e.Live(dt: 1.5f);
            Assert.That(e.is_locked, Is.False, "lock should have expired by now");
            Assert.That(e.behavior, Is.EqualTo(expected: "Flee"),
                "Q-S13: post-unlock first Step 5 honors latch — exactly one frame of skip");
        }
    }
}
