#nullable enable
using NUnit.Framework;
namespace Animo.Tests.EditMode.ValidatorTests {
    [TestFixture]
    public class A011aRuleIdTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_A011PersonaActionsRequiredTests_AssertsA011a() {
            string? path = null;
            { var p = System.IO.Path.Combine(RepoRoot(), "Tests~", "EditModeTests", "Validator", "A011_PersonaActionsRequiredTests.cs"); if (System.IO.File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S100: A011_PersonaActionsRequiredTests.cs must exist.");
            var text = System.IO.File.ReadAllText(path!);
            Assert.That(text, Does.Contain("\"A011a\""), "Q-S100: test must assert rule_id A011a.");
        }
        [Test] public void Case02_EmptyAndNullTests_AssertsA011a() {
            string? path = null;
            { var p = System.IO.Path.Combine(RepoRoot(), "Tests~", "EditModeTests", "EdgeCases", "EmptyAndNullTests.cs"); if (System.IO.File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S100: EmptyAndNullTests.cs must exist.");
            var text = System.IO.File.ReadAllText(path!);
            Assert.That(text, Does.Contain("A011"), "Q-S100: EmptyAndNullTests must cover A011.");
        }
    }
}
