using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nordic.Exceptions;
using Nordic.Extensions;
using Peer2Net;

namespace Nordic.Network
{
    public class Network {

        private static string        _bindedIp = "127.0.0.1";
        private static int           _bindedPort = -1;

        private CommunicationManager _communication = null;
        private TcpListener          _listener = null;

        public Network(string _ip, int _port) {
            _bindedIp = _ip;
            _bindedPort = _port;
        }

        public void Start() {
            this._listener.Start();
        }

        public static string GetCurrentNodeAddress()
            => _bindedIp;

        public void Stop() {
            // Broadcast disconnect.
            //this._communication.Send(null, null);
            this._listener.Stop();
        }

        public bool Setup() {
            var _listener = new TcpListener(_bindedPort);

            if (_listener != null)
                this._communication = new CommunicationManager(_listener.Cast<TcpListener>());

            if (this._communication != null) {
                this._listener = _listener.Cast<TcpListener>();

                // Add HTTPS handling
                return true;
            }

            return false;
        }
    }
}
