// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S80 (v0.1.5): `Agent` MonoBehaviour
    /// declares `Update()` method that calls `_engine.Live(dt: Time.deltaTime)`.
    /// Pre-Q-S80 NPCs would freeze after Awake — no Live(dt) on
    /// subsequent frames. Phase 3 contract verified at runtime once
    /// the Agent class implementation ships in Phase 3.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AgentUpdateMethodDeclaredTests {
        [Test] public void Case01_AgentClass_DeclaresUpdateMethod() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Animo.Agent must declare a private/protected Update() method that calls " +
                "_engine.Live(dt: Time.deltaTime). Q-S80 fix: per-frame engine driver " +
                "without which NPCs freeze after Awake. Verified at runtime once the Agent " +
                "class ships with full Phase 3 implementation.");
        }
    }
}
