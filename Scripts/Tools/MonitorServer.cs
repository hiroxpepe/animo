// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Animo.Core;

namespace Animo.Tools {

    /// <summary>
    /// Puts a MonitorSet on a WebSocket. This is the thin pipe between the agents
    /// and the browser: each frame it advances every agent and ships all their
    /// snapshots out as JSON, and it hands any message from the browser to
    /// StepInReader, which routes it to the watched agent. It holds no game logic
    /// of its own — the set, the loop, and the reader, all tested on their own,
    /// hold that. This class is only the wire. A single-agent run is just a set
    /// of one. More than one dashboard may connect at once; each runs on its own
    /// task and is closed cleanly when it drops or the server stops.
    /// </summary>
    public sealed class MonitorServer {

        readonly MonitorSet _set;
        readonly int _port;
        readonly int _frame_delay_ms;

        public MonitorServer(MonitorSet set, int port = 8181, int frame_delay_ms = 100) {
            _set = set;
            _port = port;
            _frame_delay_ms = frame_delay_ms;
        }

        public MonitorServer(MonitorLoop loop, int port = 8181, int frame_delay_ms = 100)
            : this(setOfOne(loop), port, frame_delay_ms) { }

        /// <summary>
        /// Listen and take dashboards as they connect. Each connection runs its
        /// own two flows on its own task; the listener stays free to take the
        /// next one, so a second dashboard is not blocked behind the first.
        /// </summary>
        public async Task RunAsync(CancellationToken token) {
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{_port}/");
            listener.Start();
            Console.WriteLine($"Monitor is listening on ws://localhost:{_port}/");
            try {
                while (!token.IsCancellationRequested) {
                    var context = await listener.GetContextAsync();
                    if (!context.Request.IsWebSocketRequest) {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                        continue;
                    }
                    var socket_context = await context.AcceptWebSocketAsync(null);
                    // Run this dashboard on its own task; do not block the accept loop.
                    _ = handleAsync(socket_context.WebSocket, token);
                }
            } finally {
                listener.Stop();
                listener.Close();
            }
        }

        static MonitorSet setOfOne(MonitorLoop loop) {
            var set = new MonitorSet(loop.DeltaTime);
            set.Add(loop.Engine.AgentID, loop.Engine);
            return set;
        }

        static async Task closeQuietly(WebSocket socket) {
            if (socket.State != WebSocketState.Open && socket.State != WebSocketState.CloseReceived)
                return;
            try {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None);
            } catch {
                // closing a broken socket may throw; nothing more to do
            }
        }

        async Task handleAsync(WebSocket socket, CancellationToken token) {
            try {
                var incoming = readLoopAsync(socket, token);
                while (socket.State == WebSocketState.Open && !token.IsCancellationRequested) {
                    var snapshot_set = _set.TickAll();
                    var message = new {
                        watched = _set.Watched,
                        ids = _set.Ids,
                        agents = snapshot_set,
                    };
                    var bytes = Encoding.UTF8.GetBytes(JSON.Serialize(message));
                    await socket.SendAsync(bytes, WebSocketMessageType.Text, true, token);
                    await Task.Delay(_frame_delay_ms, token);
                }
                await incoming;
            } catch (OperationCanceledException) {
                // server is stopping — fall through to close
            } catch (WebSocketException) {
                // dashboard dropped mid-flight — fall through to close
            } finally {
                await closeQuietly(socket);
                socket.Dispose();
            }
        }

        async Task readLoopAsync(WebSocket socket, CancellationToken token) {
            var buffer = new byte[4096];
            while (socket.State == WebSocketState.Open && !token.IsCancellationRequested) {
                var result = await socket.ReceiveAsync(buffer, token);
                if (result.MessageType == WebSocketMessageType.Close) break;
                var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
                StepInReader.Read(_set, text);
            }
        }
    }
}
