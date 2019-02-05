using Newtonsoft.Json;
using Nordic.Extensions;
using Nordic.Security.Cryptography;
using Nordic.Security.ServerAuthenticator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.SharedCache
{
    public class SharedCache {
        struct Cache {
            public IList<string>    _cachedAddressList;
            public DateTime         _lastCacheSync;
            public string           _signature;
        }

        private Cache _cache = new Cache();

        public SharedCache() {
            this._cache._cachedAddressList = new List<string>();
        }

        public void AddAddress(string _ip) {
            if (!this.IsListed(_ip))
                this._cache._cachedAddressList.Add(_ip);
        }

        public void RemoveAddress(string _ip) {
            this._cache._cachedAddressList.Remove(_ip);
        }

        public bool IsListed(string _ip) {
            return this._cache._cachedAddressList.Contains(_ip);
        }

        public SharedCache(IList<string> _addresses) {
            this._cache._cachedAddressList = _addresses;
            this._cache._lastCacheSync = DateTime.Now;

            var _sha = new Sha256();
            foreach (var entry in this._cache._cachedAddressList)
                _sha.Enqueue(entry.ToByteArray());

            this._cache._signature = _sha.ToString();

            // TODO: Move signature from simple hash to authed server signature
            //this._cache._signature = ServerAuthenticator.Sign();
        }

        private void ForcePackAsNew() {
            this.Pack();
            this._cache._lastCacheSync = DateTime.Now;
        }

        private void Pack() {
            var _sha = new Sha256();
            foreach (var entry in this._cache._cachedAddressList)
                _sha.Enqueue(entry.ToByteArray());

            this._cache._signature = _sha.ToString();
        }

        public string ToJson() {
            this.ForcePackAsNew();
            this.Pack();
            return JsonConvert.SerializeObject(_cache);
        }
    }
}
