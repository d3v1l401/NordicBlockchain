using Nordic.Blockchain;
using Nordic.Blockchain.Operations;
using System;
using Xunit;

namespace NordicTester
{
    public class BlockchainTests
    {
        private readonly Blockchain _blockchain = new Blockchain();

        [Fact]
        public void TestBlockchain() {
            IOperation _op = new OperationNewBlock("lmf", "No data", "no sig");
            _blockchain.Add(new BlockData("lmf", Nordic.Blockchain.Operations.IOperation.OPERATION_TYPE.OPERATION_GENESIS_BLOCK, ""));

        }

        [Fact]
        public void TestBlock()
        {

        }
    }
}
