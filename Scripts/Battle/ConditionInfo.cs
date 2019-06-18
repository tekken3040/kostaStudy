using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;


public class ConditionInfo
{
	public UInt16 u2ID;
	public string sName;
	public string sDescription;

	public string sIcon;
	public string sCircle;

	public UInt16 u2Group;
	public Byte u1GroupLevel;

	public UInt32 u4DurationTime;
	public Byte u1EndType;			// 1 TimeOut, 2 be attacked, 3 anyone

	public Byte u1Movable;			// 1 able, 2 impossible, 3 impossible after time
	public UInt32 u4TimeForStop;	// at 3 impossible after time, the time

	public bool bBaseAttack;
	public bool bSkill;

	public Int16 u2MoveSpeedPercent;
	public Status cPercent;

	public float fAddDefPer;
	public float fAddRegPer;
	public float fAddEvaPer;

	public Int32 s4DurationDamage;
	public UInt32 u4DamageDurationTime;

	public bool bUnbeatable;

	public bool bChangable;
	public UInt16 u2ChangingClassID;

	public bool bAbsorption;
	public float fShieldPer;
	public bool bConditionClear;

	public SocketInfo cCreatePos;
	
	public UInt16 SetInfo(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment
		sName = cols[idx++];
		sDescription = cols[idx++];

		sIcon = cols[idx++];
		sCircle = cols[idx++];

		u2Group = Convert.ToUInt16(cols[idx++]);
		u1GroupLevel = Convert.ToByte(cols[idx++]);

		u4DurationTime = Convert.ToUInt32(cols[idx++]);
		u1EndType = Convert.ToByte(cols[idx++]);
		u1Movable = Convert.ToByte(cols[idx++]);
		u4TimeForStop = Convert.ToUInt32(cols[idx++]);

		bBaseAttack = Convert.ToByte(cols[idx++]) == 1;
		bSkill = Convert.ToByte(cols[idx++]) == 1;

		u2MoveSpeedPercent = Convert.ToInt16(cols[idx++]);
		cPercent.Set(cols, ref idx, false);

		fAddDefPer = (float)Convert.ToDouble(cols[idx++]);
		fAddRegPer = (float)Convert.ToDouble(cols[idx++]);
		fAddEvaPer = (float)Convert.ToDouble(cols[idx++]);

		s4DurationDamage = Convert.ToInt32(cols[idx++]);
		u4DamageDurationTime = Convert.ToUInt32(cols[idx++]);

		bUnbeatable = Convert.ToByte(cols[idx++]) == 1;

		bChangable = Convert.ToByte(cols[idx++]) == 1;
		u2ChangingClassID = Convert.ToUInt16(cols[idx++]);

		bAbsorption = Convert.ToByte(cols[idx++]) == 1;
		fShieldPer = (float)Convert.ToDouble(cols[idx++]);
		bConditionClear = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		idx++;

		UInt16 socketId = Convert.ToUInt16(cols[idx++]);
		if (socketId != 0) 
			cCreatePos = SocketInfoMgr.Instance.GetInfo(socketId);

		return u2ID;
	}
}

