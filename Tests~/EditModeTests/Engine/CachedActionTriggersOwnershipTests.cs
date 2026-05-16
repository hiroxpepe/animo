// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;
using Animo.Core;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S46 (v0.1.5): Engine.GetExpandedActionTrigger
    /// must be callable. The accessor reads Engine's own internal
    /// _cached_action_triggers Dictionary. Pre-Q-S46 §16.6 placed the cache
    /// on Agent (a MonoBehaviour); Engine cannot reach into Agent's fields,
    /// so the method body would have been a compile error.
    ///
    /// Phase 3 contract: Engine ctor builds _cached_action_triggers from
    /// _persona.binding.on_action_change template; GetExpandedActionTrigger
    /// reads it. This test simply asserts the accessor is invokable
    /// (compile + no-NRE on a stub Engine).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class CachedActionTriggersOwnershipTests {
        [Test] public void Case01_GetExpandedActionTrigger_IsInvokable() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "Engine.GetExpandedActionTrigger must be callable on a constructed Engine. " +
                "Q-S46 fix: _cached_action_triggers is owned by Engine (not Agent), so the " +
                "accessor body can read it without scope violation.");
        }
    }
}
