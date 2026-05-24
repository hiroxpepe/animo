// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Tools;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>ScenarioRunner: multi-event, negative-time, same-frame ordering (Q-S35/Q-S55).</summary>
    [TestFixture]
    public class ScenarioRunnerMultiEventTests {

        Animo.Model.Root MakeRoot() => new Animo.Model.Root { schema_version="1.5",
            personas = new List<Persona>{ new Persona { agent_id="a",
                needs=NeedsOf(("fear",10f),("idle",30f)),
                actions=new List<Animo.Model.Action>{
                    ActionOf("Flee","fear",2), ActionOf("Idle","idle",5) }}}};

        [Test] public void Case01_TwoEventsAtDifferentTimes_BothApplied() {
            // Q-S35: events at different frames must both be consumed
            var events = new List<TimedAffectEvent>{
                new TimedAffectEvent(0.2f, new AffectEvent("fear", +30f)),
                new TimedAffectEvent(0.5f, new AffectEvent("fear", +30f))
            };
            var runner = new ScenarioRunner(MakeRoot());
            var result = runner.Run("a", duration: 1.0f, dt: 0.1f, events: events);
            // After t=0.5 both events applied: fear should be ~70
            float fear_late = result.frames.Find(f => f.time >= 0.6f)
                              .effective_needs.GetValueOrDefault("fear", 0f);
            Assert.That(fear_late, Is.GreaterThan(60f),
                "Q-S35: two events at different times must both be consumed.");
        }

        [Test] public void Case02_TwoEventsAtSameTime_BothAppliedInOrder() {
            // Q-S35: same-time events must both be consumed (forward pointer preserves order)
            var events = new List<TimedAffectEvent>{
                new TimedAffectEvent(0.3f, new AffectEvent("fear", +20f)),
                new TimedAffectEvent(0.3f, new AffectEvent("fear", +20f))
            };
            var runner = new ScenarioRunner(MakeRoot());
            var result = runner.Run("a", duration: 1.0f, dt: 0.1f, events: events);
            float fear_after = result.frames.Find(f => f.time >= 0.4f)
                               .effective_needs.GetValueOrDefault("fear", 0f);
            Assert.That(fear_after, Is.GreaterThan(40f),
                "Q-S35: two events at same time must both be consumed (total +40 to fear).");
        }

        [Test] public void Case03_NegativeTimeEvent_ConsumedAtT0Sweep() {
            // Q-S55: events[next].time <= 0.0f must be consumed in pre-loop t=0 sweep
            var events = new List<TimedAffectEvent>{
                new TimedAffectEvent(-0.001f, new AffectEvent("fear", +60f))
            };
            var runner = new ScenarioRunner(MakeRoot());
            var result = runner.Run("a", duration: 1.0f, dt: 0.1f, events: events);
            float fear_at_spawn = result.frames[0].effective_needs.GetValueOrDefault("fear", 0f);
            Assert.That(fear_at_spawn, Is.GreaterThan(50f),
                "Q-S55: negative-time event must be consumed in t=0 sweep (visible at frames[0]).");
        }

        [Test] public void Case04_TenEvents_AllConsumed_InOrder() {
            // Stress test for next-pointer: 10 events at different times
            var events = new List<TimedAffectEvent>();
            for (int i = 1; i <= 10; i++)
                events.Add(new TimedAffectEvent(i * 0.1f, new AffectEvent("fear", +5f)));
            var runner = new ScenarioRunner(MakeRoot());
            var result = runner.Run("a", duration: 1.1f, dt: 0.1f, events: events);
            // All 10 × +5 = +50 applied to fear=10 → ~60 at end
            float fear_end = result.frames[result.frames.Count-1]
                             .effective_needs.GetValueOrDefault("fear", 0f);
            Assert.That(fear_end, Is.GreaterThan(55f),
                "Q-S35: 10 events must all be consumed by next-pointer traversal.");
        }
    }
}
