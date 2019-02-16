using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;

namespace Nordic.Network.Client
{
    public class Client {
        private IDictionary<string, WebSocket> sessions = new Dictionary<string, WebSocket>();

        public void Connect(string _url) {
            if (!this.sessions.ContainsKey(_url)) {
                WebSocket _websck = new WebSocket(_url);

                _websck.OnMessage += (s, e) => {
                    if (e.Data[0] == 0x01) {
                        Console.WriteLine("Hello");
                    } else {
                        
                    }
                };

                _websck.Connect();
                _websck.Send("Hi");
                sessions.Add(_url, _websck);
            }
        }

        public void Send(string _url, string _data) {
            foreach (var item in sessions)
                if (item.Key == _url)
                    item.Value.Send(_data);
        }

        public void Broadcast(string _data) {
            foreach (var item in sessions)
                item.Value.Send(_data);
        }

        public IList<string> GetServers() {
            IList<string> servers = new List<string>();
            foreach (var item in sessions)
                servers.Add(item.Key);

            return servers;
        }

        public void Close() {
            foreach (var item in sessions)
                item.Value.Close();
        }
    }
}
