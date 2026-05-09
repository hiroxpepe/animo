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
    /// <summary>Decision-table tests for Validator rule A013: rates keys are a subset of needs keys (Warning).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A013_RatesKeySubsetTests {
        [Test] public void Case01_RatesKeyNotInNeeds_WarnsA013() {
            Root root = MinimalRoot();
            root.personas[0].needs = NeedsOf(("fear", 50f));
            root.personas[0].rates = RatesOf(("phantom", 1.0f));
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasWarning(r, rule_id: "A013");
        }
        [Test] public void Case02_RatesKeyInNeeds_Passes() {
            Root root = MinimalRoot();
            root.personas[0].needs = NeedsOf(("fear", 50f));
            root.personas[0].rates = RatesOf(("fear", 1.0f));
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
