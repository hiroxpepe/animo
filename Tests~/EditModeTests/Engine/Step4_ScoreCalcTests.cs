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
    /// <summary>Step 4: score = Pow(intensity,exp)*100 — numerical verification (spec §9.4).</summary>
    [TestFixture]
    public class Step4_ScoreCalcTests {

        [Test] public void Case01_AllNeedsZero_ScoreIsZero() {
            // intensity=0 → Pow(0,exp)=0 → score=0 for all exp
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("fear",0f),("idle",0f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Flee","fear",2,2.5f), ActionOf("Idle","idle",5,1.0f) }
            };
            var e = new Engine(p);
            e.Live(delta_time: 0.016f);
            Assert.That(e.GetActionScore("Flee"), Is.EqualTo(0f).Within(0.001f),
                "Step 4: need=0 → score=0.");
        }

        [Test] public void Case02_LinearExponent_ScoreEqualsIntensity() {
            // need=60, exp=1.0 → score = (60/100)^1 * 100 = 60
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("fear",60f)),
                actions = new List<Animo.Model.Action>{ ActionOf("Flee","fear",2,1.0f) }
            };
            var e = new Engine(p);
            e.Live(delta_time: 0.016f);
            Assert.That(e.GetActionScore("Flee"), Is.EqualTo(60f).Within(0.1f),
                "Step 4: exp=1.0 → score == need value.");
        }

        [Test] public void Case03_HighExponent_SuppressesLowNeed() {
            // need=30, exp=2.5 → score = (0.3)^2.5 * 100 = 4.93
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("fear",30f)),
                actions = new List<Animo.Model.Action>{ ActionOf("Flee","fear",2,2.5f) }
            };
            var e = new Engine(p);
            e.Live(delta_time: 0.016f);
            float expected = (float)(Math.Pow(0.3, 2.5) * 100.0);
            Assert.That(e.GetActionScore("Flee"), Is.EqualTo(expected).Within(0.1f),
                $"Step 4: exp=2.5, need=30 → score≈{expected:F2}.");
        }

        [Test] public void Case04_HigherNeedWinsDecision() {
            // fear=80 (Flee), idle=30 (Idle): Flee must win
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("fear",80f),("idle",30f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Flee","fear",2,1.0f), ActionOf("Idle","idle",5,1.0f) }
            };
            var e = new Engine(p);
            e.Live(delta_time: 0.016f);
            Assert.That(e.GetActionScore("Flee"), Is.GreaterThan(e.GetActionScore("Idle")),
                "Step 4: higher need value must produce higher score.");
            Assert.That(e.Behavior, Is.EqualTo("Flee"),
                "Step 4: action with highest score must be selected.");
        }

        [Test] public void Case05_ScoreFormula_MatchesSpec() {
            // spec §9.4: score = Pow(intensity, exp) × 100
            // need=70, exp=2.0 → (0.7^2)*100 = 49.0
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("fear",70f)),
                actions = new List<Animo.Model.Action>{ ActionOf("Flee","fear",2,2.0f) }
            };
            var e = new Engine(p);
            e.Live(delta_time: 0.016f);
            float expected = (float)(Math.Pow(0.7, 2.0) * 100.0);
            Assert.That(e.GetActionScore("Flee"), Is.EqualTo(expected).Within(0.01f),
                $"Step 4 spec §9.4: Pow(0.7,2.0)*100 = {expected:F2}.");
        }
    }
}
