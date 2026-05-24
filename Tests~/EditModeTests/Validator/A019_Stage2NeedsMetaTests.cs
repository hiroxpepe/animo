#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ValidatorTests {
    [TestFixture]
    public class A019_Stage2NeedsMetaTests {
        [Test] public void Case01_PersonaNeedsMetaSuppressesKindOriginatedTypo() {
            // Q-S39: A019 runs in Stage 2 against COMPOSED Persona.
            // A Need declared in needs_meta should NOT emit A019 (it's intentional).
            var root = new Animo.Model.Root { schema_version = "1.5",
                kinds = new List<Kind>{ new Kind { kind_id = "k",
                    actions = new List<Animo.Model.Action>{ ActionOf("X","oxygen",1) }}},
                personas = new List<Persona>{ new Persona { agent_id = "a",
                    kind_ids = new List<string>{"k"},
                    needs = NeedsOf(("oxygen",40f)),
                    needs_meta = new Dictionary<string, NeedMeta>{
                        ["oxygen"] = new NeedMeta { tier = 1 } }}}};
            var composed = Composer.Compose(root.personas[0], root);
            var r = Validator.ValidateStage2(composed);
            bool a019_fired = r.issues.Exists(i => i.rule_id == "A019" && i.message.Contains("oxygen"));
            Assert.That(a019_fired, Is.False,
                "Q-S39: 'oxygen' in needs_meta must suppress A019 (not a typo of standard Need).");
        }
    }
}
