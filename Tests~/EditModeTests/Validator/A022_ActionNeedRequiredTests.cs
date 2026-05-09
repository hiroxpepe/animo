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
    /// <summary>Decision-table tests for Validator rule A022: actions[].need is required (v0.1.1).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A022_ActionNeedRequiredTests {
        [Test] public void Case01_MissingActionNeed_FailsA022() {
            Root root = MinimalRoot(); root.personas[0].actions![0].need = "";
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A022");
        }
        [Test] public void Case02_PresentActionNeed_Passes() {
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
