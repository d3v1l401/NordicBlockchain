using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nordic.Exceptions;
using Nordic.Extensions;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Nordic.Network
{
    public class Network : WebSocketBehavior {

        private static string        _bindedIp = "127.0.0.1";
        private static int           _bindedPort = -1;

        private WebSocketServer      _server = null;
        private SessionHandler       _handler = null;

        public Network() {
            _bindedIp = "127.0.0.1";
            _bindedPort = 1337;
        }

        public Network(string _ip, int _port) {
            _bindedIp = _ip;
            _bindedPort = _port;

            this._handler = new SessionHandler();
            this._server = new WebSocketServer(string.Format("ws://{0}:{1}", _ip, _port));
        } 

        public bool Setup() {
            if (this._server != null) {
                this._handler = new SessionHandler();
                this._server.AddWebSocketService<Network>("/blt");
                return true;
            }

            return false;
        }

        public void Start() {
            if (this._server != null)
                this._server.Start();
        }

        public static string GetCurrentNodeAddress()
            => _bindedIp;

        public void Stop() {
            // Broadcast disconnect.
            //this._communication.Send(null, null);
            if (this._server != null)
                this._server.Stop();
        }

        protected override void OnOpen() {
            base.OnOpen();
            
        }

        protected override void OnClose(CloseEventArgs e) {
            base.OnClose(e);
            
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e) {
            base.OnError(e);
            if (this._handler == null)
                this._handler = new SessionHandler();

            this._handler.OnConnectionFailed(this, e);
        }

        protected override void OnMessage(MessageEventArgs e) {
            base.OnMessage(e);
            if (this._handler == null)
                this._handler = new SessionHandler();

            this._handler.OnPeerDataRecv(this, e);
        }

        public void Send(byte[] _data, string _to) {

            if (this._handler == null)
                this._handler = new SessionHandler();
            //base.SendAsync(_data, null);
            base.Sessions.SendTo(_data, _to);
            this._handler.OnPeerDataSent(this, null);
        }

        public bool Connect(string _ip, int _port) {
            
            return false;
        }

        public bool Disconnect(string _ip, int _port) {

            return false;
        }
    }
}
