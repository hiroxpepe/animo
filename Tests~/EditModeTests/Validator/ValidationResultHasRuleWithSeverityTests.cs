// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Core;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Compile-time test for Q-S106 (v0.1.5): ValidationResult exposes
    /// HasRuleWithSeverity(rule_id, severity) for severity-aware
    /// AssertResult helpers. Pre-Q-S106 only HasRule (severity-
    /// agnostic) existed and HasError(result, "A028") was a false-
    /// positive trap.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ValidationResultHasRuleWithSeverityTests {
        [Test] public void Case01_ValidationResult_DeclaresHasRuleWithSeverity() {
            var t = typeof(ValidationResult);
            var method = t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "HasRuleWithSeverity"
                    && m.GetParameters().Length == 2
                    && m.GetParameters()[0].ParameterType == typeof(string)
                    && m.GetParameters()[1].ParameterType == typeof(Severity));
            Assert.That(method, Is.Not.Null,
                "Q-S106: ValidationResult must declare HasRuleWithSeverity(string, Severity).");
            Assert.That(method!.ReturnType, Is.EqualTo(typeof(bool)),
                "Q-S106: HasRuleWithSeverity must return bool.");
        }
    }
}
