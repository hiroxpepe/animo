#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Tools;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class ScenarioRunnerOverShootTests {
        Root MakeRoot() => new Root { schema_version="1.5",
            personas = new List<Persona>{ new Persona { agent_id="a",
                needs=NeedsOf(("idle",30f)),
                actions=new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) }}}};

        [Test] public void Case01_DurationExactMultipleOfDt_DoesNotOverShoot() {
            var runner = new ScenarioRunner(MakeRoot());
            var result = runner.Run("a", duration: 10.0f, dt: 0.1f);
            // Exactly 100 Live(dt) calls, no overshoot
            Assert.That(result.frames.Count, Is.EqualTo(101),
                "Q-S35: exactly 100 Live(dt) calls, no extra frame.");
            float last_time = result.frames[result.frames.Count - 1].time;
            Assert.That(last_time, Is.EqualTo(10.0f).Within(0.001f),
                "last frame time must be duration (10.0f).");
        }
    }
}
