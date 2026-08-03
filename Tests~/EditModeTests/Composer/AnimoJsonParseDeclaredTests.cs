// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Model;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Compile-time test for Q-S76 (v0.1.5): `Animo.JSON.Parse(string)`
    /// is declared. Pre-Q-S76 §11.6.5 AnimoBootstrapper called
    /// Animo.JSON.Parse(...) but neither the class nor method existed —
    /// confirmed missing-type compile error.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AnimoJsonParseDeclaredTests {
        [Test] public void Case01_AnimoJson_DeclaresParseMethod() {
            var jsonType = typeof(Animo.JSON);
            var parse = jsonType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "Parse"
                    && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == typeof(string));
            Assert.That(parse, Is.Not.Null,
                "Q-S76: Animo.JSON.Parse(string) declaration required for AnimoBootstrapper.");
            Assert.That(parse!.ReturnType, Is.EqualTo(typeof(Root)),
                "Q-S76: Parse must return Animo.Model.Root.");
        }
    }
}
