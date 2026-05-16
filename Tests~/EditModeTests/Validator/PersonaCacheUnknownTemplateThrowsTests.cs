// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Spec-content test for Q-S103 (v0.1.5): §11.6.1 GetComposed
    /// throws PersonaTemplateRejectedException for unknown template_id,
    /// not the silent-corruption empty Persona fallback.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PersonaCacheUnknownTemplateThrowsTests {
        [Test] public void Case01_SpecEN_GetComposedThrowsForUnknownTemplate() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? path = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "docs", "animo_spec_v0.1.5_EN.md");
                if (File.Exists(p)) { path = p; break; }
            }
            Assert.That(path, Is.Not.Null, "Q-S103: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("throw new PersonaTemplateRejectedException"),
                "Q-S103: spec EN GetComposed must throw PersonaTemplateRejectedException " +
                "for unknown template_id, not return an empty Persona.");
        }
    }
}
