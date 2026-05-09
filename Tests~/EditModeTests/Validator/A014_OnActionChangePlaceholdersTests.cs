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
    /// <summary>Decision-table tests for Validator rule A014: binding.on_action_change placeholders only {agent_id} / {behavior}.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A014_OnActionChangePlaceholdersTests {
        [Test] public void Case01_DisallowedPlaceholder_FailsA014() {
            Root root = MinimalRoot();
            root.personas[0].binding = new Binding { on_action_change = "x_{tier}_y" };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A014");
        }
        [Test] public void Case02_AllowedPlaceholders_Passes() {
            Root root = MinimalRoot();
            root.personas[0].binding = new Binding { on_action_change = "animo_{agent_id}_{behavior}" };
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case03_PlainStringNoPlaceholders_Passes() {
            Root root = MinimalRoot();
            root.personas[0].binding = new Binding { on_action_change = "fixed_event" };
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
