// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Spec-content test for Q-S88 (v0.1.5): §16.2.2.1's Q-S27
    /// pseudocode is marked as "conceptual sketch only" with explicit
    /// pointer to §3.5.2 PHASE A as canonical.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Q88ConceptualSketchMarkerTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_Section_16_2_2_1_PseudocodeMarkedConceptualOnly() {
            string? found = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec_v0.1.5_EN.md"); if (File.Exists(p)) found = p; }
            Assert.That(found, Is.Not.Null, "Q-S88: spec EN must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("Conceptual sketch only"),
                "Q-S88: §16.2.2.1 must contain 'Conceptual sketch only' marker.");
            Assert.That(text, Does.Contain("CANONICAL implementation: §3.5.2 PHASE A"),
                "Q-S88: §16.2.2.1 must point to §3.5.2 PHASE A as canonical.");
        }
    }
}
