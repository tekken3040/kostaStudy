using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class EventPanel : MonoBehaviour
{
    public GameObject objLobbyScene;
    //public GameObject PrefMonthlyRewardWindow;
    //public GameObject PrefTimeRewardWindow;

    private GameObject MonthlyRewardWindow;         // 출석 체크 창
    private GameObject TimeRewardWindow;
    private GameObject _objGrowthPackPopup;         // 성장 패키지 팝업
    private GameObject _objUpbringingRewardPopup;   // 육성 패키지 팝업
    private GameObject _MonthlyPackagePopup;        // 30일 패키지
    private GameObject _LimitPackagePopup;          // 한정 패키지
    private GameObject _EquipPackagePopup;          // 장비 패키지
    private GameObject MarbleWindow;
    private GameObject OXQuiz;
    private PackagePopup PackageWindowScript;
    //GameObject PrefPackageChar;

    public GameObject[] Package;
    public GameObject[] Alram;  // 0.전체, 1.출석보상, 2.성장팩, 3.시간보상, 4.육성패키지, 5.캐릭터, 6. 30일패키지

    public Image imgOxBtn;
    public Text txtOXBtn;

    private ushort rewardID;

    void Awake(){
        //PrefPackageChar = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_UI_Package_Character.prefab", typeof(GameObject));
        //PrefPackageChar = AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_EventNoticePopup.prefab", typeof(GameObject)) as GameObject;
        // OX 퀴즈라면
        if (EventInfoMgr.Instance.IsOXQuestion())
        {
            imgOxBtn.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_01.event_icon_ox");
            txtOXBtn.text = TextManager.Instance.GetText("ox_btn");
        }
        else
        {
            imgOxBtn.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_01.AccessDay_icon");
            txtOXBtn.text = TextManager.Instance.GetText("icon_main_slot_event");
        }
    }

    private void OnDisable()
    {
        StopCoroutine("CheckOxTime");
    }

    public void Init() {
        CheckAlarm();

        //if (!Legion.Instance.bADView && !Legion.Instance.cTutorial.bIng)
        //{
        //    List<EventPackageInfo> lst = EventInfoMgr.Instance.CheckCharacterPackage (true);
        //    if (lst.Count > 0) {
        //        GameObject CharPackageWindow = Instantiate (PrefPackageChar) as GameObject;
        //        RectTransform rtTr = CharPackageWindow.GetComponent<RectTransform>();
        //        rtTr.SetParent (objLobbyScene.transform);
        //        rtTr.localPosition = Vector3.zero;
        //        rtTr.localScale = Vector3.one;
        //        rtTr.sizeDelta = Vector2.zero; //new Vector2 (1280, 720);
        //        rtTr.GetComponent<CharacterPackPopup>().SetPopup();
        //        //CharPackageWindow.GetComponent<CharacterPackagePopup> ().InitPopup (lst, true);
        //        Legion.Instance.bADView = true;
        //        return;
        //    }
        //}
        //
        //if (!Legion.Instance.bADView) {
        //	Legion.Instance.ShowCafeOnce ();
        //}
    }

    public void OnClickMenu(int _menu)
    {
        switch (_menu)
        {
            case 0:	// 출석 체크
                if (MonthlyRewardWindow == null)
                {
                    MonthlyRewardWindow = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_Monthly_login.prefab", typeof(GameObject)) as GameObject);
                    RectTransform rectTr = MonthlyRewardWindow.GetComponent<RectTransform>();
                    rectTr.SetParent(objLobbyScene.transform);
                    rectTr.localPosition = Vector3.zero;
                    rectTr.localScale = Vector3.one;
                    rectTr.sizeDelta = Vector2.zero;
                }
                MonthlyRewardWindow.SetActive(true);
                break;
            case 1: // 성장 패키지
                OpenGrowthPackage(0);
                break;
            case 2:	// 시간 보상
                if (TimeRewardWindow == null)
                {
                    TimeRewardWindow = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_Connecting_Reward.prefab", typeof(GameObject)) as GameObject);
                    RectTransform rectTr = TimeRewardWindow.GetComponent<RectTransform>();
                    rectTr.SetParent(objLobbyScene.transform);
                    rectTr.localPosition = Vector3.zero;
                    rectTr.localScale = Vector3.one;
                    rectTr.sizeDelta = Vector2.zero;
                }

                TimeRewardWindow.SetActive(true);
                PopupManager.Instance.AddPopup(TimeRewardWindow, TimeRewardWindow.GetComponent<TimeRewardPanel>().OnClickClose);
                break;
            case 3: // 육성 패키지
                if (_objUpbringingRewardPopup == null)
                {
                    _objUpbringingRewardPopup = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_UpbringingRewardPopup.prefab", typeof(GameObject)) as GameObject);
                    RectTransform rectTr = _objUpbringingRewardPopup.GetComponent<RectTransform>();
                    rectTr.SetParent(objLobbyScene.transform);
                    rectTr.localPosition = Vector3.zero;
                    rectTr.localScale = Vector3.one;
                    rectTr.sizeDelta = Vector2.zero;
                    _objUpbringingRewardPopup.GetComponent<UpbringingRewardPopup>().SetPopup();
                }
                _objUpbringingRewardPopup.SetActive(true);
                PopupManager.Instance.AddPopup(_objUpbringingRewardPopup, _objUpbringingRewardPopup.GetComponent<UpbringingRewardPopup>().OnClickClosePopup);
                //CheckPackageWindow (PackagePopup.PackageMenu.Promot);
                break;
            case 4: // 캐릭터 상품
                if (Legion.Instance.sName == "")
                {
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("popup_desc_char_pkg_not_crew"), null);
                    return;
                }
                else if (EventInfoMgr.Instance.GetSaleEventListByEventType((byte)PackagePopup.PackageMenu.Character).Count > 0)
                {
                    GameObject CharPackageWindow = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_CharacterPackPopup.prefab", typeof(GameObject)) as GameObject);
                    RectTransform rectTr = CharPackageWindow.GetComponent<RectTransform>();
                    rectTr.SetParent(objLobbyScene.transform);
                    rectTr.localPosition = Vector3.zero;
                    rectTr.localScale = Vector3.one;
                    rectTr.sizeDelta = Vector2.zero;

                    // 팝업 생성 셋팅이 실패 하였다면 판매 할 상품이 없다는 팝업을 띄움
                    if (!CharPackageWindow.GetComponent<CharacterPackPopup>().SetPopup(false))
                    {
                        PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("event_goods_no_sales"), null);
                    }
                    else
                    {
                        PopupManager.Instance.AddPopup(CharPackageWindow, CharPackageWindow.GetComponent<CharacterPackPopup>().OnClickClosePopup);
                    }
                }
                else
                {
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("event_goods_no_sales"), null);
                }
                break;
            case 5: // 30일 패키지?
                if (Alram[6].activeSelf)
                    GetEventReward(PackagePopup.PackageMenu.Month);
                else
                    CheckPackageWindow(PackagePopup.PackageMenu.Month);
                break;
            case 6: // 네이버 까페
                GLink.sharedInstance().executeHome();
                break;
            case 7: // 한정 상품
                if (EventInfoMgr.Instance.IsPackagePopupOpen(7) == false)
                    return;

                if (_LimitPackagePopup == null)
                {
                    _LimitPackagePopup = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_LimitPackPopup.prefab", typeof(GameObject)) as GameObject);
                    RectTransform rectTr = _LimitPackagePopup.GetComponent<RectTransform>();
                    rectTr.SetParent(objLobbyScene.transform);
                    rectTr.localPosition = Vector3.zero;
                    rectTr.localScale = Vector3.one;
                    rectTr.sizeDelta = Vector2.zero;
                    _LimitPackagePopup.GetComponent<LimitPackagePopup>().SetPopup();
                }
                _LimitPackagePopup.SetActive(true);

                PopupManager.Instance.AddPopup(_LimitPackagePopup, _LimitPackagePopup.GetComponent<LimitPackagePopup>().OnClosePopup);
                break;
            case 8: // 제작 재료 패키지 
                OpenGrowthPackage(1);
                break;
            case 9: // 장비 패키지
                if (EventInfoMgr.Instance.IsPackagePopupOpen(9) == false)
                    return;

                if (_EquipPackagePopup == null)
                {
                    _EquipPackagePopup = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_EquipPackPopup.prefab", typeof(GameObject)) as GameObject);
                    RectTransform rectTr = _EquipPackagePopup.GetComponent<RectTransform>();
                    rectTr.SetParent(objLobbyScene.transform);
                    rectTr.localPosition = Vector3.zero;
                    rectTr.localScale = Vector3.one;
                    rectTr.sizeDelta = Vector2.zero;// new Vector2(1280, 720);
                    _EquipPackagePopup.GetComponent<EquipPackagePopup>().SetPopup();
                }
                _EquipPackagePopup.SetActive(true);

                PopupManager.Instance.AddPopup(_EquipPackagePopup, _EquipPackagePopup.GetComponent<EquipPackagePopup>().OnClickClosePopup);
                break;
            case 10: //OX퀴즈
                //Legion.Instance.cEvent.u1OXtotalReward = 0;
                if(Legion.Instance.cEvent.u2OXEventID == 0)
                {
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("mark_event_empty"), null);
                    return;
                }

                if (OXQuiz == null)
                {
                    // 문제를 푸는 방식이라면 OX 패널을 아니라면 보상
                    if(EventInfoMgr.Instance.IsOXQuestion())
                    {
                        if (Legion.Instance.cEvent.u1OXtotalReward == EventOxReward.MAX_DAY)
                        {
                            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("ox_btn"), TextManager.Instance.GetText("event_end_time"), null);
                            return;
                        }

                        OXQuiz = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_UI_Event_OX_Panel.prefab", typeof(GameObject)) as GameObject);
                        // 프리팹 최상단 부모의 Anchors 타입이 달라 따로 셋팅함
                        // OX에서는 sizeDelta 타입 제로로 초기하면 UI가 안보임
                        RectTransform rectTr = OXQuiz.GetComponent<RectTransform>();
                        rectTr.SetParent(objLobbyScene.transform);
                        rectTr.localPosition = Vector3.zero;
                        rectTr.localScale = Vector3.one;
                    }
                    else
                    {
                        OXQuiz = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_UI_AccessDayReward.prefab", typeof(GameObject)) as GameObject);
                        // 프리팹 최상단 부모의 Anchors 타입이 달라 따로 셋팅함
                        // OX에서는 sizeDelta 타입 제로로 초기하면 UI가 안보임
                        RectTransform rectTr = OXQuiz.GetComponent<RectTransform>();
                        rectTr.SetParent(objLobbyScene.transform);
                        rectTr.localPosition = Vector3.zero;
                        rectTr.sizeDelta = Vector3.zero;
                        rectTr.localScale = Vector3.one;
                    }
                }
                else
                    OXQuiz.SetActive(true);

                break;
        }
    }

    private void OpenGrowthPackage(byte packageType)
    {
        if (EventInfoMgr.Instance.IsPackagePopupOpen(6) == false)
            return;

        if (_objGrowthPackPopup == null)
        {
            _objGrowthPackPopup = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_GrowthPackPopup.prefab", typeof(GameObject)) as GameObject);
            RectTransform rectTr = _objGrowthPackPopup.GetComponent<RectTransform>();
            rectTr.SetParent(objLobbyScene.transform);
            rectTr.localPosition = Vector3.zero;
            rectTr.localScale = Vector3.one;
            rectTr.sizeDelta = Vector2.zero;
        }
        if(_objGrowthPackPopup.GetComponent<GrowthPackagePopup>().SetPopup(packageType))
        {
            _objGrowthPackPopup.SetActive(true);
            PopupManager.Instance.AddPopup(_objGrowthPackPopup, _objGrowthPackPopup.GetComponent<GrowthPackagePopup>().OnClickClosePopup);
        }
        else
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("event_goods_no_sales"), null);
        }        
    }

    public void CheckAlarm(){
		//Alram
		//0 = 전체
		//1 = 출석
		//2 = 성장팩
		//3 = 접속보상
		//4 = 육성
		//5 = 캐릭터
		//6 = 30일

		bool bNew = false;

		for (int i = 1; i < Alram.Length; i++) {
			Alram [i].SetActive (false);
		}
			
//		if (CheckPackageAlarm (PackagePopup.PackageMenu.Recommend)) {
//			Alram [2].SetActive (true);
//			bNew = true;
//		}
        if((Legion.Instance.tsConnectTime.TotalSeconds == 0) && (EventInfoMgr.Instance.OnLastReward == false)){
            Alram [3].SetActive (true);
			bNew = true;
        }
		if (CheckPackageAlarm (PackagePopup.PackageMenu.Promot)) {
			Alram [4].SetActive (true);
			bNew = true;
		}
		if (CheckPackageAlarm (PackagePopup.PackageMenu.Month)) {
			Alram [6].SetActive (true);
			bNew = true;
		}
        if(Legion.Instance.cEvent.u1TodayOxDone == 2)
        {
            OxQuizTimeCheckStart();
        }
		Alram [0].SetActive (bNew);
	}

    public void OxQuizTimeCheckStart()
    {
        StartCoroutine("CheckOxTime");
    }

    private IEnumerator CheckOxTime()
    {
        Alram[7].SetActive(true);
        TimeSpan tsTimeSpan = new TimeSpan();
        while(true)
        {
            yield return new WaitForSeconds(1f);
            tsTimeSpan = TimeSpan.FromSeconds(Legion.Instance.cEvent.u2OXlefttime);
            Alram[7].transform.GetChild(0).GetComponent<Text>().text = String.Format("{0:00}:{1:00}:{2:00}", tsTimeSpan.Hours, tsTimeSpan.Minutes, tsTimeSpan.Seconds);
            if(Legion.Instance.cEvent.u2OXlefttime == 0)
            {
                Alram[7].SetActive(false);
                yield break;
            }
        }
    }

	bool CheckPackageAlarm(PackagePopup.PackageMenu pType){
		List<EventPackageInfo> lstEventList = new List<EventPackageInfo> ();
		lstEventList = EventInfoMgr.Instance.GetEventListByEventType ((byte)pType);

		if (lstEventList.Count == 0)
			return false;

		for (int i = 0; i < lstEventList.Count; i++) {
			if (EventInfoMgr.Instance.CheckRewardPossible (lstEventList [i].u2ID))
				return true;
		}

		return false;
	}

	void GetEventReward(PackagePopup.PackageMenu pType){
		if(!Legion.Instance.CheckEmptyInven())
		{
			return;
		}

		List<EventPackageInfo> lstEventList = EventInfoMgr.Instance.GetEventListByEventType ((byte)pType);

		if (lstEventList.Count == 0)
			return;

		for (int i = 0; i < lstEventList.Count; i++) {
			if (EventInfoMgr.Instance.CheckRewardPossible (lstEventList [i].u2ID)){

				EventPackageInfo pInfo = EventInfoMgr.Instance.GetPackageInfo (lstEventList [i].u2ID);
				for(int j = 0; j < pInfo.acPackageRewards.Length; ++j)
				{
					if(Legion.Instance.CheckGoodsLimitExcessx(pInfo.acPackageRewards[j].u1Type) == true)
					{
						Legion.Instance.ShowGoodsOverMessage(pInfo.acPackageRewards[j].u1Type);
						return;
					}
				}

				rewardID = lstEventList [i].u2ID;
				PopupManager.Instance.ShowLoadingPopup (1);
				Server.ServerMgr.Instance.RequestEventGoodsReward (lstEventList [i].u2ID, rewardResult);
				return;
			}
		}
	}

	void CheckPackageWindow(PackagePopup.PackageMenu eType)
    {
		if(_MonthlyPackagePopup == null)
		{
            _MonthlyPackagePopup = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_30DayPackPopup.prefab", typeof(GameObject)) as GameObject);
			RectTransform rectTr = _MonthlyPackagePopup.GetComponent<RectTransform>();
			rectTr.SetParent(objLobbyScene.transform);
			rectTr.localPosition = Vector3.zero;
			rectTr.localScale = Vector3.one;
			rectTr.sizeDelta = Vector2.zero;
        }
        MonthPackagePopup monthPopup = _MonthlyPackagePopup.GetComponent<MonthPackagePopup>();
        PopupManager.Instance.AddPopup(_MonthlyPackagePopup, monthPopup.OnClickClosePopup);
        monthPopup.SetPopup();
        
    }

    // 보상 결과 처리
    private void rewardResult(Server.ERROR_ID err)
	{
		DebugMgr.Log("rewardResult " + err);

		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EVENT_REWARD, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
            if (EventInfoMgr.Instance.dicEventReward.ContainsKey(rewardID)) {
                // 30일 패키지가 아니라면
                //EventInfoMgr.Instance.dicEventReward.Remove(rewardID);
                EventReward _eventReward = EventInfoMgr.Instance.dicEventReward[rewardID];
                _eventReward.u2EventID = rewardID;
                _eventReward.u1RewardIndex = EventInfoMgr.Instance.sEventGoodsReward.u1LastRewardIndex;
                if (EventInfoMgr.Instance.dicEventReward[rewardID].u1EventType != 5)
                    _eventReward.u4RecordValue = EventInfoMgr.Instance.sEventGoodsReward.u4RecordValue;

                EventInfoMgr.Instance.dicEventReward[rewardID] = _eventReward;
			} else {
				EventReward _eventReward = new EventReward();
				_eventReward.u2EventID = rewardID;
				_eventReward.u1RewardIndex = EventInfoMgr.Instance.sEventGoodsReward.u1LastRewardIndex;
				_eventReward.u4RecordValue = EventInfoMgr.Instance.sEventGoodsReward.u4RecordValue;
				EventInfoMgr.Instance.dicEventReward.Add (rewardID, _eventReward);
			}

			EventPackageInfo cPackage = EventInfoMgr.Instance.GetPackageInfo (rewardID);
			Legion.Instance.AddGoods (cPackage.acPackageRewards);

			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_vip_reward") , GetRewardString() + TextManager.Instance.GetText("mark_event_get_goods"), null);

			if (!CheckPackageAlarm (PackagePopup.PackageMenu.Promot)) {
				Alram [4].SetActive (false);
			}
			if (!CheckPackageAlarm (PackagePopup.PackageMenu.Month)) {
				Alram [6].SetActive (false);
			}

			bool bNew = false;

			for (int i = 1; i < Alram.Length; i++) {
				if (Alram [i].activeSelf)
					bNew = true;
			}
			Alram [0].SetActive (bNew);
		}
	}

	string GetRewardString(){
		int cnt = 0;
		string reward = "";
		EventPackageInfo info = EventInfoMgr.Instance.GetPackageInfo (rewardID);
		for (int i = 0; i < info.acPackageRewards.Length; i++) {
			if (info.acPackageRewards [i].u1Type != 0) {
				if (cnt > 0) reward += "\n";
				if (info.acPackageRewards [i].u1Type == (byte)GoodsType.CONSUME) {
					reward += TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (info.acPackageRewards [i].u2ID).sName) + " " + info.acPackageRewards [i].u4Count;
				} else {
					reward += Legion.Instance.GetConsumeString (info.acPackageRewards [i].u1Type) + " " + info.acPackageRewards [i].u4Count;
				}
				cnt++;
			}
		}

		return reward;
	}
}
