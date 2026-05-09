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
    /// <summary>Decision-table tests for Validator rule A031: Lock(duration) over 30s warns at runtime (v0.1.4).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A031_LockDurationWarnTests {
        [Test] public void Case01_StaticValidator_DoesNotEmitA031AsError() {
            // A031 is a runtime warning emitted by Engine.Lock(duration > 30) — the dynamic
            // case is covered by Engine/LockTests.Case06_LockDurationOver30s_TriggersA031Warning.
            // This guard test belongs in ValidatorTests so the 33-rule decision-table coverage
            // (spec §13.1) is structurally complete in one place; it asserts that the static
            // Validator never *misclassifies* A031 as a static Error.
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root);
            foreach (Issue i in r.issues) if (i.rule_id == "A031") Assert.That(i.severity, Is.Not.EqualTo(expected: Severity.Error));
        }
    }
}
