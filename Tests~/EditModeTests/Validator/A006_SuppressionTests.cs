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
    /// <summary>Decision-table tests for Validator rule A006: suppression keys only tier2-tier5, values 0..1.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A006_SuppressionTests {
        [Test] public void Case01_Tier1NotAllowed_FailsA006() {
            Root root = MinimalRoot();
            // tier1 is not modeled in the Suppression POCO; the schema rejects this at the JSON layer.
            // For runtime Validator we test out-of-range numeric values instead.
            root.personas[0].suppression = new Suppression { tier2 = 1.5f };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A006");
        }
        [Test] public void Case02_TierBelowZero_FailsA006() {
            Root root = MinimalRoot(); root.personas[0].suppression = new Suppression { tier2 = -0.1f };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A006");
        }
        [Test] public void Case03_TierBoundary_Passes() {
            Root root = MinimalRoot(); root.personas[0].suppression = new Suppression { tier2 = 0f, tier3 = 1f, tier4 = 0.5f, tier5 = 0.9f };
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
    }
}
