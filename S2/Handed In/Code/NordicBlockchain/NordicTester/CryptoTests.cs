using Nordic.Blockchain;
using Nordic.Blockchain.Operations;
using Nordic.Extensions;
using Nordic.Security.Cryptography;
using Nordic.Security.ServerAuthenticator;
using System;
using System.IO;
using Xunit;

namespace NordicTester
{
    public class CryptoTests
    {
        //[Fact] 
        //public void TestAuthenticators() {
        //    ServerAuthenticator.Initialize("pubKey.pem", "privKey.pem", null);
        //    var signated = ServerAuthenticator.Sign("Signed").ToByteArray().ToBase64();
        //
        //    Assert.True(!string.IsNullOrEmpty(signated) && signated.Length > 0);
        //
        //    // Here we have a problem with the class, apparently the data verification fails because the output buffer from
        //    // the Sign() function uses UTF8, but acquiring the buffer as UTF8 encoding still fails after transmission over the WebSocket.
        //    Assert.True(ServerAuthenticator.Verify(signated, "Signed".ToByteArray().ToBase64(), "_this_"));
        //    Assert.False(ServerAuthenticator.Verify(signated, "NotSigned".ToByteArray().ToBase64(), "_this_"));
        //}

        [Fact]
        public void TestRSA()
        {
            Assert.True(File.Exists("pubKey.pem"));
            Assert.True(File.Exists("privKey.pem"));

            RSA _rsa = new RSA(File.ReadAllText("privKey.pem"), File.ReadAllText("pubKey.pem"));
            var _signature = _rsa.Sign("After four years of carrying books to school, you're well prepared for a career in backpacking.");
            var _verify = _rsa.VerifySignature("After four years of carrying books to school, you're well prepared for a career in backpacking.", _signature, null);
            var _verify2 = _rsa.VerifySignature("After four two of carrying books to school, you're well prepared for a career in backpacking.", _signature, null);

            Assert.True(_verify, "Signature verification failed.");
            Assert.False(_verify2, "Signature is valid, but should not.");
        }

        [Fact]
        public void TestSHA256() {
            Sha256 _sha = new Sha256();
            Assert.NotNull(_sha);

            _sha.Enqueue("I write".ToByteArray());

            var hash = _sha.Finalize().ToBase64();
            Assert.True(hash == "7XQ6jvWaKE8mvlCBzt9J5yhqogGRm3ibU9RlARgfLO0=");

            _sha.Dequeue("write".Length);
            _sha.Enqueue("read".ToByteArray());

            var _sha2 = new Sha256();
            _sha2.Enqueue("I read".ToByteArray());
            var readHash = _sha2.Finalize().ToBase64();

            hash = _sha.Finalize().ToBase64();
            Assert.True(hash == readHash);
            Assert.True(readHash == "Xg1zevUH51M9TiiOzOgGJ9ye2QZW7S424V6tdL3O2ws=");
        }
    }
}
