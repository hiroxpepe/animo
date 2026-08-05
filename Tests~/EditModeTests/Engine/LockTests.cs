// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Decision-table tests for Lock / Unlock API (spec §24).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class LockTests {

        Engine MakeEngine() {
            Persona p = new Persona {
                agent_id = "a",
                needs = NeedsOf(("fear", 50f), ("idle", 30f), ("hunger", 40f), ("frustration", 10f)),
                actions = new List<Action> {
                    ActionOf(id: "Flee",  need: "fear",   tier: 2, exponent: 2.5f),
                    ActionOf(id: "Idle",  need: "idle",   tier: 5, exponent: 1.0f),
                    ActionOf(id: "Eat",   need: "hunger", tier: 1, exponent: 1.5f),
                }
            };
            return new Engine(persona: p);
        }
        [Test] public void Case01_InitialState_NotLocked() {
            Engine e = MakeEngine(); Assert.That(e.IsLocked, Is.False);
        }
        [Test] public void Case02_LockSetsIsLockedTrue() {
            Engine e = MakeEngine(); e.Live(delta_time: 0.016f); e.Lock(duration: 2.0f); Assert.That(e.IsLocked, Is.True);
        }
        [Test] public void Case03_LockedBehaviorIsCurrentBehavior() {
            Engine e = MakeEngine(); e.Live(delta_time: 0.016f); e.Lock(duration: 2.0f); Assert.That(e.LockedBehavior, Is.EqualTo(expected: e.Behavior));
        }
        [Test] public void Case04_LockExpiresAfterDuration() {
            Engine e = MakeEngine(); e.Live(delta_time: 0.016f); e.Lock(duration: 0.5f);
            e.Live(delta_time: 0.6f); Assert.That(e.IsLocked, Is.False);
        }
        [Test] public void Case05_UnlockReleasesImmediately() {
            Engine e = MakeEngine(); e.Live(delta_time: 0.016f); e.Lock(duration: 10f); e.Unlock(); Assert.That(e.IsLocked, Is.False);
        }
        [Test] public void Case06_LockDurationOver30s_TriggersA031Warning() {
            // (A031, roadmap §5.5.1) Engine.Lock with duration > LOCK_DURATION_WARN_THRESHOLD
            // must emit AnimoLog.Warning containing "[A031]". The lock must still succeed.
            string? captured = null;
            AnimoLog.OnLog = (level, msg) => { if (level == "Warning") captured = msg; };
            try {
                Engine e = MakeEngine();
                e.Live(delta_time: 0.016f);
                Assert.DoesNotThrow(code: () => e.Lock(duration: 60f),
                    "A031: Lock(60f) must not throw — it warns only.");
                Assert.That(captured, Is.Not.Null,
                    "A031: Lock(duration > 30s) must emit AnimoLog.Warning.");
                Assert.That(captured, Does.Contain("[A031]"),
                    "A031: Warning message must contain rule ID \"[A031]\".");
            } finally {
                AnimoLog.OnLog = null;
            }
        }
        [Test] public void Case07_HardLockMode_PreventsBehaviorChange() {
            Engine e = MakeEngine(); e.Live(delta_time: 0.016f); e.Lock(duration: 2.0f, mode: LockMode.Hard);
            string before = e.Behavior; e.Affect(need: "fear", delta: +60f); e.Live(delta_time: 0.016f);
            Assert.That(e.Behavior, Is.EqualTo(expected: before));
        }
    }
}
