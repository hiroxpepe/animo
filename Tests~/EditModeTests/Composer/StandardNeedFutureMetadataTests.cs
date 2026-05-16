// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Decision-table test for Q-S45 (v0.1.5): §3.5.2 PHASE C narrows
    /// the standard-Need skip to TIER ONLY (since §3.5 wins for tier
    /// per Q-S30) while letting other NeedMeta fields flow through
    /// ApplyNonTierMetadata for standard Needs too. v0.1.5 has no
    /// other NeedMeta fields, so this test is a placeholder Phase 3
    /// contract — when v0.2 adds a field like decay_multiplier, this
    /// test asserts that field is applied to standard Needs.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class StandardNeedFutureMetadataTests {
        [Test] public void Case01_FuturePerNeedMetadata_AppliesToStandardNeeds() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "When v0.2/v0.3 adds future NeedMeta fields (e.g. decay_multiplier), Engine " +
                "ctor PHASE C must apply them to standard Needs via ApplyNonTierMetadata. " +
                "v0.1.5: ApplyNonTierMetadata is a no-op (NeedMeta only has tier); this test " +
                "asserts the call site exists per Q-S45.");
        }
    }
}
