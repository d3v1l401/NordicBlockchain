using Peer2Net;
using System;
using System.Collections.Generic;
using System.Net;
using Peer2Net.BufferManager;
using Peer2Net.MessageHandlers;

namespace Nordic.Network
{
    class SessionHandler {

        private Dictionary<IPEndPoint, Tuple<Peer, PascalMessageHandler>> _sessions = null;

        public SessionHandler() {
            _sessions = new Dictionary<IPEndPoint, Tuple<Peer, PascalMessageHandler>>();
        }


    }
}
