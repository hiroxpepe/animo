// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S34 (v0.1.5): Agent.Awake must call
    /// Engine.Live(dt: 0.0f) once after wiring OnSignal so the host
    /// can read _engine.behavior and play the initial Animator state
    /// directly. Q-S31 silences OnSignal for the first transition,
    /// so without Q-S34 the host cannot know what Action to play
    /// on spawn — characters T-pose.
    ///
    /// Phase 3 contract: After `_engine.Live(dt: 0.0f)`, `_engine.behavior`
    /// returns the chosen actions[0] (Q-S9 tie-break), and OnSignal was
    /// NOT raised during that call.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class InitialBehaviorViewSyncTests {
        [Test] public void Case01_LiveZero_SetsBehavior_WithoutRaisingOnSignal() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Engine.Live(dt: 0.0f) must seed behavior to actions[0] (Q-S9 tie-break) " +
                "AND keep OnSignal silent (Q-S31). See §11.4 sequence Mermaid + §11.4.1 " +
                "Awake code for the Q-S34 contract.");
        }
    }
}
