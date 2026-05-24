#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class PhaseCAllNeedsViaNeedIndexTests {
        [Test] public void Case01_StandardNeedsOnly_AllReceiveApplyNonTierMetadata() {
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("fear",30f),("idle",50f)),
                actions = new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) }
            };
            // Q-S56: PHASE C iterates _need_index, calls ApplyNonTierMetadata for all.
            // v0.1.5: no-op. Verify ctor completes without error.
            Assert.DoesNotThrow(() => new Engine(p),
                "Q-S56/Q-S66: PHASE C via _need_index must not throw for standard Needs.");
        }
    }
}
