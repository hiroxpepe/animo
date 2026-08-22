// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Q-S86: Engine Step3 must use reset_threshold!.Value (no ?? fallback).</summary>
    [TestFixture]
    public class Step3ResetThresholdNonNullTests {

        [Test] public void Case01_ComposerOutput_AllThresholdsHaveNonNullResetThreshold() {
            var root = new Animo.Model.Root { schema_version = "1.5",
                personas = new List<Animo.Model.Persona> {
                    new Animo.Model.Persona { agent_id = "a",
                        needs   = NeedsOf(("fear",30f)),
                        actions = new List<Animo.Model.Action>{ ActionOf("X","fear",2) },
                        binding = new Animo.Model.Binding { thresholds = new List<Animo.Model.Threshold>{
                            ThresholdOf("fear",80f,"alert") }}}}
            };
            var composed = Composer.Compose(root.personas[0], root);
            foreach (var t in composed.binding!.thresholds)
                Assert.That(t.reset_threshold, Is.Not.Null,
                    "Q-S11+Q-S86: Composer must fill reset_threshold.");
        }

        [Test] public void Case02_EngineCs_DoesNotContainNullCoalesceForResetThreshold() {
            // Q-S86: Engine.cs Step3 must read reset_threshold!.Value (no ?? fallback).
            // This test verifies the source contract directly.
            string? root = Directory.GetCurrentDirectory();
            while (root != null && !File.Exists(Path.Combine(root, "Scripts", "Const.cs")))
                root = Directory.GetParent(root)?.FullName;
            Assert.That(root, Is.Not.Null, "Q-S86: repo root must be found.");
            var text = File.ReadAllText(Path.Combine(root!, "Scripts", "Core", "Engine.cs"));
            Assert.That(text, Does.Not.Contain("?? System.Math.Max(0f, t.trigger_threshold - 5f)"),
                "Q-S86: Engine.cs must not contain null-coalesce fallback for reset_threshold.");
            Assert.That(text, Does.Not.Contain(".HasValue\n                    ?"),
                "Q-S86: Engine.cs must not contain HasValue ternary for reset_threshold.");
        }

        [Test] public void Case03_ResetThreshold_WorksCorrectly_AtRuntime() {
            // Q-S86: runtime proof that reset works with !.Value
            int fired = 0;
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("fear",50f),("idle",30f)),
                actions = new List<Action>{ ActionOf("Idle","idle",5) },
                binding = new Binding { thresholds = new List<Threshold>{
                    ThresholdOf("fear",80f,"alert",70f) }}
            };
            var e = new Engine(p);
            e.OnSignaled += s => { if (s == "alert") fired++; };
            e.Affect("fear", +35f);  // 85 → crosses 80
            e.Live(delta_time: 0.016f);
            Assert.That(fired, Is.EqualTo(1), "Q-S86: threshold must fire.");
            e.Affect("fear", -20f);  // 65 → drops below reset 70
            e.Live(delta_time: 0.016f);
            e.Affect("fear", +20f);  // 85 → crosses again
            e.Live(delta_time: 0.016f);
            Assert.That(fired, Is.EqualTo(2), "Q-S86: threshold must refire after reset.");
        }
    }
}
