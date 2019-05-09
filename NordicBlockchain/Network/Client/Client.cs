using Nordic.Extensions;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocket4Net;

namespace Nordic.Network.Client
{
    public class Client {
        private IDictionary<string, WebSocket> sessions = new Dictionary<string, WebSocket>();

        private void OnMessage(object sender, DataReceivedEventArgs e) {

            Console.WriteLine(e.Data.HexDump());
        }

        private void OnMessageAsString(object sender, MessageReceivedEventArgs e) {
            Console.WriteLine(e.Message.ToByteArray().HexDump());

        }

        protected void OnError(object sender, ErrorEventArgs e) {
            Console.WriteLine("ERROR: " + e.Exception.Message);
        }

        public void Connect(string _url) {
            if (!this.sessions.ContainsKey(_url)) {
                WebSocket _websck = new WebSocket(_url);

                _websck.DataReceived += (s, e) => { this.OnMessage(s, e); };
                _websck.MessageReceived += (s, e) => { this.OnMessageAsString(s, e); };
                _websck.Error += (s, e) => { this.OnError(s, e); };

                _websck.Open();

                sessions.Add(_url, _websck);
            }
        }

        public WebSocket GetSession(string _url) {
            return this.sessions[_url];
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
