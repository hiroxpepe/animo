// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
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
            throw new System.NotImplementedException();
        }
    }
}
