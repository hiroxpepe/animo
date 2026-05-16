// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S65 (v0.1.5): Engine ctor PHASE A
    /// loops correctly unwrap `_persona.needs?.values` (Dictionary)
    /// rather than the `Needs` wrapper class itself. Pre-Q-S65 the
    /// code wrote `_persona.needs ?? new Dictionary<string, float>()`
    /// — confirmed type-mismatch compile error.
    ///
    /// Phase 3 contract: Engine ctor accepts a Persona with `needs`
    /// = null, with `needs.values` = empty, and with `needs.values`
    /// populated. All three produce a valid Engine without
    /// NullReferenceException.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PhaseANeedsUnwrapTests {
        [Test] public void Case01_PersonaWithNullNeeds_DoesNotThrow() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "new Engine(persona) where persona.needs == null must NOT throw " +
                "NullReferenceException. Q-S65 fix: foreach (var kv in " +
                "_persona.needs?.values ?? new Dictionary<string, float>()) — null-safe " +
                "via ?.values + ?? fallback.");
        }
    }
}
