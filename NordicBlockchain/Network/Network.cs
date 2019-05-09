using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Nordic.Exceptions;
using Nordic.Extensions;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Nordic.Network
{
    public class Network : WebSocketBehavior {

        private readonly static string __certificatePassword = "";
        private readonly static string __certificatePath = "cert.crt";

        private static string        _bindedIp = "127.0.0.1";
        private static int           _bindedPort = -1;

        private WebSocketServer      _server = null;

        private static Dictionary<string, string> _knownEndpoints = new Dictionary<string, string>();

        public Network() {
            _bindedIp = "127.0.0.1";
            _bindedPort = 1337;
        }

        public Network(string _ip, int _port) {
            _bindedIp = _ip;
            _bindedPort = _port;

            SessionHandler.getInstance(); // Force creation

            this._server = new WebSocketServer(string.Format("ws://{0}:{1}", _ip, _port));
            //this._server.SslConfiguration.ServerCertificate = new X509Certificate2(__certificatePath, __certificatePassword);
        } 

        public bool Setup() {
            if (this._server != null) {
                SessionHandler.getInstance();
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

            _knownEndpoints[Context.UserEndPoint.ToString()] = Guid.NewGuid().ToString("N");
        }

        protected override void OnClose(CloseEventArgs e) {
            base.OnClose(e);
            
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e) {
            base.OnError(e);

            SessionHandler.getInstance().OnConnectionFailed(this, e);
        }

        protected override void OnMessage(MessageEventArgs e) {
            base.OnMessage(e);

            // Ensure client went trough connection's protocol.
            if (_knownEndpoints[Context.UserEndPoint.ToString()] == null)
                return;

            var _response = SessionHandler.getInstance().OnPeerDataRecv(this, e);
            if (_response.Result != null)
                Send(_response.Result);
        }

        public void Send(byte[] _data, string _to) {
            base.Sessions.SendTo(_data, _to);

            foreach (var s in Sessions.Sessions) {
                Console.WriteLine(s.ID + ": " + s.Context.Host.ToString());
            }

            SessionHandler.getInstance().OnPeerDataSent(this, null);
        }

        public bool Connect(string _ip, int _port) {
            
            return false;
        }

        public bool Disconnect(string _ip, int _port) {

            return false;
        }
    }
}
