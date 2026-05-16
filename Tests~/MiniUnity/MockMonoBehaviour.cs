// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

namespace Animo.Tests.MiniUnity {
    /// <summary>
    /// Pure-C# stand-in for <c>UnityEngine.MonoBehaviour</c>. Tests subclass this
    /// to exercise <c>Animo.Agent</c>-style lifecycle without loading Unity.
    ///
    /// The harness drives the lifecycle explicitly (no scene autoload):
    /// <list type="bullet">
    ///   <item><see cref="Awake"/> is called once when the component is added to a <see cref="MockGameObject"/>.</item>
    ///   <item><see cref="Update"/> is called per <see cref="MockScene.Tick(float)"/>.</item>
    ///   <item><see cref="OnDestroy"/> is called when the host <see cref="MockGameObject"/> is destroyed.</item>
    /// </list>
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public abstract class MockMonoBehaviour {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun]

        /// <summary>The owning <see cref="MockGameObject"/>. Set by the harness when added.</summary>
        public MockGameObject? gameObject { get; internal set; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>Called once when the component is attached. Override for setup logic.</summary>
        public virtual void Awake() {}

        /// <summary>Called once per simulated frame. Override for per-frame logic.</summary>
        public virtual void Update() {}

        /// <summary>Called once when the component or its host is destroyed. Override for teardown.</summary>
        public virtual void OnDestroy() {}
    }
}
