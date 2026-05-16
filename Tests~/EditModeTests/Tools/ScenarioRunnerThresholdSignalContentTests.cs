// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Decision-table test for Q-S53 (v0.1.5): ScenarioRunner-driven
    /// Engine has Threshold.expanded_trigger populated. Pre-Q-S53 the
    /// initialization loop ran in Agent.Awake (which Runner doesn't
    /// invoke), so every fired signal was the empty string.
    ///
    /// Phase 3 contract: Engine ctor populates Threshold.expanded_trigger
    /// for every Threshold in binding.thresholds. ScenarioRunner-driven
    /// Engine produces non-empty signal_id when Thresholds fire.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerThresholdSignalContentTests {
        [Test] public void Case01_RunnerDrivenEngine_ThresholdFires_NonEmptySignal() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "When ScenarioRunner.Run triggers a Threshold (e.g. fear crosses 80), the " +
                "OnSignal payload must be the template-expanded string (e.g. " +
                "`animo_goblin_run_0_panic`), not the empty string. Q-S53 fix: cache " +
                "initialization in Engine ctor instead of Agent.Awake.");
        }
    }
}
