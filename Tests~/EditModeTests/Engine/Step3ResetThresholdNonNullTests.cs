// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S86 (v0.1.5): Composer.Compose fills
    /// every Threshold's reset_threshold with a numeric value before
    /// returning. Engine.Step3 reads `t.reset_threshold!.Value`
    /// without null-coalescing in the hot path.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Step3ResetThresholdNonNullTests {
        [Test] public void Case01_ComposerOutput_AllThresholdsHaveNonNullResetThreshold() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "After Composer.Compose returns a composed Persona, every Threshold in " +
                "binding.thresholds[] must have reset_threshold != null. " +
                "Q-S11 + Q-S86 contract: author-omitted reset_threshold is filled with " +
                "Math.Max(0f, trigger_threshold - 5f). Engine.Step3 then reads " +
                "t.reset_threshold!.Value directly — null forgiving is safe per Q-S11.");
        }
    }
}
