// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo;

namespace Animo.Tests.EditMode.EdgeCasesTests {
    /// <summary>
    /// Reflection test for Q-S128 (v0.1.5): Const.NEED_INDICES_BY_TIER
    /// is exposed as IReadOnlyDictionary&lt;int, IReadOnlyList&lt;int&gt;&gt;
    /// so external code cannot mutate the tier mapping. Pre-Q-S128 the
    /// type was Dictionary&lt;int, int[]&gt; which left both layers mutable.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ConstReadOnlyTierIndicesTests {
        [Test] public void Case01_NEED_INDICES_BY_TIER_IsReadOnlyDictionary() {
            var field = typeof(Const).GetField("NEED_INDICES_BY_TIER",
                BindingFlags.Public | BindingFlags.Static);
            Assert.That(field, Is.Not.Null,
                "Q-S128: Const.NEED_INDICES_BY_TIER must be declared.");
            var field_type = field!.FieldType;
            // Should be IReadOnlyDictionary<int, IReadOnlyList<int>>
            Assert.That(field_type.IsGenericType, Is.True,
                "Q-S128: NEED_INDICES_BY_TIER must be a generic type.");
            var generic_def = field_type.GetGenericTypeDefinition();
            Assert.That(generic_def, Is.EqualTo(typeof(System.Collections.Generic.IReadOnlyDictionary<,>)),
                "Q-S128: NEED_INDICES_BY_TIER must be IReadOnlyDictionary<,>.");
            var args = field_type.GetGenericArguments();
            Assert.That(args[0], Is.EqualTo(typeof(int)),
                "Q-S128: outer key type must be int (tier).");
            Assert.That(args[1].IsGenericType, Is.True,
                "Q-S128: inner type must be a generic type.");
            Assert.That(args[1].GetGenericTypeDefinition(),
                Is.EqualTo(typeof(System.Collections.Generic.IReadOnlyList<>)),
                "Q-S128: inner type must be IReadOnlyList<int>.");
        }
    }
}
