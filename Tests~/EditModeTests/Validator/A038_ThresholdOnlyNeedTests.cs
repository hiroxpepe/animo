#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ValidatorTests {
    [TestFixture]
    public class A038_ThresholdOnlyNeedTests {
        [Test] public void Case01_NeedUsedOnlyInThreshold_DoesNotTriggerA038Orphan() {
            // Q-S49: binding.thresholds[].need is 4th "in use" site.
            var root = new Animo.Model.Root { schema_version = "1.5",
                personas = new List<Persona>{ new Persona { agent_id = "a",
                    needs = NeedsOf(("fear",30f),("idle",50f)),
                    actions = new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) },
                    binding = new Binding { thresholds = new List<Threshold>{
                        ThresholdOf("fear",80f,"alert") }},
                    needs_meta = new Dictionary<string, NeedMeta>{
                        ["fear"] = new NeedMeta { tier = 2 } }}}};
            var composed = Composer.Compose(root.personas[0], root);
            var r = Validator.ValidateStage2(composed);
            bool fear_orphan = r.issues.Exists(i => i.rule_id=="A038" && i.message.Contains("fear") && i.severity==Severity.Warning);
            Assert.That(fear_orphan, Is.False,
                "Q-S49: Need referenced only in thresholds must NOT be A038 orphan.");
        }
    }
}
