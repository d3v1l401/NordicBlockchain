using Nordic.Blockchain;
using Nordic.Blockchain.Operations;
using Nordic.Security.Cryptography;
using System;
using System.IO;
using Xunit;

namespace NordicTester
{
    public class CryptoTests
    {

        [Fact]
        public void TestRSA()
        {
            Assert.True(File.Exists("pubKey.pem"), "Public key does not exist.");
            Assert.True(File.Exists("privKey.pem"), "Private key does not exist.");

            RSA _rsa = new RSA(File.ReadAllText("privKey.pem"), File.ReadAllText("pubKey.pem"));
            var _signature = _rsa.Sign("makeAwish");
            var _verify = _rsa.VerifySignature("makeAwish", _signature, null);
            var _verify2 = _rsa.VerifySignature("makeAwisha", _signature, null);

            Assert.True(_verify, "Signature verification failed.");
            Assert.False(_verify2, "Signature is valid, but should not.");
        }

        [Fact]
        public void TestBlock()
        {

        }
    }
}
