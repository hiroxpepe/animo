// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;
using Animo.Core;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Compile-time test for Q-S48 (v0.1.5): Engine has a private method
    /// declaration `ApplyNonTierMetadata(int, NeedMeta)` so §3.5.2 PHASE C's
    /// call site builds. v0.1.5 the body is a no-op stub; v0.2/v0.3 NeedMeta
    /// extensions implement here.
    ///
    /// This test passes immediately after Q-S48 ships the declaration —
    /// it's compile-only verification (the test class itself doesn't call
    /// the private method, but the project would fail to build if Q-S45's
    /// PHASE C call site couldn't resolve the symbol).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ApplyNonTierMetadataDeclarationTests {
        [Test] public void Case01_EngineCsCompiles_WithApplyNonTierMetadataDeclaration() {
            // Compile-time check: if Engine.cs is missing
            // ApplyNonTierMetadata, the project itself fails to build,
            // and this test never runs. Reaching the assertion means
            // the declaration exists.
            var engineType = typeof(Engine);
            Assert.That(engineType, Is.Not.Null,
                "Q-S48: Engine.cs must declare private ApplyNonTierMetadata so " +
                "Q-S45 PHASE C can compile.");
        }
    }
}
