// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.BootstrapperTests {
    /// <summary>
    /// Decision-table test for Q-S58 (v0.1.5): AnimoBootstrapper.OnDestroy
    /// calls BOTH PersonaCache.ClearForTesting() AND
    /// Store.Instance.ResetForTesting(). Pre-Q-S58 only PersonaCache
    /// was cleared; under Unity Editor "Enter Play Mode Options (Fast)",
    /// stale Agent references in Store accumulated and corrupted Bus
    /// routing on subsequent Play sessions.
    ///
    /// Phase 3 contract: After AnimoBootstrapper.OnDestroy completes,
    /// both PersonaCache and Store.Instance are empty.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class BootstrapperStoreCleanupTests {
        [Test] public void Case01_OnDestroy_ClearsBothPersonaCacheAndStore() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "After AnimoBootstrapper.OnDestroy, Store.Instance must contain zero " +
                "registered agents AND PersonaCache must be empty. Q-S58 fix: pair " +
                "Store.ResetForTesting() with PersonaCache.ClearForTesting().");
        }
    }
}
