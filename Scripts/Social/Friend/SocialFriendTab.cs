using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class SocialFriendTab : MonoBehaviour
{
    public SocialPanel _socialPanel;
    public GameObject BtnGroupFriendList;
    public GameObject BtnGroupFriendRequest;
    public GameObject BtnGroupFriendAccept;
    public GameObject BtnGroupFriendSearch;
    public GameObject objPrefFriendList;
    public GameObject objPrefFriendRequest;
    public GameObject objPrefFriendAccept;
    public GameObject objPrefFriendSearch;
    public GameObject ScrollList;
    public GameObject[] objAlram;
    public Toggle[] _friendMenu;
    public Text _friendName;
    public InputField _inputFieldCrewname;
    List<UInt64> tempFriendSN;
    List<UInt64> tempFriendInviteSN;

	public void Start()
	{
		// 2016. 10. 05 jy
		// 친구 최대치는 변경되지 않으므로 한번만 셋팅하면 되게함
		BtnGroupFriendList.transform.GetChild(4).GetComponent<Text>().text = "/ "+ SocialInfo.Instance.dicSocialInfo[1].u2FriendMax.ToString();
	}

    public void OnEnable()
    {
        //_friendMenu[0].isOn = true;
        //OnClickFriendMenu(0);
        if(!_socialPanel._recvFriendList)
        {
            RequestFriendList();
            _socialPanel._recvFriendList = true;
        }
        else
        {
            ReciveFriendList(Server.ERROR_ID.NONE);
        }
    }
    public void OnDisable()
    {
        _friendMenu[0].isOn = false;
        _friendMenu[1].isOn = false;
        _friendMenu[2].isOn = false;
        _friendMenu[3].isOn = false;
        Byte tempFriendExist = Legion.Instance.u1FriendExist;
        bool bFriendExist = false;
        for(int i=0; i<SocialInfo.Instance.u1FriendInviteCount; i++)
        {
            if(!SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bAccept && !SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bDeleted)
            {
                bFriendExist = true;
                break;
            }
            else
                bFriendExist = false;
        }
        if(bFriendExist)
            Legion.Instance.u1FriendExist = tempFriendExist;
        else
            Legion.Instance.u1FriendExist = 0;
    }

    public void RequestFriendList()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestFriendList(ReciveFriendList);
    }

    public void ReciveFriendList(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.CloseLoadingPopup (); 
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_social_friend_list"), TextManager.Instance.GetError (Server.MSGs.FRIEND_LIST, err), Server.ServerMgr.Instance.CallClear);
		}
        else
        {
            //_friendMenu[1].isOn = false;
            //_friendMenu[2].isOn = false;
            //_friendMenu[3].isOn = false;
            _friendMenu[0].interactable = true;
            _friendMenu[0].isOn = true;
            for(int i=0; i<3; i++)
                RefreashFriendList((Byte)i);
            //OnClickFriendMenu(0);
        }
    }

    public void ClearList()
    {
        for(int i = 0; i < ScrollList.transform.childCount; i++)
        {
            Destroy(ScrollList.transform.GetChild(i).gameObject);
        }
        ScrollList.transform.localPosition = Vector3.zero;
    }

    public void OnClickFriendMenu(int _menu)
    {
        ClearList();
        SocialInfo.Instance.dicFriendRecommend.Clear();
        SocialInfo.Instance.u1FriendRecommendCount = 0;
        if(!_friendMenu[_menu].isOn)
            return;
        //for(int i=0; i<_friendMenu.Length; i++)
        //{
        //    if(_menu == i)
        //        _friendMenu[i].isOn = true;
        //    else
        //        _friendMenu[i].isOn = false;
        //}
        switch(_menu)
        {
            case 0:
                BtnGroupFriendList.SetActive(true);
                BtnGroupFriendRequest.SetActive(false);
                BtnGroupFriendAccept.SetActive(false);
                BtnGroupFriendSearch.SetActive(false);
                _friendMenu[0].interactable = false;
                _friendMenu[1].interactable = true;
                _friendMenu[2].interactable = true;
                _friendMenu[3].interactable = true;
                InitFriendList();
                RefreashFriendList(0);
				RefreashFriendPoint();
				/*
                for(int i=0; i<SocialInfo.Instance.u1FriendCount; i++)
                {
                    if(!SocialInfo.Instance.dicFriendCount[(UInt16)i].bDeleted)
                    {
                        if(SocialInfo.Instance.dicFriendCount[(UInt16)i].u1RecvPoint != 0)
                        {
                            BtnGroupFriendList.transform.GetChild(1).GetComponent<Button>().interactable = true;
                            break;
                        }
                        else
                        {
                            BtnGroupFriendList.transform.GetChild(1).GetComponent<Button>().interactable = false;
                        }
                    }
                }
                for(int i=0; i<SocialInfo.Instance.u1FriendCount; i++)
                {
                    if(!SocialInfo.Instance.dicFriendCount[(UInt16)i].bDeleted)
                    {
                        if(SocialInfo.Instance.dicFriendCount[(UInt16)i].u1SendPoint != 1)
                        {
                            BtnGroupFriendList.transform.GetChild(2).GetComponent<Button>().interactable = true;
                            break;
                        }
                        else
                        {
                            BtnGroupFriendList.transform.GetChild(2).GetComponent<Button>().interactable = false;
                        }
                    }
                }
                */
                break;

            case 1:
                BtnGroupFriendList.SetActive(false);
                BtnGroupFriendRequest.SetActive(true);
                BtnGroupFriendAccept.SetActive(false);
                BtnGroupFriendSearch.SetActive(false);
                _friendMenu[0].interactable = true;
                _friendMenu[1].interactable = false;
                _friendMenu[2].interactable = true;
                _friendMenu[3].interactable = true;
                InitFriendRequest();
                RefreashFriendList(1);
                break;

            case 2:
                BtnGroupFriendList.SetActive(false);
                BtnGroupFriendRequest.SetActive(false);
                BtnGroupFriendAccept.SetActive(true);
                BtnGroupFriendSearch.SetActive(false);
                _friendMenu[0].interactable = true;
                _friendMenu[1].interactable = true;
                _friendMenu[2].interactable = false;
                _friendMenu[3].interactable = true;
                InitFriendAccept();
                RefreashFriendList(2);
                break;

            case 3:
                BtnGroupFriendList.SetActive(false);
                BtnGroupFriendRequest.SetActive(false);
                BtnGroupFriendAccept.SetActive(false);
                BtnGroupFriendSearch.SetActive(true);
                _friendMenu[0].interactable = true;
                _friendMenu[1].interactable = true;
                _friendMenu[2].interactable = true;
                _friendMenu[3].interactable = false;
                OnClickRefreashList();
                break;
        }
    }

    public void RefreashFriendPoint()
    {
		// 2016. 10. 05 jy 친구 새로고침 변경
		int nFriendCount = 0;
		bool isRecvPoint = false;
		bool isSendPoint = false;
		for(int i=0; i<SocialInfo.Instance.u1FriendCount; i++)
		{
            if(!SocialInfo.Instance.dicFriendCount.ContainsKey((UInt16)i))
                continue;
			if(!SocialInfo.Instance.dicFriendCount[(UInt16)i].bDeleted)
			{
				++nFriendCount;
				if(isRecvPoint == false)
				{
					if(SocialInfo.Instance.dicFriendCount[(UInt16)i].u1RecvPoint != 0)
						isRecvPoint = true;
				}
				if(isSendPoint == false)
				{
					if(SocialInfo.Instance.dicFriendCount[(UInt16)i].u1SendPoint != 1)
						isSendPoint = true;
				}
			}
		}
		BtnGroupFriendList.transform.GetChild(1).GetComponent<Button>().interactable = isRecvPoint;
		BtnGroupFriendList.transform.GetChild(2).GetComponent<Button>().interactable = isSendPoint;
		// 2016. 10. 05 jy
		// 현재 등록 되어 있는 친구 수
		BtnGroupFriendList.transform.GetChild(3).GetComponent<Text>().text = nFriendCount.ToString();
		/*
        for(int i=0; i<SocialInfo.Instance.u1FriendCount; i++)
        {
            if(!SocialInfo.Instance.dicFriendCount[(UInt16)i].bDeleted)
            {
                if(SocialInfo.Instance.dicFriendCount[(UInt16)i].u1RecvPoint != 0)
                {
                    BtnGroupFriendList.transform.GetChild(1).GetComponent<Button>().interactable = true;
                    break;
                }
                else
                {
                    BtnGroupFriendList.transform.GetChild(1).GetComponent<Button>().interactable = false;
                }
                if(SocialInfo.Instance.dicFriendCount[(UInt16)i].u1SendPoint != 1)
                {
                    BtnGroupFriendList.transform.GetChild(2).GetComponent<Button>().interactable = true;
                    break;
                }
                else
                {
                    BtnGroupFriendList.transform.GetChild(2).GetComponent<Button>().interactable = false;
                }
            }
        }
        */
    }

    public void RefreashFriendList(Byte u1FriendCategory)
    {
        switch(u1FriendCategory)
        {
            case 0:
                for(int i=0; i<SocialInfo.Instance.u1FriendCount; i++)
                {
                    if(SocialInfo.Instance.dicFriendCount[(UInt16)i].bDeleted)
                    {
                        objAlram[0].SetActive(false);
                        continue;
                    }
                    else if(SocialInfo.Instance.dicFriendCount[(UInt16)i].u1New != 1)
                    {
                        objAlram[0].SetActive(false);
                        continue;
                    }
                    else if(SocialInfo.Instance.dicFriendCount[(UInt16)i].u1New == 1)
                    {
                        objAlram[0].SetActive(true);
                        break;
                    }
                }
                tempFriendSN = new List<UInt64>();
                tempFriendInviteSN = new List<UInt64>();
                for(int i=0; i<SocialInfo.Instance.u1FriendCount; i++)
                {
                    if(SocialInfo.Instance.dicFriendCount[(UInt16)i].u1New == 1)
                    {
                        tempFriendSN.Add(SocialInfo.Instance.dicFriendCount[(UInt16)i].u8UserSN);
                        SocialInfo.Instance.dicFriendCount[(UInt16)i].u1New = 0;
                    }
                }
                if(tempFriendSN.Count > 0)
                {
                    PopupManager.Instance.ShowLoadingPopup(1);
                    Server.ServerMgr.Instance.RequestFriendNewCheck(tempFriendSN.ToArray(), tempFriendInviteSN.ToArray(), RefreashNewCheck);
                }
                break;

            case 1:
                for(int i=0; i<SocialInfo.Instance.u1FriendRequestCount; i++)
                {
                    if(!SocialInfo.Instance.dicFriendRequestCount[(UInt16)i].bDeleted)
                    {
                        BtnGroupFriendRequest.transform.GetChild(0).GetComponent<Button>().interactable = true;
                        //objAlram[1].SetActive(true);
                        break;
                    }
                    else
                    {
                        BtnGroupFriendRequest.transform.GetChild(0).GetComponent<Button>().interactable = false;
                        //objAlram[1].SetActive(false);
                    }
                }
                break;

            case 2:
                for(int i=0; i<SocialInfo.Instance.u1FriendInviteCount; i++)
                {
                    if(!SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bAccept && !SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bDeleted)
                    {
                        BtnGroupFriendAccept.transform.GetChild(0).GetComponent<Button>().interactable = true;
                        BtnGroupFriendAccept.transform.GetChild(1).GetComponent<Button>().interactable = true;
                        objAlram[2].SetActive(true);
                        _socialPanel.Alrams[0].SetActive(true);
                        _socialPanel.Alrams[3].SetActive(true);
                        break;
                    }
                    else
                    {
                        BtnGroupFriendAccept.transform.GetChild(0).GetComponent<Button>().interactable = false;
                        BtnGroupFriendAccept.transform.GetChild(1).GetComponent<Button>().interactable = false;
                        objAlram[2].SetActive(false);
                        _socialPanel.Alrams[0].SetActive(false);
                        _socialPanel.Alrams[3].SetActive(false);
                    }
                }
                tempFriendSN = new List<UInt64>();
                tempFriendInviteSN = new List<UInt64>();
                for(int i=0; i<SocialInfo.Instance.u1FriendInviteCount; i++)
                {
                    if(SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].u1New == 1)
                    {
                        tempFriendInviteSN.Add(SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].u8UserSN);
                        SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].u1New = 0;
                    }
                }
                if(tempFriendSN.Count > 0)
                {
                    PopupManager.Instance.ShowLoadingPopup(1);
                    Server.ServerMgr.Instance.RequestFriendNewCheck(tempFriendSN.ToArray(), tempFriendInviteSN.ToArray(), RefreashNewCheck);
                }
                break;

            case 3:
                break;
        }
    }
    void RefreashNewCheck(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.FRIEND_LIST, err), Server.ServerMgr.Instance.CallClear);
		}
        else
        {

        }
    }

    //친구 목록---------------------------------------------------------------------------------------
    public void InitFriendList()
    {
        for(int i=0; i<SocialInfo.Instance.u1FriendCount; i++)
        {
            if(SocialInfo.Instance.dicFriendCount[(UInt16)i].bDeleted)
                continue;
            GameObject objFriendlistSlotPref = Instantiate(objPrefFriendList);
			objFriendlistSlotPref.transform.SetParent(ScrollList.transform);
            objFriendlistSlotPref.transform.localPosition = Vector3.zero;
            objFriendlistSlotPref.transform.localScale = Vector3.one;
            objFriendlistSlotPref.GetComponent<SocialFriendSlot>().SetData(SocialInfo.Instance.dicFriendCount[(UInt16)i], (UInt16)i, this);
        }
    }
    List<UInt16> recvCnt = new List<UInt16>();
    public void OnClickRecvAllPoint()
    {
        int _point = Legion.Instance.FriendShipPoint + SocialInfo.Instance.dicSocialInfo[1].FRIEND_POINT_SENDING;
        if(SocialInfo.Instance.dicSocialInfo[1].MAX_FRIENDPOINT < _point)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_friendship_point"), TextManager.Instance.GetText("popup_desc_friendship_point_max"), null);
            return;
        }

        PopupManager.Instance.ShowLoadingPopup(1);
        List<UInt64> lstRecvPoint = new List<UInt64>();
        int _addPoint = 0;
        recvCnt.Clear();
        for(int i=0; i<SocialInfo.Instance.u1FriendCount; i++)
        {
            if(!SocialInfo.Instance.dicFriendCount[(UInt16)i].bDeleted)
                if(SocialInfo.Instance.dicFriendCount[(UInt16)i].u1RecvPoint > 0)
                {
                     _addPoint = _addPoint + SocialInfo.Instance.dicSocialInfo[1].FRIEND_POINT_SENDING;      
                if(SocialInfo.Instance.dicSocialInfo[1].MAX_FRIENDPOINT > _addPoint)
                    {
                        lstRecvPoint.Add(SocialInfo.Instance.dicFriendCount[(UInt16)i].u8UserSN);
                        recvCnt.Add((UInt16)i);
                    }
                    else
                        break;
                }
        }
        UInt64[] u8UserSN = new UInt64[lstRecvPoint.Count];
        for(int i=0; i<lstRecvPoint.Count; i++)
            u8UserSN[i] = lstRecvPoint[i];
        
        Server.ServerMgr.Instance.RequestReciveFriendPoint(u8UserSN, SuccessRecvAllPoint);
    }

    public void SuccessRecvAllPoint(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_recvpoint"), TextManager.Instance.GetError (Server.MSGs.FRIEND_RECV_POINT, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            int _pointCnt = 0;

            for(int i=0; i<recvCnt.Count; i++)
            {
                if(SocialInfo.Instance.dicFriendCount[recvCnt[i]].u1RecvPoint > 0)
                {
                    _pointCnt++;
                    SocialInfo.Instance.dicFriendCount[recvCnt[i]].u1RecvPoint = 0;
                }
            }

            for(int i = 0; i < ScrollList.transform.childCount; i++)
            {
                ScrollList.transform.GetChild(i).GetComponent<SocialFriendSlot>().DisableRecvBtn();
            }
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_friendship_point"), 
                (SocialInfo.Instance.dicSocialInfo[1].FRIEND_POINT_SENDING*_pointCnt).ToString() + TextManager.Instance.GetText("popup_desc_friendship_point"), null);
            Legion.Instance.AddGoods(Convert.ToInt16(GoodsType.FRIENDSHIP_POINT), SocialInfo.Instance.dicSocialInfo[1].FRIEND_POINT_SENDING*_pointCnt);
            RefreashFriendPoint();
        }
    }

	int sendCnt = 0;
    public void OnClickSendAllPoint()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        List<UInt64> lstSendPoint = new List<UInt64>();
		sendCnt = 0;
        for(int i=0; i<SocialInfo.Instance.u1FriendCount; i++)
        {
            if(!SocialInfo.Instance.dicFriendCount[(UInt16)i].bDeleted)
                if(SocialInfo.Instance.dicFriendCount[(UInt16)i].u1SendPoint == 0)
                {
                    lstSendPoint.Add(SocialInfo.Instance.dicFriendCount[(UInt16)i].u8UserSN);
					++sendCnt;
                }
        }
        UInt64[] u8UserSN = new UInt64[lstSendPoint.Count];
        for(int i=0; i<lstSendPoint.Count; i++)
            u8UserSN[i] = lstSendPoint[i];
        Server.ServerMgr.Instance.RequestSendFriendPoint(u8UserSN, SuccessSendAllPoint);
    }

    public void SuccessSendAllPoint(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_sendpoint"), TextManager.Instance.GetError (Server.MSGs.FRIEND_SEND_POINT, err), Server.ServerMgr.Instance.CallClear);
        }
        else if(err == Server.ERROR_ID.NONE)
        {
			for(int i=0; i<SocialInfo.Instance.u1FriendCount; i++)
			{
				// 포인트 보냄 여부를 변경한다
				if(!SocialInfo.Instance.dicFriendCount[(UInt16)i].bDeleted)
				{
					if(SocialInfo.Instance.dicFriendCount[(UInt16)i].u1SendPoint == 0)
						SocialInfo.Instance.dicFriendCount[(UInt16)i].u1SendPoint = 1;
				}
				ScrollList.transform.GetChild(i).GetComponent<SocialFriendSlot>().DisableSendBtn();
			}
			BtnGroupFriendList.transform.GetChild(2).GetComponent<Button>().interactable = false;
            RefreashFriendPoint();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), sendCnt + TextManager.Instance.GetText("popup_desc_friend_send_point"), null);
        }
    }
    //친구 목록End--------------------------------------------------------------------------------------

    //친구 요청 목록-------------------------------------------------------------------------------------
    public void InitFriendRequest()
    {
        int ListMaxCnt = 0;
        if(SocialInfo.Instance.u1FriendRequestCount > SocialInfo.Instance.dicSocialInfo[1].u2FriendInviteInPage)
            ListMaxCnt = SocialInfo.Instance.dicSocialInfo[1].u2FriendInviteInPage;
        else
            ListMaxCnt = SocialInfo.Instance.u1FriendRequestCount;
        for(int i=0; i<ListMaxCnt; i++)
        {
            GameObject objFriendRequestList = Instantiate(objPrefFriendRequest);
            objFriendRequestList.transform.SetParent(ScrollList.transform);
            objFriendRequestList.transform.localPosition = Vector3.zero;
            objFriendRequestList.transform.localScale = Vector3.one;
            objFriendRequestList.GetComponent<SocialFriendRequestSlot>().SetData(SocialInfo.Instance.dicFriendRequestCount[(UInt16)i], (UInt16)i);
        }
    }

    public void OnClickRequestAllCancel()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        List<UInt64> _userSn = new List<ulong>();
        for(int i=0; i<SocialInfo.Instance.u1FriendRequestCount; i++)
        {
            if(!SocialInfo.Instance.dicFriendRequestCount[(UInt16)i].bDeleted)
                _userSn.Add(SocialInfo.Instance.dicFriendRequestCount[(UInt16)i].u8UserSN);
        }
        UInt64[] u8UserSN = new UInt64[_userSn.Count];
        for(int i=0; i<_userSn.Count; i++)
            u8UserSN[i] = _userSn[i];
        Server.ServerMgr.Instance.RequestFriendInviteCancel(u8UserSN, SuccessRequestAllCancel);
    }

    public void SuccessRequestAllCancel(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_social_friend_request"), TextManager.Instance.GetError (Server.MSGs.FRIEND_CANCEL_INVITE, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            for(int i=0; i<SocialInfo.Instance.u1FriendRequestCount; i++)
            {
                if(!SocialInfo.Instance.dicFriendRequestCount[(UInt16)i].bDeleted)
                    SocialInfo.Instance.dicFriendRequestCount[(UInt16)i].bDeleted = true;
            }
            RefreashFriendList(1);
            ClearList();
        }
    }
    //친구 요청 목록End----------------------------------------------------------------------------------

    //친구 수락 목록-------------------------------------------------------------------------------------
    public void InitFriendAccept()
    {
        for(int i=0; i<SocialInfo.Instance.u1FriendInviteCount; i++)
        {
            GameObject objFriendInviteList = Instantiate(objPrefFriendAccept);
			objFriendInviteList.transform.SetParent(ScrollList.transform);
            objFriendInviteList.transform.localPosition = Vector3.zero;
            objFriendInviteList.transform.localScale = Vector3.one;
            objFriendInviteList.GetComponent<SocialFriendAcceptSlot>().SetData(this, SocialInfo.Instance.dicFriendInviteCount[(UInt16)i], (UInt16)i);
        }
    }

    public void OnClickAcceptAll()
    {
        if(SocialInfo.Instance.u1FriendCount == SocialInfo.Instance.dicSocialInfo[1].u2FriendMax)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_social_friend_invite"), TextManager.Instance.GetText("popup_friends_full"), null);
            return;
        }

        List<UInt64> _userSn = new List<ulong>();
        for(int i=0; i<SocialInfo.Instance.u1FriendInviteCount; i++)
        {
            if(!SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bAccept && !SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bDeleted)
                _userSn.Add(SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].u8UserSN);
        }

        // 2017. 03. 31 jy
        // 모두 수락시 추가 등록될 친구 갯수가 MAX치 보다 많아지는지 확인
        if((SocialInfo.Instance.u1FriendCount + _userSn.Count) > SocialInfo.Instance.dicSocialInfo[1].u2FriendMax)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_social_friend_invite"), TextManager.Instance.GetErrorText(Server.ERROR_ID.FRIEND_OTHER_FULL.ToString(), "", false), null);
            return;
        }

        PopupManager.Instance.ShowLoadingPopup(1);
        UInt64[] u8UserSN = new UInt64[_userSn.Count];
        for(int i=0; i<_userSn.Count; i++)
            u8UserSN[i] = _userSn[i];
        Server.ServerMgr.Instance.RequestFriendInviteConfirm(1, u8UserSN, SuccessAcceptAll);
    }

    public void SuccessAcceptAll(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_social_friend_invite"), TextManager.Instance.GetError (Server.MSGs.FRIEND_CONFIRM_INVITE, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
			uint newCnt = 0;
            for(int i=0; i<SocialInfo.Instance.u1FriendInviteCount; i++)
            {
                if(!SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bAccept && !SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bDeleted)
                {
                    SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bAccept = true;
                    SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bDeleted = false;
                    FriendCount _friend = new FriendCount();
                    //_friend.u8UserSN = SocialInfo.Instance.dicFriendInviteCount[(UInt16)SocialInfo.Instance.dicFriendCount.Count].u8UserSN;
                    //_friend.strLegionName = SocialInfo.Instance.dicFriendInviteCount[(UInt16)SocialInfo.Instance.dicFriendCount.Count].strLegionName;
                    //_friend.u2MainCharClassID = SocialInfo.Instance.dicFriendInviteCount[(UInt16)SocialInfo.Instance.dicFriendCount.Count].u2MainCharClassID;
                    //_friend.u1MainCharElement = SocialInfo.Instance.dicFriendInviteCount[(UInt16)SocialInfo.Instance.dicFriendCount.Count].u1MainCharElement;
                    //_friend.u8LastLoginTime = SocialInfo.Instance.dicFriendInviteCount[(UInt16)SocialInfo.Instance.dicFriendCount.Count].u8LastLoginTime;
                    //_friend.dtLastLoginTime = SocialInfo.Instance.dicFriendInviteCount[(UInt16)SocialInfo.Instance.dicFriendCount.Count].dtLastLoginTime;
                    //_friend.u2Level = SocialInfo.Instance.dicFriendInviteCount[(UInt16)SocialInfo.Instance.dicFriendCount.Count].u2Level;
                    _friend.u8UserSN = SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].u8UserSN;
                    _friend.strLegionName = SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].strLegionName;
                    _friend.u2MainCharClassID = SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].u2MainCharClassID;
                    _friend.u1MainCharElement = SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].u1MainCharElement;
                    _friend.u8LastLoginTime = SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].u8LastLoginTime;
                    _friend.dtLastLoginTime = SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].dtLastLoginTime;
                    _friend.u2Level = SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].u2Level;
                    _friend.u1Point = 0;
                    _friend.u1New = 1;
                    newCnt++;
                    if(SocialInfo.Instance.dicFriendCount.Count == 0)
                        SocialInfo.Instance.dicFriendCount.Add(0, _friend);
                    else
                        SocialInfo.Instance.dicFriendCount.Add((UInt16)(SocialInfo.Instance.dicFriendCount.Count), _friend);
                    SocialInfo.Instance.u1FriendCount++;
                }
            }
			Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.Friend, 0, 0, 0, 0, newCnt);
            SocialInfo.Instance.u1FriendInviteCount = 0;
            SocialInfo.Instance.dicFriendInviteCount.Clear();
            ClearList();

            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.Friend);
            //Legion.Instance.u1FriendExist = 0;
            //_socialPanel.Alrams[1].SetActive(false);
            //_socialPanel.Alrams[4].SetActive(false);
        }
    }

    public void OnClickCancelAll()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        List<UInt64> _userSn = new List<ulong>();
        for(int i=0; i<SocialInfo.Instance.u1FriendInviteCount; i++)
        {
            if(!SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bAccept && !SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bDeleted)
                _userSn.Add(SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].u8UserSN);
        }
        UInt64[] u8UserSN = new UInt64[_userSn.Count];
        for(int i=0; i<_userSn.Count; i++)
            u8UserSN[i] = _userSn[i];
        Server.ServerMgr.Instance.RequestFriendInviteConfirm(2, u8UserSN, SuccessCancelAll);
    }

    public void SuccessCancelAll(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup (); 
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_social_friend_request"), TextManager.Instance.GetError (Server.MSGs.FRIEND_CONFIRM_INVITE, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            for(int i=0; i<SocialInfo.Instance.u1FriendInviteCount; i++)
            {
                if(!SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bAccept && !SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bDeleted)
                {
                    SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bAccept = false;
                    SocialInfo.Instance.dicFriendInviteCount[(UInt16)i].bDeleted = true;
                }
            }

            ClearList();
        }
    }
    //친구 수락 목록End----------------------------------------------------------------------------------

    //추천 친구 목록-------------------------------------------------------------------------------------
    public void InitFriendRecommand()
    {
        for(int i=0; i<SocialInfo.Instance.u1FriendRecommendCount; i++)
        {
            GameObject objFriendSearchSlotPref = Instantiate(objPrefFriendSearch);
            objFriendSearchSlotPref.transform.SetParent(ScrollList.transform);
            objFriendSearchSlotPref.transform.localPosition = Vector3.zero;
            objFriendSearchSlotPref.transform.localScale = Vector3.one;
            objFriendSearchSlotPref.GetComponent<SocialFriendRecommandSlot>().SetData(SocialInfo.Instance.dicFriendRecommend[(UInt16)i]);
        }
    }

    public void OnClickFriendSearch()
    {
        if(_friendName.text == "")
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("mark_friend_search_input"), TextManager.Instance.GetErrorText("crew_enter_name", "", false), null);
            return;
        }
        else
        {
            for(int i=0; i<_friendName.text.Length; i++)
            {
                if(_friendName.text.Substring(i, 1).Equals(" "))
                {
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("mark_friend_search_input"), TextManager.Instance.GetErrorText("crew_space_name", "", false), null);
                    return;
                }
            }
        }
        SocialInfo.Instance.dicFriendRecommend.Clear();
        SocialInfo.Instance.u1FriendRecommendCount = 0;
        PopupManager.Instance.ShowLoadingPopup(1);
        //Server.ServerMgr.Instance.RequestFriendRecommend(_friendName.text, SuccessRecvList);
        Server.ServerMgr.Instance.RequestFriendRecommend(_inputFieldCrewname.text, SuccessRecvList);
    }

    public void OnClickRefreashList()
    {
        SocialInfo.Instance.dicFriendRecommend.Clear();
        SocialInfo.Instance.u1FriendRecommendCount = 0;
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestFriendRecommend("", SuccessRecvList);
    }

    public void SuccessRecvList(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("mark_friend_search_input"), TextManager.Instance.GetError (Server.MSGs.FRIEND_RECOMMEND, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            ClearList();
            InitFriendRecommand();
        }
    }
    //추천 친구 목록End-----------------------------------------------------------------------------------
}
