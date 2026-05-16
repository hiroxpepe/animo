// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// File-content test for Q-S121 (v0.1.5): schema's seven range
    /// constraints (need_value, coefficient, suppression_factor, tier,
    /// exponent, commitment.bonus, trigger_threshold) have their
    /// minimum/maximum keys removed so the values flow through to
    /// the C# Validator (A005-A012, A028) for human-readable Errors.
    /// Q-S121 generalizes Q-S108's principle.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class SchemaNoRangeConstraintsTests {
        [Test] public void Case01_Schema_NoMinimumOrMaximumOutsideDescriptions() {
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
            Assert.That(path, Is.Not.Null, "Q-S121: schema.json must exist.");
            // Read line by line; any line that has BOTH "minimum" AND a
            // colon outside a description string is a constraint.
            // Description-text mentions of the WORD minimum are fine.
            // The test inspects: a JSON line like `"minimum": 0.0,` is
            // a real constraint; `"description": "...minimum..."` is text.
            // We simply assert that grepping for the JSON-key form
            // `"minimum":` (followed by a number) produces zero lines
            // outside description strings — easiest: ensure exact
            // pattern `"minimum": ` appears only inside lines whose
            // preceding characters indicate description text.
            var lines = File.ReadAllLines(path!);
            int violation_count = 0;
            foreach (var line in lines) {
                // A constraint line looks like:    "minimum": 0.0,
                // A description line might say:   "description": "Spec range [0.0, 100.0] (A005) ..."
                // We trim the line and check whether it STARTS with the
                // JSON key `"minimum":` or `"maximum":` (true constraint
                // — the property is named `minimum`/`maximum` at top
                // level of an object). Description text mentioning the
                // word inside a longer string never starts the line
                // with that exact pattern.
                var trimmed = line.TrimStart();
                if (trimmed.StartsWith("\"minimum\":") || trimmed.StartsWith("\"maximum\":")) {
                    violation_count++;
                }
            }
            Assert.That(violation_count, Is.EqualTo(expected: 0),
                "Q-S121: schema must NOT carry top-level minimum/maximum constraints; " +
                $"found {violation_count} violation lines. Range checks belong to the C# Validator.");
        }
    }
}
