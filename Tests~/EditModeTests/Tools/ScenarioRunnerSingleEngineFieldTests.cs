#nullable enable
using System.Reflection;
using NUnit.Framework;
using Animo.Tools;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class ScenarioRunnerSingleEngineFieldTests {
        [Test] public void Case01_RunnerInternal_IsSingleEngineNotDictionary() {
            var fields = typeof(ScenarioRunner).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            bool has_engine_field = false;
            bool has_dict_field   = false;
            foreach (var f in fields) {
                if (f.FieldType == typeof(Animo.Core.Engine) ||
                    f.FieldType.Name.Contains("Engine")) has_engine_field = true;
                if (f.FieldType.Name.Contains("Dictionary") &&
                    f.FieldType.Name.Contains("Engine")) has_dict_field = true;
            }
            Assert.That(has_engine_field, Is.True, "Q-S92: ScenarioRunner must have an Engine? field.");
            Assert.That(has_dict_field,   Is.False, "Q-S60: must NOT be Dictionary<string,Engine>.");
        }
    }
}
