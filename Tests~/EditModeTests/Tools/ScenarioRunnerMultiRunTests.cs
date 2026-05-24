#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Tools;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class ScenarioRunnerMultiRunTests {
        Root MakeRoot() => new Root { schema_version="1.5",
            personas = new List<Persona>{ new Persona { agent_id="goblin",
                needs=NeedsOf(("idle",30f)),
                actions=new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) }}}};

        [Test] public void Case01_TwoRunsFromSameTemplate_DoNotCollideOnStoreRegister() {
            // Q-S42: ScenarioRunner auto-generates distinct agent_ids: goblin_run_0, goblin_run_1.
            var runner = new ScenarioRunner(MakeRoot());
            var r1 = runner.Run("goblin", duration: 1.0f, dt: 0.1f);
            var r2 = runner.Run("goblin", duration: 1.0f, dt: 0.1f);
            // Both must succeed without Store registration (Q-S50)
            Assert.That(r1.frames.Count, Is.GreaterThan(0));
            Assert.That(r2.frames.Count, Is.GreaterThan(0));
            // ScenarioRunner does NOT call Store.Register, so no collision
            Assert.That(Animo.Store.Instance.IsRegistered("goblin_run_0"), Is.False,
                "Q-S50: ScenarioRunner must NOT register engines in Store.");
        }
    }
}
