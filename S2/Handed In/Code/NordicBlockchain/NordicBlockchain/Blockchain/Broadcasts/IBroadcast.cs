using Nordic.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Blockchain.Broadcasts
{
    public abstract class IBroadcast
    {
        // ID assigned to broadcast.
        public enum BROADCAST_TYPE {
            BROADCAST_AUTH_REQ          = 0x6101,
            BROADCAST_AUTH_ACK          = 0x6102,

            BROADCAST_NEWBLOCK          = 0x2101,
            BROADCAST_SYNCH_ACK         = 0x2102,

            BROADCAST_COMPROMISE_KEY    = 0x3001,
            BROADCAST_COMPROMISE_BC     = 0x3010,

            BROADCAST_NONE = 0xFFFF - 1
        };

        public static BROADCAST_TYPE GetOperationType(UInt16 _type) {
            return (BROADCAST_TYPE)_type;
        }

        public string BroadcastAuthority { get; set; }
        public BROADCAST_TYPE BroadcastID { get; set; }
        public string BroadcastData { get; set; }
        public string Signature { get; set; }

        public abstract byte[] GetData();
        public abstract string GetAuthor();
        public abstract BROADCAST_TYPE GetID();
        public abstract string GetSignature();
        public abstract override string ToString();

        protected static void __assignOpId(IBroadcast _opType)
           => new Switch(_opType)
                .Case<BroadcastCompromise>
                    (action => { _opType.BroadcastID = BROADCAST_TYPE.BROADCAST_COMPROMISE_BC; });

    }
}
