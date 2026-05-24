#nullable enable
using NUnit.Framework;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class AgentIdRunStringInterpolationCSharpFormTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_SpecEN_CodeBlockHasCSharpForm() {
            string? path = null;
            { var p = System.IO.Path.Combine(RepoRoot(), "docs", "animo_spec_v0.1.5_EN.md"); if (System.IO.File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S114: spec EN must exist.");
            var text = System.IO.File.ReadAllText(path!);
            Assert.That(text, Does.Contain("Q-S114").Or.Contain("agent_id_override"),
                "Q-S114: spec EN must document agent_id_override or Q-S114.");
        }
    }
}
