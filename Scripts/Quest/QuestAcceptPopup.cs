using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class QuestAcceptPopup : YesNoPopup {
	public Image Icon;

	public Image Gague;
	public Text Progress;

	public Button btnOK;
	public Button btnGiveUp;
	public Button btnGoTo;
	
	public Transform rewardsParent;
	GameObject ItemSlot;

	MENU shortCut;
    int shortCutPopup;
	UInt16 shortCutChapter;
    UInt16 questTypeID;

	public RectTransform IconTrans;

	QuestInfo questInfo;
	private bool isScript = false;

	void Awake(){
		ItemSlot = AssetMgr.Instance.AssetLoad ("Prefabs/UI/Common/ItemSlot.prefab", typeof(GameObject)) as GameObject;
	}
	    
	public void SetPopup(QuestInfo info){
		isScript = false;
        PopupManager.Instance.AddPopup(this.gameObject, CloseMe);
        questInfo = info;
        PopupManager.Instance.bUseNotice = false;
		string iconName = "main";
		
		if(info.u1MainType > 1) iconName = "sub";
		
		Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_quest_"+iconName);

		for (int i=0; i<info.acReward.Length; i++) {
			if(info.acReward[i] == null) continue;
			GameObject temp = Instantiate(ItemSlot) as GameObject;
			temp.transform.SetParent(rewardsParent);
			temp.transform.localScale = Vector3.one;
			temp.GetComponent<UI_ItemListElement_Common>().SetData(info.acReward[i]);
			//temp.GetComponent<RewardButton>().SetButton(info.acReward[i].u1Type, info.acReward[i].u2ID);
		}

		AddExpPotion (info.u4RewardExp);

		shortCut = (MENU)info.u2ShortCut;
        shortCutPopup = info.u2ShortCutDetail;
		shortCutChapter = info.u2ShortCutChapter;
        questTypeID = info.u2QuestTypeID;

		if (lbl_title.preferredWidth > 200f) {
			IconTrans.anchoredPosition = new Vector2(IconTrans.anchoredPosition.x - ((lbl_title.preferredWidth-200f)/2f), IconTrans.anchoredPosition.y);
		}
	}

	public void SetGauge(float cur, float max){
		Progress.text = cur.ToString("0") + "/" + max.ToString("0");
		Gague.fillAmount = cur / max;
	}


	public void QuestAreaShortcut(QuestInfo info)
	{
		isScript = true;
		questInfo = info;

		shortCut = (MENU)info.u2ShortCut;
		shortCutPopup = info.u2ShortCutDetail;
		shortCutChapter = info.u2ShortCutChapter;
		questTypeID = info.u2QuestTypeID;

		ClickGoTo();
	}

    public void ClickGoTo()
    {
        byte diffecut = 1;

        if (shortCutPopup == 71)
            shortCutPopup = 74;
        else if (shortCutPopup == 72)
            shortCutPopup = 75;
        else if (shortCutPopup == 73)
            shortCutPopup = 76;

        switch (shortCutPopup)
        {
            case 75: diffecut = 2; break;
            case 76: diffecut = 3; break;
        }

        StageInfoMgr.Instance.ShortCutChapter = -1;
        int selectedStage = -1;
        if (shortCut == MENU.CAMPAIGN && (questInfo.u1QuestType == 2 || questInfo.u1QuestType == 18 || questInfo.u1QuestType == 19) && questTypeID != 0)
        {
            if (!StageInfoMgr.Instance.IsOpen(questTypeID, diffecut))
            {
                PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_quest_shoutcut_title"),
                    TextManager.Instance.GetText("popup_quest_shoutcut_desc"),
                    TextManager.Instance.GetText("btn_achi_shortcut"), QuickChangeScene, null);
                return;
            }
            selectedStage = questTypeID;
            StageInfoMgr.Instance.ShortCutChapter = -1;
        }
        else if (questInfo.u1QuestType == 29 && questTypeID != 0)
        {
            StageInfoMgr.Instance.LastPlayStage = -1;
            if (StageInfoMgr.Instance.dicChapterData.ContainsKey((ushort)shortCutChapter) == true)
            {
                ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[(ushort)shortCutChapter];
                // 스테이지 마지막 부터 확인 하도록
                for (int i = chapterInfo.lstStageID.Count - 1; i >= 0; --i)
                {
                    // 오픈되지 않은 스테이지는 검사하지 않는다
                    if (StageInfoMgr.Instance.IsOpen(chapterInfo.lstStageID[i], diffecut) == false)
                        continue;

                    StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[chapterInfo.lstStageID[i]];
                    // 재료 수집 퀘스트
                    if (questInfo.u1QuestType == 29)
                    {
                        if (stageInfo.CheckRewardInStage(questTypeID) > 0)
                            selectedStage = stageInfo.u2ID;
                    }
                }
            }
        }
        // 몬스터 토벌 퀘스트
        else if (questInfo.u1QuestType == 15 && questTypeID != 0)
        {
            bool isChapterID = false;
            ChapterInfo chapterInfo = null;
            StageInfoMgr.Instance.dicChapterData.TryGetValue(questTypeID, out chapterInfo);
            // questTypeID 가 챕터 ID일 경우 
            if (chapterInfo != null)
                isChapterID = true;
            else
            {
                StageInfoMgr.Instance.dicChapterData.TryGetValue(shortCutChapter, out chapterInfo);
            }

            if(chapterInfo != null)
            {
                bool isFind = false;
                for (int i = chapterInfo.lstStageID.Count - 1; i >= 0; --i)
                {
                    // 오픈되지 않아았다면 체크 하지 않음
                    if (StageInfoMgr.Instance.IsOpen(chapterInfo.lstStageID[i], diffecut) == false)
                        continue;

                    StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[chapterInfo.lstStageID[i]];
                    for (int j = 0; j < stageInfo.acPhases.Length; j++)
                    {
                        FieldInfo fieldInfo = StageInfoMgr.Instance.GetFieldInfo(stageInfo.acPhases[j].u2FieldID);
                        if (fieldInfo == null)
                            continue;

                        for (int k = 0; k < fieldInfo.acMonsterGroup.Length; k++)
                        {
                            for (int n = 0; n < fieldInfo.acMonsterGroup[k].acMonsterInfo.Length; n++)
                            {
                                ClassInfo monsterInfo = ClassInfoMgr.Instance.GetInfo(fieldInfo.acMonsterGroup[k].acMonsterInfo[n].u2MonsterID);
                                if (monsterInfo == null)
                                    continue;

                                // 챕터 아이디가 아니라면 몬스터 아이디와 비교한다
                                if (!isChapterID && questTypeID != monsterInfo.u2ID)
                                    continue;

                                // 퀘스트 조건을 체크한다
                                if (questInfo.u1Delemiter1 > 0 && questInfo.u1Delemiter1 != monsterInfo.u1Element)
                                    continue;

                                if (questInfo.u1Delemiter2 > 0 && questInfo.u1Delemiter2 != monsterInfo.u1MonsterType)
                                    continue;

                                if (questInfo.u1Delemiter3 > 0 && questInfo.u1Delemiter3 != diffecut)
                                    continue;

                                selectedStage = stageInfo.u2ID;
                                isFind = true;
                                break;
                            }
                            if (isFind == true) break;
                        }
                        if (isFind == true) break;
                    }
                    if (isFind == true) break;
                }
            }
        }
		else
		{
			StageInfoMgr.Instance.ShortCutChapter = shortCutChapter;
		} 

		if (selectedStage == -1 && StageInfoMgr.Instance.ShortCutChapter == -1) 
		{
			PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText ("popup_quest_shoutcut_title"),
				TextManager.Instance.GetText ("popup_quest_shoutcut_desc"),
				TextManager.Instance.GetText("btn_achi_shortcut"), QuickChangeScene, null);
			return;
		}

		StageInfoMgr.Instance.LastPlayStage = selectedStage;
		FadeEffectMgr.Instance.QuickChangeScene(shortCut, shortCutPopup);
		CloseMe();
	}

	// 이전 검색 코드
	void BaseCheckQuest()
	{
		/*
		else if (questInfo.u1QuestType == 15 && questTypeID != 0)
		{
			bool isFind = false;
			bool isFindStage = false;
			StageInfoMgr.Instance.LastPlayStage = -1;
			foreach(ActInfo actInfo in StageInfoMgr.Instance.dicActData.Values){
				for(int a=0; a<actInfo.lstChapterID.Count; a++){
					ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData [actInfo.lstChapterID [a]];
					for(int b=0; b<chapterInfo.lstStageID.Count; b++)
					{
						StageInfo stageInfo = StageInfoMgr.Instance.dicStageData [chapterInfo.lstStageID [b]];
						if( StageInfoMgr.Instance.IsOpen (stageInfo.u2ID, questInfo.u1Delemiter1) )
						{
							for(int i=0; i<stageInfo.acPhases.Length; i++){
								for(int j=0; j<StageInfoMgr.Instance.GetFieldInfo(stageInfo.acPhases[i].u2FieldID).acMonsterGroup.Length; j++){
									for(int k=0; k<StageInfoMgr.Instance.GetFieldInfo(stageInfo.acPhases[i].u2FieldID).acMonsterGroup[j].acMonsterInfo.Length; k++){
										ushort monID = StageInfoMgr.Instance.GetFieldInfo(stageInfo.acPhases[i].u2FieldID).acMonsterGroup[j].acMonsterInfo[k].u2MonsterID;
										if (questTypeID == monID ) //&& StageInfoMgr.Instance.IsOpen (stageInfo.u2ID, questInfo.u1Delemiter1)) 
										{
											isFindStage = true;
											break;
										}
									}
									if(isFindStage) break;
								}
								if(isFindStage) break;
							}

							if(isFindStage)
							{
								if( selectedStage == -1 )
									selectedStage = stageInfo.u2ID;
								else
								{
									if( StageInfoMgr.Instance.dicStageData[(UInt16)selectedStage].actInfo.u1Mode != stageInfo.actInfo.u1Mode )
										isFind = true;
									else
										selectedStage = stageInfo.u2ID;
								}
								isFindStage = false;
							}
						}
						if(isFind) break;
					}
					if(isFind) break;
				}
				if(isFind) break;
			}
		}
		else if (questInfo.u1QuestType == 29 && questTypeID != 0)
		{
			StageInfoMgr.Instance.LastPlayStage = -1;
			foreach(ActInfo actInfo in StageInfoMgr.Instance.dicActData.Values)
			{
				for(int a=0; a<actInfo.lstChapterID.Count; a++)
				{
					ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData [actInfo.lstChapterID [a]];

					for(int b=0; b<chapterInfo.lstStageID.Count; b++){
						StageInfo stageInfo = StageInfoMgr.Instance.dicStageData [chapterInfo.lstStageID [b]];
						if(stageInfo.CheckRewardInStage(questTypeID) > 0)
						{
							if (!StageInfoMgr.Instance.IsOpen (stageInfo.u2ID, questInfo.u1Delemiter1)) {
								PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText ("popup_quest_shoutcut_title"),
									TextManager.Instance.GetText ("popup_quest_shoutcut_desc"),
									TextManager.Instance.GetText("btn_achi_shortcut"), QuickChangeScene, null);
								
								//string title = TextManager.Instance.GetText ("popup_title_not_clear");
								//string msg = TextManager.Instance.GetText ("popup_desc_not_clear");
								//PopupManager.Instance.ShowOKPopup (title, msg, QuickChangeScene);

								return;
							}
							//selectedStage = stageInfo.u2ID;
						}
					}
				}
			}
		}
		*/
	}

	public void QuickChangeScene(object[] param)
	{
        // 시작하기전 마지막으로 간 스테이지를 초기화 한다
        StageInfoMgr.Instance.LastPlayStage = -1;

        // 퀘스트의 난이도를 통일 시킨다
        if (shortCutPopup == 71)
            shortCutPopup = 74;
        if (shortCutPopup == 72)
            shortCutPopup = 75;
        if (shortCutPopup == 73)
            shortCutPopup = 76;
        
        // 챕터가 존재하는지 찾는다
        if (StageInfoMgr.Instance.dicChapterData.ContainsKey((UInt16)shortCutChapter) == true)
        {
            ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[(UInt16)shortCutChapter];
            Byte difficulty = (Byte)Server.ConstDef.MinDifficult;
            switch (shortCutPopup)
            {
                case 75: difficulty += 1; break;
                case 76: difficulty += 2; break;
            }

            // 2017. 01. 19 jy
            while (difficulty > 0)
            {
                // 퀘스트의 챕터의 해당 난이도에 첫번째 스테이지가 오픈되어 있지 않다면
                if (StageInfoMgr.Instance.IsOpen(chapterInfo.lstStageID[0], difficulty) == false)
                {
                    // 이전 챕터가 존재한다면 이전 챕터로 변경 변경후 다시 체크
                    if (chapterInfo.prevChapter != null)
                        chapterInfo = chapterInfo.prevChapter;
                    else
                    {
                        // 이전 챕터가 존재하지 않는다면 난이도를 한단계 낮추고 챕터를 다시 셋팅후 체크한다
                        --difficulty;
                        if(difficulty < Server.ConstDef.MinDifficult)
                        {
                            DebugMgr.LogError("열린 스테이지가 없음");
                            return;
                        }
                        chapterInfo = StageInfoMgr.Instance.dicChapterData[(UInt16)shortCutChapter];
                    }
                }
                else
                {
                    break;
                }
            }

            // 퀘스트 챕터에서 클리어되지 않은 스테이지를 찾아 셋팅한다
            for (int i = 0; i < chapterInfo.lstStageID.Count; ++i)
            {
                if (StageInfoMgr.Instance.IsOpen(chapterInfo.lstStageID[i], difficulty) == false)
                    break;

                StageInfoMgr.Instance.LastPlayStage = chapterInfo.lstStageID[i];
            }
        }
        else
        {
            DebugMgr.LogError("shortCutChapterID not Setting!! shortCutChapterID = " + shortCutChapter);
        }
        
		FadeEffectMgr.Instance.QuickChangeScene(shortCut, shortCutPopup);
		CloseMe();
	}

	public void CloseMe(){
		if(isScript == false)
		{
        	PopupManager.Instance.RemovePopup(this.gameObject);
			Destroy(gameObject);
		}
	}

	//경험치물약을 위한 인벤 공간 검사
	void AddExpPotion(UInt32 u4Exp)
	{
		//경험치 계산
		Dictionary<UInt16, ConsumableItemInfo> tempConsumeItem = new Dictionary<ushort, ConsumableItemInfo>();
		List<ConsumableItemInfo> lstConsumeItem = new List<ConsumableItemInfo>();
		tempConsumeItem = ItemInfoMgr.Instance.GetConsumableItemInfo();
		Byte itemCnt = 0;
		for(int i=tempConsumeItem.Count; i>0;)
		{
			if(u4Exp < tempConsumeItem[(ushort)(58000+i)].u4Exp)
			{
				if(itemCnt != 0)
				{
					GameObject temp = Instantiate(ItemSlot) as GameObject;
					temp.transform.SetParent(rewardsParent);
					temp.transform.localScale = Vector3.one;
					temp.GetComponent<UI_ItemListElement_Common>().SetData(new Goods((Byte)GoodsType.CONSUME, (ushort)(58000+i), itemCnt));
					//temp.GetComponent<RewardButton>().SetButton((Byte)GoodsType.CONSUME, (ushort)(58000+i));
				}
				i--;
				itemCnt = 0;
				continue;
			}
			else
			{
				if(i != 4)
				{
					u4Exp -= tempConsumeItem[(ushort)(58000+i)].u4Exp;
					itemCnt++;
				}
				else
				{
					i--;
				}
			}
		}
	}
}
