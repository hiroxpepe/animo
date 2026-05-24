// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    [TestFixture]
    public class PersonaCacheFlyweightTests {
        [TearDown] public void TearDown() => PersonaCache.ClearForTesting();

        [Test] public void Case01_GetComposedBeforeInitialize_ThrowsInvalidOperation() {
            PersonaCache.ClearForTesting();
            Assert.Throws<PersonaCacheNotInitializedException>(
                () => PersonaCache.GetComposed("any"),
                "Q-S111: GetComposed before Initialize must throw PersonaCacheNotInitializedException.");
        }

        [Test] public void Case02_RepeatedGetComposed_ReturnsSameInstance() {
            var root = new Root {
                schema_version = "1.5",
                personas = new List<Persona> {
                    new Persona { agent_id = "agent_a",
                        actions = new List<Animo.Model.Action> { ActionOf("Idle","idle",5) }}
                }
            };
            PersonaCache.Initialize(root);
            var p1 = PersonaCache.GetComposed("agent_a");
            var p2 = PersonaCache.GetComposed("agent_a");
            Assert.That(p2, Is.SameAs(p1),
                "Q-S29: repeated GetComposed for same template_id must return the cached instance.");
        }
    }
}
