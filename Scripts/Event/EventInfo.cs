using UnityEngine;
using System;
using System.Collections.Generic;

public enum EVENT_TYPE
{
	NONE = 0,
	DAILYCHECK_MONTH,		// 월간 출석
	DAILYCHECK_WEEK,		// 주간 출석
	PENSIONGOODS,			// 연금상품
	ACCUMULATETIME,			// 접속 유지
	FLATGOODS,				// 정액제							
	PERIODGOODS,			// 기간제
	TIMERREWARD,			// 일일 시간 보상
	CHARACTERGOODS,			// 캐릭터 상품
	EQUIPSETGOODS,			// 장비 상품
	MONTHLYGOODS,			// 월간 상품						
	DISCOUNT,				// 비용할인
	DICE,					// 미니게임(주사위)
	DUNGEON,				// 던전이벤트
	ACCUMULATEGOODS,		// 결재 이벤트(누적 재화 이벤트)
	FIRSTPAYMENT,			// 첫결재 이벤트					
	ADDITIONALREWARD,		// 추가지급 이벤트
	SMALLPAYMENTGOODS,		// 천원샵
	IMMEDIATEPAYMENT,		// 즉지지급이벤트
	BUFF_EXP,				// 경험치 버프
	BUFF_GOLD,				// 골드 버프						
	ADDITIONALREWARD_ITEM,	// 아이템 추가지급 이벤트
    BOSSRUSH,               // 보스러쉬
}

public enum WEEK_DAY
{
    NONE = 0,
    MONDAY = 41,
    TUESDAY = 42,
    WEDNESDAY = 43,
    THURSDAY = 44,
    FRIDAY = 45,
    WEEKEND = 46,
}

public class DailyCheckReward
{
    public const Byte MAX_EVENTTYPE = 2;    //1 == 월간, 2 == 주간
	public const Byte MAX_DAY = 28;
    public enum EventType
    {
        Monthly = 1,
        Weekly = 2
    }
    public UInt16 u2EventID;
    public Byte u1EventType;
    public Byte u1Day;
    public Byte u1RewardItemType;
    public UInt16 u2RewardItemID;
    public UInt32 u4RewardItemCount;
    public Byte u1MultipleConditionType;
    public UInt16 u2MultipleConditionID;
    public UInt32 u4MultipleConditionCount;
    public Byte u1Multiple;

    public UInt16 Set(string[] cols)
	{
        UInt16 idx = 0;
        u2EventID = Convert.ToUInt16(cols[idx++]);
        u1EventType = Convert.ToByte(cols[idx++]);
        u1Day = Convert.ToByte(cols[idx++]);
        u1RewardItemType = Convert.ToByte(cols[idx++]);
        u2RewardItemID = Convert.ToUInt16(cols[idx++]);
        u4RewardItemCount = Convert.ToUInt32(cols[idx++]);
        u1MultipleConditionType = Convert.ToByte(cols[idx++]);
        u2MultipleConditionID = Convert.ToUInt16(cols[idx++]);
        u4MultipleConditionCount = Convert.ToUInt32(cols[idx++]);
        u1Multiple = Convert.ToByte(cols[idx++]);

        return u2EventID;
    }
}

public class EventMarbleGame
{
    public UInt16 u2EventID;
    public Goods cRollItem;
    public UInt16 u2BoardID;
    public UInt16[] au2ShopID;

    public UInt16 Set(string[] cols)
    {
        UInt16 idx = 0;
        u2EventID = Convert.ToUInt16(cols[idx++]);
        ++idx;

        cRollItem = new Goods(cols, ref idx);
        u2BoardID = Convert.ToUInt16(cols[idx++]);
        idx += 6;   // 주사위 눈굼 나올 확률 스킵

        au2ShopID = new UInt16[3];
        for(int i = 0; i < au2ShopID.Length; ++i)
        {
            au2ShopID[i] = Convert.ToUInt16(cols[idx++]);
        }

        return u2EventID;
    }
}

public class EventMarbleBoard
{
    public UInt16 u2BoardID;
    public UInt32 u4BoardPos;
    public Goods cReward;
    public Int32 u4Move;

    public UInt32 Set(string[] cols)
    {
        UInt16 idx = 0;
        u2BoardID = Convert.ToUInt16(cols[idx++]);
        idx++;
        u4BoardPos = Convert.ToUInt32(cols[idx++]);
        cReward = new Goods(cols, ref idx);
        u4Move = Convert.ToInt32(cols[idx++]);

        return u4BoardPos;
    }
}

public class EventMarbleItem
{
    public UInt16 u2ItemID;
    public string sName;
    public UInt16 u2EventID;
    public UInt32 u4Limit;
    public bool bResetByday;
    public UInt16 u2IconID;

	public string sDesc;

    public void Set(string[] cols)
    {
        UInt16 idx = 0;
        u2ItemID = Convert.ToUInt16(cols[idx++]);
        idx++;
        sName = cols[idx++];
        u2EventID = Convert.ToUInt16(cols[idx++]);
        u4Limit = Convert.ToUInt32(cols[idx++]);
        bResetByday = cols[idx] == "T" || cols[idx] == "t";
        idx++;
        u2IconID = Convert.ToUInt16(cols[idx++]);
		idx++;
		idx++;
		idx++;
		idx++;
		sDesc = cols[idx++];
    }
}

public struct EventMarbleBoardPos
{
    public UInt16 u2BoardID;
    public UInt32 u4BoardPos;
}

public struct EventReward
{
    public UInt16 u2EventID;
    public Byte eventType;
    public Byte u1RewardIndex;
    public UInt32 recordValue;
    public DateTime dtEventBegin;
    public Int64 u8EventBegin;
    public DateTime dtEventEnd;
    public Int64 u8EventEnd;

    public Byte u1EventType
    {
        get { return eventType; }
        set { eventType = value; }
    }
    public UInt32 u4RecordValue
    {
        get { return recordValue; }
        set { recordValue = value; }
     }
}

public class EventBuy
{
    public UInt16 u2EventID;
	public Byte u1EventBuyCnt;
    public DateTime dtBuyBegin;
    public Int64 u8BuyBegin;
    public DateTime dtBuyEnd;
    public Int64 u8BuyEnd;

    public void AddEventBuyCnt(Byte count)
    {
        u1EventBuyCnt += count;
    }
}

public struct EventGoodsBuy
{
    public Byte u1ItemType;
    public UInt16 u2ItemID;
    public UInt32 u4Count;

    public Byte u1SmithingLevel;
    public UInt16 u2ModelID;
    public UInt16 u2Level;
    public Byte u1Completeness;
    public UInt32[] u4Stat;
    public Byte u1SkillCount;
    public Byte[] u1SkillSlot;
}

public struct EventGoodsReward
{
    public Byte u1LastRewardIndex;
    public UInt32 u4RecordValue;
}

public class EventPackageInfo
{
	public const Byte MAX_PACKAGE_REWARD_CNT = 13;

	public UInt16 u2ID;
	public string sName;
	public Byte u1EventType;
    public Byte u1EventGroupIdx;
    public Byte u1EventGroupOrder;
    public Goods cNeedGoods;
	public Byte u1BuyLimit;
	public Goods cRewardVIPPoint;

	public Byte u1PensionNeedType;
	public UInt16 u1PensionNeedID;
	public UInt32 u4PensionNeedMin;
	public UInt32 u4PensionNeedMax;
	public UInt32 u4PensionNeedTurm;

	public Goods cRewardOnce;
	public UInt16 u2KeepDay;
	public UInt16 u2UnlockDay;

	public Goods cNeedMinPeriod;
	public Goods cNeedMaxPeriod;

	public Goods[] acPackageRewards;
	public string sPurchaseCode;
    public string OnestoreCord;
    public string iOSPrice;
	public string sBgImagePath;
	public string sEvnetNotice;

	public bool isNewProduct;		// 신상 여부
	//public string sCost;			// 원가
	public string sDiscountRate;	// 할인율

	public void Set(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;//comment
		sName = cols[idx++];
		u1EventType = Convert.ToByte(cols[idx++]);

        u1EventGroupIdx = Convert.ToByte(cols[idx++]);
        u1EventGroupOrder = Convert.ToByte(cols[idx++]);

        cNeedGoods = new Goods(cols, ref idx);

		u1BuyLimit = Convert.ToByte(cols[idx++]);

		cRewardVIPPoint = new Goods(cols, ref idx);

		u1PensionNeedType = Convert.ToByte(cols[idx++]);
		u1PensionNeedID = Convert.ToByte(cols[idx++]);
		u4PensionNeedMin = Convert.ToUInt32(cols[idx++]);
		u4PensionNeedMax = Convert.ToUInt32(cols[idx++]);
		u4PensionNeedTurm = Convert.ToUInt32(cols[idx++]);

		cRewardOnce = new Goods(cols, ref idx);

		u2KeepDay = Convert.ToUInt16(cols[idx++]);
		u2UnlockDay = Convert.ToUInt16(cols[idx++]);

		cNeedMinPeriod = new Goods(cols, ref idx);
		cNeedMaxPeriod = new Goods(cols, ref idx);

		acPackageRewards = new Goods[MAX_PACKAGE_REWARD_CNT];

		for (int i = 0; i < acPackageRewards.Length; i++) {
			acPackageRewards[i] = new Goods(cols, ref idx);
		}

		sPurchaseCode = cols[idx++];
        OnestoreCord = cols[idx++];
		iOSPrice = cols[idx++];
		sBgImagePath = cols[idx++];
		sEvnetNotice = cols[idx++];

		if(cols[idx] == "t" || cols[idx] == "T")
			isNewProduct = true;
		idx++;

		sDiscountRate = cols[idx++];
	}
}

public class AccumTimeEvent
{
    public const Byte MAX_REWARD_TIME = 5;

    public UInt16 u2EventID;
    public Byte u1Vip;
    public AccumTimeEventReward[] _timeEventReward;

    public Byte Set(string[] cols)
	{
        UInt16 idx = 0;

        u2EventID = Convert.ToUInt16(cols[idx++]);
        u1Vip = Convert.ToByte(cols[idx++]);
        _timeEventReward = new AccumTimeEventReward[MAX_REWARD_TIME];
        for(int i=0; i<MAX_REWARD_TIME; i++)
        {
            _timeEventReward[i].u2BaseTime = Convert.ToUInt16(cols[idx++]);
            _timeEventReward[i].u1RewardItemType = Convert.ToByte(cols[idx++]);
            _timeEventReward[i].u2RewardItemID = Convert.ToUInt16(cols[idx++]);
            _timeEventReward[i].u4RewardItemCount = Convert.ToUInt32(cols[idx++]);
        }

        return u1Vip;
    }
}

public struct AccumTimeEventReward
{
    public UInt16 u2BaseTime;
    public Byte u1RewardItemType;
    public UInt16 u2RewardItemID;
    public UInt32 u4RewardItemCount;
}

public class TimeEvent
{
    public const Byte MAX_REWARD_TIME = 5;

    public UInt16 u2EventID;
    public Byte u1Vip;
    public UInt32 u4TimeCount;
    public UInt32 u4TimePeriod;
    public UInt32[] u4RewardTime;
    public UInt16 u2OpenTime;
    public Byte[] u1RewardItemType;
    public UInt16[] u2RewardItemID;
    public UInt32[] u4RewardItemCount;

    public UInt16 Set(string[] cols)
	{
        UInt16 idx = 0;

        u2EventID = Convert.ToUInt16(cols[idx++]);
        u1Vip = Convert.ToByte(cols[idx++]);
        u4TimeCount = Convert.ToUInt32(cols[idx++]);
        u4TimePeriod = Convert.ToUInt32(cols[idx++]);

        u4RewardTime = new UInt32[MAX_REWARD_TIME];
        for(int i=0; i<MAX_REWARD_TIME; i++)
            u4RewardTime[i] = Convert.ToUInt32(cols[idx++]);
        u2OpenTime = Convert.ToUInt16(cols[idx++]);

        u1RewardItemType = new Byte[MAX_REWARD_TIME];
        u2RewardItemID = new UInt16[MAX_REWARD_TIME];
        u4RewardItemCount = new UInt32[MAX_REWARD_TIME];
        for(int i=0; i<MAX_REWARD_TIME; i++)
        {
            u1RewardItemType[i] = Convert.ToByte(cols[idx++]);
            u2RewardItemID[i] = Convert.ToUInt16(cols[idx++]);
            u4RewardItemCount[i] = Convert.ToUInt32(cols[idx++]);
        }

        return u2EventID;
    }
}

public class AdvertiseInfo
{
	public Byte u1Pos;
	public Byte u1ViewCount;
	public Goods cReward;

	public Byte Set(string[] cols)
	{
		UInt16 idx = 0;

		u1Pos = Convert.ToByte(cols[idx++]);
		u1ViewCount = Convert.ToByte(cols[idx++]);

		cReward = new Goods (cols, ref idx);

		return u1Pos;
	}
}

public class ClassGoodsInfo
{
	public UInt16 u2ID;
	public string sName;
	public UInt16 u2ClassID;
	public UInt16 u2Level;
	public UInt16 u2Face;
	public UInt16 u2Hair;
	public UInt16 u2HairColor;
//	public FaceModelInfo cFaceInfo;
//	public HairModelInfo cHairInfo;
//	public HairColorInfo cHairColor;
	public UInt16[] u2Equips;
	public Goods cReward;

	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;

		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;
		sName = cols[idx++];
		u2ClassID = Convert.ToUInt16(cols[idx++]);
		u2Level = Convert.ToUInt16(cols[idx++]);
		u2Face = Convert.ToUInt16(cols[idx++]);
		u2Hair = Convert.ToUInt16(cols[idx++]);
		u2HairColor = Convert.ToUInt16(cols[idx++]);

		u2Equips = new UInt16[ClassInfo.MAX_BASEEQUIP_OF_CLASS];
		for (int i = 0; i < ClassInfo.MAX_BASEEQUIP_OF_CLASS; i++) {
			u2Equips[i] = Convert.ToUInt16(cols[idx++]);
		}

		cReward = new Goods (cols, ref idx);

		return u2ID;
	}
}

public class ClassGoodsEquipInfo
{
	public UInt16 u2ID;
	public UInt16 u2Equip;

	public UInt16 u2Level;
	public Byte u1SmithingLevel;
	public Byte u1StarLevel;

	public UInt32[] au4Stats;
	public Byte[] au1Skills;

	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;

		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;

		u2Equip = Convert.ToUInt16(cols[idx++]);
		u2Level = Convert.ToUInt16(cols[idx++]);
		u1SmithingLevel = Convert.ToByte(cols[idx++]);
		u1StarLevel = Convert.ToByte(cols[idx++]);

		au4Stats = new UInt32[Server.ConstDef.EquipStatPointType+Server.ConstDef.SkillOfEquip*2];

		for (int i = 0; i < Server.ConstDef.EquipStatPointType; i++) {
			au4Stats[Server.ConstDef.SkillOfEquip+i] = Convert.ToUInt32(cols[idx++]);
		}

		au1Skills = new Byte[Server.ConstDef.SkillOfEquip];
		
		for (int i = 0; i < Server.ConstDef.SkillOfEquip; i++) {
			au1Skills[i] = Convert.ToByte(cols[idx++]);
			au4Stats[i] = Convert.ToUInt32(cols[idx++]);
		}

		for (int i = Server.ConstDef.EquipStatPointType+Server.ConstDef.SkillOfEquip; i < Server.ConstDef.EquipStatPointType+Server.ConstDef.SkillOfEquip*2; i++) {
			au4Stats[i] = 0;
		}

		return u2ID;
	}
}

public class EventDungeonShopInfo
{
	public const Byte MAX_EXCHANGE_SHOP_CNT = 20;
	public const Byte MAX_EXCHANGE_TAB_CNT = 5;

	public UInt16 u2EventID;
	public string sMainBtnName;
	public string sTitle;
	public string sDescription;
	public Byte u1EventType;
	public Byte u1UIType;
	public List<UInt16> au2ShopID;

	public string sMainBtnImagePath;
	public Byte[] au1TabCount;
	public string[] asTabSlotBG;
	public string[] asTabName;

	public string sBgImagePath;
	public string sAdventoBtnImagePath;
	public string sIllustPath;
	public string sSubscription;

	public UInt16 Set(string[] cols)
	{
		au2ShopID = new List<ushort> ();

		UInt16 idx = 0;

		u2EventID = Convert.ToUInt16(cols[idx++]);
		idx++;
		sMainBtnName = cols [idx++];
		sTitle = cols [idx++];
		sDescription = cols [idx++];
		u1EventType = Convert.ToByte(cols[idx++]);
		u1UIType = Convert.ToByte(cols[idx++]);

		for (int i = 0; i < MAX_EXCHANGE_SHOP_CNT; i++) {
			UInt16 shopId = Convert.ToUInt16(cols[idx++]);
			if (shopId > 0) {
				au2ShopID.Add (shopId);
				if(shopId < 50000) EventInfoMgr.Instance.dicFixshopEventID.Add (shopId, u2EventID);
			}
		}

		sMainBtnImagePath = cols [idx++];

		au1TabCount = new byte[MAX_EXCHANGE_TAB_CNT];
		asTabSlotBG = new string[MAX_EXCHANGE_TAB_CNT];
		asTabName = new string[MAX_EXCHANGE_TAB_CNT];

		for (int i = 0; i < MAX_EXCHANGE_TAB_CNT; i++) {
			au1TabCount[i] = Convert.ToByte(cols[idx++]);
			asTabSlotBG[i] = cols [idx++];
			asTabName[i] = cols [idx++];
		}

		sBgImagePath = cols [idx++];
		sAdventoBtnImagePath = cols [idx++];
		sIllustPath = cols [idx++];
		sSubscription = cols [idx++];

		return u2EventID;
	}
}

public class EventDungeonStageInfo
{
	public const Byte MAX_STAGE_DIFFICULT = 5;

	public UInt16 u2EventID;
	public Byte u1OpenType;
	public Goods cOpenItem;

	public Goods[] acConsumeItem;
	public UInt16[] au2StageID;
    //#ODIN [이벤트 던전 클리어 오딘 포인트] 변수
    public Goods[] acClearPoint;

	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;

		u2EventID = Convert.ToUInt16(cols[idx++]);
		idx++;

		u1OpenType = Convert.ToByte(cols[idx++]);
		cOpenItem = new Goods(cols, ref idx);

		acConsumeItem = new Goods[MAX_STAGE_DIFFICULT];
        acClearPoint = new Goods[MAX_STAGE_DIFFICULT];
        au2StageID = new UInt16[MAX_STAGE_DIFFICULT];
        
        for (int i = 0; i < MAX_STAGE_DIFFICULT; i++) {
			acConsumeItem[i] = new Goods(cols, ref idx);
            //#ODIN [이벤트 던전 클리어 오딘 포인트]
            acClearPoint[i] = new Goods(cols, ref idx);
            au2StageID[i] = Convert.ToUInt16(cols[idx++]);
        }

		return u2EventID;
	}

	public List<UInt16> IsRewardMaterialStages(UInt16 _id)
	{
		List<UInt16> stages = new List<ushort> ();
		for(int a=0; a<MAX_STAGE_DIFFICULT; a++)
		{
			if(au2StageID [a] > 0){
				StageInfo stageInfo = StageInfoMgr.Instance.dicStageData [au2StageID [a]];

				for(int i=0; i<stageInfo.RewardItems.Length; i++)
				{
					for (int j = 0; j < stageInfo.RewardItems [i].Count; j++) {
						if (stageInfo.RewardItems[i][j].cRewards.u1Type == (Byte)GoodsType.MATERIAL) {
							if (stageInfo.RewardItems [i][j].cRewards.u2ID == _id) {
								stages.Add (au2StageID [a]);
							}
						}
					}
				}
			}
		}

		return stages;
	}
}

public class EventItemBuyCountInfo
{
    public UInt16 u2EventID;
	public UInt16 u2ShopID;
	public UInt16 u2ShopGroupID;
	public UInt32 u4BuyCount;
}

public class EventDungeonOpenInfo
{
	public UInt16 u2EventID;
	public Byte u1DayScheduleType;
	public Byte u1DayInWeek;
	public Byte u1OpenDayCount;
	public Byte[] au1OpenDays;
	public Byte u1DayTurm;

	public Byte u1TimeScheduleType;
	public Byte u1OpenTimeCount;
	public DateTime[] adtOpenTime;
	public UInt16[] au2OpeningMinute;
}

public class UserEventInfo
{
	public List<EventItemBuyCountInfo> lstItemBuyHistory;
	public Dictionary<UInt16, EventDungeonOpenInfo> dicDungeonOpenInfo;
	public List<EventDungeonInfo> openStageIds;
	public UInt16 selectedOpenEventID = 0;

    public Byte MAX_OXQUESTION_CNT = 20;
    public enum QuizzState
    {
        None = 0,
        TodayQuestionDone = 9998,
        TodayRewardDone = 9999,
    }
    public UInt16 u2OXEventID = 0;
    public Byte u1OXtotalReward = 0;
    public Byte u1OXquestion = 0;
    public UInt16 u2OXlefttime = 0;
    public Byte u1OXanswer = 0;
    public Byte u1TodayOxDone = 0;

	public UserEventInfo()
	{
		lstItemBuyHistory = new List<EventItemBuyCountInfo> ();
		dicDungeonOpenInfo = new Dictionary<ushort, EventDungeonOpenInfo> ();
		openStageIds = new List<EventDungeonInfo> ();
	}

	public bool CheckOpenStage(UInt16 eventId, UInt16 stageId)
	{
		if (openStageIds.FindIndex(cs => cs.u2EventID == eventId && cs.u2StageID == stageId) < 0) {
			return false;
		}
		return true;
	}

	public bool AddOpenStage(UInt16 eventId, UInt16 stageId, Byte closed, Byte playCount)
	{
		if (openStageIds.FindIndex(cs => cs.u2EventID == eventId && cs.u2StageID == stageId) < 0) {
			EventDungeonInfo item = new EventDungeonInfo();
            item.u2EventID = eventId;
            item.u2StageID = stageId;
            item.u1Closed = closed;
            item.u1PlayCount = playCount;
			openStageIds.Add(item);
			return true;
		}
		return false;
	}

	public void RemovePlayedStage()
	{
		int idx = openStageIds.FindIndex (cs => cs.u2EventID == selectedOpenEventID && cs.u2StageID == Legion.Instance.SelectedStage.u2ID);
		if (idx >= 0) {
			openStageIds.RemoveAt (idx);
			selectedOpenEventID = 0;
		}
	}

	public void AddBuyCount(ShopGoodInfo shopInfo, UInt32 count, UInt16 eventID)
	{
		UInt16 shopID = shopInfo.u2ID;
		if (!ShopInfoMgr.Instance.dicShopGoodData.ContainsKey (shopID)) {
			DebugMgr.LogError ("Don't have shopID = " + shopID);
		}

		int idx = lstItemBuyHistory.FindIndex (cs => cs.u2ShopID == shopID);

		if (idx < 0) {
			EventItemBuyCountInfo temp = new EventItemBuyCountInfo ();
			temp.u2EventID = eventID;
			temp.u2ShopID = shopID;
			temp.u2ShopGroupID = shopInfo.u2GroupIndex;
			temp.u4BuyCount = count;
			lstItemBuyHistory.Add (temp);
		} else {
			lstItemBuyHistory [idx].u4BuyCount += count;
		}
	}

	public UInt32 GetBuyCount(UInt16 shopID)
	{
		int idx = lstItemBuyHistory.FindIndex (cs => cs.u2ShopID == shopID);

		if (idx < 0)
			return 0;

		return lstItemBuyHistory [idx].u4BuyCount;
	}
}
public enum DISCOUNT_ITEM
{
    UPGRADEFORGE,
    SMITH,
    BUYGOLD,
    CHANGELOOK,
    FUSION,
    BUYKEY,
    RESETSKILL,
    RESETSTAT,
    OPENCREW,
    OPENSKILL,
    LOTTERY,
    BUYSHOP,
    MAX
}

public class EventDisCountinfo
{
    public ushort u2EventID;
    public byte u1EventType;
    public uint u4DiscountItem;
    public byte u1DiscountRate;
	public Int64 s8EventBegin;
	public Int64 s8EventEnd;
    public DateTime dtEventBegin;
    public DateTime dtEventEnd;

    public bool[] discountItem = new bool[(byte)DISCOUNT_ITEM.MAX];
    public double discountRate;

    public void SetInfo(EventReward val)
    {
        u2EventID = val.u2EventID;
        u1EventType = val.u1EventType;
        u4DiscountItem = val.u4RecordValue;
        u1DiscountRate = val.u1RewardIndex;
		s8EventBegin = val.u8EventBegin;
		s8EventEnd = val.u8EventEnd;
        dtEventBegin = val.dtEventBegin;
        dtEventEnd = val.dtEventEnd;
    }

    public void Convert()
    {
        for (int i = 0; i < (byte)DISCOUNT_ITEM.MAX; i++)
        {
            if ((u4DiscountItem & Utillity.BitMaskType2(i + 1)) > 0) discountItem[i] = true;
            else discountItem[i] = false;
        }

		discountRate = (double)(100-u1DiscountRate) / (double)100;
    }

	public bool CheckEventTime() 
	{ 
		if(s8EventBegin+s8EventEnd == 0) 
		{ 
			return true; 
		} 

		if (Legion.Instance.ServerTime > dtEventBegin && Legion.Instance.ServerTime < dtEventEnd) 
		{ 
			return true; 
		} 

		if ((s8EventBegin == 0 && Legion.Instance.ServerTime > dtEventBegin) || (s8EventBegin == 0 && Legion.Instance.ServerTime < dtEventEnd)) 
		{ 
			return true; 
		} 

		return false; 
	}
}

public class BossRushInfo
{
    public UInt16 u2EventID;
    public Byte u1GuildType;
    public Byte u1Guage;
    public Byte u1OpenType;
    public Byte u1OpenID;
    public UInt16 u2OpenCount;
    public Byte u1OpenRetryUpCount;
    public Byte u1OpenMaxCount;
    public Byte u1RetryWhenFail;
    public Byte u1RetryWhenClose;
    public Goods[] rewardGoods;

    public Byte REWARD_STEP = 5;
    public Byte REWARD_STEP_IN_CNT = 3;

    public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;

		u2EventID = Convert.ToUInt16(cols[idx++]);
		idx++;

		u1GuildType = Convert.ToByte(cols[idx++]);
        u1Guage = Convert.ToByte(cols[idx++]);
        u1OpenType = Convert.ToByte(cols[idx++]);
        u1OpenID = Convert.ToByte(cols[idx++]);
        u2OpenCount = Convert.ToUInt16(cols[idx++]);
        u1OpenRetryUpCount = Convert.ToByte(cols[idx++]);
        u1OpenMaxCount = Convert.ToByte(cols[idx++]);
        u1RetryWhenFail = Convert.ToByte(cols[idx++]);
        u1RetryWhenClose = Convert.ToByte(cols[idx++]);

        rewardGoods = new Goods[REWARD_STEP*REWARD_STEP_IN_CNT];

        for(int i=0; i<rewardGoods.Length; i++)
        {
            //rewardGoods[i] = new Goods(Convert.ToByte(cols[idx++]), Convert.ToUInt16(cols[idx++]), Convert.ToUInt16(cols[idx++]));
            rewardGoods[i] = new Goods(cols, ref idx);
        }

		return u2EventID;
	}
}

public struct EventDungeonInfo
{
    public UInt16 u2EventID;
    public UInt16 u2StageID;
    public Byte u1Closed;
    public Byte u1PlayCount;
}

public class EventOxReward
{
    public enum REWARD_TYPE
    {
        None = 0,
        Daily = 1,
        Stack = 2,
    }

    public UInt16 u2EventID;
    public bool isQuestion;    // 문제 푸는지 여부 : TRUE == 0X 퀴즈 / FALSE == 보상
    public Byte u1Type;
    public Byte u1AnswerCnt;
    public Goods rewards;

    public const Byte MAX_DAY = 20;
    public const Byte MAX_STACKDAY = 4;
    public const Byte MAX_TYPE = 2;

    public Byte Set(string[] cols)
    {
        UInt16 idx = 0;
        u2EventID = Convert.ToUInt16(cols[idx++]);
        isQuestion = (cols[idx][0] == 'T' || cols[idx][0] == 't');
        idx++;

        u1Type = Convert.ToByte(cols[idx++]);
        u1AnswerCnt = Convert.ToByte(cols[idx++]);
        rewards = new Goods(Convert.ToByte(cols[idx++]), Convert.ToUInt16(cols[idx++]), Convert.ToUInt32(cols[idx++]));

        return u1Type;
    }
}

public class EventOxQuestion
{
    public UInt16 u2EventID;
    public Byte u1Number;
    public String strQuestion;
    public bool bAnswer;
    public Byte u1Retry;

    public Byte Set(string[] cols)
    {
        UInt16 idx = 0;
        u2EventID = Convert.ToUInt16(cols[idx++]);
        u1Number = Convert.ToByte(cols[idx++]);
        strQuestion = Convert.ToString(cols[idx++]);
        bAnswer = cols[idx][0] == 'T' || cols[idx][0] == 't';
		idx++;
        u1Retry = Convert.ToByte(cols[idx++]);

        return u1Number;
    }
}