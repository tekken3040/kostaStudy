using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EquipmentInfoMgr : Singleton<EquipmentInfoMgr>
{
	private Dictionary<UInt16, EquipmentInfo> dicData;
	private Dictionary<UInt16, EquipmentScrollSetInfo> dicScrollSetData;
	public int statPointPerLevel;
	public int skillPointPerLevel;
	public int skillMaxPoint;
	public int lvPerCharStatPoint;
	public int lvPerEquipStatPoint;
	public int lvPerSkillPoint;
	
	//캐릭터 스탯 구매 관련
    private int limitBuyCharStatPoint;
    public int LIMIT_CHAR_STATPOINT
    {
        get{ return limitBuyCharStatPoint + LegionInfoMgr.Instance.GetCurrentVIPInfo().u1AddStatusBuyPt; }
    }
	public Goods cCharStatGoods;
	public int BuyCharStatAddGoods;
	public int BuyCharStatMaxGoods;
	
	//장비 스탯 구매 관련
    private int limitBuyEquipStatPoint;
    public int LIMIT_EQUIP_STATPOINT
    {
        get{ return limitBuyEquipStatPoint + LegionInfoMgr.Instance.GetCurrentVIPInfo().u1AddEquipBuyPt; }
    }
	public Goods cEquipStatGoods;
	public int BuyEquipStatAddGoods;
	public int BuyEquipStatMaxGoods; 
	
    private int limitBuySkillPoint;
    public int LIMIT_SKILLPOINT
    {
        get{ return limitBuySkillPoint + LegionInfoMgr.Instance.GetCurrentVIPInfo().u1AddSkillBuyPt; }
    }
	public Goods cSkillPointGoods;
	public int BuySkillPointAddGoods;
	public int BuySkillPointMaxGoods; 

    public UInt16 u2BaseStatPointExp;
    public int PlusStatPointExp;
    public int MinusStatPointExp;
    public UInt16 u2EquipHPStatPointMax;
    public UInt16 u2EquipStrStatpointMax;
    public UInt16 u2EquipIntStatPointMax;
    public UInt16 u2EquipDefStatPointMax;
    public UInt16 u2EquipResStatPointMax;
    public UInt16 u2EquipAgiStatPointMax;
    public UInt16 u2EquipCriStatPointMax;
    public UInt16 u2EquipSkillPointMax;
    public UInt16 u2EquipStatPointTotalMax;
    public Double[] fCompleteness;

	private bool loadedInfo=false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}
	
	public void AddInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		EquipmentInfo info = new EquipmentInfo();
		dicData.Add(info.SetInfo(cols), info);
	}

	public EquipmentInfo GetInfo(UInt16 id)
	{
		EquipmentInfo ret;
		dicData.TryGetValue(id, out ret);
		return ret;
	}

	public UInt16[] GetIDs()
	{
		UInt16[] ret = new UInt16[dicData.Keys.Count];
		dicData.Keys.CopyTo(ret, 0);
		return ret;
	}

	public List<EquipmentInfo> GetList(UInt16 classID, Byte posID)
	{
		List<EquipmentInfo> ret = new List<EquipmentInfo>();
		ret.Clear();
		UInt16[] equipIDs = GetIDs();
		for(int i=0; i<equipIDs.Length; i++)
		{
			if(GetInfo(equipIDs[i]) != null)
			{
				EquipmentInfo equipInfo = GetInfo(equipIDs[i]);
				if(equipInfo.u2ClassID == classID && (Byte)equipInfo.u1PosID == posID)
				{
					ret.Add(equipInfo);
				}
			}
		}
		return ret;
	}

	public List<EquipmentInfo> GetListOwnDesign(UInt16 classID, Byte posID)
	{
		List<EquipmentInfo> ret = new List<EquipmentInfo>();
		ret.Clear();
		UInt16[] equipIDs = GetIDs();

		for(int i=0; i<equipIDs.Length; i++)
		{
			if(GetInfo(equipIDs[i]) != null)
			{
				EquipmentInfo equipInfo = GetInfo(equipIDs[i]);
				if(!equipInfo.bCreate) continue;

				try{
					if(equipInfo.u2ClassID == classID && (Byte)equipInfo.u1PosID == posID && Legion.Instance.acEquipDesign.Get(equipIDs[i]-Server.ConstDef.BaseEquipDesignID) )
					{
						ret.Add(equipInfo);
					}
				}catch(System.Exception ex){
//					DebugMgr.Log("-- Param --");
//					DebugMgr.Log("Class ID : "+ classID + " POS ID : " + posID);
//					DebugMgr.Log("-- Value --");
//					DebugMgr.Log("Class ID : "+ equipInfo.u2ClassID + " POS ID : " + (Byte)equipInfo.u1PosID + " GET ID : " + (equipIDs[i]-Server.ConstDef.BaseEquipDesignID));
				}

			}
		}
		return ret;
	}
    public List<EquipmentInfo> GetListAllDesign()
	{
		List<EquipmentInfo> ret = new List<EquipmentInfo>();
		ret.Clear();
		UInt16[] equipIDs = GetIDs();

		for(int i=0; i<equipIDs.Length; i++)
		{
			if(GetInfo(equipIDs[i]) != null)
			{
				EquipmentInfo equipInfo = GetInfo(equipIDs[i]);
				if(!equipInfo.bCreate) continue;

				try{
					if(Legion.Instance.acEquipDesign.Get(equipIDs[i]-Server.ConstDef.BaseEquipDesignID) )
					{
						ret.Add(equipInfo);
					}
				}catch(System.Exception ex){
//					DebugMgr.Log("-- Param --");
					//DebugMgr.Log("Class ID : "+ classID + " POS ID : " + posID);
//					DebugMgr.Log("-- Value --");
//					DebugMgr.Log("Class ID : "+ equipInfo.u2ClassID + " POS ID : " + (Byte)equipInfo.u1PosID + " GET ID : " + (equipIDs[i]-Server.ConstDef.BaseEquipDesignID));
				}

			}
		}
		return ret;
	}
	public List<UInt16> GetListOwnLookDesign(UInt16 classID, Byte posID, UInt16 baseModelID)
	{
		List<UInt16> ret = new List<UInt16>();
		ret.Clear();
		UInt16[] equipIDs = GetIDs();
		DebugMgr.Log("equipIDs : " + equipIDs.Length);
		for(int i=0; i<equipIDs.Length; i++)
		{
			EquipmentInfo equipInfo;
			if(dicData.TryGetValue(equipIDs[i], out equipInfo))
			{
				if(!equipInfo.bCreate) continue;

				if(ret.Contains(equipInfo.u2ModelID)) continue; // 현재 선택된 모델정보는 리스트에 보여주지 않음
				try{
					if(equipInfo.u2ClassID == classID && (Byte)equipInfo.u1PosID == posID &&
						Legion.Instance.acLookDesign.Get(equipInfo.u2ModelID-Server.ConstDef.BaseLookDesignID) )
					{
						ret.Add(equipInfo.u2ModelID);
					}
				}catch(System.Exception ex){
//					DebugMgr.Log("-- Param --");
//					DebugMgr.Log("Class ID : "+ classID + " POS ID : " + posID);
//					DebugMgr.Log("-- Value --");
				}

			}
		}
		return ret;
	}
    public List<UInt16> GetListAllLookDesign()
	{
		List<UInt16> ret = new List<UInt16>();
		ret.Clear();
		UInt16[] equipIDs = GetIDs();
//		DebugMgr.Log("equipIDs : " + equipIDs.Length);
		for(int i=0; i<equipIDs.Length; i++)
		{
			EquipmentInfo equipInfo;
			if(dicData.TryGetValue(equipIDs[i], out equipInfo))
			{
				if(!equipInfo.bCreate) continue;

				if(ret.Contains(equipInfo.u2ModelID)) continue; // 현재 선택된 모델정보는 리스트에 보여주지 않음
				try{
					if(Legion.Instance.acLookDesign.Get(equipInfo.u2ModelID-Server.ConstDef.BaseLookDesignID))
					{
						ret.Add(equipInfo.u2ModelID);
					}
				}catch(System.Exception ex){
//					DebugMgr.Log("-- Param --");
					//DebugMgr.Log("Class ID : "+ classID + " POS ID : " + posID);
//					DebugMgr.Log("-- Value --");
				}

			}
		}
		return ret;
	}
	public void Init()
	{
		dicData = new Dictionary<UInt16, EquipmentInfo>();
        dicScrollSetData = new Dictionary<UInt16, EquipmentScrollSetInfo>();
		DataMgr.Instance.LoadTable(this.SetAddStatusPerPointInfo, "StatusPoint");
		DataMgr.Instance.LoadTable(this.AddInfo, "Equipment");
		DataMgr.Instance.LoadTable(this.AddEquipmentSetInfo, "EquipSet");
	}

	public Status statusPerPoint;
	public void SetAddStatusPerPointInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		int idx=0;

        statusPerPoint.Set(1, Convert.ToUInt32(cols[idx++]));
		statusPerPoint.Set(2, Convert.ToUInt16(cols[idx++]));
		statusPerPoint.Set(3, Convert.ToUInt16(cols[idx++]));
		statusPerPoint.Set(4, Convert.ToUInt16(cols[idx++]));
		statusPerPoint.Set(5, Convert.ToUInt16(cols[idx++]));
		statusPerPoint.Set(6, Convert.ToUInt16(cols[idx++]));
		statusPerPoint.Set(7, Convert.ToUInt16(cols[idx++]));

		statPointPerLevel = Convert.ToInt32(cols[idx++]);
		skillPointPerLevel = Convert.ToInt32(cols[idx++]);
		skillMaxPoint = Convert.ToInt32(cols[idx++]);
		
		lvPerCharStatPoint = Convert.ToInt32(cols[idx++]);
		lvPerEquipStatPoint = Convert.ToInt32(cols[idx++]);
		lvPerSkillPoint = Convert.ToInt32(cols[idx++]);
		
		limitBuyCharStatPoint = Convert.ToInt32(cols[idx++]);
		cCharStatGoods = new Goods(cols, ref idx);
		BuyCharStatAddGoods = Convert.ToInt32(cols[idx++]);
		BuyCharStatMaxGoods = Convert.ToInt32(cols[idx++]);
		
		limitBuyEquipStatPoint = Convert.ToInt32(cols[idx++]);
		cEquipStatGoods = new Goods(cols, ref idx);
		BuyEquipStatAddGoods = Convert.ToInt32(cols[idx++]);
		BuyEquipStatMaxGoods = Convert.ToInt32(cols[idx++]);
		
		limitBuySkillPoint = Convert.ToInt32(cols[idx++]);
		cSkillPointGoods = new Goods(cols, ref idx);
		BuySkillPointAddGoods = Convert.ToInt32(cols[idx++]);
		BuySkillPointMaxGoods = Convert.ToInt32(cols[idx++]);

        u2BaseStatPointExp = Convert.ToUInt16(cols[idx++]);
        PlusStatPointExp = Convert.ToInt16(cols[idx++]);
        MinusStatPointExp = Convert.ToInt16(cols[idx++]);
        u2EquipHPStatPointMax = Convert.ToUInt16(cols[idx++]);
        u2EquipStrStatpointMax = Convert.ToUInt16(cols[idx++]);
        u2EquipIntStatPointMax = Convert.ToUInt16(cols[idx++]);
        u2EquipDefStatPointMax = Convert.ToUInt16(cols[idx++]);
        u2EquipResStatPointMax = Convert.ToUInt16(cols[idx++]);
        u2EquipAgiStatPointMax = Convert.ToUInt16(cols[idx++]);
        u2EquipCriStatPointMax = Convert.ToUInt16(cols[idx++]);
        u2EquipSkillPointMax = Convert.ToUInt16(cols[idx++]);
        u2EquipStatPointTotalMax = Convert.ToUInt16(cols[idx++]);

        fCompleteness = new Double[6];
        for(int i=0; i<6; i++)
            fCompleteness[i] = Convert.ToDouble(cols[idx++]);
	}

	public UInt32 GetAddStatusPerPoint(Byte statType)
	{
		UInt32 ret=0;
		switch(statType)
		{
			//case 1 : ret = statusPerPoint.u4HP; break;
			//case 2 : ret = (UInt32)statusPerPoint.u2Strength; break;
			//case 3 : ret = (UInt32)statusPerPoint.u2Intelligence; break;
			//case 4 : ret = (UInt32)statusPerPoint.u2Defence; break;
			//case 5 : ret = (UInt32)statusPerPoint.u2Resistance; break;
			//case 6 : ret = (UInt32)statusPerPoint.u2Agility; break;
			//case 7 : ret = (UInt32)statusPerPoint.u2Critical; break;
            case 1 : ret = (UInt32)statusPerPoint.GetStat(1); break;
			case 2 : ret = (UInt32)statusPerPoint.GetStat(2); break;
			case 3 : ret = (UInt32)statusPerPoint.GetStat(3); break;
			case 4 : ret = (UInt32)statusPerPoint.GetStat(4); break;
			case 5 : ret = (UInt32)statusPerPoint.GetStat(5); break;
			case 6 : ret = (UInt32)statusPerPoint.GetStat(6); break;
			case 7 : ret = (UInt32)statusPerPoint.GetStat(7); break;
		}
		return ret;
	}

	public UInt32 u2Power(Status status)
	{
		float ret = 0;
		//ret += status.u4HP * 0.04f;
		//ret += status.u2Strength * 2.1f;
		//ret += status.u2Intelligence * 1.8f;
		//ret += status.u2Defence * 1.8f;
		//ret += status.u2Resistance * 1.8f;
		//ret += status.u2Agility * 1.6f;
		//ret += status.u2Critical * 1.6f;
        ret += status.GetStat(1) * 0.04f;
		ret += status.GetStat(2) * 2.1f;
		ret += status.GetStat(3) * 1.8f;
		ret += status.GetStat(4) * 1.8f;
		ret += status.GetStat(5) * 1.8f;
		ret += status.GetStat(6) * 1.6f;
		ret += status.GetStat(7) * 1.6f;
		ret /= 7f;
		return (UInt32)ret;
	}


	public void AddEquipmentSetInfo(string[] cols)
	{
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		EquipmentScrollSetInfo info = new EquipmentScrollSetInfo();
		dicScrollSetData.Add(info.SetInfo(cols), info);
	}

	public EquipmentScrollSetInfo GetEquipmentSetInfo(UInt16 id)
	{
		EquipmentScrollSetInfo ret;
		dicScrollSetData.TryGetValue(id, out ret);
		return ret;
	}
}

