#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Tools;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class ScenarioRunnerSpawnFrameTests {
        Root MakeRoot() => new Root { schema_version="1.5",
            personas = new List<Persona>{ new Persona { agent_id="a",
                needs=NeedsOf(("idle",30f)),
                actions=new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) }}}};

        [Test] public void Case01_FirstFrameTimeIsZero_NotDt() {
            var runner = new ScenarioRunner(MakeRoot());
            var result = runner.Run("a", duration: 1.0f, delta_time: 0.1f);
            Assert.That(result.frames[0].time, Is.EqualTo(0.0f).Within(1e-5f),
                "Q-S51: frames[0] is spawn frame at time=0, not first delta_time.");
        }
    }
}
