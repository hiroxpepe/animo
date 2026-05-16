// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table tests for Q-S54 (v0.1.5): GetNeed returns
    /// effective_needs (post-Influence-cascade per Q-S23); GetBaseNeed
    /// returns base _needs (pre-cascade). Pre-Q-S54 the spec said
    /// "current value" without disambiguation, leaving inspector tools
    /// unable to reason about cascade-driven behavior.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class GetNeedSemanticsTests {
        [Test] public void Case01_GetNeed_ReturnsEffective_AfterInfluenceCascade() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "With Influence amplifying fear from base 30 to effective 80, " +
                "Engine.GetNeed(\"fear\") must return 80 (the value Step 4 actually consumes), " +
                "not 30. Q-S54 fix: GetNeed reads _effective_needs.");
        }

        [Test] public void Case02_GetBaseNeed_ReturnsBase_BypassingCascade() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "GetBaseNeed(\"fear\") must return the base (pre-cascade) value (e.g. 30 in " +
                "the Case01 setup), giving inspector tools a way to read both layers.");
        }
    }
}
