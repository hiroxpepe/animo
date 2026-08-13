// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Spec-content test for Q-S104 (v0.1.5): §26.3.1 Run loop
    /// normalizes `events` once at entry to Array.Empty, so Run()
    /// called with default events=null does not NRE.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerEventsNullGuardTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_RunNormalizesEventsToEmptyArray() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S104: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("Make `events` safe"),
                "Q-S104: spec EN must normalize events to empty array before the Run loops.");
        }
    }
}
