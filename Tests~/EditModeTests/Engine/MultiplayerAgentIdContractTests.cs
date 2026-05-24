#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class MultiplayerAgentIdContractTests {
        [Test] public void Case01_UuidStyleAgentId_ProducesValidBusPayload() {
            var p = new Persona { agent_id = "goblin_uuid_a1b2c3d4",
                needs = NeedsOf(("idle",30f)),
                actions = new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) },
                binding = new Binding { on_action_change = "animo_{agent_id}_{behavior}" }
            };
            var e = new Engine(p);
            Assert.That(e.GetExpandedActionTrigger("Idle"),
                Is.EqualTo("animo_goblin_uuid_a1b2c3d4_Idle"),
                "Q-S59: UUID-style agent_id must produce valid Bus payload.");
        }
    }
}
