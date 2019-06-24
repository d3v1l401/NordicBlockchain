using Nordic.Blockchain;
using Nordic.Blockchain.Operations;
using System;
using System.Globalization;
using Xunit;

namespace NordicTester
{
    public class BlockchainTests
    {
        private readonly Blockchain _blockchain = new Blockchain();
        private string _testingTxId = string.Empty;

        [Fact]
        public void TestBlockchainGenesis() {

            // Genesi does exist (automatically created).
            var _genesis = this._blockchain.GetBlock(0);
            Assert.NotNull(_genesis);

            // It's not corrupted.
            Assert.NotNull(_genesis.Data);
            // As genesis block, there should be nothing before.
            Assert.True(string.IsNullOrEmpty(_genesis.PrevHash));

            // There's the proper operation justified for its creation.
            var _genesisData = _genesis.Data[0]._operation;
            Assert.NotNull(_genesisData);
            Assert.NotNull(_genesisData.OperationData);

            // Contains the standard message I wrote as default.
            Assert.Equal("Let there be light and JSON", _genesisData.OperationData);

            Assert.True(this._blockchain.Validity());
        }

        [Fact]
        public void TestTx() {
            var _tx = new OperationTransaction("Unit Testing-Luca", "8173661.0", "none");
            this._testingTxId = _tx.GetIdentifier();

            // We have to go trough the same process of the miner or no votes would be needed.
            this._blockchain.ProcessOperation(_tx);

            var _status = this._blockchain.getTxStatus(this._testingTxId).Result;

            Assert.NotNull(_status);
            // Is the same transaction we talking about.
            Assert.True(_status.GetIdentifier() == this._testingTxId);
            // It has no votes (still no miner).
            Assert.False(_status.Votes() == 1);

            // Cast a vote
            var _accepted = this._blockchain.Vote(this._testingTxId).Result;
            Assert.True(_accepted);

            // It should be now confirmed (default testing votes needed for confirmation is 1).
            var _newStatus = this._blockchain.getTxStatus(this._testingTxId).Result;
            Assert.True(_newStatus.Votes() == 1);
            Assert.True(_newStatus.IsDecisive());

            var _lastBlockWithTx = (OperationTransaction)this._blockchain.LastBlock().Data[0]._operation;
            Assert.True(_lastBlockWithTx.GetIdentifier() == this._testingTxId);

            // This Tx should not exist.
            _newStatus = this._blockchain.getTxStatus(this._testingTxId + "0").Result;
            Assert.Null(_newStatus);
        }

        [Fact]
        public void TestBlock() {
            // Create a transaction, current default limit per-block is 1 operation, therefore should trigger a new block creation.
            var _tx = new OperationTransaction("Unit Testing-Luca", "8173661.0", "none");
            this._testingTxId = _tx.GetIdentifier();

            this._blockchain.AddSingle(new BlockData(_tx));

            // Should trigger new block creation!
            var _lastBlock = this._blockchain.LastBlock();
            Assert.NotEqual(_lastBlock.Index, this._blockchain.GetBlock(0).Index);
            Assert.NotEqual(_lastBlock, this._blockchain.GetBlock(0));

            // Concatenation is correct
            Assert.Equal(_lastBlock.PrevHash, this._blockchain.GetBlock(0).Hash);

            // Blockchain hasn't been compromised.
            Assert.True(this._blockchain.Validity());

            // It is now.
            CultureInfo culture = new CultureInfo("en-US");
            DateTime tempDate = Convert.ToDateTime("1/1/2010 12:10:15 PM", culture);

            this._blockchain.GetBlock(0).Timestamp = tempDate;
            Assert.False(this._blockchain.Validity());
        }
    }
}
