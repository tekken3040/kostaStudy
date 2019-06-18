using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using CodeStage.AntiCheat.ObscuredTypes;
using IgaworksUnityAOS;

// 2016. 12. 22 jy
// 기획 컴터에서 GooglePlayGames를 인식하여 Pull시 불편을 느낀다고함
#if UNITY_EDITOR
#else
using GooglePlayGames;
#endif
using UnityEngine.SocialPlatforms;

public class LobbyScene : BaseScene
{
    public enum LobbyAlram
    {
        QUEST = 0,
        GUILD,
        SHOP,
        INVENTROY,
        LEAGUE,
        CAMPAINGN,
        CREW,
        SOCIAL,
        MAIL,
        ODIN,
    }

    enum LobbyQuestAlram
    {
        GUILD = 0,
        SHOP,
        CAMPAINGN,
        CREW,
    }

    MENU[] menuType = new MENU[] { MENU.MAIN, MENU.INVENTORY, MENU.SHOP, MENU.CREW, MENU.CHARACTER_INFO, MENU.QUEST };
    public GameObject _socialPanel;                 //소셜 패널
    public GameObject _tempBattleFieldPanel;        //임시 전장 패널
    public EventPanel _eventPanel;                  //이벤트 패널
    public GameObject _mainMenu;                    //메인메뉴
    public GameObject _CrewMenu;                    //크루메뉴
    public GameObject _userInfo;                    //사용자 이름,VIP레벨
    public GameObject _goods;                       //재화 목록
    public GameObject _userGold;                    //사용자 골드
    public GameObject _userCash;                    //사용자 캐쉬
    public GameObject _userKey;                     //사용자 열쇠
    public GameObject _characterPos;                //현재 크루의 캐릭터 배치
    public GameObject _bgCharacterPos;              //맵위 캐릭터 배치
    public Image _crewEmblem;                       //크루 엠블럼
    public Image _crewEmblemNum;                    //크루 엠블럼 번호
    public Text _crewPower;						    //크루 전투력
    public UI_Panel_CharacterInfo _characterInfo;   //캐릭터 세부정보(스킬, 장비 등)
    //public GameObject Pref_CharacterInfo;         //캐릭터 세부정보 프리펩
    public GameObject Btn_ShowMenu;                 //메뉴 보이기 버튼
    public Button Btn_DivisionMark;                 //리그 디비전 마크
    //public GameObject Pref_BattleField;           //전장 프리펩
    //public GameObject Pref_SocialPanel;           //소셜 프리펩
    //public GameObject Pref_CrewPanel;             //크루 프리펩
    public GameObject[] _objAlramIcon;              //알람 아이콘(0:퀘스트, 1:길드, 2:상점, 3:인벤, 4:리그, 5:캠페인, 6:크루, 7:소셜, 8:메일)
    public GameObject[] _objQuestIcon;              //알람 아이콘(0:길드, 1:상점, 2:캠페인, 3:크루)
    public Camera _characterViewCam;                //캐릭터 뷰 카메라
    public Vector3[] charCamPos;                    //캐릭터 카메라 위치값(0:메인, 1:크루, 2:캐릭터)
    public Vector3[] charCamRot;                    //캐릭터 카메라 회전값(0:메인, 1:크루, 2:캐릭터)
    public int[] charCamFOV;                        //캐릭터 카메라 Field Of View(0:메인, 1:크루, 2:캐릭터);
    public QuickQuestButton Btn_QuickQuest;         //현재 퀘스트 버튼
    public GameObject _dirLight;                    //메인 라이트

    public Text m_VipLevelText;
    public Image imgOdinGradeIcon;
    public Image imgOdinProgressGruge;
    public Vector3 _characterRotation;              //캐릭터 회전

    public Vector3[] _characterRootPostion;         //캐릭터 부모 위치
    /*
    // 2016.11.21 jy 사용하는 부분이 없어져 주석 걸음 기존에 셋팅값은 스샷찍어 저장함
    public Vector3[] _characterScale;           //캐릭터 크기	
    public Vector3[] _characterPostion;         //캐릭터 위치	
    public Vector3[] _characterParentScale;     //캐릭터 부모 크기
    public Vector3[] Flag_Pos;                  //깃발 위치(0 == 챔피언, 1 == 전설, 2 == 일반)
    public Vector3[] FlagNum_Pos;               //깃발 숫자 위치(0 == 챔피언, 1 == 전설, 2 == 일반)
    */
    public Vector3[] _lightRotation;            //디렉셔널 라이트 회전값(0:메인, 1:크루)

    public Sprite[] Sprite_Element;             //속성 이미지

    //private int _nSelectedSlot;					//선택된 캐릭터
    private int _nSelectedCrew;					//선택된 캐릭터의 크루
    StringBuilder tempStringBuilder;            //스트링 합치기용 스트링 빌더
    public int prevScreen;                      //이전 화면 0=로비, 1=크루

    public Button _ShortcutAchieveBtn;          // 업적 바로가기 버튼
    private int _nShortcutAchieveType;          // 바로가기 업적 타입

    private QuestPanel _cQuestPanel;

    public RectTransform LeftBtnParent;
    public GameObject objStrongRecommendBtn;
    GameObject EventDungeonBtn;
	public GameObject AdventoDungeonBtn;
    private float LeftBtnGapX = 110f;
    private float LeftBtnPosY = -30f;
	Dictionary<UInt16, GameObject> dicUnlockDungeonBtns = new Dictionary<ushort, GameObject>();

    [SerializeField] GameObject BoostExp;
    [SerializeField] GameObject BoostGold;
    /*
	// 2016.12. 19 jy
	// 사용하지 않는 변수
	float[] times = new float[3];
	float[] ticks = new float[3];
	*/
    private Vector3[,] _characterWindowPos = new Vector3[4, 3];

    private int _nCamViewIndex = 0;
    TimeSpan tsLeftTime;
    TimeSpan tsStartTime;

    public SubChatting _subChatWidown;
    private OdinMainPopup _OdinMissionPopup;

	public GameObject _btnLeague;

    void Awake()
    {
		#if UNITY_ONESTORE
		const string platform = "OneStore";
		#elif UNITY_ANDROID        
		const string platform = "Google";
		#elif UNITY_IOS
		const string platform = "Apple";
		#endif  
        //FadeEffectMgr.Instance.FadeIn();
        tempStringBuilder = new StringBuilder();
        prevScreen = 0;
        //TextManager.Instance.lstTextObject.Clear();
        //Legion.Instance.SetUserData();
        //Legion.Instance.sName = "소라카직스웨인";점
        //ShopInfoMgr.Instance.SettingInApp();
#if UNITY_EDITOR

#elif UNITY_IOS
		IgaworksCorePluginIOS.SetUserId(Server.ServerMgr.Instance.GetBuildTag()+"_"+Server.ServerMgr.platform+"_"+Server.ServerMgr.id);
		LiveOpsPluginIOS.LiveOpsInitPush();
		LiveOpsPluginIOS.LiveOpsSetTargetingStringData(Application.systemLanguage.ToString(), "Language");
		LiveOpsPluginIOS.LiveOpsSetTargetingStringData(platform, "Store"); 
		LiveOpsPluginIOS.LiveOpsSetTargetingNumberData(Legion.Instance.u2Level, "Level");
		if(!Legion.Instance.pushRegisted || Legion.Instance.termspush)
		{
			LiveOpsPluginIOS.LiveOpsSetRemotePushEnable(true);
		}
#elif UNITY_ANDROID
		IgaworksUnityPluginAOS.Common.setUserId(Server.ServerMgr.Instance.GetBuildTag()+"_"+Server.ServerMgr.platform+"_"+Server.ServerMgr.id);
		IgaworksUnityPluginAOS.LiveOps.initialize ("432232593496");//432232593496
		IgaworksUnityPluginAOS.LiveOps.setTargetingData("Level", Legion.Instance.u2Level);
		IgaworksUnityPluginAOS.LiveOps.setTargetingData("Language", Application.systemLanguage.ToString());
		IgaworksUnityPluginAOS.LiveOps.setTargetingData("Store", platform); 

		if(!Legion.Instance.pushRegisted || Legion.Instance.termspush)
		{
			IgaworksUnityPluginAOS.LiveOps.enableService(true);
		}

		IgaworksUnityPluginAOS.LiveOps.setLiveOpsPopupEventListener ();

		IgaworksUnityPluginAOS.OnLiveOpsCancelPopupBtnClick = mOnLiveOpsPopupCancel;
		IgaworksUnityPluginAOS.OnLiveOpsPopupClick = mOnLiveOpsPopupClick;

		//		IgaworksUnityPluginAOS.OnReceiveDeeplinkData = mOnReceiveDeeplinkData;
		//		IgaworksUnityPluginAOS.LiveOps.requestPopupResource ();
		//		IgaworksUnityPluginAOS.LiveOps.showPopUp("testnotice");
#endif

        _dirLight = GameObject.Find("envi").transform.GetChild(0).gameObject;
        Server.ServerMgr.Instance.LegionMark(3, emptyMethod2);
        if (Legion.Instance.cLeagueCrew.u1Count == 0 && Legion.Instance.sName != "")
        {
			if (Legion.Instance.cTutorial.au1Step[4] == Server.ConstDef.LastTutorialStep)
            {
                ObscuredPrefs.SetBool("RegistLeague", false);
            }
        }

        EventDungeonBtn = AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/Btn_EventDungeon.prefab", typeof(GameObject)) as GameObject;
        List<EventDungeonShopInfo> eventDungeonList = EventInfoMgr.Instance.CheckOpenDungeon();
        for (int i = 0; i < eventDungeonList.Count; i++) {
			if (eventDungeonList [i].u1UIType == 1) {
				AdventoDungeonBtn.SetActive (true);
				AdventoDungeonBtn.GetComponent<UI_EventDungeonBtn> ().Init (eventDungeonList [i]);
			} else {
				GameObject btnObj = Instantiate (EventDungeonBtn) as GameObject;
				float ui_Xpos = GetLeftBtnLastX () + LeftBtnGapX;
				btnObj.transform.SetParent (LeftBtnParent);
				btnObj.transform.localScale = Vector3.one;
				btnObj.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (ui_Xpos, LeftBtnPosY, 0);
				btnObj.GetComponent<UI_EventDungeonBtn> ().Init (eventDungeonList [i]);
			}
        }
        CheckUnlockBoostEvent();
        CreateMarbleEventBtn();
		CheckBossRush ();
    }

    float GetLeftBtnLastX() {
        float x = -250f;
        for (int i = 0; i < LeftBtnParent.childCount; i++) {
			if (LeftBtnParent.GetChild (i).gameObject.activeSelf) {
				if (LeftBtnParent.GetChild (i).GetComponent<RectTransform> ().anchoredPosition.x > x) {
					x = LeftBtnParent.GetChild (i).GetComponent<RectTransform> ().anchoredPosition.x;
				}
			}
        }

        return x;
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

        //foreach (KeyValuePair<UInt16, EventReward> item in EventInfoMgr.Instance.dicEventReward)
        //{
        //    if(item.Value.eventType == (Byte)EVENT_TYPE.BUFF_EXP)
        //    {
        //        tsLeftTime = item.Value.dtEventEnd - Legion.Instance.ServerTime;
        //        tsStartTime = Legion.Instance.ServerTime - item.Value.dtEventBegin;
        //        UInt16 _eventID = item.Key;
        //        if(tsStartTime.TotalSeconds < 0)
        //        {
        //            EventInfoMgr.Instance.u4ExpBoostPer = 0;
        //            if(EventInfoMgr.Instance.dicEventReward.ContainsKey(_eventID))
        //                EventInfoMgr.Instance.dicEventReward.Remove(_eventID);
        //            BoostExp.SetActive(false);
        //            break;
        //        }
        //        if(tsLeftTime.TotalSeconds < 0)
        //        {
        //            EventInfoMgr.Instance.u4ExpBoostPer = 0;
        //            EventInfoMgr.Instance.dicEventReward.Remove(_eventID);
        //            BoostExp.SetActive(false);
        //            break;
        //        }
        //        BoostExp.SetActive(true);
        //        EventInfoMgr.Instance.u4ExpBoostPer = item.Value.recordValue;
        //        break;
        //    }
        //    else
        //    {
        //        BoostExp.SetActive(false);
        //    }
        //}
        //
        //foreach (KeyValuePair<UInt16, EventReward> item in EventInfoMgr.Instance.dicEventReward)
        //{
        //    if(item.Value.eventType == (Byte)EVENT_TYPE.BUFF_GOLD)
        //    {
        //        tsLeftTime = item.Value.dtEventEnd - Legion.Instance.ServerTime;
        //        tsStartTime = Legion.Instance.ServerTime - item.Value.dtEventBegin;
        //        UInt16 _eventID = item.Key;
        //        if(tsStartTime.TotalSeconds < 0)
        //        {
        //            EventInfoMgr.Instance.u4GoldBoostPer = 0;
        //            if(EventInfoMgr.Instance.dicEventReward.ContainsKey(_eventID))
        //                EventInfoMgr.Instance.dicEventReward.Remove(_eventID);
        //            BoostGold.SetActive(false);
        //            break;
        //        }
        //        if(tsLeftTime.TotalSeconds < 0)
        //        {
        //            EventInfoMgr.Instance.u4GoldBoostPer = 0;
        //            EventInfoMgr.Instance.dicEventReward.Remove(_eventID);
        //            BoostGold.SetActive(false);
        //            break;
        //        }
        //        BoostGold.SetActive(true);
        //        EventInfoMgr.Instance.u4GoldBoostPer = item.Value.recordValue;
        //        break;
        //    }
        //    else
        //    {
        //        BoostGold.SetActive(false);
        //    }
        //}
    }

    void mOnLiveOpsPopupCancel() {
        DebugMgr.Log("PopupCancel :::: call");
    }

    void mOnLiveOpsPopupClick() {
        DebugMgr.Log("PopupClick :::: call");
    }

    void mOnReceiveDeeplinkData(string deeplink) {
        //전달받은 딥링크 데이터를 이용하여 추가 액션을 구현합니다.
        DebugMgr.Log("deep link data :::: " + deeplink);
    }

    //void OnDisable()
    //{
    //TextManager.Instance.lstTextObject.Clear();
    //}

	void mOnReceiveRegistrationId(string regId) {
		DebugMgr.Log("regId = "+regId);
        if (regId != "") {
            // 푸쉬 기기 등록 성공시 서버로 기기 정보 보냄
            //new MobileNativeMessage("Succeeded", "Device Id: " + res.deviceId);
			if (!Legion.Instance.pushRegisted) {
				PopupManager.Instance.ShowLoadingPopup(1);
				Server.ServerMgr.Instance.OptionPush (regId, AckOptionPush);
				Legion.Instance.pushRegisted = true;
			}
        } else {
            //new MobileNativeMessage("Failed", "No device id");
            DebugMgr.Log("Regist Failed");
        }
    }

    private void AckOptionPush(Server.ERROR_ID err)
    {
        DebugMgr.Log("AckShopAuto " + err);

        PopupManager.Instance.CloseLoadingPopup();

        if (err != Server.ERROR_ID.NONE) {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.OPTION_PUSH, err), Server.ServerMgr.Instance.CallClear);
        }
        else
        {
            for (int i = 0; i < Legion.MAX_PUSH_EVENT_ID; ++i)
            {
                Byte hexCode = (Byte)(0x01 << i + 1);
                if (ObscuredPrefs.GetBool("pushActive" + i) == true)
                    Legion.Instance.pushSetting |= hexCode;
            }
            Server.ServerMgr.Instance.OptionSet(Legion.Instance.pushSetting, (Byte)TextManager.Instance.eLanguage, RequestTogglePush);
        }
    }

    public void RequestTogglePush(Server.ERROR_ID err)
    {
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.OPTION_SET, err), Server.ServerMgr.Instance.CallClear);
            return;
        }

        else if (err == Server.ERROR_ID.NONE)
        {
            ObscuredPrefs.SetInt("pushSetting", Legion.Instance.pushSetting);
            //if(_togglePush.GetComponent<Toggle>().isOn)
            //Legion.Instance.SetPushAtiveList();
            /*
                if((Legion.Instance.pushSetting & 0x02) > 0)
                {
                    Legion.Instance.pushActive = true;
                    ObscuredPrefs.SetBool("pushActive", true);
                }
                else
                {
                    Legion.Instance.pushActive = false;
                    ObscuredPrefs.SetBool("pushActive", false);
                }
            */
        }
    }

    void Start()
    {
        //#CHATTING
		//기능막음 2017.08.08 jc
		_subChatWidown.gameObject.SetActive(false);

//        if (_subChatWidown != null)
//        {
//            if (PopupManager.Instance.IsChattingActive())
//            {
//                PopupManager.Instance.SetSubChtting(_subChatWidown);
//                _subChatWidown.gameObject.SetActive(true);
//            }
//        }

        Legion.Instance.googleAnalytics.LogEvent(new EventHitBuilder().SetEventCategory("Lobby").SetEventAction("Open").SetEventLabel("LobbyOpen"));
        CheckOptions();
        SetEffectCam();
        InitCharacter();
        InitCrewEmblem();
        InitUserInfo();
        InitClickWindow();
        SetDivisionMark();
        _nSelectedCrew = Legion.Instance.cBestCrew.u1Index;
        //추후 튜토리얼 구현 후 적용
        if (Legion.Instance.sName == "")
        {
            _userInfo.SetActive(false);
            //Transform Btn_Crew = _mainMenu.transform.FindChild("BG_Left").FindChild("Btn_Crew");
            //Btn_Crew.gameObject.SetActive(false);
            //Btn_Crew.GetChild(0).gameObject.SetActive(false);
            //Btn_Crew.GetChild(1).gameObject.SetActive(false);
            //Btn_Crew.GetChild(4).gameObject.SetActive(true);
            //Btn_Crew.GetComponent<Button>().interactable = false;
        }

		//for Crew Tutorial Force Quit
		bool bRestartCrewTutorial = false;
        UInt16 tempStageNum = LegionInfoMgr.Instance.acCrewOpenGoods[0][1].u2ID;
        if (StageInfoMgr.Instance.dicStageData[tempStageNum].clearState > 0)
        {
            _userInfo.SetActive(true);
            if (Legion.Instance.cTutorial.au1Step[4] != Server.ConstDef.LastTutorialStep && Legion.Instance.cTutorial.au1Step[4] != 0)
            {
                OnClickCrew();
				bRestartCrewTutorial = true;
                //Transform Btn_Crew = _mainMenu.transform.FindChild("BG_Left").FindChild("Btn_Crew");
                //Btn_Crew.gameObject.SetActive(true);
            }
        }

		if (!bRestartCrewTutorial) {
			if (Legion.Instance.bCrewToChar || Legion.Instance.bStageToCrew) 
			{
				OnClickCrew ();
			}
			else if (Legion.Instance.bLeagueToCharInfo) 
			{
				StartCoroutine (OnClickCharacterInfo (3, 0, Legion.Instance.acHeros.Find(cs => cs.u1Index == UI_League.Instance.u1SelectLeagueCharIndex), 3));
			} 
			else 
			{
				ChangeCamView ("LOBBY");
				// 2017. 01. 16 jy
				// 크루 메뉴에 입장시에도 튜토리얼이 진행되었으므로 
				// 로비 일때만 튜토리얼이 진행됟도록 튜토리얼 체크 코드 위치 변경
				if (GameManager.Instance.GetReserveCount () == 0) {
					Legion.Instance.cTutorial.CheckTutorial (MENU.MAIN);
					_eventPanel.Init ();
					CheckLoginPopupStep ();
                }
			}
		}

        //if(Legion.Instance.cBestCrew.u1Count == 0)
        //{
        //    OnClickCrew();
        //}

        Btn_QuickQuest.SetButton();
        RefreshAlram();

        //		if (!Legion.Instance.bADView && !Legion.Instance.bCafeView && !Legion.Instance.cTutorial.bIng) {
        //			GLink.sharedInstance ().executeHome ();
        //		}
        //
        StartCoroutine(CheckReservedPopup());

        //StartCoroutine(OnAnimation());

        //		if(	Legion.Instance.cTutorial.CheckTutorial(MENU.MAIN) )
        //		{
        //			DebugMgr.Log("Tut Main");
        //		}
        /*else if( Legion.Instance.cTutorial.CheckTutorial(MENU.CHARACTER_INFO) )
		{

			DebugMgr.Log("Tut Equip");
		}*/

        //for (int i = 0; i < 3; i++) ticks [i] = UnityEngine.Random.Range (10f, 15f);
        //InitClickWindow();
        //Legion.Instance.u1RecvLoginReward = 1;
        //_eventPanel.GetComponent<EventPanel>().OnClickMenu(0);

        // 2017. 01. 06 jy
        // 최초 계정생성후 접속시 푸시 셋팅이 서버값과 다르면 현재 클라의 값을 서버에 넘긴다
        if (ObscuredPrefs.GetBool("IsRequestPushSetting", false) == true)
        {
            Legion.Instance.SetPushAtiveList(true);
            Server.ServerMgr.Instance.OptionSet(Legion.Instance.pushSetting, (Byte)TextManager.Instance.eLanguage, RequestTogglePush);
            ObscuredPrefs.DeleteKey("IsRequestPushSetting");
        }
        else
        {
            // 클라 푸쉬 옵션과 서버 옵션이 다르다면 동기화 시킨다
            int push = ObscuredPrefs.GetInt("pushSetting", -1);
            if(push > 0 && Legion.Instance.pushSetting != push)
            {
                Legion.Instance.pushSetting = (Byte)push;
                Server.ServerMgr.Instance.OptionSet(Legion.Instance.pushSetting, (Byte)TextManager.Instance.eLanguage, RequestTogglePush);
            }
        }

		if (!Legion.Instance.pushRegisted) {
			UM_NotificationController.OnPushIdLoadResult += OnPushIdLoaded;
			UM_NotificationController.Instance.RetrieveDevicePushId ();
		}

#if UNITY_ANDROID
//		IgaworksUnityPluginAOS.LiveOps.setRegistrationIdEventListener();
//		IgaworksUnityPluginAOS.OnReceiveRegistrationId = mOnReceiveRegistrationId;
#elif UNITY_IOS
//		byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;
//		if (token != null) {
//			string hexToken = System.BitConverter.ToString (token).Replace ("-", "");
//			DebugMgr.Log ("push token: " + hexToken);
//		}
//
//		if (!Legion.Instance.pushRegisted) {
//			PopupManager.Instance.ShowLoadingPopup(1);
//			Server.ServerMgr.Instance.OptionPush (token, AckOptionPush);
//			Legion.Instance.pushRegisted = true;
//		}
#endif
    }

	private void OnPushIdLoaded (UM_PushRegistrationResult res) {
		if(res.IsSucceeded) {
			//DebugMgr.Log("regId = "+res.deviceId);
			if (!Legion.Instance.pushRegisted) {
				PopupManager.Instance.ShowLoadingPopup(1);
				Server.ServerMgr.Instance.OptionPush (res.deviceId, AckOptionPush);
				Legion.Instance.pushRegisted = true;
			}
		} else {
			DebugMgr.Log("Regist Failed");
		}
		UM_NotificationController.OnPushIdLoadResult -= OnPushIdLoaded;
	}

    public void CheckOptions()
    {
        GameObject _lobbyDirectional = GameObject.Find("Directional Light");
		if (GraphicOption.shadow [Legion.Instance.graphicGrade])
            _lobbyDirectional.GetComponent<Light>().shadows = LightShadows.Hard;
        else
            _lobbyDirectional.GetComponent<Light>().shadows = LightShadows.None;

//        if (ObscuredPrefs.GetBool("FrameToggle", true))
//            Application.targetFrameRate = 60;
//        else
//            Application.targetFrameRate = 24;
    }

    public void SetEffectCam()
    {
        if (ObscuredPrefs.GetBool("EffectCamColor", false))
            _characterViewCam.GetComponent<ColorCorrectionCurves>().enabled = true;
        else
            _characterViewCam.GetComponent<ColorCorrectionCurves>().enabled = false;

        if (ObscuredPrefs.GetBool("EffectCamBloom", false))
            _characterViewCam.GetComponent<Bloom>().enabled = true;
        else
            _characterViewCam.GetComponent<Bloom>().enabled = false;

        if (ObscuredPrefs.GetBool("EffectCamDOF", false))
            _characterViewCam.GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>().enabled = true;
        else
            _characterViewCam.GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>().enabled = false;
    }

    public void MonthlyLoginReward(Server.ERROR_ID err)
    {
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
            return;
        }

        else if (err == Server.ERROR_ID.NONE)
        {
            Legion.Instance.u1RecvLoginReward = 1;
            
            CheckLoginPopupStep();
            //_eventPanel.GetComponent<EventPanel>().OnClickMenu(0);
        }
    }

    public override void RefreshAlram()
    {
        // 2016. 12. 26 jy 대장간 입장 버튼 숨김
        //CheckAlram_Forge();
        CheckAlram_Inventory();
        CheckAlram_Shop();
        CheckAlram_Campaign();
        CheckAlram_GuardianGuild();
        CheckAlram_Quest();
        CheckAlram_Crew();
        CheckAlram_Social();
        CheckAlram_BattleField();
        CheckAlram_VIP();
    }
    
    // 새로운 아이템이나 스탯이 남은 장비가 있으면 표시해준다
    void CheckAlram_Inventory()
    {
        bool check = false;
        foreach (Item item in Legion.Instance.cInventory.dicInventory.Values)
        {
            if (item.isNew)
            {
                if (item.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT && ((EquipmentItem)item).attached.hero != null)
                    continue;

                check = true;
                break;
            }
        }
        
        _objAlramIcon[(int)LobbyAlram.INVENTROY].SetActive(check);
    }

    // 암시장 입장 가능시간 or 일반 상점 갱신시 or 장비 상점 갱신시 느낌표 표시
    void CheckAlram_Shop()
    {
        bool check = false;
        // 상점에 입장한적이 없다면 체크하지 않는다
        if (ShopInfoMgr.Instance.lastShopTime.Equals(DateTime.MinValue) == true)
        {
            ShopInfoMgr.Instance.shopVisit = false;
            ShopInfoMgr.Instance.normalCheck = false;
        
            // 암시장이 오픈되어 있다면 New 표시
            if ((BLACK_SHOP_STATE)Legion.Instance.u1BlackMarketOpen != BLACK_SHOP_STATE.CLOSE)
            {
                check = true;
                ShopInfoMgr.Instance.blackCheck = true;
            }
            else
                ShopInfoMgr.Instance.blackCheck = false;
        }
        else
        {
            ShopInfoMgr.Instance.blackCheck = false;
            switch ((BLACK_SHOP_STATE)Legion.Instance.u1BlackMarketOpen)
            {
                case BLACK_SHOP_STATE.OPEN:
                    // 마지막 상점 입장시간과 현재 시간과 다른지 체크
                    if (ShopInfoMgr.Instance.lastShopTime.ToString("MM/dd/yyyy HH") != Legion.Instance.ServerTime.ToString("MM/dd/yyyy HH"))
                    {
                        check = true;
                        ShopInfoMgr.Instance.blackCheck = true;
                        ShopInfoMgr.Instance.shopVisit = false;
                    }
                    break;
                case BLACK_SHOP_STATE.VIP_OPEN:
                    // 년월일이 시간 다른지 체크
                    if (ShopInfoMgr.Instance.lastShopTime.ToString("MM/dd/yyyy HH") != Legion.Instance.ServerTime.ToString("MM/dd/yyyy HH"))
                    {
                        // 암시장 정보를 받는다
                        ShopInfo blackShop = ShopInfoMgr.Instance.dicShopData[3];
                        int hour = Legion.Instance.ServerTime.Hour;
                        for (int i = 0; i < blackShop.u1ArrResetTime.Length; i++)
                        {
                            // 오픈시간과 같은 시간인지 확인한다
                            if (hour == blackShop.u1ArrResetTime[i])
                            {
                                check = true;
                                ShopInfoMgr.Instance.blackCheck = true;
                                ShopInfoMgr.Instance.shopVisit = false;
                                break;
                            }
                        }
                    }
                    break;
            }

            //ShopInfo blackShop = ShopInfoMgr.Instance.dicShopData[3];
            //ShopInfo blackShop2 = ShopInfoMgr.Instance.dicShopData[1];
            //// 암시장 상시 오픈 상태에서
            //if ((BLACK_SHOP_STATE)Legion.Instance.u1BlackMarketOpen == BLACK_SHOP_STATE.VIP_OPEN)
            //{
            //    DateTime blackShopRefreshTime = DateTime.MinValue;
            //    for (int i = 0; i < blackShop2.u1ArrResetTime.Length; i++)
            //    {
            //        if (ShopInfoMgr.Instance.lastShopTime < Legion.Instance.ServerTime.Date.AddHours(blackShop2.u1ArrResetTime[i]))
            //        {
            //            blackShopRefreshTime = Legion.Instance.ServerTime.Date.AddHours(blackShop2.u1ArrResetTime[i]);
            //            break;
            //        }
            //    }
            //
            //    if (Legion.Instance.ServerTime > blackShopRefreshTime)
            //    {
            //        check = true;
            //        ShopInfoMgr.Instance.blackCheck = true;
            //        ShopInfoMgr.Instance.shopVisit = false;
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < blackShop.u1ArrResetTime.Length; i++)
            //    {
            //        if (Legion.Instance.ServerTime >= Legion.Instance.ServerTime.Date.AddHours(blackShop.u1ArrResetTime[i])
            //        && Legion.Instance.ServerTime <= Legion.Instance.ServerTime.Date.AddHours(blackShop.u1ArrResetTime[i]).AddSeconds(blackShop.u4KeepTime))
            //        {
            //            check = true;
            //            ShopInfoMgr.Instance.blackCheck = true;
            //            ShopInfoMgr.Instance.shopVisit = false;
            //            break;
            //        }
            //    }
            //}

            ShopInfo normalShop = ShopInfoMgr.Instance.dicShopData[1];
            DateTime noramlRefreshTime = DateTime.MinValue;
            for (int i = 0; i < normalShop.u1ArrResetTime.Length; i++)
            {
                if (ShopInfoMgr.Instance.lastShopTime < Legion.Instance.ServerTime.Date.AddHours(normalShop.u1ArrResetTime[i]))
                {
                    noramlRefreshTime = Legion.Instance.ServerTime.Date.AddHours(normalShop.u1ArrResetTime[i]);
                    break;
                }
            }

            if (Legion.Instance.ServerTime > noramlRefreshTime)
            {
                check = true;
                ShopInfoMgr.Instance.normalCheck = true;
                ShopInfoMgr.Instance.shopVisit = false;
            }

            ShopInfo equipShop = ShopInfoMgr.Instance.dicShopData[2];
            DateTime equipRefreshTime = DateTime.MinValue;

            for (int i = 0; i < equipShop.u1ArrResetTime.Length; i++)
            {
                if (ShopInfoMgr.Instance.lastShopTime < Legion.Instance.ServerTime.Date.AddHours(equipShop.u1ArrResetTime[i]))
                {
                    equipRefreshTime = Legion.Instance.ServerTime.Date.AddHours(equipShop.u1ArrResetTime[i]);
                    break;
                }
            }

            if (Legion.Instance.ServerTime > equipRefreshTime)
            {
                check = true;
                ShopInfoMgr.Instance.equipCheck = true;
                ShopInfoMgr.Instance.shopVisit = false;
            }
        }
        _objAlramIcon[(int)LobbyAlram.SHOP].SetActive(check);
        _objQuestIcon[(int)LobbyQuestAlram.SHOP].SetActive(Legion.Instance.cQuest.CheckQuestAlarm(MENU.SHOP, 0));
    }

    // 수령하지 않은 반복 보상 or 별보상이 있으면 표시
    void CheckAlram_Campaign()
    {
        bool check = false;

        for (int i = 0; i < Legion.Instance.acCrews.Length; i++)
        {
            if (!Legion.Instance.acCrews[i].abLocks[0])
            {
                if (Legion.Instance.acCrews[i].DispatchStage != null)
                {
                    TimeSpan timeSpan = Legion.Instance.acCrews[i].DispatchTime - Legion.Instance.ServerTime;

                    if (timeSpan.Ticks <= 0)
                    {
                        check = true;
                        break;
                    }
                }
            }
        }

        if (!check)
        {
            foreach (ActInfo actInfo in StageInfoMgr.Instance.dicActData.Values)
            {
                // 2016. 06. 30 jy
                // 탑 모드에서는 반복 및 별 보상 없으므로 확인하지 않는다
                if (actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER)
                    continue;

                for (int i = 0; i < actInfo.lstChapterID.Count; i++)
                {
                    if (StageInfoMgr.Instance.dicChapterData[actInfo.lstChapterID[i]].RewardEnable())
                    {
                        check = true;
                        break;
                    }
                }
            }
        }

        _objAlramIcon[(int)LobbyAlram.CAMPAINGN].SetActive(check);
        _objQuestIcon[(int)LobbyQuestAlram.CAMPAINGN].SetActive(Legion.Instance.cQuest.CheckQuestAlarm(MENU.CAMPAIGN, 0));
    }

    void CheckAlram_Quest()
    {
        // 완료된 퀘스트가 있다면 New 표시를 띄운다 
        if(Legion.Instance.cQuest.isClearQuest() == true)
        { 
            _objAlramIcon[(int)LobbyAlram.QUEST].SetActive(true);
        }
        else
        {
            if (Legion.Instance.cQuest.u2IngQuest != 0)
            {
                _objAlramIcon[(int)LobbyAlram.QUEST].SetActive(false);
                return;
            }
            _objAlramIcon[(int)LobbyAlram.QUEST].SetActive(Legion.Instance.cQuest.IsHaveOpenQuest());
        }
    }

    void CheckAlram_GuardianGuild()
    {
        bool check = false;

        // 2016. 11. 28 jy
        // 업적 바로가기 버튼이 생겨서 무조건 업적을 체크해야 하기때문에 알람체크 우선순위를 위로 변경
        check = CheckClearAchievement();
        if (!check)
        {
            for (int i = 0; i < Legion.Instance.acHeros.Count; i++)
            {
                if (Legion.Instance.acHeros[i].u1SeatNum != 0)
                {
                    UInt16 id = (UInt16)(Server.ConstDef.BaseCharTrainingID + Legion.Instance.acHeros[i].u1RoomNum);
                    TimeSpan timeSpan = QuestInfoMgr.Instance.GetCharTrainingInfo()[id].doneTime - Legion.Instance.ServerTime;

                    if (timeSpan.Ticks <= 0)
                    {
                        check = true;
                        break;
                    }
                }
            }
        }

        //장비 수련의 방이 없어졌으므로 체크할 필요가 없음
        //if (!check)
        //{
        //    Legion.Instance.cInventory.EquipSort();
        //    List<EquipmentItem> lstItem = Legion.Instance.cInventory.lstSortedEquipment;
        //    for (int i = 0; i < lstItem.Count; i++)
        //    {
        //        if (lstItem[i].u1SeatNum != 0)
        //        {
        //            UInt16 id = (UInt16)(Server.ConstDef.BaseEquipTrainingID + lstItem[i].u1RoomNum);
        //            TimeSpan timeSpan = QuestInfoMgr.Instance.GetEquipTrainingInfo()[id].doneTime - Legion.Instance.ServerTime;
        //
        //            if (timeSpan.Ticks <= 0)
        //            {
        //                check = true;
        //                break;
        //            }
        //        }
        //    }
        //}

        _objAlramIcon[(int)LobbyAlram.GUILD].SetActive(check);
        _objQuestIcon[(int)LobbyQuestAlram.GUILD].SetActive(Legion.Instance.cQuest.CheckQuestAlarm(MENU.HERO_GUILD, 0));
    }

    void CheckAlram_Crew()
    {
        if (Legion.Instance.sName == "")
        {
            _objAlramIcon[(int)LobbyAlram.CREW].SetActive(false);
            _objQuestIcon[(int)LobbyQuestAlram.CREW].SetActive(false);
            return;
        }
        _objAlramIcon[(int)LobbyAlram.CREW].SetActive(Legion.Instance.CheckNoticeAlram());
        _objQuestIcon[(int)LobbyQuestAlram.CREW].SetActive(Legion.Instance.cQuest.CheckQuestAlarm(MENU.CREW, 0));
    }

    void CheckAlram_Social()
    {
		if (_objAlramIcon [(int)LobbyAlram.SOCIAL] == null || _objAlramIcon [(int)LobbyAlram.MAIL] == null)
			return;

        // 친구 리스트 알람 및 메일 리스트 알람
        if (Legion.Instance.u1FriendExist != 0)
            _objAlramIcon[(int)LobbyAlram.SOCIAL].SetActive(true);
        else
            _objAlramIcon[(int)LobbyAlram.SOCIAL].SetActive(false);

        if (Legion.Instance.u1MailExist == 1)
            _objAlramIcon[(int)LobbyAlram.MAIL].SetActive(true);
        else
            _objAlramIcon[(int)LobbyAlram.MAIL].SetActive(false);
    }

    void CheckAlram_BattleField()
    {
        if (LegionInfoMgr.Instance.IsContentOpen(LegionInfoMgr.LEAGUE_CONTENT_ID) == true)
        {
            if (!ObscuredPrefs.GetBool("RegistLeague", false))
            {
                if (Legion.Instance.sName == "")
                    _objAlramIcon[(int)LobbyAlram.LEAGUE].SetActive(false);
                else
                {
                    _objAlramIcon[(int)LobbyAlram.LEAGUE].SetActive(true);
                    return;
                }
            }
        }

        if (LegionInfoMgr.Instance.IsContentOpen(LegionInfoMgr.RANK_CONTENT_ID) == true)
        {
            if (Legion.Instance.u1RankRewad == 1)
            {
                _objAlramIcon[(int)LobbyAlram.LEAGUE].SetActive(true);
                return;
            }
            else
                _objAlramIcon[(int)LobbyAlram.LEAGUE].SetActive(false);
        }
        
        _objAlramIcon[(int)LobbyAlram.LEAGUE].SetActive(false);
    }

    //팝업 출력용
    void emptyMethod2(Server.ERROR_ID err)
	{
        // Awake 에서 메일 리스트를 호출하지만
        // 기존 로직에 알람이 먼저 확인하여 New 표시가 뜨지않음
        // 우편을 받으면 셋팅하도록 수정
        CheckAlram_Social();
    }

    IEnumerator OnAnimation()
    {
        yield return new WaitForSeconds(1.1f);
        //_mainMenu.GetComponent<Animator>().enabled = true;
        mainPanel.GetComponent<Animator>().enabled = true;
    }

	public override IEnumerator CheckReservedPopup()
	{
		GameManager.ReservedPopup reservedPopup = null;
        
        bool setPopup = false;
        
		for(int i=0; i<menuType.Length; i++)
		{
			//DebugMgr.Log(menuType[i]);
			reservedPopup = GameManager.Instance.GetReservedPopup(menuType[i]);
			if(reservedPopup != null)
			{
                setPopup = true;
                
				DebugMgr.Log("OK");
				DebugMgr.Log("Reserved Popup : " + reservedPopup.menu);
				if(reservedPopup.menu == MENU.MAIN)
				{
					if (reservedPopup.popup == (int)POPUP_MAIN.ADVENTO) {
						if (AdventoDungeonBtn.activeSelf) {
							AdventoDungeonBtn.GetComponent<UI_EventDungeonBtn> ().OnClickThis ();
						}
					}
                    else if (reservedPopup.popup == (int)POPUP_MAIN.ODIN)
                    {
                        onClickVIPButton();
                    }
                }
				else if(reservedPopup.menu == MENU.INVENTORY)
				{                    
                    yield return StartCoroutine(base.OpenInventory(false));
					inventoryPanel.Open(reservedPopup.GetReservedPopupInventory());
				}
				else if(reservedPopup.menu == MENU.SHOP)
				{                    
                    yield return StartCoroutine(base.OpenShop(false));
					shopPanel.Open(reservedPopup.GetReservedPopupShop());
				}
				else if(reservedPopup.menu == MENU.CREW)
				{
                    Legion.Instance.bStageToCrew = false;
					OnClickCrew();
				}
				else if(reservedPopup.menu == MENU.CHARACTER_INFO)
				{
                    if(reservedPopup.param != null)
                        yield return StartCoroutine(OnClickCharacterInfo(0, (byte)reservedPopup.param[0], (Hero)reservedPopup.param[1], 1, false));
                    else
						yield return StartCoroutine(OnClickCharacterInfo(0, Legion.Instance.cBestCrew.u1Index, null, 0, false));
                        
                    _characterInfo.Open(reservedPopup.GetReservedPopupCharacterInfo());
				}
				else if(reservedPopup.menu == MENU.QUEST)
				{
					OnClickQuest ();
				}
				break;
			}
		}
        
        if(!setPopup)
        {
            if(!Legion.Instance.bCrewToChar && !Legion.Instance.bStageToCrew)
                FadeEffectMgr.Instance.FadeIn();
            if(Legion.Instance.bLeagueToCharInfo)
		        FadeEffectMgr.Instance.FadeIn();
        }
	}

    public void ChangeCamView(string _type)
    {
		Quaternion rot = Quaternion.identity;
		StopCoroutine("MainCrewAni");
        switch(_type)
        {
            case "LOBBY":
				if(_characterViewCam.GetComponent<UI_CameraMove>().CameraViewIndex != 0)
					_characterViewCam.GetComponent<UI_CameraMove>().SetLobbyCamera();
				else
				{
					_characterViewCam.gameObject.transform.localPosition = charCamPos[0];
					rot.eulerAngles = charCamRot[0];
					_characterViewCam.gameObject.transform.rotation = rot;
					_characterViewCam.GetComponent<UI_CameraMove>().SaveCameraInfo();
				}
				_characterViewCam.fieldOfView = charCamFOV[0] * Legion.Instance.ratio;
				SetCrewPower();
				SetCharacterClickWindow(ObscuredPrefs.GetInt("LobbyCameraIndex"));
				StartCoroutine("MainCrewAni");
				SetCharacterExpBarPos("LOBBY");
                break;
            case "CREW":
                _characterViewCam.transform.localPosition = charCamPos[1];
                rot.eulerAngles = charCamRot[1];
                _characterViewCam.transform.rotation = rot;
				_characterViewCam.fieldOfView = charCamFOV[1] * Legion.Instance.ratio;
				SetCharacterExpBarPos("CREW");
                break;
            case "CHARACTER_INFO":
                _characterViewCam.transform.localPosition = charCamPos[2];
                rot.eulerAngles = charCamRot[2];
                _characterViewCam.transform.rotation = rot;
				_characterViewCam.fieldOfView = charCamFOV[2] * Legion.Instance.ratio;
                break;
        }
    }

    //캠페인 버튼 선택
    public void OnClickCampaign()
    {
        StartCoroutine(GoStage());
    }

    IEnumerator GoStage()
    {
        FadeEffectMgr.Instance.FadeOut();
		//yield return new WaitForEndOfFrame ();
        yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        //현재 캐릭터 회전값 저장
        SaveCharacterRotation();
		AssetMgr.Instance.SceneLoad("SelectStageScene");
    }

    //리그 버튼 선택
    public void OnClickTempBattleField()
    {
        StartCoroutine(TempFade("MAIN"));
    }

    IEnumerator TempFade(string _panel)
    {
        FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        if(_panel == "MAIN")
        {
            //mainPanel.SetActive(true);
            _tempBattleFieldPanel.SetActive(false);
			FadeEffectMgr.Instance.FadeIn();
        }
        else if(_panel == "BATTLEFIELD")
        {

			AssetMgr.Instance.SceneLoad("BossRushScene");

//			SetMenuHideButtonEnable(false);
//            if(_tempBattleFieldPanel != null)
//                _tempBattleFieldPanel.SetActive(true);
//            else
//            {
//                _tempBattleFieldPanel = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/BattleField/BattleField.prefab", typeof(GameObject)) as GameObject);
//				RectTransform rectTr = _tempBattleFieldPanel.GetComponent<RectTransform>();
//				rectTr.SetParent(Scene.GetCurrent().mainPanel.transform);
//				rectTr.localScale = Vector3.one;
//				rectTr.localPosition = Vector3.zero;
//				rectTr.sizeDelta = Vector2.zero;
//                _tempBattleFieldPanel.SetActive(true);
//            }
        }
        //FadeEffectMgr.Instance.FadeIn();
    }

	void CheckBossRush()
	{

		if(!EventInfoMgr.Instance.dicEventReward.ContainsKey(EventInfoMgr.Instance.lstBossRush[0].u2EventID))
		{
			_btnLeague.SetActive (false);
			return;
		}
		else if(StageInfoMgr.Instance.OpenBossRush == 0)
		{
			_btnLeague.SetActive (false);
			return;
		}

		_btnLeague.SetActive (true);
	}
	
    public void OnClickLeague()
    {

		if(!EventInfoMgr.Instance.dicEventReward.ContainsKey(EventInfoMgr.Instance.lstBossRush[0].u2EventID))
		{
			return;
		}
		else if(StageInfoMgr.Instance.OpenBossRush == 0)
		{
			return;
		}

        StartCoroutine(TempFade("BATTLEFIELD"));
    }

    public void OpenLeagueScene(Server.ERROR_ID err)
    {
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
			DebugMgr.LogError("패킷 호출 하는 부분이 없음");
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            /*LeagueInfomation _leagueInfomation = UI_League.Instance._leagueInfomation;
            if(_leagueInfomation.u1CrewCount == 0)
            {
                PopupManager.Instance.ShowOKPopup(null, "리그에 등록된 크루가 없습니다.", null);
                return;
            }*/
            StartCoroutine(GoToLeague());
        }
    }
    IEnumerator GoToLeague()
    {
        if(!Legion.Instance.bLeagueToCharInfo)
        FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		PopupManager.Instance.CloseLoadingPopup();
        //현재 캐릭터 회전값 저장
        SaveCharacterRotation();
		AssetMgr.Instance.SceneLoad("LeagueScene");
    }
    //이벤트 패널 버튼 선택
    public void OnClickEventOpen()
    {
		if (this.transform.GetChild (0).GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Ani_UI_Main_ShowEvent")) {
			this.transform.GetChild (0).GetComponent<Animator> ().Play ("Ani_UI_Main_HideEvent");
		} else {
			this.transform.GetChild (0).GetComponent<Animator> ().Play ("Ani_UI_Main_ShowEvent");
		}
    }
    //크루 버튼 선택
    public void OnClickCrew()
    {
        for(int i=0; i<3; i++)
        {
            _characterPos.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }

        StartCoroutine(GoToCrew());
		SetMenuHideButtonEnable(false);
    }
    IEnumerator GoToCrew()
    {
        //if(Legion.Instance.bCrewToChar)
        //    Legion.Instance.bCrewToChar = false;
        //else if(!Legion.Instance.bStageToCrew)
        
        if(!Legion.Instance.bCrewToChar && !Legion.Instance.bStageToCrew)
        {
			if (Legion.Instance.cTutorial.au1Step [4] == Server.ConstDef.LastTutorialStep) {
//				yield return new WaitForSeconds (FadeEffectMgr.GLOBAL_FADE_TIME);
				FadeEffectMgr.Instance.FadeOut ();
				yield return new WaitForSeconds (FadeEffectMgr.GLOBAL_FADE_TIME);
			}
//		    yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        }
        ChangeCamView("CREW");
        //현재 캐릭터 회전값 저장
        SaveCharacterRotation();
        _dirLight.transform.eulerAngles = _lightRotation[1];
       
		if(_CrewMenu == null)
        {
            //_CrewMenu = Instantiate(Pref_CrewPanel);
            _CrewMenu = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/Pref_UI_Main_CrewMenu.prefab", typeof(GameObject)) as GameObject);
			RectTransform rectTr = _CrewMenu.GetComponent<RectTransform>();
			rectTr.SetParent(Scene.GetCurrent().mainPanel.transform);
			rectTr.localScale = Vector3.one;
			rectTr.localPosition = Vector3.zero;
			rectTr.sizeDelta = Vector2.zero;
			rectTr.name = "Pref_UI_Main_CrewMenu";
        }
		_CrewMenu.SetActive(true);
        _mainMenu.SetActive(false);
        _userInfo.SetActive(false);
        _goods.SetActive(false);

        LeanTween.alpha(Btn_ShowMenu.GetComponent<RectTransform>(), 1f, 0.3f).setDelay(0.1f);

        Btn_ShowMenu.gameObject.SetActive(false);
		for(int j=0; j<Crew.MAX_CHAR_IN_CREW; j++){
			_characterPos.transform.GetChild(j).GetComponent<CanvasGroup>().alpha = 1f;
		}
    }
    //인벤토리 버튼 선택
    public void OnClickInventory()
    {
		StartCoroutine(base.OpenInventory(true));
    }
	
    //퀘스트 버튼 선택
    public void OnClickQuest()
    {
		if(_cQuestPanel == null)
		{
			GameObject questPanal = AssetMgr.Instance.AssetLoad("Prefabs/UI/Quest/QuestPanel.prefab", typeof(GameObject)) as GameObject;
			questPanal = Instantiate(questPanal) as GameObject;
			RectTransform rtTr = questPanal.GetComponent<RectTransform>();
			rtTr.SetParent(this.transform);
			rtTr.sizeDelta = Vector2.zero;
			rtTr.anchoredPosition3D = Vector3.zero;
			rtTr.localScale = Vector3.one;

			_cQuestPanel = questPanal.GetComponent<QuestPanel>();
		}
		PopupManager.Instance.AddPopup(_cQuestPanel.gameObject, _cQuestPanel.OnClose);
		_cQuestPanel.gameObject.SetActive(true);

		PlayCharacterAnim (false);
    }

	public void OnClickGuardianGuild()
	{
		if (_objAlramIcon [(int)LobbyAlram.GUILD].activeSelf) 
		{
			if(Legion.Instance.cTutorial.au1Step[2] == Server.ConstDef.LastTutorialStep)
			{
				// 2017. 01. 16 jy
				// 퀘스트씬에 인벤토리가 생겨서 인벤토리 New에도 반응하여 업적만 체크함
				for(byte i = 1; i <= 3; ++i)
				{
					if(Legion.Instance.cQuest.CheckAlarmAchievement(i) == true)
					{
						PopupManager.Instance.SetNoticePopup (MENU.HERO_GUILD);
						break;
					}
				}
			}	
		}
		StartCoroutine(GoGuardianGuild());
	}

	IEnumerator GoGuardianGuild()
	{
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        //현재 캐릭터 회전값 저장
        SaveCharacterRotation();
		AssetMgr.Instance.SceneLoad("QuestScene", false);
	}

//    public void OnClicktestPlayGame()
//	{
//		if (Legion.Instance.osLogin) {
//			GooglePlayManager.Instance.ShowAchievementsUI ();
//		}
//    }

	public void OnClickGuild()
	{
		if (Legion.Instance.sName == "") {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("menu_guild"), TextManager.Instance.GetText("popup_desc_guild_lock"), null);
			return;
		}
		StartCoroutine(GoGuild());
	}

	IEnumerator GoGuild()
	{
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		//현재 캐릭터 회전값 저장
		SaveCharacterRotation();
		AssetMgr.Instance.SceneLoad("GuildScene", false);
	}

    //소셜 버튼 선택
	public void OnClickSocial(int menuType)
    {
        if (menuType == 0 && Legion.Instance.sName == "")
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_social_friend"), TextManager.Instance.GetText("popup_desc_friend"), null);
            return;
        }
        //menuType = 0:친구메뉴, 1:우편메뉴, 2:공지메뉴,
        StartCoroutine(OpenSocial(menuType));
        //TextManager.Instance.SetLanguage("KOREAN");
    }

	IEnumerator OpenSocial(int menuType)
    {
        FadeEffectMgr.Instance.FadeOut(FadeEffectMgr.GLOBAL_FADE_TIME);
        yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        //mainPanel.SetActive(false);
        //Scene.GetCurrent().mainPanel.GetComponent<Toggle>().interactable = false;
		SetMenuHideButtonEnable(false);
		if(_socialPanel == null)
        {
            _socialPanel = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Social/SocialPanel.prefab", typeof(GameObject)) as GameObject);
            // 2016. 12. 26 jy 
            // 소설패널 부모 변경
            // 친구 포인트 클릭시 친구목록으로 이동되도록 변경에 따른 조치
            // mainPanel 에 넣을시 mainPanel이 비활성화 되어 소셜도 같이 비활성화 처리됨
            //_socialPanel.transform.SetParent(Scene.GetCurrent().mainPanel.transform);
            RectTransform rtTr = _socialPanel.GetComponent<RectTransform>();
            rtTr.SetParent(this.transform);
            rtTr.localScale = Vector3.one;
            rtTr.localPosition = Vector3.zero;
            rtTr.sizeDelta = Vector2.zero;
        }
        else
        {
            FadeEffectMgr.Instance.FadeIn(FadeEffectMgr.GLOBAL_FADE_TIME);
            _socialPanel.transform.SetAsLastSibling();
        }
        
        _socialPanel.GetComponent<SocialPanel>().OnClickListMenu(menuType);
        _socialPanel.SetActive(true);
    }

    //옵션 버튼 선택
    public void OnClickOption()
    {
        //TextManager.Instance.SetLanguage("ENGLISH");
        PopupManager.Instance.ShowOptionPopup();
        return;
    }

	//빠른 퀘스트 버튼 선택
	public void OnClickQuickQuest()
	{
		SetQuestPopup ();
	}

    //캐릭터 정보창 선택
    public void OnClickCharInfo(int slotNum)
    {		
		//cotroutine으로 호출해서 로비화면 보이고 전환 되는것 의심
        StartCoroutine(OnClickCharacterInfo(slotNum, Legion.Instance.cBestCrew.u1Index, null, 0));
        prevScreen = 0;
    }

    public Hero infoHero;
    private int _from;
    public IEnumerator OnClickCharacterInfo(int slotNum, int crewIdx, Hero inHero, int _inList, bool fade = true)
    {
        _from = _inList;

		SetMenuHideButtonEnable(false);
        //mainPanel.GetComponent<Toggle>().interactable = false;
        
        
        
		// 2016. 12. 30 jy
		// 첫번째 슬롯의 캐릭이 없다면 에러가 나옴 예외처리 : 다른 크루를 캐릭정보를 셋팅한다
		if (Legion.Instance.equipNeedHeroIndex > -1) {
			infoHero = (Hero)Legion.Instance.SelectedCrew.acLocation [Legion.Instance.equipNeedHeroIndex];
			Legion.Instance.equipNeedHeroIndex = -1;
		} else {
			if(crewIdx != 0)
			infoHero = ((Hero)Legion.Instance.acCrews[crewIdx-1].acLocation[slotNum]);
			else if(inHero != null)
			infoHero = inHero;
		}

		if(infoHero == null)
		{
			for(int i = 0; i < Crew.MAX_CHAR_IN_CREW; ++i)
			{
				infoHero = ((Hero)Legion.Instance.acCrews[crewIdx-1].acLocation[i]);
				if(infoHero != null)
				{
					slotNum = i;
					break;
				}
			}
		}

		if (infoHero.cObject != null) 
		{
			infoHero.cObject.GetComponent<HeroObject> ().SaveTransform ();
		}
		else
        {
            infoHero.InitModelObject();
			HeroObject heroObj =  infoHero.cObject.GetComponent<HeroObject>();
			heroObj.SetLayer(LayerMask.NameToLayer("BGMainMap"));
			heroObj.SetAnimations_UI();
			heroObj.PlayAnim("UI_Class_Default");
        }

		_nSelectedCrew = slotNum;
        
        Legion.Instance.cSelectedSlot = slotNum;

		for(int i=0; i<3; i++)
        {
			_characterPos.transform.GetChild(i).gameObject.SetActive(false);
            if(i != slotNum)
            {
				_bgCharacterPos.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        //if(_characterInfo == null)
		//{
        //    _characterInfo = Instantiate(Pref_CharacterInfo as GameObject).;
		//	RectTransform rtTr = _characterInfo.GetComponent<RectTransform>();
		//	rtTr.SetParent(mainPanel.transform);
		//	rtTr.localPosition = Vector3.zero;
		//	rtTr.sizeDelta = Vector2.zero;
		//	rtTr.localScale = Vector3.one;
		//}
		
        _characterInfo.SetData(infoHero, this, _inList);
        _mainMenu.SetActive(false);
        _userInfo.SetActive(false);
		LeanTween.alpha(Btn_ShowMenu.GetComponent<RectTransform>(), 0f, 0.3f);
        
        if(fade)
            yield return StartCoroutine(ShowCharacterInfo());
        else
        {
            _characterInfo.gameObject.SetActive(true);
            //_CrewMenu.gameObject.SetActive(false);
            if(_CrewMenu != null)
                _CrewMenu.GetComponent<UI_CrewMenu>().HideCharacterSlot();
        }
        ChangeCamView("CHARACTER_INFO");
        _dirLight.transform.eulerAngles = _lightRotation[0];
        //yield return null;
        //yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        
        //yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME/2);
    }

	IEnumerator ShowCharacterInfo()
	{
		yield return new WaitForEndOfFrame ();
        if(!Legion.Instance.bLeagueToCharInfo)
		    FadeEffectMgr.Instance.FadeOut();
        _goods.SetActive(true);
		_characterInfo.gameObject.SetActive(true);
        //_CrewMenu.gameObject.SetActive(false);
        if(_CrewMenu != null)
            _CrewMenu.GetComponent<UI_CrewMenu>().HideCharacterSlot();
	}

    public void CallCharacterInfoPopup()
    {
        StartCoroutine(ShowCharacterInfo());
    }

    //bool equipNew = false;
	public void OnCloseCharacterInfo(Crew _crew, Byte assignedCrew)
	{
		List<EquipmentItem> ret = new List<EquipmentItem>();
		ret = Legion.Instance.cInventory.GetNewEquip();
		

		for(int i=0; i<3; i++)
		{
			if(_crew.acLocation[i] != null)
			{
                // 2016. 09. 28 jy 
                // 캐릭터정보창의 캐릭터가 크루에 포함되어 잇지 않다면 체크 하지 않는다
                if (assignedCrew == 0)
                    continue;

                _characterPos.transform.GetChild(i).gameObject.SetActive(true);
				_bgCharacterPos.transform.GetChild(i).gameObject.SetActive(true);

				SetCharacterLevelExp((Hero)_crew.acLocation[i], i);
				//if(_crew.acLocation[i].GetComponent<StatusComponent>().CheckHaveStatPoint(_crew.acLocation[i].cLevel.u2Level))
				//	_characterPos.transform.GetChild(i).GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(true);
				//else if(_crew.acLocation[i].GetComponent<SkillComponent>().SkillPoint > 0)
				//	_characterPos.transform.GetChild(i).GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(true);
				//else

				//{
				//	for(int j=0; j<((Hero)_crew.acLocation[i]).acEquips.Length; j++)
				//	{
				//		for(int k=0; k<ret.Count; k++)
				//		{
				//			if(ret[k].GetEquipmentInfo().u2ClassID != ((Hero)_crew.acLocation[i]).acEquips[j].GetEquipmentInfo().u2ClassID)
				//				continue;
                //
				//			if(ret[k].GetEquipmentInfo().u1PosID == ((Hero)_crew.acLocation[i]).acEquips[j].GetEquipmentInfo().u1PosID && ret[k].isNew)
				//			{
				//				equipNew = true;
				//				break;
				//			}
				//		}
	            //
				//		//if(equipNew)
				//		//{
				//		//	_characterPos.transform.GetChild(i).GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(true);
				//		//	break;
				//		//}
				//		//else
				//		//	_characterPos.transform.GetChild(i).GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(false);
				//	}
				//}
			}
			else
			{
				_characterPos.transform.GetChild(i).gameObject.SetActive(false);
				_bgCharacterPos.transform.GetChild(i).gameObject.SetActive(false);
			}
		}

        if(prevScreen == 2)
        {
            _goods.SetActive(false);
            _CrewMenu.SetActive(true);

            //_CrewMenu.GetComponent<UI_CrewMenu>().SetCrewCharacters();
            ChangeCamView("CREW");
            _dirLight.transform.eulerAngles = _lightRotation[1];
            //this.transform.GetChild(0).GetComponent<Animator>().Play("Ani_UI_Main_HideMenu");
            LeanTween.alpha(Btn_ShowMenu.GetComponent<RectTransform>(), 1f, 0.3f).setDelay(0.1f);
            //LeanTween.alpha(Btn_ShowMenu.GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>(), 1f, 0.3f).setDelay(0.1f);
            //Btn_ShowMenu.GetComponent<RectTransform>().GetChild(0).GetChild(0).GetComponent<Image>().enabled = true;
        }
        else if(prevScreen == 1)
        {
            _goods.SetActive(false);
            ChangeCamView("CREW");
            _dirLight.transform.eulerAngles = _lightRotation[1];
            
            LeanTween.alpha(Btn_ShowMenu.GetComponent<RectTransform>(), 1f, 0.3f).setDelay(0.1f);
        }
        else
        {
            ChangeCamView("LOBBY");
            _goods.SetActive(true);
            _dirLight.transform.eulerAngles = _lightRotation[0];
            this.transform.GetChild(0).GetComponent<Animator>().Play("Ani_UI_Main_ShowMenu");
            LeanTween.alpha(Btn_ShowMenu.GetComponent<RectTransform>(), 0f, 0.3f);
            
			SetMenuHideButtonEnable(true);

			_mainMenu.SetActive(true);
			if(Legion.Instance.sName != "")
				_userInfo.SetActive(true);
            //리그 호출
            if(Legion.Instance.bLeagueToCharInfo)
                AssetMgr.Instance.SceneLoad("ALeagueScene", false);
        }
       
        RefreshAlram();
	}
	//크루 깃발 초기화
    public void InitCrewEmblem()
    {
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append("Sprites/Common/flag_01.division_").Append(Legion.Instance.cBestCrew.u1Index.ToString());
        _crewEmblem.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
        _crewEmblem.SetNativeSize();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        if(Legion.Instance.cBestCrew.u1Index == 4 || Legion.Instance.cBestCrew.u1Index == 5)
            tempStringBuilder.Append("Sprites/Common/flag_01.num_s_").Append(Legion.Instance.cBestCrew.u1Index.ToString());
        else
            tempStringBuilder.Append("Sprites/Common/flag_01.num_g_").Append(Legion.Instance.cBestCrew.u1Index.ToString());

        _crewEmblemNum.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
        _crewEmblemNum.SetNativeSize();
    }

    public void ResetCharacterTransform()
    {
        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            if(Legion.Instance.cBestCrew.acLocation[i] != null)
            {
                _bgCharacterPos.transform.GetChild(i).rotation = Quaternion.Euler(Legion.Instance.v3MainCharRotation[i]);
			}
            else
            {
                Legion.Instance.v3MainCharRotation[i] = Vector3.zero;
            }
        }

		SetMenuHideButtonEnable(true);
		// 2016. 12. 16 jy
		// 캐릭터 경험치바의 위치 조정은 캐릭터 3D 오브젝트가 
		// 로비 위치로 이동후 셋팅되어야 함
		//StartCoroutine(SetCharacterExpBarPos());
    }
    //대표 크루 캐릭터 배치
    public void InitCharacter()
    {
        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            if(Legion.Instance.cBestCrew.acLocation[i] != null)
            {
				Hero hero = (Hero)Legion.Instance.cBestCrew.acLocation[i];
				hero.InitModelObject();
				hero.cObject.transform.SetParent(_bgCharacterPos.transform.GetChild(i));
				hero.cObject.transform.localScale = Vector3.one;
				hero.cObject.transform.localPosition = Vector3.zero;
				hero.cObject.transform.localRotation = Quaternion.Euler(_characterRotation);
				hero.cObject.GetComponent<HeroObject>().SetLayer(LayerMask.NameToLayer("BGMainMap"));
				hero.cObject.GetComponent<HeroObject>().SetAnimations_UI();

                SetCharacterLevelExp(((Hero)Legion.Instance.cBestCrew.acLocation[i]), i);
                _characterPos.transform.GetChild(i).gameObject.SetActive(true);
                _bgCharacterPos.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                _characterPos.transform.GetChild(i).gameObject.SetActive(false);
                _bgCharacterPos.transform.GetChild(i).gameObject.SetActive(false);
                Legion.Instance.v3MainCharRotation[i] = Vector3.zero;
            }
        }
    }
    //캐릭터 경험치 및 레벨
    bool bTemp = false;
    public void SetCharacterLevelExp(Hero inHero, int heroPos)
    {
        int temp = EquipmentInfoMgr.Instance.GetInfo(inHero.acEquips[6].cItemInfo.u2ID).u1Element;
        _characterPos.transform.GetChild(heroPos).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Sprite_Element[temp-2];

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(inHero.cLevel.u2Level.ToString());
        _characterPos.transform.GetChild(heroPos).GetChild(1).GetChild(0).GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();

        float charExpPercent = (float)(((float)inHero.cLevel.u8Exp)/((float)inHero.cLevel.u8NextExp));
        _characterPos.transform.GetChild(heroPos).GetChild(1).GetChild(1).GetChild(1).GetComponent<Image>().fillAmount = charExpPercent;

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(ConvertExpValue(inHero.cLevel.u8Exp)).Append(" / ").Append(ConvertExpValue(inHero.cLevel.u8NextExp));
        _characterPos.transform.GetChild(heroPos).GetChild(1).GetChild(1).GetChild(3).GetComponent<Text>().text = tempStringBuilder.ToString();
        
        for(int i=0; i<inHero.acEquips.Length; i++)
        {
            if(inHero.acEquips[i].GetComponent<StatusComponent>().CheckHaveEquipStatPoint(inHero.acEquips[i].cLevel.u2Level))
            {
                bTemp = true;
                break;
            }
        }
        //if(equipNew)
        //    bTemp = equipNew;
        StartCoroutine(DelayNewCheck(inHero, heroPos));
    }

    IEnumerator DelayNewCheck(Hero inHero, int heroPos)
    {
        yield return new WaitForEndOfFrame();
        if(inHero.GetComponent<StatusComponent>().CheckHaveStatPoint(inHero.cLevel.u2Level))
            _characterPos.transform.GetChild(heroPos).GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(true);
        else if(inHero.GetComponent<SkillComponent>().SkillPoint > 0)
            _characterPos.transform.GetChild(heroPos).GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(true);
        else if(bTemp)
            _characterPos.transform.GetChild(heroPos).GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(true);
        else
            _characterPos.transform.GetChild(heroPos).GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(false);
    }
    //유저정보 초기화(VIP레벨/아이디)
    public void InitUserInfo()
    {
		/*
        //VIP레벨
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        //tempStringBuilder.Append("<color=#30EEEC>").Append("VIP ").Append(Legion.Instance.u1VIP.ToString()).Append("</color>");
        tempStringBuilder.Append("VIP ").Append(Legion.Instance.u1VIPLevel.ToString());
        m_VipLevelText.text = tempStringBuilder.ToString();
		*/
        //유저 아이디
		_userInfo.transform.FindChild("Text_NAME").GetComponent<Text>().text = Legion.Instance.sName;

		SetCrewPower();
    }
    //캐릭터 투구 체크
    public void OnCheckHelmet()
    {
        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            if(Legion.Instance.cBestCrew.acLocation[i] != null)
            {
                if(ObscuredPrefs.GetBool("HelmetToggle", true))
                    ((Hero)Legion.Instance.cBestCrew.acLocation[i]).cObject.GetComponent<HeroObject>().OnOffHelmet(ObscuredPrefs.GetBool("HelmetToggle", true));
                else
                    ((Hero)Legion.Instance.cBestCrew.acLocation[i]).cObject.GetComponent<HeroObject>().OnOffHelmet(ObscuredPrefs.GetBool("HelmetToggle", true));
            }
        }
    }

	// 카메라 시점을 변경하는 함수
	public void OnClickChangeCameraView()
	{
		if( _characterViewCam != null)
		{
			UI_CameraMove camMove = _characterViewCam.GetComponent<UI_CameraMove>();
			camMove.CamAnimation();	
			SetCharacterClickWindow(camMove.CameraViewIndex);
		}
	}
		
    public void ShowMenu()
    {
		SetMenuHideButtonEnable(true);
        this.transform.GetChild(0).GetComponent<Animator>().Play("Ani_UI_Main_ShowMenu");
        LeanTween.alpha(Btn_ShowMenu.GetComponent<RectTransform>(), 0f, 0.3f);
        Btn_ShowMenu.GetComponent<Button>().interactable = false;
    }

	bool isHide = false;
    public bool IsMenuHide
    {
        set { isHide = value; }
        get { return isHide; }
    }
    public void HideMenu()
    {
		// 카메라 에니메이션이 작동 중이라면 하지 않는다
		if(_characterViewCam.GetComponent<Animator>().enabled == true)
			return;

		if(isHide == true)
        {
            this.transform.GetChild(0).GetComponent<Animator>().Play("Ani_UI_Main_ShowMenu");
            LeanTween.alpha(Btn_ShowMenu.GetComponent<RectTransform>(), 0f, 0.3f);

            Btn_ShowMenu.GetComponent<Button>().interactable = false;
			if(_nCamViewIndex != _characterViewCam.GetComponent<UI_CameraMove>().CameraViewIndex)
                StartCoroutine(SetCharacterExpBarPos());
        }
        else
		{
			_nCamViewIndex = _characterViewCam.GetComponent<UI_CameraMove>().CameraViewIndex;
            this.transform.GetChild(0).GetComponent<Animator>().Play("Ani_UI_Main_HideMenu");
            LeanTween.alpha(Btn_ShowMenu.GetComponent<RectTransform>(), 1f, 0.3f).setDelay(0.1f);
            Btn_ShowMenu.GetComponent<Button>().interactable = true;
        }
		isHide = !isHide;
    }

	public void PlayCharacterAnim(bool bPlay)
	{
		StopCoroutine("MainCrewAni");
		if(bPlay && prevScreen == 0) 
			StartCoroutine("MainCrewAni");
	}

    IEnumerator AddOnClickMenuHide()
    {
        yield return new WaitForSeconds(0.2f);
        this.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(ShowMenu);
    }

    IEnumerator AddOnClickMenuShow()
    {
        yield return new WaitForSeconds(0.2f);
        this.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveListener(ShowMenu);
    }

    public void SaveCharacterRotation()
    {
        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            if(Legion.Instance.cBestCrew.acLocation[i] != null)
            {
                Legion.Instance.v3MainCharRotation[i] = _bgCharacterPos.transform.GetChild(i).eulerAngles;
            }
            else
            {
                Legion.Instance.v3MainCharRotation[i] = Vector3.zero;
            }
        }
    }

	void SetQuestPopup(){
		if (Legion.Instance.cQuest.u2IngQuest == 0) 
		{
			OnClickQuest();
			return;
		}

		UserQuest info = Legion.Instance.cQuest.CurrentUserQuest();

		if (info.isClear ()) 
		{
			//FadeEffectMgr.Instance.QuickChangeScene(MENU.HERO_GUILD, (int)POPUP_HERO_GUILD.QUEST, null);
			OnClickQuest();
		}
		else 
		{
			GameObject temp = Instantiate (AssetMgr.Instance.AssetLoad ("Prefabs/UI/Common/Pref_UI_QuestAccept.prefab", typeof(GameObject))) as GameObject;
			QuestAcceptPopup tempScript = temp.GetComponent<QuestAcceptPopup> ();
			QuestInfo quest = info.GetInfo ();
			tempScript.Show (TextManager.Instance.GetText(quest.sName), TextManager.Instance.GetText(quest.sDescription), TextManager.Instance.GetText(quest.sSummary), null, null);
			tempScript.SetPopup (quest);

			if (info.isIng ()) {
				tempScript.SetGauge ((float)Legion.Instance.cQuest.u4QuestCount, (float)info.u4MaxCount);
				tempScript.btnOK.gameObject.SetActive (false);
				tempScript.btnGiveUp.onClick.AddListener (() => { GiveUpQuest (tempScript); });
			}

			RectTransform rcTr = temp.GetComponent<RectTransform>();
			rcTr.SetParent (transform);
			rcTr.localScale = Vector3.one;
			rcTr.anchoredPosition3D = new Vector3(0,0,-1000f);
			rcTr.sizeDelta = Vector2.zero;
		}
	}

	public bool CheckClearAchievement()
	{
		_nShortcutAchieveType = 0;
		tempStringBuilder.Remove(0, tempStringBuilder.Length);
		// 업적 -> 일일 -> 주간순으로 체크
		if( Legion.Instance.cQuest.CheckAlarmAchievement(1) == true)
		{
			_nShortcutAchieveType = (int)POPUP_HERO_GUILD.ACHIEVEMENT;
			_ShortcutAchieveBtn.transform.GetChild(0).GetComponent<Text>().text = TextManager.Instance.GetText("main_mark_achievement_clear_1");
		}
		else if( Legion.Instance.cQuest.CheckAlarmAchievement(2) == true)
		{
			_nShortcutAchieveType = (int)POPUP_HERO_GUILD.DAILY_ACHIEVEMENT;
			_ShortcutAchieveBtn.transform.GetChild(0).GetComponent<Text>().text = TextManager.Instance.GetText("main_mark_achievement_clear_2");
		}
		else if( Legion.Instance.cQuest.CheckAlarmAchievement(3) == true )
		{
			_nShortcutAchieveType = (int)POPUP_HERO_GUILD.WEEKLY_ACHIEVEMENT;
			_ShortcutAchieveBtn.transform.GetChild(0).GetComponent<Text>().text = TextManager.Instance.GetText("main_mark_achievement_clear_3");
		}

		// 클리어된 업적 타입을 체크하여 바로가기 버튼을 비/활성화를 한다
		if( _nShortcutAchieveType == 0 )
		{
			_ShortcutAchieveBtn.gameObject.SetActive(false);
			return false;
		}

		_ShortcutAchieveBtn.gameObject.SetActive(true);
		return true;
	}
		
	// 업적 바로가기 버튼
	public void OnClickShortcutAchievement()
	{
		if(_nShortcutAchieveType == 0)
			return;

		// 버튼이 활성화 되어있을때만 바로가기 한다 
		if( _ShortcutAchieveBtn.gameObject.activeSelf == true )
			FadeEffectMgr.Instance.QuickChangeScene(MENU.HERO_GUILD, _nShortcutAchieveType, null);
	}

	void GiveUpQuest(QuestAcceptPopup tempScript){
		
		object[] param = new object[1]{tempScript};
		
		PopupManager.Instance.ShowYesNoPopup (TextManager.Instance.GetText("popup_title_quest_giveup"), TextManager.Instance.GetText("popup_desc_quest_giveup"), TextManager.Instance.GetText("popup_btn_quest_giveup"), AcceptGiveUp, param);
	}
	
	void AcceptGiveUp(object[] param){
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequestQuestCancel(RequestQuestGiveUp);
		
		QuestAcceptPopup temp = (QuestAcceptPopup)param[0];
		
		temp.CloseMe();
	}
	
	void RequestQuestGiveUp(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		DebugMgr.Log(err);
		
		if (err != Server.ERROR_ID.NONE) {
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.QUEST_CANCEL, err), Server.ServerMgr.Instance.CallClear);
		} else {
			Legion.Instance.cQuest.CancelQuest ();
			Btn_QuickQuest.SetButton();
            CheckAlram_Quest();
		}
	}

    public void onClickVIPButton()
    {
        //UI_VIPInfoPopUp obj = ((GameObject)Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/VIPInfoPopUp.prefab", typeof(GameObject)))).GetComponent<UI_VIPInfoPopUp>();
        if(_OdinMissionPopup == null)
        { 
            _OdinMissionPopup = ((GameObject)Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/Pref_UI_Main_OdinMissionPopup.prefab", typeof(GameObject)))).GetComponent<OdinMainPopup>();

            RectTransform rectTr = _OdinMissionPopup.GetComponent<RectTransform>();
		    rectTr.SetParent(transform);
		    rectTr.anchoredPosition3D = Vector3.zero;
		    rectTr.localScale = Vector3.one;
		    rectTr.sizeDelta = Vector2.zero;

            //obj.RefleshVIPreward();
        }
        //_OdinMissionPopup.SetOdinPopup();
        _OdinMissionPopup.gameObject.SetActive(true);
    }

    public void CheckAlram_VIP()
    {
        _objAlramIcon[(int)LobbyAlram.ODIN].SetActive(false);
        if (Legion.Instance.u1VIPLevel < 0)
            return;

        if(Legion.Instance.u1VIPLevel < 11)
        {
            m_VipLevelText.text = TextManager.Instance.GetText(string.Format("odin_name_{0}", Legion.Instance.u1VIPLevel));
        }
        else
        {
            m_VipLevelText.text = TextManager.Instance.GetText(string.Format("odin_short_name_{0}", Legion.Instance.u1VIPLevel));
        }
        
        imgOdinGradeIcon.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Common/otm_ambulram.Odin_Grade_icon_{0}", Legion.Instance.u1VIPLevel));
        if (Legion.Instance.u1VIPLevel < (LegionInfoMgr.Instance.dicVipData.Count -1))
        {
            VipInfo nextVipInfo = LegionInfoMgr.Instance.GetVipInfo((Byte)(Legion.Instance.u1VIPLevel + 1));
            imgOdinProgressGruge.fillAmount = ((float)Legion.Instance.u4VIPPoint / (float)nextVipInfo.cUnlockGoods.u4Count);
        }
        else
        {
            imgOdinProgressGruge.fillAmount = 1;
        }

        int lv = Legion.Instance.RecursiveCheckOdinLevel(Legion.Instance.u1VIPLevel, Legion.Instance.u4VIPPoint);
        if(lv > Legion.Instance.u1VIPLevel)
        {
            _objAlramIcon[(int)LobbyAlram.ODIN].SetActive(true);
        }
        else
        {
            if(Legion.Instance.cQuest.IsClearOdinMission())
            {
                _objAlramIcon[(int)LobbyAlram.ODIN].SetActive(true);
            }
        }
    }

	IEnumerator MainCrewAni()
	{
		float nextTime = 0f;
		Byte heroIndex = 0;
        Crew mainCrew;
		StringBuilder aniName = new StringBuilder();
		while(true)
		{
			nextTime = UnityEngine.Random.Range (10f, 15f);
			yield return new WaitForSeconds(nextTime);

			if(_mainMenu.activeSelf)
			{
				mainCrew = Legion.Instance.cBestCrew;

				for(;heroIndex < mainCrew.acLocation.Length; ++heroIndex)
				{
					if( mainCrew.acLocation[heroIndex] != null )
						break;
				}

				if(heroIndex >= mainCrew.acLocation.Length)
				{
					for(Byte i = 0; i < mainCrew.acLocation.Length; ++i )
					{
						if( mainCrew.acLocation [i] != null )
						{
							heroIndex = i;
							break;
						}
					}
				}

                // [예외처리] Index Out Of Range Exception
                // 인덱스가 변환되지 않거나 캐릭터 갯수보다 크다면
                if (heroIndex >= mainCrew.acLocation.Length)
                {
                    heroIndex = 0;
                }

                if ( mainCrew.acLocation [heroIndex] != null )
				{
					if(((Hero)mainCrew.acLocation [heroIndex]).cObject != null)
					{
						// 2016. 12. 19jy 
						// 코루틴으로 작동하므로 안전하게 StringBuiler를 별도로 사용 하도록 한다
						if (UnityEngine.Random.Range (0, 100) < 70) {
							aniName.Remove (0, aniName.Length);
							aniName.Append ("UI_Class_Main_").Append (UnityEngine.Random.Range (1, 4));
							((Hero)mainCrew.acLocation [heroIndex]).cObject.GetComponent<HeroObject> ().PlayAnim (aniName.ToString ());
						}
					}
				}
				++heroIndex;
			}
		}
	}

	private void InitClickWindow()
	{
		// 2016. 08. 31 jy 
		// 레이캐스트로 캐릭터 회전 및 캐릭터 정보창 입장을 컨트롤하기에는
		// 팝업 오픈 여부를 체크하기 상당히 까다로움
		// 기존 방식으로 되돌리고 카메라가 시점을 변경하였을때 캐릭터 윈도우창을 변경 하도록 수정
		_characterWindowPos[0, 0] = new Vector3(17, -50, 0);
		_characterWindowPos[0, 1] = new Vector3(-330, -50, 0);
		_characterWindowPos[0, 2] = new Vector3(384, -50, 0);

		_characterWindowPos[1, 0] = new Vector3(35, -50, 0);
		_characterWindowPos[1, 1] = new Vector3(-323, -80, 0);
		_characterWindowPos[1, 2] = new Vector3(384, -50, 0);

		_characterWindowPos[2, 0] = new Vector3(35, -50, 0);
		_characterWindowPos[2, 1] = new Vector3(-280, -50, 0);
		_characterWindowPos[2, 2] = new Vector3(370, -50, 0);

		_characterWindowPos[3, 0] = new Vector3(45, -60, 0);
		_characterWindowPos[3, 1] = new Vector3(-260, -80, 0);
		_characterWindowPos[3, 2] = new Vector3(350, -70, 0);

		SetCharacterClickWindow(ObscuredPrefs.GetInt("LobbyCameraIndex"));
	}

    public string ConvertExpValue(UInt64 u8Exp)
    {
        if(u8Exp < 1000)
            return u8Exp.ToString();

        int tempExp = (int)(Math.Log(u8Exp)/Math.Log(1000));
		return String.Format("{0:F2}{1}", u8Exp/Math.Pow(1000, tempExp), "KMB".ToCharArray()[tempExp-1]);
    }

	private void SetCharacterClickWindow(int camViewIndex)
	{
		for(int i = 0; i < Crew.MAX_CHAR_IN_CREW; ++i)
		{
			_characterPos.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition3D = _characterWindowPos[camViewIndex, i];
		}
	}

	public void SetCrewPower()
	{
		tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(TextManager.Instance.GetText("mark_power")).Append(" ").Append(Legion.Instance.cBestCrew.u4Power.ToString());

		_crewPower.text = tempStringBuilder.ToString();
	}

	public void SetCharacterExpBarPos(string type)
	{
		if(type.CompareTo("CREW") == 0)
		{
			mainPanel.GetComponent<Animator>().enabled = false;
			for(int i = 0; i < Crew.MAX_CHAR_IN_CREW; ++i)
			{
				RectTransform rectTr = _characterPos.transform.GetChild(i).GetChild(1).GetComponent<RectTransform>();
				Vector3 pos = rectTr.localPosition;
				pos.y = -(_characterPos.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.y * 0.5f);
				rectTr.localPosition = pos;
				rectTr.localScale = Vector3.one;

				rectTr.GetChild(0).GetChild(3).gameObject.SetActive(true);
				rectTr.GetChild(1).GetChild(2).gameObject.SetActive(true);
				rectTr.GetChild(1).GetChild(3).gameObject.SetActive(true);
			}
		}
		else
		{
			for(int i = 0; i < Crew.MAX_CHAR_IN_CREW; ++i)
			{
				Transform tr = _characterPos.transform.GetChild(i).GetChild(1);
				tr.localScale = new Vector3(0.7f, 0.7f, 1);
				tr.GetChild(0).GetChild(3).gameObject.SetActive(false);
				tr.GetChild(1).GetChild(2).gameObject.SetActive(false);
				tr.GetChild(1).GetChild(3).gameObject.SetActive(false);
			}
			mainPanel.GetComponent<Animator>().enabled = true;
            StartCoroutine(SetCharacterExpBarPos());
        }
	}

	public void CloseCharacterInfo(bool bBeforeMenuIsCrew = false)
	{
		if (_CrewMenu != null)
		_CrewMenu.GetComponent<UI_CrewMenu>().ShowCharacterSlot();

		_goods.SetActive(true);

		//if(!bBeforeMenuIsCrew) StartCoroutine(SetCharacterExpBarPos());
	}
		
	// 2016. 12. 19 jy
	// 로비 캐릭터 경험치바 및 레벨 표시 UI 위치 셋팅
	public IEnumerator SetCharacterExpBarPos()
	{
		// 2017. 01. 10 jy
		// 전투 패배시 마을로 나오면 캐릭터 애니메이션이 초기화 되지않아 
		// 경험치 바가 발부분에 오는 현상이 있어 한프레임 늦게 셋팅 한다
		yield return null;

		for(int i = 0; i < Crew.MAX_CHAR_IN_CREW; ++i)
		{
			if( Legion.Instance.cBestCrew.acLocation[i] == null) 
				continue;

            if (_bgCharacterPos.transform.GetChild(i).childCount <= 0)
                continue;

			Transform headBone = _bgCharacterPos.transform.GetChild(i).GetChild(0).GetComponent<HeroObject>().GetBones("Bip01-Head");
			RectTransform rectTr = _characterPos.transform.GetChild(i).GetChild(1).GetComponent<RectTransform>();

			Vector3 tmpPos = new Vector3(headBone.position.x, headBone.position.y + 0.25f, headBone.position.z);
			Vector3 textPos = Camera.main.ViewportToWorldPoint(_characterViewCam.WorldToViewportPoint(tmpPos));

			// 2016. 12. 29 jy 
			// 크루 메뉴로 진입시 레벨바가 위치가 삐뚤어지며 
			// 화면 밖으로 사라져서 Y 값만 변경하도록 수정
			textPos.x = rectTr.position.x;
			textPos.z = rectTr.position.z;
			rectTr.position = textPos;
		}
	}
	public void SetMenuHideButtonEnable(bool isEnable)
	{
		mainPanel.GetComponent<Button>().enabled = isEnable;
		PlayCharacterAnim (isEnable);
	}

	// 2016. 12. 09 jy
	// 디비전 마크를 셋팅한다
	private void SetDivisionMark()
	{
		//기능막음 2017.08.08 jc
		Btn_DivisionMark.gameObject.SetActive(false);
		return;

		// 리그에 참전한 적이 없다면 버튼을 비활성화 한다
		if(Legion.Instance.GetDivision == 0)
		{
			Btn_DivisionMark.gameObject.SetActive(false);
			return;
		}
		// 디비전 이미지를 자식넣고 Mask 셋팅하여 
		// 후광이 Mask 적용 안되도록 셋팅함
		Byte division = Legion.Instance.GetDivision;
		// 마크 셋팅
		tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append("Sprites/Common/flag_01.btn_Division_").Append(division.ToString());
		Btn_DivisionMark.image.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
		Btn_DivisionMark.image.SetNativeSize();
		// 위치를 잡기 위하여 부모의 사이즈를 이미지 사이즈로 맞춘다
		Btn_DivisionMark.GetComponent<RectTransform>().sizeDelta = Btn_DivisionMark.image.GetComponent<RectTransform>().sizeDelta;

		// 디비전 마스크는 Start() 함수에서만 작동하므로 자식의 컴포넌트를 참조함
		// 추후 Start() 함수외 수시로 변경 될시 맴버변수로 셋팅해야 함
		Image gloriole =  Btn_DivisionMark.transform.GetChild(0).GetComponent<Image>();
		// 후광 셋팅
		tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append("Sprites/Common/flag_01.Btn_Division_Gloriole_").Append(division.ToString());
		gloriole.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
		gloriole.SetNativeSize();
	}

	public void OnClickDivisionMark()
	{
        if (LegionInfoMgr.Instance.IsContentOpen(LegionInfoMgr.LEAGUE_CONTENT_ID) == false)
        {
            LegionInfoMgr.Instance.ShowContentNoticePopup(LegionInfoMgr.LEAGUE_CONTENT_ID);
            return;
        }
            
        
		if (AssetMgr.Instance.CheckDivisionDownload(6,0)) {
			return;
		}

		AssetMgr.Instance.SceneLoad("ALeagueScene", false);
	}

    public void OpenStrongRecommend()
    {
        GameObject strongPopup = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_StrongRecommendPopup.prefab", typeof(GameObject)) as GameObject);
        RectTransform trTr = strongPopup.GetComponent<RectTransform>();
        trTr.SetParent(this.transform);
        trTr.anchoredPosition3D = Vector3.zero;
        trTr.localScale = Vector3.one;
        trTr.sizeDelta = Vector2.zero;

        PopupManager.Instance.AddPopup(strongPopup, strongPopup.GetComponent<StrongRecommendPopup>().OnClickClose);
    }

    public void CreateMarbleEventBtn()
    {
		if(EventInfoMgr.Instance.GetOpenMarbleGameInfo() != null)
        {
            GameObject marbleBtnObj = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/Btn_EventMarble.prefab", typeof(GameObject)) as GameObject);
            RectTransform rectTr = marbleBtnObj.GetComponent<RectTransform>();
            float btnPosX = GetLeftBtnLastX() + LeftBtnGapX;
            rectTr.SetParent(LeftBtnParent);
            rectTr.anchoredPosition3D = new Vector3(btnPosX, LeftBtnPosY, 0);
            rectTr.localScale = Vector3.one;
            rectTr.sizeDelta = Vector2.zero;
			marbleBtnObj.transform.FindChild("Image").GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite ("Sprites/Event/event_01.event_icon_Marble");
			marbleBtnObj.transform.FindChild("Icon").GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite ("Sprites/Event/event_01.EventDungeonBtn");
            marbleBtnObj.GetComponent<Button>().onClick.AddListener(OnClickMarble);
            marbleBtnObj.SetActive(true);
        }
    }

    public void OnClickMarble()
    {
        // 오픈된 마블의 이벤트가 ID가 0이라면 리턴
		if (EventInfoMgr.Instance.GetOpenMarbleGameInfo() == null)
            return;

        GameObject marbleWindow = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_EventMarbleGame.prefab", typeof(GameObject)) as GameObject);
        RectTransform rectTr = marbleWindow.GetComponent<RectTransform>();
        rectTr.SetParent(this.transform);
        rectTr.localPosition = Vector3.zero;
        rectTr.localScale = Vector3.one;
        rectTr.sizeDelta = Vector2.zero;

        marbleWindow.SetActive(true);

		PopupManager.Instance.AddPopup (marbleWindow, marbleWindow.GetComponent<EventMarbleMain> ().OnClickMarbleMainClose);
    }

	// 2016. 12. 27 jy
	// 퀘스트를 로비로 따로 빼내면서 퀘스트 수락후 바로 이동을 로비씬으로 옴김
	public void ShowSelectedQuestWithDelay(float delay)
	{
		Invoke ("ShowSelectedQuest", delay);
	}

	void ShowSelectedQuest ()
	{
        if(_cQuestPanel != null)
        {
            _cQuestPanel.QuestAreaShortcut();
        }
	}

	public void RefreshQuest()
	{
		Btn_QuickQuest.SetButton();
		RefreshAlram();
	}

    public void CheckLoginPopupStep()
    {
        if (Legion.Instance.cTutorial.bIng == true)
            return;

        switch (Legion.Instance.GetLoginPopupStep)
        {
            case Legion.LoginPopupStep.EVENT_NOTICE:
                {
                    GameObject eventPopupWindow = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_EventNoticePopup.prefab", typeof(GameObject))) as GameObject;
                    RectTransform rtTr = eventPopupWindow.GetComponent<RectTransform>();
                    rtTr.SetParent(mainPanel.transform);
                    rtTr.localPosition = Vector3.zero;
                    rtTr.localScale = Vector3.one;
                    rtTr.sizeDelta = Vector2.zero;
                    eventPopupWindow.SetActive(true);
                    eventPopupWindow.GetComponent<EventNoticePopup>().OpenPopup();
                    //Legion.Instance.bADView = true;
                }
                break;
            case Legion.LoginPopupStep.EVENT_MONTHLY_PACK:
                {
                    GameObject eventPopupWindow = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_MonthlyPackPopup.prefab", typeof(GameObject))) as GameObject;
                    RectTransform rtTr = eventPopupWindow.GetComponent<RectTransform>();
                    rtTr.SetParent(mainPanel.transform);
                    rtTr.localPosition = Vector3.zero;
                    rtTr.localScale = Vector3.one;
                    rtTr.sizeDelta = Vector2.zero;
                    eventPopupWindow.SetActive(true);

                    eventPopupWindow.GetComponent<BaseEventPopup>().SetPopup(false);
                }
                break;
            case Legion.LoginPopupStep.EVENT_30DAY_PACK:
                {
                    bool isBuyPackage = false;
                    List<EventPackageInfo> monthPackList = EventInfoMgr.Instance.GetEventListByEventType(5);
                    if (monthPackList.Count > 0)
                    {
                        int buyIndex = -1;
                        for (int i = 0; i < monthPackList.Count; ++i)
                        {
                            // 0 오픈하지 않는 이벤트
                            // 1 구매가능한 이벤트
                            // 2 구매 완료된 이벤트 
                            int check = EventInfoMgr.Instance.CheckBuyPossible(monthPackList[i].u2ID);
                            // 구매가 완료 품목이 있다면 확인을 바로 종료 한다
                            if (check == 2)
                            {
                                isBuyPackage = true;
                                break;
                            }
                            // 구매할 조건이 안되는 상품이라면 일단 체크 하고 다음 물품을 확인한다
                            else if(check == 0)
                            {
                                isBuyPackage = true;
                            }

                            // Tutorial Package은 그룹 인덱스가 1
                            if (monthPackList[i].u1EventGroupIdx == 1)
                            {
                                buyIndex = i;    
                            }
                        }
                    }

                    // 구매 여부 확인
                    if (!isBuyPackage)
                    {
                        GameObject eventPopupWindow = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Tutorial/Pref_TutorialEventPackPopup.prefab", typeof(GameObject))) as GameObject;
                        RectTransform rtTr = eventPopupWindow.GetComponent<RectTransform>();
                        rtTr.SetParent(mainPanel.transform);
                        rtTr.localPosition = Vector3.zero;
                        rtTr.localScale = Vector3.one;
                        rtTr.sizeDelta = Vector2.zero;
                        eventPopupWindow.SetActive(true);

                        eventPopupWindow.GetComponent<BaseEventPopup>().SetPopup(false);
                    }
                    else
                    {
                        Legion.Instance.SubLoginPopupStep(Legion.LoginPopupStep.EVENT_30DAY_PACK);
                    }
                }
                break;
                
            case Legion.LoginPopupStep.LOGIN_REWARED:
                Legion.Instance.cQuest.UpdateLoginAchievement();
                break;
			case Legion.LoginPopupStep.OX_EVENT:
				if (Legion.Instance.sName != "") {
					_eventPanel.OnClickMenu (10);
				} else {
					Legion.Instance.SubLoginPopupStep(Legion.LoginPopupStep.OX_EVENT);
				}
                break;
            case Legion.LoginPopupStep.ODIN_PAGE:
                {
                    onClickVIPButton();
                }
                break;
            case Legion.LoginPopupStep.NAVER_CAFE:
                Legion.Instance.ShowCafeOnce();
                break;
        }
    }

    public void OpenEventWindown(EVENT_TYPE eventType)
    {
        switch(eventType)
        {
            case EVENT_TYPE.DICE:
                OnClickMarble();
                break;
	        case EVENT_TYPE.FIRSTPAYMENT:           // 첫결재 이벤트		
            case EVENT_TYPE.ADDITIONALREWARD:		// 결재 이벤트(누적 재화 이벤트)
            case EVENT_TYPE.ADDITIONALREWARD_ITEM:
                StartCoroutine(ShowShop(true, 1));
                break;
        }
    }

    public void OpenCreateEquipWindow()
    {
        StartCoroutine(OnClickCharacterInfo(0, Legion.Instance.cBestCrew.u1Index, null, 0, false));

        _characterInfo.Open(POPUP_CHARACTER_INFO.EQUIP_CREATE);
    }
    /*
    // 2017. 02. 21 jy
    // 언제 대장간 기능을 다시 만들어달라고 할지 모르니
    // 대장간 씬 없어짐에 따라 대장간 관련 함수들을 모아놓음
    //제련소 버튼 선택
    
    void CheckAlram_Forge()
    {
        bool check = false;
        for (int i = 0; i < Legion.Instance.acEquipDesignNew.Count; i++)
        {
            if (Legion.Instance.acEquipDesignNew.Get(i))
            {
                check = true;
                break;
            }
        }

        for (int i = 0; i < Legion.Instance.acLookDesignNew.Count; i++)
        {
            if (Legion.Instance.acLookDesignNew.Get(i))
            {
                check = true;
                break;
            }
        }

        Legion.Instance.cInventory.EquipSort();
        //for(int i=0; i<Legion.Instance.cInventory.lstSortedEquipment.Count; i++)
        //{
        //	if(Legion.Instance.cInventory.lstSortedEquipment[i].isNew)
        //	{
        //		check = true;
        //		break;
        //	}
        //}

        bool upgradeCheck = false;
        if (Legion.Instance.u1ForgeLevel < ForgeInfo.FORGE_LEVEL_MAX - 5)
        {
            ForgeInfo _cNextForgeInfo = ForgeInfoMgr.Instance.GetList()[Legion.Instance.u1ForgeLevel];
            for (int i = 0; i < _cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials.Length; i++)
            {
                UInt16 ownCount = 0;
                Item item = null;
                UInt16 invenSlotNum = 0;
                if (Legion.Instance.cInventory.dicItemKey.TryGetValue(_cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials[i].u2ID, out invenSlotNum))
                {
                    if (Legion.Instance.cInventory.dicInventory.TryGetValue(invenSlotNum, out item))
                        ownCount = ((MaterialItem)item).u2Count;
                }
                if (ownCount < _cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials[i].u4Count)
                {
                    upgradeCheck = false;
                    break;
                }
                else
                    upgradeCheck = true;
            }
        }

        _objAlramIcon[1].SetActive(check | upgradeCheck);
        _objQuestIcon[0].SetActive(Legion.Instance.cQuest.CheckQuestAlarm(MENU.FORGE, 0)); //알람 아이콘(0:대장간, 1:길드, 2:상점, 3:캠페인, 4:크루)
    }
    public void OnClickForge()
    {
		if (_objAlramIcon [1].activeSelf) {
			PopupManager.Instance.SetNoticePopup (MENU.FORGE);
		}
		StartCoroutine(GoForge());
    }

	IEnumerator GoForge()
	{
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        //현재 캐릭터 회전값 저장
        SaveCharacterRotation();
		AssetMgr.Instance.SceneLoad("ForgeScene", false);
	}
    */
}