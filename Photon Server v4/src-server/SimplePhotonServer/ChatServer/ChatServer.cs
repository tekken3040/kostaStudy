using System;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace ChatServer
{
    public class ChatServer : ApplicationBase
    {
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new ChatPeer(initRequest);
        }

        protected override void Setup()
        {
        }

        protected override void TearDown()
        {
        }
    }

    public class ChatPeer : ClientPeer
    {
        private static event Action<ChatPeer, EventData, SendParameters> BroadcastMessage;

        public ChatPeer(InitRequest initRequest) : base(initRequest)
        {
            BroadcastMessage += OnBroadcastMessage;
        }
        
        protected override void OnDisconnect(DisconnectReason disconnectCode, string reasonDetail)
        {
            BroadcastMessage -= OnBroadcastMessage;
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (operationRequest.OperationCode == 1) // Chat Custom Operation Code = 1
            {
                // broadcast chat custom event to other peers
                var eventData = new EventData(1) { Parameters = operationRequest.Parameters }; // Chat Custom Event Code = 1
                BroadcastMessage(this, eventData, sendParameters);
                // send operation response (~ACK) back to peer
                var response = new OperationResponse(operationRequest.OperationCode);
                SendOperationResponse(response, sendParameters);
            }
        }

        private void OnBroadcastMessage(ChatPeer peer, EventData eventData, SendParameters sendParameters)
        {
            if (peer != this) // do not send chat custom event to peer who called the chat custom operation
            {
                SendEvent(eventData, sendParameters);
            }
        }
    }
}
