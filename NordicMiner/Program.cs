using Nordic.Blockchain.Operations;
using Nordic.Network;
using Nordic.Network.Client;
using Nordic.Security.CLM_Manager;
using Nordic.Extensions;
using System;
using System.Threading.Tasks;
using Nordic.Security.ClientAuthenticator;
using System.IO;
using Nordic.Security.Cryptography;

namespace NordicMiner
{
    class Program {
        static Client _client = new Client();
        static string _defaultEndpoint = "ws://127.0.0.1:1337/blt";
        static ClmManager _clm         = null;
        static string _hardcodedChallenge     = "miner_test";
        static RSA _crypto = null;

        static string _lastTxId = string.Empty;

        private static string pendingTicketProcessor(OperationPendingAck _op) {

            if (_op != null) {
                Console.WriteLine("We have a transaction to check!");
                var _tokens = _op.OperationData.Split('|');
                if (_tokens.Length == 5) {

                    var _author = _tokens[0];
                    var _signature = _tokens[1];
                    var _queueDate = DateTime.FromOADate(Double.Parse(_tokens[2]));
                    var _type = _tokens[3];
                    var _sent = DateTime.FromOADate(Double.Parse(_tokens[4]));

                    var _span = DateTime.UtcNow - _sent;

                    Console.WriteLine("Transaction by [ " + _author + " ] queued " + _queueDate.ToString() + " and on hold here since " + _sent.ToString());

                    // Max 12 hours to be processed, otherwise it gets removed.
                    if (_span.TotalHours >= 12) {
                        return null;
                    }

                    if (_author != null && _author.Length > 0) {
                        // And exists.
                        if (_signature != null && _signature.Length > 0) {
                            // And is valid.
                            if (_type.Equals("TRANSACTION_REQUEST")) {
                                // And the operation is effectively a transaction request.

                                Sha256 _sha = new Sha256();
                                _sha.Enqueue((_author + "-" + _signature + "-" + _queueDate.ToString()).ToByteArray());
                                var _txId = _sha.Finalize().ToBase64();

                                _lastTxId = _txId;

                                var _ticketVote = new ClmManager(new OperationConfirmTx(_hardcodedChallenge, _txId, "sign"));
                                var _buffer = _ticketVote.GetBuffer().Result;
                                return _buffer.ToBase64();
                            }
                        }
                    }

                }
            }

            return string.Empty;
        }

        static async Task Main(string[] args) {

            Console.WriteLine("Loading Nordic Miner (test)...");

            Console.WriteLine("Loading cryptographic configuration...");
            _crypto = new RSA(File.ReadAllText("miner_privKeyOut.pem"), File.ReadAllText("miner_pubKey.pem"));
            if (_crypto == null)
                throw new Exception("Could not initialize RSA");

            ClientAuthenticator.Initialize("miner_pubKey.pem", "miner_privKeyOut.pem", "");
            ClientAuthenticator.Add("node", File.ReadAllText("node_pubKey.pem"));

            Console.WriteLine("Connecting to node...");
            _client.TicketProcessor = new Client.processPendingTicket(pendingTicketProcessor);
            _client.Connect(_defaultEndpoint);
            await Task.Delay(3000);

            Console.WriteLine("Authenticating...");
            // Request connection (only notifies of existence for the node).
            IOperation _auth = new OperationAuthRequest(_hardcodedChallenge, "", ClientAuthenticator.Sign(_hardcodedChallenge));
            _clm = new ClmManager(_auth);
            var _buff = _clm.GetBuffer().Result.ToBase64();
            _client.Send(_defaultEndpoint, _buff);

            // We should have a response for auth, with assigned token as confirmation.
            await Task.Delay(5000);
            Console.WriteLine("Received auth token: " + _client.GetToken());
            await Task.Delay(1000);
            if (string.IsNullOrEmpty(_client.GetToken())) {
                Console.WriteLine("Didn't receive a token yet or got refused.");
                Console.ReadLine();
                return;
            }


            Console.WriteLine("Asking for something to do...");
            // Request a pending operation (transaction), process it and report to node.
            _clm = new ClmManager(new OperationPendingRequest(_hardcodedChallenge, ClientAuthenticator.GetPubKey(), ClientAuthenticator.Sign(_hardcodedChallenge)));
            _buff = _clm.GetBuffer().Result.ToBase64();
            _client.Send(_defaultEndpoint, _buff);


            await Task.Delay(5000);

            // Impersonate administrator, ask for statistics (last block)
            _clm = new ClmManager(new OperationStatsRequest("admin_test", "", ""));
            _buff = _clm.GetBuffer().Result.ToBase64();
            _client.Send(_defaultEndpoint, _buff);


            await Task.Delay(5000);

            // Impersonating user, ask for tx status.
            _clm = new ClmManager(new OperationTxStatus("user_test", _lastTxId, ""));
            _buff = _clm.GetBuffer().Result.ToBase64();
            _client.Send(_defaultEndpoint, _buff);

            await Task.Delay(500000000);

            Console.ReadLine();
        }
    }
}
