using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Exceptions
{
    public class UnsupportedProtocolException: Exception {
        public UnsupportedProtocolException(string message) : base(message) {

        }
    }

    public class NullServiceProviderException: Exception {
        public NullServiceProviderException(string message): base (message) {
            
        }
    }

    public class RSADecodeException : Exception {
        public RSADecodeException(string message): base(message) {

        }
    }

    public class RSAProviderException: Exception {
        public RSAProviderException(string message): base(message) {
            
        }
    }

    public class VaultEntryNotFound: Exception {
        public VaultEntryNotFound(string message): base(message)  {

        }
    }
}
