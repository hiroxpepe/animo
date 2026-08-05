// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace Animo.Tools {
    /// <summary>
    /// Single simulation frame. Recorded once per Live(delta_time) call.
    /// (Q-S132) Phase 3 lightweight snapshot: stores float[] values as
    /// Dictionary for API clarity; Phase 4 optimization may switch to
    /// shared key arrays + parallel float[] per Q-S132 contract.
    /// </summary>
    [Serializable]
    public sealed class TraceFrame {
        public float time;
        public string behavior = "";
        public Dictionary<string, float> needs          = new();
        public Dictionary<string, float> effective_needs = new();
        public Dictionary<string, float> action_scores  = new();
        public bool   is_locked;
        public string locked_behavior = "";
        public List<string> signals_fired = new();
    }

    /// <summary>
    /// (v0.1.5, Q-S93) Aggregate result of ScenarioRunner.Run.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [Serializable]
    public sealed class TraceResult {
        public string agent_id = "";
        public float  duration;
        public float  delta_time;
        public List<TraceFrame> frames = new();

        // (Q-S93) Populated by ScenarioRunner.Run in a single post-run pass.
        public Dictionary<string, int>   behavior_count      { get; } = new();
        public Dictionary<string, float> behavior_total_time { get; } = new();

        /// <summary>
        /// (Q-S93) Serialize to CSV. Columns: time, behavior, is_locked,
        /// locked_behavior, needs.*, effective_needs.*, action_scores.*, signals_fired.
        /// Column order is stable (sorted keys) for regression diffing.
        /// </summary>
        public string ToCSV() {
            if (frames.Count == 0) return "";
            var need_keys   = sortedKeys(frames[0].needs);
            var effective_keys    = sortedKeys(frames[0].effective_needs);
            var score_keys  = sortedKeys(frames[0].action_scores);

            var invariant_culture = CultureInfo.InvariantCulture;
            var builder = new StringBuilder();
            // Header
            builder.Append("time,behavior,is_locked,locked_behavior");
            foreach (var k in need_keys)  builder.Append($",needs.{k}");
            foreach (var k in effective_keys)   builder.Append($",effective_needs.{k}");
            foreach (var k in score_keys) builder.Append($",score.{k}");
            builder.AppendLine(",signals_fired");

            // Rows
            foreach (var frame in frames) {
                builder.Append($"{frame.time.ToString("F4", invariant_culture)},{csv(frame.behavior)},{frame.is_locked},{csv(frame.locked_behavior)}");
                foreach (var k in need_keys)  builder.Append($",{frame.needs.GetValueOrDefault(k).ToString("F4", invariant_culture)}");
                foreach (var k in effective_keys)   builder.Append($",{frame.effective_needs.GetValueOrDefault(k).ToString("F4", invariant_culture)}");
                foreach (var k in score_keys) builder.Append($",{frame.action_scores.GetValueOrDefault(k).ToString("F4", invariant_culture)}");
                builder.AppendLine($",\"{string.Join(";", frame.signals_fired)}\"");
            }
            return builder.ToString();
        }

        /// <summary>(Q-S93) Serialize to JSON.</summary>
        public string ToJSON() {
            var object_value = new {
                agent_id,
                duration,
                delta_time,
                behavior_count,
                behavior_total_time,
                frames
            };
            return JsonConvert.SerializeObject(object_value, Formatting.Indented);
        }

        /// <summary>
        /// Populate behavior_count and behavior_total_time from frames[].
        /// Called once by ScenarioRunner.Run after all frames are recorded.
        /// </summary>
        internal void BuildAnalysis() {
            behavior_count.Clear();
            behavior_total_time.Clear();
            // (#6/#7/#9) Use frame time deltas; skip zero-time frames (spawn threshold=0 and
            // boundary Live(0.0f) frames) for both count and total_time so analysis
            // reflects real simulated time, not number of recorded snapshots.
            for (int i = 0; i < frames.Count; i++) {
                if (string.IsNullOrEmpty(frames[i].behavior)) continue;
                float previous  = i > 0 ? frames[i - 1].time : 0f;
                float delta = System.Math.Max(0f, frames[i].time - previous);
                if (delta <= 0f) continue;  // skip zero-time frames (no real time advanced)
                behavior_count.TryGetValue(frames[i].behavior, out var c);
                behavior_count[frames[i].behavior] = c + 1;
                behavior_total_time.TryGetValue(frames[i].behavior, out var threshold);
                behavior_total_time[frames[i].behavior] = threshold + delta;
            }
        }



        static List<string> sortedKeys(Dictionary<string, float> dictionary) {
            var keys = new List<string>(dictionary.Keys);
            keys.Sort(System.StringComparer.Ordinal);
            return keys;
        }
        static string csv(string value) =>
            value.Contains(',') || value.Contains('"') ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
    }
}
