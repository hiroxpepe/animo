#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Tools;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class ScenarioRunnerIntegerStepTests {
        Root MakeRoot() => new Root { schema_version="1.5",
            personas = new List<Persona>{ new Persona { agent_id="a",
                needs=NeedsOf(("idle",30f)),
                actions=new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) }}}};

        [Test] public void Case01_Run_ExecutesExactlyFloorDurationOverDtLiveCalls() {
            var runner = new ScenarioRunner(MakeRoot());
            var result = runner.Run("a", duration: 10.0f, dt: 0.1f);
            // 1 spawn frame + 100 loop frames = 101 frames total
            Assert.That(result.frames.Count, Is.EqualTo(101),
                "Q-S84: Run must produce exactly 100 Live(dt) calls (+ spawn frame = 101 frames).");
        }
        [Test] public void Case02_IEEE754_FloorWouldUnderShoot_RoundCorrects() {
            // float32 10.0f / 0.1f = 99.9999..., Math.Floor would give 99 steps (wrong).
            // Math.Round gives 100.
            var runner = new ScenarioRunner(MakeRoot());
            var result = runner.Run("a", duration: 10.0f, dt: 0.1f);
            Assert.That(result.frames.Count, Is.EqualTo(101),
                "Q-S98: System.Math.Round must give 100 steps, not 99 (floor bug).");
        }
    }
}
