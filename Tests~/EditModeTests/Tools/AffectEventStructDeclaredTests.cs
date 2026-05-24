#nullable enable
using NUnit.Framework;
using Animo.Tools;
namespace Animo.Tests.EditMode.ToolsTests {
    [TestFixture]
    public class AffectEventStructDeclaredTests {
        [Test] public void Case01_AffectEvent_DeclaredInAnimoToolsNamespace() {
            var ae = new AffectEvent("fear", 10f, false);
            Assert.That(ae.need,        Is.EqualTo("fear"));
            Assert.That(ae.delta,       Is.EqualTo(10f));
            Assert.That(ae.force_reset, Is.False);
            Assert.That(typeof(AffectEvent).IsValueType, Is.True, "Q-S67: AffectEvent must be a struct.");
        }
    }
}
