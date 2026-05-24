#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ValidatorTests {
    [TestFixture]
    public class A038_Stage2OrphanTests {
        [Test] public void Case01_KindDeclaresBroadNeedsMeta_ChildUsesSubset_NoA038Warning() {
            // Q-S41: Stage 2 A038 checks composed "in use" union, not raw JSON.
            var root = new Animo.Model.Root { schema_version = "1.5",
                kinds = new List<Kind>{ new Kind { kind_id = "k",
                    actions = new List<Animo.Model.Action>{ ActionOf("X","oxygen",1) },
                    needs_meta = new Dictionary<string, NeedMeta>{
                        ["oxygen"] = new NeedMeta { tier = 1 },
                        ["thirst"] = new NeedMeta { tier = 1 } }}},
                personas = new List<Persona>{ new Persona { agent_id = "a",
                    kind_ids = new List<string>{"k"},
                    needs = NeedsOf(("oxygen",40f)) }}};
            var composed = Composer.Compose(root.personas[0], root);
            // oxygen is used in actions, thirst is orphan (not in needs/actions/influences/thresholds/rates)
            var r = Validator.ValidateStage2(composed);
            bool oxygen_orphan = r.issues.Exists(i => i.rule_id == "A038" && i.message.Contains("oxygen"));
            Assert.That(oxygen_orphan, Is.False,
                "Q-S41: needs_meta entries whose Need IS in use must NOT emit A038 orphan.");
        }
        [Test] public void Case02_GenuinelyOrphanedNeedsMeta_StillEmitsA038Warning() {
            var root = new Animo.Model.Root { schema_version = "1.5",
                personas = new List<Persona>{ new Persona { agent_id = "a",
                    needs = NeedsOf(("idle",30f)),
                    actions = new List<Animo.Model.Action>{ ActionOf("Idle","idle",5) },
                    needs_meta = new Dictionary<string, NeedMeta>{
                        ["phantom"] = new NeedMeta { tier = 1 } }}}};
            var composed = Composer.Compose(root.personas[0], root);
            var r = Validator.ValidateStage2(composed);
            Assert.That(r.HasRuleWithSeverity("A038", Severity.Warning), Is.True,
                "Q-S41: genuinely orphaned needs_meta entry must emit A038 Warning.");
        }
    }
}
