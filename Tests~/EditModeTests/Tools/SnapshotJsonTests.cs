// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.Tools {

    /// <summary>
    /// The monitor sends a snapshot to the dashboard as JSON. The keys must be
    /// the snake_case names the snapshot carries, so the browser reads the same
    /// need names the engine uses. These tests hold that the write is there and
    /// that it keeps those names.
    /// </summary>
    [TestFixture]
    public class SnapshotJsonTests {

        static EngineSnapshot MakeSnapshot() {
            var persona = new Persona {
                agent_id = "scout",
                needs = NeedsOf(("hunger", 30f), ("fear", 10f), ("idle", 70f)),
                rates = RatesOf(("hunger", +0.5f)),
                actions = new List<Animo.Model.Action> {
                    ActionOf("Eat", "hunger", 1, 1.0f),
                    ActionOf("Flee", "fear", 1, 1.0f),
                },
            };
            var engine = new Animo.Core.Engine(persona);
            engine.Live(0.5f);
            return engine.Snapshot();
        }

        [Test]
        public void Serialize_WritesTheSnakeCaseKeys() {
            var json = Animo.JSON.Serialize(MakeSnapshot());
            Assert.That(json, Does.Contain("base_needs"));
            Assert.That(json, Does.Contain("effective_needs"));
            Assert.That(json, Does.Contain("action_scores"));
            Assert.That(json, Does.Contain("is_locked"));
        }

        [Test]
        public void Serialize_KeepsTheNeedNames() {
            var json = Animo.JSON.Serialize(MakeSnapshot());
            Assert.That(json, Does.Contain("hunger"));
            Assert.That(json, Does.Contain("fear"));
        }

        [Test]
        public void Serialize_KeepsTheBehavior() {
            var json = Animo.JSON.Serialize(MakeSnapshot());
            Assert.That(json, Does.Contain("behavior"));
        }
    }
}
