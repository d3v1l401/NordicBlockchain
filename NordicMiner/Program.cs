using Nordic.Blockchain.Operations;
using Nordic.Network;
using Nordic.Network.Client;
using Nordic.Security.CLM_Manager;
using Nordic.Extensions;
using System;
using System.Threading.Tasks;
using Nordic.Security.ClientAuthenticator;
using System.IO;

namespace NordicMiner
{
    class Program {
        static Client _client = new Client();
        static string _defaultEndpoint = "ws://127.0.0.1:1337/blt";
        static ClmManager _clm         = null;
        static string _hardcodedChallenge     = "miner_test";

        static async Task Main(string[] args) {

            ClientAuthenticator.Initialize("miner_pubKey.pem", "miner_privKeyOut.pem", "");
            ClientAuthenticator.Add("node", File.ReadAllText("node_pubKey.pem"));

            _client.Connect(_defaultEndpoint);
            await Task.Delay(3000);

            // Request connection (only notifies of existance for the node).
            IOperation _auth = new OperationAuthRequest(_hardcodedChallenge, "", ClientAuthenticator.Sign(_hardcodedChallenge));
            _clm = new ClmManager(_auth);
            var _buff = _clm.GetBuffer().Result.ToBase64();
            _client.Send(_defaultEndpoint, _buff);

            // We should have a response for auth, with assigned token as confirmation.
            await Task.Delay(5000);
            Console.WriteLine("Received auth token: " + _client.GetToken());
            await Task.Delay(1000);
            if (string.IsNullOrEmpty(_client.GetToken())) {
                Console.WriteLine("Didn't receive a token yet or refused.");
                Console.ReadLine();
                return;
            }

            // Request a pending operation

            // Perform checks & confirm if fine.
            IOperation _op = new OperationConfirmTx("", "", "");

            //_client.Send(_defaultEndpoint, "");

            // Send notification to the dedicated node for this Tx.

            Console.ReadLine();
        }
    }
}
