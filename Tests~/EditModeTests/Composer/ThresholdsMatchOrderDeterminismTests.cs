#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;
namespace Animo.Tests.EditMode.ComposerTests {
    [TestFixture]
    public class ThresholdsMatchOrderDeterminismTests {
        [Test] public void Case01_NonTransitiveEpsilon_FirstOccurrenceWins() {
            // Use clearly-distinct thresholds to test first-occurrence-wins:
            // A=70f, B=80f, C=80.005f
            // A≉B (diff=10 >> EPSILON=0.01)
            // B≈C (diff=0.005 < EPSILON)
            // A≉C (diff≈10 >> EPSILON)
            //
            // Kind has [A=70, B=80]. Persona has [C=80.005].
            // During Kind merge: A vs (empty) → add A; B vs A → diff=10 > 0.01 → add B.
            // Kind result: [A=70, B=80].
            // During Persona merge: C vs A → diff≈10 > 0.01 → no; C vs B → diff=0.005 < 0.01 → B replaced by C.
            // Final: [A=70, C=80.005] = 2 thresholds.
            var root = new Root { schema_version = "1.5",
                kinds = new List<Kind> { new Kind { kind_id = "k",
                    actions = new List<Animo.Model.Action> { ActionOf("X","fear",2) },
                    binding = new Binding { thresholds = new List<Threshold> {
                        ThresholdOf("fear", 70.0f, "a_event", 65f),
                        ThresholdOf("fear", 80.0f, "b_event", 75f) }}}},
                personas = new List<Persona> { new Persona { agent_id = "p",
                    kind_ids = new List<string> { "k" },
                    binding = new Binding { thresholds = new List<Threshold> {
                        ThresholdOf("fear", 80.005f, "c_event", 75f) }}}}
            };
            var composed = Composer.Compose(root.personas[0], root);
            Assert.That(composed.binding!.thresholds.Count, Is.EqualTo(2),
                "Q-S85: B(80) and C(80.005) collapse (B≈C by EPSILON=0.01), A(70) remains. Total=2.");
            bool has_a = composed.binding.thresholds.Exists(t => t.trigger == "a_event");
            bool has_c = composed.binding.thresholds.Exists(t => t.trigger == "c_event");
            bool has_b = composed.binding.thresholds.Exists(t => t.trigger == "b_event");
            Assert.That(has_a, Is.True,  "A (clearly distinct from B and C) must remain.");
            Assert.That(has_c, Is.True,  "C (persona) must replace B (B≈C by EPSILON).");
            Assert.That(has_b, Is.False, "B must be replaced by C.");
        }
    }
}
