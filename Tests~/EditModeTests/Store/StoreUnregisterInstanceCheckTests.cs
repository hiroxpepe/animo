// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;
using Animo;

namespace Animo.Tests.EditMode.StoreTests {
    /// <summary>
    /// Decision-table tests for Q-S22 (v0.1.5): Store.Unregister(agent)
    /// must verify that the dictionary entry's instance is the same as
    /// the passed agent before removing. Without this check, a duplicate
    /// Agent B (rejected at Register time by Q-S6's "keep first") would
    /// on its OnDestroy assassinate the original Agent A's registration.
    ///
    /// Pairs symmetrically with Q-S6:
    ///   - Register protects against duplicate intrusion (keep first)
    ///   - Unregister protects against duplicate exit (only the same
    ///     instance may remove the entry)
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class StoreUnregisterInstanceCheckTests {

        [SetUp]
        public void Setup() {
            Store.ResetForTesting();
        }

        sealed class FakeAgent : IAnimoAgent {
            public string agent_id { get; }
            public FakeAgent(string id) { agent_id = id; }
        }

        [Test] public void Case01_DuplicateRejectedThenItsDestroyDoesNotEvictOriginal() {
            // Pre: Agent A registered. Agent B (different instance, same id)
            //      attempts Register → Q-S6 keep first → A is preserved.
            // Q-S22: B's OnDestroy → Unregister(B) → must NOT evict A.
            FakeAgent agent_a = new FakeAgent(id: "goblin_01");
            FakeAgent agent_b = new FakeAgent(id: "goblin_01");
            Store.Instance.Register(agent: agent_a);
            Store.Instance.Register(agent: agent_b);   // Q-S6 rejects, keeps A

            Store.Instance.Unregister(agent: agent_b); // Q-S22: must NOT evict A

            Assert.That(Store.Instance.IsRegistered(agent_id: "goblin_01"), Is.True,
                "Q-S22: original Agent A's registration must survive a duplicate's Unregister attempt. " +
                "A naive _agents.Remove(agent.agent_id) would assassinate A; Q-S22 requires " +
                "ReferenceEquals(_agents[id], agent) before removing.");
        }

        [Test] public void Case02_SameInstance_UnregisterRemovesEntry() {
            // Q-S22 control case: A's own OnDestroy → Unregister(A) → does remove.
            FakeAgent agent_a = new FakeAgent(id: "goblin_02");
            Store.Instance.Register(agent: agent_a);

            Store.Instance.Unregister(agent: agent_a);

            Assert.That(Store.Instance.IsRegistered(agent_id: "goblin_02"), Is.False,
                "Q-S22: same-instance Unregister must remove normally (this is the non-Q-S22 baseline)");
        }
    }
}
