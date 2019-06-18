using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class SocialNoticeSlot : MonoBehaviour
{
    public Text textDate;
    public Text textTitle;

    SocialNoticeTab _noticeTab;
    SocialNotice _noticeSlot;
    UInt16 _dicKey;
    DateTime recvTime;
    StringBuilder tempStringBuilder;

    public void Awake()
    {
        tempStringBuilder = new StringBuilder();
    }

    public void SetData(SocialNotice _slot, UInt16 _key, SocialNoticeTab _tab)
    {
        _noticeTab = _tab;
        _noticeSlot = _slot;
        _dicKey = _key;

        recvTime = _noticeSlot.dtShowDay;
        //TimeSpan timespan = new TimeSpan(timeTicks);
		textDate.text = string.Format(TextManager.Instance.GetText("mark_notice_time_day"), recvTime.ToString("yy"), recvTime.Month, recvTime.Day);
		textTitle.text = _noticeSlot.strTitle;
    }

    public void OnClickShowInfo()
    {
        for(int i=0; i<_noticeTab.ScrollList.transform.GetChildCount(); i++)
        {
            if(this.transform.GetSiblingIndex() != _noticeTab.ScrollList.transform.GetChild(i).GetSiblingIndex())
            {
                _noticeTab.ScrollList.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        _noticeTab.btnOk.gameObject.SetActive(true);
        _noticeTab.NoticeInfoGroup.SetActive(true);
		_noticeTab._noticeInfoText.text = _noticeSlot.strContent;
        this.GetComponent<Button>().interactable = false;
    }
}
