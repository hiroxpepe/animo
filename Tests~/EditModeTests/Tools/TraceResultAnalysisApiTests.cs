// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Animo.Tools;

namespace Animo.Tests.EditMode.ToolsTests {
    /// <summary>
    /// Compile-time test for Q-S93 (v0.1.5): TraceResult exposes the
    /// behavior_count, behavior_total_time, ToCSV, ToJSON surface that
    /// spec §26.3 promised.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class TraceResultAnalysisApiTests {
        [Test] public void Case01_TraceResult_DeclaresBehaviorCount() {
            var t = typeof(TraceResult);
            var prop = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.Name == "behavior_count");
            Assert.That(prop, Is.Not.Null, "Q-S93: TraceResult.behavior_count required.");
            Assert.That(prop!.PropertyType, Is.EqualTo(typeof(Dictionary<string, int>)),
                "Q-S93: behavior_count must be Dictionary<string, int>.");
        }

        [Test] public void Case02_TraceResult_DeclaresBehaviorTotalTime() {
            var t = typeof(TraceResult);
            var prop = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.Name == "behavior_total_time");
            Assert.That(prop, Is.Not.Null, "Q-S93: TraceResult.behavior_total_time required.");
            Assert.That(prop!.PropertyType, Is.EqualTo(typeof(Dictionary<string, float>)),
                "Q-S93: behavior_total_time must be Dictionary<string, float>.");
        }

        [Test] public void Case03_TraceResult_DeclaresToCsv() {
            var t = typeof(TraceResult);
            var method = t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "ToCSV" && m.GetParameters().Length == 0);
            Assert.That(method, Is.Not.Null, "Q-S93: TraceResult.ToCSV() required.");
            Assert.That(method!.ReturnType, Is.EqualTo(typeof(string)),
                "Q-S93: ToCSV must return string.");
        }

        [Test] public void Case04_TraceResult_DeclaresToJson() {
            var t = typeof(TraceResult);
            var method = t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "ToJSON" && m.GetParameters().Length == 0);
            Assert.That(method, Is.Not.Null, "Q-S93: TraceResult.ToJSON() required.");
            Assert.That(method!.ReturnType, Is.EqualTo(typeof(string)),
                "Q-S93: ToJSON must return string.");
        }
    }
}
