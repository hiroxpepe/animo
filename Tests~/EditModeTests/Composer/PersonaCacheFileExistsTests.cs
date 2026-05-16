// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Compile-time test for Q-S79 (v0.1.5): `Animo.PersonaCache` type
    /// resolves at runtime — the spec described the implementation in
    /// §11.6.1 but the physical .cs file did not exist before Q-S79.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PersonaCacheFileExistsTests {
        [Test] public void Case01_PersonaCacheType_Resolves() {
            var t = typeof(Animo.PersonaCache);
            Assert.That(t, Is.Not.Null,
                "Q-S79: Animo.PersonaCache type must resolve at runtime.");
            // Verify the three method declarations match §11.6.1 signatures.
            var initialize = t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "Initialize");
            var getComposed = t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "GetComposed");
            var clearForTesting = t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "ClearForTesting");
            Assert.That(initialize, Is.Not.Null, "Q-S79: PersonaCache.Initialize required.");
            Assert.That(getComposed, Is.Not.Null, "Q-S79: PersonaCache.GetComposed required.");
            Assert.That(clearForTesting, Is.Not.Null, "Q-S79: PersonaCache.ClearForTesting required.");
        }
    }
}
