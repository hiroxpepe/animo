// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Decision-table test for Q-S61 (v0.1.5): Persona inheritance is
    /// additive — a child Persona inheriting from a Kind cannot remove
    /// a Kind Action by omission. Every Kind Action whose `id` is
    /// missing from the Persona is appended at the tail.
    ///
    /// Phase 3 contract: Composer.Compose with Persona omitting
    /// Kind's "Idle" Action still produces composed actions[] containing
    /// "Idle" (from Kind, appended at tail per Q-S19).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PersonaInheritanceAdditiveTests {
        [Test] public void Case01_PersonaOmitsKindAction_KindActionStillAppendedAtTail() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Composer.Compose with Kind = [Idle, Patrol] and Persona = [Attack] must " +
                "produce composed actions[] = [Attack, Idle, Patrol] — Persona-first, then " +
                "Kind appended. Q-S61 design property: inheritance is additive, never " +
                "subtractive (children can't lose critical Kind fallbacks like Idle).");
        }
    }
}
