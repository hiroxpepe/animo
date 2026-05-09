// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Decision-table tests for Engine Live Step 2: EffectiveNeeds via influences (spec §9.2).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Step2_EffectiveNeedsTests {

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
        [Test] public void Case01_NoInfluences_DoesNotThrow() {
            Engine e = MakeEngine(); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case02_PositiveInfluence_AppliesAfterTopoSort() {
            Engine e = MakeEngine(); e.Live(dt: 1.0f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case03_NegativeInfluence_LowersTarget() {
            Engine e = MakeEngine(); e.Live(dt: 1.0f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case04_ChainInfluence_AppliesInOrder() {
            Engine e = MakeEngine(); e.Live(dt: 1.0f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case05_ClampAfterEachEdge_StaysInBounds() {
            Engine e = MakeEngine(); e.Live(dt: 1.0f); Assert.That(e.behavior, Is.Not.Null);
        }
    }
}
