using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class OdinMainPopup : MonoBehaviour
{
    public Image imgMainPopup;
    public Text _txtOdinGradeTitle;

    public GameObject objOdinMission;               // 오딘 임무 셋팅
    public GameObject objOdinComplete;              // 오딘 완료 셋팅

    public GameObject objGradeInfoBottomBar;        // 오딘 등급정보 바
    public GameObject objCompleteBottomBar;         // 오딘 컴플리트 바

    // 임무 관련 변수들
    public GameObject _objNormalMissionGrop;
    public OdinMissionSlot[] _acNormalMissionSlot;
    public OdinMissionSlot _cMidMissionSlot;

    public Text _txtOdinExp;
    public Image _imgOdinExpProgress;

    public Button _btnGradeUp;                  // 승급 버튼

    // 등급 업 보상
    public Text _txtGradeOdinReward;
    public RectTransform _rtTrRewardSlotsParent;            // 아이템 슬롯의 부모
    public UI_ItemListElement_Common[] cGradeUpItemSlot;
    // 다음 등급 보상
    public Text _txtNextOdinReward;
    public GameObject _objRewardGetText;
    private float fSlotSpacing;                     // 슬롯 간격
    private float fNextOdinRewardTxtPosX;

    public RectTransform trSubParent;           // 팝업 부모
    public Text txtGetOdinPointInfo;            // 오딘 포인트 얻는 방법

    private OdinMissionSlot cRequestMissionSlot;   // 서버 요청 보낸 임무;

    public OdinMissionReward cMissionRewardEffect;  // 임무 보상 연출
    public OdinGradeupPopup cGradeupPopup;          // 오딘 등급 변경 팝업

    public void Awake()
    {
        // 기본 위치를 저장한다
        fNextOdinRewardTxtPosX = _txtNextOdinReward.rectTransform.anchoredPosition3D.x;
        // 슬롯의 간격을의 반지름? 저장
        fSlotSpacing = _rtTrRewardSlotsParent.GetComponent<HorizontalLayoutGroup>().spacing * 0.5f;

        cRequestMissionSlot = null;
        SetOdinPointGetDesc();
    }

    public void OnEnable()
    {
        cMissionRewardEffect.gameObject.SetActive(false);
        PopupManager.Instance.AddPopup(this.gameObject, OnClickClose);
        SetOdinPopup();
    }
    // 팝업 셋팅
    public void SetOdinPopup()
    {
        _btnGradeUp.transform.localScale = Vector3.one;

        LeanTween.cancel(_btnGradeUp.gameObject);

        if (CheckOdinComplete())
        {
            SetCompletePopup();
        }
        else
        {
            SetUserOdinInfo();
            SetOdinMissionSlots();
        }
        this.gameObject.SetActive(true);
    }
    // 오딘 완료 체크
    public bool CheckOdinComplete()
    {
        // 만렙이 아니라면
        if (Legion.Instance.u1VIPLevel < LegionInfoMgr.Instance.dicVipData.Count - 1)
        {
            return false;
        }

        // 남은 미션이 존재 한다면
        if(Legion.Instance.cQuest.userOdinMissionList.Count > 0)
        {
            return false;
        }
        
        return true;
    }
    // 오딘 컴플리트 팝업 셋팅
    private void SetCompletePopup()
    {
        objOdinMission.SetActive(false);
        objOdinComplete.SetActive(true);

        imgMainPopup.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/odin_final.odin_final");
        imgMainPopup.SetNativeSize();
    } 
    // 유저 오딘 정보 셋팅
    private void SetUserOdinInfo()
    {
        objOdinMission.SetActive(true);
        objOdinComplete.SetActive(false);

        imgMainPopup.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/RTO_Popup.RTO_Popup");
        imgMainPopup.SetNativeSize();

        _txtOdinGradeTitle.text = string.Format(TextManager.Instance.GetText("odin_mission_name"), TextManager.Instance.GetText(string.Format("odin_name_{0}", Legion.Instance.u1VIPLevel)));

        if (Legion.Instance.u1VIPLevel < (LegionInfoMgr.Instance.dicVipData.Count - 1))
        {
            objGradeInfoBottomBar.SetActive(true);
            objCompleteBottomBar.SetActive(false);
            // 다음 레벨의 정보 얻기
            VipInfo nextVipInfo = LegionInfoMgr.Instance.GetVipInfo((Byte)(Legion.Instance.u1VIPLevel + 1));
            string nextOdinGradeName = TextManager.Instance.GetText(string.Format("odin_name_{0}", nextVipInfo.u1Level));

            // 현재 경험치의 퍼센트 계산
            float percent = ((float)Legion.Instance.u4VIPPoint / (float)nextVipInfo.cUnlockGoods.u4Count);
            _txtOdinExp.text = string.Format(TextManager.Instance.GetText("odin_achieve_check"),
                nextOdinGradeName,                  // 다음 등급 이름
                (int)(percent * 100),               // 퍼센트[소수점 버림]
                Legion.Instance.u4VIPPoint,         // 현재 포인트
                nextVipInfo.cUnlockGoods.u4Count);  // 다음 등급업까지 필요 경험치

            // 경험치 바 이미지 셋팅
            if(percent >= 1)
                _imgOdinExpProgress.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_02.ox_progress");
            else
                _imgOdinExpProgress.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_02.progress_fill");

            _imgOdinExpProgress.fillAmount = percent;

            // 승급 보상 슬롯을 셋팅한다
            int slotIdx = 0;    // 셋팅된 슬롯의 갯수
            for (int i = 0; i < nextVipInfo.acOnceReward.Length; ++i)
            {
                // 아이템 타입이 0이라면 다음 보상을 넘긴다
                if (nextVipInfo.acOnceReward[i].u1Type == 0)
                {
                    continue;
                }
                // 아이템 셋팅 및 활성화
                cGradeUpItemSlot[slotIdx].SetData(nextVipInfo.acOnceReward[i]);
                cGradeUpItemSlot[slotIdx].gameObject.SetActive(true);
                slotIdx++;
            }
            // 셋팅되지 않은 슬롯은 승급 보상바의 정보를 세팅한 후 비활성화 한다

            // 승급 보상 바 셋팅
            int checkingLevel = Legion.Instance.RecursiveCheckOdinLevel(Legion.Instance.u1VIPLevel, Legion.Instance.u4VIPPoint);
            if (checkingLevel > Legion.Instance.u1VIPLevel)
            {
                _objRewardGetText.gameObject.SetActive(true);
                _txtGradeOdinReward.gameObject.SetActive(false);
                _txtNextOdinReward.gameObject.SetActive(true);

                _btnGradeUp.interactable = true;
                _txtNextOdinReward.text = string.Format(TextManager.Instance.GetText("odin_upgrade_finish"), nextOdinGradeName);
                
                Vector3 pos = _txtNextOdinReward.rectTransform.anchoredPosition3D;
                pos.x = fNextOdinRewardTxtPosX;
                // 셋팅한 슬롯 개수가 전체 슬롯 개수보다 작으면
                if (slotIdx < cGradeUpItemSlot.Length)
                {
                    pos.x += fSlotSpacing * (cGradeUpItemSlot.Length - slotIdx);
                }
                _txtNextOdinReward.rectTransform.anchoredPosition3D = pos;
                _rtTrRewardSlotsParent.SetParent(_txtNextOdinReward.rectTransform);

                GradeUpBtnEffect();
            }
            else
            {
                _objRewardGetText.gameObject.SetActive(false);
                _txtGradeOdinReward.gameObject.SetActive(true);
                _txtNextOdinReward.gameObject.SetActive(false);

                _btnGradeUp.interactable = false;
                _txtGradeOdinReward.text = string.Format(TextManager.Instance.GetText("odin_advance_reward"), nextOdinGradeName);
                _rtTrRewardSlotsParent.SetParent(_txtGradeOdinReward.rectTransform);
            }
            _rtTrRewardSlotsParent.anchoredPosition3D = Vector3.zero;

            // 셋팅 안된 아이템 슬롯 비활성화
            for (; slotIdx < cGradeUpItemSlot.Length; ++slotIdx)
            {
                cGradeUpItemSlot[slotIdx].gameObject.SetActive(false);
            }
        }
        else
        {
            objGradeInfoBottomBar.SetActive(false);
            objCompleteBottomBar.SetActive(true);
        }
    }
    // 보상 아이템 슬롯 셋팅
    private void SetRewardItemSlot(VipInfo nextVipInfo)
    {


    }
    // 임무 슬롯 셋팅
    private void SetOdinMissionSlots()
    {
        bool isMissionSet = false;
        int count = Legion.Instance.cQuest.userOdinMissionList.Count;
        if (count > 0)
        {
            for (int i = 0; i < _acNormalMissionSlot.Length; ++i)
            {
                if (i < count)
                {
                    UserOdinMission userOidnMission = Legion.Instance.cQuest.userOdinMissionList[i];
                    OdinMissionInfo odinMissionInfo = userOidnMission.GetInfo();
                    // 임무 오픈 조건이 되지 않았거나 미션 아이디가 0이라면
                    if(!Legion.Instance.CheckEnoughGoods(odinMissionInfo.cMissionOpenInfo) ||
                        odinMissionInfo.u2ID == 0)
                        continue;
                    
                    isMissionSet = true;
                    // 미션 그릅 타입이 1.일반 이라면
                    if (odinMissionInfo.u1RollingType == 1)
                    {
                        _acNormalMissionSlot[i].gameObject.SetActive(true);
                        _acNormalMissionSlot[i].SetMissionSlot(userOidnMission);

                        _objNormalMissionGrop.SetActive(true);
                        _cMidMissionSlot.gameObject.SetActive(false);
                    }
                    else
                    {
                        _cMidMissionSlot.SetMissionSlot(userOidnMission);

                        _objNormalMissionGrop.SetActive(false);
                        _cMidMissionSlot.gameObject.SetActive(true);
                        // 중간 미션은 1개만 존재하도록 기획되어 다음 미션을 확인하지않고 바로 반복문 종료
                        break;
                    }
                }
                else
                {
                    _acNormalMissionSlot[i].gameObject.SetActive(false);
                    break;
                }
            }
        }

        // 퀘스트가 없을때 셋팅
        if (!isMissionSet)
        {
            _objNormalMissionGrop.SetActive(false);
            _cMidMissionSlot.gameObject.SetActive(false);
        }
    }
    // 오딘 포인트 얻는 방법
    private void SetOdinPointGetDesc()
    {
        if(txtGetOdinPointInfo != null)
        {
            StringBuilder sb = new StringBuilder();
            // 스테이지 포인트 셋팅
            ChapterInfo chapterInfo = null;
            if(StageInfoMgr.Instance.dicChapterData.TryGetValue((UInt16)(Server.ConstDef.BaseChapterID + 1), out chapterInfo))
            {
                sb.Append(string.Format(TextManager.Instance.GetText("odin_point_get_stage"), chapterInfo.cPlayPayBack.u4Count)).Append("\n");
            }
            // 강림 포인트 셋팅
            EventDungeonStageInfo eventStageInfo = EventInfoMgr.Instance.GetEventDungeonStageInfo(59501);
            if (eventStageInfo != null)
            {
                for(int i = 0; i < eventStageInfo.acClearPoint.Length; ++i)
                {
                    if (eventStageInfo.acClearPoint[i].u1Type == 0)
                        continue;

                    sb.Append(string.Format(TextManager.Instance.GetText(string.Format("odin_point_get_advento_{0}", i)), eventStageInfo.acClearPoint[i].u4Count)).Append("\n");
                }
            }
            // 보스 러쉬 포인트 셋팅
            eventStageInfo = EventInfoMgr.Instance.GetEventDungeonStageInfo(59551);
            if(eventStageInfo != null)
            {
                sb.Append(string.Format(TextManager.Instance.GetText("odin_point_get_boss"), eventStageInfo.acClearPoint[0].u4Count)).Append("\n");
            }
            // 리그 포인트 셋팅
            LeagueInfo leagueInfo = null;
            if (LeagueInfoMgr.Instance.dicLeagueData.TryGetValue(4501, out leagueInfo))
            {
                for (int i = 0; i < leagueInfo.arrResultOidnPoint.Length; ++i)
                {
                    if (leagueInfo.arrResultOidnPoint[i].u1Type == 0)
                        continue;

                    sb.Append(string.Format(TextManager.Instance.GetText(string.Format("odin_point_get_league_{0}", i)), leagueInfo.arrResultOidnPoint[i].u4Count)).Append("\n");
                }
            }
            // 길드전 승리
            if(GuildInfoMgr.Instance.cGuildInfo.gWinOdinGoods.u1Type != 0)
            {
                sb.Append(string.Format(TextManager.Instance.GetText("odin_point_get_guild_win"), GuildInfoMgr.Instance.cGuildInfo.gWinOdinGoods.u4Count)).Append("\n");
            }
            // 길드전 패배
            if (GuildInfoMgr.Instance.cGuildInfo.gDefeatOdinGoods.u1Type != 0)
            {
                sb.Append(string.Format(TextManager.Instance.GetText("odin_point_get_guild_lose"), GuildInfoMgr.Instance.cGuildInfo.gDefeatOdinGoods.u4Count));
            }
            txtGetOdinPointInfo.text = sb.ToString();
        }
    }
    // 등급업 버튼 이펙트
    private void GradeUpBtnEffect()
    {
        RectTransform gradeUpBtn = _btnGradeUp.GetComponent<RectTransform>();
        gradeUpBtn.localScale = Vector3.one;

        LeanTween.scale(gradeUpBtn, new Vector3(1.02f, 1.02f, 1), 0.25f).setLoopPingPong();
    }

    // ================================== 클릭 이벤트 함수 =====================================================//
    // 단계별 혜택창 오픈
    public void OpenVIPInfoPopUp()
    {
        UI_VIPInfoPopUp obj = ((GameObject)Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/VIPInfoPopUp.prefab", typeof(GameObject)))).GetComponent<UI_VIPInfoPopUp>();

        RectTransform rectTr = obj.GetComponent<RectTransform>();
        rectTr.SetParent(trSubParent);
        rectTr.anchoredPosition3D = Vector3.zero;
        rectTr.localScale = Vector3.one;
        rectTr.sizeDelta = Vector2.zero;

        rectTr.SetAsLastSibling();

        PopupManager.Instance.AddPopup(obj.gameObject, obj.onClickClose);
    }
    // 팝업 닫기
    public void OnClickClose()
    {
        Legion.Instance.SubLoginPopupStep(Legion.LoginPopupStep.ODIN_PAGE);
        PopupManager.Instance.RemovePopup(this.gameObject);
        this.gameObject.SetActive(false);
    }
    // 오딘 등급업
    public void OnClickGradeUp()
    {
        int checkingLevel = Legion.Instance.RecursiveCheckOdinLevel(Legion.Instance.u1VIPLevel, Legion.Instance.u4VIPPoint);
        if (checkingLevel > Legion.Instance.u1VIPLevel)
        {
            Server.ServerMgr.Instance.RequestMissionReward(2, 0, (Byte)checkingLevel, ReceiveOdinGradeReward);
        }
    }
    // 임무 변경 확인
    public void MissionChangeOk(object[] param)
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        // 패킷 보내기전 응답 왓을때 임무를 담을 큐를 비운다
        Legion.Instance.cQuest.receiveOdinMissionList.Clear();
        Server.ServerMgr.Instance.RequestMissionPushback(cRequestMissionSlot.SlotMissionInfo.GetInfo().u2ID, ReceiveMissionChange);
    }
    // 임무 슬롯 클릭 이벤트 (슬롯 인덱스) #중간 임무의 슬롯 인덱스는 노말 슬롯의 갯수 보다 무조건 커야함
    public void OnClickMissionSlot(int slotIdx)
    {
        cRequestMissionSlot = null;
        if (slotIdx < _acNormalMissionSlot.Length)
        {
            cRequestMissionSlot = _acNormalMissionSlot[slotIdx];
        }
        else
        {
            cRequestMissionSlot = _cMidMissionSlot;
        }

        if (cRequestMissionSlot != null)
        {
            // 현재 선택된 임무가 진행 가능한지 여부 확인
            if (cRequestMissionSlot.SlotMissionInfo.IsPossible())
            {
                // 임무가 완료 되었는지 확인
                if (cRequestMissionSlot.SlotMissionInfo.IsClear())
                {
                    PopupManager.Instance.ShowLoadingPopup(1);
                    // 패킷 보내기전 응답 왓을때 임무를 담을 큐를 비운다
                    Legion.Instance.cQuest.receiveOdinMissionList.Clear();
                    //보상 요청 패킷
                    Server.ServerMgr.Instance.RequestMissionReward(1, cRequestMissionSlot.SlotMissionInfo.GetInfo().u2ID, 0, ReceiveMissionReward);
                }
                else
                {
                    // 1번 임무만 
                    if (cRequestMissionSlot.SlotMissionInfo.GetInfo().u1RollingType == 1)
                    {
                        // 임무 변경 요청 팝업
                        PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("odin_popup_mission_change"), MissionChangeOk, null);
                    }
                    else
                    {
                        // 중간 임무 미완료 안내 팝업
                        PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("odin_popup_mission_doing"), null);
                    }
                }
            }
            else
            {
                // 임무 변경 불가 안내 팝업
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), 
                    string.Format(TextManager.Instance.GetText("odin_popup_mission_limit"), 
                    TextManager.Instance.GetText(string.Format("odin_name_{0}", cRequestMissionSlot.SlotMissionInfo.GetInfo().cMissionOpenInfo.u4Count))), null);
            }
        }
    }
    // ================================== 클릭 이벤트 함수 end =================================================//

    // ================================== 서버 응답후 콜백 함수 ================================================//
    // 임무 완료 보상 서버 응답 받음
    public void ReceiveMissionReward(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetError(Server.MSGs.MISSION_REWARD, err), null);
        }
        else
        {
            // VIP 포인트 충전 [연출이 있을거라 예상함]
            Legion.Instance.AddVIPPoint(cRequestMissionSlot.SlotMissionInfo.GetFistReward().u4Count);
            if(cMissionRewardEffect != null)
            {
                cMissionRewardEffect.SetMissionReward(cRequestMissionSlot.SlotMissionInfo.GetInfo());
            }
            // 보상을 받은 임무를 리스트에서 삭제한다
            Legion.Instance.cQuest.RemoveOdinMission(cRequestMissionSlot.SlotMissionInfo.GetInfo().u2ID);

            // 새로 받은 임무를 추가한다
            while (Legion.Instance.cQuest.receiveOdinMissionList.Count > 0)
            {
                Legion.Instance.cQuest.userOdinMissionList.Add(Legion.Instance.cQuest.receiveOdinMissionList.Dequeue());
            }

            // 팝업 셋팅
            SetOdinPopup();
            // 현재 씬이 로비인지 확인하여 알람 체크
            LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
            if (lobbyScene != null)
            {
                lobbyScene.CheckAlram_VIP();
            }
            cRequestMissionSlot = null;
        }
    }
    // 오딘 등급업 보상 서버 응답 받음
    public void ReceiveOdinGradeReward(Server.ERROR_ID err)
    {
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetError(Server.MSGs.MISSION_REWARD, err), Server.ServerMgr.Instance.CallClear);
        }
        else
        {
            // 레벨업에 따른 시스템 언락
            cGradeupPopup.SetPopup(Legion.Instance.u1VIPLevel, (Byte)Legion.Instance.VipLevelUpUnlockSystem());
            // 로비 VIP 아이콘 변경
            LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
            if (lobbyScene != null)
            {
                lobbyScene.CheckAlram_VIP();
            }
            SetOdinPopup();
        }
    }
    // 임무 변경 서버 응답 받음
    public void ReceiveMissionChange(Server.ERROR_ID err)
    {
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetError(Server.MSGs.MISSION_PUSHBACK, err), Server.ServerMgr.Instance.CallClear);
        }
        else
        {
            if (Legion.Instance.cQuest.receiveOdinMissionList.Count > 0)
            {
                UserOdinMission nextMission = Legion.Instance.cQuest.receiveOdinMissionList.Dequeue();
                // 기존 임무와 변경하기
                if (Legion.Instance.cQuest.ChangeOdinMission(cRequestMissionSlot.SlotMissionInfo.GetInfo().u2ID, nextMission))
                {
                    DebugMgr.LogError("임무 정상 교체됨");
                }
                cRequestMissionSlot.SetMissionSlot(nextMission);
            }
            else
            {
                cRequestMissionSlot.gameObject.SetActive(false);
            }
        }
    }
    // ================================ 서버 응답후 콜백 함수 end ==============================================//
}
