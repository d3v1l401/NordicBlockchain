﻿using Newtonsoft.Json;
using Nordic.Exceptions;
using Nordic.Security.Cryptography;
using Nordic.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Security
{
    public class TrustVault {
        private static readonly string CURRENT_NODE_IDENTIFIER = "_this_";

        private Dictionary<string, RSA> _vault = new Dictionary<string, RSA>();
        private string _pemPubKey = string.Empty;

        public TrustVault(string _thisNodePrivKey, string _thisNodePubKey) {
            this._vault.Add(CURRENT_NODE_IDENTIFIER, new RSA(_thisNodePrivKey, _thisNodePubKey));
            this._pemPubKey = _thisNodePubKey;
            if (this._vault[CURRENT_NODE_IDENTIFIER] == null)
                throw new RSAProviderException("This node has no RSA provider");
            
        }

        public string GetPubKey() 
            => this._pemPubKey;
        

        public string Sign(string _inputData) {
            return this._vault[CURRENT_NODE_IDENTIFIER].Sign(_inputData);
        }

        public bool Add(string _identifier, string _pubKey) {
            if (!this._vault.ContainsKey(_identifier))
                this._vault[_identifier] = new RSA(null, _pubKey);
            else
                return false;

            return this._vault[_identifier] != null ? true : false;
        }

        public bool Verify(string _inputData, string _signature, string _from) {
            if (!this._vault.ContainsKey(_from))
                throw new VaultEntryNotFound(_from);

            // null => referring to the same RSA instance of such item, in this case the vault _from
            // not null => verify signature against a specific public key
            return this._vault[_from].VerifySignature(_inputData, _signature, this._vault[_from].GetPublicKey());
        }

        public void FromJson(string _jsonPayload) {
            var _filteredCopy = JsonConvert.DeserializeObject<Dictionary<string, string>>(_jsonPayload);

            foreach (var entry in _filteredCopy) 
                if (!this._vault.ContainsKey(entry.Key))
                    this._vault.Add(entry.Key, new RSA(null, entry.Value));
        }

        public string ToJson() {
            var _filteredCopy = new Dictionary<string, string>();
            foreach (var entry in this._vault)
                if (entry.Key.Equals(CURRENT_NODE_IDENTIFIER))
                    _filteredCopy.Add(Network.Network.GetCurrentNodeAddress(), entry.Value.ToString());
                else
                    _filteredCopy.Add(entry.Key, entry.Value.ToString());

            return JsonConvert.SerializeObject(_filteredCopy, Formatting.Indented);
        }
        
    }
}
