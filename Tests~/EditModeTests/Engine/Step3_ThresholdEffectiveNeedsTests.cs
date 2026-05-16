// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S23 (v0.1.5): Step 3 Threshold check
    /// must compare _previous_effective_needs against _effective_needs,
    /// not against base _needs. Influence cascade (§9.6.5) writes only
    /// to _effective_needs; pre-Q-S23 a frustration→anger chain that
    /// pushed eff_anger over a Threshold's trigger published no Bus
    /// signal — the Action layer (already on _effective_needs) saw the
    /// rise but the Threshold layer did not.
    ///
    /// Direct Bus-publish observation requires a MockBus injection
    /// point on Engine ctor (Phase 3 follow-up). Here we pin the
    /// preconditions the Engine implementation must satisfy:
    ///   - Engine constructs without throwing on a fixture with both
    ///     an Influence and a Threshold targeting the same Need
    ///   - Affect(frustration, +X) followed by Live(dt) does NOT throw
    ///   - GetNeed("anger") reads back the BASE Need (= 0), confirming
    ///     that the Influence does NOT mutate the base — only the
    ///     effective array, which Q-S23 makes Threshold observe.
    /// A Phase 3 test with MockBus injection will assert that
    /// "animo_a_anger_burst" was actually published.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Step3_ThresholdEffectiveNeedsTests {

        [Test] public void Case01_InfluenceCascadeRaisesEffective_TriggerFires() {
            Persona p = new Persona {
                agent_id = "a",
                needs    = NeedsOf(("frustration", 0f), ("anger", 0f), ("idle", 30f)),
                actions  = new List<Animo.Model.Action> {
                    ActionOf(id: "Sulk", need: "anger", tier: 2),
                    ActionOf(id: "Idle", need: "idle",  tier: 5)
                },
                influences = new List<Influence> {
                    InfluenceOf(source: "frustration", target: "anger", coefficient: +1.0f)
                },
                binding = new Binding {
                    on_action_change = "animo_{agent_id}_{behavior}",
                    thresholds = new List<Threshold> {
                        ThresholdOf(need: "anger", trigger: 70.0f, trigger_event: "animo_{agent_id}_anger_burst")
                    }
                }
            };
            Engine engine = new Engine(persona: p);
            Assert.That(engine.GetNeed(need: "frustration"), Is.EqualTo(expected: 0f),
                "precondition: base frustration starts at 0");
            Assert.That(engine.GetNeed(need: "anger"), Is.EqualTo(expected: 0f),
                "precondition: base anger starts at 0");

            // Frame 0: noop frame — must not fire spuriously (Q-S8 seed).
            Assert.DoesNotThrow(code: () => engine.Live(dt: 0.016f));

            // Frame 1: pump frustration so cascade should lift eff_anger > 70.
            engine.Affect(need: "frustration", delta: +80f);
            Assert.DoesNotThrow(code: () => engine.Live(dt: 0.016f),
                "Q-S23: Engine must construct + run with both Influence and Threshold " +
                "targeting the same Need without throwing — base for the Phase 3 MockBus " +
                "assertion that 'animo_a_anger_burst' is actually published.");

            // The base _needs[anger] must remain at 0: Influence writes only
            // to _effective_needs (§9.6.5). Q-S23's contract is that Step 3
            // observes the effective array (where the cascade lands), not
            // the base — making the cascade's emotional rise visible to Bus.
            //
            // (v0.1.5, Q-S120) Use GetBaseNeed (not GetNeed). Pre-Q-S120
            // this called engine.GetNeed("anger") which Q-S54 now defines
            // as the EFFECTIVE value (post-Influence-cascade) — the
            // cascade-driven rise IS visible through GetNeed, so this
            // assertion would fail (expected 0, actual ~80) once Phase 3
            // wires Q-S54's effective semantics. The intent of this
            // assertion is to confirm the BASE remains untouched, which
            // is what GetBaseNeed reads. The Q-S100/Q-S101-style sed of
            // Q-S54 left this companion test out of sync.
            Assert.That(engine.GetBaseNeed(need: "anger"), Is.EqualTo(expected: 0f).Within(0.001f),
                "Q-S23 implementation contract: Influence cascade does NOT mutate the base " +
                "_needs[anger]. The cascade-driven rise lives in _effective_needs only, which " +
                "is exactly why Threshold must read the effective array (Q-S23 fix) to see the " +
                "rise. A Phase 3 MockBus test will assert that the Threshold actually fired.");
        }
    }
}
