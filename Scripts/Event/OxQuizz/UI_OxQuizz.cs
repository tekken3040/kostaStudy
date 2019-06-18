using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class UI_OxQuizz : MonoBehaviour
{
    [SerializeField] Text txtQuestion;
    [SerializeField] Text txtRewardTitle;
    [SerializeField] Text txtRewardValue;
    [SerializeField] Text txtStackDay;
    [SerializeField] Text txtDesc;
    [SerializeField] Text txtBannerTitle;
    [SerializeField] Text txtBannerDesc1;
    [SerializeField] Text txtBannerDesc2;
    [SerializeField] Text txtFailTime;

    [SerializeField] Image imgRewardIcon;
    [SerializeField] Image imgRewardCharSmallIcon;
    [SerializeField] Image imgRewardCharLargeIcon;
    [SerializeField] Image imgRewardBanner;
    [SerializeField] Image imgRewardProgress;

    [SerializeField] Button Btn_RewardGet;
    [SerializeField] Button Btn_Ox_O;
    [SerializeField] Button Btn_Ox_X;

    [SerializeField] GameObject objPopupGroup;
    [SerializeField] GameObject objCorrectPopup;
    [SerializeField] GameObject objFailPopup;
    [SerializeField] GameObject objRewardPopup;

    [SerializeField] Image[] imgStackRewardIcon;
    [SerializeField] Text[] txtStackRewardDay;
    [SerializeField] GameObject _effect_Btn;

    StringBuilder tempStringBuilder;
    EventPanel _eventPanel;
    Byte u1RewardType = 0;
    Byte u1TodayRewardNum = 0;
    TimeSpan tsLeftTime;
    bool tempAnswer = false;

    public void OnEnable()
    {
        //테스트용 셋팅
        //Legion.Instance.cEvent.u1OXanswer = 1;
        //Legion.Instance.cEvent.u1OXquestion = 1;
        //Legion.Instance.cEvent.u2OXlefttime = 9998;
        //Legion.Instance.cEvent.u1OXtotalReward = 0;
        //Legion.Instance.cEvent.u1TodayOxDone = 0;
        if(_eventPanel == null)
            _eventPanel = GameObject.Find("EventPanel").GetComponent<EventPanel>();
        PopupManager.Instance.AddPopup(this.gameObject, OnClickClose);
        InitData();
    }

    public void InitData()
    {
        tempStringBuilder = new StringBuilder();

        tempStringBuilder.Append(Legion.Instance.cEvent.u1OXanswer).Append(TextManager.Instance.GetText("mark_time_day"));
        txtStackDay.text = tempStringBuilder.ToString();
        txtDesc.text = TextManager.Instance.GetText("ox_ui_desc");
        txtBannerTitle.text = TextManager.Instance.GetText("ox_ui_banner_top");
        txtBannerDesc1.text = TextManager.Instance.GetText("ox_ui_banner_low_1");
        txtBannerDesc2.text = TextManager.Instance.GetText("ox_ui_banner_low_2");
        Btn_Ox_O.transform.GetChild(0).gameObject.SetActive(false);
        Btn_Ox_X.transform.GetChild(0).gameObject.SetActive(false);
        for(int i=0; i<3; i++)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append((i+1)*5).Append(TextManager.Instance.GetText("ox_ui_day"));
            txtStackRewardDay[i].text = tempStringBuilder.ToString();
            //imgStackRewardIcon[i].sprite = AtlasMgr.Instance.GetGoodsIcon(EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Stack - 1), i][(Byte)((i + 1) * 5)].rewards);
            EventOxReward oxReward = EventInfoMgr.Instance.GetTypeOfOXReward(EventOxReward.REWARD_TYPE.Stack, i);
            if (oxReward != null)
            {
                imgStackRewardIcon[i].gameObject.SetActive(true);
                imgStackRewardIcon[i].sprite = AtlasMgr.Instance.GetGoodsIcon(oxReward.rewards);
            }
            else
            {
                imgStackRewardIcon[i].gameObject.SetActive(false);
            }
            
        }
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(20).Append(TextManager.Instance.GetText("ox_ui_day"));
        txtStackRewardDay[3].text = tempStringBuilder.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        if(Legion.Instance.cEvent.u2OXlefttime == 9998 || Legion.Instance.cEvent.u2OXlefttime == 9999)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetText("ox_ui_question_right")).Append(TextManager.Instance.GetText("ox_ui_question_cha_msg"));
            txtQuestion.text = tempStringBuilder.ToString();
        }
        else if(Legion.Instance.cEvent.u2OXlefttime == 0)
        {
            EventOxQuestion oxQuestion = EventInfoMgr.Instance.GetOXQuestion(Legion.Instance.cEvent.u1OXquestion - 1);
            if (oxQuestion != null)
            {
                txtQuestion.text = TextManager.Instance.GetText(oxQuestion.strQuestion);
            }
            else
            {
                txtQuestion.text = "";
            }
            //txtQuestion.text = TextManager.Instance.GetText(EventInfoMgr.Instance.dicOxQuestion[Legion.Instance.cEvent.u1OXquestion].strQuestion);
        }
        else
        {
            objPopupGroup.SetActive(true);
            objFailPopup.SetActive(true);
            tsLeftTime = TimeSpan.FromSeconds(Legion.Instance.cEvent.u2OXlefttime);
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(String.Format("{0:00}:{1:00}", tsLeftTime.Hours, tsLeftTime.Minutes));
            tempStringBuilder.Append(TextManager.Instance.GetText("ox_ui_question_wrong_time"));

            if(tempAnswer)
            {
                Btn_Ox_O.transform.GetChild(0).gameObject.SetActive(true);
                Btn_Ox_X.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                Btn_Ox_O.transform.GetChild(0).gameObject.SetActive(false);
                Btn_Ox_X.transform.GetChild(0).gameObject.SetActive(true);
            }
            txtFailTime.text = tempStringBuilder.ToString();
        }
        //txtStackDay.text = tempStringBuilder.ToString();

        GetRewardImage();
    }

    private void GetRewardImage()
    {
        if(Legion.Instance.cEvent.u1OXanswer == 0)
            imgRewardProgress.rectTransform.sizeDelta = new Vector2(0, imgRewardProgress.rectTransform.sizeDelta.y);
        else if(Legion.Instance.cEvent.u1OXanswer == 20)
            imgRewardProgress.rectTransform.sizeDelta = new Vector2(420, imgRewardProgress.rectTransform.sizeDelta.y);
        else
            imgRewardProgress.rectTransform.sizeDelta = new Vector2(
                (float)(21*Legion.Instance.cEvent.u1OXanswer), 
                imgRewardProgress.rectTransform.sizeDelta.y);

        if(Legion.Instance.cEvent.u1OXanswer == 0)
            u1TodayRewardNum = 1;
        else
            u1TodayRewardNum = Legion.Instance.cEvent.u1OXanswer;

        if(Legion.Instance.cEvent.u1OXtotalReward == 20)
        {
            imgRewardIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_02.ox_char_icon_4");
            txtRewardTitle.text = String.Format(TextManager.Instance.GetText("ox_ui_reward_title_day"), Legion.Instance.cEvent.u1OXtotalReward);
            txtRewardValue.text = TextManager.Instance.GetText("ox_reward_equip_set_4");
            u1RewardType = (Byte)EventOxReward.REWARD_TYPE.Stack;
            //Btn_RewardGet.interactable = false;
            RewardGetBtnEnable(false);
        }
        else
        {
            if(Legion.Instance.cEvent.u2OXlefttime == 9998)
            {
                EventOxReward oxReward = EventInfoMgr.Instance.GetTypeOfOXReward(EventOxReward.REWARD_TYPE.Daily, u1TodayRewardNum - 1);
                if(oxReward != null)
                {
                    imgRewardIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(oxReward.rewards);
                }
                //imgRewardIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Daily-1), //u1TodayRewardNum-1]
                //    [u1TodayRewardNum].rewards);
                txtRewardTitle.text = TextManager.Instance.GetText("ox_ui_reward_title_today");//**
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                //switch(EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Daily-1), (u1TodayRewardNum-1)][u1TodayRewardNum].rewards.u1Type)
                switch(oxReward.rewards.u1Type)
                {
                    case (Byte)GoodsType.CASH:
                        tempStringBuilder.Append(TextManager.Instance.GetText("mark_cash"));
                        break;

                    case (Byte)GoodsType.GOLD:
                        tempStringBuilder.Append(TextManager.Instance.GetText("mark_gold"));
                        break;

                    case (Byte)GoodsType.KEY:
                        tempStringBuilder.Append(TextManager.Instance.GetText("mark_key"));
                        break;

                    case (Byte)GoodsType.CONSUME:
                        //tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo()[EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Daily-1), (u1TodayRewardNum-1)][u1TodayRewardNum].rewards.u2ID].sName));
                        tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo()[oxReward.rewards.u2ID].sName));
                        break;
                }
                tempStringBuilder.Append(" x ")
                    //.Append(EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Daily - 1), (u1TodayRewardNum - 1)][u1TodayRewardNum].rewards.u4Count)
                    .Append(oxReward.rewards.u4Count)
                    .Append(TextManager.Instance.GetText("mark_goods_number_ea"));
                txtRewardValue.text = tempStringBuilder.ToString();
                if (Legion.Instance.cEvent.u2OXlefttime == 9998)
                    //Btn_RewardGet.interactable = true;
                    RewardGetBtnEnable(true);
                else
                    RewardGetBtnEnable(false);
                    //Btn_RewardGet.interactable = false;
                u1RewardType = (Byte)EventOxReward.REWARD_TYPE.Daily;
            }
            else if(Legion.Instance.cEvent.u1OXtotalReward != Legion.Instance.cEvent.u1OXanswer &&
                Legion.Instance.cEvent.u1OXtotalReward < Legion.Instance.cEvent.u1OXanswer &&
                Legion.Instance.cEvent.u1OXanswer%5 == 0 && Legion.Instance.cEvent.u2OXlefttime == 9999)
            {
                if (Legion.Instance.cEvent.u1OXtotalReward == 0)
                {
                    EventOxReward oxReward = EventInfoMgr.Instance.GetTypeOfOXReward(EventOxReward.REWARD_TYPE.Stack, 0);
                    //imgRewardIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(EventInfoMgr.Instance.dicOxReward[(Byte)EventOxReward.REWARD_TYPE.Stack-1, 0][5].rewards);
                    imgRewardIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(oxReward.rewards);
                    txtRewardTitle.text = String.Format(TextManager.Instance.GetText("ox_ui_reward_title_day"), 5);
                }
                else
                {
                    if (Legion.Instance.cEvent.u1OXanswer == EventOxReward.MAX_DAY)
                        imgRewardIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_02.ox_char_icon_4");
                    else
                    {
                        EventOxReward oxReward = EventInfoMgr.Instance.GetTypeOfOXReward(EventOxReward.REWARD_TYPE.Stack, (Legion.Instance.cEvent.u1OXtotalReward + 5) / 5 - 1);
                        imgRewardIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(oxReward.rewards);
                        //imgRewardIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(EventInfoMgr.Instance.dicOxReward[(Byte)EventOxReward.REWARD_TYPE.Stack - 1, ((Legion.Instance.cEvent.u1OXtotalReward + 5) / 5 - 1)]
                        //[(Byte)(Legion.Instance.cEvent.u1OXtotalReward + 5)].rewards);
                    }


                    txtRewardTitle.text = String.Format(TextManager.Instance.GetText("ox_ui_reward_title_day"), Legion.Instance.cEvent.u1OXtotalReward+5);
                }
                
                switch((Legion.Instance.cEvent.u1OXtotalReward+5))
                {
                    case 5:
                        txtRewardValue.text = TextManager.Instance.GetText("ox_reward_equip_set_1");
                        break;
                    case 10:
                        txtRewardValue.text = TextManager.Instance.GetText("ox_reward_equip_set_2");
                        break;
                    case 15:
                        txtRewardValue.text = TextManager.Instance.GetText("ox_reward_equip_set_3");
                        break;
                    case 20:
                        txtRewardValue.text = TextManager.Instance.GetText("ox_reward_equip_set_4");
                        break;
                }
                //if(Legion.Instance.cEvent.u1OXtotalReward == 0)
                //    txtRewardValue.text = TextManager.Instance.GetText("ox_reward_equip_set_1");
                //else if(Legion.Instance.cEvent.u1OXanswer == EventOxReward.MAX_DAY)
                //    txtRewardValue.text = TextManager.Instance.GetText("ox_reward_equip_set_4");
                //Btn_RewardGet.interactable = true;
                RewardGetBtnEnable(true);
                u1RewardType = (Byte)EventOxReward.REWARD_TYPE.Stack;
            }
            else if(Legion.Instance.cEvent.u2OXlefttime == 0 && Legion.Instance.cEvent.u1TodayOxDone != 2)
            {
                Byte tempNum = 0;
                if(Legion.Instance.cEvent.u1OXanswer == 0)
                    tempNum = 1;
                else
                    tempNum = (Byte)(Legion.Instance.cEvent.u1OXanswer + 1);

                EventOxReward oxReward = EventInfoMgr.Instance.GetTypeOfOXReward(EventOxReward.REWARD_TYPE.Daily, tempNum - 1);
                imgRewardIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(oxReward.rewards);

                //imgRewardIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Daily-1), (tempNum-1)]
                //    [(Byte)(tempNum)].rewards);
                txtRewardTitle.text = TextManager.Instance.GetText("ox_ui_reward_title_today");//**
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                //switch(EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Daily-1), (tempNum-1)][(Byte)(tempNum)].rewards.u1Type)
                switch(oxReward.rewards.u1Type)
                {
                    case (Byte)GoodsType.CASH:
                        tempStringBuilder.Append(TextManager.Instance.GetText("mark_cash"));
                        break;

                    case (Byte)GoodsType.GOLD:
                        tempStringBuilder.Append(TextManager.Instance.GetText("mark_gold"));
                        break;

                    case (Byte)GoodsType.KEY:
                        tempStringBuilder.Append(TextManager.Instance.GetText("mark_key"));
                        break;

                    case (Byte)GoodsType.CONSUME:
                        //tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo()[EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Daily-1), (tempNum-1)][(Byte)(tempNum)].rewards.u2ID].sName));
                        tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo()[oxReward.rewards.u2ID].sName));
                        break;
                }
                tempStringBuilder.Append(" x ")
                    //Append(EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Daily-1), (tempNum-1)][(Byte)(tempNum)].rewards.u4Count)
                    .Append(oxReward.rewards.u4Count)
                    .Append(TextManager.Instance.GetText("mark_goods_number_ea"));
                txtRewardValue.text = tempStringBuilder.ToString();
                if (Legion.Instance.cEvent.u2OXlefttime == 9998)
                    RewardGetBtnEnable(true);
                //Btn_RewardGet.interactable = true;
                else
                    RewardGetBtnEnable(false);
                //Btn_RewardGet.interactable = false;
                u1RewardType = (Byte)EventOxReward.REWARD_TYPE.Daily;
            }
            else
            {
                EventOxReward oxReward = EventInfoMgr.Instance.GetTypeOfOXReward(EventOxReward.REWARD_TYPE.Daily, u1TodayRewardNum);
                //imgRewardIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Daily-1), (u1TodayRewardNum-1)]
                //    [u1TodayRewardNum].rewards);
                imgRewardIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(oxReward.rewards);
                txtRewardTitle.text = TextManager.Instance.GetText("ox_ui_reward_title_today");//**
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                //switch(EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Daily-1), (u1TodayRewardNum-1)][u1TodayRewardNum].rewards.u1Type)
                switch(oxReward.rewards.u1Type)
                {
                    case (Byte)GoodsType.CASH:
                        tempStringBuilder.Append(TextManager.Instance.GetText("mark_cash"));
                        break;

                    case (Byte)GoodsType.GOLD:
                        tempStringBuilder.Append(TextManager.Instance.GetText("mark_gold"));
                        break;

                    case (Byte)GoodsType.KEY:
                        tempStringBuilder.Append(TextManager.Instance.GetText("mark_key"));
                        break;

                    case (Byte)GoodsType.CONSUME:
                        //tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo()[EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Daily-1), (u1TodayRewardNum-1)][u1TodayRewardNum].rewards.u2ID].sName));
                        tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo()[oxReward.rewards.u2ID].sName));
                        break;
                }
                tempStringBuilder.Append(" x ").
                    //Append(EventInfoMgr.Instance.dicOxReward[(Byte)(EventOxReward.REWARD_TYPE.Daily-1), (u1TodayRewardNum-1)[u1TodayRewardNum].rewards.u4Count)
                    Append(oxReward.rewards.u4Count)
                    .Append(TextManager.Instance.GetText("mark_goods_number_ea"));
                txtRewardValue.text = tempStringBuilder.ToString();
                if(Legion.Instance.cEvent.u2OXlefttime == 9998)
                    RewardGetBtnEnable(true);
                //Btn_RewardGet.interactable = true;
                else
                    RewardGetBtnEnable(false);
                //Btn_RewardGet.interactable = false;
                u1RewardType = (Byte)EventOxReward.REWARD_TYPE.Daily;
            }
        }
        imgRewardIcon.SetNativeSize();
        if(Legion.Instance.cEvent.u1TodayOxDone == 1)
        {
            Btn_Ox_O.interactable = false;
            Btn_Ox_X.interactable = false;
        }
    }

    public void OnClickClose()
    {
        if(Legion.Instance.cEvent.u1TodayOxDone == 1 && objCorrectPopup.active)
        {
            PopupManager.Instance.RemovePopup(objCorrectPopup);
            objCorrectPopup.SetActive(false);
            objPopupGroup.SetActive(false);
        }
        else
        {
            if(Legion.Instance.cEvent.u1TodayOxDone == 2)
                _eventPanel.OxQuizTimeCheckStart();
            PopupManager.Instance.RemovePopup(this.gameObject);
            this.gameObject.SetActive(false);

            Legion.Instance.SubLoginPopupStep(Legion.LoginPopupStep.OX_EVENT);
        }

    }

    public void OnClickReward()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestEventGoodsReward(59852, AskGetReward, u1RewardType);
    }

    private void AskGetReward(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EVENT_REWARD, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
            if (u1RewardType == (Byte)EventOxReward.REWARD_TYPE.None)
                return;
            else if (u1RewardType == (Byte)EventOxReward.REWARD_TYPE.Stack)
            {
                //Legion.Instance.AddGoods(EventInfoMgr.Instance.dicOxReward[u1RewardType-1, Legion.Instance.cEvent.u1OXanswer/5-1][Legion.Instance.cEvent.u1OXanswer].rewards);
                Legion.Instance.AddGoods(EventInfoMgr.Instance.GetTypeOfOXReward((EventOxReward.REWARD_TYPE)u1RewardType, Legion.Instance.cEvent.u1OXanswer / 5 - 1).rewards);
            }
            else
            {
                //Legion.Instance.AddGoods(EventInfoMgr.Instance.dicOxReward[u1RewardType - 1, Legion.Instance.cEvent.u1OXanswer - 1][Legion.Instance.cEvent.u1OXanswer].rewards);
                Legion.Instance.AddGoods(EventInfoMgr.Instance.GetTypeOfOXReward((EventOxReward.REWARD_TYPE)u1RewardType, Legion.Instance.cEvent.u1OXanswer - 1).rewards);
            }
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            //Legion.Instance.cEvent.u1OXtotalReward++;
            if(u1RewardType == (Byte)(EventOxReward.REWARD_TYPE.Daily))
            {
                tempStringBuilder.Append(TextManager.Instance.GetText("ox_popup_desc_reward_today"));
                Legion.Instance.cEvent.u2OXlefttime = 9999;
            }
            else
            {
                if(Legion.Instance.cEvent.u1OXtotalReward == 0)
                {
                    tempStringBuilder.Append(String.Format(TextManager.Instance.GetText("ox_popup_desc_reward_day"), 5));
                    Legion.Instance.cEvent.u1OXtotalReward = 5;
                }
                else if(Legion.Instance.cEvent.u1OXanswer == EventOxReward.MAX_DAY)
                {
                    Legion.Instance.cEvent.u1OXtotalReward = EventOxReward.MAX_DAY;
                    //objPopupGroup.SetActive(true);
                    //objRewardPopup.SetActive(true);
                    //PopupManager.Instance.AddPopup(objRewardPopup, OnClickClose);
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("ox_popup_title_reward"), TextManager.Instance.GetText("ox_reward_character"), null);
                    //Btn_RewardGet.interactable = false;
                    RewardGetBtnEnable(false);
                }
                else
                {
                    Legion.Instance.cEvent.u1OXtotalReward += 5;
                    tempStringBuilder.Append(String.Format(TextManager.Instance.GetText("ox_popup_desc_reward_day"), Legion.Instance.cEvent.u1OXtotalReward));
                }
            }
            if(Legion.Instance.cEvent.u1OXanswer != EventOxReward.MAX_DAY)
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("ox_popup_title_reward"), tempStringBuilder.ToString(), null); 
            
            GetRewardImage();
        }
    }

    public void OnClickAnswer(bool _answer)
    {
        tempAnswer = _answer;
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestOxAnswer(Legion.Instance.cEvent.u1OXquestion, Convert.ToByte(_answer), AskAnswer);
    }

    private void AskAnswer(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EVENT_OX, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
            objPopupGroup.SetActive(true);
            if(Legion.Instance.cEvent.u1TodayOxDone == 1)
            {
                GetRewardImage();
                objCorrectPopup.SetActive(true);
                PopupManager.Instance.AddPopup(objCorrectPopup, OnClickClose);
                //Btn_Ox_O.interactable = false;
                //Btn_Ox_X.interactable = false;
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append(TextManager.Instance.GetText("ox_ui_question_right")).Append(TextManager.Instance.GetText("ox_ui_question_cha_msg"));
                txtQuestion.text = tempStringBuilder.ToString();
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append(Legion.Instance.cEvent.u1OXanswer).Append(TextManager.Instance.GetText("mark_time_day"));
                txtStackDay.text = tempStringBuilder.ToString();
            }
            else
            {
                objFailPopup.SetActive(true);
                tsLeftTime = TimeSpan.FromSeconds(Legion.Instance.cEvent.u2OXlefttime);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append(String.Format("{0:00}:{1:00}", tsLeftTime.Hours, tsLeftTime.Minutes));
                tempStringBuilder.Append(TextManager.Instance.GetText("ox_ui_question_wrong_time"));

                if(tempAnswer)
                {
                    Btn_Ox_O.transform.GetChild(0).gameObject.SetActive(true);
                    Btn_Ox_X.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    Btn_Ox_O.transform.GetChild(0).gameObject.SetActive(false);
                    Btn_Ox_X.transform.GetChild(0).gameObject.SetActive(true);
                }
                txtFailTime.text = tempStringBuilder.ToString();
            }
        }
    }

    private void RewardGetBtnEnable(bool isEnable)
    {
        Btn_RewardGet.interactable = isEnable;
        _effect_Btn.SetActive(isEnable);
    }
}
