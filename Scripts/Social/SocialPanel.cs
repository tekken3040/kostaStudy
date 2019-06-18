using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class SocialPanel : MonoBehaviour
{
    //public GameObject _mainPanel;
    public GameObject objSocialList;
    public GameObject objSocialMenu;
    public GameObject _friendTab;
    public GameObject _mailTab;
    public GameObject _noticeTab;
    public GameObject[] Alrams;             //0:친구목록, 1:우편목록, 2:공지목록, 3:친구메뉴, 4:우편메뉴, 5:공지메뉴
    public Toggle[] _toggle;
    public bool _recvFriendList;
    public bool _recvMailList;
    public bool _recvNoticeList;

    public void OnEnable()
    {
		FadeEffectMgr.Instance.FadeIn(FadeEffectMgr.GLOBAL_FADE_TIME);
		PopupManager.Instance.AddPopup(this.gameObject, OnClickBack);
        StartCoroutine(CheckNew());
		/*
		// 2016. 12. 26 jy
		// 소셜 메뉴 리스트가 보이지 않도록 수정함에 따라 주석
        _recvFriendList = false;
        _recvMailList = false;
        _recvNoticeList = false;

		_friendTab.SetActive(false);
		_mailTab.SetActive(false);
		_noticeTab.SetActive(false);

        FadeEffectMgr.Instance.FadeIn(FadeEffectMgr.GLOBAL_FADE_TIME);
        objSocialList.SetActive(true);
        objSocialMenu.SetActive(false);
		
        PopupManager.Instance.AddPopup(this.gameObject, OnClickBack);
		       
		if(Legion.Instance.u1FriendExist != 0)
		{
			Alrams[0].SetActive(true);
			Alrams[3].SetActive(true);
		}
		else
		{
			Alrams[0].SetActive(false);
			Alrams[3].SetActive(false);
		}
		if(Legion.Instance.u1MailExist == 1)
		{
			Alrams[1].SetActive(true);
			Alrams[4].SetActive(true);
		}
		else
		{
			Alrams[1].SetActive(false);
			Alrams[4].SetActive(false);
		}
		*/
    }

    public void OnClickBack()
    {
        PopupManager.Instance.RemovePopup(this.gameObject);
        StartCoroutine(OpenMainPanel());
    }

    IEnumerator OpenMainPanel()
    {
        _noticeTab.GetComponent<SocialNoticeTab>().OnClickBtnOK();
        _friendTab.SetActive(false);
        _mailTab.SetActive(false);
        _noticeTab.SetActive(false);
        FadeEffectMgr.Instance.FadeOut(FadeEffectMgr.GLOBAL_FADE_TIME);
        yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        Scene.GetCurrent().RefreshAlram();
        
		LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
		if(lobbyScene != null)
		{
			lobbyScene.SetMenuHideButtonEnable(true); //mainPanel.GetComponent<Toggle>().interactable = true;
		}

        SocialInfo.Instance.ClearSocialInfo();
        //this.gameObject.SetActive(false);
        Destroy(this.gameObject);
        FadeEffectMgr.Instance.FadeIn(FadeEffectMgr.GLOBAL_FADE_TIME);
    }

    IEnumerator CheckNew()
    {
        yield return new WaitForEndOfFrame();
        if(Legion.Instance.u1FriendExist != 0)
        {
            Alrams[0].SetActive(true);
            Alrams[3].SetActive(true);
        }
        else
        {
            Alrams[0].SetActive(false);
            Alrams[3].SetActive(false);
        }
        if(Legion.Instance.u1MailExist != 0)
        {
            Alrams[1].SetActive(true);
            Alrams[4].SetActive(true);
        }
        else
        {
            Alrams[1].SetActive(false);
            Alrams[4].SetActive(false);
        }
    }
    public void OnClickListMenu(int _menu)
    {
        //StartCoroutine(CheckNew());
        switch(_menu)
        {
            case 0:
                if(Legion.Instance.sName == "")
                {
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_social_friend"), TextManager.Instance.GetText("popup_desc_friend"), null);
                    return;
                }
                _friendTab.SetActive(true);
                _mailTab.SetActive(false);
                _noticeTab.SetActive(false);
                break;
            case 1:
                _friendTab.SetActive(false);
                _mailTab.SetActive(true);
                _noticeTab.SetActive(false);
                break;
            case 2:
                _friendTab.SetActive(false);
                _mailTab.SetActive(false);
                _noticeTab.SetActive(true);
                break;
        }

        objSocialMenu.SetActive(true);
        _toggle[_menu].isOn = true;
        objSocialList.SetActive(false);
    }

    public void OnClickTopMenu(int _menu)
    {
        //for(int i=0; i<SocialInfo.Instance.u1FriendInviteCount; i++)
        //{
        //    if(SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bAccept || SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bDeleted)
        //    {
        //        Alrams[0].SetActive(false);
        //        Alrams[3].SetActive(false);
        //    }
        //    else
        //    {
        //        Alrams[0].SetActive(true);
        //        Alrams[3].SetActive(true);
        //        break;
        //    }
        //}
        //for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        //{
        //    if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
        //    {
        //        Alrams[1].SetActive(false);
        //        Alrams[4].SetActive(false);
        //    }
        //    else
        //    {
        //        Alrams[1].SetActive(true);
        //        Alrams[4].SetActive(true);
        //        break;
        //    }
        //}
        StartCoroutine(CheckNew());
        switch(_menu)
        {
            case 0:
                if(Legion.Instance.sName == "")
                {
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_social_friend"), TextManager.Instance.GetText("popup_desc_friend"), null);
                    return;
                }
                _friendTab.SetActive(true);
                _mailTab.SetActive(false);
                _noticeTab.SetActive(false);
                break;
            case 1:
                _friendTab.SetActive(false);
                _mailTab.SetActive(true);
                _noticeTab.SetActive(false);
                break;
            case 2:
                _friendTab.SetActive(false);
                _mailTab.SetActive(false);
                _noticeTab.SetActive(true);
                break;
        }
    }

	// 2016. 12. 26 jy
	// 기존 프렌드포인트 버튼을 클릭시 소셜패널을 오픈시키지만
	// 소셜 패널안에서 작동하는 버튼으로 별도로 처리한다
	public void OnClickFriendGoodsBtn()
	{
		// 기존에 친구 목록이 오픈되어 있다면 작동 시키지 않는다
		if(_friendTab.activeSelf == true)
			return;

		OnClickListMenu(0);
	}
}
