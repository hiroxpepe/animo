// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;

namespace Animo.Model {

    /// <summary>JSON root: schema_version + kinds + personas.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Root {
        public string schema_version { get; set; } = "";
        public List<Kind> kinds { get; set; } = new();
        public List<Persona> personas { get; set; } = new();
    }

    /// <summary>Type definition. Cascades into Personas via kind_ids.</summary>
    public class Kind {
        public string kind_id { get; set; } = "";
        public Rates? rates { get; set; }
        public Suppression? suppression { get; set; }
        public List<Influence>? influences { get; set; }
        public List<Action>? actions { get; set; }
        public Commitment? commitment { get; set; }
        public Binding? binding { get; set; }
    }

    /// <summary>Individual agent definition. Inherits via kind_ids.</summary>
    public class Persona {
        public string agent_id { get; set; } = "";
        public string? persona_name { get; set; }
        public List<string>? kind_ids { get; set; }
        public Needs? needs { get; set; }
        public Rates? rates { get; set; }
        public Suppression? suppression { get; set; }
        public List<Influence>? influences { get; set; }
        public List<Action>? actions { get; set; }
        public Commitment? commitment { get; set; }
        public Binding? binding { get; set; }
    }

    /// <summary>Need value set [0, 100]. Float dictionary backed.</summary>
    public class Needs {
        public Dictionary<string, float> values { get; set; } = new();
        public float Get(string need) => throw new System.NotImplementedException();
        public float Normalized(string need) => throw new System.NotImplementedException();
        public void Clamp() => throw new System.NotImplementedException();
    }

    /// <summary>Need change rate per second. Negative pulls toward 0; positive pushes toward 100.</summary>
    public class Rates {
        public Dictionary<string, float> values { get; set; } = new();
    }

    /// <summary>Tier suppression factors [0, 1]. Only tier2..tier5 are valid.</summary>
    public class Suppression {
        public float tier2 { get; set; } = 0f;
        public float tier3 { get; set; } = 0f;
        public float tier4 { get; set; } = 0f;
        public float tier5 { get; set; } = 0f;
    }

    /// <summary>Directed need-to-need effect. Coefficient in [-1, 1].</summary>
    public class Influence {
        public string source { get; set; } = "";
        public string target { get; set; } = "";
        public float coefficient { get; set; } = 0f;
    }

    /// <summary>Action definition. need is required since v0.1.1.</summary>
    public class Action {
        public string id { get; set; } = "";
        public string need { get; set; } = "";
        public int tier { get; set; } = 1;
        public float exponent { get; set; } = 1.0f;
        // need_index cache is internal in spec; tests use the public API only.
        internal int need_index;
    }

    /// <summary>Action continuation bonus. v0.1.3 dropped 'decay' field.</summary>
    public class Commitment {
        public float bonus { get; set; } = 0f;
    }

    /// <summary>Germio integration binding.</summary>
    public class Binding {
        public string? on_action_change { get; set; }
        public List<Threshold>? thresholds { get; set; }
    }

    /// <summary>Two-stage hysteresis threshold trigger (v0.1.1).</summary>
    public class Threshold {
        public string need { get; set; } = "";
        public float trigger_threshold { get; set; } = 0f;
        public float? reset_threshold { get; set; }
        public string trigger { get; set; } = "";
        internal int need_index;
    }
}
