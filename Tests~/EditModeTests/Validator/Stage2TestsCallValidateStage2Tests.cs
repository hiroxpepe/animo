#nullable enable
using NUnit.Framework;
namespace Animo.Tests.EditMode.ValidatorTests {
    [TestFixture]
    public class Stage2TestsCallValidateStage2Tests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [NUnit.Framework.TestCaseAttribute("A025_GhostCycleStage2Tests.cs")]
        [NUnit.Framework.TestCaseAttribute("A035_PostComposeTriggerGtResetTests.cs")]
        [NUnit.Framework.TestCaseAttribute("A036_ComposedActionsEmptyTests.cs")]
        [NUnit.Framework.TestCaseAttribute("A037_MultiEdgeSameTargetTests.cs")]
        public void Case_AllFourFiles_CallValidateStage2NotValidate(string filename) {
            string? path = null;
            { var p = System.IO.Path.Combine(RepoRoot(), "Tests~", "EditModeTests", "Validator", filename); if (System.IO.File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, $"Q-S90: {filename} must exist.");
            var text = System.IO.File.ReadAllText(path!);
            Assert.That(text, Does.Contain("ValidateStage2"), $"Q-S90: {filename} must call ValidateStage2.");
        }
    }
}
