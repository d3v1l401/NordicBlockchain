using Newtonsoft.Json;
using Nordic.Blockchain.Operations;
using Nordic.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Blockchain
{
    public class BlockData {
        public IOperation _operation;

        public BlockData(IOperation _operation) {
            switch (_operation.GetID()) {
                case IOperation.OPERATION_TYPE.TRANSACTION_REQUEST:

                    this._operation = _operation;

                    break;
                case IOperation.OPERATION_TYPE.OPERATION_GENESIS_BLOCK:

                    this._operation = new Operations.OperationTransaction("", "Let there be light and JSON", "d3vil401");

                    break;
                case IOperation.OPERATION_TYPE.BROADCAST_NEW_BLOCK:

                    this._operation = new Operations.OperationNewBlock(_operation.GetAuthor(), _operation.GetData().ToStringBuffer(), _operation.Signature);

                    break;
            }
        }

        public BlockData(string _author, IOperation.OPERATION_TYPE _opc, string _signature) {
            switch (_opc) {
                case IOperation.OPERATION_TYPE.OPERATION_GENESIS_BLOCK:

                    this._operation = new Operations.OperationTransaction("", "Let there be light and JSON", "d3vil401");

                    break;
                case IOperation.OPERATION_TYPE.BROADCAST_NEW_BLOCK:

                    this._operation = new Operations.OperationNewBlock(_author, "", "d3vil401");

                    break;
            }
        }

        public override string ToString() 
            => this._operation.ToString();
        
    }
}
