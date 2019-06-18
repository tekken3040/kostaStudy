using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class NoticePopup : MonoBehaviour 
{
	public Text _txtNoticeTitle;
	public RectTransform _slotParent;
	public GameObject _noticeSlotPref;
	public GameObject _objBackBtn;
	public Text _txtNoticeContent;

	public void OnEnable()
	{
		_txtNoticeTitle.text = TextManager.Instance.GetText("btn_social_notice");
        _txtNoticeContent.gameObject.SetActive(false);
        SetNoticeList();
		StartCoroutine("ScaleAnimation");
	}

	public void OnClickBackBtn()
	{
		_slotParent.gameObject.SetActive(true);
		_txtNoticeTitle.text = TextManager.Instance.GetText("btn_social_notice");
		_txtNoticeContent.gameObject.SetActive(false);
		_objBackBtn.SetActive(false);
	}

	public void SetNoticeList()
	{
		for(int i = 0; i < SocialInfo.Instance.u1NoticeCount; ++i)
		{
			GameObject objNoticePref = Instantiate(_noticeSlotPref);
			objNoticePref.transform.SetParent(_slotParent);
			objNoticePref.transform.localPosition = Vector3.zero;
			objNoticePref.transform.localScale = Vector3.one;
			objNoticePref.transform.SetAsFirstSibling();
			objNoticePref.GetComponent<NoticeSlot>().SetNoticeData(SocialInfo.Instance.dicNotice[(UInt16)i], this);
		}
	}

	public void SetClickSlotInfo(SocialNotice NoticeInfo)
	{
		_slotParent.gameObject.SetActive(false);
		_txtNoticeTitle.text = NoticeInfo.strTitle;
		_txtNoticeContent.text = NoticeInfo.strContent;
		_txtNoticeContent.gameObject.SetActive(true);
		_objBackBtn.SetActive(true);
	}

	IEnumerator ScaleAnimation()
	{
		LeanTween.scale(this.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.1f);
		yield return new WaitForSeconds(0.1f);
		LeanTween.scale(this.gameObject, Vector3.one, 0.1f);
	}
}