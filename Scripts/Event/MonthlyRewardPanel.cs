using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text;

public class MonthlyRewardPanel : MonoBehaviour
{
    public Image _titleImg;
    public GameObject GridMonth;
	public GameObject MonthSlotList;
    public GameObject GridWeek;
	public GameObject WeekSlotList;
    public GameObject TouchBlock;
    //public Text Title;
    //public Text RewardTitle;
    public int CurrentPage;
    public Toggle Tgl_Month;
    public Toggle Tgl_Week;
    //public Image WeeklyChar;
    public GameObject _scaleTweenObj;
    public GameObject BtnClose;

    public GameObject RewardPopup;
    public Text RewardPopupTitle;

    StringBuilder tempStringBuilder;
    UInt16 u2EventIDKey;
    UInt16 tempMonthlyKey = 0;
    UInt16 tempWeeklyKey = 0;

    public void Awake()
    {
        tempStringBuilder = new StringBuilder();
    }

    public void OnEnable()
    {
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        RewardPopup.SetActive(false);
        
        for(int i=0; i<EventInfoMgr.Instance.lstMonthlyEventID.Count; i++)
        {
            if(EventInfoMgr.Instance.dicEventReward.ContainsKey(EventInfoMgr.Instance.lstMonthlyEventID[i]))
            {
                tempMonthlyKey = EventInfoMgr.Instance.lstMonthlyEventID[i];
                break;
            }
        }
        
        for(int i=0; i<EventInfoMgr.Instance.dicEventReward.Count; i++)
        {
            if(EventInfoMgr.Instance.dicEventReward.ContainsKey(EventInfoMgr.Instance.lstWeeklyEventID[i]))
            {
                tempWeeklyKey = EventInfoMgr.Instance.lstWeeklyEventID[i];
                break;
            }
        }
        //임시 테스트용
        //EventReward tempEventReward = new EventReward();
        //tempEventReward = EventInfoMgr.Instance.dicEventReward[tempMonthlyKey];
        //EventInfoMgr.Instance.dicEventReward.Remove(tempMonthlyKey);
        //tempEventReward.u4RecordValue = 1;
        //EventInfoMgr.Instance.dicEventReward.Add(tempMonthlyKey, tempEventReward);

        //tempEventReward = EventInfoMgr.Instance.dicEventReward[tempWeeklyKey];
        //EventInfoMgr.Instance.dicEventReward.Remove(tempWeeklyKey);
        //tempEventReward.u4RecordValue = 1;
        //EventInfoMgr.Instance.dicEventReward.Add(tempWeeklyKey, tempEventReward);

        if(EventInfoMgr.Instance.dicEventReward[tempMonthlyKey].u4RecordValue != 0)
            Init(0);
        else if(EventInfoMgr.Instance.dicEventReward[tempWeeklyKey].u4RecordValue != 0)
        {
            recvState = 1;
            Init(1);
        }
        else
            Init(0);
        
        //Tgl_Month.gameObject.SetActive(false);
        //Tgl_Week.gameObject.SetActive(true);
        _scaleTweenObj.GetComponent<RectTransform>().localScale = new Vector3(0f, 0f, 1f);
        PopupManager.Instance.AddPopup(this.gameObject, OnClickClose);
        //LeanTween.scale(this.GetComponent<RectTransform>(), Vector2.one*1.2f, 0.1f).setOnComplete((LeanTween.scale(this.GetComponent<RectTransform>(), Vector2.one, 0.1f).setDelay(0.1f).onCompleteObject));
        StartCoroutine(RewardPopupAni2());
    }
    IEnumerator RewardPopupAni2()
    {
        yield return new WaitForEndOfFrame();
        if(Legion.Instance.u1RecvLoginReward == 1)
        {
            TouchBlock.gameObject.SetActive(true);
            if(u2EventIDKey != 0)
            {
                if(EventInfoMgr.Instance.dicEventReward.ContainsKey(tempMonthlyKey) &&
                   EventInfoMgr.Instance.dicEventReward[tempMonthlyKey].u4RecordValue != 0)
                {
                    BtnClose.SetActive(false);
                    yield return new WaitForSeconds(1.5f);
                }
                else if(EventInfoMgr.Instance.dicEventReward.ContainsKey(tempWeeklyKey) &&
                    EventInfoMgr.Instance.dicEventReward[tempWeeklyKey].u4RecordValue != 0)
                {
                    BtnClose.SetActive(false);
                }
                //if(EventInfoMgr.Instance.dicEventReward[u2EventIDKey].u4RecordValue != 0)
                //{
                //    BtnClose.SetActive(false);
                //    //yield return new WaitForSeconds(1.5f);
                //}
                else
                {
                    Legion.Instance.u1RecvLoginReward = 0;
                    BtnClose.SetActive(true);
                    TouchBlock.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            Legion.Instance.u1RecvLoginReward = 0;
            BtnClose.SetActive(true);
            TouchBlock.gameObject.SetActive(false);
        }
        LeanTween.scale(_scaleTweenObj.GetComponent<RectTransform>(), Vector2.one*1.2f, 0.1f).setOnComplete((LeanTween.scale(_scaleTweenObj.GetComponent<RectTransform>(), Vector2.one, 0.1f).setDelay(0.1f).onCompleteObject));

        yield return new WaitForSeconds(2f);

        if(u2EventIDKey != 0)
        {
            if(EventInfoMgr.Instance.dicEventReward.ContainsKey(u2EventIDKey) && 
                EventInfoMgr.Instance.dicEventReward[u2EventIDKey].u4RecordValue != 0)
            {
                if(Legion.Instance.u1RecvLoginReward == 1)
                {
                    RewardPopup.SetActive(true);
                    //LeanTween.scale(RewardPopup.GetComponent<RectTransform>(), Vector3.one*1.2f, 0.1f).setDelay(0.1f).setOnComplete((LeanTween.scale(RewardPopup.GetComponent<RectTransform>(), Vector3.one, 0.1f).setDelay(0.1f).onCompleteObject));
                    //Legion.Instance.u1RecvLoginReward = 0;
                    RecvRewardPopupOpen();
                    yield return new WaitForSeconds(2f);
                    if (RewardPopup.activeSelf == true)
                        OnClickOk();
                }
            }
        }
    }

    IEnumerator RewardPopupAni()
    {
        //this.transform.localScale = Vector3.zero;
        LeanTween.scale(_scaleTweenObj.GetComponent<RectTransform>(), Vector3.zero, 0.1f).setOnComplete(
        LeanTween.scale(_scaleTweenObj.GetComponent<RectTransform>(), Vector2.one*1.2f, 0.1f).setDelay(0.1f).setOnComplete((LeanTween.scale(_scaleTweenObj.GetComponent<RectTransform>(), Vector2.one, 0.1f).setDelay(0.1f).onCompleteObject)).onComplete);
        if(Legion.Instance.u1RecvLoginReward == 1)
            TouchBlock.gameObject.SetActive(true);
        else
            TouchBlock.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(2f);
        if(u2EventIDKey != 0)
        {
            if(EventInfoMgr.Instance.dicEventReward[u2EventIDKey].u4RecordValue != 0)
            {
                if(Legion.Instance.u1RecvLoginReward == 1)
                {
                    RewardPopup.SetActive(true);
                    //LeanTween.scale(RewardPopup.GetComponent<RectTransform>(), Vector3.one*1.2f, 0.1f).setDelay(0.1f).setOnComplete((LeanTween.scale(RewardPopup.GetComponent<RectTransform>(), Vector3.one, 0.1f).setDelay(0.1f).onCompleteObject));
                    //Legion.Instance.u1RecvLoginReward = 0;
                    RecvRewardPopupOpen();
                    yield return new WaitForSeconds(2f);
                    if (RewardPopup.activeSelf == true)
                        OnClickOk();
                }
            }
        }
    }
    int dayIndex;
    public void Init(int category)
    {
        bool bCheck = false;
        dayIndex = 0;
        CurrentPage = category;

        SetTitleImg(CurrentPage);
        if (CurrentPage == 0)
        {
            //Title.text = TextManager.Instance.GetText("popup_title_month");
            GridMonth.SetActive(true);
            GridWeek.SetActive(false);
            //WeeklyChar.gameObject.SetActive(false);

            for(int i=0; i<EventInfoMgr.Instance.lstMonthlyEventID.Count; i++)
            {
                if(EventInfoMgr.Instance.dicEventReward.ContainsKey(EventInfoMgr.Instance.lstMonthlyEventID[i]))
                {
                    dayIndex = EventInfoMgr.Instance.dicEventReward[EventInfoMgr.Instance.lstMonthlyEventID[i]].u1RewardIndex;;
                    u2EventIDKey = EventInfoMgr.Instance.lstMonthlyEventID[i];
                    break;
                }
            }

            if(Legion.Instance.u1VIPLevel == 0)
                dayIndex = 0;

			for(int i=0; i<MonthSlotList.transform.childCount; i++)
            {
				if( (i+1) <= dayIndex)
                    bCheck = true;
                else
                    bCheck = false;
                if(u2EventIDKey != 0)
                {
                    if(EventInfoMgr.Instance.dicEventReward[u2EventIDKey].u4RecordValue != 0)
						MonthSlotList.transform.GetChild(i).GetComponent<MonthlyRewardSlot>().SetData(EventInfoMgr.Instance.dicDailyCheckReward[(Byte)DailyCheckReward.EventType.Monthly, (i+1)][u2EventIDKey], bCheck, dayIndex);
                    else
						MonthSlotList.transform.GetChild(i).GetComponent<MonthlyRewardSlot>().SetData(EventInfoMgr.Instance.dicDailyCheckReward[(Byte)DailyCheckReward.EventType.Monthly, (i+1)][u2EventIDKey], bCheck, 0);
                }
                else
					MonthSlotList.transform.GetChild(i).GetComponent<MonthlyRewardSlot>().SetData(EventInfoMgr.Instance.dicDailyCheckReward[(Byte)DailyCheckReward.EventType.Monthly, (i+1)][u2EventIDKey], bCheck, 0);
            }
        }
        else if(CurrentPage == 1)
        {
            //Title.text = TextManager.Instance.GetText("popup_title_week");
            GridMonth.SetActive(false);
            GridWeek.SetActive(true);
            //WeeklyChar.gameObject.SetActive(true);

            for(int i=0; i<EventInfoMgr.Instance.dicEventReward.Count; i++)
            {
                if(EventInfoMgr.Instance.dicEventReward.ContainsKey(EventInfoMgr.Instance.lstWeeklyEventID[i]))
                {
                    dayIndex = EventInfoMgr.Instance.dicEventReward[EventInfoMgr.Instance.lstWeeklyEventID[i]].u1RewardIndex;
                    u2EventIDKey = EventInfoMgr.Instance.lstWeeklyEventID[i];
                    break;
                }
                else
                {
                    dayIndex = 0;
                    u2EventIDKey = 0;
                }
            }

            if(Legion.Instance.u1VIPLevel == 0)
                dayIndex = 0;

			for(int i=0; i< WeekSlotList.transform.childCount; i++)
            {
				if((i+1) <=dayIndex)
                    bCheck = true;
                else
                    bCheck = false;
                if(u2EventIDKey != 0)
                {
                    if(EventInfoMgr.Instance.dicEventReward[u2EventIDKey].u4RecordValue != 0)
						WeekSlotList.transform.GetChild(i).GetComponent<WeeklyRewardSlot>().SetData(EventInfoMgr.Instance.dicDailyCheckReward[(Byte)DailyCheckReward.EventType.Weekly, (i+1)][u2EventIDKey], bCheck, dayIndex, i+1);
                    else
						WeekSlotList.transform.GetChild(i).GetComponent<WeeklyRewardSlot>().SetData(EventInfoMgr.Instance.dicDailyCheckReward[(Byte)DailyCheckReward.EventType.Weekly, (i+1)][u2EventIDKey], bCheck, 0, i+1);
                }
                else
					WeekSlotList.transform.GetChild(i).GetComponent<WeeklyRewardSlot>().SetData(EventInfoMgr.Instance.dicDailyCheckReward[(Byte)DailyCheckReward.EventType.Weekly, (i+1)][u2EventIDKey], bCheck, 0, i+1);
            }
        }
    }

    public void OnClickClose()
    {
        Legion.Instance.cTutorial.CheckTutorial (MENU.MAIN);

        PopupManager.Instance.RemovePopup(this.gameObject);
		StartCoroutine(ClosePanelAni());
    }

    IEnumerator ClosePanelAni()
    {
        LeanTween.scale(_scaleTweenObj.GetComponent<RectTransform>(), Vector3.zero, 0.1f);
        yield return new WaitForSeconds(0.1f);

        //Legion.Instance.ShowCafeOnce ();
        if(Legion.Instance.IsLoginPopupStepTpye(Legion.LoginPopupStep.LOGIN_REWARED) == true)
            Legion.Instance.SubLoginPopupStep(Legion.LoginPopupStep.LOGIN_REWARED);

        Destroy(this.gameObject);
    }

    public void OnClickVIPReward()
    {
        Init(CurrentPage);
    }

    public void OnClickMonthReward()
    {
        if(!Tgl_Week.isOn)
            return;

        CurrentPage = 1;
        OnClickVIPReward();
        StartCoroutine(RewardPopupAni());
        //Tgl_Month.gameObject.SetActive(false);
        //Tgl_Week.gameObject.SetActive(true);
    }

    public void OnClickWeekReward()
    {
        if(!Tgl_Month.isOn)
            return;

        CurrentPage = 0;
        OnClickVIPReward();
        StartCoroutine(RewardPopupAni());
        //Tgl_Month.gameObject.SetActive(true);
        //Tgl_Week.gameObject.SetActive(false);
    }

    public void RecvRewardPopupOpen()
    {
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		if(recvState == 1)
			tempStringBuilder.Append(TextManager.Instance.GetText("popup_title_week"/*"popup_title_month"*/)).Append(" ").Append(dayIndex).Append(TextManager.Instance.GetText("popup_title_reward_day"));
		else
			tempStringBuilder.Append(TextManager.Instance.GetText("popup_title_month")).Append(" ").Append(dayIndex).Append(TextManager.Instance.GetText("popup_title_reward_day"));
        RewardPopupTitle.text = tempStringBuilder.ToString();
    }

    int recvState = 0;
    public void OnClickOk()
    {
        if (RewardPopup.activeSelf == false)
            return;

        if (recvState == 0)
		{
	        UInt16 u2WeeklyID = 0;
	        for(int i=0; i<EventInfoMgr.Instance.lstWeeklyEventID.Count; i++)
	        {
	            if(EventInfoMgr.Instance.dicEventReward.ContainsKey(EventInfoMgr.Instance.lstWeeklyEventID[i]))
	            {
	                if(EventInfoMgr.Instance.dicEventReward[EventInfoMgr.Instance.lstWeeklyEventID[i]].u4RecordValue != 0)
	                {
	                    u2WeeklyID = EventInfoMgr.Instance.lstWeeklyEventID[i];
						recvState = 0;
	                    break;
	                }
		            else
		            {
		                recvState = 1;
		            }
		        }
			}
		}
        //if(u2WeeklyID == 0)
        //    RewardPopup.SetActive(false);
        if(recvState == 0)
        {
            EventReward tempEventReward = new EventReward();
            if(EventInfoMgr.Instance.dicEventReward.ContainsKey(tempMonthlyKey))
            {
                tempEventReward = EventInfoMgr.Instance.dicEventReward[tempMonthlyKey];
                EventInfoMgr.Instance.dicEventReward.Remove(tempMonthlyKey);
            }
            tempEventReward.u4RecordValue = 0;
            EventInfoMgr.Instance.dicEventReward.Add(tempMonthlyKey, tempEventReward);
            recvState = 1;
            //OnClickWeekReward();
            Tgl_Week.isOn = true;
            StartCoroutine(RewardPopupAni());
            RewardPopup.SetActive(false);
        }
        else
        {
            TouchBlock.gameObject.SetActive(false);
            Legion.Instance.u1RecvLoginReward = 0;
            EventReward tempEventReward = new EventReward();
            if (EventInfoMgr.Instance.dicEventReward.ContainsKey(tempMonthlyKey))
            {
                tempEventReward = EventInfoMgr.Instance.dicEventReward[tempWeeklyKey];
                EventInfoMgr.Instance.dicEventReward.Remove(tempWeeklyKey);
            }
            tempEventReward.u4RecordValue = 0;
            EventInfoMgr.Instance.dicEventReward.Add(tempWeeklyKey, tempEventReward);
            
            RewardPopup.SetActive(false);
            OnClickClose();
        }
    }

    private void SetTitleImg(int category)
    {
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append("Sprites/").Append(TextManager.Instance.GetImagePath()).Append(".LoginReward_Title_").Append(category);
        _titleImg.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.ToString());
        _titleImg.SetNativeSize();
    }
}