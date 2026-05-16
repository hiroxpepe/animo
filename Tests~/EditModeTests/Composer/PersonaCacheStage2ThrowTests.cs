// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Decision-table test for Q-S38 (v0.1.5): PersonaCache.GetComposed
    /// MUST throw InvalidOperationException when stage-2 validation
    /// reports errors (e.g. composed actions[] empty per A036).
    /// Pre-Q-S38 it logged and returned the broken Persona, letting
    /// Engine build and crash on first Live(dt) inside Update().
    ///
    /// Phase 3 contract: GetComposed throws; Agent.Awake catches
    /// and disables itself; the rest of the scene continues.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PersonaCacheStage2ThrowTests {
        [Test] public void Case01_Stage2HasErrors_GetComposedThrowsInvalidOperationException() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "PersonaCache.GetComposed must throw InvalidOperationException when " +
                "Validator.ValidateStage2(composed) returns HasErrors==true (e.g. A036). " +
                "Pre-Q-S38 the broken Persona was returned and Engine crashed on first " +
                "Live via Q-S9 actions.First() on empty list. See §11.6 GetComposed " +
                "code (Q-S38) for the throw + §11.4.1 Awake code for the catch.");
        }
    }
}
