using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class GuildInfoPopup : MonoBehaviour {
	public RectTransform ListParent;
	public GameObject InfoSlotPref;

	public Text txtTitle;

	public GameObject objManage;
	public Text txtManage;
	public GameObject objPublic;
	public Button btnPublic;
	public Text txtPublic;

	public Button btnCenter;
	public Text txtCenter;

	public GameObject objDetailPopup;

    public Text txtGuildMember;

	List<GuildInfoSlot> lstSlots = new List<GuildInfoSlot>();

	bool bInit = false;
    StringBuilder tempStringBuilder;

	enum INFO_TYPE{
		ATTEND = 1,
		MANAGE = 2
	}

	INFO_TYPE infoType = INFO_TYPE.ATTEND;

	// Use this for initialization
	void OnEnable () {
		PopupManager.Instance.AddPopup(gameObject, OnClickClose);
        tempStringBuilder = new StringBuilder();
		if (GuildInfoMgr.Instance.bGuildMaster) {
			btnPublic.enabled = true;
		} else {
			btnPublic.enabled = false;
			txtManage.text = TextManager.Instance.GetText ("btn_guildinfo_secession");
		}
			
		if (GuildInfoMgr.Instance.cGuildMemberInfo.u1Public == 1) {
			txtPublic.text = TextManager.Instance.GetText ("btn_guild_open");
		} else {
			txtPublic.text = TextManager.Instance.GetText ("btn_guild_close");
		}

		switch (infoType) {
		case INFO_TYPE.ATTEND:
			txtTitle.text = TextManager.Instance.GetText ("popup_title_guild_guildinfo");
			txtCenter.text = TextManager.Instance.GetText ("btn_guildinfo_login_reward");
			if (GuildInfoMgr.Instance.bGuildMaster) txtManage.text = TextManager.Instance.GetText ("popup_title_guild_manage");
			break;
		case INFO_TYPE.MANAGE:
			txtTitle.text = TextManager.Instance.GetText ("btn_guildinfo_guildmanage");
			txtCenter.text = TextManager.Instance.GetText ("btn_guild_manage_delete");
			if (GuildInfoMgr.Instance.bGuildMaster) txtManage.text = TextManager.Instance.GetText ("btn_guild_manage_guildinfo");
			break;
		}

		if (!bInit) {
			InitList ();
		} else {
			RefreshList ();
		}
        tempStringBuilder.Append(GuildInfoMgr.Instance.cGuildMemberInfo.u1MemberCount).Append("/").Append(GuildInfoMgr.Instance.cGuildInfo.u1MaxMember);
        txtGuildMember.text = tempStringBuilder.ToString();
		CheckCenterBtn ();
	}

	void CheckCenterBtn()
	{
		switch (infoType) {
		case INFO_TYPE.ATTEND:
			if (Legion.Instance.ServerTime.ToString ("yy-MM-dd") == GuildInfoMgr.Instance.cGuildMemberInfo.GetMyInfo ().dtJoinDate.ToString ("yy-MM-dd")) {
				btnCenter.interactable = false;
				txtCenter.color = Color.gray;
			} else {
				if ((GuildInfoMgr.Instance.cGuildMemberInfo.u1RewardFlag & 0x02) != 0) {
					btnCenter.interactable = false;
					txtCenter.color = Color.gray;
				} else {
					btnCenter.interactable = true;
					txtCenter.color = Color.white;
				}
			}
			break;
		case INFO_TYPE.MANAGE:
			btnCenter.interactable = true;
			txtCenter.color = Color.white;
			break;
		}
	}

	public void AddMember()
	{
		GuildInfoMgr.Instance.cGuildMemberInfo.u1MemberCount++;
		txtGuildMember.text = GuildInfoMgr.Instance.cGuildMemberInfo.u1MemberCount+("/")+GuildInfoMgr.Instance.cGuildInfo.u1MaxMember;
	}

	public void SubMember()
	{
		GuildInfoMgr.Instance.cGuildMemberInfo.u1MemberCount--;
		txtGuildMember.text = GuildInfoMgr.Instance.cGuildMemberInfo.u1MemberCount+("/")+GuildInfoMgr.Instance.cGuildInfo.u1MaxMember;
	}
	
	void InitList(){
		for (int i = 0; i < GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Count; i++) {
			GameObject temp = Instantiate (InfoSlotPref) as GameObject;
			temp.transform.SetParent (ListParent);
			temp.transform.localScale = Vector3.one;
			temp.transform.localPosition = Vector3.zero;
			GuildInfoSlot scr = temp.GetComponent<GuildInfoSlot> ();
			scr.SetSlot (GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i], (int)infoType, GuildInfoMgr.Instance.bGuildMaster, this);
			lstSlots.Add(scr);
		}
		bInit = true;
	}

	public void RemoveSlot(GuildInfoSlot slot){
		lstSlots.Remove (slot);
	}

	public void ChangeType(int tType){
		switch (tType) {
		case 1:
			infoType = INFO_TYPE.ATTEND;
			if (!GuildInfoMgr.Instance.bGuildMaster) {
				btnPublic.enabled = false;
				txtManage.text = TextManager.Instance.GetText ("btn_guildinfo_secession");
			}
			txtCenter.text = TextManager.Instance.GetText ("btn_guildinfo_login_reward");
			txtTitle.text = TextManager.Instance.GetText ("popup_title_guild_guildinfo");
		break;
		case 2:
			infoType = INFO_TYPE.MANAGE;
			txtTitle.text = TextManager.Instance.GetText ("btn_guildinfo_guildmanage");
			txtCenter.text = TextManager.Instance.GetText ("btn_guild_manage_delete");
		break;
		}
		CheckCenterBtn ();
	}

	public void RefreshList(){
		for (int i = 0; i <GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Count; i++) {
			if (GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember [i].bMember) {
				if (i < lstSlots.Count) {
					lstSlots [i].SetSlot (GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember [i], (int)infoType, GuildInfoMgr.Instance.bGuildMaster, this);
					lstSlots [i].gameObject.SetActive (true);
				} else {
					GameObject temp = Instantiate (InfoSlotPref) as GameObject;
					temp.transform.SetParent (ListParent);
					temp.transform.localScale = Vector3.one;
					temp.transform.localPosition = Vector3.zero;
					GuildInfoSlot scr = temp.GetComponent<GuildInfoSlot> ();
					scr.SetSlot (GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i], (int)infoType, GuildInfoMgr.Instance.bGuildMaster, this);
					lstSlots.Add(scr);
				}
			} else {
				if (infoType == INFO_TYPE.ATTEND) {
					if (i < lstSlots.Count) {
						lstSlots [i].gameObject.SetActive (false);
					}
				} else {
					if (i < lstSlots.Count) {
						lstSlots [i].SetSlot (GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember [i], (int)infoType, GuildInfoMgr.Instance.bGuildMaster, this);
						lstSlots [i].transform.SetSiblingIndex (0);
						lstSlots [i].gameObject.SetActive(true);
					} else {
						GameObject temp = Instantiate (InfoSlotPref) as GameObject;
						temp.transform.SetParent (ListParent);
						temp.transform.localScale = Vector3.one;
						temp.transform.localPosition = Vector3.zero;
						temp.transform.SetSiblingIndex (0);
						GuildInfoSlot scr = temp.GetComponent<GuildInfoSlot> ();
						scr.SetSlot (GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember [i], (int)infoType, GuildInfoMgr.Instance.bGuildMaster, this);
						lstSlots.Add (scr);
					}
				}
			}
		}
	}

	public void OnClickClose(){
		PopupManager.Instance.RemovePopup(gameObject);
		gameObject.SetActive (false);
	}

	public void OnClickManage(){
		if (GuildInfoMgr.Instance.bGuildMaster) {
			switch (infoType) {
			case INFO_TYPE.ATTEND:
				PopupManager.Instance.ShowLoadingPopup (1);
				Server.ServerMgr.Instance.RequestGuildJoinList (ManageCallback);
				break;
			case INFO_TYPE.MANAGE:
				ChangeType((int)INFO_TYPE.ATTEND);
				txtManage.text = TextManager.Instance.GetText ("popup_title_guild_manage");
				txtCenter.text = TextManager.Instance.GetText ("btn_guildinfo_login_reward");
				RefreshList ();
				break;
			}
		} else {
			PopupManager.Instance.ShowYesNoPopup (TextManager.Instance.GetText ("popup_title_guild_secession"), TextManager.Instance.GetText ("desc_guild_guild_secession"), TextManager.Instance.GetText ("btn_guild_guild_secession"), GuildSignOut, null);
		}
	}

	void ManageCallback(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE) {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_REQUEST_JOIN_LIST, err), Server.ServerMgr.Instance.CallClear);
		} else {
			ChangeType((int)INFO_TYPE.MANAGE);
			txtManage.text = TextManager.Instance.GetText ("btn_guild_manage_guildinfo");
			txtCenter.text = TextManager.Instance.GetText ("btn_guild_manage_delete");
			RefreshList ();
			txtGuildMember.text = GuildInfoMgr.Instance.cGuildMemberInfo.u1MemberCount+("/")+GuildInfoMgr.Instance.cGuildInfo.u1MaxMember;

			GuildPanel panel = GameObject.FindObjectOfType<GuildPanel> ();
			if (panel != null) 
			{
				if (panel.MASTER_ENTRY) {
					panel.OnClickGuildEntryToggle ();
				} else {
					panel.OnClickEntryToggle ();
				}
			}
		}
	}

	public void OnClickPublic(){
		if (!GuildInfoMgr.Instance.bGuildMaster)
			return;

		if (CheckWaitingMamber ()) {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("err_title_guild_switch_condition_reject"), TextManager.Instance.GetText ("err_desc_guild_switch_condition_reject"), null);
			return;
		}

		if (GuildInfoMgr.Instance.cGuildMemberInfo.u1Public == 1) {
			PopupManager.Instance.ShowLoadingPopup (1);
			Server.ServerMgr.Instance.RequestGuildMark (6, PublicCallback);
		} else {
			PopupManager.Instance.ShowLoadingPopup (1);
			Server.ServerMgr.Instance.RequestGuildMark (5, PublicCallback);
		}
	}

	bool CheckWaitingMamber()
	{
		int idx = GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.FindIndex (cs => cs.bMember == false);

		if (idx > -1)
			return true;

		return false;
	}

	void PublicCallback(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE) {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_MARK, err), Server.ServerMgr.Instance.CallClear);
		} else {
			if (GuildInfoMgr.Instance.cGuildMemberInfo.u1Public == 1) {
				GuildInfoMgr.Instance.cGuildMemberInfo.u1Public = 2;
				txtPublic.text = TextManager.Instance.GetText ("btn_guild_close");
			} else {
				GuildInfoMgr.Instance.cGuildMemberInfo.u1Public = 1;
				txtPublic.text = TextManager.Instance.GetText ("btn_guild_open");
			}
		}
	}

	void DeleteCallback(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE) {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_MARK, err), Server.ServerMgr.Instance.CallClear);
		} else {
			Debug.LogError ("길드 삭제");
			GuildInfoMgr.Instance.InitUserData ();
			AssetMgr.Instance.SceneLoad("GuildScene");
		}
	}

	public void OnClickAttendReward(){
		switch (infoType) {
		case INFO_TYPE.ATTEND:
			int max = 0;
			for (int i = 0; i < GuildInfoMgr.Instance.cGuildLoginInfo.Count; i++) {
				if (GuildInfoMgr.Instance.cGuildMemberInfo.u1LastDailyCheckCount > GuildInfoMgr.Instance.cGuildLoginInfo [i].u1MemberCount)
					max = i+1;
			}

			if (max > GuildInfoMgr.Instance.cGuildLoginInfo.Count-1)
				max = GuildInfoMgr.Instance.cGuildLoginInfo.Count-1;

			Legion.Instance.AddGoods (GuildInfoMgr.Instance.cGuildLoginInfo [max].gReward);

			for (int i = 0; i < GuildInfoMgr.Instance.cGuildLoginInfo [max].gReward.Length; i++) {
				if (Legion.Instance.CheckGoodsLimitExcessx (GuildInfoMgr.Instance.cGuildLoginInfo [max].gReward [i])) {
					Legion.Instance.ShowGoodsOverMessage(GuildInfoMgr.Instance.cGuildLoginInfo [max].gReward [i].u1Type);
					return;
				}
			}

			PopupManager.Instance.ShowLoadingPopup (1);
			Server.ServerMgr.Instance.RequestGuildMark (2, RewardCallback);
			break;
		case INFO_TYPE.MANAGE:
			PopupManager.Instance.ShowYesNoPopup (TextManager.Instance.GetText ("popup_title_guild_delete"), TextManager.Instance.GetText ("desc_guild_delete"), TextManager.Instance.GetText ("btn_guild_delete"), DeleteGuild, null);
			break;
		}
	}

	void DeleteGuild(object[] param)
	{
		PopupManager.Instance.ShowLoadingPopup (1);
		Server.ServerMgr.Instance.RequestGuildMark (7, DeleteCallback);
	}

	void RewardCallback(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE) {
			if (err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET) {
				GuildInfoMgr.Instance.CheckGuildError(err);
                return;
			} else {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_MARK, err), Server.ServerMgr.Instance.CallClear);
			}
		} else {
			int max = 0;
			for (int i = 0; i < GuildInfoMgr.Instance.cGuildLoginInfo.Count; i++) {
				if (GuildInfoMgr.Instance.cGuildMemberInfo.u1LastDailyCheckCount > GuildInfoMgr.Instance.cGuildLoginInfo [i].u1MemberCount)
					max = i+1;
			}

			if (max > GuildInfoMgr.Instance.cGuildLoginInfo.Count-1)
				max = GuildInfoMgr.Instance.cGuildLoginInfo.Count-1;

			Legion.Instance.AddGoods (GuildInfoMgr.Instance.cGuildLoginInfo [max].gReward);

			int cnt = 0;
			string reward = "";
			for (int i = 0; i < GuildInfoMgr.Instance.cGuildLoginInfo [max].gReward.Length; i++) {
				if (GuildInfoMgr.Instance.cGuildLoginInfo [max].gReward [i].u1Type != 0) {
					if (cnt > 0) reward += "\n";
					if (GuildInfoMgr.Instance.cGuildLoginInfo [max].gReward [i].u1Type == (byte)GoodsType.CONSUME) {
						reward += TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (GuildInfoMgr.Instance.cGuildLoginInfo [max].gReward [i].u2ID).sName) + " " + GuildInfoMgr.Instance.cGuildLoginInfo [max].gReward [i].u4Count;
					} else {
						reward += Legion.Instance.GetConsumeString (GuildInfoMgr.Instance.cGuildLoginInfo [max].gReward [i].u1Type) + " " + GuildInfoMgr.Instance.cGuildLoginInfo [max].gReward [i].u4Count;
					}
					cnt++;
				}
			}

			GuildInfoMgr.Instance.cGuildMemberInfo.u1RewardFlag += 0x02;
			CheckCenterBtn ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("btn_guildinfo_login_reward"),
				string.Format(TextManager.Instance.GetText ("desc_guild_login_reward_member"), GuildInfoMgr.Instance.cGuildMemberInfo.u1LastDailyCheckCount) + "\n\n<color=#ffff00>" + reward
				+"</color>", null);
			Debug.LogError ("출석 보상");
		}
	}

	void GuildSignOut(object[] param){
		PopupManager.Instance.ShowLoadingPopup (1);
		Server.ServerMgr.Instance.RequestGuildMark (4, SignOutCallback);
	}

	void SignOutCallback(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE) {
			if (err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET)
            {
				GuildInfoMgr.Instance.CheckGuildError(err);
                return;
			}
            else if(err == Server.ERROR_ID.GUILD_REQUEST_CONFLICT)
            {
                GuildInfoMgr.Instance.CheckGuildError(err);
                return;
            }
            else
            {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_MARK, err), Server.ServerMgr.Instance.CallClear);
			}
		} else {
			Debug.LogError ("길드 탈퇴");
			GuildInfoMgr.Instance.InitUserData ();
            //#CHATTING
            // 길드 채팅 서버 연결 해제
            PopupManager.Instance.GuildChatDisconnect();
			AssetMgr.Instance.SceneLoad("GuildScene");
		}
	}

	public void ShowDetailPopup()
	{
		objDetailPopup.SetActive (true);
	}
}
