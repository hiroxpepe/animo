// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;
using Animo;

namespace Animo.Tests.EditMode.StoreTests {
    /// <summary>
    /// Decision-table tests for Store.Register duplicate-id contract
    /// (v0.1.5, Q-S6). Spec §11.2 (v0.1.5):
    /// - first Register: succeeds
    /// - re-Register same instance: no-op (idempotent)
    /// - re-Register different instance with same agent_id: Warning + no-op,
    ///   original kept
    /// - never throws (Awake-time exception would corrupt scene load)
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class StoreRegisterDuplicateTests {

        [SetUp]
        public void Setup() {
            Store.ResetForTesting();
        }

        sealed class FakeAgent : IAnimoAgent {
            public string agent_id { get; }
            public FakeAgent(string id) { agent_id = id; }
            public void Affect(string need, float delta, bool force_reset = false) {}
        }

        [Test] public void Case01_FirstRegister_Succeeds() {
            FakeAgent a = new FakeAgent(id: "goblin_01");
            Store.Instance.Register(agent: a);
            Assert.That(Store.Instance.IsRegistered(agent_id: "goblin_01"), Is.True);
        }

        [Test] public void Case02_ReRegisterSameInstance_IsIdempotentNoOp() {
            FakeAgent a = new FakeAgent(id: "goblin_01");
            Store.Instance.Register(agent: a);
            // Second call with the SAME instance must not throw, must remain registered.
            Assert.DoesNotThrow(code: () => Store.Instance.Register(agent: a));
            Assert.That(Store.Instance.IsRegistered(agent_id: "goblin_01"), Is.True);
        }

        [Test] public void Case03_ReRegisterDifferentInstance_KeepsFirstNoThrow() {
            // Spec §11.2 (Q-S6): different instance with same agent_id → Warning,
            // original kept, no exception. Unity Awake must never explode.
            FakeAgent first  = new FakeAgent(id: "goblin_01");
            FakeAgent second = new FakeAgent(id: "goblin_01");
            Store.Instance.Register(agent: first);
            Assert.DoesNotThrow(code: () => Store.Instance.Register(agent: second),
                "duplicate Register must NOT throw — would corrupt Unity scene load");
            Assert.That(Store.Instance.IsRegistered(agent_id: "goblin_01"), Is.True);
        }

        [Test] public void Case04_AfterUnregisterFirst_CanRegisterDifferentInstance() {
            // Once the first agent unregisters (e.g. OnDestroy), the slot is free
            // and a new agent with the same id can claim it without a Warning.
            FakeAgent first  = new FakeAgent(id: "goblin_01");
            FakeAgent second = new FakeAgent(id: "goblin_01");
            Store.Instance.Register(agent: first);
            Store.Instance.Unregister(agent: first);
            Assert.DoesNotThrow(code: () => Store.Instance.Register(agent: second));
            Assert.That(Store.Instance.IsRegistered(agent_id: "goblin_01"), Is.True);
        }
    }
}
