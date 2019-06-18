using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public enum GameStyle
{
	None,
	Stage,
	League,
	Guild,
    ReloadLobby,
	AnimTest
}

public class Battle : MonoBehaviour
{

	public enum BATTLE_STATE
	{
		None,
		Load,
		BossLoad,
		ShowStartDirection,
		ShowStartMsg,
		MoveToNext,
		Ready,
		Start,
		Battle,
		PhaseClear,
		End,
		Result,
		Pause
	}
	public enum BATTLE_RESULT_TYPE
	{
		AllKilled = 0,
		KillAll = 1,
		TimeOver = 2
	}
	public enum BROADCAST_EVENT_TYPE
	{
		Load,
		StartPhase,
		GroupRespawn
	}
	
	public StageInfo cBattleStage;
	public FieldInfo cCurrentBattleField;
	public Reward cBattleReward;
	public BattleCrew[] acCrews;
	private BattleCrew[] acEnemys;
	public BATTLE_STATE eBattleState
	{
		get { return ePhaseState; }
		set
		{
			if (ePhaseState == value)
			{
				DebugMgr.Log("set same phase :" + value);
				return;
			}
			//DebugMgr.Log(ePhaseState + "->" + value);
			ePhaseState = value;
			bPhaseFirst = true;
		}
	}
	public Byte u1PhaseNo;
	public Byte u1Round = 1;
	public Byte u1UserCrewIndex = 0;
	public Byte u1EnemyCrewIndex = 0;
	public bool bSetting;
	public float fStateDelay;
	public float phaseLimitTime;
	//아이템박스에 사운드 추가를 위한 오디오클립
	public AudioClip itemDropAudio;
	// temp var for phase
	BATTLE_STATE ePhaseState = BATTLE_STATE.None;
	bool bPhaseFirst = false;
	
	//	MovingBetweenPhase cMBP;
	
	public BattleUIMgr battleUIMgr;
	
	public Controller cCont;
	
	GameObject pChar;
	GameObject pBox;
	GameObject pCol;
	List<GameObject> lstBox = new List<GameObject>();
	List<VFX> lstBoxEff = new List<VFX>();

	Dictionary<ushort, int> MaxInAllGroup = new Dictionary<ushort, int>();
	
	public CameraMove2 cCameraMove2;
	int curGroup;
	int curIdx;
	int curPosIdx;
	int remainCount;

	string BossName;

	Byte u1Environment = 1;
	Byte u1SkyBox = 0;
	bool bWin = false;

	public AudioListener battleAudioListner;

	public GameStyle eGameStyle;
	public BATTLE_RESULT_TYPE eResultType;

	public Byte TutorialCheckType = 0;
	public int TutorialCheckStep = 0;
	bool bTutorialCheckNeed = false;

	public bool bDirection = false;
	bool bLimitStage = false;

    public SubChatting _subChatWidown;

    void Awake()
	{
		SoundManager.Instance.OnBattleListner (battleAudioListner);
        //#CHATTING
    	if(_subChatWidown != null)
    	{
            if (PopupManager.Instance.IsChattingActive())
            {
                if (Legion.Instance.cTutorial.au1Step[0] > 0)
                {
                    PopupManager.Instance.SetSubChtting(_subChatWidown);
                    _subChatWidown.gameObject.SetActive(true);
                }
                else
                {
                    _subChatWidown.gameObject.SetActive(false);
                }
            }
            else
            {
                _subChatWidown.gameObject.SetActive(false);
            }
    	}
		
		VFXMgr.Instance.Init();

		//QualitySettings.vSyncCount = 0;
		//Application.targetFrameRate = 5;
		cCameraMove2 = Camera.main.GetComponent<CameraMove2>();
		if(cCameraMove2 != null) cCameraMove2.cBattle = this;
		cCont = GetComponent<Controller>();
		cCont.cBattle = this;
		
		pChar = AssetMgr.Instance.AssetLoad("Prefabs/Models/Character.prefab", typeof(GameObject)) as GameObject;
		pBox = AssetMgr.Instance.AssetLoad("Prefabs/Models/Etc/ItemBox.prefab", typeof(GameObject)) as GameObject;
		pCol = AssetMgr.Instance.AssetLoad("Prefabs/Models/AnimCollider.prefab", typeof(GameObject)) as GameObject;
	}

	public GameObject GetAnimColWithCreate(){
		return Instantiate (pCol);
	}
	
	void Start()
	{
		//Screen.SetResolution(1280, 720, true);
		// load info
		eGameStyle = Legion.Instance.eGameStyle;
		eBattleState = BATTLE_STATE.Load;
		OnLoadFinished();
	}
	
	void ChangeSkyBox(Byte u1Type, Byte u1Type2){
		if (u1Type > 0) {
			if (u1Environment != u1Type) {
				u1Environment = u1Type;
				cCameraMove2.SetEnviroment (u1Type);
			}
		}

		if (u1Type2 > 0) {
			if (u1SkyBox != u1Type2) {
				u1SkyBox = u1Type2;
				FogInfo temp = StageInfoMgr.Instance.dicFogData[u1SkyBox];
				RenderSettings.fogColor = temp.color;
				RenderSettings.fogMode = temp.mode;
				if(temp.mode == FogMode.Exponential || temp.mode == FogMode.ExponentialSquared){
					RenderSettings.fogDensity = temp.density;
				}else{
					RenderSettings.fogStartDistance = temp.start;
					RenderSettings.fogEndDistance = temp.end;
				}

				RenderSettings.skybox = AssetMgr.Instance.AssetLoad("SkyBoxs/" + temp.name +"/"+temp.name + ".mat", typeof(Material)) as Material;
			}
		}
	}

	public void OnLoadFinished()
	{
		bLimitStage = true;
		switch (eGameStyle) {
		case GameStyle.Stage: LoadStage(); break;
		case GameStyle.League: LoadLeague(); break;
		case GameStyle.Guild: LoadGuildBattle (); break;
		}
	}

	void LoadStage()
	{
		cBattleStage = StageInfoMgr.Instance.dicStageData[Legion.Instance.u2SelectStageID];
		u1PhaseNo = 0;

		cCurrentBattleField = cBattleStage.getField(u1PhaseNo);
		phaseLimitTime = (float)cCurrentBattleField.u4PhaseLimitTime;
		if (!Server.ServerMgr.bConnectToServer) {
			Legion.Instance.cReward = new Reward(cBattleStage, 1);
			Legion.Instance.cReward.AddNewRewardByIndex(0,1);
		}

		cBattleReward = Legion.Instance.SelectedReward;
		cBattleReward.SetBoxOutline(cBattleStage);

		curGroup = 0;
		ChangeSkyBox(cBattleStage.acPhases[u1PhaseNo].u1Environment, cBattleStage.acPhases[u1PhaseNo].u1SkyBox);
		InitCrew();
		InitPortal();

		InitCharacterObject(acCrews[0], 0, -1);
		battleUIMgr.SetUI(this, acCrews[0]);
		battleUIMgr.SetActive(false);
		GetComponent<Controller>().SetUI();

		if (cBattleStage.u2ID == Tutorial.TUTORIAL_STAGE_ID) {
			acCrews [0].u1ComboElement = 2;
			acCrews [0].u1ComboCount = 3;

			acCrews [0].u2LastDebuff = 3414;//침묵
			acCrews [0].acCharacters [0].FindChainSkill (acCrews [0].u2LastDebuff);

			bossTime = cCameraMove2.SetBossCamera();
			eBattleState = BATTLE_STATE.BossLoad;

			battleUIMgr.DisableAllSkillButton ();

			for(int j=0; j<acCrews[0].acCharacters.Length; j++){
				acCrews [0].acCharacters [j].cObject.SetActive (false);
			}
		} else {
			eBattleState = BATTLE_STATE.ShowStartDirection;
		}
		
		fStateDelay = 0.0f;
		#if UNITY_ANDROID
		IgaworksUnityAOS.IgaworksUnityPluginAOS.Adbrix.retention("StageBattle");
		#elif UNITY_IOS
		AdBrixPluginIOS.Retention("StageBattle");
		#endif
	}

	void LoadLeague(){
		LeagueInfo lInfo = Legion.Instance.SelectedLeague;

		cBattleStage = new StageInfo ();
		cBattleStage.acPhases = new StageInfo.PhaseInfo[1];
		cBattleStage.acPhases [0].u1PhaseNum = 1;
		cBattleStage.acPhases [0].u2FieldID = lInfo.u2FieldID;
		cBattleStage.acPhases [0].u1Environment = lInfo.u1DayOrNight;
		cBattleStage.acPhases [0].u1SkyBox = lInfo.u1SkyBox;
		cCurrentBattleField = lInfo.getField();
		phaseLimitTime = (float)cCurrentBattleField.u4PhaseLimitTime;
		
		u1PhaseNo = 0;
		curGroup = 0;
		ChangeSkyBox(cBattleStage.acPhases[u1PhaseNo].u1Environment, cBattleStage.acPhases[u1PhaseNo].u1SkyBox);
		InitCrew();

		InitCharacterObject(acCrews[0], 0, -1);
		InitCharacterObject(acCrews[1], 1, -1);
		battleUIMgr.SetUI(this, acCrews[0]);
		battleUIMgr.SetActive(false);
		GetComponent<Controller>().SetUI();
		
		eBattleState = BATTLE_STATE.ShowStartDirection;
		
		fStateDelay = 0.0f;
		#if UNITY_ANDROID
		IgaworksUnityAOS.IgaworksUnityPluginAOS.Adbrix.retention("LeagueBattle");
		#elif UNITY_IOS
		AdBrixPluginIOS.Retention("LeagueBattle");
		#endif
	}

	void LoadGuildBattle(){
		cBattleStage = new StageInfo ();
		cBattleStage.acPhases = new StageInfo.PhaseInfo[1];
		cBattleStage.acPhases [0].u1PhaseNum = 1;
		cBattleStage.acPhases [0].u2FieldID = GuildInfoMgr.GuildBattleField;
		cBattleStage.acPhases [0].u1Environment = 1;
		cBattleStage.acPhases [0].u1SkyBox = 1;
		cCurrentBattleField = StageInfoMgr.Instance.GetFieldInfo(GuildInfoMgr.GuildBattleField);
		phaseLimitTime = (float)cCurrentBattleField.u4PhaseLimitTime;

		u1Round = 1;
		u1PhaseNo = 0;
		curGroup = 0;
		ChangeSkyBox(cBattleStage.acPhases[u1PhaseNo].u1Environment, cBattleStage.acPhases[u1PhaseNo].u1SkyBox);
		InitCrew();

		InitCharacterObject(acCrews[0], 0, -1);
		InitCharacterObject(acCrews[1], 1, -1);
		battleUIMgr.SetUI(this, acCrews[0]);
		battleUIMgr.SetActive(false);
		GetComponent<Controller>().SetUI();

		eBattleState = BATTLE_STATE.ShowStartDirection;

		fStateDelay = 0.0f;
		#if UNITY_ANDROID
		IgaworksUnityAOS.IgaworksUnityPluginAOS.Adbrix.retention("LeagueBattle");
		#elif UNITY_IOS
		AdBrixPluginIOS.Retention("LeagueBattle");
		#endif
	}
	
	void InitCrew()
	{
		// 필드 x 필드에 나오는 몬스터 그룹의 수 = 크루의 수.
		// 필드 = 크루.
		
		acCrews = new BattleCrew[2];

		switch (eGameStyle) {
		case GameStyle.Stage: 
			acCrews[0] = new BattleCrew(Legion.Instance.cBestCrew, this, 0);
			InitMonsterCrew();
			break;
		case GameStyle.League:
			acCrews [0] = new BattleCrew (Legion.Instance.cLeagueCrew, this, 0);
            acCrews [1] = new BattleCrew (UI_League.Instance.EnemyCrew, this, 1);
			cCont.cAIController.SetEnemyUserQueue ();
			break;
		case GameStyle.Guild:
			for (byte i = 0; i < GuildInfoMgr.Instance.cUserCrews.Length; i++) {
				if (GuildInfoMgr.Instance.cUserCrews [i].u1Count > 0) {
					u1UserCrewIndex = i;
					break;
				}
			}
				
			for (byte i = 0; i < GuildInfoMgr.Instance.cEnemyCrews.Length; i++) {
				if (GuildInfoMgr.Instance.cEnemyCrews [i].u1Count > 0) {
					u1EnemyCrewIndex = i;
					break;
				}
			}

			acCrews [0] = new BattleCrew (GuildInfoMgr.Instance.cUserCrews [u1UserCrewIndex], this, 0);
			acCrews [1] = new BattleCrew (GuildInfoMgr.Instance.cEnemyCrews[u1EnemyCrewIndex], this, 1);
			cCont.cAIController.SetEnemyUserQueue ();
			break;
		}

		cCont.cAIController.SetUserQueue ();
	}
	
	public void InitMonsterCrew()
	{
		MaxInAllGroup.Clear();

		int crewCount = cBattleStage.getField(u1PhaseNo).u1MonsterGroupCount;
		acEnemys = new BattleCrew[crewCount];

		for(int i=0; i<crewCount; i++){
			acEnemys[i] = new BattleCrew(Legion.Instance.SelectedStage.getField(u1PhaseNo), this, i);
			cBattleReward.SetBoxAtMonster(acEnemys[i]);
		}

		foreach (ushort val in MaxInAllGroup.Keys) {
			ObjMgr.Instance.AddMonsterPool (val, MaxInAllGroup [val] + 1);
		}

		InitCharacterObject(acEnemys[curGroup], 1, curGroup);
	}

	public void AddMaxMonsterInGroupInfo(ushort id, int max){
		if (MaxInAllGroup.ContainsKey (id)) {
			if(MaxInAllGroup[id] < max) MaxInAllGroup[id] = max;
		}else{
			MaxInAllGroup.Add(id, max);
		}
	}

	void InitPortal(){
		if (Legion.Instance.SelectedStage.u1PhaseCount > 1) {
			Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/Models/Etc/Portal.prefab", typeof(GameObject)) as GameObject,
			            new Vector3(Legion.Instance.SelectedStage.getField(u1PhaseNo).cPortalPos.X, 0, Legion.Instance.SelectedStage.getField(u1PhaseNo).cPortalPos.Y),
			            Quaternion.Euler(-90,0,0));
		}
	}
	
	
	void FixedUpdate()
	{
		switch(eBattleState)
		{
		case BATTLE_STATE.Battle:
			if(!cCont.bDirection && bLimitStage){
				if(phaseLimitTime > 0){
					phaseLimitTime -= Time.fixedDeltaTime;
					if(phaseLimitTime < 0){
						phaseLimitTime = 0;
						OnBattleEnd(BATTLE_RESULT_TYPE.TimeOver);
					}
				}
			}
			break;
		}
	}

	void OnApplicationPause(bool pause)
	{
		if (pause) {
			battleUIMgr.OnClickPause ();
		}
	}

	float eventTime=0;
	float bossTime=0;
	void Update()
	{
		switch (eBattleState)
		{
		case BATTLE_STATE.Load :
			break;
		case BATTLE_STATE.BossLoad:
			if (eventTime < bossTime) {
				if (eventTime == 0) {
					cCameraMove2.EnableSkillAnimator ();
					battleUIMgr.ViewBossName(BossName);
				}
				eventTime += Time.deltaTime;
			}
			else
			{
				eventTime = 0.3f;
				eBattleState = BATTLE_STATE.ShowStartDirection;

				cCameraMove2.DisableDirectionCam();
				battleUIMgr.HideBossName ();
				bDirection = false;

				for(int j=0; j<acCrews[0].acCharacters.Length; j++){
					acCrews [0].acCharacters [j].cObject.SetActive (true);
					acCrews [0].acCharacters[j].SubAnimationPlay ("Appear_Start");
				}
			}

			break;
		case BATTLE_STATE.ShowStartDirection :
			if(eventTime < 2f)
			{
				eventTime += Time.deltaTime;

				if(eventTime > 0.3f){
					for(int i=0; i<acCrews.Length; i++){
						for(int j=0; j<acCrews[i].acCharacters.Length; j++){
							if(acCrews[i].acCharacters[j].bHero && !acCrews[i].acCharacters[j].isDead){
								acCrews[i].acCharacters[j].LandCharacter();
							}
						}
					}
				}
			}
			else
			{
				EventToCharacters(BROADCAST_EVENT_TYPE.Load, 0);
				eBattleState = BATTLE_STATE.ShowStartMsg;
				battleUIMgr.ShowMsgBattleStart();
				eventTime = 0.0f;
			}
			break;

		case BATTLE_STATE.ShowStartMsg :
			if(eventTime < 1.5f)
			{
				eventTime += Time.deltaTime;
			}
			else
			{
				battleUIMgr.SetActive(true);
				if (acCrews [0].u1ComboCount > 0) battleUIMgr.ForceCombo (acCrews [0].u1ComboElement, acCrews [0].u1ComboCount);

				if (Legion.Instance.cTutorial.au1Step [0] < Server.ConstDef.LastTutorialStep) {
					battleUIMgr.MenuBtn.SetActive (false);
					battleUIMgr.objAutoButtonLayout.SetActive (false);
				}
				eBattleState = BATTLE_STATE.MoveToNext;
			}
			break;
		case BATTLE_STATE.MoveToNext:
			EventToCharacters(BROADCAST_EVENT_TYPE.Load, 1);
			eBattleState = BATTLE_STATE.Ready;

			if (bPhaseFirst)
			{
				bSetting = true;
//				battleUIMgr.InitMiniGauge(acCrews[1]);
			
				bPhaseFirst = false;

				battleUIMgr.FirstShowAll();
				switch(eGameStyle){
				case GameStyle.League:
					fStateDelay = 3.2f;
					break;
				case GameStyle.Guild:
					fStateDelay = 3.2f;
					break;
				default:
					fStateDelay = 0.1f;
					break;
				}
			}
			break;
		case BATTLE_STATE.Ready:
			// print ready start effect
			if (bPhaseFirst)
			{
				bPhaseFirst = false;
			}
			// if end start call start()
			if(fStateDelay > 0){
				fStateDelay -= Time.deltaTime;
			}else{
				eBattleState = BATTLE_STATE.Start;
			}
			break;
		case BATTLE_STATE.Start:
			OnStartBattle ();

			if (u1PhaseNo == 0) {
				if (Legion.Instance.cTutorial.au1Step [0] == Server.ConstDef.LastTutorialStep) {
					if (Legion.Instance.bAuto != cCont.bAuto)
						battleUIMgr.OnClickAuto ();
				}
			}
			
			break;
		case BATTLE_STATE.Battle:
			if(fStateDelay > 0){
				fStateDelay -= Time.deltaTime;
			}else{
				bTutorialCheckNeed = true;
				CheckTutorial (3);
			}

			// print aggro/time
			//float time = GetComponent<Controller>().fPlayTime;
			// wait controller call OnBattleEnd()
			
			break;
		case BATTLE_STATE.PhaseClear:
			break;
		case BATTLE_STATE.End:
			// print end effect
			fStateDelay -= Time.deltaTime;
			if (TutorialCheckType == 1 && fStateDelay < 0.5f) {
				TutorialCheckType = 2;
			}

			if(fStateDelay < 0){
				if(CheckTutorial(7)){
					EndTutorial();
					return;
				}

				DeleteObject();
				GetItemBox ();
				Byte aliveCharCnt=3;

				if(eGameStyle == GameStyle.Stage)
				{
					if(!bWin)
						aliveCharCnt = 0;
					else
					{
						for(int i=0; i<acCrews[0].acCharacters.Length; i++)
						{
							if(acCrews[0].acCharacters[i] != null)
							{
								if(acCrews[0].acCharacters[i].isDead)
								{
									if(aliveCharCnt > 0) 
										aliveCharCnt--;
								}
							}
						}

						cBattleReward.u1AliveCharCnt = aliveCharCnt;
					}
				}

				if(Server.ServerMgr.bConnectToServer)
				{
					PopupManager.Instance.ShowLoadingPopup(1);
					switch(eGameStyle)
					{
					case GameStyle.Stage : 
						Server.ServerMgr.Instance.ClearStage(Legion.Instance.SelectedCrew, aliveCharCnt, BattleResult);
						break;
					case GameStyle.League:
						Server.ServerMgr.Instance.RequestLeagueMatchResult((byte)eResultType , LeagueResult); 
						break;
					case GameStyle.Guild:
						Server.ServerMgr.Instance.RequestGuildMatchResult((byte)eResultType , GuildResult); 
						break;
					}
				}
				else
				{
					#if UNITY_EDITOR
					// 2016. 7. 5 jy
					// 클라이언트 싱글화 전투 후 다음 스테이지로 넘어갈 수 있도록 값을 저장한다
					switch(eGameStyle)
					{
					case GameStyle.Stage : 
						Server.Message.SingeClientProcess(Server.MSGs.STAGE_RESULT, BattleResult, Legion.Instance.SelectedCrew, (Byte)3);
						break;
					case GameStyle.League:
						break;
					}
					#else
					battleUIMgr.ShowMsgBattleEnd(bWin);
					#endif

				}
				eBattleState = BATTLE_STATE.Result;
			}
			if (bPhaseFirst)
			{
				bPhaseFirst = false;
			}
			break;
		case BATTLE_STATE.Result:
			return;
			// print result
			//			DebugMgr.Log("게임종료");
		}
		
		if(bSetting) UpdateLegionCool();
	}

	public bool CheckTutorial(int type){
		if(eGameStyle != GameStyle.Stage) return false;
		if (Legion.Instance.cTutorial.au1Step [0] == Server.ConstDef.LastTutorialStep) {
			return false;
		}

		if (Legion.Instance.cTutorial.au1Step [0] == 0) {
			TutorialCheckType = 1;
		}

		int iStep = 4;
		switch (TutorialCheckType) {
		case 1: 
			switch(type){
			case 3:
				if (bTutorialCheckNeed && TutorialCheckStep < 4) {
					if (TutorialCheckStep == 0){
						battleUIMgr.ShowTutorialMsg (TextManager.Instance.GetText ("odin_tutorial_1"));
						fStateDelay = 3.5f;
					} else if (TutorialCheckStep == 1) {
						battleUIMgr.ShowTutorialMsg (TextManager.Instance.GetText ("odin_tutorial_2"));
						fStateDelay = 6.5f;
					} else if (TutorialCheckStep == 2) {
						battleUIMgr.ShowTutorialMsg (TextManager.Instance.GetText ("odin_tutorial_3"));
						fStateDelay = 2.0f;
					} else if (TutorialCheckStep == 3) {
						StartCoroutine (TimeScaleDownForUpdate ());
					}

					TutorialCheckStep++;
					bTutorialCheckNeed = false;

					return true;
				}
				return false;
				break;
			case 4: iStep = 4; break;
			case 5: iStep = 5; break;
			case 6: iStep = 6; break;
			case 7: iStep = 7; break;
			}
			break;
		case 2: 
			switch(type){
			case 7: iStep = 7; break;
			}
			break;
		}

		//DebugMgr.LogError (TutorialCheckType + "-" + TutorialCheckStep + ":" + iStep);

		if (TutorialCheckStep > 0 && iStep <= TutorialCheckStep) {
			return false;
		}

		Legion.Instance.cTutorial.AddController (cCont);

		if (Legion.Instance.cTutorial.CheckTutorial (MENU.BATTLE)) {
			TutorialCheckStep = Legion.Instance.cTutorial.CurrentTutorial.u1TutorialPart;
			cCont.PauseGame ();

			switch (TutorialCheckStep) {
			case (int)TutorialSubPart.USE_SKILL:
				battleUIMgr.EnableAllSkillButton ();
				battleUIMgr.FocusSkillButton (0, 0);
				Time.timeScale = 1.0f;
				break;
			case (int)TutorialSubPart.BOSS_SKILL:
				battleUIMgr.SetActive (false);
				break;
			}

			if(TutorialCheckType == 0){
				TutorialCheckType = Legion.Instance.cTutorial.CurrentTutorial.u1TutorialType;
			}
			return true;
		} else {
			if (iStep == 1) iStep = Server.ConstDef.LastTutorialStep;
		}

		return false;
	}

	IEnumerator TimeScaleDownForUpdate()
	{
		while (Time.timeScale > 0.2f) {
			yield return new WaitForSeconds (0.2f);
			if (Time.timeScale <= 0.1f) {
				Time.timeScale = 0f;
			} else {
				Time.timeScale -= 0.1f;
			}
		}

		CheckTutorial(5);
	}

	public void SpawnNext(BattleCharacter dieChar){
		int length = cCurrentBattleField.acMonsterGroup[curGroup].u1MonsterCount;
		Vector3 basePos = new Vector3(cCurrentBattleField.acMonsterGroupPosInfo[curGroup].cCenterPos.X, 0, cCurrentBattleField.acMonsterGroupPosInfo[curGroup].cCenterPos.Y);
		Pos[] groupPos = StageInfoMgr.Instance.GetPosInfo(cCurrentBattleField.acMonsterGroupPosInfo[curGroup].u2PosID);

		curIdx++;
		if(curIdx >= cCurrentBattleField.acMonsterGroup[curGroup].u1MonsterCountMax){
			remainCount--;
			if(remainCount <= 0){
				curGroup++;
				if(curGroup >= acEnemys.Length){
					u1PhaseNo++;
					if(u1PhaseNo < Legion.Instance.SelectedStage.u1PhaseCount){
						StartCoroutine ( GoToBossField() );
					}else{
						ClearStage();
					}
				}else{
					GetItemBox ();
					InitCharacterObject(acEnemys[curGroup], 1, curGroup);
					EventToCharacters(BROADCAST_EVENT_TYPE.GroupRespawn, 0);
					EventToCharacters(BROADCAST_EVENT_TYPE.Load, 1);
				}
			}
			return;
		}

		for (int i=0; i<acCrews[1].acCharacters.Length; i++) {
			if (acCrews[1].acCharacters[i].cObject == dieChar.cObject)
			{
				BattleCharacter btChar = acEnemys[curGroup].acCharacters[curIdx];
				btChar.iTeamIdx = 1;

				Monster cMonster = (Monster)btChar.cCharacter;
				GameObject cObject = ObjMgr.Instance.GetMonsterAtPool(cMonster.cClass.u2ID);
				cObject.name = btChar.cCharacter.sName;

				acCrews[1].acCharacters[i] = btChar;

				cObject.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));

				btChar.cObject = cObject;
				if(curPosIdx >= groupPos.Length) curPosIdx = 0;
				cObject.transform.position = basePos + new Vector3(groupPos[curPosIdx].X, btChar.CheckGround(), groupPos[curPosIdx].Y);
				
				curPosIdx++;

				btChar.SetIdle(true);

				VFXMgr.Instance.GetVFX("/Common/Eff_Common_Spawn", btChar.cObject.transform.position, Quaternion.identity);
				return;
			}
		}
	}

	public bool CheckPhase(){
		if(curGroup == acEnemys.Length){
			if(u1PhaseNo == cBattleStage.u1PhaseCount-1){
				return true;
			}
		}

		return false;
	}

	//	public void goToNextPhase()
	//	{
	//		cMBP.GoNext();
	//	}
	//
	//	public void autoNextPhase(bool auto)
	//	{
	//		cMBP.SetAuto(auto);
	//	}
	
	public void DropItemBox(DropBox[] acDropBoxs, Vector3 pos){
		int cnt = 0;
		pos.y = 0.1f;
		for(int i=0; i<acDropBoxs.Length; i++){
			// 2016. 09. 08 jy
			// 드랍아이템 박스의 갯수를 최대 3개 셋팅 한다
			// 보상에 재화가 들어가면서 드랍아이템 갯수가 너무 많아 렉걸림
			if(cnt >= 3)
				break;
			
			pos.x += 0.1f;
			GameObject cObject = Instantiate(pBox, pos, Quaternion.Euler(new Vector3(0,UnityEngine.Random.Range(0f,360f),0) ) ) as GameObject;
			lstBox.Add(cObject);
			//아이템박스에 사운드 붙이기
			SoundPlayer soundPlayer = cObject.AddComponent<SoundPlayer>() as SoundPlayer;
			cObject.GetComponent<SoundPlayer>().audioClip = itemDropAudio;
			cObject.GetComponent<SoundPlayer>().enabled = false;
			cObject.GetComponent<SoundPlayer>().enabled = true;
			Transform cEff = VFXMgr.Instance.GetVFX("/Common/Eff_Common_ItemBox", pos, Quaternion.identity).transform;
			cEff.parent = cObject.transform.FindChild("Bone01");
			VFX cVFX = new VFX( cEff );
			lstBoxEff.Add(cVFX);
			
			cnt++;
		}
	}
	
	void GetItemBox(){
		for (int i=0; i<acCrews[0].acCharacters.Length; i++) {
			if(acCrews[0].acCharacters[i].a_col.bFocus){
				RemoveItemBox(acCrews[0].acCharacters[i].cObject.transform);
				return;
			}
		}
	}

	void RemoveItemBox(Transform tTransform){
		for (int i=0; i<lstBox.Count; i++) {
			lstBox[i].GetComponent<Animator>().Play("ItemBoxGet");
//			lstBox[i].AddComponent<TweenPosition>().SetTarget(tTransform);
			Destroy(lstBox[i],2.0f);
			lstBoxEff[i].SetEmit(false);
			Destroy(lstBoxEff[i].gameObject,2.0f);
			
			//			// 보상 임시 코드
			//			if(i==0)
			//				battleUIMgr.AddGold(150);
			//			else
			//				battleUIMgr.AddBox(1);
		}
		
		lstBox.RemoveRange(0, lstBox.Count);
		lstBoxEff.RemoveRange(0, lstBoxEff.Count);
	}
	
	void UpdateLegionCool(){
		if(cCont.bDirection) return;

		for(int i=0; i<acCrews.Length; i++){
			if(acCrews[i] != null && acCrews[i].fLegionCoolTime > 0f){
				acCrews[i].fLegionCoolTime -= Time.deltaTime;
				if(acCrews[i].fLegionCoolTime <= 0.0f){
					acCrews[i].fLegionCoolTime = 0f;
				}
//				battleUIMgr.ui_HeroSkill.UpdateLegionCool(acCrews[i].fLegionCoolTime/LegionInfoMgr.Instance.fAllSkilCoolTime);
			}
		}
	}

	IEnumerator GoToBossField(){
		bDirection = true;

		for(int i=0; i<acCrews[0].acCharacters.Length; i++){
			if(!acCrews[0].acCharacters[i].isDead){
				acCrews[0].acCharacters[i].cTarget = null;
				acCrews[0].acCharacters[i].obs.enabled = false;
			}
		}

		yield return new WaitForSeconds(0.1f);

		cCameraMove2.DisableDirectionCam();
		for(int i=0; i<acCrews[0].acCharacters.Length; i++){
			if(!acCrews[0].acCharacters[i].isDead){
				acCrews[0].acCharacters[i].RunToPortal();
			}
		}

		yield return new WaitForSeconds(1.5f);

		for(int i=0; i<acCrews[0].acCharacters.Length; i++){
			if(!acCrews[0].acCharacters[i].isDead){
				acCrews [0].acCharacters [i].SetIdle (false);
			}
		}

		//CheckTutorial (4);
		while (Legion.Instance.cTutorial.bIng) {
			yield return new WaitForSeconds (0.1f);
		}

		FadeEffectMgr.Instance.FadeOut(1f);
		yield return new WaitForSeconds(1f);
		cCont.EndBattle();
		battleUIMgr.SetActive(false);

		Destroy(GameObject.FindGameObjectWithTag("Portal"));

		for(int i=0; i<acCrews[0].acCharacters.Length; i++){
			if(!acCrews[0].acCharacters[i].isDead){
				acCrews[0].acCharacters[i].WaitBoss();
			}
		}

		eBattleState = BATTLE_STATE.PhaseClear;
		DeleteObject();
		ObjMgr.Instance.RemoveMonsterPool ();
		cCurrentBattleField = cBattleStage.getField(u1PhaseNo);
		curGroup = 0;

		AssetMgr.Instance.SceneLoadAsync("Boss", true);

		ChangeSkyBox(cBattleStage.acPhases[u1PhaseNo].u1Environment, cBattleStage.acPhases[u1PhaseNo].u1SkyBox);

		yield return new WaitForSeconds(0.1f);

		Vector3 startPos = new Vector3(cCurrentBattleField.cCrewStart.X, 0f, cCurrentBattleField.cCrewStart.Y);

		int left = -1;
		for (int i=0; i<acCrews[0].acCharacters.Length; i++) {
			GameObject cObject = acCrews[0].acCharacters[i].cObject;
			left *= -1;
			Quaternion rot = Quaternion.Euler(new Vector3(0, cCurrentBattleField.cCrewStart.Dir, 0));
			cObject.transform.position = startPos + (rot * new Vector3(Mathf.Round((i+1)/2)*1.0f*left, 0, 0));
			cObject.transform.rotation = rot;
			acCrews[0].acCharacters[i].a_col.enabled = true;
		}

		InitMonsterCrew();

		yield return new WaitForEndOfFrame();

		float dTime = cCameraMove2.SetBossCamera();

		yield return new WaitForEndOfFrame();

		cCameraMove2.EnableSkillAnimator ();
		battleUIMgr.ViewBossName(BossName);

		yield return new WaitForSeconds(dTime);
		cCameraMove2.DisableDirectionCam();
		battleUIMgr.SetActive(true);
		bDirection = false;

		yield return new WaitForSeconds(1.0f);

		eBattleState = BATTLE_STATE.MoveToNext;
	}

	public void ClearStage(){
		if(eBattleState != BATTLE_STATE.Battle) return;

		for (int i=0; i<acCrews[1].acCharacters.Length; i++) {
			acCrews[1].acCharacters[i].SetDamageColor(0);
			if (!acCrews[1].acCharacters[i].isDead){
				acCrews[1].acCharacters[i].SetDeath();
			}
		}

		cCont.EndBattle();
		OnBattleEnd(Battle.BATTLE_RESULT_TYPE.KillAll);
		
		for(int i=0; i<acCrews[0].acCharacters.Length; i++){
			if(!acCrews[0].acCharacters[i].isDead){
				acCrews[0].acCharacters[i].PlayWin();
			}
		}
	}

	public void DefeatStage(){
		cCont.EndBattle();

		for (int i=0; i<acCrews[0].acCharacters.Length; i++) {
			acCrews[0].acCharacters[i].SetDamageColor(0);
			if (!acCrews[0].acCharacters[i].isDead){
				acCrews[0].acCharacters[i].SetDeath();
			}
		}
		
		for(int i=0; i<acCrews[1].acCharacters.Length; i++){
			if(!acCrews[1].acCharacters[i].isDead){
				acCrews[1].acCharacters[i].PlayWin();
			}
		}
	}

	void InitCharacterObject(BattleCrew battleCrew, int crewIndex, int group)
	{
		battleCrew.u1ComboCount = 0;
		battleCrew.u1ComboElement = 0;
		battleCrew.u2LastBuff = 0;
		battleCrew.u2LastDebuff = 0;

		Vector3 startPos = new Vector3 (cCurrentBattleField.cCrewStart.X, 2f, cCurrentBattleField.cCrewStart.Y);
		float dir = cCurrentBattleField.cCrewStart.Dir;

		Vector3 basePos = Vector3.zero;
		Pos[] groupPos = new Pos[10];
		int length = 0;
		int left = -1;

		if (crewIndex == 0){
			length = battleCrew.acCharacters.Length;
			groupPos = StageInfoMgr.Instance.GetPosInfo (cCurrentBattleField.cCrewStartPosID);
		}else if (crewIndex == 1) {
			switch(eGameStyle){
			case GameStyle.League: case GameStyle.Guild:
				startPos = new Vector3 (cCurrentBattleField.cEnemyStart.X, 2f, cCurrentBattleField.cEnemyStart.Y);
				dir = cCurrentBattleField.cEnemyStart.Dir;
				length = battleCrew.acCharacters.Length;
				left *= -1;
				groupPos = StageInfoMgr.Instance.GetPosInfo (cCurrentBattleField.cEnemyStartPosID);
				break;
			case GameStyle.Stage:
				length = cCurrentBattleField.acMonsterGroup [group].u1MonsterCount;
				basePos = new Vector3 (battleCrew.cPosInfo.X, 0, battleCrew.cPosInfo.Y);
				groupPos = StageInfoMgr.Instance.GetPosInfo (cCurrentBattleField.acMonsterGroupPosInfo [group].u2PosID);
				curPosIdx = 0;
				remainCount = length;
				curIdx = length - 1;

				if (group == curGroup) {
					acCrews [1] = new BattleCrew (this);
					acCrews [1].acCharacters = new BattleCharacter[length];
				}
				break;
			}
			bTutorialCheckNeed = true;
		}

		for (int charIdx = 0; charIdx < length; charIdx++)
		{
			if (battleCrew.acCharacters[charIdx] != null)
			{
				BattleCharacter btChar = battleCrew.acCharacters[charIdx];
		
				if (btChar.cCharacter is Hero)
				{
					btChar.iTeamIdx = crewIndex;
					btChar.iCharIdx = charIdx;

					Hero cHero = (Hero)btChar.cCharacter;
					cHero.InitModelObject();

					left *= -1;
					cHero.cObject.GetComponent<HeroObject>().SetAnimations_Battle();
					btChar.cObject = cHero.cObject;
					btChar.cObject.SetActive (true);
					Quaternion rot = Quaternion.Euler(new Vector3(0, dir, 0));
					btChar.fFirstAngle = dir;
					switch(eGameStyle){
					case GameStyle.League: case GameStyle.Guild:
						cHero.cObject.transform.position = startPos + new Vector3 (groupPos [charIdx].X, 0, groupPos [charIdx].Y);
						break;
					case GameStyle.Stage:
						cHero.cObject.transform.position = startPos + (rot * new Vector3 (Mathf.Round ((charIdx + 1) / 2) * 1.0f * left, btChar.CheckGround (), 0));
						break;
					}

					cHero.cObject.transform.rotation = rot;
					btChar.SaveCurrentBipPos ();
					btChar.LandCharacter();
				}
				else if (btChar.cCharacter is Monster)
				{
					Monster cMonster = (Monster)btChar.cCharacter;
					GameObject cObject = ObjMgr.Instance.GetMonsterAtPool(cMonster.cClass.u2ID);
					cObject.SetActive(true);
					cObject.name = btChar.cCharacter.sName;
					btChar.iTeamIdx = crewIndex;
					btChar.iCharIdx = charIdx;

					if(cMonster.cClass.u1MonsterType == 2) BossName = cMonster.cClass.sName;
					

					acCrews[1].acCharacters[charIdx] = btChar;
					cObject.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
					btChar.cObject = cObject;

					if(curPosIdx >= groupPos.Length) curPosIdx = 0;
					cObject.transform.position = basePos + new Vector3(groupPos[curPosIdx].X, btChar.CheckGround(), groupPos[curPosIdx].Y);
					curPosIdx++;

					btChar.SetIdle(true);
				}

				if(crewIndex == 1) VFXMgr.Instance.GetVFX("/Common/Eff_Common_Spawn", btChar.cObject.transform.position, Quaternion.identity);
//				if(u1PhaseNo != 0) btChar.cObject.SetActive(false);
			}
		}
	}
	
	
	void StartBattle()
	{
	}
	
	public void OnStartBattle()
	{
		if (eBattleState != BATTLE_STATE.Start)
		{
			DebugMgr.Log("[OnStartButton]Wrong State " + eBattleState);
			return;
		}
		eBattleState = BATTLE_STATE.Battle;
		GetComponent<Controller>().StartBattle();
		EventToCharacters(BROADCAST_EVENT_TYPE.StartPhase, 2);
	}

	void EndTutorial(){
//		for (int j = 0; j < acCrews[0].acCharacters.Length; j++) {
//			acCrews [0].acCharacters [j].isDead = true;
//		}
		cCont.EndBattle();
		eBattleState = BATTLE_STATE.Result;
	}

	public void UserAllDeathByTime(float time){
		Invoke("UserAllDeath", time);
	}

	void UserAllDeath(){
		for(int i=0; i<acCrews[0].acCharacters.Length; i++){
			if(!acCrews[0].acCharacters[i].isDead){
				acCrews[0].acCharacters[i].SetDeath();
			}
		}
	}

	//[SerializeField] FrameChecker frameChecker;
	public void OnBattleEnd(BATTLE_RESULT_TYPE eType)
	{
		if (eBattleState != BATTLE_STATE.Battle)
		{
			DebugMgr.Log("[OnBattleEnd]Wrong State " + eBattleState);
			return;
		}

		if (TutorialCheckStep != Server.ConstDef.LastTutorialStep) {
			if(CheckTutorial(6)){
				return;
			}
		}
		Time.timeScale = 1.0f;
		//frameChecker.Report(Legion.Instance.u2SelectStageID);
		eResultType = eType;
		switch (eType)
		{
		case BATTLE_RESULT_TYPE.TimeOver:
			if (eGameStyle == GameStyle.Guild) {
				int result = CheckGuildBattleEnd ();

				switch(result){
				case 0:
					eBattleState = BATTLE_STATE.End;
					bWin = false;
					DefeatStage ();
					break;
				case 1:
					ClearStage ();
					break;
				}

				return;
			}

			eBattleState = BATTLE_STATE.End;

			bWin = false;
			DefeatStage();
			//battleUIMgr.ShowMsgBattleEnd(false, acCrews[0]);
			break;
		case BATTLE_RESULT_TYPE.AllKilled:
			eBattleState = BATTLE_STATE.End;

			if(TutorialCheckType == 1) fStateDelay = 5.0f;

			bWin = false;
			//battleUIMgr.ShowMsgBattleEnd(false, acCrews[0]);
			break;
		case BATTLE_RESULT_TYPE.KillAll:
//			if (u1PhaseNo >= cBattleStage.u1PhaseCount)
//			{
			eBattleState = BATTLE_STATE.End;

			bWin = true;
			//battleUIMgr.ShowMsgBattleEnd(true, acCrews[0]);
			fStateDelay = 2.0f;
//			}
//			else
//			{
//				eBattleState = BATTLE_STATE.PhaseClear;
//				fStateDelay = 3.0f;
//				
//			}
			break;
		}		
	}

	int CheckGuildBattleEnd()
	{
		bool bLose = false; 
		if (u1UserCrewIndex == 2) {
			bLose = true;
		} else {
			int nextUser = 0;
			for (int i = u1UserCrewIndex + 1; i < GuildInfoMgr.Instance.cUserCrews.Length; i++) {
				if (GuildInfoMgr.Instance.cUserCrews [i].u1Count > 0) {
					nextUser = i;
				}
			}

			if (nextUser == 0)
				bLose = true;
		}

		if (bLose) {
			battleUIMgr.SetGuildCrewIcon (0, u1UserCrewIndex, 0);
			return 0;
		}


		bool bWin = false; 
		if (u1EnemyCrewIndex == 2) {
			bWin = true;
		} else {
			int nextEnemy = 0;
			for (int i = u1EnemyCrewIndex + 1; i < GuildInfoMgr.Instance.cEnemyCrews.Length; i++) {
				if (GuildInfoMgr.Instance.cEnemyCrews [i].u1Count > 0) {
					nextEnemy = i;
				}
			}

			if (nextEnemy == 0)
				bWin = true;
		}
		
		if (bWin) {
			battleUIMgr.SetGuildCrewIcon (1, u1EnemyCrewIndex, 0);
			return 1;
		}

		eBattleState = BATTLE_STATE.Load;
		cCont.EndBattle ();

		u1Round++;

		battleUIMgr.SetGuildCrewIcon (0, u1UserCrewIndex, 0);
		battleUIMgr.SetGuildCrewIcon (1, u1EnemyCrewIndex, 0);

		for (int j = 0; j < acCrews [0].acCharacters.Length; j++) {
			acCrews [0].acCharacters [j].SetDeath ();
		}
		for (int j = 0; j < acCrews [1].acCharacters.Length; j++) {
			acCrews [1].acCharacters [j].SetDeath ();
		}

		SetNewGuildRound (2);

		return 2;
	}

	public void LeagueResult(Server.ERROR_ID err)
	{
		DebugMgr.Log(err);

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.LEAGUE_MATCH_RESULT, err), emptyMethodChangeSceneLeague);
			return;
		}

		else
		{
			PopupManager.Instance.CloseLoadingPopup();
			Legion.Instance.UpdateLeagueInfoByResult ((Byte)eResultType);
			battleUIMgr.ShowMsgBattleEnd(bWin);
		}
	}

	public void GuildResult(Server.ERROR_ID err)
	{
		DebugMgr.Log(err);

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			if (err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET) {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetText ("err_guild_already_exit_guild"), emptyMethodChangeSceneGuild);
			} else if (err == Server.ERROR_ID.GUILD_REQUEST_CONFLICT) {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("err_title_guild_exit_reject"), TextManager.Instance.GetText ("err_desc_guild_exit_reject"), emptyMethodChangeSceneGuild);
			} else if (err == Server.ERROR_ID.GUILD_DATA_DENIED) {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_guild_calculate"), TextManager.Instance.GetText ("popup_desc_guild_calculate"), emptyMethodChangeSceneGuild);
			} else {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_MATCH_RESULT, err), emptyMethodChangeSceneGuild);
			}
			return;
		}
		else
		{
			PopupManager.Instance.CloseLoadingPopup();
			Legion.Instance.UpdateGuildInfoByResult ((Byte)eResultType);
			battleUIMgr.ShowMsgBattleEnd(bWin);
		}
	}

	public void BattleResult(Server.ERROR_ID err)
	{
		DebugMgr.Log(err);
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_RESULT, err), emptyMethodChangeSceneCamp);
			return;
		}
		
		else
		{
			PopupManager.Instance.CloseLoadingPopup();
			battleUIMgr.ShowMsgBattleEnd(bWin);
		}
	}

	public void DeleteObject(bool bLast = true)
	{
		switch(eGameStyle)
		{
		case GameStyle.League:
			for (int j = 0; j < acCrews[1].acCharacters.Length; j++)
			{
				if (acCrews[1].acCharacters[j].isDead)
				{
					acCrews[1].acCharacters[j].cObject.SetActive(false);
				}
			}
		break;
		case GameStyle.Guild:
			if (!bLast) {
				for (int j = 0; j < acCrews [0].acCharacters.Length; j++) {
					if (acCrews [0].acCharacters [j].isDead) {
						acCrews [0].acCharacters [j].cObject.SetActive (false);
					}
				}
				for (int j = 0; j < acCrews [1].acCharacters.Length; j++) {
					if (acCrews [1].acCharacters [j].isDead) {
						Destroy (acCrews [1].acCharacters [j].cObject);
					}
				}
			}
			break;
		}
			
	}

	public void QuitEvent()
	{
		for (int i = 0; i < acCrews[0].acCharacters.Length; i++)
		{
			acCrews [0].acCharacters [i].QuitEvent ();
		}
	}
	
	void EventToCharacters(BROADCAST_EVENT_TYPE eType, Byte u1CrewIndex)
	{
		if (u1CrewIndex == 2)
		{
			acCrews[0].EventToCharacters(eType, acCrews[1]);
			acCrews[1].EventToCharacters(eType, acCrews[0]);
		}
		else
		{
			acCrews[u1CrewIndex].EventToCharacters(eType, acCrews[(u1CrewIndex + 1) % 2]);
		}
	}

	void emptyMethodChangeSceneCamp(object[] param)
	{
		StartCoroutine(ChangeSceneWithName("SelectStageScene"));
	}

	IEnumerator ChangeSceneWithName(string sceneName)
	{
		Server.ServerMgr.Instance.ClearFirstJobError ();
		FadeEffectMgr.Instance.FadeOut();
		
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		AssetMgr.Instance.SceneLoad(sceneName);
	}

	void emptyMethodChangeSceneLeague(object[] param)
	{
		StartCoroutine(ChangeSceneWithName("ALeagueScene"));
	}

	void emptyMethodChangeSceneGuild(object[] param)
	{
		StartCoroutine(ChangeSceneWithName("GuildScene"));
	}

	public void SetNewGuildRound(int respawnCrewIdx)
	{
		battleUIMgr.ShowCombo (0, 0);
		battleUIMgr.RemoveChainState ();
		StartCoroutine (StartSetNewGuildRound (respawnCrewIdx));
	}

	IEnumerator StartSetNewGuildRound(int respawnCrewIdx)
	{
		if (respawnCrewIdx == 0) {
			for(int i=0; i<acCrews[1].acCharacters.Length; i++){
				if(!acCrews[1].acCharacters[i].isDead){
					acCrews[1].acCharacters[i].PlayWin();
				}
			}
		} else if (respawnCrewIdx == 1) {
			for(int i=0; i<acCrews[0].acCharacters.Length; i++){
				if(!acCrews[0].acCharacters[i].isDead){
					acCrews[0].acCharacters[i].PlayWin();
				}
			}
		}

		yield return new WaitForSeconds(3.0f);

		FadeEffectMgr.Instance.FadeOut();

		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME+0.1f);
		if (respawnCrewIdx == 2) {
			for (int j = 0; j < acCrews [0].acCharacters.Length; j++) {
				acCrews [0].acCharacters [j].cObject.SetActive (false);
			}
			for (int j = 0; j < acCrews [1].acCharacters.Length; j++) {
				Destroy (acCrews [1].acCharacters [j].cObject);
			}

			u1UserCrewIndex++;
			if(u1UserCrewIndex < 2 && GuildInfoMgr.Instance.cUserCrews [u1UserCrewIndex].u1Count == 0) u1UserCrewIndex++;
			u1EnemyCrewIndex++;
			if(u1EnemyCrewIndex < 2 && GuildInfoMgr.Instance.cEnemyCrews [u1EnemyCrewIndex].u1Count == 0) u1EnemyCrewIndex++;

			battleUIMgr.SetGuildCrewIcon (0, u1UserCrewIndex, 1);
			battleUIMgr.SetGuildCrewIcon (1, u1EnemyCrewIndex, 1);
		}

		FadeEffectMgr.Instance.FadeIn();

		phaseLimitTime = (float)cCurrentBattleField.u4PhaseLimitTime;
		eBattleState = BATTLE_STATE.ShowStartDirection;

		DeleteObject (false);

		if (respawnCrewIdx == 0) 
		{
			acCrews [0] = new BattleCrew (GuildInfoMgr.Instance.cUserCrews [u1UserCrewIndex], this, 0);
			InitCharacterObject (acCrews[0], 0, -1);

			RepositionGuildCrew (1);
			battleUIMgr.SetHeroes (acCrews [0].acCharacters);
			battleUIMgr.initHeroView ();

			cCont.cAIController.InitUserEndEnemyQueue ();
			battleUIMgr.CheckSkillOrder ();
		}
		else if (respawnCrewIdx == 1) 
		{
			acCrews [1] = new BattleCrew (GuildInfoMgr.Instance.cEnemyCrews [u1EnemyCrewIndex], this, 1);
			InitCharacterObject (acCrews[1], 1, -1);

			RepositionGuildCrew (0);
			battleUIMgr.SetEnemys (acCrews [1].acCharacters);
			battleUIMgr.initEnemyView ();

			cCont.cAIController.InitUserEndEnemyQueue ();
			battleUIMgr.CheckSkillOrder ();
		}
		else if (respawnCrewIdx == 2) 
		{
			acCrews [0] = new BattleCrew (GuildInfoMgr.Instance.cUserCrews [u1UserCrewIndex], this, 0);
			InitCharacterObject (acCrews[0], 0, -1);

			acCrews [1] = new BattleCrew (GuildInfoMgr.Instance.cEnemyCrews [u1EnemyCrewIndex], this, 1);
			InitCharacterObject (acCrews[1], 1, -1);

			battleUIMgr.SetHeroes (acCrews [0].acCharacters);
			battleUIMgr.SetEnemys (acCrews [1].acCharacters);

			battleUIMgr.initHeroView ();
			battleUIMgr.initEnemyView ();

			cCont.cAIController.InitUserEndEnemyQueue ();
			battleUIMgr.CheckSkillOrder ();
		}

		battleUIMgr.RefreshCamera ();
	}

	void RepositionGuildCrew(int crewIndex)
	{
		acCrews[crewIndex].u1ComboCount = 0;
		acCrews[crewIndex].u1ComboElement = 0;
		acCrews[crewIndex].u2LastBuff = 0;
		acCrews[crewIndex].u2LastDebuff = 0;

		Vector3 startPos = new Vector3 (cCurrentBattleField.cCrewStart.X, 2f, cCurrentBattleField.cCrewStart.Y);
		float dir = cCurrentBattleField.cCrewStart.Dir;

		Vector3 basePos = Vector3.zero;
		Pos[] groupPos = new Pos[10];
		int length = 0;
		int left = -1;

		if (crewIndex == 0){
			length = acCrews[crewIndex].acCharacters.Length;
			groupPos = StageInfoMgr.Instance.GetPosInfo (cCurrentBattleField.cCrewStartPosID);
		}else if (crewIndex == 1) {
			startPos = new Vector3 (cCurrentBattleField.cEnemyStart.X, 2f, cCurrentBattleField.cEnemyStart.Y);
			dir = cCurrentBattleField.cEnemyStart.Dir;
			length = acCrews[crewIndex].acCharacters.Length;
			left *= -1;
			groupPos = StageInfoMgr.Instance.GetPosInfo (cCurrentBattleField.cEnemyStartPosID);
		}

		for (int charIdx = 0; charIdx < length; charIdx++) {
			if (acCrews [crewIndex].acCharacters [charIdx] != null) {
				BattleCharacter btChar = acCrews[crewIndex].acCharacters [charIdx];
				if (!btChar.isDead) {
					btChar.InitState();

					Hero cHero = (Hero)btChar.cCharacter;
					Quaternion rot = Quaternion.Euler (new Vector3 (0, dir, 0));
					btChar.fFirstAngle = dir;

					cHero.cObject.transform.position = startPos + new Vector3 (groupPos [charIdx].X, 0, groupPos [charIdx].Y);

					cHero.cObject.transform.rotation = rot;
					btChar.SaveCurrentBipPos ();
					btChar.LandCharacter ();
				}
			}
		}
	}

}