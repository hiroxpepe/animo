// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// File-content test for Q-S87 (v0.1.5): MockScene.cs uses reusable
    /// scratch buffers (List<T> with Clear+AddRange), not per-Tick array
    /// allocations.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class MockSceneScratchBufferTests {
        [Test] public void Case01_MockScene_UsesReusableScratchBuffers() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            string? found = null;
            foreach (var r in roots) {
                var p = Path.Combine(r, "Tests~", "MiniUnity", "MockScene.cs");
                if (File.Exists(p)) { found = p; break; }
            }
            Assert.That(found, Is.Not.Null, "Q-S87: MockScene.cs must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("_obj_scratch"),
                "Q-S87: MockScene must declare _obj_scratch reusable List buffer.");
            Assert.That(text, Does.Contain("_comp_scratch"),
                "Q-S87: MockScene must declare _comp_scratch reusable List buffer.");
            Assert.That(text, Does.Not.Contain("_objects.ToArray()"),
                "Q-S87: MockScene Tick MUST NOT call _objects.ToArray() (per-frame alloc).");
        }
    }
}
