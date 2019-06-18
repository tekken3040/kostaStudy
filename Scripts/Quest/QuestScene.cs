using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class QuestScene : BaseScene {
    
    MENU[] menuType = new MENU[]{MENU.HERO_GUILD};

    enum QuestSceneMeun
    {
        NONE = -1,
        DAILY = 0,
        WEEK,
        ACHIEVE,
        TRAINING,
    };

	public GameObject BtnMenu;
	public GameObject ToggleMenu;

	public GameObject QuestObj;
    public GameObject CharTrainingObj;
    public GameObject EquipTrainingObj;
    public GameObject[] alramObject;		// 0:일일, 1:주간, 2:업적, 3:트레이닝
    public GameObject[] toggleAlramObject;  // 0:일일, 1:주간, 2:업적, 3:트레이닝

    public GameObject[] questLeadObject;
    
	QuestAchieveList QuestPopupScript;
    TrainingRoomList CharTrainingScript;
    TrainingRoomList EquipTrainingScript;
	GameObject TraningObj;

	public Transform PopupParent;

	Toggle[] ToggleBtns;

    QuestSceneMeun m_eCurMenu = QuestSceneMeun.NONE;
	
	void Awake()
	{
		#if UNITY_ANDROID
		IgaworksUnityAOS.IgaworksUnityPluginAOS.Adbrix.retention("GuardianGuild");
		#elif UNITY_IOS
		AdBrixPluginIOS.Retention("GuardianGuild");
		#endif
		int toggleMenuCount = ToggleMenu.transform.GetChild (0).childCount;
		ToggleBtns = new Toggle[toggleMenuCount];
		for(int i=0; i< toggleMenuCount; i++){
			ToggleBtns[i] = ToggleMenu.transform.GetChild(0).GetChild(i).GetComponent<Toggle>();
		}             
        
        RefreshAlram();
	}

    public void ShowSelectedQuestWithDelay(float delay){
		Invoke ("ShowSelectedQuest", delay);
	}

	void ShowSelectedQuest (){
		//QuestPopupScript.ShowSelectedQuestPopup ();

		// 2016. 12. 06 jy
		// 퀘스트 받을시 퀘스트 지역으로 바로가기 해달라고 요청옴
		QuestPopupScript.QuestAreaShortcut();
	}
	
	void CheckToggleMenu(QuestSceneMeun menu)
	{
		if (m_eCurMenu == menu)
			return;

        QuestSceneMeun before = m_eCurMenu;
        m_eCurMenu = menu;

		RefreshAlarm(before);
		RefreshAlarm(m_eCurMenu);

		if (!ToggleMenu.activeSelf) {
//			if (before > 0 && before != idx) ToggleBtns [before].isOn = false;
			ToggleBtns [(int)menu].isOn = true;
		} else {
			if(before > 0)
				ToggleBtns [(int)m_eCurMenu].GetComponent<ToggleImage> ().image.gameObject.SetActive (false);

			ToggleBtns [(int)menu].GetComponent<ToggleImage> ().image.gameObject.SetActive(true);
		}

		//			for(int i=0; i<ToggleBtns.Length; i++){
		//				if(i == idx) ToggleBtns[i].image.gameObject.SetActive(true);
		//				else ToggleBtns[i].image.gameObject.SetActive(false);
		//			}
	}

    public void SetDailyQuest(){
		CheckToggleMenu(QuestSceneMeun.DAILY);
		BtnMenu.SetActive(false);
		ToggleMenu.SetActive(true);
		QuestObj.SetActive(true);
        CharTrainingObj.SetActive(false);
		//EquipTrainingObj.SetActive(false);
		QuestPopupScript.SetList(QuestAchieveList.QuestListType.Daily);
	}

	public void SetWeeklyQuest(){
		CheckToggleMenu(QuestSceneMeun.WEEK);
		BtnMenu.SetActive(false);
		ToggleMenu.SetActive(true);
		QuestObj.SetActive(true);
        CharTrainingObj.SetActive(false);
		//EquipTrainingObj.SetActive(false);
		QuestPopupScript.SetList(QuestAchieveList.QuestListType.Weekly);
	}

	public void SetAchieve(){
		CheckToggleMenu(QuestSceneMeun.ACHIEVE);
		BtnMenu.SetActive(false);
		ToggleMenu.SetActive(true);
		QuestObj.SetActive(true);
        CharTrainingObj.SetActive(false);
		//EquipTrainingObj.SetActive(false);
		QuestPopupScript.SetList(QuestAchieveList.QuestListType.Achievement);
	}

	public void SetTraning(){
		CheckToggleMenu(QuestSceneMeun.TRAINING);
		BtnMenu.SetActive(false);
		ToggleMenu.SetActive(true);
		QuestObj.SetActive(false);
        CharTrainingObj.SetActive(true);
        //EquipTrainingObj.SetActive(false);
       	CharTrainingScript.Init();
	}

	void Start(){
        Legion.Instance.googleAnalytics.LogEvent(new EventHitBuilder().SetEventCategory("Guild").SetEventAction("Open").SetEventLabel("GuildOpen"));
		BtnMenu.SetActive(true);
		ToggleMenu.SetActive(false);
		QuestPopupScript = QuestObj.GetComponent<QuestAchieveList>();
        CharTrainingScript = CharTrainingObj.GetComponent<TrainingRoomList>();
        //EquipTrainingScript = EquipTrainingObj.GetComponent<TrainingRoomList>();
        
		QuestPopupScript._trPopupParent = PopupParent.GetComponent<RectTransform>();

		Legion.Instance.cTutorial.CheckTutorial(MENU.HERO_GUILD);
        PopupManager.Instance.AddPopup(gameObject, OnClickBack);
		//FadeEffectMgr.Instance.FadeIn(1f);
        FadeEffectMgr.Instance.FadeIn(FadeEffectMgr.GLOBAL_FADE_TIME);
		//CheckQuestLead();
        
        StartCoroutine(CheckReservedPopup());
	}
    
	public override IEnumerator CheckReservedPopup()
	{
		if(Legion.Instance.bStageFailed)
			PopupManager.Instance.SetNoticePopup (MENU.HERO_GUILD);
		
		GameManager.ReservedPopup reservedPopup = null;
		for(int i=0; i<menuType.Length; i++)
		{
			DebugMgr.Log(menuType[i]);
			reservedPopup = GameManager.Instance.GetReservedPopup(menuType[i]);
			if(reservedPopup != null)
			{
				DebugMgr.Log("OK");
				DebugMgr.Log("Reserved Popup : " + reservedPopup.menu);

                switch(reservedPopup.GetReservedPopupHeroGuild())
                {
				case POPUP_HERO_GUILD.DAILY_ACHIEVEMENT:
					SetDailyQuest();
					break;
				case POPUP_HERO_GUILD.WEEKLY_ACHIEVEMENT:
                    SetWeeklyQuest();
					break;
				case POPUP_HERO_GUILD.ACHIEVEMENT:                       
                    SetAchieve();
                    break;
				//  2016. 12. 27 jy
				//  퀘스트 분리
				//case POPUP_HERO_GUILD.QUEST:
                //    SetQuest();
                //    break;
				case POPUP_HERO_GUILD.QUEST_SELECT:
                    //SetDailyQuest();
                    break;
				case POPUP_HERO_GUILD.TRAINING_HERO:
                    SetTraning();
                    break;
				//2016. 10. 18 jy 
				//병기 훈련 통합
				//case POPUP_HERO_GUILD.TRAINING_EQUIP:
                //    SetWeapon();
                //    break;
                }
				yield break;
			}
		}
	}
    
	public void OnClickBack()
	{
        PopupManager.Instance.RemovePopup(gameObject);
        StartCoroutine(ChangeScene());
    }
    
    private IEnumerator ChangeScene()
    {
		FadeEffectMgr.Instance.FadeOut ();
		yield return new WaitForSeconds (FadeEffectMgr.GLOBAL_FADE_TIME);
		AssetMgr.Instance.SceneLoad("LobbyScene");
    }    
    
    public override void RefreshAlram()
    {
		CheckDailyAlram();
		CheckWeeklyAlram();
		CheckAchieveAlram();
        CheckCharTrainingAlram();
        // 2016. 12. 27 jy
        // 퀘스트 별도 분리됨
        //CheckQuestAlram();
        // 2016. 10. 18 jy
        // 병기 훈련 통합
        //CheckEquipTrainingAlram();
        // 2017. 01. 02 jy
        // 인벤토리 추가
        //CheckInventoryAlram();
    }

    void RefreshAlarm(QuestSceneMeun menu)
	{
		switch (menu) 
		{
		/* 2016. 12. 27 jy
		// 퀘스트 별도 분리됨
		case 0:
			CheckQuestAlram ();
			break; */
		case QuestSceneMeun.DAILY:
			CheckDailyAlram ();
			break;
		case QuestSceneMeun.WEEK:
			CheckWeeklyAlram ();
			break;
		case QuestSceneMeun.ACHIEVE:
			CheckAchieveAlram ();
			break;
		case QuestSceneMeun.TRAINING:
			CheckCharTrainingAlram ();
			break;
        /*
        case 5:
            // 2017. 01. 02 jy
            // 인벤토리 추가
            //CheckInventoryAlram();
        
            // 2016. 10. 18 jy
            // 병기 훈련 통합
            CheckEquipTrainingAlram ();
            break;
        */
        }
    }

	public void CheckDailyAlram()
	{
		bool check = Legion.Instance.cQuest.CheckAlarmAchievement(2);

        int index = (int)QuestSceneMeun.DAILY;
        alramObject[index].gameObject.SetActive(check);
		toggleAlramObject[index].gameObject.SetActive(check);
	}

	public void CheckWeeklyAlram()
	{
		bool check = Legion.Instance.cQuest.CheckAlarmAchievement(3);

        int index = (int)QuestSceneMeun.WEEK;
        alramObject[index].gameObject.SetActive(check);
		toggleAlramObject[index].gameObject.SetActive(check);
	}

	public void CheckAchieveAlram()
	{
		bool check = Legion.Instance.cQuest.CheckAlarmAchievement(1);

        int index = (int)QuestSceneMeun.ACHIEVE;
        alramObject[index].gameObject.SetActive(check);
		toggleAlramObject[index].gameObject.SetActive(check);
	}
    
    public void CheckCharTrainingAlram()
    {
        bool check = false;

        int index = (int)QuestSceneMeun.TRAINING;
        for (int i=0; i<Legion.Instance.acHeros.Count; i++)
        {
            if(Legion.Instance.acHeros[i].u1SeatNum != 0)
            {
                UInt16 id = (UInt16)(Server.ConstDef.BaseCharTrainingID + Legion.Instance.acHeros[i].u1RoomNum);
                TimeSpan timeSpan = QuestInfoMgr.Instance.GetCharTrainingInfo()[id].doneTime - Legion.Instance.ServerTime;
                
                if(timeSpan.Ticks <= 0)
                {
                    check = true;
                    break;
                }
            }
        }        
        
        alramObject[index].gameObject.SetActive(check);
		toggleAlramObject[index].gameObject.SetActive(check);
    }

    /* 퀘스트 분리됨에 따라 사용하지 않는 함수 모아둠
    // 2016. 12. 27 jy 
    // 퀘스트를 메인으로 분리함
    public void CheckQuestLead()
    {
    	if (Legion.Instance.cTutorial.bIng || Legion.Instance.cQuest.u2IngQuest > 0) {
    		for (int i=0; i<questLeadObject.Length; i++) {
    			questLeadObject [i].SetActive (false);
    		}
    	} else {
    		if (Legion.Instance.cQuest.IsHaveOpenQuest ()) {
    			for (int i = 0; i < questLeadObject.Length; i++) {
    				questLeadObject [i].SetActive (true);
    			}
    		}
    	}
    }

    // 2016. 12. 27 jy 
    // 퀘스트 분리
    public void SetQuest()
    {
    	CheckToggleMenu(0);
    	BtnMenu.SetActive(false);
    	ToggleMenu.SetActive(true);
    	QuestObj.SetActive(true);
    	CharTrainingObj.SetActive(false);
    	//EquipTrainingObj.SetActive(false);
    	QuestPopupScript.SetList(QuestAchieveList.QuestListType.Quest);
    }

    // 2016. 12. 27 jy
    // 퀘스트씬에서 퀘스트 분리
    public void CheckQuestAlram()
    {
        return;
    
        bool check = Legion.Instance.cQuest.isClearQuest();
    
        alramObject[0].gameObject.SetActive(check);
        toggleAlramObject[0].gameObject.SetActive(check);
    }
    */

    /* 병기 훈련 통합으로 사용하지 않는 함수 모아둠
    // 2016. 10. 18 jy 
    // 병기 훈련 통합
    public void SetWeapon(){
        CheckToggleMenu(5);
        BtnMenu.SetActive(false);
        ToggleMenu.SetActive(true);
        QuestObj.SetActive(false);
        CharTrainingObj.SetActive(false);
        EquipTrainingObj.SetActive(true);
        EquipTrainingScript.Init();
    }

    // 2016. 10. 18 jy
    // 병기 훈련 통합 장비 트레이닝 알람 체크 안함
    public void CheckEquipTrainingAlram()
    {
        return;
    
        bool check = false;
        List<EquipmentItem> lstItem = Legion.Instance.cInventory.lstSortedEquipment;
        for(int i=0; i<lstItem.Count; i++)
        {
            if(lstItem[i].u1SeatNum != 0)
            {
                UInt16 id = (UInt16)(Server.ConstDef.BaseEquipTrainingID + lstItem[i].u1RoomNum);
                TimeSpan timeSpan = QuestInfoMgr.Instance.GetEquipTrainingInfo()[id].doneTime - Legion.Instance.ServerTime;
                
                if(timeSpan.Ticks <= 0)
                {
                    check = true;
                    break;
                }
            }
        }
    
        alramObject[5].gameObject.SetActive(check);
    	toggleAlramObject[5].gameObject.SetActive(check);
    }
    */

    // 2017. 01. 02 jy
    // 인벤토리 퀘스트 씬으로 이동
    // 언제또 빼달라고 할지 모르니 인벤토리 관련 함수는 모아두는 걸루다....
    //public void SetInventory()
    //{
    //	StartCoroutine(base.OpenInventory(false));
    //}
    //
    //public void CheckInventoryAlram()
    //{
    //	bool check = false;
    //
    //	foreach (Item item in Legion.Instance.cInventory.dicInventory.Values)
    //	{
    //		if(item.isNew)
    //		{
    //			if(item.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT &&
    //				((EquipmentItem)item).attached.hero != null)
    //				continue;
    //			check = true;
    //			break;
    //		}
    //	} 
    //
    //	alramObject[5].gameObject.SetActive(check);
    //	toggleAlramObject[5].gameObject.SetActive(check);
    //}
}
