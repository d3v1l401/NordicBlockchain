﻿using Nordic.Blockchain.Operations;
using Nordic.Extensions;
using Nordic.Security.CLM_Manager;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocket4Net;

namespace Nordic.Network.Client
{
    public class Client {
        private IDictionary<string, WebSocket> sessions = new Dictionary<string, WebSocket>();
        private string _authToken = string.Empty;

        public delegate string processPendingTicket(OperationPendingAck _op);
        public processPendingTicket TicketProcessor = null;

        public string GetToken()
            => this._authToken;

        private void OnMessage(WebSocket sender, DataReceivedEventArgs e) {

            //Console.WriteLine(e.Data.HexDump());

            if (e.Data == null || e.Data.Length == 0)
                return;

            var _op = new ClmManager(e.Data);
            var _class = _op.GetClass();

            switch (_class.Result.GetID()) {
                case IOperation.OPERATION_TYPE.AUTHENTICATE_RESPONSE:

                    this._authToken = _class.Result.OperationData;

                break;
                case IOperation.OPERATION_TYPE.PENDING_OPERATION_ACK:

                    var _result = this.TicketProcessor(_class.Result.Cast<OperationPendingAck>());
                    sender.Send(_result);

                    break;
                case IOperation.OPERATION_TYPE.OPERATION_STATS_ACK:

                    var stats = _class.Result.OperationData.FromBase64().ToStringBuffer().Decompress().Split("&");
                    if (stats.Length == 2)
                    {
                        Console.WriteLine("There are " + stats[1] + " pending operations, last block info:");
                        Console.WriteLine(stats[0]);
                    } else
                        Console.WriteLine(_class.Result.OperationData.FromBase64().ToStringBuffer().Decompress());

                    break;
                case IOperation.OPERATION_TYPE.TRANSACTION_STATUS_ACK:

                    var _txDump = _class.Result.OperationData.Split('&');
                    
                    Console.WriteLine("Transaction details: \n" + _txDump[0]);
                    var _detail = int.Parse(_txDump[1]) >= 1 ? "Registered" : "Awaiting votes";
                    Console.WriteLine("Current votes: " + _txDump[1] + " (" + _detail + ")");

                    break;
                default:

                    Console.WriteLine("Unknown packet received for client: " + _class.Result.GetID());

                break;
            }
        }

        // Override default receive event to data only.
        private void OnMessageAsString(WebSocket sender, MessageReceivedEventArgs e) {
            this.OnMessage(sender, new DataReceivedEventArgs(e.Message.ToByteArrayUTF()));
        }

        protected void OnError(object sender, ErrorEventArgs e) {
            Console.WriteLine("ERROR: " + e.Exception.Message);
        }

        public void Connect(string _url) {
            if (!this.sessions.ContainsKey(_url)) {
                WebSocket _websck = new WebSocket(_url);

                _websck.DataReceived += (s, e) => { this.OnMessage((WebSocket)s, e); };
                _websck.MessageReceived += (s, e) => { this.OnMessageAsString((WebSocket)s, e); };
                _websck.Error += (s, e) => { this.OnError(s, e); };

                _websck.Open();

                sessions.Add(_url, _websck);
            }
        }

        public WebSocket GetSession(string _url) {
            return this.sessions[_url];
        }

        public void Send(string _url, string _data) {
            foreach (var item in sessions)
                if (item.Key == _url)
                    item.Value.Send(_data);
        }

        public void Broadcast(string _data) {
            foreach (var item in sessions)
                item.Value.Send(_data);
        }

        public IList<string> GetServers() {
            IList<string> servers = new List<string>();
            foreach (var item in sessions)
                servers.Add(item.Key);

            return servers;
        }

        public void Close() {
            foreach (var item in sessions)
                item.Value.Close();
        }
    }
}
