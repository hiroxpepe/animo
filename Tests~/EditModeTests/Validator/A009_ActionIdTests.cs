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
    /// <summary>Decision-table tests for Validator rule A009: actions[].id is not empty.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A009_ActionIdTests {
        [Test] public void Case01_EmptyActionId_FailsA009() {
            Root root = MinimalRoot(); root.personas[0].actions![0].id = "";
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A009");
        }
        [Test] public void Case02_NonEmptyActionId_Passes() {
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
