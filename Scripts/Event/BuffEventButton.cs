using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

public class BuffEventButton : MonoBehaviour
{
    String ExpTime;
    String GoldTime;

    String DayHour;
    String HourMinutes;
    String MinutesSeconds;
    String Seconds;

    TimeSpan tsLeftTime;
    StringBuilder tempStringBuilder;
    UInt16 _eventID;
    bool bTimeOut = false;
    int _type = 0;
    public void OnEnable()
    {
        tempStringBuilder = new StringBuilder();
        tempStringBuilder.Append("{0}").Append(TextManager.Instance.GetText("mark_time_day")).Append("{1}").Append(TextManager.Instance.GetText("mark_time_hour"));
        DayHour = tempStringBuilder.ToString();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append("{0}").Append(TextManager.Instance.GetText("mark_time_hour")).Append("{1}").Append(TextManager.Instance.GetText("mark_time_min"));
        HourMinutes = tempStringBuilder.ToString();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append("{0}").Append(TextManager.Instance.GetText("mark_time_min")).Append("{1}").Append(TextManager.Instance.GetText("mark_title_second"));
        MinutesSeconds = tempStringBuilder.ToString();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append("{0}").Append(TextManager.Instance.GetText("mark_title_second"));
        Seconds = tempStringBuilder.ToString();
    }

    public void OnClickBuff(int type)
    {
        _type = type;
        CheckUnlockBoostEvent(type);
        switch(type)
        {
            case (int)EVENT_TYPE.BUFF_EXP:
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                if(bTimeOut)
                {
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("event_exp_add_title"), "이벤트가 종료되었습니다", ReloadEvent);
                }
                else
                {
                    tempStringBuilder.Append(TextManager.Instance.GetText("event_exp_add_desc"));
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("event_exp_add_title"), 
                        string.Format(tempStringBuilder.ToString(), EventInfoMgr.Instance.u4ExpBoostPer, ExpTime), null);
                }
                break;

            case (int)EVENT_TYPE.BUFF_GOLD:
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                if(bTimeOut)
                {
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("event_exp_add_title"), TextManager.Instance.GetText("event_end_time"), ReloadEvent);
                }
                else
                {
                    tempStringBuilder.Append(TextManager.Instance.GetText("event_gold_add_desc"));
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("event_gold_add_title"), 
                        string.Format(tempStringBuilder.ToString(), EventInfoMgr.Instance.u4GoldBoostPer, GoldTime), null);
                }
                break;
        }
    }

    void ReloadEvent(object[] param)
    {
        EventInfoMgr.Instance.dicEventReward.Remove(_eventID);
        switch(_type)
        {
            case (int)EVENT_TYPE.BUFF_EXP:
                EventInfoMgr.Instance.lstExpBuffEvent.Remove(EventInfoMgr.Instance.lstExpBuffEvent[0]);
                break;

            case (int)EVENT_TYPE.BUFF_GOLD:
                EventInfoMgr.Instance.lstGoldBuffEvent.Remove(EventInfoMgr.Instance.lstGoldBuffEvent[0]);
                break;
        }
        bTimeOut = false;
        this.gameObject.SetActive(false);
        //PopupManager.Instance.ShowLoadingPopup(1);
        //Server.ServerMgr.Instance.RequestEventReload(EventReloadResult);
    }

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
            bTimeOut = false;
            this.gameObject.SetActive(false);
        }
    }

    public void CheckUnlockBoostEvent(int type)
    {
        if(type == (int)EVENT_TYPE.BUFF_EXP)
        {
            EventReward tempItem = EventInfoMgr.Instance.lstExpBuffEvent[0];

            tsLeftTime = tempItem.dtEventEnd - Legion.Instance.ServerTime;
            UInt16 _eventID = tempItem.u2EventID;

            if(tsLeftTime.TotalSeconds < 0)
            {
                EventInfoMgr.Instance.u4ExpBoostPer = 0;
                bTimeOut = true;
            }
            else
            {
                EventInfoMgr.Instance.u4ExpBoostPer = tempItem.recordValue;
            }
            if(tsLeftTime.TotalHours > 24)
                ExpTime = String.Format(DayHour, tsLeftTime.Days, tsLeftTime.Hours);
            else if(tsLeftTime.TotalMinutes > 60)
                ExpTime = String.Format(HourMinutes, tsLeftTime.Hours, tsLeftTime.Minutes);
            else if(tsLeftTime.TotalSeconds > 60)
                ExpTime = String.Format(MinutesSeconds, tsLeftTime.Minutes, tsLeftTime.Seconds);
            else
                ExpTime = String.Format(Seconds, tsLeftTime.Seconds);

            //foreach (KeyValuePair<UInt16, EventReward> item in EventInfoMgr.Instance.dicEventReward)
            //{
            //    if(item.Value.eventType == (Byte)EVENT_TYPE.BUFF_EXP)
            //    {
            //        tsLeftTime = item.Value.dtEventEnd - Legion.Instance.ServerTime;
            //        _eventID = item.Key;
            //        if(tsLeftTime.TotalSeconds < 0)
            //        {
            //            EventInfoMgr.Instance.u4ExpBoostPer = 0;
            //            bTimeOut = true;
            //            break;
            //        }
            //        //ExpTime = item.Value.dtEventEnd.ToString();
            //        if(tsLeftTime.TotalHours > 24)
            //            ExpTime = String.Format(DayHour, tsLeftTime.Days, tsLeftTime.Hours);
            //        else if(tsLeftTime.TotalMinutes > 60)
            //            ExpTime = String.Format(HourMinutes, tsLeftTime.Hours, tsLeftTime.Minutes);
            //        else if(tsLeftTime.TotalSeconds > 60)
            //            ExpTime = String.Format(MinutesSeconds, tsLeftTime.Minutes, tsLeftTime.Seconds);
            //        else
            //            ExpTime = String.Format(Seconds, tsLeftTime.Seconds);
            //        
            //        //break;
            //    }
            //}
        }
        else if(type == (int)EVENT_TYPE.BUFF_GOLD)
        {
            EventReward tempItem = EventInfoMgr.Instance.lstGoldBuffEvent[0];

            tsLeftTime = tempItem.dtEventEnd - Legion.Instance.ServerTime;
            UInt16 _eventID = tempItem.u2EventID;

            if(tsLeftTime.TotalSeconds < 0)
            {
                EventInfoMgr.Instance.u4GoldBoostPer = 0;
                bTimeOut = true;
            }
            else
            {
                EventInfoMgr.Instance.u4GoldBoostPer = tempItem.recordValue;
            }
            if(tsLeftTime.TotalHours > 24)
                GoldTime = String.Format(DayHour, tsLeftTime.Days, tsLeftTime.Hours);
            else if(tsLeftTime.TotalMinutes > 60)
                GoldTime = String.Format(HourMinutes, tsLeftTime.Hours, tsLeftTime.Minutes);
            else if(tsLeftTime.TotalSeconds > 60)
                GoldTime = String.Format(MinutesSeconds, tsLeftTime.Minutes, tsLeftTime.Seconds);
            else
                GoldTime = String.Format(Seconds, tsLeftTime.Seconds);
            //foreach (KeyValuePair<UInt16, EventReward> item in EventInfoMgr.Instance.dicEventReward)
            //{
            //    if(item.Value.eventType == (Byte)EVENT_TYPE.BUFF_GOLD)
            //    {
            //        tsLeftTime = item.Value.dtEventEnd - Legion.Instance.ServerTime;
            //        _eventID = item.Key;
            //        if(tsLeftTime.TotalSeconds < 0)
            //        {
            //            EventInfoMgr.Instance.u4GoldBoostPer = 0;
            //            bTimeOut = true;
            //            break;
            //        }
            //        //GoldTime = item.Value.dtEventEnd.ToString();
            //        if(tsLeftTime.TotalHours > 24)
            //            GoldTime = String.Format(DayHour, tsLeftTime.Days, tsLeftTime.Hours);
            //        else if(tsLeftTime.TotalMinutes > 60)
            //            GoldTime = String.Format(HourMinutes, tsLeftTime.Hours, tsLeftTime.Minutes);
            //        else if(tsLeftTime.TotalSeconds > 60)
            //            GoldTime = String.Format(MinutesSeconds, tsLeftTime.Minutes, tsLeftTime.Seconds);
            //        else
            //            GoldTime = String.Format(Seconds, tsLeftTime.Seconds);
            //        //break;
            //    }
            //}
        }

        //foreach (KeyValuePair<UInt16, EventReward> item in EventInfoMgr.Instance.dicEventReward)
        //{
        //    if(item.Value.eventType == (Byte)EVENT_TYPE.BUFF_GOLD)
        //    {
        //        tsLeftTime = item.Value.dtEventEnd - Legion.Instance.ServerTime;
        //        if(tsLeftTime.TotalSeconds < 0)
        //        {
        //            bTimeOut = true;
        //            break;
        //        }
        //        //GoldTime = item.Value.dtEventEnd.ToString();
        //        if(tsLeftTime.TotalHours > 24)
        //            GoldTime = String.Format(DayHour, tsLeftTime.Days, tsLeftTime.Hours);
        //        else if(tsLeftTime.TotalMinutes > 60)
        //            GoldTime = String.Format(HourMinutes, tsLeftTime.Hours, tsLeftTime.Minutes);
        //        else if(tsLeftTime.TotalSeconds > 3600)
        //            GoldTime = String.Format(MinutesSeconds, tsLeftTime.Minutes, tsLeftTime.Seconds);
        //        else
        //            GoldTime = String.Format(Seconds, tsLeftTime.Seconds);
        //        break;
        //    }
        //}
    }
}
