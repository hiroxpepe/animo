// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// File-content test for Q-S101 (v0.1.5, post-centennial first):
    /// the physical Scripts/Agent.cs file contains the Q-S96 null-safe
    /// `agent_id` getter and OnDestroy early-return guard. Pre-Q-S101
    /// only the spec narrative had these; the .cs file lagged.
    ///
    /// This test embodies the Phase_2_4_22 N-round review process
    /// upgrade: spec narrative ↔ physical Scripts/*.cs file
    /// synchronization is now part of the consistency check. Every
    /// spec patch that touches a code block must also touch the
    /// physical file, and a test like this verifies it.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AgentNullSafePhysicalFileTests {
        static string? FindFile(string relativePath) {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            foreach (var r in roots) {
                var p = Path.Combine(r, relativePath);
                if (File.Exists(p)) return p;
            }
            return null;
        }

        [Test] public void Case01_PhysicalAgentCs_HasNullSafeAgentIdGetter() {
            var path = FindFile(Path.Combine("Scripts", "Agent.cs"));
            Assert.That(path, Is.Not.Null, "Q-S101: Scripts/Agent.cs must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("_composed_persona?.agent_id ?? \"<uninitialized>\""),
                "Q-S101: physical Scripts/Agent.cs must have the Q-S96 null-safe agent_id getter.");
        }

        [Test] public void Case02_PhysicalAgentCs_HasOnDestroyGuard() {
            var path = FindFile(Path.Combine("Scripts", "Agent.cs"));
            Assert.That(path, Is.Not.Null, "Q-S101: Scripts/Agent.cs must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("if (_composed_persona == null) return;"),
                "Q-S101: physical Scripts/Agent.cs OnDestroy must early-return when _composed_persona is null.");
        }
    }
}
