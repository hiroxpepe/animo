#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Tools;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class ScenarioRunnerT0EventVisibilityTests {
        Root MakeRoot() => new Root { schema_version="1.5",
            personas = new List<Persona>{ new Persona { agent_id="a",
                needs=NeedsOf(("fear",10f),("idle",30f)),
                actions=new List<Animo.Model.Action>{ ActionOf("Idle","idle",5), ActionOf("Flee","fear",2) }}}};

        [Test] public void Case01_T0Event_VisibleInFrameAtTimeZero() {
            var events = new List<TimedAffectEvent>{
                new TimedAffectEvent(0f, new AffectEvent("fear", +50f)) };
            var runner = new ScenarioRunner(MakeRoot());
            var result = runner.Run("a", duration: 1.0f, delta_time: 0.1f, events: events);
            // frames[0] is after t=0 Affect → fear should be 60
            float fear_at_spawn = result.frames[0].effective_needs.GetValueOrDefault("fear", 0f);
            Assert.That(fear_at_spawn, Is.EqualTo(60f).Within(0.1f),
                "Q-S55: t=0 event must be visible in spawn frame (frames[0]).");
        }
    }
}
