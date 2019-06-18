using System;
using System.Collections;
using System.Collections.Generic;

public class FieldInfo
{
	public struct MonsterInfo
	{
		public UInt16 u2MonsterID;
		public Level cLevel;
		public Byte u1SpawnCount;

		public void Set(string[] data, ref UInt16 idx)
		{
			u2MonsterID = Convert.ToUInt16(data[idx++]);
			cLevel.u2Level = Convert.ToUInt16(data[idx++]);
			u1SpawnCount = Convert.ToByte(data[idx++]);
		}
	}
	
	public struct MonsterGroup
	{
		//		public UInt16 u2FieldID;
		//		public Byte u1GroupNum;
		private FieldInfo cParent;
		public UInt16[] u2DiffcultyAddLevel;
		public Byte u1GroupNum;
		public Byte u1SubGroupNum;
		public Byte u1MonsterCountMax;
		public Byte u1MonsterCount;
		public MonsterInfo[] acMonsterInfo;

		public Pos getCenterPos()
		{
			return cParent.acMonsterGroupPosInfo[u1GroupNum-1].cCenterPos;
		}

		public void Set(FieldInfo parent, string[] data, Byte monsterCount, ref UInt16 idx)
		{
			string datacombine="";
			for(int i=0; i<data.Length; i++)
			{
				datacombine+=data[i].ToString() + "_";
			}
			cParent = parent;

			u1GroupNum = Convert.ToByte(data[idx++]);

			u2DiffcultyAddLevel = new UInt16[2];
			for(int i=0; i<u2DiffcultyAddLevel.Length; i++)
			{
				u2DiffcultyAddLevel[i] = Convert.ToUInt16(data[idx++]);
			}

			u1SubGroupNum = Convert.ToByte(data[idx++]);
			u1MonsterCountMax = Convert.ToByte(data[idx++]);
			u1MonsterCount = Convert.ToByte(data[idx++]);
			acMonsterInfo = new MonsterInfo[u1SubGroupNum];
			for(int i=0; i<u1SubGroupNum; i++)
			{
				acMonsterInfo[i].Set(data, ref idx);

			}
		}
	}

	public struct MonsterGroupPos
	{
		public UInt16 u2PosID;
		public Pos cCenterPos;
		public void Set(string[] data, ref UInt16 idx)
		{
			cCenterPos = new Pos();
			u2PosID = Convert.ToUInt16(data[idx++]);
			cCenterPos.Set(data, ref idx);
//			cCenterPos.X = (float)Convert.ToDouble(data[idx++]);
//			cCenterPos.Y = (float)Convert.ToDouble(data[idx++]);
		}
		
	}
	
	public UInt16 u2ID;
	public string sFieldModelName;
	public UInt32 u4PhaseLimitTime;
	public PosAndDir cCrewStart;
	public UInt16 cCrewStartPosID;
	public PosAndDir cCrewEnd;
	public PosAndDir cEnemyStart;
	public UInt16 cEnemyStartPosID;
	public Pos cPortalPos;
	
	public Byte u1MonsterGroupCount;
	public MonsterGroup[] acMonsterGroup;
	public MonsterGroupPos[] acMonsterGroupPosInfo;

	public string sFieldName;
	
	public FieldInfo()
	{
	}
	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment
		sFieldModelName = cols[idx++];
		u4PhaseLimitTime = Convert.ToUInt32( cols[idx++] );
		cCrewStart.Set(cols, ref idx);
		cCrewEnd.Set(cols, ref idx);
		cPortalPos.Set(cols, ref idx);
		u1MonsterGroupCount = Convert.ToByte(cols[idx++]);
		acMonsterGroup = new MonsterGroup[u1MonsterGroupCount];
		
		acMonsterGroupPosInfo = new MonsterGroupPos[u1MonsterGroupCount];
		for (Byte i = 0; i < u1MonsterGroupCount; i++)
		{
			acMonsterGroupPosInfo[i].Set(cols, ref idx);
		}
		
		return u2ID;
	}
	public UInt16 SetBattleField(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment
		sFieldModelName = cols[idx++];
		u4PhaseLimitTime = Convert.ToUInt32( cols[idx++] );
		cCrewStart.Set(cols, ref idx);
		cCrewStartPosID = Convert.ToUInt16( cols[idx++] );
		cCrewEnd.Set(cols, ref idx);
		cEnemyStart.Set(cols, ref idx);
		cEnemyStartPosID = Convert.ToUInt16( cols[idx++] );
		sFieldName = cols[idx++];
		
		return u2ID;
	}

	public void addMonsterGroup(string[] cols)
	{
		UInt16 idx = 1; // 0 = FieldID
		idx++;	// comment
		Byte groupNum = Convert.ToByte(cols[idx]);

		acMonsterGroup[groupNum-1].Set(this, cols, u1MonsterGroupCount, ref idx);

	}
}

