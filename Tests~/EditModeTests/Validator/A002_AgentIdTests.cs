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
    /// <summary>Decision-table tests for Validator rule A002: persona.agent_id is snake_case, not empty, unique, ≤128 chars.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A002_AgentIdTests {
        [Test] public void Case01_EmptyAgentId_FailsA002() {
            Root root = MinimalRoot(); root.personas[0].agent_id = "";
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A002");
        }
        [Test] public void Case02_PascalCaseAgentId_FailsA002() {
            Root root = MinimalRoot(); root.personas[0].agent_id = "GoblinScout";
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A002");
        }
        [Test] public void Case03_DoubleUnderscoreAgentId_FailsA002() {
            Root root = MinimalRoot(); root.personas[0].agent_id = "goblin__scout";
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A002");
        }
        [Test] public void Case04_TrailingUnderscoreAgentId_FailsA002() {
            Root root = MinimalRoot(); root.personas[0].agent_id = "goblin_";
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A002");
        }
        [Test] public void Case05_DigitFirstAgentId_FailsA002() {
            Root root = MinimalRoot(); root.personas[0].agent_id = "1goblin";
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A002");
        }
        [Test] public void Case06_Over128Chars_FailsA002() {
            Root root = MinimalRoot(); root.personas[0].agent_id = new string('a', 129);
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A002");
        }
        [Test] public void Case07_DuplicateAgentId_FailsA002() {
            Root root = MinimalRoot();
            root.personas.Add(item: new Persona { agent_id = "agent_a", actions = new List<Action> { ActionOf(id: "X", need: "idle", tier: 5) } });
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A002");
        }
        [Test] public void Case08_ValidAgentId_Passes() {
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
