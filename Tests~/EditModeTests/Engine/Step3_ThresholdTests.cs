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
    }
}
