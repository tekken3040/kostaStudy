using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SocialFriendRequestSlot : MonoBehaviour
{
    FriendRequestCount _FriendRequest;
    UInt16 _keyDic;
    public Image ClassIcon;
    public Image CharElement;
    public Text CharLvl;
    public Text CharName;
    public Text LoginTime;
    public GameObject Btn_Delete;
    Int64 timeTicks;

    public void SetData(FriendRequestCount _Friend, UInt16 _key)
    {
        _FriendRequest = new FriendRequestCount();
        _FriendRequest = _Friend;
        _keyDic = _key;
        ClassIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + _FriendRequest.u2MainCharClassID);
        CharLvl.text = _FriendRequest.u2Level.ToString();
        CharName.text = _FriendRequest.strLegionName;
        CharElement.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + (_FriendRequest.u1MainCharElement));

		timeTicks = /*DateTime.Now.Ticks*/Legion.Instance.ServerTime.Ticks - _FriendRequest.dtLastLoginTime.Ticks;
        TimeSpan timespan = new TimeSpan(timeTicks);
        if(timespan.Days > 0)
            LoginTime.text = string.Format(TextManager.Instance.GetText("mark_time_left_day"), timespan.Days);
        else
            LoginTime.text = string.Format(TextManager.Instance.GetText("mark_time_left_hour"), timespan.Hours,timespan.Minutes );
        if(_FriendRequest.bDeleted)
            this.gameObject.SetActive(false);
    }

    public void OnClickRequestCancel()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        UInt64[] u8UserSN = new UInt64[1];
        u8UserSN[0] = _FriendRequest.u8UserSN;
        Server.ServerMgr.Instance.RequestFriendInviteCancel(u8UserSN, SuccessRequestCancel);
    }

    public void SuccessRequestCancel(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_social_friend_request"), TextManager.Instance.GetError (Server.MSGs.FRIEND_CANCEL_INVITE, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            SocialInfo.Instance.dicFriendRequestCount[_keyDic].bDeleted = true;
            GameObject.Destroy(this.gameObject);
        }
    }
}
