// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.IO;
using NUnit.Framework;

namespace Animo.Tests.EditMode.EdgeCaseTests {
    /// <summary>
    /// Decision-table tests for Q-S151 (v0.1.5, Phase_2_4_29).
    /// Gemini round 32: 1 adopted / 2 hallucinations rejected.
    ///
    /// Q-S151: Needs / Rates JSON-bridge deserialization contract.
    ///
    /// Hallucinations rejected:
    ///   HALLUC #27: PersonaCache violates Q-S111/Q-S144 with concrete code
    ///               — Gemini quoted 3 lines that do NOT physically exist in
    ///               PersonaCache.cs (grep returns 0 hits across all Scripts/).
    ///               GetComposed body is `throw new NotImplementedException();`.
    ///   HALLUC #28: System.Linq missing causes CS1061 — FirstOrDefault is not
    ///               called anywhere in Scripts/. Build is 0 Warning 0 Error.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class Round32ContractTests {

        static string RepoRoot() {
            string? dir = Directory.GetCurrentDirectory();
            while (dir != null && !File.Exists(Path.Combine(dir, "Scripts", "Const.cs")))
                dir = Directory.GetParent(dir)?.FullName;
            return dir ?? Directory.GetCurrentDirectory();
        }

        // ── Q-S151: Needs/Rates JSON deserialization contract ────────────────
        [Test] public void Case01_DataCs_DocumentsNeedsJsonBridgeContract() {
            var path = Path.Combine(RepoRoot(), "Scripts", "Model", "Data.cs");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("Q-S151"),
                "Q-S151: Data.cs Needs class must reference Q-S151 in its docstring.");
            Assert.That(text, Does.Contain("JsonExtensionData"),
                "Q-S151: Data.cs Needs/Rates docstring must document the " +
                "[JsonExtensionData] Phase 3 implementation pattern.");
            Assert.That(text, Does.Contain("FLAT object"),
                "Q-S151: docstring must clarify that JSON shape is flat, not wrapper.");
        }

        [Test] public void Case02_JsonCs_DocumentsPhase3DeserializeContract() {
            var path = Path.Combine(RepoRoot(), "Scripts", "JSON.cs");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Contain("Q-S151"),
                "Q-S151: Json.cs Parse method must reference Q-S151.");
            Assert.That(text, Does.Contain("custom"),
                "Q-S151: Json.cs must mention the custom JsonConverter " +
                "or [JsonExtensionData] requirement.");
        }

        // ── HALLUC #27 evidence: PersonaCache.GetComposed is just throw NI ───
        [Test] public void Case03_PersonaCache_GetComposedIsThrowNotImplOnly() {
            // Gemini Round 32 claimed PersonaCache.cs lines 91-97 contain:
            //   AnimoLog.Error(msg);
            //   throw new InvalidOperationException(msg);
            // Physical grep evidence: NO such lines exist in PersonaCache.cs.
            // The GetComposed method body is `throw new NotImplementedException();`.
            // This test guards against that hallucination by asserting the
            // current stub state and the ABSENCE of the claimed code.
            var path = Path.Combine(RepoRoot(), "Scripts", "PersonaCache.cs");
            Assert.That(File.Exists(path), Is.True);
            var text = File.ReadAllText(path);
            Assert.That(text, Does.Not.Contain("AnimoLog.Error(msg)"),
                "HALLUC #27 evidence: PersonaCache.cs must NOT contain " +
                "`AnimoLog.Error(msg)`. Q-S144 logging-responsibility contract: " +
                "PersonaCache throws only; logging is Agent.Awake's job.");
            Assert.That(text, Does.Not.Contain("FirstOrDefault"),
                "HALLUC #28 evidence: PersonaCache.cs must NOT call FirstOrDefault. " +
                "GetComposed is `throw new NotImplementedException();` in v0.1.5.");
            // Affirmative check: the stub state is preserved.
            Assert.That(text, Does.Contain("public static Persona GetComposed(string template_id)"),
                "Q-S111: PersonaCache.GetComposed signature must be preserved.");
        }

        // ── HALLUC #28 evidence: build cleanly without System.Linq ───────────
        [Test] public void Case04_Animo_BuildCleanly_NoSystemLinqNeeded() {
            // If PersonaCache.cs really used FirstOrDefault without
            // `using System.Linq;`, the build would emit CS1061. The fact
            // that all 419 tests + assembly build with 0 Error proves
            // FirstOrDefault is not used; HALLUC #28 is structurally
            // impossible while preserving 0-error build state.
            //
            // This test is informational — Round32ContractTests.Case03
            // already grep-asserts FirstOrDefault is absent. We add this
            // explicit reflection assertion as a second line of defense.
            var personaCacheType = typeof(Animo.PersonaCache);
            var getComposed = personaCacheType.GetMethod("GetComposed");
            Assert.That(getComposed, Is.Not.Null,
                "HALLUC #28 affirmative: PersonaCache.GetComposed exists and is loaded — " +
                "proves the type compiled successfully; no CS1061.");
            Assert.That(getComposed!.IsStatic, Is.True,
                "Q-S111: GetComposed must be static (PersonaCache is a static-method facade).");
        }
    }
}
