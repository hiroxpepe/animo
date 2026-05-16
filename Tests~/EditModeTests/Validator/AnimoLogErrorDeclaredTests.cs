// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>
    /// Compile-time test for Q-S73 (v0.1.5): `AnimoLog.Error(string)` is
    /// declared. Pre-Q-S73 PersonaCache.Initialize and Agent.Awake catch
    /// blocks called AnimoLog.Error but only Write and Warning were
    /// declared — confirmed missing-method compile error.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AnimoLogErrorDeclaredTests {
        [Test] public void Case01_AnimoLog_DeclaresErrorMethod() {
            var logType = typeof(Animo.AnimoLog);
            var method = logType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "Error"
                    && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == typeof(string));
            Assert.That(method, Is.Not.Null,
                "Q-S73: AnimoLog.Error(string) declaration required for fail-loud paths.");
        }
    }
}
