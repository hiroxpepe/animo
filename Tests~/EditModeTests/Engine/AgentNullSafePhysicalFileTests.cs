#nullable enable
using NUnit.Framework;
namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class AgentNullSafePhysicalFileTests {
        static string RepoRoot() {
            string? d = System.IO.Directory.GetCurrentDirectory();
            while (d != null && !System.IO.File.Exists(System.IO.Path.Combine(d, "Scripts", "Const.cs")))
                d = System.IO.Directory.GetParent(d)?.FullName;
            return d ?? System.IO.Directory.GetCurrentDirectory();
        }

        [Test] public void Case01_PhysicalAgentCs_HasNullSafeAgentIdGetter() {
            string? path = null;
            { var p = System.IO.Path.Combine(RepoRoot(), "Scripts", "Agent.cs"); if (System.IO.File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S101: Scripts/Agent.cs must exist.");
            var text = System.IO.File.ReadAllText(path!);
            Assert.That(text, Does.Contain("agent_id"), "Q-S101: Agent.cs must have agent_id property.");
        }
        [Test] public void Case02_PhysicalAgentCs_HasOnDestroyGuard() {
            string? path = null;
            { var p = System.IO.Path.Combine(RepoRoot(), "Scripts", "Agent.cs"); if (System.IO.File.Exists(p)) path = p; }
            Assert.That(path, Is.Not.Null, "Q-S101: Scripts/Agent.cs must exist.");
            var text = System.IO.File.ReadAllText(path!);
            Assert.That(text, Does.Contain("OnDestroy"), "Q-S101: Agent.cs must have OnDestroy null-safe guard.");
        }
    }
}
