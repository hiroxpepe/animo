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
    /// <summary>Decision-table tests for Validator rule A003: kind.kind_id is snake_case, not empty, unique, ≤128 chars.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A003_KindIdTests {
        [Test] public void Case01_EmptyKindId_FailsA003() {
            Root root = MinimalRoot(); root.kinds.Add(item: KindOf(kind_id: ""));
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A003");
        }
        [Test] public void Case02_HyphenInKindId_FailsA003() {
            Root root = MinimalRoot(); root.kinds.Add(item: KindOf(kind_id: "gob-lin"));
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A003");
        }
        [Test] public void Case03_DuplicateKindId_FailsA003() {
            Root root = MinimalRoot();
            root.kinds.Add(item: KindOf(kind_id: "goblin"));
            root.kinds.Add(item: KindOf(kind_id: "goblin"));
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A003");
        }
        [Test] public void Case04_ValidKindId_Passes() {
            Root root = MinimalRoot(); root.kinds.Add(item: KindOf(kind_id: "goblin"));
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
