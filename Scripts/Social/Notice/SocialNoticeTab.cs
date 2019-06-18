using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SocialNoticeTab : MonoBehaviour
{
    public SocialPanel _socialPanel;
    public GameObject objPrefNoticeList;
    public GameObject ScrollList;
    public GameObject NoticeInfoGroup;
    public Toggle[] _mailMenu;
    public Button btnOk;
    public Text _noticeInfoText;
    public SocialNoticeTab _noticeTab;
    public Text EmptyNotice;
    public Text EmptyEvent;

    public void OnEnable()
    {
        _mailMenu[0].isOn = true;
        if(!_socialPanel._recvNoticeList)
        {
            RequestNoticeList();
            _socialPanel._recvNoticeList = true;
        }

        else
        {
            ReciveNoticeList(Server.ERROR_ID.NONE);
        }
    }

    public void RequestNoticeList()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestGetNotice(ReciveNoticeList);
    }

    public void ReciveNoticeList(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.SOCIAL_GET_NOTICE, err), Server.ServerMgr.Instance.CallClear);
		}
        else
        {
            if(SocialInfo.Instance.u1NoticeCount == 0)
                EmptyNotice.gameObject.SetActive(true);
            else
                EmptyNotice.gameObject.SetActive(false);
            _mailMenu[0].GetComponent<Toggle>().isOn = true;
            _mailMenu[1].GetComponent<Toggle>().isOn = false;
            OnClickNoticeMenu(0);
        }
    }

    public void ClearList()
    {
        for(int i=0; i<ScrollList.transform.GetChildCount()-1; i++)
        {
            GameObject.Destroy(ScrollList.transform.GetChild(i).gameObject);
        }
    }

    public void OnClickNoticeMenu(int _menu)
    {
        ClearList();

        if(!_mailMenu[_menu].isOn)
            return;
        switch(_menu)
        {
            case 0:
                _mailMenu[0].GetComponent<Toggle>().isOn = true;
                _mailMenu[1].GetComponent<Toggle>().isOn = false;
                EmptyEvent.gameObject.SetActive(false);
                InitNoticeList();
                break;

            case 1:
                _mailMenu[0].GetComponent<Toggle>().isOn = false;
                _mailMenu[1].GetComponent<Toggle>().isOn = true;
                EmptyEvent.gameObject.SetActive(true);
                break;
        }
    }

    public void OnClickBtnOK()
    {
        for(int i=0; i<ScrollList.transform.GetChildCount()-1; i++)
        {
            ScrollList.transform.GetChild(i).gameObject.SetActive(true);
            ScrollList.transform.GetChild(i).GetComponent<Button>().interactable = true;
        }
        ScrollList.transform.GetChild(ScrollList.transform.GetChildCount()-1).gameObject.SetActive(false);
        btnOk.gameObject.SetActive(false);
        NoticeInfoGroup.SetActive(false);
    }

    public void InitNoticeList()
    {
        for(int i=0; i<SocialInfo.Instance.u1NoticeCount; i++)
        {
            GameObject objNoticelistPref = Instantiate(objPrefNoticeList);
            objNoticelistPref.transform.parent = ScrollList.transform;
            objNoticelistPref.transform.localPosition = Vector3.zero;
            objNoticelistPref.transform.localScale = Vector3.one;
            objNoticelistPref.GetComponent<SocialNoticeSlot>().SetData(SocialInfo.Instance.dicNotice[(UInt16)i], (UInt16)i, _noticeTab);
			objNoticelistPref.transform.SetAsFirstSibling();
        }
		NoticeInfoGroup.transform.SetAsLastSibling();
	}
}
