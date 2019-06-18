using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class SocialFriendSlot : MonoBehaviour
{
    FriendCount _FriendCnt;
    UInt16 _keyDic;
    public Image ClassIcon;
    public Image CharElement;
    public Text CharLvl;
    public Text CharName;
    public Text LoginTime;
    public GameObject Btn_RecvPoint;
    public GameObject Btn_SendPoint;
    Int64 timeTicks;
    SocialFriendTab _cParent;

    public void SetData(FriendCount _Friend, UInt16 _key, SocialFriendTab _parent)
    {
        _FriendCnt = new FriendCount();
        _FriendCnt = _Friend;
        _cParent = _parent;
        _keyDic = _key;
        ClassIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + _FriendCnt.u2MainCharClassID);
        CharLvl.text = _FriendCnt.u2Level.ToString();
        CharName.text = _FriendCnt.strLegionName;
        CharElement.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + (_FriendCnt.u1MainCharElement));
        //SocialInfo.Instance.dicFriendCount[_keyDic].u1New = 0;
		timeTicks = /*DateTime.Now.Ticks*/ Legion.Instance.ServerTime.Ticks - _FriendCnt.dtLastLoginTime.Ticks;
        TimeSpan timespan = new TimeSpan(timeTicks);
        if(timespan.Days > 0)
            LoginTime.text = string.Format(TextManager.Instance.GetText("mark_time_left_day"), timespan.Days);
        else
            LoginTime.text = string.Format(TextManager.Instance.GetText("mark_time_left_hour"), timespan.Hours,timespan.Minutes );

        if((_FriendCnt.u1SendPoint) > 0)
            Btn_SendPoint.GetComponent<Button>().interactable = false;
        else
            Btn_SendPoint.GetComponent<Button>().interactable = true;

        if((_FriendCnt.u1RecvPoint) > 0)
            Btn_RecvPoint.GetComponent<Button>().interactable = true;
        else
            Btn_RecvPoint.GetComponent<Button>().interactable = false;
    }

    public void OnClickSendPoint()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        UInt64[] u8UserSN = new UInt64[1];
        u8UserSN[0] = _FriendCnt.u8UserSN;
        Server.ServerMgr.Instance.RequestSendFriendPoint(u8UserSN, SuccessSendPoint);
    }

    public void SuccessSendPoint(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_sendpoint"), TextManager.Instance.GetError (Server.MSGs.FRIEND_SEND_POINT, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            SocialInfo.Instance.dicFriendCount[_keyDic].u1SendPoint = 1;
            Btn_SendPoint.GetComponent<Button>().interactable = false;
            _cParent.RefreashFriendPoint();
        }
    }

    public void OnClickRecvPoint()
    {
        int _point = Legion.Instance.FriendShipPoint + SocialInfo.Instance.dicSocialInfo[1].FRIEND_POINT_SENDING;
        if(SocialInfo.Instance.dicSocialInfo[1].MAX_FRIENDPOINT < _point)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_friendship_point"), TextManager.Instance.GetText("popup_desc_friendship_point_max"), null);
            return;
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        UInt64[] u8UserSN = new UInt64[1];
        u8UserSN[0] = _FriendCnt.u8UserSN;
        Server.ServerMgr.Instance.RequestReciveFriendPoint(u8UserSN, SuccessRecvPoint);
    }

    public void SuccessRecvPoint(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_recvpoint"), TextManager.Instance.GetError (Server.MSGs.FRIEND_RECV_POINT, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            SocialInfo.Instance.dicFriendCount[_keyDic].u1RecvPoint = 0;
            Legion.Instance.AddGoods(Convert.ToInt16(GoodsType.FRIENDSHIP_POINT), SocialInfo.Instance.dicSocialInfo[1].FRIEND_POINT_SENDING);
            Btn_RecvPoint.GetComponent<Button>().interactable = false;
            _cParent.RefreashFriendPoint();
        }
    }

    public void OnClickDelete()
    {
        PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_friend_delete"), TextManager.Instance.GetText("popup_desc_friend_delete"), TextManager.Instance.GetText("btn_friendlist_delete"), RequestFriendDelete, null);
        return;
    }

    public void RequestFriendDelete(object[] param)
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        UInt64[] u8UserSN = new UInt64[1];
        u8UserSN[0] = _FriendCnt.u8UserSN;
        Server.ServerMgr.Instance.RequestFriendDrop(u8UserSN, SuccessDelete);
    }

    public void SuccessDelete(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_friendlist_delete"), TextManager.Instance.GetError (Server.MSGs.FRIEND_DROP, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            SocialInfo.Instance.u1FriendCount--;
            //SocialInfo.Instance.dicFriendCount[_keyDic].bDeleted = true;
            SocialInfo.Instance.dicFriendCount.Remove(_keyDic);
            Destroy(this.gameObject);
			_cParent.RefreashFriendPoint();
        }
    }

    public void DisableRecvBtn()
    {
        Btn_RecvPoint.GetComponent<Button>().interactable = false;
    }

    public void DisableSendBtn()
    {
        Btn_SendPoint.GetComponent<Button>().interactable = false;
    }
}
