using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HangmanGameWPF.Services
{
    internal class ChatClient
    {
        private const string ServerHost = "localhost";
        private const int ServerPort = 9000;

        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _receiveThread;
        private bool _connected;

        public event Action<string> MessageReceived;

        public bool Connect(int gameId, int userId, string userName)
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(ServerHost, ServerPort);
                _stream = _client.GetStream();
                _connected = true;

                string joinMsg = $"JOIN:{gameId}:{userId}:{userName}";
                byte[] data = Encoding.UTF8.GetBytes(joinMsg);
                _stream.Write(data, 0, data.Length);

                _receiveThread = new Thread(ReceiveLoop) { IsBackground = true };
                _receiveThread.Start();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SendMessage(int senderId, string senderName, int gameId, string text)
        {
            if (!_connected) return;
            try
            {
                string msg = $"MSG:{senderId}:{senderName}:{gameId}:{text}";
                byte[] data = Encoding.UTF8.GetBytes(msg);
                _stream.Write(data, 0, data.Length);
            }
            catch (Exception) { }
        }

        public void Disconnect()
        {
            _connected = false;
            try
            {
                if (_stream != null)
                {
                    byte[] leave = Encoding.UTF8.GetBytes("LEAVE");
                    _stream.Write(leave, 0, leave.Length);
                    _stream.Close();
                }
                _client?.Close();
            }
            catch (Exception) { }
        }

        private void ReceiveLoop()
        {
            var buffer = new byte[4096];
            while (_connected)
            {
                try
                {
                    int count = _stream.Read(buffer, 0, buffer.Length);
                    if (count == 0) break;
                    string msg = Encoding.UTF8.GetString(buffer, 0, count).Trim();
                    MessageReceived?.Invoke(msg);
                }
                catch (Exception) { break; }
            }
        }
    }
}
