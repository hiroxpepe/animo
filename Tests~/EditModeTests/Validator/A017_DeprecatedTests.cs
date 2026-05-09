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
    /// <summary>Decision-table tests for Validator rule A017: [deprecated v0.1.3] hysteresis.bonus ≤ hysteresis.decay (decay removed).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A017_DeprecatedTests {
        [Test] public void Case01_StaticValidator_DoesNotEnforceA017Anymore() {
            // A017 was the old "hysteresis.bonus ≤ hysteresis.decay" check.
            // commitment.decay was removed in v0.1.3, so A017 is no longer enforced.
            // Guard test: the static Validator never produces an A017 issue at all.
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.HasRule(rule_id: "A017"), Is.False);
        }
    }
}
