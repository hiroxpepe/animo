// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EdgeCaseTests {
    /// <summary>
    /// Decision-table test for Q-S27 (v0.1.5): Engine ctor MUST reserve
    /// fixed slots `0..STANDARD_NEEDS.Length-1` for the eight standard
    /// Needs regardless of what the Persona declares. Without this,
    /// Q-S16's static `Const.NEED_INDEX_FEAR=2` and `NEED_INDICES_BY_TIER`
    /// reads either misalign (cross-Need misread) or `IndexOutOfRange`
    /// for any Persona that omits standard Needs (e.g. peaceful villager
    /// with only `idle, curiosity, confidence`).
    ///
    /// Phase 3 implementation: Engine ctor performs the two-step assignment
    /// (reserve standard slots, then append non-standard). This test pins
    /// the resulting observable behavior: GetNeed("fear") returns 0 (the
    /// default for an unmentioned standard Need slot).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class StandardNeedFixedSlotTests {

        [Test] public void Case01_PersonaOmitsFear_GetNeedFearReturnsZero_NotIndexOutOfRange() {
            // Pre: peaceful villager Persona declaring ONLY non-fear,
            //      non-frustration Needs. Pre-Q-S27 the Engine ctor
            //      would assign indices in declaration order:
            //         { idle: 0, curiosity: 1, confidence: 2 }
            //      Reading _effective_needs[NEED_INDEX_FEAR=2] would
            //      return confidence's value (cross-Need misread in
            //      Maslow tier-2 suppression). Reading index 7 (frustration)
            //      would IndexOutOfRange.
            // Post-Q-S27: standard slots are reserved up front. Reading
            //             GetNeed("fear") returns 0 because Persona didn't
            //             declare fear (default), but the slot exists.
            Persona p = new Persona {
                agent_id = "villager",
                needs    = NeedsOf(("idle", 30f), ("curiosity", 50f), ("confidence", 70f)),
                actions  = new List<Animo.Model.Action> {
                    ActionOf(id: "Idle",   need: "idle",       tier: 5),
                    ActionOf(id: "Wander", need: "curiosity",  tier: 5),
                    ActionOf(id: "Brag",   need: "confidence", tier: 4)
                }
            };
            Engine engine = new Engine(persona: p);

            // Q-S27: omitted standard Needs read as 0, not as some other
            // Need's value, and no IndexOutOfRange.
            Assert.DoesNotThrow(code: () => engine.GetNeed(need: "fear"),
                "Q-S27: GetNeed must not throw IndexOutOfRange for an omitted standard Need");
            Assert.That(engine.GetNeed(need: "fear"), Is.EqualTo(expected: 0f),
                "Q-S27: omitted standard Need 'fear' must read as 0, not as confidence's value");
            Assert.That(engine.GetNeed(need: "frustration"), Is.EqualTo(expected: 0f),
                "Q-S27: omitted standard Need 'frustration' (index 7) must have a slot — pre-Q-S27 this would IndexOutOfRange");

            // Declared Needs read back correctly.
            Assert.That(engine.GetNeed(need: "confidence"), Is.EqualTo(expected: 70f),
                "Q-S27: declared standard Need 'confidence' (index 4 in the reserved layout) reads back correctly");
        }
    }
}
