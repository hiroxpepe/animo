// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// File-existence test for Q-S83 (v0.1.5): Scripts/Agent.cs is
    /// physically present in the repository.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AgentFileExistsTests {
        [Test] public void Case01_AgentCs_ExistsInScriptsDir() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? found = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "Scripts", "Agent.cs");
                if (File.Exists(p)) { found = p; break; }
            }
            Assert.That(found, Is.Not.Null, "Q-S83: Scripts/Agent.cs must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("class Agent"),
                "Q-S83: Agent.cs must declare class Agent.");
            Assert.That(text, Does.Contain("UNITY_5_3_OR_NEWER"),
                "Q-S83: Agent.cs must be bracketed in #if UNITY_5_3_OR_NEWER.");
        }
    }
}
