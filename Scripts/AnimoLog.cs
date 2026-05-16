// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

namespace Animo {
    /// <summary>Animo's logging facade. Wraps Unity Debug or stdout depending on environment.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class AnimoLog {
        public static void Write(string message) {
            // Phase 3 implementation. No-op here so test harness need not stub it.
        }

        public static void Warning(string message) {
            // Phase 3 implementation.
        }

        /// <summary>
        /// (v0.1.5, Q-S73 + Q-S127) Error-severity log output. Used by the
        /// fail-loud paths: `PersonaCache.GetComposed` (Q-S38 stage-2
        /// throw) catch sites and `Agent.Awake` (Q-S38 try/catch) when
        /// a template is rejected and the Agent is disabled. Pre-Q-S73
        /// §11.4.1 + §11.6.1 sample code called `AnimoLog.Error(msg)`
        /// but the method had no declaration in `Scripts/AnimoLog.cs`
        /// — confirmed missing-method compile error.
        ///
        /// (v0.1.5, Q-S127) The Phase 3 implementation comment names
        /// `System.Console.Error.WriteLine` (fully qualified) instead
        /// of bare `Console.Error.WriteLine` because this file has no
        /// `using System;` directive. A Phase 3 implementer copy-pasting
        /// the comment literally would otherwise hit CS0103 ("the name
        /// `Console` does not exist"). Either form compiles when the
        /// Phase 3 body is written; the qualified form is documented
        /// here so the contract is self-contained.
        /// </summary>
        public static void Error(string message) {
            // Phase 3 implementation. Wraps `UnityEngine.Debug.LogError`
            // in editor/runtime, falls back to `System.Console.Error.WriteLine`
            // in headless environments (tests, server simulation).
        }
    }
}
