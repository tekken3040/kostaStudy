using System;
using System.IO;
using System.Text;

namespace Server
{
    delegate ERROR_ID MessageHandler(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2);

    public static class Message
    {
        static MessageHandler[] handlers;

        static Message()
        {
            handlers = new MessageHandler[(int)MSGs.LAST];
        }
        public static ERROR_ID Process(ERROR_ID result, byte[] requestPacket, out UInt16 u2SeqNo, ServerMgr.RequestJob job)
        {
            u2SeqNo = 0;
            if (requestPacket.Length < ConstDef.HeaderSize)
            {
                return ERROR_ID.REQUEST_PACKET_TOO_SHORT;
            }
            if (requestPacket.Length > ConstDef.ServerPacketMaxSize)
            {
                return ERROR_ID.REQUEST_PACKET_TOO_LONG;
            }

            MemoryStream msBufR = new MemoryStream(requestPacket);
            BinaryReader brIn = new BinaryReader(msBufR, Encoding.Unicode);

            brIn.BaseStream.Position = 0;           // binary reader 용 Reset
            byte u1MSGType = brIn.ReadByte();
            u2SeqNo = brIn.ReadUInt16();

            if (u1MSGType >= (ushort)MSGs.LAST || handlers[u1MSGType] == null)
            {
                return ERROR_ID.REQUEST_MESSAGETYPE_UNDEFINED;
            }

            return handlers[u1MSGType](result, job.callback, brIn, job.obj1, job.obj2);
        }

        static byte[] DecryptRequestData(string request)
        {
            return Base64String.ToByteArray(request);
        }

        public static REQ_ENCRYTO_DATA EncryotRequestData(string id, byte[] message, Int32 length)
        {
            var request = new REQ_ENCRYTO_DATA
            {
                ID = id,
                Data = Base64String.ToBase64String(message, length),
            };

            return request;
        }

        public static ERROR_ID ParseMessage(RES_ENCRYTO_DATA requestPacket, out UInt16 seqNo, ServerMgr.RequestJob job)
        {
            byte[] message = DecryptRequestData(requestPacket.Data);
            return Process((ERROR_ID)requestPacket.Result, message, out seqNo, job);
        }
    }
}