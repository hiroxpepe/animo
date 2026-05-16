// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S68 (v0.1.5): `Animo.Agent` (the
    /// MonoBehaviour) declares `IAnimoAgent` in its base type list and
    /// implements `string agent_id { get; }`. Pre-Q-S68 the spec
    /// narrative said only "Animo.Agent : MonoBehaviour" without
    /// naming the interface — `Store.Register(IAnimoAgent agent)` call
    /// at Awake step (4) was a confirmed cannot-convert compile error.
    ///
    /// Phase 3 contract: typeof(Animo.Agent) implements IAnimoAgent;
    /// agent.agent_id returns the post-Q-S28-override runtime-unique
    /// value.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AgentImplementsIAnimoAgentTests {
        [Test] public void Case01_AgentType_ImplementsIAnimoAgentInterface() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "typeof(Animo.Agent) must implement IAnimoAgent. Reflection assertion: " +
                "typeof(Agent).GetInterfaces().Contains(typeof(IAnimoAgent)). " +
                "Q-S68 fix: class declaration `Agent : MonoBehaviour, IAnimoAgent` with " +
                "`public string agent_id => _composed_persona.agent_id`.");
        }
    }
}
