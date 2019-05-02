using Nordic.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Blockchain.Operations
{
    public abstract class IOperation {
        // ID assigned to action.
        public enum OPERATION_TYPE {
            TRANSACTION_REQUEST             = 0x1001,
            //TRANSACTION_MANUAL_CONFIRM      = 0x1011,
            //TRANSACTION_MANUAL_REJECT       = 0x1012,
            TRANSACTION_MINER_CONFIRM       = 0x1014,

            BROADCAST_NEW_BLOCK             = 0x2001,
            BROADCAST_CLOSED_BLOCK          = 0x2010,

            SECURITY_BC_COMPROMISE_NOTICE   = 0x4001,

            OPERATION_GENESIS_BLOCK         = 0xD301,

            OPERATION_NONE                  = 0xFFFF - 1
        };

        public static OPERATION_TYPE GetOperationType(UInt16 _type) {
            return (OPERATION_TYPE)_type;
        }

        public string            OperationAuthor { get; set; }    // Author token
        public OPERATION_TYPE    OperationID { get; set; }
        public string            OperationData { get; set; }      // Base64 Encoded String for data, avoiding binary length determination.
        public string            Signature { get; set; }

        public abstract byte[] GetData();
        public abstract string GetAuthor();
        public abstract OPERATION_TYPE GetID();
        public abstract string GetSignature();
        public abstract override string ToString();

        protected static void __assignOpId(IOperation _opType)
           => new Switch(_opType)
                .Case<OperationTransaction>
                    (action => { _opType.OperationID = OPERATION_TYPE.TRANSACTION_REQUEST; })
                .Case<OperationNewBlock>
                    (action => { _opType.OperationID = OPERATION_TYPE.BROADCAST_NEW_BLOCK; });
    }
}


