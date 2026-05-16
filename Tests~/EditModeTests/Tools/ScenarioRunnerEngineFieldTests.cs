// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Core;
using Animo.Tools;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Compile-time test for Q-S92 (v0.1.5): ScenarioRunner declares an
    /// internal Engine field (per Q-S60 decision). Pre-Q-S92 Q-S82's
    /// file materialization left the field undeclared — Phase 3 would
    /// hit a compile error.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class ScenarioRunnerEngineFieldTests {
        [Test] public void Case01_ScenarioRunner_DeclaresEngineField() {
            var t = typeof(ScenarioRunner);
            var field = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(f => f.FieldType == typeof(Engine)
                    || (f.FieldType.IsGenericType
                        && f.FieldType.GetGenericTypeDefinition() == typeof(System.Nullable<>)
                        && System.Nullable.GetUnderlyingType(f.FieldType) == typeof(Engine))
                    || f.Name == "_engine");
            Assert.That(field, Is.Not.Null,
                "Q-S92: ScenarioRunner must declare an Engine field per Q-S60 decision " +
                "(single Engine instance, not Dictionary<string, Engine>).");
        }
    }
}
