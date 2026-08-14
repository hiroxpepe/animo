#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class InitialBehaviorViewSyncTests {
        [Test] public void Case01_LiveZero_SetsBehavior_WithoutRaisingOnSignal() {
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("idle",50f)),
                actions = new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) }
            };
            var e = new Engine(p);
            int signals = 0;
            e.OnSignaled += _ => signals++;
            e.Live(0.0f);
            Assert.That(e.Behavior, Is.EqualTo("Idle"),
                "Q-S34: Live(0) must seed behavior to actions[0] via Q-S9.");
            Assert.That(signals, Is.EqualTo(0),
                "Q-S31: first transition must NOT raise OnSignaled.");
        }
    }
}
