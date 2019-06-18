using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class NoticeSlot : MonoBehaviour 
{
	public Text	_txtTitle;
	public Text _txtDate;
	protected NoticePopup _cNoticePopup;
	protected SocialNotice _cNoticeInfo;

	public void SetNoticeData(SocialNotice noticeInfo, NoticePopup parentClass)
	{
		_cNoticeInfo = noticeInfo;
		_cNoticePopup = parentClass;
		_txtTitle.text = _cNoticeInfo.strTitle;
		_txtDate.text = string.Format(TextManager.Instance.GetText("mark_notice_time_day"), _cNoticeInfo.dtShowDay.ToString("yyyy"), _cNoticeInfo.dtShowDay.Month, _cNoticeInfo.dtShowDay.Day);
	}

	public void OnClickNotice()
	{
		_cNoticePopup.SetClickSlotInfo(_cNoticeInfo);
	}
}
