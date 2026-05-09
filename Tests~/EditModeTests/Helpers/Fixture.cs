// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using Animo.Model;

namespace Animo.Tests.EditMode.Helpers {
    /// <summary>
    /// Fluent builders for constructing minimal-yet-valid test fixtures.
    /// Keeps the 180+ test methods readable without repeating boilerplate.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Fixture {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Root / Persona / Kind shorthand

        /// <summary>A minimum legal Root with one Persona that defines one Action.</summary>
        public static Root MinimalRoot(string agent_id = "agent_a") {
            return new Root {
                schema_version = "1.4",
                personas = new List<Persona> {
                    new Persona {
                        agent_id = agent_id,
                        actions = new List<Action> {
                            new Action { id = "Idle", need = "idle", tier = 5, exponent = 1.0f }
                        }
                    }
                }
            };
        }

        /// <summary>An empty Root (no schema_version, no personas) — useful for A000/A001 cases.</summary>
        public static Root EmptyRoot() => new Root();

        /// <summary>Build a Persona without making it valid; tests fill in just what they need.</summary>
        public static Persona PersonaOf(string agent_id) {
            return new Persona { agent_id = agent_id };
        }

        /// <summary>Build a Kind without making it valid; tests fill in just what they need.</summary>
        public static Kind KindOf(string kind_id) {
            return new Kind { kind_id = kind_id };
        }

        /// <summary>Build an Action with sensible defaults.</summary>
        public static Action ActionOf(string id, string need, int tier = 1, float exponent = 1.0f) {
            return new Action { id = id, need = need, tier = tier, exponent = exponent };
        }

        /// <summary>Build an Influence.</summary>
        public static Influence InfluenceOf(string source, string target, float coefficient) {
            return new Influence { source = source, target = target, coefficient = coefficient };
        }

        /// <summary>Build a Threshold (reset_threshold optional).</summary>
        public static Threshold ThresholdOf(string need, float trigger, string trigger_event, float? reset = null) {
            return new Threshold {
                need = need,
                trigger_threshold = trigger,
                reset_threshold = reset,
                trigger = trigger_event
            };
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Needs / Rates dictionary shorthand

        public static Needs NeedsOf(params (string k, float v)[] entries) {
            Needs n = new();
            foreach ((string k, float v) e in entries) n.values[e.k] = e.v;
            return n;
        }

        public static Rates RatesOf(params (string k, float v)[] entries) {
            Rates r = new();
            foreach ((string k, float v) e in entries) r.values[e.k] = e.v;
            return r;
        }
    }
}
