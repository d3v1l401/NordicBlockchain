using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Extensions
{
    public static class EString {
        public static byte[] ToByteArray(this string o) {
            return Encoding.ASCII.GetBytes(o);
        }

        public static byte[] ToByteArrayUTF(this string o) {
            return Encoding.UTF8.GetBytes(o);
        }

        public static byte[] FromBase64(this string o) {
            return Convert.FromBase64String(o);
        }
    }
}
