// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;
using Animo;
using Animo.Core;

namespace Animo.Tests.EditMode.EdgeCaseTests {
    /// <summary>
    /// Decision-table tests for Q-S140, Q-S142, Q-S143, Q-S144, Q-S145,
    /// Q-S146, Q-S147, Q-S148 (v0.1.5, Phase_2_4_27). Gemini round 30.
    /// Nine adopted findings; eleven hallucinations rejected.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Round30ContractTests {

        static string RepoRoot() {
            string? dir = System.IO.Directory.GetCurrentDirectory();
            while (dir != null &&
                   !System.IO.File.Exists(System.IO.Path.Combine(dir, "Scripts", "Const.cs")))
                dir = System.IO.Directory.GetParent(dir)?.FullName;
            return dir ?? System.IO.Directory.GetCurrentDirectory();
        }

        // ── Q-S140: Agent.OnDestroy Unlock contract ──────────────────────────
        [Test] public void Case01_AgentOnDestroy_DocumentsUnlockContract() {
            var path = Path.Combine(RepoRoot(), "Scripts", "Agent.cs");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("Q-S140"),
                "Q-S140: Agent.cs OnDestroy must document the Engine.Unlock() contract " +
                "per spec §24.6.2.");
            Assert.That(text, Does.Contain("_engine?.Unlock()"),
                "Q-S140: Agent.cs OnDestroy must call _engine?.Unlock() before Unregister.");
        }

        // ── Q-S142: _locked_behavior_index field ─────────────────────────────
        [Test] public void Case02_Engine_Declares_LockedBehaviorIndex() {
            var path = Path.Combine(RepoRoot(), "Scripts", "Engine.cs");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("_locked_behavior_index"),
                "Q-S142: Engine.cs must declare _locked_behavior_index field. " +
                "Spec §24 / line 237 reference _action_scores[locked_behavior_index] " +
                "but the field was never declared — Phase 3 compile error.");
            Assert.That(text, Does.Contain("-1"),
                "Q-S142: _locked_behavior_index sentinel must be -1 (not locked).");
        }

        // ── Q-S143: [Serializable] on exceptions ─────────────────────────────
        [Test] public void Case03_PersonaCacheExceptions_AreSerializable() {
            Assert.That(
                typeof(PersonaCacheNotInitializedException)
                    .IsDefined(typeof(System.SerializableAttribute), inherit: false),
                Is.True,
                "Q-S143: PersonaCacheNotInitializedException must have [Serializable] " +
                "for correct behavior across Unity Editor assembly reload boundaries.");
            Assert.That(
                typeof(PersonaTemplateRejectedException)
                    .IsDefined(typeof(System.SerializableAttribute), inherit: false),
                Is.True,
                "Q-S143: PersonaTemplateRejectedException must have [Serializable].");
        }

        // ── Q-S144: AnimoLog.Error logging responsibility ────────────────────
        [Test] public void Case04_SpecDocumentsLoggingResponsibility() {
            var path = Path.Combine(RepoRoot(), "docs",
                "animo_spec.md");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("Q-S144"),
                "Q-S144: spec EN must document the AnimoLog.Error single-responsibility " +
                "contract: PersonaCache throws (no log); Agent.Awake catches and logs.");
        }

        // ── Q-S145: agent_id_override empty string guard ─────────────────────
        [Test] public void Case05_ScenarioRunner_DocumentsEmptyOverrideGuard() {
            var path = Path.Combine(RepoRoot(), "Scripts", "Tools", "ScenarioRunner.cs");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("Q-S145"),
                "Q-S145: ScenarioRunner.cs must document the agent_id_override " +
                "empty-string fail-loud contract.");
        }

        // ── Q-S146: ValidationResult safe defaults ───────────────────────────
        [Test] public void Case06_ValidationResult_ErrorsReturnsEmptyNotThrows() {
            var result = new ValidationResult();
            Assert.DoesNotThrow(
                () => { var _ = result.errors; },
                "Q-S146: ValidationResult.errors must not throw NotImplementedException " +
                "— debugger auto-evaluation would flood the IDE.");
            Assert.That(result.errors, Is.Not.Null,
                "Q-S146: errors must return a non-null list (empty is fine).");
            Assert.DoesNotThrow(() => { var _ = result.warnings; },
                "Q-S146: warnings must not throw.");
            Assert.DoesNotThrow(() => { var _ = result.infos; },
                "Q-S146: infos must not throw.");
        }

        // ── Q-S147: Agent.Update null _engine guard ──────────────────────────
        [Test] public void Case07_AgentUpdate_DocumentsNullEngineGuard() {
            var path = Path.Combine(RepoRoot(), "Scripts", "Agent.cs");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("Q-S147"),
                "Q-S147: Agent.cs Update must document the null _engine guard " +
                "(MockScene dispatches Update regardless of MonoBehaviour.enabled).");
            Assert.That(text, Does.Contain("if (_engine == null) return;"),
                "Q-S147: Agent.cs Update must have if (_engine == null) return; guard.");
        }

        // ── Q-S148: Store.IsRegistered detailed contract ─────────────────────
        [Test] public void Case08_Store_IsRegistered_HasDetailedContract() {
            var path = Path.Combine(RepoRoot(), "Scripts", "Store.cs");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("Q-S148"),
                "Q-S148: Store.cs IsRegistered docstring must reference Q-S148.");
            Assert.That(text, Does.Contain("FIRST-registered"),
                "Q-S148: docstring must clarify the 'keep first' duplicate-register " +
                "interaction — critical for test authors reasoning about Store state.");
        }

        // ── Q-S141: DeepCopy on model classes ────────────────────────────────
        [Test] public void Case09_ModelClasses_HaveDeepCopy() {
            // Structural verification: Q-S141 pattern requires all
            // reference-type model classes used in Persona.DeepCopy to
            // declare their own DeepCopy() so future field additions
            // trigger a compiler error here. Verified via reflection.
            var types = new[] {
                typeof(Animo.Model.Influence),
                typeof(Animo.Model.Action),
                typeof(Animo.Model.Commitment),
                typeof(Animo.Model.Binding),
                typeof(Animo.Model.Threshold),
            };
            foreach (var t in types) {
                var method = t.GetMethod("DeepCopy",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance);
                Assert.That(method, Is.Not.Null,
                    $"Q-S141: {t.Name}.DeepCopy() must be declared — Q-S134 pattern " +
                    $"extended to all model classes used in Persona.DeepCopy().");
                Assert.That(method!.ReturnType, Is.EqualTo(t),
                    $"Q-S141: {t.Name}.DeepCopy() must return {t.Name}.");
            }
        }
    }
}
