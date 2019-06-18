using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public class LeagueScene : BaseScene
{
    [SerializeField] GameObject MainPanel;
    [SerializeField] GameObject CrewSetPanel;

    [SerializeField] GameObject CharacterInfoPopup;
    [SerializeField] GameObject IntroduceLeaguePopup;
    [SerializeField] GameObject RankPopup;
    [SerializeField] GameObject MatchPopup;
    [SerializeField] GameObject PromotedPopup;
    [SerializeField] GameObject RewardPopup;
    [SerializeField] GameObject MatchList;
    [SerializeField] GameObject RevengePopup;

    [SerializeField] Text txtMainPanelName;
    [SerializeField] Text txtCrewPanelName;
    [SerializeField] Text txtPlayerRank;
    [SerializeField] Text txtPlayerRankPoint;
    [SerializeField] Text txtPlayerCrewPower;
    [SerializeField] Text txtPlayerCrewPower2;

    [SerializeField] UI_League_Slot[] _charSlot;
    [SerializeField] Button Btn_Rwd;

    [SerializeField] Image imgPlayerDivision;
    [SerializeField] Image imgDivision;
	[SerializeField] UI_DivisionMark cDivisionMark;	// 디비전 마크

    [SerializeField]
    SubChatting _subChattingWidown;

    GameObject _prefMatchCrewListSlot;
    StringBuilder tempStringBuilder;

	float slotCount = 0f;
	float beforeRankIndex = -1f;
	float currentRankIndex = 0f;

    public enum CloseType
    {
        LEAGUE_SCENE,
        CREW_SET,
        CHAR_INFO,
        INTRO_LEAGUE,
        RANK,
        MATCH
    }

    void Awake()
    {
		#if UNITY_ANDROID
		IgaworksUnityAOS.IgaworksUnityPluginAOS.Adbrix.retention("League");
		#elif UNITY_IOS
		AdBrixPluginIOS.Retention("League");
		#endif
		//#CHATTING
		if(_subChattingWidown != null)
		{
            if (PopupManager.Instance.IsChattingActive())
            {
                PopupManager.Instance.SetSubChtting(_subChattingWidown);
                _subChattingWidown.gameObject.SetActive(true);
            }
            else
            {
                _subChattingWidown.gameObject.SetActive(false);
            }
        }
        FadeEffectMgr.Instance.FadeIn();
        tempStringBuilder = new StringBuilder();
        _prefMatchCrewListSlot = AssetMgr.Instance.AssetLoad("Prefabs/UI/League/CrewListSlot_.prefab", typeof(GameObject)) as GameObject;
        if(!ObscuredPrefs.GetBool("RegistLeague", false))
            ObscuredPrefs.SetBool("RegistLeague", true);
    }
    bool registLeague = false;
    public void OnEnable()
    {
        PopupManager.Instance.AddPopup(gameObject, RemovePopupInLeagueScene);
        Legion.Instance.bLeagueToCharInfo = false;
        Init();
        if(Legion.Instance.cLeagueCrew.u1Count == 0)
        {
            OnClickOpenSetCrew();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_league_start"), TextManager.Instance.GetText("popup_desc_league_start"), null);
        }
        else
        {
            registLeague = true;
            //InitMathingList();
            RequestMatchList();
        }
    }
	
    public void Init()
    {
        MainPanel.SetActive(true);
        CrewSetPanel.SetActive(false);
        txtMainPanelName.text = Legion.Instance.sName;
        txtCrewPanelName.text = Legion.Instance.sName;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(TextManager.Instance.GetText("mark_league_total_power")).Append(" ").Append(Legion.Instance.cLeagueCrew.u4Power);
        txtPlayerCrewPower.text = tempStringBuilder.ToString();
        txtPlayerCrewPower2.text = tempStringBuilder.ToString();

		SetDivisionInfo();

        for(int i=0; i<Legion.Instance.cLeagueCrew.acLocation.Length; i++)
        {
            if(Legion.Instance.cLeagueCrew.acLocation[i] != null)
                //_charSlot[i].SetData((Byte)(((Hero)Legion.Instance.cLeagueCrew.acLocation[i]).u1Index-1));
                _charSlot[i].SetData((Byte)i);
            else
                _charSlot[i].SetData(0, true);
        }
        for(int i=0; i<LeagueCrew.MAX_CHAR_IN_CREW; i++)
        {
            if(Legion.Instance.cLeagueCrew.acLocation[i] != null)
            {
                _charSlot[i].SetLeader(true);
                break;
            }
            else
                _charSlot[i].SetLeader(false);
        }
        //if((Legion.Instance.u1LeagueReward & 0x1) > 0 || (Legion.Instance.u1LeagueReward & 0x2) > 0)
        //    Btn_Rwd.interactable = true;
        //else
        //    Btn_Rwd.interactable = false;
    }

    private IEnumerator ClearMatchList()
    {
        for(int i=0; i<MatchList.transform.GetChildCount(); i++)
            Destroy(MatchList.transform.GetChild(i).gameObject);
        yield return new WaitForEndOfFrame();
    }

    public void InitMathingList()
    {
		//imgPlayerDivision.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_06.division_t_"+Legion.Instance.GetDivision);
        StartCoroutine(ClearMatchList());
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(UI_League.Instance.cLeagueMatchList.u4MyRank).Append(" ").Append(TextManager.Instance.GetText("mark_league_rank"));
        txtPlayerRank.text = tempStringBuilder.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(UI_League.Instance.cLeagueMatchList.u4MyPoint).Append(" ").Append(TextManager.Instance.GetText("mark_league_point"));
        txtPlayerRankPoint.text = tempStringBuilder.ToString();

		slotCount = 0;
        for(int i=0; i<UI_League.Instance.cLeagueMatchList.u1Count; i++)
        {
            if(UI_League.Instance.cLeagueMatchList.sListSlotData[i].u1Revenge == 3)
                continue;
            GameObject _listSlot = Instantiate(_prefMatchCrewListSlot);
            _listSlot.transform.SetParent(MatchList.transform);
            _listSlot.transform.localPosition = Vector3.zero;
            _listSlot.transform.localScale = Vector3.one;
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(_prefMatchCrewListSlot.name).Append(i);
            _listSlot.name = tempStringBuilder.ToString();
			if (_listSlot.GetComponent<UI_League_ListSlot> ().SetData (UI_League.Instance.cLeagueMatchList.sListSlotData [i], GetComponent<LeagueScene> ())) {
				currentRankIndex = i;
			}
			if (UI_League.Instance.cLeagueMatchList.sListSlotData [i].u4Rank == UI_League.Instance.u1BeforeRank) {
				beforeRankIndex = i;
			}
			slotCount++;
        }
        if(UI_League.Instance.u8SelectEnemyCrewSN != 0)
            ReloadkMatch();

		// 새로운 시즌이 시작되었다면 연출을 보여준다
		if(UI_League.Instance.cLeagueMatchList.u1LastCheckDicisionIndex == 0)
		{
			// 이전 시즌이 없다면 디비전이 없다면
			if(UI_League.Instance.cLeagueMatchList.u1PrevDivisionIndex == 0)
				return;

			// 자주 사용하지 않기 때문에 이후 이전 시즌 결과는 프리펩으로 빼서 사용한다
			GameObject objPrevResult = AssetMgr.Instance.AssetLoad("Prefabs/UI/League/UI_PrevLeagueResult.prefab", typeof(GameObject)) as GameObject;
			GameObject prveResult = Instantiate(objPrevResult);
			RectTransform rtTr = prveResult.GetComponent<RectTransform>();
			rtTr.SetParent(MainPanel.transform);
			rtTr.SetAsLastSibling();
			rtTr.anchoredPosition3D = Vector3.zero;
			rtTr.localScale = Vector3.one;
			rtTr.sizeDelta = Vector2.zero;

			prveResult.SetActive(true);
		}
		else
		{
			if(UI_League.Instance.u1Prom > 0) //0:없음 1:승격 2:강등
			{
				PromotedPopup.SetActive(true);
				UI_League_PromotedPopup PromPopSc = PromotedPopup.GetComponent<UI_League_PromotedPopup> ();
				//if(UI_League.Instance.cLeagueMatchList.u1DivisionIndex > UI_League.Instance.cLeagueMatchList.u1LastCheckDicisionIndex)
				//{
				//	//보상 버튼 활성화
	            //    Btn_Rwd.interactable = true;
				//}

				UI_League.Instance.u1Prom = 0;
			}
		}
		if((Legion.Instance.u1LeagueReward & 0x1) > 0 || (Legion.Instance.u1LeagueReward & 0x2) > 0)
			Btn_Rwd.interactable = true;
		else
			Btn_Rwd.interactable = false;

        if(Legion.Instance.bLeagueToCharInfo)
        {
            Legion.Instance.bLeagueToCharInfo = false;
            OnClickOpenSetCrew();
        }
        StartCoroutine("SetListPosition");
        OpenRevengePopup();		
        
    }

    IEnumerator SetListPosition()
    {
		yield return new WaitForEndOfFrame();

		if (beforeRankIndex < 0) {
			if (UI_League.Instance.u1BeforeRank > UI_League.Instance.cLeagueMatchList.u4MyRank) {
				MatchList.transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 0;
				UI_League.Instance.MyLeagueCrewSlot.transform.SetSiblingIndex ((int)slotCount-1);
				beforeRankIndex = slotCount - 1;
			} else if(UI_League.Instance.u1BeforeRank == UI_League.Instance.cLeagueMatchList.u4MyRank){
				UI_League.Instance.MyLeagueCrewSlot.transform.SetSiblingIndex ((int)currentRankIndex);

				float temp = currentRankIndex/slotCount;

				MatchList.transform.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1-temp;
				LayoutRebuilder.MarkLayoutForRebuild (MatchList.GetComponent<RectTransform> ());

				yield break;
			} else {
				MatchList.transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 1;
				UI_League.Instance.MyLeagueCrewSlot.transform.SetSiblingIndex (0);
				beforeRankIndex = 0;
			}
		} else {
			UI_League.Instance.MyLeagueCrewSlot.transform.SetSiblingIndex ((int)beforeRankIndex);

			float temp2 = beforeRankIndex/slotCount;

			MatchList.transform.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1-temp2;
		}
		LayoutRebuilder.MarkLayoutForRebuild (MatchList.GetComponent<RectTransform> ());

		if (currentRankIndex != UI_League.Instance.MyLeagueCrewSlot.transform.GetSiblingIndex ()) {
			yield return new WaitForSeconds (0.2f);

			LeanTween.scale (UI_League.Instance.MyLeagueCrewSlot, Vector3.one * 0.2f, 0.3f).setEase(LeanTweenType.easeOutBack);

			yield return new WaitForSeconds (0.3f);

			UI_League.Instance.MyLeagueCrewSlot.SetActive (false);
			LayoutRebuilder.MarkLayoutForRebuild (MatchList.GetComponent<RectTransform> ());

			yield return new WaitForEndOfFrame ();
		} else {
			yield break;
		}

		float change = 0;
		float smoothStep = 0f;
		float pos = beforeRankIndex/slotCount;
		float temp3 = currentRankIndex/slotCount;

		while (change < 1f) {
			change += Time.fixedDeltaTime;

			smoothStep = Mathf.SmoothStep(pos,temp3,change);

			MatchList.transform.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1-smoothStep;
			yield return new WaitForEndOfFrame ();
		}

		MatchList.transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 1-temp3;
		UI_League.Instance.MyLeagueCrewSlot.SetActive (true);
		UI_League.Instance.MyLeagueCrewSlot.transform.SetSiblingIndex ((int)currentRankIndex);
		LayoutRebuilder.MarkLayoutForRebuild (MatchList.GetComponent<RectTransform> ());
		LeanTween.scale (UI_League.Instance.MyLeagueCrewSlot, Vector3.one * 1.0f, 0.4f).setEase(LeanTweenType.easeOutBack);
    }

	public void SetDivisionInfo()
	{
		Byte myDivision = Legion.Instance.GetDivision;
		// 디비전 텍스트 셋팅
		if(myDivision > 0)
		{
			tempStringBuilder.Remove(0, tempStringBuilder.Length);
			tempStringBuilder.Append("Sprites/BattleField/league_06.division_t_").Append(myDivision.ToString());
			if (myDivision >= 5) 
			{
				imgDivision.gameObject.SetActive (true);
				imgPlayerDivision.gameObject.SetActive (false);

				imgDivision.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
				imgDivision.SetNativeSize();
			}
			else
			{
				imgDivision.gameObject.SetActive(false);
				imgPlayerDivision.gameObject.SetActive(true);

				imgPlayerDivision.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
				imgPlayerDivision.SetNativeSize();
			}

			// 디비전 아이콘 셋팅
			tempStringBuilder.Remove(0, tempStringBuilder.Length);
			tempStringBuilder.Append("Sprites/BattleField/league_06.division_").Append(myDivision.ToString());
            //imgDivisionMark.sprite = AtlasMgr.Instance.GetSprite (tempStringBuilder.ToString());
            //imgDivisionMark.SetNativeSize ();

            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.LeagueDivision);
		}
		else
		{
			imgDivision.gameObject.SetActive(false);
			imgPlayerDivision.gameObject.SetActive(true);
			// 디비전 텍스트 아이콘 셋팅
			imgPlayerDivision.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_06.division_t_1");
			imgPlayerDivision.SetNativeSize();
			// 디비전 아이콘 셋팅
			//imgDivisionMark.sprite = AtlasMgr.Instance.GetSprite ("Sprites/BattleField/league_06.division_1");
			//imgDivisionMark.SetNativeSize ();
		}

		cDivisionMark.SetDivisionMark(myDivision);
	}

    public void RequestMatchList()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestLeagueMatchList(RecieveMatchList);
    }

    public void RecieveMatchList(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
			return;
        }
        else if(err == Server.ERROR_ID.NONE)
        {
			SetDivisionInfo();
            InitMathingList();
        }
    }

    public void OpenRevengePopup()
    {
        if(UI_League.Instance.RevengeCrew.strLegionName != null)
        {
            RevengePopup.SetActive(true);
            RevengePopup.GetComponent<RevengePopup>().SetData(UI_League.Instance.RevengeCrew);
        }
    }

    public void OnClickSlot()
    {
        Legion.Instance.bLeagueToCharInfo = true;
        OnClickClose(1);
    }
    
    public void RefreashCrewSlots()
    {
		tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(TextManager.Instance.GetText("mark_league_total_power")).Append(" ").Append(Legion.Instance.cLeagueCrew.u4Power);
		txtPlayerCrewPower.text = tempStringBuilder.ToString();
		txtPlayerCrewPower2.text = tempStringBuilder.ToString();
        for(int i=0; i<Legion.Instance.cLeagueCrew.acLocation.Length; i++)
        {
            if(Legion.Instance.cLeagueCrew.acLocation[i] != null)
                //_charSlot[i].SetData((Byte)(((Hero)Legion.Instance.cLeagueCrew.acLocation[i]).u1Index-1));
                _charSlot[i].SetData((Byte)i);
            else
                _charSlot[i].SetData(0, true);
        }
        for(int i=0; i<LeagueCrew.MAX_CHAR_IN_CREW; i++)
        {
            if(Legion.Instance.cLeagueCrew.acLocation[i] != null)
            {
                _charSlot[i].SetLeader(true);
                break;
            }
            else
                _charSlot[i].SetLeader(false);
        }
    }
    //팝업 출력용
    void emptyMethod2(object[] param)
	{

	}
    public void RemovePopupInLeagueScene()
    {
        PopupManager.Instance.RemovePopup(gameObject);
        OnClickClose(0);
    }
    public void RemovePopupInCrewSet()
    {
        PopupManager.Instance.RemovePopup(CrewSetPanel);
        OnClickClose(1);
    }
    public void RemovePopupInCharInfo()
    {
        PopupManager.Instance.RemovePopup(CharacterInfoPopup);
        OnClickClose(2);
    }
    public void RemovePopupInIntro()
    {
        PopupManager.Instance.RemovePopup(IntroduceLeaguePopup);
        OnClickClose(3);
    }
    public void RemovePopupInRank()
    {
        PopupManager.Instance.RemovePopup(RankPopup);
        OnClickClose(4);
    }
    public void RemovePopupInMatch()
    {
        PopupManager.Instance.RemovePopup(MatchPopup);
        OnClickClose(5);
    }
    
    public void OnClickClose(int _type)
    {
        switch(_type)
        {
            case (Byte)CloseType.LEAGUE_SCENE:
                AssetMgr.Instance.SceneLoad("LobbyScene");
                //RemovePopupInLeagueScene();
                break;

			case (Byte)CloseType.CREW_SET:
                //RemovePopupInCrewSet();
                if(Legion.Instance.cLeagueCrew.bDirty)
                {
                    RequestSetLeagueCrew();
                }
                else if(Legion.Instance.bLeagueToCharInfo)
                {
                    AssetMgr.Instance.SceneLoad("LobbyScene");
                }
                else if(Legion.Instance.GetDivision == 0)
                {
                    Legion.Instance.bLeagueToCharInfo = false;
                    AssetMgr.Instance.SceneLoad("LobbyScene");
                    
                }
                else
                {
                    RecieveSetLeagueCrew(Server.ERROR_ID.NONE);
                }
                break;

            case (Byte)CloseType.CHAR_INFO:
                //RemovePopupInCharInfo();
                CharacterInfoPopup.SetActive(false);
                break;

            case (Byte)CloseType.INTRO_LEAGUE:
                IntroduceLeaguePopup.SetActive(false);
                break;

            case (Byte)CloseType.RANK:
                //RemovePopupInRank();
                RankPopup.SetActive(false);
                break;

            case (Byte)CloseType.MATCH:
				UI_League.Instance.u8SelectEnemyCrewSN = 0;
				UI_League.Instance.u1SelectEnemyCrewRevenge = 0;
                MatchPopup.SetActive(false);
                break;
        }
    }

    public void RequestSetLeagueCrew()
    {
        Byte[] tempIndex = new Byte[LeagueCrew.MAX_CHAR_IN_CREW];
        for(int i=0; i<LeagueCrew.MAX_CHAR_IN_CREW; i++)
        {
            if(Legion.Instance.cLeagueCrew.acLocation[i] != null)
                tempIndex[i] = ((Hero)Legion.Instance.cLeagueCrew.acLocation[i]).u1Index;
            else
                tempIndex[i] = 0;
        }
        bool bEmptyCrew = false;
        for(int i=0; i<3; i++)
        {
            if(Legion.Instance.cLeagueCrew.acLocation[i] != null)
            {
                bEmptyCrew = false;
                break;
            }
            else
            {
                bEmptyCrew = true;
            }
        }
        if(bEmptyCrew)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_league_member_min"), Server.ServerMgr.Instance.CallClear);
			return;
        }
        else
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.RequestLeagueSetCrew(tempIndex, RecieveSetLeagueCrew);
        }
    }

    public void RecieveSetLeagueCrew(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        
        if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CREW_OPENSLOT, err), Server.ServerMgr.Instance.CallClear);
			return;
		}

		else if (err == Server.ERROR_ID.NONE) 
		{
            MainPanel.SetActive(true);
            CrewSetPanel.SetActive(false);
            RefreashCrewSlots();
            if(!registLeague && Legion.Instance.cLeagueCrew.u1Count > 0)
            {
                if(Legion.Instance.bLeagueToCharInfo)
                    OnClickClose(0);
                else
                    RequestMatchList();
            }
            else if(Legion.Instance.bLeagueToCharInfo)
                OnClickClose(0);
        }
    }

    public void OnClickOpenSetCrew()
    {
        PopupManager.Instance.AddPopup(CrewSetPanel, RemovePopupInCrewSet);
        MainPanel.SetActive(false);
        CrewSetPanel.SetActive(true);
    }

    public void OnClickOpenLeagueIntro()
    {
        IntroduceLeaguePopup.SetActive(true);
        PopupManager.Instance.AddPopup(IntroduceLeaguePopup, RemovePopupInIntro);
    }

    public void OnClickOpenLegendRank()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestLeagueLegend(RecieveLeagueLegendList);
    }

    public void RecieveLeagueLegendList(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        
        if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CREW_OPENSLOT, err), Server.ServerMgr.Instance.CallClear);
			return;
		}

		else if (err == Server.ERROR_ID.NONE) 
		{
            PopupManager.Instance.AddPopup(RankPopup, RemovePopupInRank);
            RankPopup.SetActive(true);
        }
    }

    public void ReloadkMatch()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestLeagueMatch(UI_League.Instance.u8SelectEnemyCrewSN, UI_League.Instance.u1SelectEnemyCrewRevenge, ReceiveMatchPlayer);
    }

    public void ReceiveMatchPlayer(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
			return;
        }
        else if(err == Server.ERROR_ID.NONE)
        {
            UI_League.Instance.CreateEnemyCrew();
            //StartLeague();
            ShowBattleScreen();
        }
    }

    public void ShowBattleScreen()
    {
        PopupManager.Instance.AddPopup(MatchPopup, RemovePopupInMatch);
        MatchPopup.SetActive(true);
    }

    public void OnClickEnemyCharSlot(int _index)
    {
        if(UI_League.Instance.EnemyCrew.acLocation[_index] == null)
            return;
        PopupManager.Instance.AddPopup(CharacterInfoPopup, RemovePopupInCharInfo);
        CharacterInfoPopup.SetActive(true);
        CharacterInfoPopup.GetComponent<CharInfoPopup>().SetData((Hero)UI_League.Instance.EnemyCrew.acLocation[_index]);
    }

    public void OnClickRewardPopup()
    {
		if(Legion.Instance.CheckEmptyInven() == false)
			return;

        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestLeagueReawrd(RecieveLeagueReward);
    }

    public void RecieveLeagueReward(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        
        if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CREW_OPENSLOT, err), Server.ServerMgr.Instance.CallClear);
			return;
		}

		else if (err == Server.ERROR_ID.NONE) 
		{
            Goods tempGoods = new Goods();
            for(int i=0; i<UI_League.Instance._leagueReward.u1DivRwdCount; i++)
            {
                tempGoods.u1Type = UI_League.Instance._leagueReward.lstDivRwdItem[i].u1DivRwdType;
                tempGoods.u2ID = UI_League.Instance._leagueReward.lstDivRwdItem[i].u2DivRwdID;
                tempGoods.u4Count = UI_League.Instance._leagueReward.lstDivRwdItem[i].u4DivRwdNumber;
                Legion.Instance.AddGoods(tempGoods);
                //Legion.Instance.cInventory.AddItem(0, tempGoods.u2ID, tempGoods.u4Count);
            }
            for(int i=0; i<UI_League.Instance._leagueReward.u1PromotionCount; i++)
            {
                for(int j=0; j<UI_League.Instance._leagueReward.lstProRwdItem[i].u1ProRwdCount; j++)
                {
                    tempGoods.u1Type = UI_League.Instance._leagueReward.lstProRwdItem[i].lstProRwdItem[j].u1DivRwdType;
                    tempGoods.u2ID = UI_League.Instance._leagueReward.lstProRwdItem[i].lstProRwdItem[j].u2DivRwdID;
                    tempGoods.u4Count = UI_League.Instance._leagueReward.lstProRwdItem[i].lstProRwdItem[j].u4DivRwdNumber;
                    Legion.Instance.AddGoods(tempGoods);
                    //Legion.Instance.cInventory.AddItem(0, tempGoods.u2ID, tempGoods.u4Count);
                }
            }
            Btn_Rwd.interactable = false;
            RewardPopup.SetActive(true);
        }
    }

	UInt32 GetBuyKeyNeedCount()
	{
		UInt64 NeedCount = LegionInfoMgr.Instance.GetLeagueKeyCharge._cLeagueKeyCharge.u4Count;
		if (Legion.Instance.u1LeagueKeyBuyCount > 0)
			NeedCount += (UInt64)((ushort)Legion.Instance.u1LeagueKeyBuyCount * LegionInfoMgr.Instance.GetLeagueKeyCharge.u2LeagueKeyChargeUpgrade);

		if (NeedCount > LegionInfoMgr.Instance.GetLeagueKeyCharge.u2LeagueKeyChargePriceMax)
			NeedCount = LegionInfoMgr.Instance.GetLeagueKeyCharge.u2LeagueKeyChargePriceMax;

		return (UInt32)NeedCount;
	}

	public void OnClickBuyKey()
	{
		if(!Legion.Instance.CheckEnoughGoods(2,GetBuyKeyNeedCount())){
			PopupManager.Instance.ShowChargePopup(2); 
			return;
		}

		string tempStr = string.Format(TextManager.Instance.GetText("popup_desc_league_key_buy"), GetBuyKeyNeedCount(), LegionInfoMgr.Instance.GetLeagueKeyCharge.u2LeagueKeyCharge);

		PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_league_key_buy"), tempStr, TextManager.Instance.GetText("btn_league_key_charge"), BuyKey, null);
	}

	void BuyKey(object[] param)
	{
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequestLeagueBuyKey(BuyKeyResult);
	}

	public void BuyKeyResult(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.LEAGUE_BUY_KEY, err), Server.ServerMgr.Instance.CallClear);
			return;
		}

		else if (err == Server.ERROR_ID.NONE) 
		{
			//Legion.Instance.AddGoods ( new Goods((Byte)GoodsType.LEAGUE_KEY,0,LegionInfoMgr.Instance.GetLeagueKeyCharge.u2LeagueKeyCharge) );
			Legion.Instance.SubGoods ( new Goods((Byte)GoodsType.CASH,0,GetBuyKeyNeedCount()) );
			if(Legion.Instance.u1LeagueKeyBuyCount < 255 )Legion.Instance.u1LeagueKeyBuyCount++;
		}
	}

	public void StartLeague()
    {
        if(!Legion.Instance.CheckEnoughGoods((int)GoodsType.LEAGUE_KEY, LeagueInfoMgr.Instance.dicLeagueData[Legion.Instance.GetDivisionID].cPlayGoods.u4Count))
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_league_key_not"), TextManager.Instance.GetText("popup_desc_league_key_not"), null);
            return;
        }
        UI_League.Instance.u8SelectEnemyCrewSN = 0;
        UI_League.Instance.u1SelectEnemyCrewRevenge = 0;
		StartCoroutine(ChangeScene());
	}

	private IEnumerator ChangeScene()
	{
		FadeEffectMgr.Instance.FadeOut(1f);
		yield return new WaitForSeconds(1f);
		AssetMgr.Instance.SceneLoad("ALeagueLoading");
	}

    public override IEnumerator CheckReservedPopup()
	{
        yield return new WaitForEndOfFrame();
    }
    
    public override void RefreshAlram()
    {
    }
}
