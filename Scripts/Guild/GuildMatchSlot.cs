using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GuildMatchSlot : MonoBehaviour
{
    [SerializeField] Text txtGuildTier;
    [SerializeField] Text txtGuildName;
    [SerializeField] Text txtGuildPower;

    [SerializeField] Image imgGuildTier;

    GuildMatchList _list;
    GuildPanel _parent;

    public void SetData(GuildMatchList list, GuildPanel parent)
    {
        _list = list;
        _parent = parent;

        txtGuildName.text = _list.strGuildName;
        txtGuildPower.text = _list.u8Power.ToString();
        txtGuildTier.text = GuildInfoMgr.Instance.GetGuildTier(_list.u2Score).ToString();
    }

    public void OnClickMatch()
    {
		if (GuildInfoMgr.Instance.cGuildMemberInfo.dtRewardEndTime <= Legion.Instance.ServerTime) {
			_parent.objCalculate.SetActive (true);
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_guild_calculate"), TextManager.Instance.GetText ("popup_desc_guild_calculate"), null);
			_parent.objGuildBattleList.SetActive (false);
			return;
		}

        TimeSpan tsLeftTime = Legion.Instance.ServerTime - GuildInfoMgr.Instance.MyGuildInfo.dtJoinDate;
        if(tsLeftTime.TotalDays == 0)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_guild_battle_reject"), TextManager.Instance.GetText("popup_desc_guild_battle_reject2"), null);
            return;
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        if(GuildInfoMgr.Instance.bDirty)
        {
            UInt64[] tempSN = new UInt64[3];

            for(int i=0; i<3; i++)
            {
                if(_parent.GetComponent<GuildPanel>().MASTER_ENTRY)
                {
                    if(GuildInfoMgr.Instance.dicMasterEntry.ContainsKey((Byte)(i+1)))
                        tempSN[i] = GuildInfoMgr.Instance.dicMasterEntry[(Byte)(i+1)].u8UserSN;
                    else
                        tempSN[i] =  0;
                    }
                else
                {
                    if(GuildInfoMgr.Instance.dicUserEntry.ContainsKey((Byte)(i+1)))
                        tempSN[i] = GuildInfoMgr.Instance.dicUserEntry[(Byte)(i+1)].u8UserSN;
                    else
                        tempSN[i] =  0;
                }
            }

            if(_parent.GetComponent<GuildPanel>().MASTER_ENTRY)
                Server.ServerMgr.Instance.RequestGuildSetCrew((Byte)GuildInfoMgr.SET_CREW_TYPE.GuildMaster, tempSN[0], tempSN[1], tempSN[2], 0, AckSelectCrew);
            else
                Server.ServerMgr.Instance.RequestGuildSetCrew((Byte)GuildInfoMgr.SET_CREW_TYPE.UserCustom, tempSN[0], tempSN[1], tempSN[2], 0, AckSelectCrew);
        }
        else
            Server.ServerMgr.Instance.RequestGuildMatch(_list.u8GuildSN, AckGuildMatch);
    }

    private void AckSelectCrew(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_SET_CREW, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
            GuildInfoMgr.Instance.bDirty = false;
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.RequestGuildMatch(_list.u8GuildSN, AckGuildMatch);
        }
    }

    private void AckGuildMatch(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
            if(err == Server.ERROR_ID.FRIEND_REQUEST_DELETED || err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET)
            {
                GuildInfoMgr.Instance.CheckGuildError(err);
                return;
            }
            else
            {
			    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_MATCH, err), Server.ServerMgr.Instance.CallClear);
                return;
            }
		}
		else
		{
			GuildInfoMgr.Instance.cGuildMatchData.u2Score = GuildInfoMgr.Instance.lstGuildMatchList.Find (cs => cs.u8GuildSN == _list.u8GuildSN).u2Score;
            _parent.ShowMatchPopup();
        }
    }
}
