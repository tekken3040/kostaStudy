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
        //접속한 클라이언트 모두에게 메시지를 전달하는 Broadcast 생성
        private static event Action<ChatPeer, EventData, SendParameters> BroadcastMessage;

        //요청이 들어올경우 브로드캐스트에 추가
        public ChatPeer(InitRequest initRequest) : base(initRequest)
        {
            BroadcastMessage += OnBroadcastMessage;
        }
        
        //요청이 처리되거나 연결이 해제되었을때 브로드캐스트 삭제
        protected override void OnDisconnect(DisconnectReason disconnectCode, string reasonDetail)
        {
            BroadcastMessage -= OnBroadcastMessage;
        }

        //요청 수행
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (operationRequest.OperationCode == 1) // 요청 코드가 1인경우
            {
                // broadcast chat custom event to other peers
                var eventData = new EventData(1)// Chat Custom Event Code = 1
                {
                    Parameters = operationRequest.Parameters
                }; 
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
