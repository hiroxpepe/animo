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
    /// Decision-table test for Q-S26 (v0.1.5): Engine exposes
    /// `public event Action<string>? OnSignaled` so external listeners
    /// (Agent in Unity, MockBus in tests) can receive Threshold fire
    /// signals and behavior-change signals without Engine holding any
    /// Bus reference. Pre-Q-S26 §16.5's `_bus.Publish(...)` inside Engine
    /// was architecturally impossible — §12.1 explicitly bans Engine
    /// from holding Bus.
    ///
    /// This test passes immediately in Phase_2_4_11 because OnSignaled is
    /// the API surface; full firing semantics (Step 3 actually invokes
    /// it) are Phase 3 implementation work.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class EngineOnSignalEventTests {

        [Test] public void Case01_OnSignalEventCanBeSubscribedAndUnsubscribed() {
            Persona p = new Persona {
                agent_id = "a",
                needs    = NeedsOf(("idle", 30f)),
                actions  = new List<Animo.Model.Action> { ActionOf(id: "Idle", need: "idle", tier: 5) }
            };
            Engine engine = new Engine(persona: p);

            int call_count = 0;
            Action<string> handler = (signal_id) => call_count++;

            // Q-S26 contract: subscription should not throw.
            Assert.DoesNotThrow(code: () => { engine.OnSignaled += handler; },
                "Q-S26: subscribing to Engine.OnSignaled must not throw");

            // Unsubscription should not throw either.
            Assert.DoesNotThrow(code: () => { engine.OnSignaled -= handler; },
                "Q-S26: unsubscribing from Engine.OnSignaled must not throw");

            // No invocation happened (we didn't call Live or fire anything).
            Assert.That(call_count, Is.EqualTo(expected: 0));
        }
    }
}
