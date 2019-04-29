using Nordic.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Nordic.Extensions;

namespace Nordic.Security.Cryptography
{
    public class Sha256 : IHash {

        public static int HASH_SIZE = 64;

        private SHA256 _sha256;
        private List<byte> _buffer;
        public Sha256() {
            this._sha256 = SHA256.Create();
            this._buffer = new List<byte>();
        }
        
        public void Clear() 
           => this._buffer.Clear();

        public void Dequeue(int _count) {
            for (var i = 0; i < _count; i++)
                this._buffer.RemoveAt(this._buffer.Count);
        }

        public void Enqueue(byte[] _buffer) {
            foreach (var b in _buffer)
                this._buffer.Add(b);
        }

        public override string ToString() 
            => Convert.ToBase64String(this.Finalize(), Base64FormattingOptions.None);

        public byte[] Finalize()
            => this._sha256.ComputeHash(this._buffer.ToArray());
    }
}
