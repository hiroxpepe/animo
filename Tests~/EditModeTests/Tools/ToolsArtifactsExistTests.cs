// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Compile-time test for Q-S82 (v0.1.5): Tools/ScenarioRunner.cs +
    /// Tools/TraceResult.cs + Tools/Animo.Tools.asmdef materialized.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ToolsArtifactsExistTests {
        [Test] public void Case01_ScenarioRunnerType_Resolves() {
            var t = typeof(Animo.Tools.ScenarioRunner);
            Assert.That(t, Is.Not.Null,
                "Q-S82: Animo.Tools.ScenarioRunner type must resolve.");
        }

        [Test] public void Case02_TraceResultType_Resolves() {
            var t = typeof(Animo.Tools.TraceResult);
            Assert.That(t, Is.Not.Null,
                "Q-S82: Animo.Tools.TraceResult type must resolve.");
        }

        [Test] public void Case03_TraceFrameType_Resolves() {
            var t = typeof(Animo.Tools.TraceFrame);
            Assert.That(t, Is.Not.Null,
                "Q-S82: Animo.Tools.TraceFrame type must resolve.");
        }
    }
}
