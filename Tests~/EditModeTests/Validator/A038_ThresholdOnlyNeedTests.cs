// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Decision-table test for Q-S49 (v0.1.5): A038 orphan check broadens
    /// to 4 sites — needs[], actions[].need, influences[].source/target,
    /// AND binding.thresholds[].need. A Need used signal-only via Threshold
    /// (e.g. oxygen → UI alert with no scoring action) must NOT trigger
    /// A038 orphan Warning. Pre-Q-S49 thresholds were missing from the
    /// "in use" list.
    ///
    /// Phase 3 contract: Validator.ValidateStage2 considers Threshold need
    /// references when deciding whether needs_meta entry is "in use".
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A038_ThresholdOnlyNeedTests {
        [Test] public void Case01_NeedUsedOnlyInThreshold_DoesNotTriggerA038Orphan() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Persona with needs_meta { oxygen: tier:1 } and binding.thresholds[].need = oxygen " +
                "(but no actions/influences referencing oxygen) must NOT emit A038 orphan Warning. " +
                "Q-S49 fix: thresholds[].need is the 4th 'in use' site.");
        }
    }
}
