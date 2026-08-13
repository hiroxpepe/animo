// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Spec-content test for Q-S96 (v0.1.5): §11.4.1 Agent class makes
    /// agent_id getter null-safe AND adds OnDestroy early-return for
    /// Awake-failed (Q-S38) Agents. Pre-Q-S96 the chain
    /// OnDestroy → Store.Unregister → agent_id getter → null deref
    /// would NRE at scene unload time, breaking Q-S38's fail-loud
    /// "keep-scene-alive" guarantee.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AgentOnDestroyNullSafeTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_AgentIdGetterIsNullSafe() {
            string? found = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec.md"); if (File.Exists(p)) found = p; }
            Assert.That(found, Is.Not.Null, "Q-S96: spec EN must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("_composed_persona?.agent_id ?? \"<uninitialized>\""),
                "Q-S96: spec EN agent_id getter must be null-safe.");
            Assert.That(text, Does.Contain("if (_composed_persona == null) return;"),
                "Q-S96: spec EN OnDestroy must early-return when _composed_persona is null.");
        }
    }
}
