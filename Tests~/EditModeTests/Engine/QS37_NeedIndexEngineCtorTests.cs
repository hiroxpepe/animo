// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S37 (v0.1.5): Action.need_index and
    /// Threshold.need_index are populated in Engine ctor (post-DeepCopy),
    /// NOT in Composer. Two Personas declaring the same custom Needs in
    /// different orders must get DIFFERENT need_index values for those
    /// Needs (each reflecting its own Engine's array layout).
    ///
    /// Phase 3 contract: per §3.5.2 PHASE B, Engine ctor walks
    /// _composed_persona.actions and assigns action.need_index =
    /// _need_index[action.need] using the per-Persona _need_index map.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class NeedIndexEngineCtorTests {
        [Test] public void Case01_TwoPersonasDifferentNonStandardOrder_HaveDifferentNeedIndex() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Engine ctor PHASE B (§3.5.2) must populate Action.need_index from the " +
                "PER-PERSONA _need_index map (post-DeepCopy). Two Personas declaring " +
                "['oxygen', 'thirst'] vs ['thirst', 'oxygen'] must get DIFFERENT indices " +
                "for those Needs. Pre-Q-S37 a Composer-side bake would share one layout " +
                "across both Engines.");
        }
    }
}
