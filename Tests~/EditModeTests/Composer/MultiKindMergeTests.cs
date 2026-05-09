// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>Decision-table tests for multi-kind composition (spec §8).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class MultiKindMergeTests {

        [Test]
        public void Case01_TwoKindsMergeInOrder_LastWins() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> {
                    new Kind { kind_id = "first",  rates = RatesOf(("hunger", 1.0f)),
                        actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } },
                    new Kind { kind_id = "second", rates = RatesOf(("hunger", 5.0f)) },
                },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "first", "second" } } }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.rates!.values["hunger"], Is.EqualTo(expected: 5.0f));
        }

        [Test]
        public void Case02_ThreeKindsCascade_PersonaWinsLast() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> {
                    new Kind { kind_id = "a", commitment = new Commitment { bonus = 1f },
                        actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } },
                    new Kind { kind_id = "b", commitment = new Commitment { bonus = 2f } },
                    new Kind { kind_id = "c", commitment = new Commitment { bonus = 3f } },
                },
                personas = new List<Persona> {
                    new Persona { agent_id = "p", kind_ids = new List<string> { "a", "b", "c" },
                        commitment = new Commitment { bonus = 99f } }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.commitment!.bonus, Is.EqualTo(expected: 99f));
        }

        [Test]
        public void Case03_OrderMattersForActionsByMatchingId() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> {
                    new Kind { kind_id = "first",  actions = new List<Action> { ActionOf(id: "Patrol", need: "idle", tier: 5, exponent: 1.0f) } },
                    new Kind { kind_id = "second", actions = new List<Action> { ActionOf(id: "Patrol", need: "idle", tier: 5, exponent: 2.5f) } },
                },
                personas = new List<Persona> { new Persona { agent_id = "p", kind_ids = new List<string> { "first", "second" } } }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.actions, Has.Count.EqualTo(expected: 1));
            Assert.That(c.actions![0].exponent, Is.EqualTo(expected: 2.5f));
        }

        [Test]
        public void Case04_DistinctActionIdsFromMultipleKindsAccumulate() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> {
                    new Kind { kind_id = "monster",  actions = new List<Action> { ActionOf(id: "Roar", need: "fear", tier: 2) } },
                    new Kind { kind_id = "predator", actions = new List<Action> { ActionOf(id: "Hunt", need: "hunger", tier: 1) } },
                    new Kind { kind_id = "boss",     actions = new List<Action> { ActionOf(id: "Heal", need: "fatigue", tier: 1) } },
                },
                personas = new List<Persona> { new Persona { agent_id = "p", kind_ids = new List<string> { "monster", "predator", "boss" } } }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.actions, Has.Count.EqualTo(expected: 3));
        }

        [Test]
        public void Case05_KindOnlyAttributes_StillContributeToCascade() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> {
                    new Kind { kind_id = "base",       actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } },
                    new Kind { kind_id = "rates_only", rates = RatesOf(("hunger", 7.0f)) },
                },
                personas = new List<Persona> { new Persona { agent_id = "p", kind_ids = new List<string> { "base", "rates_only" } } }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.rates!.values["hunger"], Is.EqualTo(expected: 7.0f));
            Assert.That(c.actions, Has.Count.EqualTo(expected: 1));
        }

        [Test]
        public void Case06_EmptyKindIds_BehavesLikePersonaOnly() {
            Root root = new Root {
                schema_version = "1.4",
                personas = new List<Persona> {
                    new Persona { agent_id = "a",
                        actions = new List<Action> { ActionOf(id: "X", need: "fear", tier: 2) } }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.actions, Has.Count.EqualTo(expected: 1));
        }
    }
}
