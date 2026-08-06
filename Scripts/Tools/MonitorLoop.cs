// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using Animo.Core;

namespace Animo.Tools {

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    // public Classes

    /// <summary>
    /// Drives a live <see cref="Engine"/> for the monitor. Each tick it applies
    /// any step-ins queued from the dashboard at the head, advances the engine by
    /// one delta_time (unless paused), and returns a snapshot to send out. It holds no
    /// socket of its own, so the loop can be read and tested on its own; the
    /// server layer calls Tick and ships the snapshot.
    ///
    /// A step-in is kept as a small action on the engine, queued now and run at
    /// the head of the next advancing frame, so a change from the dashboard never
    /// falls in the middle of a step.
    /// </summary>
    public sealed class MonitorLoop {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Const [nouns]

        /// <summary>The smallest step the loop will run. A smaller ask clamps up.</summary>
        public const float DT_MIN = 0.001f;

        /// <summary>The largest step the loop will run. A larger ask clamps down.</summary>
        public const float DT_MAX = 10f;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        readonly Engine _engine;
        readonly Queue<Action<Engine>> _pending_steps = new();
        float _delta_time;
        bool _paused;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public MonitorLoop(Engine engine, float delta_time) {
            _engine = engine;
            _delta_time = clampDeltaTime(delta_time);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        /// <summary>The engine being driven, so a caller can read its state.</summary>
        public Engine Engine => _engine;

        /// <summary>True while the loop holds the engine still.</summary>
        public bool IsPaused => _paused;

        /// <summary>
        /// The step size used by each advancing frame. A value from the dashboard
        /// is clamped to a sane range, so a wild number cannot run the engine off
        /// a cliff or freeze it at zero.
        /// </summary>
        public float DeltaTime {
            get => _delta_time;
            set => _delta_time = clampDeltaTime(value);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Queue an Affect step-in. It lands at the head of the next frame, so the
        /// change never falls in the middle of a step.
        /// </summary>
        public void QueueAffect(string need, float delta, bool force_reset = false) {
            _pending_steps.Enqueue(engine => engine.Affect(need, delta, force_reset));
        }

        /// <summary>Queue a Lock step-in, applied at the head of the next frame.</summary>
        public void QueueLock(float duration, LockMode mode = LockMode.Hard) {
            _pending_steps.Enqueue(engine => engine.Lock(duration, mode));
        }

        /// <summary>Queue an Unlock step-in, applied at the head of the next frame.</summary>
        public void QueueUnlock() {
            _pending_steps.Enqueue(engine => engine.Unlock());
        }

        /// <summary>Hold the engine still. Ticks still hand out snapshots.</summary>
        public void Pause() => _paused = true;

        /// <summary>Let the engine advance again on the next frame.</summary>
        public void Resume() => _paused = false;

        /// <summary>
        /// One tick: apply queued step-ins at the head, advance one delta_time unless
        /// paused, then return the snapshot to send to the dashboard.
        /// </summary>
        public EngineSnapshot Tick() {
            applyPending();
            if (!_paused)
                _engine.Live(_delta_time);
            return _engine.Snapshot();
        }

        /// <summary>
        /// Move exactly one frame even while paused, then stay paused. This is how
        /// a designer reads a hard moment closely: pause, then step frame by frame.
        /// Queued step-ins are applied at its head, the same as a normal tick.
        /// </summary>
        public EngineSnapshot Step() {
            applyPending();
            _engine.Live(_delta_time);
            return _engine.Snapshot();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static float clampDeltaTime(float delta_time) {
            if (delta_time < DT_MIN) return DT_MIN;
            if (delta_time > DT_MAX) return DT_MAX;
            return delta_time;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        void applyPending() {
            while (_pending_steps.Count > 0)
                _pending_steps.Dequeue()(_engine);
        }
    }
}
