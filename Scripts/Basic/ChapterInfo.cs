using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChapterInfo{
	
	public const int MAX_REWARD_ROTATION = 3;
    public const int MAX_REWARD_TOWER = 9;

	public UInt16 u2ID;
	public string strName;
	public string strShortcutName;
	public Byte u1Number;
	public UInt16 u2ActID;

	public Byte u1OpenCondition;
	public Byte u1OpenConditionValue;

	public Goods[] acPlayGoods;

	public Goods cPlayPayBack;

	public Goods cReturnGoods;

	public Byte[] u1RestrictUnderLevel;

	public Byte u1RepeatCount;
	public Byte u1MaxRepeatCount;

	public Goods[] acFirstRewards;
	public Goods[][] acRepeatRewards;
	public Byte u1StarCount;
	public Goods[] acStarRewards;
    public Goods[] acTowerRewards;

    public int m_nDirectionType;
    public string m_strBackGroundName;
	
    //서버에서 관리되는 데이터

    public ActInfo actInfo = null;
	public List<UInt16> lstStageID = new List<UInt16>();
	public Byte openState;
	public Byte openStateByStage;
	public Byte[] repeatCount;
	public Byte[] starCount;
	public Byte[] repeatType;
    public bool IsLastChapterInAct = false;
	public StageInfo[] lastStage = new StageInfo[StageInfo.Forest.ELEMENT_ALL];
    public ChapterInfo prevChapter = null;
    public ChapterInfo nextChapter = null;


	public UInt16 Set(string[] cols)
	{
		lstStageID.Clear();
		
		UInt16 idx = 0;
		Byte rewardType = 0;
		UInt16 rewardID = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++; // comment
		strName = cols[idx++];
		u1Number = Convert.ToByte(cols[idx++]);
		u2ActID = Convert.ToUInt16(cols[idx++]);
		StageInfoMgr.Instance.dicActData.TryGetValue(u2ActID, out actInfo);
		
		u1OpenCondition = Convert.ToByte(cols[idx++]);
		u1OpenConditionValue = Convert.ToByte(cols[idx++]);

		rewardType = Convert.ToByte(cols[idx++]);
		rewardID = Convert.ToUInt16(cols[idx++]);

		acPlayGoods = new Goods[Server.ConstDef.MaxDifficult];

		for(int i=0; i<acPlayGoods.Length; i++)
		{
			acPlayGoods[i] = new Goods(rewardType, rewardID, Convert.ToUInt32(cols[idx++]));
		}
		// 열쇠소모시 지급 재화
		cPlayPayBack = new Goods(Convert.ToByte(cols[idx++]), Convert.ToUInt16(cols[idx++]), Convert.ToUInt32(cols[idx++]));
			
		cReturnGoods = new Goods(cols, ref idx);

		// 난이도별 레벨 설정 값
		u1RestrictUnderLevel = new Byte[Server.ConstDef.MaxDifficult];
		for(int i = 0; i < u1RestrictUnderLevel.Length; ++i)
		{
			u1RestrictUnderLevel[i] = Convert.ToByte(cols[idx++]);
		}

		u1RepeatCount = Convert.ToByte(cols[idx++]);
		u1MaxRepeatCount = Convert.ToByte(cols[idx++]);		
			
		rewardType = Convert.ToByte(cols[idx++]);
		rewardID = Convert.ToUInt16(cols[idx++]);

		acFirstRewards = new Goods[Server.ConstDef.MaxDifficult];
		for(int i=0; i<acFirstRewards.Length; i++)
		{
			acFirstRewards[i] = new Goods(rewardType, rewardID, Convert.ToUInt32(cols[idx++]));
		}

		acRepeatRewards = new Goods[MAX_REWARD_ROTATION][];
		for(int i=0; i<MAX_REWARD_ROTATION; i++)
		{
			rewardType = Convert.ToByte(cols[idx++]);
			rewardID = Convert.ToUInt16(cols[idx++]);
			acRepeatRewards[i] = new Goods[Server.ConstDef.MaxDifficult];
			for(int j=0; j<Server.ConstDef.MaxDifficult; j++)
			{
				acRepeatRewards[i][j] = new Goods(rewardType, rewardID, Convert.ToUInt32(cols[idx++]));
			}
		}

		u1StarCount = Convert.ToByte(cols[idx++]);

		rewardType = Convert.ToByte(cols[idx++]);
		rewardID = Convert.ToUInt16(cols[idx++]);
		
		acStarRewards = new Goods[Server.ConstDef.MaxDifficult];
		
		for(int i=0; i<acStarRewards.Length; i++)
		{
			acStarRewards[i] = new Goods(rewardType, rewardID, Convert.ToUInt32(cols[idx++]));
		}

        acTowerRewards = new Goods[MAX_REWARD_TOWER];
        for (int i = 0; i < MAX_REWARD_TOWER; i++)
        {
            acTowerRewards[i] = new Goods(cols, ref idx);
        }

        m_nDirectionType = Convert.ToInt32(cols[idx++]);
        m_strBackGroundName = cols[idx++];
		repeatCount = new Byte[Server.ConstDef.MaxDifficult];
		starCount = new Byte[Server.ConstDef.MaxDifficult];		
		repeatType = new Byte[Server.ConstDef.MaxDifficult];
		strShortcutName = cols[idx++];
		
		return u2ID;
	}
	
	public ActInfo GetActInfo()
	{
		return StageInfoMgr.Instance.dicActData[u2ActID];
	}

    public void SetLink(StageInfo[] prevStage)
    {
		for (int i = 0; i < StageInfo.Forest.ELEMENT_ALL; i++)
            lastStage[i] = null;
        foreach (UInt16 stageID in lstStageID)
        {
            StageInfo info = StageInfoMgr.Instance.dicStageData[stageID];
            if (prevStage[info.u1ForestElement] != null)
            {
                info.prevStage = prevStage[info.u1ForestElement];
                prevStage[info.u1ForestElement].nextStage = info;
            }
            prevStage[info.u1ForestElement] = info;
            if (lastStage[info.u1ForestElement] == null) info.IsFirstStageInChapter =  true;
            lastStage[info.u1ForestElement] = info;
        }
		for (int i = 0; i < StageInfo.Forest.ELEMENT_ALL; i++)
            if (lastStage[i] != null) lastStage[i].IsLastStageInChapter = true;
    }

    public bool IsClear(StageInfo exceptStage, Byte u1Difficulty)
    {
		for (int i = 0; i < StageInfo.Forest.ELEMENT_ALL; i++)
        {
            if (lastStage[i] == null || lastStage[i] == exceptStage) continue;
            switch (u1Difficulty)
            {
                case 1:
                    if ((lastStage[i].clearState & 0x03) == 0) return false;
                    break;
                case 2:
                    if ((lastStage[i].clearState & 0x0C) == 0) return false;
                    break;
                case 3:
                    if ((lastStage[i].clearState & 0x30) == 0) return false;
                    break;
            }
        }
        return true;
    }

    public bool IsClear(Byte u1Difficulty, Byte u1Element)
    {
        if (lastStage[u1Element] == null) return true;
        switch (u1Difficulty)
        {
            case 1:
                if ((lastStage[u1Element].clearState & 0x03) == 0) return false;
                break;
            case 2:
                if ((lastStage[u1Element].clearState & 0x0C) == 0) return false;
                break;
            case 3:
                if ((lastStage[u1Element].clearState & 0x30) == 0) return false;
                break;
        }
        return true;
    }

    // 해당 챕터를 플레이 하는데 소비되는 재화
	public Goods GetConsumeGoods(){
		return acPlayGoods[Legion.Instance.SelectedDifficult - 1];
	}
	
    //챕터 열림 정보 업데이트
	public void UpdateChpaterOpen()
    {
		switch (u1OpenCondition)
        {
        case 1: // if always
			openState = openStateByStage;
            break;
        case 2: // if check level
			if (Legion.Instance.TopLevel >= u1OpenConditionValue)
				openState = openStateByStage;
            else 
				openState = 0;
            break;
        case 3: // if check previous chapter
		
			if (prevChapter != null)
            {
                if (prevChapter.openState > 1)
                {
                    openState = openStateByStage;
				    if(prevChapter.openState < openState)
					    openState = prevChapter.openState;
                }
                else
				    openState = 0;
            }
            break;
        }
        if (IsLastChapterInAct)
        {
            actInfo.openState = openState;
        }
    }
	
    //조건에 따른 챕터 열림 확인
	public bool CheckChapterOpen(byte difficult = 0)
	{
		bool chapterOpen = false;
		if(difficult == 0)
			difficult = Legion.Instance.SelectedDifficult;
		
		if(!StageInfoMgr.Instance.CheckUnlockDifficult(difficult, actInfo.u2ID))
			chapterOpen = false;
        else if (!actInfo.CheckActOpen())
			chapterOpen = false;
        else
		{
			switch (u1OpenCondition)
			{
				case 1: // if always
				{
					chapterOpen = true;
				}
				break;
				case 2: // if check level
				{
					if (Legion.Instance.TopLevel >= u1OpenConditionValue)
						chapterOpen = true;
					else
						chapterOpen = false;
				}
				break;
				case 3: // if check previous chapter
				{
					if (prevChapter != null && prevChapter.openState > difficult)
						chapterOpen = true;
					else
						chapterOpen = false;
				}
				break;
			}
		}
		
		return chapterOpen;
	}
    
    //느낌표 처리를 위한 안받은 보상이 있는지 여부 확인
    public bool RewardEnable()
    {
        if(!CheckChapterOpen())
            return false;
        
        for(int i=0; i<Server.ConstDef.MaxDifficult; i++)
        {
            if(repeatCount[i] >= u1RepeatCount)
            {
                return true;
            }
        }
                      
        
        for(int i=0; i<Server.ConstDef.MaxDifficult; i++)
        {
            bool isGet = false;
            
            if(starCount[i] == 0)
            {
                for(int j=0; j<lstStageID.Count; j++)
                {
                    if(StageInfoMgr.Instance.GetStageClearStar(lstStageID[j]) != 0)
                    {
                        isGet = true;
                        break;
                    }
                }
            }
            
            if(starCount[i] >= u1StarCount && !isGet)
            {
                return true;
            }
        }                
        
        return false;
    }
}
