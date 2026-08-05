// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Tools;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Compile-time test for Q-S99 (v0.1.5): ScenarioRunner declares
    /// an internal int field for the run-counter (per Q-S42 contract).
    /// Pre-Q-S99 Q-S82's file materialization missed the field;
    /// Q-S92 added _engine but missed _sequence.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerSeqFieldTests {
        [Test] public void Case01_ScenarioRunner_DeclaresSeqIntField() {
            var t = typeof(ScenarioRunner);
            var field = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(f => f.Name == "_sequence" && f.FieldType == typeof(int));
            Assert.That(field, Is.Not.Null,
                "Q-S99: ScenarioRunner must declare an int _sequence field for Q-S42's " +
                "${template_id}_run_${_sequence++} default agent_id_override generation.");
        }
    }
}
