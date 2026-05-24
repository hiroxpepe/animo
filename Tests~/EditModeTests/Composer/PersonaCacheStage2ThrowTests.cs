// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Q-S38: PersonaCache.GetComposed throws when stage-2 validation has errors.
    /// </summary>
    [TestFixture]
    public class PersonaCacheStage2ThrowTests {
        [TearDown] public void TearDown() => PersonaCache.ClearForTesting();

        [Test] public void Case01_Stage2HasErrors_GetComposedThrowsInvalidOperationException() {
            // A Persona with no actions (A036 Error: composed actions[] empty).
            var root = new Root {
                schema_version = "1.5",
                personas = new List<Persona> {
                    new Persona { agent_id = "broken", kind_ids = new List<string>() }
                }
            };
            PersonaCache.Initialize(root);
            Assert.Throws<PersonaTemplateRejectedException>(
                () => PersonaCache.GetComposed("broken"),
                "Q-S38: PersonaCache.GetComposed must throw PersonaTemplateRejectedException " +
                "when stage-2 has errors (A036: composed actions[] empty).");
        }
    }
}
