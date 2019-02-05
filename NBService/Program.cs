using Nordic.Blockchain;
using Nordic.Network;
using Nordic.SharedCache;
using System;

namespace NBService
{
    class Program
    {
        static void Main(string[] args) {

            Console.WriteLine("---------- BLOCKCHAIN ----------");

            Blockchain _nbStructure = new Blockchain();
            var gBlock = _nbStructure.GetBlock(0);

            Console.WriteLine(gBlock.ToString());
            _nbStructure.Add(new BlockData(1, 1, "Lol"));
            Console.WriteLine(_nbStructure.LastBlock().ToString());

            Console.WriteLine("----------- NETWORK ------------");
            Console.WriteLine("Setting up network for 127.0.0.1:1337 (LOCAL ONLY BINDING!).");

            Network _net = new Network("127.0.0.1", 1337);
            if (_net.Setup()) {
                _net.Start();
                Console.WriteLine("Network started on 127.0.0.1:1337.");
                
            } else
                Console.WriteLine("Network setup failed.");

            Console.WriteLine("---------- SHAREDCACHE ---------");

            SharedCache _cache = new SharedCache();
            _cache.AddAddress("127.0.0.1");
            Console.WriteLine(_cache.ToJson());

            Console.Read();
        }
    }
}
