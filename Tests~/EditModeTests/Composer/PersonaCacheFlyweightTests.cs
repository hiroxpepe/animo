// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Decision-table tests for Q-S29 (v0.1.5): Animo.PersonaCache must
    /// validate the Root once at Initialize and compose each template
    /// at most once per session.
    ///
    /// Phase 3 contract: `Animo.PersonaCache` static class with
    /// Initialize(Root), GetComposed(string), ClearForTesting(). See
    /// spec §11.6 for the full surface and §11.6.5 for the bootstrapper
    /// pattern. Test bodies are Phase 3 work; this fixture pins the
    /// spec expectation in test form (Red baseline).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PersonaCacheFlyweightTests {

        [Test] public void Case01_GetComposedBeforeInitialize_ThrowsInvalidOperation() {
            // Phase 3: PersonaCache.GetComposed without prior Initialize → throws.
            // Master's policy: fail-loud, not lazy-init.
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Animo.PersonaCache static class with Initialize+GetComposed+ClearForTesting. " +
                "See §11.6 (Q-S29) for full contract.");
        }

        [Test] public void Case02_RepeatedGetComposed_ReturnsSameInstance() {
            // Phase 3: repeated GetComposed for same template_id → SameAs.
            Assert.Fail(message: "Phase 3 implementation pending — see §11.6 (Q-S29).");
        }
    }
}
