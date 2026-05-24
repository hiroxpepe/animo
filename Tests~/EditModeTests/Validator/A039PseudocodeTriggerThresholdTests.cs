// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Spec-content test for Q-S105 (v0.1.5): A039 pseudocode in §13
    /// uses `trigger_threshold` (the float field), not `trigger`
    /// (the string event-name field). Pre-Q-S105 the pseudocode wrote
    /// `next.trigger - prev.trigger` — naive Phase 3 transcription
    /// would hit a "cannot subtract string from string" CS error.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A039PseudocodeTriggerThresholdTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_A039PseudocodeUsesTriggerThreshold() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec_v0.1.5_EN.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S105: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("next.trigger_threshold - prev.trigger_threshold"),
                "Q-S105: A039 pseudocode must subtract trigger_threshold (float), " +
                "not trigger (string event-name).");
        }
    }
}
