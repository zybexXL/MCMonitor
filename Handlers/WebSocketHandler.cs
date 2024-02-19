using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WatsonWebsocket;

namespace MCMonitor
{
    internal class WebSocketHandler : BaseHandler
    {
        int port;
        WatsonWsServer server;
        List<Guid> connectedClients = new List<Guid>();

        public WebSocketHandler(Config config) : base(config)
        {
            if (!int.TryParse(config[ConfigProperty.WebsocketPort], out port) || port < 0 || port > 65535)
                isEnabled = false;

            if (isEnabled)
            {
                NeedsPlaybackInfo = true;
                NeedsDetails = config[ConfigProperty.WebsocketDetails] == "1";

                Start();
            }
        }

        public override bool Process(MCEventInfo e)
        {
            if (!isEnabled || server == null || connectedClients.Count == 0)
                return true;

            string json = JsonSerializer.Serialize(e, jsonOptions);

            lock (connectedClients)
                foreach (var client in connectedClients)
                    try { server.SendAsync(client, json); } catch { }

            return true;
        }

        public void Start()
        {
            if (!isEnabled) return;

            try
            {
                server = new WatsonWsServer("localhost", port);
                server.ClientConnected += ClientConnected;
                server.ClientDisconnected += ClientDisconnected;
                server.MessageReceived += MessageReceived;
                server.Start();

                Logger.Log("WebSocket server started");
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Failed to start WebSocket server");
            }
        }

        public void Stop()
        {
            if (!isEnabled) return;

            try
            {
                server?.Stop();
            }
            catch { }
        }

        void ClientConnected(object sender, ConnectionEventArgs args)
        {
            Logger.Log($"WebSocket client connected: {args.Client}");
            lock (connectedClients)
                connectedClients.Add(args.Client.Guid);
        }

        void ClientDisconnected(object sender, DisconnectionEventArgs args)
        {
            Logger.Log($"WebSocket client disconnected: {args.Client}");
            lock (connectedClients)
                connectedClients.Remove(args.Client.Guid);
        }

        void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Logger.Log($"WebSocket message received from {args.Client}. MessageType={args.MessageType} [Ignored]");
            if (args.MessageType == WebSocketMessageType.Text)
                Logger.Log("Message content: " + Encoding.UTF8.GetString(args.Data));
        }
    }
}
