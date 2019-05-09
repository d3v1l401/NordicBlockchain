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
    public class Deserializer : IStreamProcessor {
        private string readString(BinaryReader _reader) {
            if (_reader != null) {

                var _strSize = _reader.ReadInt16();

                // If empty string, leave it empty.
                if (_strSize == 0)
                    return "";

                if (_strSize > 0 && _strSize < (Int16.MaxValue - 1)) {
                    var _asString = new string(_reader.ReadChars(_strSize));
                    return _asString;
                } else
                    throw new MalformedCLMPacket("String size is null or exceeds maximum value.");

            } else
                throw new IllegalStreamOperation("Read string called over an empty memory reader.");
        }

        public async Task<IOperation> Process(byte[] _buffer, IOperation.OPERATION_TYPE _type) {

            if (_buffer == null || _buffer.Length > (UInt32.MaxValue - Cryptography.Sha256.HASH_SIZE))
                throw new MalformedCLMPacket("Buffer either null or too small (" + _buffer == null ? "null" : _buffer.Length + ")");

            IOperation _rawClass = null;

            using (MemoryStream stream = new MemoryStream(_buffer)) {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    // Unpack data by type
                    var _opType = (short)0;
                    var _author = "";
                    var _data = "";
                    var _signature = "";

                    switch (_type) {

                        case IOperation.OPERATION_TYPE.TRANSACTION_REQUEST:

                            _opType = reader.ReadInt16();
                            if (_opType != (short)IOperation.OPERATION_TYPE.TRANSACTION_REQUEST)
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationTransaction).ToString() + " but message optype is " + _opType + ".");

                            _author     = this.readString(reader);
                            _data       = this.readString(reader);
                            _signature  = this.readString(reader);

                            _rawClass = new OperationTransaction(_author, _data, _signature);

                            break;
                        case IOperation.OPERATION_TYPE.BROADCAST_NEW_BLOCK:

                            // TODO
                            break;
                        case IOperation.OPERATION_TYPE.AUTHENTICATE_REQUEST:

                            _opType = reader.ReadInt16();
                            if (_opType != (short)IOperation.OPERATION_TYPE.AUTHENTICATE_REQUEST)
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationAuthRequest).ToString() + " but message optype is " + _opType + ".");

                            _author = this.readString(reader);
                            _data = this.readString(reader);
                            _signature = this.readString(reader);

                            _rawClass = new OperationAuthRequest(_author, _data, _signature);

                            if (!ClientAuthenticator.ClientAuthenticator.Verify(_signature, _author.ToByteArray().ToBase64(), "miner_test"))
                                return null;

                            break;
                        case IOperation.OPERATION_TYPE.AUTHENTICATE_RESPONSE:

                            _opType = reader.ReadInt16();
                            if (_opType != (short)IOperation.OPERATION_TYPE.AUTHENTICATE_RESPONSE)
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationAuthAck).ToString() + " but message optype is " + _opType + ".");

                            _author = this.readString(reader);
                            _data = this.readString(reader);
                            _signature = this.readString(reader);

                            _rawClass = new OperationAuthAck(_author, _data, _signature);

                            break;
                        default:
                            throw new IllegalStreamOperation(string.Format("Unknown packet type {0}", _type));
                    }
                }
            }

            return _rawClass;
        }

        public async Task<byte[]> Process(IOperation _operation)
         => throw new IllegalStreamOperation("Called serialization process in deserialization class.");
        
    }
}

