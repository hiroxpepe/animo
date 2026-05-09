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
    /// <summary>Decision-table tests for Validator rule A016: binding is missing (Warning).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A016_BindingMissingTests {
        [Test] public void Case01_MissingBinding_WarnsA016() {
            Root root = MinimalRoot(); root.personas[0].binding = null;
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasWarning(r, rule_id: "A016");
        }
        [Test] public void Case02_PresentBinding_Passes() {
            Root root = MinimalRoot(); root.personas[0].binding = new Binding { on_action_change = "x_{agent_id}_{behavior}" };
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
