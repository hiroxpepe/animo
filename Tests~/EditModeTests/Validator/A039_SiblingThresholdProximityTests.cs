#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ValidatorTests {
    [TestFixture]
    public class A039_SiblingThresholdProximityTests {

        Persona compose_with_thresholds(float t1, float t2) {
            var root = new Animo.Model.Root {
                schema_version = "1.5",
                kinds = new List<Kind>(),
                personas = new List<Persona> {
                    new Persona { agent_id = "a",
                        needs = NeedsOf(("fear",30f)),
                        actions = new List<Animo.Model.Action>{ ActionOf("X","fear",2) },
                        binding = new Binding { thresholds = new List<Threshold>{
                            ThresholdOf("fear",t1,"e1",t1-6f), ThresholdOf("fear",t2,"e2",t2-6f) }}}
                }
            };
            return Composer.Compose(root.personas[0], root);
        }

        [Test] public void Case01_SiblingTriggersAt78And79_EmitsA039Warning() {
            var composed = compose_with_thresholds(78f, 79f);
            var r = Validator.ValidateStage2(composed);
            Assert.That(r.HasRuleWithSeverity("A039", Severity.Warning), Is.True,
                "Q-S47 + Q-S122: sibling thresholds at 78 and 79 (diff=1.0, inclusive) must emit A039 Warning.");
        }

        [Test] public void Case02_SiblingTriggersWithFloat32Drift_EmitsA039Warning() {
            // Q-S135: SIBLING_THRESHOLD_EPSILON = 0.001f covers float32 drift.
            // 2.4f - 1.4f = 1.0000001f in C# float32 arithmetic; EPSILON catches it.
            var composed = compose_with_thresholds(2.4f, 1.4f);
            var r = Validator.ValidateStage2(composed);
            Assert.That(r.HasRuleWithSeverity("A039", Severity.Warning), Is.True,
                "Q-S135: float32 drift cases (2.4f-1.4f>1.0f) must be caught by SIBLING_THRESHOLD_EPSILON.");
        }
    }
}
