using System;
using System.Collections.Generic;
using System.Text;

public struct Level
{
	const Byte GRADE_MAX = 19;
	public const Byte STAR_MAX_CHAR = 5;
	public const Byte STAR_MAX_EQUIP = 5;
	public UInt16 u2Level;
    //public UInt32 u4Exp;
	public UInt64 u8Exp;

	public UInt64 u8NextExp
	{
		get{
			UInt64 ret=1;
			if(u2Level < Server.ConstDef.MaxHeroLevel)
				ret = ClassInfoMgr.Instance.GetNextExp(u2Level);
            else
                ret = 0;

			return ret;
		}
	}

	void Clear()
	{
		u2Level = 1;
		u8Exp = 0;
	}

	public void Set(string[] data, ref UInt16 idx)
	{
		DebugMgr.Log("data length : " + data.Length);
		DebugMgr.Log(string.Format("{0} {1} {2} {3}", data[0], data[1], data[2], data[3]));
		u2Level = Convert.ToUInt16(data[idx++]);
		u8Exp = 0;
	}
}

public class LevelComponent : ZComponent
{
	public bool bDummy = false;
	public Level cLevel;
    public bool bLevelup = false;

	public void Set(UInt16 u2Level, UInt64 u8Exp)//UInt32 u4Exp)
	{
		cLevel.u2Level = u2Level;
		cLevel.u8Exp = u8Exp;
			
		if(owner.GetComponent<StatusComponent>() != null)
		{
			if(owner is Hero)
			{
				StatusComponent c = owner.GetComponent<StatusComponent>();
				c.SetByLevel(cLevel);
			}
			else
			{
				StatusComponent c = owner.GetComponent<StatusComponent>();
				c.SetByLevelEquip(cLevel);
			}
		}
	}

	public void Set(Level level)
	{
		cLevel = level;
		StatusComponent c = owner.GetComponent<StatusComponent>();
		if (c != null) c.SetByLevel(cLevel);
	}

	public void LevelUp()
	{
		cLevel.u2Level++;
		cLevel.u8Exp = 0;
        bLevelup = true;
		StatusComponent c = owner.GetComponent<StatusComponent>();
		if (c != null)
		{
			c.LevelUp(1);
        }
    }

	public Byte AddExp(UInt64 u8Exp)
	{
		if (cLevel.u2Level == Server.ConstDef.MaxHeroLevel) {
			return 0;
		}
		cLevel.u8Exp += u8Exp;
		if (cLevel.u8Exp < cLevel.u8NextExp)
		{
			return 0;
		}
		Byte LevelUp = 0;
		while (cLevel.u8Exp >= cLevel.u8NextExp)
		{
			cLevel.u8Exp -= cLevel.u8NextExp;
			cLevel.u2Level++;
			LevelUp++;
            bLevelup = true;
			if(!bDummy)
			{
	            if(owner is Hero)
	            {
					Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.CharLevel, 0, (Byte)((Hero)owner).cClass.u2ID, 0, 0, cLevel.u2Level);
	            }
	            else
	            {
					EquipmentItem item = (EquipmentItem)owner;
					if(item.attached.hero != null){
						if(cLevel.u2Level > item.attached.hero.cLevel.u2Level){
							cLevel.u2Level = item.attached.hero.cLevel.u2Level;
							cLevel.u8Exp = cLevel.u8NextExp-1;
						}
					}
					Legion.Instance.SetTopEquipLevel (cLevel.u2Level);
					Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.EquipLevel, item.cItemInfo.u2ID, (Byte)item.GetEquipmentInfo().u1PosID, 0, 0, cLevel.u2Level);
	            }
			}
            if (cLevel.u2Level == Server.ConstDef.MaxHeroLevel)
            {
                cLevel.u8Exp = 0;
			    break;
		    }
		}

		StatusComponent c = owner.GetComponent<StatusComponent>();
		if (c != null)
		{
			if(owner is Hero)
			{
				c.LevelUp(LevelUp);
			}
			if(owner is EquipmentItem)
			{
				c.SetByLevelEquip(cLevel);
			}
		}

		SkillComponent sc = owner.GetComponent<SkillComponent> ();
		if(sc != null) sc.SkillPointUp(cLevel.u2Level, LevelUp);
		return LevelUp;
	}
}