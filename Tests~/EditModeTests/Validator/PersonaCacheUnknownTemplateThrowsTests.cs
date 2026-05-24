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
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_GetComposedThrowsForUnknownTemplate() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec_v0.1.5_EN.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S103: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("throw new PersonaTemplateRejectedException"),
                "Q-S103: spec EN GetComposed must throw PersonaTemplateRejectedException " +
                "for unknown template_id, not return an empty Persona.");
        }
    }
}
