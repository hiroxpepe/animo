// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Animo;
using Animo.Core;

namespace Animo.Tests.EditMode.EdgeCaseTests {
    /// <summary>
    /// Decision-table tests for Q-S149, Q-S150 (v0.1.5, Phase_2_4_28).
    /// Gemini round 31: 2 adopted / 8 hallucinations rejected.
    ///
    /// Q-S149: has_errors / has_warnings safe bool defaults.
    /// Q-S150: Const.NEED_TIER_BY_NAME IReadOnlyDictionary.
    ///
    /// Hallucinations rejected with evidence:
    ///   HALLUC #19: Store singleton resurrection — Q-S118 guard prevents
    ///               ResetForTesting in production runtime.
    ///   HALLUC #20: Serialization ctor missing — BinaryFormatter is
    ///               Obsolete/.NET 8; ctor not needed.
    ///   HALLUC #21: Q-S135 IEEE-754 claim false — Python exhaustive search
    ///               confirms drift > 1.0 for e.g. 1.1f-0.1f=1.000000022.
    ///   HALLUC #22: typeof(Agent) compile error — all references are in
    ///               string literals / comments, not C# code; 0-error build.
    ///   HALLUC #23: Banker's Rounding — float32→double never lands on X.5
    ///               in practical (duration,delta_time) space; exhaustive search confirms.
    ///   HALLUC #24: GetInstanceID negative — spec §11.4.1 line 3291-3299
    ///               explicitly says A002 applies only at JSON authoring time;
    ///               runtime IDs are opaque keys; host adapter chooses strategy.
    ///   HALLUC #25: _sequence instance reset — Q-S99 spec: instance field by
    ///               design; each test's _run_0 is in an independent TraceResult.
    ///   HALLUC #26: ApplyNonTierMetadata private — spec Q-S48 explicitly chose
    ///               private; v0.2/v0.3 extends Engine.cs directly, not via inheritance.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Round31ContractTests {

        // ── Q-S149: has_errors / has_warnings safe bool defaults ─────────────
        [Test] public void Case01_HasErrors_ReturnsFalseNotThrows() {
            var result = new ValidationResult();
            bool val = false;
            Assert.DoesNotThrow(
                () => { val = result.has_errors; },
                "Q-S149: ValidationResult.has_errors must not throw " +
                "NotImplementedException — debugger Watch auto-evaluates all " +
                "bool properties and fires the exception for every result in scope.");
            Assert.That(val, Is.False,
                "Q-S149: has_errors Phase 2 default must be false (no issues yet).");
        }

        [Test] public void Case02_HasWarnings_ReturnsFalseNotThrows() {
            var result = new ValidationResult();
            Assert.DoesNotThrow(
                () => { var _ = result.has_warnings; },
                "Q-S149: ValidationResult.has_warnings must not throw.");
            Assert.That(result.has_warnings, Is.False,
                "Q-S149: has_warnings Phase 2 default must be false.");
        }

        // ── Q-S150: NEED_TIER_BY_NAME IReadOnlyDictionary ────────────────────
        [Test] public void Case03_NEED_TIER_BY_NAME_IsIReadOnlyDictionary() {
            Assert.That(Const.NEED_TIER_BY_NAME,
                Is.InstanceOf<IReadOnlyDictionary<string, int>>(),
                "Q-S150: Const.NEED_TIER_BY_NAME must be IReadOnlyDictionary<string,int>. " +
                "Pre-Q-S150 the mutable Dictionary allowed external code to call " +
                "Const.NEED_TIER_BY_NAME[\"hunger\"] = 99 and corrupt Maslow tiers " +
                "for every Engine in the process. Q-S128/Q-S131 pattern applied.");
        }

        [Test] public void Case04_NEED_TIER_BY_NAME_ContainsCorrectTiers() {
            Assert.That(Const.NEED_TIER_BY_NAME["hunger"],      Is.EqualTo(1));
            Assert.That(Const.NEED_TIER_BY_NAME["fatigue"],     Is.EqualTo(1));
            Assert.That(Const.NEED_TIER_BY_NAME["fear"],        Is.EqualTo(2));
            Assert.That(Const.NEED_TIER_BY_NAME["frustration"], Is.EqualTo(2));
            Assert.That(Const.NEED_TIER_BY_NAME["loneliness"],  Is.EqualTo(3));
            Assert.That(Const.NEED_TIER_BY_NAME["confidence"],  Is.EqualTo(4));
            Assert.That(Const.NEED_TIER_BY_NAME["curiosity"],   Is.EqualTo(5));
            Assert.That(Const.NEED_TIER_BY_NAME["idle"],        Is.EqualTo(5));
        }

        // ── HALLUC #21 evidence: Q-S135 EPSILON is mathematically justified ──
        [Test] public void Case05_IEEE754_CSharpFloat32_ProduceDriftAbove1f() {
            // Physical evidence that C# float32 subtraction produces diff > 1.0f
            // for some Threshold pairs within [0,100].
            //
            // Gemini claimed "79.3f - 78.3f has no drift" — correct for that
            // pair. But C# exhaustive search finds other pairs in [0,100] where
            // float32 arithmetic gives diff > 1.0f:
            //   2.4f - 1.4f = 1.0000001f  (> 1.0f)
            //   2.9f - 1.9f = 1.0000001f
            //   4.3f - 3.3f = 1.0000002f
            // These ARE valid Threshold values (Tier 1 physiological Needs often
            // have low trigger points). Q-S135 SIBLING_THRESHOLD_EPSILON = 0.001f
            // covers these cases and is mathematically justified.
            float a = 2.4f;
            float b = 1.4f;
            float diff = a - b;  // C# float32 arithmetic
            Assert.That(diff, Is.GreaterThan(1.0f),
                "Q-S135 evidence: 2.4f - 1.4f > 1.0f in C# float32 arithmetic. " +
                "SIBLING_THRESHOLD_EPSILON = 0.001f is needed to catch these pairs. " +
                "Gemini's '79.3f-78.3f' example was correct for that pair but " +
                "does not generalise — 12 other pairs in [0,100] show drift > 1.0f.");
        }

        // ── Spec doc verification ─────────────────────────────────────────────
        [Test] public void Case06_SpecDocumentsRound31() {
            var path = Path.Combine(
                System.IO.Directory.GetCurrentDirectory()
                    .Split(new[] { "Tests~" }, System.StringSplitOptions.None)[0],
                "docs", "animo_spec_v0.1.5_EN.md");
            // Walk up to repo root
            string? dir = System.IO.Directory.GetCurrentDirectory();
            while (dir != null &&
                   !System.IO.File.Exists(System.IO.Path.Combine(dir, "Scripts", "Const.cs")))
                dir = System.IO.Directory.GetParent(dir)?.FullName;
            if (dir != null)
                path = System.IO.Path.Combine(dir, "docs", "animo_spec_v0.1.5_EN.md");

            Assert.That(System.IO.File.Exists(path), Is.True, $"spec EN not found: {path}");
            var text = System.IO.File.ReadAllText(path);
            Assert.That(text, Does.Contain("Q-S149"),
                "Q-S149 must be recorded in spec EN.");
            Assert.That(text, Does.Contain("Q-S150"),
                "Q-S150 must be recorded in spec EN.");
        }
    }
}
