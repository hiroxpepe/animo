// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// File-content test for Q-S89 (v0.1.5): Schemas/animo.schema.json
    /// declares `needs_meta` property in both kind and persona, with a
    /// `needs_meta_map` definition that the property references. Pre-Q-S89
    /// the schema rejected every spec-compliant Q-S30 needs_meta block at
    /// ajv before reaching the C# Validator.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class SchemaNeedsMetaPropertyTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_Schema_DeclaresNeedsMetaInKindAndPersona() {
            string? found = null;
            { var p = Path.Combine(RepoRoot(), "Schemas", "animo.schema.json"); if (File.Exists(p)) found = p; }
            Assert.That(found, Is.Not.Null, "Q-S89: Schemas/animo.schema.json must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("\"needs_meta_map\""),
                "Q-S89: schema must declare needs_meta_map definition.");
            Assert.That(text, Does.Contain("\"need_meta\""),
                "Q-S89: schema must declare need_meta definition with tier ∈ [1, 5].");
            Assert.That(text, Does.Contain("\"needs_meta\":"),
                "Q-S89: schema must declare needs_meta property in kind and/or persona.");
        }
    }
}
