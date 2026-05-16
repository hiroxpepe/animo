// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Decision-table test for Q-S35 (v0.1.5): ScenarioRunner.Run with
    /// duration=10.0f, dt=0.1f must call Engine.Live exactly 100 times,
    /// not 101. Q-S33's `<= duration + EPSILON` ran one extra Live;
    /// Q-S35 final form (strict `<` outer + post-loop sweep) is exact.
    ///
    /// Phase 3 contract: total Live calls = floor(duration / dt).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerOverShootTests {
        [Test] public void Case01_DurationExactMultipleOfDt_DoesNotOverShoot() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Animo.Tools.ScenarioRunner.Run(duration: 10.0f, dt: 0.1f) must call " +
                "Engine.Live exactly 100 times. Pre-Q-S35 the EPSILON form ran 101 times. " +
                "See §26.3.1 + §26.3.1a (Q-S35 final form) for the iteration trace.");
        }
    }
}
