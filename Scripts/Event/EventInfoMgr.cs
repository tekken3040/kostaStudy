using UnityEngine;
using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EventInfoMgr : Singleton<EventInfoMgr>
{
    public Dictionary<UInt16, DailyCheckReward>[,] dicDailyCheckReward;
    public List<UInt16> lstMonthlyEventID;
    public List<UInt16> lstWeeklyEventID;

    public Dictionary<UInt16, EventReward> dicEventReward;
    public Dictionary<UInt16, EventBuy> dicEventBuy;
    public Dictionary<UInt16, Goods> dicMarbleBag;
    public Dictionary<UInt16, EventMarbleGame> dicMarbleGame;
    public Dictionary<UInt32, EventMarbleBoard> dicMarbleBoardInfo;
    public Dictionary<UInt16, EventMarbleItem> dicMarbleGoods;
    public List<EventDisCountinfo> lstDiscountInfo;
	public Dictionary<UInt16, UInt16> dicFixshopEventID;

    public Byte u1EventCount;
    public Byte u1EventGoodsCount;

    public List<EventGoodsBuy> lstEventGoodsBuy;
    public EventGoodsReward sEventGoodsReward;
    public Byte u1EventGoodsItemCount;

	public Dictionary<UInt16, EventPackageInfo> dicEventPackage;

	public Dictionary<Byte, AdvertiseInfo> dicAdReward;

	public Dictionary<UInt16, ClassGoodsInfo> dicClassGoods;
	public Dictionary<UInt16, ClassGoodsEquipInfo> dicClassGoodsEquip;

    public AccumTimeEvent sAccumTimeEvent;
    public Dictionary<UInt16, TimeEvent> dicTimeEvent;
    public List<UInt16> lstTimeEventID;
    public bool OnLastReward = false;

	public Dictionary<UInt16, EventDungeonShopInfo> dicDungeonShop;
	public List<EventDungeonStageInfo> lstDungeonStage;
    public List<EventReward> lstExpBuffEvent;
    public List<EventReward> lstGoldBuffEvent;

    public UInt32 u4GoldBoostPer = 0;
    public UInt32 u4ExpBoostPer = 0;

    public List<BossRushInfo> lstBossRush;
    public byte u1BossRushProgress = 0;
    public byte u1BossRushRewardIdx = 0;

    //public Dictionary<Byte, EventOxReward>[,] dicOxReward;
    public Dictionary<UInt16, List<EventOxReward>> dicOxReward;

    //public Dictionary<Byte, EventOxQuestion> dicOxQuestion;
    public Dictionary<UInt16, List<EventOxQuestion>> dicOxQuestion;

    private bool loadedInfo = false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}

    public void Awake()
    {
		lstMonthlyEventID = new List<UInt16>();
		lstWeeklyEventID = new List<UInt16>();

		sAccumTimeEvent = new AccumTimeEvent();
		dicTimeEvent = new Dictionary<UInt16, TimeEvent>();
		lstTimeEventID = new List<UInt16>();

		dicEventPackage = new Dictionary<ushort, EventPackageInfo> ();
		dicAdReward = new Dictionary<byte, AdvertiseInfo> ();

		dicClassGoods = new Dictionary<ushort, ClassGoodsInfo> ();
		dicClassGoodsEquip = new Dictionary<ushort, ClassGoodsEquipInfo> ();
		dicDailyCheckReward = new Dictionary<UInt16, DailyCheckReward>[DailyCheckReward.MAX_EVENTTYPE+1, DailyCheckReward.MAX_DAY+1];
		for(int i=1; i<DailyCheckReward.MAX_EVENTTYPE+1; i++)
		{
			for(int j=1; j<DailyCheckReward.MAX_DAY+1; j++)
			{
				dicDailyCheckReward[i, j] = new Dictionary<UInt16, DailyCheckReward>();
			}
		}
		dicMarbleGoods = new Dictionary<ushort, EventMarbleItem>();
		dicDungeonShop = new Dictionary<ushort, EventDungeonShopInfo> ();
		lstDungeonStage = new List<EventDungeonStageInfo> ();
        lstExpBuffEvent = new List<EventReward>();
        lstGoldBuffEvent = new List<EventReward>();

		dicMarbleGame = new Dictionary<ushort, EventMarbleGame>();
		dicMarbleBoardInfo = new Dictionary<UInt32, EventMarbleBoard>();
        lstDiscountInfo = new List<EventDisCountinfo>();

		dicFixshopEventID = new Dictionary<ushort, ushort> ();
        lstBossRush = new List<BossRushInfo>();
        //dicOxReward = new Dictionary<Byte, EventOxReward>[EventOxReward.MAX_TYPE, EventOxReward.MAX_DAY + EventOxReward.MAX_STACKDAY];
        dicOxReward = new Dictionary<UInt16, List<EventOxReward>>();
        //for (int i=0; i<EventOxReward.MAX_TYPE; i++)
        //{
            //for(int j=0; j<EventOxReward.MAX_DAY + EventOxReward.MAX_STACKDAY; j++)
        //    {
        //        dicOxReward[i, j] = new Dictionary<Byte, EventOxReward>();
        //    }
        //}

        //dicOxQuestion = new Dictionary<Byte, EventOxQuestion>();
        dicOxQuestion = new Dictionary<UInt16, List<EventOxQuestion>>();

        InitUserData ();
    }

	public void InitUserData()
	{
		dicEventReward = new Dictionary<UInt16, EventReward>();
		dicEventBuy = new Dictionary<UInt16, EventBuy>();
        dicMarbleBag = new Dictionary<UInt16, Goods>();
        
        u1EventCount = 0;
		u1EventGoodsCount = 0;

		lstEventGoodsBuy = new List<EventGoodsBuy>();
		sEventGoodsReward = new EventGoodsReward();
		u1EventGoodsItemCount = 0;
    }

    public void Init()
    {
        DataMgr.Instance.LoadTable(this.AddInfo, "DailyCheck");
		DataMgr.Instance.LoadTable(this.AddPackageInfo, "EventPackage");
        DataMgr.Instance.LoadTable(this.AddTimeRewardInfo, "AccumTimeEvent");
        DataMgr.Instance.LoadTable(this.AddTimeEventInfo, "TimeEvent");
		DataMgr.Instance.LoadTable(this.AddAdvertiseInfo, "AD");
		DataMgr.Instance.LoadTable(this.AddClassGoodsInfo, "ClassGoods");
		DataMgr.Instance.LoadTable(this.AddClassGoodsEquipInfo, "ClassGoodsEquip");
        DataMgr.Instance.LoadTable(this.AddMarbleGameInfo, "DiceGame");
        DataMgr.Instance.LoadTable(this.AddMarbleBoardInfo, "DiceBoard");
		DataMgr.Instance.LoadTable(this.AddMarbleGoods, "EventItem");
		DataMgr.Instance.LoadTable(this.AddExchangeShopInfo, "Dungeon");
		DataMgr.Instance.LoadTable(this.AddDungeonStageInfo, "DungeonStage");
        DataMgr.Instance.LoadTable(this.AddBossRushInfo, "BossRushReward");
        DataMgr.Instance.LoadTable(this.AddOxRewardInfo, "OXreward");
        DataMgr.Instance.LoadTable(this.AddOxQuestionInfo, "OXquestion");
    }

    public void AddInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}

        DailyCheckReward info = new DailyCheckReward();
        info.Set(cols);
        dicDailyCheckReward[info.u1EventType, info.u1Day].Add(info.u2EventID, info);
        if(info.u1EventType == (Byte)DailyCheckReward.EventType.Monthly)
        {
            if(!lstMonthlyEventID.Contains(info.u2EventID))
                lstMonthlyEventID.Add(info.u2EventID);
        }
        else if(info.u1EventType == (Byte)DailyCheckReward.EventType.Weekly)
        {
            if(!lstWeeklyEventID.Contains(info.u2EventID))
                lstWeeklyEventID.Add(info.u2EventID);
        }
    }

	public void AddPackageInfo(string[] cols)
	{
		if (cols == null)
		{
			return;
		}

		EventPackageInfo info = new EventPackageInfo();
		info.Set(cols);
		dicEventPackage.Add(info.u2ID, info);
	}

    public void AddTimeRewardInfo(string[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        AccumTimeEvent info = new AccumTimeEvent();
        info.Set(cols);
        sAccumTimeEvent = info;
    }

    public void AddTimeEventInfo(string[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        TimeEvent info = new TimeEvent();
        info.Set(cols);
        dicTimeEvent.Add(info.u2EventID, info);
        lstTimeEventID.Add(info.u2EventID);
    }

	public void AddAdvertiseInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		AdvertiseInfo info = new AdvertiseInfo();
		info.Set(cols);
		dicAdReward.Add(info.u1Pos, info);
	}

	public void AddClassGoodsInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		ClassGoodsInfo info = new ClassGoodsInfo();
		info.Set(cols);
		dicClassGoods.Add(info.u2ID, info);
	}

	public void AddClassGoodsEquipInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		ClassGoodsEquipInfo info = new ClassGoodsEquipInfo();
		info.Set(cols);
		dicClassGoodsEquip.Add(info.u2ID, info);
	}

    public void AddMarbleGameInfo(string[] cols)
    {
        if (cols == null)
        {
            loadedInfo = true;
            return;
        }
        EventMarbleGame info = new EventMarbleGame();
        info.Set(cols);
        dicMarbleGame.Add(info.u2EventID, info);
    }

    public void AddMarbleBoardInfo(string[] cols)
    {
        if (cols == null)
        {
            loadedInfo = true;
            return;
        }

        EventMarbleBoard info = new EventMarbleBoard();
        info.Set(cols);
        dicMarbleBoardInfo.Add(info.u4BoardPos, info);
    }

    public void AddMarbleGoods(string[] cols)
    {
        if (cols == null)
        {
            loadedInfo = true;
            return;
        }

        EventMarbleItem info = new EventMarbleItem();
        info.Set(cols);
        dicMarbleGoods.Add(info.u2ItemID, info);
    }

	public void AddExchangeShopInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		EventDungeonShopInfo info = new EventDungeonShopInfo();
		info.Set(cols);
		dicDungeonShop.Add(info.u2EventID, info);
	}

	public void AddDungeonStageInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		EventDungeonStageInfo info = new EventDungeonStageInfo();
		info.Set(cols);
		lstDungeonStage.Add(info);
	}

    public void AddBossRushInfo(String[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        BossRushInfo info = new BossRushInfo();
        info.Set(cols);
        lstBossRush.Add(info);
    }

    public void AddOxRewardInfo(String[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        EventOxReward info = new EventOxReward();
        info.Set(cols);

        if(!dicOxReward.ContainsKey(info.u2EventID))
        {
            dicOxReward.Add(info.u2EventID, new List<EventOxReward>());
        }
        dicOxReward[info.u2EventID].Add(info);
        //if(info.u1Type == (Byte)EventOxReward.REWARD_TYPE.Daily)
        //    dicOxReward[info.u1Type-1, info.u1AnswerCnt-1].Add(info.u1AnswerCnt, info);
        //else
        //    dicOxReward[info.u1Type-1, (info.u1AnswerCnt/5)-1].Add(info.u1AnswerCnt, info);
    }

    public void AddOxQuestionInfo(String[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        EventOxQuestion info = new EventOxQuestion();
        info.Set(cols);

        if (!dicOxQuestion.ContainsKey(info.u2EventID))
        {
            dicOxQuestion.Add(info.u2EventID, new List<EventOxQuestion>());
        }
        dicOxQuestion[info.u2EventID].Add(info);
    }

    public EventPackageInfo GetPackageInfo(UInt16 id)
	{
		EventPackageInfo ret;
		dicEventPackage.TryGetValue(id, out ret);
		return ret;
	}

	public List<EventPackageInfo> GetEventListByEventType(Byte u1type){
		return dicEventPackage.Values.Where (cs => cs.u1EventType == u1type).ToList();
	}

    public List<EventPackageInfo> GetOpenEventListByEventType(Byte u1type)
    {
        return dicEventPackage.Values.Where(cs => cs.u1EventType == u1type && CheckBuyPossible(cs.u2ID) != 0).ToList();
    }
    
    public List<EventPackageInfo> GetSaleEventListByEventType(Byte u1type)
    {
        return dicEventPackage.Values.Where(cs => cs.u1EventType == u1type && CheckBuyPossible(cs.u2ID) == 1).ToList();
    }

    // 패키지 판매되는 상품이 있는지 확인
    public bool IsPackagePopupOpen(byte packageIdx)
    {
        // 판매 가능한 상품의 갯수가 0보다 작을때는 팝업으로 안내한다
        if (GetSaleEventListByEventType(packageIdx).Count <= 0)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("event_goods_no_sales"), null);
            return false;
        }

        return true;
    }

    public int CheckBuyPossible(UInt16 id, bool bAd = false)
    {
        // 0 오픈하지 않는 이벤트
        // 1 구매가능한 이벤트
        // 2 구매 완료된 이벤트 
		if (!EventInfoMgr.Instance.dicEventPackage.ContainsKey (id))
			return 0;
		
		if (!EventInfoMgr.Instance.dicEventBuy.ContainsKey (id))
			return 0;

		EventPackageInfo pInfo = GetPackageInfo (id);

		if (pInfo.u1EventType == (Byte)EVENT_TYPE.PERIODGOODS || pInfo.u1EventType == 7) {
			if (Legion.Instance.CheckEnoughGoods (pInfo.cNeedMinPeriod)) {
				if (pInfo.cNeedMaxPeriod.u1Type == (Byte)GoodsType.LEVEL) {
					if (Legion.Instance.TopLevel > pInfo.cNeedMaxPeriod.u4Count) {
						return 0;
					}
				}
			} else {
				return 0;
			}
		} else if (pInfo.u1EventType == (Byte)EVENT_TYPE.CHARACTERGOODS) {
			if (Legion.Instance.cTutorial.au1Step [4] != Server.ConstDef.LastTutorialStep)
				return 0;

			if (bAd) {
				//if (PlayerPrefs.GetString ("AD" + id) == DateTime.Now.Date.ToShortDateString ())
				if (PlayerPrefs.GetString ("AD" + id) == Legion.Instance.ServerTime.ToShortDateString())
					return 0;
			}
		}

		// 2016. 09. 09 jy
		// 구매한 패키지 상품도 공개하고 구매만 못하게 해달라고 하여 반환값 변경
		if (pInfo.u1BuyLimit > 0) {
			if (EventInfoMgr.Instance.dicEventBuy [id].u1EventBuyCnt >= pInfo.u1BuyLimit) {
				return 2;// return 0;
			}
		}

		if (pInfo.u2KeepDay > 0) {
			if (EventInfoMgr.Instance.dicEventReward.ContainsKey (id)) {
				if (EventInfoMgr.Instance.dicEventReward [id].u4RecordValue > pInfo.u2UnlockDay) {
					return 0;
				} else {
					return 2;
				}
			}
		}

		return 1;
	}
		
	public bool CheckRewardPossible(UInt16 id){
		if (!EventInfoMgr.Instance.dicEventReward.ContainsKey (id))
			return false;

		EventPackageInfo pInfo = GetPackageInfo (id);

		int rewardIdx = 0;
		//연금(육성패키지)
		if (pInfo.u1EventType == (Byte)EVENT_TYPE.PENSIONGOODS) {
			for (uint i = pInfo.u4PensionNeedMin; i <= pInfo.u4PensionNeedMax; i += pInfo.u4PensionNeedTurm) {
				if (Legion.Instance.CheckEnoughGoods (new Goods (pInfo.u1PensionNeedType, pInfo.u1PensionNeedID, i))) {
					rewardIdx++;
					if (rewardIdx > EventInfoMgr.Instance.dicEventReward [id].u1RewardIndex)
						return true;
				}
			}
		} else if (pInfo.u1EventType == (Byte)EVENT_TYPE.FLATGOODS) {
			if (EventInfoMgr.Instance.dicEventReward [id].u1RewardIndex == 0)
				return true;
		}

		return false;
	}

	public bool CheckRewardPossibleIndex(UInt16 id, Byte rIdx){
		if (!EventInfoMgr.Instance.dicEventPackage.ContainsKey (id))
			return false;

		if (!EventInfoMgr.Instance.dicEventReward.ContainsKey (id))
			return false;

		EventPackageInfo pInfo = GetPackageInfo (id);

		//연금(육성패키지)
		if (pInfo.u1EventType == (Byte)EVENT_TYPE.PENSIONGOODS) {
			for (uint i = pInfo.u4PensionNeedMin; i <= pInfo.u4PensionNeedMax; i += pInfo.u4PensionNeedTurm) {
				if (Legion.Instance.CheckEnoughGoods (new Goods (pInfo.u1PensionNeedType, pInfo.u1PensionNeedID, i))) {
					if (rIdx+1 > EventInfoMgr.Instance.dicEventReward [id].u1RewardIndex)
						return true;
				}
			}
		}

		return false;
	}

	public List<EventPackageInfo> CheckCharacterPackage(bool bAd){
		return dicEventPackage.Values.Where (cs => cs.u1EventType == (Byte)EVENT_TYPE.CHARACTERGOODS && CheckBuyPossible(cs.u2ID, bAd) == 1).ToList();
	}

    //======================= 마블 이벤트 관련 함수  =======================//

    // 마블이벤트가 오픈되어 있는지를 체크하여 오픈되 마블 이벤트의 아이디를 저장한다
	public EventMarbleGame GetOpenMarbleGameInfo()
    {
		foreach (UInt16 eventID in dicMarbleGame.Keys)
        {
			if(dicEventReward.ContainsKey(eventID) == true)
            {
                EventReward envetInfo = dicEventReward[eventID];
                // 끝나는 시간이 0이라면 계속 진행이므로 시간을 체크하지 않는다
                if(envetInfo.u8EventEnd == 0)
                {
					return dicMarbleGame[eventID];
                }
                // 현재 진행 시간인지를 체크한다
                if (Legion.Instance.ServerTime > envetInfo.dtEventBegin && Legion.Instance.ServerTime < envetInfo.dtEventEnd)
                {
					return dicMarbleGame[eventID];
                }
            }
        }
		return null;
    }

	public EventMarbleGame GetOpenMarbleGameInfo(UInt16 eventID)
	{
		if(dicEventReward.ContainsKey(eventID) == true)
		{
			EventReward envetInfo = dicEventReward[eventID];
			// 끝나는 시간이 0이라면 계속 진행이므로 시간을 체크하지 않는다
			if(envetInfo.u8EventEnd == 0)
			{
				return dicMarbleGame[eventID];
			}
			// 현재 진행 시간인지를 체크한다
			if (Legion.Instance.ServerTime > envetInfo.dtEventBegin && Legion.Instance.ServerTime < envetInfo.dtEventEnd)
			{
				return dicMarbleGame[eventID];
			}
		}
		return null;
	}
		
    public List<EventMarbleBoard> GetMarbleBoardInfoList(UInt16 eventID)
    {
        return dicMarbleBoardInfo.Values.Where(info => info.u2BoardID == eventID).ToList();
    }

    //======================= 마블 이벤트 관련 함수 end =======================//

    public List<EventDungeonShopInfo> CheckOpenDungeon(){
		List<EventDungeonShopInfo> temp = new List<EventDungeonShopInfo> ();
		foreach (EventDungeonShopInfo tc in dicDungeonShop.Values) {
			if (tc.u1EventType == (Byte)EVENT_TYPE.DUNGEON) {
				UInt16 eventID = tc.u2EventID;
				if (Legion.Instance.cEvent.dicDungeonOpenInfo.ContainsKey (eventID)) {
					if (CheckOpen (Legion.Instance.cEvent.dicDungeonOpenInfo [eventID])) {
						temp.Add (tc);
					}
				}
			}
		}

		return temp;
	}

	public EventDungeonShopInfo CheckOpenSmallPayShop(){
		foreach (EventDungeonShopInfo tc in dicDungeonShop.Values) {
			if (tc.u1EventType == (Byte)EVENT_TYPE.SMALLPAYMENTGOODS) {
				UInt16 eventID = tc.u2EventID;
				if (dicEventReward.ContainsKey (eventID)) {
					return tc;
				}
			}
		}

		return null;
	}

	public bool CheckOpen(EventDungeonOpenInfo openInfo){
		if (!dicEventReward.ContainsKey (openInfo.u2EventID)) {
			return false;
		}

		switch (openInfo.u1DayScheduleType) {
		case 0:
			//NoneEvent
			break;
		case 1:
			//EveryDay
			if (CheckOpenTime (openInfo)) {
				return true;
			}
			break;
		case 2:
			//DayInWeek
			if ((openInfo.u1DayInWeek & (1 << (7 - (int)Legion.Instance.ServerTime.DayOfWeek))) > 0) {
				if (CheckOpenTime (openInfo)) {
					return true;
				}
			}
			break;
		case 3:
			//FixedDayInMonth
			for (int i = 0; i < openInfo.u1OpenDayCount; i++) {
				if (openInfo.au1OpenDays[i] == (int)Legion.Instance.ServerTime.Day) {
					if (CheckOpenTime (openInfo)) {
						return true;
					}
				}
			}
			break;
		case 4:
			//StartDayAndTurm
			DateTime beginDate = dicEventReward[openInfo.u2EventID].dtEventBegin;
			if ( ((TimeSpan)(Legion.Instance.ServerTime - beginDate)).Days % openInfo.u1DayTurm == 0) {
				if (CheckOpenTime (openInfo)) {
					return true;
				}
			}
			break;
		}

		return false;
	}

	public string GetEventDate(UInt16 eventID){

		if (!Legion.Instance.cEvent.dicDungeonOpenInfo.ContainsKey (eventID))
			return "";

		EventDungeonOpenInfo openInfo = Legion.Instance.cEvent.dicDungeonOpenInfo[eventID];

		string result = "";
		switch (openInfo.u1DayScheduleType) {
		case 0:
			//NoneEvent
			break;
		case 1:
			//EveryDay
			if (openInfo.u1TimeScheduleType != 0) {
				int hour = 0;
				int min = 0;
				Byte cnt = openInfo.u1OpenTimeCount;
				for (int i = 0; i < cnt; i++) {
					if (Legion.Instance.ServerTime >= openInfo.adtOpenTime [i]
					   && (Legion.Instance.ServerTime < openInfo.adtOpenTime [i].AddMinutes (openInfo.au2OpeningMinute [i]))) {
						hour = (openInfo.adtOpenTime [i].AddMinutes (openInfo.au2OpeningMinute [i]) - Legion.Instance.ServerTime).Hours;
						min = (openInfo.adtOpenTime [i].AddMinutes (openInfo.au2OpeningMinute [i]) - Legion.Instance.ServerTime).Minutes;
					}
				}
				result = " "+String.Format (TextManager.Instance.GetText ("event_dungeon_open_time_over"), hour, min);
			}
			break;
		case 2:
			//DayInWeek
			result = "(";
			for (int i = 0; i < 7; i++) {
				if ((openInfo.u1DayInWeek & (1 << (7 - i))) > 0) {
					switch (i) {
					case 0:
						result += TextManager.Instance.GetText ("weekly_sun") + ",";
						break;
					case 1:
						result += TextManager.Instance.GetText ("weekly_mon") + ",";
						break;
					case 2:
						result += TextManager.Instance.GetText ("weekly_tue") + ",";
						break;
					case 3:
						result += TextManager.Instance.GetText ("weekly_wed") + ",";
						break;
					case 4:
						result += TextManager.Instance.GetText ("weekly_thu") + ",";
						break;
					case 5:
						result += TextManager.Instance.GetText ("weekly_fri") + ",";
						break;
					case 6:
						result += TextManager.Instance.GetText ("weekly_sat") + ",";
						break;
					}
				}
			}

			result = result.Remove (result.Length - 1, 1);
			result += ")";

			if (openInfo.u1TimeScheduleType != 0) {
				int hour = 0;
				int min = 0;
				Byte cnt = openInfo.u1OpenTimeCount;
				for (int i = 0; i < cnt; i++) {
					if (Legion.Instance.ServerTime >= openInfo.adtOpenTime [i]
						&& (Legion.Instance.ServerTime < openInfo.adtOpenTime [i].AddMinutes (openInfo.au2OpeningMinute [i]))) {
						hour = (openInfo.adtOpenTime [i].AddMinutes (openInfo.au2OpeningMinute [i]) - Legion.Instance.ServerTime).Hours;
						min = (openInfo.adtOpenTime [i].AddMinutes (openInfo.au2OpeningMinute [i]) - Legion.Instance.ServerTime).Minutes;
					}
				}
				result += " "+String.Format (TextManager.Instance.GetText ("event_dungeon_open_time_over"), hour, min);
			}
			break;
		case 3:
			//FixedDayInMonth
			string days = "";
			for (int i = 0; i < openInfo.u1OpenDayCount; i++) {
				days += openInfo.au1OpenDays [i].ToString ()+",";
			}
			days = days.Remove (days.Length - 1, 1);
			result += " "+String.Format (TextManager.Instance.GetText ("event_dungeon_open_montly"), days);

			if (openInfo.u1TimeScheduleType != 0) {
				int hour = 0;
				int min = 0;
				Byte cnt = openInfo.u1OpenTimeCount;
				for (int i = 0; i < cnt; i++) {
					if (Legion.Instance.ServerTime >= openInfo.adtOpenTime [i]
						&& (Legion.Instance.ServerTime < openInfo.adtOpenTime [i].AddMinutes (openInfo.au2OpeningMinute [i]))) {
						hour = (openInfo.adtOpenTime [i].AddMinutes (openInfo.au2OpeningMinute [i]) - Legion.Instance.ServerTime).Hours;
						min = (openInfo.adtOpenTime [i].AddMinutes (openInfo.au2OpeningMinute [i]) - Legion.Instance.ServerTime).Minutes;
					}
				}
				result += " "+String.Format (TextManager.Instance.GetText ("event_dungeon_open_time_over"), hour, min);
			}
			break;
		case 4:
			//StartDayAndTurm
			result = String.Format (TextManager.Instance.GetText ("event_dungeon_open_daily_interval"), openInfo.u1DayTurm);

			if (openInfo.u1TimeScheduleType != 0) {
				int hour = 0;
				int min = 0;
				Byte cnt = openInfo.u1OpenTimeCount;
				for (int i = 0; i < cnt; i++) {
					if (Legion.Instance.ServerTime >= openInfo.adtOpenTime [i]
						&& (Legion.Instance.ServerTime < openInfo.adtOpenTime [i].AddMinutes (openInfo.au2OpeningMinute [i]))) {
						hour = (openInfo.adtOpenTime [i].AddMinutes (openInfo.au2OpeningMinute [i]) - Legion.Instance.ServerTime).Hours;
						min = (openInfo.adtOpenTime [i].AddMinutes (openInfo.au2OpeningMinute [i]) - Legion.Instance.ServerTime).Minutes;
					}
				}
				result += " "+String.Format (TextManager.Instance.GetText ("event_dungeon_open_time_over"), hour, min);
			}
			break;
		}

		return result;
	}

	byte WeekBit(int dayInWeek)
	{
		switch (dayInWeek)
		{
		case 6: return 0x01;
		case 5: return 0x02;
		case 4: return 0x04;
		case 3: return 0x08;
		case 2: return 0x10;
		case 1: return 0x20;
		case 0: return 0x40;
		}
		return 0x7F;
	}

	bool CheckOpenTime(EventDungeonOpenInfo openInfo){
		if (openInfo.u1TimeScheduleType == 0)
			return true;

		Byte cnt = openInfo.u1OpenTimeCount;
		for (int i = 0; i < cnt; i++) {
			DateTime temp = DateTime.Today.AddHours(openInfo.adtOpenTime [i].Hour).AddMinutes(openInfo.adtOpenTime [i].Minute);
			if (Legion.Instance.ServerTime >= temp
				&& (Legion.Instance.ServerTime < temp.AddMinutes(openInfo.au2OpeningMinute[i])) ) {
				return true;
			}
		}

		return false;
	}

    public EventDisCountinfo GetDiscountEventinfo(DISCOUNT_ITEM type)
    {
        EventDisCountinfo info = lstDiscountInfo.Find
        (cs => cs.discountItem[(uint)type] == true);

        if (info == null)
        {
            return null;
        }

		if (info.CheckEventTime())
        {
            return info;
        }

        return null;
    }

    public void SetDisCountEventList()
    {
        // 비용할인 이벤트 추가
        foreach (KeyValuePair<UInt16, EventReward> val in EventInfoMgr.Instance.dicEventReward)
        {
            if (val.Value.u1EventType == (byte)EVENT_TYPE.DISCOUNT)
            {
                EventDisCountinfo tempinfo = new EventDisCountinfo();
                tempinfo.SetInfo(val.Value);
                tempinfo.Convert();
                lstDiscountInfo.Add(tempinfo);
            }
        }
    }


	public UInt32 GetEventItemCount(UInt16 itemID){
		if (dicMarbleBag.ContainsKey (itemID)) {
			return dicMarbleBag [itemID].u4Count;
		}

		return 0;
	}

    /// ======================== AD SET =============================//
    public const string AD_PREF_LIST_KEY = "ADPerfList";
    private Dictionary<UInt16, string> dirADMarkList = new Dictionary<UInt16, string>();
    private string ADPrefString;

    // 보지 않을 이벤트 팝업 목록을 로드한다
    public void LoadADPref()
    {
        ADPrefString = PlayerPrefs.GetString(AD_PREF_LIST_KEY,"");
        if (ADPrefString == "" || ADPrefString == null)
            return;

        DivisionADList(ADPrefString);
    }

    // 문자열로된 목록을 쪼개서 저장한다
    private void DivisionADList(string prefString)
    {
        // KeyValue 묶음을 잘라낸다
        string[] division = prefString.Split(',');
        for(int i = 0; i < division.Length; ++i)
        {
            // key 값과 value 값을 서로 잘라낸다
            string[] keyValueDiv = division[i].Split('.');
            if (keyValueDiv.Length != 2)
                continue;

            UInt16 key = UInt16.Parse(keyValueDiv[0]);
            if (dirADMarkList.ContainsKey(key) == false)
                dirADMarkList.Add(key, keyValueDiv[1]);
        }
    }

    // 보지 않을 이벤트 팝업을 추가한다
    public void AddADMark(UInt16 key)
    {
        // 저장소가 셋팅되지 않았고 이미 존재하는 키값은 저장하지 않는다
        if (dirADMarkList != null && dirADMarkList.ContainsKey(key) == true)
            return;

        // 저장할 Value 값을 불러온다
        StringBuilder tempString = new StringBuilder();
        string shortDate = Legion.Instance.ServerTime.ToShortDateString();
        // 맨처음 부분인지 확인하여 셋팅한다
        if(dirADMarkList.Count > 0)
            tempString.Append(ADPrefString).Append(",").Append(key.ToString()).Append(".").Append(shortDate);
        else
            tempString.Append(key.ToString()).Append(".").Append(shortDate);

        dirADMarkList.Add(key, shortDate);
        ADPrefString = tempString.ToString();
        PlayerPrefs.SetString(AD_PREF_LIST_KEY, ADPrefString);
        //DebugMgr.LogError(PlayerPrefs.GetString(AD_PREF_LIST_KEY));    // 테스트 용 추후 삭제
    }

    // 보지 않을 이벤트 목록을 삭제한다
    public void DeleteADPref()
    {
        if (PlayerPrefs.HasKey(AD_PREF_LIST_KEY) == true)
            PlayerPrefs.DeleteKey(AD_PREF_LIST_KEY);
    }

    // 보지 않을 이벤트 목록인지 확인한다
    public bool CheckADValue(UInt16 eventSN)
    {
        if(dirADMarkList.ContainsKey(eventSN) == true)
        {
            if (dirADMarkList[eventSN] == Legion.Instance.ServerTime.ToShortDateString())
                return true;
        }

        return false;
    }

	public void CheckChangeEvent(UInt16 eventID, UInt16 originID = 0){
		if (eventID == 0) {
			if (originID != 0) {
				//removed event command
			}
		}else{
			if (!EventInfoMgr.Instance.dicEventReward.ContainsKey (eventID)) {
				PopupManager.Instance.ShowLoadingPopup (1);
				Server.ServerMgr.Instance.RequestEventReload (EventReloadResult);
			}
		}
		Legion.Instance.effectEventID = 0;
	}

	protected void EventReloadResult(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EVENT_RELOAD, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			
		}
	}

    public EventOxReward GetOXReward(int rewardIdx)
    {
        if (rewardIdx < 0)
        { 
            return null;
        }
        else if (!dicOxReward.ContainsKey(Legion.Instance.cEvent.u2OXEventID))
        {
            return null;
        }
        else if(dicOxReward[Legion.Instance.cEvent.u2OXEventID].Count <= rewardIdx)
        {
            return null;
        }

        return dicOxReward[Legion.Instance.cEvent.u2OXEventID][rewardIdx];
    }

    public bool IsOXQuestion()
    {
        EventOxReward oxReward = GetOXReward(0);
        if(oxReward == null)
        {
            return false;
        }
        return oxReward.isQuestion;
    }

    public EventOxReward GetTypeOfOXReward(EventOxReward.REWARD_TYPE eType,  int rewardIdx)
    {
        if (rewardIdx < 0)
        {
            return null;
        }
        else if (!dicOxReward.ContainsKey(Legion.Instance.cEvent.u2OXEventID))
        {
            return null;
        }

        int rewardCount = dicOxReward[Legion.Instance.cEvent.u2OXEventID].Count;
        for (int i = 0, j = 0; i< rewardCount; ++i)
        {
            if(dicOxReward[Legion.Instance.cEvent.u2OXEventID][i].u1Type == (int)eType)
            {
                if(j == rewardIdx)
                {
                    return dicOxReward[Legion.Instance.cEvent.u2OXEventID][i];
                }
                j++;
            }
        }

        return null;
    }

    public EventOxQuestion GetOXQuestion(int questionIdx)
    {
        if (questionIdx < 0)
        {
            return null;
        }
        else if (!dicOxQuestion.ContainsKey(Legion.Instance.cEvent.u2OXEventID))
        {
            return null;
        }
        else if (dicOxQuestion[Legion.Instance.cEvent.u2OXEventID].Count <= questionIdx)
        {
            return null;
        }

        return dicOxQuestion[Legion.Instance.cEvent.u2OXEventID][questionIdx];
    }

    IEnumerator OxTimer()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            Legion.Instance.cEvent.u2OXlefttime--;
            if(Legion.Instance.cEvent.u2OXlefttime <= 0)
            {
                Legion.Instance.cEvent.u2OXlefttime = 0;
                yield break;
            }
        }
    }

    public EventDungeonStageInfo GetEventDungeonStageInfo(UInt16 eventID)
    {
        for(int i = 0; i < lstDungeonStage.Count; ++i)
        {
            if(lstDungeonStage[i].u2EventID == eventID)
            {
                return lstDungeonStage[i];
            }
        }

        return null;
    }

    // 이벤트 던전 입장 아이템 찾기
    public Goods GetEventDungeonClearPoint(UInt16 stageID)
    {
        for (int i = 0; i < lstDungeonStage.Count; ++i)
        {
            for(int j = 0; j < lstDungeonStage[i].au2StageID.Length; ++j)
            {
                if (lstDungeonStage[i].au2StageID[j] == 0)
                {
                    break;
                }

                if (lstDungeonStage[i].au2StageID[j] == stageID)
                {
                    return lstDungeonStage[i].acClearPoint[j];
                }
            }   
        }

        return null;
    }
}
