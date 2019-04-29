using Nordic.Blockchain.Operations;
using Nordic.Extensions;
using Nordic.Security.ServerAuthenticator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nordic.Blockchain
{
    public class Blockchain {
        private IList<Block> _chain { set; get; }
        private IList<BlockData> PendingOperations { get; set; }

        public Blockchain() {
            this.PendingOperations = new List<BlockData>();
            this._chain = new List<Block>();

            this.Add(new BlockData("", IOperation.OPERATION_TYPE.OPERATION_GENESIS_BLOCK, "Nope"));
            this.ProcessPendingOperation("d3vil401");
        }

        public Block LastBlock() {
            if (this._chain.Count == 0)
                return null;

            return this._chain[this._chain.Count - 1];
        }

        public Block GetBlock(ulong _id) 
            => this._chain.Where(x => x.Index == _id).Single();

        public Block[] GetBlocksByTimespan(DateTime _from, DateTime _to) 
            => this._chain.Where(x => x.Timestamp >= _from && x.Timestamp <= _to).ToArray();

        public bool Validity() {
            for (int i = 1; i < this._chain.Count; i++) {
                if (this._chain[i].Hash != this._chain[i].RecalculateHash())
                    return false;

                if (this._chain[i].PrevHash != this._chain[i - 1].Hash)
                    return false;
            }

            return true;
        }
        public bool Validity(Block _block) {
            if (this.GetBlock(_block.Index).Hash != this.GetBlock(_block.Index + 1).PrevHash)
                return false;

            return true;
        }

        public bool Validity(ulong _id) 
            => this.Validity(this.GetBlock(_id));

        public void Add(BlockData _data) {
            Block latestBlock = this.LastBlock();
            var newBlock = new Block(DateTime.Now, this.LastBlock(), _data);

            if (latestBlock != null)
            {
                newBlock.Index = latestBlock.Index + 1;
                newBlock.PrevHash = latestBlock.Hash;
            } else {
                // TODO: GENESIS-ONLY ENFORCE!
                newBlock.Index = 0;
                newBlock.PrevHash = string.Empty;
            }

            this._chain.Add(newBlock);
        }

        public void ProcessPendingOperation(string _minerIdentifier) {
            this.Add(this.PendingOperations.ToList());

            this.PendingOperations.Clear();
            var _signature = ServerAuthenticator.Sign(_minerIdentifier);

            this.Add(new BlockData(_minerIdentifier, IOperation.OPERATION_TYPE.BROADCAST_NEW_BLOCK, _signature));
        }

        public void Add(List<BlockData> _data) {
            Block latestBlock = this.LastBlock();
            var newBlock = new Block(DateTime.Now, this.LastBlock(), _data);

            if (latestBlock != null) {
                newBlock.Index = latestBlock.Index + 1;
                newBlock.PrevHash = latestBlock.Hash;
            } else {
                newBlock.Index = 0;
                newBlock.PrevHash = string.Empty;
            }
            
            this._chain.Add(newBlock);
        }
    }
}
