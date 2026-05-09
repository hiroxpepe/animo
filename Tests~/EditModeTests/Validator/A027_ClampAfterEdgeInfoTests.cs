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
    /// <summary>Decision-table tests for Validator rule A027: [info] influence applies clamp after each edge.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A027_ClampAfterEdgeInfoTests {
        [Test] public void Case01_StaticValidator_DoesNotEmitA027AsError() {
            // A027 is informational (clamp after each edge in influence chain).
            // Guard test: the static Validator never misclassifies A027 as Error.
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root);
            foreach (Issue i in r.issues) if (i.rule_id == "A027") Assert.That(i.severity, Is.Not.EqualTo(expected: Severity.Error));
        }
    }
}
