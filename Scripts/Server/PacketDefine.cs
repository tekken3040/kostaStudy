using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server
{
	class ConstDef
	{
		public static ushort TICKET_SWEEP = 58004;
		public static int HeaderSize = 3;
		public static int ClientPacketMaxSize = 10000;
		public static int ServerPacketMaxSize = 60000;

		public static int MaxHeroBelongTo = 30;
		public static int BaseHeroLevel = 1;
        //원스토어 CBT 30으로 수정
		public static int MaxHeroLevel = 60;
		public const int MaxClassOfHero = 12;
		public static int LengthOfShape = 3;
		//public static int EquipOfHero = 10;
		public static int HeroInCrew = 3;
		public static int NumOfMixItem = 4;
		//public static int NumOfInitialItem = 7;
//		public static int SizeOfBag = 250;
		public static UInt16 BaseRuneID = 4650;
		public static int SizeOfRuneBuffer = 500;
		public static UInt16 BaseActID = 9200; 
		public static UInt16 BaseChapterID = 1700;
		public static int MaxChapter = 200;
		public static UInt16 BaseStageID = 6000;
		public static int SizeOfStageBuffer = 1100;
		public static int BaseForestStageID = 6500;
		public static int SizeOfForestStageBuffer = 300;
		public static int SizeOfForestStateBuffer = 6;
		public static int CharStatPointType = 7;
		public static int SkillOfEquip = 3;
		public static int EquipStatPointType = 3;
		public static UInt16 DefaultSkillSelectSlot = 455;
		public static int MaxCrewRune = 5;
		public static int MaxRuneLevel = 10;
		public static int MaxItemSlot = 10;
		public static int MaxForgeLevel = 10;
		public static int BaseEquipDesignID = 10000;
		public static int SizeOfEquipDesignBuffer = 625;
		public static int BaseLookDesignID = 30000;
		public static int SizeOfLookDesignBuffer = 625;
        public static int SizeOfMakeNewBinaryBuffer = 2;
		//		public static int MaxEquipDesign = 200;
		//		public static int MaxLookDesign = 150;
		public static int MaxCharClass = 12;
		public static int MaxCharTrainingRoom = 20;
		public static int MaxEquipTrainingRoom = 20;
        public static int BaseCharTrainingID = 9100;
        public static int BaseEquipTrainingID = 9150;
		public static int MaxDifficult = 3;
		public static int MinDifficult = 1;

		public static int BaseQuestID = 52001;
		public static int SizeOfQuestDoneBuffer = 250;
		public static int BaseAchievementID = 50001;
		public const int SizeOfAchievementDoneBuffer = 250;
		public static int[] PartPosOfAchievementDoneBuffer = new int[] { 50240, 50001, 50160 };
		public const int SizeOfAchievementBoolBuffer = 40;
		public static int[] PartPosOfAchievementBoolBuffer = new int[] { 16, 0, 8 };
		public const int SizeOfAchievementU1Buffer = 200;
		public static int[] PartPosOfAchievementU1Buffer = new int[] { 150, 0, 50 };
		public const int SizeOfAchievementU2Buffer = 3500;
		public static int[] PartPosOfAchievementU2Buffer = new int[] { 25, 0, 5 };
		public const int SizeOfAchievementU4Buffer = 5600;
		public static int[] PartPosOfAchievementU4Buffer = new int[] { 2, 0, 1 };

		public static byte LastTutorialStep = 200;
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

	public enum MSGs
	{
		NONE = 0,

		AUTH_JOIN = 1,
		AUTH_LOGIN = 2,
		AUTH_USERINFO = 3,
        AUTH_USERSTATE = 4,
		AUTH_LOGOUT = 5,
		AUTH_QUIT = 6,

		LEGION_SET_NAME = 7,
		LEGION_TUTORIAL = 8,
		LEGION_MARK = 9,

		CREW_OPEN = 11,
		CREW_OPENSLOT = 12,
		CREW_CHANGE = 13,
		CREW_SELECT = 14,
		CREW_RUNE = 15,

		CHAR_CREATE = 21,
		CHAR_RETIRE = 22,
		CHAR_STAT_RESET = 23,
		CHAR_STAT_POINT = 24,
		CHAR_EQUIP_CHANGE = 25,
		CHAR_SKILL_OPEN = 26,
		CHAR_SKILL_RESET = 27,
		CHAR_SKILL_CHANGE = 28,
		CHAR_BEGIN_TRAINING = 29,
		CHAR_END_TRAINING = 30,
		CHAR_CANCEL_TRAINING = 31,
		CHAR_FINISH_TRAINING = 32,
		CHAR_OPEN_TRAINING_SEAT = 33,
		CHAR_BUY_STAT_POINT = 34,
		CHAR_BUY_SKILL_POINT = 35,

		STAGE_START = 41,
		STAGE_RESULT = 42,
		STAGE_DISPATCH = 43,
		STAGE_DISPATCH_CANCEL = 44,
		STAGE_DISPATCH_RESULT = 45,
		STAGE_DISPATCH_FINISH = 46,
		STAGE_SWEEP = 47,
		STAGE_REPEATREWARD = 48,
		STAGE_STARREWARD = 49,
		STAGE_REPEAT = 50,

		FORGE_SMITH = 51,
		FORGE_NAME = 52,
		FORGE_RUNE = 53,
		FORGE_FUSION = 54,
		FORGE_CHANGE_LOOK = 55,
		FORGE_UPGRADE = 56,
		FORGE_CHECK_DESIGN = 57,

		LEAGUE_MATCH = 61,
		LEAGUE_MATCH_RESULT = 62,
		LEAGUE_START_MATCH = 63,
		LEAGUE_REVENGEMESSAGE = 64,
		LEAGUE_SET_CREW = 65,
		LEAGUE_LEGENDRANK = 66,
		LEAGUE_REWARD = 67,
		LEAGUE_GET_MATCHLIST = 68,
		LEAGUE_BUY_KEY = 69,
		LEAGUE_CHECK = 70,

		ITEM_USE = 76,
		ITEM_SELL = 77,

		EQUIP_STAT_RESET = 81,
		EQUIP_STAT_POINT = 82,
		EQUIP_BEGIN_TRAINING = 83,
		EQUIP_END_TRAINING = 84,
		EQUIP_CANCEL_TRAINING = 85,
		EQUIP_FINISH_TRAINING = 86,
		EQUIP_OPEN_TRAINING_SEAT = 87,
		EQUIP_BUY_STAT_POINT = 88,
		EQUIP_CHECK_SLOT = 89,

		SHOP_LIST = 91,
		SHOP_BUY = 92,
		SHOP_REGISTER = 93,
		SHOP_FIXSHOP = 94,

		QUEST_ACCEPT = 101,
		QUEST_CANCEL = 102,
		QUEST_COMPLETE = 103,
		QUEST_ACHIEVEMENT_REWARD = 104,

		MAIL_LIST = 106,
		MAIL_READ = 107,
		MAIL_GET = 108,

		MISSION_REWARD = 109,
		MISSION_PUSHBACK = 110,

		FRIEND_LIST = 111,
		FRIEND_INVITE = 112,
		FRIEND_CANCEL_INVITE = 113,
		FRIEND_CONFIRM_INVITE = 114,
		FRIEND_RECOMMEND = 115,
		FRIEND_DROP = 116,
		FRIEND_SEND_POINT = 117,
		FRIEND_RECV_POINT = 118,
		FRIEND_CHECK_NEW = 119,

		MODE_BUY_FOREST_TICKET = 121,

		SOCIAL_GET_NOTICE = 131,
		SOCIAL_COUPON = 132,
		RANK_INFO = 133,
		RANK_LIST = 134,
		RANK_REWARD = 135,
		CHAT_CHANGE_CHANNEL = 136,

		EVENT_BUY = 141,
		EVENT_REWARD = 142,
		AD_REWARD = 143,
        EVENT_RELOAD = 145,
		EVENT_ITEMUSE = 146,
        EVENT_OX = 147,

        GUILD_CREATE = 151,
        GUILD_INFO = 152,
        GUILD_MARK = 153,
        GUILD_SET_MEMBER = 154,
        GUILD_SET_CREW = 155,
        GUILD_SEARCH = 156,
        GUILD_REQUEST_JOIN_LIST = 157,
        GUILD_REQUEST_JOIN = 158,
        GUILD_MATCH_INFO = 159,
        GUILD_MATCH = 160,
        GUILD_START_MATCH = 161,
        GUILD_MATCH_RESULT = 162,
        GUILD_RANK_INFO = 163,

		OPTION_PUSH = 171,
		OPTION_ACCOUNT = 172,
		OPTION_SET = 173,

		ETC_COMMAND = 181,

		LAST = 200,
	}

	public enum ERROR_ID
	{
		NONE = 0,
		NONE_SESSION_NOSAVE = 1,

		NETWORK_ANSWER_FAILED = 2,
		NETWORK_ANSWER_DELAYED = 3,

		REDIS_START_PARSE_DB_CONNECT_STRING = 6,
		REDIS_START_EXCEPTION = 7,
		REDIS_START_SET_TEST = 8,

		LOAD_CONFIG_MONGODB = 10,
		LOAD_CONFIG_REDIS = 11,
		LOAD_CONFIG_MYSQLDB = 12,

		EXCEPTION_GAME_CONTENT_LOAD = 13,


		FAIL_NETWORK_HTTP_REQUEST = 14,
		//EXCEPTION_HTTP_REQUEST = 15,

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

		ACCOUNTDB_GETCONNECT_FAIL = 27,
		ACCOUNTDB_QUERY_FAIL = 28,
		REQUEST_SESSION_MISMATCH = 29,
		USERDB_GETCONNECT_FAIL = 30,
		USERDB_QUERY_FAIL = 31,
		//UserDB_JOB_FAIL = 32,
		USERDB_QUERY_FAIL_SESSION_NOSAVE = 33,
		REDISRANK_QUERY_FAIL = 34,
		#if __GUILD
		GUILDDB_QUERY_FAIL = 35,
		#endif

		STRING_TOOLONG = 41,
		STRING_TOOSHORT = 42,
		INVALID_VALUE = 43,
		OPERATING_DONE = 44,
		NOT_EXIST = 45,
		NOT_ENOUGH = 46,
		ITEMID_INVALID = 47,
		NOT_CLEAR = 48,
		FULL = 49,
		DUPLICATE = 50,
		OPERATING_NOT_YET = 51,
		OPERATING_ARLEADY = 52,
		MAX_VALUE = 53,
		TOO_MUCH = 54,
		EMPTY = 55,
		NOT_MATCHED = 56,
		LOW_LEVEL = 57,
		PREV_UNDONE = 58,
		NOT_OPERATING = 59,
		TUTORIAL_ARLEADY = 60,
		EXIST = 61,
		OUT_OF_FUNCTION = 62,

		NOT_IN_FRIEND = 63,
		NOT_IN_LEAGUE = 64,
		//NOT_IN_RANK = 65,
		FULL_ROLLBACK = 66,

		INVEN_SLOT_IN_SHOP = 67,
		SHOP_OVER_RENEW = 68,
		SHOP_OVER_EQUIPCOUNT = 69,
		EQUIPED = 70,
		SHOP_NO_DUPLICATE_PURCHASE = 71,


		LOGICAL_ERROR = 100,

		CREATE_ACCOUNT_ID_TOOLONG = 101,
		CREATE_ACCOUNT_ID_TOOSHORT = 102,
		CREATE_ACCOUNT_ID_WRONGCHAR = 103,
		CREATE_ACCOUNT_PW_TOOLONG = 104,
		CREATE_ACCOUNT_PW_TOOSHORT = 105,
		SITE_AUTH_TOKEN_FAIL = 106,
		CREATE_ACCOUNT_DUPLICATE = 107,

		LOGIN_ID_FAIL = 111,
		LOGIN_PW_FAIL = 112,
		LOGIN_VERSION_MISMATCH = 113,
		LOGIN_INSPECTION = 114,
		LOGIN_BLOCK = 115,
		LOGIN_BLOCK_FUNCTION = 116,
		LOGIN_DAY_CHANGED = 117,
		LOGIN_VERSION_UNDER = 118,
		LOGIN_CLOSE_APP = 119,

		LEGION_NAME_DUPLICATE = 121,

		INVEN_FULL = 131,
		CHANNEL_FULL = 132,

		DISPATCH_TIME_LEFT = 141,
		ACHIEVEMENT_TIME_OUT = 142,
		EVENT_NOT_SALE = 143,
		AD_REWARD_EXHAUSTION = 144,
		AD_REWARD_NOT_YET = 145,

		SHOP_NO_DATA = 151,
		SOLDOUT = 152,
		SHOP_NO_DISCOUNT = 153,
		SHOP_NOT_SAMEGOODS = 154,
		SHOP_RECEIPT_INVALID = 155,
		SHOP_RECEIPT_MISMATCH = 156,
		SHOP_RECEIPT_DUPLICATE = 157,
		SHOP_RECEIPT_CANCELED = 158,

		LEAGUE_DATA_DENIED = 161,
		LEAGUE_NO_MATCHUP = 162,

		TRAIN_MORE_CASH = 171,
		LINK_EXIST = 172,
		RANK_DATA_DENIED = 173,
		GUILD_DATA_DENIED = 174,

		COUPON_INVALID = 176,
		COUPON_DUPLICATE = 177,
		COUPON_FULL = 178,
		COUPON_DONE = 179,

		FRIEND_REQUEST_FULL = 181,
		FRIEND_OTHER_FULL = 182,
		FRIEND_CANCELED = 183,
		FRIEND_TODAY_FULL = 184,
		FRIEND_REQUEST_OTHER = 185,
		FRIEND_REQUEST_DELETED = 186,
		MAIL_DELETED = 187,

		GUILD_REQUEST_FULL = 191,
        GUILD_REQUEST_ALREADY = 192,
        GUILD_REQUEST_NOT_YET = 193,
        GUILD_REQUEST_CONFLICT = 194,
	}

	class PacketHeader
	{
		public static void Set(BinaryWriter bwOut, MSGs u1MSGType, ushort u2SeqNo)
		{
			bwOut.BaseStream.Position = 0;
			bwOut.Write((byte)u1MSGType);
			bwOut.Write(u2SeqNo);
		}
	}
}
