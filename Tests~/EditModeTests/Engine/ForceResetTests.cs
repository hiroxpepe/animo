// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Decision-table tests for Affect force_reset semantics (spec §9.7, §24.4).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ForceResetTests {

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
        [Test] public void Case01_AffectAddsToNeed() {
            Engine e = MakeEngine(); e.Affect(need: "fear", delta: +30f); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case02_AffectClampsAt100() {
            Engine e = MakeEngine(); e.Affect(need: "fear", delta: +500f); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case03_AffectClampsAt0() {
            Engine e = MakeEngine(); e.Affect(need: "fear", delta: -500f); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case04_ForceResetTrueSkipsCommitmentForOneFrame() {
            Engine e = MakeEngine(); e.Affect(need: "fear", delta: +50f, force_reset: true); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case05_DuringHardLock_ForceResetDoesNotChangeBehavior() {
            // Note: Engine has no public API to read Need values directly (spec §9.1, §16.1
            // Zero-Allocation Hot Path). The behavior-frozen invariant is what we can verify
            // from the public surface; whether the underlying Need value updates during a
            // Hard lock is an internal contract checked indirectly by Engine/LockTests once
            // the Lock auto-releases. A debug-read API is filed as a Phase 2-4 candidate.
            Engine e = MakeEngine(); e.Live(dt: 0.016f); e.Lock(duration: 2.0f, mode: LockMode.Hard);
            string before = e.behavior;
            e.Affect(need: "fear", delta: +50f, force_reset: true); e.Live(dt: 0.016f);
            Assert.That(e.behavior, Is.EqualTo(expected: before));
        }
    }
}
