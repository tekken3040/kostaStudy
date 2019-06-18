using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

public class EventMarbleMain : MonoBehaviour
{
    public Image _imgMarbleWindow;
    public Text diceText;
    public Text guardianMarkText;

    public Button diceBtn;
    public Image popupImg;
    public Text popupText;
    public GameObject popupObject;
    public GameObject diceNumShow;

    GameObject marbleTradeWindow;

    int achieveindex;
    int _chessManPos = 0;
    private UInt16 u2SymbolMarkID = 0;  // 교환 할 재화의 ID;

    private EventMarbleGame cMarbleGameInfo;

    private List<EventMarbleBoard> BoardInfoList;
    public List<EventMarbleBoardSlot> cBoardSlotList;

    private List<UserAchievement> achievementReward;
    public MarbleAchieveSlot[] arrAchieveSlotList;
    public AudioClip moveAudio;

    delegate void GetGmark();
    GetGmark popupMessage;

    private void Awake()
    {
        cMarbleGameInfo = EventInfoMgr.Instance.GetOpenMarbleGameInfo();
        if(cMarbleGameInfo == null)
        {
            Destroy(this.gameObject);
            return;
        }

        // 징표의 아이디값 찾아오기
        if (ShopInfoMgr.Instance.dicShopGoodData.ContainsKey(cMarbleGameInfo.au2ShopID[0]))
            u2SymbolMarkID = ShopInfoMgr.Instance.dicShopGoodData[cMarbleGameInfo.au2ShopID[0]].cBuyGoods.u2ID;

        // 보드 정보 셋팅
        BoardInfoList = EventInfoMgr.Instance.GetMarbleBoardInfoList(cMarbleGameInfo.u2BoardID);
        SetBoardRewardInfo();
        // 업적 셋팅
        achievementReward = Legion.Instance.cQuest.dicAchievements.Values.Where(cs => cs.GetInfo().u2EventID == cMarbleGameInfo.u2EventID).ToList();
        SetAchievementBtn();
        // 증표 셋팅
        SetGuardianMark();
        // 주사위 갯수 셋팅
        SetDiceCount();
    }

    // 마블 보드 셋팅
    private void SetBoardRewardInfo()
    {
        for (int i = 0; i < cBoardSlotList.Count; ++i)
        {
            if (i >= BoardInfoList.Count)
                break;

            cBoardSlotList[i].SetBoardSlot(BoardInfoList[i]);
        }

        _chessManPos = EventInfoMgr.Instance.dicEventReward[cMarbleGameInfo.u2EventID].u1RewardIndex;
        // 보드 위치 이펙트 이미지 활성화
        cBoardSlotList[_chessManPos].BoardEffectActive(true);
    }
    // 업적 셋팅
    private void SetAchievementBtn()
    {
        for (int i = 0; i < arrAchieveSlotList.Length; ++i)
        {
            if (i >= achievementReward.Count)
            {
                arrAchieveSlotList[i].gameObject.SetActive(false);
                continue;
            }

            arrAchieveSlotList[i].SetAchieveSlot(achievementReward[i]);
        }
    }
    // 증표 셋팅
    private void SetGuardianMark()
    {
        // 가디언 징표 갯수 텍스트 셋팅
        if (EventInfoMgr.Instance.dicMarbleBag.ContainsKey(u2SymbolMarkID))
            guardianMarkText.text = EventInfoMgr.Instance.dicMarbleBag[u2SymbolMarkID].u4Count.ToString();
        else
            guardianMarkText.text = "0";
    }
    // 주사위 셋팅
    private void SetDiceCount()
    {
        UInt32 dicItemCount = 0;
        if (EventInfoMgr.Instance.dicMarbleBag.ContainsKey(cMarbleGameInfo.cRollItem.u2ID))
        {
            dicItemCount = EventInfoMgr.Instance.dicMarbleBag[cMarbleGameInfo.cRollItem.u2ID].u4Count;
            if (dicItemCount == 0)
                diceBtn.interactable = false;
            else
                diceBtn.interactable = true;
        }
        diceText.text = dicItemCount.ToString();
    }
    // 주사위 굴리기 버튼 클릭
    public void OnClickDiceRolling()
    {
        // 마블 이벤트 정보가 없다면
        if (cMarbleGameInfo == null)
            return;
        // 다이스 아이템이 없거나 갯수가 부족하다면
        if (!EventInfoMgr.Instance.dicMarbleBag.ContainsKey(cMarbleGameInfo.cRollItem.u2ID) ||
            !Legion.Instance.CheckEnoughGoods(cMarbleGameInfo.cRollItem))
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("event_marble_dice_empty"), null);
            return;
        }

		PopupManager.Instance.ShowLoadingPopup (1);

        Server.ServerMgr.Instance.RequestEventGoodsReward(cMarbleGameInfo.u2EventID, RecieveDiceAndGurdianMark);
    }

    private void RecieveDiceAndGurdianMark(Server.ERROR_ID err)
    {
		PopupManager.Instance.CloseLoadingPopup();
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),
                TextManager.Instance.GetError(Server.MSGs.EVENT_REWARD, err) + TextManager.Instance.GetText("event_reward failed"),
                null);
            return;
        }
        else if (err == Server.ERROR_ID.NONE)
        {
            // 주사위가 갯수가 있는지 확인
            if (Legion.Instance.CheckEnoughGoods(cMarbleGameInfo.cRollItem))
            {
                // 주사위를 차감한다
                Legion.Instance.SubGoods(cMarbleGameInfo.cRollItem);
                SetDiceCount();
            }
            StartCoroutine("DicePopup");
        }
    }

    private IEnumerator DicePopup()
    {
        diceBtn.interactable = false;
        //yield return new WaitForSeconds(0.5f);

        //cBoardSlotList[(int)EventInfoMgr.Instance.sEventGoodsReward.u4RecordValue].BoardEffectActive(true);
        diceNumShow.SetActive(true);
        diceNumShow.GetComponentInChildren<Text>().text = string.Format(TextManager.Instance.GetText("event_marble_dice_number"), (int)EventInfoMgr.Instance.sEventGoodsReward.u4RecordValue);

        yield return new WaitForSeconds(1f);

        //cBoardSlotList[(int)EventInfoMgr.Instance.sEventGoodsReward.u4RecordValue].BoardEffectActive(false);
        diceNumShow.SetActive(false);

        // 이동할 주사위말 인덱스 셋팅
        ChessManMove(EventInfoMgr.Instance.sEventGoodsReward.u1LastRewardIndex,
            (int)EventInfoMgr.Instance.sEventGoodsReward.u4RecordValue);
    }

    // 보드 이동
    private void ChessManMove(int currentPos, int moveNum)
    {
        // 현재 위치에 이동 할 숫자를 더 한다
        _chessManPos += moveNum;
        // 위치가 보드의 갯수보다 같거나 크면
        if (_chessManPos >= BoardInfoList.Count)
            _chessManPos = (Byte)(_chessManPos % BoardInfoList.Count);

        // 변경된 값을 저장한다
        EventReward eventInfo = EventInfoMgr.Instance.dicEventReward[cMarbleGameInfo.u2EventID];
        eventInfo.u1RewardIndex = (Byte)(_chessManPos);
        EventInfoMgr.Instance.dicEventReward[cMarbleGameInfo.u2EventID] = eventInfo;
        //EventInfoMgr.Instance.dicEventReward.Add(cMarbleGameInfo.u2EventID, eventInfo);

        // 주사위말 이동
        StartCoroutine(SecMoveChessMan(currentPos, moveNum));
    }

    IEnumerator SecMoveChessMan(int PastIndex, int moveIndex)
    {
        // 뒤로 가기 처리
        if (moveIndex < 0)
        {
            moveIndex = Math.Abs(moveIndex);
            for (int i = 0; i < moveIndex; i++)
            {
                yield return new WaitForSeconds(0.15f);

                cBoardSlotList[PastIndex].BoardEffectActive(false);

                if (PastIndex == 0)
                    PastIndex = 15;
                else
                    --PastIndex;

                cBoardSlotList[PastIndex].BoardEffectActive(true);
                SoundManager.Instance.PlayEff(moveAudio);
            }
        }
        else
        {
            // 앞으로 가기 처리
            for (int i = 0; i < moveIndex; i++)
            {
                yield return new WaitForSeconds(0.15f);

                cBoardSlotList[PastIndex].BoardEffectActive(false);
                if (PastIndex == 15)
                {
                    PastIndex = 0;
                    popupMessage = new GetGmark(GmarkPopupMessage);
                }
                else
                {
                    ++PastIndex;
                }
                cBoardSlotList[PastIndex].BoardEffectActive(true);
                SoundManager.Instance.PlayEff(moveAudio);
            }
        }

        yield return new WaitForSeconds(1f);

        // 주사위 굴려서 나온 말판 위치에 이동이 발생할 경우
        if (BoardInfoList[_chessManPos].u4Move != 0)
        {
            ChessManMove(_chessManPos, BoardInfoList[_chessManPos].u4Move);
            yield break;
        }
        //말 위치에 따른 보상 팝업
        MarbleRewardPopup();
    }

    private void MarbleRewardPopup()
    {
        if (BoardInfoList[_chessManPos].cReward.u1Type != 0)
        {
            Goods marbleRewardGoods = BoardInfoList[_chessManPos].cReward;
            popupImg.sprite = AtlasMgr.Instance.GetGoodsIcon(marbleRewardGoods);
            popupText.text = Legion.Instance.GetGoodsName(marbleRewardGoods) + " " + marbleRewardGoods.u4Count + TextManager.Instance.GetText("event_marble_post_send");// "를(을)\n우편함으로 지급하였습니다.";
            popupObject.SetActive(true);
        }

        StartCoroutine(gMarkPopupDelay());
        SetDiceCount();
    }

    private IEnumerator gMarkPopupDelay()
    {
        yield return new WaitForSeconds(1.5f);

        popupObject.SetActive(false);
        if (popupMessage != null)
        {
            popupMessage();
            popupMessage -= GmarkPopupMessage;
        }
    }

    private void GmarkPopupMessage()
    {
        StringBuilder tempString = new StringBuilder();
        Goods marbleRewardGoods = BoardInfoList[0].cReward;
        popupImg.sprite = AtlasMgr.Instance.GetGoodsIcon(marbleRewardGoods);
        popupText.text = tempString.Append(Legion.Instance.GetGoodsName(marbleRewardGoods)).Append(" ").Append(marbleRewardGoods.u4Count).Append(TextManager.Instance.GetText("event_marble_post_send")).ToString();
    }

    
    public void OnClickAhieveMentReward(int index)
    {
        // 이미 클리어 되었고 보상을 받았다면 리턴
        if (achievementReward[index].isClear() == true && achievementReward[index].bRewarded == true)
            return;

        PopupManager.Instance.ShowLoadingPopup(1);

        achieveindex = index;
        UInt16[] arrID = new UInt16[1];
        arrID[0] = achievementReward[achieveindex].u2ID;
        Server.ServerMgr.Instance.RequestQuestAchievementReward(arrID, 255, 0, RecieveAchieveMentReward);
    }

    private void RecieveAchieveMentReward( Server.ERROR_ID err)
    {
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),
                TextManager.Instance.GetError(Server.MSGs.QUEST_ACHIEVEMENT_REWARD, err),
                null);
            return;
        }

        else if (err == Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();

            arrAchieveSlotList[achieveindex].SetAchieveSlotClear();
            Legion.Instance.cQuest.dicAchievements[achievementReward[achieveindex].u2ID].bRewarded = true;
            Legion.Instance.AddGoods(achievementReward[achieveindex].GetInfo().acReward[0]);
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("event_title_marble"), TextManager.Instance.GetText("event_marble_dice_get"), null);
            SetDiceCount();
        }
    }

    // 업적 바로가기
    public void GotoAchieve(int index)
    {
        if (Legion.Instance.sName == "" && index == 1)
            return;

        AchievementInfo info = achievementReward[index].GetInfo();
        MENU shortCut = (MENU)info.u2ShortCut;
        int shortCutPopup = info.u2ShortCutDetail;
        FadeEffectMgr.Instance.QuickChangeScene(shortCut, shortCutPopup);
    }
    // 마블 보상 교환 창 오픈
    public void OpenTradeWindow()
    {
        if (marbleTradeWindow == null)
        {
            marbleTradeWindow = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_EventMarbleTrade.prefab", typeof(GameObject)) as GameObject);
            RectTransform rectTr = marbleTradeWindow.GetComponent<RectTransform>();
            rectTr.SetParent(this.transform);
            rectTr.localPosition = Vector3.zero;
            rectTr.localScale = Vector3.one;
            rectTr.sizeDelta = Vector2.zero;

            EventMarbleTrade tradePopup = marbleTradeWindow.GetComponent<EventMarbleTrade>();
            tradePopup._CloseBtn.onClick.AddListener(CloseTradePopup);
            tradePopup.SetTradeItemInfo(cMarbleGameInfo);
        }

        marbleTradeWindow.SetActive(true);
        _imgMarbleWindow.gameObject.SetActive(false);
		PopupManager.Instance.AddPopup (marbleTradeWindow, CloseTradePopup);
    }

    public void CloseTradePopup()
    {
        SetGuardianMark();
        _imgMarbleWindow.gameObject.SetActive(true);
        marbleTradeWindow.SetActive(false);
		PopupManager.Instance.RemovePopup (marbleTradeWindow);
    }

    public void OnClickMarbleMainClose()
    {
		PopupManager.Instance.RemovePopup (gameObject);
        Destroy(this.gameObject);
    }

    public void ClosePopup()
    {
        popupObject.SetActive(false);
    }
}
