#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class PhaseANeedsUnwrapTests {
        [Test] public void Case01_PersonaWithNullNeeds_DoesNotThrow() {
            var p = new Persona { agent_id = "a",
                needs = null,
                actions = new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) }
            };
            Assert.DoesNotThrow(() => new Engine(p),
                "Q-S65: null needs must not throw NRE in PHASE A.");
        }
        [Test] public void Case02_PersonaWithNeedsValues_CorrectlySeeded() {
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("fear",40f)),
                actions = new List<Animo.Model.Action>{ ActionOf("Flee","fear",2) }
            };
            var e = new Engine(p);
            Assert.That(e.GetBaseNeed("fear"), Is.EqualTo(40f),
                "Q-S65: needs.values must be correctly seeded in _needs array.");
        }
        [Test] public void Case03_NeedsValuesDictionaryDirectReference_DoesNotThrow() {
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("idle",30f), ("fear",50f)),
                actions = new List<Animo.Model.Action>{ ActionOf("X","idle",5) }
            };
            Assert.DoesNotThrow(() => new Engine(p),
                "Q-S65: _persona.needs?.values unwrap must not throw.");
        }
    }
}
