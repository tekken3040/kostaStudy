using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TimeGoods
{
    public Goods goods;
    public UInt32 chargeTime;
    public UInt32 limitGoodCount;
    private UInt32 maxGoodCount;
    public UInt32 MAX_COUNT
    {
        set { maxGoodCount = value; }
        get
        {
            if(goods.u1Type == (Byte)GoodsType.KEY)
                return maxGoodCount + LegionInfoMgr.Instance.GetVipInfo(Legion.Instance.u1VIPLevel).u2AddMaxEnergy;
            else
                return maxGoodCount;
        }
    }
}

public class LegionInfoMgr : Singleton<LegionInfoMgr>
{
    public const UInt16 LEAGUE_CONTENT_ID = 1001;
    public const UInt16 RANK_CONTENT_ID = 1002;

    public float fGroupCool;
	public float fCameraMoveDistance = 0f;
	public float fCameraRotateDistance = 0f;
	public float fCameraMoveSpd = 0f;
	public float fCameraMoveAccelation = 0f;

	public float fCameraDistance = 0f;
	public float fCameraHeight = 0f;
	public float fModelHeight = 0f;

	public int MaxComboCount = 0;
	public int MaxComboAddPerDamge = 0;

	public UInt16 u2LevelDamageVal = 0;

	public Goods[][] acCrewOpenGoods;

	public Goods[] acSkillOpenGoods;

	public Goods cSkillResetGoods;

    public float skillResetUpgrade = 0;
    public float skillResetPriceMax = 0;

	public Goods cCharStatResetGoods;

    public float charResetUpgrade = 0;
    public float charResetPriceMax = 0;

	public Goods cEquipStatResetGoods;

    public float equipResetUpgrade = 0;
    public float equipResetPriceMax = 0;
    
	public int limitCharSlot = 0;
	public UInt16 maxItemCount = 0;
	public Byte baseSkillPoint = 0;
	public int SizeOfBag = 0;
    
    public TimeGoods keyTime;
    public TimeGoods leagueKeyTime;

	public Dictionary<Byte, VipInfo> dicVipData;

	public float fMaxCameraDistance;
	public float fMinCameraDistance;
	public float fMaxCameraRotationLR;
	public float fMaxCameraRotationTop;
	public float fMaxCameraRotationBottom;

	public float fMinCharctersSpace;
	public float fMinCharSpaceZoomPer;
	public float fMaxCharctersSpace;
	public float fMaxCharSpaceZoomPer;

	public float fBasicCameraDistance;
	public float fBasicCameraHeight;
	public float fBasicCamaraViewPtHeight;

    public Byte _u1DatelineHour;

	private UInt32 _u4GoldMax;
	public UInt32 GoldMax { get { return _u4GoldMax; } }
	private UInt32 _u4CashMax;
	public UInt32 CashMax { get { return _u4CashMax; } }
	private Goods _cReviewReward;
	public Goods ReviewReward { get { return _cReviewReward; } } 

    public struct LeagueKeyCharge
    {
        public Goods _cLeagueKeyCharge;
        public UInt16 u2LeagueKeyCharge;
        public UInt16 u2LeagueKeyChargeUpgrade;
        public UInt16 u2LeagueKeyChargePriceMax;
    }
    private LeagueKeyCharge sLeagueKeyCharge;
    public LeagueKeyCharge GetLeagueKeyCharge {get {return sLeagueKeyCharge; } }

    public struct ContentOpenInfo
    {
       public UInt16 uID;           // 컨텐츠 ID
       public Goods cConditions;    // 오픈 조건
       public string sProtocol;     // 프로토콜
    }

    public Dictionary<UInt16, ContentOpenInfo> dicContentOpenData;
    public UInt16 u2ContentOpenCode;

    void Awake()
    {
        dicVipData = new Dictionary<Byte, VipInfo>();
        dicContentOpenData = new Dictionary<ushort, ContentOpenInfo>();
    }

	private bool loadedInfo=false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}
	
	public void ReadConst(string[] cols)
	{
        if (cols == null)
		{
//			loadedInfo = true;
			return;
		}
        
		UInt16 idx = 0;
		fGroupCool = ((float)Convert.ToDouble(cols[idx++])/1000f);
		idx++;	// comment

		fCameraMoveDistance = (float)Convert.ToDouble(cols[idx++]);
		fCameraRotateDistance = (float)Convert.ToDouble(cols[idx++]);
		fCameraMoveSpd = (float)Convert.ToDouble(cols[idx++]);
		fCameraMoveAccelation = (float)Convert.ToDouble(cols[idx++]);

		fCameraDistance = (float)Convert.ToDouble(cols[idx++]);
		fCameraHeight = (float)Convert.ToDouble(cols[idx++]);
		fModelHeight = (float)Convert.ToDouble(cols[idx++]);

		MaxComboCount = Convert.ToInt32(cols[idx++]);
		MaxComboAddPerDamge = Convert.ToInt32(cols[idx++]);

		u2LevelDamageVal = Convert.ToUInt16(cols[idx++]);

        acCrewOpenGoods = new Goods[Legion.MAX_CREW_OF_LEGION][];

        for(int i=0; i<Legion.MAX_CREW_OF_LEGION; i++)
        {
            acCrewOpenGoods[i] = new Goods[Crew.MAX_CHAR_IN_CREW];
			for(int j=0; j<Crew.MAX_CHAR_IN_CREW; j++){
				acCrewOpenGoods[i][j] = new Goods(cols, ref idx);
			}
        }

		acSkillOpenGoods = new Goods[6];

		for (int i=0; i<acSkillOpenGoods.Length; i++) {
			acSkillOpenGoods[i] = new Goods(cols, ref idx);
		}
        
		cSkillResetGoods = new Goods(cols, ref idx);

        skillResetUpgrade = (float)Convert.ToDouble(cols[idx++]);
        skillResetPriceMax = (float)Convert.ToDouble(cols[idx++]);
        
		cCharStatResetGoods = new Goods(cols, ref idx);

        charResetUpgrade = (float)Convert.ToDouble(cols[idx++]);
        charResetPriceMax = (float)Convert.ToDouble(cols[idx++]);
        
		cEquipStatResetGoods = new Goods(cols, ref idx);

        equipResetUpgrade = (float)Convert.ToDouble(cols[idx++]);
        equipResetPriceMax = (float)Convert.ToDouble(cols[idx++]);

        limitCharSlot = Convert.ToInt32(cols[idx++]);
		maxItemCount = Convert.ToUInt16(cols[idx++]);
		baseSkillPoint = Convert.ToByte(cols[idx++]);
		SizeOfBag = Convert.ToInt32(cols[idx++]);

		fMaxCameraDistance = (float)Convert.ToDouble(cols[idx++]);
		fMinCameraDistance = (float)Convert.ToDouble(cols[idx++]);
		fMaxCameraRotationLR = (float)Convert.ToDouble(cols[idx++]);
		fMaxCameraRotationTop = (float)Convert.ToDouble(cols[idx++]);
		fMaxCameraRotationBottom = (float)Convert.ToDouble(cols[idx++]);

		fMinCharctersSpace = (float)Convert.ToDouble(cols[idx++]);
		fMinCharSpaceZoomPer = (float)Convert.ToDouble(cols[idx++]);
		fMaxCharctersSpace = (float)Convert.ToDouble(cols[idx++]);
		fMaxCharSpaceZoomPer = (float)Convert.ToDouble(cols[idx++]);

		fBasicCameraDistance = (float)Convert.ToDouble(cols[idx++]);
		fBasicCameraHeight = (float)Convert.ToDouble(cols[idx++]);
		fBasicCamaraViewPtHeight = (float)Convert.ToDouble(cols[idx++]);

        _u1DatelineHour = Convert.ToByte(cols[idx++]); // 게임 날짜 갱신 시간
		_u4GoldMax = Convert.ToUInt32(cols[idx++]);
		_u4CashMax = Convert.ToUInt32(cols[idx++]);
		_cReviewReward = new Goods(Convert.ToByte(cols[idx++]), Convert.ToUInt16(cols[idx++]), Convert.ToUInt32(cols[idx++]));
	}
    
    public void ReadTimeTable(string[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        
        int idx = 0;
        
        keyTime = new TimeGoods();
        
        Byte type = Convert.ToByte(cols[idx++]);
        UInt16 id = Convert.ToUInt16(cols[idx++]);
        UInt32 count = Convert.ToUInt32(cols[idx++]);
        
        keyTime.goods = new Goods(type, id, count);
        keyTime.chargeTime = Convert.ToUInt32(cols[idx++]);
        keyTime.MAX_COUNT = Convert.ToUInt32(cols[idx++]);
        keyTime.limitGoodCount = Convert.ToUInt32(cols[idx++]);

        leagueKeyTime = new TimeGoods();
        
        type = Convert.ToByte(cols[idx++]);
        id = Convert.ToUInt16(cols[idx++]);
        count = Convert.ToUInt32(cols[idx++]);
        
        leagueKeyTime.goods = new Goods(type, id, count);
        leagueKeyTime.chargeTime = Convert.ToUInt32(cols[idx++]);
        leagueKeyTime.MAX_COUNT = Convert.ToUInt32(cols[idx++]);
        leagueKeyTime.limitGoodCount = Convert.ToUInt32(cols[idx++]);

        sLeagueKeyCharge = new LeagueKeyCharge();

        type = Convert.ToByte(cols[idx++]);
        id = Convert.ToUInt16(cols[idx++]);
        count = Convert.ToUInt32(cols[idx++]);
        
        sLeagueKeyCharge._cLeagueKeyCharge = new Goods(type, id, count);
        sLeagueKeyCharge.u2LeagueKeyCharge = Convert.ToUInt16(cols[idx++]);
        sLeagueKeyCharge.u2LeagueKeyChargeUpgrade = Convert.ToUInt16(cols[idx++]);
        sLeagueKeyCharge.u2LeagueKeyChargePriceMax = Convert.ToUInt16(cols[idx++]);
    }

    public void AddVipInfo(string[] cols)
	{
		if (cols == null)
        {
            loadedInfo = true;
            return;
        }
		
		VipInfo vipInfo = new VipInfo();
		vipInfo.Set(cols);
		dicVipData.Add(vipInfo.u1Level, vipInfo);
	}

    public void ReadContentOpenTable(string[] cols)
    {
        if (cols == null)
        {
            loadedInfo = true;
            return;
        }

        int idx = 0;

        ContentOpenInfo addOpenInfo;
        addOpenInfo.uID = Convert.ToUInt16(cols[idx++]);
        addOpenInfo.cConditions = new Goods(cols, ref idx);
        addOpenInfo.sProtocol = cols[idx++];

        dicContentOpenData.Add(addOpenInfo.uID, addOpenInfo);
    }


    public VipInfo GetVipInfo(Byte u2Level)
	{
		VipInfo ret = new VipInfo();
		dicVipData.TryGetValue(u2Level, out ret);
		return ret;
	}

    public VipInfo GetCurrentVIPInfo()
    {
        VipInfo ret = new VipInfo();
        dicVipData.TryGetValue(Legion.Instance.u1VIPLevel, out ret);
        return ret;
    }

    public bool SetAddVipPoint(ShopGoodInfo _Info)
    {
        bool _bLevelUp = false;
        switch ((GoodsType)_Info.cVipGoods.u1Type)
        {
            
            case GoodsType.ODIN_POINT:
                _bLevelUp = Legion.Instance.AddVIPPoint(_Info.cVipGoods.u4Count);
                break;
            case GoodsType.CASH:
            case GoodsType.GOLD:
            case GoodsType.KEY:
                break;
        }
        return _bLevelUp;
    }

    public bool SetAddVipPoint(EventPackageInfo _Info)
    {
        bool _bLevelUp = false;

        _bLevelUp = Legion.Instance.AddVIPPoint(_Info.cRewardVIPPoint.u4Count);

        return _bLevelUp;
    }

    public int GetAddVipValue(ShopGoodInfo _Info)
    {
        int nAdd = 0;
        switch ((GoodsType)_Info.cShopItem.u1Type)
        {
            case GoodsType.GOLD:
                nAdd = (int)((_Info.cShopItem.u4Count *GetCurrentVIPInfo().acBuyBonus[0].u4Count/100));
                break;
            case GoodsType.CASH:
                nAdd = (int)((_Info.cShopItem.u4Count *GetCurrentVIPInfo().acBuyBonus[1].u4Count)/100);
                break;
            case GoodsType.KEY:
                break;
        }
        return nAdd;
    }

	public void Init()
	{
		DataMgr.Instance.LoadTable(this.ReadConst, "Legion");
        DataMgr.Instance.LoadTable(this.ReadTimeTable, "Time");
		DataMgr.Instance.LoadTable(this.AddVipInfo, "VIPData");
        DataMgr.Instance.LoadTable(this.ReadContentOpenTable, "ContentsOpen"); 

        {
            VipInfo vipInfo = new VipInfo();
            vipInfo.setDefault(0);
            dicVipData.Add(0, vipInfo);    
        }
	}

    public UInt16 GetMaxInvenSize()
    {
        UInt16 tempInven = 0;

        tempInven = (UInt16)(SizeOfBag + (Legion.Instance.acHeros.Count*10));

        return tempInven;
    }

    // ================== 컨텐츠 제한 확인 ============== // 

    public bool IsContentOpen(UInt16 contentID)
    {
        bool isOpen = false;
        // 서버에 저장된 값으로 체크한다
        switch (contentID)
        {
            case LEAGUE_CONTENT_ID:
                if (((u2ContentOpenCode & 0x01) == 0x01) && Legion.Instance.sName != "")
                    isOpen = true;
                break;
            case RANK_CONTENT_ID:
                if ((u2ContentOpenCode & 0x02) == 0x02)
                    isOpen = true;
                break;
        }

        // 현재 유저 정보를 기반으로 다시 한번 체크한다
        if(isOpen == false)
        {
            isOpen = CheckCurUserInfoReContentOpen(contentID);
            if((contentID == LEAGUE_CONTENT_ID) && Legion.Instance.sName == "")
                isOpen = false;
        }

        return isOpen;
    }

    private bool CheckCurUserInfoReContentOpen(UInt16 contentID)
    {
        if (dicContentOpenData.ContainsKey(contentID) == false)
        {
            DebugMgr.LogError("확인 할 수 없는 컨텐츠 키값 = " + contentID);
            return false;
        }

        Goods conditionsInfo = dicContentOpenData[contentID].cConditions;
        if (Legion.Instance.CheckEnoughGoods(conditionsInfo) == true)
            return true;
        else
            return false;
    }

    public void ShowContentNoticePopup(UInt16 contentID)
    {
        if (dicContentOpenData.ContainsKey(contentID) == false)
        {
            DebugMgr.LogError("확인 할 수 없는 컨텐츠 키값 = " + contentID);
            return;
        }

        Goods conditionsInfo = dicContentOpenData[contentID].cConditions;
        PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"),
            string.Format(TextManager.Instance.GetText("open_contetns"),
            Legion.Instance.GetConsumeString(conditionsInfo.u1Type),
            conditionsInfo.u4Count.ToString()), null);
    }
    // ================== 컨텐츠 제한 확인 end ============== // 
}