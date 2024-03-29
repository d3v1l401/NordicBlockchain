﻿using Newtonsoft.Json;
using Nordic.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Blockchain.Operations
{
    public class OperationStatsAck : IOperation {
        private string _txIdentifier = string.Empty;

        public OperationStatsAck(string _author, string _data, string _signature) {
            base.OperationAuthor = _author;
            base.OperationData = _data;
            base.Signature = _signature;

            __assignOpId(this);
        }

        public override string GetAuthor()
            => base.OperationAuthor;

        public override byte[] GetData()
            => base.OperationData.FromBase64();

        public override OPERATION_TYPE GetID()
            => base.OperationID;

        public override string GetSignature()
            => base.Signature;

        public override string ToString()
            => JsonConvert.SerializeObject(this);
    }
}
