// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;
using Animo;

namespace Animo.Tests.EditMode.EdgeCaseTests {
    /// <summary>
    /// Decision-table tests for Q-S16: `Animo.Const` must publish runtime
    /// maps `NEED_TIER_BY_NAME` and `NEED_INDICES_BY_TIER` so that the
    /// §9.3.4 `max_lower_tier_intensity` formula has an implementable data
    /// source. Pre-Q-S16 the §3.5 table existed only as documentation;
    /// the Engine had no way to read tier membership.
    ///
    /// Key invariants:
    ///   - All eight standard Needs appear in NEED_TIER_BY_NAME with
    ///     their §3.5 tiers.
    ///   - frustration sits at tier 2 alongside fear (not orphaned),
    ///     so it participates in suppression even when used only via
    ///     `influences` (§25.5.2 pattern).
    ///   - Non-standard Needs are NOT in either map (excluded from
    ///     suppression rather than defaulted to a tier).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class NeedTierMapTests {

        [Test] public void Case01_AllStandardNeedsMappedToTiers() {
            // The §3.5 table: hunger=1, fatigue=1, fear=2, frustration=2,
            // loneliness=3, confidence=4, curiosity=5, idle=5
            Assert.That(Const.NEED_TIER_BY_NAME["hunger"],      Is.EqualTo(expected: 1));
            Assert.That(Const.NEED_TIER_BY_NAME["fatigue"],     Is.EqualTo(expected: 1));
            Assert.That(Const.NEED_TIER_BY_NAME["fear"],        Is.EqualTo(expected: 2));
            Assert.That(Const.NEED_TIER_BY_NAME["frustration"], Is.EqualTo(expected: 2),
                "Q-S16: frustration must be tier 2 alongside fear (§3.5)");
            Assert.That(Const.NEED_TIER_BY_NAME["loneliness"],  Is.EqualTo(expected: 3));
            Assert.That(Const.NEED_TIER_BY_NAME["confidence"],  Is.EqualTo(expected: 4));
            Assert.That(Const.NEED_TIER_BY_NAME["curiosity"],   Is.EqualTo(expected: 5));
            Assert.That(Const.NEED_TIER_BY_NAME["idle"],        Is.EqualTo(expected: 5));
            Assert.That(Const.NEED_TIER_BY_NAME.Count, Is.EqualTo(expected: 8),
                "Q-S16: only the 8 standard Needs are mapped; non-standard Needs are excluded");
        }

        [Test] public void Case02_NeedIndicesByTier_FrustrationAtTier2_NotIsolated() {
            // The hot-path inverse map: tier → int[] of need indices.
            // Tier 2 must contain BOTH fear and frustration; if frustration
            // were missing here, §25.5.2's "frustration via influences only"
            // pattern would not suppress upper tiers — the original Q-S16
            // worry from Gemini's 11th review.
            //
            // (v0.1.5, Q-S128) Type updated to IReadOnlyList<int> per
            // Q-S128 read-only hardening of NEED_INDICES_BY_TIER. Pre-Q-S128
            // returned mutable int[] which let external code corrupt the
            // shared Const map; Q-S128 wraps each entry with
            // Array.AsReadOnly. Test semantics unchanged.
            System.Collections.Generic.IReadOnlyList<int> tier2 = Const.NEED_INDICES_BY_TIER[2];
            Assert.That(tier2, Does.Contain(Const.NEED_INDEX_FEAR),
                "Q-S16: tier 2 includes fear");
            Assert.That(tier2, Does.Contain(Const.NEED_INDEX_FRUSTRATION),
                "Q-S16: tier 2 MUST include frustration even when frustration has no Action — " +
                "otherwise the §25.5.2 'frustration via influences only' pattern fails to suppress upper tiers");
            Assert.That(tier2.Count, Is.EqualTo(expected: 2),
                "Q-S16: tier 2 currently holds exactly fear and frustration");
        }
    }
}
