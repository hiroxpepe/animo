// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table tests for Engine.Lock / Unlock edge cases (v0.1.5,
    /// Q9, Q10, Q14, Q15). See spec §24.5.1.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class LockEdgeCaseTests {

        Engine MakeEngine() {
            Persona p = new Persona {
                agent_id = "a",
                needs = NeedsOf(("fear", 50f), ("idle", 30f)),
                actions = new List<Animo.Model.Action> {
                    ActionOf(id: "Flee", need: "fear", tier: 2, exponent: 2.5f),
                    ActionOf(id: "Idle", need: "idle", tier: 5, exponent: 1.0f)
                }
            };
            return new Engine(persona: p);
        }

        [Test] public void Case01_LockZeroDuration_ReleasesExistingLock() {
            // Q9: Lock(0) is treated as immediate Unlock. The meaningful test is
            // the state transition from a real lock — not from is_locked=false,
            // which would pass for a buggy impl that just early-returns on dt=0.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            e.Lock(duration: 5f);
            Assert.That(e.is_locked, Is.True, "precondition: engine should be locked");
            e.Lock(duration: 0f);
            Assert.That(e.is_locked, Is.False, "Lock(0) must release the existing lock");
        }

        [Test] public void Case01b_LockZeroDuration_FromUnlockedStaysUnlocked() {
            // Companion check: Lock(0) from an unlocked state stays unlocked.
            // Together with Case01, this pins down the full Lock(0) contract.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            Assert.That(e.is_locked, Is.False, "precondition: not locked");
            e.Lock(duration: 0f);
            Assert.That(e.is_locked, Is.False);
        }

        [Test] public void Case02_LockNegativeDuration_ThrowsArgumentException() {
            // Q10: Negative duration is meaningless.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            Assert.Throws<ArgumentException>(code: () => e.Lock(duration: -1f));
        }

        [Test] public void Case03_LockWhileLocked_ReplacesDuration() {
            // Q14: Re-Lock replaces; new duration overwrites remaining.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            e.Lock(duration: 10f);
            e.Lock(duration: 0.5f);
            // After 0.6s, the second (shorter) duration must have expired.
            e.Live(dt: 0.6f);
            Assert.That(e.is_locked, Is.False);
        }

        [Test] public void Case04_LockWhileLocked_ReplacesDurationToShorter() {
            // Q14: Re-Lock with a SHORTER duration than the remaining must shrink
            // the lock — proves the second call truly replaced (not extended/ignored).
            // A "first wins" or "max wins" buggy impl would keep is_locked=true here.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            e.Lock(duration: 10f);
            Assert.That(e.is_locked, Is.True, "precondition: locked for 10s");
            e.Lock(duration: 0.1f);                 // replace with shorter
            e.Live(dt: 0.2f);                       // should auto-release
            Assert.That(e.is_locked, Is.False, "shorter Lock duration must replace, not extend");
        }

        [Test] public void Case05_LockWhileLocked_DoesNotThrow() {
            // Q14 follow-up: re-Lock with mode change must not throw.
            // Mode is not externally readable; behavioral verification of mode swap
            // requires Phase 3's debug surface and is filed as a Phase 2-4 follow-up.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            e.Lock(duration: 10f, mode: LockMode.Hard);
            Assert.DoesNotThrow(code: () => e.Lock(duration: 10f, mode: LockMode.Soft));
            Assert.That(e.is_locked, Is.True);
        }

        [Test] public void Case06_UnlockWhileNotLocked_NoOp() {
            // Q15: Unlock is idempotent.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            Assert.That(e.is_locked, Is.False);
            Assert.DoesNotThrow(code: () => e.Unlock());
            Assert.That(e.is_locked, Is.False);
        }

        [Test] public void Case07_UnlockTwice_NoOpSecondTime() {
            // Q15: Unlock after auto-release is also a no-op.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            e.Lock(duration: 0.1f);
            e.Live(dt: 0.2f);  // auto-release
            Assert.That(e.is_locked, Is.False);
            Assert.DoesNotThrow(code: () => e.Unlock());
            Assert.DoesNotThrow(code: () => e.Unlock());
            Assert.That(e.is_locked, Is.False);
        }

        // ─────────────────────────────────────────────────────────────────
        // v0.1.5 Phase_2_4_3 — Lock pipeline sub-questions Q-S1, Q-S2, Q-S3
        // (spec §24.3.1, §24.4.1, §9.2 Q-S3 timer placement)
        // ─────────────────────────────────────────────────────────────────

        [Test] public void Case08_SoftLock_CommitmentBonusFollowsLockedBehavior() {
            // Q-S1: During Soft Lock, Step 4's "current action" for commitment.bonus
            // is locked_behavior, not the internal score leader. The observable
            // consequence: even if internal needs would prefer a different action,
            // locked_behavior's commitment cushion remains intact, so on Unlock
            // there is no spurious bonus on the action that "won" internally.
            // Direct bonus value is internal; we observe the persistence proxy:
            // locked_behavior must equal behavior throughout the Soft lock.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            string snapshot = e.behavior;
            e.Lock(duration: 5f, mode: LockMode.Soft);
            Assert.That(e.locked_behavior, Is.EqualTo(expected: snapshot));
            // Push fear high so internal score would prefer Flee.
            e.Affect(need: "fear", delta: +60f);
            e.Live(dt: 0.016f);
            // commitment.bonus must still ride locked_behavior, not the internal
            // leader. The observable: locked_behavior is unchanged.
            Assert.That(e.locked_behavior, Is.EqualTo(expected: snapshot));
        }

        [Test] public void Case09_HardLock_NeedsContinueToUpdate() {
            // Q-S2: During Hard Lock, Steps 1-4 still run. Need values must
            // continue to update — verifiable through the v0.1.5 GetNeed API.
            // (Bus.Publish of A031-style threshold events during Lock requires
            // a MockBus injection point on Engine, which is filed as a Phase 3
            // testability follow-up; this test pins the Need-update contract,
            // which is the necessary precondition for Threshold to fire at all.)
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            e.Lock(duration: 5f, mode: LockMode.Hard);
            float before = e.GetNeed(need: "fear");
            e.Affect(need: "fear", delta: +30f);
            e.Live(dt: 0.016f);
            float after = e.GetNeed(need: "fear");
            Assert.That(after, Is.GreaterThan(expected: before),
                "Need must continue to update during Hard lock (spec §24.3.1)");
        }

        [Test] public void Case10_SoftLock_NeedsContinueToUpdate() {
            // Q-S2: same contract for Soft lock.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            e.Lock(duration: 5f, mode: LockMode.Soft);
            float before = e.GetNeed(need: "fear");
            e.Affect(need: "fear", delta: +30f);
            e.Live(dt: 0.016f);
            float after = e.GetNeed(need: "fear");
            Assert.That(after, Is.GreaterThan(expected: before),
                "Need must continue to update during Soft lock (spec §24.3.1)");
        }

        [Test] public void Case11_LockExpiresMidFrame_SwitchHappensSameFrame() {
            // Q-S3: When the lock-timer reaches zero in the T0 phase at the head
            // of Live(dt), Step 5 must run in the SAME frame, not the next one.
            // Setup: lock for exactly 0.1s, then call Live(0.1) so the timer
            // hits zero in T0. After this single Live call, is_locked must be
            // false AND behavior must reflect the (possibly new) Step 5 choice
            // — proving the lock check between Step 4 and Step 5 saw the
            // already-decremented state.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            e.Lock(duration: 0.1f);
            Assert.That(e.is_locked, Is.True, "precondition: locked");
            e.Affect(need: "fear", delta: +60f);  // queue a strong reason to switch
            e.Live(dt: 0.1f);                     // T0 should release the lock
            Assert.That(e.is_locked, Is.False, "T0 must auto-release in this frame");
            // The exact behavior chosen by Step 5 is up to the Engine, but the
            // observable contract is that is_locked transitioned in the same
            // Live() call as the timer expiry. A "decrement at end" buggy impl
            // would leave is_locked == true here.
        }
    }
}
