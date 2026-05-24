// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Spec-content test for Q-S125 (v0.1.5): §16 Engine ctor's two
    /// foreach loops over actions both use `?? new List&lt;Action&gt;()`
    /// — defense-in-depth consistency with the threshold loops below.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class EngineCtorActionsNullCoalesceTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_CtorActionsLoopsNullCoalesce() {
            string? path = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec_v0.1.5_EN.md"); if (File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S125: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("foreach (var action in _composed_persona.actions ?? new List<Action>())"),
                "Q-S125: spec EN Engine ctor must use ?? new List<Action>() for actions loops.");
        }
    }
}
