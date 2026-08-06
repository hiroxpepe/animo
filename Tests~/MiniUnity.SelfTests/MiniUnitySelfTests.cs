// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Tests.MiniUnity;

namespace Animo.Tests.MiniUnity.SelfTests {
    /// <summary>
    /// Self-tests for the MiniUnity harness itself. Without these, every higher
    /// test in Phase 2-3 would be running on an unverified harness.
    ///
    /// Roadmap §4.5.2 sub-task 2-2-g requires three:
    /// <list type="number">
    ///   <item>lifecycle order: Awake → Update × N → OnDestroy</item>
    ///   <item>MockBus.published_signals records in order</item>
    ///   <item>MockTime.Step advances all Update calls</item>
    /// </list>
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class MiniUnitySelfTests {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // SetUp / TearDown

        [SetUp]
        public void Setup() {
            MockTime.Reset();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        // ──────────────────────────────────────────────────────────────────────
        // Self-test 1 — lifecycle order: Awake → Update × N → OnDestroy
        // ──────────────────────────────────────────────────────────────────────

        [Test]
        public void Lifecycle_Order_AwakeUpdateNDestroy() {
            // arrange
            MockScene scene = new();
            MockGameObject obj = new(name: "agent");
            scene.Add(obj: obj);
            LifecycleProbe probe = obj.AddComponent<LifecycleProbe>();

            // act
            scene.Tick(delta_time: 0.016f);
            scene.Tick(delta_time: 0.016f);
            scene.Tick(delta_time: 0.016f);
            obj.Destroy();

            // assert
            Assert.That(probe.events, Is.EqualTo(expected: new List<string> {
                "Awake", "Update", "Update", "Update", "OnDestroy"
            }));
        }

        // ──────────────────────────────────────────────────────────────────────
        // Self-test 2 — MockBus.published_signals records in order
        // ──────────────────────────────────────────────────────────────────────

        [Test]
        public void MockBus_RecordsPublishedSignalsInOrder() {
            // arrange
            MockBus bus = new();

            // act
            bus.Publish(signal_id: "animo_goblin_01_flee");
            bus.Publish(signal_id: "animo_goblin_01_fear_critical");
            bus.Publish(signal_id: "animo_goblin_01_search_food");

            // assert
            Assert.That(bus.published_signals, Is.EqualTo(expected: new[] {
                "animo_goblin_01_flee",
                "animo_goblin_01_fear_critical",
                "animo_goblin_01_search_food"
            }));

            // Reset clears history.
            bus.Reset();
            Assert.That(bus.published_signals, Is.Empty);

            // After Reset, recording resumes from zero.
            bus.Publish(signal_id: "animo_goblin_01_idle");
            Assert.That(bus.published_signals, Is.EqualTo(expected: new[] { "animo_goblin_01_idle" }));
        }

        // ──────────────────────────────────────────────────────────────────────
        // Self-test 3 — MockTime.Step advances all Update calls
        // ──────────────────────────────────────────────────────────────────────

        [Test]
        public void MockTime_Step_AdvancesAllUpdateCalls() {
            // arrange — three independent objects, each with a delta-time recorder
            MockScene scene = new();
            MockGameObject obj_a = new(name: "a"); scene.Add(obj: obj_a);
            MockGameObject obj_b = new(name: "b"); scene.Add(obj: obj_b);
            MockGameObject obj_c = new(name: "c"); scene.Add(obj: obj_c);
            DeltaRecorder rec_a = obj_a.AddComponent<DeltaRecorder>();
            DeltaRecorder rec_b = obj_b.AddComponent<DeltaRecorder>();
            DeltaRecorder rec_c = obj_c.AddComponent<DeltaRecorder>();

            // act — three ticks at distinct delta_time
            scene.Tick(delta_time: 0.10f);
            scene.Tick(delta_time: 0.25f);
            scene.Tick(delta_time: 0.50f);

            // assert — every recorder saw the same delta_time sequence …
            float[] expected = new[] { 0.10f, 0.25f, 0.50f };
            Assert.That(rec_a.observed_dts, Is.EqualTo(expected: expected).Within(amount: 1e-6f));
            Assert.That(rec_b.observed_dts, Is.EqualTo(expected: expected).Within(amount: 1e-6f));
            Assert.That(rec_c.observed_dts, Is.EqualTo(expected: expected).Within(amount: 1e-6f));

            // … and MockTime accumulated correctly.
            Assert.That(MockTime.elapsed_seconds, Is.EqualTo(expected: 0.85f).Within(amount: 1e-6f));
        }

        // ──────────────────────────────────────────────────────────────────────
        // Self-test 4 — Tick prunes destroyed objects (added in Phase_2_2_2)
        // ──────────────────────────────────────────────────────────────────────

        [Test]
        public void Tick_PrunesDestroyedObjects() {
            // arrange
            MockScene scene = new();
            MockGameObject obj_a = new(name: "a"); scene.Add(obj: obj_a);
            MockGameObject obj_b = new(name: "b"); scene.Add(obj: obj_b);
            MockGameObject obj_c = new(name: "c"); scene.Add(obj: obj_c);
            obj_a.AddComponent<DeltaRecorder>();
            obj_b.AddComponent<DeltaRecorder>();
            obj_c.AddComponent<DeltaRecorder>();
            Assert.That(scene.objects.Count, Is.EqualTo(expected: 3));

            // act — destroy two objects, then tick
            obj_a.Destroy();
            obj_c.Destroy();
            scene.Tick(delta_time: 0.016f);

            // assert — internal list is pruned to only the live object
            Assert.That(scene.objects.Count, Is.EqualTo(expected: 1));
            Assert.That(scene.objects[0].name, Is.EqualTo(expected: "b"));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Inner test fixtures

        /// <summary>Records every lifecycle hook fired against it.</summary>
        sealed class LifecycleProbe : MockMonoBehaviour {
            public readonly List<string> events = new();
            public override void Awake()     { events.Add(item: "Awake"); }
            public override void Update()    { events.Add(item: "Update"); }
            public override void OnDestroy() { events.Add(item: "OnDestroy"); }
        }

        /// <summary>Records the MockTime.deltaTime observed at each Update.</summary>
        sealed class DeltaRecorder : MockMonoBehaviour {
            public readonly List<float> observed_dts = new();
            public override void Update() { observed_dts.Add(item: MockTime.deltaTime); }
        }
    }
}
