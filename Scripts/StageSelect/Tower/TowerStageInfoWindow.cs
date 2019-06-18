using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TowerStageInfoWindow : StageInfoWindow
{
	public Button Btn_Start;
	public Text[] rewardCount;
	public UI_ItemListElement_Common[] _rewardItemSlot;
	public GameObject towerStageInfo;
    public GameObject CrewListBG;

	private bool m_isFadeInWindow = false;

	public override void SetInfo(UInt16 stageID)
	{
		// 상시 오픈으로 필요 없어짐
		//animator.Play("Popup_Show");
		StageInfo stageInfo;
		StageInfoMgr.Instance.dicStageData.TryGetValue(stageID, out stageInfo);
		if(stageInfo == null)
			return;
		
		byte selectedDifficult = Legion.Instance.SelectedDifficult;
		// 스테이지가 오픈되지 않았다면 스타트 버튼을 비활성화 한다
		if(stageInfo.IsOpen(selectedDifficult) == false)
		{
			Btn_Start.interactable = false;
			Btn_Start.image.color = Color.gray;
		}
		else
		{
			Btn_Start.interactable = true;
			Btn_Start.image.color = Color.white;
		}

		stageTitle.text = TextManager.Instance.GetText(stageInfo.sName);
		int rewardCount = 0;
		// 현재 스테이지의 클리어 여부를 확인 한다
		bool isClear = stageInfo.IsClear(selectedDifficult);
        // 탑의 마지막층 여부를 확인한다
        DispatchLabel.SetActive(isClear);
        if ( stageInfo.IsLastStageInChapter )
		{
            // 스테이지 클리어 여부에 따라 스타트 버튼의 활성화 여부를 넣는다
            Btn_Start.gameObject.SetActive(!isClear);
            // 마지막 층 클리어가 되어 있다면 파견 보상을 띄우고 크루창을 셋팅한다
            if ( isClear )
			{
                CrewListBG.SetActive(true);
                towerStageInfo.SetActive(false);

                int unlockCnt = 0;
                for (int i = 0; i < Legion.Instance.acCrews.Length; i++)
                {
                    if (!Legion.Instance.acCrews[i].IsLock)
                        unlockCnt++;
                }

                rewardCount = SetDispatchReward(stageInfo, selectedDifficult);
                // 크루 갯수가 1개 이하 라면 파견 필요 요소를 안내한다
                if (unlockCnt < 2)
                {
                    crewList.SetActive(false);
                    GoToCrewBtn.SetActive(true);
                    if (Legion.Instance.sName == "")
                        GoToCrewBtn.GetComponent<Button>().interactable = false;
                    else
                        GoToCrewBtn.GetComponent<Button>().interactable = true;
                }
                else
                {
                    crewList.SetActive(true);
                    GoToCrewBtn.SetActive(false);

                    for (int i = 0; i < crewSlots.Length; i++)
                    {
                        crewSlots[i].Init();
                        crewSlots[i].gameObject.AddComponent<TutorialButton>().id = "Dispath_Crew_" + (i + 1).ToString();
                    }
                }
			}
			else 
			{
                CrewListBG.SetActive(false);
                towerStageInfo.SetActive(true);
                // 클리어 하지 못했다면 기본 정보상을 띄운다
                rewardCount = SetBaseReward(stageInfo, selectedDifficult);
			}
		}
		else
		{
            // 마지막층이 아니라면 무조건 스타트 버튼만을 활성화 한다
            CrewListBG.SetActive(false);
            Btn_Start.gameObject.SetActive(true);
			// 마지막 층이 아니라면 기본 스테이지 보상을 띄운다
			rewardCount = SetBaseReward(stageInfo, selectedDifficult);
			towerStageInfo.SetActive(true);
		
		}	
		// 스테이지 정보창이 오픈된 상태라면 이미지를 셋팅한다
		if( towerStageInfo.activeSelf )
		{
			recommandLevel.text = stageInfo.arrRecommandLevel[selectedDifficult - 1].ToString();

			int levelGap = Legion.Instance.AverageLevel(Legion.Instance.cBestCrew) - stageInfo.arrRecommandLevel[selectedDifficult - 1];
			// 레벨 평균과 권장레벨 차이로 난이도를 표기해준다
			if(levelGap >= RECOMMAND_LEVEL_GAP)
				recommandDiffcult.text = TextManager.Instance.GetText("mark_lvl_gap_easy");
			else if(levelGap <= -RECOMMAND_LEVEL_GAP)
				recommandDiffcult.text = TextManager.Instance.GetText("mark_lvl_gap_hard");
			else
				recommandDiffcult.text = TextManager.Instance.GetText("mark_lvl_gap_normal");
			
			// 현제 맵의 속성을 표기한다
			recommandElement.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_icon_" + stageInfo.u1RecommandElement);
			recommandElement.SetNativeSize();
		}

		// 보상 갯수에 따라 보상 아이콘을 비활성화 한다
		//for(int i=0; i<rewardButtons.Length; i++)
		for(int i=0; i<_rewardItemSlot.Length; i++)
		{
			if(i < rewardCount)
				_rewardItemSlot[i].gameObject.SetActive(true);
			else
				_rewardItemSlot[i].gameObject.SetActive(false);
		}

		// 현재 스테이지의 클리어 여부를 확인하여 시작 버튼의 활성화 여부 넣는다 
		Btn_Start.enabled = !isClear;
		if( isClear )
			AtlasMgr.Instance.SetGrayScale(Btn_Start.GetComponent<Image>());
		else
			AtlasMgr.Instance.SetDefaultShader(Btn_Start.GetComponent<Image>());

		// 선택된 맵의 ID를 저장
		Legion.Instance.u2SelectStageID = stageID;
		//PopupManager.Instance.AddPopup(gameObject, OnClickClose);
	}

	private int SetBaseReward(StageInfo stageInfo, byte selectedDifficult)
	{
		int count = 0;
		for(int i=0; i<StageInfo.MAX_REWARD_COUNT; i++)
		{
			Goods reward = stageInfo.RewardItems[selectedDifficult-1][i].cRewards;
			//Byte itemType = stageInfo.RewardItems[selectedDifficult-1][i].cRewards.u1Type;
			//UInt16 itemID = stageInfo.RewardItems[selectedDifficult-1][i].cRewards.u2ID;
			//DebugMgr.Log(reward.u2ID);
			if(reward.u1Type == 0)
				continue;

			// 16. 7. 12 jy 
			// 모든 타입의 보상 아이템을 보여준다
			_rewardItemSlot[count].SetData(reward);
			//rewardsIcons[count].sprite = AtlasMgr.Instance.GetGoodsIcon(reward);
			//rewardsGrades[count].sprite = AtlasMgr.Instance.GetGoodsGrade(reward);
			//rewardCount[count].text = reward.u4Count.ToString();
			//rewardButtons[count].SetButton(reward.u1Type, reward.u2ID);
			//rewardsIcons[count].SetNativeSize();
			/*
			if(itemID != 0)
			{
				// 16.6.14 jy
				// 고정인 보상 경우에만 획득 아이템의 아이콘을 보여준다
				if( i < StageInfo.CONFIRM_REWORD_COUNT )
				{
					rewardsIcons[count].sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(itemID).u2IconID);
					rewardsGrades[count].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(itemID));
					rewardButtons[count].SetButton(itemType, itemID);
					rewardsIcons[count].SetNativeSize();
				}
				else
				{
					// 랜덤 보상인 경우에는 ? 아이콘을 띄운다
					// [임시] 아이콘의 경로를 직접 넣음
					rewardsIcons[count].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_random_reward");
					rewardsGrades[count].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4570");
					rewardButtons[count].SetButton(0, 0);
					rewardsIcons[count].SetNativeSize();

					count++;
					break;
				}
			}*/	
			if (++count == StageInfo.CONFIRM_REWORD_COUNT)
				break;
		}

		return count;
	}

	private int SetDispatchReward(StageInfo stageInfo ,byte selectedDifficult)
	{
		int rewardStartIndex = 0;
		// 모드가 쉬움이 아니라면
		int maxCondition = (selectedDifficult * Server.ConstDef.MaxDifficult) + 1;
		if(selectedDifficult != 1)
			rewardStartIndex =  (selectedDifficult - 1) * Server.ConstDef.MaxDifficult;

		int slotCount = 0;
		for( int i = rewardStartIndex ; i <= maxCondition; ++i )
		{
			Goods reward = stageInfo.chapterInfo.acTowerRewards[i];
			if(reward.u1Type == 0)
				continue;

			_rewardItemSlot[slotCount].SetData(reward);

			if (++slotCount == StageInfo.CONFIRM_REWORD_COUNT)
				break;
		}

		return slotCount;
	}
		
	public void HideWindow()
	{
		//if(m_isFadeInWindow == true)
		//	return;
		
		CanvasGroup canvas = this.GetComponent<CanvasGroup>();
		if(canvas != null)
			canvas.alpha = 0f;	
	}

	public void FadeInWindow()
	{
		if(this.gameObject.activeSelf == false)
			return;
		//if(m_isFadeInWindow == true)
		//	return;

		//m_isFadeInWindow = true;
		StartCoroutine("WindowFadeInCoroutine");
	}

	private IEnumerator WindowFadeInCoroutine()
	{
		CanvasGroup canvas = this.GetComponent<CanvasGroup>();
		if(canvas != null)
		{
			while(true)
			{
				if( canvas.alpha < 1f )
				{
					canvas.alpha += 0.05f;
					yield return null;
				}
				else
				{
					canvas.alpha = 1f;
					//m_isFadeInWindow = false;
					yield break;
				}
			}
		}
		yield break;
	}
}
