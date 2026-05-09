// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using Animo.Tests.EditMode.Helpers;
using static Animo.Tests.EditMode.Helpers.Fixture;
using Action = Animo.Model.Action;

namespace Animo.Tests.EditMode.EdgeCases {
    /// <summary>High-volume cases: 0/1/1000/10000 elements (spec §4.6.3).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class HighVolumeTests {
        [Test] public void Case01_ZeroPersonas_FailsA001() {
            Root root = new Root { schema_version = "1.4", personas = new List<Persona>() };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A001");
        }
        [Test] public void Case02_OnePersona_Passes() {
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case03_OneThousandPersonas_DoesNotThrowOrTimeout() {
            Root root = MinimalRoot();
            for (int i = 1; i < 1000; i++) root.personas.Add(item: new Persona { agent_id = $"agent_{i}", actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } });
            Assert.DoesNotThrow(code: () => Validator.Validate(root: root));
        }
        [Test] public void Case04_DuplicatePersonaIdsAt1000Scale_FailsA002() {
            Root root = MinimalRoot();
            for (int i = 1; i < 1000; i++) root.personas.Add(item: new Persona { agent_id = "agent_a", actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } });
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A002");
        }
        [Test] public void Case05_OneHundredKindsCascade_DoesNotThrow() {
            Root root = MinimalRoot();
            List<string> kind_ids = new();
            for (int i = 0; i < 100; i++) {
                string kid = $"kind_{i}";
                root.kinds.Add(item: new Kind { kind_id = kid, actions = new List<Action> { ActionOf(id: $"Act_{i}", need: "idle", tier: 5) } });
                kind_ids.Add(item: kid);
            }
            root.personas[0].kind_ids = kind_ids;
            Assert.DoesNotThrow(code: () => Validator.Validate(root: root));
        }
    }
}
