// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Decision-table test for Q-S50 (v0.1.5): ScenarioRunner does NOT
    /// interact with Animo.Store. The runner maintains its own internal
    /// Dictionary<string, Engine> for routing Affect/Lock; Store.Register
    /// requires IAnimoAgent which the runner does not produce.
    ///
    /// Phase 3 contract: ScenarioRunner.Run() does NOT call Store.Register.
    /// Two runs from the same template must NOT contend with Store at all.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerStoreIndependenceTests {
        [Test] public void Case01_RunnerDoesNotCallStoreRegister() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "ScenarioRunner.Run() must NOT call Animo.Store.Instance.Register. The runner " +
                "maintains its own internal Dictionary<string, Engine>; Store remains the " +
                "Unity-Agent-only registry. Q-S50 type-system correction.");
        }
    }
}
