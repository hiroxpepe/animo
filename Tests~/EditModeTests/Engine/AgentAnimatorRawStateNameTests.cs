// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Spec-content test for Q-S102 (v0.1.5): §11.4.1 Awake step (6)
    /// passes the RAW _engine.behavior to _animator?.Play, not the
    /// expanded trigger. Pre-Q-S102 Q-S44 routed through
    /// GetExpandedActionTrigger which produced runtime-instance ids
    /// no Animator Controller could match — every NPC froze in T-pose.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AgentAnimatorRawStateNameTests {
        static string? FindSpec() {
            var roots = new[] {
                "/home/claude/animo_full/animo",
                System.Environment.CurrentDirectory,
                Path.Combine(System.Environment.CurrentDirectory, "..", ".."),
            };
            foreach (var r in roots) {
                var p = Path.Combine(r, "docs", "animo_spec_v0.1.5_EN.md");
                if (File.Exists(p)) return p;
            }
            return null;
        }
        [Test] public void Case01_SpecEN_AwakeUsesRawBehaviorForAnimator() {
            var path = FindSpec();
            Assert.That(path, Is.Not.Null, "Q-S102: spec EN must exist.");
            var text = File.ReadAllText(path!);
            Assert.That(text, Does.Contain("_animator?.Play(stateName: _engine.behavior)"),
                "Q-S102: Awake must call _animator?.Play with raw _engine.behavior, " +
                "not GetExpandedActionTrigger output.");
        }
    }
}
