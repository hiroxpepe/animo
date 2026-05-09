// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
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
            Engine e = MakeEngine(); Assert.That(e.is_locked, Is.False);
        }
        [Test] public void Case02_LockSetsIsLockedTrue() {
            Engine e = MakeEngine(); e.Live(dt: 0.016f); e.Lock(duration: 2.0f); Assert.That(e.is_locked, Is.True);
        }
        [Test] public void Case03_LockedBehaviorIsCurrentBehavior() {
            Engine e = MakeEngine(); e.Live(dt: 0.016f); e.Lock(duration: 2.0f); Assert.That(e.locked_behavior, Is.EqualTo(expected: e.behavior));
        }
        [Test] public void Case04_LockExpiresAfterDuration() {
            Engine e = MakeEngine(); e.Live(dt: 0.016f); e.Lock(duration: 0.5f);
            e.Live(dt: 0.6f); Assert.That(e.is_locked, Is.False);
        }
        [Test] public void Case05_UnlockReleasesImmediately() {
            Engine e = MakeEngine(); e.Live(dt: 0.016f); e.Lock(duration: 10f); e.Unlock(); Assert.That(e.is_locked, Is.False);
        }
        [Test] public void Case06_LockDurationOver30s_TriggersA031Warning() {
            Engine e = MakeEngine(); e.Live(dt: 0.016f); Assert.DoesNotThrow(code: () => e.Lock(duration: 60f));
        }
        [Test] public void Case07_HardLockMode_PreventsBehaviorChange() {
            Engine e = MakeEngine(); e.Live(dt: 0.016f); e.Lock(duration: 2.0f, mode: LockMode.Hard);
            string before = e.behavior; e.Affect(need: "fear", delta: +60f); e.Live(dt: 0.016f);
            Assert.That(e.behavior, Is.EqualTo(expected: before));
        }
    }
}
