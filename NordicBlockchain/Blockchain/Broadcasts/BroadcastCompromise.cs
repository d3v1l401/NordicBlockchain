using Newtonsoft.Json;
using Nordic.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Blockchain.Broadcasts
{
    class BroadcastCompromise : IBroadcast {

        public BroadcastCompromise(string _authority, string _data, string _signature) {

            base.Signature = _signature;
            base.BroadcastAuthority = _authority;
            base.BroadcastData = _data;

            __assignOpId(this);
        }

        public override string GetAuthor()
            => base.BroadcastAuthority;

        public override byte[] GetData()
            => base.BroadcastData.ToByteArray();

        public override BROADCAST_TYPE GetID()
            => base.BroadcastID;

        public override string GetSignature()
            => base.Signature;

        public override string ToString()
            => JsonConvert.SerializeObject(this);
    }
}
