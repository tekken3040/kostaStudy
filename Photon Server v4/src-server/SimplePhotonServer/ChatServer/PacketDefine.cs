using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class PacketDefine
    {
        public struct PacketHeader
        {
            public MSGs u1Type;
            public object obj;
        }

        public struct CARD_INIT
        {
            public byte u1Count;
            public string imgName;
            public string frameName;
            public bool bAura;
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
}
