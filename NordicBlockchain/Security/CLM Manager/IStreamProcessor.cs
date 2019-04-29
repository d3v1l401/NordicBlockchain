using Nordic.Blockchain.Operations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nordic.Security.CLM_Manager
{
    public interface IStreamProcessor {

        Task<IOperation> Process(byte[] _buffer, IOperation.OPERATION_TYPE _type);
        Task<byte[]> Process(IOperation _operation);
    }
}
