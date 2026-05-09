// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>Decision-table tests for missing-Need fill (spec §8.8).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class MissingNeedFillTests {

        [Test]
        public void Case01_RatesKeyMissingFromNeeds_FillsToZero() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k",
                    rates = RatesOf(("hunger", 2.0f)),
                    actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } } },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "k" },
                        needs = NeedsOf(("idle", 30f)) }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.needs!.values.ContainsKey(key: "hunger"), Is.True);
            Assert.That(c.needs!.values["hunger"], Is.EqualTo(expected: 0.0f));
        }

        [Test]
        public void Case02_InfluenceSourceMissingFromNeeds_FillsToZero() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k",
                    influences = new List<Influence> { InfluenceOf(source: "fear", target: "confidence", coefficient: -0.6f) },
                    actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } } },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "k" },
                        needs = NeedsOf(("idle", 30f)) }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.needs!.values["fear"],       Is.EqualTo(expected: 0.0f));
            Assert.That(c.needs!.values["confidence"], Is.EqualTo(expected: 0.0f));
        }

        [Test]
        public void Case03_ActionNeedMissingFromNeeds_FillsToZero() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k",
                    actions = new List<Action> {
                        ActionOf(id: "Flee", need: "fear", tier: 2),
                        ActionOf(id: "Idle", need: "idle", tier: 5) } } },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "k" },
                        needs = NeedsOf(("idle", 30f)) }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.needs!.values["fear"], Is.EqualTo(expected: 0.0f));
        }

        [Test]
        public void Case04_NeedsAllPresent_NoExtraKeysAdded() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k",
                    actions = new List<Action> { ActionOf(id: "X", need: "fear", tier: 2) } } },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "k" },
                        needs = NeedsOf(("fear", 50f)) }
                }
            };
            Persona c = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(c.needs!.values.Count, Is.EqualTo(expected: 1));
        }
    }
}
