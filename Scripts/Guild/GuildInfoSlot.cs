using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class GuildInfoSlot : MonoBehaviour {

	public Text txtCrewPower;
	public Text txtCrewName;
	public Text txtLoginCheck;
	public Button btnLoginCheck;
	public GameObject objLoginCheck;

	public Text txtSignOut;
	public GameObject objSignOut;

	public GameObject objDetail;

	public Image imgMe;
	public Image imgMe2;
	public Image imgMaster;

	GuildMember memberInfo;
	bool bGuildMaster;

	int iSlotType = 0;

	ColorBlock grayBlock;
	ColorBlock whiteBlock;

	GuildInfoPopup parentScript;

	void Awake(){
		grayBlock.normalColor = Color.white;
		grayBlock.highlightedColor = Color.white;
		grayBlock.pressedColor = new Color(0.8f, 0.8f, 0.8f);
		grayBlock.disabledColor = Color.gray;
		grayBlock.colorMultiplier = 1;
		grayBlock.fadeDuration = 0.1f;

		whiteBlock.normalColor = Color.white;
		whiteBlock.highlightedColor = Color.white;
		whiteBlock.pressedColor = new Color(0.8f, 0.8f, 0.8f);
		whiteBlock.disabledColor = Color.white;
		whiteBlock.colorMultiplier = 1;
		whiteBlock.fadeDuration = 0.1f;
	}

	public void SetSlot(GuildMember member, int slotType, bool bMaster, GuildInfoPopup prScr){
		memberInfo = new GuildMember ();
		memberInfo = member;
		iSlotType = slotType;
		bGuildMaster = bMaster;
		parentScript = prScr;
		txtCrewPower.text = member.u8Power.ToString ();
		txtCrewName.text = member.strLegionName;
		switch (slotType) {
		case 1:
			SetAttend ();
			break;
		case 2:
			if (bMaster) {
				SetManage ();
			} else {
				DebugMgr.LogError ("I'm Not Master");
			}
			break;
		}
	}

	void SetAttend(){
		if (Legion.Instance.sName == memberInfo.strLegionName) {
			imgMe.enabled = true;
			imgMe2.enabled = true;
			objDetail.SetActive (false);

			if (Legion.Instance.ServerTime.ToString("yy-MM-dd") == GuildInfoMgr.Instance.cGuildMemberInfo.GetMyInfo ().dtJoinDate.ToString("yy-MM-dd")) {
				btnLoginCheck.enabled = true;
				txtLoginCheck.text = TextManager.Instance.GetText ("btn_guildinfo_login");
				txtLoginCheck.color = Color.white;
				btnLoginCheck.colors = whiteBlock;
			} else {
				if ((memberInfo.u1Option & 0x08) != 0) {
					btnLoginCheck.enabled = false;
					txtLoginCheck.text = TextManager.Instance.GetText ("btn_guildinfo_login_finish");
					txtLoginCheck.color = Color.gray;
					btnLoginCheck.colors = grayBlock;
				} else {
					btnLoginCheck.enabled = true;
					txtLoginCheck.text = TextManager.Instance.GetText ("btn_guildinfo_login");
					txtLoginCheck.color = Color.white;
					btnLoginCheck.colors = whiteBlock;
				}
			}
		} else {
			imgMe.enabled = false;
			imgMe2.enabled = false;
			objDetail.SetActive (true);
			btnLoginCheck.enabled = false;
			if ((memberInfo.u1Option & 0x08) != 0) {
				txtLoginCheck.text = TextManager.Instance.GetText ("btn_guildinfo_login_finish");
				txtLoginCheck.color = Color.gray;
				btnLoginCheck.colors = grayBlock;
			} else {
				txtLoginCheck.text = TextManager.Instance.GetText ("btn_guildinfo_login");
				txtLoginCheck.color = Color.white;
				btnLoginCheck.colors = whiteBlock;
			}
		}

		if ((memberInfo.u1Option & 0x10) != 0) {
			imgMaster.enabled = true;
		} else {
			imgMaster.enabled = false;
		}

		objLoginCheck.SetActive (true);
		objSignOut.SetActive (false);
	}

	void SetManage(){
		imgMe2.enabled = false;
		objDetail.SetActive (false);
		objSignOut.SetActive (true);
		objLoginCheck.SetActive (true);
		btnLoginCheck.enabled = true;
		txtLoginCheck.color = Color.white;
		if (memberInfo.bMember) {
			txtLoginCheck.text = TextManager.Instance.GetText ("btn_guild_manage_mandate");
			txtSignOut.text = TextManager.Instance.GetText ("btn_guild_manage_make_secession");
		}else{
			txtLoginCheck.text = TextManager.Instance.GetText ("btn_guild_manage_approve");
			txtSignOut.text = TextManager.Instance.GetText ("btn_guild_manage_reject");
		}

		if (Legion.Instance.sName == memberInfo.strLegionName) {
			imgMe.enabled = true;
			objSignOut.SetActive (false);
			objLoginCheck.SetActive (false);
		} else {
			imgMe.enabled = false;
		}
	}

	public void OnClickBlue(){
		switch (iSlotType) {
		case 1:
			if (Legion.Instance.ServerTime.ToString ("yy-MM-dd") == GuildInfoMgr.Instance.cGuildMemberInfo.GetMyInfo ().dtJoinDate.ToString ("yy-MM-dd")) {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_guild_login_reject"), TextManager.Instance.GetText ("desc_title_guild_login_reject"), null);
				return;
			}

			PopupManager.Instance.ShowLoadingPopup (1);
			Server.ServerMgr.Instance.RequestGuildMark (1, AttendCallback);
			break;
		case 2:
			if (bGuildMaster) 
			{
				if (memberInfo.bMember) {
					PopupManager.Instance.ShowYesNoPopup (TextManager.Instance.GetText ("btn_guild_manage_mandate"), TextManager.Instance.GetText ("desc_guild_guild_mandate"), TextManager.Instance.GetText ("btn_guild_guild_mandate"), ChangeMaster, null);
				} else {
					if (GuildInfoMgr.Instance.cGuildMemberInfo.u1MemberCount >= GuildInfoMgr.Instance.cGuildInfo.u1MaxMember) {
						PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("btn_guildinfo_guildmanage"), TextManager.Instance.GetText ("err_desc_guild_member_full"), null);
						return;
					}

					PopupManager.Instance.ShowLoadingPopup (1);
					Server.ServerMgr.Instance.RequestGuildSetMember (1, memberInfo.u8UserSN, AcceptCallback);
				}
			} else {
				DebugMgr.LogError ("I'm Not Master");
			}
			break;
		}
	}

	void AttendCallback(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE) {
            if (err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET) {
                GuildInfoMgr.Instance.CheckGuildError(err);
                return;
            }else {
			    PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_MARK, err), Server.ServerMgr.Instance.CallClear);
            }
        } else {
			txtLoginCheck.text = TextManager.Instance.GetText ("btn_guildinfo_login_finish");
			memberInfo.u1Option += 0x08;
			btnLoginCheck.enabled = false;
			txtLoginCheck.color = Color.gray;
			btnLoginCheck.colors = grayBlock;
		}
	}

	void ChangeMaster(object[] param){
		PopupManager.Instance.ShowLoadingPopup (1);
		Server.ServerMgr.Instance.RequestGuildSetMember (4, memberInfo.u8UserSN, ChangeCallback);
	}

	void ChangeCallback(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE) {
			if (err == Server.ERROR_ID.FRIEND_REQUEST_DELETED) {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("err_title_guild_absent"), TextManager.Instance.GetText ("err_guild_already_exit_member"), Server.ServerMgr.Instance.CallClear);

				DeleteThisMember ();
			} else {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_SET_MEMBER, err), Server.ServerMgr.Instance.CallClear);
			}
		} else {
			GuildInfoMgr.Instance.bGuildMaster = false;
			Byte tempVal = 0x10;
			GuildInfoMgr.Instance.cGuildMemberInfo.GetMyInfo ().u1Option -= tempVal;
			GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Find (cs => cs.u8UserSN == memberInfo.u8UserSN).u1Option += tempVal;
			parentScript.ChangeType (1);
			parentScript.RefreshList ();
		}
	}

	void AcceptCallback(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE) {
			if (err == Server.ERROR_ID.FRIEND_REQUEST_DELETED) {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetText ("err_guild_already_exit_member"), Server.ServerMgr.Instance.CallClear);

				GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Remove (memberInfo);
				parentScript.RemoveSlot(this);
				Destroy (gameObject);
			} else {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_SET_MEMBER, err), Server.ServerMgr.Instance.CallClear);
			}
		} else {
			GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Find (cs => cs.u8UserSN == memberInfo.u8UserSN).bMember = true;
			parentScript.AddMember ();
			parentScript.RefreshList ();
		}
	}

	public void OnClickRed(){
		switch (iSlotType) {
		case 1:
			//PopupManager.Instance.ShowLoadingPopup (1);
			//Server.ServerMgr.Instance.RequestGuildMark (1, AttendCallback);
			break;
		case 2:
			if (bGuildMaster) 
			{
				if (memberInfo.bMember) {
					PopupManager.Instance.ShowYesNoPopup (TextManager.Instance.GetText ("popup_title_guild_make_secession"), TextManager.Instance.GetText ("desc_guild_make_secession"), TextManager.Instance.GetText ("btn_guild_make_secession"), ForceOut, null);
				} else {
					PopupManager.Instance.ShowLoadingPopup (1);
					Server.ServerMgr.Instance.RequestGuildSetMember (2, memberInfo.u8UserSN, RejectCallback);
				}
			} else {
				DebugMgr.LogError ("I'm Not Master");
			}
			break;
		}
	}

	string OnlyDeckUser()
	{
		if (GuildInfoMgr.Instance.dicMasterEntry.Count == 1) {
			return GuildInfoMgr.Instance.dicMasterEntry.First().Value.strLegionName;
		}

		return "";
	}

	void ForceOut(object[] param){
		if (memberInfo.strLegionName == OnlyDeckUser ()) {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("btn_guild_manage_make_secession"), TextManager.Instance.GetText ("desc_guild_manage_make_secession_fail"), null);
			return;
		}

		PopupManager.Instance.ShowLoadingPopup (1);
		Server.ServerMgr.Instance.RequestGuildSetMember (3, memberInfo.u8UserSN, ForceCallback);
	}

	void ForceCallback(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE) {
			if (err == Server.ERROR_ID.FRIEND_REQUEST_DELETED) {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("err_title_guild_absent"), TextManager.Instance.GetText ("err_guild_already_exit_member"), Server.ServerMgr.Instance.CallClear);

				DeleteThisMember ();
			} else {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_SET_MEMBER, err), Server.ServerMgr.Instance.CallClear);
			}
		} else {
			GuildPanel panel = GameObject.FindObjectOfType<GuildPanel> ();
			if(panel != null)
			{
				byte masterkey = GuildInfoMgr.Instance.dicMasterEntry.FirstOrDefault (cs => cs.Value.strLegionName == memberInfo.strLegionName).Key;
				byte userkey = GuildInfoMgr.Instance.dicUserEntry.FirstOrDefault (cs => cs.Value.strLegionName == memberInfo.strLegionName).Key;
				if (panel.MASTER_ENTRY) {
					if (masterkey > 0) {
						panel.objEntrySlot [masterkey - 1].GetComponent<GuildEntrySlot> ().SetEmpty (false);
						GuildInfoMgr.Instance.dicMasterEntry.Remove (masterkey);
						if(userkey > 0) GuildInfoMgr.Instance.dicUserEntry.Remove (userkey);
					}
				} else {
					if (userkey > 0) {
						panel.objEntrySlot [userkey - 1].GetComponent<GuildEntrySlot> ().SetEmpty (false);
						GuildInfoMgr.Instance.dicUserEntry.Remove (userkey);
						if(masterkey > 0) GuildInfoMgr.Instance.dicMasterEntry.Remove (masterkey);
					}
				}
			}
			DeleteThisMember ();
		}
	}

	void DeleteThisMember()
	{
		if(memberInfo.bMember) parentScript.SubMember ();
		GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Remove (memberInfo);
		parentScript.RemoveSlot(this);
		Destroy (gameObject);
	}

	void RejectCallback(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE) {
			if (err == Server.ERROR_ID.FRIEND_REQUEST_DELETED) {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_SET_MEMBER, err), Server.ServerMgr.Instance.CallClear);

				GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Remove (memberInfo);
				parentScript.RemoveSlot(this);
				Destroy (gameObject);
			} else {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_SET_MEMBER, err), Server.ServerMgr.Instance.CallClear);
			}
		} else {
			DeleteThisMember ();
		}
	}

	public void OnClickDetail(){
		PopupManager.Instance.ShowLoadingPopup (1);
		Server.ServerMgr.Instance.RequestGuildSetMember (5, memberInfo.u8UserSN, DetailCallback);
	}

	void DetailCallback(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE) {
			if (err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET) {
				GuildInfoMgr.Instance.CheckGuildError(err);
                return;
			} else if (err == Server.ERROR_ID.FRIEND_REQUEST_DELETED) {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("err_title_guild_absent"), TextManager.Instance.GetText ("err_guild_already_exit_member"), Server.ServerMgr.Instance.CallClear);

				DeleteThisMember ();
			} else {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.GUILD_SET_MEMBER, err), Server.ServerMgr.Instance.CallClear);
			}
		} else {
			parentScript.ShowDetailPopup ();
		}
	}
}
