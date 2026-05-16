// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;

namespace Animo {
    /// <summary>
    /// Singleton registry mapping `agent_id` → live Agent instance. The relay
    /// window for Germio Executor's `Affect` calls. See spec §11.
    ///
    /// v0.1.5: Q-S6 contract for duplicate Register — Warning + no-op (keep
    /// first), never throw.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Store {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Singleton

        static Store? _instance;
        public static Store Instance => _instance ??= new Store();

        /// <summary>Reset the singleton state. Test-only seam (no spec contract).</summary>
        public static void ResetForTesting() {
            _instance = null;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Register an Agent under its `agent_id`. Spec §11.2 (v0.1.5):
        /// duplicate `agent_id` → Warning + no-op (original is kept).
        /// </summary>
        public void Register(IAnimoAgent agent) {
            throw new NotImplementedException();
        }

        /// <summary>Unregister. Unknown id → Warning + no-op (spec §11.2).</summary>
        public void Unregister(IAnimoAgent agent) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Relay an Affect to the registered Agent. Unknown id → Warning + no-op.
        /// Misuse contract for need / delta values matches `Engine.Affect`
        /// (spec §11.3.1, v0.1.5).
        /// </summary>
        public void Affect(string agent_id, string need, float delta, bool force_reset = false) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// (v0.1.5, Q-S148) Returns true if an Agent with the given
        /// `agent_id` is currently registered in this Store.
        ///
        /// Behaviour contract (Phase 3 implementation):
        ///   - Returns the presence of the FIRST-registered Agent for
        ///     this `agent_id`. Subsequent duplicate registrations are
        ///     silently no-op'd per §11.2 (Warning + keep first); this
        ///     method cannot distinguish "first registration" from
        ///     "duplicate registration attempted".
        ///   - Unknown `agent_id` (never registered, or previously
        ///     unregistered) returns false — no Warning, no exception.
        ///   - Empty string returns false (no side effect).
        ///   - Thread-safety: Store is not thread-safe; call only from
        ///     Unity's main thread.
        ///
        /// Typical use: test assertions and Executor guard checks.
        ///   `Assert.That(store.IsRegistered("goblin_01"), Is.True);`
        /// </summary>
        public bool IsRegistered(string agent_id) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Minimal Agent surface that Store needs. The real `Animo.Agent` class
    /// implements this; tests can fake it with a tiny shim.
    /// </summary>
    public interface IAnimoAgent {
        string agent_id { get; }
    }
}
