// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace Animo.Tools {
    /// <summary>
    /// Single simulation frame. Recorded once per Live(dt) call.
    /// (Q-S132) Phase 3 lightweight snapshot: stores float[] values as
    /// Dictionary for API clarity; Phase 4 optimization may switch to
    /// shared key arrays + parallel float[] per Q-S132 contract.
    /// </summary>
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
    public sealed class TraceResult {
        public string agent_id = "";
        public float  duration;
        public float  dt;
        public List<TraceFrame> frames = new();

        // (Q-S93) Populated by ScenarioRunner.Run in a single post-run pass.
        public Dictionary<string, int>   behavior_count      { get; } = new();
        public Dictionary<string, float> behavior_total_time { get; } = new();

        /// <summary>
        /// Populate behavior_count and behavior_total_time from frames[].
        /// Called once by ScenarioRunner.Run after all frames are recorded.
        /// </summary>
        internal void BuildAnalysis() {
            behavior_count.Clear();
            behavior_total_time.Clear();
            // (#6/#7/#9) Use frame time deltas; skip zero-time frames (spawn t=0 and
            // boundary Live(0.0f) frames) for both count and total_time so analysis
            // reflects real simulated time, not number of recorded snapshots.
            for (int i = 0; i < frames.Count; i++) {
                if (string.IsNullOrEmpty(frames[i].behavior)) continue;
                float prev  = i > 0 ? frames[i - 1].time : 0f;
                float delta = System.Math.Max(0f, frames[i].time - prev);
                if (delta <= 0f) continue;  // skip zero-time frames (no real time advanced)
                behavior_count.TryGetValue(frames[i].behavior, out var c);
                behavior_count[frames[i].behavior] = c + 1;
                behavior_total_time.TryGetValue(frames[i].behavior, out var t);
                behavior_total_time[frames[i].behavior] = t + delta;
            }
        }

        /// <summary>
        /// (Q-S93) Serialize to CSV. Columns: time, behavior, is_locked,
        /// locked_behavior, needs.*, effective_needs.*, action_scores.*, signals_fired.
        /// Column order is stable (sorted keys) for regression diffing.
        /// </summary>
        public string ToCsv() {
            if (frames.Count == 0) return "";
            var need_keys   = SortedKeys(frames[0].needs);
            var eff_keys    = SortedKeys(frames[0].effective_needs);
            var score_keys  = SortedKeys(frames[0].action_scores);

            var ic = CultureInfo.InvariantCulture;
            var sb = new StringBuilder();
            // Header
            sb.Append("time,behavior,is_locked,locked_behavior");
            foreach (var k in need_keys)  sb.Append($",needs.{k}");
            foreach (var k in eff_keys)   sb.Append($",eff.{k}");
            foreach (var k in score_keys) sb.Append($",score.{k}");
            sb.AppendLine(",signals_fired");

            // Rows
            foreach (var f in frames) {
                sb.Append($"{f.time.ToString("F4", ic)},{Csv(f.behavior)},{f.is_locked},{Csv(f.locked_behavior)}");
                foreach (var k in need_keys)  sb.Append($",{f.needs.GetValueOrDefault(k).ToString("F4", ic)}");
                foreach (var k in eff_keys)   sb.Append($",{f.effective_needs.GetValueOrDefault(k).ToString("F4", ic)}");
                foreach (var k in score_keys) sb.Append($",{f.action_scores.GetValueOrDefault(k).ToString("F4", ic)}");
                sb.AppendLine($",\"{string.Join(";", f.signals_fired)}\"");
            }
            return sb.ToString();
        }

        /// <summary>(Q-S93) Serialize to JSON.</summary>
        public string ToJson() {
            var obj = new {
                agent_id,
                duration,
                dt,
                behavior_count,
                behavior_total_time,
                frames
            };
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        static List<string> SortedKeys(Dictionary<string, float> d) {
            var keys = new List<string>(d.Keys);
            keys.Sort(System.StringComparer.Ordinal);
            return keys;
        }
        static string Csv(string s) =>
            s.Contains(',') || s.Contains('"') ? $"\"{s.Replace("\"", "\"\"")}\"" : s;
    }
}
