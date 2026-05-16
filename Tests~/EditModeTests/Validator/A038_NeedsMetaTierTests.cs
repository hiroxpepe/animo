// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Decision-table tests for Q-S30 / Validator rule A038 (v0.1.5):
    /// `needs_meta[need].tier` must be in [1, 5]; out-of-range ⇒ Error.
    /// Reference to a Need not declared in `needs` ⇒ Warning.
    ///
    /// Phase 3 contract: Persona gains `needs_meta: Dictionary<string, NeedMeta>`
    /// and a new NeedMeta type with `int tier`. Validator A038 fires per the
    /// rules above. Test bodies are Phase 3 work; this fixture pins the spec
    /// expectation in test form (Red baseline).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A038_NeedsMetaTierTests {

        [Test] public void Case01_TierOutOfRange_Errors() {
            // Phase 3: needs_meta { oxygen: { tier: 99 } } → A038 Error.
            // Pinned in spec §3.5.2 + §13.1 (A038 row).
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Persona.needs_meta + NeedMeta type + Validator A038 rule. " +
                "See §3.5.2 (Q-S30) and §13.1 A038 row for the contract.");
        }

        [Test] public void Case02_ValidTier_NoA038() {
            // Phase 3: needs_meta { oxygen: { tier: 1 } } → no A038.
            Assert.Fail(message: "Phase 3 implementation pending — see §3.5.2 (Q-S30).");
        }
    }
}
