#nullable enable
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class NeedTierIndicesIsIntArrayTests {
        [Test] public void Case01_NeedTierIndices_FieldTypeIsDictionaryOfIntArray() {
            var field = typeof(Engine).GetField("_need_tier_indices",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(field, Is.Not.Null, "Q-S69: Engine must have _need_tier_indices field.");
            Assert.That(field!.FieldType,
                Is.EqualTo(typeof(Dictionary<int, int[]>)),
                "Q-S69: _need_tier_indices must be Dictionary<int, int[]>, not List<int>.");
        }
    }
}
