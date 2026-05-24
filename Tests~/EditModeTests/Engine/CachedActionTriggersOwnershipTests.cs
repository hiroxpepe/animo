#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class CachedActionTriggersOwnershipTests {
        [Test] public void Case01_GetExpandedActionTrigger_IsInvokable() {
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
