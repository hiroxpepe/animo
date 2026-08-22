// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

using Animo;
using Animo.Model;
using Animo.Tools;

namespace Animo.Tests.Integration {
    ///////////////////////////////////////////////////////////////////////////////////////////////////
    // public Classes

    /// <summary>
    /// End-to-end tests for the two personas the PoC runs on (animo TASK-013).
    ///
    /// place_curious and company_seeking were built as one pair, not one at a
    /// time: Maslow's own holding-back becomes, in play, the bond between them.
    /// place_curious cannot reach Explore while loneliness sits high, so it only
    /// truly goes exploring once company_seeking comes near; and each time it
    /// walks off, company_seeking feels separation climb, and calls out.
    ///
    /// Every value was worked out and checked by real sums in
    /// docs/persona_design_spec.md §6. These tests hold the file to those sums.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class PoCPairEndToEndTests {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods

        static string RepoRoot() {
            string? dir = Directory.GetCurrentDirectory();
            while (dir != null && !File.Exists(Path.Combine(dir, "Scripts", "Const.cs")))
                dir = Directory.GetParent(dir)?.FullName;
            if (dir == null)
                throw new DirectoryNotFoundException("Could not locate repo root from " + Directory.GetCurrentDirectory());
            return dir;
        }

        static Root LoadPair() {
            string path = Path.Combine(RepoRoot(), "examples", "poc_pair.json");
            Assume.That(File.Exists(path), $"poc_pair.json must exist at {path}");
            return JSON.Parse(File.ReadAllText(path));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods

        [Test] public void PoCPair_FileHoldsBothPersonas() {
            var root = LoadPair();

            var runner_a = new ScenarioRunner(root);
            var result_a = runner_a.Run(agent_id: "place_curious_01", duration: 1.0f, delta_time: 0.1f);
            var runner_b = new ScenarioRunner(root);
            var result_b = runner_b.Run(agent_id: "company_seeking_01", duration: 1.0f, delta_time: 0.1f);

            Assert.That(result_a.frames.Count, Is.GreaterThan(0), "place_curious_01 must run.");
            Assert.That(result_b.frames.Count, Is.GreaterThan(0), "company_seeking_01 must run.");
        }

        [Test] public void PlaceCurious_AtTheStart_ReachesForTheOtherOne() {
            var root = LoadPair();
            var runner = new ScenarioRunner(root);

            var result = runner.Run(agent_id: "place_curious_01", duration: 0.1f, delta_time: 0.1f);

            Assert.That(result.frames[0].behavior, Is.EqualTo("Approach"),
                "§6: at the start it reaches for the other one, not for a new place. "
                + "This is what makes the pair a pair.");
        }

        [Test] public void CompanySeeking_AtTheStart_ReachesForTheOtherOne() {
            var root = LoadPair();
            var runner = new ScenarioRunner(root);

            var result = runner.Run(agent_id: "company_seeking_01", duration: 0.1f, delta_time: 0.1f);

            Assert.That(result.frames[0].behavior, Is.EqualTo("Approach"),
                "§6: at the start, Approach scores 54.5 — it wants the other one, with no doubt at all.");
        }

        [Test] public void PlaceCurious_OnceNoLongerAlone_GoesExploring() {
            var root = LoadPair();
            var runner = new ScenarioRunner(root);

            // The other one arrives: loneliness is quieted, as Approach landing
            // would quiet it. Maslow's own holding-back lifts, and what the
            // character truly wants comes through.
            var events = new List<TimedAffectEvent> {
                new TimedAffectEvent(time: 0.5f,
                    event_value: new AffectEvent(need: "loneliness", delta: -30f))
            };
            var result = runner.Run(agent_id: "place_curious_01", duration: 1.0f,
                delta_time: 0.1f, events: events);

            string last = result.frames[result.frames.Count - 1].behavior;
            Assert.That(last, Is.EqualTo("Explore"),
                "This is the whole point of the pair: one cannot explore until the other comes.");
        }

        [Test] public void CompanySeeking_LeftBehind_CallsOut() {
            var root = LoadPair();
            var runner = new ScenarioRunner(root);

            // The other one walks off exploring: separation climbs.
            var events = new List<TimedAffectEvent> {
                new TimedAffectEvent(time: 0.5f,
                    event_value: new AffectEvent(need: "loneliness", delta: -30f)),
                new TimedAffectEvent(time: 1.0f,
                    event_value: new AffectEvent(need: "separation", delta: 40f))
            };
            var result = runner.Run(agent_id: "company_seeking_01", duration: 2.0f,
                delta_time: 0.1f, events: events);

            string last = result.frames[result.frames.Count - 1].behavior;
            Assert.That(last, Is.EqualTo("Call"),
                "Left behind, it stands and calls out.");
        }

        [Test] public void EveryBehaviour_HoldsAName() {
            var root = LoadPair();
            var runner = new ScenarioRunner(root);

            var result = runner.Run(agent_id: "place_curious_01", duration: 5.0f, delta_time: 0.1f);

            foreach (var frame in result.frames) {
                Assert.That(frame.behavior, Is.Not.Empty,
                    "A frame with no behavior at all would leave modio nothing to carry out.");
            }
        }

        [Test] public void PlaceCurious_OverAMinute_ShowsEveryOneOfItsFive() {
            var root = LoadPair();
            var runner = new ScenarioRunner(root);

            // What modio would give back, carrying each deed through: it meets
            // the other one, walks two new places, shows what it found, and
            // heads home.
            var events = new List<TimedAffectEvent> {
                new TimedAffectEvent(  8f, new AffectEvent("loneliness",  -30f)),
                new TimedAffectEvent( 20f, new AffectEvent("curiosity",   -25f)),
                new TimedAffectEvent( 28f, new AffectEvent("recognition", -35f)),
                new TimedAffectEvent( 36f, new AffectEvent("curiosity",   -25f)),
                new TimedAffectEvent( 44f, new AffectEvent("exposure",    -30f)),
                new TimedAffectEvent( 52f, new AffectEvent("loneliness",  -30f))
            };
            var result = runner.Run(agent_id: "place_curious_01", duration: 60f,
                delta_time: 0.1f, events: events);

            Assert.That(result.behavior_count.Count, Is.EqualTo(5),
                "Every one of Maslow's five stages must show itself over a minute. "
                + "A stage that never once wins is a stage written for nothing.");
        }

        [Test] public void CompanySeeking_OverAMinute_ShowsEveryOneOfItsFive() {
            var root = LoadPair();
            var runner = new ScenarioRunner(root);

            var events = new List<TimedAffectEvent> {
                new TimedAffectEvent(  8f, new AffectEvent("loneliness",   -30f)),
                new TimedAffectEvent(  8f, new AffectEvent("separation",   -40f)),
                new TimedAffectEvent( 24f, new AffectEvent("usefulness",   -30f)),
                new TimedAffectEvent( 36f, new AffectEvent("togetherness", -30f)),
                new TimedAffectEvent( 48f, new AffectEvent("loneliness",   -30f)),
                new TimedAffectEvent( 48f, new AffectEvent("separation",   -40f))
            };
            var result = runner.Run(agent_id: "company_seeking_01", duration: 60f,
                delta_time: 0.1f, events: events);

            Assert.That(result.behavior_count.Count, Is.EqualTo(5),
                "One arrival quiets two wants at once (loneliness and separation), "
                + "which is what lets the higher stages ever be heard.");
        }

        [Test] public void NoPersonaHoldsIdle() {
            var root = LoadPair();
            var runner = new ScenarioRunner(root);

            var result = runner.Run(agent_id: "place_curious_01", duration: 5.0f, delta_time: 0.1f);

            foreach (var frame in result.frames) {
                Assert.That(frame.behavior, Is.Not.EqualTo("Idle"),
                    "docs/persona_design_spec.md §3: idle is barred as a Need in any persona. "
                    + "It always wins over the wants that give a character its own shape.");
            }
        }
    }
}
