using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PrecedenceInfo
{
	public UInt16 u2ID;
	public UInt16[] asTargetID;
	
	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16 (cols [idx++]);
		
		asTargetID = new UInt16[10];
		
		for (int i = 0; i < asTargetID.Length; i++)
		{
			asTargetID[i] = Convert.ToUInt16 (cols [idx++]);
		}
		
		return u2ID;
	}
}

public class AttackSetInfo
{
	public UInt16 u2ID;
	public string sStartAnim;
	public AttackModelInfo cAttack; 
	public float fDelay;
	public float fMaxDelay;
	public Byte u1AttackType;
	public bool bStay;

	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16 (cols [idx++]);
		idx++;
		sStartAnim = cols [idx++];
		UInt16 attackId = Convert.ToUInt16(cols[idx++]);
		if(attackId != 0) cAttack = ClassInfoMgr.Instance.GetAttackInfo(attackId);
		fDelay = (float)Convert.ToDouble(cols[idx++]);
		fMaxDelay = (float)Convert.ToDouble(cols[idx++]);
		u1AttackType = Convert.ToByte (cols [idx++]);
		bStay = cols[idx] == "T" || cols[idx] == "t";

		return u2ID;
	}
}

public class AttackModelInfo
{
	public UInt16 u2ID;
	//public string sName;

	public Byte u1BasicAttack;	// 1 : short range, 2 : 
	public Byte u1BasicAttackType;
	public Byte u1MultiTargetAttackNumber;
	public bool bPenetrate;
	public UInt16 u2DurationTime;
	public UInt16 u2AttackTick;
	public float u2ThrowRange;
	public float u2AttackRange;
	public UInt16 u2AttackAngle;
	
	public SocketInfo cMissileCreatePos;
	public UInt16 u2MissileShootTime;
	public UInt16 u2MissileSpeed;

	public string sMissileModelName;
	public string sMissileCrushModelName;
	public string sMissileHitModelName;
	public Byte u1HitPos;

	public string sVibratePattern;
	public float fVibratePower;
	public UInt16 u2VibrateTime;

	public UInt16 u2AttackPower;
	public bool bKnockback;
	public float fKnockbackDist;
	public UInt16 u2DamagePercent;

	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment

		u1BasicAttack = Convert.ToByte(cols[idx++]);
		u1BasicAttackType = Convert.ToByte(cols[idx++]);
		u1MultiTargetAttackNumber = Convert.ToByte(cols[idx++]);

		bPenetrate = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		u2DurationTime = Convert.ToUInt16(cols[idx++]);
		u2AttackTick= Convert.ToUInt16(cols[idx++]);

		u2ThrowRange = (float)Convert.ToDouble(cols[idx++]);
		u2AttackRange = (float)Convert.ToDouble(cols[idx++]);
		u2AttackAngle = Convert.ToUInt16(cols[idx++]);

		UInt16 socketId = Convert.ToUInt16(cols[idx++]);
		if (socketId != 0) cMissileCreatePos = SocketInfoMgr.Instance.GetInfo(socketId);
		u2MissileShootTime = Convert.ToUInt16(cols[idx++]);
		u2MissileSpeed = Convert.ToUInt16(cols[idx++]);

		sMissileModelName = cols[idx++];
		sMissileCrushModelName = cols[idx++];
		sMissileHitModelName = cols[idx++];

		u1HitPos = Convert.ToByte(cols[idx++]);

		sVibratePattern = cols[idx++];
		fVibratePower = (float)Convert.ToDouble(cols[idx++]);
		u2VibrateTime = Convert.ToUInt16(cols[idx++]);

		u2AttackPower = Convert.ToUInt16(cols[idx++]);
		bKnockback = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		fKnockbackDist = (float)Convert.ToDouble(cols[idx++]);
		u2DamagePercent = Convert.ToUInt16(cols[idx++]);

		return u2ID;
	}
}

public class SkillAIInfo
{
	public const int MAX_ACTIVESKILL_IN_BATTLE = 6;

	public UInt16 u2ID;
	//public string sName;

	public Byte u1AIType;	// 1 : random, 2 : sequence
	public UInt32 u4Time;

	public UInt16[] au2Values;

	public Byte u1Target;
	public bool bLoop;
	public UInt16 u2HPPer;
	public UInt16 u2MPPer;
	public UInt16 u2Damage;
	public UInt16 u2UseSkillID;

	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment

		u1AIType = Convert.ToByte(cols[idx++]);
		u4Time = Convert.ToUInt32(cols[idx++]);

		au2Values = new UInt16[MAX_ACTIVESKILL_IN_BATTLE];
		for (int i = 0; i < MAX_ACTIVESKILL_IN_BATTLE; i++)
		{
			au2Values[i] = Convert.ToUInt16(cols[idx++]);
		}

		u1Target = Convert.ToByte(cols[idx++]);
		bLoop = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		u2HPPer = Convert.ToUInt16(cols[idx++]);
		u2MPPer = Convert.ToUInt16(cols[idx++]);
		u2Damage = Convert.ToUInt16(cols[idx++]);
		u2UseSkillID = Convert.ToUInt16(cols[idx++]);

		return u2ID;
	}
}

public class FaceInfo
{
	public UInt16 u2ID;
	public UInt16 u2ClassID;
	public Byte u1Element;
	public UInt16 u2ModelID;
	public UInt16 u2ReplaceModelID;
	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment
		u2ClassID = Convert.ToUInt16(cols[idx++]);
		u1Element = Convert.ToByte(cols[idx++]);
		u2ModelID = Convert.ToUInt16(cols[idx++]);
		u2ReplaceModelID = Convert.ToUInt16(cols[idx++]);
		return u2ID;
	}
}

public class HairColorInfo
{
	public UInt16 u2ID;
	public UInt16 u2ClassID;
	public String strColor;
	public Byte u1Type;
	public Byte[] R;
	public Byte[] G;
	public Byte[] B;
	public float[] Str;

	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++; // comment
		u2ClassID = Convert.ToUInt16(cols[idx++]);
		strColor = cols[idx++];
		u1Type = Convert.ToByte (cols [idx++]);

		R = new Byte[3];
		G = new Byte[3];
		B = new Byte[3];
		Str = new float[3];
		for (int i=0; i<3; i++) {
			R[i] = Convert.ToByte (cols [idx++]);
			G[i] = Convert.ToByte (cols [idx++]);
			B[i] = Convert.ToByte (cols [idx++]);
			Str[i] = (float)Convert.ToDouble (cols [idx++]);
		}
		return u2ID;
	}
}

//public class AnimLengthInfo
//{
//	public string sName;
//	public float fLength;
//
//	public string Set(string[] cols)
//	{
//		UInt16 idx = 0;
//		sName = cols[idx++];
//		fLength = (float)Convert.ToDouble (cols [idx++]);
//
//		return sName;
//	}
//}

public class ClassInfo
{
	public const int MAX_FACE_OF_CLASS = 7;
	public const int MAX_BASEEQUIP_OF_MODEL = 8;
	public const int MAX_BASEEQUIP_OF_CLASS = 10;
	public const int MAX_BASEEQUIP_OF_MONSTER = 5;
	public const int MAX_ATTACK_OF_CLASS = 3;
	public const int MAX_AVOID_ATTACK_OF_CLASS = 1;
	public const int MAX_RUSH_ATTACK_OF_CLASS = 2;
	public const int MAX_GRADE_OF_CHARACTER = 10;	//
	public const int MAX_SKILL_OF_CHARACTER = 6;	//
	public const int MAX_CHARACTER_SKILL_AI = 1;
	public const int MAX_MONSTER_SKILL_AI = 3;
	public const int MAX_CHARACTER_AUTO_STATUS = 3;
	public const float MOVE_MAGNIF = 1.8f;
	public const int LAST_CLASS_ID = 10;
	public const Byte COMMON_CLASS_ID = 11;

	public const int MAX_STATUS_RATIO = 20;	// 비율
//	public struct AttackInfo
//	{
//		public AttackModelInfo cAttackModel;
//		public UInt16 u1Percent;		// %
//		public UInt16 u1DamageFactor;	// %
//	}

    public enum ATTACK_ELEMENT
    {
        PHYSICAL = 1,
        MAGICAL = 2,
        OFFENSIVE = 3,
        DEFENSIVE = 4,
        BALANCE = 5,
        SPECIALIZE = 6
    }

	public struct AttackInfo
	{
		public AttackSetInfo cAttackSet;
		public UInt16 u1Percent;
		public Byte u1Type;
	}

	public struct FaceModelInfo
	{
		public UInt16 u2ID;
		public UInt16 u2ModelID;
		public ModelInfo cModelInfo;
		public void Set(FaceInfo faceInfo)
		{
			u2ID = faceInfo.u2ID;
			u2ModelID = faceInfo.u2ModelID;
			cModelInfo = new ModelInfo();
			cModelInfo = ModelInfoMgr.Instance.GetInfo(u2ModelID);
			// DebugMgr.Log("facemodel info : " + cModelInfo.u2ID);
		}
	}

	public struct HairModelInfo
	{
		public UInt16 u2ID;
		public UInt16 u2ModelID;
		public ModelInfo cModelInfo;
		public UInt16 u2ReplaceModelID;
		public void Set(FaceInfo faceInfo)
		{
			u2ID = faceInfo.u2ID;
			u2ModelID = faceInfo.u2ModelID;
			cModelInfo = new ModelInfo();
			cModelInfo = ModelInfoMgr.Instance.GetInfo(u2ModelID);
			u2ReplaceModelID = faceInfo.u2ReplaceModelID;
		}
	}
	public UInt16 u2ID;

	string _sName;
	public string sName
	{
		set{ _sName = value; }
		get{ return _sName; }//return TextManager.Instance.GetText(_sName); }
	}
	string _sDescription;
	public string sDescription
	{
		set{ _sDescription = value; }
		get{ return TextManager.Instance.GetText(_sDescription); }
	}

	public EquipmentItem[] u2BasicEquips;
	public List<FaceModelInfo> lstFaceInfo;
	public List<HairModelInfo> lstHairInfo;
	public List<HairColorInfo> lstHairColor;
	public Byte u1MonsterType;	
	public Byte u1AttackDistance;		// 1 Melee, 2 Range
	public Byte u1BasicAttackElement;

	public bool bRangeTargetting;
	public Byte u1TargetType;
	public Byte u1TargetDistance;
	public Byte u1TargetElement;
	public PrecedenceInfo cPriority;
	public Byte u1Element;

	public float fViewDistance;
	public float fAttackDecisionDist;
	public float fBasicAttackDist;
	public float fJumpAttackDist;
	public float fRushAttackDist;

	public AttackInfo[] acAttacks; //basic & avoid
	public AttackInfo[] acRushAttacks; //rush & jump

	public UInt16 u2Speed;

	public StatusInfo cStatus;

	public VariableRange cCriticalRange;
	public VariableRange cAttackSpeedRange;
	public VariableRange cEvasionRange;
	public VariableRange cDefPerRange;
	public VariableRange cPhysicalAttack;
	public VariableRange cMagicAttack;
	public VariableRange cPhysicalAttackAgility;
	public VariableRange cMagicAttackAgility;
	public VariableRange cPhysicalDefence;
	public VariableRange cMagicDefence;
	public VariableRange cEvasionFactor;
	public VariableRange cCriticalFactor;
	public VariableRange cAttackSpeedFactor;


	public List<SkillInfo> acActiveSkills;
	public List<SkillInfo> acPassiveSkills;

	public UInt16 u2SkillGroup;
	public SkillAIInfo[] cAIInfo;
	public SocketInfo cHitPos;

	public UInt16 u2AttackReg;
	public bool bKnockbackIgnore;
	public float fScale;
	public float fDiameter;

	public Byte[] au1AutoStat;
	public Byte u1SkillTree;

	private String m_sFeature;		// 특성[특화성]
	public String Feature		 
	{
		set { m_sFeature = value; }
		get { return TextManager.Instance.GetText(m_sFeature); }
	}

	private String sAttackAttribute;	// 속성
	public String AttackAttribute
	{
		set { sAttackAttribute = value;}
		get { return TextManager.Instance.GetText(sAttackAttribute); }
	}

	private Byte u1AttackRatio;		// 공격력 비율
	public Byte AttackRatio
	{
		get { return u1AttackRatio; }
		set { u1AttackRatio =value; }
	}
	private Byte u1DefenceRatio;	// 방어력 비율
	public Byte DefenceRatio
	{
		get { return u1DefenceRatio; } 
		set { u1DefenceRatio = value; }
	}
	private Byte _u1ClassLockType;
	public Byte ClassLockType
	{
		get{ return _u1ClassLockType; }
		set{ _u1ClassLockType = value; }
	}
	private string strUnLockInfo;
	public string UnLockInfo
	{
		get { return strUnLockInfo; }
		set { strUnLockInfo = value; }
	}

	public string sDirectionCam;

	public bool bTutorialReward;
	public UInt16 u2EquipSetID;

	public EquipmentItem[] u2CreateSceneEquips;

    private String strClassDefaultName;
    public String ClassDefaultName { get { return strClassDefaultName; } }

    public ClassInfo()
	{
		lstFaceInfo = new List<FaceModelInfo>();
		lstHairInfo = new List<HairModelInfo>();
		lstHairColor = new List<HairColorInfo>();

		acActiveSkills = new List<SkillInfo>();
		acPassiveSkills = new List<SkillInfo>();
    }
    public ClassInfo(UInt16 id, string name)
    {
        u2ID = id;
        sName = name;
		lstFaceInfo = new List<FaceModelInfo>();
		lstHairInfo = new List<HairModelInfo>();
    }
	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		UInt16 id;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment
		sName = cols[idx++];
		sDescription = cols[idx++];
		u2BasicEquips = new EquipmentItem[MAX_BASEEQUIP_OF_CLASS];
		for (int bIdx = 0; bIdx < MAX_BASEEQUIP_OF_CLASS; bIdx++) 
		{
			id = Convert.ToUInt16(cols[idx++]);
			// DebugMgr.Log("basic equip : " + id);
			if (id != 0)
			{
///				u2BasicEquips[bIdx] = new ModelInfo();
//				DebugMgr.Log("basic equip1 : " + EquipmentInfoMgr.Instance.GetInfo(id).u2ID);
				u2BasicEquips[bIdx] = new EquipmentItem(id);
//				DebugMgr.Log("basic equip2 : " + u2BasicEquips[bIdx].u2ID);
			}
		}

		u1AttackDistance = Convert.ToByte(cols[idx++]);
		u1BasicAttackElement = Convert.ToByte(cols[idx++]);

		bRangeTargetting = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		u1TargetType = Convert.ToByte(cols[idx++]);
		u1TargetDistance = Convert.ToByte(cols[idx++]);
		u1TargetElement = Convert.ToByte(cols[idx++]);
		
		fViewDistance = (float)Convert.ToDouble(cols[idx++]);
		fAttackDecisionDist = (float)Convert.ToDouble(cols[idx++]);
		fBasicAttackDist = (float)Convert.ToDouble(cols[idx++]);
		fJumpAttackDist = (float)Convert.ToDouble(cols[idx++]);
		fRushAttackDist = (float)Convert.ToDouble(cols[idx++]);

		acAttacks = new AttackInfo[MAX_ATTACK_OF_CLASS+MAX_AVOID_ATTACK_OF_CLASS];
		for (int aIdx = 0; aIdx < MAX_ATTACK_OF_CLASS+MAX_AVOID_ATTACK_OF_CLASS; aIdx++)
		{
			id = Convert.ToUInt16(cols[idx++]);
			if (id != 0)
				acAttacks[aIdx].cAttackSet = ClassInfoMgr.Instance.GetAttackSetInfo(id);
			acAttacks[aIdx].u1Percent = Convert.ToUInt16(cols[idx++]);
			acAttacks[aIdx].u1Type = 1;
		}

		acRushAttacks = new AttackInfo[MAX_RUSH_ATTACK_OF_CLASS];
		for (int aIdx = 0; aIdx < MAX_RUSH_ATTACK_OF_CLASS; aIdx++)
		{
			id = Convert.ToUInt16(cols[idx++]);
			if (id != 0)
				acRushAttacks[aIdx].cAttackSet = ClassInfoMgr.Instance.GetAttackSetInfo(id);
			acRushAttacks[aIdx].u1Percent = Convert.ToUInt16(cols[idx++]);
			acRushAttacks[aIdx].u1Type = 2;
		}

		u2Speed = Convert.ToUInt16(cols[idx++]);
//		u2Aggro = Convert.ToUInt16(cols[idx++]);
//		fAggroFactor = Convert.ToDouble(cols[idx++]);

		cStatus = new StatusInfo();
		cStatus.cBasic.Set(cols, ref idx, true);
		cStatus.cLevelUp.Set(cols, ref idx, true);

		cCriticalRange.Set(cols, ref idx);
		cAttackSpeedRange.Set(cols, ref idx);
		cEvasionRange.Set(cols, ref idx);
		cDefPerRange.Set(cols, ref idx);
		cPhysicalAttack.Set(cols, ref idx);
		cMagicAttack.Set(cols, ref idx);
		cPhysicalAttackAgility.Set(cols, ref idx);
		cMagicAttackAgility.Set(cols, ref idx);
		cPhysicalDefence.Set(cols, ref idx);
		cMagicDefence.Set(cols, ref idx);
		cEvasionFactor.Set(cols, ref idx);
		cCriticalFactor.Set(cols, ref idx);
		cAttackSpeedFactor.Set(cols, ref idx);

		u2SkillGroup = u2ID;

		cAIInfo = new SkillAIInfo[MAX_CHARACTER_SKILL_AI];
		for(int i=0; i<cAIInfo.Length; i++){
			id = Convert.ToUInt16(cols[idx++]);
			if (id != 0) cAIInfo[i] = ClassInfoMgr.Instance.GetSkillAIInfo(id);
		}

		id = Convert.ToUInt16(cols[idx++]);
		if (id != 0) cHitPos = SocketInfoMgr.Instance.GetInfo(id);

		u2AttackReg = Convert.ToUInt16(cols[idx++]);
		bKnockbackIgnore = cols[idx] == "T" || cols[idx] == "t";
		idx++;

		fScale = 1.0f;
		fDiameter = (float)Convert.ToDouble(cols[idx++]);

		au1AutoStat = new Byte[MAX_CHARACTER_AUTO_STATUS];
		for(int i=0; i<au1AutoStat.Length; i++){
			au1AutoStat[i] = Convert.ToByte(cols[idx++]);
		}

		u1SkillTree = Convert.ToByte(cols[idx++]);
		cPriority = ClassInfoMgr.Instance.GetPrecedenceInfo(Convert.ToUInt16(cols[idx++]));//idx++;// 캐릭터 우선순위 
		m_sFeature = cols[idx++];						// 특성
		sAttackAttribute = cols[idx++];					// 속성
		u1AttackRatio = Convert.ToByte(cols[idx++]);	// 공격력 비율
		u1DefenceRatio = Convert.ToByte(cols[idx++]);	// 방어력 비율
		_u1ClassLockType = Convert.ToByte(cols[idx++]);	// 클래스 잠김 여부
		strUnLockInfo = cols[idx++];
		idx++;
		idx++;
		idx++;
		bTutorialReward = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		u2EquipSetID = Convert.ToUInt16(cols[idx++]);

		u2CreateSceneEquips = new EquipmentItem[MAX_BASEEQUIP_OF_CLASS];
		for (int bIdx = 0; bIdx < MAX_BASEEQUIP_OF_CLASS; bIdx++) 
		{
			id = Convert.ToUInt16(cols[idx++]);
			if (id != 0)
			{
				u2CreateSceneEquips[bIdx] = new EquipmentItem(id);
			}
		}

        strClassDefaultName = cols[idx++];

        return u2ID;
	}

	public void AddFace(FaceInfo faceInfo)
	{
		FaceModelInfo faceModelInfo = new FaceModelInfo();
		faceModelInfo.Set(faceInfo);
//		DebugMgr.Log("FaceMODEL : " + faceModelInfo.u2ModelID);
		lstFaceInfo.Add(faceModelInfo);
	}

	public void AddHair(FaceInfo faceInfo)
	{
		HairModelInfo hairModelInfo = new HairModelInfo();
		hairModelInfo.Set(faceInfo);
		lstHairInfo.Add(hairModelInfo);
	}

	public void AddHairColor(HairColorInfo hairColorInfo)
	{
		lstHairColor.Add(hairColorInfo);
	}

	public UInt16 SetMonster(string[] cols)
	{
		UInt16 idx = 0;
		UInt16 id;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment
		sName = cols[idx++];

		u2BasicEquips = new EquipmentItem[MAX_BASEEQUIP_OF_MONSTER];
		for (int bIdx = 0; bIdx < MAX_BASEEQUIP_OF_MONSTER; bIdx++)
		{
			id = Convert.ToUInt16(cols[idx++]);
			if (id != 0)
			{
				// DebugMgr.Log("MOnster Model : " + id);
				u2BasicEquips[bIdx] = new EquipmentItem(id);
			}
		}

		u1MonsterType = Convert.ToByte(cols[idx++]);

		u1AttackDistance = Convert.ToByte(cols[idx++]);
		u1BasicAttackElement = Convert.ToByte(cols[idx++]);

		bRangeTargetting = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		u1TargetDistance = Convert.ToByte(cols[idx++]);
		u1TargetElement = Convert.ToByte(cols[idx++]);

		cPriority = ClassInfoMgr.Instance.GetPrecedenceInfo(Convert.ToUInt16(cols[idx++]));
		u1Element = Convert.ToByte(cols[idx++]);
		
		fViewDistance = (float)Convert.ToDouble(cols[idx++]);
		fBasicAttackDist = (float)Convert.ToDouble(cols[idx++]);

		acAttacks = new AttackInfo[MAX_ATTACK_OF_CLASS];
		for (int aIdx = 0; aIdx < MAX_ATTACK_OF_CLASS; aIdx++)
		{
			id = Convert.ToUInt16(cols[idx++]);
			if (id != 0)
				acAttacks[aIdx].cAttackSet = ClassInfoMgr.Instance.GetAttackSetInfo(id);
			acAttacks[aIdx].u1Percent = Convert.ToUInt16(cols[idx++]);
			acAttacks[aIdx].u1Type = 1;
		}

		u2Speed = Convert.ToUInt16(cols[idx++]);

		cStatus = new StatusInfo();
		cStatus.cBasic.Set(cols, ref idx, true);
		cStatus.cLevelUp.Set(cols, ref idx, true);

		cCriticalRange.Set(cols, ref idx);
		cAttackSpeedRange.Set(cols, ref idx);

		cEvasionRange.Set(cols, ref idx);
		cDefPerRange.Set(cols, ref idx);
		cPhysicalAttack.Set(cols, ref idx);
		cMagicAttack.Set(cols, ref idx);
		cPhysicalAttackAgility.Set(cols, ref idx);
		cMagicAttackAgility.Set(cols, ref idx);
		cPhysicalDefence.Set(cols, ref idx);
		cMagicDefence.Set(cols, ref idx);
		cEvasionFactor.Set(cols, ref idx);
		cCriticalFactor.Set(cols, ref idx);
		cAttackSpeedFactor.Set(cols, ref idx);

		UInt16 socketId = Convert.ToUInt16(cols[idx++]);
		if (socketId != 0) cHitPos = SocketInfoMgr.Instance.GetInfo(socketId);

		u2SkillGroup = Convert.ToUInt16(cols[idx++]);

		cAIInfo = new SkillAIInfo[MAX_MONSTER_SKILL_AI];
		for(int i=0; i<cAIInfo.Length; i++){
			id = Convert.ToUInt16(cols[idx++]);
			if (id != 0) cAIInfo[i] = ClassInfoMgr.Instance.GetSkillAIInfo(id);
		}

		u2AttackReg = Convert.ToUInt16(cols[idx++]);
		bKnockbackIgnore = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		fScale = (float)Convert.ToDouble(cols[idx++]);
		fDiameter = (float)Convert.ToDouble(cols[idx++]);

		sDirectionCam = cols[idx++];

		return u2ID;
	}
}