using Nordic.Blockchain.Operations;
using Nordic.Exceptions;
using Nordic.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Nordic.Security.CLM_Manager
{
    public class Serializer : IStreamProcessor {
        public async Task<IOperation> Process(byte[] _buffer, IOperation.OPERATION_TYPE _type) 
            => throw new IllegalStreamOperation("Called deserialization process in serialization class.");

        private void writeString(BinaryWriter _writer, string _data) {
            _writer.Write((short)_data.Length);
            _writer.Write(_data.ToByteArray());
        }

        private byte[] createEncapsulation(byte[] _message) {
            return _message;
        }

        public async Task<byte[]> Process(IOperation _operation) {
            byte[] _rawBuffer = null;
            IOperation.OPERATION_TYPE _type = _operation.OperationID;

            new Switch(_operation)
                .Case<OperationTransaction> (action => {

                    using (MemoryStream _memory = new MemoryStream(_rawBuffer)) {
                        using (BinaryWriter _writer = new BinaryWriter(_memory)) {

                            _writer.Write(_type.Cast<short>());
                            this.writeString(_writer, _operation.GetAuthor());
                            this.writeString(_writer, _operation.GetData().Cast<string>());
                            this.writeString(_writer, _operation.GetSignature());
                        }
                    }

                })
                .Case<OperationNewBlock> (action => {


                });

            return _rawBuffer;
        }
    }
}
