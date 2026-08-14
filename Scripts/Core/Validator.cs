// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Animo.Model;

namespace Animo.Core {
    ///////////////////////////////////////////////////////////////////////////////////////////////////
    // public Enums [noun]

    /// <summary>Severity of a validation issue.</summary>
    public enum Severity { Info, Warning, Error }

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    // public Classes

    /// <summary>A single validation issue (e.g. A025 cycle detected).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [Serializable]
    public class Issue {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Issue() {}
        public Issue(string rule_id, Severity severity, string message, string? path = null) {
            this.rule_id  = rule_id;
            this.severity = severity;
            this.message  = message;
            this.path     = path;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

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

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        readonly List<Issue> _errors   = new();
        readonly List<Issue> _warnings = new();
        readonly List<Issue> _infos    = new();

        // Authoritative flat list (all issues).

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        public List<Issue> issues { get; set; } = new();

        // (Q-S149) Safe bool defaults → Phase 3 O(1) reads.
        public bool has_errors   => _errors.Count   > 0;
        public bool has_warnings => _warnings.Count > 0;

        // (Q-S146) Safe list defaults.
        public IReadOnlyList<Issue> errors   => _errors;
        public IReadOnlyList<Issue> warnings => _warnings;
        public IReadOnlyList<Issue> infos    => _infos;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public bool HasRule(string rule_id) =>
            issues.Any(i => i.rule_id == rule_id);

        public bool HasRuleWithSeverity(string rule_id, Severity severity) =>
            issues.Any(i => i.rule_id == rule_id && i.severity == severity);

        // (Q-S72) Merge another ValidationResult's issues into this one.
        public void Merge(ValidationResult other) {
            foreach (var issue in other.issues) Add(issue: issue);
        }

        // Helper used by Validator internally.

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // internal Methods [verb]

        internal void Add(Issue issue) {
            issues.Add(issue);
            switch (issue.severity) {
                case Severity.Error:   _errors.Add(issue);   break;
                case Severity.Warning: _warnings.Add(issue); break;
                default:               _infos.Add(issue);    break;
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    // public Classes

    /// <summary>animo.json validator implementing rules A000–A040.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Validator {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Const [nouns]

        const int MAX_ID_LEN = 128;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Const [nouns]

        internal const float SIBLING_THRESHOLD_EPSILON = 0.001f;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Fields

        static readonly Regex SNAKE_CASE = new(@"^[a-z][a-z0-9]*(_[a-z0-9]+)*$", RegexOptions.Compiled);

        // ─── Stage 1 ──────────────────────────────────────────────────────────

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static ValidationResult Validate(Root root) {
            var result = new ValidationResult();
            var emit   = (Issue i) => result.Add(issue: i);

            // A000: schema_version exists and is not empty.
            if (string.IsNullOrEmpty(root.schema_version))
                emit(new Issue(rule_id: "A000", severity: Severity.Error, message: "schema_version is missing or empty.", path: "schema_version"));
            else if (!Const.SUPPORTED_SCHEMA_VERSIONS.Contains(root.schema_version))
                emit(new Issue(rule_id: "A021", severity: Severity.Error,
                    message: $"schema_version '{root.schema_version}' is not supported. Expected one of: {string.Join(", ", Const.SUPPORTED_SCHEMA_VERSIONS)}.",
                    path: "schema_version"));

            // A001: personas exists and is not empty.
            if (root.personas == null || root.personas.Count == 0)
                emit(new Issue(rule_id: "A001", severity: Severity.Error, message: "personas array is missing or empty.", path: "personas"));

            // Validate each kind.
            var kind_ids_seen = new Dictionary<string, int>();
            for (int kind_index = 0; kind_index < root.kinds.Count; kind_index++) {
                var kind = root.kinds[kind_index];
                // A003: kind_id snake_case, not empty, unique, ≤128.
                if (string.IsNullOrEmpty(kind.kind_id)) {
                    emit(new Issue(rule_id: "A003", severity: Severity.Error, message: $"kinds[{kind_index}].kind_id is empty.", path: $"kinds[{kind_index}].kind_id"));
                } else {
                    if (kind.kind_id.Length > MAX_ID_LEN)
                        emit(new Issue(rule_id: "A003", severity: Severity.Error,
                            message: $"kinds[{kind_index}].kind_id exceeds {MAX_ID_LEN} chars (A018 merged into A003).", path: $"kinds[{kind_index}].kind_id"));
                    else if (!SNAKE_CASE.IsMatch(kind.kind_id))
                        emit(new Issue(rule_id: "A003", severity: Severity.Error,
                            message: $"kinds[{kind_index}].kind_id '{kind.kind_id}' is not snake_case.", path: $"kinds[{kind_index}].kind_id"));
                    if (kind_ids_seen.ContainsKey(kind.kind_id))
                        emit(new Issue(rule_id: "A003", severity: Severity.Error,
                            message: $"kinds[{kind_index}].kind_id '{kind.kind_id}' is not unique.", path: $"kinds[{kind_index}].kind_id"));
                    else kind_ids_seen[kind.kind_id] = kind_index;
                }
                validateKindFields(kind: kind, kind_index: kind_index, emit: emit);
            }

            // Validate each persona.
            var persona_ids_seen = new Dictionary<string, int>();
            for (int persona_index = 0; persona_index < root.personas.Count; persona_index++) {
                var persona = root.personas[persona_index];
                // A002: agent_id snake_case, not empty, unique, ≤128.
                if (string.IsNullOrEmpty(persona.agent_id)) {
                    emit(new Issue(rule_id: "A002", severity: Severity.Error, message: $"personas[{persona_index}].agent_id is empty.", path: $"personas[{persona_index}].agent_id"));
                } else {
                    if (persona.agent_id.Length > MAX_ID_LEN)
                        emit(new Issue(rule_id: "A002", severity: Severity.Error,
                            message: $"personas[{persona_index}].agent_id exceeds {MAX_ID_LEN} chars (A018 merged into A002).", path: $"personas[{persona_index}].agent_id"));
                    else if (!SNAKE_CASE.IsMatch(persona.agent_id))
                        emit(new Issue(rule_id: "A002", severity: Severity.Error,
                            message: $"personas[{persona_index}].agent_id '{persona.agent_id}' is not snake_case.", path: $"personas[{persona_index}].agent_id"));
                    if (persona_ids_seen.ContainsKey(persona.agent_id))
                        emit(new Issue(rule_id: "A002", severity: Severity.Error,
                            message: $"personas[{persona_index}].agent_id '{persona.agent_id}' is not unique.", path: $"personas[{persona_index}].agent_id"));
                    else persona_ids_seen[persona.agent_id] = persona_index;
                }
                validatePersonaFields(persona: persona, persona_index: persona_index, root: root, emit: emit);
            }

            return result;
        }

        // ─── Stage 2 ──────────────────────────────────────────────────────────

        public static ValidationResult ValidateStage2(Persona composed) {
            var result = new ValidationResult();
            var emit   = (Issue i) => result.Add(issue: i);
            string id  = composed.agent_id;

            // Collect "in use" Need names (5-site union per Q-S41+Q-S49+Q-S57+Q-S124).
            var in_use = new HashSet<string>();
            if (composed.needs      != null) foreach (var entry in composed.needs.values) in_use.Add(entry.Key);
            if (composed.actions    != null) foreach (var action   in composed.actions) in_use.Add(action.need);
            if (composed.influences != null) foreach (var influence in composed.influences) {
                in_use.Add(influence.source); in_use.Add(influence.target);
            }
            if (composed.binding    != null) foreach (var threshold in composed.binding.thresholds) in_use.Add(threshold.need);
            if (composed.rates      != null) foreach (var entry in composed.rates.values) in_use.Add(entry.Key);

            // A019: typo check — unknown Need in in_use vs standard + needs_meta.
            var known = new HashSet<string>(Const.STANDARD_NEEDS);
            if (composed.needs_meta != null) foreach (var entry in composed.needs_meta) known.Add(entry.Key);
            foreach (var need in in_use) {
                if (!known.Contains(need)) {
                    // Check for levenshtein proximity to standard names.
                    string? close = Const.STANDARD_NEEDS
                        .FirstOrDefault(candidate => levenshtein(first: need, second: candidate) <= 2);
                    if (close != null)
                        emit(new Issue(rule_id: "A019", severity: Severity.Warning,
                            message: $"persona '{id}': Need '{need}' looks like a typo of '{close}'.",
                            path: $"persona.{need}"));
                }
            }

            // A025 stage 2: composed influences cycle.
            if (composed.influences != null && hasCycle(influences: composed.influences))
                emit(new Issue(rule_id: "A025", severity: Severity.Error,
                    message: $"persona '{id}': composed influences[] contains a cycle.", path: "influences"));

            // A035: after Composer fills reset_threshold, trigger > reset strictly.
            if (composed.binding != null)
                for (int i = 0; i < composed.binding.thresholds.Count; i++) {
                    var threshold = composed.binding.thresholds[i];
                    if (threshold.reset_threshold.HasValue && threshold.trigger_threshold <= threshold.reset_threshold.Value)
                        emit(new Issue(rule_id: "A035", severity: Severity.Error,
                            message: $"persona '{id}' threshold[{i}]: trigger_threshold ({threshold.trigger_threshold}) ≤ reset_threshold ({threshold.reset_threshold.Value}) after fill.",
                            path: $"binding.thresholds[{i}]"));
                }

            // A036: composed actions[] must be non-empty.
            if (composed.actions == null || composed.actions.Count == 0)
                emit(new Issue(rule_id: "A036", severity: Severity.Error,
                    message: $"persona '{id}': composed actions[] is empty — Engine Step 5 would throw.", path: "actions"));

            // A037: multiple influences writing to same target.
            if (composed.influences != null) {
                var targets = new Dictionary<string, int>();
                foreach (var influence in composed.influences) {
                    if (targets.ContainsKey(influence.target))
                        emit(new Issue(rule_id: "A037", severity: Severity.Warning,
                            message: $"persona '{id}': multiple influences write to Need '{influence.target}' (order-dependent).", path: "influences"));
                    else targets[influence.target] = 1;
                }
            }

            // A038 Stage 2: needs_meta orphan — Need in meta but not in 5-site in_use union.
            if (composed.needs_meta != null)
                foreach (var entry in composed.needs_meta)
                    if (!in_use.Contains(entry.Key))
                        emit(new Issue(rule_id: "A038", severity: Severity.Warning,
                            message: $"persona '{id}': needs_meta['{entry.Key}'] references a Need not in use (orphan).",
                            path: $"needs_meta['{entry.Key}']"));

            // A039: sibling threshold proximity Warning (Q-S47 + Q-S122 inclusive).
            if (composed.binding != null) {
                var by_need = composed.binding.thresholds.GroupBy(threshold => threshold.need);
                foreach (var group in by_need) {
                    var sorted = group.OrderBy(threshold => threshold.trigger_threshold).ToList();
                    for (int i = 0; i < sorted.Count - 1; i++) {
                        float difference = sorted[i + 1].trigger_threshold - sorted[i].trigger_threshold;
                        if (difference <= 1.0f + SIBLING_THRESHOLD_EPSILON)
                            emit(new Issue(rule_id: "A039", severity: Severity.Warning,
                                message: $"persona '{id}': sibling thresholds on '{group.Key}' at {sorted[i].trigger_threshold} and {sorted[i+1].trigger_threshold} are within 1.0f of each other.",
                                path: "binding.thresholds"));
                    }
                }
            }

            // A040: composed actions[].id must be unique.
            if (composed.actions != null) {
                var ids_seen = new HashSet<string>();
                foreach (var act in composed.actions)
                    if (!ids_seen.Add(act.id))
                        emit(new Issue(rule_id: "A040", severity: Severity.Error,
                            message: $"persona '{id}': composed actions[].id '{act.id}' is not unique.",
                            path: "actions"));
            }

            return result;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static void validateKindFields(Kind kind, int kind_index, Action<Issue> emit) {
            string kind_path = $"kinds[{kind_index}]";
            if (kind.rates != null)       validateRates(rates: kind.rates, path: $"{kind_path}.rates", emit: emit);
            if (kind.suppression != null) validateSuppression(suppression: kind.suppression, path: $"{kind_path}.suppression", emit: emit);
            if (kind.influences != null)  validateInfluences(influences: kind.influences, path: $"{kind_path}.influences", emit: emit);
            if (kind.actions != null)     validateActions(acts: kind.actions, path: $"{kind_path}.actions", emit: emit);
            if (kind.commitment != null)  validateCommitment(commitment: kind.commitment, path: $"{kind_path}.commitment", emit: emit);
            if (kind.binding != null)     validateBinding(binding: kind.binding, path: $"{kind_path}.binding", emit: emit);
            if (kind.needs_meta != null)  validateNeedsMeta(meta: kind.needs_meta, path: $"{kind_path}.needs_meta", emit: emit);
            // A025 stage 1: influence cycle in raw kind.
            if (kind.influences != null && hasCycle(influences: kind.influences))
                emit(new Issue(rule_id: "A025", severity: Severity.Error,
                    message: $"kinds[{kind_index}] has a cycle in influences[].", path: $"{kind_path}.influences"));
        }

        static void validatePersonaFields(Persona persona, int persona_index, Root root, Action<Issue> emit) {
            string persona_path = $"personas[{persona_index}]";
            // A004: all kind_ids exist in kinds.
            if (persona.kind_ids != null) {
                var kind_id_set = new HashSet<string>(root.kinds.Select(k => k.kind_id));
                for (int i = 0; i < persona.kind_ids.Count; i++) {
                    if (!kind_id_set.Contains(persona.kind_ids[i]))
                        emit(new Issue(rule_id: "A004", severity: Severity.Error,
                            message: $"personas[{persona_index}].kind_ids[{i}] '{persona.kind_ids[i]}' not found in kinds.",
                            path: $"{persona_path}.kind_ids[{i}]"));
                }
                // A033: duplicate kind_ids.
                var seen = new HashSet<string>();
                foreach (var kind_id in persona.kind_ids)
                    if (!seen.Add(kind_id))
                        emit(new Issue(rule_id: "A033", severity: Severity.Warning,
                            message: $"personas[{persona_index}].kind_ids contains duplicate '{kind_id}'. Composer keeps last occurrence.",
                            path: $"{persona_path}.kind_ids"));
            }
            // A011a: no kind_ids → must have at least one action.
            if ((persona.kind_ids == null || persona.kind_ids.Count == 0) &&
                (persona.actions  == null || persona.actions.Count  == 0))
                emit(new Issue(rule_id: "A011a", severity: Severity.Error,
                    message: $"personas[{persona_index}] has no kind_ids and no actions (at least one action required).",
                    path: $"{persona_path}.actions"));
            if (persona.needs      != null) validateNeeds(needs: persona.needs, path: $"{persona_path}.needs", emit: emit);
            if (persona.rates      != null) validateRates(rates: persona.rates, path: $"{persona_path}.rates", emit: emit);
            if (persona.suppression!= null) validateSuppression(suppression: persona.suppression, path: $"{persona_path}.suppression", emit: emit);
            if (persona.influences != null) validateInfluences(influences: persona.influences, path: $"{persona_path}.influences", emit: emit);
            if (persona.actions    != null) validateActions(acts: persona.actions, path: $"{persona_path}.actions", emit: emit);
            if (persona.commitment != null) validateCommitment(commitment: persona.commitment, path: $"{persona_path}.commitment", emit: emit);
            if (persona.binding    != null) validateBinding(binding: persona.binding, path: $"{persona_path}.binding", emit: emit);
            if (persona.needs_meta != null) validateNeedsMeta(meta: persona.needs_meta, path: $"{persona_path}.needs_meta", emit: emit);
            // A013: rates keys are subset of needs keys.
            if (persona.rates != null && persona.needs != null) {
                foreach (var rate_key in persona.rates.values.Keys)
                    if (!persona.needs.values.ContainsKey(rate_key))
                        emit(new Issue(rule_id: "A013", severity: Severity.Warning,
                            message: $"{persona_path}: rates key '{rate_key}' is not in needs.", path: $"{persona_path}.rates"));
            }

            // A020a/b/c: cross-kind checks (kind fields referencing Needs not in referencing Persona.needs).
            if (persona.kind_ids != null && persona.needs != null) {
                var persona_needs = new HashSet<string>(persona.needs.values.Keys);
                foreach (var kind_id in persona.kind_ids) {
                    var kind = findKind(root: root, kind_id: kind_id);
                    if (kind == null) continue;
                    // A020a: kind.rates key not in persona.needs.
                    if (kind.rates != null)
                        foreach (var rate_key in kind.rates.values.Keys)
                            if (!persona_needs.Contains(rate_key))
                                emit(new Issue(rule_id: "A020a", severity: Severity.Warning,
                                    message: $"{persona_path}: kind '{kind_id}' rates key '{rate_key}' not in persona.needs.", path: $"{persona_path}.kind_ids"));
                    // A020b: kind.influences source/target not in persona.needs.
                    if (kind.influences != null)
                        foreach (var influence in kind.influences) {
                            if (!persona_needs.Contains(influence.source))
                                emit(new Issue(rule_id: "A020b", severity: Severity.Warning,
                                    message: $"{persona_path}: kind '{kind_id}' influence source '{influence.source}' not in persona.needs.", path: $"{persona_path}.kind_ids"));
                            if (!persona_needs.Contains(influence.target))
                                emit(new Issue(rule_id: "A020b", severity: Severity.Warning,
                                    message: $"{persona_path}: kind '{kind_id}' influence target '{influence.target}' not in persona.needs.", path: $"{persona_path}.kind_ids"));
                        }
                    // A020c: kind.actions[].need not in persona.needs.
                    if (kind.actions != null)
                        foreach (var act in kind.actions)
                            if (!persona_needs.Contains(act.need))
                                emit(new Issue(rule_id: "A020c", severity: Severity.Warning,
                                    message: $"{persona_path}: kind '{kind_id}' action '{act.id}' need '{act.need}' not in persona.needs.", path: $"{persona_path}.kind_ids"));
                }
            }
                emit(new Issue(rule_id: "A016", severity: Severity.Warning,
                    message: $"personas[{persona_index}] has no binding. Composer will fill defaults.",
                    path: $"{persona_path}.binding"));
            // A025 stage 1: cycle in raw persona influences.
            if (persona.influences != null && hasCycle(influences: persona.influences))
                emit(new Issue(rule_id: "A025", severity: Severity.Error,
                    message: $"personas[{persona_index}] has a cycle in influences[].",
                    path: $"{persona_path}.influences"));
            // A029: commitment omitted but 2+ actions.
            if (persona.commitment == null && persona.actions != null && persona.actions.Count >= 2)
                emit(new Issue(rule_id: "A029", severity: Severity.Warning,
                    message: $"personas[{persona_index}] has {persona.actions.Count} actions but no commitment (chattering risk).",
                    path: $"{persona_path}.commitment"));
            // A030: no actions or influences use frustration.
            bool uses_frustration =
                (persona.actions    != null && persona.actions.Any(action => action.need == "frustration")) ||
                (persona.influences != null && persona.influences.Any(i =>
                    i.source == "frustration" || i.target == "frustration"));
            if (!uses_frustration && (persona.actions != null || persona.influences != null))
                emit(new Issue(rule_id: "A030", severity: Severity.Warning,
                    message: $"personas[{persona_index}]: no actions or influences use 'frustration' (feedback loop may be missing).",
                    path: $"{persona_path}"));
            // A032: no fallback low-tier action other than idle.
            if (persona.actions != null) {
                bool has_fallback = persona.actions.Any(action => action.need != "idle" && action.tier <= 2);
                if (!has_fallback && persona.actions.Any())
                    emit(new Issue(rule_id: "A032", severity: Severity.Info,
                        message: $"personas[{persona_index}]: no low-tier fallback action besides idle.",
                        path: $"{persona_path}.actions"));
            }
        }

        static void validateNeeds(Needs needs, string path, Action<Issue> emit) {
            // A005: all values in [0, 100]. NaN and Infinity are also errors.
            foreach (var entry in needs.values)
                if (float.IsNaN(entry.Value) || float.IsInfinity(entry.Value) ||
                    entry.Value < 0f || entry.Value > 100f)
                    emit(new Issue(rule_id: "A005", severity: Severity.Error,
                        message: $"{path}['{entry.Key}'] = {entry.Value} is outside [0, 100] (or NaN/Infinity).", path: path));
        }

        static void validateRates(Rates rates, string path, Action<Issue> emit) {
            // (#8) Rates values are unrestricted in range per spec (positive or negative),
            // but NaN/Infinity would corrupt every Step 1 decay (need = NaN*delta_time → NaN),
            // silently destroying the agent state. Reject as A005-class Error.
            foreach (var entry in rates.values) {
                if (float.IsNaN(entry.Value) || float.IsInfinity(entry.Value))
                    emit(new Issue(rule_id: "A005", severity: Severity.Error,
                        message: $"{path}['{entry.Key}'] = {entry.Value} is NaN or Infinity.", path: path));
            }
        }

        static void validateSuppression(Suppression suppression, string path, Action<Issue> emit) {
            // A006: suppression values 0.0 to 1.0.
            foreach (var (name, value) in new[] {
                ("tier2", suppression.tier2), ("tier3", suppression.tier3), ("tier4", suppression.tier4), ("tier5", suppression.tier5) }) {
                if (value < 0f || value > 1f)
                    emit(new Issue(rule_id: "A006", severity: Severity.Error,
                        message: $"{path}.{name} = {value} is outside [0.0, 1.0].", path: $"{path}.{name}"));
            }
        }

        static void validateInfluences(List<Influence> influences, string path, Action<Issue> emit) {
            // A012: coefficient in [-1, 1]. NaN/Infinity also invalid.
            for (int i = 0; i < influences.Count; i++) {
                var coefficient = influences[i].coefficient;
                if (float.IsNaN(coefficient) || float.IsInfinity(coefficient) || coefficient < -1f || coefficient > 1f)
                    emit(new Issue(rule_id: "A012", severity: Severity.Error,
                        message: $"{path}[{i}].coefficient = {coefficient} is outside [-1.0, 1.0] (or NaN/Infinity).",
                        path: $"{path}[{i}].coefficient"));
            }
        }

        static void validateActions(List<Animo.Model.Action> acts, string path, Action<Issue> emit) {
            for (int i = 0; i < acts.Count; i++) {
                var act = acts[i];
                string action_path = $"{path}[{i}]";
                // A009: id not empty.
                if (string.IsNullOrEmpty(act.id))
                    emit(new Issue(rule_id: "A009", severity: Severity.Error, message: $"{action_path}.id is empty.", path: $"{action_path}.id"));
                // A007: tier 1-5.
                if (act.tier < 1 || act.tier > 5)
                    emit(new Issue(rule_id: "A007", severity: Severity.Error,
                        message: $"{action_path}.tier = {act.tier} is outside [1, 5].", path: $"{action_path}.tier"));
                // A008: exponent 0.1-5.0.
                if (act.exponent < 0.1f || act.exponent > 5.0f)
                    emit(new Issue(rule_id: "A008", severity: Severity.Error,
                        message: $"{action_path}.exponent = {act.exponent} is outside [0.1, 5.0].", path: $"{action_path}.exponent"));
                // A022: need is required.
                if (string.IsNullOrEmpty(act.need))
                    emit(new Issue(rule_id: "A022", severity: Severity.Error, message: $"{action_path}.need is required.", path: $"{action_path}.need"));
                // A024: if need is 'idle', tier should be 5.
                if (act.need == "idle" && act.tier != 5)
                    emit(new Issue(rule_id: "A024", severity: Severity.Warning,
                        message: $"{action_path}: action uses 'idle' Need but tier is {act.tier} (should be 5).", path: $"{action_path}.tier"));
            }
        }

        static void validateCommitment(Commitment commitment, string path, Action<Issue> emit) {
            // A028: bonus < 0 Error; bonus > 30 Warning; ceiling at 50.
            if (commitment.bonus < 0f)
                emit(new Issue(rule_id: "A028", severity: Severity.Error,
                    message: $"{path}.bonus = {commitment.bonus} is negative (must be ≥ 0).", path: $"{path}.bonus"));
            else if (commitment.bonus > 50f)
                emit(new Issue(rule_id: "A028", severity: Severity.Error,
                    message: $"{path}.bonus = {commitment.bonus} exceeds ceiling of 50.", path: $"{path}.bonus"));
            else if (commitment.bonus > 30f)
                emit(new Issue(rule_id: "A028", severity: Severity.Warning,
                    message: $"{path}.bonus = {commitment.bonus} exceeds 30 (lock-in risk).", path: $"{path}.bonus"));
        }

        static void validateBinding(Binding binding, string path, Action<Issue> emit) {
            // A014: on_action_change placeholders.
            if (binding.on_action_change != null)
                checkPlaceholders(template: binding.on_action_change, rule_id: "A014", path: path + ".on_action_change",
                    allowed: Const.TEMPLATE_PLACEHOLDERS_ACTION, emit: emit);
            // A010, A015, A023, A034 on thresholds.
            for (int i = 0; i < binding.thresholds.Count; i++) {
                var threshold  = binding.thresholds[i];
                string threshold_path = $"{path}.thresholds[{i}]";
                // A010: trigger_threshold in (0, 100].
                if (threshold.trigger_threshold <= 0f || threshold.trigger_threshold > 100f)
                    emit(new Issue(rule_id: "A010", severity: Severity.Error,
                        message: $"{threshold_path}.trigger_threshold = {threshold.trigger_threshold} is outside (0, 100].", path: threshold_path));
                // A023: trigger > reset (if reset provided).
                if (threshold.reset_threshold.HasValue && threshold.trigger_threshold <= threshold.reset_threshold.Value)
                    emit(new Issue(rule_id: "A023", severity: Severity.Error,
                        message: $"{threshold_path}: trigger_threshold ({threshold.trigger_threshold}) must be > reset_threshold ({threshold.reset_threshold.Value}).", path: threshold_path));
                // A034: reset_threshold < 0 Error.
                if (threshold.reset_threshold.HasValue && threshold.reset_threshold.Value < 0f)
                    emit(new Issue(rule_id: "A034", severity: Severity.Error,
                        message: $"{threshold_path}.reset_threshold = {threshold.reset_threshold.Value} is negative.", path: threshold_path));
                // A015: trigger placeholders.
                if (!string.IsNullOrEmpty(threshold.trigger))
                    checkPlaceholders(template: threshold.trigger, rule_id: "A015", path: $"{threshold_path}.trigger",
                        allowed: Const.TEMPLATE_PLACEHOLDERS_THRESHOLD, emit: emit);
            }
            // (#9) A039 Stage 1: sibling threshold proximity in RAW JSON.
            // Without this, Composer.mergeThresholds (EPSILON=0.01) would collapse
            // duplicates BEFORE Stage 2 A039 ran, silently hiding author mistakes.
            // Stage 1 sees the raw list and warns the author directly.
            var by_need = binding.thresholds.GroupBy(threshold => threshold.need);
            foreach (var group in by_need) {
                var sorted = group.OrderBy(threshold => threshold.trigger_threshold).ToList();
                for (int i = 0; i < sorted.Count - 1; i++) {
                    float difference = sorted[i + 1].trigger_threshold - sorted[i].trigger_threshold;
                    if (difference <= 1.0f + SIBLING_THRESHOLD_EPSILON)
                        emit(new Issue(rule_id: "A039", severity: Severity.Warning,
                            message: $"{path}: sibling thresholds on '{group.Key}' at {sorted[i].trigger_threshold} and {sorted[i+1].trigger_threshold} are within 1.0f of each other.",
                            path: $"{path}.thresholds"));
                }
            }
        }

        static void validateNeedsMeta(Dictionary<string, NeedMeta> meta, string path, Action<Issue> emit) {
            // A038 Stage 1: tier out of range → Error.
            // A038 Stage 1: standard Need tier mismatch → Warning.
            foreach (var entry in meta) {
                if (entry.Value.tier < 1 || entry.Value.tier > 5)
                    emit(new Issue(rule_id: "A038", severity: Severity.Error,
                        message: $"{path}['{entry.Key}'].tier = {entry.Value.tier} is outside [1, 5].", path: path));
                else if (Const.NEED_TIER_BY_NAME.TryGetValue(entry.Key, out var expected_tier) &&
                         entry.Value.tier != expected_tier)
                    emit(new Issue(rule_id: "A038", severity: Severity.Warning,
                        message: $"{path}['{entry.Key}'].tier = {entry.Value.tier} overrides standard tier {expected_tier} (spec §3.5 value wins).", path: path));
            }
        }

        static void checkPlaceholders(string template, string rule_id, string path,
                                       IReadOnlyList<string> allowed, Action<Issue> emit) {
            var found = Regex.Matches(template, @"\{([^}]+)\}");
            foreach (Match match in found) {
                string key = match.Groups[1].Value;
                if (!allowed.Contains(key))
                    emit(new Issue(rule_id: rule_id, severity: Severity.Error,
                        message: $"{path}: placeholder '{{{key}}}' is not allowed. Allowed: {{{string.Join("}, {", allowed)}}}.",
                        path: path));
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
            foreach (var influence in influences) {
                if (!graph.ContainsKey(influence.source)) graph[influence.source] = new List<string>();
                graph[influence.source].Add(influence.target);
            }
            var visited = new HashSet<string>();
            var in_stack = new HashSet<string>();
            bool dfs(string node) {
                if (in_stack.Contains(node)) return true;
                if (visited.Contains(node))  return false;
                visited.Add(node); in_stack.Add(node);
                if (graph.TryGetValue(node, out var neighbors))
                    foreach (var n in neighbors) if (dfs(node: n)) return true;
                in_stack.Remove(node);
                return false;
            }
            foreach (var node in graph.Keys) if (dfs(node: node)) return true;
            return false;
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        static int levenshtein(string first, string second) {
            int first_length = first.Length, second_length = second.Length;
            var distance = new int[first_length + 1, second_length + 1];
            for (int i = 0; i <= first_length; i++) distance[i, 0] = i;
            for (int j = 0; j <= second_length; j++) distance[0, j] = j;
            for (int i = 1; i <= first_length; i++)
                for (int j = 1; j <= second_length; j++) {
                    int cost = first[i-1] == second[j-1] ? 0 : 1;
                    distance[i, j] = Math.Min(Math.Min(distance[i-1, j] + 1, distance[i, j-1] + 1), distance[i-1, j-1] + cost);
                }
            return distance[first_length, second_length];
        }
    }
}
