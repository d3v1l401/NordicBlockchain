using Nordic.Blockchain.Operations;
using Nordic.Exceptions;
using Nordic.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Security.CLM_Manager
{
    public class Serializer<T> : IStreamProcessor {
        public IOperation Process(byte[] _buffer, IOperation.OPERATION_TYPE _type) 
            => throw new IllegalStreamOperation("Called deserialization process in serialization class.");
        

        public byte[] Process(IOperation _operation) {
            byte[] _rawBuffer = null;

            new Switch(_operation)
                .Case<OperationTransaction> (action => {



                })
                .Case<OperationNewBlock> (action => {


                });

            return _rawBuffer;
        }
    }
}
