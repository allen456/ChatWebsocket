using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ChatApp.Middleware
{
    public class WebSocketServerConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        
        public string AddSocket(WebSocket socket)
        {
            string sockentconid = LoremNET.Lorem.Words(2, false);
            _sockets.TryAdd(sockentconid, socket);
            return sockentconid;
        }

        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _sockets;
        }
    }
}