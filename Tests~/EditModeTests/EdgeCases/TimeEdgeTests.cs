// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using Animo.Tests.EditMode.Helpers;
using static Animo.Tests.EditMode.Helpers.Fixture;
using Action = Animo.Model.Action;

namespace Animo.Tests.EditMode.EdgeCases {
    /// <summary>Time edges: dt=0, dt<0, dt=NaN, dt very large (spec §4.6.3).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class TimeEdgeTests {
        [Test] public void Case01_DtZero_DoesNotThrow() {
            Persona p = new Persona { agent_id = "a", needs = NeedsOf(("idle", 30f)),
                actions = new List<Action> { ActionOf(id: "Idle", need: "idle", tier: 5) } };
            Engine e = new Engine(persona: p);
            Assert.DoesNotThrow(code: () => e.Live(dt: 0f));
        }
        [Test] public void Case02_DtNegative_ThrowsArgumentException() {
            Persona p = new Persona { agent_id = "a", needs = NeedsOf(("idle", 30f)),
                actions = new List<Action> { ActionOf(id: "Idle", need: "idle", tier: 5) } };
            Engine e = new Engine(persona: p);
            // Decided in Phase_2_3_2 (formerly TBD per spec §4.7.1 Q12).
            // Negative time would corrupt natural decay (Step 1) and propagate NaN-like
            // garbage through influences. Animo's fail-loud philosophy (spec §16) demands
            // immediate rejection rather than silent acceptance.
            Assert.Throws<ArgumentException>(code: () => e.Live(dt: -1f));
        }
        [Test] public void Case03_DtNaN_ThrowsArgumentException() {
            Persona p = new Persona { agent_id = "a", needs = NeedsOf(("idle", 30f)),
                actions = new List<Action> { ActionOf(id: "Idle", need: "idle", tier: 5) } };
            Engine e = new Engine(persona: p);
            // Decided in Phase_2_3_2 (formerly TBD per spec §4.7.1 Q13).
            // NaN dt would poison every Need on Step 1 and propagate to every action score,
            // collapsing the entire engine into an unrecoverable state. Reject immediately.
            Assert.Throws<ArgumentException>(code: () => e.Live(dt: float.NaN));
        }
        [Test] public void Case04_DtVeryLarge_NeedsClampedNotOverflowed() {
            Persona p = new Persona { agent_id = "a", needs = NeedsOf(("hunger", 50f), ("idle", 30f)),
                rates = RatesOf(("hunger", +1f)),
                actions = new List<Action> { ActionOf(id: "Idle", need: "idle", tier: 5) } };
            Engine e = new Engine(persona: p);
            Assert.DoesNotThrow(code: () => e.Live(dt: 1e6f));
        }
        [Test] public void Case05_ManySmallTicksEqualOneBigTick_RoughlyEqual() {
            Persona p = new Persona { agent_id = "a", needs = NeedsOf(("hunger", 0f), ("idle", 30f)),
                rates = RatesOf(("hunger", +1f)),
                actions = new List<Action> { ActionOf(id: "Idle", need: "idle", tier: 5) } };
            Engine e = new Engine(persona: p);
            for (int i = 0; i < 60; i++) e.Live(dt: 1f / 60f);
            Assert.That(e.behavior, Is.Not.Null);
        }
    }
}
