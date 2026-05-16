// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S44 (v0.1.5): Engine.GetExpandedActionTrigger
    /// returns the same template-expanded string OnBehaviorChanged would
    /// publish to Bus, for the named behavior. Used by Agent.Awake step
    /// (6) to keep frame-1 Animator state-name format consistent with
    /// frame-2+ Bus payloads.
    ///
    /// Phase 3 contract: GetExpandedActionTrigger("flee") returns
    /// e.g. "animo_goblin_47291_flee" (template-expanded with the
    /// runtime-unique agent_id from Q-S28 override). Falls back to
    /// the raw behavior id if binding.on_action_change is unset.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class GetExpandedActionTriggerTests {
        [Test] public void Case01_KnownBehavior_ReturnsTemplateExpandedTrigger() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Engine.GetExpandedActionTrigger(behavior) must read _cached_action_triggers " +
                "and return the template-expanded form (same as OnBehaviorChanged would publish). " +
                "Q-S44 fix for Q-S34's frame-1-vs-frame-2 Animator state-name asymmetry.");
        }
    }
}
