using UnityEngine;
using System;
using System.Collections.Generic;
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
			handlers[(int)MSGs.AUTH_JOIN] = AccountHandler.Create;
			handlers[(int)MSGs.AUTH_LOGIN] = AccountHandler.LogIn;
			handlers[(int)MSGs.AUTH_USERINFO] = AccountHandler.ReadUserInfo;
            handlers[(int)MSGs.AUTH_USERSTATE] = AccountHandler.LoginInfoMore;
            handlers[(int)MSGs.AUTH_LOGOUT] = AccountHandler.Logout;
            handlers[(int)MSGs.AUTH_QUIT] = AccountHandler.Quit;
			handlers[(int)MSGs.LEGION_SET_NAME] = AccountHandler.SetName;
			handlers[(int)MSGs.LEGION_TUTORIAL] = AccountHandler.UpdateTutorial;
            handlers[(int)MSGs.LEGION_MARK] = AccountHandler.LegionMark;
			handlers[(int)MSGs.CREW_OPEN] = CrewHandler.Open;
			handlers[(int)MSGs.CREW_OPENSLOT] = CrewHandler.OpenSlot;
			handlers[(int)MSGs.CREW_CHANGE] = CrewHandler.Change;
			handlers[(int)MSGs.CREW_SELECT] = AccountHandler.SelectCrew;
			handlers[(int)MSGs.CHAR_CREATE] = CharacterHandler.Create;
			handlers[(int)MSGs.CHAR_RETIRE] = CharacterHandler.Retire;
			handlers[(int)MSGs.CHAR_STAT_RESET] = CharacterHandler.ResetStat;
			handlers[(int)MSGs.CHAR_STAT_POINT] = CharacterHandler.PointStat;
			handlers[(int)MSGs.CHAR_EQUIP_CHANGE] = CharacterHandler.ChangeEquip;
            handlers[(int)MSGs.CHAR_BUY_STAT_POINT] = CharacterHandler.BuyStatusPoint;
			handlers[(int)MSGs.CHAR_SKILL_OPEN] = SkillHandler.Open;
			handlers[(int)MSGs.CHAR_SKILL_RESET] = SkillHandler.Reset;
			handlers[(int)MSGs.CHAR_SKILL_CHANGE] = SkillHandler.Change;
			handlers[(int)MSGs.CHAR_BUY_SKILL_POINT] = SkillHandler.BuySkillPoint;
			//handlers[(int)MSGs.CHAR_EQUIP_CHANGE] = CharacterHandler.ChangeEquip;
            handlers[(int)MSGs.CHAR_BEGIN_TRAINING] = CharacterHandler.CharBeginTraining;
            handlers[(int)MSGs.CHAR_CANCEL_TRAINING] = CharacterHandler.CharCancelTraining;
            handlers[(int)MSGs.CHAR_FINISH_TRAINING] = CharacterHandler.CharFinishTraining;
            handlers[(int)MSGs.CHAR_END_TRAINING] = CharacterHandler.CharEndTraining;
            handlers[(int)MSGs.CHAR_OPEN_TRAINING_SEAT] = CharacterHandler.CharOpenTrainingSeat;
			handlers[(int)MSGs.STAGE_START] = StageHandler.Start;
			handlers[(int)MSGs.STAGE_RESULT] = StageHandler.Result;
			handlers[(int)MSGs.STAGE_DISPATCH] = StageHandler.Dispatch;
			handlers[(int)MSGs.STAGE_DISPATCH_CANCEL] = StageHandler.CancelDispatch;
			handlers[(int)MSGs.STAGE_DISPATCH_RESULT] = StageHandler.GetDispatchResult;
			handlers[(int)MSGs.STAGE_DISPATCH_FINISH] = StageHandler.FinishDispatch;
			handlers[(int)MSGs.STAGE_REPEATREWARD] = StageHandler.RepeatReward;
			handlers[(int)MSGs.STAGE_STARREWARD] = StageHandler.StarReward;
			handlers[(int)MSGs.STAGE_SWEEP] = StageHandler.Sweep;
			handlers[(int)MSGs.EQUIP_STAT_RESET] = ItemHandler.ResetStat;
			handlers[(int)MSGs.EQUIP_STAT_POINT] = ItemHandler.PointStat;
            handlers[(int)MSGs.EQUIP_BEGIN_TRAINING] = ItemHandler.EquipBeginTraining;
            handlers[(int)MSGs.EQUIP_CANCEL_TRAINING] = ItemHandler.EquipCancelTraining;
            handlers[(int)MSGs.EQUIP_FINISH_TRAINING] = ItemHandler.EquipFinishTraining;
            handlers[(int)MSGs.EQUIP_END_TRAINING] = ItemHandler.EquipEndTraining;
            handlers[(int)MSGs.EQUIP_OPEN_TRAINING_SEAT] = ItemHandler.EquipOpenTrainingSeat;            
            handlers[(int)MSGs.EQUIP_CHECK_SLOT] = ItemHandler.EquipCheckSlot;
			handlers[(int)MSGs.ITEM_USE] = ItemHandler.UseItem;
			handlers[(int)MSGs.ITEM_SELL] = ItemHandler.SellItem;
			handlers[(int)MSGs.SHOP_LIST] = ShopHandler.ShopList;
			handlers[(int)MSGs.SHOP_BUY] = ShopHandler.ShopBuy;
			handlers[(int)MSGs.SHOP_REGISTER] = ShopHandler.ShopReigister;
			handlers[(int)MSGs.SHOP_FIXSHOP] = ShopHandler.ShopFixShop;
			handlers[(int)MSGs.LEAGUE_MATCH] = LeagueHandler.LeagueMatch;
			handlers[(int)MSGs.LEAGUE_MATCH_RESULT] = LeagueHandler.LeagueMatchResult;
            handlers[(int)MSGs.LEAGUE_START_MATCH] = LeagueHandler.LeagueMatchStart;
			handlers[(int)MSGs.LEAGUE_REVENGEMESSAGE] = LeagueHandler.LeagueRevengeMsg;
            handlers[(int)MSGs.LEAGUE_SET_CREW] = LeagueHandler.LeagueSetCrew;
			//handlers[(int)MSGs.LEAGUE_DISPATCH] = LeagueHandler.LeagueDispatch;
			//handlers[(int)MSGs.LEAGUE_DISPATCH_CANCEL] = LeagueHandler.LeagueDispatchCancel;
			//handlers[(int)MSGs.LEAGUE_DISPATCH_RESULT] = LeagueHandler.LeagueDispatchResult;
			//handlers[(int)MSGs.LEAGUE_DISPATCH_FINISH] = LeagueHandler.LeagueDispatchFinish;
			handlers[(int)MSGs.LEAGUE_REWARD] = LeagueHandler.LeagueReward;
			//handlers[(int)MSGs.LEAGUE_INFO] = LeagueHandler.LeagueInfo;
			//handlers[(int)MSGs.LEAGUE_GROUP] = LeagueHandler.LeagueGroup;
			//handlers[(int)MSGs.LEAGUE_GROUPS] = LeagueHandler.LeagueGroups;
			handlers[(int)MSGs.LEAGUE_LEGENDRANK] = LeagueHandler.LeagueLegend;
            handlers[(int)MSGs.LEAGUE_GET_MATCHLIST] = LeagueHandler.LeagueMatchList;
            handlers[(int)MSGs.LEAGUE_BUY_KEY] = LeagueHandler.LeagueBuyKey;
            handlers[(int)MSGs.LEAGUE_CHECK] = LeagueHandler.LeagueDivisionCheck;
			handlers[(int)MSGs.FORGE_SMITH] = ForgeHandler.SmithingSuccess;
			handlers[(int)MSGs.FORGE_CHECK_DESIGN] = ForgeHandler.CheckDesignSuccess;
			handlers[(int)MSGs.FORGE_NAME] = ForgeHandler.ChangeEquipName;
			handlers[(int)MSGs.FORGE_RUNE] = ForgeHandler.ForgeRune;
			handlers[(int)MSGs.FORGE_FUSION] = ForgeHandler.Fusion;
			handlers[(int)MSGs.FORGE_CHANGE_LOOK] = ForgeHandler.ChangeLook;
			handlers[(int)MSGs.FORGE_UPGRADE] = ForgeHandler.UpgradeForge;
			handlers[(int)MSGs.CREW_RUNE] = LeagueHandler.SetCrewRune;
			handlers[(int)MSGs.EQUIP_BUY_STAT_POINT] = ItemHandler.BuyEquipmentStatPoint;
			handlers[(int)MSGs.QUEST_ACCEPT] = QuestHandler.QuestAccept;
			handlers[(int)MSGs.QUEST_CANCEL] = QuestHandler.QuestCancel;
			handlers[(int)MSGs.QUEST_COMPLETE] = QuestHandler.QuestComplete;
			handlers[(int)MSGs.QUEST_ACHIEVEMENT_REWARD] = QuestHandler.AchievementReward;
            handlers[(int)MSGs.OPTION_PUSH] = OptionHandler.OptionPush;
			handlers[(int)MSGs.OPTION_ACCOUNT] = OptionHandler.OptionPush;
            handlers[(int)MSGs.OPTION_SET] = OptionHandler.OptionSet;
			handlers[(int)MSGs.ETC_COMMAND] = Cheat.EtcCommand;
            handlers[(int)MSGs.FRIEND_LIST] = SocialHandler.ReciveFriendList;
            handlers[(int)MSGs.FRIEND_INVITE] = SocialHandler.FriendInvite;
            handlers[(int)MSGs.FRIEND_CANCEL_INVITE] = SocialHandler.FriendInviteCancel;
            handlers[(int)MSGs.FRIEND_CONFIRM_INVITE] = SocialHandler.FriendInviteConfirm;
            handlers[(int)MSGs.FRIEND_RECOMMEND] = SocialHandler.FriendRecommend;
            handlers[(int)MSGs.FRIEND_DROP] = SocialHandler.FriendDrop;
            handlers[(int)MSGs.FRIEND_SEND_POINT] = SocialHandler.SendFriendshipPoint;
            handlers[(int)MSGs.FRIEND_RECV_POINT] = SocialHandler.ReciveFriendshipPoint;
            handlers[(int)MSGs.FRIEND_CHECK_NEW] = SocialHandler.FriendNewCheck;
            handlers[(int)MSGs.MAIL_LIST] = SocialHandler.ReciveMailList;
            handlers[(int)MSGs.MAIL_GET] = SocialHandler.GetItemInMail;
			handlers[(int)MSGs.MODE_BUY_FOREST_TICKET] = StageHandler.BuyTicket_Forest;
            handlers[(int)MSGs.MAIL_READ] = SocialHandler.ReadMail;
            handlers[(int)MSGs.SOCIAL_GET_NOTICE] = SocialHandler.GetNotice;
            handlers[(int)MSGs.SOCIAL_COUPON] = SocialHandler.RequestCoupon;
            handlers[(int)MSGs.RANK_INFO] = RankHandler.GetRankInfo;
            handlers[(int)MSGs.RANK_LIST] = RankHandler.GetRankList;
            handlers[(int)MSGs.RANK_REWARD] = RankHandler.GetRankReward;
            handlers[(int)MSGs.EVENT_BUY] = EventHandler.RecvEventGoodsBuy;
            handlers[(int)MSGs.EVENT_REWARD] = EventHandler.RecvEventGoodsReward;
			handlers[(int)MSGs.EVENT_ITEMUSE] = EventHandler.UseEventItem;
			handlers[(int)MSGs.AD_REWARD] = EventHandler.RecvAdReward;
            handlers[(int)MSGs.EVENT_RELOAD] = EventHandler.EventReload;
            handlers[(int)MSGs.EVENT_OX] = EventHandler.EventOxQuizz;
            handlers[(int)MSGs.GUILD_CREATE] = GuildHandler.GuildCreate;
            handlers[(int)MSGs.GUILD_INFO] = GuildHandler.GuildInfo;
            handlers[(int)MSGs.GUILD_MARK] = GuildHandler.GuildMark;
            handlers[(int)MSGs.GUILD_SET_MEMBER] = GuildHandler.GuildSetMember;
            handlers[(int)MSGs.GUILD_SET_CREW] = GuildHandler.GuildSetCrew;
            handlers[(int)MSGs.GUILD_SEARCH] = GuildHandler.GuildSearch;
            handlers[(int)MSGs.GUILD_REQUEST_JOIN_LIST] = GuildHandler.GuildRequestJoinList;
            handlers[(int)MSGs.GUILD_REQUEST_JOIN] = GuildHandler.GuildRequestJoin;
            handlers[(int)MSGs.GUILD_MATCH_INFO] = GuildHandler.GuildMatchInfo;
            handlers[(int)MSGs.GUILD_MATCH] = GuildHandler.GuildMatch;
            handlers[(int)MSGs.GUILD_START_MATCH] = GuildHandler.GuildStartMatch;
            handlers[(int)MSGs.GUILD_MATCH_RESULT] = GuildHandler.GuildMatchResult;
            handlers[(int)MSGs.GUILD_RANK_INFO] =GuildHandler.GuildRankInfo;

            handlers[(int)MSGs.MISSION_REWARD] = QuestHandler.MissionReward;
            handlers[(int)MSGs.MISSION_PUSHBACK] = QuestHandler.MissionPushback;
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

			brIn.BaseStream.Position = 0;  			// binary reader 용 Reset
			byte u1MSGType = brIn.ReadByte();
			u2SeqNo = brIn.ReadUInt16();
			//ServerMgr.RequestJob job = ServerMgr.Instance.GetJob(u2SeqNo);
			//if (job == null)
			//{
			//	return ERROR_ID.FAIL_TO_FIND_IN_QUEUE;
			//}
			//ushort u2RandomNo = brIn.ReadUInt16();
			//ushort u2Checksum = brIn.ReadUInt16();

			if (u1MSGType >= (ushort)MSGs.LAST || handlers[u1MSGType] == null)
			{
				return ERROR_ID.REQUEST_MESSAGETYPE_UNDEFINED;
			}

			// 2016. 08. 26. jy 재로그인시와 점검
			if(result == ERROR_ID.LOGIN_INSPECTION )
			{
				// 점검중
				PopupManager.Instance.CloseLoadingPopup();
				string Reason = brIn.ReadString();
				DateTime endTime = DateTime.FromBinary((Int64)brIn.ReadUInt64());
				DateTime serverTime = DateTime.FromBinary((Int64)brIn.ReadUInt64());
				endTime = endTime.Add(DateTime.Now - serverTime);

				PopupManager.Instance.ShowBigOKPopup(TextManager.Instance.GetText("title_notice"), Reason + string.Format(TextManager.Instance.GetText("server_check_time"), endTime.Month, endTime.Day, endTime.Hour, endTime.Minute) , ServerMgr.Instance.ApplicationShutdown);

				return ERROR_ID.NONE;
			}
			else if(result == ERROR_ID.LOGIN_CLOSE_APP || result == ERROR_ID.LOGIN_VERSION_MISMATCH)
			{
				PopupManager.Instance.CloseLoadingPopup();
				//DebugMgr.LogError("클라이언트 종료");
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetError(MSGs.NONE, result), ServerMgr.Instance.ApplicationShutdown);
				return ERROR_ID.NONE;
			}
			else if(result == ERROR_ID.LOGIN_DAY_CHANGED)
			{
				PopupManager.Instance.CloseLoadingPopup();
				//DebugMgr.LogError("재로그인 시작");
				// 튜토리얼시 클라이언트 종료
				if(Legion.Instance.cTutorial.bIng == true)
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("popup_error_app_off"), ServerMgr.Instance.ApplicationShutdown);
				else
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetError(MSGs.NONE, result), DataMgr.Instance.ReLoadUserData);

				return ERROR_ID.NONE;
			}
            else if(result == ERROR_ID.REQUEST_SESSION_NOT_FOUND || result == ERROR_ID.REQUEST_SESSION_MISMATCH)
            {
                PopupManager.Instance.CloseLoadingPopup();
                //DebugMgr.LogError("세션 만료");
                // 세션 만료하여 세션아이디를 지운다
                ServerMgr.sessionID = null;
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetError(MSGs.NONE, result), DataMgr.Instance.ReLoadUserData);
                return ERROR_ID.NONE;
            }
			else if(result != ERROR_ID.NONE && result <= ERROR_ID.LOGICAL_ERROR)
			{
				PopupManager.Instance.CloseLoadingPopup();
				//DebugMgr.LogError("서버 에러 재로그인 시작");
				// 튜토리얼시 클라이언트 종료
				if(Legion.Instance.cTutorial.bIng == true)
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("popup_error_app_off"), ServerMgr.Instance.ApplicationShutdown);
				else
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetError(MSGs.NONE, result), DataMgr.Instance.ReLoadUserData);
				
				return ERROR_ID.NONE;
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

		public static ERROR_ID DayChangeReStartClient(ERROR_ID errorID)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("...알림"), TextManager.Instance.GetText("...날 변경"), null);
			return errorID;
		}

		#if UNITY_EDITOR
		public static void SingeClientProcess(MSGs mesType, OnResponse callBack, System.Object obj1, System.Object obj2)
		{
			if ( ServerMgr.bConnectToServer == true )
				return;

			handlers[(int)mesType](ERROR_ID.NONE, callBack, null, obj1, obj2);
		}
		#endif
	}
}
