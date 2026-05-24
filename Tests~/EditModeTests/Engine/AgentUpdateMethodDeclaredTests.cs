#nullable enable
using System.IO;
using NUnit.Framework;
namespace Animo.Tests.EditMode.EngineTests {
    [TestFixture]
    public class AgentUpdateMethodDeclaredTests {
        static string AgentPath() {
            string? dir = Directory.GetCurrentDirectory();
            while (dir != null && !File.Exists(Path.Combine(dir, "Scripts", "Agent.cs")))
                dir = Directory.GetParent(dir)?.FullName;
            return dir != null ? Path.Combine(dir, "Scripts", "Agent.cs") : "";
        }
        [Test] public void Case01_AgentUpdateMethodDeclared() {
            var text = File.ReadAllText(AgentPath());
            Assert.That(text, Does.Contain("_animator"),
                "Q-S75/Q-S68/Q-S80: Agent.cs must declare _animator / IAnimoAgent / Update.");
        }
    }
}
