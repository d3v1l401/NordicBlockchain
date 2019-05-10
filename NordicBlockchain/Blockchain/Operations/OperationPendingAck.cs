using Newtonsoft.Json;
using Nordic.Extensions;
using Nordic.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Blockchain.Operations
{
    public class OperationPendingAck : IOperation
    {
        private string _txIdentifier = string.Empty;

        public OperationPendingAck(string _author, string _data, string _signature)
        {
            base.OperationAuthor = _author;
            base.OperationData = _data;
            base.Signature = _signature;

            __assignOpId(this);
        }

        public bool AssignTx(OperationTransaction _tx, string _minerPubKey) {

            // Encrypt for the miner?
            try {

                this.OperationData = (_tx.OperationAuthor + "|"             // 0
                    + _tx.Signature + "|"                                   // 1
                    + Convert.ToString(_tx.GetQueueDate().ToOADate()) + "|" // 2
                    + _tx.OperationID + "|"                                 // 3 
                    + Convert.ToString(DateTime.UtcNow.ToOADate()));        // 4
                //RSA _rsa = new RSA(null, _minerPubKey);
                //var _encrypted = _rsa.Encrypt(this.OperationData.ToByteArray().ToStringBuffer());

                return true;

            } catch (Exception ex) {
                Console.WriteLine("Could not encrypt transaction details: " + ex.Message);
            }
           
            return false;
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
