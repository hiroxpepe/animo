// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Decision-table test for Q-S85 (v0.1.5): MergeThresholds is
    /// order-deterministic via first-occurrence-wins semantics.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ThresholdsMatchOrderDeterminismTests {
        [Test] public void Case01_NonTransitiveEpsilon_FirstOccurrenceWins() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Given Kind thresholds [A=80.000, B=80.006] and Persona threshold " +
                "[C=80.012], MergeThresholds must produce a deterministic result " +
                "regardless of A/B input order. Q-S85 fix: first-occurrence-wins — " +
                "Persona's C matches the FIRST entry in merged list (A), overriding A; " +
                "B remains untouched.");
        }
    }
}
