#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class GetExpandedActionTriggerTests {
        [Test] public void Case01_KnownBehavior_ReturnsTemplateExpandedTrigger() {
            var p = new Persona { agent_id = "goblin_01",
                needs = NeedsOf(("idle",50f)),
                actions = new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) },
                binding = new Binding { on_action_change = "animo_{agent_id}_{behavior}" }
            };
            var e = new Engine(p);
            Assert.That(e.GetExpandedActionTrigger("Idle"), Is.EqualTo("animo_goblin_01_Idle"),
                "Q-S44: GetExpandedActionTrigger must return template-expanded string.");
        }
        [Test] public void Case02_IsInvokableOnConstructedEngine() {
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("idle",30f)),
                actions = new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) }
            };
            var e = new Engine(p);
            Assert.DoesNotThrow(() => e.GetExpandedActionTrigger("Idle"),
                "Q-S46: GetExpandedActionTrigger must be callable on a constructed Engine.");
        }
    }
}
