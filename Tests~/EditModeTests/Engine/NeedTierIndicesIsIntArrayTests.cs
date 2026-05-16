// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S69 (v0.1.5): `Engine._need_tier_indices`
    /// has the field type `Dictionary<int, int[]>`, not
    /// `Dictionary<int, List<int>>`. Pre-Q-S69 the §16.6 declaration
    /// said `int[]` but PHASE C ctor code wrote `List<int>` — type
    /// mismatch with the field declaration.
    ///
    /// Phase 3 contract: Reflection on Engine type shows
    /// `_need_tier_indices` as `Dictionary<int, int[]>`. The §16.1
    /// zero-alloc Hot Path rule mandates `int[]` for cache-friendly
    /// iteration during Step 4's `max_lower_tier_intensity` lookup.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class NeedTierIndicesIsIntArrayTests {
        [Test] public void Case01_NeedTierIndices_FieldTypeIsDictionaryOfIntArray() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Reflection on Engine type must show _need_tier_indices field of type " +
                "Dictionary<int, int[]>, not Dictionary<int, List<int>>. Q-S69 keeps int[] " +
                "for §16.1 zero-alloc Hot Path; ctor uses local List<int> scratch + " +
                "finalize-to-int[] pass.");
        }
    }
}
