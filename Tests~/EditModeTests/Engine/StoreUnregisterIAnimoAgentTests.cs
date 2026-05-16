// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Compile-time test for Q-S81 (v0.1.5): `Store.Unregister` accepts
    /// `IAnimoAgent` (interface form), not `Animo.Agent` (concrete form).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class StoreUnregisterIAnimoAgentTests {
        [Test] public void Case01_Store_UnregisterAcceptsIAnimoAgent() {
            var storeType = typeof(Animo.Store);
            var method = storeType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "Unregister"
                    && m.GetParameters().Length == 1);
            Assert.That(method, Is.Not.Null,
                "Q-S81: Store.Unregister(...) declaration required.");
            var paramType = method!.GetParameters()[0].ParameterType;
            Assert.That(paramType, Is.EqualTo(typeof(Animo.IAnimoAgent)),
                "Q-S81: Unregister parameter type MUST be IAnimoAgent (interface), " +
                "not Animo.Agent (concrete). Concrete-form would create an incompatible " +
                "overload that fails to satisfy the interface contract.");
        }
    }
}
