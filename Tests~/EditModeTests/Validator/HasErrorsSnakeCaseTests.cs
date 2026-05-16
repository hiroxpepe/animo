// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Core;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Compile-time test for Q-S74 (v0.1.5): `ValidationResult.has_errors`
    /// uses snake_case. Pre-Q-S74 §11.6.1 sample code wrote `HasErrors`
    /// (PascalCase) while Validator.cs declared `has_errors` (snake_case)
    /// — confirmed property-not-found compile error. Animo's API surface
    /// uniformly uses snake_case (Persona.agent_id, Issue.rule_id, etc.).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class HasErrorsSnakeCaseTests {
        [Test] public void Case01_ValidationResult_DeclaresHasErrorsAsSnakeCase() {
            var resultType = typeof(ValidationResult);
            var prop = resultType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.Name == "has_errors");
            Assert.That(prop, Is.Not.Null,
                "Q-S74: ValidationResult.has_errors (snake_case) required for API consistency.");
            Assert.That(prop!.PropertyType, Is.EqualTo(typeof(bool)),
                "Q-S74: has_errors must be a bool.");
        }
    }
}
