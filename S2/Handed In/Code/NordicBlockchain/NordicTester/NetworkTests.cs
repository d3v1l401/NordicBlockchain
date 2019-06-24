using Nordic.Blockchain.Operations;
using Nordic.Extensions;
using Nordic.Network;
using Nordic.Network.Client;
using Nordic.Security.ClientAuthenticator;
using Nordic.Security.CLM_Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace NordicTester
{
    public class NetworkTests {

        private static readonly string endpoint = "ws://127.0.0.1:1337/blt";

        [Fact]
        public void TestServer() {
            Network _net = new Network("127.0.0.1", 1337);
            Assert.NotNull(_net);

            Assert.True(_net.Setup());
            _net.Start();

            _net.Stop();
        }

        [Fact]
        public void TestClient() {
            ClientAuthenticator.Initialize("miner_pubKey.pem", "miner_privKeyOut.pem", "");
            ClientAuthenticator.Add("node", File.ReadAllText("pubKey.pem"));
            ClientAuthenticator.Add("unit_test", File.ReadAllText("miner_pubKey.pem"));

            Network _net = new Network("127.0.0.1", 1337);
            Assert.NotNull(_net);

            Assert.True(_net.Setup());
            _net.Start();

            Client _client = new Client();
            _client.Connect(endpoint);

            // Server shouldn't care of this packet.
            _client.Send(endpoint, "thisisnotvaliddata".ToByteArray().ToBase64());

            // But should for this one.
            IOperation _auth = new OperationAuthRequest("unit_test", "", ClientAuthenticator.Sign("unit_test"));
            ClmManager _clm = new ClmManager(_auth);
            var _buff = _clm.GetBuffer().Result.ToBase64();
            _client.Send(endpoint, _buff);

            // Test fails with an exception if it fails at all.

            _net.Stop();
        }

        [Fact]
        public async System.Threading.Tasks.Task TestCLMAsync() {
            // Reusing old generated keys.
            ClientAuthenticator.Initialize("miner_pubKey.pem", "miner_privKeyOut.pem", "");
            ClientAuthenticator.Add("node", File.ReadAllText("pubKey.pem"));
            ClientAuthenticator.Add("unit_test", File.ReadAllText("miner_pubKey.pem"));

            ClmManager _clm = new ClmManager(new OperationAuthRequest("unit_test", "", ClientAuthenticator.Sign("unit_test")));
            Assert.NotNull(_clm);

            var _authReqBuffer = _clm.GetBuffer().Result;
            Assert.NotNull(_authReqBuffer);

            _clm = new ClmManager(_authReqBuffer.ToBase64().ToByteArray());
            Assert.NotNull(_clm);

            var _back = await _clm.GetClass();
            Assert.NotNull(_back);

            Assert.Equal(_back.OperationAuthor, "unit_test");
            Assert.True(string.IsNullOrEmpty(_back.OperationData));
        }
    }
}
