using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Nordic.Extensions;
using Nordic.Exceptions;
using System.IO;
using PemUtils;

// beginor's
// https://gist.github.com/beginor/0d0acd7304c0e1d98d89e687aa8322e1
// ASN.1 encoded keys compatibility layer for OpenSSL's generated keys.

namespace Nordic.Security.Cryptography
{
    public class RSA {

        public override string ToString()
            => this._pubKeyCopy;

        private RSACryptoServiceProvider _privateKeyRsaProvider;
        private RSACryptoServiceProvider _publicKeyRsaProvider;
        private string _pubKeyCopy = string.Empty;

        public string Sign(string _input) {
            return this._privateKeyRsaProvider.SignData(_input.ToByteArray(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1).ToBase64();
        }

        public bool VerifySignature(string _input, string _signature, RSACryptoServiceProvider _signerPubKey) {
            if (_signerPubKey == null)
                return this._publicKeyRsaProvider.VerifyData(_input.ToByteArray(), _signature.FromBase64(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            else
                return _signerPubKey.VerifyData(_input.ToByteArray(), _signature.FromBase64(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public RSACryptoServiceProvider GetPublicKey()
            => this._publicKeyRsaProvider;

        public string Decrypt(string _input) {
            if (_privateKeyRsaProvider != null)
                return _decrypt(_input);
            else throw new NullServiceProviderException("Private RSA Provider = nullptr");
        }

        public string Encrypt(string _input) {
            if (_publicKeyRsaProvider != null)
                return _encrypt(_input);
            else throw new NullServiceProviderException("Private RSA Provider = nullptr");
        }
        private string _encrypt(string _input) {
            byte[] _data = _input.ToByteArrayUTF();
            int _maxBlockSize = this._publicKeyRsaProvider.KeySize / 8 - 11;

            if (_data.Length <= _maxBlockSize)
                return this._publicKeyRsaProvider.Encrypt(_data, false).ToBase64();
            else {
                // Need to chunk
                using (MemoryStream _memStream = new MemoryStream(_data))
                using (MemoryStream _crStream = new MemoryStream()) {
                    byte[] _buffer = new byte[_maxBlockSize];
                    int _blockSize = _memStream.Read(_buffer, 0, _maxBlockSize);

                    while (_blockSize > 0) {
                        byte[] _toEncrypt = new byte[_blockSize];
                        Array.Copy(_buffer, 0, _toEncrypt, 0, _blockSize);

                        byte[] _encrypted = this._publicKeyRsaProvider.Encrypt(_toEncrypt, false);
                        _crStream.Write(_encrypted, 0, _encrypted.Length);

                        _blockSize = _memStream.Read(_buffer, 0, _maxBlockSize);
                    }

                    return _crStream.ToArray().ToBase64();
                }
            }
        }
        private string _decrypt(string _input) {
            byte[] _data = _input.ToByteArrayUTF();
            int _maxBlockSize = this._privateKeyRsaProvider.KeySize / 8;

            if (_data.Length <= _maxBlockSize)
                return this._privateKeyRsaProvider.Decrypt(_data, false).ToBase64();
            else {
                using (MemoryStream _crStream = new MemoryStream(_data))
                using (MemoryStream _memStream = new MemoryStream()) {
                    byte[] _buffer = new byte[_maxBlockSize];
                    int _blockSize = _memStream.Read(_buffer, 0, _maxBlockSize);

                    while (_blockSize > 0) {
                        byte[] _toDecrypt = new byte[_blockSize];
                        Array.Copy(_buffer, 0, _toDecrypt, 0, _blockSize);

                        byte[] _decrypted = this._publicKeyRsaProvider.Decrypt(_toDecrypt, false);
                        _crStream.Write(_decrypted, 0, _decrypted.Length);

                        _blockSize = _memStream.Read(_buffer, 0, _maxBlockSize);
                    }

                    return _memStream.ToArray().ToBase64();
                }
            }
        }

        private RSACryptoServiceProvider createProviderFromPrivKey(string _privKey) {
            var privKeyBits = _privKey.ToByteArrayUTF();
            var rsa = new RSACryptoServiceProvider();
            RSAParameters _params;

            using (var stream = new MemoryStream(privKeyBits))
            using (var reader = new PemReader(stream)) {
                _params = reader.ReadRsaKey();
            }

            rsa.ImportParameters(_params);
            return rsa;
        }

        private RSACryptoServiceProvider createProviderFromPubKey(string _pubKey) {
            var pubKeyBits = _pubKey.ToByteArrayUTF();
            var rsa = new RSACryptoServiceProvider();
            RSAParameters _params;

            using (var stream = new MemoryStream(pubKeyBits))
            using (var reader = new PemReader(stream)) {
                _params = reader.ReadRsaKey();
            }

            this._pubKeyCopy = _pubKey;
            rsa.ImportParameters(_params);
            return rsa;
        }

        public RSA(string _privateKey, string _publicKey) {
            if (!string.IsNullOrEmpty(_privateKey))
                this._privateKeyRsaProvider = this.createProviderFromPrivKey(_privateKey);
            
            if (!string.IsNullOrEmpty(_publicKey))
                this._publicKeyRsaProvider = this.createProviderFromPubKey(_publicKey);
        }
    }
}
