using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

//파견을 보내거나 파견중을 보여주는 팝업
public class DispatchWindow : MonoBehaviour {

	public ChapterTab chapterTab;
	public Text title;
	public Text stageTitle;
	public GameObject[] characterSlots;
	public Image[] portrait;
	public Text[] charName;
	public Text[] charLevel;
	public Image[] charElement;
	public Text[] charExpText;
	public Image[] charExpGague;
	
	public Text crewName;
	public Image crewFlag;
	public Image crewFlagNumber;
	
	//Done
	//public GameObject resultRewardObj;
	public UI_ItemListElement_Common[] _cRewardSlot;

	public Text goldValue;
	public Text expValue;
	
	public Text timeText;
	public Text timeValue;
	
	public GameObject cancelPopup;
	public GameObject donePopup;
	public Text donePrice;
	
	public GameObject btnDispatch;
	public GameObject btnCancel;
	public GameObject btnDone;
	public GameObject btnRepeat;	
	public Animator animator;
	public Animator cancelAnim;
	public Animator doneAnim;
    
    public GameObject[] levelUp;
	
	StageInfo stageInfo;
	Crew crew;
	Byte stageDiffect;
    UInt32 u4DoneCostCount; // 빠른 완료 재화

    public GameObject questMark;
	
	public void SetDispatchWindow(UInt16 stageID, int crewIdx)
	{
        for(int i=0; i<levelUp.Length; i++)
            levelUp[i].SetActive(false);        
        
		animator.Play("Popup_Show");
		
        //크루정보 설정
		stageInfo = StageInfoMgr.Instance.dicStageData[stageID];
		crew = Legion.Instance.acCrews[crewIdx];
		crewFlag.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.division_" + (crewIdx + 1));
		crewFlagNumber.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.num_s_" + (crewIdx + 1));
		crewFlagNumber.SetNativeSize();
		crewName.text = TextManager.Instance.GetText("mark_direction_crew") + (crewIdx + 1);

        // 빠른 완료 및 파견 취소 팝업 닫기
        CloseCancelPopup();
        CloseDonePopup();
		//파견중이 아닐경우 처리
		if(crew.DispatchStage == null)
		{
            questMark.SetActive(false);
            btnDispatch.SetActive(true);
			btnCancel.SetActive(false);
			btnDone.SetActive(false);
			btnRepeat.SetActive(false);	
			title.text = TextManager.Instance.GetText("popup_stage_mark_dispatch");
			stageDiffect = Legion.Instance.SelectedDifficult;

			if (Legion.Instance.cQuest.CheckQuestAlarm (MENU.CAMPAIGN, 0))
            {
				QuestInfo tempQuest = Legion.Instance.cQuest.CurrentQuest ();
                if (tempQuest.u1QuestType == 18)
                {
                    Byte bossType = stageInfo.u1BossType;
                    if (bossType < 2) bossType = 1;
                    if (tempQuest.u2QuestTypeID > 0)
                    {
                        if (stageID == tempQuest.u2QuestTypeID)
                        {
                            if ((tempQuest.u1Delemiter1 == 0 || tempQuest.u1Delemiter1 == stageDiffect)
                                && (tempQuest.u1Delemiter2 == 0 || tempQuest.u1Delemiter2 == bossType)
                               && (tempQuest.u1Delemiter3 == 0 || tempQuest.u1Delemiter3 == stageInfo.GetActInfo().u1Number))
                            {
                                questMark.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        if ((tempQuest.u1Delemiter1 == 0 || tempQuest.u1Delemiter1 == stageDiffect)
                            && (tempQuest.u1Delemiter2 == 0 || tempQuest.u1Delemiter2 == bossType)
                           && (tempQuest.u1Delemiter3 == 0 || tempQuest.u1Delemiter3 == stageInfo.GetActInfo().u1Number))
                        {
                           questMark.SetActive(true);
                        }
                    }
                }
			}
		}
		else
		{
			stageInfo = crew.DispatchStage;
			stageDiffect = crew.StageDifficulty;
			btnDispatch.SetActive(false);
			btnCancel.SetActive(true);
            if(stageInfo.actInfo.u1Mode != ActInfo.ACT_TYPE.TOWER)
			    btnDone.SetActive(true);
            else
                btnDone.SetActive(false);
            btnRepeat.SetActive(false);
			title.text = TextManager.Instance.GetText("popup_title_dispatch_ing");
		}
		
		timeText.gameObject.SetActive(true);
		timeValue.gameObject.SetActive(true);
		
		stageTitle.text = TextManager.Instance.GetText(stageInfo.sName);
		SetCharInfo();
		SetRewards();
		SetTime();
        
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);		
	}
	
    //파견 결과 처리
	public void SetDispatchResult(UInt16 stageID, int crewIdx)
	{
        for(int i=0; i<levelUp.Length; i++)
            levelUp[i].SetActive(false);
        
		animator.Play("Popup_Show");
		stageInfo = StageInfoMgr.Instance.dicStageData[stageID];
		//크루 정보
		crew = Legion.Instance.acCrews[crewIdx];
		stageDiffect = crew.StageDifficulty;

        // 경험치 보상 처리
        for(int i=0; i<Server.ConstDef.HeroInCrew; i++)
        {
            if(Legion.Instance.acCrews[crewIdx].acLocation[i] != null)
            {					
                Hero hero = (Hero)Legion.Instance.acCrews[crewIdx].acLocation[i];
				//Byte level = hero.GetComponent<LevelComponent>().AddExp(stageInfo.arrGetExp[crew.StageDifficulty-1] + StageInfoMgr.Instance.u8AddedExp);
                Byte level = hero.GetComponent<LevelComponent>().AddExp(StageInfoMgr.Instance.u8AddedExp);
                if(level > 0)
                {
                    //레벨이 올랐을 경우 레벨업 연출
                    StartCoroutine(ShowExp(i));
                }
            }
        }	
				
		title.text = TextManager.Instance.GetText("popup_title_dispatch_done");
		stageTitle.text = TextManager.Instance.GetText(stageInfo.sName);
		crewFlag.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.division_" + (crewIdx + 1));
		crewFlagNumber.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.num_s_" + (crewIdx + 1));
		crewFlagNumber.SetNativeSize();
		crewName.text = TextManager.Instance.GetText("mark_direction_crew") + (crewIdx + 1);
		
		btnDispatch.SetActive(false);
		btnCancel.SetActive(false);
		btnDone.SetActive(false);
		btnRepeat.SetActive(true);
		
		timeText.gameObject.SetActive(false);
		timeValue.gameObject.SetActive(false);
		
		SetCharInfo();

		if(stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER)
			SetTowerResultRewards();
		else
			SetResultRewards();	        
		
	}    
    private IEnumerator ShowExp(int index)
    {
        yield return new WaitForSeconds(0.1f);
        levelUp[index].SetActive(true);
    }
	
    //크루에 포함된 캐릭터 정보 세팅
	private void SetCharInfo()
	{
		// 2016. 08. 24 jy 
		// 크루 1번 슬롯이 비어 있다면 파견창 1번째 슬롯이 비활성화 되어 보기 안좋아 보여서 수정
		int slotCount = portrait.Length;
		int characterSlotIndex = 0;
		for(int i = 0; i<portrait.Length; ++i)
		{
			if(crew.acLocation[i] == null)
			{
				characterSlots[--slotCount].SetActive(false);
			}
			else
			{
				characterSlots[characterSlotIndex].SetActive(true);
				portrait[characterSlotIndex].sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + crew.acLocation[i].cClass.u2ID);
				portrait[characterSlotIndex].SetNativeSize();
				charName[characterSlotIndex].text = crew.acLocation[i].sName;
				charLevel[characterSlotIndex].text = crew.acLocation[i].cLevel.u2Level.ToString();
				charElement[characterSlotIndex].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + (((Hero)crew.acLocation[i]).GetHeroElement() - 1));
				charExpText[characterSlotIndex].text = string.Format(TextManager.Instance.GetText("text_exp") + " {0}/{1}", ConvertExpValue(crew.acLocation[i].cLevel.u8Exp), ConvertExpValue(crew.acLocation[i].cLevel.u8NextExp));
				
				float per = (float)((float)crew.acLocation[i].cLevel.u8Exp / (float)crew.acLocation[i].cLevel.u8NextExp);
				charExpGague[characterSlotIndex].fillAmount = per;
				++characterSlotIndex;
			}
		}
	}
	
    // 보상 정보 보여줌
	private void SetRewards()
	{
		byte selectedDifficult;
		int count = 0;
		// 파견중이 아니라면 스테이지 정보에서 보상 아이템을 셋팅한다
		if( crew.DispatchStage == null )
		{
			selectedDifficult = Legion.Instance.SelectedDifficult;
			if(stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER )
				count = SetTowerRewardInfo(selectedDifficult);
			else
				count = SetBaseRewardInfo(selectedDifficult);
		}
		else
		{
			selectedDifficult = crew.StageDifficulty;
			// 16. 6. 16 jy
			// crew에 셋팅되어 있는 보상 정보를 읽어서 배치한다
			RewardItem[] rewardItems = crew.DispatchRewaerd.GetReward();
			for(int i = 0 ; i< rewardItems.Length; ++i)
			{
				Goods goods = rewardItems[i].cRewards;
				if(goods.u1Type == 0)
					continue;

				_cRewardSlot[i].SetData(goods);

				if (++count == 3)
					break;
			}
		}

		for(int i = 0 ; i < _cRewardSlot.Length; ++ i)
		{
			if(i < count)
				_cRewardSlot[i].gameObject.SetActive(true);
			else
				_cRewardSlot[i].gameObject.SetActive(false);
		}
		if(EventInfoMgr.Instance.u4GoldBoostPer == 0)
		    goldValue.text = stageInfo.acGetGoods[selectedDifficult-1].u4Count.ToString();
        else
        {
            UInt32 tempGold = 0;
            tempGold = (UInt32)(stageInfo.acGetGoods[selectedDifficult-1].u4Count*(float)(EventInfoMgr.Instance.u4GoldBoostPer/100)+stageInfo.acGetGoods[selectedDifficult-1].u4Count);
            goldValue.text = tempGold.ToString();
        }
        if(EventInfoMgr.Instance.u4ExpBoostPer == 0)
		    expValue.text = stageInfo.arrGetExp[selectedDifficult-1].ToString();
        else
        {
            UInt64 tempExp = 0;
            tempExp = (UInt64)(stageInfo.arrGetExp[selectedDifficult-1]*(float)(EventInfoMgr.Instance.u4ExpBoostPer/100)+stageInfo.arrGetExp[selectedDifficult-1]);
            expValue.text = tempExp.ToString();
        }
	}

	// 타워 챕터 보상 목록 셋팅
	private int SetTowerRewardInfo(byte selectedDifficult)
	{
		int rewardStartIndex = 0;
        // 모드가 쉬움이 아니라면
        int maxCondition = (selectedDifficult * Server.ConstDef.MaxDifficult);
		if(selectedDifficult != 1)
			rewardStartIndex =  (selectedDifficult - 1) * Server.ConstDef.MaxDifficult;
		
		int slotCount = 0;
		for( int i = rewardStartIndex ; i < maxCondition; ++i )
		{
			Goods reward = stageInfo.chapterInfo.acTowerRewards[i];
			if(reward.u1Type == 0)
				continue;

			_cRewardSlot[slotCount].SetData(reward);

			if (++slotCount >= 3)
				break;
		}

		return slotCount;
	}

	// 기본 보상 정보를 셋팅 하는 
	private int SetBaseRewardInfo(byte selectedDifficult)
	{
		int count = 0;
		for(int i=0; i< StageInfo.MAX_REWARD_COUNT; i++)
		{
			Goods reward = stageInfo.RewardItems[selectedDifficult-1][i].cRewards;
			if(reward.u2ID != 0)
			{
				// 16.6.14 jy
				// 고정인 보상 경우에만 획득 아이템의 아이콘을 보여준다
				if( i < StageInfo.CONFIRM_REWORD_COUNT )
					_cRewardSlot[count].SetData(reward, true);
				else
				{
					// 랜덤 보상인 경우에는 ? 아이콘을 띄운다
					_cRewardSlot[count].SetRandemItemData();
					++count;
					break;
				}
				++count;
			}
			if (count == 3)
				break;
		}
		return count;
	}
	  
    // 파견 결과로 받은 보상 보여줌
	private void SetResultRewards()
	{
		RewardItem[] rewardItem = Legion.Instance.cReward.GetReward();       
        int count = 0;

		for(int i=0; i<rewardItem.Length; i++)
        {
			if(rewardItem[i].cRewards.u1Type == 0)
                continue;
			
			_cRewardSlot[count].SetData(rewardItem[count].cRewards);  
            count++;

			if (count == 3)
				break;
        }

		for(int i = 0; i < _cRewardSlot.Length; ++i)
		{
			if(i < count)
				_cRewardSlot[i].gameObject.SetActive(true);
			else
				_cRewardSlot[i].gameObject.SetActive(false);
		}
		
		byte selectedDifficult;
		if(crew.DispatchStage != null)
			selectedDifficult = crew.StageDifficulty;//Legion.Instance.selectedDifficult;
		else
			selectedDifficult = Legion.Instance.SelectedDifficult;
				
        //표기 확인후 작업
		//goldValue.text = stageInfo.acGetGoods[selectedDifficult-1].u4Count.ToString();
        goldValue.text = (Legion.Instance.Gold - StageInfoMgr.Instance.u4PrevTotalGold).ToString();
		//expValue.text = stageInfo.arrGetExp[selectedDifficult-1].ToString();
        expValue.text = StageInfoMgr.Instance.u8AddedExp.ToString();
	}

	private void SetTowerResultRewards()
	{
		RewardItem[] rewardItem = Legion.Instance.cReward.GetReward();

		int count = 0;
		for(int i = 0; i < rewardItem.Length; ++i )
		{
			_cRewardSlot[count].SetData(rewardItem[i].cRewards);

			if (++count == 3)
				break;
		}

		for(int i = 0; i < _cRewardSlot.Length; ++i)
		{
			if(i < count)
				_cRewardSlot[i].gameObject.SetActive(true);
			else
				_cRewardSlot[i].gameObject.SetActive(false);
		}
		byte selectedDifficult;
		if(crew.DispatchStage != null)
			selectedDifficult = crew.StageDifficulty;//Legion.Instance.selectedDifficult;
		else
			selectedDifficult = Legion.Instance.SelectedDifficult;
		
        //표기 확인후 작업
		//goldValue.text = stageInfo.acGetGoods[selectedDifficult-1].u4Count.ToString();
        goldValue.text = (Legion.Instance.Gold - StageInfoMgr.Instance.u4PrevTotalGold).ToString();
		//expValue.text = stageInfo.arrGetExp[selectedDifficult-1].ToString();
        expValue.text = StageInfoMgr.Instance.u8AddedExp.ToString();
	}
	
	private void SetTime()
	{
		byte selectedDifficult;
		if(crew.DispatchStage != null)
			selectedDifficult = crew.StageDifficulty;//Legion.Instance.selectedDifficult;
		else
			selectedDifficult = Legion.Instance.SelectedDifficult;
		
        // 파견중이 아니면 파견 예상 시간을 보여준다
		if(crew.DispatchStage == null)
		{
			int levelGap = Legion.Instance.AverageLevel(crew) -stageInfo.arrRecommandLevel[selectedDifficult-1];
			// - crew.DispatchStage.arrRecommandLevel[selectedDifficult-1];			

			/*
            //레벨 차이에 따라 파견시간이 달라진다
			foreach(int level in StageInfoMgr.Instance.dicDispatchTime.Keys)
			{
				if(level <= levelGap)
					break;
				
				levelID = level;
			}

			UInt32 dispatchTime = stageInfo.u4ExploreTime + timeGap;
			TimeSpan timeSpan = TimeSpan.FromSeconds(dispatchTime);
			timeText.text = TextManager.Instance.GetText ("popup_desc_dispatch_time");
			timeValue.text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
			StageInfoMgr.Instance.dispatchTime = dispatchTime;
			UInt32 timeGap = 0;
			*/
			// 2016. 07. 05 jy
			// 파견 타이머 변경
			// 파견 시간을 저장할 변수
			TimeSpan dispatchTime;
			//  크루 평균 렙보다 던전 권장레벨이 높으면
			if(levelGap < 0)
			{
				// 렙 차이에 따른 추가 시간 데이터에서 찾기
                //int levelID = 0;
				//foreach(int level in StageInfoMgr.Instance.dicDispatchTime.Keys)
				//{
                //    if (level <= levelGap )
                //    {
                //        levelID = level;
                //        break;
                //    }
                //    levelID = level;
				//}
                DispatchTimeInfo dispatchTimeInfo = StageInfoMgr.Instance.GetDispatchInfo(levelGap);
                dispatchTime = TimeSpan.FromSeconds(Legion.Instance.getSubVIPDispatchTime(stageInfo.u4ExploreTime + (dispatchTimeInfo.u2AddIime * 60), true));
                //if (stageInfo.actInfo.u1Mode != ActInfo.ACT_TYPE.TOWER)
                //{
                //    // 찾은 타임을 넣기
                //    //dispatchTime = TimeSpan.FromSeconds(Legion.Instance.getSubVIPDispatchTime(StageInfoMgr.Instance.dicDispatchTime[levelID]));
                //    dispatchTime = TimeSpan.FromSeconds(Legion.Instance.getSubVIPDispatchTime(stageInfo.u4ExploreTime + (dispatchTimeInfo.u2AddIime * 60), true));
                //}
                //else
                //{
                //    //dispatchTime = TimeSpan.FromSeconds(Legion.Instance.getSubVIPDispatchTime(stageInfo.u4ExploreTime + (StageInfoMgr.Instance.dicDispatchTime//[levelID] * 60), true ));
                //
                //}	
			}
			else
			{
				// 기본 타임 넣기
				dispatchTime = TimeSpan.FromSeconds(Legion.Instance.getSubVIPDispatchTime(stageInfo.u4ExploreTime , true));
			}
			timeText.text = TextManager.Instance.GetText ("popup_desc_dispatch_time");
			timeValue.text = string.Format("{0:00}:{1:00}:{2:00}", dispatchTime.Hours, dispatchTime.Minutes, dispatchTime.Seconds);
			StageInfoMgr.Instance.dispatchTime = (uint)(dispatchTime.TotalSeconds);
		} 
		else
		{
            //파견중이면 남은시간 보여줌
			timeText.gameObject.SetActive(true);
			timeValue.gameObject.SetActive(true);
			timeText.text = TextManager.Instance.GetText ("popup_desc_tra_char_ing_titme");
			StartCoroutine(CheckDispatchTime(crew.DispatchTime));
		}
	}

	private IEnumerator CheckDispatchTime(DateTime dispatchTime)
	{
		while(true)
		{
			TimeSpan timeSpan = dispatchTime - Legion.Instance.ServerTime;
			
			if(timeSpan.Ticks > 0)
			{
                int hour = (int)(timeSpan.TotalSeconds / 3600);
                int min = (int)((timeSpan.TotalSeconds % 3600) / 60);
                int sec = (int)((timeSpan.TotalSeconds % 3600) % 60);                     
				timeValue.text = string.Format("{0:00}:{1:00}:{2:00}", hour, min, sec);

                // 빠른 완료 팝업이 오픈되어 있다면
                if(donePopup.activeSelf)
                {
                    SetDonePrice(timeSpan);
                }
			}
			else
			{
				gameObject.SetActive(false);
			}
			
			yield return new WaitForSeconds(1f);
		}
	}
	
	public void OnClickDispatch()
	{
		chapterTab.RequestDispatch(stageInfo, crew, stageDiffect);
	}
	
    //취소 하겠냐는 팝업
	public void OpenCancelPopup()
	{
		cancelAnim.Play("Popup_Show");
		cancelPopup.SetActive(true);
	}
	
    //빠른완료 하겠냐는 팝업
	public void OpenDonePopup()
	{
		doneAnim.Play("Popup_Show");
		donePopup.SetActive(true);

        SetDonePrice(crew.DispatchTime - Legion.Instance.ServerTime);
    }

    private void SetDonePrice(TimeSpan remnantTime)
    {
        // 빠른 완료 금액
        UInt32 constCount = StageInfoMgr.Instance.dicChapterData[stageInfo.u2ChapterID].cReturnGoods.u4Count;
        // 파견 시간이 기본 파견 시간 보다 크다면
        if (remnantTime.TotalSeconds > stageInfo.u4ExploreTime)
        {
            DispatchTimeInfo dispatchTime = StageInfoMgr.Instance.GetDispatchInfo(remnantTime);
            if (dispatchTime != null)
            {
                // 빠른 완료 금액 + (빠른 완료 금액 * 증가 비율 * 0.01f)
                constCount += (UInt32)(Math.Ceiling((double)(constCount * (dispatchTime.u2AddPriceRatio / 100))));
            }
        }
        // 남은 시간이 기본 시간 보다 작고 && 기본 시간 - 남은 시간이 1보다 크면
        else if (remnantTime.TotalSeconds < stageInfo.u4ExploreTime)
        {
            // (빠른 완료 금액 * (남은 파견 시간[분] / 기본 파견 시간[분]))
            constCount = (UInt32)(Math.Ceiling((double)(constCount * remnantTime.TotalSeconds / stageInfo.u4ExploreTime)));
        }

        if (u4DoneCostCount != constCount)
        {
            u4DoneCostCount = constCount;
            donePrice.text = constCount.ToString();
        }
    }
	
	public void CloseCancelPopup()
	{
		cancelPopup.SetActive(false);
	}
	
	public void CloseDonePopup()
	{
		donePopup.SetActive(false);
	}
	
	public void OnClickClose()
	{		
        PopupManager.Instance.RemovePopup(gameObject);
		gameObject.SetActive(false);
		//Legion.Instance.cQuest.CheckEndDirection (AchievementTypeData.Dispatch);
	}
	
	public void OnClickCancel()
	{
		cancelPopup.SetActive(false);
		chapterTab.DispatchCancel(crew);
	}
	
    // 빠른 완료 클릭
	public void OnClickDone()
	{
        //캐쉬 부족하면 불가능
		if(!Legion.Instance.CheckEnoughGoods(stageInfo.GetChapterInfo().cReturnGoods.u1Type, u4DoneCostCount))
		{
			PopupManager.Instance.ShowChargePopup(stageInfo.GetChapterInfo().cReturnGoods.u1Type);
			return;
		}

		gameObject.SetActive(false);
		donePopup.SetActive(false);
		chapterTab.DispatchDone(crew, u4DoneCostCount);
	}

    public string ConvertExpValue(UInt64 u8Exp)
    {
        string strConvertedExp = "0";

        if(u8Exp < 1000)
            return (strConvertedExp = u8Exp.ToString());

        int tempExp = (int)(Math.Log(u8Exp)/Math.Log(1000));
        strConvertedExp = String.Format("{0:F2}{1}", u8Exp/Math.Pow(1000, tempExp), "KMB".ToCharArray()[tempExp-1]);

        return strConvertedExp;
    }
}
