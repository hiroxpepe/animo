// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;
using Animo.Tests.MiniUnity;

namespace Animo.Tests.MiniUnity.SelfTests {
    /// <summary>
    /// Self-test for Q-S21: MockScene.Tick must NOT call Update on a
    /// component whose host GameObject was destroyed earlier in the
    /// same frame's component loop. Pre-Q-S21, the inner per-component
    /// loop had no `obj.is_active` guard, so a Destroy triggered from
    /// CompA.Update would synchronously OnDestroy CompB, then the loop
    /// would still call CompB.Update — a Unity-lifecycle violation.
    ///
    /// This test goes into MiniUnity.SelfTests (Green) rather than
    /// EditModeTests (Red baseline) because it asserts on the test
    /// harness itself. With Phase_2_4_9's MockScene fix in place, this
    /// test passes immediately.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class MockSceneZombieUpdateTest {

        // Component that destroys its own GameObject during Update.
        class DestroyerComp : MockMonoBehaviour {
            public int update_count = 0;
            public override void Update() {
                update_count++;
                gameObject!.Destroy();
            }
        }

        // Component that simply counts how many times Update was called.
        // After Q-S21 fix, this should be 0 if a sibling DestroyerComp
        // ran first in the same frame.
        class CounterComp : MockMonoBehaviour {
            public int update_count = 0;
            public bool destroyed = false;
            public override void Update() {
                if (destroyed) {
                    Assert.Fail(message: "Q-S21 violation: Update called after OnDestroy. " +
                        "MockScene.Tick must break out of the per-component inner loop " +
                        "when its host GameObject becomes inactive mid-frame.");
                }
                update_count++;
            }
            public override void OnDestroy() {
                destroyed = true;
            }
        }

        [Test] public void Case01_DestroyDuringUpdate_LaterComponentsDoNotReceiveUpdate() {
            // Setup: one GameObject with two components — DestroyerComp
            // first (it will destroy the GameObject from inside its
            // Update), CounterComp second (it must NOT be Updated this
            // frame because its OnDestroy already ran).
            MockScene scene = new MockScene();
            MockGameObject obj = new MockGameObject(name: "subject");
            DestroyerComp destroyer = obj.AddComponent<DestroyerComp>();
            CounterComp counter = obj.AddComponent<CounterComp>();
            scene.Add(obj: obj);

            // Single Tick: DestroyerComp.Update runs → destroys obj →
            // CounterComp.OnDestroy runs (sets destroyed=true). Q-S21
            // requires the inner loop to break here, so CounterComp.Update
            // must NOT be called.
            scene.Tick(dt: 0.016f);

            Assert.That(destroyer.update_count, Is.EqualTo(expected: 1),
                "DestroyerComp must have received exactly one Update");
            Assert.That(counter.destroyed, Is.True,
                "CounterComp.OnDestroy must have fired (synchronous in MockGameObject.Destroy)");
            Assert.That(counter.update_count, Is.EqualTo(expected: 0),
                "Q-S21: CounterComp.Update must NOT have been called after its OnDestroy. " +
                "MockScene.Tick's inner component loop must check obj.is_active each iteration.");
        }
    }
}
