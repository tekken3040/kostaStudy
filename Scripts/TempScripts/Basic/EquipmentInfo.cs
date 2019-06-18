using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public class EquipmentInfo : ItemInfo
{
	//public GameObject objModel;

	public UInt16 u2ClassID;
	public ModelInfo cModel;
	public UInt16 u2ModelID;

	public enum EQUIPMENT_POS
	{
		NONE = 0,
		HELMET = 1,
		CHEST = 2,
		SHOULDER = 3,
		GLOVE = 4,
		PANTS = 5,
		SHOES = 6,
		WEAPON_R = 7,
		WEAPON_L = 8,
		ACCESSORY_1 = 9,
		ACCESSORY_2 = 10,
		END = 11,
	}

	public EQUIPMENT_POS u1PosID;
	public Byte u1Element;

	public struct StatAddInfo
	{
		public Byte u1StatType;
		public UInt16 u2BaseStatMax;
		public UInt16 u2BaseStatMin;
		public UInt16 u2AddStatMaxForgeLevel;
		public UInt16 u2AddStatMinForgeLevel;
		public UInt16 u2LevelUpStat;
		public void Set(string[] cols, ref int idx)
		{
			u1StatType = Convert.ToByte(cols[idx++]);
			u2BaseStatMax = Convert.ToUInt16(cols[idx++]);
			u2BaseStatMin = Convert.ToUInt16(cols[idx++]);
			u2AddStatMaxForgeLevel = Convert.ToUInt16(cols[idx++]);
			u2AddStatMinForgeLevel = Convert.ToUInt16(cols[idx++]);
			u2LevelUpStat = Convert.ToUInt16(cols[idx++]);
		}
	}
	public const Byte ADD_STAT_TYPE_MAX = 3;
	public StatAddInfo[] acStatAddInfo;
	public const Byte ADD_SKILL_MAX = 3;
	public Goods cBuyGoods;
	public Goods cSellGoods;

	public bool bRemoveHair;
	public bool bCreate;

//	public Goods cCreateGoods;

//	public const Byte EQUIP_MODEL_MAX = 15;
//
//	public UInt16[] au2EquipModelID;
//
	public StatusInfo cStatus;


//    public string imagePath;
//
//
//	public struct SkillUpgrade
//	{
//		public UInt16 u2ID;
//		public Byte u1BasePoint;
//		public void Set(string[] cols, ref UInt16 idx)
//		{
//			u2ID = Convert.ToUInt16(cols[idx++]);
//			u1BasePoint = Convert.ToByte(cols[idx++]);
//		}
//	}
//	public const Byte MAX_SLOT_SKILL_UPGRADE = 5;
//	public SkillUpgrade[] cSkillUpgrade;
//
	public struct SmithingMaterial
	{
		public Goods cCreateGoods;
		public Goods[] acMaterial;
		public const int MAX_MATERIAL = 4;
		public void Set(string[] cols, ref int idx)
		{
			cCreateGoods = new Goods(cols, ref idx);

			acMaterial = new Goods[MAX_MATERIAL];
			for(int i=0; i<MAX_MATERIAL; i++)
			{
				acMaterial[i] = new Goods(cols, ref idx);
			}
		}
	}

    public Byte u1Specialize;
	public Dictionary<Byte, SmithingMaterial> dicSmithingMaterial;

	public EquipmentInfo()
	{
	}

	public EquipmentInfo(UInt16 id, string name)
		: base(id, name)
	{
	}

	public UInt16 SetInfo(string[] cols)
	{
		int idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment
		sName = cols[idx++];
		sDescription = cols[idx++];
		u2ClassID = Convert.ToUInt16(cols[idx++]);
		u2ModelID = Convert.ToUInt16(cols[idx++]);
		if (u2ModelID != 0) cModel = ModelInfoMgr.Instance.GetInfo(u2ModelID);
		u1PosID = (EQUIPMENT_POS)Convert.ToByte(cols[idx++]);
		switch((Byte)u1PosID)
		{
		case 1:
            base.eOrder = ITEM_ORDER.HELMET; break;			
		case 2:
			base.eOrder = ITEM_ORDER.CHEST; break;
		case 3:
            base.eOrder = ITEM_ORDER.SHOULDER; break;			
		case 4:
			base.eOrder = ITEM_ORDER.GLOVE; break;
		case 5:
			base.eOrder = ITEM_ORDER.PANTS; break;
		case 6:
            base.eOrder = ITEM_ORDER.SHOES; break;			
		case 7:
			base.eOrder = ITEM_ORDER.WEAPON_R; break;
		case 8:
			base.eOrder = ITEM_ORDER.WEAPON_L; break;
		case 9:			
            base.eOrder = ITEM_ORDER.ACCESSORY_1; break;
		case 10:			
            base.eOrder = ITEM_ORDER.ACCESSORY_2; break;
		}

		if(u2ModelID <= Server.ConstDef.BaseLookDesignID + Server.ConstDef.SizeOfLookDesignBuffer * sizeof(byte) * 8)
		{
			ModelInfoMgr.Instance.SetData(u2ModelID, u2ClassID, u1PosID);
		}
		u1Element = Convert.ToByte(cols[idx++]);

		cStatus = new StatusInfo();
		acStatAddInfo = new StatAddInfo[ADD_STAT_TYPE_MAX];
		for(int i=0; i<ADD_STAT_TYPE_MAX; i++)
		{
			acStatAddInfo[i].Set(cols,ref idx);
			switch(acStatAddInfo[i].u1StatType)
			{
			//case 1 : cStatus.cLevelUp.u4HP = acStatAddInfo[i].u2LevelUpStat;	break;
			//case 2 : cStatus.cLevelUp.u2Strength = (Int16)acStatAddInfo[i].u2LevelUpStat;	break;
			//case 3 : cStatus.cLevelUp.u2Intelligence = (Int16)acStatAddInfo[i].u2LevelUpStat;	break;
			//case 4 : cStatus.cLevelUp.u2Defence = (Int16)acStatAddInfo[i].u2LevelUpStat;	break;
			//case 5 : cStatus.cLevelUp.u2Resistance = (Int16)acStatAddInfo[i].u2LevelUpStat;	break;
			//case 6 : cStatus.cLevelUp.u2Agility = (Int16)acStatAddInfo[i].u2LevelUpStat;	break;
			//case 7 : cStatus.cLevelUp.u2Critical = (Int16)acStatAddInfo[i].u2LevelUpStat;	break;
            case 1 : cStatus.cLevelUp.Set(1, acStatAddInfo[i].u2LevelUpStat);	break;
			case 2 : cStatus.cLevelUp.Set(2, acStatAddInfo[i].u2LevelUpStat);	break;
			case 3 : cStatus.cLevelUp.Set(3, acStatAddInfo[i].u2LevelUpStat);	break;
			case 4 : cStatus.cLevelUp.Set(4, acStatAddInfo[i].u2LevelUpStat);	break;
			case 5 : cStatus.cLevelUp.Set(5, acStatAddInfo[i].u2LevelUpStat);	break;
			case 6 : cStatus.cLevelUp.Set(6, acStatAddInfo[i].u2LevelUpStat);	break;
			case 7 : cStatus.cLevelUp.Set(7, acStatAddInfo[i].u2LevelUpStat);	break;
			}
		}

		cBuyGoods = new Goods (cols, ref idx);
		cSellGoods = new Goods (cols, ref idx);

		if(Convert.ToByte(cols[idx++]) == 1)
			bRemoveHair = true;
		else
			bRemoveHair = false;

		if(cols[idx++] == "T")
			bCreate = true;
		else
			bCreate = false;

//		cCreateGoods = new Goods (cols, ref idx);
		      
        idx += 15;
        //idx++;
        //idx++;
        //idx++;
        //idx++;
        //idx++;
        //idx++;

		dicSmithingMaterial = new Dictionary<byte, SmithingMaterial>();
		for(Byte i=0; i<Server.ConstDef.MaxForgeLevel; i++)
		{
			Byte level = (Byte)(i+1);
			SmithingMaterial material = new SmithingMaterial();
			material.Set(cols, ref idx);
			dicSmithingMaterial.Add(level, material);
		}
        u1Specialize = Convert.ToByte(cols[idx++]);
		return u2ID;
	}

	public string EquipTypeKey()
	{
		switch(u1PosID)
		{
		case EQUIPMENT_POS.HELMET:
			return "equip_helmet";
		case EQUIPMENT_POS.CHEST:
			return "equip_chest";
		case EQUIPMENT_POS.SHOES:
			return "equip_shoes";
		case EQUIPMENT_POS.GLOVE:
			return "equip_glove";
		case EQUIPMENT_POS.PANTS:
			return "equip_pants";
		case EQUIPMENT_POS.SHOULDER:
			return "equip_shoulder";
		case EQUIPMENT_POS.WEAPON_L:
			return "equip_l_weapon";
		case EQUIPMENT_POS.WEAPON_R:
			return "equip_r_weapon";
		case EQUIPMENT_POS.ACCESSORY_1:
			return "equip_acc";
		case EQUIPMENT_POS.ACCESSORY_2:
			return "equip_acc";
		}

		return "";
	}

    public string GetHexColor(Byte u1Specialize)
    {
        string hexColor = "";
        switch(u1Specialize)
        {
            case (Byte)ClassInfo.ATTACK_ELEMENT.OFFENSIVE:
                hexColor = "#ff0000ff";
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.DEFENSIVE:
                hexColor = "#00ff00ff";
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.BALANCE:
                hexColor = "#0080ffff";
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.SPECIALIZE:
                hexColor = "#c600ffff";
                break;
        }
        return hexColor;
    }
}

public class EquipmentScrollSetInfo
{
	public UInt16 u2ID;
	public string sName;
	public string sDescription;
	public UInt16[] au2EquipIDs;

	public const int MAX_SCROLL_IN_SET = 8;

	public UInt16 SetInfo(string[] cols)
	{
		int idx = 0;
		u2ID = Convert.ToUInt16 (cols [idx++]);
		idx++; // comment
		sName = cols [idx++];
		sDescription = cols [idx++];

		au2EquipIDs = new ushort[MAX_SCROLL_IN_SET];
		for (int i = 0; i < MAX_SCROLL_IN_SET; i++) {
			au2EquipIDs[i] = Convert.ToUInt16 (cols [idx++]);
		}

		return u2ID;
	}
}