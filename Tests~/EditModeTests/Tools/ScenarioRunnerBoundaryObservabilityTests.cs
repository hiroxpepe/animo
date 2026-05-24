#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Tools;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class ScenarioRunnerBoundaryObservabilityTests {
        Root MakeRoot() => new Root { schema_version="1.5",
            personas = new List<Persona>{ new Persona { agent_id="a",
                needs=NeedsOf(("fear",10f),("idle",50f)),
                actions=new List<Animo.Model.Action>{ ActionOf("Idle","idle",5), ActionOf("Flee","fear",2) }}}};

        [Test] public void Case01_EventAtDuration_AppearsInFinalTraceFrame() {
            var events = new List<TimedAffectEvent>{
                new TimedAffectEvent(10.0f, new AffectEvent("fear", +50f)) };
            var runner = new ScenarioRunner(MakeRoot());
            var result = runner.Run("a", duration: 10.0f, dt: 0.1f, events: events);
            Assert.That(result.frames.Count, Is.GreaterThanOrEqualTo(101),
                "Q-S40: boundary event frame must be appended (total >= 101).");
        }
    }
}
