using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using IgaworksUnityAOS;
using UnityEngine.Advertisements;
using Facebook.Unity;
using Facebook.Unity.Mobile;

public class Legion : Singleton<Legion>
{
	public const float BaseScreenSizeX = 1280f;
	public const float BaseScreenSizeY = 720f;

	public const int MAX_CREW_OF_LEGION = 5;
	//public Byte tmpHeroNum=0;
    //구글 애널리틱스 프리펩
    public GoogleAnalyticsV3 googleAnalytics;
    public UInt16 u2LastLoginServer; //마지막 접속 서버
    public UInt16 u2RecommendServerID = 0;
    public Byte u1ServerCount; //서버 갯수
    public struct ServerGroup
    {
        public UInt16 u2ServerID;           //서버 ID
        public string strServerNameCode;    //서버 이름 코드, 치환 후 출력
        public Byte u1State;                //서버 상태 1 == 정상
        public UInt16 u2Port;               //포트 번호
        public Byte u1CharCreated;          //생성된 캐릭터 유무 1==캐릭터 있음, 0==없음
        public string strLegionName;        //군단 이름(군단 이름이 없을 경우 "견습가디언");
        public Byte u1Congestion;           //1: 원활, 2: 혼잡, 3: 포화
        public Byte u1New;                  //1:신규 서버, 0: 신규 아님
    }
    public List <ServerGroup> lstServerGroup;
	public bool bLoaded;
	public bool bCrewToChar;
    public bool bStageToCrew;
    public bool bCharInfoToCrew;
    public bool bLeagueToCharInfo;
	public bool bAuto;
    public Hero cSelectHeroElement;
	
    public enum Days
    {
        NONE = 0,
        MON = 1,
        TUE = 2,
        WED = 3,
        THU = 4,
        FRI = 5,
        Sat = 6,
        Sun = 7,
    }

	public string sName;
	public UInt16 u2UserPicture;
	private ObscuredUInt u4Gold;
    public UInt32 Gold
    {
        set
        {
            u4Gold.RandomizeCryptoKey();
            u4Gold = value;
			RefreshGoodInfo ();
        }
        get
        {
            u4Gold.RandomizeCryptoKey();
            return u4Gold;
        }
    }
	public bool bCheckGold;
	private ObscuredUInt u4ServerGold;
	public UInt32 ServerGold
	{
		set
		{
			u4ServerGold.RandomizeCryptoKey();
			u4ServerGold = value;
		}
		get
		{
			u4ServerGold.RandomizeCryptoKey();
			return u4ServerGold;
		}
	}
	private ObscuredUInt u4Cash;
    public UInt32 Cash
    {
        set
        {
            u4Cash.RandomizeCryptoKey();
            u4Cash = value;
			RefreshGoodInfo ();
        }
        get
        {
            u4Cash.RandomizeCryptoKey();
            return u4Cash;
        }
    }
	public bool bCheckCash;
	private ObscuredUInt u4ServerCash;
    public UInt32 ServerCash
    {
        set
        {
            u4ServerCash.RandomizeCryptoKey();
            u4ServerCash = value;
        }
        get
        {
            u4ServerCash.RandomizeCryptoKey();
            return u4ServerCash;
        }
    }
	private ObscuredUShort u2Energy;
    public UInt16 Energy
    {
        set
        {
            u2Energy.RandomizeCryptoKey();
            u2Energy = value;
			RefreshGoodInfo ();
        }
        get
        {
            u2Energy.RandomizeCryptoKey();
            return u2Energy;
        }
    }
	public UInt16 u2ClearTicket; //소탕권
	private ObscuredUShort u2LeagueKey;
    public UInt16 LeagueKey
    {
        set
        {
            u2LeagueKey.RandomizeCryptoKey();
            u2LeagueKey = value;
			RefreshGoodInfo ();
        }
        get
        {
            u2LeagueKey.RandomizeCryptoKey();
            return u2LeagueKey;
        }
    }
	public Byte u1LeagueKeyBuyCount;
	private ObscuredUShort u2FriendShipPoint;
    public UInt16 FriendShipPoint
    {
        set
        {
            u2FriendShipPoint.RandomizeCryptoKey();
            u2FriendShipPoint = value;
			RefreshGoodInfo ();
        }
        get
        {
            u2FriendShipPoint.RandomizeCryptoKey();
            return u2FriendShipPoint;
        }
    }

    public Byte u1VIPLevel;
    public UInt32 u4VIPPoint;
    private bool m_bVIPLevelUp;
    public bool VIP_LEVELUP
    {
        get{ return m_bVIPLevelUp; }
    }

	Byte _u1ForgeLevel;
	public Byte u1ForgeLevel
	{
		get{ return _u1ForgeLevel; }
		set{
			_u1ForgeLevel = value;
		}
	}

    private bool m_bAutoContinue = false;
    public bool AUTOCONTINUE
    {
        get{ return m_bAutoContinue; }
        set{ m_bAutoContinue = value; }
    }
	public UInt16 u2Level;
	public UInt16 u2EquipTopLevel;
	public List<Hero> acHeros;
	public Hero cLastUpdatedHero;
	public Crew[] acCrews;
	public Crew cBestCrew;
    public int tempCrewIndex = -1;

	public Crew cBestCrewBackUp;
	public Inventory cInventory;
	public Runeventory cRuneventory;
	public int cSelectedSlot;
	public Byte u1BlackMarketOpen; //1:close 2:open 3:VIPopen
	public DateTime BlackMarketLeftTime;

	public Byte[] runeList;
	private Byte selectedDifficult = 1;
    public Byte SelectedDifficult
    {
        get { return selectedDifficult; }
        set { selectedDifficult = value; }
    }

	public Byte bAdventoStage = 0;

    UInt16 u2SelectFieldID;
	public UInt16 u2SelectStageID = 6001;
	public GameStyle eGameStyle;

	public Byte[] charAvailable;

	public Byte CheckClassAvailable(UInt16 classID)
	{
		if(charAvailable == null)
			return ClassInfoMgr.Instance.GetInfo(classID).ClassLockType;
		
		/*
		if (charAvailable [classID - 1] == 1)
			return true;

		return false;
		*/
		return charAvailable[classID - 1];
	}

	public Byte[] charTrainingRoom;
	public Byte[] equipTrainingRoom;

	public Byte[] adRemainCount = new byte[3];
	public UInt16[] adLeftTime = new ushort[3];

	public BitArray acEquipDesign;
	public BitArray acEquipDesignNew;
    public BitArray acEquipDesignMake;
	public BitArray acLookDesign;
	public BitArray acLookDesignNew;
    public BitArray acMakeNewBinary;

	public Quest cQuest;

	public Tutorial cTutorial;

	public UserEventInfo cEvent;

    public Vector3[] v3MainCharRotation; 
    
    public DateTime nextEnergyChargeTime;
    public DateTime nextLeagueEnergyChargeTime;
    public bool osLogin = false;
	public byte checkLoginAchievement = 0;
    public byte pushSetting = 0;
	public bool termspush = false;
    public bool pushRegisted = false;
	public bool[] pushActive = new bool[MAX_PUSH_EVENT_ID]; // 0.서버, 1.파견, 2.상점, 3.암시장, 4.트레이닝, 5.열쇠 6.시간 보상
    public byte u1MailExist;
    public byte u1FriendExist;
    public byte u1RankRewad;
    public Byte u1MarkType = 0;
    public Byte u1RecvLoginReward = 0;
    public Byte u1VIPUpgrade = 0;

    public List<Hero> lstSortedChar;

    public double timeDist = 0;
	public DateTime ServerTime
	{
		get { return DateTime.Now.AddSeconds(timeDist); }
	}    

	public UInt16 TopLevel
	{
		get
		{
			if(acHeros.Count < 1)
				return 0;

			UInt16 top = u2Level;

			for(int i=0; i<acHeros.Count; i++)
			{
				if(top < acHeros[i].cLevel.u2Level)
					top = acHeros[i].cLevel.u2Level;
			}

			u2Level = top;
			return top;
		}
	}

	public bool bTowerClearPopup;
	public bool bTowerResetPopup;

	public bool bStageFailed = false;
    // 2017. 02. 02 jy
    // 로그인 팝업 스탭 추가 해서 필요없을꺼라 생각해서 주석 함
	//public bool bADView = false;
	//public bool bCafeView = false;

    public ChangeCharInfo eCharState = ChangeCharInfo.NONE;
    public enum ChangeCharInfo
    {
        NONE,
        CHANGED,
        ERROR
    }

	//event update
	public UInt16 effectEventID = 0;

	public UInt16 GetTopEquipLevel()
	{
		if(cInventory.lstSortedEquipment.Count < 1)
			return 0;

		UInt16 top = 1;

		for(int i=0; i<cInventory.lstSortedEquipment.Count; i++)
		{
			if(top < cInventory.lstSortedEquipment[i].cLevel.u2Level)
				top = cInventory.lstSortedEquipment[i].cLevel.u2Level;
		}

		u2EquipTopLevel = top;
		return top;
	}

	public void SetTopEquipLevel(UInt16 _u2Lv)
	{
		if (u2EquipTopLevel < _u2Lv) {
			u2EquipTopLevel = _u2Lv;
		}
	}
	
	public int AverageLevel(Crew crew)
	{
        UInt16 averLevel = 0;
        int count = 0;

        for(int i=0; i<crew.acLocation.Length; i++)
        {
            if(crew.acLocation[i] != null)
            {
                averLevel += crew.acLocation[i].cLevel.u2Level;
                count++;
            }
        }
        
        return averLevel / 3;
	}

	List<UInt16> downloadClass = new List<ushort> ();

	//TestEnemy
	public LeagueCrew cTestEnemyCrew;
	public string sEnemyName;

//	int selectedChar;
//	public void SelectCharacter(int charIdx)
//	{
//		selectedChar = charIdx;
//	}
	public Character GetSelectedCharacter()
	{
		return cBestCrew.First;
	}
	
	public Reward cReward;
	
	public Crew SelectedCrew
	{
		get { return cBestCrew; }
	}
	public StageInfo SelectedStage
	{
		get { return StageInfoMgr.Instance.dicStageData[u2SelectStageID]; }
	}
	public Reward SelectedReward
	{
		get	{ return cReward; }
	}

    public EventDungeonInfo SelectedBossRushStage;

	// 2016. 11. 22 jc
	// legue Data
	public const Byte MAX_CHAR_IN_LEAGUE_CREW = 5;

	public Byte GetDivision
	{
		get
		{
			if(UI_League.Instance.cLeagueMatchList.u1DivisionIndex == 6 && UI_League.Instance.cLeagueMatchList.u4MyRank == 1)
				return (Byte)(UI_League.Instance.cLeagueMatchList.u1DivisionIndex + 1); 
			else	
				return UI_League.Instance.cLeagueMatchList.u1DivisionIndex; 
		}
	}

	public UInt16 GetDivisionID
	{
		get{ return (UInt16)(4500+UI_League.Instance.cLeagueMatchList.u1DivisionIndex); }
	}
	
//	Byte u1LegendState;
//	public Byte GetLegend
//	{
//	    get { return u1LegendState; }
//	}
//	public void SetLegendState(Byte _legend)
//	{
//	    u1LegendState = _legend;
//	}

	public Byte[] au1LeagueCharIndex = new Byte[MAX_CHAR_IN_LEAGUE_CREW];
	public Byte u1LeagueReward;
	public UInt16 u2SeasonNum;

	public UInt16 u2LeagueWin;
	public UInt16 u2LeagueDraw;
	public UInt16 u2LeagueLose;

	public LeagueCrew cLeagueCrew;

	public Byte u1LastBattleResult;
	public Int16 u2LastBattleResultPoint;

	public LeagueInfo SelectedLeague
	{
		get { return LeagueInfoMgr.Instance.GetLeagueInfo((UInt16)(4500+UI_League.Instance.cLeagueMatchList.u1DivisionIndex)); }
	}

	public void UpdateLeagueInfoByResult(Byte _u1ResultType)
	{
		u1LastBattleResult = _u1ResultType;
		switch (_u1ResultType) {
		case 0:
			//UI_League.Instance._leagueMatch.u2Lose++;
			break;
		case 1:
			//UI_League.Instance._leagueMatch.u2Win++;
			AddGoods (SelectedLeague.cWinReward);
			break;
		case 2:
			//UI_League.Instance._leagueMatch.u2Draw++;
			break;
		}
	}

	public void UpdateGuildInfoByResult(Byte _u1ResultType)
	{
		u1LastBattleResult = _u1ResultType;
		switch (_u1ResultType) {
		case 0:
			break;
		case 1:
			break;
		case 2:
			break;
		}
	}

    // ============= 로그인 시 팝업 스탭 관련 ==================//
    public enum LoginPopupStep
    {
        NONE = -1,          // 없음
        EVENT_NOTICE = 0,   // 이벤트 공지
        EVENT_MONTHLY_PACK, // 월간 상품
        LOGIN_REWARED,      // 로그인 보상
        EVENT_30DAY_PACK,   // 30일 패키지
        OX_EVENT,           // OX 이벤트
        ODIN_PAGE,          // 오딘 페이지
        NAVER_CAFE,         // 까페
        STEP_MAX,           // 스탭 갯수
    }

    private Byte u1LoginPopupStep;

    // 스탭 추가
    public void AddLoginPopupStep(LoginPopupStep eStep)
    {
        // 불필요 값이 들어온다면 추가하지 않는다
        if (eStep == LoginPopupStep.STEP_MAX || eStep == LoginPopupStep.NONE)
            return;

        int step = (int)eStep;
        u1LoginPopupStep = (Byte)(u1LoginPopupStep | (0x01 << step));
    }
    // 스탭 제거
    public void SubLoginPopupStep(LoginPopupStep eStep)
    {
        // 이벤트 스텝이 0보다 크면 스텝을 제거 한다
        if (u1LoginPopupStep > 0)
        {
            int step = (int)eStep;
            Byte stepCode = (Byte)(0x01 << step);
            if ((u1LoginPopupStep & stepCode) == stepCode)
                u1LoginPopupStep = (Byte)(u1LoginPopupStep ^ (0x01 << step));

            // 다음 팝업창을 확인한다
            LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
            if (lobbyScene != null)
                lobbyScene.CheckLoginPopupStep();
        }
    }

    // 스탭 체크
    public LoginPopupStep GetLoginPopupStep
    {
        get
        {
            if (u1LoginPopupStep > 0)
            {
                int stepCount = (int)LoginPopupStep.STEP_MAX;
                for (int i = 0; i < stepCount; ++i)
                {
                    Byte stepCode = (Byte)(0x01 << i);
                    if ((stepCode & u1LoginPopupStep) == stepCode)
                        return (LoginPopupStep)i;
                }
            }
            return LoginPopupStep.NONE;
        }
    }

    // 스탭 체크
    public bool IsLoginPopupStepTpye(LoginPopupStep eStep)
    {
        if (u1LoginPopupStep > 0)
        {
            int step = (int)eStep;
            Byte stepCode = (Byte)(0x01 << step);
            if ((stepCode & u1LoginPopupStep) == stepCode)
                return true;
        }
        return false;
    }
    // ============= 로그인 시 팝업 스탭 관련 END ==================//

    // 2016. 11. 16 jy
    // 스크린 배율
    public float ratio;
	private Vector2 screenRatio;
	public Vector2 ScreenRatio
	{
		get { return screenRatio;}
		set 
		{ 
			if( value.x > BaseScreenSizeX )
				screenRatio.x = value.x / BaseScreenSizeX;
			else
				screenRatio.x = 1f;
			
			if( value.y > BaseScreenSizeY )
				screenRatio.y = value.y / BaseScreenSizeY;
			else
				screenRatio.y = 1f;
		}
	}

	// 2016. 12. 30 jc
	public enum PushEventID{
		//1,2,3,4,5 = Crew Dispatch
		Dispatch = 0,
		Shop = 100,
		BlackShop = 101,
		Training = 200,
		MaxKey = 500,
		TimeReward = 600,
        ToDayAccess_M = 700,          // 오늘 접속 보상 (아침)
        ToDayAccess_A = 701,          // 오늘 접속 보상 (오후)
        ToDayAccess_E = 702,          // 오늘 접속 보상 (저녁)
        TomorrowAccess_M = 800,       // 다음날 접속 보상 (저녁)
        TomorrowAccess_A = 801,       // 다음날 접속 보상 (아침)
        TomorrowAccess_E = 802,       // 다음날 접속 보상 (오후)
        AfterTomorrowAccess_M = 900,  // 모레 접속 보상 (저녁)
        AfterTomorrowAccess_A = 901,  // 모레 접속 보상 (아침)
        AfterTomorrowAccess_E = 902,  // 모레 접속 보상 (오후)
    }
	// 2017. 01. 04 jy
	// pushEventID의 갯수를 셋팅한다 
	// PushEventID 숫자가 증가하면 증가한만큼 증가시켜야 한다
    // push 갯수 + 1 해야한다
	public const int MAX_PUSH_EVENT_ID = 3;

	//그래픽 옵션
	public int graphicGrade = 0;

	//추천장비제작
	public int equipShortCut = 0;
	public int equipNeedHeroIndex = -1;

	void Awake()
	{
        if((float)(Screen.width/Screen.height) > 1.6f && (float)(Screen.width/Screen.height) < 1.8f)
            Screen.SetResolution(1280, 720, true);
        else if((float)(Screen.width/Screen.height) > 1.5f && (float)(Screen.width/Screen.height) < 1.7f)
            Screen.SetResolution(1280, 800, true);
        else if((float)(Screen.width/Screen.height) > 1.2f && (float)(Screen.width/Screen.height) < 1.4f)
            Screen.SetResolution(1024, 768, true);
        else
            Screen.SetResolution(1280, 720, true);
		//DebugMgr.Log ("Legion Awake");
		if (GameObject.Find ("GAv3") != null) {
			googleAnalytics = GameObject.Find ("GAv3").GetComponent<GoogleAnalyticsV3> ();
			googleAnalytics.StartSession();
		}
#if UNITY_EDITOR
#elif UNITY_IOS

        // Set the Handler class. This needs to be a unity GameObject
        IgaworksCorePluginIOS.SetCallbackHandler("IgaworksSample");											// Set this to the name of your linked GameObject 

		// init
		IgaworksCorePluginIOS.IgaworksCoreWithAppKey("123092069","4ace4bb84e6d4977");

		// set UseIgaworksRewardServer
		IgaworksCorePluginIOS.SetUseIgaworksRewardServer(true);

		// set log level
		IgaworksCorePluginIOS.SetLogLevel(IgaworksCorePluginIOS.IgaworksCoreLogTrace);

		// set IgaworksCoreDelegate
		IgaworksCorePluginIOS.SetIgaworksCoreDelegate();
#elif UNITY_ANDROID
		IgaworksUnityPluginAOS.InitPlugin ();
		IgaworksUnityPluginAOS.Common.startApplication ();
#endif
		UM_GameServiceManager.OnPlayerConnected += OnPlayerConnected;
		UM_GameServiceManager.OnPlayerDisconnected += OnPlayerDisconnected;
		InitUserData ();
	}

	void Start()
	{

        //DebugMgr.Log ("Legion Start");
        //IgaworksUnityPluginAOS.Common.setUserId ("");

        //Vungle.init("580d80a23e64c2d316000050", "581f069a169b16114d00012e", "Test_Windows");
        #if UNITY_EDITOR

        #elif UNITY_ANDROID
			IgaworksUnityPluginAOS.LiveOps.initialize ("432232593496");
			IgaworksUnityPluginAOS.LiveOps.setNotificationIconStyle("icon_24_02","icon_02", "ffffffff");
			Advertisement.Initialize ("1177666", false);
			AdColony.Configure (AdColony.version, "appd264eebbb50e4056b9", "vz0907fb8e95b947d89d");

			if(FB.IsInitialized) FB.Mobile.FetchDeferredAppLinkData(DeepLinkCallback);
        #elif UNITY_IOS
			Vungle.init("580d80a23e64c2d316000050", "581f069a169b16114d00012e", "Test_Windows");
			Advertisement.Initialize ("1191027", false);
			AdColony.Configure (AdColony.version, "app8ff7e22696544f809a", "vze65cf270b2e64e9895");
			if(FB.IsInitialized) FB.Mobile.FetchDeferredAppLinkData(DeepLinkCallbackIOS);
#endif
    }
    
	public void InitUserData()
	{
		acCrews = new Crew[MAX_CREW_OF_LEGION];
		for (int i = 0; i < MAX_CREW_OF_LEGION; i++)
		{
			acCrews[i] = new Crew();
			acCrews[i].u1Index = (Byte)(i + 1);
		}
		acHeros = new List<Hero>();
		cBestCrewBackUp = new Crew();
		cBestCrew = new Crew(); 
        cLeagueCrew = new LeagueCrew();
		lstSortedChar = new List<Hero> ();

		tempCrewIndex = -1;

		cInventory = new Inventory();
		cRuneventory = new Runeventory();
		cTutorial = new Tutorial();
		cQuest = new Quest();
		cEvent = new UserEventInfo ();

        SelectedBossRushStage = new EventDungeonInfo();

		bLoaded = false;
		bCrewToChar = false;
		bStageToCrew = false;
        bCharInfoToCrew = false;
        bLeagueToCharInfo = false;

		if (DataMgr.Instance.LoadStep != DataMgr.LOAD_STEP.NONE)
		{
			DataMgr.Instance.SetLoadStep (DataMgr.LOAD_STEP.RELOGIN);
			//DataMgr.Instance.LoadAllStep(OnLoadFinished, OnLoadFinished, !Server.ServerMgr.bConnectToServer);
		}
		v3MainCharRotation = new Vector3[3];
		for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
		{
			v3MainCharRotation[i] = Vector3.zero;
		}
		//화면 켜짐상태 유지 임시 코드
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		for(int i = 0; i < MAX_PUSH_EVENT_ID; ++i)
		{
			//pushActive = ObscuredPrefs.GetBool("pushActive", true);
			pushActive[i] = ObscuredPrefs.GetBool("pushActive_" + i, true);
		}
		if (PlayerPrefs.GetInt ("bAuto", 0) == 1) bAuto = true;

		cSelectHeroElement = new Hero();

		u2EquipTopLevel = 1;

		SelectedDifficult = 1;

		u2SelectFieldID = 0;
		u2SelectStageID = 6001;
		eGameStyle = GameStyle.None;

		charTrainingRoom = new byte[Server.ConstDef.MaxCharTrainingRoom];
		equipTrainingRoom = new byte[Server.ConstDef.MaxEquipTrainingRoom];

		acEquipDesign = new BitArray(Server.ConstDef.SizeOfEquipDesignBuffer*8, true);
		acEquipDesignNew = new BitArray(Server.ConstDef.SizeOfEquipDesignBuffer*8, true);
        acEquipDesignMake = new BitArray(Server.ConstDef.SizeOfEquipDesignBuffer*8, true);
		acLookDesign = new BitArray(Server.ConstDef.SizeOfLookDesignBuffer*8, true);
		acLookDesignNew = new BitArray(Server.ConstDef.SizeOfLookDesignBuffer*8, true);
        acMakeNewBinary = new BitArray(Server.ConstDef.SizeOfMakeNewBinaryBuffer*8, true);

		//cTutorial = new Tutorial();
	
		osLogin = false;
		checkLoginAchievement = 0;
		pushSetting = 0;
		pushRegisted = false;
		termspush = false;
		u1MailExist = 0;
		u1FriendExist = 0;
		u1RankRewad = 0;

		timeDist = 0;

		bTowerClearPopup = false;
		bTowerResetPopup = false;
        m_bVIPLevelUp = false;

		equipShortCut = 0;
		equipNeedHeroIndex = -1;

		//bADView = false;
		//bCafeView = false;

		charAvailable = new byte[Server.ConstDef.MaxCharClass];

		EventInfoMgr.Instance.InitUserData ();

		ObjMgr.Instance.RemoveAll ();
	}

	public void OnLoadFinished()
	{
	}

	public void AwayBattle()
	{
		if (Legion.Instance.eGameStyle == GameStyle.None || Legion.Instance.eGameStyle == GameStyle.AnimTest)
			return;
		
		Time.timeScale = 1.0f;
		cEvent.selectedOpenEventID = 0;
		PopupManager.Instance.HideItemInfo ();
		SoundManager.Instance.OffBattleListner ();
		ObjMgr.Instance.RemoveMonsterPool();
		ObjMgr.Instance.RemoveHeroModelPool ();
		VFXMgr.Instance.RemoveAll();
		SoundManager.Instance.PlayLoadBGM ("Sound/BGM/BGM_Common_Main");
//		GC.Collect();
		Legion.Instance.eGameStyle = GameStyle.None;
	}

    public Hero GetHero(Byte u1Index)
    {
		foreach (Hero hero in acHeros)
		{
			if (hero.u1Index == u1Index)
			{
                return hero;
			}
		}
        return null;
    }
	public void AddNewHero(Hero newHero)
	{
		Byte u1NewIndex = 1;
		bool bMatched = true;
		while (bMatched)
		{
			bMatched = false;
			foreach (Hero hero in acHeros)
			{
				if (hero.u1Index == u1NewIndex)
				{
					u1NewIndex++;
					bMatched = true;
					break;
				}
			}
		}
		newHero.u1Index = u1NewIndex;
		acHeros.Add(newHero);
		cLastUpdatedHero = newHero;
	}
    public void RemoveHero(Hero hero)
    {
        if (hero != null)
        {
            if (hero.u1AssignedCrew != 0) acCrews[hero.u1AssignedCrew - 1].Resign(hero);
            acHeros.Remove(hero);
        }
    }

	public int CheckDispatch(UInt16 stageID)
	{
		for(int i=0; i<acCrews.Length; i++)
		{
			if(acCrews[i].DispatchStage != null && acCrews[i].DispatchStage.u2ID == stageID && (acCrews[i].StageDifficulty == SelectedDifficult))
				return i;
		}

		return -1;
	}

	public void SetRuneventory()
	{
		for(int i=0; i<Server.ConstDef.SizeOfRuneBuffer; i++)
		{
			if(runeList[i] != 0)
			{
				UInt16 runeID = (UInt16)(Server.ConstDef.BaseRuneID + i);
				cRuneventory.AddItem(runeID, runeList[i]);
			}
		}
	}

	public void SetAchievement()
	{

	}

	public bool CheckEmptyInven(){
		if (Legion.Instance.cInventory.IsInvenFull()) {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_inven_full"), TextManager.Instance.GetText("popup_desc_full_inven"), null);
			return false;
		}

		return true;
	}

	public string GetConsumeString(int index)
	{
		switch((GoodsType)index)
		{
		case GoodsType.GOLD:
			return TextManager.Instance.GetText("mark_gold");
		case GoodsType.CASH:
			return TextManager.Instance.GetText("mark_cash");
		case GoodsType.KEY:
			return TextManager.Instance.GetText("mark_key");
		case GoodsType.LEAGUE_KEY:
			return TextManager.Instance.GetText("mark_leaguekey");
		case GoodsType.FRIENDSHIP_POINT:
			return TextManager.Instance.GetText("mark_friendshippoint");
		case GoodsType.LEVEL:
			return TextManager.Instance.GetText("mark_level");
		case GoodsType.VIP_LEVEL:
			return "VIP";
        case GoodsType.ODIN_POINT:
			return "ODIN POINT";
		case GoodsType.EQUIP:
			return "EQUIP";
		case GoodsType.MATERIAL:
			return "MATERIAL";
		case GoodsType.CONSUME:
			return "CONSUME";
		case GoodsType.SCROLL:
			return "SCROLL";
		case GoodsType.EQUIP_COUPON:
			return TextManager.Instance.GetText("mark_equipment_gacha");
		case GoodsType.MATERIAL_COUPON:
			return TextManager.Instance.GetText("mark_material_gacha");
		case GoodsType.TRAINING_ROOM:
			return "TRAINING ROOM";
		case GoodsType.EQUIP_TRAINING:
			return "EQUIP TRAINING";
        case GoodsType.STAGE_CLEAR:
            return "STAGE CLEAR";
		}

		return "";
	}

	public string GetGoodsName(Goods goods)
	{
		switch((GoodsType)goods.u1Type)
		{
		case GoodsType.GOLD:
			return TextManager.Instance.GetText("mark_gold");
		case GoodsType.CASH:
			return TextManager.Instance.GetText("mark_cash");
		case GoodsType.KEY:
			return TextManager.Instance.GetText("mark_key");
		case GoodsType.LEAGUE_KEY:
			return TextManager.Instance.GetText("mark_leaguekey");
		case GoodsType.FRIENDSHIP_POINT:
			return TextManager.Instance.GetText("mark_friendshippoint");
		case GoodsType.LEVEL:
			return TextManager.Instance.GetText("mark_level");
		case GoodsType.VIP_LEVEL:
			return "VIP";
		case GoodsType.ODIN_POINT:
			return "ODIN POINT";
		case GoodsType.EQUIP:
			if(goods.u2ID == 0) return TextManager.Instance.GetText("recomm_gacha");
			return TextManager.Instance.GetText(EquipmentInfoMgr.Instance.GetInfo(goods.u2ID).sName);
		case GoodsType.MATERIAL:
			return TextManager.Instance.GetText (ItemInfoMgr.Instance.GetMaterialItemInfo (goods.u2ID).sName);
		case GoodsType.CONSUME:
			return TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (goods.u2ID).sName);
		case GoodsType.SCROLL:
			return "SCROLL";
		case GoodsType.EQUIP_COUPON:
			return TextManager.Instance.GetText("mark_equipment_gacha");
		case GoodsType.MATERIAL_COUPON:
			return TextManager.Instance.GetText("mark_material_gacha");
		case GoodsType.TRAINING_ROOM:
			return "TRAINING ROOM";
		case GoodsType.EQUIP_TRAINING:
			return "EQUIP TRAINING";
		case GoodsType.STAGE_CLEAR:
			return "STAGE CLEAR";
        case GoodsType.CHARACTER_PACKAGE:
            return TextManager.Instance.GetText("mark_char_name");
        case GoodsType.EQUIP_GOODS:
            {
                ClassGoodsEquipInfo goodsEquipInfo = null;
                if (EventInfoMgr.Instance.dicClassGoodsEquip.TryGetValue(goods.u2ID, out goodsEquipInfo))
                {
                    EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(goodsEquipInfo.u2Equip);
                    if (equipInfo != null)
                    {
                        return TextManager.Instance.GetText(equipInfo.EquipTypeKey());
                    }
                }
            }
            break;
		case GoodsType.EVENT_ITEM:
			if (EventInfoMgr.Instance.dicMarbleGoods.ContainsKey (goods.u2ID)) {
				return TextManager.Instance.GetText (EventInfoMgr.Instance.dicMarbleGoods [goods.u2ID].sName);
			}
			break;
		}

		return "";
	}

	public bool CheckEnoughGoods(int index, long value)
	{
		switch((GoodsType)index)
		{
		case GoodsType.GOLD:
			return (value > Gold) ? false : true;
		case GoodsType.CASH:
			return (value > Cash) ? false : true;
		case GoodsType.KEY:
			return (value > Energy) ? false : true;
        case GoodsType.LEAGUE_KEY:
            return (value > LeagueKey) ? false : true;
		case GoodsType.FRIENDSHIP_POINT:
			return (value > FriendShipPoint) ? false : true;
        case GoodsType.STAGE_CLEAR:
            return StageInfoMgr.Instance.IsClear((ushort)value);
		}
        
		return true;
	}

	public bool CheckEnoughGoods(Goods goods)
	{
		ushort slot = 0;

		switch((GoodsType)goods.u1Type)
		{
		case GoodsType.GOLD:
			return (goods.u4Count > Gold) ? false : true;
		case GoodsType.CASH:
			return (goods.u4Count > Cash) ? false : true;
		case GoodsType.KEY:
			return (goods.u4Count > Energy) ? false : true;
		case GoodsType.LEAGUE_KEY:
			return (goods.u4Count > LeagueKey) ? false : true;
		case GoodsType.FRIENDSHIP_POINT:
			return (goods.u4Count > FriendShipPoint) ? false : true;
		case GoodsType.LEVEL:
			return (goods.u4Count > TopLevel) ? false : true;
		case GoodsType.VIP_LEVEL:
			return (goods.u4Count > u1VIPLevel) ? false : true;
		case GoodsType.MATERIAL:
			if (goods.u2ID == 0) {
				DebugMgr.LogError("Goods ID is None !!");
				return false;
			}
			
			slot = Legion.Instance.cInventory.dicInventory.FirstOrDefault(cs => cs.Value.cItemInfo.u2ID == goods.u2ID).Key;
			if(slot > 0){
				MaterialItem have = (MaterialItem)Legion.Instance.cInventory.dicInventory[slot];
				if(have.u2Count < goods.u4Count) return false;
			}
			break;
		case GoodsType.CONSUME:
			if (goods.u2ID == 0) {
				DebugMgr.LogError("Goods ID is None !!");
				return false;
			}
			
			slot = Legion.Instance.cInventory.dicInventory.FirstOrDefault(cs => cs.Value.cItemInfo.u2ID == goods.u2ID).Key;
			if(slot > 0){
				ConsumableItem have = (ConsumableItem)Legion.Instance.cInventory.dicInventory[slot];
				if(have.u2Count < goods.u4Count) return false;
			}
			break;
		case GoodsType.SCROLL:
			return false;
		case GoodsType.EQUIP_COUPON:
			return false;
		case GoodsType.MATERIAL_COUPON:
			return false;
		case GoodsType.TRAINING_ROOM:
			return false;
		case GoodsType.EQUIP_TRAINING:
			return false;
		case GoodsType.STAGE_CLEAR:
			if (goods.u2ID == 0) {
				DebugMgr.LogError("Goods ID is None !!");
				return false;
			}
			return StageInfoMgr.Instance.dicStageData[goods.u2ID].IsClear((byte)goods.u4Count);
            // if(StageInfoMgr.Instance.dicStageData[goods.u2ID].clearState > 0)
            //     return true;
            // else
            //	  return false;
        case GoodsType.EVENT_ITEM:
            if (EventInfoMgr.Instance.dicMarbleBag.ContainsKey(goods.u2ID))
            {
                if (EventInfoMgr.Instance.dicMarbleBag[goods.u2ID].u4Count >= goods.u4Count)
                    return true; 
            }
            return false;
        }
		return true;
	}

	public bool CheckGoodsLimitExcessx(Byte goodsType)
	{
		switch((GoodsType)goodsType)
		{
		case GoodsType.GOLD:
			if( LegionInfoMgr.Instance.GoldMax <= Gold )
				return true;
			break;
		case GoodsType.CASH:
			if( LegionInfoMgr.Instance.CashMax <= Cash )
				return true;
			break;
		case GoodsType.KEY:
			if( LegionInfoMgr.Instance.keyTime.limitGoodCount <= Energy )
				return true;
			break;
		case GoodsType.FRIENDSHIP_POINT:
			if( SocialInfo.Instance.dicSocialInfo[1].MAX_FRIENDPOINT <= FriendShipPoint )
				return true;
			break;
        case GoodsType.CHARACTER_PACKAGE:
            if (LegionInfoMgr.Instance.limitCharSlot <= acHeros.Count)
                return true;
            break;
                // 추후 추가 재화 생성시 추가 
        }

		return false;
	}

	public bool CheckGoodsLimitExcessx(Goods goods)
	{
		if(goods == null)
			return false;

		if(CheckGoodsLimitExcessx(goods.u1Type))
			return true;

		switch((GoodsType)goods.u1Type)
		{
		case GoodsType.GOLD:
			if(LegionInfoMgr.Instance.GoldMax < (Gold + goods.u4Count))
				return true;
			break;
		case GoodsType.CASH:
			if(LegionInfoMgr.Instance.CashMax < (Cash + goods.u4Count))
				return true;
			break;
		case GoodsType.KEY:
			if(LegionInfoMgr.Instance.keyTime.limitGoodCount < (Energy + goods.u4Count))
				return true;
			break;
		case GoodsType.FRIENDSHIP_POINT:
			if(SocialInfo.Instance.dicSocialInfo[1].MAX_FRIENDPOINT < (FriendShipPoint+ goods.u4Count))
				return true;//< (u2FriendShipPoint + goods.u4Count))
			break;
        case GoodsType.EQUIP:
        case GoodsType.EQUIP_GOODS:
            if (LegionInfoMgr.Instance.GetMaxInvenSize() < (cInventory.dicInventory.Count + goods.u4Count))
                return true;
            break;
        //case GoodsType.MATERIAL:
        //    {
        //        UInt16 slot = Legion.Instance.cInventory.dicInventory.FirstOrDefault(cs => cs.Value.cItemInfo.u2ID == goods.u2ID).Key;
        //        if (slot > 0)
        //        {
        //            if (LegionInfoMgr.Instance.maxItemCount < ((MaterialItem)(Legion.Instance.cInventory.dicInventory[slot])).u2Count + goods.u4Count)
        //                return true;
        //        }
        //    }
        //    break;
        //case GoodsType.CONSUME:
        //    {
        //        UInt16 slot = Legion.Instance.cInventory.dicInventory.FirstOrDefault(cs => cs.Value.cItemInfo.u2ID == goods.u2ID).Key;
        //        if (slot > 0)
        //        {
        //
        //            if (LegionInfoMgr.Instance.maxItemCount < ((ConsumableItem)(Legion.Instance.cInventory.dicInventory[slot])).u2Count + goods.u4Count)
        //                return true;
        //        }
        //    }
        //    break;
        case GoodsType.CHARACTER_PACKAGE:
            if (LegionInfoMgr.Instance.limitCharSlot < (acHeros.Count + goods.u4Count))
                return true;
            break;                
        // 추후 추가 재화 생성시 추가 

        }
		
		return false;
	}

	public bool CheckGoodsLimitExcessx(ShopGoodInfo cInfo)
	{
		if(cInfo.cShopItem == null)
			return false;

		if(CheckGoodsLimitExcessx(cInfo.cShopItem))
			return true;

		Byte u1Add = 0;
		EventReward eventInfo;
		if (EventInfoMgr.Instance.dicEventReward.FirstOrDefault(cs => cs.Value.u1EventType == (Byte)EVENT_TYPE.FIRSTPAYMENT).Key > 0)
		{
			eventInfo = EventInfoMgr.Instance.dicEventReward.FirstOrDefault(cs => cs.Value.u1EventType == (Byte)EVENT_TYPE.FIRSTPAYMENT).Value;

			bool isFirstBuy = false;
			int buyListCount = Legion.Instance.cEvent.lstItemBuyHistory.Count;
			for (int i = 0; i < buyListCount; ++i)
			{
				if (Legion.Instance.cEvent.lstItemBuyHistory[i].u2ShopID == cInfo.u2ID &&
				Legion.Instance.cEvent.lstItemBuyHistory[i].u2EventID == eventInfo.u2EventID)
				{
					isFirstBuy = true;
					break;
				}
			}

			if (isFirstBuy == false)
			{
				u1Add = eventInfo.u1RewardIndex;
			}
		}

		switch((GoodsType)cInfo.cShopItem.u1Type)
		{
		case GoodsType.GOLD:
		if( LegionInfoMgr.Instance.GoldMax < ( Gold + cInfo.cShopItem.u4Count + LegionInfoMgr.Instance.GetAddVipValue(cInfo) + cInfo.GetBuyBonus() + ((cInfo.cShopItem.u4Count * u1Add)/100) ) )
			return true;
			break;
		case GoodsType.CASH:
		if( LegionInfoMgr.Instance.CashMax < ( Cash + cInfo.cShopItem.u4Count + LegionInfoMgr.Instance.GetAddVipValue(cInfo) + cInfo.GetBuyBonus() + ((cInfo.cShopItem.u4Count * u1Add)/100) ) )
			return true;
			break;
		// 추후 추가 재화 생성시 추가 
		}

		return false;
	}

	public void ShowGoodsOverMessage(Byte goodsType)
	{
		string message = "";
		switch((GoodsType)goodsType)
		{
		case GoodsType.GOLD:
			message = TextManager.Instance.GetText("mark_over_max_gold");
			break;
		case GoodsType.CASH:
			message = TextManager.Instance.GetText("mark_over_max_cash");
			break;
		case GoodsType.KEY:
			message = TextManager.Instance.GetText("mark_over_max_key");
			break;
		case GoodsType.FRIENDSHIP_POINT:
			message = TextManager.Instance.GetText("popup_maxium_goods_friendspoint");
			break;
        case GoodsType.EQUIP:
        case GoodsType.EQUIP_GOODS:
        //case GoodsType.CONSUME:
        //case GoodsType.MATERIAL:
            message = TextManager.Instance.GetText("popup_desc_full_inven");
            break;
        case GoodsType.CHARACTER_PACKAGE:
            message = TextManager.Instance.GetText("popup_not_create_crew");
            break;
		// 추후 추가 재화 생성시 추가 
		}

		if(message != "")
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), message, null);
	}

	public void SubGoods(int index, long value)
	{
		switch((GoodsType)index)
		{
		    case GoodsType.GOLD:
			    Gold -= (UInt32)value;
                if(Gold < 0)
                    Gold = 0;
                break;
		    case GoodsType.CASH:
			    Cash -= (UInt32)value;
                if(Cash < 0)
                    Cash = 0;
                break;
		    case GoodsType.KEY:
			    Energy -= (UInt16)value;
                break;
		    case GoodsType.LEAGUE_KEY:
			    //u2LeagueKey -= (UInt16)value;
                LeagueKey -= (UInt16)value;
                break;
		    case GoodsType.FRIENDSHIP_POINT:
			    //u2FriendShipPoint -= (UInt16)value;
                FriendShipPoint -= (UInt16)value;
                if(FriendShipPoint < 0)
                    FriendShipPoint = 0;
                break;
		}
	}

	public void SubGoods(Goods goods)
	{
		ushort slot = 0;

		switch((GoodsType)goods.u1Type)
		{
		case GoodsType.GOLD:
			Gold -= (UInt32)goods.u4Count; break;
		case GoodsType.CASH:
			Cash -= (UInt32)goods.u4Count;
			if (bCheckCash && ServerCash != Cash) {
				Cash = ServerCash;
			}
			bCheckCash = false;
			break;
		case GoodsType.KEY:
			Energy -= (UInt16)goods.u4Count; break;
		case GoodsType.LEAGUE_KEY:
			LeagueKey -= (UInt16)goods.u4Count; break;
		case GoodsType.FRIENDSHIP_POINT:
			FriendShipPoint -= (UInt16)goods.u4Count; break;
		case GoodsType.MATERIAL:
			if(goods.u2ID == 0) DebugMgr.LogError("Goods ID is None !!");
			
			slot = Legion.Instance.cInventory.dicInventory.FirstOrDefault(cs => cs.Value.cItemInfo.u2ID == goods.u2ID).Key;
			if(slot > 0){
				((MaterialItem)(Legion.Instance.cInventory.dicInventory[slot])).u2Count -= (UInt16)goods.u4Count;

				if(((MaterialItem)(Legion.Instance.cInventory.dicInventory[slot])).u2Count <= 0)
					cInventory.RemoveItem(goods.u2ID);
			}
			break;
		case GoodsType.CONSUME:
			if(goods.u2ID == 0) DebugMgr.LogError("Goods ID is None !!");
			
			slot = Legion.Instance.cInventory.dicInventory.FirstOrDefault(cs => cs.Value.cItemInfo.u2ID == goods.u2ID).Key;
			if(slot > 0){
				((ConsumableItem)(Legion.Instance.cInventory.dicInventory[slot])).u2Count -= (UInt16)goods.u4Count;

				if(((ConsumableItem)(Legion.Instance.cInventory.dicInventory[slot])).u2Count <= 0)
					cInventory.RemoveItem(goods.u2ID);
			}
			break;
		case GoodsType.EQUIP:
			DebugMgr.LogError("Need Check Equip Remove Function");
//			if(goods.u2ID == 0) DebugMgr.LogError("Goods ID is None !!");
//			
//			slot = Legion.Instance.cInventory.dicInventory.FirstOrDefault(cs => cs.Value.cItemInfo.u2ID == goods.u2ID).Key;
//			if(slot > 0){
//				cInventory.RemoveItem(goods.u2ID);
//			}
			break;
        case GoodsType.EVENT_ITEM:
            if (EventInfoMgr.Instance.dicMarbleBag.ContainsKey(goods.u2ID))
            {
				EventInfoMgr.Instance.dicMarbleBag[goods.u2ID].u4Count -= (UInt32)goods.u4Count;
            }
            break;
                
		}
	}

	public void AddGoods(int index, long value)
	{
		AddGoods(new Goods((Byte)index, 0, (UInt32)value));
	}

	public void AddGoods(Goods[] goods){
		for(int i=0; i<goods.Length; i++){
			if(goods[i] != null) AddGoods(goods[i]);
		}
	}

	public void AddGoods(Goods goods, bool bAchieve = true)
	{
        if (goods == null)
        {
            return;
        }
        //ushort slot = 0;
        UInt32 gap = 0;
		switch ((GoodsType)goods.u1Type) {
		case GoodsType.GOLD:
			Gold += (UInt32)goods.u4Count;
				
			if( CheckGoodsLimitExcessx(goods.u1Type) )
			{
				ShowGoodsOverMessage(goods.u1Type);
				gap = LegionInfoMgr.Instance.GoldMax - Gold;
				Gold = LegionInfoMgr.Instance.GoldMax;
			}
			
			if (bCheckGold && ServerGold != Gold) {
				gap = ServerGold - Gold;
				Gold = ServerGold;
			}
			bCheckGold = false;
			
			if(bAchieve) cQuest.UpdateAchieveCnt (AchievementTypeData.Gold, 0, 0, 0, 0, (UInt32)goods.u4Count+gap);
			break;
		case GoodsType.CASH:
			Cash += (UInt32)(goods.u4Count);
				
			if (CheckGoodsLimitExcessx (goods.u1Type) )
			{
				ShowGoodsOverMessage (goods.u1Type);
				gap = LegionInfoMgr.Instance.CashMax - Cash;
				Cash = LegionInfoMgr.Instance.CashMax;
			}

			if (bCheckCash && ServerCash != Cash) {
				gap = ServerCash - Cash;
				Cash = ServerCash;
			}
			bCheckCash = false;

            if (bAchieve)  cQuest.UpdateAchieveCnt (AchievementTypeData.Cash, 0, 0, 0, 0, (UInt32)(goods.u4Count+gap));
			break;
		case GoodsType.KEY:
			{
				if( CheckGoodsLimitExcessx(goods.u1Type) == false )
				{
					Energy += (UInt16)goods.u4Count;
                    if (bAchieve) cQuest.UpdateAchieveCnt (AchievementTypeData.StageKey, 0, 0, 0, 0, (UInt32)goods.u4Count);
				}
				else
				{
					ShowGoodsOverMessage(goods.u1Type);
                    if (bAchieve) cQuest.UpdateAchieveCnt (AchievementTypeData.StageKey, 0, 0, 0, 0, goods.u4Count - (LegionInfoMgr.Instance.keyTime.limitGoodCount - (UInt32)Energy));
					Energy = (UInt16)LegionInfoMgr.Instance.keyTime.limitGoodCount;
				}
			}
			break;
		case GoodsType.LEAGUE_KEY:
			{
				LeagueKey += (UInt16)goods.u4Count;
            
				if (LeagueKey > LegionInfoMgr.Instance.leagueKeyTime.limitGoodCount)
					LeagueKey = (UInt16)LegionInfoMgr.Instance.leagueKeyTime.limitGoodCount;             
			}
			break;
		case GoodsType.ODIN_POINT:
			AddVIPPoint(goods.u4Count);
			break;
		case GoodsType.FRIENDSHIP_POINT:
			if( CheckGoodsLimitExcessx(goods.u1Type) == false )
				FriendShipPoint += (UInt16)goods.u4Count;
			else
			{
				ShowGoodsOverMessage(goods.u1Type);
				FriendShipPoint = SocialInfo.Instance.dicSocialInfo[1].MAX_FRIENDPOINT;
			}
			break;

		case GoodsType.MATERIAL:
			if (goods.u2ID == 0)
				DebugMgr.LogError ("Goods ID is None !!");

			cInventory.AddItem (0, goods.u2ID, goods.u4Count);
			
//			slot = Legion.Instance.cInventory.dicInventory.FirstOrDefault(cs => cs.Value.cItemInfo.u2ID == goods.u2ID).Key;
//			if(slot > 0){
//				((MaterialItem)(Legion.Instance.cInventory.dicInventory[slot])).u2Count += (UInt16)goods.u4Count;
//			}
			break;
		case GoodsType.CONSUME:
			if (goods.u2ID == 0)
				DebugMgr.LogError ("Goods ID is None !!");

			cInventory.AddItem (0, goods.u2ID, goods.u4Count);

//			slot = Legion.Instance.cInventory.dicInventory.FirstOrDefault(cs => cs.Value.cItemInfo.u2ID == goods.u2ID).Key;
//			if(slot > 0){
//				((ConsumableItem)(Legion.Instance.cInventory.dicInventory[slot])).u2Count += (UInt16)goods.u4Count;
//			}
			break;
		case GoodsType.SCROLL:
			if (!acEquipDesign.Get (goods.u2ID - Server.ConstDef.BaseEquipDesignID)) {
				acEquipDesign.Set (goods.u2ID - Server.ConstDef.BaseEquipDesignID, true);
				acEquipDesignNew.Set (goods.u2ID - Server.ConstDef.BaseEquipDesignID, true);
				acEquipDesignNew.Set (0, true);
				acEquipDesignMake.Set (goods.u2ID - Server.ConstDef.BaseEquipDesignID, true);
			}
			EquipmentInfo eInfo = EquipmentInfoMgr.Instance.GetInfo (goods.u2ID);
			if (eInfo.u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && eInfo.u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2) {
				if (!acLookDesign.Get (eInfo.u2ModelID - Server.ConstDef.BaseLookDesignID)) {
					acLookDesign.Set (eInfo.u2ModelID - Server.ConstDef.BaseLookDesignID, true);
					acLookDesignNew.Set (eInfo.u2ModelID - Server.ConstDef.BaseLookDesignID, true);
					acLookDesignNew.Set (0, true);
				}
			}
			break;
		case GoodsType.SCROLL_SET:

			EquipmentScrollSetInfo essInfo = EquipmentInfoMgr.Instance.GetEquipmentSetInfo (goods.u2ID);
			if (essInfo == null) {
				DebugMgr.LogError ("EquipmentScrollSetInfo is None !!");
				return;
			}

			for (int i = 0; i < EquipmentScrollSetInfo.MAX_SCROLL_IN_SET; i++) {
				AddGoods (new Goods ((Byte)GoodsType.SCROLL, essInfo.au2EquipIDs [i], 1));
			}
			break;
		case GoodsType.TRAINING_ROOM:

			Legion.Instance.charTrainingRoom [goods.u2ID - Server.ConstDef.BaseCharTrainingID - 1] = QuestInfoMgr.Instance.GetCharTrainingInfo()[goods.u2ID].GetOpenedSlotCount();
			break;

		case GoodsType.EQUIP_TRAINING:

			Legion.Instance.equipTrainingRoom [goods.u2ID - Server.ConstDef.BaseEquipTrainingID - 1] = QuestInfoMgr.Instance.GetEquipTrainingInfo()[goods.u2ID].GetOpenedSlotCount();
			break;

		case GoodsType.CHARACTER_PACKAGE:
			AddCharacterGoods (goods.u2ID);
			break;

		case GoodsType.CHARACTER_AVAILABLE:
            Legion.Instance.charAvailable[goods.u2ID - 1] = 1;

            downloadClass.Add(goods.u2ID);

            if (AssetMgr.Instance.useAssetBundle) {
                ShowClassDownload();
            }
#if UNITY_EDITOR
            else
            {
                string desc = string.Format(TextManager.Instance.GetText("popup_desc_class_unlock"), TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(goods.u2ID).sName));
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_class_unlock"), desc, ClassDownloadStart);
            }
#endif
            break;
        case GoodsType.EQUIP_GOODS:
            ClassGoodsEquipInfo egInfo = EventInfoMgr.Instance.dicClassGoodsEquip [goods.u2ID];
            EquipmentInfo egEquipInfo = EquipmentInfoMgr.Instance.GetInfo (egInfo.u2Equip);
            UInt16 egSlotNum = Legion.Instance.cInventory.AddEquipment(0, 0, egInfo.u2Equip, egInfo.u2Level, 0, egInfo.au1Skills, egInfo.au4Stats, 0, "", 
				Legion.Instance.sName, egEquipInfo.u2ModelID, true, egInfo.u1SmithingLevel, 0, 0, egInfo.u1StarLevel);
            Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.EquipLevel, egInfo.u2Equip, (Byte)egEquipInfo.u1PosID, 0, 0, egInfo.u2Level);
		    Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.GetEquip, egInfo.u2Equip, (Byte)egEquipInfo.u1PosID, egInfo.u1SmithingLevel, (Byte)egEquipInfo.u2ClassID, 1);
            break;
		case GoodsType.EVENT_ITEM:
			if (EventInfoMgr.Instance.dicMarbleBag.ContainsKey (goods.u2ID)) {
				EventInfoMgr.Instance.dicMarbleBag [goods.u2ID].u4Count += goods.u4Count;
			} else {
				Goods temp = new Goods (goods.u1Type, goods.u2ID, goods.u4Count);
				EventInfoMgr.Instance.dicMarbleBag.Add (goods.u2ID, temp);
			}
			break;
		}
	}

	public void AddCharacterGoods(UInt16 goodsID){
		ClassGoodsInfo info = EventInfoMgr.Instance.dicClassGoods [goodsID];

		if (info == null) {
			DebugMgr.LogError ("CHARACTER_PACKAGE NULL");
			return;
		}

		ClassInfo classinfo = ClassInfoMgr.Instance.GetInfo (info.u2ClassID);

		int u1SelectedFace = classinfo.lstFaceInfo.FindIndex (cs => cs.u2ID == info.u2Face);
		int u1SelectedHair = classinfo.lstHairInfo.FindIndex (cs => cs.u2ID == info.u2Hair);
		int u1SelectedHairColor = classinfo.lstHairColor.FindIndex (cs => cs.u2ID == info.u2HairColor);

		Hero hero = new Hero (0, info.u2ClassID, info.sName, (byte)(u1SelectedHair + 1), (byte)(u1SelectedHairColor + 1), (byte)(u1SelectedFace + 1));
		hero.u1MadeByUser = 2;
		UInt32[] stats = new UInt32[Server.ConstDef.CharStatPointType];
		hero.GetComponent<StatusComponent> ().LoadStatus (stats, 0);
		hero.GetComponent<LevelComponent> ().Set (info.u2Level, 0);
		List<LearnedSkill> initSkills = hero.GetComponent<SkillComponent>().GetInitSkill ();
		hero.GetComponent<SkillComponent> ().LoadSkill (Server.ConstDef.DefaultSkillSelectSlot, initSkills, 0, 0, 0);

		Legion.Instance.AddNewHero (hero);

		AddGoods (info.cReward);

		for (int i = 0; i < info.u2Equips.Length; i++) {
			ClassGoodsEquipInfo ginfo = EventInfoMgr.Instance.dicClassGoodsEquip [info.u2Equips[i]];
			EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo (ginfo.u2Equip);
			UInt16 slotNum = Legion.Instance.cInventory.AddEquipment(0, 0, ginfo.u2Equip, ginfo.u2Level, 0, ginfo.au1Skills, ginfo.au4Stats, 0, "", 
				Legion.Instance.sName,equipInfo.u2ModelID, true, ginfo.u1SmithingLevel, 0, 0, ginfo.u1StarLevel);

			hero.SetEquip((EquipmentItem)Legion.Instance.cInventory.dicInventory[slotNum]);

			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.EquipLevel, ginfo.u2Equip, (Byte)equipInfo.u1PosID, 0, 0, ginfo.u2Level);

			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.GetEquip, ginfo.u2Equip, (Byte)equipInfo.u1PosID, ginfo.u1SmithingLevel, (Byte)equipInfo.u2ClassID, 1);
		}

		Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.CharLevel, 0, (Byte)info.u2ClassID, 0, 0, info.u2Level);
	}

	private void ShowClassDownload(){
		if (downloadClass.Count > 0) {
			AssetMgr.Instance.InitDownloadList ();
			string className = "";
			for (int i = 0; i < downloadClass.Count; i++) {
				if (i > 0) className += ", ";

				className += TextManager.Instance.GetText (ClassInfoMgr.Instance.GetInfo (downloadClass [i]).sName);
				if (!AssetMgr.Instance.AddDivisionDownload (1, downloadClass [i])) {
					downloadClass.RemoveAt (i);
					i--;
				}
			}

			className += "";

			string desc = string.Format(TextManager.Instance.GetText("popup_desc_class_unlock"), className);

			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_class_unlock"), desc, ClassDownloadStart);
		}
	}

	private void ClassDownloadStart(object[] objs){
		AssetMgr.Instance.ShowDownLoadPopup ();
		downloadClass.Clear ();
	}
    
    private void RefreshGoodInfo()
    {
        GoodsInfo[] goodInfo = GameObject.FindObjectsOfType<GoodsInfo>() as GoodsInfo[];
        
        for(int i=0; i<goodInfo.Length; i++)
            goodInfo[i].Refresh();
    }

    public void CheckCrewSlotUnlock()
    {
        if(StageInfoMgr.Instance.dicChapterData[(UInt16)(Server.ConstDef.BaseChapterID+2)].CheckChapterOpen())
        {
            acCrews[0].abLocks[1] = false;
            if(u2Level > 14)
                acCrews[0].abLocks[2] = false;
        }
    }
    
    public bool CheckNoticeAlram()
    {
        bool bCheckAlram = false;

        for(int i=0; i<acHeros.Count; i++)
        {
            acHeros[i].GetComponent<StatusComponent>().CountingStatPoint(acHeros[i].cLevel.u2Level);
            if(acHeros[i].GetComponent<StatusComponent>().STAT_POINT > 0)
            {
                bCheckAlram = true;
                break;
            }
            else if(acHeros[i].GetComponent<SkillComponent>().SkillPoint > 0)
            {
                bCheckAlram = true;
                break;
            }
            else
            {
                for(int j=0; j<acHeros[i].acEquips.Length; j++)
                {
                    if(acHeros[i].acEquips[j].GetComponent<StatusComponent>().CheckHaveEquipStatPoint(acHeros[i].acEquips[j].cLevel.u2Level))
                    {
                        bCheckAlram = true;
                        break;
                    }
                    else
                        bCheckAlram = false;
                }
                if(bCheckAlram)
                    break;
            }
        }

        return bCheckAlram;
    }
    
    public void SetServerTime(DateTime serverTime)
    {        
        TimeSpan timeSpan = serverTime - DateTime.Now;
        
        StopCoroutine("CheckTime");
        timeDist = timeSpan.TotalSeconds;
        StartCoroutine("CheckTime");
    }
    
    private IEnumerator CheckTime()
    {
		yield return StartCoroutine(Utillity.WaitForRealSeconds(1f));

        while (true)
        {            
            if(Energy < LegionInfoMgr.Instance.keyTime.MAX_COUNT)
            {
                TimeSpan timeSpan = nextEnergyChargeTime - ServerTime;
                
                if(timeSpan.Ticks <= 0)
                {
                    Energy+=1;
                    nextEnergyChargeTime = ServerTime.AddSeconds(LegionInfoMgr.Instance.keyTime.chargeTime);              
                }
            }
            
            if(LeagueKey < LegionInfoMgr.Instance.leagueKeyTime.MAX_COUNT)
            {
                TimeSpan timeSpan = nextLeagueEnergyChargeTime - ServerTime;
             
                if(timeSpan.Ticks <= 0)
                {
                    LeagueKey+=1;               
                    nextLeagueEnergyChargeTime = ServerTime.AddSeconds(LegionInfoMgr.Instance.leagueKeyTime.chargeTime);     
                }
            }    

			for (int i = 0; i < adRemainCount.Length; i++) {
				if (adRemainCount [i] > 0 && adLeftTime[i] > 0) {
					adLeftTime [i] -= 1;
				}
			}
                                          
            yield return StartCoroutine(Utillity.WaitForRealSeconds(1f));
        }
    }
    public TimeSpan tsConnectTime;
    public DateTime dtPrevConnectTime;
    public void ConnectTimeCount()
    {
        StartCoroutine("CheckConnentTime");
		//AddLocalNotifications (PushEventID.TimeReward);
    }
    public void StopConnectTimeCount()
    {
        StopCoroutine("CheckConnentTime");
		CancelLocalNotifications (PushEventID.TimeReward);
    }
    private IEnumerator CheckConnentTime()
    {
        //DebugMgr.LogError(tsConnectTime.Seconds);
        while (true)
        {
            if(tsConnectTime.TotalSeconds == 0)
            {
                LobbyScene _scene = Scene.GetCurrent() as LobbyScene;
                if(_scene != null)
                    _scene._eventPanel.CheckAlarm();
                yield break;
            }
            tsConnectTime = tsConnectTime.Subtract(TimeSpan.FromSeconds(1f));
            //tsConnectTime = TimeSpan.FromSeconds(ServerTime.Second).Subtract(TimeSpan.FromSeconds(1f));
            //DebugMgr.LogError(tsConnectTime.Seconds);
            //DebugMgr.LogError(tsConnectTime.Duration());
            yield return StartCoroutine(Utillity.WaitForRealSeconds(1f));
        }
    }

    public void SortAllHero()
    {
        lstSortedChar = new List<Hero>();
        for(int i=0; i<acHeros.Count; i++)
            lstSortedChar.Add(acHeros[i]);
        lstSortedChar.Sort(delegate(Hero x, Hero y) 
            {
                int compare = 0;

                //레벨
                if(compare == 0)
                    compare = y.cLevel.u2Level.CompareTo(x.cLevel.u2Level);
                //클래스
                if(compare == 0)
                    compare = y.cClass.u2ID.CompareTo(x.cClass.u2ID);
                return compare;
            });
    }

    float timeBackground;
	List<int> dicPush = new List<int>();
    
    void OnApplicationQuit()
    {
		PlayerPrefs.SetInt ("ApplicationQuit", 1);

		if (googleAnalytics != null) {
			googleAnalytics.Dispose ();
			googleAnalytics.StopSession ();
		}
        PushSetting(true);
    }
    
    void DeepLinkCallback(IResult result)
    {
        Debug.Log("DeepLinkCallback call ");
        DebugMgr.LogError("딥링크 콜");
        if (result != null && !string.IsNullOrEmpty(result.RawResult))
        {
            try
            {
                DebugMgr.LogError("딥링크 호출");
                Dictionary<string, object> jsonObjects = MiniJson.Deserialize(result.RawResult) as Dictionary<string, object>;
                string extras = (string)jsonObjects["extras"];
                Dictionary<string, object> extrasJSON = MiniJson.Deserialize(extras) as Dictionary<string, object>;
                string nativeURL = (string)extrasJSON["com.facebook.platform.APPLINK_NATIVE_URL"];                   
                // Report deeplink for IgawCommon to get tracking parameters
                // IgaworksUnityPluginAOS.Common.setReferralUrl(nativeURL);
                // setReferralUrl 이 setReferralUrlForFacebook 으로 변경됩니다.
                IgaworksUnityPluginAOS.Common.setReferralUrlForFacebook(nativeURL);
            }
            catch (Exception e)
            {
                DebugMgr.LogError("딥링크 에러");
                Debug.Log("Error: " + e.Message);
            }               
        }          
    }

    void DeepLinkCallbackIOS(IAppLinkResult result)
    {
        if (!String.IsNullOrEmpty(result.Url))
        {
            IgaworksCorePluginIOS.SetReferralUrl(result.Url);
        }    
    }

    void OnApplicationPause(bool pause)
    {
        PushSetting(pause);
    }

    public void PushSetting(bool isPause)
    {
        if (isPause)
        {
            dtPrevConnectTime = DateTime.Now;
#if UNITY_ANDROID
            IgaworksUnityPluginAOS.Common.endSession();
#elif UNITY_IOS
			Vungle.onPause ();
#endif
        }
        else
        {
            TimeSpan nowTime = DateTime.Now - dtPrevConnectTime;
            if (nowTime.Seconds >= 3600)
                tsConnectTime = TimeSpan.FromSeconds(0f);
            else
            {
                if ((tsConnectTime.TotalSeconds - nowTime.TotalSeconds) <= 0)
                    tsConnectTime = TimeSpan.FromSeconds(0f);
                else
                    tsConnectTime = TimeSpan.FromSeconds((int)(tsConnectTime.TotalSeconds - nowTime.TotalSeconds));
            }
#if UNITY_ANDROID
            IgaworksUnityPluginAOS.Common.startSession();
            IgaworksUnityPluginAOS.LiveOps.resume();
#elif UNITY_IOS
			Vungle.onResume ();
#endif
        }

        // 2017. 01. 07 jy
        // on off를 바로 적용하여 다음 푸시 인덱스부터 체크하기로 시작한다
        for (int i = 0; i < MAX_PUSH_EVENT_ID; ++i)
        {
            // 빽그라운드 및 어플 종료시
            if (isPause == true)
            {
                if (pushActive[i] == true)
                {
                    timeBackground = Time.realtimeSinceStartup;
                    if (bLoaded)
                        AddLocalNotifications(i);
                }
                else
                {
                    CancelLocalNotifications(i);
                    if (osLogin)
                        ConnectGameService();
                }
            }
            else
            {
                CancelLocalNotifications(i);
                if (osLogin)
                    ConnectGameService();
            }
        }
    }

    public void AddLocalNotifications(int pushEventIdx)
	{
		switch(pushEventIdx)
        {
            case 0:
                AddLocalNotifications(PushEventID.Shop);
                AddLocalNotifications(PushEventID.BlackShop);
                AddLocalNotifications(PushEventID.TimeReward);
                break;
            case 1:
                AddLocalNotifications(PushEventID.Dispatch);
                AddLocalNotifications(PushEventID.Training);
                break;
            case 2:
                AddLocalNotifications(PushEventID.MaxKey);
                //아침 점심 저녁 키 보상은 별도로 셋팅한다
                //AddLoaclPushTimeKeyReward();
                break;
		//case 1:
		//	AddLocalNotifications(PushEventID.Dispatch);
		//	break;
		//case 2:
		//	AddLocalNotifications(PushEventID.Shop);
		//	break;
		//case 3:
		//	AddLocalNotifications(PushEventID.BlackShop);
		//	break;
		//case 4:
		//	AddLocalNotifications(PushEventID.Training);
		//	break;
		//case 5:
		//	AddLocalNotifications(PushEventID.MaxKey);
		//	break;
		//case 6:
		//	AddLocalNotifications(PushEventID.TimeReward);
		//	break;
		}
	}

	public void AddLocalNotifications(Legion.PushEventID pID)
	{
		switch(pID)
		{
		case PushEventID.Dispatch:
			AddLocalPushDispatch();
			break;
		case PushEventID.Shop:
			AddLocalPushShop();
			break;
		case PushEventID.BlackShop:
			AddLocalPushBlack();
			break;
		case PushEventID.Training:
			AddLocalPushCharTraining();
			break;
		case PushEventID.MaxKey:
			AddLocalPushEquipKey();
			break;
		case PushEventID.TimeReward:
			AddLocalPushTimeReward();
			break;
		}
	}

	public void CancelAllLocalNotifications()
	{
		//IgaworksUnityPluginAOS.LiveOps.cancelClientPushEvent ();
		for (int i = 0; i < dicPush.Count; i++) {
			IgaworksUnityPluginAOS.LiveOps.cancelClientPushEvent (dicPush [i]);
		}
		dicPush.Clear();
	}
	
	public void CancelLocalNotifications(int pIdIdx)
	{
		switch(pIdIdx)
		{
            case 0:
                CancelLocalNotifications(PushEventID.Shop);
                CancelLocalNotifications(PushEventID.BlackShop);
                CancelLocalNotifications(PushEventID.TimeReward);
                break;
            case 1:
                CancelLocalNotifications(PushEventID.Dispatch);
                CancelLocalNotifications(PushEventID.Training);
                break;
            case 2:
                CancelLocalNotifications(PushEventID.MaxKey);
                //CloseLoaclPushTimeKeyReward();
                break;
                //case 1:
                //	CancelLocalNotifications(PushEventID.Dispatch);
                //	break;
                //case 2:
                //	CancelLocalNotifications(PushEventID.Shop);
                //	break;
                //case 3:
                //	CancelLocalNotifications(PushEventID.BlackShop);
                //	break;
                //case 4:
                //	CancelLocalNotifications(PushEventID.Training);
                //	break;
                //case 5:
                //	CancelLocalNotifications(PushEventID.MaxKey);
                //	break;
                //case 6:
                //    CancelLocalNotifications(PushEventID.TimeReward);
                //    break;
        }
	}

	public void CancelLocalNotifications(Legion.PushEventID pId)
	{
		if (dicPush.Contains ((int)pId)) {
		#if UNITY_IOS
			LiveOpsPluginIOS.LiveOpsCancelLocalPush((int)pId);
		#elif UNITY_ANDROID
			IgaworksUnityPluginAOS.LiveOps.cancelClientPushEvent ((int)pId);
		#endif
			dicPush.Remove ((int)pId);
            // 테스트용 
            //DebugMgr.LogError("CancelPush ID = " + pId);
		}
	}
    
    private void AddLocalPushDispatch()
    {
        for(int i=0; i<acCrews.Length; i++)
        {
            if(acCrews[i].DispatchStage != null)
            {
				//if(!dicPush.Contains((int)acCrews[i].u1Index))
				if(!dicPush.Contains((int)PushEventID.Dispatch))
                {                
                    TimeSpan timeSpan = acCrews[i].DispatchTime - ServerTime;
                    
                    if(timeSpan.TotalSeconds > 0)
                    {
                        string title = TextManager.Instance.GetText("popup_title_dispatch_done");
                        string msg = TextManager.Instance.GetText("push_dispatch");
					#if UNITY_ANDROID
						IgaworksUnityPluginAOS.LiveOps.setNormalClientPushEvent ((int)(timeSpan.TotalSeconds), msg, (int)PushEventID.Dispatch, false);
					#elif UNITY_IOS
						LiveOpsPluginIOS.LiveOpsRegisterLocalPushNotification((int)PushEventID.Dispatch, acCrews[i].DispatchTime.ToString("yyyyMMddHHmmss"), msg, title, null, 0, null);
					#endif
                        //UM_NotificationController.Instance.ScheduleLocalNotification(title, msg, (int)(timeSpan.TotalSeconds));
						dicPush.Add((int)PushEventID.Dispatch);
                        //DebugMgr.Log("파견 푸쉬 등록");
                    }
                }
            }
        }
    }
    
    private void AddLocalPushShop()
    { 
		if(dicPush.Contains((int)PushEventID.Shop))
            return;
        
        ShopInfo normalShop = ShopInfoMgr.Instance.dicShopData[1];
        
        DateTime normalRefreshTime = DateTime.MinValue;
        
        for(int i=0; i<normalShop.u1ArrResetTime.Length; i++)
        {
            if(ServerTime < Legion.Instance.ServerTime.Date.AddHours(normalShop.u1ArrResetTime[i]))
            {
                normalRefreshTime = Legion.Instance.ServerTime.Date.AddHours(normalShop.u1ArrResetTime[i]);
            }
        }
        
        if(normalRefreshTime == DateTime.MinValue)
        {
            normalRefreshTime = Legion.Instance.ServerTime.Date.AddHours(24 + normalShop.u1ArrResetTime[0]);
        }
        
        TimeSpan timeSpan = normalRefreshTime - Legion.Instance.ServerTime;

        // ShopInfo equipShop = ShopInfoMgr.Instance.dicShopData[1];
        
        // DateTime equipRefreshTime = DateTime.MinValue;
        
        // for(int i=0; i<equipShop.u1ArrResetTime.Length; i++)
        // {
        //     if(serverTime < Legion.Instance.ServerTime.Date.AddHours(equipShop.u1ArrResetTime[i]))
        //     {
        //         equipRefreshTime = Legion.Instance.ServerTime.Date.AddHours(equipShop.u1ArrResetTime[i]);
        //     }
        // }
        
        // TimeSpan timeSpan2 = equipRefreshTime - Legion.Instance.serverTime;
        
        // int second = (timeSpan.TotalSeconds > timeSpan2.TotalSeconds) ? (int)timeSpan.TotalSeconds : (int)timeSpan2.TotalSeconds;
        
        if(timeSpan.TotalSeconds > 0)
        {
            string title = TextManager.Instance.GetText("popup_title_refreash");
            string msg = TextManager.Instance.GetText("push_shop_reset");
		#if UNITY_ANDROID
			IgaworksUnityPluginAOS.LiveOps.setNormalClientPushEvent ((int)(timeSpan.TotalSeconds), msg, (int)PushEventID.Shop, false);
		#elif UNITY_IOS
			LiveOpsPluginIOS.LiveOpsRegisterLocalPushNotification((int)PushEventID.Shop, normalRefreshTime.ToString("yyyyMMddHHmmss"), msg, title, null, 0, null);
		#endif
            //UM_NotificationController.Instance.ScheduleLocalNotification(title, msg, (int)(timeSpan.TotalSeconds));
			dicPush.Add((int)PushEventID.Shop);
            //DebugMgr.Log("상점 푸쉬 등록");
        }        
    }    
    
    private void AddLocalPushBlack()
    {
        if(ShopInfoMgr.Instance.lastShopTime >= ServerTime)
            return;           
            
        if((BLACK_SHOP_STATE)Legion.Instance.u1BlackMarketOpen > BLACK_SHOP_STATE.OPEN)
            return;
        
		if(dicPush.Contains((int)PushEventID.BlackShop))
            return;
        
		TimeSpan timeSpan = Legion.Instance.BlackMarketLeftTime - ServerTime;
        
        if(timeSpan.TotalSeconds > 0)
        {
            string title = TextManager.Instance.GetText("popup_title_shop_buy_black");
            string msg = TextManager.Instance.GetText("push_open_blackmarket");
		#if UNITY_ANDROID
			IgaworksUnityPluginAOS.LiveOps.setNormalClientPushEvent ((int)(timeSpan.TotalSeconds), msg, (int)PushEventID.BlackShop, false);
		#elif UNITY_IOS
			LiveOpsPluginIOS.LiveOpsRegisterLocalPushNotification((int)PushEventID.BlackShop, Legion.Instance.BlackMarketLeftTime.ToString("yyyyMMddHHmmss"), msg, title, null, 0, null);
		#endif
           // UM_NotificationController.Instance.ScheduleLocalNotification(title, msg, (int)timeSpan.TotalSeconds);
			dicPush.Add((int)PushEventID.BlackShop); 
        }                 
    }    
    
    private void AddLocalPushCharTraining()
    {
       Dictionary<UInt16, CharTrainingInfo> dicCharTraining = QuestInfoMgr.Instance.GetCharTrainingInfo(); 
        
       foreach(CharTrainingInfo info in dicCharTraining.Values)
        {
            //if(dicPush.Contains(info.u2ID))
			if(dicPush.Contains((int)PushEventID.Training))
                continue;
            
            if(info.timeType > 0)
            {
                TimeSpan timeSpan = info.doneTime - Legion.Instance.ServerTime;
                
                if(timeSpan.TotalSeconds > 0)
                {
                    string title = TextManager.Instance.GetText("btn_guild_main_tra_char");
                    string msg = TextManager.Instance.GetText("push_char_traning");
				#if UNITY_ANDROID
					IgaworksUnityPluginAOS.LiveOps.setNormalClientPushEvent ((int)(timeSpan.TotalSeconds), msg, (int)PushEventID.Training, false);
				#elif UNITY_IOS
					LiveOpsPluginIOS.LiveOpsRegisterLocalPushNotification((int)PushEventID.Training, info.doneTime.ToString("yyyyMMddHHmmss"), msg, title, null, 0, null);
				#endif
                   	//UM_NotificationController.Instance.ScheduleLocalNotification(title, msg, (int)timeSpan.TotalSeconds);
					dicPush.Add((int)PushEventID.Training);
                    //DebugMgr.Log("수련의 방 푸쉬 등록");
                }
            } 
        }        
    }      
    
    private void AddLocalPushEquipKey()
    {
		if(dicPush.Contains((int)PushEventID.MaxKey))
            return;
        
        if(Energy < LegionInfoMgr.Instance.keyTime.MAX_COUNT)
        {
            int count = (int)(LegionInfoMgr.Instance.keyTime.MAX_COUNT - Energy); 
            
            TimeSpan timeSpan = ServerTime.AddSeconds(LegionInfoMgr.Instance.keyTime.chargeTime * count) - ServerTime;
            
            string title = TextManager.Instance.GetText("popup_title_key_fill_up");
            string msg = TextManager.Instance.GetText("push_key_max");
		#if UNITY_ANDROID
			IgaworksUnityPluginAOS.LiveOps.setNormalClientPushEvent ((int)(timeSpan.TotalSeconds), msg, (int)PushEventID.MaxKey, false);
		#elif UNITY_IOS
			LiveOpsPluginIOS.LiveOpsRegisterLocalPushNotification((int)PushEventID.MaxKey, ServerTime.AddSeconds(LegionInfoMgr.Instance.keyTime.chargeTime * count).ToString("yyyyMMddHHmmss"), msg, title, null, 0, null);
		#endif
            //UM_NotificationController.Instance.ScheduleLocalNotification(title, msg, (int)timeSpan.TotalSeconds);
            dicPush.Add((int)PushEventID.MaxKey);
//            DebugMgr.Log("열쇠 충전 푸쉬 등록");                                   
        }
    }

	private void AddLocalPushTimeReward()
	{
		if(dicPush.Contains((int)PushEventID.TimeReward))
		    return;

		if(Legion.Instance.tsConnectTime.TotalSeconds > 0)
		{
			string title = TextManager.Instance.GetText("popup_title_time_reward");
			string msg = TextManager.Instance.GetText("push_time_reward");
		#if UNITY_ANDROID
			IgaworksUnityPluginAOS.LiveOps.setNormalClientPushEvent ((int)(Legion.Instance.tsConnectTime.TotalSeconds), msg, (int)PushEventID.TimeReward, false);
		#elif UNITY_IOS
			LiveOpsPluginIOS.LiveOpsRegisterLocalPushNotification((int)PushEventID.TimeReward, DateTime.Now.AddSeconds(Legion.Instance.tsConnectTime.TotalSeconds).ToString("yyyyMMddHHmmss"), msg, title, null, 0, null);
		#endif
			//UM_NotificationController.Instance.ScheduleLocalNotification(title, msg, (int)timeSpan.TotalSeconds);
			dicPush.Add((int)PushEventID.TimeReward);
			//DebugMgr.Log("시간 보상 푸쉬 등록");                                   
		}
	}

    // 오전 오후 저녁에 열쇠 푸쉬 셋팅
    public void AddLoaclPushTimeKeyReward()
    {
        string pushMsg;
        for (int i = 0; i < 3; ++i)
        {
            // 인덱스에 따라 시간때별 메시지를 셋팅한다
            if (i == 0)
                pushMsg = TextManager.Instance.GetText("push_key_morning");
            else if (i == 1)
                pushMsg = TextManager.Instance.GetText("push_key_noon");
            else
                pushMsg = TextManager.Instance.GetText("push_key_evening");

            // 업적에서 받을 보상의 시간을 받기 위하여 정보를 불러온다
            AchievementInfo achieveInfo = cQuest.dicAchievements[(UInt16)(50013 + i)].GetInfo();
            // 푸시까지 남은시간을 확인한ㄷ
            TimeSpan gapTime = new DateTime().AddHours(achieveInfo.u1StartTime) - new DateTime().AddHours(ServerTime.Hour).AddMinutes(ServerTime.Minute).AddSeconds(ServerTime.Second);
            //오늘의 남은 타입 접속 체크 후 푸쉬를 셋팅합니다.
            if (gapTime.Ticks > 0)
            {
                // 푸쉬가 셋팅 안되어 있다면 셋팅한다
                int pushKey = (int)(PushEventID.ToDayAccess_M) + i;
                if (!dicPush.Contains(pushKey))
                {
                    // 푸쉬 셋팅
                    TimeSpan timeSpan = ServerTime.Add(gapTime) - ServerTime;
				#if UNITY_ANDROID
                    IgaworksUnityPluginAOS.LiveOps.setNormalClientPushEvent((int)(timeSpan.TotalSeconds), pushMsg, pushKey, false);
				#elif UNITY_IOS
					LiveOpsPluginIOS.LiveOpsRegisterLocalPushNotification((int)pushKey, DateTime.Today.AddHours(achieveInfo.u1StartTime).ToString("yyyyMMddHHmmss"), pushMsg, TextManager.Instance.GetText(achieveInfo.sName), null, 0, null);
				#endif
				     //DebugMgr.LogError("오늘 " + pushMsg + "= " + ServerTime.Add(timeSpan) + "초 =" + timeSpan.TotalSeconds);
                    dicPush.Add(pushKey);
                }
            }
			
            // 내일과 다음날 것을 셋팅한다
            for (int j = 0; j < 2; ++j)
            {
                int pushKey = (int)PushEventID.TomorrowAccess_M + (j * 100 + i);
                if (dicPush.Contains(pushKey))
                    continue;

                TimeSpan timeSpan = ServerTime.AddDays((j + 1)).Add(gapTime) - ServerTime;
			#if UNITY_ANDROID
				IgaworksUnityPluginAOS.LiveOps.setNormalClientPushEvent((int)(timeSpan.TotalSeconds), pushMsg, pushKey, false);
			#elif UNITY_IOS
				LiveOpsPluginIOS.LiveOpsRegisterLocalPushNotification((int)pushKey, DateTime.Today.AddDays((j + 1)).AddHours(achieveInfo.u1StartTime).ToString("yyyyMMddHHmmss"), pushMsg, TextManager.Instance.GetText(achieveInfo.sName), null, 0, null);
			#endif
                
                //DebugMgr.LogError(i+"째날 " + pushMsg + "= " + ServerTime.Add(timeSpan) + "초 =" + timeSpan.TotalSeconds);
                dicPush.Add(pushKey);
            }
        }
    }

    private void CloseLoaclPushTimeKeyReward()
    {
        for(int i = 0; i < 3; ++i)
        {
            CancelLocalNotifications(PushEventID.ToDayAccess_M + i);
            CancelLocalNotifications(PushEventID.TomorrowAccess_M + i);
            CancelLocalNotifications(PushEventID.AfterTomorrowAccess_M + i);
        }
    }

    public void ConnectGameService()
    {
        if(!bLoaded)
            return;
        
#if UNITY_ANDROID && !UNITY_ONESTORE
		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED) {
			OnPlayerConnected ();
		} else {
			GooglePlayConnection.Instance.Connect ();
		}
#elif UNITY_IOS
		//Social.localUser.Authenticate((bool success) => {});
		if(this.gameObject != null){
			if(UM_GameServiceManager.Instance.ConnectionSate == UM_ConnectionState.CONNECTED){
				OnPlayerConnected ();
			} else {
				UM_GameServiceManager.Instance.Connect();
			}
		}
#endif
    }    
    
    public void UpdateOSAchievement(UInt16 id)
    {
//        DebugMgr.Log(UM_GameServiceManager.Instance.IsConnected);

#if UNITY_ONESTORE
		return;
#endif
        
        if(UM_GameServiceManager.Instance.IsConnected == false)
            return;
        
        UM_Achievement achievement = UltimateMobileSettings.Instance.GetAchievementById(id.ToString());
        
        if(achievement != null)
        {
            float progress = UM_GameServiceManager.Instance.GetAchievementProgress(id.ToString());
            
//            DebugMgr.Log("Progress : " + progress);
            
            if(progress == 100f)
                return;
            
            AchievementInfo achieveInfo = QuestInfoMgr.Instance.GetAchieveInfo(id);
            
            float afterProgress = (cQuest.dicAchievements[achieveInfo.u2ID].GetCount() / (float)achieveInfo.u4MaxCount) * 100f;

			if (afterProgress < 100f)
				return;
            
//            DebugMgr.Log("After Progress : " + progress);
                        
            UM_GameServiceManager.Instance.IncrementAchievement(id.ToString(), afterProgress);   
        }
    }
    
	private void OnPlayerConnected() {
		osLogin = true;
	}
	

	private void OnPlayerDisconnected() {
		osLogin = false;
	}
	    
    
	public void SetUserData()
	{
		cInventory.lstInShop = new Dictionary<ushort, EquipmentItem>();
		if (bLoaded) return;
		sName = "Parabellum";
        //sName = "";
		u2Level = 10;
		u2Energy = 100;
		u4Gold = 10000000;
		u4Cash = 1000000;

		//			cInventory.AddEquipment(0,10104,1,0,1);
		cInventory.AddItem(1,58004,4);
		cInventory.AddItem(2,58002,11);
		cInventory.AddItem(3,58003,5);

		cInventory.AddItem(0,3523,300);
		cInventory.AddItem(0,3539,300);
		cInventory.AddItem(0,3370,300);
		cInventory.AddItem(0,3640,300);

		cInventory.AddItem(0,3563,300);
		cInventory.AddItem(0,3585,300);
		cInventory.AddItem(0,3608,300);

		cInventory.AddItem(0,2023,300);
		cInventory.AddItem(0,2048,300);
		cInventory.AddItem(0,2211,300);
		cInventory.AddItem(0,2233,300);
		cInventory.AddItem(0,2256,300);

		cInventory.AddItem(0,2380,300);
		cInventory.AddItem(0,2402,300);
		cInventory.AddItem(0,2425,300);

		cInventory.AddItem(0,2549,300);
		cInventory.AddItem(0,2571,300);
		cInventory.AddItem(0,2594,300);
		for (int i=0; i<37; i++) {
			cInventory.AddItem ((ushort)(4+i), (ushort)(2001+i), 500);
		}

		Byte[] slots = new Byte[Server.ConstDef.SkillOfEquip];
		UInt32[] stats = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType * 2];

		for(int i=0; i<stats.Length; i++)
		{
			stats[i] = 0;
		}
		UInt16 idx=1;
		u1ForgeLevel = 1;
		acEquipDesign = new BitArray(Server.ConstDef.SizeOfEquipDesignBuffer*8, true);
		acEquipDesignNew = new BitArray(Server.ConstDef.SizeOfEquipDesignBuffer*8, true);
        acEquipDesignMake = new BitArray(Server.ConstDef.SizeOfEquipDesignBuffer*8, true);
		acLookDesign = new BitArray(Server.ConstDef.SizeOfLookDesignBuffer*8, true);
		acLookDesignNew = new BitArray(Server.ConstDef.SizeOfLookDesignBuffer*8, true);
        
		stats = CreateStat(10001);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,10001,10,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(10051);
		slots = CreateSkillSlot(1);
		cInventory.AddEquipment(idx++,0,10051,10,0,slots,stats,0,"","",0,false,1);
		stats = CreateStat(10101);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,10101,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(10151);
		slots = CreateSkillSlot(3);
		cInventory.AddEquipment(idx++,0,10151,1,0,slots,stats,0,"","",0,false,3);
		stats = CreateStat(10201);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,10201,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(10251);
		slots = CreateSkillSlot(4);
		cInventory.AddEquipment(idx++,0,10251,1,0,slots,stats,0,"","",0,false,4);
		stats = CreateStat(10302);
		slots = CreateSkillSlot(5);
		cInventory.AddEquipment(idx++,0,10302,1,0,slots,stats,0,"","",0,false,5);
		stats = CreateStat(10351);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,10351,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(10401);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,10401,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(10451);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,10451,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(10501);
		slots = CreateSkillSlot(8);
		cInventory.AddEquipment(idx++,0,10501,1,0,slots,stats,0,"","",0,false,8);
		stats = CreateStat(10551);
		slots = CreateSkillSlot(7);
		cInventory.AddEquipment(idx++,0,10551,1,0,slots,stats,0,"","",0,false,7);
		stats = CreateStat(10601);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,10601,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(10651);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,10651,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(10701);
		slots = CreateSkillSlot(3);
		cInventory.AddEquipment(idx++,0,10701,1,0,slots,stats,0,"","",0,false,3);
		stats = CreateStat(10751);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,10751,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(10801);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,10801,1,0,slots,stats,0,"","",0,false,2);

		stats = CreateStat(10851);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,10851,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(10901);
		slots = CreateSkillSlot(1);
		cInventory.AddEquipment(idx++,0,10901,1,0,slots,stats,0,"","",0,false,1);
		stats = CreateStat(10951);
		slots = CreateSkillSlot(1);
		cInventory.AddEquipment(idx++,0,10951,1,0,slots,stats,0,"","",0,false,1);
		stats = CreateStat(11001);
		slots = CreateSkillSlot(1);
		cInventory.AddEquipment(idx++,0,11001,1,0,slots,stats,0,"","",0,false,1);
		stats = CreateStat(11051);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,11051,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(11101);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,11101,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(11151);
		slots = CreateSkillSlot(4);
		cInventory.AddEquipment(idx++,0,11151,1,0,slots,stats,0,"","",0,false,4);
		stats = CreateStat(11201);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,11201,1,0,slots,stats,0,"","",0,false,2);

		stats = CreateStat(11251);
		slots = CreateSkillSlot(3);
		cInventory.AddEquipment(idx++,0,11251,1,0,slots,stats,0,"","",0,false,3);
		stats = CreateStat(11301);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,11301,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(11351);
		slots = CreateSkillSlot(7);
		cInventory.AddEquipment(idx++,0,11351,1,0,slots,stats,0,"","",0,false,7);
		stats = CreateStat(11401);
		slots = CreateSkillSlot(6);
		cInventory.AddEquipment(idx++,0,11401,1,0,slots,stats,0,"","",0,false,6);
		stats = CreateStat(11451);
		slots = CreateSkillSlot(5);
		cInventory.AddEquipment(idx++,0,11451,1,0,slots,stats,0,"","",0,false,5);

		stats = CreateStat(11501);
		slots = CreateSkillSlot(2);
		cInventory.AddEquipment(idx++,0,11501,1,0,slots,stats,0,"","",0,false,2);
		stats = CreateStat(11551);
		slots = CreateSkillSlot(1);
		cInventory.AddEquipment(idx++,0,11551,1,0,slots,stats,0,"","",0,false,1);

		List<LearnedSkill> lstLearnInfo = null;
        LearnedSkill temp;
		Hero[] tmpHeroes = new Hero[3];
		tmpHeroes[0] = new Hero(1, 1, "User1", 1, 1, 1);
		UInt32[] heroStats = new UInt32[Server.ConstDef.CharStatPointType];
		tmpHeroes[0].GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
		tmpHeroes[0].GetComponent<LevelComponent>().Set(10, 0);
		//tmpHeroes[0].Wear(new UInt16[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
		tmpHeroes[0].MakeBasicEquipAndWear(1);

		tmpHeroes[0].GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);
		tmpHeroes[1] = new Hero(2, 2, "User2", 1, 1, 1);
		heroStats = new UInt32[Server.ConstDef.CharStatPointType];
		tmpHeroes[1].GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
		tmpHeroes[1].GetComponent<LevelComponent>().Set(10, 0);
		//tmpHeroes[1].Wear(new UInt16[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
		tmpHeroes[1].MakeBasicEquipAndWear(1);

		tmpHeroes[1].GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);

		tmpHeroes[2] = new Hero(3, 4, "chichi23", 1, 1, 1);
		heroStats = new UInt32[Server.ConstDef.CharStatPointType];
		tmpHeroes[2].GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
		tmpHeroes[2].GetComponent<LevelComponent>().Set(10, 0);
		//tmpHeroes[2].Wear(new UInt16[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 });
		tmpHeroes[2].MakeBasicEquipAndWear(1);

		tmpHeroes[2].GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);

		acCrews[0].UnLock();
		acCrews[0].abLocks[1] = false;
		acCrews[0].abLocks[2] = false;
		//acCrews[0].Dispatch(StageInfoMgr.Instance.GetStageInfo(6001), 1 , DateTime.Now.AddSeconds(10));

		AddNewHero(tmpHeroes[0]);
		AddNewHero(tmpHeroes[1]);
		AddNewHero(tmpHeroes[2]);
		cLastUpdatedHero = null;

		cBestCrew = acCrews[0];
		cBestCrew.abLocks[0] = false;
		cBestCrew.Fill(tmpHeroes[0], 0);
		cBestCrew.abLocks[1] = false;
		cBestCrew.Fill(tmpHeroes[1], 1);
		cBestCrew.abLocks[2] = false;
		cBestCrew.Fill(tmpHeroes[2], 2);
		u2SelectStageID = 6001;
		//u2SelectFieldID = 4001;

		cRuneventory.AddItem(4651, 10);
		cRuneventory.AddItem(4661, 10);
		cRuneventory.AddItem(4671, 10);
		cRuneventory.AddItem(4681, 10);
		cRuneventory.AddItem(4691, 10);

		cRuneventory.AddItem(4701, 10);
		cRuneventory.AddItem(4711, 10);
		cRuneventory.AddItem(4721, 10);
		cRuneventory.AddItem(4731, 10);
		cRuneventory.AddItem(4741, 10);

		acCrews[0].SetRunes(new ushort[5]{4651, 4651, 4652, 4661, 0});

		// ObjMgr.Instance.InitMyHero();
		DebugMgr.Log("Legion Init");
		bLoaded = true;

		CreateEnemyCrew ();
		
		Legion.Instance.cTutorial.au1Step = new Byte[9];
        //Legion.Instance.cTutorial.au1Step = new Byte[23];
		for (int i = 0; i < Legion.Instance.cTutorial.au1Step.Length; i++)
		{
			Legion.Instance.cTutorial.au1Step[i] = Server.ConstDef.LastTutorialStep;
		}
        //캐릭터 스텟 및 스킬 테스트용
//		Legion.Instance.cTutorial.au1Step[0] = 11; //11:스테이지 14:대장간 16:상점(비정상) 19:캐릭터
//        Legion.Instance.cTutorial.au1Step[1] = 0;
//        Legion.Instance.cTutorial.au1Step[2] = 0;
        Legion.Instance.cTutorial.au1Step[3] = 0; //파견
//        //크루 튜토리얼 테스트용
//        Legion.Instance.cTutorial.au1Step[4] = 0;
//		Legion.Instance.cTutorial.au1Step[5] = 0; //체인
//		Legion.Instance.cTutorial.au1Step[6] = 0; //콤보
        //StageInfoMgr.Instance.dicStageData[6003].clearState = 3;
		cQuest.InitList();
	}

	public UInt32[] CreateStat(UInt16 equipID)
	{
		UInt32[] stats = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType * 2];

		for(int i=0; i<stats.Length; i++)
		{
			stats[i] = 0;
		}
		stats[0] = (UInt16)(UnityEngine.Random.Range((int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[0].u2BaseStatMin,
			(int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[0].u2BaseStatMax)
			+ ((int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[0].u2AddStatMaxForgeLevel * Legion.Instance.u1ForgeLevel));
		stats[1] = (UInt16)(UnityEngine.Random.Range((int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[1].u2BaseStatMin,
			(int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[1].u2BaseStatMax)
			+ ((int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[1].u2AddStatMaxForgeLevel * Legion.Instance.u1ForgeLevel));
		stats[2] = (UInt16)(UnityEngine.Random.Range((int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[2].u2BaseStatMin,
			(int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[2].u2BaseStatMax)
			+ ((int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[2].u2AddStatMaxForgeLevel * Legion.Instance.u1ForgeLevel));


		return stats;
	}

	public Byte[] CreateSkillSlot(Byte forgeLevel)
	{
		ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[((int)forgeLevel-1)];
		Byte[] skillSlots = new byte[Server.ConstDef.SkillOfEquip];
		for(int i=0; i<forgeInfo.cSmithingInfo.u1RandomSkillCount+forgeInfo.cSmithingInfo.u1SelectSkillCount; i++)
		{
			skillSlots[i] = (Byte)UnityEngine.Random.Range(1, 21);
		}
		return skillSlots;
	}


	public void CreateTutorialCrew(){
		cBestCrew = new Crew();
		
		List<LearnedSkill> lstLearnInfo = null;
		LearnedSkill temp;
		Hero[] tmpHeroes = new Hero[3];
		tmpHeroes[0] = new Hero(1, 2, TextManager.Instance.GetText("char_1_name"), 1, 1, 1);
		UInt32[] heroStats = new UInt32[Server.ConstDef.CharStatPointType];
		tmpHeroes[0].GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
		tmpHeroes[0].GetComponent<LevelComponent>().Set(40, 0);
		//tmpHeroes[0].Wear(new UInt16[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
		tmpHeroes[0].acEquips[0] = new EquipmentItem(10431);
		tmpHeroes[0].acEquips[1] = new EquipmentItem(10481);
		tmpHeroes[0].acEquips[2] = new EquipmentItem(10531);
		tmpHeroes[0].acEquips[3] = new EquipmentItem(10581);
		tmpHeroes[0].acEquips[4] = new EquipmentItem(10631);
		tmpHeroes[0].acEquips[5] = new EquipmentItem(10681);
		tmpHeroes[0].acEquips[6] = new EquipmentItem(10729);
		tmpHeroes[0].acEquips[7] = new EquipmentItem(10780);
		lstLearnInfo = new List<LearnedSkill>();
		temp = new LearnedSkill();
		temp.u1SlotNum = 13;
		temp.u2Level = 1;
		temp.u1UseIndex = 1;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 9;
		temp.u2Level = 1;
		temp.u1UseIndex = 2;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 17;
		temp.u2Level = 1;
		temp.u1UseIndex = 3;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 1;
		temp.u2Level = 1;
		temp.u1UseIndex = 4;
		lstLearnInfo.Add(temp);

		tmpHeroes[0].GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);
		
		
		tmpHeroes[1] = new Hero(2, 3, TextManager.Instance.GetText("char_2_name"), 1, 1, 1);
		heroStats = new UInt32[Server.ConstDef.CharStatPointType];
		tmpHeroes[1].GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
		tmpHeroes[1].GetComponent<LevelComponent>().Set(40, 0);
		//tmpHeroes[1].Wear(new UInt16[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
		tmpHeroes[1].acEquips[0] = new EquipmentItem(10813);
		tmpHeroes[1].acEquips[1] = new EquipmentItem(10863);
		tmpHeroes[1].acEquips[2] = new EquipmentItem(10905);
		tmpHeroes[1].acEquips[3] = new EquipmentItem(10963);
		tmpHeroes[1].acEquips[4] = new EquipmentItem(11013);
		tmpHeroes[1].acEquips[5] = new EquipmentItem(11063);
		tmpHeroes[1].acEquips[6] = new EquipmentItem(11126);
		tmpHeroes[1].acEquips[7] = new EquipmentItem(11176);
		lstLearnInfo = new List<LearnedSkill>();
		temp = new LearnedSkill();
		temp.u1SlotNum = 12;
		temp.u2Level = 1;
		temp.u1UseIndex = 1;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 10;
		temp.u2Level = 1;
		temp.u1UseIndex = 2;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 16;
		temp.u2Level = 1;
		temp.u1UseIndex = 3;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 15;
		temp.u2Level = 1;
		temp.u1UseIndex = 4;
		lstLearnInfo.Add(temp);

		tmpHeroes[1].GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);

		
		tmpHeroes[2] = new Hero(3, 10, TextManager.Instance.GetText("char_3_name"), 2, 2, 1);
		heroStats = new UInt32[Server.ConstDef.CharStatPointType];
		tmpHeroes[2].GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
		tmpHeroes[2].GetComponent<LevelComponent>().Set(40, 0);
		//tmpHeroes[2].Wear(new UInt16[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 });
		tmpHeroes[2].acEquips[0] = new EquipmentItem(13610);
		tmpHeroes[2].acEquips[1] = new EquipmentItem(13660);
		tmpHeroes[2].acEquips[2] = new EquipmentItem(13710);
		tmpHeroes[2].acEquips[3] = new EquipmentItem(13760);
		tmpHeroes[2].acEquips[4] = new EquipmentItem(13810);
		tmpHeroes[2].acEquips[5] = new EquipmentItem(13860);
		tmpHeroes[2].acEquips[6] = new EquipmentItem(13976);
		tmpHeroes[2].acEquips[7] = new EquipmentItem(13926);
		lstLearnInfo = new List<LearnedSkill>();
		temp = new LearnedSkill();
		temp.u1SlotNum = 15;
		temp.u2Level = 1;
		temp.u1UseIndex = 1;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 16;
		temp.u2Level = 1;
		temp.u1UseIndex = 2;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 6;
		temp.u2Level = 1;
		temp.u1UseIndex = 3;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 18;
		temp.u2Level = 1;
		temp.u1UseIndex = 4;
		lstLearnInfo.Add(temp);
		
		tmpHeroes[2].GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);
		
		cBestCrew.UnLock();
		
		cBestCrew.abLocks[0] = false;
		cBestCrew.Fill(tmpHeroes[0], 0);
		cBestCrew.abLocks[1] = false;
		cBestCrew.Fill(tmpHeroes[1], 1);
		cBestCrew.abLocks[2] = false;
		cBestCrew.Fill(tmpHeroes[2], 2);
		
		//cTestEnemyCrew.SetRunes(new ushort[5]{4651, 4661, 4671, 4681, 0});
	}

	public void CreateEnemyCrew(){
		sEnemyName = "FolkaDolka";
		cTestEnemyCrew = new LeagueCrew();

		List<LearnedSkill> lstLearnInfo = null;
		LearnedSkill temp;
		Hero[] tmpHeroes = new Hero[5];
		tmpHeroes[0] = new Hero(1, 1, "Enemy1", 1, 1, 1);
		UInt32[] heroStats = new UInt32[Server.ConstDef.CharStatPointType];
		tmpHeroes[0].GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
		tmpHeroes[0].GetComponent<LevelComponent>().Set(40, 0);
		//tmpHeroes[0].Wear(new UInt16[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
		tmpHeroes[0].acEquips[0] = new EquipmentItem(10001);
		tmpHeroes[0].acEquips[1] = new EquipmentItem(10051);
		tmpHeroes[0].acEquips[2] = new EquipmentItem(10101);
		tmpHeroes[0].acEquips[3] = new EquipmentItem(10151);
		tmpHeroes[0].acEquips[4] = new EquipmentItem(10201);
		tmpHeroes[0].acEquips[5] = new EquipmentItem(10251);
		tmpHeroes[0].acEquips[6] = new EquipmentItem(10301);
		tmpHeroes[0].acEquips[7] = new EquipmentItem(10351);
		lstLearnInfo = new List<LearnedSkill>();
		temp = new LearnedSkill();
		temp.u1SlotNum = 1;
		temp.u2Level = 1;
		temp.u1UseIndex = 1;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 8;
		temp.u2Level = 1;
		temp.u1UseIndex = 2;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 6;
		temp.u2Level = 1;
		temp.u1UseIndex = 3;
		lstLearnInfo.Add(temp);
		tmpHeroes[0].GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);


		tmpHeroes[1] = new Hero(2, 3, "Enemy2", 1, 1, 1);
		heroStats = new UInt32[Server.ConstDef.CharStatPointType];
		tmpHeroes[1].GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
		tmpHeroes[1].GetComponent<LevelComponent>().Set(40, 0);
		//tmpHeroes[1].Wear(new UInt16[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
		tmpHeroes[1].acEquips[0] = new EquipmentItem(10801);
		tmpHeroes[1].acEquips[1] = new EquipmentItem(10851);
		tmpHeroes[1].acEquips[2] = new EquipmentItem(10901);
		tmpHeroes[1].acEquips[3] = new EquipmentItem(10951);
		tmpHeroes[1].acEquips[4] = new EquipmentItem(11001);
		tmpHeroes[1].acEquips[5] = new EquipmentItem(11051);
		tmpHeroes[1].acEquips[6] = new EquipmentItem(11101);
		tmpHeroes[1].acEquips[7] = new EquipmentItem(11151);
		lstLearnInfo = new List<LearnedSkill>();
		temp = new LearnedSkill();
		temp.u1SlotNum = 1;
		temp.u2Level = 1;
		temp.u1UseIndex = 1;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 4;
		temp.u2Level = 1;
		temp.u1UseIndex = 2;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 6;
		temp.u2Level = 1;
		temp.u1UseIndex = 3;
		lstLearnInfo.Add(temp);
		
		tmpHeroes[1].GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);
		
		tmpHeroes[2] = new Hero(3, 4, "Enemy3", 1, 1, 1);
		heroStats = new UInt32[Server.ConstDef.CharStatPointType];
		tmpHeroes[2].GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
		tmpHeroes[2].GetComponent<LevelComponent>().Set(40, 0);
		//tmpHeroes[2].Wear(new UInt16[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 });
		tmpHeroes[2].acEquips[0] = new EquipmentItem(11201);
		tmpHeroes[2].acEquips[1] = new EquipmentItem(11251);
		tmpHeroes[2].acEquips[2] = new EquipmentItem(11301);
		tmpHeroes[2].acEquips[3] = new EquipmentItem(11351);
		tmpHeroes[2].acEquips[4] = new EquipmentItem(11401);
		tmpHeroes[2].acEquips[5] = new EquipmentItem(11451);
		tmpHeroes[2].acEquips[6] = new EquipmentItem(11501);
		tmpHeroes[2].acEquips[7] = new EquipmentItem(11551);
		lstLearnInfo = new List<LearnedSkill>();
		temp = new LearnedSkill();
		temp.u1SlotNum = 1;
		temp.u2Level = 1;
		temp.u1UseIndex = 1;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 5;
		temp.u2Level = 1;
		temp.u1UseIndex = 2;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 18;
		temp.u2Level = 1;
		temp.u1UseIndex = 3;
		lstLearnInfo.Add(temp);
		
		tmpHeroes[2].GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);

		tmpHeroes[3] = new Hero(4, 4, "Enemy4", 1, 1, 1);
		heroStats = new UInt32[Server.ConstDef.CharStatPointType];
		tmpHeroes[3].GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
		tmpHeroes[3].GetComponent<LevelComponent>().Set(40, 0);
		//tmpHeroes[3].Wear(new UInt16[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 });
		tmpHeroes[3].acEquips[0] = new EquipmentItem(11201);
		tmpHeroes[3].acEquips[1] = new EquipmentItem(11251);
		tmpHeroes[3].acEquips[2] = new EquipmentItem(11301);
		tmpHeroes[3].acEquips[3] = new EquipmentItem(11351);
		tmpHeroes[3].acEquips[4] = new EquipmentItem(11401);
		tmpHeroes[3].acEquips[5] = new EquipmentItem(11451);
		tmpHeroes[3].acEquips[6] = new EquipmentItem(11501);
		tmpHeroes[3].acEquips[7] = new EquipmentItem(11551);
		lstLearnInfo = new List<LearnedSkill>();
		temp = new LearnedSkill();
		temp.u1SlotNum = 1;
		temp.u2Level = 1;
		temp.u1UseIndex = 1;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 5;
		temp.u2Level = 1;
		temp.u1UseIndex = 2;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 18;
		temp.u2Level = 1;
		temp.u1UseIndex = 3;
		lstLearnInfo.Add(temp);

		tmpHeroes[3].GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);

		tmpHeroes[4] = new Hero(5, 4, "Enemy5", 1, 1, 1);
		heroStats = new UInt32[Server.ConstDef.CharStatPointType];
		tmpHeroes[4].GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
		tmpHeroes[4].GetComponent<LevelComponent>().Set(40, 0);
		//tmpHeroes[4].Wear(new UInt16[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 });
		tmpHeroes[4].acEquips[0] = new EquipmentItem(11201);
		tmpHeroes[4].acEquips[1] = new EquipmentItem(11251);
		tmpHeroes[4].acEquips[2] = new EquipmentItem(11301);
		tmpHeroes[4].acEquips[3] = new EquipmentItem(11351);
		tmpHeroes[4].acEquips[4] = new EquipmentItem(11401);
		tmpHeroes[4].acEquips[5] = new EquipmentItem(11451);
		tmpHeroes[4].acEquips[6] = new EquipmentItem(11501);
		tmpHeroes[4].acEquips[7] = new EquipmentItem(11551);
		lstLearnInfo = new List<LearnedSkill>();
		temp = new LearnedSkill();
		temp.u1SlotNum = 1;
		temp.u2Level = 1;
		temp.u1UseIndex = 1;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 5;
		temp.u2Level = 1;
		temp.u1UseIndex = 2;
		lstLearnInfo.Add(temp);
		temp = new LearnedSkill();
		temp.u1SlotNum = 18;
		temp.u2Level = 1;
		temp.u1UseIndex = 3;
		lstLearnInfo.Add(temp);

		tmpHeroes[4].GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);
        
		Character outChar;

		cTestEnemyCrew.Assign(tmpHeroes[0], 0, out outChar);
		cTestEnemyCrew.Assign(tmpHeroes[1], 1, out outChar);
		cTestEnemyCrew.Assign(tmpHeroes[2], 2, out outChar);
		cTestEnemyCrew.Assign(tmpHeroes[3], 3, out outChar);
		cTestEnemyCrew.Assign(tmpHeroes[4], 4, out outChar);
	}

    public bool AddVIPPoint(UInt32 addPoint)
    {
        //int checkLevel = 0;
        u4VIPPoint += addPoint;

        if (cTutorial.au1Step[0] != Server.ConstDef.LastTutorialStep)
            return false;

        // 자동 레벨업 할 필요가 없어짐
        //checkLevel = RecursiveCheckOdinLevel((int)u1VIPLevel, u4VIPPoint);
        //if (checkLevel > u1VIPLevel)
        //{
        //    m_bVIPLevelUp = true;
        //    //[예외 처리] 변경해야 할 Vip Level이 Max Level 보다 크다면 Max 레벨로 강제 변환
        //    if (checkLevel > LegionInfoMgr.Instance.dicVipData.Count - 1)
        //    {
        //        checkLevel = LegionInfoMgr.Instance.dicVipData.Count - 1;
        //        if (u1VIPLevel >= checkLevel)
        //        {
        //            m_bVIPLevelUp = false;
        //        }
        //    }
        //}
        return m_bVIPLevelUp;
    }

    public Byte VipLevelUpUnlockSystem()
    {
        UInt32 checkPoint = Legion.Instance.u4VIPPoint;
        Byte checkLevel = (Byte)RecursiveOdinLevel((int)u1VIPLevel, ref checkPoint);
        VipInfo vipInfo = LegionInfoMgr.Instance.GetVipInfo((Byte)checkLevel);
        // [예외처리]
        if (vipInfo == null)
        {
            m_bVIPLevelUp = false;
            DebugMgr.LogError("Update VipInfo null");
        }

        u1VIPLevel = checkLevel;    // 레벨 변경
        u4VIPPoint = checkPoint;    // 포인트 변경
        // 암시장 오픈
        if (vipInfo.bOpenBlackMarket)
            Legion.Instance.u1BlackMarketOpen = (Byte)BLACK_SHOP_STATE.VIP_OPEN;
        // 탐색의 숲 오픈
        if (vipInfo.bOpenForest)
            StageInfoMgr.Instance.OpenForestElement = StageInfo.Forest.ELEMENT_ALL;
        // 캐릭터 오픈
        for (int i = 0; i < vipInfo.u1Level; ++i)
        {
            VipInfo vipData = LegionInfoMgr.Instance.GetVipInfo((Byte)(i + 1));
            if (vipData == null)
                break;

            // 오픈 클래스 아이디가 0이 아니라면 클래스를 해제한다
            if (vipData.u2OpenClassID == 0)
                continue;

            // 현재 캐릭터가 잠겨 있다면 추가 한다
            if (CheckClassAvailable(vipData.u2OpenClassID) == 0)
                AddGoods(new Goods((Byte)GoodsType.CHARACTER_AVAILABLE, vipData.u2OpenClassID, 1));
        }

        return checkLevel;
    }

    // 현 포인트로 도달 레벨을 체크하기 위한 변수
    public int RecursiveCheckOdinLevel(int checkLevel, UInt32 checkPoint)
    {
        VipInfo vipInfo = LegionInfoMgr.Instance.GetVipInfo((Byte)(checkLevel + 1));
        if (vipInfo == null)
            return checkLevel;

        if (checkLevel <= 18 && vipInfo.cUnlockGoods.u4Count <= checkPoint)
        {
            checkPoint -= vipInfo.cUnlockGoods.u4Count;
            return RecursiveCheckOdinLevel(checkLevel + 1, checkPoint);
        }
        return checkLevel;
    }

    // 포인트로 도달 레벨을 반환하며 남은 경험치를 반환 위한 변수
    public int RecursiveOdinLevel(int checkLevel, ref UInt32 checkPoint)
    {
        VipInfo vipInfo = LegionInfoMgr.Instance.GetVipInfo((Byte)(checkLevel + 1));
        if (vipInfo == null)
            return checkLevel;

        if (checkLevel <= 18 && vipInfo.cUnlockGoods.u4Count <= checkPoint)
        {
            checkPoint -= vipInfo.cUnlockGoods.u4Count;
            return RecursiveOdinLevel(checkLevel + 1 , ref checkPoint);
        }
        return checkLevel;
    }
    
    public int getSubVIPTrainingTime(UInt16 _time , bool _bEquip)
    {
        int addTime = 0;
        if (_bEquip)
        {
            addTime = (_time * LegionInfoMgr.Instance.GetCurrentVIPInfo().u1ReduceEquipTrPer) / 100;
        }
        else
        {
            addTime = (_time * LegionInfoMgr.Instance.GetCurrentVIPInfo().u1ReduceCharTrPer) / 100;
        }
        return (int)(_time - addTime);
    }

    public int getSubVIPDispatchTime(UInt32 _time , bool _bSecond = false)
    {
        if (_bSecond == false)
			_time = _time * 60;

		return (int)(_time - (_time * LegionInfoMgr.Instance.GetCurrentVIPInfo().u1ReduceDispatchPer / 100));
    }

    public void SetPushSetting(Byte pushCode)
    {
        pushSetting = pushCode;
        for (int i = 0; i < MAX_PUSH_EVENT_ID; ++i)
        {
            Byte hexCode = (Byte)(0x01 << i + 1);
            bool isActive = (pushSetting & hexCode) == hexCode;
            pushActive[i] = isActive;
            ObscuredPrefs.SetBool("pushActive" + i, isActive);
        }
    }

    public void SetPushActive(int idx, bool isActive)
	{
		pushActive[idx] = isActive;
		ObscuredPrefs.SetBool("pushActive" + idx, isActive);
    }

	public void SetPushAtiveList(bool isActive)
	{
        for (int i = 0; i < MAX_PUSH_EVENT_ID; ++i)
        {
            if (isActive == true)
                pushSetting |= (Byte)(0x01 << i + 1);
            else
            {
                Byte hexCode = (Byte)(0x01 << i + 1);
                if ((pushSetting & hexCode) == hexCode)
                    pushSetting -= hexCode;
            }
            SetPushActive(i, isActive);
        }
	}

	public void ShowCafeOnce(){
        //if (!Legion.Instance.cTutorial.bIng && Legion.Instance.checkLoginAchievement == 1) {
        //	Legion.Instance.cQuest.UpdateLoginAchievement ();
        //	return;
        //}

        if (Legion.Instance.cTutorial.bIng == true)
            return;

        //if (!Legion.Instance.cTutorial.bIng && !Legion.Instance.bCafeView) {
#if !UNITY_EDITOR
			GLink.sharedInstance ().executeHome ();
			//Legion.Instance.bCafeView = true;
#elif UNITY_EDITOR
            //DebugMgr.LogError("대용! 이벤트 창오픈!");
#endif
            SubLoginPopupStep(LoginPopupStep.NAVER_CAFE);
        //}
    }
}