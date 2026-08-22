// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EdgeCaseTests {
    /// <summary>
    /// File-string compliance tests for Q-S132, Q-S133, Q-S136, Q-S137,
    /// Q-S138, Q-S139 (v0.1.5, Phase_2_4_26). Gemini round 29.
    ///
    /// Each case verifies that a specific contract is recorded in the
    /// physical source file — the same "process-discipline" layer
    /// introduced by Q-S101 / Q-S119.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Round29ContractTests {

        static string RepoRoot() {
            string? dir = Directory.GetCurrentDirectory();
            while (dir != null && !File.Exists(Path.Combine(dir, "Scripts", "Const.cs")))
                dir = Directory.GetParent(dir)?.FullName;
            return dir ?? Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_ScenarioRunner_DocumentsSystemMathRound() {
            // Q-S136: ScenarioRunner.cs docstring must use System.Math.Round
            // (fully qualified) per Q-S127 pattern — no `using System;` in file.
            var path = Path.Combine(RepoRoot(), "Scripts", "Tools", "ScenarioRunner.cs");
            Assert.That(File.Exists(path), Is.True, $"ScenarioRunner.cs not found at {path}");
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("System.Math.Round"),
                "Q-S136: ScenarioRunner.cs must reference System.Math.Round " +
                "(fully qualified) to prevent CS0103 when Phase 3 transcribes the pseudocode.");
        }

        [Test] public void Case02_AnimoBootstrapper_DocumentsParseFailContract() {
            // Q-S133: AnimoBootstrapper.cs must document the JSON parse
            // fail-loud contract so Phase 3 does not swallow the exception.
            var path = Path.Combine(RepoRoot(), "Scripts", "AnimoBootstrapper.cs");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("JSON parse failure contract"),
                "Q-S133: AnimoBootstrapper.cs must document the JSON parse fail-loud contract.");
        }

        [Test] public void Case03_Validator_DocumentsValidationResultInternalDesign() {
            // Q-S138: Validator.cs must document the O(1) internal backing-list
            // design for has_errors / errors / warnings / infos.
            var path = Path.Combine(RepoRoot(), "Scripts", "Core", "Validator.cs");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("SIBLING_THRESHOLD_EPSILON"),
                "Q-S135: Validator.cs ValidateStage2 docstring must reference SIBLING_THRESHOLD_EPSILON.");
            Assert.That(text, Does.Contain("O(1) per query"),
                "Q-S138: Validator.cs must document the O(1) internal backing-list design.");
        }

        [Test] public void Case04_MockScene_NoduplicateNullableEnable() {
            // Q-S139: MiniUnity source files must not have duplicate
            // `#nullable enable` directives — one at file top is sufficient.
            string[] files = {
                Path.Combine(RepoRoot(), "Tests~", "MiniUnity", "MockBus.cs"),
                Path.Combine(RepoRoot(), "Tests~", "MiniUnity", "MockGameObject.cs"),
                Path.Combine(RepoRoot(), "Tests~", "MiniUnity", "MockMonoBehaviour.cs"),
                Path.Combine(RepoRoot(), "Tests~", "MiniUnity", "MockScene.cs"),
            };
            foreach (var f in files) {
                Assert.That(File.Exists(f), Is.True, $"MiniUnity file not found: {f}");
                var text = File.ReadAllText(f);
                int count = 0;
                int idx = 0;
                while ((idx = text.IndexOf("#nullable enable", idx, System.StringComparison.Ordinal)) >= 0) {
                    count++;
                    idx++;
                }
                Assert.That(count, Is.EqualTo(1),
                    $"Q-S139: {Path.GetFileName(f)} must have exactly one '#nullable enable' " +
                    $"(found {count}). Duplicate in namespace body is cosmetically wrong.");
            }
        }

        [Test] public void Case05_TraceFrame_DocumentsPhase3LightweightSnapshotNote() {
            // Q-S132: TraceResult.cs must document the OOM risk of per-frame
            // Dictionary allocation and the Phase 3 mitigation strategy.
            var path = Path.Combine(RepoRoot(), "Scripts", "Tools", "TraceResult.cs");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("Q-S132"),
                "Q-S132: TraceResult.cs TraceFrame class docstring must reference Q-S132 " +
                "Phase 3 lightweight snapshot implementation note.");
        }

        [Test] public void Case06_MockScene_DocumentsITimeProviderAddContract() {
            // Q-S137: MockScene.cs must document how ITimeProvider is
            // conveyed to an Agent when calling MockScene.Add.
            var path = Path.Combine(RepoRoot(), "Tests~", "MiniUnity", "MockScene.cs");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("Q-S137"),
                "Q-S137: MockScene.cs Add method must document the ITimeProvider DI pattern.");
        }
    }
}
