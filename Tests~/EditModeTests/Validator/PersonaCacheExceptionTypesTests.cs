// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Compile-time test for Q-S111 (v0.1.5): two distinct exception
    /// types exist in Animo namespace —
    /// PersonaCacheNotInitializedException (architectural startup) and
    /// PersonaTemplateRejectedException (per-Agent authoring). Both
    /// inherit InvalidOperationException for backward compatibility.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PersonaCacheExceptionTypesTests {
        static Type? FindAnimoType(string name) {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => {
                    try { return a.GetTypes(); }
                    catch { return new Type[0]; }
                })
                .FirstOrDefault(t => t.Name == name && t.Namespace == "Animo");
        }

        [Test] public void Case01_PersonaCacheNotInitializedException_Exists() {
            var t = FindAnimoType("PersonaCacheNotInitializedException");
            Assert.That(t, Is.Not.Null,
                "Q-S111: Animo.PersonaCacheNotInitializedException must exist.");
            Assert.That(typeof(InvalidOperationException).IsAssignableFrom(t!), Is.True,
                "Q-S111: must inherit InvalidOperationException.");
        }

        [Test] public void Case02_PersonaTemplateRejectedException_Exists() {
            var t = FindAnimoType("PersonaTemplateRejectedException");
            Assert.That(t, Is.Not.Null,
                "Q-S111: Animo.PersonaTemplateRejectedException must exist.");
            Assert.That(typeof(InvalidOperationException).IsAssignableFrom(t!), Is.True,
                "Q-S111: must inherit InvalidOperationException.");
        }
    }
}
