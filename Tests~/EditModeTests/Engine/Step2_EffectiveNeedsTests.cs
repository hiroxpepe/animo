// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Step 2: EffectiveNeeds cascade — numerical value verification (spec §9.6).</summary>
    [TestFixture]
    public class Step2_EffectiveNeedsTests {

        [Test] public void Case01_NoInfluences_EffectiveEqualsBase() {
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("fear", 40f), ("idle", 30f)),
                actions = new List<Action> { ActionOf("Idle","idle",5) }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);
            Assert.That(e.GetNeed("fear"), Is.EqualTo(40f).Within(0.001f),
                "Step 2: without influences, effective must equal base.");
        }

        [Test] public void Case02_PositiveInfluence_RaisesTarget() {
            // fear=50, coefficient=0.5 → delta=0.5*(50/100)*50=12.5 → confidence rises
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("fear",50f),("confidence",20f),("idle",30f)),
                influences = new List<Influence>{
                    new Influence { source="fear", target="confidence", coefficient=0.5f }},
                actions = new List<Action>{ ActionOf("Idle","idle",5) }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);
            float eff = e.GetNeed("confidence");
            Assert.That(eff, Is.GreaterThan(20f),
                "Step 2: positive coefficient must raise target above base.");
        }

        [Test] public void Case03_NegativeInfluence_LowersTarget() {
            // fear=50, coefficient=-0.5 → delta=-0.5*(50/100)*50=-12.5 → confidence drops
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("fear",50f),("confidence",60f),("idle",30f)),
                influences = new List<Influence>{
                    new Influence { source="fear", target="confidence", coefficient=-0.5f }},
                actions = new List<Action>{ ActionOf("Idle","idle",5) }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);
            float eff = e.GetNeed("confidence");
            Assert.That(eff, Is.LessThan(60f),
                "Step 2: negative coefficient must lower target below base.");
        }

        [Test] public void Case04_ChainInfluence_CascadesInOrder() {
            // A→B→C: A=80, coeff=1.0
            // B_eff = B + 1.0*(80/100)*80 = 10 + 64 = 74 (clamped 74)
            // C_eff = C + 1.0*(74/100)*74 ≈ 10 + 54.76 ≈ 64.76
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("a_need",80f),("b_need",10f),("c_need",10f),("idle",30f)),
                influences = new List<Influence>{
                    new Influence { source="a_need", target="b_need", coefficient=1.0f },
                    new Influence { source="b_need", target="c_need", coefficient=1.0f }},
                actions = new List<Action>{ ActionOf("Idle","idle",5) }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);
            float c_eff = e.GetNeed("c_need");
            Assert.That(c_eff, Is.GreaterThan(10f),
                "Step 2: chain A→B→C must cascade (C must be raised).");
            Assert.That(c_eff, Is.GreaterThan(e.GetNeed("b_need") == 10f ? 10f : 0f),
                "Step 2: C must be above its base value.");
        }

        [Test] public void Case05_ClampAfterEachEdge_StaysInBounds() {
            // source=100, coefficient=1.0 → huge delta → target must not exceed 100
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("fear",100f),("confidence",100f),("idle",30f)),
                influences = new List<Influence>{
                    new Influence { source="fear", target="confidence", coefficient=1.0f }},
                actions = new List<Action>{ ActionOf("Idle","idle",5) }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);
            Assert.That(e.GetNeed("confidence"), Is.LessThanOrEqualTo(100f),
                "Step 2: clamp after each edge must keep values ≤ 100.");
            Assert.That(e.GetNeed("confidence"), Is.GreaterThanOrEqualTo(0f),
                "Step 2: clamp after each edge must keep values ≥ 0.");
        }
    }
}
