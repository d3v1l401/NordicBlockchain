﻿using Nordic.Blockchain.Operations;
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
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationTransaction).ToString() + " but message optype is " + ((IOperation.OPERATION_TYPE)_opType).ToString() + ".");

                            _author     = this.readString(reader);
                            _data       = this.readString(reader);
                            _signature  = this.readString(reader);

                            _rawClass = new OperationTransaction(_author, _data, _signature);

                            break;
                        //case IOperation.OPERATION_TYPE.BROADCAST_NEW_BLOCK:
                        //
                        //    // TODO
                        //    break;
                        case IOperation.OPERATION_TYPE.AUTHENTICATE_REQUEST:

                            _opType = reader.ReadInt16();
                            if (_opType != (short)IOperation.OPERATION_TYPE.AUTHENTICATE_REQUEST)
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationAuthRequest).ToString() + " but message optype is " + ((IOperation.OPERATION_TYPE)_opType).ToString() + ".");

                            _author = this.readString(reader);
                            _data = this.readString(reader);
                            _signature = this.readString(reader);

                            _rawClass = new OperationAuthRequest(_author, _data, _signature);

                            // Check the miner does exist in the public keys database.
                            if (!ClientAuthenticator.ClientAuthenticator.Verify(_author, _signature, _author))
                                return null;

                            break;
                        case IOperation.OPERATION_TYPE.AUTHENTICATE_RESPONSE:

                            _opType = reader.ReadInt16();
                            if (_opType != (short)IOperation.OPERATION_TYPE.AUTHENTICATE_RESPONSE)
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationAuthAck).ToString() + " but message optype is " + ((IOperation.OPERATION_TYPE)_opType).ToString() + ".");

                            _author = this.readString(reader);
                            _data = this.readString(reader);
                            _signature = this.readString(reader);

                            _rawClass = new OperationAuthAck(_author, _data, _signature);

                            break;
                        case IOperation.OPERATION_TYPE.PENDING_OPERATION_REQ:

                            _opType = reader.ReadInt16();
                            if (_opType != (short)IOperation.OPERATION_TYPE.PENDING_OPERATION_REQ)
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationPendingRequest).ToString() + " but message optype is " + ((IOperation.OPERATION_TYPE)_opType).ToString() + ".");

                            _author = this.readString(reader);
                            _data = this.readString(reader);
                            _signature = this.readString(reader);

                            _rawClass = new OperationPendingRequest(_author, _data, _signature);

                            break;
                        case IOperation.OPERATION_TYPE.PENDING_OPERATION_ACK:

                            _opType = reader.ReadInt16();
                            if (_opType != (short)IOperation.OPERATION_TYPE.PENDING_OPERATION_ACK)
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationPendingAck).ToString() + " but message optype is " + ((IOperation.OPERATION_TYPE)_opType).ToString() + ".");

                            _author = this.readString(reader);
                            _data = this.readString(reader);
                            _signature = this.readString(reader);

                            _rawClass = new OperationPendingAck(_author, _data, _signature);

                            break;
                        case IOperation.OPERATION_TYPE.TRANSACTION_MINER_CONFIRM:

                            _opType = reader.ReadInt16();
                            if (_opType != (short)IOperation.OPERATION_TYPE.TRANSACTION_MINER_CONFIRM)
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationConfirmTx).ToString() + " but message optype is " + ((IOperation.OPERATION_TYPE)_opType).ToString() + ".");

                            _author = this.readString(reader);
                            _data = this.readString(reader);
                            _signature = this.readString(reader);

                            _rawClass = new OperationConfirmTx(_author, _data, _signature);

                            break;
                        case IOperation.OPERATION_TYPE.OPERATION_STATS_REQ:


                            _opType = reader.ReadInt16();
                            if (_opType != (short)IOperation.OPERATION_TYPE.OPERATION_STATS_REQ)
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationStatsRequest).ToString() + " but message optype is " + ((IOperation.OPERATION_TYPE)_opType).ToString() + ".");

                            _author = this.readString(reader);
                            _data = this.readString(reader);
                            _signature = this.readString(reader);

                            _rawClass = new OperationStatsRequest(_author, _data, _signature);

                            break;
                        case IOperation.OPERATION_TYPE.OPERATION_STATS_ACK:

                            _opType = reader.ReadInt16();
                            if (_opType != (short)IOperation.OPERATION_TYPE.OPERATION_STATS_ACK)
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationStatsAck).ToString() + " but message optype is " + ((IOperation.OPERATION_TYPE)_opType).ToString() + ".");

                            _author = this.readString(reader);
                            _data = this.readString(reader);
                            _signature = this.readString(reader);

                            _rawClass = new OperationStatsAck(_author, _data, _signature);

                            break;

                        case IOperation.OPERATION_TYPE.TRANSACTION_STATUS_REQ:

                            _opType = reader.ReadInt16();
                            if (_opType != (short)IOperation.OPERATION_TYPE.TRANSACTION_STATUS_REQ)
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationTxStatus).ToString() + " but message optype is " + ((IOperation.OPERATION_TYPE)_opType).ToString() + ".");

                            _author = this.readString(reader);
                            _data = this.readString(reader);
                            _signature = this.readString(reader);

                            _rawClass = new OperationTxStatus(_author, _data, _signature);

                            break;

                        case IOperation.OPERATION_TYPE.TRANSACTION_STATUS_ACK:

                            _opType = reader.ReadInt16();
                            if (_opType != (short)IOperation.OPERATION_TYPE.TRANSACTION_STATUS_ACK)
                                throw new MalformedCLMPacket("Requested deserialization of " + typeof(OperationTxStatusAck).ToString() + " but message optype is " + ((IOperation.OPERATION_TYPE)_opType).ToString() + ".");

                            _author = this.readString(reader);
                            _data = this.readString(reader);
                            _signature = this.readString(reader);

                            _rawClass = new OperationTxStatusAck(_author, _data, _signature);

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

