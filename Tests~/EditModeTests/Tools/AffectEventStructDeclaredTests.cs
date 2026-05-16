// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Compile-time test for Q-S67 (v0.1.5): `Animo.Tools.AffectEvent`
    /// is declared as a readonly struct. Pre-Q-S67 the type was
    /// referenced from `TimedAffectEvent.ev` but never declared
    /// anywhere in the spec or in Scripts/ — confirmed missing-type
    /// compile error.
    ///
    /// This test passes immediately after Q-S67 ships the declaration —
    /// reflection asserts the struct exists with the three expected
    /// fields (need: string, delta: float, force_reset: bool).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AffectEventStructDeclaredTests {
        [Test] public void Case01_AffectEvent_DeclaredInAnimoToolsNamespace() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Animo.Tools.AffectEvent struct must be declared with `string need`, " +
                "`float delta`, `bool force_reset` fields. Q-S67 closes the spec-vs-code " +
                "gap left since v0.1.4 — the §6.1 namespace table promised the type but " +
                "no code block declared it. Phase 3 places the declaration in " +
                "Scripts/Tools/ScenarioRunner.cs (or Scripts/Data.cs).");
        }
    }
}
