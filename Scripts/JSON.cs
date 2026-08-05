// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Animo.Model;

namespace Animo {
    /// <summary>
    /// (v0.1.5, Q-S76 + Q-S151) JSON parsing facade for animo.json files.
    /// Uses Newtonsoft.Json with custom converters for Needs/Rates flat-object
    /// shape. JSON: {"hunger": 40, "fatigue": 20} → Needs.values["hunger"]=40.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class JSON {
        static readonly JsonSerializerSettings SETTINGS = new JsonSerializerSettings {
            Converters = { new NeedsConverter(), new RatesConverter() },
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        /// <summary>
        /// Parse an animo.json text payload into a Root aggregate.
        /// (Q-S151) Needs/Rates flat-JSON is handled by NeedsConverter/RatesConverter.
        /// </summary>
        public static Root Parse(string text) {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("JSON text cannot be null or empty.", nameof(text));
            var root = JsonConvert.DeserializeObject<Root>(text, SETTINGS);
            if (root == null)
                throw new InvalidOperationException("JSON.Parse: deserialization returned null.");
            return root;
        }

        /// <summary>
        /// Write a value out to JSON text. This is the mirror of Parse: the
        /// monitor uses it to turn an EngineSnapshot into the message it sends to
        /// the dashboard each frame. The value's own field names are kept as the
        /// keys, so a snake_case snapshot writes snake_case keys.
        /// </summary>
        public static string Serialize(object value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return JsonConvert.SerializeObject(value, SETTINGS);
        }

        // ── Custom converters ──────────────────────────────────────────────

        /// <summary>
        /// (Q-S151) Converts flat JSON object {"hunger": 40, "fatigue": 20}
        /// into Needs.values Dictionary. Without this, Newtonsoft maps
        /// properties to Needs class members (finds none), leaving values empty.
        /// </summary>
        sealed class NeedsConverter : JsonConverter<Needs> {
            public override Needs? ReadJson(JsonReader reader, Type type, Needs? existing,
                                            bool hasExisting, JsonSerializer s) {
                if (reader.TokenType == JsonToken.Null) return null;
                var object_value = JObject.Load(reader);
                var needs = new Needs();
                foreach (var property in object_value.Properties())
                    needs.values[property.Name] = property.Value.Value<float>();
                return needs;
            }
            public override void WriteJson(JsonWriter w, Needs? v, JsonSerializer s) {
                w.WriteStartObject();
                if (v != null) foreach (var entry in v.values) {
                    w.WritePropertyName(entry.Key); w.WriteValue(entry.Value);
                }
                w.WriteEndObject();
            }
        }

        /// <summary>(Q-S151) Same flat-object converter for Rates.</summary>
        sealed class RatesConverter : JsonConverter<Rates> {
            public override Rates? ReadJson(JsonReader reader, Type type, Rates? existing,
                                            bool hasExisting, JsonSerializer s) {
                if (reader.TokenType == JsonToken.Null) return null;
                var object_value = JObject.Load(reader);
                var rates = new Rates();
                foreach (var property in object_value.Properties())
                    rates.values[property.Name] = property.Value.Value<float>();
                return rates;
            }
            public override void WriteJson(JsonWriter w, Rates? v, JsonSerializer s) {
                w.WriteStartObject();
                if (v != null) foreach (var entry in v.values) {
                    w.WritePropertyName(entry.Key); w.WriteValue(entry.Value);
                }
                w.WriteEndObject();
            }
        }
    }
}
