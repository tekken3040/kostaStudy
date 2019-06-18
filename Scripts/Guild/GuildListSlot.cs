using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class GuildListSlot : MonoBehaviour
{
    [SerializeField] Text txtGuildTier;
    [SerializeField] Text txtGuildName;
    [SerializeField] Text txtGuildMemberCnt;
	[SerializeField] Text txtJoinRequest;

    [SerializeField] Image imgGuildTier;

    [SerializeField] Button btnJoin;

    private GuildList _guildInfo;
    private StringBuilder tempStringBuilder;

    public void SetData(GuildList guildInfo)
    {
        tempStringBuilder = new StringBuilder();
        _guildInfo = guildInfo;
        txtGuildTier.text = GuildInfoMgr.Instance.GetGuildTier(_guildInfo.u2Score).ToString();
        txtGuildName.text = _guildInfo.strGuildName;
		tempStringBuilder.Append(_guildInfo.u1MemberCount).Append("/").Append(GuildInfoMgr.Instance.cGuildInfo.u1MaxMember);
        txtGuildMemberCnt.text = tempStringBuilder.ToString();
		//if (_guildInfo.u1Public == 1) {
		//	txtJoinRequest.text = TextManager.Instance.GetText ("guild_join");
		//} else {
		//	txtJoinRequest.text = TextManager.Instance.GetText ("guild_join_request");
		//}
        //티어 이미지는 데이터 나오고 추가
        //imgGuildTier.sprite = AtlasMgr.Instance.GetSprite(
    }

    public void OnClickJoin()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestGuildJoin(_guildInfo.u8GuildSN, AckGuildJoin);
    }

    private void AckGuildJoin(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			
			if(err == Server.ERROR_ID.FRIEND_REQUEST_DELETED || err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET)
            {
                GuildInfoMgr.Instance.CheckGuildError(err);
                return;
            }
			else if(err == Server.ERROR_ID.GUILD_REQUEST_ALREADY)
			{
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetErrorText("GUILD_REQUEST_ALREADY", "", false), Server.ServerMgr.Instance.CallClear);
				return;
			}
			else if(err == Server.ERROR_ID.FRIEND_CANCELED)
			{
				GuildInfoMgr.Instance.bRefreshGuild = true;
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("err_title_guild_join"), TextManager.Instance.GetText("err_desc_guild_join"), ReloadGuildScene);
				return;
			}
			else if(err == Server.ERROR_ID.FRIEND_OTHER_FULL)
			{
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("err_title_guild_member_full"), TextManager.Instance.GetText("err_desc_guild_member_full"), Server.ServerMgr.Instance.CallClear);
				return;
			}
            else
            {
			    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_REQUEST_JOIN, err), Server.ServerMgr.Instance.CallClear);
                return;
            }
		}
		else
		{
            if (GuildInfoMgr.Instance.u8GuildSN > 0)
            {
				//GuildInfoMgr.Instance.u1GuildCrewIndex = Legion.Instance.cBestCrew.u1Index;
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_guild_join_request"), TextManager.Instance.GetText("err_guild_already_success"), JoinGuild);
                return;
			}
            else
            {
                if (_guildInfo.u1Public == 1)
                {
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("err_title_guild_free_join_reject"), TextManager.Instance.GetText("err_desc_guild_free_join_reject"), null);
                }
                else
                {
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_guild_join_request"), TextManager.Instance.GetText("popup_desc_guild_join_request"), null);
                }
			}
        }
    }

    private void JoinGuild(object[] param)
    {
        //#CHATTING
        //길드 채팅 연결
        PopupManager.Instance.GuildChatConnect();
		AssetMgr.Instance.SceneLoad("GuildScene");
    }

	private void ReloadGuildScene(object[] param)
	{
		Server.ServerMgr.Instance.ClearFirstJobError();
		AssetMgr.Instance.SceneLoad("GuildScene");
	}
}
