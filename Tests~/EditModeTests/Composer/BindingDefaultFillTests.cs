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
    /// Decision-table tests for Composer's binding-default-fill behavior
    /// (v0.1.5, Q-S7). When the input persona has `binding == null`, the
    /// composed Persona must have a non-null `binding` whose template
    /// strings are the engine defaults from `Animo.Const`. This guarantees
    /// that `Agent.Awake`'s String Cache (spec §16.4) cannot crash with
    /// NRE — even though Validator A016 still warns about the JSON omission.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class BindingDefaultFillTests {

        [Test] public void Case01_NullBindingInPersona_ComposerFillsDefault() {
            // Pre: persona has binding == null
            // Post: composed.binding is non-null, on_action_change is the engine default
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
                "Composer must fill missing binding (spec §10.3 step 7, v0.1.5 Q-S7)");
            Assert.That(composed.binding!.on_action_change, Is.EqualTo(expected: Const.DEFAULT_ON_ACTION_CHANGE));
        }

        [Test] public void Case02_NullBindingInKind_ComposerFillsDefault() {
            // Pre: persona inherits from a Kind whose binding is also null
            Persona p = new Persona {
                agent_id = "a",
                kind_ids = new List<string> { "K" },
                binding  = null
            };
            Root r = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> {
                    new Kind { kind_id = "K",
                        actions = new List<Animo.Model.Action> { ActionOf(id: "X", need: "idle", tier: 5) },
                        binding = null }
                },
                personas = new List<Persona> { p }
            };
            Persona composed = Composer.Compose(persona: p, root: r);
            Assert.That(composed.binding, Is.Not.Null);
            Assert.That(composed.binding!.on_action_change, Is.EqualTo(expected: Const.DEFAULT_ON_ACTION_CHANGE));
        }

        [Test] public void Case03_PartialBindingProvided_OnlyMissingTemplatesFilled() {
            // Pre: binding has on_action_change set but other templates null
            // Post: provided value preserved; missing fields default-filled
            Persona p = new Persona {
                agent_id = "a",
                kind_ids = new List<string>(),
                actions  = new List<Animo.Model.Action> { ActionOf(id: "X", need: "idle", tier: 5) },
                binding  = new Binding { on_action_change = "custom_{agent_id}_{behavior}" }
            };
            Root r = new Root { schema_version = "1.5", personas = new List<Persona> { p } };
            Persona composed = Composer.Compose(persona: p, root: r);
            Assert.That(composed.binding, Is.Not.Null);
            Assert.That(composed.binding!.on_action_change, Is.EqualTo(expected: "custom_{agent_id}_{behavior}"),
                "Composer must preserve user-provided template values");
        }
    }
}
