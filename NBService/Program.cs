using Nordic.Blockchain;
using Nordic.Network;
using Nordic.Security;
using Nordic.Security.Cryptography;
using Nordic.SharedCache;
using System;
using System.IO;

namespace NBService
{
    class Program
    {
        static void Main(string[] args) {

            Console.WriteLine("---------- BLOCKCHAIN ----------\n");

            Blockchain _nbStructure = new Blockchain();
            var gBlock = _nbStructure.GetBlock(0);

            Console.WriteLine(gBlock.ToString());
            _nbStructure.Add(new BlockData(1, 1, "Lol"));
            Console.WriteLine(_nbStructure.LastBlock().ToString());


            Console.WriteLine("\n---------- NODE VAULT ----------\n");

            Console.WriteLine("Importing current node credentials...");
            TrustVault _vault = new TrustVault(File.ReadAllText("privKey.pem"), File.ReadAllText("pubKey.pem"));
            Console.WriteLine(_vault.ToJson());

            Console.WriteLine("\n----------- NETWORK ------------\n");
            Console.WriteLine("Setting up network for 127.0.0.1:1337 (LOCAL ONLY BINDING!).");

            Network _net = new Network("127.0.0.1", 1337);
            if (_net.Setup()) {
                _net.Start();
                Console.WriteLine("Network started on 127.0.0.1:1337.");
                
            } else
                Console.WriteLine("Network setup failed.");

            Console.WriteLine("\n---------- SHAREDCACHE ---------\n");

            Console.WriteLine("Building Shared Node Cache for online nodes...");
            Console.WriteLine("\tOnly 1 node online.");
            SharedCache _cache = new SharedCache();
            _cache.AddAddress("127.0.0.1");
            Console.WriteLine(_cache.ToJson());

            Console.WriteLine("\n-------------- RSA -------------\n");

            RSA _rsa = new RSA(File.ReadAllText("privKey.pem"), File.ReadAllText("pubKey.pem"));
            var _signature = _rsa.Sign("makeAwish");
            var _verify = _rsa.VerifySignature("makeAwish", _signature, null);

            Console.WriteLine("Signed: " + _signature + "\n\n");
            Console.WriteLine("Verify: " + _verify);

            Console.Read();
        }
    }
}
