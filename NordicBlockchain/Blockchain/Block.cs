using Newtonsoft.Json;
using Nordic.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Nordic.Blockchain
{
    public class Block {
        // For debug purpose I'm forcing to 1 transaction per block, to check automated creation of blocks
        private readonly static int LEDGER_MAX = 1; //500;
        
        public ulong Index { get; set; }
        public DateTime Timestamp { get; set; }
        public string PrevHash { get; set; }
        public string Hash { get; set; }
        //public string NextHash { get; set; } // Enforcing blockchain by double dependency (3 blocks depend on each other) - disabled by default as not implemented.
        public IList<BlockData> Data { get; set; }

        private string CreateHash() {
            Sha256 _sha = new Sha256();
            _sha.Enqueue(Encoding.ASCII.GetBytes($"{Timestamp.ToString()}{PrevHash.ToString() ?? ""}-{Data.ToString() ?? ""}"));

            return _sha.ToString();
        }

        public void UpdateHash()
            => this.Hash = this.CreateHash();

        public string RecalculateHash()
            => this.CreateHash();

        public void AddTransaction(BlockData _tx)
            => this.Data.Add(_tx);

        public bool Add(BlockData _data) {
            // Max entries per block, wait for next block.
            if (this.Data.Count > LEDGER_MAX) 
                return false;

            this.Data.Add(_data);
            this.UpdateHash();
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
            this.Hash = this.CreateHash();
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
            this.Hash = this.CreateHash();
        }

        public Block(Block _prevBlock) {

            this.Data = new List<BlockData>();

            if (_prevBlock == null)
                this.Index = 0;
            else {
                this.Index = _prevBlock.Index + 1;
                this.PrevHash = _prevBlock.Hash;
                this.Hash = this.CreateHash();
            }
            

        }

        public bool IsMaxLedger()
            => this.Data.Count >= LEDGER_MAX ? true : false;

        public override string ToString() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public string ToJson() {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}
