using System;
using System.Collections.Generic;
using System.Text;

public enum SKILL_TARGET_TYPE
{
	Self = 1,
	Ally_Dead = 2,
//	Self_And_Ally,
	Enemy = 3,
	Ally_Low_HP = 4,
	Ally = 5,
	Self_And_Enemy = 6,
//	Ally_High_Aggro,
	Ally_And_Enemy = 7
}

public class SkillInfo
{
	public const int MAX_GRADE_OF_SKILL = 99;	//
	public const int MAX_BUFF_CALLED_BY_SKILL = 3;	//
	public const int MAX_DEBUFF_CALLED_BY_SKILL = 3;	//
	public const int MAX_MATERIAL_BY_LEARN = 4;	//
	public const int MAX_USE_SLOT = 12;	//
	public static readonly int[] USE_MANA_BY_INDEX = new int[3]{30,60,100};

    public UInt16 u2ID;
	public string sName;
	public string sDescription;
	public UInt16 u2ClassID;
	public Byte u1SlotNum;
	public Byte u1ActWay;			// 1 Active 2 Passive + auto active
	public bool bOpen;

	public Byte[] au1LinkSlot = new Byte[4];
	public Byte u1UsedSlot;
	public UInt16 u2NeedLevel;

	public float fSkillCool;
	public AttackSetInfo cAttackModel;

	public Byte u1BasicElement;		// 1 Physical 2 Magical
	public Byte u1Element;		// 0 None 1 Fire 2 Water 3 Wind 4 Change
	public SKILL_TARGET_TYPE u1Target;			// 1 Self 2 Out Team + Self 3 Enemy Team 4 lowest heath in our team 5 our team, 6 all, 7 max aggro in our team
	public Byte u1ActSituation;		// 1 None, 2 Start into Phase, 3 First Attack, 4 Killed, 5 Use Skill, 6 By the period
	public UInt32 u4CheckTime;		// if Situation is 6, The period
	public UInt16 u2ActSituationPercent;
	public Byte u1ActSituationPercentApply;	// 1 ToAll, 2 Re-Randominze to each target
	public float fPercentLvUpBonus;
	public float fActPerLvUpBonus;
	public float fCondTimeLvUpBonus;

	public Byte u1Duration;			// 1 Conitnual 2 Once
	public UInt32 u4DurationTime;
	public UInt32 u4TickTime;

	public VariableRange cChangeFactor;

	public UInt16 u2HPPercent;
	public UInt16 u2HealPercent;
	public UInt16 u2DamagePercent;

	public UInt16 u2LinkCondition;
	public Byte u1LinkBonus;
	public bool bComboBomb;

	public Status cPassive;
	public Status cPassiveUp;

	public float fPush;
	public float fRush;
	public UInt16 u2ResurrectionHPPercent;
	public bool bIgnoreDef;
	public bool bAddDamage;
	public bool bAttackCrit;
	public bool bSkillCrit;

	public ConditionInfo cBuff;
	public ConditionInfo cDebuff;
	
	public bool CheckAllySkill() {
		
		if(this.u1Target == SKILL_TARGET_TYPE.Enemy) return false;
		
		return true;
	}

	public bool CheckEnemySkill() {
		
		if(this.u1Target == SKILL_TARGET_TYPE.Ally || this.u1Target == SKILL_TARGET_TYPE.Ally_Dead
		   || this.u1Target == SKILL_TARGET_TYPE.Ally_Low_HP || this.u1Target == SKILL_TARGET_TYPE.Self) return false;

		return true;
	}

    public SkillInfo()
	{
    }
	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		UInt16 id;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment
		sName = cols[idx++];
		sDescription = cols[idx++];

		u2ClassID = Convert.ToUInt16(cols[idx++]);
		
		u1SlotNum = Convert.ToByte(cols[idx++]);
		u1ActWay = Convert.ToByte(cols[idx++]);

		bOpen = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		
		for (int i=0; i<au1LinkSlot.Length; i++) {
			au1LinkSlot[i] = Convert.ToByte(cols[idx++]);
		}

		u1UsedSlot = Convert.ToByte(cols[idx++]);
		u2NeedLevel = Convert.ToUInt16(cols[idx++]);
		fSkillCool = (float)Convert.ToDouble(cols[idx++]);

		id = Convert.ToUInt16(cols[idx++]);
		if (id != 0)
		cAttackModel = ClassInfoMgr.Instance.GetAttackSetInfo(id);

		u1BasicElement = Convert.ToByte(cols[idx++]);
		u1Element = Convert.ToByte(cols[idx++]);
		u1Target = (SKILL_TARGET_TYPE) Convert.ToByte(cols[idx++]);
		u1ActSituation = Convert.ToByte(cols[idx++]);
		u4CheckTime = Convert.ToUInt32(cols[idx++]);
		u2ActSituationPercent = Convert.ToUInt16(cols[idx++]);
		u1ActSituationPercentApply = Convert.ToByte(cols[idx++]);
		fPercentLvUpBonus = (float)Convert.ToDouble(cols[idx++]);
		fActPerLvUpBonus = (float)Convert.ToDouble(cols[idx++]);
		fCondTimeLvUpBonus = (float)Convert.ToDouble(cols[idx++]);
		
		u1Duration = Convert.ToByte(cols[idx++]);
		u4DurationTime = Convert.ToUInt32(cols[idx++]);
		u4TickTime = Convert.ToUInt32(cols[idx++]);

		u2HPPercent = Convert.ToUInt16(cols[idx++]);
		u2HealPercent = Convert.ToUInt16(cols[idx++]);
		u2DamagePercent = Convert.ToUInt16(cols[idx++]);

		u2LinkCondition = Convert.ToUInt16(cols[idx++]);
		u1LinkBonus = Convert.ToByte(cols[idx++]);

		bComboBomb = cols[idx] == "T" || cols[idx] == "t";
		idx++;

		cPassive.Set(cols, ref idx, true);
		cPassiveUp.Set(cols, ref idx, true);

		fPush = (float)Convert.ToDouble(cols[idx++]);
		fRush = (float)Convert.ToDouble(cols[idx++]);
		u2ResurrectionHPPercent = Convert.ToUInt16(cols[idx++]);
		bIgnoreDef = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		bAddDamage = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		bAttackCrit = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		bSkillCrit = cols[idx] == "T" || cols[idx] == "t";
		idx++;

		id = Convert.ToUInt16(cols[idx++]);
		if (id != 0) cBuff = ConditionInfoMgr.Instance.GetInfo(id);
		else cBuff = null;

		id = Convert.ToUInt16(cols[idx++]);
		if (id != 0) cDebuff = ConditionInfoMgr.Instance.GetInfo(id);
		else cDebuff = null;

		List<KeyValuePair<UInt16, ClassInfo>> infos = ClassInfoMgr.Instance.GetSkillGroup(u2ClassID);
		for(int i=0; i<infos.Count; i++){
			ClassInfo info = infos[i].Value;
			if (info != null) {
				if (u1ActWay == 1)
					info.acActiveSkills.Add(this);
				else if (u1ActWay == 2)
					info.acPassiveSkills.Add(this);
			}
		}
		return u2ID;
	}

	public string GetSkillDescToLevel(UInt16 Level){
		string conditionTime = "";
		string conditionPer = "";
		string damage = "";
		string chainDamage = "";
		string passive = "";
		string heal = "";

		float LevelStat = 0;
		float LevelPer = 0;
		float LevelTime = 0;

		if (Level > 1) {
			for (int i=0; i<Level-1; i++) {
				if(u1ActWay == 1) LevelStat += fPercentLvUpBonus;
				else LevelStat += cPassiveUp.GetFirstStat();
				LevelPer += fActPerLvUpBonus;
				LevelTime += fCondTimeLvUpBonus;
			}
		}

		if (cBuff != null){
			conditionTime = ((float)(cBuff.u4DurationTime/1000f + LevelTime)).ToString("#.00");

		}else if (cDebuff != null){
			conditionTime = ((float)(cDebuff.u4DurationTime/1000f + LevelTime)).ToString("#.00");
		}

		conditionPer = (u2ActSituationPercent + LevelPer).ToString("#.0");
		damage = (u2DamagePercent + LevelStat).ToString("0");
		chainDamage = u1LinkBonus.ToString();
		passive = (cPassive.GetFirstStat()+LevelStat).ToString("0");
		heal = (u2HealPercent + LevelStat).ToString("#.0");

		return string.Format(TextManager.Instance.GetText(sDescription), conditionTime, conditionPer, damage, chainDamage, passive, heal);
	}
}