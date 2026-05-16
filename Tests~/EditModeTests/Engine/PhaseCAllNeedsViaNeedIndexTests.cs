// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S66 (v0.1.5): Engine ctor PHASE C
    /// Step 3 iterates `_need_index` directly to call ApplyNonTier-
    /// Metadata for every Need. Pre-Q-S66 the code wrote
    /// `_composed_persona.needs.Count` and `_composed_persona.needs[idx]`
    /// — `Needs` class has neither property. Q-S56 self-introduced
    /// compile error.
    ///
    /// Phase 3 contract: Engine ctor with a Persona using only
    /// standard Needs (and no needs_meta) still calls
    /// ApplyNonTierMetadata once per Need.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PhaseCAllNeedsViaNeedIndexTests {
        [Test] public void Case01_StandardNeedsOnly_AllReceiveApplyNonTierMetadata() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Engine ctor with a Persona that uses only the 8 standard Needs and has " +
                "no needs_meta entry must still call ApplyNonTierMetadata 8 times (one per " +
                "Need in _need_index). Q-S66 fix: PHASE C Step 3 iterates _need_index map " +
                "(canonical 'every Need known to this Engine' built in PHASE A).");
        }
    }
}
