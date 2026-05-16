// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;

namespace Animo.Tools {
    /// <summary>
    /// (v0.1.5, Q-S82) Snapshot of a single simulated frame produced by
    /// `ScenarioRunner.Run`. The runner records one TraceFrame at the
    /// spawn boundary (t=0 after Q-S55 sweeps any t=0 events) plus one
    /// after every `Live(dt)` tick, plus optionally one final frame at
    /// `time == duration` for boundary events (Q-S40).
    ///
    /// Pre-Q-S82 §26.3 declared this type but no `Scripts/Tools/
    /// TraceResult.cs` file existed in the repository — the
    /// `Animo.Tools` namespace had no source to compile.
    /// Q-S82 ships the file with field declarations matching §26.3.
    /// Phase 3 wires up the actual recording.
    ///
    /// (v0.1.5, Q-S132) Phase 3 OOM risk and mitigation contract:
    /// The current field declarations allocate three Dictionary and one List
    /// per TraceFrame. A 1-hour Soak Test at 60 fps = 216,000 frames
    /// × 4 heap objects = ~864,000 allocations in the test harness alone.
    /// Phase 3 implementation MUST use a lightweight alternative:
    ///   Option A (recommended): replace Dictionary fields with parallel
    ///     float[] arrays and a shared string[] key array stored once in
    ///     TraceResult (the key order is fixed per Run). Each TraceFrame
    ///     then holds only float[] need_values, float[] effective_values,
    ///     float[] score_values — no per-frame Dictionary allocation.
    ///   Option B: object pool of TraceFrame instances cleared and reused
    ///     between runs (requires clearing Dictionaries, not replacing).
    /// Note: ScenarioRunner is NOT the Zero-GC hot path (Engine.Live is),
    /// but soak tests (Phase 5) run the runner for 3600 seconds at 60fps;
    /// unchecked allocation here will OOM the test runner before Phase 5
    /// even starts. Resolve in Phase 3 before Phase 5 soak is attempted.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public sealed class TraceFrame {
        public float time;
        public string behavior = "";
        public Dictionary<string, float> needs = new();
        public Dictionary<string, float> effective_needs = new();
        public Dictionary<string, float> action_scores = new();
        public bool is_locked;
        public string locked_behavior = "";
        public List<string> signals_fired = new();
    }

    /// <summary>
    /// Aggregate result of a `ScenarioRunner.Run` invocation. Holds
    /// the chronological list of `TraceFrame`s plus run-level
    /// metadata and analysis APIs.
    ///
    /// (v0.1.5, Q-S93) Pre-Q-S93 this class declared only `agent_id`,
    /// `duration`, `dt`, `frames` — but spec §26.3 promised
    /// `behavior_count`, `behavior_total_time`, `ToCsv()`, `ToJson()`
    /// as the analysis surface for ScenarioRunner consumers. Without
    /// these, sim results could not be exported to CSV / JSON for
    /// regression baselines, and behavior occupancy queries (e.g.
    /// "did NPC spend at least 5 seconds fleeing?") had no API.
    /// Q-S93 ships the spec-promised members as Phase 3 stubs.
    /// </summary>
    public sealed class TraceResult {
        public string agent_id = "";
        public float duration;
        public float dt;
        public List<TraceFrame> frames = new();

        /// <summary>
        /// (v0.1.5, Q-S93) Per-behavior occurrence count over the
        /// recorded frames. Key = action_id (e.g. "Flee"), value =
        /// number of frames where TraceFrame.behavior matched that id.
        /// Phase 3 populates by iterating `frames` once at the end of
        /// `ScenarioRunner.Run` (single pass; no re-computation per
        /// query). v0.1.5 stub: empty Dictionary; Phase 3 implements.
        /// </summary>
        public Dictionary<string, int> behavior_count { get; } = new();

        /// <summary>
        /// (v0.1.5, Q-S93) Per-behavior cumulative occupancy time over
        /// the recorded frames. Key = action_id, value = sum of `dt`
        /// for frames where the behavior was active. Phase 3 populates
        /// alongside `behavior_count` in the same single pass.
        /// </summary>
        public Dictionary<string, float> behavior_total_time { get; } = new();

        /// <summary>
        /// (v0.1.5, Q-S93) Serialize the trace to CSV for spreadsheet
        /// analysis. Columns: time, behavior, is_locked, locked_behavior,
        /// {needs}, {effective_needs}, {action_scores}, signals_fired.
        /// Phase 3 implementation handles header generation from the
        /// first non-empty TraceFrame's keys (ensuring stable column
        /// order across rows).
        /// </summary>
        public string ToCsv() => throw new System.NotImplementedException();

        /// <summary>
        /// (v0.1.5, Q-S93) Serialize the trace to JSON for downstream
        /// tools (regression diff, plotting). Wraps frames + metadata
        /// + behavior_count + behavior_total_time in a root object
        /// matching schemas/trace.schema.json (Phase 3 to define).
        /// </summary>
        public string ToJson() => throw new System.NotImplementedException();
    }
}
