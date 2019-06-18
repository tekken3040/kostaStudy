using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public class DropBox
{
	public RewardItem item;

//	public DropBox(UInt16 u2ItemID, UInt16 u2Level, Byte _u1Star, UInt16 u2Count, Byte u1Type)
//	{
//		item = new RewardItem {
//				u2ID = u2ItemID,
//				u2Level = (Byte)u2Level,
//				u1Star = _u1Star,
//				u2Num = u2Count,
//				u1MonsterType = u1Type
//		};
//	}

	public DropBox(RewardItem _item, Int32 u4Count)
	{
		item = _item;
		item.cRewards.u4Count = (uint)u4Count;
	}

	public Byte Type
	{
		get { return item.u1MonsterType; }
	}
}

public class Reward
{
	int[] aPhaseCounts = null;
	Byte u1CallPhase = 0;
	StageInfo info;
    public UInt16 stageID
    {
        get { return (info == null ? (UInt16)0 : info.u2ID); }
    }
    Byte u1Difficulty;
    public Byte difficulty
    {
        get { return u1Difficulty; }
    }
	List<DropBox> lstAll;
	List<DropBox> lstNormal;
	List<DropBox> lstBoss;
	public Byte u1AliveCharCnt = 0;
	public Reward(StageInfo _info, Byte _u1Difficulty)
	{
		info = _info;
        u1Difficulty = _u1Difficulty;
		lstAll = new List<DropBox>();
		lstNormal = new List<DropBox>();
		lstBoss = new List<DropBox>();
	}
	public void AddNewRewardByIndex(Byte u1Index, Int32 u4count)
	{
		DropBox box;
		box  = new DropBox(info.GetRewardInfoByIndex((Byte)(u1Difficulty-1), u1Index), u4count);

		if(box.item.cRewards == null)
			return;
		
		lstAll.Add(box);
		switch (box.Type)
		{
		case 1:lstNormal.Add (box); break;
		case 2:lstBoss.Add (box); break;
		}
	}

	public void AddTowerDispatchRewardByIndex(Byte u1Index, Int32 u4count)
	{
		if(info.actInfo.u1Mode != ActInfo.ACT_TYPE.TOWER)
			return;

		DropBox box;
		box  = new DropBox(info.GetTowerRewordInfoByIndex((Byte)(u1Difficulty-1), u1Index), u4count);

		if(box.item.cRewards == null)
			return;

		lstAll.Add(box);
		switch (box.Type)
		{
		case 1:lstNormal.Add (box); break;
		case 2:lstBoss.Add (box); break;
		}
	}
	public void SetBoxOutline(StageInfo cBattleField)
	{
		int count = lstNormal.Count;
		int phase = 0;

		u1CallPhase = 0;

		for (Byte i=0; i<cBattleField.u1PhaseCount; i++) {
			phase += (int)cBattleField.getField(i).u1MonsterGroupCount;
		}

		aPhaseCounts = new int[phase];
		for (int i = 0; i < count; i++)
		{
			phase--;
			aPhaseCounts[phase]++;
			if (phase == 0) phase = aPhaseCounts.Length;
		}
	}

	public void SetBoxAtMonster(BattleCrew crew)
	{
		int[] aMonsterCounts = new int[crew.acCharacters.Length];
		int index = 0;
		int i;
		int j;

//		DebugMgr.LogError(u1CallPhase+"/"+aPhaseCounts.Length);

		for (i = 0; i < aPhaseCounts[u1CallPhase]; i++)
		{
//			DebugMgr.LogError(aMonsterCounts.Length+"/"+index);
			aMonsterCounts[index++]++;
			if (index == crew.acCharacters.Length) index = 0;
		}

		for (i = 0; i < crew.acCharacters.Length; i++)
		{
			if (crew.acCharacters[i].cCharacter.cClass.u1MonsterType == 2 && lstBoss.Count > 0)
			{
				crew.acCharacters[i].acDropBoxs = new DropBox[aMonsterCounts[i] + lstBoss.Count];
				for (j = 0; j < lstBoss.Count; j++)
				{
					crew.acCharacters[i].acDropBoxs[aMonsterCounts[i] + j] = lstBoss[j];
				}
				lstBoss.Clear();
			}
			else
			{
				crew.acCharacters[i].acDropBoxs = new DropBox[aMonsterCounts[i]];
			}

			for (j = 0; j < aMonsterCounts[i];j++)
			{
				DropBox box = lstNormal[UnityEngine.Random.Range(0,lstNormal.Count - 1)];
				crew.acCharacters[i].acDropBoxs[j] = box;
				lstNormal.Remove(box);
			}
		}
		u1CallPhase++;
	}
	 
	public RewardItem[] GetReward()
	{
		RewardItem[] retValue = new RewardItem[lstAll.Count];
		int index = 0;
		foreach (DropBox box in lstAll)
		{
			retValue[index++] = box.item;
		}

		return retValue;
	}

	public void PutIntoBag(Crew crew)
	{
		for (int i = 0; i < lstAll.Count; i++) {
			Legion.Instance.AddGoods(lstAll[i].item.cRewards);
		}
	}
}
