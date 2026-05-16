// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Decision-table test for Q-S39 (v0.1.5): A019 (typo Warning) runs
    /// in Stage 2 against the COMPOSED Persona, so it sees the merged
    /// `needs_meta`. A Persona declaring `needs_meta { oxygen: tier:1 }`
    /// over a Kind whose actions[] use `oxygen` must NOT trigger A019.
    /// Pre-Q-S39 this was a Stage 1 false positive.
    ///
    /// Phase 3 contract: Validator.ValidateStage2 runs A019 against
    /// composed Need names; suppresses for any name in composed needs_meta.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A019_Stage2NeedsMetaTests {
        [Test] public void Case01_PersonaNeedsMetaSuppressesKindOriginatedTypo() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Validator.ValidateStage2 must run A019 against the COMPOSED Persona " +
                "(post-Composer), so a Persona's needs_meta can suppress the typo " +
                "Warning for Need names that originated in a Kind's actions[]. " +
                "See §13.1 A019 row + §13.2 'Why A019 in Stage 2' (Q-S39).");
        }
    }
}
