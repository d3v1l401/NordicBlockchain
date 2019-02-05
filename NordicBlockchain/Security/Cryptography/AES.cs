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
            this.SetIV(_iv.ToByteArray());
            this.SetKey(_key.ToByteArray());
            this._aes = Aes.Create();
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
                    using (StreamWriter sWriter = new StreamWriter(crStream)) {
                        sWriter.Write(_input);
                    }

                    return memStream.ToArray();
                }
            }
        }

        public void SetIV(byte[] _iv) {
            if (_iv != null || _iv.Length != 0)
                this._aes.IV = _iv;
            else
                this._aes.GenerateIV();
        }

        public void SetKey(byte[] _key) {
            if (_key != null || _key.Length != 0)
                this._aes.Key = _key;
            else
                this._aes.GenerateKey();
        }
    }
}
