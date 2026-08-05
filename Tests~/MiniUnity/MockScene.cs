// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;

namespace Animo.Tests.MiniUnity {
    /// <summary>
    /// Pure-C# stand-in for a Unity Scene. Holds <see cref="MockGameObject"/>
    /// instances and drives their components' <c>Update</c> per frame.
    ///
    /// Order of <c>Update</c> calls follows registration order (the order
    /// objects were added). Within an object, components are updated in
    /// insertion order. Destroyed objects are skipped automatically.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class MockScene {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        readonly List<MockGameObject> _objects = new();

        // (v0.1.5, Q-S87) Reusable scratch buffers for the per-Tick
        // snapshot. Pre-Q-S87 Tick allocated `_objects.ToArray` plus
        // a fresh `new MockMonoBehaviour[comps.Count]` every frame —
        // a 1-hour Soak Test (216,000 frames) burnt ~432,000 array
        // allocations in the test infrastructure alone, defeating the
        // very Zero-GC contract the harness exists to verify. Reusing
        // List<T> scratch buffers (which grow to peak capacity then
        // stop allocating on subsequent reuses) eliminates the
        // allocation tower while preserving Q-S21's zombie-Update
        // protection (snapshot-then-iterate semantics unchanged).
        readonly List<MockGameObject>      _obj_scratch  = new();
        readonly List<MockMonoBehaviour>   _comp_scratch = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun]

        /// <summary>Read-only view of every registered object (active and destroyed).</summary>
        public IReadOnlyList<MockGameObject> objects => _objects;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Register an object so it receives <c>Update</c> calls during
        /// <see cref="Tick(float)"/>.
        ///
        /// (v0.1.5, Q-S137) Phase 3 ITimeProvider DI pattern for Agent tests:
        /// Q-S115 declared that Agent.Update should read from an `ITimeProvider`
        /// rather than `UnityEngine.Time.deltaTime` directly. In EditMode tests,
        /// MockScene.Tick already calls `MockTime.Step(delta_time)` before dispatching
        /// Update; Phase 3 must ensure each Agent receives a MockTime-backed
        /// ITimeProvider. Recommended pattern for test fixtures:
        ///
        ///   var scene = new MockScene();
        ///   var go    = new MockGameObject();
        ///   var agent = go.AddComponent&lt;Agent&gt;();
        ///   agent.SetTimeProvider(new MockTimeProvider());  // Phase 3 API
        ///   scene.Add(go);
        ///   scene.Tick(0.1f);
        ///
        /// `MockTimeProvider` wraps `MockTime.deltaTime` (the static field
        /// MockScene.Tick already advances). A convenience overload
        /// `MockScene.Add(MockGameObject, ITimeProvider)` may be added in
        /// Phase 3 to inject the provider automatically at registration time,
        /// so callers do not need to reach into Agent internals.
        /// </summary>
        /// <param name="obj">The game object to add.</param>
        public void Add(MockGameObject obj) {
            _objects.Add(item: obj);
        }

        /// <summary>
        /// Advance one simulated frame: set <see cref="MockTime.deltaTime"/> to
        /// <paramref name="delta_time"/>, then call <c>Update</c> on every component of
        /// every active object, in registration order.
        ///
        /// Destroyed objects are pruned from the internal list before iteration
        /// so they cannot accumulate over many ticks. The component list is
        /// snapshotted per object so that an <c>Update</c> implementation may
        /// safely add or remove components without affecting iteration.
        /// </summary>
        /// <param name="delta_time">Frame delta in seconds.</param>
        public void Tick(float delta_time) {
            MockTime.Step(delta_time: delta_time);

            // Drop already-destroyed objects so a long-running test cannot
            // accumulate dead references across many ticks.
            _objects.RemoveAll(match: o => !o.is_active);

            // (v0.1.5, Q-S87) Snapshot via reusable scratch buffers.
            // Clear() preserves capacity — after the first Tick the
            // List backing arrays stop growing. Allocations: amortized
            // O(1) per Tick instead of O(n+m) per Tick where n =
            // object count, m = max component count. Q-S21 semantics
            // preserved: we still iterate the snapshot, not the live
            // list, so destructive Update calls don't affect this
            // frame's iteration.
            _obj_scratch.Clear();
            _obj_scratch.AddRange(collection: _objects);
            for (int oi = 0; oi < _obj_scratch.Count; oi++) {
                MockGameObject obj = _obj_scratch[oi];
                if (!obj.is_active) continue;
                IReadOnlyList<MockMonoBehaviour> comps = obj.GetAllComponents();
                _comp_scratch.Clear();
                for (int i = 0; i < comps.Count; i++) _comp_scratch.Add(item: comps[i]);
                for (int ci = 0; ci < _comp_scratch.Count; ci++) {
                    // v0.1.5 (Q-S21): re-check is_active each iteration.
                    // A previous component's Update may have called Destroy()
                    // on this same GameObject, which fires OnDestroy on the
                    // remaining components synchronously. Without this break,
                    // we would call Update on already-destroyed components —
                    // a Unity-lifecycle violation that would crash hot-path
                    // resources (zombie Update). Mirrors Unity's contract:
                    // once GameObject is destroyed, no further Update for
                    // any of its components THIS FRAME.
                    if (!obj.is_active) break;
                    _comp_scratch[ci].Update();
                }
            }
        }
    }
}
