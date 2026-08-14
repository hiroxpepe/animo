// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S25 (v0.1.5): Threshold needs a 1-bit
    /// `is_above` state to implement the §12.3.2 hysteresis state machine.
    /// Without state, prev<trig && curr>=trig cross-detection chatters
    /// around `trigger`; the value never has to drop to `reset_threshold`
    /// to re-arm, so reset_threshold becomes dead code.
    ///
    /// Phase 3 (full Engine impl) will assert the firing sequence
    /// directly via OnSignaled subscription. Here we pin the data-shape
    /// contract: Threshold has the is_above field.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ThresholdHysteresisStateTests {

        [Test] public void Case01_ThresholdInstanceHasIsAboveStateField() {
            // Q-S25 contract: every Threshold instance carries an
            // `internal bool is_above` field. This test confirms the
            // field exists on the class (compile-time verification: the
            // assignment below would not compile if Threshold lacked the
            // field). Engine ctor will seed it from spawn _effective_needs.
            Threshold t = ThresholdOf(need: "fear", trigger: 80.0f, trigger_event: "fear_burst", reset: 70.0f);
            // Default value: false (Below state — "ready to fire" per §12.3.2).
            Assert.That(t.is_above, Is.False,
                "Q-S25: Threshold.is_above must default to false (Below state, ready to fire). " +
                "Engine ctor will overwrite this with the spawn-time evaluation if the spawn " +
                "Need is already at or above trigger_threshold (Q-S8 + Q-S23 + Q-S25 seeding).");

            // The field is mutable from inside the Animo namespace
            // (internal access — the test runs in Animo.Tests.* which
            // is part of the same assembly through InternalsVisibleTo).
            t.is_above = true;
            Assert.That(t.is_above, Is.True);
        }
    }
}
