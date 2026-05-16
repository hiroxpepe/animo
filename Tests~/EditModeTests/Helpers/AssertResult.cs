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

        /// <summary>
        /// (v0.1.5, Q-S106) Assert the result contains an Error issue
        /// with the given rule_id.
        ///
        /// Pre-Q-S106 this checked `result.has_errors == true` AND
        /// `result.HasRule(rule_id) == true` — both passed when JSON
        /// yielded ANY error PLUS the named rule firing as a Warning.
        /// `HasError(result, "A028")` would pass when A028 fired only
        /// as a Warning (alongside any unrelated Error). False-positive
        /// trap. Q-S106 uses HasRuleWithSeverity so only an actual
        /// Error of the named rule passes.
        /// </summary>
        public static void HasError(ValidationResult result, string rule_id) {
            Assert.That(result.HasRuleWithSeverity(rule_id, Severity.Error), Is.True,
                $"expected error issue with rule_id {rule_id}");
        }

        /// <summary>
        /// (v0.1.5, Q-S106) Assert the result contains a Warning issue
        /// with the given rule_id (severity-tagged check).
        /// </summary>
        public static void HasWarning(ValidationResult result, string rule_id) {
            Assert.That(result.HasRuleWithSeverity(rule_id, Severity.Warning), Is.True,
                $"expected warning issue with rule_id {rule_id}");
        }
    }
}
