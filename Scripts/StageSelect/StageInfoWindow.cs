using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

// 선택한 스테이지 정보를 보여준다
public class StageInfoWindow : MonoBehaviour {

	public const int RECOMMAND_LEVEL_GAP = 5; // 권장 레벨 차이

	public delegate void OnClickDispatch(int crewIdx);
	public delegate void OnClickSweep();
	public delegate void OnClickGuide();
	public OnClickDispatch onClickDispatch;
	public OnClickSweep onClickSweep;
	public OnClickGuide onClickGuide;

	public Text stageTitle;
	public Image stageImage;
	public RewardButton[] rewardButtons;
	public Image[] rewardsIcons;
    public Image[] rewardsGrades;
    public GameObject crewList;
	public CrewSlot[] crewSlots;
	public Text rewardText;
	public Text expText;
	public Text expValue;
	public Text rewardValue;
	public Text recommandLevel;
	public Text recommandDiffcult;
	public Image recommandElement;
	public Text sweepTickeText;
    public GameObject DispatchLabel;
    public GameObject GoToCrewBtn;
    public GameObject m_ToggleBtnAuto;
	public Animator animator;
    
    public GameObject[] questMark; //0:stage 1:dispatch 2:sweep

	public Text _textRecommendEquipInfo;// 추천 장비 정보

    public RepeatTargetInfo cRepeatTarget;  //반복 전투 목표 설정

	public GameObject GuideBtn;

    public GameObject BoostExp;
    public GameObject BoostGold;

	public GameObject CrewOpenMsg;

    TimeSpan tsLeftTime;
    TimeSpan tsStartTime;

    public virtual void SetInfo(UInt16 stageID)
	{
		if (CrewOpenMsg != null) {
			CrewOpenMsg.SetActive (false);
			if (Legion.Instance.sName == ""){
				if (LegionInfoMgr.Instance.acCrewOpenGoods [0] [1].u2ID == stageID) {
					CrewOpenMsg.SetActive (true);
				}
			}
		}

		animator.Play("Popup_Show");
		CheckUnlockBoostEvent();
		StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[stageID];
		
		stageTitle.text = TextManager.Instance.GetText(stageInfo.sName);
        switch (stageInfo.u1RecommandElement)
        {
            case 2:
                UIManager.Instance.SetGradientFromElement(stageTitle.GetComponent<Gradient>(), 4);
                break;
            case 3:
                UIManager.Instance.SetGradientFromElement(stageTitle.GetComponent<Gradient>(), 2);
                break;
            case 4:
                UIManager.Instance.SetGradientFromElement(stageTitle.GetComponent<Gradient>(), 3);
                break;
        }
		stageImage.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/" + stageInfo.stageIconPath);
		
		byte selectedDifficult = Legion.Instance.SelectedDifficult;

        if (GuideBtn != null)
        {
            if (stageInfo.au2GuideID[selectedDifficult - 1] == 0)
                GuideBtn.SetActive(false);
            else
                GuideBtn.SetActive(true);
        }

        UInt16 nItemId;
        for (int i = 0; i < rewardButtons.Length; i++)
        {
            rewardButtons[i].gameObject.SetActive(false);
        }

        // 아이템 슬롯 마지막에서 부터 아이템을 셋팅한다
        int currentSlotIdx = rewardButtons.Length -1;
        for (int i = 0; i < StageInfo.MAX_REWARD_COUNT; ++i)
        {
            nItemId = stageInfo.RewardItems[selectedDifficult-1][i].cRewards.u2ID;
            if (nItemId != 0 && i >= StageInfo.CONFIRM_REWORD_COUNT)
            {
                rewardsIcons[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_random_reward");
                rewardsGrades[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4570");
                rewardButtons[currentSlotIdx].SetButton((Byte)GoodsType.RANDOM_REWARD, 0);
                rewardsIcons[currentSlotIdx].SetNativeSize();
                rewardButtons[currentSlotIdx].gameObject.SetActive(true);
                currentSlotIdx--;
                break;
            }
            else if (nItemId != 0)
            {
                rewardsIcons[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(nItemId).u2IconID);
                rewardsGrades[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(nItemId));
                rewardButtons[currentSlotIdx].SetButton(stageInfo.RewardItems[selectedDifficult-1][i].cRewards.u1Type, nItemId);
                rewardsIcons[currentSlotIdx].SetNativeSize();
                rewardButtons[currentSlotIdx].gameObject.SetActive(true);
                currentSlotIdx--;
            }
        }
        // 이벤트 상품 셋팅
		List<EventDungeonShopInfo> eventDungeonList = EventInfoMgr.Instance.CheckOpenDungeon ();
		for (int i = 0; i < eventDungeonList.Count; i++)
        {
			if (eventDungeonList [i].u1UIType == 3)
            {
                for (int j = 0; j < StageInfo.MAX_REWARD_COUNT; ++j)
                {
                    nItemId = stageInfo.RewardItems[selectedDifficult - 1][StageInfo.MAX_REWARD_COUNT + j].cRewards.u2ID;
                    if (nItemId != 0 && j >= StageInfo.CONFIRM_REWORD_COUNT)
                    {
                        rewardsIcons[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_random_reward");
                        rewardsGrades[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4570");
                        rewardButtons[currentSlotIdx].SetButton((Byte)GoodsType.RANDOM_REWARD, 0);
                        rewardsIcons[currentSlotIdx].SetNativeSize();
                        rewardButtons[currentSlotIdx].gameObject.SetActive(true);
                        currentSlotIdx--;
                        break;
                    }
                    else if (nItemId != 0)
                    {
                        rewardsIcons[currentSlotIdx].sprite = AtlasMgr.Instance.GetGoodsIcon(new Goods((byte)GoodsType.EVENT_ITEM, nItemId, 0));
                        rewardsGrades[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(nItemId));
						rewardButtons[currentSlotIdx].SetButton(stageInfo.RewardItems[selectedDifficult - 1][StageInfo.MAX_REWARD_COUNT + j].cRewards.u1Type, nItemId);
                        rewardsIcons[currentSlotIdx].SetNativeSize();
                        rewardButtons[currentSlotIdx].gameObject.SetActive(true);
                        currentSlotIdx--;
                    }
                }
            }
		}

        Transform obj = transform.FindChild("Popup").FindChild("SweepTicket");
        if (obj != null)
            obj.GetComponent<RewardButton>().SetButton((Byte)GoodsType.CONSUME, Server.ConstDef.TICKET_SWEEP);

        if (m_ToggleBtnAuto != null && m_ToggleBtnAuto.activeSelf)
            m_ToggleBtnAuto.GetComponent<Toggle>().isOn = Legion.Instance.AUTOCONTINUE;
			
        if(EventInfoMgr.Instance.u4GoldBoostPer == 0)
		    rewardValue.text = stageInfo.acGetGoods[selectedDifficult-1].u4Count.ToString();
        else
        {
            UInt32 tempGold = 0;
            tempGold = (UInt32)(stageInfo.acGetGoods[selectedDifficult-1].u4Count*(float)(EventInfoMgr.Instance.u4GoldBoostPer/100)+stageInfo.acGetGoods[selectedDifficult-1].u4Count);
            rewardValue.text = tempGold.ToString();
        }

        if(EventInfoMgr.Instance.u4ExpBoostPer == 0)
		    expValue.text = stageInfo.arrGetExp[selectedDifficult-1].ToString();
        else
        {
            UInt64 tempExp = 0;
            tempExp = (UInt64)(stageInfo.arrGetExp[selectedDifficult-1]*(float)(EventInfoMgr.Instance.u4ExpBoostPer/100)+stageInfo.arrGetExp[selectedDifficult-1]);
            expValue.text = tempExp.ToString();
        }
		
		recommandLevel.text = stageInfo.arrRecommandLevel[selectedDifficult-1].ToString();
		
		int levelGap = Legion.Instance.AverageLevel(Legion.Instance.cBestCrew) - stageInfo.arrRecommandLevel[selectedDifficult-1];
		
        //레벨 평균과 권장레벨 차이로 난이도를 표기해준다
		if(levelGap >= RECOMMAND_LEVEL_GAP)
			recommandDiffcult.text = TextManager.Instance.GetText("mark_lvl_gap_easy");
		else if(levelGap <= -RECOMMAND_LEVEL_GAP)
			recommandDiffcult.text = TextManager.Instance.GetText("mark_lvl_gap_hard");
		else
			recommandDiffcult.text = TextManager.Instance.GetText("mark_lvl_gap_normal");
			
		recommandElement.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_icon_" + stageInfo.u1RecommandElement);
		recommandElement.SetNativeSize();
		
		Legion.Instance.u2SelectStageID = stageID;

        if (StageInfoMgr.Instance.IsRepeatItemInfo())
        {
            if (cRepeatTarget != null)
            {
                cRepeatTarget.gameObject.SetActive(true);
                cRepeatTarget.SetTargetInfo(StageInfoMgr.Instance.RepeatTargetItem);
                if (m_ToggleBtnAuto != null)
                    m_ToggleBtnAuto.SetActive(false);
            }
            else
            {
                if (m_ToggleBtnAuto != null)
                    m_ToggleBtnAuto.SetActive(true);
            }
        }
        else
        {
            if (cRepeatTarget != null)
                cRepeatTarget.gameObject.SetActive(false);
            if (m_ToggleBtnAuto != null)
                m_ToggleBtnAuto.SetActive(true);
        }
        
		// 2016. 08. 29jy 
		// 추가 정보창 셋팅
		SetStageMoreInfo(stageInfo, selectedDifficult);
        //소탕권 정보
		if(Legion.Instance.cInventory.dicItemKey.ContainsKey(Server.ConstDef.TICKET_SWEEP))
			sweepTickeText.text = ((ConsumableItem)Legion.Instance.cInventory.dicInventory[Legion.Instance.cInventory.dicItemKey[Server.ConstDef.TICKET_SWEEP]]).u2Count.ToString();
		else
			sweepTickeText.text = "0";

		int unlockCnt = 0;
		for (int i = 0; i < Legion.Instance.acCrews.Length; i++) {
			if (!Legion.Instance.acCrews [i].IsLock)
				unlockCnt++;
		}
        
        //튜토리얼 중엔 크루 목록을 보여주지 않는다
        if (unlockCnt < 2)
        {
            crewList.SetActive(false);
            GoToCrewBtn.SetActive(true);
            if (Legion.Instance.sName == "")
                GoToCrewBtn.GetComponent<Button>().interactable = false;
            else
                GoToCrewBtn.GetComponent<Button>().interactable = true;

            DispatchLabel.SetActive(false);
        }
        else
        {
            crewList.SetActive(true);
			DispatchLabel.SetActive(true);
            GoToCrewBtn.SetActive(false);
         
            for(int i=0; i<crewSlots.Length; i++)
			{
			 	crewSlots[i].Init(stageID);
				crewSlots[i].gameObject.AddComponent<TutorialButton>().id = "Dispath_Crew_"+(i+1).ToString();
			}
        }
        
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);

		for (int i=0; i<questMark.Length; i++) {
			questMark[i].SetActive(false);
		}
		
		if (Legion.Instance.cQuest.CheckQuestAlarm (MENU.CAMPAIGN, 0)) {
			QuestInfo tempQuest = Legion.Instance.cQuest.CurrentQuest ();
			if(stageInfo.IsLastStageInChapter && tempQuest.u1QuestType == 20){
				if(stageInfo.GetActInfo().u1Number == tempQuest.u1Delemiter1){
					if(tempQuest.u2QuestTypeID == stageInfo.GetChapterInfo().u2ID){
						if(tempQuest.u1Delemiter2 > 0){
							if(tempQuest.u1Delemiter2 == Legion.Instance.SelectedDifficult){
								questMark[0].SetActive(true);
								return;
							}
						}else{
							questMark[0].SetActive(true);
							return;
						}
					}
				}
			}else if(tempQuest.u1QuestType == 2 || tempQuest.u1QuestType == 18 || tempQuest.u1QuestType == 19){
				Byte bossType = stageInfo.u1BossType;
				if(bossType < 2) bossType = 1;
				else if(bossType == 3) bossType = 2;
				if (tempQuest.u2QuestTypeID > 0)
                {
					if (stageID == tempQuest.u2QuestTypeID)
                    {
						if((tempQuest.u1Delemiter1 == 0 || tempQuest.u1Delemiter1 == Legion.Instance.SelectedDifficult)
						   && (tempQuest.u1Delemiter2 == 0 || tempQuest.u1Delemiter2 == bossType)
						   && (tempQuest.u1Delemiter3 == 0 || tempQuest.u1Delemiter3 == stageInfo.GetActInfo().u1Number))
						{
                            if (tempQuest.u1QuestType == 2) questMark[0].SetActive(true);
                            else if (tempQuest.u1QuestType == 18) questMark[1].SetActive(true);
                            else if (tempQuest.u1QuestType == 19) questMark[2].SetActive(true);
						}
					}
				}else{
					if((tempQuest.u1Delemiter1 == 0 || tempQuest.u1Delemiter1 == Legion.Instance.SelectedDifficult)
					   && (tempQuest.u1Delemiter2 == 0 || tempQuest.u1Delemiter2 == bossType)
					   && (tempQuest.u1Delemiter3 == 0 ||tempQuest.u1Delemiter3 == stageInfo.GetActInfo().u1Number))
					{
                        if (tempQuest.u1QuestType == 2) questMark[0].SetActive(true);
                        else if (tempQuest.u1QuestType == 18) questMark[1].SetActive(true);
                        else if (tempQuest.u1QuestType == 19) questMark[2].SetActive(true);
					}
				}
			}
            else if(tempQuest.u1QuestType == 15)
            {
                bool isChapterID = false;
                ChapterInfo chapterInfo = null;
                // 퀘스트 타입 아이디가 챕터 아이디인지 확인
                StageInfoMgr.Instance.dicChapterData.TryGetValue(tempQuest.u2QuestTypeID, out chapterInfo);
                if (chapterInfo != null)
                {
                    if (tempQuest.u2QuestTypeID != stageInfo.chapterInfo.u2ID)
                        return;

                    isChapterID = true;
                }

                for (int i=0; i<stageInfo.acPhases.Length; i++)
                {
                    FieldInfo fieldInfo = StageInfoMgr.Instance.GetFieldInfo(stageInfo.acPhases[i].u2FieldID);
                    if (fieldInfo == null)
                        continue;

                    for (int j = 0; j < fieldInfo.acMonsterGroup.Length; j++)
                    {
                        for (int k = 0; k < fieldInfo.acMonsterGroup[j].acMonsterInfo.Length; k++)
                        {
                            ClassInfo monsterInfo = ClassInfoMgr.Instance.GetInfo(fieldInfo.acMonsterGroup[j].acMonsterInfo[k].u2MonsterID);
                            if (monsterInfo == null)
                                continue;

                            if (isChapterID == false && tempQuest.u2QuestTypeID > 0 && monsterInfo.u2ID != tempQuest.u2QuestTypeID)
                                continue;

                            if (tempQuest.u1Delemiter1 > 0 && monsterInfo.u1Element != tempQuest.u1Delemiter1)
                                continue;

                            if (tempQuest.u1Delemiter2 > 0 && monsterInfo.u1MonsterType != tempQuest.u1Delemiter2)
                                continue;

                            if (tempQuest.u1Delemiter3 > 0 && Legion.Instance.SelectedDifficult != tempQuest.u1Delemiter3)
                                continue;

                            questMark[0].SetActive(true);
                            return;
                        }
                    }
				}
			}else if(tempQuest.u1QuestType == 29){
				if(tempQuest.u2QuestTypeID > 0){
					if(stageInfo.CheckRewardInStage(tempQuest.u2QuestTypeID) > 0)
					{
						questMark[0].SetActive (true);
						questMark[2].SetActive (true);
						return;
					}
				}
			}
		}
	}

	protected void SetStageMoreInfo(StageInfo stageInfo, Byte selectDiffulct)
	{
		if(_textRecommendEquipInfo == null)
			return;

		ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetInfo(stageInfo.smithID[selectDiffulct-1]);
		if(forgeInfo == null)
		{
			_textRecommendEquipInfo.gameObject.SetActive(false);
			return;
		}
		_textRecommendEquipInfo.gameObject.SetActive(true);

		string recommend = TextManager.Instance.GetText(forgeInfo.sName);
		int idx = recommend.LastIndexOf('<');
		if(idx > 0)
		{
			recommend =  recommend.Insert(idx, TextManager.Instance.GetText(stageInfo.recommendEquipStringKey[selectDiffulct-1]));
			_textRecommendEquipInfo.text = string.Format(TextManager.Instance.GetText("stage_equip_info"), recommend , TextManager.Instance.GetText(stageInfo.recommendEquipPartsCountKey[selectDiffulct - 1]+"part"));// 추천 장비 정보
		}
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
                ExpBoostIconActice(false);
            }
            else if(tsLeftTime.TotalSeconds < 0)
            {
                EventInfoMgr.Instance.u4ExpBoostPer = 0;
                EventInfoMgr.Instance.dicEventReward.Remove(_eventID);
                ExpBoostIconActice(false);
            }
            else
            {
                ExpBoostIconActice(true);
                EventInfoMgr.Instance.u4ExpBoostPer = tempItem.recordValue;
            }
        }
        else
            ExpBoostIconActice(false);

        if (EventInfoMgr.Instance.lstGoldBuffEvent.Count != 0)
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
                GoldBoostIconActice(false);
            }
            else if(tsLeftTime.TotalSeconds < 0)
            {
                EventInfoMgr.Instance.u4GoldBoostPer = 0;
                EventInfoMgr.Instance.dicEventReward.Remove(_eventID);
                GoldBoostIconActice(false);
            }
            else
            {
                GoldBoostIconActice(true);
                EventInfoMgr.Instance.u4GoldBoostPer = tempItem.recordValue;
            }
        }
        else
            GoldBoostIconActice(false);
    }
    //소탕권 갱신
    public void RefreshTicket()
    {
		if(Legion.Instance.cInventory.dicItemKey.ContainsKey(Server.ConstDef.TICKET_SWEEP))
		{
			sweepTickeText.text = ((ConsumableItem)Legion.Instance.cInventory.dicInventory[Legion.Instance.cInventory.dicItemKey[Server.ConstDef.TICKET_SWEEP]]).u2Count.ToString();
		}
		else
		{	
			sweepTickeText.text = "0";
		}        
    }
	
    // 스테이지 시작
	public void OnClickStart()
	{
		if (!Legion.Instance.CheckEmptyInven ())
			return;
		
        StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[Legion.Instance.u2SelectStageID];
        ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[stageInfo.u2ChapterID];

		if (stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.STAGE) {
			if (AssetMgr.Instance.CheckDivisionDownload (2, stageInfo.u2ChapterID)) {
				return;
			}
		}else if (stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST) {
			if (AssetMgr.Instance.CheckDivisionDownload (3, stageInfo.u2ChapterID)) {
				return;
			}
		}else if (stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER) {
			if (AssetMgr.Instance.CheckDivisionDownload (4, 0)) {
				return;
			}
		}

		// 아직 오픈되지 않은 스테이지 라면
		if(stageInfo.IsOpen(Legion.Instance.SelectedDifficult) == false)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("popup_desc_not_clear"), null);
			return;
		}
        
        //키가 없으면 불가능
		if(!Legion.Instance.CheckEnoughGoods(chapterInfo.GetConsumeGoods()))
		{
			PopupManager.Instance.ShowChargePopup(chapterInfo.GetConsumeGoods().u1Type);
			return;
		}

		// 2016. 10. 17 jy 탑 액트만 재화 오버 확인을 한다
		if(chapterInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER)
		{
			// 이미 클리어한 층은 스타트 시키지 않는다
			if(stageInfo.IsClear(Legion.Instance.SelectedDifficult) == true)
				return;
				
			for(Byte i = 0; i < stageInfo.arrRewardItemNum[Legion.Instance.SelectedDifficult - 1]; ++i)
			{
				RewardItem rewardItem = stageInfo.GetRewardInfoByIndex((Byte)(Legion.Instance.SelectedDifficult - 1), i);
				if(rewardItem.bInit != false)
				{
					if(Legion.Instance.CheckGoodsLimitExcessx(rewardItem.cRewards.u1Type) == true)
					{
						Legion.Instance.ShowGoodsOverMessage(rewardItem.cRewards.u1Type);	
						return;
					}
				}
			}
		}

		// 입장 가능한 레벨인지 체크하며 반환값이 1이 아니라면 팝업을 띄운다
		int restrictLevel = stageInfo.CheckRestrictLevel(Legion.Instance.SelectedDifficult - 1);
		if(restrictLevel != 1)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), string.Format(TextManager.Instance.GetText("popup_stage_lv_limit"), restrictLevel), null);
			return;
		}
        
        // 재료 수집 정보가 있을때는 별도의 반복 전투 팝업을 띄워준다
        if(StageInfoMgr.Instance.IsRepeatItemInfo())
        {
            Legion.Instance.AUTOCONTINUE = true;
            string msg = string.Format(TextManager.Instance.GetText("repeat_battle_desc"), stageInfo.RepeatGoods().u4Count);
            PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("repeat_battle"), msg, RequestStartStage, null);    
            return;
        }

        RequestStartStage(null);
    }

    public void RequestStartStage(object[] param)
    {
        StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[Legion.Instance.u2SelectStageID];
        // 반복 전투를 요청하는 상황에서는 돈을 체크한다
        if (Legion.Instance.AUTOCONTINUE == true)
        {
            Goods repeatGoods = stageInfo.RepeatGoods();
            if (Legion.Instance.CheckEnoughGoods(repeatGoods) == false)
            {
                PopupManager.Instance.ShowChargePopup(repeatGoods.u1Type);
                return;
            }
        }

        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.StartStage(Legion.Instance.cBestCrew, stageInfo, Legion.Instance.SelectedDifficult, AckStartStage, Legion.Instance.AUTOCONTINUE);
    }
    
    public void OnClickSweepButton()
	{
		if(onClickSweep != null)
			onClickSweep();
	}
	
	public void OnClickDispatchButton(int crewIdx)
	{
		if(onClickDispatch != null)
			onClickDispatch(crewIdx);
	}
	
	public void OnClickClose()
	{
		// 2016. 09. 29. jy
		// 창을 닫을시 반복전투를 해제한다
		Legion.Instance.AUTOCONTINUE = false;
        StageInfoMgr.Instance.RepeatItemInfoDelete();
        PopupManager.Instance.RemovePopup(gameObject);        
		gameObject.SetActive(false);
	}

	public void OnClickGuideButton()
	{
		if (onClickGuide != null)
			onClickGuide ();
	}

	public void OnClickRepeat(){
		Legion.Instance.AUTOCONTINUE = m_ToggleBtnAuto.GetComponent<Toggle> ().isOn;
		if (Legion.Instance.AUTOCONTINUE) 
		{
			StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[Legion.Instance.u2SelectStageID];
			string title = TextManager.Instance.GetText ("repeat_battle");
			string msg = string.Format(TextManager.Instance.GetText ("repeat_battle_desc"), stageInfo.RepeatGoods().u4Count);
			PopupManager.Instance.ShowBigOKPopup (title, msg, null);
		}
	}

    public void OnClickGoToCrew()
    {
        // 튜토리얼 중에는 작동시키지 않는다
        if (Legion.Instance.cTutorial.bIng == true)
            return;

        if (GoToCrewBtn.activeSelf == false)
            return;

        FadeEffectMgr.Instance.QuickChangeScene(MENU.CREW);
    }
	
	private void AckStartStage(Server.ERROR_ID err)
	{
	 	DebugMgr.Log(err);

	 	if(err != Server.ERROR_ID.NONE)
	 	{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_START, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			if(Legion.Instance.AUTOCONTINUE == true)
			{
				StageInfo stageInfo = null;
                StageInfoMgr.Instance.dicStageData.TryGetValue(Legion.Instance.u2SelectStageID, out stageInfo);
                if (stageInfo != null)
					Legion.Instance.SubGoods(stageInfo.RepeatGoods());
			}

            StartCoroutine("ChangeScene");
		}
        
        PopupManager.Instance.CloseLoadingPopup();
	}
    	
	private IEnumerator ChangeScene()
	{
		FadeEffectMgr.Instance.FadeOut(1f);
		yield return new WaitForSeconds(1f);
		AssetMgr.Instance.SceneLoad("Battle");
	}

    // 골드 부스터 이벤트 아이콘 비/활성화 함수
    protected void GoldBoostIconActice(bool isActice)
    {
        if (BoostGold == null)
            return;

        BoostGold.SetActive(isActice);
    }
    // 경험치 부스터 이벤트 아이콘 비/활성화 함수
    protected void ExpBoostIconActice(bool isActice)
    {
        if (BoostExp == null)
            return;

        BoostExp.SetActive(isActice);
    }
}
