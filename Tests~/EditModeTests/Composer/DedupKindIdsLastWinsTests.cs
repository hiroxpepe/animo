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
    /// Decision-table tests for Composer dedup of duplicate kind_ids (v0.1.5, Q7).
    /// Spec §8.3 mandates last-wins cascade. When `["goblin","scout","goblin"]`
    /// is dedup'd, the LAST `goblin` must win — equivalent to processing
    /// `["scout","goblin"]` — not the first, which would be `["goblin","scout"]`
    /// (where scout would override goblin, the opposite of what the JSON
    /// literally requested).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class DedupKindIdsLastWinsTests {

        [Test] public void Case01_DuplicateLast_WinsOverIntermediate() {
            // Setup: goblin and scout define the same action id "X" with different
            // exponents. kind_ids = ["goblin","scout","goblin"] — dedup must keep
            // the LAST goblin so its exponent overrides scout's.
            Root root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> {
                    new Kind { kind_id = "goblin",
                        actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5, exponent: 1.0f) } },
                    new Kind { kind_id = "scout",
                        actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5, exponent: 2.5f) } }
                },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "goblin", "scout", "goblin" } }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            // Expected: goblin (last) wins → exponent = 1.0f
            // If implementation keeps the FIRST occurrence, scout wins → exponent = 2.5f
            Assert.That(c.actions![0].exponent, Is.EqualTo(expected: 1.0f),
                "kind_ids dedup must keep LAST occurrence to preserve last-wins cascade (spec §8.3)");
        }

        [Test] public void Case02_AdjacentDuplicate_BehavesLikeSingle() {
            // ["goblin","goblin"] dedup is trivially the same as ["goblin"].
            Root root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> { new Kind { kind_id = "goblin",
                    actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5, exponent: 3.0f) } } },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "goblin", "goblin" } }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.actions![0].exponent, Is.EqualTo(expected: 3.0f));
        }

        [Test] public void Case03_NonAdjacentDuplicate_LastPositionDominates() {
            // ["a","b","c","a"] → effective order ["b","c","a"]; "a" applied last
            Root root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> {
                    new Kind { kind_id = "a", commitment = new Commitment { bonus = 1f },
                        actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } },
                    new Kind { kind_id = "b", commitment = new Commitment { bonus = 2f } },
                    new Kind { kind_id = "c", commitment = new Commitment { bonus = 3f } },
                },
                personas = new List<Persona> {
                    new Persona { agent_id = "p", kind_ids = new List<string> { "a", "b", "c", "a" } }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            // "a" is dedup'd to its last position → applied after b and c → bonus = 1f wins
            Assert.That(c.commitment!.bonus, Is.EqualTo(expected: 1f));
        }
    }
}
