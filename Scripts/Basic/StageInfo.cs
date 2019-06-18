using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public struct PosAndDir
{
	public float X;
	public float Y;
	public float Dir;
	
	public void Set(string[] data, ref UInt16 idx)
	{
		//		DebugMgr.Log("POS SET : " + data[idx]);
		//		DebugMgr.Log("POS SET : " + data[idx+1]);
		X = (float)Convert.ToDouble(data[idx++]);
		Y = (float)Convert.ToDouble(data[idx++]);
		Dir = (float)Convert.ToDouble(data[idx++]);
	}
}

public struct Pos
{
	public float X;
	public float Y;
	
	public void Set(string[] data, ref UInt16 idx)
	{
//		DebugMgr.Log("POS SET : " + data[idx]);
//		DebugMgr.Log("POS SET : " + data[idx+1]);
		X = (float)Convert.ToDouble(data[idx++]);
		Y = (float)Convert.ToDouble(data[idx++]);
	}
}

public class PositionInfo
{
	public UInt16 u2ID;
	public Byte u1Count;
	public Pos[] cPos;
	
	public PositionInfo()
	{
	}
	
	public void Set(string[] cols, ref UInt16 idx)
	{
		u2ID = Convert.ToUInt16(cols[idx++]);
//		idx++;
//		u1Count = Convert.ToByte(cols[idx++]);
		cPos = new Pos[(int)u1Count];
		for (Byte i = 0; i < u1Count; i++)
		{
			cPos[i].X = (float)Convert.ToDouble(cols[idx++]);
			cPos[i].Y = (float)Convert.ToDouble(cols[idx++]);
		}
	}
}


public struct RewardItem
{
	public bool bInit;
	public Goods cRewards;
	public Byte u1MonsterType;
	public UInt16 u2Probability;
}

public class StageInfo
{
	public const int MAX_MONSTER_IN_GROUP = 10;
	public const int MAX_REWARD_COUNT = 12;
	public const int MAX_EVENT_REWARD_COUNT = 3;
	public const int CONFIRM_REWORD_COUNT = 2;	// 확정 보상 갯수

	//DATA
	public struct PhaseInfo
	{
		public Byte u1PhaseNum;
		public UInt16 u2FieldID;
		public Byte u1Environment;
		public Byte u1SkyBox;

		public FieldInfo getField()
		{
			return StageInfoMgr.Instance.GetFieldInfo(u2FieldID);
		}
	}
	public struct Forest
	{
		public const Byte ELEMENT_ALL = 5;
		public const Byte BASE_ELEMENT = 2;
		public const Byte MAX_LANE = 3;

		public Byte u1StageRepeatCount;
		public Goods cChargeGoods;
		public UInt32 u4ChargeGoodsAddCost;
		public UInt32 u4ChargeGoodsAddCostMax;
		public Byte u1ChargeCountMax;
		// 2016. 09. 20 jy 
		// 탐색의 숲 오픈된 길은 서버에서 받는다
		//const int OPEN_COUNT = 7;
		//public Byte[] au1OpenElement;

		public void Set(string[] cols, ref int idx)
		{
			u1StageRepeatCount = Convert.ToByte(cols[idx++]);
			cChargeGoods = new Goods();
			cChargeGoods.u1Type = Convert.ToByte(cols[idx++]);
			cChargeGoods.u2ID = Convert.ToUInt16(cols[idx++]);
			cChargeGoods.u4Count = Convert.ToUInt32(cols[idx++]);
			u4ChargeGoodsAddCost = Convert.ToUInt32(cols[idx++]);
			u4ChargeGoodsAddCostMax = Convert.ToUInt32(cols[idx++]);
			u1ChargeCountMax = Convert.ToByte(cols[idx++]);
			// 2016. 09. 20 jy 
			// 탐색의 숲 오픈된 길은 서버에서 받는다
			//au1OpenElement = new byte[OPEN_COUNT];
			//for(int i=1; i<OPEN_COUNT; i++)
			//{
			//	au1OpenElement[i] = Convert.ToByte(cols[idx++]);
			//}
			//au1OpenElement[0] = Convert.ToByte(cols[idx++]);// 일요일이 맨 뒤 나와서 읽는 순서를 바꾼다.
		}
	}

	// Data
	public UInt16 u2ID;
	public string sName;
	public UInt16 u2ChapterID;
	public Byte u1ForestElement;
	public Byte u1StagePos;
	public Byte u1BossType;
	public Byte u1StageNum;
	public Byte u1PhaseCount;
	public PhaseInfo[] acPhases;
	public UInt32[] arrGetExp;
	public Goods[] acGetGoods;
	public Byte u1RecommandElement;
	public UInt32 u4ExploreTime;
	public Byte[] arrRecommandLevel;
	public Byte[] arrRewardItemNum;
	public Byte[] arrRewardEventItemNum;
	public List<RewardItem>[] RewardItems;
	public string stageIconPath;		
	public string stageMiniIconPath;
    public int stagePosX;
    public int stagePosY;
    public UInt16[] smithID;
	public string[] recommendEquipStringKey;
    public int[] recommendEquipPartsCountKey;

    public UInt16 u2ActID;
	public Goods[] arrRepeatGoods;	// 반복 전투 소모 재화
	public string sAnalyticsEventCode;

	public UInt16[] au2GuideID;
    
    //서버 데이터
    public ActInfo actInfo = null;
    public ChapterInfo chapterInfo = null;
	public Byte clearState; //스테이지 오픈 상태값
    public bool IsFirstStageInChapter = false;
    public bool IsLastStageInChapter = false;
    // 챕터나 액트와는 다르게 다른 챕터의 스테이지와도 연결한다. 따라서 오픈 조건은 챕터의 조건을 확인해야 한다.
    public StageInfo prevStage = null;
    public StageInfo nextStage = null;

	// 16.06.20 jy
	// 탐색의 숲 입장 가능 횟수 맵별 -> 난이도 별로 수정 되면서
	// 티켓정보를 StageInfoMgr 에서만 관리 함게 되어 주석 처리
	//public ForestData forestData;
	public UInt16 Set(string[] cols)
	{
		int i;
		UInt16 idx = 0;
		Byte u1RewardType;
		UInt16 u2RewardID;

		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++; // comment
		sName = cols[idx++];
		u2ChapterID = Convert.ToUInt16(cols[idx++]);
		StageInfoMgr.Instance.dicChapterData.TryGetValue(u2ChapterID, out chapterInfo);
		u1ForestElement = Convert.ToByte(cols[idx++]);
		u1StageNum = Convert.ToByte(cols[idx++]);
		u1StagePos = Convert.ToByte(cols[idx++]);
		u1BossType = Convert.ToByte(cols[idx++]); 		
		u1PhaseCount = Convert.ToByte(cols[idx++]);
		acPhases = new PhaseInfo[u1PhaseCount];
		for(i = 0; i < 2; i++)
		{
			if (i < u1PhaseCount)
			{
				acPhases[i].u1PhaseNum = Convert.ToByte(i+1);
				acPhases[i].u2FieldID = Convert.ToUInt16(cols[idx++]);
				acPhases[i].u1Environment = Convert.ToByte(cols[idx++]);
				acPhases[i].u1SkyBox = Convert.ToByte(cols[idx++]);
			}
			else
			{
				idx++;
				idx++;
				idx++;
			}
		}

		arrGetExp = new UInt32[Server.ConstDef.MaxDifficult];
		acGetGoods = new Goods[Server.ConstDef.MaxDifficult];
		arrRecommandLevel = new Byte[Server.ConstDef.MaxDifficult];
		arrRewardItemNum = new Byte[Server.ConstDef.MaxDifficult];
		arrRewardEventItemNum = new Byte[Server.ConstDef.MaxDifficult];
		RewardItems = new List<RewardItem>[Server.ConstDef.MaxDifficult];

		for(i=0; i<Server.ConstDef.MaxDifficult; i++)
		{
			arrGetExp[i] = Convert.ToUInt32(cols[idx++]);
		}
	
		u1RewardType = Convert.ToByte(cols[idx++]);
		u2RewardID = Convert.ToUInt16(cols[idx++]);

		for(i=0; i<Server.ConstDef.MaxDifficult; i++)
		{
			acGetGoods[i] = new Goods(u1RewardType, u2RewardID, Convert.ToUInt32(cols[idx++]));
		}
		
		u1RecommandElement = Convert.ToByte(cols[idx++]);
		u4ExploreTime = Convert.ToUInt32(cols[idx++]);

		for(i=0; i<Server.ConstDef.MaxDifficult; i++)
		{
			arrRecommandLevel[i] = Convert.ToByte(cols[idx++]);
		}
			
		for (int h = 0; h < Server.ConstDef.MaxDifficult; h++) 
		{
			arrRewardItemNum[h] = Convert.ToByte (cols [idx++]);
			RewardItems [h] = new List<RewardItem> ();

			for (i = 0; i < MAX_REWARD_COUNT; i++) {
				RewardItem rewardItem = new RewardItem ();
				rewardItem.bInit = true;
				u1RewardType = Convert.ToByte (cols [idx++]);
				u2RewardID = Convert.ToUInt16 (cols [idx++]);

				rewardItem.cRewards  = new Goods (u1RewardType, u2RewardID, Convert.ToUInt32 (cols [idx++]));

				rewardItem.u1MonsterType = Convert.ToByte (cols [idx++]);
				rewardItem.u2Probability = Convert.ToUInt16 (cols [idx++]);

				RewardItems[h].Add (rewardItem);
			}
		}
		
		stageIconPath = cols[idx++].Replace("/r", "");
		stageMiniIconPath = cols[idx++].Replace("/r", "");
        stagePosX = Convert.ToInt32(cols[idx++]);
        stagePosY = Convert.ToInt32(cols[idx++]);

        smithID = new UInt16[Server.ConstDef.MaxDifficult];
		recommendEquipStringKey = new string[Server.ConstDef.MaxDifficult];
        recommendEquipPartsCountKey = new int[Server.ConstDef.MaxDifficult];
        for (int j = 0; j < Server.ConstDef.MaxDifficult; ++j)
		{
			smithID[j] = Convert.ToUInt16(cols[idx++]);
			recommendEquipStringKey[j] = cols[idx++];
			recommendEquipPartsCountKey[j] = Convert.ToInt32(cols[idx++]);
        }

		arrRepeatGoods = new Goods[Server.ConstDef.MaxDifficult];
		for(int j = 0; j < Server.ConstDef.MaxDifficult; ++j)
		{
			arrRepeatGoods[j] = new Goods(Convert.ToByte(cols[idx++]),
				Convert.ToUInt16(cols[idx++]),
				Convert.ToUInt32 (cols [idx++]));
		}

		sAnalyticsEventCode = cols [idx++];

		for (int h = 0; h < Server.ConstDef.MaxDifficult; h++) 
		{
			arrRewardEventItemNum[h] = Convert.ToByte (cols [idx++]);

			for (i = 0; i < MAX_REWARD_COUNT; i++) {
				RewardItem rewardItem = new RewardItem ();
				rewardItem.bInit = true;
				u1RewardType = Convert.ToByte (cols [idx++]);
				u2RewardID = Convert.ToUInt16 (cols [idx++]);

				rewardItem.cRewards  = new Goods (u1RewardType, u2RewardID, Convert.ToUInt32 (cols [idx++]));

				rewardItem.u1MonsterType = Convert.ToByte (cols [idx++]);
				rewardItem.u2Probability = Convert.ToUInt16 (cols [idx++]);

				RewardItems[h].Add (rewardItem);
			}
		}

		au2GuideID = new UInt16[Server.ConstDef.MaxDifficult];
		for (int h = 0; h < Server.ConstDef.MaxDifficult; h++) 
		{
			au2GuideID [h] = Convert.ToUInt16 (cols [idx++]);
		}

		//Add
		if(chapterInfo != null)
        {
			u2ActID = chapterInfo.u2ActID;
			actInfo = chapterInfo.actInfo;
        }
		
		return u2ID;
	}

    public bool IsClear(byte difficulty)
    {
        switch (difficulty)
        {
            case 1:
        		if ((clearState & 0x03) == 0) return false;
                break;
            case 2:
        		if ((clearState & 0x0C) == 0) return false;
                break;
            case 3:
        		if ((clearState & 0x30) == 0) return false;
                break;
        }
        return true;
    }

    public bool IsOpen(byte difficulty)
    {
        // 이전 난이도 확인
        switch (difficulty)
        {
            case 2:
        		if ((clearState & 0x03) == 0) return false;
                break;
            case 3:
        		if ((clearState & 0x0C) == 0) return false;
                break;
        }
        // 챕터 및 액트가 열릴 수 있는 상황인지 확인
		if (!chapterInfo.CheckChapterOpen(difficulty)) return false;
        // 챕터의 첫 스테이지가 아니라면 이전 스테이지 완료 확인
        if (!IsFirstStageInChapter)
        {
            if (!prevStage.IsClear(difficulty)) return false;
        }
        return true;
    }

	public RewardItem GetRewardInfoByIndex(Byte difficult, Byte u1Index)
	{
		if (u1Index < RewardItems[difficult].Count) 
			return RewardItems[difficult][u1Index];
		
		return new RewardItem() { bInit = false };
	}

	public RewardItem GetTowerRewordInfoByIndex(Byte difficult, Byte u1Index)
	{
		int index = difficult * 3;
		//if(chapterInfo.acTowerRewards[index + u1Index].u2ID != 0)
		return new RewardItem() { cRewards = chapterInfo.acTowerRewards[index + u1Index] };

		//return new RewardItem() { bInit = false };
	}

	public int CheckRewardInStage(ushort rewardID){
		for(int i=0; i<Server.ConstDef.MaxDifficult; i++){
			for(int j=0; j<RewardItems[i].Count; j++){
				if(RewardItems[i][j].cRewards.u1Type != 0){
					if(RewardItems[i][j].cRewards.u2ID == rewardID){
						return (i+1);
					}
				}
			}
		}
		return 0;
	}

	public FieldInfo getField(Byte phaseNum)
	{
		return StageInfoMgr.Instance.GetFieldInfo(acPhases[phaseNum].u2FieldID);
	}

	public FieldInfo getFieldInfo()
	{
		return StageInfoMgr.Instance.GetFieldInfo(acPhases[currentPhaseNum].u2FieldID);
	}

	Byte currentPhaseNum;
	public void nextPhase()
	{
		currentPhaseNum++;
	}
		
	public ActInfo GetActInfo()
	{
		return StageInfoMgr.Instance.dicActData[u2ActID];
	}	
	
	public ChapterInfo GetChapterInfo()
	{
		return StageInfoMgr.Instance.dicChapterData[u2ChapterID];
	}

    public Goods RepeatGoods()
    {
        return arrRepeatGoods[Legion.Instance.SelectedDifficult - 1];
    }

	public ClassInfo GetBoss()
	{
		for (int a = 0; a < acPhases.Length; a++) {
			FieldInfo cCurrentBattleField = acPhases [a].getField ();
			for (int iGroup = 0; iGroup < cCurrentBattleField.acMonsterGroup.Length; iGroup++) {
				for (Byte i = 0; i < cCurrentBattleField.acMonsterGroup [iGroup].u1SubGroupNum; i++) {
					ushort monID = cCurrentBattleField.acMonsterGroup [iGroup].acMonsterInfo [i].u2MonsterID;
					if (ClassInfoMgr.Instance.GetInfo (monID).u1MonsterType == 2) {
						return ClassInfoMgr.Instance.GetInfo (monID);
					}
				}
			}
		}

		return null;
	}

	// 레벨 제한 체크
	public int CheckRestrictLevel(int selectDifficult)
	{
		int restrictLevel = 0;
		// 제한 레벨을 구한다
		if(chapterInfo != null)
			restrictLevel = arrRecommandLevel[selectDifficult] - chapterInfo.u1RestrictUnderLevel[selectDifficult];
		else
			restrictLevel = arrRecommandLevel[selectDifficult] - GetChapterInfo().u1RestrictUnderLevel[selectDifficult];

		// 1보다 작으면 크루의 레벨을 확인할 것도 없이 진입 가능
		if(restrictLevel < 2)
			return 1;

		Crew crew = Legion.Instance.cBestCrew;
		for(int i = 0; i < crew.acLocation.Length; ++i)
		{
			if(crew.acLocation[i] == null)
				continue;

			if(crew.acLocation[i].cLevel.u2Level >= restrictLevel)
				return 1;
		}

		return restrictLevel;
	}
}

public class GuideInfo
{
	public const int MAX_RECOMMEND_SKILL = 4;
	public const int MAX_NEED_PARTS = 3;

	// Data
	public UInt16 u2ID;
	public string sDesc;
	public UInt16[] au2RecomSkill;
	public UInt16[] au2NeedParts;

	public UInt16 Set(string[] cols)
	{
		int i;
		UInt16 idx = 0;

		u2ID = Convert.ToUInt16 (cols [idx++]);
		idx++; // comment
		sDesc = cols [idx++];

		au2RecomSkill = new UInt16[MAX_RECOMMEND_SKILL];
		for (i = 0; i < MAX_RECOMMEND_SKILL; i++) {
			au2RecomSkill [i] = Convert.ToUInt16 (cols [idx++]);
		}

		au2NeedParts = new UInt16[MAX_NEED_PARTS];
		for (i = 0; i < MAX_NEED_PARTS; i++) {
			au2NeedParts [i] = Convert.ToUInt16 (cols [idx++]);
		}

		return u2ID;
	}
}