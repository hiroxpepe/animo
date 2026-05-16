// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S56 (v0.1.5): Engine ctor PHASE C
    /// calls ApplyNonTierMetadata for EVERY Need in composed needs[],
    /// not just those listed in needs_meta. Pre-Q-S56 a Persona with
    /// no needs_meta ran zero ApplyNonTierMetadata calls.
    ///
    /// Phase 3 contract: When v0.2 adds NeedMeta fields, all Needs
    /// receive default-or-explicit metadata via the universal pass.
    /// v0.1.5 ApplyNonTierMetadata is no-op so this test asserts the
    /// pass structure (call count == composed needs[].Count) via a
    /// test-only counter or instrumentation.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ApplyNonTierMetadataCoverageTests {
        [Test] public void Case01_AllNeeds_ReceiveApplyNonTierMetadata_EvenWithoutNeedsMeta() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Engine ctor with a Persona that has no needs_meta must still call " +
                "ApplyNonTierMetadata for every Need in composed needs[]. Q-S56 fix: " +
                "PHASE C Step 3 iterates needs[], not needs_meta, with NeedMeta.DefaultFor " +
                "supplying per-Need defaults.");
        }
    }
}
