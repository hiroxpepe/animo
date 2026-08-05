// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Core;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Compile-time test for Q-S70 (v0.1.5): `Engine._lock_remaining`
    /// field is declared. Pre-Q-S70 the §9.2 T0 timer phase pseudocode
    /// and §24.3 narrative referenced this field but it had no entry
    /// in §16.6's Engine fields table and no declaration in
    /// Scripts/Engine.cs — confirmed compile error for any Phase 3
    /// implementation of T0 / Lock / Unlock.
    ///
    /// This test passes immediately after Q-S70 ships the declaration —
    /// reflection asserts the field exists as float.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class LockRemainingFieldDeclaredTests {
        [Test] public void Case01_EngineClass_DeclaresLockRemainingField() {
            var engineType = typeof(Engine);
            var lockRemaining = engineType.GetFields(
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(f => f.Name == "_lock_remaining");
            Assert.That(lockRemaining, Is.Not.Null,
                "Q-S70: Engine._lock_remaining field declaration required for §9.2 T0 " +
                "timer phase + §24 Lock mechanism implementation.");
            Assert.That(lockRemaining!.FieldType, Is.EqualTo(typeof(float)),
                "Q-S70: _lock_remaining must be a float (matches §9.2 mermaid " +
                "pseudocode: `_lock_remaining -= delta_time; if (_lock_remaining ≤ 0) Unlock()`).");
        }
    }
}
