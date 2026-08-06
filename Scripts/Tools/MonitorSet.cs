// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using Animo.Core;

namespace Animo.Tools {

    /// <summary>
    /// Holds many live agents at once, each in its own MonitorLoop, and keeps a
    /// recording per agent so a run can be looked at again. A tick advances every
    /// agent, records each one's frame, and hands back a snapshot per agent; the
    /// dashboard watches one at a time and steps into it by name. This is the
    /// Stage 3 layer over the single-agent loop.
    /// </summary>
    public sealed class MonitorSet {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        readonly Dictionary<string, MonitorLoop> _loops = new();
        readonly Dictionary<string, Recording> _recordings = new();
        readonly List<string> _order = new();
        readonly float _delta_time;
        string _watched = "";

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public MonitorSet(float delta_time) {
            _delta_time = delta_time;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        /// <summary>Every agent id, in the order the agents were added.</summary>
        public IReadOnlyList<string> Ids => _order;

        /// <summary>The agent the dashboard is watching now.</summary>
        public string Watched => _watched;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Add an agent under an id. The first one added is watched. A repeat id
        /// is refused with a warning, so a double add cannot quietly drop an agent.
        /// </summary>
        public void Add(string id, Engine engine) {
            if (_loops.ContainsKey(id)) {
                AnimoLog.Warning($"MonitorSet.Add: agent id '{id}' is already in the set; ignored.");
                return;
            }
            _loops[id] = new MonitorLoop(engine, _delta_time);
            _recordings[id] = new Recording();
            _order.Add(id);
            if (_watched.Length == 0) _watched = id;
        }

        /// <summary>The loop for one agent. An unknown id is a KeyNotFoundException.</summary>
        public MonitorLoop Loop(string id) {
            if (!_loops.TryGetValue(id, out var loop))
                throw new KeyNotFoundException($"MonitorSet.Loop: no agent with id '{id}'.");
            return loop;
        }

        /// <summary>The recording for one agent. An unknown id throws.</summary>
        public Recording Recording(string id) {
            if (!_recordings.TryGetValue(id, out var rec))
                throw new KeyNotFoundException($"MonitorSet.Recording: no agent with id '{id}'.");
            return rec;
        }

        /// <summary>Watch a different agent. An unknown id is ignored.</summary>
        public void Watch(string id) {
            if (_loops.ContainsKey(id)) _watched = id;
        }

        /// <summary>
        /// Advance every agent one frame, record each one's frame, and return the
        /// snapshot of each keyed by agent id, so the dashboard can draw the
        /// watched one and know the rest are alive.
        /// </summary>
        public Dictionary<string, EngineSnapshot> TickAll() {
            var snapshot_set = new Dictionary<string, EngineSnapshot>();
            foreach (var id in _order) {
                var snapshot = _loops[id].Tick();
                _recordings[id].Add(snapshot);
                snapshot_set[id] = snapshot;
            }
            return snapshot_set;
        }
    }
}
