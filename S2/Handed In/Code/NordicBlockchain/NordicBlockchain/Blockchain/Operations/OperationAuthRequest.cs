using Newtonsoft.Json;
using Nordic.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Blockchain.Operations
{
    public class OperationAuthRequest : IOperation
    {
        public OperationAuthRequest(string _requesterAlias, string _data, string _signature) {
            base.OperationAuthor = _requesterAlias;
            base.OperationData = _data;
            base.Signature = _signature;

            // Auto-determine type & assign proper ID.
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
