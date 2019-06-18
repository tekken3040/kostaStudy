using System;
using System.Collections.Generic;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public struct Status
{
    //public UInt32 u4HP;
    //public Int16 u2Strength;
    //public Int16 u2Intelligence;
    //public Int16 u2Defence;
    //public Int16 u2Resistance;
    //public Int16 u2Agility;
    //public Int16 u2Critical;
    //ObscuredUInt HP;
    //ObscuredInt Strength;
    //ObscuredInt Intelligence;
    //ObscuredInt Defence;
    //ObscuredInt Resistance;
    //ObscuredInt Agility;
    //ObscuredInt Critical;

    ObscuredUInt u4HP;
	ObscuredInt u2Strength;
	ObscuredInt u2Intelligence;
	ObscuredInt u2Defence;
	ObscuredInt u2Resistance;
	ObscuredInt u2Agility;
	ObscuredInt u2Critical;

	public UInt32 u4Power
	{
		get
		{
			float ret = 0;
			ret += GetStat(1) * 0.04f;
			ret += GetStat(2) * 2.1f;
			ret += GetStat(3) * 2.1f;
			ret += GetStat(4) * 1.8f;
			ret += GetStat(5) * 1.8f;
			ret += GetStat(6) * 1.6f;
			ret += GetStat(7) * 1.6f;
			ret /= 7f;
			return (UInt32)ret;
		} 
	}

    //난수 생성
    private void RandomizeObscuredVars()
	{
		u4HP.RandomizeCryptoKey();
		u2Strength.RandomizeCryptoKey();
		u2Intelligence.RandomizeCryptoKey();
		u2Defence.RandomizeCryptoKey();
        u2Resistance.RandomizeCryptoKey();
        u2Agility.RandomizeCryptoKey();
        u2Critical.RandomizeCryptoKey();
	}

	public void Clear()
	{
		u4HP = 0;
		u2Strength = 0;
		u2Intelligence = 0;
		u2Defence = 0;
		u2Resistance = 0;
		u2Agility = 0;
		u2Critical = 0;

        //HP = 0;
		//Strength = 0;
		//Intelligence = 0;
		//Defence = 0;
		//Resistance = 0;
		//Agility = 0;
		//Critical = 0;
	}
	public void Set(string[] data, ref UInt16 idx, bool bReadHP)
	{
		if (bReadHP) u4HP = Convert.ToUInt32(data[idx++]);
		else u4HP = 0;
		u2Strength = Convert.ToInt32(data[idx++]);
		u2Intelligence = Convert.ToInt32(data[idx++]);
		u2Defence = Convert.ToInt32(data[idx++]);
		u2Resistance = Convert.ToInt32(data[idx++]);
		u2Agility = Convert.ToInt32(data[idx++]);
		u2Critical = Convert.ToInt32(data[idx++]);
        //if (bReadHP) HP = Convert.ToUInt32(data[idx++]);
		//else HP = 0;
		//Strength = Convert.ToInt16(data[idx++]);
		//Intelligence = Convert.ToInt16(data[idx++]);
		//Defence = Convert.ToInt16(data[idx++]);
		//Resistance = Convert.ToInt16(data[idx++]);
		//Agility = Convert.ToInt16(data[idx++]);
		//Critical = Convert.ToInt16(data[idx++]);

        //u4HP = HP;
        //u2Strength = Convert.ToInt16(Strength);
        //u2Intelligence = Convert.ToInt16(Intelligence);
        //u2Defence = Convert.ToInt16(Defence);
        //u2Resistance = Convert.ToInt16(Resistance);
        //u2Agility = Convert.ToInt16(Agility);
        //u2Critical = Convert.ToInt16(Critical);
	}

	public void Set(Byte statusType, UInt32 value)
	{
		switch (statusType)
		{
		case 1 : 
			u4HP = value;
            //HP = value;
            //u4HP = HP;
			break;
		case 2 :
			u2Strength = (Int32)value;
            //Strength = (Int16)value;
            //u2Strength = Convert.ToInt16(Strength);
			break;
		case 3:
			u2Intelligence = (Int32)value;
            //Intelligence = (Int16)value;
            //u2Intelligence = Convert.ToInt16(Intelligence);
			break;
		case 4:
			u2Defence = (Int32)value;
            //Defence = (Int16)value;
            //u2Defence = Convert.ToInt16(Defence);
			break;
		case 5:
			u2Resistance = (Int32)value;
            //Resistance = (Int16)value;
            //u2Resistance = Convert.ToInt16(Resistance);
			break;
		case 6:
			u2Agility = (Int32)value;
            //Agility = (Int16)value;
            //u2Agility = Convert.ToInt16(Agility);
			break;
		case 7:
			u2Critical = (Int32)value;
            //Critical = (Int16)value;
            //u2Critical = Convert.ToInt16(Critical);
			break;
		}
	}

	public void Add(Status added)
	{
		u4HP += added.u4HP;
		u2Strength += added.u2Strength;
		u2Intelligence += added.u2Intelligence;
		u2Defence += added.u2Defence;
		u2Resistance += added.u2Resistance;
		u2Agility += added.u2Agility;
		u2Critical += added.u2Critical;

        //HP += added.HP;
		//Strength += added.Strength;
		//Intelligence += added.Intelligence;
		//Defence += added.Defence;
		//Resistance += added.Resistance;
		//Agility += added.Agility;
		//Critical += added.Critical;
        //
        //u4HP = HP;
        //u2Strength = Convert.ToInt16(Strength);
        //u2Intelligence = Convert.ToInt16(Intelligence);
        //u2Defence = Convert.ToInt16(Defence);
        //u2Resistance = Convert.ToInt16(Resistance);
        //u2Agility = Convert.ToInt16(Agility);
        //u2Critical = Convert.ToInt16(Critical);
	}

	public void Add(UInt32[] points)
	{
		u4HP += (UInt32)(points[0] * EquipmentInfoMgr.Instance.statusPerPoint.u4HP);
		u2Strength += (Int32)(points[1] * EquipmentInfoMgr.Instance.statusPerPoint.u2Strength);
		u2Intelligence += (Int32)(points[2] * EquipmentInfoMgr.Instance.statusPerPoint.u2Intelligence);
		u2Defence += (Int32)(points[3] * EquipmentInfoMgr.Instance.statusPerPoint.u2Defence);
		u2Resistance += (Int32)(points[4] * EquipmentInfoMgr.Instance.statusPerPoint.u2Resistance);
		u2Agility += (Int32)(points[5] * EquipmentInfoMgr.Instance.statusPerPoint.u2Agility);
		u2Critical += (Int32)(points[6] * EquipmentInfoMgr.Instance.statusPerPoint.u2Critical);	
        //HP += (UInt32)(points[0] * EquipmentInfoMgr.Instance.statusPerPoint.u4HP);
        //Strength += (Int16)(points[1] * EquipmentInfoMgr.Instance.statusPerPoint.u2Strength);
        //Intelligence += (Int16)(points[2] * EquipmentInfoMgr.Instance.statusPerPoint.u2Intelligence);
        //Defence += (Int16)(points[3] * EquipmentInfoMgr.Instance.statusPerPoint.u2Defence);
        //Resistance += (Int16)(points[4] * EquipmentInfoMgr.Instance.statusPerPoint.u2Resistance);
        //Agility += (Int16)(points[5] * EquipmentInfoMgr.Instance.statusPerPoint.u2Agility);
        //Critical += (Int16)(points[6] * EquipmentInfoMgr.Instance.statusPerPoint.u2Critical);	
        //
        //u4HP = HP;
        //u2Strength = Convert.ToInt16(Strength);
        //u2Intelligence = Convert.ToInt16(Intelligence);
        //u2Defence = Convert.ToInt16(Defence);
        //u2Resistance = Convert.ToInt16(Resistance);
        //u2Agility = Convert.ToInt16(Agility);
        //u2Critical = Convert.ToInt16(Critical);
	}
	
	public void Add(Byte statusType, UInt32 value)
	{
		switch (statusType)
		{
		case 1 : 
			u4HP += (UInt32)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
            //HP += (UInt32)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
			break;
		case 2 :
			u2Strength += (Int32)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
            //Strength += (Int16)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
			break;
		case 3:
			u2Intelligence += (Int32)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
            //Intelligence += (Int16)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
			break;
		case 4:
			u2Defence += (Int32)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
            //Defence += (Int16)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
			break;
		case 5:
			u2Resistance += (Int32)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
            //Resistance += (Int16)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
			break;
		case 6:
			u2Agility += (Int32)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
            //Agility += (Int16)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
			break;
		case 7:
			u2Critical += (Int32)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
            //Critical += (Int16)(value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType)));
			break;
		}
	}

	public void Sub(Status subed)
	{
		u4HP -= subed.u4HP;
		u2Strength -= subed.u2Strength;
		u2Intelligence -= subed.u2Intelligence;
		u2Defence -= subed.u2Defence;
		u2Resistance -= subed.u2Resistance;
		u2Agility -= subed.u2Agility;
		u2Critical -= subed.u2Critical;
        //HP -= subed.HP;
		//Strength -= subed.Strength;
		//Intelligence -= subed.Intelligence;
		//Defence -= subed.Defence;
		//Resistance -= subed.Resistance;
		//Agility -= subed.Agility;
		//Critical -= subed.Critical;
        //
        //u4HP = HP;
        //u2Strength = Convert.ToInt16(Strength);
        //u2Intelligence = Convert.ToInt16(Intelligence);
        //u2Defence = Convert.ToInt16(Defence);
        //u2Resistance = Convert.ToInt16(Resistance);
        //u2Agility = Convert.ToInt16(Agility);
        //u2Critical = Convert.ToInt16(Critical);
	}
	public void Add(Status added, Double multiple)
	{
		u4HP += (UInt32)((Double)added.u4HP * multiple);
		u2Strength += (Int32)((Double)added.u2Strength * multiple);
		u2Intelligence += (Int32)((Double)added.u2Intelligence * multiple);
		u2Defence += (Int32)((Double)added.u2Defence * multiple);
		u2Resistance += (Int32)((Double)added.u2Resistance * multiple);
		u2Agility += (Int32)((Double)added.u2Agility * multiple);
		u2Critical += (Int32)((Double)added.u2Critical * multiple);
        //HP += (UInt32)((Double)added.u4HP * multiple);
		//Strength += (Int16)((Double)added.u2Strength * multiple);
		//Intelligence += (Int16)((Double)added.u2Intelligence * multiple);
		//Defence += (Int16)((Double)added.u2Defence * multiple);
		//Resistance += (Int16)((Double)added.u2Resistance * multiple);
		//Agility += (Int16)((Double)added.u2Agility * multiple);
		//Critical += (Int16)((Double)added.u2Critical * multiple);
        //
        //u4HP = HP;
        //u2Strength = Convert.ToInt16(Strength);
        //u2Intelligence = Convert.ToInt16(Intelligence);
        //u2Defence = Convert.ToInt16(Defence);
        //u2Resistance = Convert.ToInt16(Resistance);
        //u2Agility = Convert.ToInt16(Agility);
        //u2Critical = Convert.ToInt16(Critical);
	}
	public Status AddPercent(Status percent)
	{
		Status added = new Status();
		added.u4HP = (UInt32)((UInt32)u4HP * (UInt32)percent.u4HP / (UInt32)100);
		added.u2Strength = (Int32)(u2Strength * (Int32)percent.u2Strength / (UInt32)100);
		added.u2Intelligence = (Int32)(u2Intelligence * (Int32)percent.u2Intelligence / (UInt32)100);
		added.u2Defence = (Int32)(u2Defence * (Int32)percent.u2Defence / (UInt32)100);
		added.u2Resistance = (Int32)(u2Resistance * (Int32)percent.u2Resistance / (UInt32)100);
		added.u2Agility = (Int32)(u2Agility * (Int32)percent.u2Agility / (UInt32)100);
		added.u2Critical = (Int32)(u2Critical * (Int32)percent.u2Critical / (UInt32)100);
        //added.HP = (UInt32)((UInt32)HP * (UInt32)percent.HP / (UInt32)100);
		//added.Strength = (Int16)(Strength * (Int32)percent.Strength / (UInt32)100);
		//added.Intelligence = (Int16)(Intelligence * (Int32)percent.Intelligence / (UInt32)100);
		//added.Defence = (Int16)(Defence * (Int32)percent.Defence / (UInt32)100);
		//added.Resistance = (Int16)(Resistance * (Int32)percent.Resistance / (UInt32)100);
		//added.Agility = (Int16)(Agility * (Int32)percent.Agility / (UInt32)100);
		//added.Critical = (Int16)(Critical * (Int32)percent.Critical / (UInt32)100);
		Add(added);

		return added;
	}

	public Status AddPercent(Status percent, Double multiple)
	{
		Status added = new Status();
		added.u4HP = (UInt32)((UInt32)u4HP * (Double)percent.u4HP * multiple / (UInt32)100);
		added.u2Strength = (Int32)(u2Strength * (Double)percent.u2Strength * multiple / (UInt32)100);
		added.u2Intelligence = (Int32)(u2Intelligence * (Double)percent.u2Intelligence * multiple / (UInt32)100);
		added.u2Defence = (Int32)(u2Defence * (Double)percent.u2Defence * multiple / (UInt32)100);
		added.u2Resistance = (Int32)(u2Resistance * (Double)percent.u2Resistance * multiple / (UInt32)100);
		added.u2Agility = (Int32)(u2Agility * (Double)percent.u2Agility * multiple / (UInt32)100);
		added.u2Critical = (Int32)(u2Critical * (Double)percent.u2Critical * multiple / (UInt32)100);
		Add(added);

		return added;
	}

    public Status AddPercentDetail(Status percent)
	{
		Status added = new Status();
		added.u4HP = (UInt32)((UInt32)u4HP * (UInt32)percent.u4HP / (UInt32)100);
		added.u2Strength = (Int32)(u2Strength * (Int32)percent.u2Strength / (UInt32)100);
		added.u2Intelligence = (Int32)(u2Intelligence * (Int32)percent.u2Intelligence / (UInt32)100);
		added.u2Defence = (Int32)(u2Defence * (Int32)percent.u2Defence / (UInt32)100);
		added.u2Resistance = (Int32)(u2Resistance * (Int32)percent.u2Resistance / (UInt32)100);
		added.u2Agility = (Int32)(u2Agility * (Int32)percent.u2Agility / (UInt32)100);
		added.u2Critical = (Int32)(u2Critical * (Int32)percent.u2Critical / (UInt32)100);

		return added;
	}

    public Status AddPercentDetail(Status percent, Double multiple)
	{
		Status added = new Status();
		added.u4HP = (UInt32)((UInt32)u4HP * (Double)percent.u4HP * multiple / (UInt32)100);
		added.u2Strength = (Int32)(u2Strength * (Double)percent.u2Strength * multiple / (UInt32)100);
		added.u2Intelligence = (Int32)(u2Intelligence * (Double)percent.u2Intelligence * multiple / (UInt32)100);
		added.u2Defence = (Int32)(u2Defence * (Double)percent.u2Defence * multiple / (UInt32)100);
		added.u2Resistance = (Int32)(u2Resistance * (Double)percent.u2Resistance * multiple / (UInt32)100);
		added.u2Agility = (Int32)(u2Agility * (Double)percent.u2Agility * multiple / (UInt32)100);
		added.u2Critical = (Int32)(u2Critical * (Double)percent.u2Critical * multiple / (UInt32)100);

		return added;
	}

	public void Copy(Status origin)
	{
		u4HP = origin.u4HP;
		u2Strength = origin.u2Strength;
		u2Intelligence = origin.u2Intelligence;
		u2Defence = origin.u2Defence;
		u2Resistance = origin.u2Resistance;
		u2Agility = origin.u2Agility;
		u2Critical = origin.u2Critical;
        //HP = origin.HP;
		//Strength = origin.Strength;
		//Intelligence = origin.Intelligence;
		//Defence = origin.Defence;
		//Resistance = origin.Resistance;
		//Agility = origin.Agility;
		//Critical = origin.Critical;
        //
        //u4HP = HP;
        //u2Strength = Convert.ToInt16(Strength);
        //u2Intelligence = Convert.ToInt16(Intelligence);
        //u2Defence = Convert.ToInt16(Defence);
        //u2Resistance = Convert.ToInt16(Resistance);
        //u2Agility = Convert.ToInt16(Agility);
        //u2Critical = Convert.ToInt16(Critical);
	}

	public Int32 GetStat(Byte statusType, UInt32 value)
	{
        RandomizeObscuredVars();
		switch (statusType)
		{
		case 1 : 
			return (Int32)(u4HP + (value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType))));
		case 2 :
			return (Int32)(u2Strength + (value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType))));
		case 3:
			return (Int32)(u2Intelligence + (value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType))));
		case 4:
			return (Int32)(u2Defence + (value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType))));
		case 5:
			return (Int32)(u2Resistance + (value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType))));
		case 6:
			return (Int32)(u2Agility + (value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType))));
		case 7:
			return (Int32)(u2Critical + (value*(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(statusType))));
		}

		return 0;
	}

	public Int32 GetFirstStat()
    {
        RandomizeObscuredVars();
		//if(u4HP > 0) return (Int32)u4HP;
        if(u4HP > 0) return Convert.ToInt32(u4HP);
		if(u2Strength > 0) return u2Strength;
		if(u2Intelligence > 0) return u2Intelligence;
		if(u2Defence > 0) return u2Defence;
		if(u2Resistance > 0) return u2Resistance;
		if(u2Agility > 0) return u2Agility;
		if(u2Critical > 0) return u2Critical;

		return 0;
	}

	public Int32 GetStat(Byte statusType)
	{
        RandomizeObscuredVars();
		switch (statusType)
		{
		case 1 : 
			return Convert.ToInt32(u4HP);
		case 2 :
			return (Int32)u2Strength;
		case 3:
			return (Int32)u2Intelligence;
		case 4:
			return (Int32)u2Defence;
		case 5:
			return (Int32)u2Resistance;
		case 6:
			return (Int32)u2Agility;
		case 7:
			return (Int32)u2Critical;
		}

		return 0;
	}

	public static string GetStatText(Byte statusType)
	{
		switch (statusType)
		{
		case 1 : 
			return TextManager.Instance.GetText("stat_hp");
		case 2 :
			return TextManager.Instance.GetText("stat_str");
		case 3:
			return TextManager.Instance.GetText("stat_int");
		case 4:
			return TextManager.Instance.GetText("stat_def");
		case 5:
			return TextManager.Instance.GetText("stat_res");
		case 6:
			return TextManager.Instance.GetText("stat_agi");
		case 7:
			return TextManager.Instance.GetText("stat_cri");
		}
        
		return "";
	}

	public static string GetStatDescription(Byte statusType)
	{
		switch (statusType)
		{
		case 1 : 
			return TextManager.Instance.GetText("stat_desc_hp");
		case 2 :
			return TextManager.Instance.GetText("stat_desc_str");
		case 3:
			return TextManager.Instance.GetText("stat_desc_int");
		case 4:
			return TextManager.Instance.GetText("stat_desc_def");
		case 5:
			return TextManager.Instance.GetText("stat_desc_res");
		case 6:
			return TextManager.Instance.GetText("stat_desc_agi");
		case 7:
			return TextManager.Instance.GetText("stat_desc_cri");
		}

		return "";
	}
}

public class StatusInfo
{
	public Status cBasic;
	public Status cLevelUp;
}

public class StatusComponent : ZComponent
{
    private Status cStatus;
    public Status STATUS
    {
        get { return cStatus; }
        set { cStatus = value; }
    }
    private Status cFinalStatus;
    public Status FINAL_STATUS
    {
        get { return cFinalStatus; }
        set { cFinalStatus = value; }
    }
	public StatusInfo cInfo;

	public Status EquipBase;
	public UInt32[] points;

	public Byte[] au1StatType;
    private Byte StatPoint;
    public Byte STAT_POINT
    {
        get{ return StatPoint; }
        set{ StatPoint = value; }
    }
    public Byte ResetCount;
	public Byte BuyPoint;
    private UInt16 VIPStatPoint;
    public UInt16 VIP_STATPOINT
    {
        get{ return VIPStatPoint; }
        set{ VIPStatPoint = value; }
    }
    private Byte UsePoint;
    public Byte USE_POINT
    {
        get{ return UsePoint; }
        set{ UsePoint = value; }
    }
    private UInt16 u2UnsetStatPoint;
    public UInt16 UNSET_STATPOINT
    {
        get{ return u2UnsetStatPoint; }
        set{ u2UnsetStatPoint = value;}
    }
    private UInt16 u2StatPointExp;
    public UInt16 STATPOINT_EXP
    {
        get{ return u2StatPointExp; }
        set{ u2StatPointExp = value;}
    }
    public const UInt16 MAX_STATEXP = 1000;
    public const Byte MAX_STATEXP_PROGRESS = 10;

    public void ResetStatus()
    {
        for (int i = 0; i < points.Length; i++) points[i] = 0;
        ResetCount++;
    }
    public void DoPointingStatus(UInt16[] addedPoints)
    {
        if (au1StatType != null)
        {
    		points[Server.ConstDef.SkillOfEquip] += addedPoints[Server.ConstDef.SkillOfEquip];
    		points[Server.ConstDef.SkillOfEquip + 1] += addedPoints[Server.ConstDef.SkillOfEquip + 1];
        }
        else
        {
            for (int i = 0; i < points.Length; i++){
				if(i < addedPoints.Length) points[i] += addedPoints[i];
			}
        }
    }
    public void UndoPointingStatus()
    {
    }

	public override void init(object param)
	{
		cInfo = (StatusInfo)param;
	}
	public void SetByLevel(Level level)
	{
		cStatus.Clear();
		cFinalStatus.Clear();

		cStatus.Add(cInfo.cBasic);
		//cStatus.Add(EquipBase);
		if (points != null) cStatus.Add(points);

		cStatus.Add(cInfo.cLevelUp, (level.u2Level-1));

		cFinalStatus = cStatus;
        cFinalStatus.Add(EquipBase);
	}
	public void SetByLevelEquip(Level level)
	{
        
		EquipmentItem convertOwner = (EquipmentItem)owner;
		Byte smithingLevel = convertOwner.u1SmithingLevel;
        //convertOwner.statusComponent.CountingStatPointEquip();
		float multiplePerLevelUp = 1;
		if(smithingLevel != 0)
			multiplePerLevelUp = ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[smithingLevel-1]).u4StatMultiplePerLevelUp;

        u2StatPointExp = convertOwner.statusComponent.u2StatPointExp;
        u2UnsetStatPoint = convertOwner.statusComponent.u2UnsetStatPoint;
        
		cStatus.Clear();
		cFinalStatus.Clear();

//		cStatus.Add(cInfo.cBasic);
		cStatus.Add(EquipBase);
		cStatus.Add(cInfo.cLevelUp, (level.u2Level-1) * multiplePerLevelUp * EquipmentInfoMgr.Instance.fCompleteness[convertOwner.u1Completeness-1]);

        VIPStatPoint += (UInt16)(LegionInfoMgr.Instance.GetCurrentVIPInfo().u1LvUpAddEquipPt*multiplePerLevelUp);
		cFinalStatus = cStatus;
		cFinalStatus.Add(au1StatType[0], points[Server.ConstDef.SkillOfEquip]);
		cFinalStatus.Add(au1StatType[1], points[Server.ConstDef.SkillOfEquip+1]);
		cFinalStatus.Add(au1StatType[2], points[Server.ConstDef.SkillOfEquip+2]);
	}
    public void LoadStatus(UInt32[] au2Stats, Byte u1ResetCount)
    {
        ResetCount = u1ResetCount;
        //points = new UInt16[Server.ConstDef.CharStatPointType];
		points = au2Stats;
    }

	public void LoadStatusEquipment(UInt32[] au2Stats, EquipmentInfo.StatAddInfo[] acStatType, Byte u1ResetCount) 
	{
		au1StatType = new byte[3];
		au1StatType[0] = acStatType[0].u1StatType;
		au1StatType[1] = acStatType[1].u1StatType;
		au1StatType[2] = acStatType[2].u1StatType;
		
		// au2Stats 구조 : [스킬부여포인트3개, 초기스탯(랜덤)3개(3가지 스테이터스 고정), 스탯부여포인트 3개] 
		ResetCount = u1ResetCount;
		points = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType];
		
		EquipBase.Set(au1StatType[0], au2Stats[Server.ConstDef.SkillOfEquip]);
		EquipBase.Set(au1StatType[1], au2Stats[Server.ConstDef.SkillOfEquip+1]);
		EquipBase.Set(au1StatType[2], au2Stats[Server.ConstDef.SkillOfEquip+2]);

		// 스킬 포인트 
		points[0] = au2Stats[0];
		points[1] = au2Stats[1];
		points[2] = au2Stats[2];

		// 스탯 포인트        
		points[3] = au2Stats[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType];
		points[4] = au2Stats[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType + 1];
		points[5] = au2Stats[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType + 2];
	}

	public void LoadStatus(UInt32[] au2Stats, Byte u1UseStatus1, Byte u1UseStatus2, Byte u1UseStatus3, Byte u1ResetCount)
    {
		au1StatType = new byte[3];
		au1StatType[0] = u1UseStatus1;
		au1StatType[1] = u1UseStatus2;
		au1StatType[2] = u1UseStatus3;

		// au2Stats 구조 : [스킬부여포인트3개, 초기스탯(랜덤)3개(3가지 스테이터스 고정), 스탯부여포인트 3개] 
        ResetCount = u1ResetCount;
		
		points = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType];

        // 기본 스탯
		EquipBase.Set(u1UseStatus1, au2Stats[Server.ConstDef.SkillOfEquip]);	
		EquipBase.Set(u1UseStatus2, au2Stats[Server.ConstDef.SkillOfEquip + 1]);
        EquipBase.Set(u1UseStatus3, au2Stats[Server.ConstDef.SkillOfEquip + 2]);
        
        // 스킬 포인트 
        points[0] = au2Stats[0];
		points[1] = au2Stats[1];
        points[2] = au2Stats[2];
		
	    // 스탯 포인트        
        points[3] = au2Stats[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType];
        points[4] = au2Stats[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType + 1];
        points[5] = au2Stats[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType + 2];
	}

	public void CountingStatPoint(UInt16 u2Level)
	{		
		Byte useStatPoint = 0;
		
		for(int i=0; i<points.Length; i++)
		{
			useStatPoint += (Byte)points[i];
		}

		int calStat = ((u2Level - 1)*EquipmentInfoMgr.Instance.lvPerCharStatPoint + BuyPoint + VIPStatPoint) - useStatPoint;

		if(calStat < 0)
			calStat = 0;

		StatPoint = (Byte)calStat;
        UsePoint = useStatPoint;
		DebugMgr.Log("Stat : " + StatPoint);
	}

    public Byte CountingStatPoint()
    {       
        UInt16 u2Level = ((Hero)owner).cLevel.u2Level;
        Byte useStatPoint = 0;

        for(int i=0; i<points.Length; i++)
        {
            useStatPoint += (Byte)points[i];
        }

        int calStat = ((u2Level - 1)*EquipmentInfoMgr.Instance.lvPerCharStatPoint + BuyPoint + VIPStatPoint) - useStatPoint;

        if(calStat < 0)
            calStat = 0;

        StatPoint = (Byte)calStat;
        UsePoint = useStatPoint;
        DebugMgr.Log("Stat : " + StatPoint);
        return StatPoint;
    }

    
    public void CountingStatPointEquip(UInt16 u2Level)
    {
		Byte useStatPoint = 0;
		
		for(int i=0; i<points.Length; i++)
		{
            //if(i < Server.ConstDef.SkillOfEquip)
            if(i < points.Length)
            {      
                //useStatPoint += (Byte)(points[i] * EquipmentInfoMgr.Instance.skillPointPerLevel);
                if(points[i] > 0)
                    useStatPoint += (Byte)(points[i] - ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[((EquipmentItem)owner).u1SmithingLevel-1]).cSmithingInfo.u1SkillInitLevel);
            }
            else       
		      //useStatPoint += (Byte)(points[i] * EquipmentInfoMgr.Instance.statPointPerLevel);
              useStatPoint += (Byte)(points[i]);
		}

		//int calStat = (BuyPoint + VIPStatPoint + u2UnsetStatPoint) - useStatPoint;
        int calStat = u2UnsetStatPoint - useStatPoint;
		if(calStat < 0)
			calStat = 0;

		StatPoint = (Byte)calStat;
        UsePoint = useStatPoint;
		//DebugMgr.Log("Stat : " + StatPoint);        
    }

    public Byte CountingStatPointEquip()
    {
        UInt16 u2Level = ((EquipmentItem)owner).cLevel.u2Level;

        Byte useStatPoint = 0;

        for(int i=0; i<points.Length; i++)
        {
            if(i < Server.ConstDef.SkillOfEquip)
            {            
                //useStatPoint += (Byte)(points[i] * EquipmentInfoMgr.Instance.skillPointPerLevel);
                if(points[i] > 0)
                    useStatPoint += (Byte)(points[i] - ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[((EquipmentItem)owner).u1SmithingLevel-1]).cSmithingInfo.u1SkillInitLevel);
            }
            else       
                //useStatPoint += (Byte)(points[i] * EquipmentInfoMgr.Instance.statPointPerLevel);
                useStatPoint += (Byte)(points[i]);
        }

        //int calStat = (BuyPoint + VIPStatPoint + u2UnsetStatPoint)/* - useStatPoint*/;
        int calStat = u2UnsetStatPoint + useStatPoint;

        if(calStat < 0)
            calStat = 0;

        StatPoint = (Byte)calStat;
        //u2UnsetStatPoint = StatPoint;
        UsePoint = useStatPoint;
        return StatPoint;
        //DebugMgr.Log("Stat : " + StatPoint);        
    }

	public void LevelUp(UInt16 u2LevelUp)
	{
        //TODO::SIM
        //좀더 확인이 필요하겠다 레벨업 이외에 레벨을 올려서 값을 주 받는 케이스가 있을수도 있다.
        VIPStatPoint += (UInt16)(LegionInfoMgr.Instance.GetCurrentVIPInfo().u1LvUpAddStatusPt*u2LevelUp);
		cFinalStatus.Add(cInfo.cLevelUp, u2LevelUp);
	}
	public void Wear(Status stat)
	{
		cFinalStatus.Add(stat);
	}
	public void Change(Status fromStat, Status toStat)
	{
		cFinalStatus.Sub(fromStat);
		cFinalStatus.Add(toStat);
	}
    public void addBuyStatPoint(Byte _buyPoint)
    {
        BuyPoint = (Byte)(BuyPoint + _buyPoint);
        StatPoint = (Byte)(StatPoint + VIPStatPoint + BuyPoint);
    }
    public void setBuyStatPoint(Byte _buyPoint)
    {
        BuyPoint = _buyPoint;
        StatPoint = (Byte)(StatPoint + VIPStatPoint + BuyPoint);
    }
	public bool CheckHaveStatPoint(UInt16 u2Level)
    {
        bool bHaveStatPoint = false;
        CountingStatPoint(u2Level);

        if(StatPoint > 0)
            bHaveStatPoint = true;
        else
            bHaveStatPoint = false;

        return bHaveStatPoint;
    }
	public bool CheckHaveEquipStatPoint(UInt16 u2Level)
    {
        bool bHaveStatPoint = false;
        CountingStatPointEquip(u2Level);

        if(StatPoint > 0)
            bHaveStatPoint = true;
        else
            bHaveStatPoint = false;

        return bHaveStatPoint;
    }

    public UInt16 StatPointExpUp(UInt16 _statExp, UInt16 u2Level)
    {
        u2StatPointExp += _statExp;
        if(u2StatPointExp >= MAX_STATEXP)
        {
            Byte useStatPoint = 0;
            for(int i=0; i<points.Length; i++)
            {
                if(i < Server.ConstDef.SkillOfEquip)
                {            
                    //useStatPoint += (Byte)(points[i] * EquipmentInfoMgr.Instance.skillPointPerLevel);
                    if(points[i] > 0)
                        useStatPoint += (Byte)(points[i] - ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[((EquipmentItem)owner).u1SmithingLevel-1]).cSmithingInfo.u1SkillInitLevel);
                }
                else       
                    useStatPoint += (Byte)(points[i]);
            }

            int calStat = ((u2Level - 1)*EquipmentInfoMgr.Instance.lvPerEquipStatPoint + BuyPoint + VIPStatPoint + u2UnsetStatPoint + (u2StatPointExp/MAX_STATEXP)) + useStatPoint;

            if(EquipmentInfoMgr.Instance.u2EquipStatPointTotalMax >= calStat)
                u2UnsetStatPoint += (UInt16)(u2StatPointExp/MAX_STATEXP);

            else
                u2UnsetStatPoint += (UInt16)(EquipmentInfoMgr.Instance.u2EquipStatPointTotalMax + ((u2Level - 1) 
                    * EquipmentInfoMgr.Instance.lvPerEquipStatPoint + BuyPoint + VIPStatPoint + u2UnsetStatPoint) + useStatPoint);
            u2StatPointExp = (UInt16)(u2StatPointExp%MAX_STATEXP);
        }
        else
        {
        }
        CountingStatPointEquip();
        return u2StatPointExp;
    }
}