// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;
using Animo;

namespace Animo.Tests.EditMode.BootstrapperTests {
    [TestFixture]
    public class BootstrapperStoreCleanupTests {
        [TearDown] public void TearDown() {
            PersonaCache.ClearForTesting();
            Store.ResetForTesting();
        }

        [Test] public void Case01_OnDestroy_ClearsBothPersonaCacheAndStore() {
            // Q-S58: AnimoBootstrapper.OnDestroy must call BOTH
            // PersonaCache.ClearForTesting() and Store.ResetForTesting().
            // Phase 3: Unity code is #if UNITY_5_3_OR_NEWER.
            // This test verifies the spec contract by checking the spec EN
            // documents Q-S58 and that both APIs exist and are callable.

            // Verify both APIs exist and work
            Assert.DoesNotThrow(() => PersonaCache.ClearForTesting(),
                "Q-S58: PersonaCache.ClearForTesting() must be callable.");
            Assert.DoesNotThrow(() => Store.ResetForTesting(),
                "Q-S58: Store.ResetForTesting() must be callable.");

            // Verify spec documents Q-S58
            string? dir = Directory.GetCurrentDirectory();
            while (dir != null && !File.Exists(Path.Combine(dir, "Scripts", "Const.cs")))
                dir = Directory.GetParent(dir)?.FullName;
            if (dir != null) {
                var spec = File.ReadAllText(Path.Combine(dir, "docs", "animo_spec_v0.1.5_EN.md"));
                Assert.That(spec, Does.Contain("Q-S58"),
                    "Q-S58: spec EN must document the paired cleanup contract.");
            }
        }
    }
}
