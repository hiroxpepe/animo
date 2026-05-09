// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>Decision-table tests for Composer cascading (spec §8.3, §10.3).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class KindCascadeTests {

        [Test]
        public void Case01_PersonaWithoutKindIdsKeepsOwnFields() {
            Root root = new Root {
                schema_version = "1.4",
                personas = new List<Persona> {
                    new Persona { agent_id = "a",
                        actions = new List<Action> { ActionOf(id: "X", need: "fear", tier: 2, exponent: 2.0f) } }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.actions, Has.Count.EqualTo(expected: 1));
            Assert.That(c.actions![0].exponent, Is.EqualTo(expected: 2.0f));
        }

        [Test]
        public void Case02_KindActionsCascadeIntoPersona() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k", actions = new List<Action> { ActionOf(id: "Patrol", need: "idle", tier: 5) } } },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "k" } } }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.actions, Has.Count.EqualTo(expected: 1));
            Assert.That(c.actions![0].id, Is.EqualTo(expected: "Patrol"));
        }

        [Test]
        public void Case03_PersonaActionOverridesKindActionByMatchingId() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k", actions = new List<Action> { ActionOf(id: "Patrol", need: "idle", tier: 5, exponent: 1.0f) } } },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "k" },
                        actions = new List<Action> { ActionOf(id: "Patrol", need: "idle", tier: 5, exponent: 3.0f) } }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.actions![0].exponent, Is.EqualTo(expected: 3.0f));
        }

        [Test]
        public void Case04_PersonaActionWithNewIdAppendsToKindActions() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k", actions = new List<Action> { ActionOf(id: "Patrol", need: "idle", tier: 5) } } },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "k" },
                        actions = new List<Action> { ActionOf(id: "Special", need: "fear", tier: 2) } }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.actions, Has.Count.EqualTo(expected: 2));
        }

        [Test]
        public void Case05_PersonaCommitmentBonusOverridesKind() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k",
                    actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) },
                    commitment = new Commitment { bonus = 5f } } },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "k" },
                        commitment = new Commitment { bonus = 20f } }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.commitment!.bonus, Is.EqualTo(expected: 20f));
        }

        [Test]
        public void Case06_PersonaRatesMergePerKey() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k",
                    actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) },
                    rates = RatesOf(("hunger", 1.0f), ("fear", -2.0f)) } },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "k" },
                        rates = RatesOf(("hunger", 5.0f)) }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.rates!.values["hunger"], Is.EqualTo(expected: 5.0f));
            Assert.That(c.rates!.values["fear"],   Is.EqualTo(expected: -2.0f));
        }

        [Test]
        public void Case07_PersonaSuppressionOverridesKind() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k",
                    actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) },
                    suppression = new Suppression { tier2 = 0.3f } } },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "k" },
                        suppression = new Suppression { tier2 = 0.7f } }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.suppression!.tier2, Is.EqualTo(expected: 0.7f));
        }

        [Test]
        public void Case08_PersonaInfluenceOverridesByMatchingSourceTarget() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k",
                    actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) },
                    influences = new List<Influence> { InfluenceOf(source: "fear", target: "confidence", coefficient: -0.6f) } } },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "k" },
                        influences = new List<Influence> { InfluenceOf(source: "fear", target: "confidence", coefficient: -0.9f) } }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.influences![0].coefficient, Is.EqualTo(expected: -0.9f));
        }
    }
}
