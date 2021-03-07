using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

using ChatApp.Models;
using ChatApp.Data;

namespace ChatApp.Middleware
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketServerConnectionManager _manager;

        public WebSocketServerMiddleware(RequestDelegate next, WebSocketServerConnectionManager manager)
        {
            _next = next;
            _manager = manager;
        }

        public async Task InvokeAsync(HttpContext context, ChatContext _dbcontext)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                string conn = _manager.AddSocket(webSocket);
                // -- NEW CHAT CLIENT --
                if(!_dbcontext.ChatClients.Any(x => x.ChatClientName == conn)){
                    var _bagongcliente = new ChatClient
                    {
                        ChatClientId = Guid.NewGuid(),
                        Timestamp = DateTime.Now,
                        ChatClientName = conn,
                    };
                    _dbcontext.ChatClients.Add(_bagongcliente);
                    await _dbcontext.SaveChangesAsync();
                }
                // -- NEW CHAT CLIENT --
                await SendConnectionID(webSocket, conn);
                await Receive(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)    
                    {
                        await RouteJSONMessageAsync(Encoding.UTF8.GetString(buffer, 0, result.Count), _dbcontext);
                        return;
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        string id = _manager.GetAllSockets().FirstOrDefault(s => s.Value == webSocket).Key;
                        // -- REMOVE CHAT CLIENT --
                        await DeleteMissingClients(id, _dbcontext);
                        // -- REMOVE CHAT CLIENT --
                        WebSocket sock;
                        _manager.GetAllSockets().TryRemove(id, out sock);
                        await sock.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                        return;
                    }
                    else 
                    {
                        return;
                    }
                });
            }
            else
            {
                await _next(context);
            }
        }

        private async Task RouteJSONMessageAsync(string message, ChatContext _dbcontext)
        {
            var routeOb = JsonConvert.DeserializeObject<dynamic>(message);
            if (String.IsNullOrEmpty(routeOb.To.ToString()))
            {
                foreach (var sock in _manager.GetAllSockets())
                {
                    if (sock.Value.State == WebSocketState.Open)
                    {
                        // -- SEND PAYLOAD --
                        var _payload = new PayloadViewModel
                        {
                            Galing = routeOb.From.ToString(),
                            Papunta = sock.Key.ToString(),
                            Laman = routeOb.Message.ToString(),
                            Araw = DateTime.Now,
                            Lahat = true
                        };
                        await sock.Value.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_payload)), WebSocketMessageType.Text, true, CancellationToken.None);
                        // -- SEND PAYLOAD --
                    }
                }
            }
            else
            {
                if(_manager.GetAllSockets().Any(s => s.Key == routeOb.To.ToString()))
                {
                    var sock = _manager.GetAllSockets().FirstOrDefault(s => s.Key == routeOb.To.ToString());
                    var sockreturn = _manager.GetAllSockets().FirstOrDefault(s => s.Key == routeOb.From.ToString());
                    if (sock.Value != null)
                    {
                        if (sock.Value.State == WebSocketState.Open)
                        {
                            // -- SEND PAYLOAD --
                            var _payloadkungwalaproblema = new PayloadViewModel
                            {
                                Galing = routeOb.From.ToString(),
                                Papunta = routeOb.To.ToString(),
                                Laman = routeOb.Message.ToString(),
                                Araw = DateTime.Now,
                                Lahat = false
                            };
                            await sock.Value.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_payloadkungwalaproblema)), WebSocketMessageType.Text, true, CancellationToken.None);
                            // -- SEND PAYLOAD --
                        }
                    }
                }
            }
        }

        
        private async Task DeleteMissingClients(string deletedata, ChatContext _dbcontext)
        {
            if(_dbcontext.ChatClients.Any(x => x.ChatClientName == deletedata))
            {
                _dbcontext.ChatClients.Remove(_dbcontext.ChatClients.Where(x => x.ChatClientName == deletedata).FirstOrDefault());
                await _dbcontext.SaveChangesAsync();
            }

        }

        private async Task SendConnectionID(WebSocket socket, string connID)
        {
            var buffer = Encoding.UTF8.GetBytes(connID);
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);
                handleMessage(result, buffer);
            }
        }
    }
}