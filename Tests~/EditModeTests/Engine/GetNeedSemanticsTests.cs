// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class GetNeedSemanticsTests {
        [Test] public void Case01_GetNeedReturnsEffective_AfterInfluenceCascade() {
            // Q-S54: GetNeed = effective (post-cascade).
            var p = new Persona {
                agent_id = "a",
                needs = NeedsOf(("fear",30f),("confidence",50f)),
                actions = new List<Animo.Model.Action> { ActionOf("X","fear",2) },
                influences = new List<Influence> {
                    new Influence { source="fear", target="confidence", coefficient=1.0f }
                }
            };
            var e = new Engine(p);
            e.Live(0.1f);
            // After cascade, confidence should be elevated
            float eff_confidence = e.GetNeed("confidence");
            Assert.That(eff_confidence, Is.GreaterThan(50f),
                "Q-S54: GetNeed('confidence') must return effective (post-cascade) value.");
        }

        [Test] public void Case02_GetBaseNeedReturnsBase_BeforeCascade() {
            // Q-S54: GetBaseNeed = base (pre-cascade).
            var p = new Persona {
                agent_id = "a",
                needs = NeedsOf(("fear",30f),("confidence",50f)),
                actions = new List<Animo.Model.Action> { ActionOf("X","fear",2) },
                influences = new List<Influence> {
                    new Influence { source="fear", target="confidence", coefficient=1.0f }
                }
            };
            var e = new Engine(p);
            e.Live(0.1f);
            float base_confidence = e.GetBaseNeed("confidence");
            Assert.That(base_confidence, Is.EqualTo(50f).Within(0.01f),
                "Q-S54: GetBaseNeed must return base (pre-cascade) value.");
        }
    }
}
