// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using Newtonsoft.Json.Linq;
using Animo.Core;

namespace Animo.Tools {

    /// <summary>
    /// Reads a step-in message from the dashboard and acts on the loop. The
    /// socket layer only passes the raw text in; this reader understands it. A
    /// message that cannot be read is dropped, never thrown, so a stray or
    /// half-formed message from the browser cannot stop the monitor.
    ///
    /// Message shape: {"kind":"affect","need":"fear","delta":50}
    ///   kind = affect | pause | resume | delta_time
    /// </summary>
    public static class StepInReader {

        /// <summary>
        /// Read a step-in for a set of agents. A "watch" message changes which
        /// agent the dashboard follows; every other message is sent to the
        /// watched agent's loop, so a poke lands on the one on screen.
        /// </summary>
        public static void Read(MonitorSet set, string text) {
            if (set == null) throw new ArgumentNullException(nameof(set));
            if (string.IsNullOrWhiteSpace(text)) return;

            JObject message;
            try {
                message = JObject.Parse(text);
            } catch {
                return;
            }

            if ((string?)message["kind"] == "watch") {
                var agent = (string?)message["agent"];
                if (!string.IsNullOrEmpty(agent)) set.Watch(agent!);
                return;
            }
            Read(set.Loop(set.Watched), text);
        }

        public static void Read(MonitorLoop loop, string text) {
            if (loop == null) throw new ArgumentNullException(nameof(loop));
            if (string.IsNullOrWhiteSpace(text)) return;

            JObject message;
            try {
                message = JObject.Parse(text);
            } catch {
                return; // not JSON — drop it
            }

            var kind = (string?)message["kind"];
            switch (kind) {
                case "affect":
                    var need = (string?)message["need"];
                    var delta = (float?)message["delta"];
                    if (!string.IsNullOrEmpty(need) && delta.HasValue)
                        loop.QueueAffect(need!, delta.Value);
                    break;
                case "pause":
                    loop.Pause();
                    break;
                case "resume":
                    loop.Resume();
                    break;
                case "delta_time":
                    var delta_time = (float?)message["delta_time"];
                    if (delta_time.HasValue)
                        loop.DeltaTime = delta_time.Value; // MonitorLoop.DeltaTime clamps to a sane range
                    break;
                case "lock":
                    var duration = (float?)message["duration"];
                    if (duration.HasValue) {
                        var mode = (string?)message["mode"] == "soft"
                            ? LockMode.Soft : LockMode.Hard;
                        loop.QueueLock(duration.Value, mode);
                    }
                    break;
                case "unlock":
                    loop.QueueUnlock();
                    break;
                case "step":
                    loop.Step();
                    break;
                default:
                    break; // unknown kind — ignore
            }
        }
    }
}
