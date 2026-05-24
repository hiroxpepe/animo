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
    public class ThresholdEpsilonValueTests {
        Persona compose(float t1, float t2) {
            var root = new Root {
                schema_version = "1.5",
                kinds = new List<Kind> { new Kind { kind_id = "k",
                    actions = new List<Animo.Model.Action> { ActionOf("X","fear",2) },
                    binding = new Binding { thresholds = new List<Threshold> {
                        ThresholdOf("fear", t1, "alert"), ThresholdOf("fear", t2, "panic") }}}},
                personas = new List<Persona> { new Persona { agent_id = "a",
                    kind_ids = new List<string> { "k" }}}
            };
            return Composer.Compose(root.personas[0], root);
        }

        [Test] public void Case01_AdjacentMilestones_80_0_and_80_4_KeptDistinct() {
            var c = compose(80.0f, 80.4f);
            Assert.That(c.binding!.thresholds.Count, Is.EqualTo(2),
                "Q-S47: 80.0 and 80.4 are > 0.01f apart — must remain distinct.");
        }

        [Test] public void Case02_DriftedFloat_80_0_and_80_0001_Collapse() {
            var c = compose(80.0f, 80.0001f);
            Assert.That(c.binding!.thresholds.Count, Is.EqualTo(1),
                "Q-S47: 80.0 and 80.0001 are < 0.01f apart — must collapse to one.");
        }
    }
}
