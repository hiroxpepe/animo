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
    /// <summary>Empty / null edges: empty string, empty list, missing field (spec §4.6.3).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class EmptyAndNullTests {
        [Test] public void Case01_EmptyAgentId_FailsA002() {
            Root root = MinimalRoot(); root.personas[0].agent_id = "";
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A002");
        }
        [Test] public void Case02_EmptyActionsList_FailsA011() {
            Root root = MinimalRoot(); root.personas[0].actions = new List<Action>();
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A011a");
        }
        [Test] public void Case03_EmptyKindIdsList_BehavesLikeMissing() {
            Root root = MinimalRoot(); root.personas[0].kind_ids = new List<string>();
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case04_NullNeeds_TreatedAsEmpty() {
            Root root = MinimalRoot(); root.personas[0].needs = null;
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case05_NullBinding_WarnsA016() {
            Root root = MinimalRoot(); root.personas[0].binding = null;
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasWarning(r, rule_id: "A016");
        }
        [Test] public void Case06_EmptyInfluences_NoCycleAndPasses() {
            Root root = MinimalRoot(); root.personas[0].influences = new List<Influence>();
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
