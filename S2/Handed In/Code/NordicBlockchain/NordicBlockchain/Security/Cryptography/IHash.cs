using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Security.Cryptography
{
    internal interface IHash {
        void Enqueue(byte[] _buffer);
        void Dequeue(int _count);
        void Clear();
        byte[] Finalize();
    }
}
