using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class EventNoticePopup : MonoBehaviour
{
    public EventNoticeButtonList[] _acEventBtnPrs;
    public Image _imgEventNoticeBG;
    public Button _btnClose;
    public GameObject _objCheckBox;
    public GameObject _objLoadFailImg;
    public GameObject _objLoadingImg;

    private List<SocialNoticeEvent> noticeEventList;
    private int _nCutNoticeIdx;

    private void Awake()
    {
        // 인앱 초기화
#if UNITY_EDITOR
#elif UNITY_ONESTORE
        ShopInfoMgr.Instance.SettingInAppOneStore();
#else
        ShopInfoMgr.Instance.SettingInApp();
#endif
    }
    //void OnEnable()
    //{
    //    OpenPopup();
    //}

    public void OpenPopup()
    {
        noticeEventList = SocialInfo.Instance.lstNoticeEventList;

        if (noticeEventList == null)
        {
            PopupClose();
            return;
        }

        for(_nCutNoticeIdx = 0; _nCutNoticeIdx < noticeEventList.Count; ++_nCutNoticeIdx)
        {
            if (EventInfoMgr.Instance.CheckADValue(noticeEventList[_nCutNoticeIdx].u2EventPopupSN) == false)
                break;
        }

        if (noticeEventList.Count > _nCutNoticeIdx)
            SetEventInfo();
        else
            PopupClose();
    }

    private void SetEventInfo()
    {
        for (int i = 0; i < _acEventBtnPrs.Length; ++i)
            _acEventBtnPrs[i].gameObject.SetActive(false);

        SetEventNoticeBG();
    }

    public void OnClickCheckBox()
    {
        EventInfoMgr.Instance.AddADMark(noticeEventList[_nCutNoticeIdx].u2EventPopupSN);

        OnClickClosePopup();
    }

    // 이벤트 이미즈를 셋팅을 시작한다
    public void SetEventNoticeBG()
    {
		if (_nCutNoticeIdx >= noticeEventList.Count) {
			OnClickClosePopup ();
		}

        _objLoadingImg.SetActive(true);
        _btnClose.interactable = false;
        StartCoroutine("EventBGDowonload");
    }

    // 이벤트 이미지를 셋팅한다
    private IEnumerator EventBGDowonload()
    {
        LeanTween.scale(_imgEventNoticeBG.gameObject, Vector3.one, 0.1f);

        StringBuilder tempString = new StringBuilder();
		tempString.Append(PopupManager.Instance.GetImageURL()).Append("AD/").Append(TextManager.Instance.eLanguage.ToString());
        tempString.Append("/").Append(noticeEventList[_nCutNoticeIdx].strImageURL);

        WWW www = new WWW(tempString.ToString());
        yield return www;

		if (_nCutNoticeIdx >= noticeEventList.Count) {
			OnClickClosePopup ();
			yield break;
		}

        // 로딩 이미지 숨김
        _objLoadingImg.SetActive(false);
        if (string.IsNullOrEmpty(www.error))
        {
            _imgEventNoticeBG.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            _imgEventNoticeBG.SetNativeSize();
            if (noticeEventList[_nCutNoticeIdx].u1ButtonCount > 0)
            {
                int btnCount = noticeEventList[_nCutNoticeIdx].u1ButtonCount - 1;
                NoticeEventInfo[] info = noticeEventList[_nCutNoticeIdx].arrsNoticeEventInfo;
                _acEventBtnPrs[btnCount].gameObject.SetActive(true);
                _acEventBtnPrs[btnCount].SetBtnInfo(info);
            }
            _objLoadFailImg.SetActive(false);
        }
        else
        {
            // 이미지 로드 실패시 별도 이미지 넣기
            _objLoadFailImg.SetActive(true);
        }

        _btnClose.interactable = true;
    }

    // 다음 이벤트 정보가 있는지 확인한다
    private bool CheckNextEventInfo()
    {
        // 이벤트가 갯수가 현재 인덱스보다 크면
        if (noticeEventList.Count > ++_nCutNoticeIdx)
        {
            for (; _nCutNoticeIdx < noticeEventList.Count; ++_nCutNoticeIdx)
            {
                if (EventInfoMgr.Instance.CheckADValue(noticeEventList[_nCutNoticeIdx].u2EventPopupSN) == false)
                    return true;
            }
        }
        return false;
    }

    // 팝업을 닫는다
    public void OnClickClosePopup()
    {
        if (CheckNextEventInfo() == true)
            StartCoroutine("ScaleEffect");
        else
            PopupClose();
    }

    public void PopupClose()
    {
        LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
        if(lobbyScene != null)
        {
            if( lobbyScene.IsMenuHide == true)
                lobbyScene.HideMenu();
        }
        Legion.Instance.SubLoginPopupStep(Legion.LoginPopupStep.EVENT_NOTICE);
        Destroy(this.gameObject);
    }

    private IEnumerator ScaleEffect()
    {
        LeanTween.scale(_imgEventNoticeBG.gameObject, Vector3.zero, 0.1f);
        yield return new WaitForSeconds(0.15f);
        SetEventInfo();
    }
}
