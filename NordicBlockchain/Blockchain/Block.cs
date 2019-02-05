using Newtonsoft.Json;
using Nordic.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Nordic.Blockchain
{
    public class Block {
        private readonly static int LEDGER_MAX = 500;

        public ulong Index { get; set; }
        public DateTime Timestamp { get; set; }
        public string PrevHash { get; set; }
        public string Hash { get; set; }
        public IList<BlockData> Data { get; set; }

        private string CreateHash() {
            Sha256 _sha = new Sha256();
            _sha.Enqueue(Encoding.ASCII.GetBytes($"{Timestamp}{PrevHash ?? ""}{Data}"));

            return _sha.ToString();
        }

        public string RecalculateHash() {
            return CreateHash();
        }

        public bool Add(BlockData _data) {
            // Max entries per block, wait for next block.
            if (this.Data.Count > LEDGER_MAX) 
                return false;

            this.Data.Add(_data);
            return true;
        }

        public Block(DateTime _timeStamp, Block _prevBlock, List<BlockData> _data) {
            this.Index = 0;

            if (_timeStamp == null)
                _timeStamp = DateTime.Now;
            this.Timestamp = _timeStamp;

            if (_prevBlock != null)
                this.PrevHash = _prevBlock.Hash;
            else
                this.PrevHash = string.Empty;
            
            this.Data = _data;
            this.Hash = CreateHash();
        }

        public Block(DateTime _timeStamp, Block _prevBlock, BlockData _data) {
            this.Index = 0;

            if (_timeStamp == null)
                _timeStamp = DateTime.Now;
            this.Timestamp = _timeStamp;

            if (_prevBlock != null)
                this.PrevHash = _prevBlock.Hash;
            else
                this.PrevHash = string.Empty;

            this.Data = new List<BlockData>();
            this.Data.Add(_data);
            this.Hash = CreateHash();
        }

        public override string ToString() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public string ToJson() {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}
