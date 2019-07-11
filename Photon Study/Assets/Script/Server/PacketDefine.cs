using System;
using System.IO;

namespace Server
{
    class ConstDef
    {
        public static int HeaderSize = 3;
        public static int ClientPacketMaxSize = 10000;
        public static int ServerPacketMaxSize = 60000;
    }

    public struct REQ_ENCRYTO_DATA
    {
        public string ID;
        public string Data;
    }

    public struct RES_ENCRYTO_DATA
    {
        public byte Result;
        public string Data;
    }

    class PacketHeader
    {
        public static void Set(BinaryWriter bwOut, MSGs u1MSGType, UInt16 u2SeqNo)
        {
            bwOut.BaseStream.Position = 0;
            bwOut.Write((byte)u1MSGType);
            bwOut.Write(u2SeqNo);
        }
    }

    public struct CARD_INIT
    {
        public byte u1Count;
        public string imgName;
        public string frameName;
        public bool bAura;
        //public OnResponse callback;
    }

    public enum MSGType
    {
        NONE = 0,
        INIT = 1,
        ERROR = 200,
    }

    public enum MSGs
    {
        NONE = 0,
        AUTH_JOIN = 1,
        AUTH_LOGIN = 2,
        AUTH_USERINFO = 3,
        AUTH_USERSTATE = 4,
        AUTH_LOGOUT = 5,
        AUTH_QUIT = 6,

        CREATE_CARD = 7,
        LAST = 200,
    }

    public enum ERROR_ID
    {
        NONE = 0,
        NETWORK_ANSWER_FAILED = 2,
        NETWORK_ANSWER_DELAYED = 3,
        REQUEST_PACKET_DECRYPT = 16,
        REQUEST_PACKET_ENCRYPT = 17,
        REQUEST_PACKET_TOO_SHORT = 18,
        REQUEST_PACKET_JOB = 19,
        NO_DATA_PACKET = 20,

        REQUEST_PACKET_TOO_LONG = 21,
        REQUEST_MESSAGETYPE_UNDEFINED = 22,

        PREV_REQUEST_NOT_COMPLETE = 23,
        PREV_REQUEST_FAIL_REDIS = 24,

        REQUEST_SESSION_NOT_FOUND = 25,

        REQUEST_DUPLICATION = 26,
    }
}