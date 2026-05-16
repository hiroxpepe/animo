// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo;

namespace Animo.Tests.EditMode.EdgeCaseTests {
    /// <summary>
    /// Decision-table test for Q-S131 (v0.1.5): all public string constants
    /// in Const that were previously `string[]` (mutable elements despite
    /// `readonly` field) are now `IReadOnlyList&lt;string&gt;` — same pattern as
    /// Q-S128 which widened NEED_INDICES_BY_TIER.
    ///
    /// Pre-Q-S131: `Const.STANDARD_NEEDS[0] = "fake"` would compile and
    /// silently corrupt the Maslow hierarchy for the entire process.
    /// Q-S131 closes the same hole for STANDARD_NEEDS,
    /// SUPPORTED_SCHEMA_VERSIONS, TEMPLATE_PLACEHOLDERS_ACTION, and
    /// TEMPLATE_PLACEHOLDERS_THRESHOLD.
    ///
    /// Phase 2: structural (type-level) assertions — verifiable without
    /// Phase 3 engine implementation.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ConstReadOnlyListTests {

        [Test] public void Case01_STANDARD_NEEDS_IsIReadOnlyList() {
            Assert.That(Const.STANDARD_NEEDS, Is.InstanceOf<IReadOnlyList<string>>(),
                "Q-S131: Const.STANDARD_NEEDS must be IReadOnlyList<string> — " +
                "string[] allows element mutation even with readonly field.");
        }

        [Test] public void Case02_SUPPORTED_SCHEMA_VERSIONS_IsIReadOnlyList() {
            Assert.That(Const.SUPPORTED_SCHEMA_VERSIONS, Is.InstanceOf<IReadOnlyList<string>>(),
                "Q-S131: Const.SUPPORTED_SCHEMA_VERSIONS must be IReadOnlyList<string>.");
        }

        [Test] public void Case03_TEMPLATE_PLACEHOLDERS_ACTION_IsIReadOnlyList() {
            Assert.That(Const.TEMPLATE_PLACEHOLDERS_ACTION, Is.InstanceOf<IReadOnlyList<string>>(),
                "Q-S131: Const.TEMPLATE_PLACEHOLDERS_ACTION must be IReadOnlyList<string>.");
        }

        [Test] public void Case04_TEMPLATE_PLACEHOLDERS_THRESHOLD_IsIReadOnlyList() {
            Assert.That(Const.TEMPLATE_PLACEHOLDERS_THRESHOLD, Is.InstanceOf<IReadOnlyList<string>>(),
                "Q-S131: Const.TEMPLATE_PLACEHOLDERS_THRESHOLD must be IReadOnlyList<string>.");
        }

        [Test] public void Case05_STANDARD_NEEDS_ContainsExpectedEight() {
            // Verify the values were not lost in the type widening.
            Assert.That(Const.STANDARD_NEEDS.Count, Is.EqualTo(8),
                "Q-S131: STANDARD_NEEDS must still have 8 entries after IReadOnlyList widening.");
            Assert.That(Const.STANDARD_NEEDS[Const.NEED_INDEX_HUNGER],     Is.EqualTo("hunger"));
            Assert.That(Const.STANDARD_NEEDS[Const.NEED_INDEX_FATIGUE],    Is.EqualTo("fatigue"));
            Assert.That(Const.STANDARD_NEEDS[Const.NEED_INDEX_FEAR],       Is.EqualTo("fear"));
            Assert.That(Const.STANDARD_NEEDS[Const.NEED_INDEX_LONELINESS], Is.EqualTo("loneliness"));
            Assert.That(Const.STANDARD_NEEDS[Const.NEED_INDEX_CONFIDENCE], Is.EqualTo("confidence"));
            Assert.That(Const.STANDARD_NEEDS[Const.NEED_INDEX_CURIOSITY],  Is.EqualTo("curiosity"));
            Assert.That(Const.STANDARD_NEEDS[Const.NEED_INDEX_IDLE],       Is.EqualTo("idle"));
            Assert.That(Const.STANDARD_NEEDS[Const.NEED_INDEX_FRUSTRATION],Is.EqualTo("frustration"));
        }
    }
}
