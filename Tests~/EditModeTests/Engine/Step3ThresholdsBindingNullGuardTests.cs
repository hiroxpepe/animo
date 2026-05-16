// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Spec-content test for Q-S107 (v0.1.5): §16.5 Step3_Thresholds
    /// uses the binding?.thresholds ?? Array.Empty<Threshold>() form,
    /// matching the ctor's defensive form (Q-S12 + Q-S53). Pre-Q-S107
    /// Step 3 dereferenced binding directly while ctor defended —
    /// defense-in-depth was inconsistent.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Step3ThresholdsBindingNullGuardTests {
        [Test] public void Case01_SpecEN_Step3UsesNullCoalesceForThresholds() {
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
            Assert.That(path, Is.Not.Null, "Q-S107: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("_persona.binding?.thresholds"),
                "Q-S107: Step3 must use the binding null-coalesce form.");
        }
    }
}
