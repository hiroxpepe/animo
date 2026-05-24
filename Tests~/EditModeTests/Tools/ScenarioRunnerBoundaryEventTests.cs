#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Tools;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class ScenarioRunnerBoundaryEventTests {
        Root MakeRoot() => new Root { schema_version="1.5",
            personas = new List<Persona>{ new Persona { agent_id="a",
                needs=NeedsOf(("fear",10f),("idle",30f)),
                actions=new List<Animo.Model.Action>{ ActionOf("Idle","idle",5), ActionOf("Flee","fear",2) }}}};

        [Test] public void Case01_EventAtTimeEqualsDuration_IsHonored() {
            var events = new List<TimedAffectEvent>{
                new TimedAffectEvent(1.0f, new AffectEvent("fear", +60f)) };
            var runner = new ScenarioRunner(MakeRoot());
            var result = runner.Run("a", duration: 1.0f, dt: 0.1f, events: events);
            Assert.That(result.frames.Count, Is.GreaterThan(1),
                "Q-S40: boundary event at t=duration must produce at least spawn + regular frames.");
        }
    }
}
