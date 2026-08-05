// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

namespace Animo.Tests.MiniUnity {
    /// <summary>
    /// Pure-C# stand-in for <c>UnityEngine.Time</c>. Tests set <see cref="deltaTime"/>
    /// and the active <see cref="MockScene"/> uses it during <c>Tick</c>.
    ///
    /// Static state is intentional (mirrors Unity's API). Tests should call
    /// <see cref="Reset"/> at SetUp / TearDown to avoid leakage between cases.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class MockTime {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun]

        /// <summary>Simulated delta time for the next <c>Update</c>. Defaults to 1/60 second.</summary>
        public static float deltaTime { get; set; } = 1f / 60f;

        /// <summary>Total simulated time accumulated via <see cref="Step(float)"/>.</summary>
        public static float elapsed_seconds { get; private set; } = 0f;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Advance virtual time by <paramref name="delta_time"/>. Sets <see cref="deltaTime"/>
        /// to <paramref name="delta_time"/> for the upcoming <c>Update</c> calls and
        /// accumulates <see cref="elapsed_seconds"/>.
        ///
        /// Note: This advances the clock only. To actually fire <c>Update</c> on
        /// every component, call <see cref="MockScene.Tick(float)"/>.
        /// </summary>
        /// <param name="delta_time">Seconds to advance.</param>
        public static void Step(float delta_time) {
            deltaTime = delta_time;
            elapsed_seconds += delta_time;
        }

        /// <summary>
        /// Reset to defaults. Call from test SetUp / TearDown.
        /// </summary>
        public static void Reset() {
            deltaTime = 1f / 60f;
            elapsed_seconds = 0f;
        }
    }
}
