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

        public bool has_errors => throw new NotImplementedException();
        public bool has_warnings => throw new NotImplementedException();
        public IReadOnlyList<Issue> errors => throw new NotImplementedException();
        public IReadOnlyList<Issue> warnings => throw new NotImplementedException();
        public IReadOnlyList<Issue> infos => throw new NotImplementedException();

        public bool HasRule(string rule_id) => throw new NotImplementedException();
    }

    /// <summary>animo.json validator implementing rules A000–A032.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Validator {
        /// <summary>Validate a Root and return all issues found.</summary>
        public static ValidationResult Validate(Root root) {
            throw new NotImplementedException();
        }
    }
}
