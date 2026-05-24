// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Linq;
using Animo.Core;
using Animo.Model;

namespace Animo.Tools {
    /// <summary>
    /// (v0.1.5, Q-S67) Affect payload for ScenarioRunner injection.
    /// Mirrors the argument tuple of `Engine.Affect(need, delta,
    /// force_reset)`.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public readonly struct AffectEvent {
        public string need        { get; }
        public float  delta       { get; }
        public bool   force_reset { get; }
        public AffectEvent(string need, float delta, bool force_reset = false) {
            this.need        = need;
            this.delta       = delta;
            this.force_reset = force_reset;
        }
    }

    /// <summary>(v0.1.5, Q-S4) Timed Affect injection for ScenarioRunner.</summary>
    public readonly struct TimedAffectEvent {
        public float       time { get; }
        public AffectEvent ev   { get; }
        public TimedAffectEvent(float time, AffectEvent ev) {
            this.time = time;
            this.ev   = ev;
        }
    }

    /// <summary>
    /// (v0.1.5, Q-S82) Headless simulator for `Animo.Core.Engine`. Drives
    /// `Live(dt)` over a fixed duration, optionally injecting timed
    /// `Affect` events, and records every frame's state into a
    /// `TraceResult`. Runs without Unity — pure C# tests / .NET CLI.
    ///
    /// Pre-Q-S82 §26.3 contained the API as spec text but no
    /// `Scripts/Tools/ScenarioRunner.cs` existed in the repository;
    /// the `Animo.Tools` namespace was unbuildable. Q-S82 ships the
    /// file with method declarations matching §26.3.
    ///
    /// (v0.1.5, Q-S84 + Q-S98 + Q-S117) The internal `Run` loop uses
    /// an Integer step counter
    /// (`int total_steps = (int)System.Math.Round((double)duration / (double)dt);`)
    /// to guarantee the Q-S35 contract of "exactly floor(duration / dt)
    /// Live calls" without IEEE-754 float-accumulation drift. Q-S84
    /// originally wrote `Math.Floor(duration / dt)` but float32
    /// `(10.0f / 0.1f) = 99.9999985...` floors to 99 (under-shoot by
    /// one step) — Q-S98 promotes to double and uses Round to handle
    /// the sub-LSB drift symmetrically. Q-S117 adds the `dt &lt;= 0`
    /// guard at Run entry — pre-Q-S117 a `dt = 0.0f` call produced
    /// `duration / 0 = +Infinity`, then `(int)Infinity = int.MinValue`
    /// per CLI ECMA-335 unchecked-conv, then the main loop never
    /// entered (predicate `0 &lt; -2147483648 = false`), and Run
    /// returned an empty TraceResult with no diagnostic — worst silent
    /// failure. Q-S136: `System.Math.Round` (fully qualified) — per
    /// Q-S127 pattern: no `using System;` in file header, so all
    /// BCL Math references use the `System.` prefix to prevent CS0103
    /// when Phase 3 transcribes the pseudocode. Phase 3 implementation
    /// respects all four contracts.
    /// </summary>
    public sealed class ScenarioRunner {
        readonly Root _root;

        // (v0.1.5, Q-S60 + Q-S92) Single Engine instance per Run() call.
        // Q-S60 decided "ScenarioRunner's internal field is `Engine
        // _engine` (not `Dictionary<string, Engine>`)" because the
        // current Run(string agent_id, ...) signature accepts one
        // template id and TimedAffectEvent carries no target-agent
        // field — a routing dictionary would always have exactly one
        // entry, dead structure. Q-S60 was a spec-narrative decision;
        // Q-S92 materializes the field declaration that should have
        // been added in Q-S82's file creation but was overlooked.
        // Phase 3 implementer will assign this in Run() before the
        // main loop begins. Nullable because Run() may be called
        // multiple times (each call re-assigns); between calls the
        // field is null. When v0.2 adds multi-agent Run(), the type
        // changes when the API does (Q-S60 deferred clause), not
        // before. CS0169 suppressed because Phase 3 will read/write.
        #pragma warning disable CS0169
        Engine? _engine;
        #pragma warning restore CS0169

        // (v0.1.5, Q-S42 + Q-S99) Run-counter for default agent_id_override
        // generation. Q-S42 declared the spec contract:
        //   "When `agent_id_override` is null, the runner generates
        //    `${template_id}_run_${_seq++}`."
        // so two `Run()` calls from the same template carry distinct
        // ids in trace output. Q-S99 materializes the field declaration
        // — Q-S82's file creation overlooked it (same pattern as Q-S92's
        // `_engine` omission). Phase 3 increments this in Run() when
        // building the override; the post-increment ensures the FIRST
        // Run gets `_run_0` (matching standard Unity test naming).
        // Instance field rather than static so different ScenarioRunner
        // instances (different test fixtures, parallel test runs)
        // don't share counters and step on each other's expected ids.
        // CS0414 suppressed for v0.1.5 stub (initialized but Phase 3
        // adds the read+post-increment in Run()).
        #pragma warning disable CS0414
        int _seq = 0;
        #pragma warning restore CS0414

        public ScenarioRunner(Root root) {
            _root = root;
            // Q-S29: Initialize PersonaCache once at construction.
            // Validator stage 1 runs on the Root; stage 2 runs per
            // template via PersonaCache.GetComposed.
            Animo.PersonaCache.Initialize(root: root);
        }

        /// <summary>
        /// Drive Engine.Live for `duration` seconds with frame size `dt`.
        /// Returns a `TraceResult` with one `TraceFrame` per recorded
        /// boundary (spawn + after each Live + optional final
        /// boundary-event frame per Q-S40).
        ///
        /// (v0.1.5, Q-S145) `agent_id_override` empty-string contract:
        /// passing `""` (empty string) is a fail-loud error. Validator
        /// A002 (snake_case) runs only at JSON parse time; code calling
        /// this API directly bypasses that check. An empty agent_id
        /// would corrupt Bus payload routing (expanded_action_change
        /// would contain `animo__flee` instead of `animo_goblin_01_flee`)
        /// and make TraceResult.agent_id an empty string — indistinguishable
        /// from an uninitialized result. Phase 3 Run() entry:
        ///   if (agent_id_override is string s &amp;&amp; s.Length == 0)
        ///       throw new ArgumentException(
        ///           "agent_id_override must be null or non-empty snake_case string.",
        ///           nameof(agent_id_override));
        /// null is valid (triggers auto-generation: $"{agent_id}_run_{_seq++}").
        /// </summary>
        public TraceResult Run(
            string                            agent_id,
            float                             duration,
            float                             dt = 0.1f,
            IReadOnlyList<TimedAffectEvent>?  events = null,
            string?                           agent_id_override = null
        ) {
            // (Q-S117) dt <= 0 fail-loud
            if (dt <= 0f)
                throw new System.ArgumentException(
                    $"dt must be > 0. Got {dt}.", nameof(dt));

            // (Q-S145) empty agent_id_override is fail-loud
            if (agent_id_override is string ov && ov.Length == 0)
                throw new System.ArgumentException(
                    "agent_id_override must be null or non-empty string.", nameof(agent_id_override));

            // Build composed Persona (deep copy for isolation)
            Persona? raw = null;
            foreach (var p in _root.personas)
                if (p.agent_id == agent_id) { raw = p; break; }
            if (raw == null)
                throw new System.ArgumentException(
                    $"agent_id '{agent_id}' not found in Root.", nameof(agent_id));

            // (#2) Route through PersonaCache.GetComposed so the same Stage 2 validation
            // gate that protects Unity Agents (PersonaTemplateRejectedException on A036
            // composed-actions-empty, etc.) also protects ScenarioRunner. Prior direct
            // Composer.Compose call let invalid templates crash at Live() inside Step 5.
            // GetComposed returns a shared composed template; DeepCopy so per-run mutation
            // (agent_id override) doesn't corrupt the cache for concurrent runs.
            var composed = Animo.PersonaCache.GetComposed(agent_id).DeepCopy();
            string effective_id = agent_id_override ?? $"{agent_id}_run_{_seq++}";
            composed.agent_id   = effective_id;

            _engine = new Engine(composed);

            // (#2 Phase_3_5_2) Cache need names and action ids once.
            // Persona structure is fixed at ctor time, so per-frame GetAllNeedNames()
            // and GetAllActionIds() would allocate a string[] and a List<string> on
            // every RecordFrame call (216,000 allocations per 1-hour soak test).
            // Cache once; pass into RecordFrame to keep observation layer zero-alloc.
            var _need_names_cache = _engine.GetAllNeedNames();
            var _action_ids_cache = _engine.GetAllActionIds();

            // (#2 Q-S26) Subscribe to Engine.OnSignal so signals_fired is populated per frame.
            var _pending_signals = new System.Collections.Generic.List<string>();
            _engine.OnSignal += s => _pending_signals.Add(s);
            var result = new TraceResult();

            // (#5 + Q-S35) Normalize and sort events by time. spec §26.3.1 requires
            // events to be time-ordered; the next-pointer loop below would silently
            // skip out-of-order events. Internal stable sort ensures correctness even
            // when callers pass an unsorted list. Sort is O(N log N) once at run start.
            // (#3) Use stable OrderBy (LINQ) to preserve original insertion order for
            // same-time events — Array.Sort is unstable per .NET spec and would break
            // the Q-S35 "forward-pointer preserves authored order" contract.
            TimedAffectEvent[] ev_list;
            if (events == null) {
                ev_list = System.Array.Empty<TimedAffectEvent>();
            } else {
                ev_list = events.OrderBy(e => e.time).ToArray();
            }
            int next = 0;

            // (Q-S55) Sweep events[next].time <= 0.0f BEFORE spawn Live(0.0f).
            // Includes negative-time events (hand-built tests, pre-t0 priming).
            while (next < ev_list.Length && ev_list[next].time <= 0.0f) {
                _engine.Affect(ev_list[next].ev.need, ev_list[next].ev.delta, ev_list[next].ev.force_reset);
                next++;
            }

            // Spawn frame (Q-S34)
            _engine.Live(0.0f);
            RecordFrame(result, 0f, _engine, _pending_signals, _need_names_cache, _action_ids_cache);

            // (Q-S84 + Q-S98) Integer step counter with double-precision Math.Round.
            int   total_steps = (int)System.Math.Round((double)duration / (double)dt);
            bool  boundary_consumed = false;

            for (int step = 0; step < total_steps; step++) {
                float frame_end = (step + 1) * dt;

                // (Q-S35) Consume events within the upcoming frame window (next pointer, O(1) per frame).
                while (next < ev_list.Length && ev_list[next].time < frame_end) {
                    _engine.Affect(ev_list[next].ev.need, ev_list[next].ev.delta, ev_list[next].ev.force_reset);
                    next++;
                }

                _engine.Live(dt);
                RecordFrame(result, frame_end, _engine, _pending_signals, _need_names_cache, _action_ids_cache);
            }

            // (Q-S40) Post-loop sweep: events at time == duration (or missed by loop).
            bool sweep_any = false;
            while (next < ev_list.Length && ev_list[next].time <= duration) {
                _engine.Affect(ev_list[next].ev.need, ev_list[next].ev.delta, ev_list[next].ev.force_reset);
                next++;
                sweep_any = true;
            }
            if (sweep_any) {
                _engine.Live(0.0f);
                RecordFrame(result, duration, _engine, _pending_signals, _need_names_cache, _action_ids_cache);
            }

            // (Q-S93) Populate analysis counters in a single post-run pass.
            result.agent_id  = effective_id;
            result.duration  = duration;
            result.dt        = dt;
            result.BuildAnalysis();
            return result;
        }

        static void RecordFrame(TraceResult result, float time, Engine engine,
                                   System.Collections.Generic.List<string> pending_signals,
                                   System.Collections.Generic.IReadOnlyList<string> need_names,
                                   System.Collections.Generic.IReadOnlyList<string> action_ids) {
            var frame = new TraceFrame();
            frame.time            = time;
            frame.behavior        = engine.behavior;
            frame.is_locked       = engine.is_locked;
            frame.locked_behavior = engine.locked_behavior;
            // (#1 Q-S62) Collect signals fired since last frame and clear the buffer.
            frame.signals_fired.AddRange(pending_signals);
            pending_signals.Clear();
            // (#2 Phase_3_5_2) Use cached name/id lists — no per-frame alloc.
            for (int i = 0; i < need_names.Count; i++) {
                var n = need_names[i];
                frame.needs[n]           = engine.GetBaseNeed(n);
                frame.effective_needs[n] = engine.GetNeed(n);
            }
            for (int i = 0; i < action_ids.Count; i++)
                frame.action_scores[action_ids[i]] = engine.GetActionScore(action_ids[i]);
            result.frames.Add(frame);
        }
    }
}
