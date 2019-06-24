using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Security.Cryptography
{
    internal interface IBlockCrypto {
        void SetKey(byte[] _key);
        void SetIV(byte[] _iv);

        byte[] Encrypt(string _input);
        byte[] Encrypt(byte[] _input);


        byte[] Decrypt(string _input);
        byte[] Decrypt(byte[] _input);
    }
}
