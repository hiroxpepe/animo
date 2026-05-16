// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Core;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Compile-time test for Q-S110 (v0.1.5): Engine declares the
    /// _previous_behavior string field (per §16.6 + Q-S31 silent-
    /// first-transition contract). Pre-Q-S110 only _persona and
    /// _lock_remaining were declared — same physical-gap pattern as
    /// Q-S70's _lock_remaining fix.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class EnginePreviousBehaviorFieldTests {
        [Test] public void Case01_Engine_DeclaresPreviousBehaviorField() {
            var t = typeof(Engine);
            var field = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(f => f.Name == "_previous_behavior" && f.FieldType == typeof(string));
            Assert.That(field, Is.Not.Null,
                "Q-S110: Engine must declare string _previous_behavior field " +
                "for the Q-S31 silent-first-transition contract (§16.6).");
        }
    }
}
