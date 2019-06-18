using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

public class UI_BattleResult_Win : MonoBehaviour {
	[SerializeField] GameObject resListElementCommon;
	[SerializeField] GameObject lvUpEffect;
		
	[SerializeField] UI_BattleResult_CharacterViewer[] charViewer;
//	[SerializeField] RectTransform _lbVic;
//	[SerializeField] RectTransform _lbExp;
//	[SerializeField] RectTransform _lbGold;
	[SerializeField] Text _txtChapter;

	[SerializeField] Text _txtExp;
	[SerializeField] Text _txtGold;

	[SerializeField] GameObject[] Stars;
	[SerializeField] RectTransform _trStageRewardListParent;

	[SerializeField] GameObject worldMapBtn;
	[SerializeField] GameObject restartBtn;
	[SerializeField] GameObject nextStageBtn;
	[SerializeField] GameObject MainBtn;
	[SerializeField] GameObject repeatBtn;

	[SerializeField] GameObject baseResultObject;


	// TowerStage Result Object
	[SerializeField] GameObject		towerResultObject;

	[SerializeField] Text			_textTowerFloor;
	[SerializeField] RectTransform	_trTowerRewardListParent;
	[SerializeField] GameObject		towerWorldMapBtn;
	[SerializeField] GameObject		towerMainBtn;

	[SerializeField] GameObject repeatCountMass;
	[SerializeField] Text _textRepeatCount;

	[SerializeField] GameObject goldObjects;
	[SerializeField] GameObject expObjects;
	[SerializeField] GameObject eventObjects;

	[SerializeField] GameObject objEventClear;
	[SerializeField] Image imgEventRewardIcon;
	[SerializeField] Text txtEventRewardCount;
	[SerializeField] Text txtEventRewardName;
	[SerializeField] Text txtEventRewardDesc;

    public GameObject SmithBtns;

    public GameObject objRewardOidnPoint;
    public Image imgOdinGradeIcon;
    public Text txtRewardOidnPointCount;

    int rcount = 3;

	enum RESULT_WIN_ANIM_STATE
	{
		NONE = 0,
		SHOW_PANEL = 1,
		GRADE_STAR = 2,
		EXP_UP = 3,
		ADD_GOLD = 4,
		SHOW_GRADE_REWARD = 5,
		SHOW_STAGE_REWARD = 6,
		END = 7,
	}
	RESULT_WIN_ANIM_STATE animState;

	Int32 gold_reward = 0;
	uint exp_reward = 0;

	BattleCrew _cBtCrew;
	public Reward cBattleResultReward;
	//보상획득 오디오클립
	public AudioClip rewardItemClip;

    private bool OnSurveyPopup = false;

	bool bEvent = false;

    void Awake()
	{
		//resListElementCommon = AssetMgr.Instance.AssetLoad("Prefabs/UI/Battle/Result/ListElement_Common_Mini.prefab", typeof(GameObject)) as GameObject;
		resListElementCommon = AssetMgr.Instance.AssetLoad("Prefabs/UI/Common/ItemSlot.prefab", typeof(GameObject)) as GameObject;
        lvUpEffect = AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/Eff_UI_CherryBlossom.prefab", typeof(GameObject)) as GameObject;
        OnSurveyPopup = ObscuredPrefs.GetBool("SurveyPopup");
    }


	public void OnClickMain()
	{
		StartCoroutine (ChangeSceneFade ());
	}

	public void OnClickWorldMap()
	{
        // CampUIMgr에서 사용하지만 언제 뭐에 쓰는지 몰라서 방치
        PlayerPrefs.SetInt("WorldMap", 1);

        StageInfo stageInfo = Legion.Instance.SelectedStage;

        int selectAct = 70;
        if (stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
            selectAct = 76;
        else if (stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
            selectAct = 82;

        FadeEffectMgr.Instance.QuickChangeScene(MENU.CAMPAIGN, selectAct + Legion.Instance.SelectedDifficult);
        Legion.Instance.AwayBattle();
    }

	public void OnClickRepeat()
	{
		repeatBtn.SetActive (false);
		Legion.Instance.AUTOCONTINUE = false;
        StageInfoMgr.Instance.RepeatItemInfoDelete();
        showBtn ();
	}

	IEnumerator ChangeSceneFade()
	{
		PopupManager.Instance.CloseLoadingPopup();
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		Legion.Instance.AwayBattle ();
        if(Legion.Instance.bAdventoStage == 2)
            AssetMgr.Instance.SceneLoad("BossRushScene");
        else
		    AssetMgr.Instance.SceneLoad("LobbyScene", false);
        StageInfoMgr.Instance.LastPlayStage = -1;
	}

	public void OnClickStage(bool next)
	{
		StageInfo stageInfo = Legion.Instance.SelectedStage;
		ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[stageInfo.u2ChapterID];

		if (next)
        {
			if (stageInfo.nextStage != null)
				StageInfoMgr.Instance.LastPlayStage = stageInfo.nextStage.u2ID;
		}
        else
			StageInfoMgr.Instance.LastPlayStage = Legion.Instance.SelectedStage.u2ID;

        int selectAct = 73;
        if(stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
            selectAct = 79;
        else if (stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
            selectAct = 85;

        FadeEffectMgr.Instance.QuickChangeScene(MENU.CAMPAIGN, selectAct + Legion.Instance.SelectedDifficult);
		Legion.Instance.AwayBattle ();

//		if(!Legion.Instance.CheckEnoughGoods(chapterInfo.GetConsumeGoods()))
//		{
//			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_nocost"), TextManager.Instance.GetText("popup_desc_key_lack"), null);
//			return;
//		}
//		
//		#if UNITY_EDITOR
//		string log = string.Format("[{0}] [crew : {1}] [stage : {2}] [difficult : {3}]", 
//		                           Server.MSGs.STAGE_START, Legion.Instance.cBestCrew.u1Index, Legion.Instance.SelectedStage.u2ID, Legion.Instance.selectedDifficult);
//		
//		DebugMgr.Log(log);
//		#endif
//		
//		// RKH TO DO
//		PopupManager.Instance.ShowLoadingPopup(1);
//		Server.ServerMgr.Instance.StartStage(Legion.Instance.cBestCrew, Legion.Instance.SelectedStage, (byte)Legion.Instance.selectedDifficult, AckBattleStart);
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
            //ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[Legion.Instance.SelectedStage.u2ChapterID];
            //Legion.Instance.SubGoods(chapterInfo.GetConsumeGoods());
            // 자동 전투시 재화 감소
            if (Legion.Instance.AUTOCONTINUE == true)
                Legion.Instance.SubGoods(Legion.Instance.SelectedStage.RepeatGoods());

			FadeEffectMgr.Instance.FadeOut();
			PopupManager.Instance.CloseLoadingPopup();
			SoundManager.Instance.OffBattleListner ();
			AssetMgr.Instance.SceneLoad("Battle");
		}
	}

	public void Show(BattleCrew btCrew, bool bPVP)
	{
		_cBtCrew = btCrew;

		worldMapBtn.SetActive(false);
		restartBtn.SetActive(false);
		nextStageBtn.SetActive(false);
		MainBtn.SetActive(false);
        SmithBtns.SetActive(false);
        repeatBtn.SetActive (false);
		objEventClear.SetActive (false);
        objRewardOidnPoint.SetActive(false);

        // 2016. 6 .30 jy
        // 탑 보상 버튼
        towerWorldMapBtn.SetActive(false);
		towerMainBtn.SetActive(false);
		
		Byte Score = 0;
		bool isTowerMode = false;

		if (bPVP) {
			//LeagueCrewInfo crew = UI_League.Instance._leagueInfomation.lstLeagueCrewInfo.Find(cs => cs.u1CrewIndex == UI_League.Instance.cSelectedCrew.u1Index);
			//LeagueInfo temp = LeagueInfoMgr.Instance.GetLeagueInfo (
			//	Convert.ToUInt16(crew.u1DivisionIndex + 4500));
			//gold_reward = (int)temp.cWinReward.u4Count;
			//exp_reward = (uint)temp.u4WinRewardExp;
            //
			//Score = (Byte)temp.LeagueWinPoint;
			//crew.u2Point += Score;
			//crew.u2Win += 1;
		}
        else
        {
//			leagueBtn.SetActive(false);
			if (Legion.Instance.SelectedStage.u2ChapterID != 0)
            {
				ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData [Legion.Instance.SelectedStage.u2ChapterID];
				// 2016. 6. 30 jy
				// 액트가 탑이면 별도 처리한다
				if (chapterInfo.actInfo.u1Mode != ActInfo.ACT_TYPE.TOWER)
                {
					_txtChapter.text = TextManager.Instance.GetText (StageInfoMgr.Instance.dicStageData [Legion.Instance.u2SelectStageID].sName);//TextManager.Instance.GetText(chapterInfo.strName)+" "+

					Score = Legion.Instance.cReward.u1AliveCharCnt;
					//gold_reward = (int)Legion.Instance.SelectedStage.acGetGoods [Legion.Instance.SelectedDifficult - 1].u4Count;
                    if(StageInfoMgr.Instance.u4TotalGold == 0)
                        gold_reward = (int)Legion.Instance.SelectedStage.acGetGoods [Legion.Instance.SelectedDifficult - 1].u4Count;
                    else
                        gold_reward = (int)(StageInfoMgr.Instance.u4TotalGold - StageInfoMgr.Instance.u4PrevTotalGold);
					//exp_reward = (uint)(Legion.Instance.SelectedStage.arrGetExp [Legion.Instance.SelectedDifficult - 1] + StageInfoMgr.Instance.u8AddedExp);
                    exp_reward = (uint)StageInfoMgr.Instance.u8AddedExp;

					for (int i = 0; i < 3; i++) {
						if ((i + 1) > Score)
							Stars [i].SetActive (false);
					}
				}
                else
                {
					isTowerMode = true;
					_textTowerFloor.text = TextManager.Instance.GetText (StageInfoMgr.Instance.dicStageData [Legion.Instance.u2SelectStageID].sName);
				}
			}
            else
            {
				expObjects.SetActive(false);

				bEvent = true;

				_txtChapter.text = TextManager.Instance.GetText (StageInfoMgr.Instance.dicStageData [Legion.Instance.u2SelectStageID].sName);

				for (int i = 0; i < 3; i++) {
					Stars [i].SetActive (false);
				}


				if (Legion.Instance.cEvent.selectedOpenEventID > 0) {
					Legion.Instance.cEvent.RemovePlayedStage();
				}

				if (Legion.Instance.bAdventoStage == 0)
                {
					gold_reward = (int)Legion.Instance.SelectedStage.acGetGoods [Legion.Instance.SelectedDifficult - 1].u4Count;

					goldObjects.GetComponent<RectTransform> ().anchoredPosition -= new Vector2 (0, 30f);
				}
                else if (Legion.Instance.bAdventoStage == 1)
                {
					goldObjects.SetActive (false);
					eventObjects.SetActive (true);

					RewardItem[] stageRewards = Legion.Instance.cReward.GetReward();
					imgEventRewardIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Battle/Battle_01.battle_ui_event_" + stageRewards[0].cRewards.u2ID);
					imgEventRewardIcon.SetNativeSize ();
					txtEventRewardCount.text = stageRewards [0].cRewards.u4Count.ToString();
					txtEventRewardName.text = TextManager.Instance.GetText(EventInfoMgr.Instance.dicMarbleGoods[stageRewards [0].cRewards.u2ID].sName);
					UInt16 eventID = EventInfoMgr.Instance.dicMarbleGoods [stageRewards [0].cRewards.u2ID].u2EventID;
					txtEventRewardDesc.text = String.Format(TextManager.Instance.GetText("result_event_stage_exchange_item") ,"<color=#008800>"+TextManager.Instance.GetText(EventInfoMgr.Instance.dicDungeonShop [eventID].sTitle)+"</color>");
				}
                else if(Legion.Instance.bAdventoStage == 2)
                {
                    goldObjects.SetActive (false);

                }
			}
		}

		bool bLvUp = false;

		if (exp_reward > 0) {
            int lvCharIdx = 0;
			for (int i = 0; i < btCrew.acCharacters.Length; i++) {
				if (btCrew.acCharacters [i] != null) {
					charViewer [i].gameObject.SetActive (true);
					if (charViewer [i].SetData ((Hero)btCrew.acCharacters [i].cCharacter, exp_reward)) {
						bLvUp = true;
                        lvCharIdx = i;
                        btCrew.acCharacters [i].PlayLevelup ();
					} else {
						btCrew.acCharacters [i].PlayResult ();
					}
				}
			}

            if (bLvUp)
            {
                Instantiate(lvUpEffect, btCrew.acCharacters[0].cObject.transform.position, Quaternion.identity);
            }
		}

		if (!bEvent)
        {
			SetResultInfo (bPVP, isTowerMode);
		}
        else
        {
			if(Legion.Instance.bAdventoStage == 0)
                SetResultInfo(bPVP, isTowerMode);
            else if(Legion.Instance.bAdventoStage == 2)
            {
                //보스러시 처리
                EventInfoMgr.Instance.u1BossRushProgress += EventInfoMgr.Instance.lstBossRush[0].u1Guage;
                int index = Legion.Instance.cEvent.openStageIds.FindIndex(cs => cs.u2EventID == Legion.Instance.SelectedBossRushStage.u2EventID 
                && cs.u2StageID == Legion.Instance.SelectedBossRushStage.u2StageID);
                if(index >= 0)
                {
                    Legion.Instance.SelectedBossRushStage.u1Closed = 2;
                    Legion.Instance.cEvent.openStageIds.Remove(Legion.Instance.cEvent.openStageIds[index]);
                    Legion.Instance.cEvent.openStageIds.Add(Legion.Instance.SelectedBossRushStage);
                }
                else
                {
                    Legion.Instance.SelectedBossRushStage.u1Closed = 2;
                    Legion.Instance.cEvent.openStageIds.Add(Legion.Instance.SelectedBossRushStage);
                }
                SetResultInfo(bPVP, isTowerMode);
            }
			else
                StartCoroutine(Direction ());
		}

//		if(Legion.Instance.SelectedStage.nextStage == null)
//		{
//			nextStageBtn.SetActive(false);
//		}
	}

	//void SetResultInfo(Byte iScore, bool bPVP){
	void SetResultInfo(bool bPVP, bool bTowerMode){
		_txtExp.text = "0"; 
		_txtGold.text = "0";

		if (bPVP) {
//			_lbVic.anchoredPosition = new Vector2 (130f, -165f);
//			_lbExp.anchoredPosition = new Vector2 (55f, -372f);
//			_lbGold.anchoredPosition = new Vector2 (55f, -410f);
		} else {
			// 스테이지 보상 초기화.
			RewardItem[] stageRewards = Legion.Instance.cReward.GetReward();
			int rewardCount = 0;
			for(int i=0; i<stageRewards.Length; i++)
			{
				GameObject objStageRewardListElement = getListItemElement(stageRewards[i]);
				if(objStageRewardListElement == null)
				{
					DebugMgr.LogError("objStageRewardListElement == null" );
					continue;
				}
				
				//  보상 아이템 아이콘을 넣을 부모를 모드에 따라 변경한다
				if(bTowerMode == true)
					objStageRewardListElement.transform.SetParent(_trTowerRewardListParent);
				else
					objStageRewardListElement.transform.SetParent(_trStageRewardListParent);	
					
				objStageRewardListElement.transform.localScale = Vector3.one*1.5f;
				objStageRewardListElement.transform.localPosition = Vector3.zero;
				objStageRewardListElement.SetActive (false);
				SoundPlayer spReward = objStageRewardListElement.AddComponent<SoundPlayer>() as SoundPlayer;
				objStageRewardListElement.GetComponent<SoundPlayer>().audioClip = rewardItemClip;
				++rewardCount;

                // 재료 수집 아이템 카운트 증가
                StageInfoMgr.Instance.AddRepeatTargetCount(stageRewards[i].cRewards);
            }
			// 스테이지 보상 개수에 따른 사이즈 변화.
			float listHeight = Mathf.Ceil(rewardCount/3f) * 115f;
		}

		towerResultObject.SetActive(bTowerMode);
		baseResultObject.SetActive(!bTowerMode);

		gameObject.SetActive(true);
		if( bTowerMode == true)
			StartCoroutine("TowerResultDirection");
		else
			StartCoroutine(Direction());
	}

	GameObject getListItemElement(RewardItem rewardItem)
	{
		GameObject ret = null;
		if(rewardItem.cRewards != null)
		{
			ret = Instantiate(resListElementCommon) as GameObject;
			ret.GetComponent<UI_ItemListElement_Common>().SetData(rewardItem.cRewards);
			//ret.GetComponent<RewardButton>().SetButton(rewardItem.cRewards.u1Type, rewardItem.cRewards.u2ID);
		}
		/*
		// 2016. 07 .21 jy 
		// 탑 보상으로 채화가 추가 됨에 따라 보상 아이콘 셋팅은 모든 아이템이 가능하도록 변경
		ItemInfo.ITEM_ORDER itemType = ItemInfoMgr.Instance.GetItemType(rewardItem.cRewards.u2ID);
		if(itemType == ItemInfo.ITEM_ORDER.MATERIAL)
		{
			MaterialItem item = new MaterialItem(rewardItem.cRewards.u2ID);
			item.u2Count = (ushort)rewardItem.cRewards.u4Count;

			ret = Instantiate(resListElementCommon) as GameObject;
			ret.GetComponent<UI_ItemListElement_Common>().SetData(item);
			ret.GetComponent<RewardButton>().SetButton(rewardItem.cRewards.u1Type, rewardItem.cRewards.u2ID);
		}
		else if(itemType == ItemInfo.ITEM_ORDER.CONSUMABLE)
		{
			ConsumableItem item = new ConsumableItem(rewardItem.cRewards.u2ID);
			item.u2Count = (ushort)rewardItem.cRewards.u4Count;
			
			ret = Instantiate(resListElementCommon) as GameObject;
			ret.GetComponent<UI_ItemListElement_Common>().SetData(item);
			ret.GetComponent<RewardButton>().SetButton(rewardItem.cRewards.u1Type, rewardItem.cRewards.u2ID);
		}
		else
		{
			DebugMgr.Log("Not Type");
			ret = Instantiate(resListElementCommon) as GameObject;
		}
		*/
		return ret;
	}

	IEnumerator Direction()
	{
        PopupManager.Instance.showLoading = true;
		if (!bEvent) {
			yield return new WaitForSeconds (1.5f);
			for (int i = 0; i < _trStageRewardListParent.childCount; i++) {
				_trStageRewardListParent.GetChild (i).gameObject.SetActive (true);
				LeanTween.scale (_trStageRewardListParent.GetChild (i).gameObject, Vector3.one * 0.9f, 0.2f).setEase (LeanTweenType.easeOutBack);
				yield return new WaitForSeconds (0.15f);
			}
		} else {
			yield return new WaitForSeconds (0.5f);

			objEventClear.SetActive (true);
			objEventClear.transform.localScale = Vector3.one * 2f;
			LeanTween.scale (objEventClear, Vector3.one, 0.2f).setEase (LeanTweenType.easeOutBack);
			yield return new WaitForSeconds (0.2f);
			SoundManager.Instance.PlayEff (rewardItemClip, false);

			if (Legion.Instance.bAdventoStage == 0)
            {
				for (int i = 0; i < _trStageRewardListParent.childCount; i++)
                {
					_trStageRewardListParent.GetChild (i).gameObject.SetActive (true);
					LeanTween.scale (_trStageRewardListParent.GetChild (i).gameObject, Vector3.one * 0.9f, 0.2f).setEase (LeanTweenType.easeOutBack);
					yield return new WaitForSeconds (0.15f);
				}
			}
            else if(Legion.Instance.bAdventoStage == 2)
            {
                for (int i = 0; i < _trStageRewardListParent.childCount; i++)
                {
					_trStageRewardListParent.GetChild (i).gameObject.SetActive (true);
					LeanTween.scale (_trStageRewardListParent.GetChild (i).gameObject, Vector3.one * 0.9f, 0.2f).setEase (LeanTweenType.easeOutBack);
					yield return new WaitForSeconds (0.15f);
				}
            }
            else
            {
				LeanTween.scale (txtEventRewardCount.gameObject, Vector3.one*1.2f, 0.1f).setLoopPingPong(1);
			}
		}

		float exp = (float)exp_reward;
		float gold = (float)gold_reward;
		float cur = 0;
		float time = 0.5f;

		//Legion.Instance.AddGoods(1, gold_reward);
		if (StageInfoMgr.Instance.u4TotalGold != 0) {
			Legion.Instance.Gold = StageInfoMgr.Instance.u4TotalGold;
			Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.Gold, 0, 0, 0, 0, (UInt32)gold_reward);
			StageInfoMgr.Instance.u4TotalGold = 0;
		}else
            Legion.Instance.AddGoods(1, gold_reward);

		while (cur < gold) {
			yield return new WaitForEndOfFrame();
			cur += (Time.deltaTime / time) * gold;
			time -= Time.deltaTime;
			if(cur >= gold) cur = gold;
			_txtGold.text = ((int)cur).ToString();
		}

		cur = 0;
        time = 0.5f;

		while (cur < exp) {
			yield return new WaitForEndOfFrame();
			cur += (Time.deltaTime / time) * exp;
			time -= Time.deltaTime;
			if(cur >= exp) cur = exp;
			_txtExp.text = ((int)cur).ToString();
		}

		yield return new WaitForSeconds(1f);

		if (!bEvent) {
            //#ODIN [오딘 포인트 연출]
            OnOdinPointRewardEffect();

            // 2016. 11. 01 jy
            // 리뷰 유도 팝업이 작동하지으면 퀘스트 클리어를 체크한다
            if (!AppReviewPopup())
            {
                CheckOdinMissionClear(null);
            }

			//반복 전투 상태이고 반복 수집이 완료 되었는지 체크한다
			if (Legion.Instance.AUTOCONTINUE == true && StageInfoMgr.Instance.IsRepeatColletComplete () == true) {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("title_notice"), TextManager.Instance.GetText ("repeat_material_end"), null);
				Legion.Instance.AUTOCONTINUE = false;
			}
	        
			yield return new WaitForEndOfFrame ();

			if (Legion.Instance.AUTOCONTINUE) {
				repeatBtn.SetActive (true);
				LeanTween.alpha (repeatBtn.GetComponent<RectTransform> (), 1f, 0.3f);
			} else {
				// 수집 반복 전투가 종료되면 정보를 삭제한다
				if (StageInfoMgr.Instance.IsRepeatItemInfo () == true)
					StageInfoMgr.Instance.RepeatItemInfoDelete ();

				showBtn ();
			}

			yield return new WaitForSeconds (0.3f);
		} else {
			showBtn ();
			yield return new WaitForSeconds (0.3f);
		}

		PopupManager.Instance.showLoading = false;

		if (!bEvent) {
			if (Legion.Instance.AUTOCONTINUE) {
				int cnt = rcount;
				repeatCountMass.SetActive (true);
				for (int i = 0; i < cnt; i++) {
					if (Legion.Instance.AUTOCONTINUE) {
						_textRepeatCount.text = rcount.ToString ();
						_textRepeatCount.transform.localScale = Vector3.one * 2.0f;
						LeanTween.scale (_textRepeatCount.GetComponent<RectTransform> (), Vector3.one, 0.2f).setEase (LeanTweenType.easeInOutBack);
						rcount--;
					} else {
						repeatCountMass.SetActive (false);
					}
					yield return new WaitForSeconds (1f);
				}
			}

			repeatCountMass.SetActive (false);

			if (Legion.Instance.AUTOCONTINUE)
				repeatStage ();
		}
	}

	void repeatStage(){
		StageInfo stageInfo = Legion.Instance.SelectedStage;
		ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[stageInfo.u2ChapterID];

        // 열쇠 부족한지 여부 확인
		if(!Legion.Instance.CheckEnoughGoods(chapterInfo.GetConsumeGoods()))
		{
            string msg =  Legion.Instance.GetGoodsName(chapterInfo.GetConsumeGoods()) + TextManager.Instance.GetText("repeat_battle_end_low_goods");
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_nocost"), msg, null);
			OnClickRepeat ();
			return;
		}

        // 반복전투 재화 부족 여부 확인
        if (Legion.Instance.CheckEnoughGoods(stageInfo.RepeatGoods()) == false)
        {
            string msg = Legion.Instance.GetGoodsName(chapterInfo.GetConsumeGoods()) + TextManager.Instance.GetText("repeat_battle_end_low_goods");
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_nocost"), msg, null);
            OnClickRepeat();
            return;
        }
		
		#if UNITY_EDITOR
		string log = string.Format("[{0}] [crew : {1}] [stage : {2}] [difficult : {3}]", 
		                           Server.MSGs.STAGE_START, Legion.Instance.cBestCrew.u1Index, Legion.Instance.SelectedStage.u2ID, Legion.Instance.SelectedDifficult);
		
		DebugMgr.Log(log);
		#endif
		
		// RKH TO DO
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.StartStage(Legion.Instance.cBestCrew, Legion.Instance.SelectedStage, (byte)Legion.Instance.SelectedDifficult, AckBattleStart, Legion.Instance.AUTOCONTINUE);
	}

	void showBtn(){
		StageInfo stageInfo = Legion.Instance.SelectedStage;

		if (stageInfo.u2ChapterID == 0) {
			MainBtn.SetActive (true);
			MainBtn.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-275f, 60);
			LeanTween.alpha (MainBtn.GetComponent<RectTransform> (), 1f, 0.3f);
			return;
		}

		if (stageInfo.u2ID == LegionInfoMgr.Instance.acCrewOpenGoods[0][1].u2ID && Legion.Instance.cTutorial.au1Step [4] != Server.ConstDef.LastTutorialStep) {
			MainBtn.SetActive (true);
			LeanTween.alpha (MainBtn.GetComponent<RectTransform> (), 1f, 0.3f);

			worldMapBtn.SetActive (true);
			AtlasMgr.Instance.SetGrayScale (worldMapBtn.GetComponent<Image> ());
			worldMapBtn.GetComponent<Button> ().interactable = false;
			LeanTween.alpha (worldMapBtn.GetComponent<RectTransform> (), 1f, 0.3f);

			restartBtn.SetActive (true);
			AtlasMgr.Instance.SetGrayScale (restartBtn.GetComponent<Image> ());
			restartBtn.GetComponent<Button> ().interactable = false;
			LeanTween.alpha (restartBtn.GetComponent<RectTransform> (), 1f, 0.3f);

			nextStageBtn.SetActive (true);
			AtlasMgr.Instance.SetGrayScale (nextStageBtn.GetComponent<Image> ());
			nextStageBtn.GetComponent<Button> ().interactable = false;
			LeanTween.alpha (nextStageBtn.GetComponent<RectTransform> (), 1f, 0.3f);
			return;
		}

		worldMapBtn.SetActive (true);
		LeanTween.alpha (worldMapBtn.GetComponent<RectTransform> (), 1f, 0.3f);
		MainBtn.SetActive (true);
		LeanTween.alpha (MainBtn.GetComponent<RectTransform> (), 1f, 0.3f);

		restartBtn.SetActive (true);
        LeanTween.alpha(restartBtn.GetComponent<RectTransform>(), 1f, 0.3f);
		
		if(stageInfo.IsLastStageInChapter == true &&
			stageInfo.chapterInfo.IsLastChapterInAct == true)
		{
			nextStageBtn.SetActive (true);
			AtlasMgr.Instance.SetGrayScale (nextStageBtn.GetComponent<Image> ());
			nextStageBtn.GetComponent<Button> ().interactable = false;
			LeanTween.alpha (nextStageBtn.GetComponent<RectTransform> (), 1f, 0.3f);
		}
		else
		{
			if (Legion.Instance.SelectedStage.nextStage != null) {
				nextStageBtn.SetActive (true);
				LeanTween.alpha (nextStageBtn.GetComponent<RectTransform> (), 1f, 0.3f);
			}

			// 2016. 7. 4 jy
			// 탐색의 숲 일 경우 클리어 가능 횟수를 확인하여 버튼 활성화 여부를 넣는다
			if (stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST) 
			{
				if (StageInfoMgr.Instance.GetForestTicket ()/*(stageInfo.u2ID)*/ <= 0) 
				{
					AtlasMgr.Instance.SetGrayScale (restartBtn.GetComponent<Image> ());
					restartBtn.GetComponent<Button> ().interactable = false;
					if (nextStageBtn.activeSelf == true) 
					{
						AtlasMgr.Instance.SetGrayScale (nextStageBtn.GetComponent<Image> ());
						nextStageBtn.GetComponent<Button> ().interactable = false;
					}
				}
			}
		}

        if (Legion.Instance.sName != "")
            SmithBtns.SetActive(true);
    }

	IEnumerator TowerResultDirection()
	{
		PopupManager.Instance.showLoading = true;
		yield return new WaitForSeconds(1.5f);

		for (int i = 0; i < _trTowerRewardListParent.childCount; i++) {
			_trTowerRewardListParent.GetChild (i).gameObject.SetActive (true);
			LeanTween.scale (_trTowerRewardListParent.GetChild (i).gameObject, Vector3.one, 0.2f).setEase (LeanTweenType.easeInOutBack);
			yield return new WaitForSeconds(0.15f);
		}

		yield return new WaitForSeconds(1f);

		towerWorldMapBtn.SetActive(true);
		LeanTween.alpha (towerWorldMapBtn.GetComponent<RectTransform> (), 1f, 0.3f);

		towerMainBtn.SetActive(true);
		LeanTween.alpha (towerMainBtn.GetComponent<RectTransform> (), 1f, 0.3f);
		// 마지막 스테이지 라면 메인으로 가는 버튼을 비활성화 시킨다 [마지막층 클리어 팝업창을 띄우기 위하여]
		//if( StageInfoMgr.Instance.dicStageData[Legion.Instance.u2SelectStageID].IsLastStageInChapter )
		if( Legion.Instance.SelectedStage.IsLastStageInChapter )
		{
			Legion.Instance.bTowerClearPopup = true;
			AtlasMgr.Instance.SetGrayScale(towerMainBtn.GetComponent<Image>());
			towerMainBtn.GetComponent<Button>().interactable = false;
		}

		yield return new WaitForSeconds(0.3f);

		PopupManager.Instance.showLoading = false;

        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, CheckQuestClear);
    }

	public void OnClickClose()
	{
        //DebugMgr.LogError("창 닫음");
		Destroy(gameObject);
	}

	// 퀘스트 클리어 했는지 확인
	public void CheckQuestClear(object[] param)
	{
		if(!Legion.Instance.cTutorial.CheckTutorial(MENU.BATTLE_RESULT))
		{
			if (Legion.Instance.SelectedStage.u2ID == LegionInfoMgr.Instance.acCrewOpenGoods [0] [1].u2ID && Legion.Instance.cTutorial.au1Step [4] != Server.ConstDef.LastTutorialStep) {

			} else {
				Legion.Instance.cQuest.CheckEndDirection (AchievementTypeData.StageClear);
				Legion.Instance.cQuest.CheckEndDirection (AchievementTypeData.KillMonster);
				Legion.Instance.cQuest.CheckEndDirection (AchievementTypeData.CollectItem);
			}
		}
	}

    public void CheckOdinMissionClear(object[] param)
    {
        if ( Legion.Instance.cQuest.u2ClearOdinMissionID != 0 )
        {
            Legion.Instance.AUTOCONTINUE = false;
            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, CheckQuestClear);
        }
        else
        {
            CheckQuestClear(null);
        }
    }

	// 2016. 11. 01 jy
	// 리뷰 유도 1막 1-5 클리어시 뜨도록 변경
	public bool AppReviewPopup()
	{	
		// 이미 팝업을 띄운적이 있는지 확인
		if( ObscuredPrefs.GetBool("IsAppReview") == true )
			return false;
		// 오토 여부 확인 && 1막 1-5 스테이지인지 확인
		if(Legion.Instance.AUTOCONTINUE == true || Legion.Instance.SelectedStage.u2ID != 6005)
			return false;
		// 보통 난이도인지 확인
		if( Legion.Instance.SelectedDifficult != Server.ConstDef.MinDifficult )
			return false;

		PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_review_going_title"), 
			string.Format(TextManager.Instance.GetText("popup_review_going_desc"), LegionInfoMgr.Instance.ReviewReward.u4Count), 
			TextManager.Instance.GetText("btn_review_going"), GoToReview, null, CancleReview, null);
		return true;
	}

	public void GoToReview(object[] param)
	{
        Application.OpenURL(PopupManager.Instance.GetReviewURL());//param[0].ToString());
        StartCoroutine("ReviewOpen");
        //CheckOdinMissionClear(null);
	}

	public void CancleReview(object[] param)
	{
		ObscuredPrefs.SetBool("IsAppReview", true);
	}
	
	// 바로 진행시 팝업이 빨리뜨게 됨
	public IEnumerator ReviewOpen()
	{
		yield return new WaitForSeconds(1f);
		PopupManager.Instance.ShowLoadingPopup(1);
        yield return new WaitForSeconds(1f);
        PopupManager.Instance.CloseLoadingPopup();
        Server.ServerMgr.Instance.LegionMark(5, GetReviewReward);
    }
	
	public void GetReviewReward(Server.ERROR_ID err)
	{
		if(err == Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			if( ObscuredPrefs.GetBool("IsAppReview") == false)
			{
				ObscuredPrefs.SetBool("IsAppReview", true);
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_review_going_title"), TextManager.Instance.GetText("popup_review_reward"), CheckOdinMissionClear);
			}
		}
	}

    //#ODIN [스테이지 오딘 연출]
    private void OnOdinPointRewardEffect()
    {
        StageInfo stageInfo = Legion.Instance.SelectedStage;
        if (stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.STAGE &&
            stageInfo.chapterInfo.cPlayPayBack.u1Type == (Byte)GoodsType.ODIN_POINT)
        {
            imgOdinGradeIcon.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Common/otm_ambulram.Odin_Grade_icon_{0}", Legion.Instance.u1VIPLevel));
            imgOdinGradeIcon.SetNativeSize();
			txtRewardOidnPointCount.text = string.Format("X {0}", stageInfo.chapterInfo.cPlayPayBack.u4Count);

            objRewardOidnPoint.transform.localScale = Vector3.one * 2;
            LeanTween.scale(objRewardOidnPoint, Vector3.one, 0.2f);

            Stars[0].transform.parent.gameObject.SetActive(false);
            objRewardOidnPoint.SetActive(true);
        }        
    }
//	public void SetCharacterObject(Crew crew)
//	{
//		for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
//		{
//			if(Legion.Instance.cBestCrew.acLocation[i] != null)
//			{
//				Hero hero = (Hero)Legion.Instance.cBestCrew.acLocation[i];
//				hero.InitModelObject();
//				hero.cObject.transform.SetParent(_trCharObjectSlot[i]);
//				hero.cObject.transform.localPosition = new Vector3(0f, -250f, -500f);
//				hero.cObject.transform.eulerAngles = new Vector3(3.5f, 171.7f, 359.5f);
//				hero.cObject.transform.localScale = Vector3.one * 400f;
//
//				HeroObject heroObject = hero.cObject.GetComponent<HeroObject>();
//				heroObject.SetAnimations_UI();
//				heroObject.SetLayer(5);
//			}
//		}
//	}
}