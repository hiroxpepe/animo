// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Core;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Compile-time test for Q-S72 (v0.1.5): `ValidationResult.Merge(
    /// ValidationResult)` is declared. Pre-Q-S72 §11.6.1 called
    /// _validation!.Merge(stage2) but ValidationResult had no Merge
    /// method — confirmed missing-method compile error.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ValidationResultMergeDeclaredTests {
        [Test] public void Case01_ValidationResult_DeclaresMergeMethod() {
            var resultType = typeof(ValidationResult);
            var method = resultType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "Merge"
                    && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == typeof(ValidationResult));
            Assert.That(method, Is.Not.Null,
                "Q-S72: ValidationResult.Merge(ValidationResult) declaration required.");
        }
    }
}
