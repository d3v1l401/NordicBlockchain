using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Service
{
    public class Service {

        private static Blockchain.Blockchain _blockchain;
        private static Network.Network _network;
        public static void Start() {
            _blockchain = new Blockchain.Blockchain();
            _network = new Network.Network("127.0.0.1", 1337);

            if (_network.Setup())
                _network.Start();
        }
    }
}
