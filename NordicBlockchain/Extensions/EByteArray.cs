using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Extensions
{
    public static class EByteArray {
        public static string ToBase64(this byte[] _array) {
            if (_array.Length == 0)
                return "";

            return Convert.ToBase64String(_array, Base64FormattingOptions.None);
        }
    }
}
