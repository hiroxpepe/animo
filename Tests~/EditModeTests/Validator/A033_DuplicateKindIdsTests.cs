// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using Animo.Tests.EditMode.Helpers;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Decision-table tests for Validator rule A033: duplicate id in kind_ids
    /// (v0.1.5, Q7). Composer dedupes; Validator emits a Warning.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A033_DuplicateKindIdsTests {

        [Test] public void Case01_DuplicateKindIds_WarnsA033() {
            Root root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> { new Kind { kind_id = "goblin", actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } } },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "goblin", "goblin" } } }
            };
            ValidationResult r = Validator.Validate(root: root);
            AssertResult.HasWarning(r, rule_id: "A033");
        }

        [Test] public void Case02_TripleDuplicate_WarnsA033Once() {
            Root root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> { new Kind { kind_id = "goblin", actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } } },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "goblin", "goblin", "goblin" } } }
            };
            ValidationResult r = Validator.Validate(root: root);
            AssertResult.HasWarning(r, rule_id: "A033");
        }

        [Test] public void Case03_DistinctKindIds_NoA033() {
            Root root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> {
                    new Kind { kind_id = "goblin",  actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } },
                    new Kind { kind_id = "scout",   actions = new List<Action> { ActionOf(id: "Y", need: "fear", tier: 2) } }
                },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "goblin", "scout" } } }
            };
            ValidationResult r = Validator.Validate(root: root);
            Assert.That(r.HasRule(rule_id: "A033"), Is.False);
        }

        [Test] public void Case04_NonAdjacentDuplicate_WarnsA033() {
            Root root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> {
                    new Kind { kind_id = "goblin", actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } },
                    new Kind { kind_id = "scout",  actions = new List<Action> { ActionOf(id: "Y", need: "fear", tier: 2) } }
                },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "goblin", "scout", "goblin" } } }
            };
            ValidationResult r = Validator.Validate(root: root);
            AssertResult.HasWarning(r, rule_id: "A033");
        }
    }
}
