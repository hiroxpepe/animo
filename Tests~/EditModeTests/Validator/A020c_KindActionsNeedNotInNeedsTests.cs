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
    /// <summary>Decision-table tests for Validator rule A020c: kind.actions[].need not in needs (Warning).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A020c_KindActionsNeedNotInNeedsTests {
        [Test] public void Case01_ActionNeedNotInNeeds_WarnsA020c() {
            Root root = new Root {
                schema_version = "1.4",
                kinds = new List<Kind> { new Kind { kind_id = "k", actions = new List<Action> { ActionOf(id: "X", need: "phantom", tier: 5) } } },
                personas = new List<Persona> { new Persona { agent_id = "a", kind_ids = new List<string> { "k" }, needs = NeedsOf(("idle", 50f)) } }
            };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasWarning(r, rule_id: "A020c");
        }
    }
}
