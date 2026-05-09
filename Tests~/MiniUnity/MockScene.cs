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
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        readonly List<MockGameObject> _objects = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun]

        /// <summary>Read-only view of every registered object (active and destroyed).</summary>
        public IReadOnlyList<MockGameObject> objects => _objects;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>Register an object so it receives <c>Update</c> calls during <see cref="Tick(float)"/>.</summary>
        /// <param name="obj">The game object to add.</param>
        public void Add(MockGameObject obj) {
            _objects.Add(item: obj);
        }

        /// <summary>
        /// Advance one simulated frame: set <see cref="MockTime.deltaTime"/> to
        /// <paramref name="dt"/>, then call <c>Update</c> on every component of
        /// every active object, in registration order.
        ///
        /// Destroyed objects are pruned from the internal list before iteration
        /// so they cannot accumulate over many ticks. The component list is
        /// snapshotted per object so that an <c>Update</c> implementation may
        /// safely add or remove components without affecting iteration.
        /// </summary>
        /// <param name="dt">Frame delta in seconds.</param>
        public void Tick(float dt) {
            MockTime.Step(dt: dt);

            // Drop already-destroyed objects so a long-running test cannot
            // accumulate dead references across many ticks.
            _objects.RemoveAll(match: o => !o.is_active);

            // Snapshot the object list too: a destructive Update should not
            // affect iteration this frame.
            MockGameObject[] obj_snapshot = _objects.ToArray();
            foreach (MockGameObject obj in obj_snapshot) {
                if (!obj.is_active) continue;
                IReadOnlyList<MockMonoBehaviour> comps = obj.GetAllComponents();
                MockMonoBehaviour[] comp_snapshot = new MockMonoBehaviour[comps.Count];
                for (int i = 0; i < comps.Count; i++) comp_snapshot[i] = comps[i];
                foreach (MockMonoBehaviour c in comp_snapshot) {
                    c.Update();
                }
            }
        }
    }
}
