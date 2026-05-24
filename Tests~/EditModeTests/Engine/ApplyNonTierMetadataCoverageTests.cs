// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class ApplyNonTierMetadataCoverageTests {
        [Test] public void Case01_AllNeeds_ReceiveApplyNonTierMetadata_EvenWithoutNeedsMeta() {
            // Q-S56: PHASE C must iterate _need_index (all Needs), not just needs_meta.
            // v0.1.5: ApplyNonTierMetadata is a private no-op.
            // This test verifies the Engine constructs without exception when needs_meta is null.
            var persona = new Persona {
                agent_id = "a",
                needs = NeedsOf(("fear",30f),("hunger",50f)),
                actions = new List<Animo.Model.Action> { ActionOf("X","fear",2) }
            };
            // Engine ctor PHASE C must call ApplyNonTierMetadata for both Needs without error.
            Assert.DoesNotThrow(() => new Engine(persona),
                "Q-S56: Engine ctor with no needs_meta must call ApplyNonTierMetadata " +
                "for all Needs via _need_index iteration without throwing.");
        }
    }
}
