using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Extensions
{
    public static class EString {
        public static byte[] ToByteArray(this string o) {
            return Encoding.ASCII.GetBytes(o);
        }
    }
}
