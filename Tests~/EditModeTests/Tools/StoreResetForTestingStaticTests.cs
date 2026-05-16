// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Compile-time test for Q-S78 (v0.1.5): `Store.ResetForTesting()`
    /// is declared as a static method (must be invoked as
    /// `Animo.Store.ResetForTesting()`, not via `Animo.Store.Instance.
    /// ResetForTesting()` which CS0176 forbids).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class StoreResetForTestingStaticTests {
        [Test] public void Case01_Store_ResetForTestingIsStatic() {
            var storeType = typeof(Animo.Store);
            var method = storeType.GetMethods(BindingFlags.Public | BindingFlags.Static
                                            | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "ResetForTesting"
                    && m.GetParameters().Length == 0);
            Assert.That(method, Is.Not.Null,
                "Q-S78: Store.ResetForTesting() declaration required.");
            Assert.That(method!.IsStatic, Is.True,
                "Q-S78: ResetForTesting must be static — calling via Instance " +
                "(CS0176) is forbidden.");
        }
    }
}
