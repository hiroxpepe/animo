#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class HardLockStep4ContinuityTests {
        [Test] public void Case01_LockedFrames_UpdateActionScores_ForPostUnlockContinuity() {
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("fear",30f),("idle",10f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Flee","fear",2), ActionOf("Idle","idle",5) },
                rates = RatesOf(("fear",10f))
            };
            var e = new Engine(p);
            e.Live(0.0f); // seed behavior
            e.Lock(10f, LockMode.Hard);
            for (int i = 0; i < 5; i++) e.Live(0.1f); // fear rises; scores updated
            // Q-S62: Step 4 runs even during Hard lock
            float score = e.GetActionScore("Flee");
            Assert.That(score, Is.GreaterThan(0f),
                "Q-S62: Step 4 must update action scores even during Hard lock.");
        }
    }
}
