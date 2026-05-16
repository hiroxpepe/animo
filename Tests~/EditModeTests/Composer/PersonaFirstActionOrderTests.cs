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
    /// Decision-table tests for Q-S19: §8.3 `actions` merge is now
    /// "Persona-first preserve, then append unmatched Kind ids" (was
    /// "Kind-first append"). The LLM's authored Persona.actions[] order
    /// — including index 0 — survives composition. Q-S9's
    /// declaration-order tie-break finally has the input it always
    /// assumed.
    ///
    /// Pre-Q-S19: persona [Idle, Flee] + kind [Flee, Eat] → composed
    ///            [Flee, Eat, Idle] — Idle exiled to tail; Flee at
    ///            index 0; Q-S9 picks Flee against LLM intent.
    /// Post-Q-S19: same input → composed [Idle, Flee, Eat] — Idle
    ///             at index 0 as the LLM intended.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PersonaFirstActionOrderTests {

        [Test] public void Case01_PersonaFirstAtIndexZero_KindOverlap_PersonaIndexZeroPreserved() {
            // Pre: Persona declares [Idle, Flee]; Kind declares [Flee, Eat].
            // Post-Q-S19: composed = [Idle, Flee, Eat]; index 0 is Idle.
            Kind k = new Kind {
                kind_id = "creature",
                actions = new List<Animo.Model.Action> {
                    ActionOf(id: "Flee", need: "fear",  tier: 2),
                    ActionOf(id: "Eat",  need: "hunger", tier: 1)
                }
            };
            Persona p = new Persona {
                agent_id = "a",
                kind_ids = new List<string> { "creature" },
                actions  = new List<Animo.Model.Action> {
                    ActionOf(id: "Idle", need: "idle", tier: 5),
                    ActionOf(id: "Flee", need: "fear", tier: 2)
                },
                needs = NeedsOf(("idle", 30f), ("fear", 30f), ("hunger", 30f))
            };
            Root r = new Root {
                schema_version = "1.5",
                kinds          = new List<Kind> { k },
                personas       = new List<Persona> { p }
            };
            Persona composed = Composer.Compose(persona: p, root: r);
            string[] ids = composed.actions.Select(a => a.id).ToArray();
            Assert.That(ids, Is.EqualTo(expected: new[] { "Idle", "Flee", "Eat" }),
                "Q-S19: composed actions[] must preserve Persona's declared order; " +
                "unmatched Kind ids append at the tail");
            Assert.That(composed.actions[0].id, Is.EqualTo(expected: "Idle"),
                "Q-S19: LLM-authored index-0 default must remain at index 0 — " +
                "this is what makes Q-S9 declaration-order tie-break honest");
        }

        [Test] public void Case02_PersonaFlee_KindFleeEat_ComposedIsIdleFleeEat() {
            // Pre: Persona declares only [Idle]; Kind declares [Flee, Eat].
            // Post-Q-S19: composed = [Idle, Flee, Eat] — Persona's single
            //              entry first, Kind contributions append.
            Kind k = new Kind {
                kind_id = "creature",
                actions = new List<Animo.Model.Action> {
                    ActionOf(id: "Flee", need: "fear",  tier: 2),
                    ActionOf(id: "Eat",  need: "hunger", tier: 1)
                }
            };
            Persona p = new Persona {
                agent_id = "a",
                kind_ids = new List<string> { "creature" },
                actions  = new List<Animo.Model.Action> {
                    ActionOf(id: "Idle", need: "idle", tier: 5)
                },
                needs = NeedsOf(("idle", 30f), ("fear", 30f), ("hunger", 30f))
            };
            Root r = new Root { schema_version = "1.5", kinds = new List<Kind> { k }, personas = new List<Persona> { p } };
            Persona composed = Composer.Compose(persona: p, root: r);
            string[] ids = composed.actions.Select(a => a.id).ToArray();
            Assert.That(ids, Is.EqualTo(expected: new[] { "Idle", "Flee", "Eat" }),
                "Q-S19: Persona [Idle] alone produces [Idle] first, Kind [Flee, Eat] appended");
        }
    }
}
