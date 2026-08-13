#nullable enable
using System.IO;
using NUnit.Framework;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class ScenarioRunnerMathRoundTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_RunLoopUsesMathRoundDoubleCast() {
            string? found = null;
            { var p = Path.Combine(RepoRoot(), "docs", "animo_spec.md"); if (File.Exists(p)) found = p; }
            Assert.That(found, Is.Not.Null, "Q-S98: spec EN must exist.");
            var text = File.ReadAllText(found!);
            Assert.That(text, Does.Contain("System.Math.Round"),
                "Q-S98: spec EN must reference System.Math.Round for total_steps.");
        }

        [Test] public void Case02_IEEE754_FloorWouldUnderShoot_RoundCorrects() {
            // Q-S98: double-precision division (double)duration / (double)delta_time
            // is used to avoid float32 accumulation errors.
            // Key: (double)10.0f / (double)0.1f = 99.9999985... (sub-unity error)
            // Math.Round gives 100 (correct). Math.Floor would give 99 (wrong).
            double ratio_double = (double)10.0f / (double)0.1f;
            int floorResult = (int)System.Math.Floor(ratio_double);
            int roundResult = (int)System.Math.Round(ratio_double);
            Assert.That(roundResult, Is.EqualTo(100),
                "Q-S98: Math.Round of (double)10.0f/(double)0.1f must give 100.");
            Assert.That(floorResult, Is.EqualTo(99),
                "Q-S98: Math.Floor of (double)10.0f/(double)0.1f gives 99 — that's the bug Q-S98 fixes.");
            Assert.That(floorResult, Is.LessThan(roundResult),
                "Q-S98: Floor under-shoots Round for this float32 input.");
        }
    }
}
