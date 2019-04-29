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

        public async void OnPeerDataSent(object sender, MessageEventArgs e) {
            
            // Identify author
            //if (SessionExists(e.Peer.EndPoint)) {

            //}

            using (var sr = new BinaryReader(new MemoryStream(e.RawData))) {
                
            }
        }

        public async void OnPeerDataRecv(object sender, MessageEventArgs e) {
            // Identify author
            //if (SessionExists(e.Peer.EndPoint)) {

            //}

            if (e.RawData != null && e.RawData.Length > 0) {
                try {
                    var _clm = await new ClmManager(e.RawData).GetClass();


                } catch (MalformedCLMPacket ex) {
                    await Console.Out.WriteLineAsync("Malformed packet: " + ex.Message + "\n" + ex.StackTrace);
                } catch (TamperedClmPacket ex) {
                    await Console.Out.WriteLineAsync(ex.Message + "\n" + ex.StackTrace);
                }
                
            }
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
