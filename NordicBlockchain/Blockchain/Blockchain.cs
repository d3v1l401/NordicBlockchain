﻿using Nordic.Blockchain.Operations;
using Nordic.Extensions;
using Nordic.Security.ServerAuthenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Nordic.Security.CLM_Manager;
using Nordic.Extensions;
using Newtonsoft.Json;
using System.IO;

namespace Nordic.Blockchain
{
    public class Blockchain {
        private IList<Block> _chain { set; get; }
        private IList<BlockData> PendingOperations { get; set; }
        private Dictionary<string, OperationTransaction> _votemap = new Dictionary<string, OperationTransaction>();

        private static Blockchain __instance = null;

        public static Blockchain getInstance() {
            if (__instance != null)
                return __instance;

            return __instance = new Blockchain();
        }

        public Blockchain() {
            this.PendingOperations = new List<BlockData>();
            this._chain = new List<Block>();

            this.AddSingle(new BlockData("", IOperation.OPERATION_TYPE.OPERATION_GENESIS_BLOCK, "Nope"));
            __instance = this;
            //this.ProcessPendingOperation("d3vil401");
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
            // Can't verify compromise of BC if there's only one block, it will always be true.
            if (this._chain.Count == 1)
                return true;

            for (int i = 1; i < this._chain.Count; i++) {
                if (this._chain[i].Hash != this._chain[i].RecalculateHash())
                    return false;

                if (this._chain[i].PrevHash != this._chain[i - 1].RecalculateHash())
                    return false;
            }

            return true;
        }
        public bool Validity(Block _block) {
            if (this.GetBlock(_block.Index + 1) != null)
                if (this.GetBlock(_block.Index).Hash == this.GetBlock(_block.Index + 1).PrevHash)
                    return true;

            return false;
        }

        public bool Validity(ulong _id) 
            => this.Validity(this.GetBlock(_id));

        public void AddSingle(BlockData _data) {
            var _list = new List<BlockData>();

            if (_data != null)
                _list.Add(_data);

            this.Add(_list);
        }

        public async Task<bool> Vote(string _txId) {
            if (this._votemap.ContainsKey(_txId)) {
                this._votemap[_txId].Confirm();

                if (this._votemap[_txId].IsDecisive())
                    Blockchain.getInstance().ConfirmTransaction(this._votemap[_txId].GetIdentifier());

                return true;
            }

            return false;
        }

        public async Task<OperationTransaction> getTxStatus(string _txId) {
            if (this._votemap.ContainsKey(_txId))
                return this._votemap[_txId];
            
            foreach (var _block in this._chain) {
                foreach (var _blockData in _block.Data) {
                    if (_blockData != null && _blockData._operation.GetID() == IOperation.OPERATION_TYPE.TRANSACTION_REQUEST) {

                        var _tx = (OperationTransaction)(_blockData._operation);
                        if (_tx.GetIdentifier().Equals(_txId))
                            return _tx;
                    }
                }
            }

            return null;
        }

        public async Task<string> ProcessOperation(IOperation _operation, IOperation _reqParam = null) {
            var _responseBuffer = string.Empty;

            new Switch(_operation)
                .Case<OperationConfirmTx> ( action => {

                    var _didVote = this.Vote(_operation.OperationData).Result;
                    if (!_didVote) {
                        // Didn't vote because no tx has such TxID.
                        Console.WriteLine("Couldn't vote for " + _operation.OperationData + " because it's not on votemap.");
                    }

                })
                .Case<OperationTransaction> ( action => {

                    var _data = new BlockData(_operation);
                    this.PendingOperations.Add(_data);
                    this._votemap[_operation.Cast<OperationTransaction>().GetIdentifier()] = _operation.Cast<OperationTransaction>();

                })
               .Case<OperationAuthRequest> ( action => {

                   using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                   {
                       byte[] val = new byte[32];
                       crypto.GetBytes(val);
                       var seed = BitConverter.ToInt64(val, 1);
                       var _resp = new ClmManager(new OperationAuthAck("node_test", seed.ToString(), ServerAuthenticator.Sign(seed.ToString())));
                       _responseBuffer = _resp.GetBuffer().Result.ToBase64();
                   }
               })
               .Case<OperationTxStatus> (async action => {

                   if (string.IsNullOrEmpty(_operation.OperationData))
                       return;

                   var _res = await this.getTxStatus(_operation.OperationData);
                   if (_res != null) {
                       var _resp = new ClmManager(new OperationTxStatusAck("node_test", _res.ToString() + "&" + _res.Votes(), ServerAuthenticator.Sign(_res.ToString())));
                       _responseBuffer = _resp.GetBuffer().Result.ToBase64();
                   }

               })
               .Case<OperationStatsRequest> ( action => {

                   var data = (this.LastBlock().ToString() + "&" + this.PendingOperations.Count).Compress().ToByteArray().ToBase64();
                   var _resp = new ClmManager(new OperationStatsAck("node_test", data, "none"));
                   _responseBuffer = _resp.GetBuffer().Result.ToBase64();

               })
               .Case<OperationPendingRequest> ( action => {

                   // We're just forcing the existance of a transaction by me, for the pure reason we are testing this.
                   //Security.Cryptography.RSA _rsa = new Security.Cryptography.RSA(File.ReadAllText("user_privKeyOut.pem"), File.ReadAllText("user_pubKey.pem"));
                   // Tx from me to Poul of 3200.0 kr, assets are not be kept private from miners, miner only need to know who's involved.
                   // This is not the best practice, but it's quick for our needs.
                   //this.ProcessOperation(new OperationTransaction("Luca|Poul", "3200.0", _rsa.Sign("Luca|Poul")));


                   // Get oldest pending operation, prioritizing longer awaiting operations.
                   if (this.PendingOperations.Count > 0) {
                       var _tx = this.PendingOperations.Last()._operation;
                       if (_tx != null) {
                           // Leave data empty, it's going to be filled by AssignTx
                           var _ack = new OperationPendingAck("node_test", "", "");
                           try {
                               _ack.AssignTx(_tx.Cast<OperationTransaction>(), _operation.OperationData);
                           } catch (Exception ex) {
                               Console.WriteLine(ex.Message);
                           }

                           _ack.Signature = ServerAuthenticator.Sign(_ack.OperationData);
                           var _clmTemp = new ClmManager(_ack);
                           _responseBuffer = _clmTemp.GetBuffer().Result.ToBase64();
                       }
                   }

               });

            return _responseBuffer != null ? _responseBuffer : string.Empty;
        }

        public bool ConfirmTransaction(string _txId) {
            foreach (var tx in this.PendingOperations) {
                if (tx._operation.GetID() == IOperation.OPERATION_TYPE.TRANSACTION_REQUEST && tx._operation.Cast<OperationTransaction>().GetIdentifier().Equals(_txId)) {
                    // Too many transactions, new block required.
                    if (this.LastBlock().IsMaxLedger())
                        this.AddSingle(null);

                    // Register transaction into the block.
                    this.LastBlock().AddTransaction(tx);
                    // Remove the transaction from votemap, it's done already.
                    //this._votemap[_txId] = null;
                    this._votemap.Remove(_txId);

                    return true;
                }
            }

            return false;
        }

        public void Add(List<BlockData> _data) {
            Block latestBlock = this.LastBlock();
            var newBlock = new Block(DateTime.Now, this.LastBlock(), _data);

            if (latestBlock != null) {
                newBlock.Index = latestBlock.Index + 1;
                newBlock.PrevHash = latestBlock.Hash;
                latestBlock.UpdateHash();
            } else {
                newBlock.Index = 0;
                newBlock.PrevHash = string.Empty;
            }
            
            this._chain.Add(newBlock);
        }

        public override string ToString()
            => JsonConvert.SerializeObject(this._chain.ToList(), Formatting.Indented);
    }
}
