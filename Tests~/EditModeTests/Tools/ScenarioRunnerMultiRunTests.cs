// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Decision-table test for Q-S42 (v0.1.5): two ScenarioRunner.Run()
    /// calls from the same template_id must NOT collide on Store.Register
    /// (Q-S6). Pre-Q-S42 the spec said "ScenarioRunner skips the override",
    /// hardcoding the runner to a single agent.
    ///
    /// Phase 3 contract: ScenarioRunner.Run() applies the runtime-unique
    /// override unconditionally, defaulting to "${template_id}_run_${seq++}";
    /// caller may pass agent_id_override for deterministic test names.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerMultiRunTests {
        [Test] public void Case01_TwoRunsFromSameTemplate_DoNotCollideOnStoreRegister() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Two ScenarioRunner.Run(agent_id: \"goblin\", ...) calls must produce two " +
                "distinct Store entries (auto-generated `goblin_run_0`, `goblin_run_1` per " +
                "Q-S42). Pre-Q-S42 the second call would collide and return a Store-disconnected " +
                "zombie.");
        }
    }
}
