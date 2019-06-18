using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestListPos
{
	public float SlotX;
	public float SlotY;
	public float GroupY;
	public float Height;
}

public class QuestAchieveList : QuestPanel {
	public enum QuestListType
	{
		Quest = 0,
		Achievement = 1,
		Daily = 2,
		Weekly = 3,
	}

	QuestListType eType;

	GameObject _objAchieveSlot;
	//GameObject _objQuestSlot; // 부모에 존재

	//GameObject _objQuestPopup; // 부모에 존재
	//GameObject _objRewardPopup; // 부모에 존재

	public GameObject QuestMass;
	public GameObject AchieveMass;

	//public Transform QuestSlotParent; // 부모에 존재 _trQuestSlotPaent
	public Transform AchieveSlotParent;

	AchieveSlot selectedAchieve;
	List<UInt16> lstAchive;
	// QuestSlot selectedQuest; // 부모에 존재 _cSelectedQuestSlot

	//public Transform PopupParent; // 부모에 존재 _trPopupParent
	public Text rewardText;
	public Text progressCount;
	public GameObject rewardAllDis;
	public GameObject rewardAllEnable;

	//public RectTransform questScrollArea; // 부모에 존재 _trQuestScrollArea
	//public GameObject questLine; // 부모에 존재 _objQuestLine

	// GameObject depth1Popup; //부모에 존재
	GameObject depth2Popup;

	int clearedCnt = 0;

	//QuestListPos ingPos;	// 부모에 존재
	//QuestListPos idlePos; // 부모에 존재

	void Awake()
	{
		RectTransform rtTr = QuestMass.GetComponent<RectTransform>();
		rtTr.anchoredPosition3D = Vector3.zero;
		rtTr.sizeDelta = Vector2.zero;

		rtTr = AchieveMass.GetComponent<RectTransform>();
		rtTr.anchoredPosition3D = Vector3.zero;
		rtTr.sizeDelta = Vector2.zero;

		_objAchieveSlot = AssetMgr.Instance.AssetLoad ("Prefabs/UI/Quest/AchieveSlot.prefab", typeof(GameObject)) as GameObject;
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
		/* 
		ingPos = new QuestListPos ();
		ingPos.GroupY = -117;
		ingPos.Height = 400;
		ingPos.SlotX = 4;
		ingPos.SlotY = 80;

		idlePos = new QuestListPos ();
		idlePos.GroupY = -42;
		idlePos.Height = 550;
		idlePos.SlotX = 0;
		idlePos.SlotY = 0;
		*/
	}

	public void SetList(QuestListType type){

		eType = type;
		switch (eType) {
		case QuestListType.Quest:
			QuestMass.SetActive (true);
			AchieveMass.SetActive (false);
			if(_trQuestSlotPaent.childCount + _objQuestLine.transform.childCount == 0) 
				SetQuestList();
			break;
		case QuestListType.Daily: 
		case QuestListType.Weekly: 
		case QuestListType.Achievement:
			QuestMass.SetActive (false);
			AchieveMass.SetActive (true);
			SetAchieveList();
			break;
		}
	}

	public void SetAchieveList(){
		Byte u1Period = 1;
		int sib = 0;
		if(eType == QuestListType.Daily) u1Period = 2;
		else if(eType == QuestListType.Weekly) u1Period = 3;

		int childCnt = AchieveSlotParent.childCount;
		int createCnt = 0;
		int rewardedCnt = 0;
		int periodCnt = 0;

		clearedCnt = 0;
		foreach (UserAchievement data in Legion.Instance.cQuest.dicAchievements.Values) 
        {
			if (data.GetInfo ().u2EventID > 0)
				continue;
			
			if(data.GetInfo().u1PeriodType == u1Period){
				periodCnt++;
				if (data.bRewarded) {
					if (u1Period != 1) {
						data.isOpen = true;
					}
					rewardedCnt++;
				}

				if (data.GetInfo ().u1AchievementType == 0)
					data.CheckOpen ();
				
				if(data.isOpen && (u1Period != 1 || !data.bRewarded))
                {
                    UserAchievement tempData = data;
                    GameObject temp;
                    AchieveSlot tempScript;
                    if (createCnt < childCnt) {
                        temp = AchieveSlotParent.GetChild (createCnt).gameObject;
                        temp.SetActive (true);
                        tempScript = temp.GetComponent<AchieveSlot>();

                        tempScript.SetSlot(data);
                        tempScript._btnReward.onClick.RemoveAllListeners ();
                        tempScript._btnReward.onClick.AddListener(() => { GetAchieveReward(tempScript); });
                       
                    } else {
                        temp = Instantiate(_objAchieveSlot) as GameObject;
                        tempScript = temp.GetComponent<AchieveSlot>();

                        tempScript.SetSlot(data);
                        tempScript._btnReward.onClick.AddListener(() => { GetAchieveReward(tempScript); });
                        temp.transform.SetParent(AchieveSlotParent);
                        temp.transform.localScale = Vector3.one;
                        temp.transform.localPosition = Vector3.zero;
                        //if (data.isClear ()) {
                        //    if (!data.bRewarded && data.GetInfo().acReward[0].u1Type != (byte)GoodsType.SCROLL_SET)
                        //        clearedCnt++;
                        //    temp.transform.SetSiblingIndex (sib++);
                        //}
                    }

                    if (data.isClear())
                    {
                        if (!data.bRewarded &&
                            data.GetInfo().acReward[0].u1Type != (byte)GoodsType.SCROLL_SET &&
                            data.GetInfo().acReward[0].u1Type != (byte)GoodsType.EQUIP_COUPON)
                            clearedCnt++;

                        temp.transform.SetSiblingIndex(sib++);
                    }
                    createCnt++;
            	}
			}
		}
		// 2016. 09. 19 jy
		// 전체 업적을 제외한 업적은 현재 UI상에 생성 갯수만 표시한다
		if(u1Period == 1)
			progressCount.text = string.Format("{0} / {1}", rewardedCnt, periodCnt);
		else
			progressCount.text = string.Format("{0} / {1}", rewardedCnt, createCnt);

		if (clearedCnt > 0) {
			rewardAllDis.SetActive (false);
			rewardAllEnable.SetActive (true);
			rewardText.color = Color.white;
		} else {
			rewardAllDis.SetActive (true);
			rewardAllEnable.SetActive (false);
			rewardText.color = Color.gray;
		}

		if (AchieveSlotParent.childCount > createCnt) {
			for (int i = createCnt; i < AchieveSlotParent.childCount; i++) {
				AchieveSlotParent.GetChild (i).gameObject.SetActive(false);
			}
		}
	}

	void CheckOpenAchieveList(AchievementInfo info){
		Byte u1Period = 1;
		if(eType == QuestListType.Daily) u1Period = 2;
		else if(eType == QuestListType.Weekly) u1Period = 3;
		
		foreach (UserAchievement data in Legion.Instance.cQuest.dicAchievements.Values) {
			if(data.GetInfo().u1PeriodType == info.u1PeriodType){
				if(!data.bRewarded && data.GetInfo().u2Parent == info.u2ID && data.CheckGoods()){
					data.isOpen = true;
				}
			}
		}
	}

	void GetAchieveReward(AchieveSlot info){
		if(info.cInfo.bRewarded) return;

		if (!Legion.Instance.CheckEmptyInven ())
			return;

		Goods[] rewards = info.cInfo.GetReward(0);
		for(int i = 0; i < rewards.Length; ++i)
		{
			if(Legion.Instance.CheckGoodsLimitExcessx(rewards[i].u1Type) ==  true)
			{
				Legion.Instance.ShowGoodsOverMessage(rewards[i].u1Type);
				return;
			}
		}

		if (!info.cInfo.isClear ())
		{
			GameObject temp = Instantiate(_objRewardPopup) as GameObject;
			QuestRewardPopup tempScript = temp.GetComponent<QuestRewardPopup>();
			AchievementInfo achieve = info.cInfo.GetInfo();
			
			tempScript.Show(TextManager.Instance.GetText("popup_title_achi_reward"), TextManager.Instance.GetText(achieve.sName), null, null);
			tempScript.SetPopup(achieve);

			RectTransform rtTr = temp.GetComponent<RectTransform>();
			rtTr.SetParent(_trPopupParent);
			rtTr.localScale = Vector3.one;
			rtTr.anchoredPosition3D = Vector3.zero;
			rtTr.sizeDelta = Vector2.zero;

            depth1Popup = temp;
            PopupManager.Instance.AddPopup(temp, CloseDepth1Popup);
			return;
		}

		selectedAchieve = info;

        UInt16[] arrID = new UInt16[1];
		arrID[0] = selectedAchieve.cInfo.GetInfo().u2ID;

		PopupManager.Instance.ShowLoadingPopup(1);

		Server.ServerMgr.Instance.RequestQuestAchievementReward (arrID, 255, 0, RequestAchieveReward);
	}
    
    public void GetAllAchieveReward()
    {
		if (rewardAllDis.activeSelf)
			return;

		if (!Legion.Instance.CheckEmptyInven ())
			return;
		
		Byte u1Period = 1;
		if(eType == QuestListType.Daily) u1Period = 2;
		else if(eType == QuestListType.Weekly) u1Period = 3;

        lstAchive = new List<UInt16>();
        List<int> lstAchive1 = new List<int>();
		foreach (UserAchievement data in Legion.Instance.cQuest.dicAchievements.Values) {
			if (data.GetInfo ().u2EventID > 0)
				continue;

			if(data.GetInfo().u1PeriodType == u1Period)
            {
				if(data.GetInfo().acReward[0].u1Type != (byte)GoodsType.SCROLL_SET &&
					data.GetInfo().acReward[0].u1Type != (byte)GoodsType.EQUIP && 
					data.GetInfo().acReward[0].u1Type != (byte)GoodsType.EQUIP_COUPON)
				{
					if(data.isOpen && !data.bRewarded && data.isClear())
	                {
						if(Legion.Instance.CheckGoodsLimitExcessx(data.GetInfo().acReward[0].u1Type))
						{
							Legion.Instance.ShowGoodsOverMessage(data.GetInfo().acReward[0].u1Type);
							return;
						}
						lstAchive.Add(data.u2ID);
						if (data.GetInfo().acReward[0].u2ID == 58004)
							lstAchive1.Add((int)data.GetInfo().acReward[0].u4Count);
	                }
				}
			}
		}  

//        lstAchive.Add(50002);
//        lstAchive.Add(50887);
		if (lstAchive.Count == 0)
			return;
        
        PopupManager.Instance.ShowLoadingPopup(1);
        
        Server.ServerMgr.Instance.RequestQuestAchievementReward (lstAchive.ToArray(), 255, 0, RequestAchieveReward);
    }
	/*
	// 부모에 존재
	void CloseDepth1Popup(){
		if (depth1Popup == null)
			return;

		Destroy(depth1Popup);
		PopupManager.Instance.RemovePopup(depth1Popup);
	}
	*/
	void CloseDepth2Popup(){
		if (depth2Popup == null)
			return;

		Destroy(depth2Popup);
		PopupManager.Instance.RemovePopup(depth2Popup);
	}

	void RequestAchieveReward(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		DebugMgr.Log(err);
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.QUEST_ACHIEVEMENT_REWARD, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			GameObject temp = Instantiate(_objRewardPopup) as GameObject;
			QuestRewardPopup tempScript = temp.GetComponent<QuestRewardPopup>();
			if (selectedAchieve != null) {
				AchievementInfo achieve = selectedAchieve.cInfo.GetInfo ();
			
				tempScript.Show (TextManager.Instance.GetText ("popup_title_achi_reward"), TextManager.Instance.GetText (achieve.sName), null, null);
				if(achieve.acReward[0].u1Type == 10 || achieve.acReward[0].u1Type == 14)
				{
					AchieveItem achieveItem = QuestInfoMgr.Instance.listAchieveItemData[0];
					tempScript.SetPopup (achieveItem);
					QuestInfoMgr.Instance.listAchieveItemData.Clear();
				}
				else
				{
					tempScript.SetPopup (achieve);
					Legion.Instance.AddGoods (achieve.acReward);
				}
					
				RectTransform rtTr = temp.GetComponent<RectTransform>();
				rtTr.SetParent(_trPopupParent);
				rtTr.localScale = Vector3.one;
				rtTr.anchoredPosition3D = Vector3.zero;
				rtTr.sizeDelta = Vector2.zero;
					
				selectedAchieve.cInfo.bRewarded = true;
				selectedAchieve.DestroyMe ();

				depth1Popup = temp;

				PopupManager.Instance.AddPopup (depth1Popup, CloseDepth1Popup);

				Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.Achievement, 0, achieve.u1PeriodType, 0, 0, 1);

				CheckOpenAchieveList (achieve);

				SetList (eType);

				selectedAchieve = null;

			} else {
				List<Goods> acRewards = new List<Goods> ();

				for (int i = 0; i < lstAchive.Count; i++) 
                {
                    AchievementInfo achiInfo = QuestInfoMgr.Instance.GetAchieveInfo(lstAchive[i]);
                    if(achiInfo == null)
                    {
                        continue;
                    }
                    // 유저 업적 리스트
                    if (!Legion.Instance.cQuest.dicAchievements.ContainsKey(lstAchive[i]))
                    {
                        continue;
                    }

                    Legion.Instance.cQuest.dicAchievements[lstAchive[i]].bRewarded = true;
                    CheckOpenAchieveList(achiInfo);

                    for (int j = 0; j < achiInfo.acReward.Length; j++) 
                    {
						Goods reward = achiInfo.acReward [j];
						if(reward.u1Type == 10 || reward.u1Type == 14)
							continue;
						
						int idx = acRewards.FindIndex (cs => cs.u1Type == reward.u1Type &&  cs.u2ID == reward.u2ID);
						if (idx > -1) 
                        {
							acRewards [idx].u4Count += reward.u4Count;
						} 
                        else 
                        {
							acRewards.Add(reward);
						}
					}
				}

				tempScript.Show (TextManager.Instance.GetText ("popup_title_achi_reward"), TextManager.Instance.GetText("mark_reward_get_all_result"), null, null);
				tempScript.SetPopup (acRewards.ToArray());

				RectTransform rtTr = temp.GetComponent<RectTransform>();
				rtTr.SetParent(_trPopupParent);
				rtTr.localScale = Vector3.one;
				rtTr.anchoredPosition3D = Vector3.zero;
				rtTr.sizeDelta = Vector2.zero;

				Legion.Instance.AddGoods (acRewards.ToArray());

				Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.Achievement, 0, (Byte)eType, 0, 0, (UInt32)lstAchive.Count);
				lstAchive.Clear ();

				SetList (eType);

				depth1Popup = temp;

				PopupManager.Instance.AddPopup (depth1Popup, CloseDepth1Popup);
			}

            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.Achievement);
			QuestScene questScene = Scene.GetCurrent() as QuestScene;
			if(questScene != null){
				switch(eType){
				case QuestListType.Achievement: questScene.CheckAchieveAlram(); break;
				case QuestListType.Daily: questScene.CheckDailyAlram(); break;
				case QuestListType.Weekly: questScene.CheckWeeklyAlram(); break;
				}

			}
		}
	}
	/*
	// 부모에 존재
	public void SetQuestList(){
		if (Legion.Instance.cQuest.u2IngQuest > 0) {
			_objQuestLineSetActive (true);
			questScrollArea.anchoredPosition = new Vector2 (questScrollArea.anchoredPosition.x, ingPos.GroupY);
			questScrollArea.sizeDelta = new Vector2 (960, ingPos.Height);
		} else {
			questLine.SetActive (false);
			questScrollArea.anchoredPosition = new Vector2 (questScrollArea.anchoredPosition.x, idlePos.GroupY);
			questScrollArea.sizeDelta = new Vector2 (960, idlePos.Height);
		}
			
		foreach (UserQuest data in Legion.Instance.cQuest.dicQuests.Values) {
			if(!data.bRewarded && data.CheckOpen()){
//			if(!data.bRewarded){
				GameObject temp = Instantiate(_objQuestSlot) as GameObject;
				QuestSlot tempScript = temp.GetComponent<QuestSlot>();
				tempScript.SetSlot(data, false);
				tempScript._btnSlot.onClick.AddListener(() => { SetQuestPopup(tempScript); });

				if (Legion.Instance.cQuest.u2IngQuest == data.u2ID) {
					temp.transform.SetParent (questLine.transform);
					temp.transform.localScale = Vector3.one;
					temp.transform.localPosition = new Vector3(ingPos.SlotX, ingPos.SlotY, 0);
				} else {
					temp.transform.SetParent (QuestSlotParent);
					temp.transform.localScale = Vector3.one;
					temp.transform.localPosition = Vector3.zero;

					if (data.GetInfo ().u1MainType == 1) {
						temp.transform.SetAsFirstSibling ();
					}
				}
			}
		}
	}
	*/
	/*
	// 부모에 존재
	void CheckOpenQuestList(QuestInfo info){
		foreach (UserQuest data in Legion.Instance.cQuest.dicQuests.Values) {
			if(!data.bRewarded && !data.isOpen && data.CheckOpen()){
				GameObject temp = Instantiate(_objQuestSlot) as GameObject;
				QuestSlot tempScript = temp.GetComponent<QuestSlot>();
				tempScript.SetSlot(data, false);
				tempScript._btnSlot.onClick.AddListener(() => { SetQuestPopup(tempScript); });
				temp.transform.SetParent(QuestSlotParent);
				temp.transform.localScale = Vector3.one;
				temp.transform.localPosition = Vector3.zero;
				temp.transform.SetAsFirstSibling();
			}
		}
	}
	*/
	/*
	// 부모에 존재
	void GetQuestReward(QuestSlot info){
		if(info.cInfo.bRewarded) return;

		if(!info.cInfo.isClear()) return;

		if (!Legion.Instance.CheckEmptyInven())
			return;

		QuestInfo quest = info.cInfo.GetInfo();
		for(int i = 0;i < quest.acReward.Length; ++i)
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

		selectedQuest = info;
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequestQuestComplete(RequestQuestReward);
	}
	*/

	protected override void RequestQuestReward(Server.ERROR_ID err)
	{
		/*
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
			QuestInfo quest = selectedQuest.cInfo.GetInfo();

			//string type = "";
			//if (quest.u1MainType == 1)
			//	type = TextManager.Instance.GetText("mark_quest_type_main") + " ";
			//else if(quest.u1MainType == 2)
			//	type = TextManager.Instance.GetText("mark_quest_type_sub") + " ";
		
			tempScript.Show(TextManager.Instance.GetText("popup_title_quest_reward"), TextManager.Instance.GetText(quest.sName), null, null);
			tempScript.SetPopup(quest);

			temp.transform.SetParent(PopupParent);
			temp.transform.localScale = Vector3.one;
			temp.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
			temp.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			temp.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
			
			Legion.Instance.AddGoods(quest.acReward);
			AddExpPotion (quest.u4RewardExp);
			Legion.Instance.cQuest.EndQuest();

			Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.ClearQuest, quest.u2ID, quest.u1MainType, 0, 0, 1);
			
			CheckOpenQuestList(quest);

			depth1Popup = temp;
			
			PopupManager.Instance.AddPopup(depth1Popup, CloseDepth1Popup);

			QuestDirection qd = QuestInfoMgr.Instance.GetQuestDirctionInfo(selectedQuest.cInfo.u2ID, 3);
			if(qd != null){
				PopupManager.Instance.SetQuestDirectionPopup(qd, temp);
				tempScript.popup.SetActive (false);
				temp.GetComponent<Animator> ().enabled = false;
			}

			selectedQuest.DestroyMe ();
			selectedQuest = null;

			questLine.SetActive (false);
			questScrollArea.anchoredPosition = new Vector2 (questScrollArea.anchoredPosition.x, idlePos.GroupY);
			questScrollArea.sizeDelta = new Vector2 (960, idlePos.Height);
		*/
		base.RequestQuestReward(err);
		//QuestScene questScene = Scene.GetCurrent() as QuestScene;
		//if(questScene != null)
		//{
		//	questScene.CheckQuestAlram();
		//	questScene.CheckQuestLead();
		//}
		//}
	}

	/*
	// 부모에 존재
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
	*/
	/*
	// 부모에 존재
	void SetQuestPopup(QuestSlot info){
		if (Legion.Instance.cQuest.u2IngQuest > 0) {
			if (info.cInfo.u2ID != Legion.Instance.cQuest.u2IngQuest) {

				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_quest_ing"), TextManager.Instance.GetText("popup_desc_quest_ing"), null);

				return;
			}
		}

		selectedQuest = info;
		GameObject temp = Instantiate(_objQuestPopup) as GameObject;
		QuestAcceptPopup tempScript = temp.GetComponent<QuestAcceptPopup>();
		QuestInfo quest = info.cInfo.GetInfo();

		//string type = "";
		//if (quest.u1MainType == 1)
		//	type = TextManager.Instance.GetText("mark_quest_type_main") + " ";
		//else if(quest.u1MainType == 2)
		//	type = TextManager.Instance.GetText("mark_quest_type_sub") + " ";

		tempScript.Show(TextManager.Instance.GetText(quest.sName), TextManager.Instance.GetText(quest.sDescription), TextManager.Instance.GetText(quest.sSummary), null, null);
		tempScript.SetPopup(selectedQuest.cInfo.GetInfo());

		if (info.cInfo.isIng ()) {
			if (info.cInfo.isClear ()) {
				tempScript.SetGauge ((float)Legion.Instance.cQuest.u4QuestCount, (float)info.cInfo.u4MaxCount);
				tempScript.btnOK.transform.FindChild("Text").GetComponent<Text>().text = TextManager.Instance.GetText("mark_direction_quest_done");//"퀘스트 완료";
				tempScript.btnGiveUp.gameObject.SetActive (false);
				tempScript.btnGoTo.gameObject.SetActive (false);
				tempScript.btnOK.onClick.AddListener (() => { GetQuestReward (info); });
			} else {
				tempScript.SetGauge ((float)Legion.Instance.cQuest.u4QuestCount, (float)info.cInfo.u4MaxCount);
				tempScript.btnOK.gameObject.SetActive (false);
				tempScript.btnGiveUp.onClick.AddListener (() => { GiveUpQuest (tempScript); });
			}
		} else {
			tempScript.SetGauge(0, (float)info.cInfo.u4MaxCount);
			tempScript.btnGiveUp.gameObject.SetActive (false);
			tempScript.btnGoTo.gameObject.SetActive (false);
			tempScript.btnOK.onClick.AddListener (() => { AcceptQuest (tempScript); });
		}

		RectTransform rectTr = temp.GetComponent<RectTransform>();
		rectTr.SetParent(PopupParent);
		rectTr.localScale = Vector3.one;
		rectTr.anchoredPosition3D = Vector3.zero;
		rectTr.sizeDelta = Vector2.zero;

		depth1Popup = temp;
		
		PopupManager.Instance.AddPopup(depth1Popup, CloseDepth1Popup);
	}
	*/
	/*
	// 부모에 존재
	void AcceptQuest(QuestAcceptPopup tempScript){
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequestQuestAccept(selectedQuest.cInfo.u2ID, RequestQuestAccept);

		tempScript.CloseMe();
	}
	*/

	protected override void RequestQuestAccept(Server.ERROR_ID err)
	{
		/*
		PopupManager.Instance.CloseLoadingPopup();
		
		DebugMgr.Log(err);
		
		if (err != Server.ERROR_ID.NONE) {
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.QUEST_ACCEPT, err), Server.ServerMgr.Instance.CallClear);
		} else {
			Legion.Instance.cQuest.StartQuest (selectedQuest.cInfo.u2ID);
			selectedQuest.SetAccept ();
			QuestDirection qd = QuestInfoMgr.Instance.GetQuestDirctionInfo(selectedQuest.cInfo.u2ID, 1);
			if(qd != null) PopupManager.Instance.SetQuestDirectionPopup(qd, null);

			questLine.SetActive (true);
			questScrollArea.anchoredPosition = new Vector2 (questScrollArea.anchoredPosition.x, ingPos.GroupY);
			questScrollArea.sizeDelta = new Vector2 (960, ingPos.Height);

			selectedQuest.transform.SetParent (questLine.transform);
			selectedQuest.transform.localScale = Vector3.one;
			selectedQuest.transform.localPosition = new Vector3(ingPos.SlotX, ingPos.SlotY, 0);
            */
		base.RequestQuestAccept(err);
		//QuestScene questScene = Scene.GetCurrent() as QuestScene;
		//if(questScene != null)
		//	questScene.CheckQuestLead();
		//}
	}

	public void ShowSelectedQuestPopup(){
		SetQuestPopup (_cSelectedQuestSlot);
	}
	/*
	// 부모에 존재
	void GiveUpQuest(QuestAcceptPopup tempScript){

		object[] param = new object[1]{tempScript};

		PopupManager.Instance.ShowYesNoPopup (TextManager.Instance.GetText("popup_title_quest_giveup"), TextManager.Instance.GetText("popup_desc_quest_giveup"), TextManager.Instance.GetText("popup_btn_quest_giveup"), AcceptGiveUp, param);
	}
	*/
	/*
	// 부모에 존재
	void AcceptGiveUp(object[] param){
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequestQuestCancel(RequestQuestGiveUp);

		QuestAcceptPopup temp = (QuestAcceptPopup)param[0];

		temp.CloseMe();
	}
	*/

	protected override void RequestQuestGiveUp(Server.ERROR_ID err)
	{
		/*
		PopupManager.Instance.CloseLoadingPopup();
		
		DebugMgr.Log(err);
		
		if (err != Server.ERROR_ID.NONE) {
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.QUEST_CANCEL, err), Server.ServerMgr.Instance.CallClear);
		} else {
			Legion.Instance.cQuest.CancelQuest ();
			selectedQuest.SetGiveUp();

			questLine.SetActive (false);
			questScrollArea.anchoredPosition = new Vector2 (questScrollArea.anchoredPosition.x, idlePos.GroupY);
			questScrollArea.sizeDelta = new Vector2 (960, idlePos.Height);

			selectedQuest.transform.SetParent (QuestSlotParent);
			selectedQuest.transform.localScale = Vector3.one;
			selectedQuest.transform.localPosition = Vector3.zero;
		*/
		base.RequestQuestGiveUp(err);
		//QuestScene questScene = Scene.GetCurrent() as QuestScene;
		//if(questScene != null)
		//	questScene.CheckQuestLead();
		//}
	}
	/*
	// 부모에 존재
	public void QuestAreaShortcut()
	{
		QuestAcceptPopup tempScript = new QuestAcceptPopup();
		tempScript.QuestAreaShortcut(selectedQuest.cInfo.GetInfo());
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
		*//*
	}
	*/
}
