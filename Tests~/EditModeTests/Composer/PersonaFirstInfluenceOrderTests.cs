// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Decision-table test for Q-S20: §8.3 `influences` merge is now
    /// symmetric with the Q-S19 `actions` change — Persona-first
    /// preserve, append unmatched Kind influences at the tail. This is
    /// what makes §9.6.2's stable topological sort deterministic for
    /// independent edges that share a target Need.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PersonaFirstInfluenceOrderTests {

        [Test] public void Case01_PersonaInfluenceFirst_KindAppendedSecond_OrderPreserved() {
            // Pre: Persona has [Y→C], Kind has [X→C] (independent edges
            //      both writing to the same target). Composed influences[]
            //      must put Persona's first, Kind's second.
            // Post-Q-S20: Stable topo sort + Persona-first composed order
            //             gives a deterministic apply order: Y→C then X→C.
            Kind k = new Kind {
                kind_id = "amplifier",
                actions = new List<Animo.Model.Action> { ActionOf(id: "X", need: "fear", tier: 2) },
                influences = new List<Influence> {
                    InfluenceOf(source: "fatigue", target: "fear", coefficient: +0.3f)  // Kind's edge
                }
            };
            Persona p = new Persona {
                agent_id = "a",
                kind_ids = new List<string> { "amplifier" },
                needs    = NeedsOf(("fatigue", 50f), ("hunger", 50f), ("fear", 50f)),
                influences = new List<Influence> {
                    InfluenceOf(source: "hunger", target: "fear", coefficient: +0.5f)  // Persona's edge
                }
            };
            Root r = new Root {
                schema_version = "1.5",
                kinds          = new List<Kind> { k },
                personas       = new List<Persona> { p }
            };
            Persona composed = Composer.Compose(persona: p, root: r);
            string[] sources = composed.influences!.Select(i => i.source).ToArray();
            Assert.That(sources, Is.EqualTo(expected: new[] { "hunger", "fatigue" }),
                "Q-S20: composed influences[] must preserve Persona's authored order " +
                "(hunger→fear first because Persona declared it); Kind's fatigue→fear " +
                "appends at the tail. This is the deterministic key for stable topo " +
                "sort when independent edges share a target.");
        }
    }
}
