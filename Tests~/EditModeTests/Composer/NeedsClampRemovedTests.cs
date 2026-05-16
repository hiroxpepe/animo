// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Model;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Compile-time test for Q-S63 (v0.1.5): `Needs.Clamp()` is removed
    /// from `Scripts/Data.cs`. The method had been dead code since
    /// v0.1.2's hot-path migration to flat float[] + Mathf.Clamp,
    /// existing only as a NotImplementedException trap for tool authors.
    ///
    /// This test passes immediately after Q-S63 ships the deletion —
    /// reflection asserts no `Clamp` method exists on `Needs`.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class NeedsClampRemovedTests {
        [Test] public void Case01_NeedsClass_HasNoClampMethod() {
            var needsType = typeof(Needs);
            var clampMethod = needsType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "Clamp");
            Assert.That(clampMethod, Is.Null,
                "Q-S63: Needs.Clamp() must be removed (was dead code since v0.1.2). " +
                "Hot path uses flat float[] _needs with Mathf.Clamp directly per §16.2. " +
                "The method existed only as a NotImplementedException trap.");
        }
    }
}
