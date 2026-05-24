#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ValidatorTests {
    [TestFixture]
    public class A038_RatesOnlyNeedTests {
        [Test] public void Case01_NeedReferencedOnlyInRates_DoesNotTriggerA038Orphan() {
            // Q-S57: rates.keys() is 5th "in use" site for A038 orphan check.
            var root = new Animo.Model.Root { schema_version = "1.5",
                personas = new List<Persona> {
                    new Persona { agent_id = "a",
                        needs = NeedsOf(("poison",50f),("idle",30f)),
                        actions = new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) },
                        rates = RatesOf(("poison",-0.5f)),
                        needs_meta = new Dictionary<string, NeedMeta>{
                            ["poison"] = new NeedMeta { tier = 1 } }}}};
            var composed = Composer.Compose(root.personas[0], root);
            var r = Validator.ValidateStage2(composed);
            Assert.That(r.issues.Exists(i => i.rule_id == "A038" && i.severity == Severity.Warning), Is.False,
                "Q-S57: Need referenced in rates must NOT emit A038 orphan Warning.");
        }
    }
}
