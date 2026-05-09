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
    /// <summary>Decision-table tests for Validator rule A032: [info] hint about a low-tier fallback action other than idle (v0.1.4).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A032_FallbackActionInfoTests {
        [Test] public void Case01_StaticValidator_DoesNotEmitA032AsError() {
            // A032 is informational (hint about a low-tier fallback action other than idle).
            // Guard test: the static Validator never misclassifies A032 as Error.
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root);
            foreach (Issue i in r.issues) if (i.rule_id == "A032") Assert.That(i.severity, Is.Not.EqualTo(expected: Severity.Error));
        }
    }
}
