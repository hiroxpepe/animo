// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Decision-table test for Q-S12: when Composer fills a missing
    /// `Binding`, the resulting object's `thresholds` field MUST be a
    /// non-null (empty) `List&lt;Threshold&gt;`. Q-S7 hardened against
    /// `binding == null`; without Q-S12, the same NRE migrates one line
    /// down inside `Agent.Awake`'s `foreach (var t in binding.thresholds)`.
    ///
    /// Three-layer defense (spec §10.2.3 step 7, §16.5):
    ///   1. `Binding.thresholds` is non-nullable with `= new()` default
    ///      in `Scripts/Data.cs`.
    ///   2. Composer normalizes any null `thresholds` to empty list.
    ///   3. `Agent.Awake`'s sample uses `?? Array.Empty&lt;&gt;` defense
    ///      in depth for direct-construction Personas.
    ///
    /// This test pins layer 2's contract: post-Compose, `binding.thresholds`
    /// is observably non-null.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class BindingThresholdsDefaultFillTests {

        [Test] public void Case01_NullBinding_ComposerFillsBindingWithEmptyThresholdsList() {
            // Pre: persona has binding == null (will be default-filled by Q-S7).
            // Post: composed.binding != null AND composed.binding.thresholds
            //       is a non-null (likely empty) List<Threshold>. Awake's
            //       foreach over thresholds is then safe without ?? guard.
            Persona p = new Persona {
                agent_id = "a",
                kind_ids = new List<string>(),
                actions  = new List<Animo.Model.Action> { ActionOf(id: "X", need: "idle", tier: 5) },
                needs    = NeedsOf(("idle", 30f)),
                binding  = null
            };
            Root r = new Root { schema_version = "1.5", personas = new List<Persona> { p } };
            Persona composed = Composer.Compose(persona: p, root: r);
            Assert.That(composed.binding, Is.Not.Null,
                "Q-S7: Composer must fill missing binding");
            Assert.That(composed.binding!.thresholds, Is.Not.Null,
                "Q-S12: Composer-filled Binding must have non-null thresholds (empty list ok). " +
                "Otherwise Agent.Awake's foreach over binding.thresholds NREs — Q-S7 NRE migrates one line down.");
            Assert.That(composed.binding!.thresholds!.Count, Is.EqualTo(expected: 0),
                "Q-S12: default-filled thresholds is the empty list (no thresholds declared in JSON)");
        }
    }
}
