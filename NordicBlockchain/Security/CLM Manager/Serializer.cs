using Nordic.Blockchain.Operations;
using Nordic.Exceptions;
using Nordic.Extensions;
using Nordic.Security.Cryptography;
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
            byte[] _messageBuffer = null;
            byte[] _selectedRangeBuffer = null;
            MemoryStream _memory = null;
            short _type = (short)_operation.OperationID;
            var _sizeSoFar = 0;

            using (_memory = new MemoryStream()) {
                using (BinaryWriter _writer = new BinaryWriter(_memory)) {

                    // Message buffer
                    new Switch(_operation)
                        .Case<OperationTransaction> (action => {

                            _writer.Write(_type);
                            _sizeSoFar += sizeof(short);
                            this.writeString(_writer, _operation.GetAuthor());
                            _sizeSoFar += _operation.GetAuthor().Length + sizeof(short);
                            this.writeString(_writer, _operation.OperationData);
                            _sizeSoFar += _operation.OperationData.Length + sizeof(short);
                            this.writeString(_writer, _operation.GetSignature());
                            _sizeSoFar += _operation.GetSignature().Length + sizeof(short);

                        })
                        .Case<OperationAuthRequest> (action => {

                            _writer.Write(_type);
                            _sizeSoFar += sizeof(short);
                            this.writeString(_writer, _operation.GetAuthor());
                            _sizeSoFar += _operation.GetAuthor().Length + sizeof(short);
                            this.writeString(_writer, _operation.OperationData);
                            _sizeSoFar += _operation.OperationData.Length + sizeof(short);
                            this.writeString(_writer, _operation.GetSignature());
                            _sizeSoFar += _operation.GetSignature().Length + sizeof(short);

                        })
                        .Case<OperationNewBlock> (action => {


                        });

                    // Write Hash
                    var _toHash = new byte[_memory.Length];
                    Array.Copy(_memory.GetBuffer(), 0, _toHash, 0, _memory.Length);

                    Sha256 _sha = new Sha256();
                    _sha.Enqueue(_toHash);
                    _writer.Write(_sha.Finalize());
                }
            }

            // now put size in new buffer
            using (MemoryStream _final = new MemoryStream()) {
                using (BinaryWriter _writer = new BinaryWriter(_final)) {
                    _writer.Write((UInt32)_sizeSoFar);
                    _writer.Write(_memory.GetBuffer());

                    _rawBuffer = _final.GetBuffer();


                    _selectedRangeBuffer = new byte[_sizeSoFar + sizeof(UInt32) + Cryptography.Sha256.HASH_SIZE];
                    Array.Copy(_rawBuffer, _selectedRangeBuffer, _selectedRangeBuffer.Length);
                }
            }

            return _selectedRangeBuffer;
        }
    }
}
