// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;

namespace Animo {
    /// <summary>
    /// Animo's logging facade. Wraps Unity Debug or stdout depending on environment.
    /// (v0.1.5, Q-S73 + Q-S127) Error-severity log added for fail-loud paths.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class AnimoLog {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Fields

        /// <summary>
        /// (Q-S128) Optional test hook. When set, Warning/Error/Write calls invoke
        /// this delegate in addition to their normal output. Enables EditMode tests
        /// to assert that specific log messages were emitted (e.g. A031 runtime Warning)
        /// without coupling to Unity console or stdout capture.
        /// Reset to null between tests via [TearDown].
        /// </summary>
        public static Action<string, string>? OnLog = null;  // (level, message)

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static void Write(string message) {
            OnLog?.Invoke("Write", message);
        }

        public static void Warning(string message) {
            OnLog?.Invoke("Warning", message);
        }

        /// <summary>
        /// (v0.1.5, Q-S73 + Q-S127) Error-severity log. Wraps
        /// UnityEngine.Debug.LogError in editor/runtime; falls back to
        /// System.Console.Error.WriteLine in headless environments.
        /// </summary>
        public static void Error(string message) {
            OnLog?.Invoke("Error", message);
        }
    }
}
