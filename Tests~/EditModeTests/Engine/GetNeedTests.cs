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
    /// Decision-table tests for Engine.GetNeed (v0.1.5 new). See spec §9.1.
    /// Read-only debug API; not for hot-path use.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class GetNeedTests {

        Engine MakeEngine() {
            Persona p = new Persona {
                agent_id = "a",
                needs = NeedsOf(("hunger", 50f), ("fear", 25f), ("idle", 30f)),
                actions = new List<Animo.Model.Action> { ActionOf(id: "Idle", need: "idle", tier: 5) }
            };
            return new Engine(persona: p);
        }

        [Test] public void Case01_KnownNeed_ReturnsInitialValue() {
            Engine e = MakeEngine();
            Assert.That(e.GetNeed(need: "hunger"), Is.EqualTo(expected: 50f));
        }

        [Test] public void Case02_KnownNeed_ReturnsAnotherInitialValue() {
            Engine e = MakeEngine();
            Assert.That(e.GetNeed(need: "fear"), Is.EqualTo(expected: 25f));
        }

        [Test] public void Case03_AfterAffect_ReflectsNewValue() {
            Engine e = MakeEngine();
            e.Affect(need: "hunger", delta: +20f);
            Assert.That(e.GetNeed(need: "hunger"), Is.EqualTo(expected: 70f));
        }

        [Test] public void Case04_UnknownNeed_ReturnsZeroAndWarns() {
            Engine e = MakeEngine();
            // Spec §9.1 v0.1.5: returns 0.0 after a Warning (no exception).
            float v = 0f;
            Assert.DoesNotThrow(code: () => v = e.GetNeed(need: "phantom"));
            Assert.That(v, Is.EqualTo(expected: 0f));
        }

        [Test] public void Case05_NullNeed_ThrowsArgumentNullException() {
            Engine e = MakeEngine();
            // Consistent with the rest of the API under #nullable enable.
            Assert.Throws<ArgumentNullException>(code: () => e.GetNeed(need: null!));
        }

        [Test] public void Case06_EmptyNeed_ThrowsArgumentException() {
            Engine e = MakeEngine();
            Assert.Throws<ArgumentException>(code: () => e.GetNeed(need: ""));
        }
    }
}
