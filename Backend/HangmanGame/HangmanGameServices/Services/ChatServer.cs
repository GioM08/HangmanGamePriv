using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HangmanGameServices.Services
{
    public static class ChatServer
    {
        private const int Port = 9000;
        private static TcpListener _listener;
        private static readonly ConcurrentDictionary<int, List<TcpClient>> _gameRooms
            = new ConcurrentDictionary<int, List<TcpClient>>();
        private static bool _running;

        public static void Start()
        {
            if (_running) return;
            _running = true;

            _listener = new TcpListener(IPAddress.Any, Port);
            _listener.Start();
            Debug.WriteLine($"[ChatServer] Listening on port {Port}");

            var thread = new Thread(AcceptLoop) { IsBackground = true, Name = "ChatServerAccept" };
            thread.Start();
        }

        public static void Stop()
        {
            _running = false;
            _listener?.Stop();
        }

        private static void AcceptLoop()
        {
            while (_running)
            {
                try
                {
                    var client = _listener.AcceptTcpClient();
                    var thread = new Thread(() => HandleClient(client)) { IsBackground = true };
                    thread.Start();
                }
                catch (Exception) when (!_running) { break; }
                catch (Exception ex) { Debug.WriteLine($"[ChatServer] Accept error: {ex.Message}"); }
            }
        }

        private static void HandleClient(TcpClient client)
        {
            int gameId = -1;
            try
            {
                var stream = client.GetStream();
                var buffer = new byte[4096];

                // First message is JOIN:<gameId>:<userId>:<userName>
                int count = stream.Read(buffer, 0, buffer.Length);
                string joinMsg = Encoding.UTF8.GetString(buffer, 0, count).Trim();

                if (joinMsg.StartsWith("JOIN:"))
                {
                    var parts = joinMsg.Split(':');
                    if (parts.Length >= 4 && int.TryParse(parts[1], out gameId))
                    {
                        _gameRooms.AddOrUpdate(gameId,
                            _ => new List<TcpClient> { client },
                            (_, list) => { list.Add(client); return list; });

                        // No se notifica el join en el chat; la notificación viene del callback WCF
                    }
                }

                while (client.Connected)
                {
                    count = stream.Read(buffer, 0, buffer.Length);
                    if (count == 0) break;

                    string msg = Encoding.UTF8.GetString(buffer, 0, count).Trim();
                    if (msg == "LEAVE") break;

                    Broadcast(gameId, client, msg + "\n");
                }
            }
            catch (Exception) { }
            finally
            {
                if (gameId > 0 && _gameRooms.TryGetValue(gameId, out var room))
                    room.Remove(client);
                client.Close();
            }
        }

        private static void Broadcast(int gameId, TcpClient except, string message)
        {
            if (!_gameRooms.TryGetValue(gameId, out var room)) return;
            byte[] data = Encoding.UTF8.GetBytes(message);
            var dead = new List<TcpClient>();

            foreach (var c in room)
            {
                if (c == except || !c.Connected) continue;
                try { c.GetStream().Write(data, 0, data.Length); }
                catch { dead.Add(c); }
            }
            foreach (var d in dead) room.Remove(d);
        }
    }
}
