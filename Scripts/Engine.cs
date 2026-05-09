// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using Animo.Model;

namespace Animo.Core {
    /// <summary>Lock mode for behavior locking (v0.1.4). See spec §24.2.1.</summary>
    public enum LockMode {
        Hard,
        Soft
    }

    /// <summary>
    /// Animo AI calculation engine. Runs the 5-step Live(dt) per frame:
    /// natural decay → effective needs → threshold check → score → switch.
    /// See spec §9.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Engine {

        readonly Persona _persona;

        public Engine(Persona persona) {
            _persona = persona;
        }

        /// <summary>Current chosen action id. Empty before the first Live().</summary>
        public string behavior => throw new NotImplementedException();

        /// <summary>Whether the engine is in Lock state.</summary>
        public bool is_locked => throw new NotImplementedException();

        /// <summary>The action id locked when Lock() was called. Empty if not locked.</summary>
        public string locked_behavior => throw new NotImplementedException();

        /// <summary>Advance the engine by dt seconds (5-step process).</summary>
        public void Live(float dt) {
            throw new NotImplementedException();
        }

        /// <summary>External stimulus. Add delta to the named Need; clamp to [0,100].</summary>
        public void Affect(string need, float delta, bool force_reset = false) {
            throw new NotImplementedException();
        }

        /// <summary>Lock the current behavior for duration seconds.</summary>
        public void Lock(float duration, LockMode mode = LockMode.Hard) {
            throw new NotImplementedException();
        }

        /// <summary>Manually release the lock (emergency only; auto-release is preferred).</summary>
        public void Unlock() {
            throw new NotImplementedException();
        }
    }
}
