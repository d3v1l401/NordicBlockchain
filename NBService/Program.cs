using Nordic.Blockchain;
using Nordic.Blockchain.Operations;
using Nordic.Configuration;
using Nordic.Extensions;
using Nordic.Network;
using Nordic.Network.Client;
using Nordic.Security;
using Nordic.Security.CLM_Manager;
using Nordic.Security.Cryptography;
using Nordic.Security.ServerAuthenticator;
using Nordic.SharedCache;
using System;
using System.IO;

namespace NBService
{
    class Program
    {
        static void Main(string[] args) {
            //if (!ConfigurationHelper.Import("NordicConf.json")) {
            //    Console.WriteLine("Configuration import failed, using default values and writing configuration file \"NordicConf.json\".");
            //    ConfigurationHelper.Export("NordicConf.json");
            //}

            ServerAuthenticator.Initialize("pubKey.pem", "privKey.pem", "");

            Console.WriteLine("---------- BLOCKCHAIN ----------\n");

            Blockchain _nbStructure = new Blockchain();
            var gBlock = _nbStructure.GetBlock(0);

            Console.WriteLine(gBlock.ToString());
            _nbStructure.Add(new BlockData("", IOperation.OPERATION_TYPE.SECURITY_BC_COMPROMISE_NOTICE, "Lol"));
            Console.WriteLine(_nbStructure.LastBlock().ToString());

            Console.WriteLine("Fork Validity: " + _nbStructure.Validity());

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
            var _verify2 = _rsa.VerifySignature("makeAwisha", _signature, null);

            Console.WriteLine("Signed: " + _signature + "\n\n");
            Console.WriteLine("Verify: " + _verify);
            Console.WriteLine("Verify fake one: " + _verify2);

            Client cl = new Client();
            cl.Connect("ws://127.0.0.1:1337/blt");
            bool _sent = false;
            while (true) {
                try {
                    if (!_sent) {
                        IOperation _op = new OperationTransaction("d3vil401", "13.2", "none");
                        ClmManager _clm = new ClmManager(_op);
                        var _buffer = _clm.GetBuffer().Result.ToBase64();

                        cl.Send("ws://127.0.0.1:1337/blt", _buffer);
                        _sent = true;
                    }

                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
