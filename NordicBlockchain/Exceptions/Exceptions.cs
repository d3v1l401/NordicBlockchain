using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Exceptions
{
    public class UnsupportedProtocolException: Exception {
        public UnsupportedProtocolException(string message) : base(message) {

        }
    }
}
