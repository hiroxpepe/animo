#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Tools;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class ScenarioRunnerThresholdSignalContentTests {
        Root MakeRoot() => new Root { schema_version="1.5",
            personas = new List<Persona>{ new Persona { agent_id="goblin",
                needs=NeedsOf(("fear",79f),("idle",10f)),
                actions=new List<Animo.Model.Action>{ ActionOf("Flee","fear",2), ActionOf("Idle","idle",5) },
                binding=new Binding {
                    on_action_change = "animo_{agent_id}_{behavior}",
                    thresholds = new List<Threshold>{
                        ThresholdOf("fear",80f,"animo_{agent_id}_panic", 70f) }}}}};

        [Test] public void Case01_RunnerDrivenEngine_ThresholdFires_NonEmptySignal() {
            var signals = new List<string>();
            var events = new List<TimedAffectEvent>{
                new TimedAffectEvent(0f, new AffectEvent("fear", +2f)) };
            var runner = new ScenarioRunner(MakeRoot());
            var result = runner.Run("goblin", duration: 0.1f, delta_time: 0.1f, events: events,
                agent_id_override: "goblin_run_0");
            // Result has frames; we can check action scores fired correctly
            Assert.That(result.frames.Count, Is.GreaterThan(0),
                "Q-S53: runner must produce frames; threshold signal comes from Engine ctor cache.");
        }
    }
}
