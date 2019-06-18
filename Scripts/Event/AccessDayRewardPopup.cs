using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class AccessDayRewardPopup : MonoBehaviour
{
    public Image imgPopup;
    public Image imgEventTitle;

    public Text[] txtContents;    // 타이틀 상단 내용
    public GameObject obj7DayRewardList;
    public GameObject obj10DayRewardList;
    public GameObject obj5DayRewardList;
    public AccessDayRewardSlot[] a7DaySlotList;
    public AccessDayRewardSlot[] a10DaySlotList;
    public AccessDayRewardSlot[] a5DaySlotList;

    public Text txtNextRewardTime;
    public Button btnGetReward;
    public GameObject objGetRewardBtnEffect;

    public Vector3[] titleImgPos;
    public Vector3[] contentsTextPos;

    private bool isTimeCheck;

    // 보상 팝업 관련 변수 V
    public GameObject objRewardPopup;

    public Image imgArrow;
    public GameObject[] objRewardSlot;
    public GameObject[] objItemSlotFrame;
    public Image[] imgGrades;
    public Image[] imgItemIcons;
    public Text[] txtItemNames;
    public RectTransform topRewardPopupEff;
    public RectTransform bottonRewardPopupEff;

    private bool isRewardEffect;

    private void Awake()
    {
        SetPopup();
    }
    private void OnDisable()
    {
        isTimeCheck = false;
    }
    private void OnEnable()
    {
        // 닫기 버튼 활성화
        //objCloseBtn.SetActive(true);
        // 보상 팝업 비 활성화
        objRewardPopup.SetActive(false);
        StartNextRewardTimeCheck();
        PopupManager.Instance.AddPopup(this.gameObject, ClosePopup);
    }

    public void SetPopup()
    {
        if(!EventInfoMgr.Instance.dicOxReward.ContainsKey(Legion.Instance.cEvent.u2OXEventID))
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("mark_event_empty"), null);
            ClosePopup();
            return;
        }
        isTimeCheck = false;
        // 이미지 셋팅
        imgPopup.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Event/AccesDayReward_{0}.AccesDayReward_{0}", Legion.Instance.cEvent.u2OXEventID));
        imgPopup.SetNativeSize();
        imgEventTitle.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/{0}_02.Event_Title_{1}", TextManager.Instance.GetImagePath(), Legion.Instance.cEvent.u2OXEventID));
        imgEventTitle.SetNativeSize();

        int rewardCount = EventInfoMgr.Instance.dicOxReward[Legion.Instance.cEvent.u2OXEventID].Count;
        // 0 == top, 1 == low
        for (int i = 0; i < txtContents.Length; ++i)
        {
            string contentsKey;
            if (i == 0)
            {
                contentsKey = string.Format("days_event_top_title_{0}", Legion.Instance.cEvent.u2OXEventID);
            }
            else
            {
                contentsKey = string.Format("days_event_low_title_{0}", Legion.Instance.cEvent.u2OXEventID);
            }

            string contents = TextManager.Instance.GetText(contentsKey);
            // 키값과 같다면 없는것
            if (contents.CompareTo(contentsKey) == 0)
            {
                txtContents[i].gameObject.SetActive(false);
            }
            else
            {
                txtContents[i].gameObject.SetActive(true);
                txtContents[i].text = contents;
            }
        }
        
        // 보상 갯수에 따라
        if (rewardCount == 5)
        {
            imgEventTitle.transform.position = titleImgPos[0];
            txtContents[0].GetComponent<RectTransform>().anchoredPosition3D = contentsTextPos[0];
            txtContents[0].alignment = TextAnchor.MiddleCenter;

            obj5DayRewardList.SetActive(true);
            obj7DayRewardList.SetActive(false);
            obj10DayRewardList.SetActive(false);
            for(int i = 0; i < a5DaySlotList.Length; ++i)
            {
                a5DaySlotList[i].SetSlot(i + 1, EventInfoMgr.Instance.GetOXReward(i).rewards, (Legion.Instance.cEvent.u1OXanswer > i));
            }
        }
        else if (rewardCount == 7)
        {
            imgEventTitle.transform.position = titleImgPos[0];
            txtContents[0].GetComponent<RectTransform>().anchoredPosition3D = contentsTextPos[0];
            txtContents[0].alignment = TextAnchor.MiddleCenter;

            obj5DayRewardList.SetActive(false);
            obj7DayRewardList.SetActive(true);
            obj10DayRewardList.SetActive(false);
            for (int i = 0; i < a7DaySlotList.Length; ++i)
            {
                a7DaySlotList[i].SetSlot(i + 1, EventInfoMgr.Instance.GetOXReward(i).rewards, (Legion.Instance.cEvent.u1OXanswer > i) );
            }
        }
        else if(rewardCount == 10)
        {
            imgEventTitle.transform.position = titleImgPos[1];
            txtContents[0].GetComponent<RectTransform>().anchoredPosition3D = contentsTextPos[1];
            txtContents[0].alignment = TextAnchor.MiddleLeft;

            obj5DayRewardList.SetActive(false);
            obj7DayRewardList.SetActive(false);
            obj10DayRewardList.SetActive(true);
            for (int i = 0; i < a10DaySlotList.Length; ++i)
            {
                a10DaySlotList[i].SetSlot(i  + 1, EventInfoMgr.Instance.GetOXReward(i).rewards, (Legion.Instance.cEvent.u1OXanswer > i));
            }
        }
    }
    // 보상 받기 버튼 이벤트
    public void OnClickGetReward()
    {
        if (Legion.Instance.cEvent.u2OXlefttime == 9999)
        {
            btnGetReward.interactable = false;
            objGetRewardBtnEffect.SetActive(false);
            return;
        }

        // 보상 아이템에 따른 체크
        if(Legion.Instance.CheckGoodsLimitExcessx(EventInfoMgr.Instance.GetOXReward(Legion.Instance.cEvent.u1OXanswer).rewards))
        {
            Legion.Instance.ShowGoodsOverMessage(EventInfoMgr.Instance.GetOXReward(Legion.Instance.cEvent.u1OXanswer).rewards.u1Type);
            return;
        }

        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestEventGoodsReward(Legion.Instance.cEvent.u2OXEventID, AskGetReward, 1);
    }
    // 보상 받기 서버 요청 결과
    private void AskGetReward(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EVENT_REWARD, err), Server.ServerMgr.Instance.CallClear);
        }
        else
        {
            Legion.Instance.AddGoods(EventInfoMgr.Instance.GetOXReward(Legion.Instance.cEvent.u1OXanswer).rewards);
            Legion.Instance.cEvent.u2OXlefttime = 9999;

            // V 보상 받기 이펙트 받기용 코드 
            int rewardCount = EventInfoMgr.Instance.dicOxReward[Legion.Instance.cEvent.u2OXEventID].Count;
            if (rewardCount == 5)
            {
                a5DaySlotList[Legion.Instance.cEvent.u1OXanswer].RewardCheckMarkEffect(OpenRewardPopup);
            }
            else if (rewardCount == 7)
            {
                a7DaySlotList[Legion.Instance.cEvent.u1OXanswer].RewardCheckMarkEffect(OpenRewardPopup);
            }
            else if (rewardCount == 10)
            {
                a10DaySlotList[Legion.Instance.cEvent.u1OXanswer].RewardCheckMarkEffect(OpenRewardPopup);
            }

            Legion.Instance.cEvent.u1OXanswer += 1;
            StartNextRewardTimeCheck();
        }
    }
    // 다음 보상까지 남은 시간 체크 
    private void StartNextRewardTimeCheck()
    {
        // 이미 시간 체크 중이라면 리턴
        if(isTimeCheck)
        {
            return;
        }

        // 보상 갯수를 받는다
        int rewardCount = EventInfoMgr.Instance.dicOxReward[Legion.Instance.cEvent.u2OXEventID].Count;
        // 보상을 다 받았다면 버튼을 비활성화 한다
        if(Legion.Instance.cEvent.u1OXanswer >= rewardCount)
        {
            txtNextRewardTime.text = string.Format("00:00:00");
            btnGetReward.interactable = false;
            objGetRewardBtnEffect.SetActive(false);
            //objCloseBtn.SetActive(true);
        }
        // 금일 보상을 받았다면 버튼을 비활성화 하고 다음 보상까지 남은 시간을 체크한다
        else if (Legion.Instance.cEvent.u2OXlefttime == 9999)
        {
            btnGetReward.interactable = false;
            objGetRewardBtnEffect.SetActive(false);
            StartCoroutine("NextRewardTime");
            SetSlotEffect();
            //objCloseBtn.SetActive(true);
        }
        else
        {
            txtNextRewardTime.text = string.Format("00:00:00");
            btnGetReward.interactable = true;
            objGetRewardBtnEffect.SetActive(true);
            SetSlotEffect();
            //objCloseBtn.SetActive(false);
        }
    }
    // 다음 보상 시간
    private IEnumerator NextRewardTime()
    {
        isTimeCheck = true;

        TimeSpan gapTime;
        DateTime serverTime = Legion.Instance.ServerTime;
        // 다음 목표까지 남은 시간 확인
        if (LegionInfoMgr.Instance._u1DatelineHour <= serverTime.Hour)
        {
            gapTime = new DateTime().AddDays(1).AddHours(LegionInfoMgr.Instance._u1DatelineHour)
                .Subtract(new DateTime().AddHours(serverTime.Hour).AddMinutes(serverTime.Minute).AddSeconds(serverTime.Second));
        }
        else
        {
            gapTime = new DateTime().AddHours(LegionInfoMgr.Instance._u1DatelineHour)
                .Subtract(new DateTime().AddHours(serverTime.Hour).AddMinutes(serverTime.Minute).AddSeconds(serverTime.Second));
        }

        while (true)
        {
            if (gapTime.Ticks > 0)
            {
                txtNextRewardTime.text = string.Format("{0:00}:{1:00}:{2:00}", gapTime.Hours, gapTime.Minutes, gapTime.Seconds);
            }
            else
            {
                txtNextRewardTime.text = "00:00:00";
                // 재로그인 시킴 팝업 띄우기
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetError(Server.MSGs.NONE, Server.ERROR_ID.LOGIN_DAY_CHANGED), DataMgr.Instance.ReLoadUserData);
                yield break;
            }
            // 실제 시간 1초 마다 작동
            yield return new WaitForSecondsRealtime(1f);
            gapTime = gapTime.Subtract(TimeSpan.FromSeconds(1));
        }
    }
    // 슬롯 이펙트 활상화
    public void SetSlotEffect()
    {
        int rewardCount = EventInfoMgr.Instance.dicOxReward[Legion.Instance.cEvent.u2OXEventID].Count;
        if (rewardCount == 5)
        {
            if (a5DaySlotList.Length > Legion.Instance.cEvent.u1OXanswer)
                a5DaySlotList[Legion.Instance.cEvent.u1OXanswer].SetEffectActive(true);
        }
        else if (rewardCount == 7)
        {
            if (a7DaySlotList.Length > Legion.Instance.cEvent.u1OXanswer)
                a7DaySlotList[Legion.Instance.cEvent.u1OXanswer].SetEffectActive(true);
        }
        else if (rewardCount == 10)
        {
            if (a10DaySlotList.Length > Legion.Instance.cEvent.u1OXanswer)
                a10DaySlotList[Legion.Instance.cEvent.u1OXanswer].SetEffectActive(true);
        }
    }
    // 이벤트 팝업창 닫기
    public void ClosePopup()
    {
        PopupManager.Instance.RemovePopup(this.gameObject);
        this.gameObject.SetActive(false);
		Legion.Instance.SubLoginPopupStep(Legion.LoginPopupStep.OX_EVENT);
    }

#region Reward Popup Function
    public void OpenRewardPopup(object[] param)
    {
        EventOxReward nowReward = EventInfoMgr.Instance.GetOXReward(Legion.Instance.cEvent.u1OXanswer - 1);
        EventOxReward nextReward = EventInfoMgr.Instance.GetOXReward(Legion.Instance.cEvent.u1OXanswer);
        if (nowReward != null && SetRewardPopupItemSlot(nowReward.rewards, 0))
        {
            objRewardSlot[0].SetActive(true);
        }
        else
        {
            objRewardSlot[0].SetActive(false);
        }

        if (nextReward != null && SetRewardPopupItemSlot(nextReward.rewards, 1))
        {
            objRewardSlot[1].SetActive(true);
        }
        else
        {
            objRewardSlot[1].SetActive(false);
        }
        
        // 보상 아이템 슬롯이 둘다 비활성화 되어 있다면 보상 정보가 잘못되었을 가능성이 큼
        if(!objRewardSlot[0].activeSelf && !objRewardSlot[1].activeSelf)
        {
            DebugMgr.LogError("보상 정보가 잘못되어 있음");
            objRewardPopup.SetActive(false);
        }
        else
        {
            objRewardPopup.SetActive(true);
            StartCoroutine("RewardPopupEffect");
        }
    }
    // 보상 정보창 
    public bool SetRewardPopupItemSlot(Goods reward, int idx)
    {
        if (reward == null || idx >= objRewardSlot.Length)
        {
            return false;
        }

        switch((GoodsType)reward.u1Type)
        {
            case GoodsType.GOLD:
            case GoodsType.CASH:
            case GoodsType.FRIENDSHIP_POINT:
                {
                    objItemSlotFrame[idx].SetActive(false);
                    imgItemIcons[idx].gameObject.SetActive(true);

                    if ((GoodsType)reward.u1Type == GoodsType.GOLD)
                        imgItemIcons[idx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.gold_6");
                    else if ((GoodsType)reward.u1Type == GoodsType.CASH)
                        imgItemIcons[idx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.cash_6");
                    else
                        imgItemIcons[idx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.friend_6");

                    imgItemIcons[idx].SetNativeSize();
                    imgItemIcons[idx].rectTransform.localScale = new Vector2(0.5f, 0.5f);

                    txtItemNames[idx].text = string.Format("{0} {1}", Legion.Instance.GetGoodsName(reward), reward.u4Count.ToString("n0"));
                }
                break;
            case GoodsType.KEY:
            case GoodsType.LEAGUE_KEY:
                {
                    objItemSlotFrame[idx].SetActive(false);
                    imgItemIcons[idx].gameObject.SetActive(true);
                    if ((GoodsType)reward.u1Type == GoodsType.KEY)
                        imgItemIcons[idx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.key_5");
                    else
                        imgItemIcons[idx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.league_key");

                    imgItemIcons[idx].SetNativeSize();
                    imgItemIcons[idx].rectTransform.localScale = new Vector2(0.5f, 0.5f);

                    txtItemNames[idx].text = string.Format("{0} {1} {2}", Legion.Instance.GetGoodsName(reward), reward.u4Count, TextManager.Instance.GetText("mark_goods_number_ea"));
                }
                break;
            case GoodsType.MATERIAL:
            case GoodsType.CONSUME:
                {
                    objItemSlotFrame[idx].SetActive(true);
                    imgGrades[idx].sprite = AtlasMgr.Instance.GetGoodsGrade(reward);

                    imgItemIcons[idx].sprite = AtlasMgr.Instance.GetGoodsIcon(reward);
                    imgItemIcons[idx].SetNativeSize();
                    imgItemIcons[idx].rectTransform.localScale = new Vector2(1.5f, 1.5f);
                    
                    txtItemNames[idx].text = string.Format("{0} {1}", Legion.Instance.GetGoodsName(reward), reward.u4Count);
                    break;
                }
            case GoodsType.EQUIP_COUPON:
            case GoodsType.MATERIAL_COUPON:
                {
                    objItemSlotFrame[idx].SetActive(false);
                    imgItemIcons[idx].gameObject.SetActive(true);
                    imgItemIcons[idx].sprite = AtlasMgr.Instance.GetGoodsIcon(reward);
                    imgItemIcons[idx].SetNativeSize();
                    imgItemIcons[idx].rectTransform.localScale = new Vector2(0.5f, 0.5f);

                    txtItemNames[idx].text = string.Format("{0} {1} {2}", Legion.Instance.GetGoodsName(reward), reward.u4Count,
                        TextManager.Instance.GetText("mark_goods_number_ea"));
                }
                break;
            case GoodsType.CHARACTER_PACKAGE:
                {
                    ClassGoodsInfo classGoodsInfo = null;
                    EventInfoMgr.Instance.dicClassGoods.TryGetValue(reward.u2ID, out classGoodsInfo);
                    if (classGoodsInfo != null)
                    {
                        objItemSlotFrame[idx].SetActive(true);
                        imgGrades[idx].sprite = AtlasMgr.Instance.GetGoodsGrade(reward);
                        
                        imgItemIcons[idx].gameObject.SetActive(true);
                        imgItemIcons[idx].sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Hero/hero_icon.{0}", classGoodsInfo.u2ClassID));
                        imgItemIcons[idx].SetNativeSize();
                        imgItemIcons[idx].rectTransform.localScale = new Vector2(1.5f, 1.5f);

                        txtItemNames[idx].text = string.Format("LV{0} {1}", classGoodsInfo.u2Level,
                            TextManager.Instance.GetText(ClassInfoMgr.Instance.GetClassListInfo()[classGoodsInfo.u2ClassID].sName));
                    }
                }
                break;
            case GoodsType.EQUIP_GOODS:
                {
                    ClassGoodsEquipInfo goodsEquipInfo = null;
                    EventInfoMgr.Instance.dicClassGoodsEquip.TryGetValue(reward.u2ID, out goodsEquipInfo);
                    if (goodsEquipInfo != null)
                    {
                        EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(goodsEquipInfo.u2Equip);
                        if (equipInfo != null)
                        {
                            objItemSlotFrame[idx].SetActive(true);
                            imgGrades[idx].sprite = AtlasMgr.Instance.GetGoodsGrade(reward);

                            imgItemIcons[idx].gameObject.SetActive(true);
                            imgItemIcons[idx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/" + equipInfo.cModel.sImagePath);
                            imgItemIcons[idx].SetNativeSize();
                            imgItemIcons[idx].rectTransform.localScale = new Vector2(1.5f, 1.5f);

                            txtItemNames[idx].text = string.Format("{0}{1}\n{2}",
                                TextManager.Instance.GetText(string.Format("forge_level_2_{0}", goodsEquipInfo.u1SmithingLevel)),
                                TextManager.Instance.GetText(string.Format("equip_star_{0}", goodsEquipInfo.u1StarLevel)),
                                TextManager.Instance.GetText(equipInfo.EquipTypeKey()));
                        }
                    }
                }
                break;
        }
        return true;
    }
    // 보상 정보창 연출
    private IEnumerator RewardPopupEffect()
    {
        bool scaleUp = false;
        float arrowSecond = 0.0f;
        float popupOpenTime = 0.0f;
        float seconds = 0.0f;
        // 다음 보상이 없다면 화살표를 비활성화 하고 왼쪽 슬롯을 가운데로 옴긴다 않는다
        if (!objRewardSlot[1].activeSelf)
        {
            objRewardSlot[0].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, -40, 0);
            imgArrow.gameObject.SetActive(false);
        }

        while (true)
        {
            if(Vector3.Distance(topRewardPopupEff.anchoredPosition3D, new Vector3(100, 0, 0)) >= 0.2f )
            {
                Vector3 newPos = Vector3.Lerp(topRewardPopupEff.anchoredPosition3D, new Vector3(100, 0, 0), seconds * Time.deltaTime);
                topRewardPopupEff.anchoredPosition3D = newPos;
            }
            if(Vector3.Distance(bottonRewardPopupEff.anchoredPosition3D, new Vector3(700, 0, 0)) >= 0.2f)
            {
                Vector3 newPos = Vector3.Lerp(bottonRewardPopupEff.anchoredPosition3D, new Vector3(700, 0, 0), seconds * Time.deltaTime);
                bottonRewardPopupEff.anchoredPosition3D = newPos;
            }

            if (objRewardSlot[1].activeSelf)
            {
                if (scaleUp && arrowSecond >= 0.4f)
                {
                    LeanTween.scale(imgArrow.rectTransform, Vector2.one, 0.4f);
                    scaleUp = !scaleUp;
                    arrowSecond = 0;
                }
                else if (!scaleUp && arrowSecond >= 0.4f)
                {
                    LeanTween.scale(imgArrow.rectTransform, new Vector2(0.6f, 0.6f), 0.4f);
                    scaleUp = !scaleUp;
                    arrowSecond = 0;
                }
                arrowSecond += Time.deltaTime;
            }

            popupOpenTime += Time.deltaTime;
            seconds += 0.6f;
            yield return null;
            if (popupOpenTime >= 3.0f)
            {
                CloseRewardPopup();
                yield break;
            }
        }
    }
    // 보상 팝업 닫기
    public void CloseRewardPopup()
    {
        if(objRewardPopup.activeSelf)
        {
            StopCoroutine("RewardPopupEffect");
            objRewardPopup.SetActive(false);
            ClosePopup();
            //SetSlotEffect();
        }
    }
#endregion
}