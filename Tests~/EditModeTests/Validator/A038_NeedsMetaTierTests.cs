#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ValidatorTests {
    [TestFixture]
    public class A038_NeedsMetaTierTests {
        [Test] public void Case01_TierOutOfRange_Errors() {
            var root = new Animo.Model.Root { schema_version = "1.5",
                personas = new List<Persona> {
                    new Persona { agent_id = "a",
                        actions = new List<Animo.Model.Action>{ ActionOf("X","fear",2) },
                        needs_meta = new Dictionary<string, NeedMeta>{
                            ["oxygen"] = new NeedMeta { tier = 99 } }}}};
            var r = Validator.Validate(root);
            Assert.That(r.HasRuleWithSeverity("A038", Severity.Error), Is.True,
                "A038 Stage 1: tier=99 out of [1,5] must emit Error.");
        }
        [Test] public void Case02_ValidTier_NoA038() {
            var root = new Animo.Model.Root { schema_version = "1.5",
                personas = new List<Persona> {
                    new Persona { agent_id = "a",
                        needs = NeedsOf(("oxygen",40f)),
                        actions = new List<Animo.Model.Action>{ ActionOf("X","oxygen",1) },
                        needs_meta = new Dictionary<string, NeedMeta>{
                            ["oxygen"] = new NeedMeta { tier = 1 } }}}};
            var r = Validator.Validate(root);
            Assert.That(r.HasRuleWithSeverity("A038", Severity.Error), Is.False,
                "A038: valid tier 1 must not emit Error.");
        }
    }
}
