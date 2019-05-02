using Newtonsoft.Json;
using Nordic.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nordic.Configuration
{
    public class Configuration {
        public static class BlockchainParameters {
            public static int LedgerMax = 500;

        }

        public static class SecretyParameters {
            public static string TrustVaultPath = "trustVault.json";
            public static string SharedCachePath = "sharedCache.json";

        }

        public static class CryptoParameters {
            public static string PrivateKeyPath = "privKey.pem";
            public static string PublicKeyPath = "pubKey.pem";

        }

        public static class NetworkParameters {
            public static string NodeBindingAddress = "127.0.0.1";
            public static uint NodeBindingPort = 1337;
        }
    }

    public static class ConfigurationHelper {
        private static Configuration __instance;

        public static Configuration getInstance() {
            if (__instance != null)
                return __instance;

            return __instance = new Configuration();
        }

        public static bool Import(string _path) {

            if (File.Exists(_path)) {
                __instance = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(_path));
                return true;
            }

            return false;
        }

        public static bool Export(string _path) {
            if (File.Exists(_path))
                return false;

            File.WriteAllBytes(_path, JsonConvert.SerializeObject(getInstance()).ToByteArrayUTF());
            return true;
        }
    }
}
