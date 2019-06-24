using Nordic.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Nordic.Security.Cryptography
{
    public class AES : IBlockCrypto
    {
        private Aes _aes = null;
        public AES(string _key, string _iv) {
            this._aes = Aes.Create();
            this._aes.Mode = CipherMode.CBC;
            this._aes.BlockSize = 128;
            this._aes.FeedbackSize = 128;
            this._aes.KeySize = 128;

            this.SetIV(_iv.ToByteArray());
            this.SetKey(_key.ToByteArray());
        }

        public byte[] Decrypt(string _input) {
            return this.Decrypt(_input.ToByteArray());
        }

        public byte[] Decrypt(byte[] _input) {
            var _output = string.Empty;
            ICryptoTransform decryptor = this._aes.CreateDecryptor(this._aes.Key, this._aes.IV);
            using (MemoryStream memStream = new MemoryStream(_input)) {
                using (CryptoStream crStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read)) {
                    using (StreamReader sReader = new StreamReader(crStream)) {
                        return sReader.ReadToEnd().ToByteArray();
                    }
                }
            }
        }

        public byte[] Encrypt(string _input) {
            return this.Encrypt(_input.ToByteArray());
        }

        public byte[] Encrypt(byte[] _input) {
            ICryptoTransform encryptor = this._aes.CreateEncryptor(this._aes.Key, this._aes.IV);
            
            using (MemoryStream memStream = new MemoryStream())  {
                using (CryptoStream crStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write)) {
                    crStream.Write(_input, 0, _input.Length);
                }
                return memStream.ToArray();
            }
        }

        public void SetIV(byte[] _iv) {
            if (_iv != null || _iv.Length != 0 || _iv.Length == this._aes.BlockSize / 8)
                this._aes.IV = _iv;
            else
                this._aes.GenerateIV();
        }

        public void SetKey(byte[] _key) {
            if (_key != null || _key.Length != 0 || _key.Length == this._aes.BlockSize / 8)
                this._aes.Key = _key;
            else
                this._aes.GenerateKey();
        }
    }
}
