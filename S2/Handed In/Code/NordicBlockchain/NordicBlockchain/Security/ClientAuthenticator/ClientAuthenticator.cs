using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nordic.Security.ClientAuthenticator
{
    public class ClientAuthenticator {
        private static TrustVault _vault = null;

        public static void Initialize(string _pathToPubKey, string _pathToPrivKey, string _pathToLocalVault) {
            _vault = new TrustVault(File.ReadAllText(_pathToPrivKey), File.ReadAllText(_pathToPubKey));

            if (!string.IsNullOrEmpty(_pathToLocalVault) && _pathToLocalVault.Length > 0)
                _vault.FromJson(File.ReadAllText(_pathToLocalVault));
        }

        public static string Sign(string _input) {
            return _vault.Sign(_input);
        }

        public static string GetPubKey() {
            return _vault.GetPubKey();
        }

        public static void Add(string _identifier, string _pubKey) {
            _vault.Add(_identifier, _pubKey);
        }

        public static bool Verify(string _input, string _signature, string _from) {
            return _vault.Verify(_input, _signature, _from);
        }
    }
}
