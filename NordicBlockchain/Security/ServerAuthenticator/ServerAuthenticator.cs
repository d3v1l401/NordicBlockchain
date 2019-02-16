using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nordic.Security.ServerAuthenticator
{
    class ServerAuthenticator {
        private static TrustVault _vault = null;

        public static void Initialize(string _pathToPubKey, string _pathToPrivKey, string _pathToLocalVault) {
            _vault = new TrustVault(File.ReadAllText(_pathToPrivKey), File.ReadAllText(_pathToPubKey));
            _vault.FromJson(File.ReadAllText(_pathToLocalVault));
        }

        public static string Sign(string _input) {
            return _vault.Sign(_input);
        }

        public static bool Verify(string _input, string _orData, string _from) {
            return _vault.Verify(_input, _orData, _from);
        }
    }
}
