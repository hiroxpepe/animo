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
    /// Decision-table tests for Q-S10: the `_force_reset_pending` latch must
    /// survive a Lock window. Spec §9.7.2 + §24.4.2 — the post-Step-4 clear
    /// is gated on `!is_locked`, so an emergency `Affect(force_reset: true)`
    /// raised mid-lock is honored by the first post-unlock Step 5 (no
    /// commitment cushion on the pre-lock behavior).
    ///
    /// These tests pin the *observable* contract: after Lock + emergency
    /// Affect + Unlock, the engine switches behavior on the next Live as if
    /// commitment.bonus had been zeroed for that frame. Direct observation
    /// of `_force_reset_pending` requires Phase 3's MockBus injection.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class LockForceResetCarryoverTests {

        Engine MakeEngine() {
            // A persona where, with full commitment cushion, the locked
            // "Talk" behavior would beat "Flee" at moderate fear; without
            // the cushion (latch consumed after unlock), Flee wins.
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

        [Test] public void Case01_ForceReset_DuringHardLock_LatchSurvivesUntilUnlock() {
            // Pre: engine in Hard Lock on "Talk". Player suddenly attacks.
            // During lock, an emergency Affect raises fear and force_reset.
            // Step 4 runs inside the lock and (per Q-S10) must NOT clear the
            // latch. Post-unlock Step 5 evaluates without commitment cushion.
            //
            // Observable: behavior switches to "Flee" on the first post-unlock
            // Live, even though commitment.bonus = 30 would normally keep
            // "Talk" winning.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);                // settle initial state
            e.Lock(duration: 1.0f, mode: LockMode.Hard);
            Assert.That(e.IsLocked, Is.True, "precondition: hard locked");

            // Mid-lock emergency stimulus
            e.Affect(need: "fear", delta: +20f, force_reset: true);
            e.Live(dt: 0.5f);                  // Step 4 runs in lock; latch must stay set
            Assert.That(e.IsLocked, Is.True, "still locked half-way through");

            e.Live(dt: 0.6f);                  // lock timer expires inside this Live
            // Post-unlock first Step 5: latch consumed → no commitment.bonus
            // on previous behavior → fear's score wins → behavior == "Flee".
            Assert.That(e.IsLocked, Is.False, "lock should have expired");
            Assert.That(e.Behavior, Is.EqualTo(expected: "Flee"),
                "Q-S10: force_reset latch must survive Hard Lock and be honored on first post-unlock Step 5");
        }

        [Test] public void Case02_ForceReset_DuringSoftLock_LatchSurvivesUntilUnlock() {
            // Same shape as Case01 but with Soft Lock. The latch lifecycle
            // contract is identical (§24.4 table: latch behavior column).
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            e.Lock(duration: 1.0f, mode: LockMode.Soft);
            Assert.That(e.IsLocked, Is.True, "precondition: soft locked");

            e.Affect(need: "fear", delta: +20f, force_reset: true);
            e.Live(dt: 0.5f);
            Assert.That(e.IsLocked, Is.True);

            e.Live(dt: 0.6f);
            Assert.That(e.IsLocked, Is.False);
            Assert.That(e.Behavior, Is.EqualTo(expected: "Flee"),
                "Q-S10: force_reset latch must survive Soft Lock and be honored on first post-unlock Step 5");
        }
    }
}
