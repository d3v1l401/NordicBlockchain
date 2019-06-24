using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Nordic.Extensions;
using Nordic.Security.ServerAuthenticator;
using WebSocketSharp;
using Nordic.Security.CLM_Manager;
using Nordic.Blockchain.Operations;
using Nordic.Exceptions;
using Nordic.Security.Cryptography;
using System.Threading.Tasks;

namespace Nordic.Network
{
    class SessionHandler {

        public struct Peer {
            public string Guid;
        }

        struct SessionData {
            public Peer         Peer;
            public BinaryReader Stream;
        }

        private static Dictionary<System.Net.IPEndPoint, SessionData> _sessions = null;

        public SessionHandler() 
            => _sessions = new Dictionary<System.Net.IPEndPoint, SessionData>();

        private static SessionHandler __instance = null;

        public static SessionHandler getInstance() {
            return __instance != null ? __instance : new SessionHandler();
        }

        public static bool AddSession(System.Net.IPEndPoint _endPoint, Peer _peer) {
            if (!SessionExists(_endPoint)) {
                SessionData sd = new SessionData();
                _sessions.Add(_endPoint, sd);

                if (_sessions.TryGetValue(_endPoint, out sd)) {
                    //sd.PeerSession = _peer;
                    return true;
                }
            }
            
            return false;
        }

        public static bool CloseSession(System.Net.IPEndPoint _endPoint) {
            if (SessionExists(_endPoint)) {

                return true;
            }

            return false;
        }

        public static bool SessionExists(System.Net.IPEndPoint _endPoint)
            => _sessions.ContainsKey(_endPoint);

        public async void OnPeerDataSent(Network sender, MessageEventArgs e) {
            
            // Identify author
            //if (SessionExists(e.Peer.EndPoint)) {

            //}

        }

        public async Task<string> OnPeerDataRecv(Network sender, MessageEventArgs e) {
            // Identify author
            //if (SessionExists(sender.Context.)) {

            //}

            if (e.RawData != null && e.RawData.Length > 0) {
                try {
                    // Keep async methods, receival is not dependant on synchrony and can take time.
                    var _class = await new ClmManager(e.RawData).GetClass();
                    return await Blockchain.Blockchain.getInstance().ProcessOperation(_class);

                } catch (MalformedCLMPacket ex) {
                    await Console.Out.WriteLineAsync("Malformed packet: " + ex.Message + "\n" + ex.StackTrace);
                } catch (TamperedClmPacket ex) {
                    await Console.Out.WriteLineAsync(ex.Message + "\n" + ex.StackTrace);
                }

            }
            return null;
        }

        public async void OnConnectionFailed(object sender, WebSocketSharp.ErrorEventArgs e) {
            // Node dropped connection
            Console.WriteLine("Connection failed: " + e);
            //this.OnNodeDisconnected(sender, e);
        }

        public async void OnNodeConnected(object sender) {
            //if (AddSession(e.Peer.EndPoint, e.Peer)) {
            //    // Authenticate please.
                
            //}

            
        }

        public async void OnNodeDisconnected(object sender, CloseEventArgs e) {
            //if (SessionExists(e)) {
                // Broadcast if needed?
            //    _sessions.Remove(e.EndPoint);
            //}
        }

        public override string ToString() {
            return "";
        }
    }
}
