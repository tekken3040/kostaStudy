using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.EventSystems
{
	public interface ChangeCharEvent : IEventSystemHandler 
	{
		void ChangedSlotToSlot (Hero swapHero, Hero inHero, Byte slotNumInCrew, Byte swapSlotNumInCrew);

		void ChangedListToSlot (Hero inHero, Byte slotNumInCrew);
		
		void ChangedSlotToList (Hero outHero);		
	}
}

public class UI_CrewMenu : MonoBehaviour, ChangeCharEvent
{
    LobbyScene _lobbyScene;                         //로비씬
    public GameObject MainMenu;                     //메인 메뉴
    public GameObject UserInfo;                     //유저 정보
    GameObject PrefCharSlot;                 //캐릭터 슬롯 프리펩
    GameObject PrefCharElement;              //캐릭터 아이콘 프리펩
    public GameObject CharSlotList;                 //캐릭터 슬롯 리스트
    public GameObject CharList;                     //캐릭터 리스트
    GameObject ObjAutoSetChar;               //캐릭터 자동 셋팅 팝업
    GameObject ObjUnlockPopup;               //캐릭터 자동 셋팅 팝업
    GameObject ObjNameInputPopup;            //레기온 이름 입력 팝업
    public GameObject[] CharacterSlot;              //캐릭터 슬롯
    public GameObject[] BG_CharacterSlot;           //실제 캐릭터 배치 슬롯
    public GameObject[] Btn_Crew;                   //크루 버튼
    public GameObject[] Btn_SlotOpen;               //슬롯 오픈
    public Button Btn_Back;                         //백버튼
    public Button Btn_CreateChar;                   //캐릭터 생성
    public Button Btn_SetAutoChar;                  //캐릭터 배치 버튼
    public GameObject Pref_Indicate;                //인디케이터 프리펩
    public GameObject Pref_Indicate2;               //인디케이터 프리펩2
    public Text ObjCrewPower;                       //크루 전투력

    public Vector3 v3CharacterRotation;             //캐릭터 배치 회전값
    //public Vector3 v3CharacterScale;                //캐릭터 배치 스케일값
    public Vector3[] v3CharacterPos;                //캐릭터 로컬 배치 좌표
    //public Vector3[] mainCharacterPostion;          //메인 씬 캐릭터 배치 좌표
    public Vector3[] CharacterPostion;              //캐릭터 배치 좌표

    public Text txtCreationLimitCount;              // 캐릭터 생성 제한 갯수

    public AudioClip unlockCrewSnd;

    //int SelectedCrewNum;                          //현재 선택된 크루 번호
    int PrevCrewNum;                                //이전 크루 번호
    int SelectedSlot;                               //현재 선택된 슬롯
    bool bTutorialBack;
    bool bTutorialBack2 = false;
    bool bShowCharInfo;
    Crew cSelectedCrew;                             //선택된 크루
    Crew cPrevSelectedCrew;                         //이전 크루
    Hero cSelectedHero;                             //선택된 캐릭터
    Byte u1SelectedCrewNum;                         //선택된 크루 번호

    //UInt32 u4CrtCrewPower;
    //UInt32 u4TargetCrewPower;
    StringBuilder tempStringBuilder;
    bool bCrew1_2 = false;

    void Awake()
    {
        tempStringBuilder = new StringBuilder();
        _lobbyScene = GameObject.Find("LobbyScene").GetComponent<LobbyScene>();
        MainMenu = _lobbyScene._mainMenu;
        UserInfo = _lobbyScene._userInfo;
        for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
        {
            CharacterSlot[i] = GameObject.Find("Pref_UI_Main_CharacterPostion").transform.GetChild(i).gameObject;
            BG_CharacterSlot[i] = GameObject.Find("Pref_Main_BG_Character_Pos").transform.GetChild(i).gameObject;
        }
        PrefCharSlot = AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/CharacterSlot_.prefab", typeof(GameObject)) as GameObject;
        PrefCharElement = AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/Character_.prefab", typeof(GameObject)) as GameObject;
        ObjAutoSetChar = AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/Crew/Pref_UI_AutoSetChar.prefab", typeof(GameObject)) as GameObject;
        ObjUnlockPopup = AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/Crew/Pref_UI_GoodsPopup.prefab", typeof(GameObject)) as GameObject;
        //ObjNameInputPopup = AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/Crew/Pref_UI_NameInputPopup.prefab", typeof(GameObject)) as GameObject;
        ObjNameInputPopup = this.transform.FindChild("Pref_UI_NameInputPopup").gameObject;
    }

    //에러시 서버 요청 초기화
    void emptyMethod(object[] param)
    {
        Server.ServerMgr.Instance.ClearFirstJobError();
    }
    //팝업 출력용
    void emptyMethod2(object[] param)
    {

    }

    public Crew GetSelectedCrew()
    {
        return cSelectedCrew;
    }

    public Crew GetPrevSelectedCrew()
    {
        return cPrevSelectedCrew;
    }

    bool CheckDownloadChar() {
        for (int i = 0; i < Legion.Instance.acHeros.Count; i++)
        {
            if (AssetMgr.Instance.CheckDivisionDownload(1, Legion.Instance.acHeros[i].cClass.u2ID))
                return true;
        }

        return false;
    }

    void OnEnable()
    {
        Legion.Instance.googleAnalytics.LogEvent(new EventHitBuilder().SetEventCategory("Crew").SetEventAction("Open").SetEventLabel("CrewOpen"));
        bTutorialBack = false;
        bShowCharInfo = false;
        //1번 크루 3번 슬롯 해금 체크
        if (Legion.Instance.acCrews[0].abLocks[2])
            CheckSlotOpen2();

        if (!Legion.Instance.bCharInfoToCrew)
            cSelectedCrew = Legion.Instance.cBestCrew;
        else
            Legion.Instance.bCharInfoToCrew = false;
        cPrevSelectedCrew = cSelectedCrew;
        u1SelectedCrewNum = cSelectedCrew.u1Index;
        for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
        {
            if (cSelectedCrew.acLocation[i] != null)
            {
                cSelectedHero = (Hero)(cSelectedCrew.acLocation[i]);
                break;
            }
        }
        StartCoroutine(TutorialCallByDelay());
        InitCrewList();
        InitCharacterList();
        SetCrewCharacters();

        cSelectedCrew.StartChanging();
        if (Legion.Instance.bCrewToChar) {
            bTutorialBack = true;
            OnClickCrew(Legion.Instance.cBestCrewBackUp.u1Index - 1);
        } else {
            OnClickCrew(Legion.Instance.SelectedCrew.u1Index - 1);
        }
        //Legion.Instance.tempCrewIndex = -1;

        for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
        {
            CharacterSlot[i].GetComponent<Button>().interactable = false;
            CharacterSlot[i].transform.localPosition = CharacterPostion[i];
            //CharacterSlot[i].GetComponent<RotateCharacter>().enabled = false;
        }

        txtCreationLimitCount.text = string.Format("{0} {1:D2} / {2:D2}", TextManager.Instance.GetText("cha_have_count"), Legion.Instance.acHeros.Count, LegionInfoMgr.Instance.limitCharSlot);

        if (Legion.Instance.sName == "")
            //Invoke("ShowNameInputpopup", 2f);
            ShowNameInputpopup();

        if (Legion.Instance.cTutorial.au1Step[4] == 200)
            PopupManager.Instance.AddPopup(gameObject, OnClickBack);
    }
    IEnumerator TutorialCallByDelay()
    {

        //yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        if (_lobbyScene.prevScreen != 2 || _lobbyScene.prevScreen != 1)
            FadeEffectMgr.Instance.FadeIn();
        if (Legion.Instance.cTutorial.au1Step[4] != 200)
        {
            _lobbyScene._userCash.SetActive(false);
            _lobbyScene._userGold.SetActive(false);
            if (Legion.Instance.sName == "")
            {
                Legion.Instance.cTutorial.au1Step[4] = 0;
            }
            else if (Legion.Instance.acHeros.Count == 1)
            {
                Legion.Instance.cTutorial.au1Step[4] = 1;
            }
            else if (Legion.Instance.acCrews[0].u1Count != 2)
            {
                Legion.Instance.cTutorial.au1Step[4] = 2;
                Btn_CreateChar.interactable = false;
                Btn_SetAutoChar.interactable = false;
                Btn_Back.interactable = false;
            }
            else if (Legion.Instance.acCrews[0].u1Count == 2)
            {
                Legion.Instance.cTutorial.au1Step[4] = 3;
            }
            else
            {
                Legion.Instance.cTutorial.au1Step[4] = 4;
            }

            if (Legion.Instance.cTutorial.CheckTutorial(MENU.CREW)) {
                bTutorialBack2 = true;
            }

            for (int i = 0; i < Legion.MAX_CREW_OF_LEGION; i++)
            {
                Btn_Crew[i].GetComponent<Button>().interactable = false;
            }

            Btn_SlotOpen[0].SetActive(false);
            if (Legion.Instance.acCrews[0].abLocks[1])
                CheckSlotOpen();
        }
        else
        {
            Btn_Back.interactable = true;
            Btn_CreateChar.interactable = true;
            Btn_SetAutoChar.interactable = true;

            for (int i = 0; i < Legion.MAX_CREW_OF_LEGION; i++)
            {
                Btn_Crew[i].GetComponent<Button>().interactable = true;
            }
        }
        yield return null;
    }
    //크루 튜토리얼 완료후 호출할것
    public void CheckSlotOpen()
    {
        UInt16 tempStageNum = LegionInfoMgr.Instance.acCrewOpenGoods[0][1].u2ID;
        //if(Legion.Instance.CheckEnoughGoods((int)LegionInfoMgr.Instance.acCrewOpenGoods[0][1].u1Type, (int)LegionInfoMgr.Instance.acCrewOpenGoods[0][1].u4Count))
        if (StageInfoMgr.Instance.dicStageData[tempStageNum].clearState > 0)
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.OpenSlotOfCrew(Legion.Instance.acCrews[0], 1, UnlockCharacterSlotSuccess);
        }
    }
    //크루 입장시 검사
    public void CheckSlotOpen2()
    {
        UInt16 tempStageNum = LegionInfoMgr.Instance.acCrewOpenGoods[0][2].u2ID;

        if (StageInfoMgr.Instance.dicStageData[tempStageNum].clearState > 0)
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            bCrew1_2 = true;
            Server.ServerMgr.Instance.OpenSlotOfCrew(Legion.Instance.acCrews[0], 2, UnlockCharacterSlotSuccess);
        }
    }
    public void OnClickBack()
    {
        //object[] yesEventParam = new object[1];
        if (Legion.Instance.cTutorial.au1Step[4] != 200)
        {
            OnClickBack2(null);
            return;
        }
        //PopupManager.Instance.ShowYesNoPopup("경고", "현재 크루가 메인 크루로 설정됩니다.\n계속 하시겠습니까?", OnClickBack2, yesEventParam);
        OnClickBack2(null);
        return;
    }
    public void OnClickBack2(object[] param)
    {
        if (cSelectedCrew.u1Count == 0)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("no_hero_in_crew"), emptyMethod2);
            return;
        }
        else if (cSelectedCrew.DispatchStage != null)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_main_setting_dispatch"), null);
            return;
        }
        //
        //        else if(cSelectedCrew.DispatchLeague != 0)
        //        {
        //            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_main_setting_dispatch"), null);
        //			return;
        //        }
        else
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            if (cSelectedCrew.bDirty)
            {
                cSelectedCrew.CallServer(BackSuccess);
                for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
                    Legion.Instance.v3MainCharRotation[i] = Vector3.zero;
            }
            else if (cPrevSelectedCrew != cSelectedCrew)
            {
                cSelectedCrew.CallServer(BackSuccess);
                for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
                    Legion.Instance.v3MainCharRotation[i] = Vector3.zero;
            }
            else
                BackSuccess2();
            //BackSuccess(Server.ERROR_ID.NONE);
        }
    }
    //크루 변경 완료
    public void BackSuccess(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CREW_CHANGE, err), Server.ServerMgr.Instance.CallClear);
            return;
        }

        else if (err == Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            cSelectedCrew = Legion.Instance.acCrews[u1SelectedCrewNum - 1];
            Server.ServerMgr.Instance.SelectCrew(cSelectedCrew, ChangeBestCrew);
            if (bTutorialBack2) {
                _lobbyScene.CheckLoginPopupStep();
                bTutorialBack2 = false;
            }
        }
    }
    public void BackSuccess2()
    {
        ChangeBestCrew(Server.ERROR_ID.NONE);
    }
    //크루 변경후 크루창 닫기
    public void ChangeBestCrew(Server.ERROR_ID err)
    {
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CREW_SELECT, err), Server.ServerMgr.Instance.CallClear);
            return;
        }

        else if (err == Server.ERROR_ID.NONE)
        {
			if (GuildInfoMgr.Instance.u1GuildCrewIndex == 0) {
				GuildInfoMgr.Instance.u1GuildCrewIndex = u1SelectedCrewNum;
			}

            if (bShowCharInfo)
                ShowCharacterInfoFromCrew();
            else
                Close();
            bShowCharInfo = false;
        }
    }
    private void ShowCharacterInfoFromCrew()
    {
        PopupManager.Instance.CloseLoadingPopup();
        cSelectedCrew.StartChanging();
        //transform.root.GetComponent<LobbyScene>().StartCoroutine(transform.root.GetComponent<LobbyScene>().OnClickCharacterInfo(SelectedSlot, cSelectedCrew.u1Index, null, 2));
        _lobbyScene.StartCoroutine(_lobbyScene.OnClickCharacterInfo(SelectedSlot, cSelectedCrew.u1Index, null, 2));
        _lobbyScene.prevScreen = 2;
        this.gameObject.SetActive(false);
    }
    private void Close()
    {
        if (Legion.Instance.cTutorial.au1Step[4] == 200)
            PopupManager.Instance.RemovePopup(gameObject);
        PopupManager.Instance.CloseLoadingPopup();
        FadeEffectMgr.Instance.FadeOut();
        StartCoroutine(GoToLobby());
    }

    IEnumerator GoToLobby()
    {
        yield return new WaitForSeconds(0.2f);
        _lobbyScene.ChangeCamView("LOBBY");
        Legion.Instance.bCrewToChar = false;
        if (Legion.Instance.bStageToCrew)
        {
            _lobbyScene.OnClickCampaign();
            Legion.Instance.bStageToCrew = false;
            yield break;
        }
        //_lobbyScene.transform.GetChild(0).GetComponent<Image>().sprite = AssetMgr.Instance.AssetLoad("Sprites/Main/bg_main.png", typeof(Sprite)) as Sprite;
        for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
        {
            //CharacterSlot[i].transform.localPosition = mainCharacterPostion[i];
            BG_CharacterSlot[i].transform.localPosition = _lobbyScene._characterRootPostion[i];
            //CharacterSlot[i].transform.localPosition = _lobbyScene.GetComponent<LobbyScene>()._characterRootPostion[i];
            CharacterSlot[i].GetComponent<Button>().interactable = true;
            //CharacterSlot[i].GetComponent<RotateCharacter>().enabled = true;
        }
        MainMenu.gameObject.SetActive(true);
        if (Legion.Instance.sName != null)
            UserInfo.gameObject.SetActive(true);
        //LobbyScene lobbyScene = _lobbyScene.GetComponent<LobbyScene>();
        _lobbyScene._userKey.SetActive(true);
        _lobbyScene._userCash.SetActive(true);
        _lobbyScene._userGold.SetActive(true);
        _lobbyScene.ResetCharacterTransform();
        _lobbyScene.Btn_ShowMenu.SetActive(true);
        _lobbyScene.InitCrewEmblem();
        _lobbyScene.RefreshAlram();
        _lobbyScene.ShowMenu();
        _lobbyScene._dirLight.transform.eulerAngles = _lobbyScene._lightRotation[0];
        this.gameObject.SetActive(false);
        Legion.Instance.cTutorial.CheckTutorial(MENU.CREW);

        for (int i = 0; i < CharList.transform.childCount; i++)
        {
            GameObject.Destroy(CharList.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < CharSlotList.transform.childCount; i++)
        {
            GameObject.Destroy(CharSlotList.transform.GetChild(i).gameObject);
        }
        _lobbyScene._goods.SetActive(true);
        FadeEffectMgr.Instance.FadeIn();
    }
    //크루 선택
    Byte unlockCrewIdx;
    UInt32 tempPrice;
    public void OnClickCrew(int crewNum)
    {
        object[] yesEventParam = new object[1];
        //if(Legion.Instance.tempCrewIndex < 0) return;
        if ((cSelectedCrew.u1Index - 1) == crewNum) return;
        //크루가 잠겨있으면 언락
        if (Legion.Instance.acCrews[crewNum].IsLock)
        {
            if (cSelectedCrew.u1Count == 0)
            {
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("no_hero_in_crew").ToString(), emptyMethod2);
                return;
            }

            yesEventParam[0] = crewNum;
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            //재화 필요 팝업
            GameObject pop = Instantiate(ObjUnlockPopup);
            pop.transform.SetParent(PopupManager.Instance._objPopupManager.transform);
            pop.transform.localScale = Vector3.one;
            pop.transform.localPosition = Vector3.zero;

            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(crewNum + 1).Append(TextManager.Instance.GetText("popup_desc_crew_unlock"));

            pop.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = tempStringBuilder.ToString();

            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append("Sprites/Common/common_02_renew.icon_");

            //if(Legion.Instance.GetConsumeString((int)LegionInfoMgr.Instance.acCrewOpenGoods[crewNum][0].u1Type).ToString() == GoodsType.GOLD.ToString())
            if (LegionInfoMgr.Instance.acCrewOpenGoods[crewNum][0].u1Type == (Byte)GoodsType.GOLD)
                tempStringBuilder.Append("Gold");
            //else if(Legion.Instance.GetConsumeString((int)LegionInfoMgr.Instance.acCrewOpenGoods[crewNum][0].u1Type).ToString() == GoodsType.CASH.ToString())
            else if (LegionInfoMgr.Instance.acCrewOpenGoods[crewNum][0].u1Type == (Byte)GoodsType.CASH)
                tempStringBuilder.Append("Cash");
            pop.transform.GetChild(0).GetChild(6).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
            pop.transform.GetChild(0).GetChild(6).GetComponent<Image>().SetNativeSize();

            tempPrice = LegionInfoMgr.Instance.acCrewOpenGoods[crewNum][0].u4Count;

            EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo(DISCOUNT_ITEM.OPENCREW);
            if (disInfo != null) {
                pop.GetComponent<StatResetPopup>().SetDiscount(tempPrice, disInfo.u1DiscountRate);
                tempPrice = (uint)(tempPrice * disInfo.discountRate);
            }

            pop.GetComponent<YesNoPopup>().Show(TextManager.Instance.GetText("popup_title_crew_unlock"), tempPrice.ToString(), UnlockCrew, yesEventParam);
            PopupManager.Instance.AddPopup(pop, pop.GetComponent<YesNoPopup>().OnClickNo);
            return;
        }

		if (Legion.Instance.acCrews [crewNum].DispatchStage != null) {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_retire_error_dispatch_ing"), null);
			return;
		}

        if (cSelectedCrew == Legion.Instance.acCrews[crewNum])
            return;
        unlockCrewIdx = Convert.ToByte(crewNum);
        //크루 변경
        //크루에 변동사항 있음
        if (cSelectedCrew.bDirty)
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            cSelectedCrew.CallServer(OnChangedCrew);
        }
        //크루 변경사항 없음
        else
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            OnChangedCrew(Server.ERROR_ID.NONE);
        }
    }
    //크루 언락
    public void UnlockCrew(object[] param)
    {
        Byte crewNum = Convert.ToByte(param[0]);
        unlockCrewIdx = Convert.ToByte(crewNum);
        //크루 언락시 재화 소비
        switch (crewNum)
        {
            case 0:
                break;
            case 1: case 2: case 3: case 4:
                if (!Legion.Instance.CheckEnoughGoods((int)LegionInfoMgr.Instance.acCrewOpenGoods[crewNum][0].u1Type, (int)tempPrice))
                {
                    PopupManager.Instance.ShowChargePopup(LegionInfoMgr.Instance.acCrewOpenGoods[crewNum][0].u1Type);
                    return;
                }

                break;
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.OpenCrew(Legion.Instance.acCrews[unlockCrewIdx], "#" + crewNum, UnlockSuccessCrew);
    }
    //크루 언락 성공
    public void UnlockSuccessCrew(Server.ERROR_ID err)
    {
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CREW_OPEN, err), Server.ServerMgr.Instance.CallClear);
            return;
        }

        else if (err == Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            SoundManager.Instance.PlayEff(unlockCrewSnd);
            Btn_Crew[unlockCrewIdx].transform.GetChild(0).gameObject.SetActive(true);
            Btn_Crew[unlockCrewIdx].transform.GetChild(1).gameObject.SetActive(false);
            Btn_Crew[unlockCrewIdx].transform.GetChild(3).GetComponent<Text>().color = new Color(1, 1, 1, 1);

            //if (unlockCrewIdx < 3)
            Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.CrewOpen, 0, (Byte)(unlockCrewIdx + 1), 0, 0, 1);
            if (cSelectedCrew.bDirty)
            {
                PopupManager.Instance.ShowLoadingPopup(1);
                cSelectedCrew.CallServer(OnChangedCrew);
            }
            else
                OnChangedCrew(err);
        }
    }
    //크루 변경
    public void OnChangedCrew(Server.ERROR_ID err)
    {
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CREW_CHANGE, err), Server.ServerMgr.Instance.CallClear);
            return;
        }

        else if (err == Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            if (cSelectedCrew.u1Count == 0)
            {
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_crew_new"/*"no_hero_in_crew"*/), emptyMethod2);
                return;
            }

            PrevCrewNum = cSelectedCrew.u1Index;
            cPrevSelectedCrew = cSelectedCrew;
            cSelectedCrew = Legion.Instance.acCrews[unlockCrewIdx];
            DebugMgr.Log(unlockCrewIdx);
            u1SelectedCrewNum = cSelectedCrew.u1Index;
            for (int i = 0; i < Legion.MAX_CREW_OF_LEGION; i++)
            {
                if (i != unlockCrewIdx)
                    LeanTween.scale(Btn_Crew[i].GetComponent<RectTransform>(), Vector3.one, 0.3f);
            }
            LeanTween.scale(Btn_Crew[unlockCrewIdx].GetComponent<RectTransform>(), new Vector3(1.2f, 1.2f), 0.3f);
            ResetInCrewCharacter();
            SetCrewCharacters();
            //InitCrewFlag();
            cSelectedCrew.StartChanging();

            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.CrewOpen);
        }
    }
    //슬롯 및 캐릭터 추가
    byte UnLockSlotIdx;
    public void OnClickAddSlot(int slotNum)
    {
        object[] yesEventParam = new object[1];
        yesEventParam[0] = slotNum;
        UnLockSlotIdx = Convert.ToByte(slotNum);
        unlockCrewIdx = Convert.ToByte(cSelectedCrew.u1Index - 1);
        SelectedSlot = slotNum;
        Legion.Instance.cSelectedSlot = SelectedSlot;

        if (cSelectedCrew.abLocks[slotNum])
        {
            //슬롯 언락 재화 검사
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            switch (unlockCrewIdx)
            {
                case 0:
                    if (slotNum == 2)
                    {
                        tempStringBuilder.Remove(0, tempStringBuilder.Length);
                        tempStringBuilder.Append(TextManager.Instance.GetText("popup_desc_slot_term_unlock_act"));
                        if (Convert.ToInt16(LegionInfoMgr.Instance.acCrewOpenGoods[0][2].u2ID.ToString().Substring(3, 1)) != 0)
                        {
                            tempStringBuilder.Append(Convert.ToInt16(LegionInfoMgr.Instance.acCrewOpenGoods[0][2].u2ID.ToString().Substring(2, 1)) + 1);
                            tempStringBuilder.Append("-");
                            tempStringBuilder.Append(LegionInfoMgr.Instance.acCrewOpenGoods[0][2].u2ID.ToString().Substring(3, 1));
                        }
                        else
                        {
                            tempStringBuilder.Append(Convert.ToInt16(LegionInfoMgr.Instance.acCrewOpenGoods[0][2].u2ID.ToString().Substring(2, 1)));
                            tempStringBuilder.Append("-10");
                        }

                        tempStringBuilder.Append(TextManager.Instance.GetText("popup_desc_slot_term_unlock"));
                        PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_slot_term_unlock"), tempStringBuilder.ToString(), null);
                    }
                    return;
                case 1: case 2: case 3: case 4:
                    tempStringBuilder.Append(LegionInfoMgr.Instance.acCrewOpenGoods[unlockCrewIdx][slotNum].u4Count.ToString()).Append(" ").Append(Legion.Instance.GetConsumeString((int)LegionInfoMgr.Instance.acCrewOpenGoods[unlockCrewIdx][slotNum].u1Type)).Append("를 소모하여 슬롯을 개방하시겠습니까?");
                    break;
            }

            GameObject pop = Instantiate(ObjUnlockPopup);
            pop.transform.parent = PopupManager.Instance._objPopupManager.transform;
            pop.transform.localScale = Vector3.one;
            pop.transform.localPosition = Vector3.zero;
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(slotNum + 1).Append(TextManager.Instance.GetText("popup_desc_slot_cost_unlock"));
            pop.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = tempStringBuilder.ToString();
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append("Sprites/Common/common_02_renew.icon_");
            if (unlockCrewIdx != 0)
            {
                //if(Legion.Instance.GetConsumeString((int)LegionInfoMgr.Instance.acCrewOpenGoods[unlockCrewIdx][slotNum].u1Type).ToString() == GoodsType.GOLD.ToString())
                if ((GoodsType)LegionInfoMgr.Instance.acCrewOpenGoods[unlockCrewIdx][slotNum].u1Type == GoodsType.GOLD)
                    tempStringBuilder.Append("Gold");
                //else if(Legion.Instance.GetConsumeString((int)LegionInfoMgr.Instance.acCrewOpenGoods[unlockCrewIdx][slotNum].u1Type).ToString() == GoodsType.CASH.ToString())
                else if ((GoodsType)LegionInfoMgr.Instance.acCrewOpenGoods[unlockCrewIdx][slotNum].u1Type == GoodsType.CASH)
                    tempStringBuilder.Append("Cash");
            }
            else
            {
                tempStringBuilder.Append("Gold");
            }

            pop.transform.GetChild(0).GetChild(6).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
            pop.transform.GetChild(0).GetChild(6).GetComponent<Image>().SetNativeSize();
            pop.GetComponent<YesNoPopup>().Show(TextManager.Instance.GetText("popup_title_slot_term_unlock"), LegionInfoMgr.Instance.acCrewOpenGoods[unlockCrewIdx][slotNum].u4Count.ToString(), UnlockCharacterSlot, yesEventParam);
            //PopupManager.Instance.ShowYesNoPopup("", TextManager.Instance.GetErrorText("crew_open_slot").ToString(), UnlockCharacterSlot, yesEventParam);
            PopupManager.Instance.AddPopup(pop, pop.GetComponent<YesNoPopup>().OnClickNo);
            return;
        }

        else
        {
            if (cSelectedCrew.acLocation[slotNum] == null)
            {
                return;
            }
            else
            {
                if (cSelectedCrew.DispatchStage != null)
                {
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_retire_error_dispatch_ing"), null);
                    return;
                }

                //                if(cSelectedCrew.DispatchLeague != 0)
                //                {
                //                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_retire_error_dispatch_ing"), null);
                //		        	return;
                //                }
                PopupManager.Instance.ShowLoadingPopup(1);
                bShowCharInfo = true;
                if (cSelectedCrew.bDirty)
                {
                    cSelectedCrew.CallServer(BackSuccess);
                    for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
                        Legion.Instance.v3MainCharRotation[i] = Vector3.zero;
                }
                else if (cPrevSelectedCrew != cSelectedCrew)
                {
                    cSelectedCrew.CallServer(BackSuccess);
                    for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
                        Legion.Instance.v3MainCharRotation[i] = Vector3.zero;
                }
                else
                    BackSuccess2();
                //transform.root.GetComponent<LobbyScene>().StartCoroutine(transform.root.GetComponent<LobbyScene>().OnClickCharacterInfo(slotNum, cSelectedCrew.u1Index, null, 2));
                //_lobbyScene.GetComponent<LobbyScene>().prevScreen = 2;
            }
        }
    }
    //슬롯 언락
    public void UnlockCharacterSlot(object[] param)
    {
        int _slotNumber = Convert.ToInt16(param[0]);
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        switch (unlockCrewIdx)
        {
            case 0:
                break;
            case 1: case 2: case 3: case 4:
                if (!Legion.Instance.CheckEnoughGoods((int)LegionInfoMgr.Instance.acCrewOpenGoods[unlockCrewIdx][_slotNumber].u1Type, (int)LegionInfoMgr.Instance.acCrewOpenGoods[unlockCrewIdx][_slotNumber].u4Count))
                {
                    tempStringBuilder.Append(Legion.Instance.GetConsumeString((int)LegionInfoMgr.Instance.acCrewOpenGoods[unlockCrewIdx][_slotNumber].u1Type)).Append(TextManager.Instance.GetText("popup_desc_nocost"));
                    PopupManager.Instance.ShowChargePopup(LegionInfoMgr.Instance.acCrewOpenGoods[unlockCrewIdx][_slotNumber].u1Type);
                    return;
                }

                break;
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.OpenSlotOfCrew(Legion.Instance.acCrews[unlockCrewIdx], UnLockSlotIdx, UnlockCharacterSlotSuccess);
    }
    //슬롯 언락 성공
    public void UnlockCharacterSlotSuccess(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CREW_OPENSLOT, err), Server.ServerMgr.Instance.CallClear);
            return;
        }

        else if (err == Server.ERROR_ID.NONE)
        {
            SetCrewCharacters();
            int idx = 0;
            if (bCrew1_2)
            {
                idx = 0;
                bCrew1_2 = false;
            }
            else
                idx = cSelectedCrew.u1Index - 1;
            switch (idx)
            {
                case 0:
                    //if(!Legion.Instance.acCrews[0].abLocks[2])
                    //{
                    //    tempStringBuilder.Remove(0, tempStringBuilder.Length);
                    //    tempStringBuilder.Append(TextManager.Instance.GetText("popup_desc_3_slot_unlock"));
                    //    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_slot_term_unlock"), tempStringBuilder.ToString(), null);
                    //}
                    break;
                case 1: case 2: case 3: case 4:
                    //Legion.Instance.SubGoods((int)LegionInfoMgr.Instance.acCrewOpenGoods[idx][SelectedSlot].u1Type, (int)LegionInfoMgr.Instance.acCrewOpenGoods[idx][SelectedSlot].u4Count);
                    break;
            }
        }
    }
    //레기온 이름 입력
    public void ShowNameInputpopup()
    {
        ObjNameInputPopup.SetActive(true);
    }

    public void RequsetInputLegionName()
    {
        GameObject pop = ObjNameInputPopup.transform.GetChild(0).gameObject;
        InputField nameField = pop.transform.GetChild(2).GetComponent<InputField>();
        pop.transform.GetChild(3).GetComponent<Text>().text = "";
        //Server.ServerMgr.Instance.ClearFirstJobError();
        if (nameField.text == "")
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetErrorText("crew_enter_name", "", false));
            pop.transform.GetChild(3).GetComponent<Text>().text = tempStringBuilder.ToString();
            return;
        }
        else if (Regex.Matches(nameField.text, @"[\s\Wㄱ-ㅎㅏ-ㅣ]").Count != 0)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetErrorText("CREATE_ACCOUNT_ID_WRONGCHAR", "", false));
            pop.transform.GetChild(3).GetComponent<Text>().text = tempStringBuilder.ToString();
            return;
        }
        else
        {
            for (int i = 0; i < nameField.text.Length; i++)
            {
                if (nameField.text.Substring(i, 1).Equals(" "))
                {
                    tempStringBuilder.Remove(0, tempStringBuilder.Length);
                    tempStringBuilder.Append(TextManager.Instance.GetErrorText("crew_space_name", "", false));
                    pop.transform.GetChild(3).GetComponent<Text>().text = tempStringBuilder.ToString();
                    return;
                }
            }
            Server.ServerMgr.Instance.SetLegionName(nameField.text, NameChangeSuccess);
            PopupManager.Instance.ShowLoadingPopup(1);
        }
    }
    IEnumerator CallClear()
    {
        yield return new WaitForEndOfFrame();
        Server.ServerMgr.Instance.ClearFirstJobError();
    }

    public void OnChangedNameField()
    {
        ObjNameInputPopup.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = "";
    }

    public void NameChangeSuccess(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        //if(err == Server.ERROR_ID.REQUEST_DUPLICATION)
        //{
        //	tempStringBuilder.Remove(0, tempStringBuilder.Length);
        //	tempStringBuilder.Append(TextManager.Instance.GetError(Server.MSGs.LEGION_SET_NAME, err));
        //    ObjNameInputPopup.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = tempStringBuilder.ToString();
        //    StartCoroutine("CallClear");
        //	return;
        //}
        if (err == Server.ERROR_ID.LEGION_NAME_DUPLICATE)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetError(Server.MSGs.LEGION_SET_NAME, err));
            ObjNameInputPopup.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = tempStringBuilder.ToString();
            StartCoroutine("CallClear");
            return;
        }
        else if (err == Server.ERROR_ID.CREATE_ACCOUNT_ID_WRONGCHAR)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetError(Server.MSGs.LEGION_SET_NAME, err));
            ObjNameInputPopup.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = tempStringBuilder.ToString();
            StartCoroutine("CallClear");
            return;
        }
        else if (err == Server.ERROR_ID.NONE)
        {
            ObjNameInputPopup.SetActive(false);
            //캐릭터 생성유도 튜토리얼
            Btn_CreateChar.interactable = true;
            Legion.Instance.cTutorial.CheckTutorial(MENU.CREW);
            //_lobbyScene._mainMenu.transform.GetChild(1).GetChild(4).GetChild(0).gameObject.SetActive(true);
            //_lobbyScene._mainMenu.transform.GetChild(1).GetChild(4).GetChild(1).gameObject.SetActive(true);
            //_lobbyScene._mainMenu.transform.GetChild(1).GetChild(4).GetChild(3).gameObject.SetActive(false);
            //_lobbyScene.GetComponent<LobbyScene>()._mainMenu.transform.GetChild(1).GetChild(4).GetComponent<Button>().interactable = true;
        }
        else
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
            return;
        }
    }
    //캐릭터생성
    public void OnClickCreateCharacter()
    {
        if (Legion.Instance.cTutorial.au1Step[4] != 200)
        {
            RequestCreateCharacterScene(null);
            return;
        }

        //DebugMgr.LogError(Legion.Instance.acHeros.Count);
        // 현재 생성된 캐릭터의 갯수가 보유 갯수 보다 많다면 안된다는 팝업 띄우기
        if (Legion.Instance.acHeros.Count >= LegionInfoMgr.Instance.limitCharSlot)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_char_create"), TextManager.Instance.GetText("popup_not_create_crew"), null);
        }
        else
        {
            PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_char_create"), TextManager.Instance.GetText("popup_desc_char_create"), RequestCreateCharacterScene, null);
        }
    }
    //캐릭터 생성씬 호출 요청
    public void RequestCreateCharacterScene(object[] param)
    {
        Legion.Instance.cBestCrewBackUp = cSelectedCrew;
        PopupManager.Instance.ShowLoadingPopup(1);

        if (cSelectedCrew.bDirty)
        {
            cSelectedCrew.CallServer(OnCreateCharacterScene);
        }
        //크루 변경사항 없음
        //else if(cPrevSelectedCrew.bDirty/* != cSelectedCrew*/)
        //{
        //    PopupManager.Instance.ShowLoadingPopup(1);
        //    cPrevSelectedCrew.CallServer(OnCreateCharacterScene);
        //	//OnCreateCharacterScene(Server.ERROR_ID.NONE);
        //}
        else
        {
            OnCreateCharacterScene(Server.ERROR_ID.NONE);
        }
    }
    //캐릭터 생성씬 호출
    public void OnCreateCharacterScene(Server.ERROR_ID err)
    {
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CREW_CHANGE, err), Server.ServerMgr.Instance.CallClear);
            return;
        }

        else if (err == Server.ERROR_ID.NONE)
        {
            //PlayerPrefs.SetInt("CrewToChar", 1);
            Legion.Instance.bCrewToChar = true;
            PopupManager.Instance.CloseLoadingPopup();
            StartCoroutine(ChangeCreateCharacterScene());
        }
    }
    //캐릭터 생성씬 이동
    IEnumerator ChangeCreateCharacterScene()
    {
        FadeEffectMgr.Instance.FadeOut();
        yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        AssetMgr.Instance.SceneLoad("CreateCharacterScene");
    }
    //캐릭터 자동 배치
    List<Hero> lstHero = new List<Hero>();
    public void OnClickAutoSetCharacter()
    {
        if (CheckDownloadChar())
            return;
        if (cSelectedCrew.DispatchStage != null)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_retire_error_dispatch_ing"), null);
            return;
        }

        //        if(cSelectedCrew.DispatchLeague != 0)
        //        {
        //            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_retire_error_dispatch_ing"), null);
        //			return;
        //        }
        int noCrewHero = 0;
        for (int i = 0; i < Legion.Instance.acHeros.Count; i++)
        {
            if (Legion.Instance.acHeros[i].u1AssignedCrew == 0)
                noCrewHero++;
            else if (Legion.Instance.acHeros[i].u1AssignedCrew == cSelectedCrew.u1Index)
                noCrewHero++;
        }
        if (noCrewHero == 0)
            return;

        object[] yesEventParam = new object[1];

        //yesEventParam[0] = cHero;
        GameObject pop = Instantiate(ObjAutoSetChar);
        pop.transform.SetParent(PopupManager.Instance._objPopupManager.transform);
        pop.transform.localScale = Vector3.one;
        pop.transform.localPosition = Vector3.zero;
        //pop.GetComponent<UI_Popup_AutoStatAdd>().SetRenderHeroStatus(yesEventParam);
        pop.GetComponent<YesNoPopup>().Show(TextManager.Instance.GetText("popup_title_crew_auto"), TextManager.Instance.GetText("popup_title_crew_auto"), SetCharacter, yesEventParam);
        PopupManager.Instance.AddPopup(pop, pop.GetComponent<YesNoPopup>().OnClickNo);
        return;
    }
    public void SetCharacter(object[] param)
    {
        lstHero.Clear();

        for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
        {
            if (cSelectedCrew.acLocation[i] != null)
            {
                //DebugMgr.LogError(cSelectedCrew.acLocation[i].sName);
                //DebugMgr.LogError(((Hero)cSelectedCrew.acLocation[i]).u4Power);
                if (((Hero)cSelectedCrew.acLocation[i]).cObject != null)
                {
                    ((Hero)cSelectedCrew.acLocation[i]).DestroyModelObject();
                    DebugMgr.LogError("Delete cObject");
                }
                //cSelectedCrew.Resign(cSelectedCrew.acLocation[i]);
            }
        }

        for (int i = 0; i < Legion.Instance.acHeros.Count; i++)
            lstHero.Add(Legion.Instance.acHeros[i]);

        for (int i = 0; i < Legion.Instance.acHeros.Count; i++)
        {
            for (int j = 0; j < Legion.MAX_CREW_OF_LEGION; j++)
            {
                for (int k = 0; k < Crew.MAX_CHAR_IN_CREW; k++)
                {
                    if (Legion.Instance.acCrews[j].u1Index != cSelectedCrew.u1Index)
                    {
                        if (!Legion.Instance.acCrews[j].abLocks[k])
                        {
                            if (Legion.Instance.acCrews[j].acLocation[k] == null)
                                continue;
                            if (Legion.Instance.acCrews[j].acLocation[k] == Legion.Instance.acHeros[i])
                                lstHero.Remove(Legion.Instance.acHeros[i]);
                        }
                    }
                }
            }
        }

        SetAutoCharBtnActive();
        SetHeroPower();
    }
    public class CompareClass : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            int xPower = 0;
            int yPower = 0;
            xPower = (int)((Hero)x).u4Power;
            yPower = (int)((Hero)y).u4Power;
            return ((new CaseInsensitiveComparer()).Compare(yPower, xPower));
        }
    }
    //캐릭터 전투력 체크해서 재배치
    public void SetHeroPower()
    {
        //Character tempOutChar;
        IComparer tempCompare = new CompareClass();
        lstHero.Sort(tempCompare.Compare);
        //CheckHeroPower(0, lstHero.Count - 1);
        /*for(int i=0; i<lstHero.Count; i++)
            lstHero[i].u1AssignedCrew = 0;*/
        /*
        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            if(cSelectedCrew.acLocation[i] != null)
                cSelectedCrew.Resign(cSelectedCrew.acLocation[i]);
        }
        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            cSelectedCrew.Assign(lstHero[i], i, out tempOutChar);
        }
        */
        Invoke("SetCharacterInCrew", 0.0f);
        //SetCharacterInCrew();
        //SetCrewCharacters();
    }
    public void SetCharacterInCrew()
    {
        Character outChar = null;
        int charCnt = 0;
        //cSelectedCrew.Assign(inHero, slotNumInCrew, out outChar);
        for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
        {
            for (int j = charCnt; j < lstHero.Count;)
            {
                if (!cSelectedCrew.abLocks[i])
                {
                    if (lstHero[j].u1AssignedCrew == cSelectedCrew.u1Index)
                    {
                        cSelectedCrew.Resign(lstHero[j]);
                        cSelectedCrew.Assign(lstHero[j], i, out outChar);
                    }
                    else
                    {
                        //Legion.Instance.acCrews[inHero.u1AssignedCrew-1].Resign(inHero);
                        if (cSelectedCrew.acLocation[i] != lstHero[j])
                            cSelectedCrew.Assign(lstHero[j], i, out outChar);
                    }
                    if (outChar != null)
                    {
                        cSelectedCrew.Resign(outChar);
                        for (int k = 0; k < CharList.transform.childCount; k++)
                        {
                            if (((Hero)outChar) == CharList.transform.GetChild(k).GetComponent<UI_CharElement>().cHero)
                                CharList.transform.GetChild(k).gameObject.SetActive(true);
                        }
                    }
                    charCnt++;
                    break;
                }
                else
                    break;
            }
        }

        for (int i = 0; i < CharList.transform.childCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (CharList.transform.GetChild(i).GetComponent<UI_CharElement>().cHero == cSelectedCrew.acLocation[j])
                    CharList.transform.GetChild(i).gameObject.SetActive(false);
                else if (CharList.transform.GetChild(i).GetComponent<UI_CharElement>().cHero == outChar && outChar != null)
                    CharList.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        SetCrewCharacters();
    }
    //캐릭터 전투력 비교 (퀵정렬)
    public void CheckHeroPower(int head, int tail)
    {
        if (head == tail) return;

        int l_hold, r_hold, lastIdx;
        Hero pivot;
        l_hold = head;
        r_hold = tail;
        pivot = lstHero[head];

        while (head < tail)
        {
            while ((lstHero[tail].u4Power <= pivot.u4Power) && (head < tail))
            {
                tail--;
            }
            if (head != tail)
            {
                lstHero[head] = lstHero[tail];
                head++;
            }
            while ((lstHero[head].u4Power <= pivot.u4Power) && (head < tail))
                head++;
            if (head != tail)
            {
                lstHero[tail] = lstHero[head];
                tail--;
            }
        }

        lstHero[tail] = pivot;
        lastIdx = tail;
        head = l_hold;
        tail = r_hold;
        if (lstHero[head].u4Power < pivot.u4Power)
            CheckHeroPower(head, lastIdx - 1);
        if (lstHero[tail].u4Power > pivot.u4Power)
            CheckHeroPower(lastIdx + 1, tail);
    }
    //크루 리스트 초기화(크루 메뉴 호출시)
    public void InitCrewList()
    {
        Color ActiveColor = new Color(1, 1, 1, 1);
        Color DisableColor = new Color(0.5f, 0.5f, 0.5f, 1);
        for (int i = 0; i < Legion.MAX_CREW_OF_LEGION; i++)
        {
            //GetCrewDivision(i);

            if (!Legion.Instance.acCrews[i].IsLock)
            {
                Btn_Crew[i].transform.GetChild(0).gameObject.SetActive(true);
                Btn_Crew[i].transform.GetChild(1).gameObject.SetActive(false);
                Btn_Crew[i].transform.GetChild(3).GetComponent<Text>().color = ActiveColor;
            }

            else
            {
                Btn_Crew[i].transform.GetChild(0).gameObject.SetActive(false);
                Btn_Crew[i].transform.GetChild(1).gameObject.SetActive(true);
                Btn_Crew[i].transform.GetChild(3).GetComponent<Text>().color = DisableColor;
            }
        }
        for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
        {
            if (!cSelectedCrew.abLocks[i])
            {
                if (cSelectedCrew.acLocation[i] != null)
                {
                    Btn_SlotOpen[i].GetComponent<Button>().interactable = true;
                    Btn_SlotOpen[i].GetComponent<ButtonClickAni>().enabled = true;
                    Btn_SlotOpen[i].transform.GetChild(0).gameObject.SetActive(false);
                    Btn_SlotOpen[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                    Btn_SlotOpen[i].transform.GetChild(1).gameObject.SetActive(false);

                    if (((Hero)cSelectedCrew.acLocation[i]).cObject != null)
                    {
                        //((Hero)cSelectedCrew.acLocation[i]).cObject.transform.eulerAngles = v3CharacterRotation;
                        //((Hero)cSelectedCrew.acLocation[i]).cObject.transform.localScale = v3CharacterScale;
                        //((Hero)cSelectedCrew.acLocation[i]).cObject.transform.localPosition = v3CharacterPos[i];
                        //((Hero)cSelectedCrew.acLocation[i]).cObject.transform.localRotation = Quaternion.Euler(v3CharacterRotation);
                        //((Hero)cSelectedCrew.acLocation[i]).cObject.transform.localScale = Vector3.one;
                        //((Hero)cSelectedCrew.acLocation[i]).cObject.transform.localPosition = Vector3.zero;
                        //((Hero)cSelectedCrew.acLocation[i]).cObject.GetComponent<HeroObject>().SetLayer(LayerMask.NameToLayer("UI"));
                        //((Hero)cSelectedCrew.acLocation[i]).cObject.GetComponent<HeroObject>().SetAnimations_UI();
                    }
                    else
                    {
                        if (BG_CharacterSlot[i].transform.childCount != 0)
                            GameObject.Destroy(BG_CharacterSlot[i].transform.GetChild(0).gameObject);
                        //if(CharacterSlot[i].transform.GetChild(0).childCount != 0)
                        //    GameObject.Destroy(CharacterSlot[i].transform.GetChild(0).GetChild(0).gameObject);
                        StartCoroutine(SetModelInList(((Hero)cSelectedCrew.acLocation[i]), i));
                    }
                }
                else
                {
                    Btn_SlotOpen[i].GetComponent<Button>().interactable = false;
                    Btn_SlotOpen[i].GetComponent<ButtonClickAni>().enabled = false;
                    Btn_SlotOpen[i].transform.GetChild(0).gameObject.SetActive(true);
                    Btn_SlotOpen[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    Btn_SlotOpen[i].transform.GetChild(1).gameObject.SetActive(false);
                }

            }
            else
            {
                Btn_SlotOpen[i].GetComponent<Button>().interactable = true;
                Btn_SlotOpen[i].GetComponent<ButtonClickAni>().enabled = true;
                Btn_SlotOpen[i].transform.GetChild(0).gameObject.SetActive(false);
                Btn_SlotOpen[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                Btn_SlotOpen[i].transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        Btn_Crew[cSelectedCrew.u1Index - 1].GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f);
    }
    /*
    public void GetCrewDivision(int crewIndex)
    {
        if(Legion.Instance.GetDivision == 0)
        {
            Btn_Crew[crewIndex].transform.GetChild(0).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.division_d");
            Btn_Crew[crewIndex].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.num_s_" + (crewIndex+1));
            Btn_Crew[crewIndex].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
            Btn_Crew[crewIndex].transform.GetChild(0).GetChild(0).GetComponent<Image>().SetNativeSize();
        }
        else
        {
			switch(0)//Legion.Instance.GetLegend
            {
                case 0:
                    tempStringBuilder.Remove(0, tempStringBuilder.Length);
                    tempStringBuilder.Append("Sprites/Common/flag_01.division_").Append(GetDivision(Legion.Instance.GetDivision));
                    Btn_Crew[crewIndex].transform.GetChild(0).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());

                    tempStringBuilder.Remove(0, tempStringBuilder.Length);
                    if (Legion.Instance.GetDivision < 4)
                        tempStringBuilder.Append("Sprites/Common/flag_01.num_s_" + (crewIndex+1));
                    else
                        tempStringBuilder.Append("Sprites/Common/flag_01.num_g_" + (crewIndex+1));
                    Btn_Crew[crewIndex].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
                    Btn_Crew[crewIndex].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                    Btn_Crew[crewIndex].transform.GetChild(0).GetChild(0).GetComponent<Image>().SetNativeSize();
                    break;

                case 1:
                    tempStringBuilder.Remove(0, tempStringBuilder.Length);
                    tempStringBuilder.Append("Sprites/Common/flag_01.division_").Append("legend");
                    Btn_Crew[crewIndex].transform.GetChild(0).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());

                    tempStringBuilder.Remove(0, tempStringBuilder.Length);
                    tempStringBuilder.Append("Sprites/Common/flag_01.num_g_" + (crewIndex+1));
                    Btn_Crew[crewIndex].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
                    Btn_Crew[crewIndex].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                    Btn_Crew[crewIndex].transform.GetChild(0).GetChild(0).GetComponent<Image>().SetNativeSize();
                    break;

                case 2:
                    tempStringBuilder.Remove(0, tempStringBuilder.Length);
                    tempStringBuilder.Append("Sprites/Common/flag_01.division_").Append("champion");
                    Btn_Crew[crewIndex].transform.GetChild(0).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());

                    tempStringBuilder.Remove(0, tempStringBuilder.Length);
                    tempStringBuilder.Append("Sprites/Common/flag_01.num_g_" + (crewIndex+1));
                    Btn_Crew[crewIndex].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
                    Btn_Crew[crewIndex].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                    Btn_Crew[crewIndex].transform.GetChild(0).GetChild(0).GetComponent<Image>().SetNativeSize();
                    break;
            }
        }
    }
    */
    //캐릭터 리스트 삭제
    public void DeleteCharacterList()
    {
        for (int i = 0; i < CharList.transform.childCount; i++)
        {
            GameObject.Destroy(CharList.transform.GetChild(i).gameObject);
        }
    }
    //캐릭터 리스트 초기화
    public void InitCharacterList()
    {
        //for(int i=0; i<CharList.transform.childCount; i++)
        //{
        //    GameObject.Destroy(CharList.transform.GetChild(i).gameObject);
        //}
        //for(int i=0; i<CharSlotList.transform.childCount; i++)
        //{
        //    GameObject.Destroy(CharSlotList.transform.GetChild(i).gameObject);
        //}
        if (CharList.transform.childCount == 0)
        {
            for (int i = 0; i < Legion.Instance.acHeros.Count; i++)
            {
                GameObject charListElement = Instantiate(PrefCharElement);
                //tempStringBuilder.Remove(0, tempStringBuilder.Length);
                //tempStringBuilder.Append("CharElement_").Append(i.ToString());
                RectTransform rcTr = charListElement.GetComponent<RectTransform>();
                rcTr.SetParent(CharList.GetComponent<RectTransform>());
                rcTr.name = string.Format("CharElement_{0}", i);
                rcTr.localScale = Vector3.one;
                rcTr.localPosition = Vector3.zero;
                charListElement.GetComponent<UI_CharElement>().SetData(Legion.Instance.acHeros[i], _lobbyScene, this);
            }
        }
        else
        {
            // 캐릭터 슬롯이 생성되어 있다면 캐릭터 정보를 새로고침 한다
            if (CharList.transform.childCount < Legion.Instance.acHeros.Count) {
                for (int i = 0; i < Legion.Instance.acHeros.Count; i++) {
                    if (i < CharList.transform.childCount) {
                        if (CharList.transform.GetChild(i).gameObject.activeSelf == false)
                            continue;

                        CharList.transform.GetChild(i).GetComponent<UI_CharElement>().RefreshHeroInfo();
                    } else {
                        GameObject charListElement = Instantiate(PrefCharElement);
                        //tempStringBuilder.Remove(0, tempStringBuilder.Length);
                        //tempStringBuilder.Append("CharElement_").Append(i.ToString());
                        RectTransform rcTr = charListElement.GetComponent<RectTransform>();
                        rcTr.SetParent(CharList.GetComponent<RectTransform>());
                        rcTr.name = string.Format("CharElement_{0}", i);
                        rcTr.localScale = Vector3.one;
                        rcTr.localPosition = Vector3.zero;
                        charListElement.GetComponent<UI_CharElement>().SetData(Legion.Instance.acHeros[i], _lobbyScene, this);
                    }
                }
            } else {
                for (int i = 0; i < CharList.transform.childCount; i++) {
                    if (CharList.transform.GetChild(i).gameObject.activeSelf == false)
                        continue;

                    CharList.transform.GetChild(i).GetComponent<UI_CharElement>().RefreshHeroInfo();
                }
            }
        }

        //int ActiveCharIcon = 0;
        //for(int i=0; i<CharList.transform.childCount; i++)
        //{
        //    if(CharList.transform.GetChild(i).gameObject.activeSelf)
        //		ActiveCharIcon++;
        //}

        if (CharSlotList.transform.childCount == 0)
        {
            RectTransform rtTf = CharSlotList.GetComponent<RectTransform>();
            for (int i = 0; i < LegionInfoMgr.Instance.limitCharSlot; i++)
            {
                GameObject CharSlot = Instantiate(PrefCharSlot);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append("CharSlot_").Append(i.ToString());
                RectTransform rectTr = CharSlot.GetComponent<RectTransform>();
                rectTr.SetParent(rtTf);
                rectTr.name = tempStringBuilder.ToString();
                rectTr.localScale = Vector3.one;
                rectTr.localPosition = Vector3.zero;
            }
        }
        ResetCrewListSize();
        //GridLayoutGroup charSlotListGridLayout = CharSlotList.GetComponent<GridLayoutGroup>();

        //ScrollRect[] tempScrollRect = CharList.transform.parent.parent.GetComponents<ScrollRect>();
        //if (ActiveCharIcon < 11)
        //{
        //    //for(int i=0; i<tempScrollRect.Length; i++)
        //    //    tempScrollRect[i].enabled = false;
        //    CharList.GetComponent<GridLayoutGroup>().padding = new RectOffset(10, 0, 15, 0);
        //    charSlotListGridLayout.padding = new RectOffset(10, 0, 15, 0);
        //}
        //else
        //{
        //    //for(int i=0; i<tempScrollRect.Length; i++)
        //    //    tempScrollRect[i].enabled = true;
        //    CharList.GetComponent<GridLayoutGroup>().padding = new RectOffset(10, 0, 0, 0);
        //    charSlotListGridLayout.padding = new RectOffset(10, 0, 0, 0);
        //}
        //
        //RectTransform charSlotListRtTr = CharSlotList.GetComponent<RectTransform>();
        //
        //CharList.GetComponent<RectTransform>().sizeDelta = new Vector2(charSlotListRtTr.rect.width, 
        //    ((ActiveCharIcon/3)+1)*(charSlotListGridLayout.cellSize.y + charSlotListGridLayout.spacing.y));
        //charSlotListRtTr.sizeDelta = new Vector2(charSlotListRtTr.rect.width, 
        //    ((ActiveCharIcon/3)+1)*(charSlotListGridLayout.cellSize.y + charSlotListGridLayout.spacing.y));
        //크루 튜토리얼 파트3(캐릭터 배치)
        //if(Legion.Instance.cTutorial.au1Step[4] >= 2 && Legion.Instance.cTutorial.au1Step[4] != 200)
        //    TutorialIndicate();
        if (this.gameObject.activeSelf == true)
            StartCoroutine("DelayedCallTutorial");
        SetAutoCharBtnActive();
    }
    IEnumerator DelayedCallTutorial()
    {
        yield return new WaitForEndOfFrame();
        //크루 튜토리얼 파트3(캐릭터 배치)
        if (Legion.Instance.cTutorial.au1Step[4] == 2 && Legion.Instance.cTutorial.au1Step[4] != 200)
            TutorialIndicate();
    }
    //선택 크루 캐릭터 셋팅
    public void SetCrewCharacters()
    {
        //확인하지 않은 캐릭터 체크
        for (int i = 0; i < Legion.MAX_CREW_OF_LEGION; i++)
        {
            if (!Legion.Instance.acCrews[i].IsLock /*&& Legion.Instance.acCrews[i] != cSelectedCrew*/)
            {
                for (int j = 0; j < Crew.MAX_CHAR_IN_CREW; j++)
                {
                    if (Legion.Instance.acCrews[i].acLocation[j] != null)
                    {
                        if (Legion.Instance.acCrews[i].acLocation[j].GetComponent<StatusComponent>().CheckHaveStatPoint(Legion.Instance.acCrews[i].acLocation[j].cLevel.u2Level))
                        {
                            Btn_Crew[i].transform.GetChild(2).gameObject.SetActive(true);
                            //LeanTween.rotate(Btn_Crew[i].transform.GetChild(2).GetComponent<RectTransform>(), -360f, 1.5f).setLoopType(LeanTweenType.easeInElastic).setLoopCount(0);
                            break;
                        }
                        else if (Legion.Instance.acCrews[i].acLocation[j].GetComponent<SkillComponent>().SkillPoint > 0)
                        {
                            Btn_Crew[i].transform.GetChild(2).gameObject.SetActive(true);
                            //LeanTween.rotate(Btn_Crew[i].transform.GetChild(2).GetComponent<RectTransform>(), -360f, 1.5f).setLoopType(LeanTweenType.easeInElastic).setLoopCount(0);
                            break;
                        }
                        else
                        {
                            for (int k = 0; k < ((Hero)Legion.Instance.acCrews[i].acLocation[j]).acEquips.Length; k++)
                            {
                                if (((Hero)Legion.Instance.acCrews[i].acLocation[j]).acEquips[k].GetComponent<StatusComponent>().CheckHaveEquipStatPoint(((Hero)Legion.Instance.acCrews[i].acLocation[j]).acEquips[k].cLevel.u2Level))
                                {
                                    Btn_Crew[i].transform.GetChild(2).gameObject.SetActive(true);
                                    //LeanTween.rotate(Btn_Crew[i].transform.GetChild(2).GetComponent<RectTransform>(), -360f, 1.5f).setLoopType(LeanTweenType.easeInElastic).setLoopCount(0);
                                    break;
                                }
                                else
                                    Btn_Crew[i].transform.GetChild(2).gameObject.SetActive(false);
                            }
                            if (Btn_Crew[i].transform.GetChild(2).gameObject.activeSelf)
                                break;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < CharList.transform.childCount; i++)
        {
            UI_CharElement charElement = CharList.transform.GetChild(i).GetComponent<UI_CharElement>();
            if (charElement.cHero.cObject != null)
            {
                //CharList.transform.GetChild(i).GetComponent<UI_CharElement>().cHero.DestroyModelObject();
            }
            if (charElement.cHero.GetComponent<StatusComponent>().CheckHaveStatPoint(charElement.cHero.cLevel.u2Level))
            {
                charElement.CheckNoticeTrue();
            }
            else if (charElement.cHero.GetComponent<SkillComponent>().SkillPoint > 0)
                charElement.CheckNoticeTrue();
            else
            {
                for (int j = 0; j < charElement.cHero.acEquips.Length; j++)
                {
                    if (charElement.cHero.acEquips[j].GetComponent<StatusComponent>().CheckHaveEquipStatPoint(charElement.cHero.cLevel.u2Level))
                    {
                        charElement.CheckNoticeTrue();
                        break;
                    }
                    else
                        charElement.CheckNoticeFalse();
                }
            }
        }

        UInt32 tempPower = 0;
        for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
        {
            if (cSelectedCrew.acLocation[i] != null)
                tempPower += ((Hero)cSelectedCrew.acLocation[i]).u4Power;

            if (!cSelectedCrew.abLocks[i])
            {
                if (cSelectedCrew.acLocation[i] != null)
                {
                    CharacterSlot[i].gameObject.SetActive(true);
                    BG_CharacterSlot[i].gameObject.SetActive(true);
                    if (this.gameObject.activeSelf == true)
                        StartCoroutine(SetModelInList(((Hero)cSelectedCrew.acLocation[i]), i));
                    Btn_SlotOpen[i].transform.GetChild(0).gameObject.SetActive(false);
                    Btn_SlotOpen[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                    Btn_SlotOpen[i].transform.GetChild(1).gameObject.SetActive(false);
                    Btn_SlotOpen[i].GetComponent<Button>().interactable = true;
                    Btn_SlotOpen[i].GetComponent<ButtonClickAni>().enabled = true;
                    Btn_SlotOpen[i].GetComponent<UI_CharSlot>().SetData((Byte)(cSelectedCrew.u1Index - 1));
                    _lobbyScene.SetCharacterLevelExp(((Hero)cSelectedCrew.acLocation[i]), i);
                }

                else
                {
                    //Btn_SlotOpen[i].transform.GetChild(2).GetComponent<ParticleSystem>().Play();
                    Btn_SlotOpen[i].transform.GetChild(0).gameObject.SetActive(true);
                    Btn_SlotOpen[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    Btn_SlotOpen[i].transform.GetChild(1).gameObject.SetActive(false);
                    Btn_SlotOpen[i].GetComponent<UI_CharSlot>().cHero = null;
                    Btn_SlotOpen[i].GetComponent<Button>().interactable = false;
                    Btn_SlotOpen[i].GetComponent<ButtonClickAni>().enabled = false;
                    CharacterSlot[i].gameObject.SetActive(false);
                    BG_CharacterSlot[i].gameObject.SetActive(false);
                }
            }

            else
            {
                Btn_SlotOpen[i].transform.GetChild(0).gameObject.SetActive(false);
                Btn_SlotOpen[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                Btn_SlotOpen[i].transform.GetChild(1).gameObject.SetActive(true);
                ShowUnlockSlotGoods(i);
                Btn_SlotOpen[i].GetComponent<Button>().interactable = true;
                Btn_SlotOpen[i].GetComponent<ButtonClickAni>().enabled = true;
                Btn_SlotOpen[i].GetComponent<UI_CharSlot>().cHero = null;
                CharacterSlot[i].gameObject.SetActive(false);
                BG_CharacterSlot[i].gameObject.SetActive(false);
            }

            CharacterSlot[i].transform.localPosition = CharacterPostion[i];
            BG_CharacterSlot[i].transform.localPosition = v3CharacterPos[i];
            BG_CharacterSlot[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 110f, 0f));
            //CharacterSlot[i].transform.GetChild(0).localPosition = Vector3.zero;
            //CharacterSlot[i].transform.GetChild(0).localScale = Vector3.one;
        }
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_power")).Append(" ").Append(((UInt32)tempPower).ToString());
        ObjCrewPower.text = tempStringBuilder.ToString();
        //SetCrewPower(tempPower);
    }
    //슬롯 언락 조건 표시
    public void ShowUnlockSlotGoods(int _slotNum)
    {
        switch (cSelectedCrew.u1Index)
        {
            case 1:
                Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().enabled = false;
                switch (_slotNum)
                {
                    case 1:
                        break;

                    case 2:
                        tempStringBuilder.Remove(0, tempStringBuilder.Length);
                        tempStringBuilder.Append(TextManager.Instance.GetText("popup_desc_slot_term_unlock_act")/*.Substring(22)*/);
                        if (Convert.ToInt16(LegionInfoMgr.Instance.acCrewOpenGoods[0][2].u2ID.ToString().Substring(3, 1)) != 0)
                        {
                            tempStringBuilder.Append(Convert.ToInt16(LegionInfoMgr.Instance.acCrewOpenGoods[0][2].u2ID.ToString().Substring(2, 1)) + 1);
                            tempStringBuilder.Append("-");
                            tempStringBuilder.Append(LegionInfoMgr.Instance.acCrewOpenGoods[0][2].u2ID.ToString().Substring(3, 1));
                        }
                        else
                        {
                            tempStringBuilder.Append(Convert.ToInt16(LegionInfoMgr.Instance.acCrewOpenGoods[0][2].u2ID.ToString().Substring(2, 1)));
                            tempStringBuilder.Append("-10");
                        }

                        tempStringBuilder.Append(TextManager.Instance.GetText("popup_desc_slot_unlock_clear"));
                        Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = tempStringBuilder.ToString();
                        //Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        //Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(0).transform.localPosition = Vector3.zero;
                        break;
                }
                break;

            case 2: case 3: case 4: case 5:
                Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().enabled = true;
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append("Sprites/Common/common_02_renew.icon_");
                //if(Legion.Instance.GetConsumeString((int)LegionInfoMgr.Instance.acCrewOpenGoods[cSelectedCrew.u1Index-1][_slotNum].u1Type).ToString() == GoodsType.GOLD.ToString())
                if ((GoodsType)LegionInfoMgr.Instance.acCrewOpenGoods[cSelectedCrew.u1Index - 1][_slotNum].u1Type == GoodsType.GOLD)
                    tempStringBuilder.Append("Gold");
                //else if(Legion.Instance.GetConsumeString((int)LegionInfoMgr.Instance.acCrewOpenGoods[cSelectedCrew.u1Index-1][_slotNum].u1Type).ToString() == GoodsType.CASH.ToString())
                else if ((GoodsType)LegionInfoMgr.Instance.acCrewOpenGoods[cSelectedCrew.u1Index - 1][_slotNum].u1Type == GoodsType.CASH)
                    tempStringBuilder.Append("Cash");

                Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
                Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().SetNativeSize();
                Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "\t" + LegionInfoMgr.Instance.acCrewOpenGoods[cSelectedCrew.u1Index - 1][_slotNum].u4Count.ToString();
                /*
                if(LegionInfoMgr.Instance.acCrewOpenGoods[cSelectedCrew.u1Index-1][_slotNum].u4Count.ToString().Length == 6)
                {
                    Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(0).localPosition = new Vector3(78, 0, 0);
                    Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(1).localPosition = new Vector3(-47, 0, 0);
                }
                else if(LegionInfoMgr.Instance.acCrewOpenGoods[cSelectedCrew.u1Index-1][_slotNum].u4Count.ToString().Length == 5)
                {
                    Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(0).localPosition = new Vector3(84, 0, 0);
                    Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(1).localPosition = new Vector3(-41, 0, 0);
                }
                else if(LegionInfoMgr.Instance.acCrewOpenGoods[cSelectedCrew.u1Index-1][_slotNum].u4Count.ToString().Length == 4)
                {
                    Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(0).localPosition = new Vector3(90, 0, 0);
                    Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(1).localPosition = new Vector3(-35, 0, 0);
                }
                else if(LegionInfoMgr.Instance.acCrewOpenGoods[cSelectedCrew.u1Index-1][_slotNum].u4Count.ToString().Length == 3)
                {
                    Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(0).localPosition = new Vector3(96, 0, 0);
                    Btn_SlotOpen[_slotNum].transform.GetChild(1).GetChild(0).GetChild(1).localPosition = new Vector3(-29, 0, 0);
                }
                */
                break;
        }
    }
    //크루 전환시 캐릭터 정리
    public void ResetInCrewCharacter()
    {
        for (int i = 0; i < CharList.transform.childCount; i++)
        {
            if (CharList.transform.GetChild(i).GetComponent<UI_CharElement>().cHero.cObject != null)
            {
                CharList.transform.GetChild(i).GetComponent<UI_CharElement>().cHero.DestroyModelObject();
            }
        }
    }
    //캐릭터 모델링 출력
    IEnumerator SetModelInList(Hero inHero, int childCnt)
    {
        //DebugMgr.Log("In InitModel");
        if (inHero.cObject != null)
        {
            inHero.cObject.transform.SetParent(BG_CharacterSlot[childCnt].transform);
            inHero.cObject.transform.localRotation = Quaternion.Euler(v3CharacterRotation);
            inHero.cObject.transform.localScale = Vector3.one;
            inHero.cObject.transform.localPosition = Vector3.zero;
            inHero.cObject.GetComponent<HeroObject>().SetLayer(LayerMask.NameToLayer("BGMainMap"));
            inHero.cObject.GetComponent<HeroObject>().SetAnimations_UI();
            inHero.cObject.SetActive(true);
            yield break;
            //inHero.DestroyModelObject();
        }
        while (inHero.cObject != null)
            yield return new WaitForSeconds(0.0f);

        inHero.InitModelObject();
        inHero.cObject.transform.SetParent(BG_CharacterSlot[childCnt].transform);
        inHero.cObject.transform.localRotation = Quaternion.Euler(v3CharacterRotation);
        inHero.cObject.transform.localScale = Vector3.one;
        inHero.cObject.transform.localPosition = Vector3.zero;
        inHero.cObject.GetComponent<HeroObject>().SetLayer(LayerMask.NameToLayer("BGMainMap"));
        inHero.cObject.GetComponent<HeroObject>().SetAnimations_UI();
    }
    //캐릭터 목록중 크루에 포함되어있지 않은 캐릭터 표시
    public void CheckHeroinList()
    {
        for (int i = 0; i < CharList.transform.childCount; i++)
        {
            if (CharList.transform.GetChild(i).GetComponent<UI_CharElement>().cHero.u1AssignedCrew == 0)
            {
                CharList.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void HideCharacterSlot()
    {
        for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
        {
            Btn_SlotOpen[i].gameObject.SetActive(false);
        }
    }

    public void ShowCharacterSlot()
    {
        for (int i = 0; i < Crew.MAX_CHAR_IN_CREW; i++)
        {
            Btn_SlotOpen[i].gameObject.SetActive(true);
        }
    }
    //튜토리얼 인디케이터
    GameObject tutorialCharCursor;
    public void TutorialIndicate()
    {
        GameObject tempIndicate = Instantiate(Pref_Indicate);
        tempIndicate.transform.SetParent(CharList.transform.GetChild(1).GetChild(0));
        tempIndicate.transform.localPosition = Vector3.zero;
        tempIndicate.transform.localScale = Vector3.one;
        tempIndicate.GetComponent<Image>().type = Image.Type.Simple;
        tempIndicate.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_01_renew.btn_indic_round");
        tempIndicate.GetComponent<Image>().SetNativeSize();
        tempIndicate.transform.SetParent(CharList.transform.GetChild(1));
        tempIndicate.transform.SetAsLastSibling();

        GameObject tempIndicate2 = Instantiate(Pref_Indicate2);
        tempIndicate2.transform.SetParent(Btn_SlotOpen[1].transform);
        tempIndicate2.transform.localPosition = Vector3.zero;
        tempIndicate2.transform.localScale = Vector3.one;
        tempIndicate2.GetComponent<Image>().type = Image.Type.Sliced;
        tempIndicate2.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 400);
        tempIndicate2.transform.SetAsLastSibling();

        tutorialCharCursor = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/TutorialCursorCharacter.prefab", typeof(GameObject)) as GameObject);
        tutorialCharCursor.transform.SetParent(this.transform);
        tutorialCharCursor.GetComponent<CanvasGroup>().alpha = 0;
        tutorialCharCursor.transform.localScale = Vector3.one;

        Vector2 tempPos = new Vector2();
        for (int i = 0; i < CharList.transform.childCount; i++)
        {
            if (CharList.transform.GetChild(i).gameObject.activeSelf)
            {
                //tempPos = new Vector2(CharList.transform.GetChild(i).position.x-75, CharList.transform.GetChild(i).position.y-75);
                tempPos = CharList.transform.GetChild(i).position;
                break;
            }
        }

        tutorialCharCursor.GetComponent<UI_CharTutorialCursorElement>().SetData(Legion.Instance.acHeros[1], tempPos, Btn_SlotOpen[1].transform.localPosition);
        tempIndicate.transform.SetAsLastSibling();
    }

    public void ResetCrewListSize()
    {
        int ActiveCharIcon = 0;
        for (int i = 0; i < CharList.transform.childCount; i++)
        {
            if (CharList.transform.GetChild(i).gameObject.activeSelf)
                ActiveCharIcon++;
        }

        GridLayoutGroup charSlotListGridLayout = CharSlotList.GetComponent<GridLayoutGroup>();
        if (ActiveCharIcon < 11)
        {
            CharList.GetComponent<GridLayoutGroup>().padding = new RectOffset(10, 0, 15, 0);
            charSlotListGridLayout.padding = new RectOffset(10, 0, 15, 0);
        }
        else
        {
            CharList.GetComponent<GridLayoutGroup>().padding = new RectOffset(10, 0, 0, 0);
            charSlotListGridLayout.padding = new RectOffset(10, 0, 0, 0);
        }

        RectTransform charSlotListRtTf = CharSlotList.GetComponent<RectTransform>();
        CharList.GetComponent<RectTransform>().sizeDelta = new Vector2(charSlotListRtTf.rect.width,
            ((ActiveCharIcon / 3) + 1) * (charSlotListGridLayout.cellSize.y + charSlotListGridLayout.spacing.y));
        charSlotListRtTf.sizeDelta = new Vector2(charSlotListRtTf.rect.width,
            ((ActiveCharIcon / 3) + 1) * (charSlotListGridLayout.cellSize.y + charSlotListGridLayout.spacing.y));
    }

    #region ChangeCrewSelect implementation
    //캐릭터 리스트에서 슬롯으로 배치
    public void ChangedListToSlot(Hero inHero, Byte slotNumInCrew)
    {
        if (cSelectedCrew.abLocks[slotNumInCrew])
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("crew_slot_locked"), Server.ServerMgr.Instance.CallClear);
            return;
        }

        if (cSelectedCrew.DispatchStage != null)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_retire_error_dispatch_ing"), null);
            return;
        }

        //        if(cSelectedCrew.DispatchLeague != 0)
        //        {
        //            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_retire_error_dispatch_ing"), null);
        //			return;
        //        }

        Character outChar;

        //cSelectedCrew.Assign(inHero, slotNumInCrew, out outChar);
        if (inHero.u1AssignedCrew == cSelectedCrew.u1Index)
        {
            cSelectedCrew.Resign(inHero);
            cSelectedCrew.Assign(inHero, slotNumInCrew, out outChar);
            if (outChar != null)
            {
                cSelectedCrew.Resign(outChar);
                if (((Hero)outChar).cObject != null)
                    ((Hero)outChar).DestroyModelObject();
            }
        }

        else
        {
            //Legion.Instance.acCrews[inHero.u1AssignedCrew-1].Resign(inHero);
            cSelectedCrew.Assign(inHero, slotNumInCrew, out outChar);
            if (outChar != null)
            {
                cSelectedCrew.Resign(outChar);
                if (((Hero)outChar).cObject != null)
                    ((Hero)outChar).DestroyModelObject();
            }
        }

        for (int i = 0; i < CharList.transform.childCount; i++)
        {
            GameObject childObject = CharList.transform.GetChild(i).gameObject;
            if (childObject.GetComponent<UI_CharElement>().cHero == inHero)
                //CharList.transform.GetChild(i).GetComponent<UI_CharElement>().cHero.u1AssignedCrew = cSelectedCrew.u1Index;
                childObject.gameObject.SetActive(false);
            else if (childObject.GetComponent<UI_CharElement>().cHero == outChar)
                childObject.gameObject.SetActive(true);
        }

        SetCrewCharacters();

        Btn_Back.interactable = true;
        if (Legion.Instance.cTutorial.CheckTutorial(MENU.CREW))
        {
            Destroy(Btn_SlotOpen[1].transform.GetChild(Btn_SlotOpen[1].transform.childCount - 1).gameObject);
            Destroy(tutorialCharCursor);
        }
        ResetCrewListSize();
        Btn_SlotOpen[0].SetActive(true);
        SetAutoCharBtnActive();
    }
    //슬롯에서 리스트로 배치
    public void ChangedSlotToList(Hero outHero)
    {
        if (cSelectedCrew.u1Count == 1)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_char_set"), emptyMethod2);
            //if(!outHero.cObject.active)
            //    outHero.cObject.SetActive(true);
            outHero.SetActiveWithBeforeParent();
            return;
        }
        if (cSelectedCrew.DispatchStage != null)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_retire_error_dispatch_ing"), null);
            //if(!outHero.cObject.active)
            //    outHero.cObject.SetActive(true);
            outHero.SetActiveWithBeforeParent();
            return;
        }

        //        if(cSelectedCrew.DispatchLeague != 0)
        //        {
        //            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_retire_error_dispatch_ing"), null);
        //			return;
        //        }
        cSelectedCrew.Resign(outHero);
        outHero.DestroyModelObject();
        //InitCharacterList();
        //int ActiveCharIcon = 0;
        for (int i = 0; i < CharList.transform.childCount; i++)
        {
            GameObject childObject = CharList.transform.GetChild(i).gameObject;
            if (childObject.GetComponent<UI_CharElement>().cHero == outHero)
                //CharList.transform.GetChild(i).GetComponent<UI_CharElement>().cHero.u1AssignedCrew = 0;
                childObject.SetActive(true);
        }
        //int ActiveCharIcon = 0;
        //for(int i=0; i<CharList.transform.childCount; i++)
        //{
        //    if(CharList.transform.GetChild(i).gameObject.active)
        //        ActiveCharIcon++;
        //}
        //ScrollRect[] tempScrollRect = CharList.transform.parent.parent.GetComponents<ScrollRect>();
        //if(ActiveCharIcon < 13)
        //{
        //    for(int i=0; i<tempScrollRect.Length; i++)
        //        tempScrollRect[i].enabled = false;
        //    CharList.GetComponent<GridLayoutGroup>().padding = new RectOffset(10, 0, 15, 0);
        //    CharSlotList.GetComponent<GridLayoutGroup>().padding = new RectOffset(10, 0, 15, 0);
        //}
        //else
        //{
        //    for(int i=0; i<tempScrollRect.Length; i++)
        //        tempScrollRect[i].enabled = true;
        //    CharList.GetComponent<GridLayoutGroup>().padding = new RectOffset(10, 0, 0, 0);
        //    CharSlotList.GetComponent<GridLayoutGroup>().padding = new RectOffset(10, 0, 0, 0);
        //}
        ResetCrewListSize();
        SetAutoCharBtnActive();
        SetCrewCharacters();
    }
    //슬롯에서 슬롯으로 배치
    public void ChangedSlotToSlot(Hero swapHero, Hero inHero, Byte slotNumInCrew, Byte swapSlotNumInCrew)
    {
        if (cSelectedCrew.abLocks[slotNumInCrew])
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("crew_slot_locked"), Server.ServerMgr.Instance.CallClear);
            return;
        }
        if (cSelectedCrew.DispatchStage != null)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_retire_error_dispatch_ing"), null);
            return;
        }

        //        if(cSelectedCrew.DispatchLeague != 0)
        //        {
        //            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_retire_error_dispatch_ing"), null);
        //			return;
        //        }
        Character outChar;

        if (swapHero == null)
        {
            cSelectedCrew.Resign(inHero);
            //inHero.DestroyModelObject();
            if (inHero.u1AssignedCrew != 0)
                Legion.Instance.acCrews[(inHero.u1AssignedCrew - 1)].Resign(inHero);
            cSelectedCrew.Assign(inHero, (int)(slotNumInCrew), out outChar);
        }

        else
        {
            DebugMgr.LogError("change slot to slot");
            Byte idx = swapSlotNumInCrew;

            cSelectedCrew.Assign(inHero, (int)(slotNumInCrew), out outChar);
            //inHero.DestroyModelObject();
            Character temp;

            cSelectedCrew.Assign(outChar, (int)(idx), out temp);
            //((Hero)outChar).DestroyModelObject();
        }
        _lobbyScene.prevScreen = 3;
        SetCrewCharacters();
    }
    #endregion

    // 자동 배치 버튼 활성화 여부
    private void SetAutoCharBtnActive()
    {
        if (Legion.Instance.cTutorial.au1Step[4] == 200)
        {
            int nActiveCharIcon = 0;
            for (int i = 0; i < CharList.transform.childCount; i++)
            {
                if (CharList.transform.GetChild(i).gameObject.activeSelf)
                    nActiveCharIcon++;
            }

            if (nActiveCharIcon > 0)
                Btn_SetAutoChar.interactable = true;
            else
                Btn_SetAutoChar.interactable = false;
        }
        else
        {
            Btn_SetAutoChar.interactable = false;
        }
    }

    /*
    public void SetCrewPower(UInt32 changeCrewPower)
    {
        if(u4CrtCrewPower <= 0)
        {
            u4CrtCrewPower = changeCrewPower;
            ObjCrewPower.text = string.Format("{0} {1}", TextManager.Instance.GetText("mark_power"), u4CrtCrewPower);
        }
        else if (changeCrewPower != u4CrtCrewPower)
        {
            u4TargetCrewPower = changeCrewPower;
            // 현재 코루틴이 작동하는지 체크
            {
                StartCoroutine("CrewPower");
            }
        }
    }
    private IEnumerator CrewPower()
    {
        DebugMgr.LogError(string.Format("1. {0}", u4CrtCrewPower));
        DebugMgr.LogError(string.Format("2. {0}", u4TargetCrewPower));
        // 변경할 크루 전투력 - 현재 전투력
        long crewPowerGap = Math.Abs((long)u4CrtCrewPower - (long)u4TargetCrewPower);
        DebugMgr.LogError(string.Format("3. {0}", crewPowerGap));
        
        int sss = Math.Abs(crewPowerGap).ToString().Length - 1;
        DebugMgr.LogError(string.Format("4. {0}", sss));
        if (sss > 1)
            sss = (int)Math.Pow(10, sss - 1);
        else
            sss = 1;

        DebugMgr.LogError(string.Format("5. {0}", sss));
        while (true)
        {
            if (u4CrtCrewPower != u4TargetCrewPower)
            {
                if (u4TargetCrewPower > u4CrtCrewPower)
                {
                    u4CrtCrewPower += (UInt16)sss;
                }
                else
                {
                    u4CrtCrewPower -= (UInt16)sss;
                }

                crewPowerGap -= sss;
                if (crewPowerGap < sss && sss != 1)
                {
                    sss = (int)(sss * 0.1f);
                    DebugMgr.LogError(string.Format("6. {0}", sss));
                }

                ObjCrewPower.text = string.Format("{0} {1}", TextManager.Instance.GetText("mark_power"), u4CrtCrewPower);
            }
            else
            {
                u4CrtCrewPower = u4TargetCrewPower;
                ObjCrewPower.text = string.Format("{0} {1}", TextManager.Instance.GetText("mark_power"), u4CrtCrewPower);
                DebugMgr.LogError("7. End");
                yield break;
            }

            yield return null;
        }
    }
    */
}
