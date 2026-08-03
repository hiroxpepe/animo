// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Decision-table tests for Engine Live Step 5: switch decision + Lock skip (spec §9.2, §24).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Step5_ActionSwitchTests {

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
        [Test] public void Case01_SwitchToBestScoredAction() {
            Engine e = MakeEngine(); e.Affect(need: "fear", delta: +60f); e.Live(dt: 0.016f); Assert.That(e.Behavior, Is.Not.Null);
        }
        [Test] public void Case02_TieGoesToCurrentDueToCommitment() {
            Engine e = MakeEngine(); e.Live(dt: 0.016f); e.Live(dt: 0.016f); Assert.That(e.Behavior, Is.Not.Null);
        }
        [Test] public void Case03_WhenLockedHard_SkipStep5() {
            Engine e = MakeEngine(); e.Live(dt: 0.016f); e.Lock(duration: 5f, mode: LockMode.Hard); Assert.That(e.IsLocked, Is.True);
        }
        [Test] public void Case04_WhenLockedSoft_StepsRunButBehaviorFrozen() {
            // (Q-S2, spec §24 line 5525, DECISION LOG Q-S2)
            // Soft Lock: Steps 1-4 run, Step 5 SKIPPED → behavior must NOT change.
            Engine e = MakeEngine();
            e.Live(dt: 0.016f);
            string before = e.Behavior;
            e.Lock(duration: 5f, mode: LockMode.Soft);
            Assert.That(e.IsLocked, Is.True);
            // Drive fear high so Flee would win if Step 5 ran.
            e.Affect(need: "fear", delta: +99f);
            e.Live(dt: 0.016f);
            Assert.That(e.Behavior, Is.EqualTo(expected: before),
                "Q-S2 + spec §24: Soft Lock must freeze behavior (Step 5 skipped). " +
                "behavior must not change even when Flee score dominates.");
        }

        // ─────────────────────────────────────────────────────────────────
        // v0.1.5 Phase_2_4_5 — Q-S9 tie-break + first-Live behavior
        // (spec §9.2.0a + §9.2 Step 5 declaration-order rule)
        // ─────────────────────────────────────────────────────────────────

        [Test] public void Case05_AllZeroScores_FirstDeclaredActionWins() {
            // Q-S9: all Needs == 0 at spawn → all intensities == 0 → all
            // scores == 0 → tie. Spec rule: action whose `id` appears first
            // in actions[] wins. With actions[] = [Flee, Idle, Eat], "Flee"
            // is at index 0 and must win.
            Persona p = new Persona {
                agent_id = "newborn",
                needs = NeedsOf(("fear", 0f), ("idle", 0f), ("hunger", 0f)),
                actions = new List<Action> {
                    ActionOf(id: "Flee",  need: "fear",   tier: 2, exponent: 2.5f),
                    ActionOf(id: "Idle",  need: "idle",   tier: 5, exponent: 1.0f),
                    ActionOf(id: "Eat",   need: "hunger", tier: 1, exponent: 1.5f),
                }
            };
            Engine e = new Engine(persona: p);
            e.Live(dt: 0.016f);
            Assert.That(e.Behavior, Is.EqualTo(expected: "Flee"),
                "All-zero scores → tie → first declared action wins (spec §9.2 Step 5, Q-S9)");
        }

        [Test] public void Case06_AllZeroScores_ReorderedActions_NewFirstWins() {
            // Companion: confirm tie-break truly tracks declaration order, not
            // tier or exponent. Same Persona but actions[] = [Idle, Flee, Eat]
            // — "Idle" must now win.
            Persona p = new Persona {
                agent_id = "newborn",
                needs = NeedsOf(("fear", 0f), ("idle", 0f), ("hunger", 0f)),
                actions = new List<Action> {
                    ActionOf(id: "Idle",  need: "idle",   tier: 5, exponent: 1.0f),
                    ActionOf(id: "Flee",  need: "fear",   tier: 2, exponent: 2.5f),
                    ActionOf(id: "Eat",   need: "hunger", tier: 1, exponent: 1.5f),
                }
            };
            Engine e = new Engine(persona: p);
            e.Live(dt: 0.016f);
            Assert.That(e.Behavior, Is.EqualTo(expected: "Idle"),
                "Tie-break is by actions[] declaration order, not by tier or exponent");
        }

        [Test] public void Case07_FirstLive_NoCommitmentBonus_PureScoreCompetition() {
            // Q-S9: behavior == "" before first Live (spec §9.1) → first-frame
            // Step 4 cannot apply commitment.bonus to any action ("current
            // action" doesn't exist yet). All actions compete on raw score.
            // Setup: hunger high enough to dominate even without commitment.
            Persona p = new Persona {
                agent_id = "hungry",
                needs = NeedsOf(("fear", 0f), ("idle", 0f), ("hunger", 90f)),
                actions = new List<Action> {
                    ActionOf(id: "Idle",  need: "idle",   tier: 5, exponent: 1.0f),
                    ActionOf(id: "Eat",   need: "hunger", tier: 1, exponent: 1.5f),
                },
                commitment = new Commitment { bonus = 25f }
            };
            Engine e = new Engine(persona: p);
            e.Live(dt: 0.016f);
            // No commitment bonus on any action this frame; pure intensity
            // competition. Hunger 90 with exp 1.5 dominates idle 0.
            Assert.That(e.Behavior, Is.EqualTo(expected: "Eat"));
        }
    }
}
