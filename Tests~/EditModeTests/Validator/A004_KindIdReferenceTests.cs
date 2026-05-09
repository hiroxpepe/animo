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
    /// <summary>Decision-table tests for Validator rule A004: All persona.kind_ids exist in kinds.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A004_KindIdReferenceTests {
        [Test] public void Case01_UndefinedKindId_FailsA004() {
            Root root = MinimalRoot(); root.personas[0].kind_ids = new List<string> { "ghost" };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A004");
        }
        [Test] public void Case02_DefinedKindId_Passes() {
            Root root = MinimalRoot();
            root.kinds.Add(item: KindOf(kind_id: "goblin"));
            root.personas[0].kind_ids = new List<string> { "goblin" };
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case03_PartiallyUndefinedKindIds_FailsA004() {
            Root root = MinimalRoot();
            root.kinds.Add(item: KindOf(kind_id: "goblin"));
            root.personas[0].kind_ids = new List<string> { "goblin", "ghost" };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A004");
        }
    }
}
