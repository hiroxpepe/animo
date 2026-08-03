// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Animo.Model;

namespace Animo.Core {

    /// <summary>Severity of a validation issue.</summary>
    public enum Severity { Info, Warning, Error }

    /// <summary>A single validation issue (e.g. A025 cycle detected).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [Serializable]
    public class Issue {
        public Issue() {}
        public Issue(string rule_id, Severity severity, string message, string? path = null) {
            this.rule_id  = rule_id;
            this.severity = severity;
            this.message  = message;
            this.path     = path;
        }
        public string    rule_id  { get; set; } = "";
        public Severity  severity { get; set; } = Severity.Error;
        public string    message  { get; set; } = "";
        public string?   path     { get; set; }
    }

    /// <summary>Aggregate result of running the Validator over a Root.</summary>
    [Serializable]
    public class ValidationResult {
        // (Q-S138) O(1) backing lists. O(1) per query for has_errors, errors, warnings, infos.
        // (Q-S119) Validator implements full A000-A040 rule set.
        readonly List<Issue> _errors   = new();
        readonly List<Issue> _warnings = new();
        readonly List<Issue> _infos    = new();

        // Authoritative flat list (all issues).
        public List<Issue> issues { get; set; } = new();

        // (Q-S149) Safe bool defaults → Phase 3 O(1) reads.
        public bool has_errors   => _errors.Count   > 0;
        public bool has_warnings => _warnings.Count > 0;

        // (Q-S146) Safe list defaults.
        public IReadOnlyList<Issue> errors   => _errors;
        public IReadOnlyList<Issue> warnings => _warnings;
        public IReadOnlyList<Issue> infos    => _infos;

        public bool HasRule(string rule_id) =>
            issues.Any(i => i.rule_id == rule_id);

        public bool HasRuleWithSeverity(string rule_id, Severity severity) =>
            issues.Any(i => i.rule_id == rule_id && i.severity == severity);

        // (Q-S72) Merge another ValidationResult's issues into this one.
        public void Merge(ValidationResult other) {
            foreach (var issue in other.issues) Add(issue);
        }

        // Helper used by Validator internally.
        internal void Add(Issue issue) {
            issues.Add(issue);
            switch (issue.severity) {
                case Severity.Error:   _errors.Add(issue);   break;
                case Severity.Warning: _warnings.Add(issue); break;
                default:               _infos.Add(issue);    break;
            }
        }
    }

    /// <summary>animo.json validator implementing rules A000–A040.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Validator {

        internal const float SIBLING_THRESHOLD_EPSILON = 0.001f;

        const int MAX_ID_LEN = 128;

        static readonly Regex SNAKE_CASE = new(@"^[a-z][a-z0-9]*(_[a-z0-9]+)*$", RegexOptions.Compiled);

        // ─── Stage 1 ──────────────────────────────────────────────────────────

        public static ValidationResult Validate(Root root) {
            var result = new ValidationResult();
            var emit   = (Issue i) => result.Add(i);

            // A000: schema_version exists and is not empty.
            if (string.IsNullOrEmpty(root.schema_version))
                emit(new Issue("A000", Severity.Error, "schema_version is missing or empty.", "schema_version"));
            else if (!Const.SUPPORTED_SCHEMA_VERSIONS.Contains(root.schema_version))
                emit(new Issue("A021", Severity.Error,
                    $"schema_version '{root.schema_version}' is not supported. Expected one of: {string.Join(", ", Const.SUPPORTED_SCHEMA_VERSIONS)}.",
                    "schema_version"));

            // A001: personas exists and is not empty.
            if (root.personas == null || root.personas.Count == 0)
                emit(new Issue("A001", Severity.Error, "personas array is missing or empty.", "personas"));

            // Validate each kind.
            var kind_ids_seen = new Dictionary<string, int>();
            for (int ki = 0; ki < root.kinds.Count; ki++) {
                var kind = root.kinds[ki];
                // A003: kind_id snake_case, not empty, unique, ≤128.
                if (string.IsNullOrEmpty(kind.kind_id)) {
                    emit(new Issue("A003", Severity.Error, $"kinds[{ki}].kind_id is empty.", $"kinds[{ki}].kind_id"));
                } else {
                    if (kind.kind_id.Length > MAX_ID_LEN)
                        emit(new Issue("A003", Severity.Error,
                            $"kinds[{ki}].kind_id exceeds {MAX_ID_LEN} chars (A018 merged into A003).", $"kinds[{ki}].kind_id"));
                    else if (!SNAKE_CASE.IsMatch(kind.kind_id))
                        emit(new Issue("A003", Severity.Error,
                            $"kinds[{ki}].kind_id '{kind.kind_id}' is not snake_case.", $"kinds[{ki}].kind_id"));
                    if (kind_ids_seen.ContainsKey(kind.kind_id))
                        emit(new Issue("A003", Severity.Error,
                            $"kinds[{ki}].kind_id '{kind.kind_id}' is not unique.", $"kinds[{ki}].kind_id"));
                    else kind_ids_seen[kind.kind_id] = ki;
                }
                validateKindFields(kind, ki, emit);
            }

            // Validate each persona.
            var persona_ids_seen = new Dictionary<string, int>();
            for (int pi = 0; pi < root.personas.Count; pi++) {
                var persona = root.personas[pi];
                // A002: agent_id snake_case, not empty, unique, ≤128.
                if (string.IsNullOrEmpty(persona.agent_id)) {
                    emit(new Issue("A002", Severity.Error, $"personas[{pi}].agent_id is empty.", $"personas[{pi}].agent_id"));
                } else {
                    if (persona.agent_id.Length > MAX_ID_LEN)
                        emit(new Issue("A002", Severity.Error,
                            $"personas[{pi}].agent_id exceeds {MAX_ID_LEN} chars (A018 merged into A002).", $"personas[{pi}].agent_id"));
                    else if (!SNAKE_CASE.IsMatch(persona.agent_id))
                        emit(new Issue("A002", Severity.Error,
                            $"personas[{pi}].agent_id '{persona.agent_id}' is not snake_case.", $"personas[{pi}].agent_id"));
                    if (persona_ids_seen.ContainsKey(persona.agent_id))
                        emit(new Issue("A002", Severity.Error,
                            $"personas[{pi}].agent_id '{persona.agent_id}' is not unique.", $"personas[{pi}].agent_id"));
                    else persona_ids_seen[persona.agent_id] = pi;
                }
                validatePersonaFields(persona, pi, root, emit);
            }

            return result;
        }


        // ─── Stage 2 ──────────────────────────────────────────────────────────

        public static ValidationResult ValidateStage2(Persona composed) {
            var result = new ValidationResult();
            var emit   = (Issue i) => result.Add(i);
            string id  = composed.agent_id;

            // Collect "in use" Need names (5-site union per Q-S41+Q-S49+Q-S57+Q-S124).
            var in_use = new HashSet<string>();
            if (composed.needs      != null) foreach (var kv in composed.needs.values) in_use.Add(kv.Key);
            if (composed.actions    != null) foreach (var a   in composed.actions) in_use.Add(a.need);
            if (composed.influences != null) foreach (var inf in composed.influences) {
                in_use.Add(inf.source); in_use.Add(inf.target);
            }
            if (composed.binding    != null) foreach (var t in composed.binding.thresholds) in_use.Add(t.need);
            if (composed.rates      != null) foreach (var kv in composed.rates.values) in_use.Add(kv.Key);

            // A019: typo check — unknown Need in in_use vs standard + needs_meta.
            var known = new HashSet<string>(Const.STANDARD_NEEDS);
            if (composed.needs_meta != null) foreach (var kv in composed.needs_meta) known.Add(kv.Key);
            foreach (var need in in_use) {
                if (!known.Contains(need)) {
                    // Check for levenshtein proximity to standard names.
                    string? close = Const.STANDARD_NEEDS
                        .FirstOrDefault(s => levenshtein(need, s) <= 2);
                    if (close != null)
                        emit(new Issue("A019", Severity.Warning,
                            $"persona '{id}': Need '{need}' looks like a typo of '{close}'.",
                            $"persona.{need}"));
                }
            }

            // A025 stage 2: composed influences cycle.
            if (composed.influences != null && hasCycle(composed.influences))
                emit(new Issue("A025", Severity.Error,
                    $"persona '{id}': composed influences[] contains a cycle.", "influences"));

            // A035: after Composer fills reset_threshold, trigger > reset strictly.
            if (composed.binding != null)
                for (int i = 0; i < composed.binding.thresholds.Count; i++) {
                    var t = composed.binding.thresholds[i];
                    if (t.reset_threshold.HasValue && t.trigger_threshold <= t.reset_threshold.Value)
                        emit(new Issue("A035", Severity.Error,
                            $"persona '{id}' threshold[{i}]: trigger_threshold ({t.trigger_threshold}) ≤ reset_threshold ({t.reset_threshold.Value}) after fill.",
                            $"binding.thresholds[{i}]"));
                }

            // A036: composed actions[] must be non-empty.
            if (composed.actions == null || composed.actions.Count == 0)
                emit(new Issue("A036", Severity.Error,
                    $"persona '{id}': composed actions[] is empty — Engine Step 5 would throw.", "actions"));

            // A037: multiple influences writing to same target.
            if (composed.influences != null) {
                var targets = new Dictionary<string, int>();
                foreach (var inf in composed.influences) {
                    if (targets.ContainsKey(inf.target))
                        emit(new Issue("A037", Severity.Warning,
                            $"persona '{id}': multiple influences write to Need '{inf.target}' (order-dependent).", "influences"));
                    else targets[inf.target] = 1;
                }
            }

            // A038 Stage 2: needs_meta orphan — Need in meta but not in 5-site in_use union.
            if (composed.needs_meta != null)
                foreach (var kv in composed.needs_meta)
                    if (!in_use.Contains(kv.Key))
                        emit(new Issue("A038", Severity.Warning,
                            $"persona '{id}': needs_meta['{kv.Key}'] references a Need not in use (orphan).",
                            $"needs_meta['{kv.Key}']"));

            // A039: sibling threshold proximity Warning (Q-S47 + Q-S122 inclusive).
            if (composed.binding != null) {
                var by_need = composed.binding.thresholds.GroupBy(t => t.need);
                foreach (var group in by_need) {
                    var sorted = group.OrderBy(t => t.trigger_threshold).ToList();
                    for (int i = 0; i < sorted.Count - 1; i++) {
                        float diff = sorted[i + 1].trigger_threshold - sorted[i].trigger_threshold;
                        if (diff <= 1.0f + SIBLING_THRESHOLD_EPSILON)
                            emit(new Issue("A039", Severity.Warning,
                                $"persona '{id}': sibling thresholds on '{group.Key}' at {sorted[i].trigger_threshold} and {sorted[i+1].trigger_threshold} are within 1.0f of each other.",
                                "binding.thresholds"));
                    }
                }
            }

            // A040: composed actions[].id must be unique.
            if (composed.actions != null) {
                var ids_seen = new HashSet<string>();
                foreach (var act in composed.actions)
                    if (!ids_seen.Add(act.id))
                        emit(new Issue("A040", Severity.Error,
                            $"persona '{id}': composed actions[].id '{act.id}' is not unique.",
                            "actions"));
            }

            return result;
        }

        static void validateKindFields(Kind kind, int ki, Action<Issue> emit) {
            string kp = $"kinds[{ki}]";
            if (kind.rates != null)       validateRates(kind.rates, $"{kp}.rates", emit);
            if (kind.suppression != null) validateSuppression(kind.suppression, $"{kp}.suppression", emit);
            if (kind.influences != null)  validateInfluences(kind.influences, $"{kp}.influences", emit);
            if (kind.actions != null)     validateActions(kind.actions, $"{kp}.actions", emit);
            if (kind.commitment != null)  validateCommitment(kind.commitment, $"{kp}.commitment", emit);
            if (kind.binding != null)     validateBinding(kind.binding, $"{kp}.binding", emit);
            if (kind.needs_meta != null)  validateNeedsMeta(kind.needs_meta, $"{kp}.needs_meta", emit);
            // A025 stage 1: influence cycle in raw kind.
            if (kind.influences != null && hasCycle(kind.influences))
                emit(new Issue("A025", Severity.Error,
                    $"kinds[{ki}] has a cycle in influences[].", $"{kp}.influences"));
        }

        static void validatePersonaFields(Persona persona, int pi, Root root, Action<Issue> emit) {
            string pp = $"personas[{pi}]";
            // A004: all kind_ids exist in kinds.
            if (persona.kind_ids != null) {
                var kind_id_set = new HashSet<string>(root.kinds.Select(k => k.kind_id));
                for (int i = 0; i < persona.kind_ids.Count; i++) {
                    if (!kind_id_set.Contains(persona.kind_ids[i]))
                        emit(new Issue("A004", Severity.Error,
                            $"personas[{pi}].kind_ids[{i}] '{persona.kind_ids[i]}' not found in kinds.",
                            $"{pp}.kind_ids[{i}]"));
                }
                // A033: duplicate kind_ids.
                var seen = new HashSet<string>();
                foreach (var kid in persona.kind_ids)
                    if (!seen.Add(kid))
                        emit(new Issue("A033", Severity.Warning,
                            $"personas[{pi}].kind_ids contains duplicate '{kid}'. Composer keeps last occurrence.",
                            $"{pp}.kind_ids"));
            }
            // A011a: no kind_ids → must have at least one action.
            if ((persona.kind_ids == null || persona.kind_ids.Count == 0) &&
                (persona.actions  == null || persona.actions.Count  == 0))
                emit(new Issue("A011a", Severity.Error,
                    $"personas[{pi}] has no kind_ids and no actions (at least one action required).",
                    $"{pp}.actions"));
            if (persona.needs      != null) validateNeeds(persona.needs, $"{pp}.needs", emit);
            if (persona.rates      != null) validateRates(persona.rates, $"{pp}.rates", emit);
            if (persona.suppression!= null) validateSuppression(persona.suppression, $"{pp}.suppression", emit);
            if (persona.influences != null) validateInfluences(persona.influences, $"{pp}.influences", emit);
            if (persona.actions    != null) validateActions(persona.actions, $"{pp}.actions", emit);
            if (persona.commitment != null) validateCommitment(persona.commitment, $"{pp}.commitment", emit);
            if (persona.binding    != null) validateBinding(persona.binding, $"{pp}.binding", emit);
            if (persona.needs_meta != null) validateNeedsMeta(persona.needs_meta, $"{pp}.needs_meta", emit);
            // A013: rates keys are subset of needs keys.
            if (persona.rates != null && persona.needs != null) {
                foreach (var rk in persona.rates.values.Keys)
                    if (!persona.needs.values.ContainsKey(rk))
                        emit(new Issue("A013", Severity.Warning,
                            $"{pp}: rates key '{rk}' is not in needs.", $"{pp}.rates"));
            }

            // A020a/b/c: cross-kind checks (kind fields referencing Needs not in referencing Persona.needs).
            if (persona.kind_ids != null && persona.needs != null) {
                var persona_needs = new HashSet<string>(persona.needs.values.Keys);
                foreach (var kid in persona.kind_ids) {
                    var kind = findKind(root, kid);
                    if (kind == null) continue;
                    // A020a: kind.rates key not in persona.needs.
                    if (kind.rates != null)
                        foreach (var rk in kind.rates.values.Keys)
                            if (!persona_needs.Contains(rk))
                                emit(new Issue("A020a", Severity.Warning,
                                    $"{pp}: kind '{kid}' rates key '{rk}' not in persona.needs.", $"{pp}.kind_ids"));
                    // A020b: kind.influences source/target not in persona.needs.
                    if (kind.influences != null)
                        foreach (var inf in kind.influences) {
                            if (!persona_needs.Contains(inf.source))
                                emit(new Issue("A020b", Severity.Warning,
                                    $"{pp}: kind '{kid}' influence source '{inf.source}' not in persona.needs.", $"{pp}.kind_ids"));
                            if (!persona_needs.Contains(inf.target))
                                emit(new Issue("A020b", Severity.Warning,
                                    $"{pp}: kind '{kid}' influence target '{inf.target}' not in persona.needs.", $"{pp}.kind_ids"));
                        }
                    // A020c: kind.actions[].need not in persona.needs.
                    if (kind.actions != null)
                        foreach (var act in kind.actions)
                            if (!persona_needs.Contains(act.need))
                                emit(new Issue("A020c", Severity.Warning,
                                    $"{pp}: kind '{kid}' action '{act.id}' need '{act.need}' not in persona.needs.", $"{pp}.kind_ids"));
                }
            }
                emit(new Issue("A016", Severity.Warning,
                    $"personas[{pi}] has no binding. Composer will fill defaults.",
                    $"{pp}.binding"));
            // A025 stage 1: cycle in raw persona influences.
            if (persona.influences != null && hasCycle(persona.influences))
                emit(new Issue("A025", Severity.Error,
                    $"personas[{pi}] has a cycle in influences[].",
                    $"{pp}.influences"));
            // A029: commitment omitted but 2+ actions.
            if (persona.commitment == null && persona.actions != null && persona.actions.Count >= 2)
                emit(new Issue("A029", Severity.Warning,
                    $"personas[{pi}] has {persona.actions.Count} actions but no commitment (chattering risk).",
                    $"{pp}.commitment"));
            // A030: no actions or influences use frustration.
            bool uses_frustration =
                (persona.actions    != null && persona.actions.Any(a => a.need == "frustration")) ||
                (persona.influences != null && persona.influences.Any(i =>
                    i.source == "frustration" || i.target == "frustration"));
            if (!uses_frustration && (persona.actions != null || persona.influences != null))
                emit(new Issue("A030", Severity.Warning,
                    $"personas[{pi}]: no actions or influences use 'frustration' (feedback loop may be missing).",
                    $"{pp}"));
            // A032: no fallback low-tier action other than idle.
            if (persona.actions != null) {
                bool has_fallback = persona.actions.Any(a => a.need != "idle" && a.tier <= 2);
                if (!has_fallback && persona.actions.Any())
                    emit(new Issue("A032", Severity.Info,
                        $"personas[{pi}]: no low-tier fallback action besides idle.",
                        $"{pp}.actions"));
            }
        }

        static void validateNeeds(Needs needs, string path, Action<Issue> emit) {
            // A005: all values in [0, 100]. NaN and Infinity are also errors.
            foreach (var kv in needs.values)
                if (float.IsNaN(kv.Value) || float.IsInfinity(kv.Value) ||
                    kv.Value < 0f || kv.Value > 100f)
                    emit(new Issue("A005", Severity.Error,
                        $"{path}['{kv.Key}'] = {kv.Value} is outside [0, 100] (or NaN/Infinity).", path));
        }

        static void validateRates(Rates rates, string path, Action<Issue> emit) {
            // (#8) Rates values are unrestricted in range per spec (positive or negative),
            // but NaN/Infinity would corrupt every Step 1 decay (need = NaN*dt → NaN),
            // silently destroying the agent state. Reject as A005-class Error.
            foreach (var kv in rates.values) {
                if (float.IsNaN(kv.Value) || float.IsInfinity(kv.Value))
                    emit(new Issue("A005", Severity.Error,
                        $"{path}['{kv.Key}'] = {kv.Value} is NaN or Infinity.", path));
            }
        }

        static void validateSuppression(Suppression sup, string path, Action<Issue> emit) {
            // A006: suppression values 0.0 to 1.0.
            foreach (var (name, val) in new[] {
                ("tier2", sup.tier2), ("tier3", sup.tier3), ("tier4", sup.tier4), ("tier5", sup.tier5) }) {
                if (val < 0f || val > 1f)
                    emit(new Issue("A006", Severity.Error,
                        $"{path}.{name} = {val} is outside [0.0, 1.0].", $"{path}.{name}"));
            }
        }

        static void validateInfluences(List<Influence> infs, string path, Action<Issue> emit) {
            // A012: coefficient in [-1, 1]. NaN/Infinity also invalid.
            for (int i = 0; i < infs.Count; i++) {
                var c = infs[i].coefficient;
                if (float.IsNaN(c) || float.IsInfinity(c) || c < -1f || c > 1f)
                    emit(new Issue("A012", Severity.Error,
                        $"{path}[{i}].coefficient = {c} is outside [-1.0, 1.0] (or NaN/Infinity).",
                        $"{path}[{i}].coefficient"));
            }
        }

        static void validateActions(List<Animo.Model.Action> acts, string path, Action<Issue> emit) {
            for (int i = 0; i < acts.Count; i++) {
                var act = acts[i];
                string ap = $"{path}[{i}]";
                // A009: id not empty.
                if (string.IsNullOrEmpty(act.id))
                    emit(new Issue("A009", Severity.Error, $"{ap}.id is empty.", $"{ap}.id"));
                // A007: tier 1-5.
                if (act.tier < 1 || act.tier > 5)
                    emit(new Issue("A007", Severity.Error,
                        $"{ap}.tier = {act.tier} is outside [1, 5].", $"{ap}.tier"));
                // A008: exponent 0.1-5.0.
                if (act.exponent < 0.1f || act.exponent > 5.0f)
                    emit(new Issue("A008", Severity.Error,
                        $"{ap}.exponent = {act.exponent} is outside [0.1, 5.0].", $"{ap}.exponent"));
                // A022: need is required.
                if (string.IsNullOrEmpty(act.need))
                    emit(new Issue("A022", Severity.Error, $"{ap}.need is required.", $"{ap}.need"));
                // A024: if need is 'idle', tier should be 5.
                if (act.need == "idle" && act.tier != 5)
                    emit(new Issue("A024", Severity.Warning,
                        $"{ap}: action uses 'idle' Need but tier is {act.tier} (should be 5).", $"{ap}.tier"));
            }
        }

        static void validateCommitment(Commitment c, string path, Action<Issue> emit) {
            // A028: bonus < 0 Error; bonus > 30 Warning; ceiling at 50.
            if (c.bonus < 0f)
                emit(new Issue("A028", Severity.Error,
                    $"{path}.bonus = {c.bonus} is negative (must be ≥ 0).", $"{path}.bonus"));
            else if (c.bonus > 50f)
                emit(new Issue("A028", Severity.Error,
                    $"{path}.bonus = {c.bonus} exceeds ceiling of 50.", $"{path}.bonus"));
            else if (c.bonus > 30f)
                emit(new Issue("A028", Severity.Warning,
                    $"{path}.bonus = {c.bonus} exceeds 30 (lock-in risk).", $"{path}.bonus"));
        }

        static void validateBinding(Binding binding, string path, Action<Issue> emit) {
            // A014: on_action_change placeholders.
            if (binding.on_action_change != null)
                checkPlaceholders(binding.on_action_change, "A014", path + ".on_action_change",
                    Const.TEMPLATE_PLACEHOLDERS_ACTION, emit);
            // A010, A015, A023, A034 on thresholds.
            for (int i = 0; i < binding.thresholds.Count; i++) {
                var t  = binding.thresholds[i];
                string tp = $"{path}.thresholds[{i}]";
                // A010: trigger_threshold in (0, 100].
                if (t.trigger_threshold <= 0f || t.trigger_threshold > 100f)
                    emit(new Issue("A010", Severity.Error,
                        $"{tp}.trigger_threshold = {t.trigger_threshold} is outside (0, 100].", tp));
                // A023: trigger > reset (if reset provided).
                if (t.reset_threshold.HasValue && t.trigger_threshold <= t.reset_threshold.Value)
                    emit(new Issue("A023", Severity.Error,
                        $"{tp}: trigger_threshold ({t.trigger_threshold}) must be > reset_threshold ({t.reset_threshold.Value}).", tp));
                // A034: reset_threshold < 0 Error.
                if (t.reset_threshold.HasValue && t.reset_threshold.Value < 0f)
                    emit(new Issue("A034", Severity.Error,
                        $"{tp}.reset_threshold = {t.reset_threshold.Value} is negative.", tp));
                // A015: trigger placeholders.
                if (!string.IsNullOrEmpty(t.trigger))
                    checkPlaceholders(t.trigger, "A015", $"{tp}.trigger",
                        Const.TEMPLATE_PLACEHOLDERS_THRESHOLD, emit);
            }
            // (#9) A039 Stage 1: sibling threshold proximity in RAW JSON.
            // Without this, Composer.mergeThresholds (EPSILON=0.01) would collapse
            // duplicates BEFORE Stage 2 A039 ran, silently hiding author mistakes.
            // Stage 1 sees the raw list and warns the author directly.
            var by_need = binding.thresholds.GroupBy(t => t.need);
            foreach (var group in by_need) {
                var sorted = group.OrderBy(t => t.trigger_threshold).ToList();
                for (int i = 0; i < sorted.Count - 1; i++) {
                    float diff = sorted[i + 1].trigger_threshold - sorted[i].trigger_threshold;
                    if (diff <= 1.0f + SIBLING_THRESHOLD_EPSILON)
                        emit(new Issue("A039", Severity.Warning,
                            $"{path}: sibling thresholds on '{group.Key}' at {sorted[i].trigger_threshold} and {sorted[i+1].trigger_threshold} are within 1.0f of each other.",
                            $"{path}.thresholds"));
                }
            }
        }

        static void validateNeedsMeta(Dictionary<string, NeedMeta> meta, string path, Action<Issue> emit) {
            // A038 Stage 1: tier out of range → Error.
            // A038 Stage 1: standard Need tier mismatch → Warning.
            foreach (var kv in meta) {
                if (kv.Value.tier < 1 || kv.Value.tier > 5)
                    emit(new Issue("A038", Severity.Error,
                        $"{path}['{kv.Key}'].tier = {kv.Value.tier} is outside [1, 5].", path));
                else if (Const.NEED_TIER_BY_NAME.TryGetValue(kv.Key, out var expected_tier) &&
                         kv.Value.tier != expected_tier)
                    emit(new Issue("A038", Severity.Warning,
                        $"{path}['{kv.Key}'].tier = {kv.Value.tier} overrides standard tier {expected_tier} (spec §3.5 value wins).", path));
            }
        }

        static void checkPlaceholders(string template, string rule_id, string path,
                                       IReadOnlyList<string> allowed, Action<Issue> emit) {
            var found = Regex.Matches(template, @"\{([^}]+)\}");
            foreach (Match m in found) {
                string key = m.Groups[1].Value;
                if (!allowed.Contains(key))
                    emit(new Issue(rule_id, Severity.Error,
                        $"{path}: placeholder '{{{key}}}' is not allowed. Allowed: {{{string.Join("}, {", allowed)}}}.",
                        path));
            }
        }

        // ─── Cycle detection ─────────────────────────────────────────────────

        static Kind? findKind(Root root, string kind_id) {
            foreach (var k in root.kinds) if (k.kind_id == kind_id) return k;
            return null;
        }

        static bool hasCycle(List<Influence> influences) {
            // Build adjacency.
            var graph = new Dictionary<string, List<string>>();
            foreach (var inf in influences) {
                if (!graph.ContainsKey(inf.source)) graph[inf.source] = new List<string>();
                graph[inf.source].Add(inf.target);
            }
            var visited = new HashSet<string>();
            var in_stack = new HashSet<string>();
            bool dfs(string node) {
                if (in_stack.Contains(node)) return true;
                if (visited.Contains(node))  return false;
                visited.Add(node); in_stack.Add(node);
                if (graph.TryGetValue(node, out var neighbors))
                    foreach (var n in neighbors) if (dfs(n)) return true;
                in_stack.Remove(node);
                return false;
            }
            foreach (var node in graph.Keys) if (dfs(node)) return true;
            return false;
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        static int levenshtein(string a, string b) {
            int m = a.Length, n = b.Length;
            var d = new int[m + 1, n + 1];
            for (int i = 0; i <= m; i++) d[i, 0] = i;
            for (int j = 0; j <= n; j++) d[0, j] = j;
            for (int i = 1; i <= m; i++)
                for (int j = 1; j <= n; j++) {
                    int cost = a[i-1] == b[j-1] ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i-1, j] + 1, d[i, j-1] + 1), d[i-1, j-1] + cost);
                }
            return d[m, n];
        }
    }
}
