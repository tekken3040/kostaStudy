using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using System.IO;
using System.Text;
using System;

namespace Server
{
    public delegate void OnResponse(ERROR_ID err);

    public struct RequestJob
    {
        public UInt16 u2SeqNo;
        public OnResponse callback;

        public RequestJob(UInt16 seqNo, OnResponse _callback)
        {
            u2SeqNo = seqNo;
            callback = _callback;
        }
    }

    public class ServerMgr : Singleton<ServerMgr>, IPhotonPeerListener
    {
        private bool connected;
        private PhotonPeer peer;
        private ServerMgr client;
        public OnResponse callback;

        byte[] outpacket;
        MemoryStream msBufW;
        BinaryWriter bwOut;

        UInt16 u2SeqNo = 0;

        private void Awake()
        {
            client = new ServerMgr();
            client.peer = new PhotonPeer(client, ConnectionProtocol.Tcp);
            outpacket = new byte[ConstDef.ClientPacketMaxSize];
            msBufW = new MemoryStream(outpacket);
            bwOut = new BinaryWriter(msBufW, Encoding.Unicode);
        }

        public void InitConnect()
        {
            // connect
            client.DebugReturn(DebugLevel.INFO, "Connecting to server at 127.0.0.1:4530 using TCP");
            client.peer.Connect("106.242.203.69:2749", "ChatServer");

            // client needs a background thread to dispatch incoming messages and send outgoing messages
            StartCoroutine(UpdateLoop());
            //StartCoroutine(ChatInput());
        }

        private Queue<RequestJob> requestJobs = new Queue<RequestJob>();
        public Queue<RequestJob> GetRequestQueue()
        {
            return requestJobs;
        }

        private IEnumerator ChatInput()
        {
            while (true)
            {
                if (!client.connected)
                    continue;
                // read input
                //string buffer = Console.ReadLine();
                // send to server
                //var parameters = new Dictionary<byte, object> { { 1, buffer } };
                //client.peer.OpCustom(1, parameters, true);
            }
        }

        public void RequestCreateCard(MSGs u1Type, CARD_INIT cardInit, OnResponse _callback)
        {
            Debug.Log("Request Card");
            PacketHeader.Set(bwOut, MSGs.CREATE_CARD, u2SeqNo);
            bwOut.Write(cardInit.bAura);
            bwOut.Write(cardInit.frameName);
            bwOut.Write(cardInit.imgName);
            bwOut.Write(cardInit.u1Count);

            RequestJob job = new RequestJob(u2SeqNo, _callback);
            GetRequestQueue().Enqueue(job);
            u2SeqNo++;

            byte[] u1Buff = msBufW.ToArray();//.GetBuffer();
            var parameters = new Dictionary<byte, object> { { (byte)u1Type, u1Buff } };
            client.peer.SendOperation((byte)u1Type, parameters, SendOptions.SendReliable);
        }

        private IEnumerator UpdateLoop()
        {
            while (true)
            {
                client.peer.Service();
                yield return new WaitForEndOfFrame();
            }
        }

        #region IPhotonPeerListener

        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.Log(string.Format("{0}: {1}", level, message));
        }

        public void OnEvent(EventData eventData)
        {
            DebugReturn(DebugLevel.INFO, eventData.ToStringFull());
            if (eventData.Code == 1)
            {
                DebugReturn(DebugLevel.INFO, string.Format("Chat Message: {0}", eventData.Parameters[1]));
            }
            switch (eventData.Code)
            {
                case (Byte)MSGs.CREATE_CARD:
                    //Handler.Instance.HandleResponse(eventData);
                    break;
            }
        }

        public void OnMessage(object messages)
        {
            //throw new NotImplementedException();
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            DebugReturn(DebugLevel.INFO, operationResponse.ToStringFull());
            
            switch(operationResponse.OperationCode)
            {
                case (Byte)MSGs.CREATE_CARD:
                    Handler.Instance.HandleResponse(operationResponse);
                    break;
            }
            
            //Handler.Instance.HandleResponse(operationResponse);
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            if (statusCode == StatusCode.Connect)
            {
                connected = true;
            }
            switch (statusCode)
            {
                case StatusCode.Connect:
                    DebugReturn(DebugLevel.INFO, "Connected");
                    connected = true;
                    break;
                default:
                    DebugReturn(DebugLevel.ERROR, statusCode.ToString());
                    break;
            }
        }

        #endregion
    }
}
