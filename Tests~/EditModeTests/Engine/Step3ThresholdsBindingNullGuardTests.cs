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
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_Step3UsesNullCoalesceForThresholds() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S107: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("_persona.binding?.thresholds"),
                "Q-S107: Step3 must use the binding null-coalesce form.");
        }
    }
}
