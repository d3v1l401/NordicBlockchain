﻿using Newtonsoft.Json;
using Nordic.Extensions;
using Nordic.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Blockchain.Operations
{
    public class OperationTransaction : IOperation {

        private int         _confirmCounter     = 0;
        private string      _txIdentifier       = string.Empty;
        private DateTime    _queueDate          = DateTime.Now;

        public OperationTransaction(string _author, string _data, string _signature) {
            base.OperationAuthor = _author;
            base.OperationData = _data;
            base.Signature = _signature;

            Sha256 _sha = new Sha256();
            _sha.Enqueue((_author + "-" + _signature + "-" + this._queueDate.ToString()).ToByteArray());
            this._txIdentifier = _sha.Finalize().ToBase64();

            // Auto-determine type & assign proper ID.
            __assignOpId(this);
        }

        public string GetIdentifier()
            => this._txIdentifier;

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
            => JsonConvert.SerializeObject(this); // This is never exposed outside the node!
    }
}
