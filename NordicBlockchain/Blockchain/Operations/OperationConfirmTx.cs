using Newtonsoft.Json;
using Nordic.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Blockchain.Operations
{
    public class OperationConfirmTx : IOperation
    {

        private int _confirmCounter = 0;
        public OperationConfirmTx(string _author, string _data, string _signature)
        {
            base.OperationAuthor = _author;
            base.OperationData = _data; // Tx ID
            base.Signature = _signature;

            // Auto-determine type & assign proper ID.
            __assignOpId(this);
        }

        public void Confirm() {
            if (this._confirmCounter < Int32.MaxValue)
                this._confirmCounter++;
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
