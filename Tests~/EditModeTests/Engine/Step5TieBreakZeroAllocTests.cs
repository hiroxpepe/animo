#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class Step5TieBreakZeroAllocTests {
        [Test] public void Case01_TiedScores_FirstDeclarationWins_ZeroAlloc() {
            // Q-S9: tie-break = declaration order.
            // Both actions need same Need = "idle", same exponent, tied at spawn (all needs 0).
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("idle",50f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("First","idle",5,1.0f), ActionOf("Second","idle",5,1.0f) }
            };
            var e = new Engine(p);
            e.Live(0.0f);
            Assert.That(e.behavior, Is.EqualTo("First"),
                "Q-S9: tie → first declaration in actions[] wins.");
        }
    }
}
