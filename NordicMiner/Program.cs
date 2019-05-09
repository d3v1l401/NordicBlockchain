using Nordic.Blockchain.Operations;
using Nordic.Network;
using Nordic.Network.Client;
using Nordic.Security.CLM_Manager;
using Nordic.Extensions;
using System;
using System.Threading.Tasks;

namespace NordicMiner
{
    class Program {
        static Client _client = new Client();
        static string _defaultEndpoint = "ws://127.0.0.1:1337/blt";
        static ClmManager _clm = null;
        static async Task Main(string[] args) {
            _client.Connect(_defaultEndpoint);

            await Task.Delay(5000);

            // Request connection (only notifies of existance for the node).
            IOperation _auth = new OperationAuthRequest("1", "", "");
            _clm = new ClmManager(_auth);
            var _buff = _clm.GetBuffer().Result.ToBase64();
            _client.Send(_defaultEndpoint, _buff);

            // We should have a response for auth, with assigned token as confirmation.
            

            // Request a pending operation

            // Perform checks & confirm if fine.
            IOperation _op = new OperationConfirmTx("", "", "");

            //_client.Send(_defaultEndpoint, "");

            // Send notification to the dedicated node for this Tx.

            Console.ReadLine();
        }
    }
}
