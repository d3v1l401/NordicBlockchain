using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Blockchain
{
    public class BlockData {
        public ulong    UserID { get; set; }
        public uint     OperationID { get; set; }
        public string   Signature { get; set; }

        public BlockData(ulong _uid, uint _opc, string _signature) {
            this.UserID = _uid;
            this.OperationID = _opc;
            this.Signature = _signature;
        }

        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }
    }
}
