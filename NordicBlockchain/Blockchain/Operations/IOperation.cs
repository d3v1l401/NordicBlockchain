﻿using Nordic.Extensions;
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
            // Request for transaction status.
            TRANSACTION_STATUS_REQ          = 0x1021,
            TRANSACTION_STATUS_ACK          = 0x1022,

            SECURITY_BC_COMPROMISE_NOTICE   = 0x4001,

            OPERATION_GENESIS_BLOCK         = 0xD301,

            AUTHENTICATE_REQUEST            = 0x5001,
            AUTHENTICATE_RESPONSE           = 0x5002,

            PENDING_OPERATION_REQ           = 0x6001,
            PENDING_OPERATION_ACK           = 0x6002,

            OPERATION_STATS_REQ             = 0x7001,
            OPERATION_STATS_ACK             = 0x7101,

            OPERATION_NONE                  = 0xFFFF - 1
        };

        public static OPERATION_TYPE GetOperationType(UInt16 _type) 
            => (OPERATION_TYPE)_type;

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
                .Case<OperationAuthRequest>
                    (action => { _opType.OperationID = OPERATION_TYPE.AUTHENTICATE_REQUEST; })
                .Case<OperationConfirmTx>
                    (action => { _opType.OperationID = OPERATION_TYPE.TRANSACTION_MINER_CONFIRM; })
                .Case<OperationPendingRequest>
                    (action => { _opType.OperationID = OPERATION_TYPE.PENDING_OPERATION_REQ; })
                .Case<OperationAuthAck>
                    (action => { _opType.OperationID = OPERATION_TYPE.AUTHENTICATE_RESPONSE; })
                .Case<OperationStatsRequest>
                    (action => { _opType.OperationID = OPERATION_TYPE.OPERATION_STATS_REQ; })
                .Case<OperationStatsAck>
                    (action => { _opType.OperationID = OPERATION_TYPE.OPERATION_STATS_ACK; })
                .Case<OperationTxStatus>
                    (action => { _opType.OperationID = OPERATION_TYPE.TRANSACTION_STATUS_REQ; })
                .Case<OperationTxStatusAck>
                    (action => { _opType.OperationID = OPERATION_TYPE.TRANSACTION_STATUS_ACK; })
                .Case<OperationPendingAck>
                    (action => { _opType.OperationID = OPERATION_TYPE.PENDING_OPERATION_ACK; });

    }
}


