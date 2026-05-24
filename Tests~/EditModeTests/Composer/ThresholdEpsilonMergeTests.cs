// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ComposerTests {
    [TestFixture]
    public class ThresholdEpsilonMergeTests {
        [Test] public void Case01_PersonaOverrideWithDriftedFloat_CollapsesToOne() {
            var root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> { new Kind { kind_id = "k",
                    actions = new List<Animo.Model.Action> { ActionOf("X","fear",2) },
                    binding = new Binding { thresholds = new List<Threshold> {
                        ThresholdOf("fear", 80.0f, "alert") }}}},
                personas = new List<Persona> { new Persona { agent_id = "a",
                    kind_ids = new List<string> { "k" },
                    binding = new Binding { thresholds = new List<Threshold> {
                        ThresholdOf("fear", 80.0001f, "alert_override") }}}}
            };
            var composed = Composer.Compose(root.personas[0], root);
            Assert.That(composed.binding!.thresholds.Count, Is.EqualTo(1),
                "Q-S43: drift 80.0 vs 80.0001 must collapse to one threshold (Persona wins).");
            Assert.That(composed.binding.thresholds[0].trigger, Is.EqualTo("alert_override"),
                "Persona value must win.");
        }
    }
}
