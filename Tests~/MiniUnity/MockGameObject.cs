// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Animo.Tests.MiniUnity {
    /// <summary>
    /// Pure-C# stand-in for <c>UnityEngine.GameObject</c>. Owns a list of
    /// <see cref="MockMonoBehaviour"/> components and runs their lifecycle.
    ///
    /// Component ordering matches insertion order (the harness's contract for
    /// deterministic <c>Update</c>). Multiple components of the same type are
    /// supported; <see cref="GetComponent{T}"/> returns the first one.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class MockGameObject {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        readonly List<MockMonoBehaviour> _components = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun]

        /// <summary>Identifier for the object. Free-form; tests may set it for clarity.</summary>
        public string name { get; set; } = "";

        /// <summary>True until <see cref="Destroy"/> is called.</summary>
        public bool is_active { get; private set; } = true;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>Create an unnamed game object.</summary>
        public MockGameObject() {}

        /// <summary>Create a named game object.</summary>
        /// <param name="name">Display name.</param>
        public MockGameObject(string name) {
            this.name = name;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Attach a fresh component of type <typeparamref name="T"/>. The component's
        /// <see cref="MockMonoBehaviour.gameObject"/> is wired up, then
        /// <see cref="MockMonoBehaviour.Awake"/> is called immediately.
        /// </summary>
        /// <typeparam name="T">A concrete subclass of <see cref="MockMonoBehaviour"/> with a parameterless constructor.</typeparam>
        /// <returns>The newly created and registered component.</returns>
        public T AddComponent<T>() where T : MockMonoBehaviour, new() {
            if (!is_active) {
                throw new InvalidOperationException(message: "cannot AddComponent on a destroyed MockGameObject");
            }
            T component = new();
            component.gameObject = this;
            _components.Add(item: component);
            component.Awake();
            return component;
        }

        /// <summary>
        /// Return the first attached component assignable to <typeparamref name="T"/>,
        /// or <c>null</c> if none exists.
        /// </summary>
        /// <typeparam name="T">A subclass of <see cref="MockMonoBehaviour"/>.</typeparam>
        public T? GetComponent<T>() where T : MockMonoBehaviour {
            return _components.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Return every attached component assignable to <typeparamref name="T"/>,
        /// in insertion order.
        /// </summary>
        /// <typeparam name="T">A subclass of <see cref="MockMonoBehaviour"/>.</typeparam>
        public IReadOnlyList<T> GetComponents<T>() where T : MockMonoBehaviour {
            return _components.OfType<T>().ToList();
        }

        /// <summary>
        /// Iterate every attached component in insertion order. Used by
        /// <see cref="MockScene.Tick(float)"/>.
        /// </summary>
        public IReadOnlyList<MockMonoBehaviour> GetAllComponents() {
            return _components;
        }

        /// <summary>
        /// Mark the object destroyed. Calls <see cref="MockMonoBehaviour.OnDestroy"/>
        /// on every attached component in insertion order, then clears the list.
        /// Idempotent: a second call is a no-op.
        /// </summary>
        public void Destroy() {
            if (!is_active) return;
            is_active = false;
            // copy to allow OnDestroy implementations to call Destroy on themselves
            // without mutating the list mid-iteration.
            MockMonoBehaviour[] snapshot = _components.ToArray();
            foreach (MockMonoBehaviour c in snapshot) {
                c.OnDestroy();
            }
            _components.Clear();
        }
    }
}
