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

        private string        _bindedIp = string.Empty;
        private int           _bindedPort = -1;

        private CommunicationManager _communication = null;
        private TcpListener          _listener = null;

        public Network(string _ip, int _port) {
            this._bindedIp = _ip;
            this._bindedPort = _port;
        }

        public void Start() {
            this._listener.Start();
        }

        public void Stop() {
            // Broadcast disconnect.
            //this._communication.Send(null, null);
            this._listener.Stop();
        }

        public bool Setup() {
            var _listener = new TcpListener(this._bindedPort);

            if (_listener != null)
                this._communication = new CommunicationManager(_listener.Cast<TcpListener>());

            if (this._communication != null) {
                this._listener = _listener.Cast<TcpListener>();
                return true;
            }

            return false;
        }
    }
}
