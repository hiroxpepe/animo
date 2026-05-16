// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Spec-content test for Q-S112 (v0.1.5): §11.4.1 Awake emits a
    /// log Warning once when _bus is null, per §12.1 contract.
    /// Pre-Q-S112 only `?.Publish` silently skipped — the contracted
    /// authoring-aid Warning never fired.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AgentBusNullWarningTests {
        [Test] public void Case01_SpecEN_AwakeLogsWarningWhenBusNull() {
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
            Assert.That(path, Is.Not.Null, "Q-S112: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("if (_bus == null)"),
                "Q-S112: Awake must check _bus == null and emit a Warning.");
            Assert.That(text, Does.Contain("has no Germio.Bus assigned"),
                "Q-S112: Warning message must reference §12.1 contract intent.");
        }
    }
}
