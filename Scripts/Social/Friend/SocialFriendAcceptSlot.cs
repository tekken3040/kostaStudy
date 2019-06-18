using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SocialFriendAcceptSlot : MonoBehaviour
{
    FriendInviteCount _FriendInvite;
    UInt16 _keyDic;
    public Image ClassIcon;
    public Image CharElement;
    public Text CharLvl;
    public Text CharName;
    public Text LoginTime;
    public GameObject Btn_Accept;
    public GameObject Btn_Delete;
    Int64 timeTicks;
    SocialFriendTab cParent;
    public void SetData(SocialFriendTab parent, FriendInviteCount _Friend, UInt16 _key)
    {
        cParent = parent;
        _FriendInvite = new FriendInviteCount();
        _FriendInvite = _Friend;
        _keyDic = _key;
        ClassIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + _FriendInvite.u2MainCharClassID);
        CharLvl.text = _FriendInvite.u2Level.ToString();
        CharName.text = _FriendInvite.strLegionName;
        CharElement.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + (_FriendInvite.u1MainCharElement));
        
		timeTicks = /*DateTime.Now.Ticks*/Legion.Instance.ServerTime.Ticks - _FriendInvite.dtLastLoginTime.Ticks;
        TimeSpan timespan = new TimeSpan(timeTicks);
        if(timespan.Days > 0)
            LoginTime.text = string.Format(TextManager.Instance.GetText("mark_time_left_day"), timespan.Days);
        else
            LoginTime.text = string.Format(TextManager.Instance.GetText("mark_time_left_hour"), timespan.Hours,timespan.Minutes );

        if(_FriendInvite.bAccept || _FriendInvite.bDeleted)
            this.gameObject.SetActive(false);
    }

    public void OnClickAcceptInvite()
    {
        if(SocialInfo.Instance.u1FriendCount == SocialInfo.Instance.dicSocialInfo[1].u2FriendMax)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_social_friend_invite"), TextManager.Instance.GetText("popup_friends_full"), null);
            return;
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        UInt64[] u8UserSN = new UInt64[1];
        u8UserSN[0] = _FriendInvite.u8UserSN;
        Server.ServerMgr.Instance.RequestFriendInviteConfirm(1, u8UserSN, SuccessAcceptInvite);
    }

    public void SuccessAcceptInvite(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_social_friend_invite"), TextManager.Instance.GetError (Server.MSGs.FRIEND_CONFIRM_INVITE, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            SocialInfo.Instance.dicFriendInviteCount[_keyDic].bAccept = true;
            SocialInfo.Instance.dicFriendInviteCount[_keyDic].bDeleted = false;
            FriendCount _friend = new FriendCount();
            _friend.u8UserSN = _FriendInvite.u8UserSN;
            _friend.strLegionName = _FriendInvite.strLegionName;
            _friend.u2MainCharClassID = _FriendInvite.u2MainCharClassID;
            _friend.u1MainCharElement = _FriendInvite.u1MainCharElement;
            _friend.u8LastLoginTime = _FriendInvite.u8LastLoginTime;
            _friend.dtLastLoginTime = _FriendInvite.dtLastLoginTime;
            _friend.u2Level = _FriendInvite.u2Level;
            _friend.u1Point = 0;
            _friend.u1New = 1;
            
            SocialInfo.Instance.dicFriendCount.Add((UInt16)SocialInfo.Instance.dicFriendCount.Count, _friend);
            SocialInfo.Instance.u1FriendCount++;
            cParent.RefreashFriendList(2);

			Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.Friend, 0, 0, 0, 0, 1);
            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.Friend);
            Destroy(this.gameObject);
        }
    }

    public void OnClickCancelInvite()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        UInt64[] u8UserSN = new UInt64[1];
        u8UserSN[0] = _FriendInvite.u8UserSN;
        Server.ServerMgr.Instance.RequestFriendInviteConfirm(2, u8UserSN, SeccessCancelInvite);
    }

    public void SeccessCancelInvite(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("btn_social_friend_invite"), TextManager.Instance.GetError (Server.MSGs.FRIEND_CONFIRM_INVITE, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            SocialInfo.Instance.dicFriendInviteCount[_keyDic].bAccept = false;
            SocialInfo.Instance.dicFriendInviteCount[_keyDic].bDeleted = true;
            if(SocialInfo.Instance.u1FriendInviteCount > 0)
                SocialInfo.Instance.u1FriendInviteCount--;
            SocialInfo.Instance.dicFriendInviteCount.Remove(_keyDic);
            Destroy(this.gameObject);
        }
    }
}
