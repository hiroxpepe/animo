// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S62 (v0.1.5): Step 4 (score calculation)
    /// runs every frame even under Hard lock. The post-unlock Step 5
    /// reads the locked-behavior's _action_scores to compute the
    /// smooth-out-of-lock decision; if Step 4 had skipped during lock,
    /// the score would be stale.
    ///
    /// Phase 3 contract: After 100 Live(dt) calls under Hard lock,
    /// _action_scores[locked_behavior_index] reflects the current Need
    /// state, not the score from the frame the lock began.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class HardLockStep4ContinuityTests {
        [Test] public void Case01_LockedFrames_UpdateActionScores_ForPostUnlockContinuity() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "After Hard locking and running 100 Live(dt) calls during which Needs change " +
                "(e.g. fear rises from 30 to 80), _action_scores[locked_behavior] must " +
                "reflect the current Needs (computed by Step 4 every frame), not stale " +
                "pre-lock values. Q-S62 design rationale: post-unlock Step 5 needs current " +
                "scores for smooth-out-of-lock.");
        }
    }
}
