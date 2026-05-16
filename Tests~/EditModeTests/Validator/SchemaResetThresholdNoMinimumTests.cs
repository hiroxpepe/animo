// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// File-content test for Q-S108 (v0.1.5): schema's reset_threshold
    /// has no `minimum` constraint, so explicit-negative values flow
    /// through to Validator A034 (Q-S11) for human-readable Error
    /// reporting. Pre-Q-S108 ajv hard-rejected at the gate, making
    /// A034 a permanently-unreachable dead rule.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class SchemaResetThresholdNoMinimumTests {
        [Test] public void Case01_Schema_ResetThresholdHasNoMinimum() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? path = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "Schemas", "animo.schema.json");
                if (File.Exists(p)) { path = p; break; }
            }
            Assert.That(path, Is.Not.Null, "Q-S108: schema.json must exist.");
            var text = File.ReadAllText(path!);
            // The reset_threshold definition starts with "reset_threshold":
            // and ends at the next top-level key. We only check that the
            // text immediately around "reset_threshold" does not contain
            // "minimum": 0.0 — Q-S108 removed it.
            int idx = text.IndexOf("\"reset_threshold\":");
            Assert.That(idx, Is.GreaterThan(0),
                "Q-S108: schema must declare reset_threshold property.");
            // Find the closing brace of this property definition
            int blockEnd = text.IndexOf("},", idx);
            Assert.That(blockEnd, Is.GreaterThan(idx),
                "Q-S108: reset_threshold definition must close.");
            string block = text.Substring(idx, blockEnd - idx);
            Assert.That(block, Does.Not.Contain("\"minimum\": 0.0"),
                "Q-S108: reset_threshold must NOT carry minimum:0.0 (so A034 can fire).");
        }
    }
}
