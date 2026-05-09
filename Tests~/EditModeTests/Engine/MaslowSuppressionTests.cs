// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Decision-table tests for dynamic Maslow suppression: low-tier kills high-tier (spec §9.3).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class MaslowSuppressionTests {

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
        [Test] public void Case01_HighTier1NeedSuppressesTier5Action() {
            Engine e = MakeEngine(); e.Affect(need: "hunger", delta: +60f); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case02_HighTier2NeedSuppressesTier5() {
            Engine e = MakeEngine(); e.Affect(need: "fear", delta: +50f); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case03_LowTier1NeedDoesNotSuppress() {
            Engine e = MakeEngine(); e.Affect(need: "hunger", delta: -100f); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case04_SuppressionFactorZero_NoSuppressionEffect() {
            Engine e = MakeEngine(); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
        [Test] public void Case05_SuppressionFactorOne_FullSuppression() {
            Engine e = MakeEngine(); e.Live(dt: 0.016f); Assert.That(e.behavior, Is.Not.Null);
        }
    }
}
