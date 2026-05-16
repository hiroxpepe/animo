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
    /// Decision-table test for Q-S31 (v0.1.5): the very first behavior
    /// assignment ("" → actions[0] via Q-S9 tie-break on frame 1) must
    /// NOT raise OnSignal. 100 NPCs spawning into a scene cannot publish
    /// 100 simultaneous animo_*_idle signals — that's an init storm
    /// rate-limited Bus listeners cannot absorb.
    ///
    /// Phase 3 implementation: Engine maintains _previous_behavior
    /// (defaults to ""); Step 5 reads it before assigning new value;
    /// OnBehaviorChanged returns silently when previous_behavior == "".
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class SilentFirstTransitionTests {

        [Test] public void Case01_FirstLive_NoOnSignalForFirstBehaviorAssignment() {
            // Pre: Persona with two actions; frame 1 tie-break picks
            //      actions[0] (Q-S9). Pre-Q-S31 OnSignal would raise
            //      with payload like "animo_a_idle" because behavior
            //      transitioned from "" to "Idle".
            // Post-Q-S31: OnSignal is NOT raised on frame 1.
            Persona p = new Persona {
                agent_id = "a",
                needs    = NeedsOf(("idle", 30f), ("fear", 30f)),
                actions  = new List<Animo.Model.Action> {
                    ActionOf(id: "Idle", need: "idle", tier: 5),
                    ActionOf(id: "Flee", need: "fear", tier: 2)
                },
                binding = new Binding {
                    on_action_change = "animo_{agent_id}_{behavior}"
                }
            };
            Engine engine = new Engine(persona: p);

            int signal_count = 0;
            engine.OnSignal += signal_id => signal_count++;

            // Frame 1: behavior transitions from "" to "Idle" (Q-S9 tie-break).
            // Q-S31: OnSignal must NOT be raised for this transition.
            Assert.DoesNotThrow(code: () => engine.Live(dt: 0.016f));

            Assert.That(signal_count, Is.EqualTo(expected: 0),
                "Q-S31: OnSignal must NOT raise on the very first behavior assignment " +
                "(\"\" → actions[0]). Pre-Q-S31, 100 NPCs spawning would send 100 " +
                "simultaneous init signals to Bus. Post-frame-1 transitions still fire.");
        }
    }
}
