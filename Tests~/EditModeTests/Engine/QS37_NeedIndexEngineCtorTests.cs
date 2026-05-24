#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class QS37_NeedIndexEngineCtorTests {
        [Test] public void Case01_TwoPersonasDifferentNonStandardOrder_HaveDifferentNeedIndex() {
            // Q-S37: PHASE B bakes need_index from per-Persona _need_index map.
            var p1 = new Persona { agent_id = "a",
                needs = NeedsOf(("oxygen",40f),("thirst",60f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("A","oxygen",1), ActionOf("B","thirst",1) }
            };
            var p2 = new Persona { agent_id = "b",
                needs = NeedsOf(("thirst",60f),("oxygen",40f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("A","oxygen",1), ActionOf("B","thirst",1) }
            };
            var e1 = new Engine(p1);
            var e2 = new Engine(p2);
            // Both engines work independently (baked from their own _need_index)
            Assert.DoesNotThrow(() => e1.Live(0.1f));
            Assert.DoesNotThrow(() => e2.Live(0.1f));
            Assert.That(e1.GetBaseNeed("oxygen"), Is.EqualTo(40f).Within(1f));
            Assert.That(e2.GetBaseNeed("oxygen"), Is.EqualTo(40f).Within(1f));
        }
    }
}
