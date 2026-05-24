// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    [TestFixture]
    public class PersonaInheritanceAdditiveTests {
        [Test] public void Case01_PersonaOmitsKindAction_KindActionStillAppendedAtTail() {
            var root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> {
                    new Kind { kind_id = "k",
                        actions = new List<Animo.Model.Action> {
                            ActionOf("Idle","idle",5), ActionOf("Patrol","fear",2) }}
                },
                personas = new List<Persona> {
                    new Persona { agent_id = "a", kind_ids = new List<string> { "k" },
                        actions = new List<Animo.Model.Action> { ActionOf("Attack","fear",2) }}
                }
            };
            var composed = Composer.Compose(root.personas[0], root);
            Assert.That(composed.actions, Is.Not.Null);
            Assert.That(composed.actions![0].id, Is.EqualTo("Attack"), "Persona-first: Attack at index 0");
            Assert.That(composed.actions![1].id, Is.EqualTo("Idle"),   "Kind appended: Idle at index 1");
            Assert.That(composed.actions![2].id, Is.EqualTo("Patrol"), "Kind appended: Patrol at index 2");
        }
    }
}
