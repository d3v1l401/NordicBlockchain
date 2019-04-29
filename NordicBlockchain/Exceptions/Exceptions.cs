using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Exceptions
{
    public class TamperedClmPacket : Exception {
        public TamperedClmPacket(string message) : base(string.Format("---- SECURITY: {0}", message)) {

        }
    }

    public class MalformedCLMPacket : Exception {
        public MalformedCLMPacket(string message) : base(message) {

        }
    }

    public class IllegalStreamOperation : Exception {
        public IllegalStreamOperation(string message) : base(message)
        {

        }
    }

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
