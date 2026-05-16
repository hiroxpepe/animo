// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Model;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Decision-table test for Q-S64 (v0.1.5): `Persona.DeepCopy()` is
    /// declared on the Persona class. Pre-Q-S64 the §11.4.1 Awake step
    /// (2) `_composed_persona = template.DeepCopy()` referenced an
    /// undeclared method — confirmed compile error.
    ///
    /// Phase 3 contract: After DeepCopy(), mutating the copy's
    /// agent_id, Needs.values, actions[], or any other reference-typed
    /// field MUST NOT affect the original template (which PersonaCache
    /// shares across all Agents spawned from the same template id).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PersonaDeepCopyIsolationTests {
        [Test] public void Case01_PersonaClass_DeclaresDeepCopyMethod() {
            var personaType = typeof(Persona);
            var deepCopy = personaType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "DeepCopy" && m.GetParameters().Length == 0);
            Assert.That(deepCopy, Is.Not.Null,
                "Q-S64: Persona.DeepCopy() declaration required (v0.1.5 stub returns " +
                "NotImplementedException; Phase 3 implements deep clone).");
            Assert.That(deepCopy!.ReturnType, Is.EqualTo(personaType),
                "Q-S64: DeepCopy() must return Persona.");
        }

        [Test] public void Case02_DeepCopy_IsolatesAgentIdMutation_FromTemplate() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "After `var copy = template.DeepCopy(); copy.agent_id = \"goblin_47291\";` " +
                "the original `template.agent_id` MUST remain unchanged. Q-S64 isolation " +
                "contract: PersonaCache returns shared composed templates, so DeepCopy is " +
                "the per-Agent isolation barrier.");
        }
    }
}
