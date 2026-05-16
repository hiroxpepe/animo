// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using Animo.Model;

namespace Animo.Core {

    /// <summary>Severity of a validation issue.</summary>
    public enum Severity {
        Info,
        Warning,
        Error
    }

    /// <summary>A single validation issue (e.g. A025 cycle detected).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Issue {
        public string rule_id { get; set; } = "";
        public Severity severity { get; set; } = Severity.Error;
        public string message { get; set; } = "";
        public string? path { get; set; }

        public Issue() {}

        public Issue(string rule_id, Severity severity, string message, string? path = null) {
            this.rule_id = rule_id;
            this.severity = severity;
            this.message = message;
            this.path = path;
        }
    }

    /// <summary>Aggregate result of running the Validator over a Root.</summary>
    public class ValidationResult {
        public List<Issue> issues { get; set; } = new();

        // (v0.1.5, Q-S138) Phase 3 implementation contract for has_errors,
        // errors, warnings, infos:
        //
        // WRONG (O(N) on every query — do NOT do this):
        //   public bool has_errors => issues.Any(i => i.severity == Severity.Error);
        //   public IReadOnlyList<Issue> errors => issues.Where(...).ToList();
        //
        // CORRECT (O(1) per query, O(1) per AddIssue):
        //   Maintain three private backing lists:
        //     readonly List<Issue> _errors   = new();
        //     readonly List<Issue> _warnings = new();
        //     readonly List<Issue> _infos    = new();
        //   In Validate / ValidateStage2, emit via a helper:
        //     void Emit(Issue issue) {
        //         issues.Add(issue);
        //         switch (issue.severity) {
        //             case Severity.Error:   _errors.Add(issue);   break;
        //             case Severity.Warning: _warnings.Add(issue); break;
        //             default:               _infos.Add(issue);    break;
        //         }
        //     }
        //   Then the public properties are O(1) field reads:
        //     public bool has_errors  => _errors.Count > 0;
        //     public bool has_warnings => _warnings.Count > 0;
        //     public IReadOnlyList<Issue> errors   => _errors;
        //     public IReadOnlyList<Issue> warnings => _warnings;
        //     public IReadOnlyList<Issue> infos    => _infos;
        //   Merge also appends to the backing lists:
        //     public void Merge(ValidationResult other) {
        //         issues.AddRange(other.issues);
        //         _errors.AddRange(other._errors);  // etc.
        //     }
        // The Validator runs once at scene load — not a hot path — but
        // has_errors is queried in tight loops in tests (AssertResult helpers)
        // and every O(N) call there scales with total-issue count per run.

        public bool has_errors => false;    // (Q-S149) safe Phase 2 default; Phase 3: _errors.Count > 0
        public bool has_warnings => false;  // (Q-S149) safe Phase 2 default; Phase 3: _warnings.Count > 0
        // (v0.1.5, Q-S146 + Q-S149) Q-S146 fixed errors/warnings/infos to
        // return empty lists but left has_errors + has_warnings as throw NI
        // — an incomplete fix. Debugger Watch evaluates ALL get-only
        // properties; a single throw here fires for every ValidationResult
        // in scope. Q-S149 replaces both with `=> false` (Phase 2 default:
        // no issues yet). Phase 3: replace with O(1) backing-list reads
        // per Q-S138 design: `_errors.Count > 0` and `_warnings.Count > 0`.
        public IReadOnlyList<Issue> errors   => System.Array.Empty<Issue>();
        public IReadOnlyList<Issue> warnings => System.Array.Empty<Issue>();
        public IReadOnlyList<Issue> infos    => System.Array.Empty<Issue>();

        public bool HasRule(string rule_id) => throw new NotImplementedException();

        /// <summary>
        /// (v0.1.5, Q-S106) Severity-aware variant of HasRule. Returns
        /// true only if there is at least one Issue with BOTH the given
        /// rule_id AND the given severity.
        ///
        /// Pre-Q-S106 AssertResult.HasError(result, "A028") was a
        /// false-positive trap: it checked `has_errors` (any error
        /// from any rule) AND `HasRule("A028")` (severity-agnostic).
        /// If a JSON had error A005 + warning A028, both checks
        /// passed and "A028 is an error" pass-through asserted true,
        /// even though A028 had fired only as a Warning. Q-S106 lets
        /// AssertResult.HasError invoke `HasRuleWithSeverity(rule_id,
        /// Severity.Error)` so only an actual Error of that rule
        /// passes.
        /// </summary>
        public bool HasRuleWithSeverity(string rule_id, Severity severity)
            => throw new NotImplementedException();

        /// <summary>
        /// (v0.1.5, Q-S72) Merge another ValidationResult's issues into this
        /// one. Used by `PersonaCache.GetComposed` to integrate stage-2
        /// findings (A019/A025/A035/A036/A037/A038/A039) — produced by
        /// `Validator.ValidateStage2(composed)` per template — into the
        /// session-wide ValidationResult populated by Initialize-time
        /// `Validator.Validate(root)`. Pre-Q-S72 §11.6.1 called
        /// `_validation!.Merge(stage2)` but no such method existed —
        /// confirmed missing-method compile error. Phase 3 implements as
        /// `this.issues.AddRange(other.issues)` (idempotent on the merged
        /// `other`; mutates `this`).
        /// </summary>
        public void Merge(ValidationResult other) => throw new NotImplementedException();
    }

    /// <summary>animo.json validator implementing rules A000–A039.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Validator {
        /// <summary>
        /// Stage 1 — per raw Persona/Kind/Root. Validates A000-A018,
        /// A020-A024, A026-A034, A038 tier-out-of-range — all rules
        /// that operate on the JSON shape before Composer runs.
        /// </summary>
        public static ValidationResult Validate(Root root) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// (v0.1.5, Q-S29 + Q-S39 + Q-S41 + Q-S47 + Q-S49 + Q-S57 + Q-S71 + Q-S113 + Q-S119)
        /// Stage 2 — per composed Persona. Runs A019 (typo check vs
        /// composed needs_meta — Q-S39), A025 (composed cycle), A035
        /// (post-fill trigger>reset), A036 (composed `actions[]`
        /// non-empty), A037 (multi-edge same target — Warning), A038's
        /// "needs_meta orphan" check (Q-S41 + Q-S49 + Q-S57 — Need not
        /// used in composed needs/actions/influences/thresholds/rates),
        /// A039 (sibling threshold proximity Warning — Q-S47, fires
        /// when two thresholds on the same Need have triggers within
        /// 1.0f, inclusive — Q-S122; Phase 3 pseudocode:
        ///   `if (diff &lt;= 1.0f + SIBLING_THRESHOLD_EPSILON)`
        ///   `const float SIBLING_THRESHOLD_EPSILON = 0.001f;`
        /// Q-S135: pre-Q-S135 the pseudocode used bare `&lt;= 1.0f`;
        /// a non-integer Threshold value (e.g. `fear: 79.3`) parsed
        /// as float32 can drift by ~1e-5; `79.3f - 78.3f` may resolve
        /// to `1.0001f &gt; 1.0f`, causing A039 to miss the pair.
        /// SIBLING_THRESHOLD_EPSILON = 0.001f adds three orders of
        /// magnitude above float32 drift (~1e-5) while staying far
        /// below the THRESHOLD_KEY_EPSILON = 0.01f merge window —
        /// same design reasoning as Q-S47's THRESHOLD_KEY_EPSILON.),
        /// and A040 (composed `actions[].id` uniqueness —
        /// Q-S113, Error). A038's tier-out-of-range remains a Stage 1
        /// Error.
        /// Called by `PersonaCache.GetComposed` (Q-S29) and merged into
        /// the Initialize-time ValidationResult via `ValidationResult.
        /// Merge` (Q-S72). Pre-Q-S71 §11.6.1's call site referenced
        /// this method but no declaration existed — confirmed missing-
        /// method compile error.
        ///
        /// (v0.1.5, Q-S119) A040 was added to the listing here. Q-S113
        /// (Phase_2_4_23) added the rule to spec §13 and updated the
        /// §17 Layout annotation to A000-A040, but missed this XML
        /// docstring — the Q-S101 NEW LAYER review caught all 14
        /// `Scripts/*.cs` files for spec ↔ file synchronization but
        /// did NOT scan within docstrings of those files for stage-2
        /// rule-listing currency. Q-S119 is the spec/file-content
        /// /docstring three-way sync that closes the gap. Process
        /// upgrade: any new Validator rule (Q-S113-style) must
        /// trigger an additional grep for `ValidateStage2` docstring
        /// listings in this file.
        /// </summary>
        public static ValidationResult ValidateStage2(Persona composed) {
            throw new NotImplementedException();
        }
    }
}
