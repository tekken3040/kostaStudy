using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class QuestPanel : MonoBehaviour 
{
	protected GameObject _objQuestSlot;
	protected GameObject _objQuestPopup;
	protected GameObject _objRewardPopup;
	protected GameObject depth1Popup;

	public RectTransform _trQuestScrollArea;
	public RectTransform _trQuestSlotPaent;
	public GameObject	_objQuestLine;
	public RectTransform _trPopupParent;

	protected QuestSlot _cSelectedQuestSlot;
	protected QuestListPos ingPos;
	protected QuestListPos idlePos;

    protected Dictionary<UInt16, QuestSlot> dicQuestSlot;

	void Awake()
	{
        dicQuestSlot = new Dictionary<UInt16, QuestSlot>();

        _objQuestSlot = AssetMgr.Instance.AssetLoad ("Prefabs/UI/Quest/QuestSlot.prefab", typeof(GameObject)) as GameObject;
		_objQuestPopup = AssetMgr.Instance.AssetLoad ("Prefabs/UI/Common/Pref_UI_QuestAccept.prefab", typeof(GameObject)) as GameObject;
		_objRewardPopup = AssetMgr.Instance.AssetLoad ("Prefabs/UI/Quest/Pref_UI_QuestReward.prefab", typeof(GameObject)) as GameObject;

		ingPos = new QuestListPos ();
		ingPos.GroupY = -70;
		ingPos.Height = 400;
		ingPos.SlotX = 0;
		ingPos.SlotY = 65;

		idlePos = new QuestListPos ();
		idlePos.GroupY = 0;
		idlePos.Height = 540;
		idlePos.SlotX = 0;
		idlePos.SlotY = 0;

		if(_trQuestSlotPaent.childCount + _objQuestLine.transform.childCount == 0) 
			SetQuestList();
	}

	void OnEnable()
	{
		#if UNITY_ANDROID
		IgaworksUnityAOS.IgaworksUnityPluginAOS.Adbrix.retention("Quest");
		#elif UNITY_IOS
		AdBrixPluginIOS.Retention("Quest");
		#endif

		if(Legion.Instance.cQuest.isClearQuest())
			PopupManager.Instance.SetNoticePopup (MENU.QUEST);
	}

	protected virtual void SetQuestList()
	{
		SetQuestListArea((Legion.Instance.cQuest.u2IngQuest > 0));

		foreach (UserQuest data in Legion.Instance.cQuest.dicQuests.Values) 
		{
			if(!data.bRewarded && data.CheckOpen())
			{
				GameObject temp = Instantiate(_objQuestSlot) as GameObject;
				QuestSlot tempScript = temp.GetComponent<QuestSlot>();
				tempScript.SetSlot(data, false);
				tempScript._btnSlot.onClick.AddListener(() => { SetQuestPopup(tempScript); });
                tempScript._btnAccepBtn.onClick.AddListener(() => { OnClickFastAccept(tempScript); });

                RectTransform trTr = temp.GetComponent<RectTransform>();
				if (Legion.Instance.cQuest.u2IngQuest == data.u2ID) 
				{
					trTr.SetParent (_objQuestLine.transform);
					trTr.localScale = Vector3.one;
					trTr.localPosition = new Vector3(ingPos.SlotX, ingPos.SlotY, 0);
				}
				else 
				{
					trTr.SetParent (_trQuestSlotPaent);
					trTr.localScale = Vector3.one;
					trTr.localPosition = Vector3.zero;

					if (data.GetInfo ().u1MainType == 1)
						trTr.SetAsFirstSibling ();
				}

                dicQuestSlot.Add(data.u2ID, tempScript);
            }
		}
	}

    public void OnClickFastAccept(QuestSlot slotInfo)
    {
        // 클릭한 버튼의 퀘스트 정보가 수락되어 있는 퀘스트와 같다면 바로가기
        if (Legion.Instance.cQuest.u2IngQuest == slotInfo.cInfo.u2ID)
        {
            _cSelectedQuestSlot = slotInfo;
            QuestAreaShortcut();
        }
        // 현재 진행중이 퀘스트가 없다면 퀘스트를 수락한다
        else if(Legion.Instance.cQuest.u2IngQuest == 0)
        {
            _cSelectedQuestSlot = slotInfo;
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.RequestQuestAccept(_cSelectedQuestSlot.cInfo.u2ID, RequestQuestAccept);
        }
    }

    public void SetQuestPopup(QuestSlot info)
	{
		if (Legion.Instance.cQuest.u2IngQuest > 0) 
		{
			if (info.cInfo.u2ID != Legion.Instance.cQuest.u2IngQuest) 
			{
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_quest_ing"), TextManager.Instance.GetText("popup_desc_quest_ing"), null);
				return;
			}
		}

		_cSelectedQuestSlot = info;
		GameObject temp = Instantiate(_objQuestPopup) as GameObject;
		QuestAcceptPopup tempScript = temp.GetComponent<QuestAcceptPopup>();
		QuestInfo quest = info.cInfo.GetInfo();

		tempScript.Show(TextManager.Instance.GetText(quest.sName), TextManager.Instance.GetText(quest.sDescription), TextManager.Instance.GetText(quest.sSummary), null, null);
		tempScript.SetPopup(_cSelectedQuestSlot.cInfo.GetInfo());

		if (info.cInfo.isIng ()) 
		{
			if (info.cInfo.isClear ()) 
			{
				tempScript.SetGauge ((float)Legion.Instance.cQuest.u4QuestCount, (float)info.cInfo.u4MaxCount);
				tempScript.btnOK.transform.FindChild("Text").GetComponent<Text>().text = TextManager.Instance.GetText("mark_direction_quest_done");//"퀘스트 완료";
				tempScript.btnGiveUp.gameObject.SetActive (false);
				tempScript.btnGoTo.gameObject.SetActive (false);
				tempScript.btnOK.onClick.AddListener (() => { GetQuestReward (info); });
			}
			else 
			{
				tempScript.SetGauge ((float)Legion.Instance.cQuest.u4QuestCount, (float)info.cInfo.u4MaxCount);
				tempScript.btnOK.gameObject.SetActive (false);
				tempScript.btnGiveUp.onClick.AddListener (() => { GiveUpQuest (tempScript); });
			}
		} 
		else 
		{
			tempScript.SetGauge(0, (float)info.cInfo.u4MaxCount);
			tempScript.btnGiveUp.gameObject.SetActive (false);
			tempScript.btnGoTo.gameObject.SetActive (false);
			tempScript.btnOK.onClick.AddListener (() => { AcceptQuest (tempScript); });
		}

		RectTransform rectTr = temp.GetComponent<RectTransform>();
		rectTr.SetParent(_trPopupParent);
		rectTr.localScale = Vector3.one;
		rectTr.anchoredPosition3D = Vector3.zero;
		rectTr.sizeDelta = Vector2.zero;

		depth1Popup = temp;
		PopupManager.Instance.AddPopup(depth1Popup, CloseDepth1Popup);
	}

	protected void GetQuestReward(QuestSlot info)
	{
		if(info.cInfo.bRewarded) 
			return;

		if(info.cInfo.isClear() == false) 
			return;

		if (Legion.Instance.CheckEmptyInven() == false)
			return;

		QuestInfo quest = info.cInfo.GetInfo();
		for(int i = 0 ; i < quest.acReward.Length; ++i)
		{
			if( quest.acReward[i].u1Type == 0 )
				continue;

			if(Legion.Instance.CheckGoodsLimitExcessx(quest.acReward[i].u1Type) == true)
			{
				Legion.Instance.ShowGoodsOverMessage(quest.acReward[i].u1Type);
				return;
			}
		}

		CloseDepth1Popup ();
			
		_cSelectedQuestSlot = info;
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequestQuestComplete(RequestQuestReward);
	}
		
	protected virtual void RequestQuestReward(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		DebugMgr.Log(err);
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.QUEST_COMPLETE, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			GameObject temp = Instantiate(_objRewardPopup) as GameObject;
			QuestRewardPopup tempScript = temp.GetComponent<QuestRewardPopup>();
			QuestInfo quest = _cSelectedQuestSlot.cInfo.GetInfo();

			tempScript.Show(TextManager.Instance.GetText("popup_title_quest_reward"), TextManager.Instance.GetText(quest.sName), null, null);
			tempScript.SetPopup(quest);

			RectTransform rtTr = temp.GetComponent<RectTransform>();
			rtTr.SetParent(_trPopupParent);
			rtTr.localScale = Vector3.one;
			rtTr.anchoredPosition3D = Vector3.zero;
			rtTr.sizeDelta = Vector2.zero;

			Legion.Instance.AddGoods(quest.acReward);
			AddExpPotion (quest.u4RewardExp);
			Legion.Instance.cQuest.EndQuest();
			Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.ClearQuest, quest.u2ID, quest.u1MainType, 0, 0, 1);

			CheckOpenQuestList(quest);

			depth1Popup = temp;
			PopupManager.Instance.AddPopup(depth1Popup, CloseDepth1Popup);

			QuestDirection qd = QuestInfoMgr.Instance.GetQuestDirctionInfo(_cSelectedQuestSlot.cInfo.u2ID, 3);
			if(qd != null)
			{
				PopupManager.Instance.SetQuestDirectionPopup(qd, temp);
				tempScript.popup.SetActive (false);
				temp.GetComponent<Animator> ().enabled = false;
			}

            // 완료 되었다면 삭제
            dicQuestSlot.Remove(quest.u2ID);

			_cSelectedQuestSlot.DestroyMe ();
			_cSelectedQuestSlot = null;

			SetQuestListArea(false);
		}
	}

	protected void CheckOpenQuestList(QuestInfo info)
	{
		foreach (UserQuest data in Legion.Instance.cQuest.dicQuests.Values) 
		{
			if(!data.bRewarded && !data.isOpen && data.CheckOpen())
			{
				GameObject temp = Instantiate(_objQuestSlot) as GameObject;
				QuestSlot tempScript = temp.GetComponent<QuestSlot>();
				tempScript.SetSlot(data, false);
				tempScript._btnSlot.onClick.AddListener(() => { SetQuestPopup(tempScript); });
                tempScript._btnAccepBtn.onClick.AddListener(() => { OnClickFastAccept(tempScript); });

                RectTransform rtTr = temp.GetComponent<RectTransform>();
				rtTr.SetParent(_trQuestSlotPaent);
				rtTr.localScale = Vector3.one;
				rtTr.localPosition = Vector3.zero;
				rtTr.SetAsFirstSibling();
			}
		}
	}

	protected void GiveUpQuest(QuestAcceptPopup tempScript)
	{
		object[] param = new object[1]{tempScript};
		PopupManager.Instance.ShowYesNoPopup (TextManager.Instance.GetText("popup_title_quest_giveup"), TextManager.Instance.GetText("popup_desc_quest_giveup"), TextManager.Instance.GetText("popup_btn_quest_giveup"), AcceptGiveUp, param);
	}

	protected void AcceptGiveUp(object[] param)
	{
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequestQuestCancel(RequestQuestGiveUp);

		QuestAcceptPopup temp = (QuestAcceptPopup)param[0];
		temp.CloseMe();
	}

	protected virtual void RequestQuestGiveUp(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		DebugMgr.Log(err);

		if (err != Server.ERROR_ID.NONE) {
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.QUEST_CANCEL, err), Server.ServerMgr.Instance.CallClear);
		}
		else 
		{
			Legion.Instance.cQuest.CancelQuest ();
			_cSelectedQuestSlot.SetGiveUp();

			SetQuestListArea(false);

			RectTransform rtTr = _cSelectedQuestSlot.GetComponent<RectTransform>();
			rtTr.SetParent (_trQuestSlotPaent);
			rtTr.localScale = Vector3.one;
			rtTr.localPosition = Vector3.zero;
		}
	}

	protected void AcceptQuest(QuestAcceptPopup tempScript)
	{
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequestQuestAccept(_cSelectedQuestSlot.cInfo.u2ID, RequestQuestAccept);

        if(tempScript != null)
		    tempScript.CloseMe();
	}
		
	protected virtual void RequestQuestAccept(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE) 
		{
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.QUEST_ACCEPT, err), Server.ServerMgr.Instance.CallClear);
		}
		else 
		{
			Legion.Instance.cQuest.StartQuest (_cSelectedQuestSlot.cInfo.u2ID);
			_cSelectedQuestSlot.SetAccept ();

			QuestDirection qd = QuestInfoMgr.Instance.GetQuestDirctionInfo(_cSelectedQuestSlot.cInfo.u2ID, 1);
			if(qd != null)
				PopupManager.Instance.SetQuestDirectionPopup(qd, null);

			SetQuestListArea(true);

			RectTransform rtTr = _cSelectedQuestSlot.GetComponent<RectTransform>();
			rtTr.SetParent (_objQuestLine.transform);
			rtTr.localScale = Vector3.one;
			rtTr.localPosition = new Vector3(ingPos.SlotX, ingPos.SlotY, 0);
        }
	}
		
	//경험치물약을 위한 인벤 공간 검사
	protected void AddExpPotion(UInt32 u4Exp)
	{
		//경험치 계산
		Dictionary<UInt16, ConsumableItemInfo> tempConsumeItem = new Dictionary<ushort, ConsumableItemInfo>();
		List<ConsumableItemInfo> lstConsumeItem = new List<ConsumableItemInfo>();
		tempConsumeItem = ItemInfoMgr.Instance.GetConsumableItemInfo();
		Byte itemCnt = 0;
		for(int i = tempConsumeItem.Count; i > 0; )
		{
			if(u4Exp < tempConsumeItem[(ushort)(58000+i)].u4Exp)
			{
				if(itemCnt != 0)
				{
					Legion.Instance.cInventory.AddItem(0, (ushort)(58000+i), itemCnt);
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

	/// <summary>
	/// 퀘스트 리스트 영역 설정
	/// </summary>
	protected void SetQuestListArea(bool inQuestProgress)
	{
		_objQuestLine.SetActive (inQuestProgress);
		if(inQuestProgress == true)
		{
			_trQuestScrollArea.anchoredPosition = new Vector2 (_trQuestScrollArea.anchoredPosition.x, ingPos.GroupY);
			_trQuestScrollArea.sizeDelta = new Vector2 (860, ingPos.Height);
		}
		else 
		{
			_trQuestScrollArea.anchoredPosition = new Vector2 (_trQuestScrollArea.anchoredPosition.x, idlePos.GroupY);
			_trQuestScrollArea.sizeDelta = new Vector2 (860, idlePos.Height);
		}

        foreach (QuestSlot slot in dicQuestSlot.Values)
        {
            slot.RefreshSlot();
        }
    }

	protected virtual void CloseDepth1Popup()
	{
		if (depth1Popup == null)
			return;

		Destroy(depth1Popup);
		PopupManager.Instance.RemovePopup(depth1Popup);
	}

	public void QuestAreaShortcut()
	{
		QuestAcceptPopup tempScript = new QuestAcceptPopup();
		tempScript.QuestAreaShortcut(_cSelectedQuestSlot.cInfo.GetInfo());
        /*
		QuestInfo questInfo = selectedQuest.cInfo.GetInfo();
		StageInfoMgr.Instance.ShortCutChapter = -1;
		int selectedStage = -1;

		if ((MENU)questInfo.u2ShortCut == MENU.CAMPAIGN && (questInfo.u1QuestType == 2 || questInfo.u1QuestType == 18 || questInfo.u1QuestType == 19) && questTypeID != 0) {
			if (!StageInfoMgr.Instance.IsOpen (questInfo.u2QuestTypeID, questInfo.u1Delemiter1)) 
			{
				PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText ("popup_quest_shoutcut_title"),
					TextManager.Instance.GetText ("popup_quest_shoutcut_desc"),
					TextManager.Instance.GetText("btn_achi_shortcut"), QuickChangeScene, null);

				return;
			}
			selectedStage = questTypeID;
			StageInfoMgr.Instance.ShortCutChapter = -1;
		}
		else if (questInfo.u1QuestType == 15 && questTypeID != 0)
		{
			bool isFind = false;
			bool isFindStage = false;
			StageInfoMgr.Instance.LastPlayStage = -1;
			foreach(ActInfo actInfo in StageInfoMgr.Instance.dicActData.Values)
			{
				for(int a=0; a<actInfo.lstChapterID.Count; a++)
				{
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
		} else if (questInfo.u1QuestType == 29 && questTypeID != 0){
			StageInfoMgr.Instance.LastPlayStage = -1;
			foreach(ActInfo actInfo in StageInfoMgr.Instance.dicActData.Values){
				for(int a=0; a<actInfo.lstChapterID.Count; a++){
					ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData [actInfo.lstChapterID [a]];

					for(int b=0; b<chapterInfo.lstStageID.Count; b++){
						StageInfo stageInfo = StageInfoMgr.Instance.dicStageData [chapterInfo.lstStageID [b]];
						if(stageInfo.CheckRewardInStage(questTypeID) > 0)
						{
							if (!StageInfoMgr.Instance.IsOpen (stageInfo.u2ID, questInfo.u1Delemiter1)) {
								string title = TextManager.Instance.GetText ("popup_title_not_clear");
								string msg = TextManager.Instance.GetText ("popup_desc_not_clear");
								PopupManager.Instance.ShowOKPopup (title, msg, null);
								return;
							}
							selectedStage = stageInfo.u2ID;
						}
					}

				}
			}
		} else {
			StageInfoMgr.Instance.ShortCutChapter = shortCutChapter;
		} 

		if (selectedStage == -1 && StageInfoMgr.Instance.ShortCutChapter == -1) {
			string title = TextManager.Instance.GetText ("popup_title_not_clear");
			string msg = TextManager.Instance.GetText ("popup_desc_not_clear");
			PopupManager.Instance.ShowOKPopup (title, msg, null);
			return;
		}

		StageInfoMgr.Instance.LastPlayStage = selectedStage;

		if (shortCutPopup == 71)
			shortCutPopup = 74;
		if (shortCutPopup == 72)
			shortCutPopup = 75;
		if (shortCutPopup == 73)
			shortCutPopup = 76;

		FadeEffectMgr.Instance.QuickChangeScene(shortCut, shortCutPopup);
		*/
    }

	public void OnClose()
	{
		this.gameObject.SetActive(false);
		LobbyScene scene = Scene.GetCurrent() as LobbyScene;
		if (scene != null) {
			scene.RefreshQuest ();
			scene.PlayCharacterAnim (true);
		}
			
		PopupManager.Instance.RemovePopup(gameObject);

		if (Legion.Instance.checkLoginAchievement == 1) {
			if (scene != null) {
				scene.RefreshQuest ();
				scene.PlayCharacterAnim (true);
			}
		}
	}
}
