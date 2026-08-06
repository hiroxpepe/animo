// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;

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
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        readonly Dictionary<string, IAnimoAgent> _agents = new();
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Properties [noun, adjective]

        public static Store Instance => _instance ??= new Store();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

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
            if (_agents.ContainsKey(agent.agent_id)) {
                AnimoLog.Warning($"Store.Register: agent_id '{agent.agent_id}' already registered (keep first, no-op).");
                return;
            }
            _agents[agent.agent_id] = agent;
        }

        /// <summary>Unregister. Unknown id → Warning + no-op (spec §11.2).</summary>
        public void Unregister(IAnimoAgent agent) {
            if (!_agents.TryGetValue(agent.agent_id, out var registered)) {
                AnimoLog.Warning($"Store.Unregister: agent_id '{agent.agent_id}' is not registered (no-op).");
                return;
            }
            // Instance-equality check (Q-S22).
            if (!ReferenceEquals(registered, agent)) {
                AnimoLog.Warning($"Store.Unregister: agent_id '{agent.agent_id}' is registered to a different instance (no-op).");
                return;
            }
            _agents.Remove(agent.agent_id);
        }

        /// <summary>
        /// Relay an Affect to the registered Agent. Unknown id → Warning + no-op.
        /// </summary>
        public void Affect(string agent_id, string need, float delta, bool force_reset = false) {
            if (!_agents.TryGetValue(agent_id, out var agent)) {
                AnimoLog.Warning($"Store.Affect: agent_id '{agent_id}' is not registered (no-op).");
                return;
            }
            agent.Affect(need, delta, force_reset);
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
            if (string.IsNullOrEmpty(agent_id)) return false;
            return _agents.ContainsKey(agent_id);
        }
    }

    /// <summary>
    /// Minimal Agent surface that Store needs. The real `Animo.Agent` class
    /// implements this; tests can fake it with a tiny shim.
    /// </summary>
    public interface IAnimoAgent {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective]

        string agent_id { get; }
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// (v0.1.5, Q-S4) Relay Affect from Store to the Agent's Engine.
        /// Called by Store.Affect to route Germio Executor events.
        /// </summary>
        void Affect(string need, float delta, bool force_reset = false);
    }
}
