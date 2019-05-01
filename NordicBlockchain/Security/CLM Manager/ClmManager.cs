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
    public class ClmManager {

        private IOperation.OPERATION_TYPE _operation = IOperation.OPERATION_TYPE.OPERATION_NONE;
        private byte[]                    _rawBuff = null;
        private IOperation                _class = null;

        ~ClmManager() {
            this._operation = IOperation.OPERATION_TYPE.OPERATION_NONE;
        }

        public async Task<IOperation> GetClass() {
            if (this._rawBuff != null && this._operation != IOperation.OPERATION_TYPE.OPERATION_NONE)
                return await new Deserializer().Process(this._rawBuff, this._operation);
           
            return null;
        }

        public async Task<byte[]> GetBuffer() {
            return await new Serializer().Process(this._class);
        }

        public ClmManager(IOperation _toSerialize)
            => this._class = _toSerialize ?? throw new MalformedCLMPacket("Specified null class to serialize");

        public ClmManager(byte[] _toDeserialize) {
            if (_toDeserialize != null && _toDeserialize.Length >= 0) {

                using (MemoryStream stream = new MemoryStream(_toDeserialize)) {
                    using (BinaryReader reader = new BinaryReader(stream)) {

                        var _messageSize = reader.ReadInt32();
                        if (!(_messageSize > 0) || (_messageSize > (Int32.MaxValue - Sha256.HASH_SIZE)))
                            throw new MalformedCLMPacket("Message size is null or exceeds packet length (" + _messageSize + ").");

                        var _messageBuffer = reader.ReadBytes(_messageSize);

                        if (_messageBuffer == null || !(_messageBuffer.Length > 0) || _messageBuffer.Length != _messageSize)
                            throw new MalformedCLMPacket(string.Format("Message size discrepancy from message size {0} != {1}", _messageSize, _messageBuffer == null ? 0 : _messageBuffer.Length));

                        var _hash = reader.ReadBytes(Sha256.HASH_SIZE);

                        // Check the hash
                        Sha256 _digest = new Sha256();
                        _digest.Enqueue(_messageBuffer);
                        var _real = _digest.Finalize();

                        if (!Array.Equals(_digest, _real))
                            throw new TamperedClmPacket(string.Format("Tampered packet {0} != {1}", _digest.ToString(), _real.ToString()));

                        // Determine message type

                        var _opType = reader.ReadUInt16();

                        this._operation = IOperation.GetOperationType(_opType);

                        // Stop here, delegate the message to Process()
                        this._rawBuff = _messageBuffer;
                    }
                }

            } else
                throw new MalformedCLMPacket("Empty buffer");
        }

        public override string ToString() {
            return base.ToString();
        }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }
    }
}
