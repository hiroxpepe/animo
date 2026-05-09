// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using Animo.Tests.EditMode.Helpers;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>Decision-table tests for Validator rule A020b: kind.influences source/target not in needs (Warning).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A020b_KindInfluencesNotInNeedsTests {
        [Test] public void Case01_InfluenceSourceNotInNeeds_WarnsA020b() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k", influences = new List<Influence> { InfluenceOf(source: "phantom", target: "fear", coefficient: 0.5f) }, actions = new List<Action> { ActionOf(id: "A", need: "idle", tier: 5) } } },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "k" }, needs = NeedsOf(("fear", 50f), ("idle", 30f)) } }
            };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasWarning(r, rule_id: "A020b");
        }
    }
}
