// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Decision-table test for Q-S60 (v0.1.5): ScenarioRunner's internal
    /// state is a single Engine instance per Run() call, not a
    /// Dictionary<string, Engine>. The v0.1.5 Run signature accepts one
    /// template id; a routing dictionary would always have one entry.
    ///
    /// Phase 3 contract: ScenarioRunner has a single `Engine _engine`
    /// internal field. Reflection on the type asserts no
    /// Dictionary<string, Engine> field exists at this version.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerSingleEngineFieldTests {
        [Test] public void Case01_RunnerInternal_IsSingleEngineNotDictionary() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Reflection on ScenarioRunner type must show a single private Engine field, " +
                "no Dictionary<string, Engine>. Q-S60 fix: YAGNI applied — the dictionary " +
                "shape arrives only when v0.2 adds multi-agent Run().");
        }
    }
}
