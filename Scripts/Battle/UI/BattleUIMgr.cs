using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;

public class BattleUIMgr : MonoBehaviour
{
	public enum DAMAGE_TEXT_TYPE
	{
		DAMAGED_HERO,
		DAMAGED_MONSTER,
		CRITICAL,
		GUARD,
		MISS,
		HEAL_HERO,
		HEAL_MONSTER,
		SLOW,
		AVOID,
		CHAIN_DMG,
		CHAIN_HEAL,
		COMBO_DMG,
		REVIVE_HERO,
	}
	Battle _cBattle;
	BattleCrew _cPlayerCrew;
	BattleCharacter[] _acHeroes;

	BattleCrew _cEnemyCrew;
	BattleCharacter[] _acEnemys;

	GameObject Mass;
	public GameObject MenuBtn;
	
	int _selectedHero=-1;
	
	public Camera cMainCam;
	public Camera cUICam;

	public CameraMove2 cCamScript;

//	public GameObject objCameraChangerLayout;
//	CameraChanger cameraChanger;

	public GameObject[] objHeroViewerLayout;
	HeroViewer[] heroViewer;
	public GameObject objSkillButtonPrefab;

	public GameObject objSkillReserve;

	public GameObject objTimerLayout;
	public GameObject objGuildTimerLayout;
	LimitTimer timer;
	public GameObject objAutoButtonLayout;
	AutoButton autoButton;
	public GameObject objRepeatButtonLayout;
	Toggle repeatButton;
	public GameObject objRepeating;

	GameObject CondIconObj;

	//League
	GameObject LeagueMass;

	float[] afHpBarSize = new float[]{310, 410, 510};
	int currentBarIdx = -1;

	public GameObject ElementConcern;
	public GameObject HpBar;
	public RectTransform HpBorder;
	public Image HpGage;
	public Image NextGage;
	public Text HpText;
	public RectTransform TargetCondiParent;
	List<UI_ConditionIcon> TargetCondis = new List<UI_ConditionIcon>();
	public Text TargetName;
	public Sprite[] HpVari;
	public Image FadeBox;
	public GameObject StartMsg;
	public Text ChapterTxt;
	public Text MapNameTxt;
	public GameObject LeagueStartMsg;
	public RectTransform LeagueMsgTopLine;
	public RectTransform LeagueMsgBotLine;
	public Text LeagueMapNameTxt;

	public GameObject LeagueVSMsg;
	public GameObject LeagueHomeTeam;
	public Text LeagueHomeTeamTxt;
	public GameObject LeagueAwayTeam;
	public Text LeagueAwayTeamTxt;
	public Image LeagueVS_V;
	public Image LeagueVS_S;

	public GameObject LeagueFightMsg;
	public Image[] FightMsgImgs;

	public GameObject ResultObj;
	public Animator ResultMark;
	public GameObject VictoryMsg;
	public GameObject DefeatMsg;
	public GameObject DrawMsg;
	public GameObject ResultBtnMass;

	private GameObject TouchedIcon;

	public RectTransform SkillInfoTr;
	public Image imgSkillIcon;
	public Image imgSkillEle;
	public Text TxtSkillName;
	public Text TxtSkillDesc;

	public RectTransform CondiInfoTr;
	public Text TxtCondiName;
	public Text TxtCondiDesc;
	
	public Transform objDmgTextParent;
	DamageText[] dmgTextQueue;
	const int DMG_TEXT_QUEUE_MAX=50;
	int dmgTextPointer;
	bool _isSkillChange = false;
	bool _bUpScale = false;
	float fSkillScale = 1.0f;

	GameObject breakTextPref;

	BattleCharacter cTarget;
	[SerializeField] GameObject menuCanvas;
	[SerializeField] GameObject resultCanvas;
	[SerializeField] GameObject objResultWin;

	public GameObject BossNameMass;
	public Image BossTitle;
	public Text BossName;

	public GameObject BossIndicator;
	GameObject BossIndiEff;
	GameObject BossIndiVFX;

	private GameObject DeathEff;

	public Transform ComboBombParent;
	GameObject ComboBombObj;

	Animator ComboAnim;
	GameObject ComboGroup;
	Image ComboImg;
	Image ComboImgWhite;
	byte u1ComboCount;
	Text ComboCount;
	Text ComboDamage;
	Outline ComboDamageOutline;

	GameObject SkillChainEffBuff;
	GameObject SkillChainEffDebuff;
	bool setUI=false;

	bool bResult = false;

	float beforePer = 0f;
	float DisableTime = 1f;

	float skillDist = 120f;

	public static Color WarnColor = new Color(1f,1f,1f,1f);

	int[] reserveIndex = new int[2]{-1,-1};

	string sChangeSceneName;

	Vector2 firstJoystickPt;
	public GameObject ContMass;
	public RectTransform Joystick;
	bool bStickTouch;
	float stickAngle;

	int iInfoTouchDown;
	float fInfoTouchTime;

    // 반복전투 재료 목표
    public GameObject TargetItemWindow;
    public Image TargetItemIcon;
    public Image TargetItemGrade;
    public RewardButton cTargetItemButton;
    public Text TargetCount;

	public GameObject objTutorialDesc;
	public CanvasGroup cgTutorialDesc;
	public Text txtTutorialDesc;

	public GameObject objGuildBattle;
	GuildBattleUI[] acGuildLayout;

	public GameObject[] objEnemyViewerLayout;
	EnemyViewer[] enemyViewer;

	private GameObject FocusEff;
    
    void Awake(){
		CondIconObj = AssetMgr.Instance.AssetLoad("Prefabs/UI/Battle/CondiIcon2.prefab", typeof(GameObject)) as GameObject;
		SkillChainEffBuff = AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/Ani_UI_Battle_SkillChain_Heal.prefab", typeof(GameObject)) as GameObject;
		SkillChainEffDebuff = AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/Ani_UI_Battle_SkillChain_Atk.prefab", typeof(GameObject)) as GameObject;
		BossIndiEff = AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/Boss_Floor_Red.prefab", typeof(GameObject)) as GameObject;
		DeathEff = AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/UI_Eff_Battle_Death.prefab", typeof(GameObject)) as GameObject;
		ComboBombObj = AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/Ani_UI_Battle_SkillChain_Combo.prefab", typeof(GameObject)) as GameObject;
		breakTextPref = AssetMgr.Instance.AssetLoad("Prefabs/UI/Battle/SkillBreakText.prefab", typeof(GameObject)) as GameObject;
		FocusEff = AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/UI_Eff_Tutorial_Focus.prefab", typeof(GameObject)) as GameObject;

		firstJoystickPt = Joystick.anchoredPosition;
	}

	public void SetUI(Battle battle, BattleCrew playerBattleCrew)
	{
		Mass = transform.FindChild("Mass").gameObject;
		LeagueMass = Mass.transform.FindChild("League").gameObject;
		BossNameMass.SetActive(false);
		_cBattle = battle;
		_cPlayerCrew = playerBattleCrew;
		_acHeroes = playerBattleCrew.acCharacters;

        SetTargetItemUIActive(false);
		if (_cBattle.eGameStyle == GameStyle.League) {
            objRepeatButtonLayout.SetActive(false);
            MenuBtn.SetActive (false);
			LeagueMass.SetActive (true);
			_cEnemyCrew = _cBattle.acCrews [1];
			_acEnemys = _cEnemyCrew.acCharacters;
			objAutoButtonLayout.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (585f, 117f);
			//_cBattle.cCont.ChangeAuto (true);
		} else if (_cBattle.eGameStyle == GameStyle.Guild) {
			objRepeatButtonLayout.SetActive(false);
			ElementConcern.SetActive (false);
			objGuildBattle.SetActive (true);
			MenuBtn.SetActive (false);
			_cEnemyCrew = _cBattle.acCrews [1];
			_acEnemys = _cEnemyCrew.acCharacters;

			acGuildLayout = new GuildBattleUI[2];
			acGuildLayout [0] = new GuildBattleUI (objGuildBattle.transform.FindChild ("User").gameObject, 
													GuildInfoMgr.Instance.cGuildMemberInfo.strGuildName, 
													GuildInfoMgr.Instance.GetGuildTier(GuildInfoMgr.Instance.cGuildMemberInfo.u2Score).ToString(),
													GuildInfoMgr.Instance.cUserCrews);
			acGuildLayout [1] = new GuildBattleUI (objGuildBattle.transform.FindChild ("Enemy").gameObject,
													GuildInfoMgr.Instance.cGuildMatchData.strMatchingGuildName, 
													GuildInfoMgr.Instance.GetGuildTier(GuildInfoMgr.Instance.cGuildMatchData.u2Score).ToString(),
													GuildInfoMgr.Instance.cEnemyCrews);

			objAutoButtonLayout.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (585f, -50f);

			initEnemyView ();
		} else {
			repeatButton = objRepeatButtonLayout.GetComponent<Toggle> ();
			if (Legion.Instance.AUTOCONTINUE)
			{
                SetRepeatTargetItemInfo();
                objRepeatButtonLayout.SetActive (true);
				repeatButton.isOn = true;
			} else {
                objRepeatButtonLayout.SetActive (false);
			}
            objRepeating.SetActive (Legion.Instance.AUTOCONTINUE);
		}

        Transform ComboParent;
        ComboParent = Mass.transform.FindChild("SkillCombo");
		ComboAnim = ComboParent.GetComponent<Animator>();
		ComboGroup = ComboParent.FindChild("Group1").gameObject;
		ComboImg = ComboGroup.transform.FindChild("Icon").GetComponent<Image>();
		ComboImgWhite = ComboGroup.transform.FindChild("White").GetComponent<Image>();
		ComboCount = ComboGroup.transform.FindChild("Text").GetComponent<Text>();
		ComboDamage = ComboGroup.transform.FindChild("Percent").GetComponent<Text>();
		ComboDamageOutline = ComboGroup.transform.FindChild("Percent").GetComponent<Outline>();

		initHeroView();
		initTimer();
		initDmgText();
		initAutoButton();
		//initSelecteCharacter();
		_acHeroes[0].SetFocus(true);
		DisableMonsterHp();

		cCamScript = cMainCam.GetComponent<CameraMove2> ();
		cCamScript.SetStartCamera ();
		
		setUI = true;
	}

	public void ShowTutorialMsg(string txt)
	{
		objTutorialDesc.SetActive (true);
		LeanTween.cancel (objTutorialDesc);
		LeanTween.value (objTutorialDesc, 1f, 0.5f, 0.5f).setOnUpdate ((float alpha) => { cgTutorialDesc.alpha = alpha; }).setLoopPingPong();
		txtTutorialDesc.text = txt;
	}

	public void HideTutorialMsg()
	{
		objTutorialDesc.SetActive (false);
		LeanTween.cancel (objTutorialDesc);
	}

	public void SetJoyStick(bool bStick){
		ContMass.SetActive(bStick);
	}

	public void OnClickQuit()
	{
		if(Legion.Instance.SelectedStage.u2ChapterID == 0)
        {
            if(Legion.Instance.bAdventoStage == 2)
                sChangeSceneName = "BossRushScene";
            else
                sChangeSceneName = "LobbyScene";
        }
		else sChangeSceneName = "SelectStageScene";
			
		_cBattle.QuitEvent ();
		if(Server.ServerMgr.bConnectToServer){
			PopupManager.Instance.ShowLoadingPopup(1);
			Server.ServerMgr.Instance.ClearStage(Legion.Instance.SelectedCrew, (byte)0, CheckQuitState);
		}else StartCoroutine(ChangeSceneFade());
	}

	public void OnClickQuitInDefeat()
	{
		Legion.Instance.AUTOCONTINUE = false;

		if(Legion.Instance.bAdventoStage == 2)
            sChangeSceneName = "BossRushScene";
        else
            sChangeSceneName = "LobbyScene";
		StageInfoMgr.Instance.LastPlayStage = -1;
		StartCoroutine(ChangeSceneFade());
	}

	public void OnClickQuitStage()
	{
		// 2016. 11. 01 jy
		Legion.Instance.AUTOCONTINUE = false;

//		CheckAndGoToDefeatScene ();
        if(Legion.Instance.bAdventoStage == 2)
            sChangeSceneName = "BossRushScene";
        else
            sChangeSceneName = "SelectStageScene";
		//sChangeSceneName = "SelectStageScene";
        StartCoroutine(ChangeSceneFade());
	}

	public void GoToShop(){
		Legion.Instance.bStageFailed = true;
		FadeEffectMgr.Instance.QuickChangeScene(MENU.SHOP);
		Legion.Instance.AwayBattle ();
	}

	public void GoToForge(){
		Legion.Instance.bStageFailed = true;
		FadeEffectMgr.Instance.QuickChangeScene(MENU.FORGE, (int)POPUP_FORGE.SMITH);
		Legion.Instance.AwayBattle ();
	}

	public void GoToForgeUpgrade(){
		Legion.Instance.bStageFailed = true;
		FadeEffectMgr.Instance.QuickChangeScene(MENU.FORGE, (int)POPUP_FORGE.UPGRADE);
		Legion.Instance.AwayBattle ();
	}

	// 2016. 12. 09 jy
	// 패배시 버튼 클릭시 마다 바로갈 위치 셋팅된 곳으로 보내기
	public void OnClickGoToBtn(int btnIndex)
	{
		StageInfoMgr.Instance.LastPlayStage = -1;
		switch(btnIndex)
		{
		case 0:
			FadeEffectMgr.Instance.QuickChangeScene(MENU.CHARACTER_INFO, (int)POPUP_CHARACTER_INFO.EQUIP_CREATE);
			break;
		case 1:
			Legion.Instance.bStageFailed = true;
			FadeEffectMgr.Instance.QuickChangeScene(MENU.SHOP);
			break;
		case 2:
			Legion.Instance.bStageFailed = true;
			FadeEffectMgr.Instance.QuickChangeScene(MENU.HERO_GUILD, (int)POPUP_HERO_GUILD.TRAINING_HERO);
			break;
        case 3:
            // 제작 등급 업그레이드
            FadeEffectMgr.Instance.QuickChangeScene(MENU.CHARACTER_INFO, (int)POPUP_CHARACTER_INFO.EQUIP_CREATE_UPGRADE);
            break;
		}
        
		Legion.Instance.AwayBattle ();
	}

	void GetLeagueInfoResult(Server.ERROR_ID err){
		if(err != Server.ERROR_ID.NONE) 
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
			DebugMgr.LogError("패킷 호출 하는 부분이 없음");
			return;
		}
		else 
		{
			StartCoroutine(ChangeSceneFade());
		}
	}

	public void RetryStage(){
		StageInfo stageInfo = Legion.Instance.SelectedStage;
		ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[stageInfo.u2ChapterID];

		if(stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
		{
			Byte ticketCount = StageInfoMgr.Instance.GetForestTicket();
			if(ticketCount <= 0)
			{
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("forest_clear_count_zero"), null);
				return;
			}
		}
			
		if(!Legion.Instance.CheckEnoughGoods(chapterInfo.GetConsumeGoods()))
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_nocost"), TextManager.Instance.GetText("popup_desc_key_lack"), null);
			return;
		}
		
		#if UNITY_EDITOR
		string log = string.Format("[{0}] [crew : {1}] [stage : {2}] [difficult : {3}]", 
		                           Server.MSGs.STAGE_START, Legion.Instance.cBestCrew.u1Index, Legion.Instance.SelectedStage.u2ID, Legion.Instance.SelectedDifficult);
		
		DebugMgr.Log(log);
		#endif

		ObjMgr.Instance.RemoveMonsterPool();
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.StartStage(Legion.Instance.cBestCrew, Legion.Instance.SelectedStage, (byte)Legion.Instance.SelectedDifficult, AckBattleStart);
	}

	public void AckBattleStart(Server.ERROR_ID err)
	{
		DebugMgr.Log (err);
		
		if(err != Server.ERROR_ID.NONE) 
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_START, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else 
		{
			ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[Legion.Instance.SelectedStage.u2ChapterID];
			Legion.Instance.SubGoods(chapterInfo.GetConsumeGoods());
			FadeEffectMgr.Instance.FadeOut();
			PopupManager.Instance.CloseLoadingPopup();
			SoundManager.Instance.OffBattleListner ();
			AssetMgr.Instance.SceneLoad("Battle");
		}
	}

	public void CheckQuitState(Server.ERROR_ID err)
	{
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_RESULT, err), Server.ServerMgr.Instance.CallClear);
			return;
		}

		else
		{
			StartCoroutine(ChangeSceneFade());
		}
	}

	IEnumerator ChangeSceneFade()
	{
		PopupManager.Instance.CloseLoadingPopup();
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		Legion.Instance.AwayBattle ();
		AssetMgr.Instance.SceneLoad(sChangeSceneName);
	}

	public GameObject pause;
	public void OnClickPause()
	{
		if (_cBattle == null)
			return;
		
		if(!_cBattle.cCont.isPlay()) return;
		if(_cBattle.TutorialCheckType > 0) return;
		if(_cBattle.eGameStyle != GameStyle.Stage) return;
		if(_cBattle.bDirection)
			return;

		if (Legion.Instance.cTutorial.au1Step [0] != Server.ConstDef.LastTutorialStep) return;
		//_cBattle.ClearStage();
		if(pause != null)
        {            
            pause.SetActive(true);
			_cBattle.cCont.PauseGame ();
            PopupManager.Instance.AddPopup(pause, OnClickPauseClose);
        }
	}

	public void OnClickPauseClose()
	{
		if(pause !=null)
        {
            pause.SetActive(false);
			_cBattle.cCont.ResumeGame ();
            PopupManager.Instance.RemovePopup(pause);
        }
	}

	public void OnClickMySkill(int charIdx, int skillIdx){
		CondiInfoTr.gameObject.SetActive(false);

		iInfoTouchDown = 1;
		fInfoTouchTime = 0.0f;

		BattleSkill sk = _acHeroes [charIdx].cSkills.lstcSelectedActiveSkill [skillIdx];

		Vector3 pos = new Vector3(0,150,0);

		SkillInfoTr.SetParent (heroViewer [charIdx].gameObject.transform);
		SkillInfoTr.localPosition = pos;

		imgSkillIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Skill/Atlas_SkillIcon_" + _acHeroes[charIdx].cCharacter.cClass.u2ID + "." + sk.cInfo.u2ID);
		Byte element = sk.cInfo.u1Element;
		imgSkillEle.sprite =  AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.common_02_skill_element_"+(element == 5 ? _acHeroes[charIdx].u1Element : element));
		TxtSkillName.text = TextManager.Instance.GetText(sk.cInfo.sName);
		TxtSkillDesc.text = sk.GetSkillDesc ();
	}

	public void DisableSkillInfo(){
		if(iInfoTouchDown == 1) iInfoTouchDown = 0;

		SkillInfoTr.gameObject.SetActive(false);
	}

	public void OnClickCondition(int charIdx, UInt16 condiId, GameObject obj){
		SkillInfoTr.gameObject.SetActive(false);

		iInfoTouchDown = 2;
		fInfoTouchTime = 0.3f;
		TouchedIcon = obj;

		ConditionInfo cond = ConditionInfoMgr.Instance.GetInfo (condiId);

		if (charIdx > -1) {
			CondiInfoTr.SetParent (heroViewer [charIdx].gameObject.transform);
			CondiInfoTr.localPosition = new Vector3 (0, 250, 0);
		} else {
			CondiInfoTr.SetParent (HpBar.transform);
			CondiInfoTr.localPosition = new Vector3 (0, -250, 0);
		}

		TxtCondiName.text = TextManager.Instance.GetText(cond.sName);
		TxtCondiDesc.text = TextManager.Instance.GetText(cond.sDescription);
	}

	public void DisableConditionInfo(GameObject obj){
		if (TouchedIcon != obj)
			return;
		
		if(iInfoTouchDown == 2) iInfoTouchDown = 0;

		TouchedIcon = null;

		CondiInfoTr.gameObject.SetActive(false);
	}

	public void ViewBossName(string Name){
		BossNameMass.SetActive(true);
		BossName.text = TextManager.Instance.GetText(Name);

		BossNameMass.transform.localScale = new Vector3(1.5f, 1.2f, 1.0f);
		LeanTween.scale (BossNameMass, Vector3.one, 0.5f);

		BossTitle.rectTransform.anchoredPosition = new Vector2 (-450f, BossTitle.rectTransform.anchoredPosition.y);
		LeanTween.alpha (BossTitle.GetComponent<RectTransform>(), 1.0f, 0.2f).setDelay(0.5f).setEase(LeanTweenType.easeOutExpo);
		LeanTween.move (BossTitle.rectTransform, new Vector3 (-200f, BossTitle.rectTransform.anchoredPosition.y, 0), 1.0f).setDelay(0.5f).setEase(LeanTweenType.easeOutQuart);

		BossName.rectTransform.anchoredPosition = new Vector2 (200f, BossName.rectTransform.anchoredPosition.y);
		LeanTween.value (BossName.gameObject, 0f, 1f, 0.2f).setOnUpdate((float alpha)=>{BossName.color = new Color(1,1,1,alpha);});
		LeanTween.move (BossName.rectTransform, new Vector3 (-50f, BossName.rectTransform.anchoredPosition.y, 0), 1.0f).setDelay(0.5f).setEase(LeanTweenType.easeOutQuart);
	}

	public void HideBossName(){
		BossNameMass.SetActive(false);
	}

	public void ShowWarning(Transform tParent){
		if(BossIndicator.activeSelf) return;

		if(_cBattle.TutorialCheckType == 1) return;

		BossIndicator.SetActive(true);
		BossIndiVFX = Instantiate (BossIndiEff) as GameObject;
		BossIndiVFX.transform.SetParent(tParent);
		BossIndiVFX.transform.localPosition = Vector3.zero;

		BossIndicator.transform.localScale = Vector3.one*0.7f;
		LeanTween.scale(BossIndicator.GetComponent<RectTransform>(), Vector3.one*1.4f, 0.3f).setLoopPingPong();

		for(int i=0; i<_acHeroes.Length; i++)
		{
			heroViewer[i].SetShowBreakSkill();
		}
	}

	public void HideWarning(){
		if (BossIndiVFX != null) {
			LeanTween.cancel (BossIndicator);
			Destroy (BossIndiVFX);
			BossIndicator.SetActive (false);

			for(int i=0; i<_acHeroes.Length; i++)
			{
				heroViewer[i].SetHideBreakSkill();
			}
		}
	}

	public void SetActive(bool b){
		Mass.SetActive(b);
		if (b) {
			BossNameMass.SetActive (false);
		} else {
			HideTutorialMsg ();
		}
	}

	public void SetHeroes(BattleCharacter[] tHeroes)
	{
		_acHeroes = tHeroes;
	}

	public void SetEnemys(BattleCharacter[] tEnemys)
	{
		_acEnemys = tEnemys;
	}

	public void initHeroView()
	{
		//cameraChanger = new CameraChanger (objCameraChangerLayout, _acHeroes[0]);

		for (int i = 0; i < objHeroViewerLayout.Length; i++) {
			objHeroViewerLayout [i].SetActive (false);
		}

		heroViewer = new HeroViewer[objHeroViewerLayout.Length];
		int uiIndex = 0;
		for(int idx=0; idx<_acHeroes.Length; idx++)
		{
			if (_acHeroes [idx] == null) continue;

			int ele = _acHeroes[idx].u1Element;
			string hpBarName = "Sprites/Battle/Battle_02.battle_ui_hp_hero_" + ele;
			if (_acHeroes [idx].bSupport) {
				if(idx < _acHeroes.Length-1 && _acHeroes.Length <= idx+1) objHeroViewerLayout [3].GetComponent<RectTransform> ().anchoredPosition = objHeroViewerLayout [4].GetComponent<RectTransform> ().anchoredPosition;
				if(uiIndex < 3) uiIndex = 3;
				hpBarName += "_sup";
			}

			heroViewer[idx] = new HeroViewer(objHeroViewerLayout[uiIndex], _acHeroes[idx], idx, AtlasMgr.Instance.GetSprite("Sprites/Battle/Battle_02.battle_ui_class_ele_"+ele),
				AtlasMgr.Instance.GetSprite(hpBarName), _acHeroes [idx].bSupport);

			heroViewer [idx].Reset ();

			objHeroViewerLayout [uiIndex].SetActive (true);
			uiIndex++;
		}
	}

	public void initEnemyView()
	{
		//cameraChanger = new CameraChanger (objCameraChangerLayout, _acHeroes[0]);

		for (int i = 0; i < objEnemyViewerLayout.Length; i++) {
			objEnemyViewerLayout [i].SetActive (false);
		}

		enemyViewer = new EnemyViewer[objEnemyViewerLayout.Length];
		int uiIndex = 0;
		for(int idx=0; idx<_acEnemys.Length; idx++)
		{
			if (_acEnemys [idx] == null) continue;

			int ele = _acEnemys[idx].u1Element;
			string hpBarName = "Sprites/Battle/Battle_02.battle_ui_hp_hero_" + ele;

			enemyViewer[idx] = new EnemyViewer(objEnemyViewerLayout[uiIndex], _acEnemys[uiIndex].cCharacter.cLevel.u2Level, AtlasMgr.Instance.GetSprite("Sprites/Battle/Battle_02.battle_ui_class_ele_"+ele),
				AtlasMgr.Instance.GetSprite(hpBarName));

			enemyViewer [idx].Reset ();

			objEnemyViewerLayout [uiIndex].SetActive (true);
			uiIndex++;
		}
	}

	void UpdateCoolTime(){
		for(int i=0; i<_acHeroes.Length; i++)
		{
			heroViewer[i].SetCoolPer();
			heroViewer[i].SetCoolText();
		}
	}

	public void ShowCombo(Byte Ele, Byte u1Combo){
		if (u1Combo <= 0) {
			ComboAnim.enabled = false;
			ComboAnim.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (800, 205);
			ComboGroup.SetActive (false);
			return;
		}

		if (u1ComboCount == u1Combo)
			return;

		string ComboName = "Ani_UI_Battle_SkillCombo";
		string AddAnim = "Ani_UI_Battle_SkillCombo_Add";

		ComboAnim.enabled = true;

		if (u1Combo == 1) {
			ComboAnim.Play (ComboName, 0, 0f);
			ComboImg.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.element_icon_" + Ele);
			ComboImgWhite.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.element_icon_" + Ele);
			ComboImg.SetNativeSize();
			ComboImgWhite.SetNativeSize();
			ComboDamageOutline.effectColor = EquipmentItem.equipElementColors2 [Ele];
		} else {
			ComboAnim.Play (AddAnim, 0, 0f);
		}
		u1ComboCount = u1Combo;
		ComboCount.text = "x"+u1Combo;
		ComboDamage.text = string.Format (TextManager.Instance.GetText ("battle_combo_damage_per"), LegionInfoMgr.Instance.MaxComboAddPerDamge * u1Combo);
	}

	public void ForceCombo(Byte Ele, Byte u1Combo){
		string ComboName = "Ani_UI_Battle_SkillCombo";

		ComboAnim.Play (ComboName, 0, 0f);
		ComboImg.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.element_icon_" + Ele);
		ComboImgWhite.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.element_icon_" + Ele);
		ComboImg.SetNativeSize();
		ComboImgWhite.SetNativeSize();
		ComboDamageOutline.effectColor = EquipmentItem.equipElementColors2 [Ele];

		ComboCount.text = "x"+u1Combo;
		Debug.LogError (u1Combo);
		ComboDamage.text = string.Format (TextManager.Instance.GetText ("battle_combo_damage_per"), LegionInfoMgr.Instance.MaxComboAddPerDamge * u1Combo);
	}

	public void SetChainState(int charIdx, int skillIdx){
		heroViewer[charIdx].SetChain (skillIdx, true);
	}

	public void RemoveChainState(){
		for (int i=0; i<_acHeroes.Length; i++) {
			heroViewer [i].SetDechainAll ();
		}
	}

	public void SetMonsterHp(BattleCharacter target){
		if(target == null){
			DisableMonsterHp();
			return;
		}

		if (_cBattle.eGameStyle == GameStyle.Guild)
			return;

		if(target == cTarget) return;

		cTarget = target;

		InitConditionIconForMonster();
		AddConditionIconForMonster();

		HpBar.SetActive(true);

//		int barIdx = -1;
//		if(cTarget.cCharacter.cClass.u1MonsterType == 1) barIdx = 0;
//		else if(cTarget.cCharacter.cClass.u1MonsterType == 2) barIdx = 2;
//		else barIdx = 1;
//
//		if (currentBarIdx != barIdx) {
//			currentBarIdx = barIdx;
//			HpBorder.sizeDelta = new Vector2(afHpBarSize[barIdx],HpBorder.sizeDelta.y);
//		}

		currentBarIdx = 0;

		int variIdx = target.u1Element-1;

		if(variIdx < 0) variIdx = 0;

		HpGage.sprite = HpVari[variIdx];
		NextGage.sprite = HpVari[variIdx];
		//TargetName.color = EquipmentItem.equipElementColors[variIdx+1];
		string AttackType = "";
		if (cTarget.cCharacter.cClass.u1BasicAttackElement == 1)
			AttackType = TextManager.Instance.GetText ("phy");
		else if(cTarget.cCharacter.cClass.u1BasicAttackElement == 2)
			AttackType = TextManager.Instance.GetText ("mag");
		
		TargetName.text = "Lv."+cTarget.cCharacter.cLevel.u2Level+" ["+AttackType+"] "+TextManager.Instance.GetText(cTarget.cCharacter.cClass.sName);

		float hp = SetHpGage();

		DisableTime = 1f;
	}

	void InitConditionIconForMonster(){
		for (int i=0; i<TargetCondis.Count; i++) {
			TargetCondis [i].DestroyMe ();
		}
		TargetCondis.Clear ();
	}

	void AddConditionIconForMonster(){
		for (int i=0; i<cTarget.cCondis.lstConditions.Count; i++) {
			AddConditionMonster(cTarget, cTarget.cCondis.lstConditions[i]);
		}
	}
	
	float SetHpGage(){
		float bossHpCount = 1f;
		//if(currentBarIdx == 2) bossHpCount = 3f;
		
		float per = cTarget.GetHPPer();
		HpGage.fillAmount = per;

		if (cTarget.isDead) {
			beforePer = 0;
			HpText.text = "0 / " + cTarget.cBattleStatus.GetStat(1).ToString ("#,###,##0");
//			HpGage.transform.localScale = new Vector3(0,0,0);
		} else {
			beforePer = per;
			HpText.text = cTarget.u4HP.ToString ("#,###,##0") + " / " + cTarget.cBattleStatus.GetStat(1).ToString ("#,###,##0");
//			if(per == 1.0f) HpGage.transform.localScale = new Vector3(1,1,1);
//			else HpGage.transform.localScale = new Vector3((beforePer%(1f/bossHpCount))*bossHpCount,1,1);
		}
		
//		return (beforePer%(1f/bossHpCount))*bossHpCount;

		return per;
	}
	
	void DisableMonsterHp(){
		HpBar.SetActive(false);
		currentBarIdx = -1;
		TargetName.text = "";
		DisableTime = 1f;
	}
	
	void UpdateMonsterHp(){
		float lastPer = SetHpGage();

		if (lastPer < NextGage.fillAmount) {
			float dec = Mathf.Lerp(NextGage.fillAmount, lastPer, 0.1f);
			NextGage.fillAmount = dec;
		}else if(lastPer > NextGage.fillAmount){
			NextGage.fillAmount = lastPer;
		}
		
		if (beforePer <= 0f) {
			DisableTime -= Time.deltaTime;
			if(DisableTime <= 0){
				DisableMonsterHp();
			}
		}
	}
	
	private void initTimer()
	{
		if (Legion.Instance.cTutorial.au1Step[0] != Server.ConstDef.LastTutorialStep) {
			objTimerLayout.SetActive (false);
			timer = new LimitTimer(objTimerLayout);
		} else {
			if (_cBattle.eGameStyle == GameStyle.Guild) {
				objTimerLayout.SetActive (false);
				objGuildTimerLayout.SetActive (true);
				timer = new LimitTimer (objGuildTimerLayout);
			} else {
				objTimerLayout.SetActive (true);
				timer = new LimitTimer(objTimerLayout);
			}
		}
		//DebugMgr.Log("InitTImer : " + _cBattle.phaseLimitTime);
		timer.SetTime(_cBattle.phaseLimitTime);
	}
	
	private void initAutoButton()
	{
		autoButton = new AutoButton(objAutoButtonLayout);
	}
	
	private void initDmgText()
	{
		dmgTextQueue = new DamageText[DMG_TEXT_QUEUE_MAX];
		for(int i=0; i<DMG_TEXT_QUEUE_MAX; i++)
		{
			dmgTextQueue[i] = new DamageText(objDmgTextParent);
		}
	}
	
	public void FirstShowAll()
	{
		HideMsgBattleStart();
        PopupManager.Instance.showLoading = false;
	}

	public void ShowMsgBattleStart()
	{
		if (_cBattle.eGameStyle == GameStyle.League) {
			LeagueStartMsg.SetActive (true);
			FadeBox.enabled = true;
			LeanTween.alpha (FadeBox.rectTransform, 0.8f, 0.2f);

			LeagueMapNameTxt.text = TextManager.Instance.GetText (Legion.Instance.SelectedLeague.getField ().sFieldName);
			LeagueHomeTeamTxt.text = Legion.Instance.sName;
			LeagueAwayTeamTxt.text = UI_League.Instance.sEnemyName;
			LeanTween.moveLocalX (LeagueMsgTopLine.gameObject, 200, 1.5f);
			LeanTween.moveLocalX (LeagueMsgBotLine.gameObject, -200, 1.5f);
		} else if (_cBattle.eGameStyle == GameStyle.Guild) {
			if (_cBattle.u1Round == 1) {
				LeagueHomeTeamTxt.text = GuildInfoMgr.Instance.cGuildMemberInfo.strGuildName;
				LeagueAwayTeamTxt.text = GuildInfoMgr.Instance.cGuildMatchData.strMatchingGuildName;

				LeagueVSMsg.SetActive (true);
				LeanTween.moveLocalX (LeagueHomeTeam, 0, 0.3f).setEase (LeanTweenType.easeOutCubic);
				LeanTween.moveLocalX (LeagueAwayTeam, 0, 0.3f).setDelay (0.1f).setEase (LeanTweenType.easeOutCubic);

				LeanTween.alpha (LeagueVS_V.rectTransform, 1f, 0.1f).setDelay (0.4f);
				LeanTween.scale (LeagueVS_V.rectTransform, Vector3.one * 0.9f, 0.15f).setDelay (0.4f).setEase (LeanTweenType.easeOutBack);
				LeanTween.alpha (LeagueVS_S.rectTransform, 1f, 0.1f).setDelay (0.5f);
				LeanTween.scale (LeagueVS_S.rectTransform, Vector3.one * 0.9f, 0.15f).setDelay (0.5f).setEase (LeanTweenType.easeOutBack);
			}
		} else {
			StartMsg.SetActive(true);
			StartMsg.GetComponent<Animator>().enabled = true;

			if (Legion.Instance.SelectedStage.u2ID == Tutorial.TUTORIAL_STAGE_ID)
				ChapterTxt.text = TextManager.Instance.GetText ("mark_first_battle");
			else if (Legion.Instance.SelectedStage.u2ChapterID == 0)
				ChapterTxt.text = "";
			else
				ChapterTxt.text = TextManager.Instance.GetText (StageInfoMgr.Instance.dicChapterData [Legion.Instance.SelectedStage.u2ChapterID].strName);
			MapNameTxt.text = TextManager.Instance.GetText (StageInfoMgr.Instance.dicStageData [Legion.Instance.u2SelectStageID].sName);
		}
	}

	public void HideMsgBattleStart()
	{
		if (_cBattle.eGameStyle == GameStyle.League) {
			LeagueStartMsg.SetActive (false);

			StartCoroutine (ShowVSMsgDirection ());
		} else if (_cBattle.eGameStyle == GameStyle.Guild) {
			LeagueVSMsg.SetActive (false);

			StartCoroutine (ShowRoundMsgDirection ());
		} else {
			StartMsg.SetActive(false);
			StartMsg.GetComponent<Animator>().enabled = false;
		}
	}

	public void ShowMsgPhaseStart(int phaseNum)
	{
	}

	IEnumerator ShowVSMsgDirection(){
		LeagueVSMsg.SetActive (true);
		LeanTween.moveLocalX (LeagueHomeTeam, 0, 0.3f).setEase(LeanTweenType.easeOutCubic);
		LeanTween.moveLocalX (LeagueAwayTeam, 0, 0.3f).setDelay (0.1f).setEase(LeanTweenType.easeOutCubic);

		LeanTween.alpha (LeagueVS_V.rectTransform, 1f, 0.1f).setDelay (0.4f);
		LeanTween.scale (LeagueVS_V.rectTransform, Vector3.one*0.9f, 0.15f).setDelay (0.4f).setEase(LeanTweenType.easeOutBack);
		LeanTween.alpha (LeagueVS_S.rectTransform, 1f, 0.1f).setDelay (0.5f);
		LeanTween.scale (LeagueVS_S.rectTransform, Vector3.one*0.9f, 0.15f).setDelay (0.5f).setEase(LeanTweenType.easeOutBack);

		yield return new WaitForSeconds (1.5f);
		LeagueVSMsg.SetActive (false);
		StartCoroutine (ShowFightMsgDirection ());
	}

	IEnumerator ShowRoundMsgDirection(){
		LeagueStartMsg.SetActive (true);
		FadeBox.enabled = true;
		LeanTween.alpha (FadeBox.rectTransform, 0.8f, 0.2f);

		LeagueMsgTopLine.transform.localPosition = new Vector3(-100f, LeagueMsgTopLine.transform.localPosition.y, LeagueMsgTopLine.transform.localPosition.z);
		LeagueMsgBotLine.transform.localPosition = new Vector3(100f, LeagueMsgBotLine.transform.localPosition.y, LeagueMsgBotLine.transform.localPosition.z);

		LeagueMapNameTxt.text = "Round "+_cBattle.u1Round.ToString();
		LeanTween.moveLocalX (LeagueMsgTopLine.gameObject, 200, 1.5f);
		LeanTween.moveLocalX (LeagueMsgBotLine.gameObject, -200, 1.5f);

		yield return new WaitForSeconds (1.5f);
		LeagueStartMsg.SetActive (false);
		StartCoroutine (ShowFightMsgDirection ());
	}

	IEnumerator ShowFightMsgDirection(){
		LeagueFightMsg.SetActive (true);
		for (int i = 0; i < FightMsgImgs.Length; i++) {
			LeanTween.alpha (FightMsgImgs [i].rectTransform, 1f, 0.05f).setDelay(0.03f*(float)i);
			LeanTween.alpha (FightMsgImgs [i].rectTransform, 0f, 0.1f).setDelay(1f+ 0.05f*(float)i);
			LeanTween.scale (FightMsgImgs [i].rectTransform, Vector3.one, 0.1f).setDelay(0.03f*(float)i);
		}
		LeanTween.alpha (FadeBox.rectTransform, 0f, 0.2f).setDelay(1.3f);

		yield return new WaitForSeconds (1.7f);
		LeagueFightMsg.SetActive (false);
		FadeBox.enabled = false;
	}

	public void ShowMsgBattleEnd(bool win)
	{
		if(pause !=null) pause.SetActive(false);

		FadeBox.enabled = true;
		BossIndicator.SetActive(false);
		ResultObj.SetActive(true);
		ResultMark.Play("Ani_UI_Battle_End_BG");

		if (win) {
			StartCoroutine (BattleEndWin ());
		} else {
            StageInfoMgr.Instance.RepeatItemInfoDelete();
			Mass.SetActive(false);
			StartCoroutine (BattleEndLose ());
		}
	}

	void CheckVib(bool bWin){
		if (!ObscuredPrefs.GetBool ("BattlePushToggle", true)) {
			return;
		}

		if (Legion.Instance.AUTOCONTINUE) {
			if (bWin)
				return;
		}

		Handheld.Vibrate ();
	}

	IEnumerator BattleEndLose()
	{
		yield return new WaitForSeconds(2f);

//		if (_cBattle.eGameStyle == GameStyle.League) {
//			if (_cBattle.eResultType == Battle.BATTLE_RESULT_TYPE.AllKilled) {
//				DefeatMsg.SetActive (true);
//			} else {
//				DrawMsg.SetActive (true);
//			}
//		}else{
			CheckVib(false);
			DefeatMsg.SetActive (true);
//		}
//
		float alpha = 0;
		while (alpha < 0.5f) {
			alpha += Time.deltaTime;
			if(alpha > 0.5f) alpha = 0.5f;
			FadeBox.color = new Color (1, 1, 1, alpha);
			yield return new WaitForSeconds (Time.deltaTime);
		}

		bResult = true;

		if (_cBattle.eGameStyle == GameStyle.League) {
			cCamScript.enabled = false;
			StartCoroutine (ShowLeagueResultScreen ());
		} else if (_cBattle.eGameStyle == GameStyle.Guild) {
			cCamScript.enabled = false;
			StartCoroutine (ShowGuildResultScreen ());
		} else {
			if(Legion.Instance.SelectedStage.u2ChapterID == 0)
                StartCoroutine(GoToLobby());
			else
                ResultBtnMass.SetActive (true);
		}
		//DefeatMsg.GetComponent<Animator>().enabled = true;
//		yield return new WaitForSeconds(2f);

	}

//	void CheckAndGoToDefeatScene(){
//		string[] randomScenes = new string[4]{"SelectStageScene", "SelectStageScene", "ForgeScene", "LobbyScene"};
//
//		Legion.Instance.bStageFailed = true;
//
//		sChangeSceneName = randomScenes[UnityEngine.Random.Range(0,randomScenes.Length)];
//		if (sChangeSceneName == "LobbyScene") {
//			FadeEffectMgr.Instance.QuickChangeScene(MENU.SHOP);
//			Legion.Instance.AwayBattle ();
//		}else{
//			StartCoroutine (ChangeSceneFade ());
//		}
//	}
    protected void EventReloadResult(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_DISPATCH_RESULT, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
        }
    }
	IEnumerator BattleEndWin()
	{
		CheckVib(true);
		VictoryMsg.SetActive (true);
		//if (_cBattle.eGameStyle == GameStyle.League) VictoryMsg.GetComponent<RectTransform>().anchoredPosition = new Vector2(37, 104);

		float alpha = 0;
		while (alpha < 0.5f) {
			alpha += Time.deltaTime;
			if(alpha > 0.5f) alpha = 0.5f;
			FadeBox.color = new Color (1, 1, 1, alpha);
			yield return new WaitForSeconds (Time.deltaTime);
		}
		
		yield return new WaitForSeconds(2f);

		if (_cBattle.eGameStyle == GameStyle.League) {
			cCamScript.enabled = false;
			StartCoroutine (ShowLeagueResultScreen ());
		} else if (_cBattle.eGameStyle == GameStyle.Guild) {
			cCamScript.enabled = false;
			StartCoroutine (ShowGuildResultScreen ());
		} else {
			StartCoroutine (ShowResultScreen ());
		}
	}

	IEnumerator ShowResultScreen(){
        bResult = true;
		if(StageInfoMgr.Instance.u1EventIDCount > 0)
		{
			for(int i=0; i<StageInfoMgr.Instance.arrEventID.Length; i++)
			{
				if(!EventInfoMgr.Instance.dicEventReward.ContainsKey(StageInfoMgr.Instance.arrEventID[i]))
				{
					StageInfoMgr.Instance.bReloadEvent = false;
					PopupManager.Instance.ShowLoadingPopup(1);
					Server.ServerMgr.Instance.RequestEventReload(EventReloadResult);
					break;
				}
				else
				{
					StageInfoMgr.Instance.bReloadEvent = true;
					continue;
				}
			}
		}
		else
			StageInfoMgr.Instance.bReloadEvent = true;
		
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		ObjMgr.Instance.RemoveMonsterPool();
		menuCanvas.SetActive(false);
		resultCanvas.SetActive(true);
		SetResultPosition(_cPlayerCrew);
		cCamScript.SetResultCam();
		
		objResultWin.GetComponent<UI_BattleResult_Win>().Show(_cPlayerCrew, _cBattle.eGameStyle == GameStyle.League);
		FadeEffectMgr.Instance.FadeIn();
	}

	IEnumerator ShowLeagueResultScreen(){
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		bResult = true;
		Legion.Instance.AwayBattle ();
		AssetMgr.Instance.SceneLoad("ALeagueResultScene", false);
	}

	IEnumerator ShowGuildResultScreen(){
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		bResult = true;
		Legion.Instance.AwayBattle ();
		AssetMgr.Instance.SceneLoad("GuildResultScene", false);
	}

	IEnumerator GoToLobby()
	{
		PopupManager.Instance.CloseLoadingPopup();
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		Legion.Instance.AwayBattle ();
        if(Legion.Instance.bAdventoStage == 2)
        {
            int index = Legion.Instance.cEvent.openStageIds.FindIndex(cs => cs.u2EventID == Legion.Instance.SelectedBossRushStage.u2EventID 
                && cs.u2StageID == Legion.Instance.SelectedBossRushStage.u2StageID);
            if(index >= 0)
            {
                Legion.Instance.cEvent.openStageIds.Remove(Legion.Instance.cEvent.openStageIds[index]);
                Legion.Instance.cEvent.openStageIds.Add(Legion.Instance.SelectedBossRushStage);
            }
            else
            {
                Legion.Instance.cEvent.openStageIds.Add(Legion.Instance.SelectedBossRushStage);
            }
            AssetMgr.Instance.SceneLoad("BossRushScene");
        }
        else
		    AssetMgr.Instance.SceneLoad("LobbyScene", false);
		StageInfoMgr.Instance.LastPlayStage = -1;
	}

	void SetResultPosition(BattleCrew btCrew){
		Vector3 startPos = new Vector3(_cBattle.cCurrentBattleField.cCrewEnd.X, 0f, _cBattle.cCurrentBattleField.cCrewEnd.Y);
		
		int left = 1;
		float z = 0;
		for (int i=0; i<btCrew.acCharacters.Length; i++) {
			GameObject cObject = btCrew.acCharacters[i].cObject;
			if(!cObject.activeSelf) cObject.SetActive(true);
			left *= -1;
			if(i > 0) z = -0.15f;
			Quaternion rot = Quaternion.Euler(new Vector3(0, _cBattle.cCurrentBattleField.cCrewEnd.Dir, 0));
			cObject.transform.position = startPos + (rot * new Vector3(Mathf.Round((i+1)/2)*0.7f*left, 0, z));
			rot = Quaternion.Euler(0, _cBattle.cCurrentBattleField.cCrewEnd.Dir+(Mathf.Round((i+1)/2)*20f*left), 0);
			cObject.transform.rotation = rot;
			btCrew.acCharacters[i].PlayWin();
		}
	}

	public void EnableAllSkillButton()
	{
		for(int idx=0; idx<_acHeroes.Length; idx++)
		{
			heroViewer[idx].SkillSetActive(true);
		}
		//setSkillButton();
	}
	
	public void DisableAllSkillButton()
	{
		for(int idx=0; idx<_acHeroes.Length; idx++)
		{
			heroViewer[idx].SkillSetActive(false);
		}
	}

	public void FocusSkillButton(int charIdx, int skillIdx)
	{
		GameObject eff = Instantiate (FocusEff) as GameObject;
		eff.name = "FocusEff";
		eff.transform.SetParent (heroViewer [charIdx].GetSkillButtonTrans(skillIdx));
		eff.transform.localPosition = Vector3.zero;
		heroViewer [charIdx].SetFocusObj (skillIdx);
	}
	
	public void AddGold(int gold)
	{
	}
	
	public void AddBox(int box)
	{
	}

	int FindCharacterIndex(BattleCharacter btChar){
		for (int i = 0; i < _acHeroes.Length; i++) {
			if(_acHeroes[i] == btChar) return i;
		}

		return 0;
	}

	public void SetDefault(int idx)
	{
		heroViewer [idx].SetDefault ();
	}
	public void SetWarning(int idx)
	{
		heroViewer [idx].SetWarning ();
	}
	public void SetRebirth(int idx)
	{
		heroViewer [idx].SetDefault ();
		CheckSkillOrder ();
	}
	public void SetDeath(int teamIdx, int charIdx)
	{
		switch(teamIdx){
		case 0:
			if (reserveIndex [0] == charIdx)
				RemoveReserve ();
			
			heroViewer [charIdx].SetDead ();
			CheckSkillOrder ();
			break;
		case 1:
			_cBattle.cCont.cAIController.SetEnemyUserQueue ();
			if (_cBattle.eGameStyle == GameStyle.Guild) {
				enemyViewer [charIdx].SetDead ();
			}
			break;
		}
	}

	public void DuplicateDmg(Vector3 cObjectPos, String text, DAMAGE_TEXT_TYPE textType, int txtSize = 38)
	{
		//damagetext height
		Vector3 tmpPos = new Vector3(cObjectPos.x, cObjectPos.y, cObjectPos.z);
		Vector3 textPos = cUICam.ViewportToWorldPoint(cMainCam.WorldToViewportPoint(tmpPos));
		dmgTextQueue[dmgTextPointer++].ShowDamage(textPos, text, textType, Color.black, 0.15f, txtSize);

		if(dmgTextPointer >= DMG_TEXT_QUEUE_MAX)
		{
			dmgTextPointer = 0;
		}
	}

	public void DuplicateDmg(Vector3 cObjectPos, String text, DAMAGE_TEXT_TYPE textType, Color outline)
	{
		//damagetext height
		Vector3 tmpPos = new Vector3(cObjectPos.x, cObjectPos.y, cObjectPos.z);
		Vector3 textPos = cUICam.ViewportToWorldPoint(cMainCam.WorldToViewportPoint(tmpPos));
		dmgTextQueue[dmgTextPointer++].ShowDamage(textPos, text, textType, outline, 0.15f);

		if(dmgTextPointer >= DMG_TEXT_QUEUE_MAX)
		{
			dmgTextPointer = 0;
		}
	}

	public UI_ConditionIcon AddCondition(BattleCharacter battleChar, ConditionInfo info){
		if (!battleChar.bHero) {
			return null;
		}

		GameObject temp = Instantiate(CondIconObj) as GameObject;
		UI_ConditionIcon tempScript = temp.GetComponent<UI_ConditionIcon>();
		tempScript.SetInfo(info, true);

		EventTrigger trigger = temp.GetComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener( (eventData) => { battleChar.cBattle.battleUIMgr.OnClickCondition(FindCharacterIndex(battleChar), info.u2ID, temp); } );
		trigger.triggers.Add(entry);

		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.PointerUp;
		entry2.callback.AddListener( (eventData) => { battleChar.cBattle.battleUIMgr.DisableConditionInfo(temp); } );
		trigger.triggers.Add(entry2);
		
		if(battleChar.iTeamIdx == 0) heroViewer [FindCharacterIndex (battleChar)].AddCondition(temp);

		return tempScript;
	}

	public UI_ConditionIcon AddConditionMonster(BattleCharacter battleChar, Condition info){
		if(cTarget != battleChar) return null;

		GameObject temp = Instantiate(CondIconObj) as GameObject;
		UI_ConditionIcon tempScript = temp.GetComponent<UI_ConditionIcon>();
		tempScript.SetInfo(info.cInfo, false);
		info.SetIcon(tempScript);

		temp.transform.SetParent(TargetCondiParent);
		temp.transform.localScale = Vector3.one;
		temp.transform.localPosition = Vector3.zero;

		EventTrigger trigger = temp.GetComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener( (eventData) => { battleChar.cBattle.battleUIMgr.OnClickCondition(-1, info.cInfo.u2ID, temp); } );
		trigger.triggers.Add(entry);

		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.PointerUp;
		entry2.callback.AddListener( (eventData) => { battleChar.cBattle.battleUIMgr.DisableConditionInfo(temp); } );
		trigger.triggers.Add(entry2);

		TargetCondis.Add(tempScript);

		return tempScript;
	}

	public void ShowComboBomb(){
		ComboAnim.Play ("Ani_UI_Battle_SkillCombo_Bomb",0,0f);
		RemoveChainState();

		GameObject temp = Instantiate(ComboBombObj) as GameObject;
		temp.transform.SetParent(ComboBombParent);
		temp.transform.localScale = Vector3.one;
		temp.transform.localPosition = Vector3.zero;
	}

	public void ShowSkillChain(bool bBuff){
		if (bBuff) {
			GameObject temp = Instantiate(SkillChainEffBuff) as GameObject;
			temp.transform.SetParent(ComboBombParent);
			temp.transform.localScale = Vector3.one;
			temp.transform.localPosition = Vector3.zero;
		}else{
			GameObject temp = Instantiate(SkillChainEffDebuff) as GameObject;
			temp.transform.SetParent(ComboBombParent);
			temp.transform.localScale = Vector3.one;
			temp.transform.localPosition = Vector3.zero;
		}
	}

	public void ShowHeroSkillBreak(int idx){
		heroViewer [idx].SetSkillBreak ();
	}

	public void ShowBossSkillBreak(Vector3 pos){
		GameObject objText = GameObject.Instantiate(breakTextPref);
		objText.transform.SetParent(objDmgTextParent);

		Vector3 textPos = cUICam.ViewportToWorldPoint(cMainCam.WorldToViewportPoint(pos));
		objText.transform.position = textPos;
		objText.transform.localScale = Vector3.one*2f;
	}
	
	public void RemoveReserve()
	{
		objSkillReserve.SetActive(false);
		_cBattle.cCont.bReserve = false;
		reserveIndex[0] = -1;
		reserveIndex[1] = -1;
	}
	public void StopDir(){}
	public void Reserve(bool enable, int charIdx, int idx){
		if (_acHeroes [charIdx].isDead)
			return;

		objSkillReserve.transform.SetParent (heroViewer [charIdx].gameObject.transform);
		objSkillReserve.transform.localPosition = heroViewer [charIdx].GetSkillButtonPos (idx)*0.8f + new Vector3(80,30,0);
		objSkillReserve.transform.SetSiblingIndex (2);
		objSkillReserve.SetActive(enable);
		reserveIndex[0] = charIdx;
		reserveIndex[1] = idx;
		_cBattle.cCont.bReserve = true;
	}

	public void PlaySkillUI(){
		CheckSkillOrder ();
	}
	
	public void initSelecteCharacter()
	{
		if(_acHeroes[0].isDead) return;
		if(_selectedHero != -1) _acHeroes[_selectedHero].SetFocus(false);
		_acHeroes[0].SetFocus(true);
		_selectedHero = 0;
	}

	public void SetHeroDeselect(int focus)
	{
		for (int i = 0; i < _acHeroes.Length; i++) {
			if (!_acHeroes [i].isDead) {
				heroViewer [i].Deselect ();
			}
		}

		if (focus != _selectedHero) {
			if (_selectedHero != -1 && _selectedHero < _acHeroes.Length) {
				_acHeroes [_selectedHero].SetFocus (false);
			}
			_acHeroes [focus].SetFocus (true);
		}

		_selectedHero = -1;
	}
	
	public void OnSelectCharaceter(int charIdx)
	{
		if (charIdx == -1) {
			return;
		}
		if(_acHeroes[charIdx].isDead) return;

		if (cCamScript.bFixed)
			return;

		if (charIdx == _selectedHero) {
			if (cCamScript.ChangeCameraType ()) {
				heroViewer[charIdx].Deselect ();
			} else {
				heroViewer[charIdx].Select();
			}
			return;
		}
		if (_selectedHero == -1) {
			if (cCamScript.bCrew) {
				cCamScript.ChangeCameraType ();
				_acHeroes [charIdx].SetFocus (true);
				cCamScript.SetFocus (Convert.ToByte (charIdx));
				heroViewer [charIdx].Select ();
				_selectedHero = charIdx;
			}
		} else {
			if (cCamScript.bCrew) {
				_acHeroes [charIdx].SetFocus (true);
				cCamScript.SetFocus (Convert.ToByte (charIdx));
				_selectedHero = charIdx;
			} else {
				if (_selectedHero != -1)
					_acHeroes [_selectedHero].SetFocus (false);
				_acHeroes [charIdx].SetFocus (true);

				cCamScript.SetFocus (Convert.ToByte (charIdx));

				if (_selectedHero > -1)
					heroViewer [_selectedHero].Deselect ();
				heroViewer [charIdx].Select ();
				_selectedHero = charIdx;
				bStickTouch = false;
			}
		}
	}
	
	public void OnClickSkill(int charIndex, int buttonIndex)
	{
		if(_isSkillChange) return;

		if (_cBattle.TutorialCheckType == 1){
			if (_cBattle.TutorialCheckStep < 4) {
				return;
			}else{
				int focus = heroViewer [charIndex].SetFocusObj (-1);
				if (focus > -1) {
					if (heroViewer [charIndex].GetSkillButtonTrans (focus).FindChild ("FocusEff") != null) {
						Destroy (heroViewer [charIndex].GetSkillButtonTrans (focus).FindChild ("FocusEff").gameObject);
					}
				}
			}
		}



		if (!_cBattle.cCont.UseSkill (0, charIndex, buttonIndex)) {
			if(charIndex == reserveIndex[0] && buttonIndex == reserveIndex[1]) RemoveReserve();
			else Reserve (true, charIndex, buttonIndex);
		} else {
			RemoveReserve();
		}
	}

	public void OnMoveCharacter(BaseEventData eventData)
	{
		PointerEventData ptData = eventData as PointerEventData;

		Vector2 curPos = ptData.position + new Vector2(-84f, -84f);

		if (Vector2.Distance (firstJoystickPt, curPos) > 30f) {
			Joystick.anchoredPosition = curPos.normalized * 30f;
		} else {
			Joystick.anchoredPosition = curPos;
		}

		Vector2 nm = curPos.normalized;
		stickAngle = Angle(curPos);

		if (!bStickTouch) {
			_acHeroes [_selectedHero].SetMoveByBtn (stickAngle);
			bStickTouch = true;
		}
	}

	public static float Angle(Vector2 p_vector2)
	{
		if (p_vector2.x < 0)
		{
			return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
		}
		else
		{
			return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
		}
	}
	
	public void OnStopCharacter(BaseEventData eventData)
	{
		Joystick.anchoredPosition = firstJoystickPt;
		_acHeroes[_selectedHero].SetStopByBtn();
		bStickTouch = false;
	}

	public void OnClickAttack()
	{
		_acHeroes[_selectedHero].SetAttackByBtn();
	}

	public void OnClickAuto()
	{
		if (_cBattle.eBattleState != Battle.BATTLE_STATE.Battle) {
			return;
		}

		autoButton.Toggle();
		_cBattle.cCont.ChangeAuto (autoButton.IsActive);
		CheckSkillOrder ();
	}

	public void OnClickRepeat()
	{
		Legion.Instance.AUTOCONTINUE = repeatButton.isOn;
		objRepeating.SetActive (repeatButton.isOn);
		if (Legion.Instance.AUTOCONTINUE)
        {
            AtlasMgr.Instance.SetDefaultShader(repeatButton.GetComponent<Image>());
            SetTargetItemUIActive(true);
        }
		else
        {
            AtlasMgr.Instance.SetGrayScale(repeatButton.GetComponent<Image>());
            SetTargetItemUIActive(false);
        }
	}

	public void CheckSkillOrder(){
		List<skillQueue> lst = _cBattle.cCont.cAIController.GetAutoSkill ();
		for (int i = 0; i < lst.Count; i++) {
			if (i < _cBattle.cCont.cAIController.GetAutoIndex ()) {
				heroViewer [lst [i].charIdx].SetSkillOrder (lst [i].skillIdx, false, 0);
			} else {
				heroViewer [lst [i].charIdx].SetSkillOrder (lst [i].skillIdx, true, i+1);
			}
		}
	}
	
	private void UpdateHP()
	{
		for(int i=0; i<_acHeroes.Length; i++)
		{
            heroViewer[i].SetHp(Convert.ToUInt32(_acHeroes[i].cBattleStatus.GetStat(1)), _acHeroes[i].u4HP);
		}

		if(currentBarIdx > -1) UpdateMonsterHp();

		if (_cBattle.eGameStyle == GameStyle.Guild) {
			for (int i = 0; i < _acEnemys.Length; i++) {
				enemyViewer [i].SetHp (Convert.ToUInt32 (_acEnemys [i].cBattleStatus.GetStat (1)), _acEnemys [i].u4HP);
			}
		}
	}
	
	private void UpdateTimer()
	{
		timer.SetTime (_cBattle.phaseLimitTime);
	}
	
	private void UpdateSkillCoolActive()
	{
		for(int i=0; i<_acHeroes.Length; i++)
		{
			heroViewer[i].UpdateSkillCoolActive();
		}
	}

    public void SetRepeatTargetItemInfo()
    {
        if(StageInfoMgr.Instance.RepeatTargetItem == null)
        {
            TargetItemWindow.SetActive(false);
            return;
        }

        SetTargetItemUIActive(true);
        if (TargetItemWindow.activeSelf == false)
            return;

        Goods targetInfo = StageInfoMgr.Instance.RepeatTargetItem;
        TargetItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(targetInfo.u2ID).u2IconID);
        TargetItemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(targetInfo.u2ID));
        cTargetItemButton.SetButton(targetInfo.u1Type, targetInfo.u2ID);

        StringBuilder tempString = new StringBuilder();
        tempString.Append(StageInfoMgr.Instance.u4CurTargetItemCount).Append(" / ").Append(targetInfo.u4Count);
        TargetCount.text = tempString.ToString();
    }

    private void SetTargetItemUIActive(bool isActive)
    {
        if (isActive == true)
        {
            // 반복 전투가 비활성화 이거나 리그 모드 이거나 재료 수집 전투가 아니라면
            if (Legion.Instance.AUTOCONTINUE == false || 
                _cBattle.eGameStyle == GameStyle.League ||
                StageInfoMgr.Instance.RepeatTargetItem == null)
            {
                isActive = false;
            }
        }

        TargetItemWindow.SetActive(isActive);
    }

	public void SetGuildCrewIcon(int teamIdx, int crewIdx, int type)
	{
		acGuildLayout [teamIdx].SetCrewState (crewIdx, type);
	}

	public void RefreshCamera()
	{
		cCamScript.DisableDirectionCam ();
		_acHeroes[0].SetFocus(true);
		cCamScript.SetStartCamera ();
	}

    void Update()
	{
		if(!setUI) return;

		if (bStickTouch) {
			_acHeroes[_selectedHero].SetMoveByBtn(stickAngle);
		}

		UpdateHP();
		UpdateTimer();
		UpdateCoolTime();
		UpdateSkillCoolActive();
//		if(_isSkillChange) UpdateSkillChange();

		if (iInfoTouchDown > 0) {
			fInfoTouchTime += Time.fixedDeltaTime;
			if (fInfoTouchTime > 0.3f) {
				if(iInfoTouchDown == 1) SkillInfoTr.gameObject.SetActive (true);
				else if(iInfoTouchDown == 2) CondiInfoTr.gameObject.SetActive (true);
				iInfoTouchDown = 0;
			}
		}

		if (reserveIndex[0] > -1) {
			if(_cBattle.cCont.UseSkill (0, reserveIndex[0], reserveIndex[1])){
				RemoveReserve();
			}
		}
	}

	class CameraChanger
	{
		private Transform ImageGroup;
		private Image _imgFace;
		private Image _imgElement;

		private Text _txtName;
		private Text _txtLevel;

		private string sClassID;

		private GameObject _objCamChanger;
		public GameObject gameObject
		{
			get{ return _objCamChanger.gameObject; }
		}

		void InitComponent(GameObject camChanger)
		{
			_objCamChanger = camChanger;
			ImageGroup = _objCamChanger.transform.FindChild ("HeroViewBG");
			_imgFace = ImageGroup.FindChild("Face").GetComponent<Image>();
			_imgElement = ImageGroup.FindChild("Element").GetComponent<Image>();
			_txtName = ImageGroup.FindChild("Name").GetComponent<Text>();
			_txtLevel = ImageGroup.FindChild("Level").GetComponent<Text>();
		}

		public CameraChanger(GameObject camChanger, BattleCharacter btCharInfo)
		{
			InitComponent(camChanger);
			sClassID = btCharInfo.cCharacter.cClass.u2ID.ToString();
			_imgFace.sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon."+sClassID);
			_imgFace.SetNativeSize();

			_txtLevel.text = btCharInfo.cCharacter.cLevel.u2Level.ToString();

			SetName(btCharInfo.cCharacter.sName);
			SetElement(AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_"+btCharInfo.u1Element));

			camChanger.AddComponent<TutorialButton>().id = "CameraChange";
		}

		void SetElement(Sprite eleImg)
		{
			_imgElement.sprite = eleImg;
		}

		void SetName(string name)
		{
			_txtName.text = name;
		}

		public void SetCharacter(BattleCharacter btCharInfo){
			sClassID = btCharInfo.cCharacter.cClass.u2ID.ToString();
			_imgFace.sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon."+sClassID);
			_imgFace.SetNativeSize();

			_txtLevel.text = btCharInfo.cCharacter.cLevel.u2Level.ToString();

			SetName(btCharInfo.cCharacter.sName);
			SetElement(AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_"+btCharInfo.u1Element));
		}
	}
	
	class HeroViewer
	{
		enum BorderStyle{
			Default,
			Active,
			Selected,
			Warning,
			Dead
		}

		private GameObject _objHeroViewer;
		public GameObject gameObject
		{
			get{ return _objHeroViewer.gameObject; }
		}
//		private RectTransform _trHeroViewer;
//		public RectTransform transform
//		{
//			get{ return _trHeroViewer; }
//		}
		//	private GameObject objHpGauge;
		private Transform ImageGroup;
		private RectTransform SkillGroup;
		private Image _imgHpGauge;
		private Image _imgHpFade;
		private Image _imgRect;
		private Image _imgFace;
		private GameObject _objDead;
		private Image _imgEle;
		private Image _imgEleBorder;
		private Image _imgTriangle;

		private Text _txtLevel;

		private RectTransform _parentCondis;

		private CanvasGroup _txtSkillBreak;

		bool _bSelected;
		bool _bAlphaUp;
		bool _bWarning;
		bool _bWarnColorUp;
		bool _bActiving;

		bool _isDead;

		bool _bSupporter;

		List<BattleSkill> lstCurSkills;
		List<SkillButton> lstSkillButton;

		Animator Anim;

		private string sClassID;

		int focusIdx = -1;

		public HeroViewer(GameObject heroViewer, BattleCharacter btCharInfo, int idx, Sprite eleIcon, Sprite hpgage, bool bSupporter)
		{
			_bSupporter = bSupporter;
			InitComponent(heroViewer);
		
			if(!_bSupporter){
				Anim = _objHeroViewer.GetComponent<Animator> ();
				if(SkillGroup.childCount > 0){
					for(int i=0; i<SkillGroup.childCount; i++){
						Destroy(SkillGroup.GetChild(i).gameObject);
					}
				}
				InitSkill(btCharInfo, idx);
			}

			sClassID = btCharInfo.cCharacter.cClass.u2ID.ToString();
			_txtLevel.text = btCharInfo.cCharacter.cLevel.u2Level.ToString();

			_imgEle.sprite = eleIcon;

			_imgFace.sprite = AtlasMgr.Instance.GetSprite("Sprites/Battle/Battle_02.battle_ui_class_"+sClassID);

			_imgHpGauge.sprite = hpgage;
			_imgHpFade.sprite = hpgage;
		}

		void InitComponent(GameObject heroViewer)
		{
			_objHeroViewer = heroViewer;
//			_trHeroViewer = heroViewer.GetComponent<RectTransform>();

			_imgTriangle = _objHeroViewer.transform.FindChild ("HeroSkillBG").GetComponent<Image> ();

			ImageGroup = _objHeroViewer.transform.FindChild ("HeroInfo");
			_imgRect = ImageGroup.GetComponent<Image>();

			_imgHpGauge = ImageGroup.FindChild("Hp").GetComponent<Image>();
			_imgHpFade = ImageGroup.FindChild("FadeHp").GetComponent<Image>();

			_imgFace = ImageGroup.FindChild ("Face").GetComponent<Image> ();
			_objDead = ImageGroup.FindChild ("Dead").gameObject;

			_imgEle = ImageGroup.FindChild ("Ele").GetComponent<Image> ();
			_imgEleBorder = ImageGroup.FindChild ("EleBorder").GetComponent<Image> ();

			_txtLevel = ImageGroup.FindChild ("Level").GetComponent<Text> ();

			_parentCondis = ImageGroup.FindChild("Condis").GetComponent<RectTransform>();


			if (!_bSupporter) {
				SkillGroup = ImageGroup.FindChild ("SkillGroup").GetComponent<RectTransform>();

				_txtSkillBreak = heroViewer.transform.FindChild ("SkillBreak").GetComponent<CanvasGroup> ();
			}
		}

		//Skill

		void InitSkill(BattleCharacter hero, int idx){
			lstCurSkills = new List<BattleSkill>();
			lstCurSkills = hero.cSkills.lstcSelectedActiveSkill;

			lstSkillButton = new List<SkillButton> ();

			GameObject skillIcon = AssetMgr.Instance.AssetLoad("Prefabs/UI/Battle/SkillButton.prefab", typeof(GameObject)) as GameObject;

			for (int i = 0; i < lstCurSkills.Count; i++) {
				if (i > 4) continue;
				GameObject btnObj = Instantiate (skillIcon);
				btnObj.transform.SetParent (SkillGroup);
				btnObj.transform.localScale = Vector3.one;
				btnObj.transform.localPosition = Vector3.zero;
				SkillButton btn = new SkillButton (btnObj);
				int skillIdx = i;
				btn._button.onClick.AddListener(() => { hero.cBattle.battleUIMgr.OnClickSkill(idx, skillIdx); } );

				EventTrigger trigger = btn._objSkillButton.GetComponent<EventTrigger>();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerDown;
				entry.callback.AddListener( (eventData) => { hero.cBattle.battleUIMgr.OnClickMySkill(idx, skillIdx); } );
				trigger.triggers.Add(entry);

				EventTrigger.Entry entry2 = new EventTrigger.Entry();
				entry2.eventID = EventTriggerType.PointerUp;
				entry2.callback.AddListener( (eventData) => { hero.cBattle.battleUIMgr.DisableSkillInfo(); } );
				trigger.triggers.Add(entry2);

				btn.SetIcon (hero, lstCurSkills [i].cInfo.u2ID, lstCurSkills [i].cInfo.u1Element);

				btnObj.AddComponent<TutorialButton> ().id = "SkillButton_" +hero.iCharIdx+"_"+skillIdx;
				lstSkillButton.Add (btn);
			}
		}

		public Transform GetSkillButtonTrans(int idx)
		{
			return lstSkillButton [idx]._objSkillButton.transform;
		}

		public Vector3 GetSkillButtonPos(int idx)
		{
			return lstSkillButton [idx]._objSkillButton.transform.localPosition;
		}

		public int SetFocusObj(int idx)
		{
			int before = focusIdx;
			
			if (idx > -1)
				focusIdx = idx;
			else
				focusIdx = -1;
			
			return before;
		}

		public void UpdateSkillCoolActive()
		{
			if (_bSupporter)
				return;
			
			for(int i=0; i<lstSkillButton.Count; i++)
			{
				lstSkillButton[i].UpdateSkillActive();
			}
		}

		public void EnableAllSkillButton()
		{
			if (_bSupporter)
				return;
			
			for(int idx=0; idx<lstSkillButton.Count; idx++)
			{
				lstSkillButton[idx]._objSkillButton.SetActive(true);
			}
			//setSkillButton();
		}

		public void DisableAllSkillButton()
		{
			if (_bSupporter)
				return;
			
			for(int idx=0; idx<lstSkillButton.Count; idx++)
			{
				lstSkillButton[idx]._objSkillButton.SetActive(false);
			}
		}

		public void SetSkillOrder(int idx, bool b, int order)
		{
			if (_bSupporter)
				return;
			
			if (_isDead)
				return;

			if (idx > lstSkillButton.Count - 1)
				return;

			lstSkillButton[idx].SetOrder(b, order.ToString());
		}

		public void SetChain(int idx, bool b)
		{
			if (_bSupporter)
				return;
			
			if (_isDead)
				return;
			
			if (idx > lstSkillButton.Count - 1)
				return;
			
			lstSkillButton[idx].SetChain(b);
		}

		public void SetDechainAll()
		{
			if (_bSupporter)
				return;
			
			for(int idx=0; idx<lstSkillButton.Count; idx++)
			{
				lstSkillButton[idx].SetChain(false);
			}
		}

		public void SetCoolPer()
		{
			if (_bSupporter)
				return;
			
			for (int idx = 0; idx < lstSkillButton.Count; idx++) 
			{
				float per = lstCurSkills [idx].GetCoolTimePer ();
				lstSkillButton[idx].SetCoolPer(per);
			}
		}

		public void SetCoolText()
		{
			if (_bSupporter)
				return;
			
			for(int idx=0; idx<lstSkillButton.Count; idx++)
			{
				int cool = lstCurSkills [idx].GetCoolTime ();
				lstSkillButton[idx].SetCoolText(cool);
			}
		}

		public void SkillSetActive(bool isActive)
		{
			if (_bSupporter)
				return;

			for (int i = 0; i < lstSkillButton.Count; i++) {
				lstSkillButton [i].SetGrayScale (!isActive);
			}
		}

		public void SetShowBreakSkill()
		{
			if (_bSupporter)
				return;

			for(int idx=0; idx<lstSkillButton.Count; idx++)
			{
				if(lstCurSkills [idx].cInfo.cDebuff != null){
					if (!lstCurSkills [idx].cInfo.cDebuff.bSkill && lstCurSkills [idx].cInfo.cDebuff.u4TimeForStop == 0) {
						lstSkillButton [idx].ShowBreakSkill ();
					}
				}
			}
		}

		public void SetHideBreakSkill()
		{
			if (_bSupporter)
				return;

			for(int idx=0; idx<lstSkillButton.Count; idx++)
			{
				lstSkillButton [idx].HideBreakSkill ();
			}
		}

		//Char
		void PlayAnim(string name){
			if (_bSupporter)
				return;

			Anim.Play (name);
		}

		public void SetFirstActive()
		{
			_bSelected = true;
			ChangeBorder (BorderStyle.Selected);
			PlayAnim ("Selected");
		}
		
		public void Select()
		{
			_bSelected = true;
			ChangeBorder(BorderStyle.Selected);
			PlayAnim ("Active");
			_imgFace.color = Color.white;
		}
		
		public void Deselect()
		{
			_bSelected = false;
			ChangeBorder(BorderStyle.Default);
			PlayAnim ("Deactive");
			//_imgFace.color = Color.gray;
		}

		void ChangeBorder(BorderStyle st){
			if (_bSupporter)
				return;
				
			switch (st) {
			case BorderStyle.Selected:
				_imgRect.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Battle/Battle_02.battle_ui_skill_border_select");
				_imgEleBorder.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Battle/Battle_02.battle_ui_class_ele_border_sel");
				break;

			default:
				_imgRect.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Battle/Battle_02.battle_ui_skill_border_deselect");
				_imgEleBorder.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Battle/Battle_02.battle_ui_class_ele_border");
				break;
			}
		}

		public void SetHp(uint max, uint hp)
		{
			float lastPer = (float)hp / (float)max;
			_imgHpGauge.fillAmount = lastPer;

			if (lastPer < _imgHpFade.fillAmount) {
				float dec = Mathf.Lerp(_imgHpFade.fillAmount, lastPer, 0.1f);
				_imgHpFade.fillAmount = dec;
			}else if(lastPer > _imgHpFade.fillAmount){
				_imgHpFade.fillAmount = lastPer;
			}

			FadeEff ();
		}

		public void SetWarning(){
			if(_bWarning) return;

			_bWarning = true;
			_bWarnColorUp = false;
			//ChangeBorder(BorderStyle.Warning);

			_imgHpGauge.color = WarnColor;
		}

		public void Reset(){
			_bWarning = false;
			_imgHpGauge.color = Color.white;

			SetGrayScaleSkill (false);
			AtlasMgr.Instance.SetDefaultShader (_imgRect);
			AtlasMgr.Instance.SetDefaultShader (_imgEle);
			AtlasMgr.Instance.SetDefaultShader (_imgEleBorder);
			AtlasMgr.Instance.SetDefaultShader (_imgFace);
			AtlasMgr.Instance.SetDefaultShader (_imgTriangle);
			_txtLevel.color = Color.white;
			_isDead = false;
			_objDead.SetActive(false);
			_imgFace.enabled = true;
		}

		public void SetDefault(){
			_bWarning = false;
			_imgHpGauge.color = Color.white;

			if (_isDead) {
				SetGrayScaleSkill (false);
				AtlasMgr.Instance.SetDefaultShader (_imgRect);
				AtlasMgr.Instance.SetDefaultShader (_imgEle);
				AtlasMgr.Instance.SetDefaultShader (_imgEleBorder);
				AtlasMgr.Instance.SetDefaultShader (_imgFace);
				AtlasMgr.Instance.SetDefaultShader (_imgTriangle);
				_txtLevel.color = Color.white;
				_isDead = false;
				_objDead.SetActive(false);
				_imgFace.enabled = true;
			}
		}

		public void SetSkillBreak(){
			if (!_bSupporter) {
				LeanTween.cancel (_txtSkillBreak.gameObject);

				_txtSkillBreak.alpha = 0f;
				_txtSkillBreak.transform.localScale = Vector3.one * 1.5f;
				_txtSkillBreak.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-90f, 80f);

				LeanTween.value (_txtSkillBreak.gameObject, 0f, 1f, 0.1f).setOnUpdate ((float alpha) => {
					_txtSkillBreak.alpha = alpha;
				});

				LeanTween.scale (_txtSkillBreak.gameObject, Vector3.one * 1.0f, 0.2f);
				LeanTween.moveLocalX (_txtSkillBreak.gameObject, -100f, 0.2f);

				LeanTween.moveLocalX (_txtSkillBreak.gameObject, -95f, 0.07f).setDelay (0.2f).setLoopPingPong (5);
				LeanTween.moveLocalY (_txtSkillBreak.gameObject, 85f, 0.07f).setDelay (0.2f).setLoopPingPong (5);

				LeanTween.value (_txtSkillBreak.gameObject, 1f, 0f, 0.2f).setDelay (1.1f).setOnUpdate ((float alpha) => {
					_txtSkillBreak.alpha = alpha;
				});
				LeanTween.moveLocalY (_txtSkillBreak.gameObject, 100f, 0.3f).setDelay (1f);
			}
		}

		public void SetDead(){
			if(_isDead) return;

			_bActiving = true;
			_IsChanging = false;
			_bSelected = false;
			_bWarning = false;
			SetDechainAll();
			ChangeBorder(BorderStyle.Dead);
			PlayAnim ("Deselect");
			//SetGray
			AtlasMgr.Instance.SetGrayScale(_imgRect);
			AtlasMgr.Instance.SetGrayScale(_imgEle);
			AtlasMgr.Instance.SetGrayScale(_imgEleBorder);
			AtlasMgr.Instance.SetGrayScale(_imgFace);
			AtlasMgr.Instance.SetGrayScale(_imgTriangle);
			_txtLevel.color = Color.gray;
			SetGrayScaleSkill (true);
			_isDead = true;
			_imgFace.enabled = false;
			_objDead.SetActive(true);
		}

		void SetGrayScaleSkill(bool bGray){
			if (_bSupporter)
				return;
			
			for (int i = 0; i < lstSkillButton.Count; i++) {
				lstSkillButton [i].SetGrayScale (bGray);
				if(bGray) lstSkillButton [i].SetChain (false);
			}
		}

		public void FadeEff(){
			if (_bWarning) {
				float color = _imgHpGauge.color.a;
				if(_bWarnColorUp){
					color += Time.deltaTime;
					if(color >= 1){ color = 1; _bWarnColorUp = false; }
					_imgHpGauge.color = new Color(WarnColor.r,WarnColor.g,WarnColor.b,color);
				}else{
					color -= Time.deltaTime;
					if(color <= 0.2f){ color = 0.2f; _bWarnColorUp = true; }
					_imgHpGauge.color = new Color(WarnColor.r,WarnColor.g,WarnColor.b,color);
				}
			}
		}

		public void AddCondition(GameObject tIcon){
			tIcon.transform.SetParent(_parentCondis);
			tIcon.transform.localScale = Vector3.one;
			tIcon.transform.localPosition = Vector3.zero;
		}

		bool _IsChanging;
		public bool IsChanging
		{
			get{ return _IsChanging; }
		}
		public bool bWarning
		{
			get{ return _bWarning; }
		}
	}

	class LimitTimer
	{
		GameObject objTimer;
		Text _txtTime;
		public LimitTimer(GameObject timer)
		{
			objTimer = timer;
			_txtTime = timer.GetComponent<RectTransform>().FindChild("Text").GetComponent<Text>();
		}
		
		public void SetTime(float time)
		{
			Int32 m = Mathf.FloorToInt(time / 60f);
			Int32 s = Mathf.FloorToInt(time % 60f);
			
			_txtTime.text = m.ToString("0")+":"+s.ToString("00");
		}
	}
	
	public class DamageText
	{
		GameObject objDmgText;
		FadeText txt;
		public DamageText(Transform queueParent)
		{
			objDmgText = GameObject.Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Battle/DmgText.prefab", typeof(GameObject)) as GameObject);
			txt = objDmgText.GetComponent<FadeText>();
			objDmgText.GetComponent<Transform>().SetParent(queueParent);
			objDmgText.GetComponent<Transform>().localScale = Vector3.one;
		}
		public void ShowDamage(Vector3 textPos, string damage, BattleUIMgr.DAMAGE_TEXT_TYPE textType, Color outline, float time, int txtSize = 38)
		{
			
			objDmgText.transform.position = textPos;
			txt.SetText(damage, textType, outline, true, time, txtSize);
			objDmgText.SetActive(true);
		}
	}
	class MonsterViewer
	{
		Image imgHpGauge;
		Text txtName;
		public MonsterViewer(GameObject monsterViewer)
		{
			imgHpGauge = monsterViewer.GetComponent<RectTransform>().FindChild("Bar").GetComponent<Image>();
			txtName = monsterViewer.GetComponent<RectTransform>().FindChild("Name").GetComponent<Text>();
		}
	}
	
	class SkillButton
	{
		public GameObject _objSkillButton;
		public Button _button;
		Animator _Animator;
		RectTransform _objTrans;
		Image _imgSkillIcon;
		Image _imgCoolDown;
		Image _imgCoolOn;
		Image _imgElement;
		Image _imgChain;
		GameObject _objOrder;
		Text _txtOrder;
		Text _txtCoolDown;
		GameObject _objBreakSkill;
		int iCurCool;

		bool bChain;
		bool bDead;

		public SkillButton(GameObject skillButton)
		{
			_objSkillButton = skillButton;
			_Animator = _objSkillButton.GetComponent<Animator>();
			_button = _objSkillButton.GetComponent<Button>();
			_objTrans = skillButton.GetComponent<RectTransform>();
			_imgSkillIcon = skillButton.transform.FindChild("Icon").GetComponent<Image>();
			_imgElement = skillButton.transform.FindChild("Element").GetComponent<Image>();
			_imgCoolDown = _objSkillButton.transform.FindChild("Cooldown").GetComponent<Image>();
			_imgCoolOn = _objSkillButton.transform.FindChild("Coolon").GetComponent<Image>();
			_imgChain = _objSkillButton.transform.FindChild("Chain").GetComponent<Image>();
			_imgChain.enabled = false;
			_objOrder = _objSkillButton.transform.FindChild("Order").gameObject;

			_txtOrder = _objOrder.transform.FindChild("OrderText").GetComponent<Text>();
			_txtCoolDown = _objSkillButton.transform.FindChild("CoolText").GetComponent<Text>();

			_objBreakSkill = _objSkillButton.transform.FindChild("BreakSkill").gameObject;

			_objSkillButton.AddComponent<TutorialButton>().id = _objSkillButton.name;
		}
		public void SetScaleX(float scale)
		{
			_objTrans.localRotation = Quaternion.Euler(0,90f-(scale*90f),0);
			//_objTrans.localScale = new Vector3(scale,1,1);
		}
		public void SetIcon(BattleCharacter hero, UInt16 skillId, Byte element)
		{	
			_imgSkillIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Skill/Atlas_SkillIcon_" + hero.cCharacter.cClass.u2ID + "." + skillId);
			_imgElement.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.common_02_skill_element_"+(element == 5 ? hero.u1Element : element));
			_imgElement.SetNativeSize();
		}
		public void SetActive(bool visible)
		{
			_objSkillButton.SetActive(visible);
		}
		public void SetGrayScale(bool bGray)
		{
			if (bGray) {
				AtlasMgr.Instance.SetGrayScale (_imgSkillIcon);
				AtlasMgr.Instance.SetGrayScale (_imgElement);
			} else {
				AtlasMgr.Instance.SetDefaultShader (_imgSkillIcon);
				AtlasMgr.Instance.SetDefaultShader (_imgElement);
			}

			EnableBtn (!bGray);
			bDead = bGray;
		}
		public void SetCoolDown(bool visible)
		{
			_imgCoolDown.enabled = visible;
		}
		public void SetCoolPer(float per)
		{
			_imgCoolDown.fillAmount = per;
		}

		public void SetChain(bool b)
		{
			bChain = b;
			_imgChain.enabled = b;
			if (b) {
				_imgChain.color = Color.white;
				LeanTween.alpha (_imgChain.rectTransform, 0f, 0.25f).setLoopPingPong ();
			}else LeanTween.cancel(_imgChain.gameObject);
		}

		public void SetOrder(bool bEnable, string sOrder){
			_objOrder.SetActive (bEnable);
			if (bEnable)
				_txtOrder.text = sOrder;
		}

		public void SetCoolText(int cool)
		{
			if(cool == iCurCool) return;
			if (cool == 0) {
				_txtCoolDown.text = "";
				SetSkillActive(true);
			}else{
				_txtCoolDown.text = cool.ToString();
			}
			iCurCool = cool;
		}

		void SetSkillActive(bool visible)
		{
			if (bDead)
				return;

			if(!visible) AtlasMgr.Instance.SetGrayScale(_imgSkillIcon);
			else AtlasMgr.Instance.SetDefaultShader(_imgSkillIcon);
			_imgCoolOn.color = Color.white;
			_imgCoolOn.enabled = visible;
		}

		void EnableBtn(bool bEnable)
		{
			_button.interactable = bEnable;
		}

		public void PlayAnim(string animName)
		{
			_Animator.Play(animName);
		}

		public void UpdateSkillActive(){
			if (_imgCoolOn.enabled) {
				_imgCoolOn.color = new Color(_imgCoolOn.color.r, _imgCoolOn.color.g, _imgCoolOn.color.b, _imgCoolOn.color.a - Time.deltaTime*5f);
				if(_imgCoolOn.color.a <= 0f){
					_imgCoolOn.enabled = false;
//					SetCoolDown(false);
				}
			}
		}

		public void ShowBreakSkill(){
			_objBreakSkill.SetActive (true);
			_objBreakSkill.transform.localScale = Vector3.one;
			LeanTween.scale (_objBreakSkill, Vector3.one * 1.2f, 0.2f).setLoopPingPong ();
		}

		public void HideBreakSkill(){
			if (_objBreakSkill.activeSelf) {
				LeanTween.cancel (_objBreakSkill);
				_objBreakSkill.SetActive (false);
			}
		}
	}
	
	class AutoButton
	{
		GameObject _objLayout;
		GameObject _objActiveEffect;
		Animator _buttonAnimator;
		bool _active;
		public bool IsActive
		{
			get
			{
				return _active;
			}
		}
		
		public AutoButton(GameObject autoButtonLayout)
		{
			_objLayout = autoButtonLayout;
			_buttonAnimator = autoButtonLayout.GetComponent<Animator>();
			_objActiveEffect = autoButtonLayout.GetComponent<RectTransform>().FindChild("ActiveEffect").gameObject;
			_objActiveEffect.SetActive(false);
		}
		
		public void Toggle()
		{
			_active = !_active;
			if(_active) On();
			else Off();
		}
		
		private void On()
		{
			_active = true;
			_objActiveEffect.SetActive(true);
		}
		
		private void Off()
		{
			_active = false;
			_objActiveEffect.SetActive(false);
		}
	}

	class GuildBattleUI
	{
		GameObject _objLayout;
		Text _txtGuildName;

		Image _imgGuildFlag;
		Text _txtGuildRank;

		Image[] _imgCrewStates;

		public GuildBattleUI(GameObject guildLayout, string gname, string rank, Crew[] tCrew)
		{
			_objLayout = guildLayout;
			_txtGuildName = guildLayout.transform.FindChild("GuildName").GetComponent<Text>();
			_imgGuildFlag = guildLayout.transform.FindChild("Flag").GetComponent<Image>();
			_txtGuildRank = guildLayout.transform.FindChild("Rank").GetComponent<Text>();
			_imgCrewStates = new Image[3];
			for(int i=0; i<_imgCrewStates.Length; i++){
				_imgCrewStates[i] = guildLayout.transform.FindChild("CrewState").FindChild("Crew"+(i+1)).GetComponent<Image>();
			}

			_txtGuildName.text = gname;
			_txtGuildRank.text = rank;
			//_imgGuildFlag.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_05_renew.guild_platinum");

			for(int i=0; i<tCrew.Length; i++){
				if(tCrew[i].u1Count == 0){
					SetCrewState(i, 0);
				}
			}
		}

		public void SetCrewState(int idx, int state)
		{
			switch (state) {
			case 0:
				_imgCrewStates [idx].sprite = AtlasMgr.Instance.GetSprite ("Sprites/Battle/Battle_02.battle_ui_guild_crew_off");
				break;
			case 1:
				_imgCrewStates [idx].sprite = AtlasMgr.Instance.GetSprite ("Sprites/Battle/Battle_02.battle_ui_guild_crew_on");
				break;
			case 2:
				_imgCrewStates [idx].sprite = AtlasMgr.Instance.GetSprite ("Sprites/Battle/Battle_02.battle_ui_guild_crew_normal");
				break;
			}
		}
	}


	class EnemyViewer {
		private GameObject _objHeroViewer;
		public GameObject gameObject
		{
			get{ return _objHeroViewer.gameObject; }
		}
			
		private Image _imgHpGauge;
		private Image _imgHpFade;

		private Image _imgEle;

		private Text _txtLevel;

		private bool _isDead;

		public EnemyViewer(GameObject eViewer, UInt16 level, Sprite eleIcon, Sprite hpgage)
		{
			InitComponent(eViewer);

			_txtLevel.text = level.ToString();

			_imgEle.sprite = eleIcon;

			_imgHpGauge.sprite = hpgage;
			_imgHpFade.sprite = hpgage;
		}

		void InitComponent(GameObject eViewer)
		{
			_objHeroViewer = eViewer;

			Transform ImageGroup = _objHeroViewer.transform.FindChild ("HeroInfo");

			_imgHpGauge = ImageGroup.FindChild("Hp").GetComponent<Image>();
			_imgHpFade = ImageGroup.FindChild("FadeHp").GetComponent<Image>();

			_imgEle = ImageGroup.FindChild ("Ele").GetComponent<Image> ();

			_txtLevel = ImageGroup.FindChild ("Level").GetComponent<Text> ();
		}

		public void SetHp(uint max, uint hp)
		{
			float lastPer = (float)hp / (float)max;
			_imgHpGauge.fillAmount = lastPer;

			if (lastPer < _imgHpFade.fillAmount) {
				float dec = Mathf.Lerp(_imgHpFade.fillAmount, lastPer, 0.1f);
				_imgHpFade.fillAmount = dec;
			}else if(lastPer > _imgHpFade.fillAmount){
				_imgHpFade.fillAmount = lastPer;
			}
		}

		public void Reset()
		{
			_imgHpGauge.color = Color.white;

			AtlasMgr.Instance.SetDefaultShader (_imgEle);
			_txtLevel.color = Color.white;
			_isDead = false;
		}

		public void SetDead(){
			if(_isDead) return;

			AtlasMgr.Instance.SetGrayScale(_imgEle);
			_txtLevel.color = Color.gray;
			_isDead = true;
		}
	}

}

