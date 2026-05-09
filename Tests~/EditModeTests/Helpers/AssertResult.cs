// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;
using Animo.Core;

namespace Animo.Tests.EditMode.Helpers {
    /// <summary>Assertion helpers for ValidationResult.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class AssertResult {

        /// <summary>Assert the result has zero errors.</summary>
        public static void IsClean(ValidationResult result) {
            Assert.That(result.has_errors, Is.False, "expected no errors");
        }

        /// <summary>Assert the result contains an Error issue with the given rule_id.</summary>
        public static void HasError(ValidationResult result, string rule_id) {
            Assert.That(result.has_errors, Is.True, $"expected at least one error of {rule_id}");
            Assert.That(result.HasRule(rule_id), Is.True, $"expected error {rule_id}");
        }

        /// <summary>Assert the result contains a Warning issue with the given rule_id.</summary>
        public static void HasWarning(ValidationResult result, string rule_id) {
            Assert.That(result.HasRule(rule_id), Is.True, $"expected warning {rule_id}");
        }
    }
}
