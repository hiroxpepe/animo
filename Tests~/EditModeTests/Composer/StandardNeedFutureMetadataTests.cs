#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.ComposerTests {
    [TestFixture]
    public class StandardNeedFutureMetadataTests {
        [Test] public void Case01_FuturePerNeedMetadata_AppliesToStandardNeeds() {
            // Q-S45: PHASE C calls ApplyNonTierMetadata for all Needs.
            // v0.1.5: no-op. Verify Engine ctor runs PHASE C without error.
            var p = new Persona { agent_id = "a",
                needs = NeedsOf(("fear",30f),("idle",50f)),
                actions = new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) }
            };
            Assert.DoesNotThrow(() => new Engine(p),
                "Q-S45: PHASE C ApplyNonTierMetadata call site must exist and not throw.");
        }
    }
}
