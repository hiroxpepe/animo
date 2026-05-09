// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>Decision-table tests for Composer deep-copy semantics (spec §10.2).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class DeepCopyTests {

        [Test]
        public void Case01_ComposedPersonaIsDifferentReferenceFromInput() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k", actions = new List<Action> { ActionOf(id: "X", need: "fear", tier: 2) } } },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "k" } } }
            };
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(composed, Is.Not.SameAs(expected: root.personas[0]));
        }

        [Test]
        public void Case02_NeedsDictionaryIsDifferentReference() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k", actions = new List<Action> { ActionOf(id: "X", need: "fear", tier: 2) } } },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "k" }, needs = NeedsOf(("fear", 50f)) } }
            };
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(composed.needs, Is.Not.SameAs(expected: root.personas[0].needs));
        }

        [Test]
        public void Case03_ActionsListIsDifferentReference() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k", actions = new List<Action> { ActionOf(id: "X", need: "fear", tier: 2) } } },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "k" } } }
            };
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(composed.actions, Is.Not.SameAs(expected: root.kinds[0].actions));
        }

        [Test]
        public void Case04_ActionItemsAreDifferentReferences() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k", actions = new List<Action> { ActionOf(id: "X", need: "fear", tier: 2) } } },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "k" } } }
            };
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            Assert.That(composed.actions![0], Is.Not.SameAs(expected: root.kinds[0].actions![0]));
        }

        [Test]
        public void Case05_TwoComposedPersonasFromSameKindAreIndependent() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k", actions = new List<Action> { ActionOf(id: "X", need: "fear", tier: 2) } } },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "k" } },
                    new Persona { agent_id = "b", kind_ids = new List<string> { "k" } }
                }
            };
            Persona ca = Composer.Compose(persona: root.personas[0], root: root);
            Persona cb = Composer.Compose(persona: root.personas[1], root: root);
            Assert.That(ca.actions![0], Is.Not.SameAs(expected: cb.actions![0]));
        }

        [Test]
        public void Case06_MutatingComposedActionDoesNotAffectKindSource() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k", actions = new List<Action> { ActionOf(id: "X", need: "fear", tier: 2, exponent: 1f) } } },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "k" } } }
            };
            Persona composed = Composer.Compose(persona: root.personas[0], root: root);
            composed.actions![0].exponent = 999f;
            Assert.That(root.kinds[0].actions![0].exponent, Is.EqualTo(expected: 1f));
        }
    }
}
