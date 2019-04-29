using Nordic.Blockchain.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Security.CLM_Manager
{
    public interface IStreamProcessor {

        IOperation Process(byte[] _buffer, IOperation.OPERATION_TYPE _type);
        byte[] Process(IOperation _operation);
    }
}
