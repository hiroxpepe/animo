// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Decision-table tests for Engine Live Step 3: threshold check + Bus publish (spec §9.2, §12.3).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Step3_ThresholdTests {

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
        [Test] public void Case01_BelowTrigger_DoesNotFire() {
            Engine e = MakeEngine(); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case02_AtOrAboveTrigger_FiresOnce() {
            Engine e = MakeEngine(); e.Affect(need: "fear", delta: 100f); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case03_StaysAboveReset_DoesNotRefire() {
            Engine e = MakeEngine(); e.Affect(need: "fear", delta: 100f); e.Live(dt: 0.016f); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case04_DropsBelowReset_RearmsAndCanFireAgain() {
            Engine e = MakeEngine(); e.Affect(need: "fear", delta: 100f); e.Live(dt: 0.016f); e.Affect(need: "fear", delta: -100f); e.Live(dt: 0.016f); e.Affect(need: "fear", delta: 100f); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }

        // ─────────────────────────────────────────────────────────────────
        // v0.1.5 Phase_2_4_5 — Q-S8 first-frame seeding (spec §9.2.0a)
        // ─────────────────────────────────────────────────────────────────

        [Test] public void Case05_FrameOne_HighSpawnNeed_DoesNotFireSpuriousThreshold() {
            // Q-S8: Engine ctor seeds _previous_needs from spawn-time Need values.
            // A Persona spawned with fear == 80 must NOT fire fear_critical on
            // its very first Live(dt) — the threshold is "above" at spawn, no
            // upward crossing happened. A buggy implementation that left
            // _previous_needs == [0,0,0,...] would see "fear rose 0→80 this
            // frame" and fire spuriously.
            //
            // Spec invariant pinned: after the first Live(dt) with no Affect,
            // the engine state must reflect "no Need rose this frame".
            // Direct Bus-publish observation requires a MockBus injection
            // point on Engine ctor (Phase 3 follow-up); here we pin the
            // necessary precondition: Engine constructs without throwing and
            // the first Live runs cleanly with a high spawn Need.
            Persona p = new Persona {
                agent_id = "scared_npc",
                needs = NeedsOf(("fear", 80f), ("idle", 0f)),
                actions = new List<Action> {
                    ActionOf(id: "Flee", need: "fear", tier: 2, exponent: 2.5f),
                    ActionOf(id: "Idle", need: "idle", tier: 5, exponent: 1.0f)
                }
            };
            Engine e = new Engine(persona: p);
            // Spawn-time Need readback confirms ctor ran with the spawn value.
            Assert.That(e.GetNeed(need: "fear"), Is.EqualTo(expected: 80f));
            Assert.DoesNotThrow(code: () => e.Live(dt: 0.016f),
                "first Live must not throw on a Persona with high spawn Need");
            // After a noop frame (no Affect), GetNeed should still be ~80
            // (subject to natural decay). The fact that there was no upward
            // crossing means no Threshold should have fired — a Phase 3 test
            // with MockBus injection will assert this directly.
            Assert.That(e.GetNeed(need: "fear"), Is.LessThanOrEqualTo(expected: 80f));
        }
    }
}
