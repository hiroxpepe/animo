// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Animo;
using Animo.Core;
using Animo.Tools;

namespace Animo.Monitor {

    /// <summary>
    /// The console that runs the live monitor. It loads a persona file, builds
    /// one or more agents into a MonitorSet, and serves that set over a WebSocket
    /// for the dashboard to read. This is the smallest program that runs the real
    /// engine — no Unity, just the loops on a wire.
    ///
    /// Use: dotnet run -- path/to/persona.json agent_id[,agent_id...] [port]
    /// </summary>
    public static class Program {

        public static async Task<int> Main(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("use: monitor <persona.json> <agent_id[,agent_id...]> [port]");
                return 1;
            }
            var persona_path = args[0];
            var agent_ids = args[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
            var port = args.Length >= 3 && int.TryParse(args[2], out var p) ? p : 8181;

            if (!File.Exists(persona_path)) {
                Console.WriteLine($"persona file not found: {persona_path}");
                return 1;
            }

            var root = JSON.Parse(File.ReadAllText(persona_path));
            PersonaCache.Initialize(root);

            var set = new MonitorSet(delta_time: 0.5f);
            foreach (var raw_id in agent_ids) {
                var agent_id = raw_id.Trim();
                var composed = PersonaCache.GetComposed(agent_id).DeepCopy();
                composed.agent_id = agent_id;
                set.Add(agent_id, new Engine(composed));
            }
            var server = new MonitorServer(set, port);

            using var cancel = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => { e.Cancel = true; cancel.Cancel(); };
            Console.WriteLine($"Monitor: {agent_ids.Length} agent(s) from {persona_path}");
            try {
                await server.RunAsync(cancel.Token);
            } catch (OperationCanceledException) {
                Console.WriteLine("Monitor stopped.");
            }
            return 0;
        }
    }
}
