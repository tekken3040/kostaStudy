using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ForgeInfo
{
	public const UInt16 FORGE_LEVEL_MAX = 10;
	public UInt16 u2ID;
	public string sName;
	public Byte u1Level;
	public struct SmithingInfo
	{
		public Byte u1RandomSkillCount;
		public Byte u1SelectSkillCount;
		public Byte u1SkillInitLevel;
		public Byte[] u1StatProbability;
		public const int MAX_STAT_PROBABILITY_AREA_COUNT=6;
		public void Set(string[] cols, ref int idx)
		{
			u1RandomSkillCount = Convert.ToByte(cols[idx++]);
			u1SelectSkillCount = Convert.ToByte(cols[idx++]);
			u1SkillInitLevel = Convert.ToByte(cols[idx++]);
			//u1StatProbability = new byte[MAX_STAT_PROBABILITY_AREA_COUNT];
			for(int i=0; i<MAX_STAT_PROBABILITY_AREA_COUNT; i++)
			{
				//u1StatProbability[i] = Convert.ToByte(cols[idx++]);
				idx++;
			}
		}

        public int GetSkillCount()
        {
            return u1RandomSkillCount + u1SelectSkillCount;
        }

//		public Dictionary<EquipmentInfo.EQUIPMENT_POS, SmithingMaterialInfo> dicMaterialInfo;
//
//		public void AddMaterial(string[] cols)
//		{
//			int idx = 0;
//			idx++; // ForgeID
//			EquipmentInfo.EQUIPMENT_POS posID = (EquipmentInfo.EQUIPMENT_POS)Convert.ToByte(cols[idx++]);
//
//			SmithingMaterialInfo materialInfo = new SmithingMaterialInfo();
//			materialInfo.Set(cols, ref idx);
//
//			if(dicMaterialInfo == null)
//				dicMaterialInfo = new Dictionary<EquipmentInfo.EQUIPMENT_POS, SmithingMaterialInfo>();
//			dicMaterialInfo.Add(posID, materialInfo);
//		}

//		public struct SmithingMaterialInfo
//		{
//			public Goods cGoods;
//
//			public const Byte NEED_MATERIAL_MAX = 4;
//			public Byte u1MaterialCount;
//			public Goods[] acMaterial;
//			public void Set(string[] cols, ref int idx)
//			{
//				cGoods = new Goods (cols, ref idx);
//				u1MaterialCount = Convert.ToByte(cols[idx++]);
//				acMaterial = new Goods[u1MaterialCount];
//				for(int i=0; i<u1MaterialCount; i++)
//				{
//					acMaterial[i] = new Goods(cols, ref idx);
//				}
//			}
//		}
	}
	public SmithingInfo cSmithingInfo;

	public struct ForgeUpgradeInfo
	{
		public const int MAX_MAT_COUNT = 4;
		public Goods[] acUpgradeMaterials;
		public Goods cUpgradeGoods;
		public void Set(string[] cols, ref int idx)
		{
			Byte matCount = Convert.ToByte(cols[idx++]);
			acUpgradeMaterials = new Goods[matCount];
			for(Byte i=0; i<matCount; i++)
			{
				acUpgradeMaterials[i] = new Goods(cols, ref idx);
			}
			idx += (MAX_MAT_COUNT-matCount) * 3;
			cUpgradeGoods = new Goods(cols, ref idx);
		}
	}
	public ForgeUpgradeInfo cUpgradeInfo;
	
	public struct LookChangeInfo
	{
		public Goods cChangeGoods;
		public Byte u1RandomModelBound;
		public Byte u1SelectModelBound;
		public const int MAX_LOOK_COUNT = 15;
		public void Set(string[] cols, ref int idx)
		{
			cChangeGoods = new Goods(cols, ref idx);
//			u1RandomModelBound = Convert.ToByte(cols[idx++]);
//			u1SelectModelBound = Convert.ToByte(cols[idx++]);
		}
	}

    public struct UnlockCategoryInfo
    {
        public Byte u1PosID;
        public Byte u1EquipSlotCondition;
        public UInt16 u2EquipSlotID;
        public UInt16 u2EquipSlotValue;

        public Byte Set(string[] cols)
	    {
            int idx = 0;
            u1PosID = Convert.ToByte(cols[idx++]);
            u1EquipSlotCondition = Convert.ToByte(cols[idx++]);
            u2EquipSlotID = Convert.ToUInt16(cols[idx++]);
            u2EquipSlotValue = Convert.ToUInt16(cols[idx++]);

            return u1PosID;
        }
    }

	public LookChangeInfo cLookChangeInfo;
	public Goods cFusionGoods;

	public float u4StatMultiplePerSmithingLevel;
	public UInt16 u2RankScore;
	public float u4StatMultiplePerLevelUp;
    public UInt32 u4FusionExp;
    public Byte[] u1PosSkillActWay;
    public float u2EquipShopCostFacter;

	public UInt16 SetInfo(string[] cols, Byte level)
	{
		u1Level = level;
		int idx = 0;
		try
		{
			u2ID = Convert.ToUInt16(cols[idx++]);
			idx++; // comment
			sName = cols[idx++];
			cSmithingInfo.Set(cols, ref idx);
			cUpgradeInfo.Set(cols, ref idx);
			cLookChangeInfo.Set(cols, ref idx);
			cFusionGoods = new Goods(cols, ref idx);
			u4StatMultiplePerSmithingLevel = float.Parse(cols[idx++]);
			u2RankScore = Convert.ToUInt16(cols[idx++]);
			u4StatMultiplePerLevelUp = float.Parse(cols[idx++]);
            u4FusionExp = Convert.ToUInt32(cols[idx++]);
            u1PosSkillActWay = new Byte[Server.ConstDef.MaxItemSlot];
            for(int i=0; i<Server.ConstDef.MaxItemSlot; i++)
                u1PosSkillActWay[i] = Convert.ToByte(cols[idx++]);
			u2EquipShopCostFacter = (float)Convert.ToDouble(cols[idx++]);
		}
		catch(System.Exception ex)
		{
			DebugMgr.LogError("Error : " + idx);
			DebugMgr.LogError("Error : " + cols[idx]);
		}
		return u2ID;
	}

//	public static Color32[] forgeLevelNameGradientColor_Start = {
//		new Color32(255, 255, 255, 255), // 무속성, 속성 없음
//		new Color32(214, 65, 107, 255), // 불										
//		new Color32(57, 255, 255, 255), // 물
//		new Color32(125, 255, 149, 255)
//	}; //바람
//
//	public static Color32[] forgeLevelNameGradientColor_End = {
//		new Color32(255, 255, 255, 255), // 무속성, 속성 없음
//		new Color32(214, 65, 107, 255), // 불										
//		new Color32(57, 255, 255, 255), // 물
//		new Color32(125, 255, 149, 255)
//	}; //바람
//
//	public static Color32[] forgeLevelColor = {
//		new Color32(88, 88, 88, 255),
//		new Color32(117, 134, 134, 255),
//		new Color32(255, 255, 255, 255),
//		new Color32(251, 225, 42, 255),
//		new Color32(10, 220, 158, 255),
//		new Color32(26, 112, 248, 255),
//		new Color32(133, 20, 218, 255),
//		new Color32(241, 55, 194, 255),
//		new Color32(255, 111, 46, 255),
//		new Color32(255, 222, 39, 255)
//	};
}
