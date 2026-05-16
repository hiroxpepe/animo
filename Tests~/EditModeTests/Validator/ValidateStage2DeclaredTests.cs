// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Compile-time test for Q-S71 (v0.1.5): `Validator.ValidateStage2(
    /// Persona)` is declared. Pre-Q-S71 §11.6.1 PersonaCache called the
    /// method but Scripts/Validator.cs declared only Validate(Root) —
    /// confirmed missing-method compile error.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ValidateStage2DeclaredTests {
        [Test] public void Case01_Validator_DeclaresValidateStage2Method() {
            var validatorType = typeof(Validator);
            var method = validatorType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "ValidateStage2"
                    && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == typeof(Persona));
            Assert.That(method, Is.Not.Null,
                "Q-S71: Validator.ValidateStage2(Persona) declaration required.");
            Assert.That(method!.ReturnType, Is.EqualTo(typeof(ValidationResult)),
                "Q-S71: ValidateStage2 must return ValidationResult.");
        }
    }
}
