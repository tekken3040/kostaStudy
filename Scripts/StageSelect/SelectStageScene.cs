using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
//액트 리스트 출력
public class SelectStageScene : BaseScene {
	
    MENU[] menuType = new MENU[]{MENU.CAMPAIGN};
    
	public GameObject actList;
	public RectTransform actToggles;
	public RectTransform actContent;
	public ChapterTab cCurChapterTab;
	public ChapterTab cChapterTab;
	public ForestTab cForestTab;
	public TowerTab cTowerTab;
    public GameObject crewList;
    
    public delegate void RefreshAlramInfo();
    public event RefreshAlramInfo refreshRewardInfo;
    public event RefreshAlramInfo refreshDispatchInfo;
	
	private UInt16 actID = 0;
	private GameObject actItem;
	private GameObject actToggleItem;
    private Dictionary<UInt16, ActToggle> actTabBtnList = new Dictionary<UInt16, ActToggle>();    // 신규 TabButton List;

    public GameObject BoostExp;
    public GameObject BoostGold;
    TimeSpan tsLeftTime;
    TimeSpan tsStartTime;

    public SubChatting _subChattingWidown;

    public void Awake()
	{
		bool bStage = true;
        //#CHATTING
        if (_subChattingWidown != null)
        {
            _subChattingWidown.gameObject.SetActive(false);
            if (PopupManager.Instance.IsChattingActive())
            {
                PopupManager.Instance.SetSubChtting(_subChattingWidown);
            }
        }
        // 액트 슬롯 및 TabToggleBtn 생성
        SetActList();
        CheckUnlockBoostEvent();
        //마지막으로 플레이한 스테이지나 퀘스트 업적등 바로가기로 입력된 스테이지가 있을경우 바로가기 처리하는 부분
		if (StageInfoMgr.Instance.LastPlayStage != -1) {
			if (StageInfoMgr.Instance.dicStageData.ContainsKey ((UInt16)StageInfoMgr.Instance.LastPlayStage)) {
				StageInfo stageInfo = StageInfoMgr.Instance.dicStageData [(UInt16)StageInfoMgr.Instance.LastPlayStage];
				ActInfo actInfo = stageInfo.actInfo;
				if (actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST) {
					bStage = false;
                    cCurChapterTab = cForestTab;
				} else if (actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER) {
					bStage = false;
                    cCurChapterTab = cTowerTab;
				} else if (actInfo.u1Mode == ActInfo.ACT_TYPE.STAGE) {
                    cCurChapterTab = cChapterTab;
				}
				OnClickAct (stageInfo.u2ActID);
                cCurChapterTab.SetPage (stageInfo.chapterInfo.u2ID);
				actID = stageInfo.u2ActID;
			}
		} else if (StageInfoMgr.Instance.ShortCutChapter > 0) {
			if (StageInfoMgr.Instance.dicChapterData.ContainsKey ((UInt16)StageInfoMgr.Instance.ShortCutChapter)) {
				ChapterInfo cInfo = StageInfoMgr.Instance.dicChapterData [(UInt16)StageInfoMgr.Instance.ShortCutChapter];
				ActInfo actInfo = cInfo.actInfo;
				if (actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST) {
					bStage = false;
                    cCurChapterTab = cForestTab;
				} else if (actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER) {
					bStage = false;
                    cCurChapterTab = cTowerTab;
				} else if (actInfo.u1Mode == ActInfo.ACT_TYPE.STAGE) {
                    cCurChapterTab = cChapterTab;
				}
				OnClickAct (cInfo.u2ActID);
                cCurChapterTab.SetPage (cInfo.u2ID);
				actID = cInfo.u2ActID;
			}
			AssetMgr.Instance.CheckDivisionDownload (2, StageInfoMgr.Instance.ShortCutChapter);			
			StageInfoMgr.Instance.ShortCutChapter = -1;
		}

		FadeEffectMgr.Instance.FadeIn(FadeEffectMgr.GLOBAL_FADE_TIME);
		StartCoroutine(CheckReservedPopup());

		if(Legion.Instance.cTutorial.au1Step[4] != Server.ConstDef.LastTutorialStep)
            crewList.SetActive(false);
        else
            crewList.SetActive(true);

        if (bStage)
        {
            Legion.Instance.cTutorial.CheckTutorial(1007);
            if (Legion.Instance.cTutorial.au1Step[0] == Server.ConstDef.LastTutorialStep)
                Legion.Instance.cQuest.CheckIngDirection();
        }
		
		ObjMgr.Instance.RemoveHeroModelPool ();

		Legion.Instance.bCharInfoToCrew = false;
		Legion.Instance.bStageToCrew = false;
		Legion.Instance.bLeagueToCharInfo = false;

		#if UNITY_ANDROID
		IgaworksUnityAOS.IgaworksUnityPluginAOS.Adbrix.retention("Campaign");
		#elif UNITY_IOS
		AdBrixPluginIOS.Retention("Campaign");
		#endif
	}

    public void CheckUnlockBoostEvent()
    {
        if(EventInfoMgr.Instance.lstExpBuffEvent.Count != 0)
        {
            EventInfoMgr.Instance.lstExpBuffEvent.Sort
            (
                delegate(EventReward x, EventReward y) 
                {
                    int compare = 0;
                    
                    compare = ((EventReward)x).dtEventBegin.CompareTo(((EventReward)y).dtEventBegin);

                    return compare;
                }
            );

            EventReward tempItem = EventInfoMgr.Instance.lstExpBuffEvent[0];

            tsLeftTime = tempItem.dtEventEnd - Legion.Instance.ServerTime;
            tsStartTime = Legion.Instance.ServerTime - tempItem.dtEventBegin;
            UInt16 _eventID = tempItem.u2EventID;
            if(tsStartTime.TotalSeconds < 0)
            {
                EventInfoMgr.Instance.u4ExpBoostPer = 0;
                BoostExp.SetActive(false);
            }
            else if(tsLeftTime.TotalSeconds < 0)
            {
                EventInfoMgr.Instance.u4ExpBoostPer = 0;
                EventInfoMgr.Instance.dicEventReward.Remove(_eventID);
                BoostExp.SetActive(false);
            }
            else
            {
                BoostExp.SetActive(true);
                EventInfoMgr.Instance.u4ExpBoostPer = tempItem.recordValue;
            }
        }
        else
            BoostExp.SetActive(false);

        if(EventInfoMgr.Instance.lstGoldBuffEvent.Count != 0)
        {
            EventInfoMgr.Instance.lstGoldBuffEvent.Sort
            (
                delegate(EventReward x, EventReward y) 
                {
                    int compare = 0;
                    
                    compare = ((EventReward)x).dtEventBegin.CompareTo(((EventReward)y).dtEventBegin);

                    return compare;
                }
            );

            EventReward tempItem = EventInfoMgr.Instance.lstGoldBuffEvent[0];

            tsLeftTime = tempItem.dtEventEnd - Legion.Instance.ServerTime;
            tsStartTime = Legion.Instance.ServerTime - tempItem.dtEventBegin;
            UInt16 _eventID = tempItem.u2EventID;

            if(tsStartTime.TotalSeconds < 0)
            {
                EventInfoMgr.Instance.u4GoldBoostPer = 0;
                BoostGold.SetActive(false);
            }
            else if(tsLeftTime.TotalSeconds < 0)
            {
                EventInfoMgr.Instance.u4GoldBoostPer = 0;
                EventInfoMgr.Instance.dicEventReward.Remove(_eventID);
                BoostGold.SetActive(false);
            }
            else
            {
                BoostGold.SetActive(true);
                EventInfoMgr.Instance.u4GoldBoostPer = tempItem.recordValue;
            }
        }
        else
            BoostGold.SetActive(false);
    }
    // 별 또는 반복 보상 정보가 갱신 되었을 경우 느낌표 처리
    public void RefreshRewardInfo()
    {
        if(refreshRewardInfo != null)
            refreshRewardInfo();
    }
    
    // 파견 정보가 갱신 되었을 경우 느낌표 처리
    public void RefreshDispatchInfo()
    {
        if(refreshDispatchInfo != null)
            refreshDispatchInfo();
    }    
	
	public void OnClickBack()
	{
        PopupManager.Instance.RemovePopup(gameObject);
		StartCoroutine(ChangeScene());	
	}

	private IEnumerator ChangeScene()
	{
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		AssetMgr.Instance.SceneLoad("LobbyScene");
        StageInfoMgr.Instance.LastPlayStage = -1;
	}
	
    //액트 리스트 출력 하는 부분
	public void SetActList()
	{
		int index = 0;
		foreach(ActInfo actInfo in StageInfoMgr.Instance.dicActData.Values)
		{
            //일반 모드는 오픈된 목록만 보여줌 다른 모드는 상시 보여줌
			if(actInfo.CheckActOpen() || actInfo.u1Mode != ActInfo.ACT_TYPE.STAGE)
			{
                actInfo.ActIndex = index++;
                // 액트 슬롯 생성
                SetActSlot(actInfo);
                SetActTabButton(actInfo);
			}
		}

        // 액트 슬롯이 생성이 되었다면 크기를 변경한다
        if (actContent.childCount > 0)
        {
            GridLayoutGroup grid = actContent.GetComponent<GridLayoutGroup>();
            actContent.sizeDelta = new Vector2((grid.cellSize.x + grid.spacing.x) * StageInfoMgr.Instance.dicActData.Count - grid.spacing.x + grid.padding.left, actContent.sizeDelta.y);
        }
        PopupManager.Instance.AddPopup(gameObject, OnClickBack);
	}

    // 액트 슬롯 생성
    private void SetActSlot(ActInfo actInfo)
    {
        // 바로가기 상황이라면 슬롯을 생성하지 않는다
        if (StageInfoMgr.Instance.LastPlayStage != -1 || StageInfoMgr.Instance.ShortCutChapter > 0)
            return;

        // 슬롯
        if (actItem == null)
            actItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/Campaign/ActSlot.prefab", typeof(GameObject)) as GameObject;

        //슬롯 생성
        GameObject instItem = Instantiate(actItem) as GameObject;
        instItem.transform.SetParent(actContent);
        instItem.transform.localScale = Vector3.one;
        instItem.transform.localPosition = Vector3.zero;

        ActSlotData actSlotData = new ActSlotData();
        actSlotData.index = actInfo.ActIndex;
        actSlotData.actInfo = actInfo;

        ActSlot actSlot = instItem.GetComponent<ActSlot>();
        actSlot.InitSlot(actSlotData);
        actSlot.onClickAct = OnClickAct;
    }
    // 액트 탭 버튼 셋팅

    private void SetActTabButton(ActInfo actInfo)
    {
        // 상단 토글 메뉴
        if (actToggleItem == null)
            actToggleItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/Campaign/ActToggle.prefab", typeof(GameObject)) as GameObject;

        //토글 메뉴 생성
        GameObject tabButton = Instantiate(actToggleItem) as GameObject;
        tabButton.transform.SetParent(actToggles);
        tabButton.transform.localScale = Vector3.one;
        tabButton.transform.localPosition = Vector3.zero;

        ActToggle actTab = tabButton.GetComponent<ActToggle>();
        actTab.SetToggle(actInfo);
        actTab.onClickToggle = ChangeAct;
        actTabBtnList.Add(actInfo.u2ID, actTab);
    }

    // 액트 슬롯을 클릭 했을 경우 챕터 정보를 세팅해 준다.
    private void OnClickAct(UInt16 actID)
	{
		if(this.actID == actID)
			return;
		
		if(cCurChapterTab != null)
            cCurChapterTab.gameObject.SetActive(false);

        StartCoroutine(FadeOut(actID));
	}

    // 토글을 클릭할 경우 해당 액트에 맞게 챕터 정보를 갱신
	private void ChangeAct(UInt16 actID)
	{
		if(this.actID == actID)
			return;
        
        // 이전 탭 버튼 클릭 이미지 해제
        if (actTabBtnList.ContainsKey(this.actID) == true)
            actTabBtnList[this.actID].ChangedBtnSprite(false);

        this.actID = actID;

		ActInfo actInfo;
		StageInfoMgr.Instance.dicActData.TryGetValue(actID, out actInfo);
		if(actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
		{
			cChapterTab.gameObject.SetActive(false);
			cTowerTab.gameObject.SetActive(false);
			cForestTab.gameObject.SetActive(true);
            cCurChapterTab = cForestTab;
		}
		else if(actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER)
		{
			cChapterTab.gameObject.SetActive(false);
			cTowerTab.gameObject.SetActive(true);
			cForestTab.gameObject.SetActive(false);
            cCurChapterTab = cTowerTab;
		}
		else
		{
			cChapterTab.gameObject.SetActive(true);
			cTowerTab.gameObject.SetActive(false);
			cForestTab.gameObject.SetActive(false);
            cCurChapterTab = cChapterTab;
		}

        cCurChapterTab.SetChapterPage(actID);
	}
    
    // 하단 크루 메뉴 선택시 처리
	public void OnClickCrew(int crewIdx)
	{
		Crew crew = Legion.Instance.acCrews[crewIdx];
        //파견중이 아니면 크루 설정 창으로
		if(crew.DispatchStage == null)
		{
            Legion.Instance.bStageToCrew = true;
            Legion.Instance.tempCrewIndex = crewIdx;
			OnClickBack();
		}
		else
		{
//			StageInfo stageInfo = StageInfoMgr.Instance.dicStageData [crew.DispatchStage.u2ID];
//
//			actToggles.gameObject.SetActive (true);
//			if (actTabBtnList [stageInfo.u2ActID].IsSelected == false)
//				actTabBtnList [stageInfo.u2ActID].OnValueChanged ();
//			else
//				cCurChapterTab.CreateMoveCloudEffect ();
//
//			if (crew.StageDifficulty != Legion.Instance.SelectedDifficult)
//				cCurChapterTab.RefreshDifficult (crew.StageDifficulty);
//        
//			actID = stageInfo.u2ActID;

			if (cCurChapterTab == null) {
				StageInfo stageInfo = StageInfoMgr.Instance.dicStageData [crew.DispatchStage.u2ID];

				if(stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
				{
					cCurChapterTab = cForestTab;
				}
				else if(stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER)
				{
					cCurChapterTab = cTowerTab;
				}
				else
				{
					cCurChapterTab = cChapterTab;
				}
			}

			cCurChapterTab.OnClickDispatch (crewIdx);
        }
	}
    
    // 바로가기 팝업 처리
	public override IEnumerator CheckReservedPopup()
	{
		Legion.Instance.cTutorial.bStageReservedPopup = false;
		if(Legion.Instance.bStageFailed)
			PopupManager.Instance.SetNoticePopup (MENU.CAMPAIGN);

		GameManager.ReservedPopup reservedPopup = null;
		for(int i=0; i<menuType.Length; i++)
		{
			//DebugMgr.Log(menuType[i]);
			reservedPopup = GameManager.Instance.GetReservedPopup(menuType[i]);
            if (reservedPopup != null)
            {
                Legion.Instance.cTutorial.bStageReservedPopup = true;
                POPUP_CAMPAIGN campaignPopup = reservedPopup.GetReservedPopupCampaign();
                if (cCurChapterTab == null)
                {
                    ActInfo.ACT_TYPE actMode = 0;
                    switch (campaignPopup)
                    {
                        case POPUP_CAMPAIGN.STAGE_SELECT_EASY:
                        case POPUP_CAMPAIGN.STAGE_SELECT_NORMAL:
                        case POPUP_CAMPAIGN.STAGE_SELECT_HELL:
                        case POPUP_CAMPAIGN.STAGE_INFO_EASY:
                        case POPUP_CAMPAIGN.STAGE_INFO_NORMAL:
                        case POPUP_CAMPAIGN.STAGE_INFO_HELL:
                            actMode = ActInfo.ACT_TYPE.STAGE;
                            cCurChapterTab = cChapterTab;
                            break;
                        case POPUP_CAMPAIGN.TOWER_SELECT_EASY:
                        case POPUP_CAMPAIGN.TOWER_SELECT_NORMAL:
                        case POPUP_CAMPAIGN.TOWER_SELECT_HELL:
                        case POPUP_CAMPAIGN.TOWER_INFO_EASY:
                        case POPUP_CAMPAIGN.TOWER_INFO_NORMAL:
                        case POPUP_CAMPAIGN.TOWER_INFO_HELL:
                            actMode = ActInfo.ACT_TYPE.TOWER;
                            cCurChapterTab = cTowerTab;
                            break;
                        case POPUP_CAMPAIGN.FOREST_SELECT_EASY:
                        case POPUP_CAMPAIGN.FOREST_SELECT_NORMAL:
                        case POPUP_CAMPAIGN.FOREST_SELECT_HELL:
                        case POPUP_CAMPAIGN.FOREST_INFO_EASY:
                        case POPUP_CAMPAIGN.FOREST_INFO_NORMAL:
                        case POPUP_CAMPAIGN.FOREST_INFO_HELL:
                            actMode = ActInfo.ACT_TYPE.FOREST;
                            cCurChapterTab = cForestTab;
                            break;
                    }

                    ActInfo actInfo = null;
                    foreach(ActInfo info in StageInfoMgr.Instance.dicActData.Values)
                    {
                        if(info.u1Mode == actMode)
                        {
                            actInfo = info;
                            break;
                        }
                    }
                    if (actInfo != null)
                        ChangeAct(actInfo.u2ID);
                    else
                        yield break;
                }

                switch (campaignPopup)
                {
                    case POPUP_CAMPAIGN.STAGE_SELECT_EASY:
                    case POPUP_CAMPAIGN.TOWER_SELECT_EASY:
                    case POPUP_CAMPAIGN.FOREST_SELECT_EASY:
                        cCurChapterTab.RefreshDifficult(1);
                        break;
                    case POPUP_CAMPAIGN.STAGE_SELECT_NORMAL:
                    case POPUP_CAMPAIGN.TOWER_SELECT_NORMAL:
                    case POPUP_CAMPAIGN.FOREST_SELECT_NORMAL:
                        cCurChapterTab.RefreshDifficult(2);
                        break;
                    case POPUP_CAMPAIGN.STAGE_SELECT_HELL:
                    case POPUP_CAMPAIGN.TOWER_SELECT_HELL:
                    case POPUP_CAMPAIGN.FOREST_SELECT_HELL:
                        cCurChapterTab.RefreshDifficult(3);
                        break;
                    case POPUP_CAMPAIGN.STAGE_INFO_EASY:
                    case POPUP_CAMPAIGN.TOWER_INFO_EASY:
                    case POPUP_CAMPAIGN.FOREST_INFO_EASY:
                        cCurChapterTab.RefreshDifficult(1);
                        cCurChapterTab.OpenStageInfo((UInt16)StageInfoMgr.Instance.LastPlayStage);
                        break;
                    case POPUP_CAMPAIGN.STAGE_INFO_NORMAL:
                    case POPUP_CAMPAIGN.TOWER_INFO_NORMAL:
                    case POPUP_CAMPAIGN.FOREST_INFO_NORMAL:
                        cCurChapterTab.RefreshDifficult(2);
                        cCurChapterTab.OpenStageInfo((UInt16)StageInfoMgr.Instance.LastPlayStage);
                        break;
                    case POPUP_CAMPAIGN.STAGE_INFO_HELL:
                    case POPUP_CAMPAIGN.TOWER_INFO_HELL:
                    case POPUP_CAMPAIGN.FOREST_INFO_HELL:
                        cCurChapterTab.RefreshDifficult(3);
                        cCurChapterTab.OpenStageInfo((UInt16)StageInfoMgr.Instance.LastPlayStage);
                        break;
                    default:
                        DebugMgr.LogError("CheckReservedPopup() not Setting case param");
                        break;
                }
				yield break;
			}
		}
	}    

    public override void RefreshAlram()
    {
        
    }

    private IEnumerator FadeOut(UInt16 actID)
    {
        //ActInfo actInfo = StageInfoMgr.Instance.dicActData[actID];
        //string actName = TextManager.Instance.GetText(actInfo.strName);
        if (StageInfoMgr.Instance.LastPlayStage == -1 &&
            StageInfoMgr.Instance.ShortCutChapter == -1)
        {
            FadeEffectMgr.Instance.FadeOut(0.6f);
            yield return new WaitForSeconds(0.6f);
        }

        actToggles.gameObject.SetActive(true);
        actList.gameObject.SetActive(false);

        //lstToggle[index].isOn = true;
        if (actTabBtnList.ContainsKey(actID) == true)
            actTabBtnList[actID].OnValueChanged();

        yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);

        FadeEffectMgr.Instance.FadeIn(0.4f);

        //#CHATTING
        if (_subChattingWidown != null && !_subChattingWidown.gameObject.activeSelf)
        {
            if (PopupManager.Instance.IsChattingActive())
            {
                _subChattingWidown.gameObject.SetActive(true);
            }
        }
        yield return null;
	}
}