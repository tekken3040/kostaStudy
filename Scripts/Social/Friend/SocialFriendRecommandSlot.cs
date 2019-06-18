using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SocialFriendRecommandSlot : MonoBehaviour
{
    FriendRecommend _FriendRecommand;

    public Image ClassIcon;
    public Image CharElement;
    public Text CharLvl;
    public Text CharName;
    public Text LoginTime;
    public GameObject Btn_Recommand;
    Int64 timeTicks;

    public void SetData(FriendRecommend _Friend)
    {
        _FriendRecommand = new FriendRecommend();
        _FriendRecommand = _Friend;

        ClassIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + _FriendRecommand.u2MainCharClassID);
        CharLvl.text = _FriendRecommand.u2Level.ToString();
        CharName.text = _FriendRecommand.strLegionName;
        CharElement.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + (_FriendRecommand.u1MainCharElement));
        
		timeTicks = /*DateTime.Now.Ticks*/Legion.Instance.ServerTime.Ticks - _FriendRecommand.dtLastLoginTime.Ticks;
        TimeSpan timespan = new TimeSpan(timeTicks);
        if(timespan.Days > 0)
            LoginTime.text = string.Format(TextManager.Instance.GetText("mark_time_left_day"), timespan.Days);
        else
            LoginTime.text = string.Format(TextManager.Instance.GetText("mark_time_left_hour"), timespan.Hours,timespan.Minutes );
        
    }

    public void OnClickInvite()
    {
        if(SocialInfo.Instance.u1FriendCount == SocialInfo.Instance.dicSocialInfo[1].u2FriendMax)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_social_friend_invite"), TextManager.Instance.GetText("popup_friends_full"), null);
            return;
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        UInt64[] u8UserSN = new UInt64[1];
        u8UserSN[0] = _FriendRecommand.u8UserSN;
        Server.ServerMgr.Instance.RequestFriendInvite(u8UserSN, SuccessInvite);
    }

    public void SuccessInvite(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_social_friend_invite"), TextManager.Instance.GetError (Server.MSGs.FRIEND_INVITE, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            FriendRequestCount _friendRequest = new FriendRequestCount();
            _friendRequest.u8UserSN = _FriendRecommand.u8UserSN;
            _friendRequest.strLegionName = _FriendRecommand.strLegionName;
            _friendRequest.u2MainCharClassID = _FriendRecommand.u2MainCharClassID;
            _friendRequest.u1MainCharElement = _FriendRecommand.u1MainCharElement;
            _friendRequest.u8LastLoginTime = _FriendRecommand.u8LastLoginTime;
            _friendRequest.dtLastLoginTime = _FriendRecommand.dtLastLoginTime;
            _friendRequest.u2Level = _FriendRecommand.u2Level;
            _friendRequest.bDeleted = false;
            SocialInfo.Instance.dicFriendRequestCount.Add(Convert.ToUInt16(SocialInfo.Instance.dicFriendRequestCount.Count), _friendRequest);
            SocialInfo.Instance.u1FriendRequestCount++;
            GameObject.Destroy(this.gameObject);
        }
    }
}
