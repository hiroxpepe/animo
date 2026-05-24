#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Tools;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class ScenarioRunnerStoreIndependenceTests {
        [TearDown] public void TearDown() => Animo.Store.ResetForTesting();

        Root MakeRoot() => new Root { schema_version="1.5",
            personas = new List<Persona>{ new Persona { agent_id="a",
                needs=NeedsOf(("idle",30f)),
                actions=new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) }}}};

        [Test] public void Case01_RunnerDoesNotCallStoreRegister() {
            var runner = new ScenarioRunner(MakeRoot());
            runner.Run("a", duration: 1.0f, dt: 0.1f);
            Assert.That(Animo.Store.Instance.IsRegistered("a"),       Is.False,
                "Q-S50: ScenarioRunner.Run must NOT register engine in Store.");
            Assert.That(Animo.Store.Instance.IsRegistered("a_run_0"), Is.False,
                "Q-S50: auto-generated override id must also not be in Store.");
        }
    }
}
