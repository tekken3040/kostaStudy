using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DispatchTimeInfo
{
    public int lvGap;
    public UInt32 u2AddIime;        // 추가 시간
    public UInt32 u2AddPriceRatio;  // 추가 금액 비율

    public void Set(string[] col, ref int idx)
    {
        lvGap = int.Parse(col[idx++]);
        u2AddIime = UInt32.Parse(col[idx++]);
        u2AddPriceRatio = UInt32.Parse(col[idx++]);
    }
}

public class StageInfoMgr : Singleton<StageInfoMgr>
{
	public struct ForestTicketData
	{
		public Byte[] u1TicketCount;
		public Byte[] au1ChargedTicketCount;
	}

	public Dictionary<UInt16, ActInfo> dicActData;
	public Dictionary<UInt16, ChapterInfo> dicChapterData;
	public Dictionary<UInt16, StageInfo> dicStageData;
	//public Dictionary<int, UInt32> dicDispatchTime;
    public List<DispatchTimeInfo> dispatchTimeList;
    public Dictionary<UInt16, Pos[]> dicPosData;
	public Dictionary<UInt16, GuideInfo> dicGuideData;
	public Dictionary<UInt16, FieldInfo> dicFieldData;
	public Dictionary<int, FogInfo> dicFogData;
	public Byte repeatReward = 100;
	public UInt32 dispatchTime;

	public int LastPlayStage = -1;
	public int ShortCutChapter = -1;

    public Goods RepeatTargetItem;
    public UInt32 u4CurTargetItemCount;

    public UInt64 u8AddedExp = 0;
    public UInt32 u4TotalGold = 0;
    public Byte u1EventIDCount = 0;
    public UInt16[] arrEventID;
    public UInt32 u4PrevTotalGold = 0;
    private bool reloadEvent = false;
    public bool bReloadEvent
    {
        get { return reloadEvent; }
        set { reloadEvent = value; }
    }
    private bool loadedInfo = false;
	public bool LoadedInfo 
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}

	public ForestTicketData forestTicketData;
	private Byte _openForestElement;
	public Byte OpenForestElement
	{ 
		get { return _openForestElement; }
		set { _openForestElement = value; }
	}

    private Byte _openBossRush;
    public Byte OpenBossRush
    {
        get
        {
            return _openBossRush;
        }
        set
        {
            _openBossRush = value;
        }
    }

	private void AddActInfo (string[] cols)
	{
		if (cols == null)
			return;
		
		ActInfo info = new ActInfo ();
		info.Set (cols);
		dicActData.Add (info.u2ID, info);
	}

	private void AddChapterInfo (string[] cols)
	{
		if (cols == null)
			return;
		
		ChapterInfo info = new ChapterInfo ();
		info.Set (cols);
		dicChapterData.Add (info.u2ID, info);
		if (dicActData.ContainsKey (info.u2ActID))
			dicActData [info.u2ActID].lstChapterID.Add (info.u2ID);
	}

	private void AddStageInfo (string[] cols)
	{
		if (cols == null) {
			loadedInfo = true;
			return;
		}
		
		StageInfo info = new StageInfo ();
		info.Set (cols);
		dicStageData.Add (info.u2ID, info);
		if (dicChapterData.ContainsKey (info.u2ChapterID))
			dicChapterData [info.u2ChapterID].lstStageID.Add (info.u2ID);
	}

	private void AddGuideInfo (string[] cols)
	{
		if (cols == null) {
			loadedInfo = true;
			return;
		}

		GuideInfo info = new GuideInfo ();
		info.Set (cols);
		dicGuideData.Add (info.u2ID, info);
	}

	private void AddDispatchTime (string[] cols)
	{
		if (cols == null) {
			loadedInfo = true;
			return;
		}

        for(int i = 0; i < cols.Length -1;)
        {
            DispatchTimeInfo info = new DispatchTimeInfo();
            info.Set(cols, ref i);

            dispatchTimeList.Add(info);
        }
        // 이전 코드
		//for (int i = 0; i < cols.Length - 1; i++)
        //{ 
        //    dicDispatchTime.Add (Convert.ToInt32 (cols [i]), Convert.ToUInt32 (cols [i + 1]));
		//	i++;
		//}
	}

	private void AddFieldInfo (string[] cols)
	{
		if (cols == null)
			return;

		FieldInfo info = new FieldInfo ();
		info.Set (cols);
		dicFieldData.Add (info.u2ID, info);
	}

	private void AddBattleFieldInfo (string[] cols)
	{
		if (cols == null)
			return;
		
		FieldInfo info = new FieldInfo ();
		info.SetBattleField (cols);
		dicFieldData.Add (info.u2ID, info);
	}

	private void AddPosInfo (string[] cols)
	{
		if (cols == null)
			return;
		
		Pos[] pos = new Pos[StageInfo.MAX_MONSTER_IN_GROUP];

		int idx = 0;
		UInt16 u2ID = Convert.ToUInt16 (cols [idx++]);
		idx++; // comment
		for (Byte i = 0; i < StageInfo.MAX_MONSTER_IN_GROUP; i++) 
		{
			pos [i].X = (float)Convert.ToDouble (cols [idx++]);
			pos [i].Y = (float)Convert.ToDouble (cols [idx++]);
		}

		dicPosData.Add (u2ID, pos);
	}

	private void AddMonsterInfo (string[] cols)
	{
		if (cols == null)
			return;

		UInt16 id = Convert.ToUInt16 (cols [0]);
		FieldInfo info;
		if (dicFieldData.TryGetValue (id, out info)) {
			info.addMonsterGroup (cols);
		}
	}

	public GuideInfo GetGuideInfo (UInt16 id)
	{
		GuideInfo ret;
		dicGuideData.TryGetValue (id, out ret);
		return ret;
	}

	public Pos[] GetPosInfo (UInt16 id)
	{
		Pos[] ret;
		dicPosData.TryGetValue (id, out ret);
		return ret;
	}

	public FieldInfo GetFieldInfo (UInt16 id)
	{
		FieldInfo ret;
		dicFieldData.TryGetValue (id, out ret);
		return ret;
	}

    public DispatchTimeInfo GetDispatchInfo(int lvGap)
    {
        // 렙 차이에 따른 추가 시간 데이터에서 찾기
        for (int i = 0; i < dispatchTimeList.Count; ++i)
        {
            if (dispatchTimeList[i].lvGap <= lvGap)
            {
                return dispatchTimeList[i];
            }
        }

        return dispatchTimeList.Last();
    }
    public DispatchTimeInfo GetDispatchInfo(TimeSpan time)
    {
        //남은 시간 = 파견 종료 시간 - 현재 시간
        for (int i = 0; i < dispatchTimeList.Count; ++i)
        {
            if(dispatchTimeList[i].u2AddIime >= time.TotalMinutes)
            {
                return dispatchTimeList[i];
            }
        }

        return dispatchTimeList.Last();
    }

	public StageInfo.Forest forest;

	public void AddForestCondition (string[] cols)
	{
		if (cols == null)
			return;

		int idx = 0;

		forest.Set (cols, ref idx);
	}

	public void Init ()
	{
		dicPosData = new Dictionary<UInt16, Pos[]> ();
		DataMgr.Instance.LoadTable (this.AddPosInfo, "Position");

		dicFieldData = new Dictionary<UInt16, FieldInfo> ();
		DataMgr.Instance.LoadTable (this.AddFieldInfo, "Field");
		DataMgr.Instance.LoadTable (this.AddMonsterInfo, "FieldMonster");

		DataMgr.Instance.LoadTable (this.AddBattleFieldInfo, "BattleField");

		dicActData = new Dictionary<ushort, ActInfo> ();
		DataMgr.Instance.LoadTable (this.AddActInfo, "Act");

		dicChapterData = new Dictionary<ushort, ChapterInfo> ();
		DataMgr.Instance.LoadTable (this.AddChapterInfo, "Chapter");

		dicStageData = new Dictionary<UInt16, StageInfo> ();
		DataMgr.Instance.LoadTable (this.AddStageInfo, "Stage");
		// 스테이지 데이타 줄 세우기
		StageInfo[] prevStage = new StageInfo[StageInfo.Forest.ELEMENT_ALL];
		Byte prevActMode = 0;
		int i;
		ActInfo prevAct = null;
		foreach (ActInfo actInfo in dicActData.Values)
		{
			if ((byte)actInfo.u1Mode != prevActMode) 
			{
				prevActMode = (byte)actInfo.u1Mode;
				for (i = 0; i < StageInfo.Forest.ELEMENT_ALL; i++)
					prevStage [i] = null;
				prevAct = null;
			} 
			else if (prevAct != null) 
			{
				actInfo.prevAct = prevAct;
				prevAct.nextAct = actInfo;
			}
			actInfo.SetLink (prevStage);
			prevAct = actInfo;
		}

		dicGuideData = new Dictionary<UInt16, GuideInfo> ();
		DataMgr.Instance.LoadTable (this.AddGuideInfo, "Guide");

		//dicDispatchTime = new Dictionary<int, UInt32> ();
        dispatchTimeList = new List<DispatchTimeInfo>();
        DataMgr.Instance.LoadTable (this.AddDispatchTime, "Dispatch");

		dicFogData = new Dictionary<int, FogInfo> ();
		FogColorData fogDatas = (FogColorData)AssetMgr.Instance.AssetLoad ("SkyBoxs/FogColors.asset", typeof(FogColorData));
        
		DataMgr.Instance.LoadTable (this.AddForestCondition, "Forest");

		//DebugMgr.Log (fogDatas);
		        
		fogDatas.AddInfo ();
	}
    
	//챕터가 열렸는지 계산함
	public void CalculateChapterOpen ()
	{
		//OpenForestElement;//StageInfoMgr.Instance.forest.au1OpenElement [(int)Legion.Instance.ServerTime.DayOfWeek];
		for (int i = 0; i < Server.ConstDef.MaxChapter; i++) {
			UInt16 chapterID = (UInt16)(i + Server.ConstDef.BaseChapterID);
			
			ChapterInfo info;
			if (dicChapterData.TryGetValue (chapterID, out info) == false)
				continue;

			// 스테이지 상태 확인
			byte clearState = 0;
			if (info.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
			{
				if (OpenForestElement == StageInfo.Forest.ELEMENT_ALL) 
				{
					for (int j = StageInfo.Forest.BASE_ELEMENT; j < StageInfo.Forest.ELEMENT_ALL; j++) 
					{
						StageInfo lastStage = info.lastStage [j];
						if (lastStage != null)
							clearState |= lastStage.clearState;
					}
				}
				else 
				{
					StageInfo lastStage = info.lastStage [OpenForestElement];
					if (lastStage != null)
						clearState = lastStage.clearState;
				}
			}
			else
			{
				StageInfo lastStage = info.lastStage [0];
				if (lastStage != null)
					clearState = lastStage.clearState;
			}
			
			if ((clearState & 0x30) > 0)
				info.openStateByStage = 4;
			else if ((clearState & 0x0C) > 0)
				info.openStateByStage = 3;
			else if ((clearState & 0x03) > 0)
				info.openStateByStage = 2;
			else
				info.openStateByStage = 1;

			info.UpdateChpaterOpen ();
		}
	}


	//해당 스테이지를 특정 난이도로 실행할 수 있는 지 확인
	public bool IsOpen (UInt16 stageID, byte difficulty)
	{
		if (difficulty == 0)
			difficulty = 1;
		StageInfo stageInfo = null;
		if (dicStageData.TryGetValue (stageID, out stageInfo))
			return stageInfo.IsOpen (difficulty);
		return false;
	}

	public bool IsOpenForest (UInt16 stageID, byte difficulty, Byte openElement)
	{
		if (difficulty == 0)
			difficulty = 1;
		StageInfo stageInfo = null;
        if(!dicStageData.TryGetValue(stageID, out stageInfo))
        {
            return false;
        }

        if (openElement != StageInfo.Forest.ELEMENT_ALL) {
			if (stageInfo.u1ForestElement == openElement)
			{
				return stageInfo.IsOpen (difficulty);
			} 
		} else {
            // 2016. 08. 23 jy 
            // 이전 스테이지가 없다면 최초의 스테이지이며 오픈했는지만 확인한다
            if (stageInfo.prevStage == null)
            {
                // 최소 난이도 보다 크다면
                if (difficulty > Server.ConstDef.MinDifficult)
                {
                    // 이전 난이도의 챕터의 마지막 스테이지를 클리어 했는지 확인
                    ChapterInfo chapterInfo = null;
                    dicChapterData.TryGetValue(stageInfo.GetActInfo().lstChapterID.Last(), out chapterInfo);
                    if (chapterInfo.lastStage[stageInfo.u1ForestElement] != null)
                    {
                        // 챕터의 마지막 스테이지 클리어 여부 확인
                        if (!chapterInfo.lastStage[stageInfo.u1ForestElement].IsClear((Byte)(difficulty - 1)))
                        {
                            return false;
                        }
                    }
                }
                return stageInfo.IsOpen(difficulty);
            }
            else if(stageInfo.IsOpen (difficulty))
            {
            	// 현재 스테이지가 오픈했는지 확인하고 오픈 되었다면 이전 스테이지가 클리어 되었는지 확인한다
            	return stageInfo.prevStage.IsClear(difficulty);
            }
		}

		return false;
	}


	//해당 스테이지를 특정 난이도로 완료했는 지 확인
	public bool IsClear (UInt16 stageID, byte difficulty = 1)
	{
		StageInfo stageInfo = null;
		if (dicStageData.TryGetValue (stageID, out stageInfo))
			return stageInfo.IsClear (difficulty);
		return false;
	}
	//해당 스테이지를 어떤 난이도로 클리어 했는지 확인
	//public int GetStageClearDifficult(UInt16 stageID)
	//{
	//	if ((dicStageData[stageID].clearState & 0x30) > 0) return 4; //매우 어려움
	//	else if ((dicStageData[stageID].clearState & 0x0C) > 0) return 3; // 어려움
	//	else if ((dicStageData[stageID].clearState & 0x03) > 0) return 2; // 일반
	//	else return 1;
	//}

	//해당 스테이지가 선택한 난이도에서 별 몇개로 클리어 했는지 얻어옴
	public int GetStageClearStar (UInt16 stageID)
	{		                
		switch (Legion.Instance.SelectedDifficult) {
		case 2:
			if ((dicStageData [stageID].clearState & 0x0C) > 0)
				return (dicStageData [stageID].clearState & 0x0C) >> 2;
			else
				return 0;

		case 3:
			if ((dicStageData [stageID].clearState & 0x30) > 0)
				return (dicStageData [stageID].clearState & 0x30) >> 4;
			else
				return 0;    

		default :
			if ((dicStageData [stageID].clearState & 0x03) > 0)
				return dicStageData [stageID].clearState & 0x03;
			else
				return 0; 
		}
	}

	//스테이지 클리어 정보 업데이트
	public void UpdateStageClearByClear (UInt16 stageID, Byte u1Difficulty, Byte u1Result)
	{
		StageInfo stageInfo = dicStageData [stageID];

		if (stageInfo.actInfo == null)
			return;

		// 2016. 7. 6 jy
		// 현재 스테이지가 탑이라면 클리어 별 업적 갱신을 하지 않는다
		if ( stageInfo.actInfo.u1Mode != ActInfo.ACT_TYPE.TOWER ) 
		{
			//별갯수가 갱신될 경우 업적 업데이트
			if (u1Result > GetStageClearStar (stageID))
			{
				Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.StageStar, 0, u1Difficulty, 0, 0, (uint)(u1Result - GetStageClearStar (stageID)));
			}
		}

		ChapterInfo chapterInfo = null;
		switch (u1Difficulty) {
		case 1:
			if (u1Result > (stageInfo.clearState & 0x03)) {
				// 해당 난이도 처음 클리어이며, 챕터의 마지막 스테이지일 경우 챕터 정보를 갱신하자.
				if ((stageInfo.clearState & 0x03) == 0) {
					if (stageInfo.sAnalyticsEventCode != "0") {
						AccountManager.Instance.FBEventLog (stageInfo.sAnalyticsEventCode);
					}
					if (stageInfo.IsLastStageInChapter) {
						chapterInfo = dicChapterData [stageInfo.u2ChapterID];
						if (stageInfo.u1ForestElement == 0 || chapterInfo.IsClear (stageInfo, u1Difficulty))
							Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.ChapterClear, stageInfo.u2ChapterID, u1Difficulty, 0, 0, 1);
					}
				}
				stageInfo.clearState &= 0xFC;
				stageInfo.clearState |= u1Result;
			}
			break;
		case 2:
			u1Result <<= 2;
			if (u1Result > (stageInfo.clearState & 0x0C)) {
				if ((stageInfo.clearState & 0x0C) == 0) {
					if (stageInfo.sAnalyticsEventCode != "0") {
						AccountManager.Instance.FBEventLog (stageInfo.sAnalyticsEventCode);
					}
					if (stageInfo.IsLastStageInChapter) {
						chapterInfo = dicChapterData [stageInfo.u2ChapterID];
						if (stageInfo.u1ForestElement == 0 || chapterInfo.IsClear (stageInfo, u1Difficulty))
							Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.ChapterClear, stageInfo.u2ChapterID, u1Difficulty, 0, 0, 1);
					}
				}
				stageInfo.clearState &= 0xF3;
				stageInfo.clearState |= u1Result;
			}
			break;
		case 3:
			u1Result <<= 4;
			if (u1Result > (stageInfo.clearState & 0x30)) {
				if ((stageInfo.clearState & 0x30) == 0) {
					if (stageInfo.sAnalyticsEventCode != "0") {
						AccountManager.Instance.FBEventLog (stageInfo.sAnalyticsEventCode);
					}
					if (stageInfo.IsLastStageInChapter) {
						chapterInfo = dicChapterData [stageInfo.u2ChapterID];
						if (stageInfo.u1ForestElement == 0 || chapterInfo.IsClear (stageInfo, u1Difficulty))
							Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.ChapterClear, stageInfo.u2ChapterID, u1Difficulty, 0, 0, 1);
					}
				}
				stageInfo.clearState &= 0xCF;
				stageInfo.clearState |= u1Result;
			}
			break;
		}

		if (chapterInfo != null) {
			chapterInfo.openStateByStage = (Byte)(u1Difficulty + 1);
			chapterInfo.UpdateChpaterOpen ();

			//이후 챕터가 존재하고 이전 챕터 열림 상태에 따라 열리는 챕터라면 정보를 갱신해 준다.
			if (chapterInfo.nextChapter != null && chapterInfo.nextChapter.u1OpenCondition == 3)
				chapterInfo.nextChapter.UpdateChpaterOpen ();
	            
			if (chapterInfo.IsLastChapterInAct)
				Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.ModeClear, stageInfo.u2ActID, u1Difficulty, 0, 0, 1);
		}
	}

	public bool CheckUnlockDifficult (int difficult, UInt16 actID)//chapterID)
	{
		if (difficult == 1)
			return true;
        
		//if (dicChapterData [actID].openState < difficult)
		if (dicActData [actID].openState < difficult)
			return false;

		return true;
	}
	
	//반복 보상 정보 업데이트
	public void UpdateClearPoint (UInt16 stageID)
	{
		ChapterInfo chapterInfo = dicChapterData [dicStageData [stageID].u2ChapterID];
		chapterInfo.repeatCount [Legion.Instance.SelectedDifficult - 1] += 1;
		
		if (chapterInfo.repeatCount [Legion.Instance.SelectedDifficult - 1] > chapterInfo.u1MaxRepeatCount)
			chapterInfo.repeatCount [Legion.Instance.SelectedDifficult - 1] = chapterInfo.u1MaxRepeatCount;
	}
	
	//별 보상 정보 업데이트
	public void UpdateStarPoint (UInt16 stageID)
	{
		bool isGet = false;
        
		ChapterInfo chapterInfo = dicChapterData [dicStageData [stageID].u2ChapterID];
		
		// 0이고 마지막 스테이지가 별3개이면 보상을 받은 상태
		if (chapterInfo.starCount [Legion.Instance.SelectedDifficult - 1] == 0) {       
			if (StageInfoMgr.Instance.GetStageClearStar (chapterInfo.lstStageID [chapterInfo.lstStageID.Count - 1]) > 2)
				isGet = true;
		}                
        
		chapterInfo.starCount [Legion.Instance.SelectedDifficult - 1] = 0;
		
		if (!isGet) {        
			foreach (StageInfo info in dicStageData.Values) {
				if (info.u2ChapterID == chapterInfo.u2ID) {
					if (GetStageClearStar (info.u2ID) > 3) {
						DebugMgr.LogError ("Star Counting Error : " + GetStageClearStar (info.u2ID));
					}
                    
					chapterInfo.starCount [Legion.Instance.SelectedDifficult - 1] += (Byte)GetStageClearStar (info.u2ID);
					if (chapterInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER) {
						
					} else if (chapterInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST) {
						if (chapterInfo.starCount [Legion.Instance.SelectedDifficult - 1] > 63)
							chapterInfo.starCount [Legion.Instance.SelectedDifficult - 1] = 63;
					} else if (chapterInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.STAGE) {
						if (chapterInfo.starCount [Legion.Instance.SelectedDifficult - 1] > 30)
							chapterInfo.starCount [Legion.Instance.SelectedDifficult - 1] = 30;
					}
				}
			}
		}
	}

	public void AddFogInfo (int id, FogInfo info)
	{
		dicFogData.Add (id, info);
	}

	public UInt16 GetStageID (int act, int chapter, int stage)
	{
		ActInfo actInfo = dicActData.ElementAtOrDefault (act - 1).Value;
        
		ChapterInfo chapterInfo = dicChapterData [actInfo.lstChapterID [chapter - 1]];
        
		return chapterInfo.lstStageID [stage - 1];
	}

	public List<UInt16> GetForestStageIDList (UInt16 chapterID, Byte element)
	{
		List<UInt16> ret = new List<UInt16> ();

		ChapterInfo chapterInfo = null;
		chapterInfo = dicChapterData [chapterID];

		for (int i = 0; i < chapterInfo.lstStageID.Count; i++) {
			StageInfo stageInfo = dicStageData [chapterInfo.lstStageID [i]];
			if (stageInfo.u1ForestElement == element) {
				ret.Add (chapterInfo.lstStageID [i]);
			}
		}
		return ret;
	}

	public Byte GetForestTicket ()//(UInt16 stageID) <= 탐색의 숲이 스테이지 별이 아닌 난이도 별로 변경되어 스테이지 ID값이 필요 없어짐
	{
        // 16.06.20 jy
        // 탐색의 숲 입장 가능 횟수 맵별 -> 난이도 별로 수정
        return forestTicketData.u1TicketCount[Legion.Instance.SelectedDifficult - 1];
	}

	public void ResetForestTicket()// (UInt16 stageID) <= 탐색의 숲이 스테이지 별이 아닌 난이도 별로 변경되어 스테이지 ID값이 필요 없어짐
	{
        // 16.06.20 jy
        // 탐색의 숲 입장 가능 횟수 맵별 -> 난이도 별로 수정
        forestTicketData.u1TicketCount[(Legion.Instance.SelectedDifficult-1)] = StageInfoMgr.Instance.forest.u1StageRepeatCount;
	}

	public void UseForestTicket (UInt16 stageID)
	{
		// 16.06.20 jy
		// 탐색의 숲 입장 가능 횟수 맵별 -> 난이도 별로 수정
		--forestTicketData.u1TicketCount[Legion.Instance.SelectedDifficult-1];
	}

	public void AddForestChargedTicketCount()
	{
		++forestTicketData.au1ChargedTicketCount [(Legion.Instance.SelectedDifficult - 1)];
	}

	public Byte GetForestChargedTicketCount() // (UInt16 stageID) <= 탐색의 숲이 스테이지 별이 아닌 난이도 별로 변경되어 스테이지 ID값이 필요 없어짐
	{
		// 16.06.20 jy
		// 탐색의 숲 입장 가능 횟수 맵별 -> 난이도 별로 수정
		return (Byte)(forestTicketData.au1ChargedTicketCount [(Legion.Instance.SelectedDifficult - 1)]);
		//return (Byte)(dicStageData[stageID].forestData.au1ChargedTicketCount[(Legion.Instance.selectedDifficult-1)]);
	}

    public bool IsRepeatItemInfo()
    {
        if (RepeatTargetItem == null)
            return false;

        return true;
    }

    public bool IsRepeatColletComplete()
    {
        if (RepeatTargetItem == null)
            return false;

        if (RepeatTargetItem.u4Count > u4CurTargetItemCount)
            return false;

        return true;
    }

    public void AddRepeatTargetCount(Goods rewardInfo)
    {
        if (RepeatTargetItem == null)
            return;

        if (rewardInfo.u2ID == RepeatTargetItem.u2ID && rewardInfo.u1Type == RepeatTargetItem.u1Type)
            u4CurTargetItemCount += rewardInfo.u4Count;
    }


    public void RepeatItemInfoDelete()
    {
        RepeatTargetItem = null;
        u4CurTargetItemCount = 0;
    }
}
	