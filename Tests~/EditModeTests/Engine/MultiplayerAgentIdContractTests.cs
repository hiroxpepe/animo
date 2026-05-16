// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S59 (v0.1.5): The agent_id override
    /// strategy is host-adapter's choice. Engine ctor accepts ANY
    /// runtime-unique string as agent_id post-override; Engine doesn't
    /// require GetInstanceID-style format. Test verifies a UUID-style
    /// override works identically to a GetInstanceID-style override.
    ///
    /// Phase 3 contract: Engine ctor with agent_id "goblin_uuid_a1b2c3d4"
    /// produces the same Bus payload format (template-expanded) as
    /// agent_id "goblin_47291"; both are runtime-unique strings.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class MultiplayerAgentIdContractTests {
        [Test] public void Case01_UuidStyleAgentId_ProducesValidBusPayload() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Engine ctor with agent_id like \"goblin_uuid_a1b2c3d4\" (Q-S59 multiplayer-" +
                "safe override) must produce template-expanded Bus payloads identical in " +
                "format to GetInstanceID-style agent_ids. Engine is content-agnostic; the " +
                "host adapter chooses the strategy.");
        }
    }
}
