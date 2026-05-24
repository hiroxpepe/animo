// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Step 3: Threshold hysteresis — OnSignal fire verification (spec §12.3).</summary>
    [TestFixture]
    public class Step3_ThresholdTests {

        Engine MakeEngineWithThreshold(float fear_init, float trigger, float reset) {
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("fear", fear_init), ("idle", 30f)),
                actions = new List<Action> { ActionOf("Idle","idle",5) },
                binding = new Binding {
                    thresholds = new List<Threshold>{
                        ThresholdOf("fear", trigger, "fear_alert", reset) }}
            };
            return new Engine(p);
        }

        [Test] public void Case01_BelowTrigger_DoesNotFire() {
            int fired = 0;
            Engine e = MakeEngineWithThreshold(fear_init: 50f, trigger: 80f, reset: 70f);
            e.OnSignal += s => { if (s == "fear_alert") fired++; };
            e.Live(dt: 0.016f);
            Assert.That(fired, Is.EqualTo(0),
                "Step 3: below trigger must NOT fire OnSignal.");
        }

        [Test] public void Case02_AtOrAboveTrigger_FiresOnce() {
            int fired = 0;
            Engine e = MakeEngineWithThreshold(fear_init: 79f, trigger: 80f, reset: 70f);
            e.OnSignal += s => { if (s == "fear_alert") fired++; };
            e.Affect("fear", +2f);  // → 81, crosses 80
            e.Live(dt: 0.016f);
            Assert.That(fired, Is.EqualTo(1),
                "Step 3: crossing trigger must fire OnSignal exactly once.");
        }

        [Test] public void Case03_StaysAboveReset_DoesNotRefire() {
            int fired = 0;
            Engine e = MakeEngineWithThreshold(fear_init: 79f, trigger: 80f, reset: 70f);
            e.OnSignal += s => { if (s == "fear_alert") fired++; };
            e.Affect("fear", +2f);
            e.Live(dt: 0.016f);   // fires once
            e.Live(dt: 0.016f);   // stays above reset → must NOT refire
            Assert.That(fired, Is.EqualTo(1),
                "Step 3: staying above reset threshold must NOT refire.");
        }

        [Test] public void Case04_DropsBelowReset_RearmsAndCanFireAgain() {
            int fired = 0;
            Engine e = MakeEngineWithThreshold(fear_init: 79f, trigger: 80f, reset: 70f);
            e.OnSignal += s => { if (s == "fear_alert") fired++; };
            e.Affect("fear", +2f);
            e.Live(dt: 0.016f);   // fires #1
            e.Affect("fear", -20f); // → ~61, drops below reset 70
            e.Live(dt: 0.016f);   // rearmed
            e.Affect("fear", +25f); // → ~86, crosses 80 again
            e.Live(dt: 0.016f);   // fires #2
            Assert.That(fired, Is.EqualTo(2),
                "Step 3: dropping below reset must rearm and allow second firing.");
        }

        [Test] public void Case05_SpawnAboveTrigger_DoesNotFireOnFirstFrame() {
            // Q-S8/Q-S25: spawn-time seeding sets is_above=true without firing.
            int fired = 0;
            Engine e = MakeEngineWithThreshold(fear_init: 90f, trigger: 80f, reset: 70f);
            e.OnSignal += s => { if (s == "fear_alert") fired++; };
            e.Live(dt: 0.016f);
            Assert.That(fired, Is.EqualTo(0),
                "Step 3 Q-S8: spawning above trigger must NOT fire (already above at construction).");
        }
    }
}
