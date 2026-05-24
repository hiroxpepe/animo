// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    [TestFixture]
    public class PersonaDeepCopyIsolationTests {
        [Test] public void Case01_PersonaClass_DeclaresDeepCopyMethod() {
            var m = typeof(Persona).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "DeepCopy" && m.GetParameters().Length == 0);
            Assert.That(m, Is.Not.Null, "Q-S64: Persona.DeepCopy() must be declared.");
            Assert.That(m!.ReturnType, Is.EqualTo(typeof(Persona)));
        }

        [Test] public void Case02_DeepCopy_IsolatesAgentIdMutation_FromTemplate() {
            var template = new Persona {
                agent_id = "goblin_scout",
                actions  = new List<Animo.Model.Action> { ActionOf("Idle","idle",5) }
            };
            var copy = template.DeepCopy();
            copy.agent_id = "goblin_47291";
            Assert.That(template.agent_id, Is.EqualTo("goblin_scout"),
                "Q-S64: mutating copy.agent_id must not change template.agent_id.");
        }
    }
}
