// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Decision-table test for Q-S57 (v0.1.5): A038 orphan check
    /// includes rates as the 5th "in use" site. A pure-rate Need
    /// (e.g. poison decaying via rates only, read by UI without any
    /// Action/Influence/Threshold) does NOT trigger A038.
    ///
    /// Phase 3 contract: Validator.ValidateStage2 considers the union
    /// `needs[] ∪ actions[].need ∪ influences[].source/target ∪
    /// binding.thresholds[].need ∪ rates.keys()` when deciding whether
    /// needs_meta entry is "in use".
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A038_RatesOnlyNeedTests {
        [Test] public void Case01_NeedReferencedOnlyInRates_DoesNotTriggerA038Orphan() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Persona with `needs: [\"poison\"]`, `needs_meta: { poison: { tier: 1 } }`, " +
                "`rates: { poison: -0.5 }` and no actions/influences/thresholds referencing " +
                "poison must NOT emit A038 orphan Warning. Q-S57 fix: rates.keys() is the " +
                "5th 'in use' site.");
        }
    }
}
