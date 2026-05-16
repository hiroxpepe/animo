// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Decision-table test for Q-S43 (v0.1.5): the (need, trigger_threshold)
    /// compound key compares trigger_threshold with EPSILON tolerance,
    /// not raw float ==. A Persona overriding `trigger_threshold: 80.0`
    /// with `80.0001` must produce ONE merged Threshold (Persona wins),
    /// not two near-duplicates.
    ///
    /// Phase 3 contract: Composer.MergeThresholds uses
    /// Math.Abs(a.trigger_threshold - b.trigger_threshold) < 0.5f
    /// (THRESHOLD_KEY_EPSILON), wider than IEEE-754 round-trip drift,
    /// narrower than authored milestone spacing (>= 5 by A035 / Q-S15).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ThresholdEpsilonMergeTests {
        [Test] public void Case01_PersonaOverrideWithDriftedFloat_CollapsesToOne() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Composer.MergeThresholds(kind_thresholds with trigger_threshold=80.0, " +
                "persona_thresholds with trigger_threshold=80.0001) must produce ONE merged " +
                "threshold (Persona's value wins). Pre-Q-S43 raw == created two duplicates " +
                "that both fired.");
        }
    }
}
