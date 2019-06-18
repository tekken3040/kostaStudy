using UnityEngine;
using UnityEngine.UI;
using System;

public class GuildRankSlot : MonoBehaviour
{
    [SerializeField] Text txtName;
    [SerializeField] Text txtPower;
    [SerializeField] Text txtRank;
    [SerializeField] Text txtScore;

    [SerializeField] Image imgFlag;
    [SerializeField] Image imgFlag2;

    GuildRankPopup cParent;
    GuildRankList cData;

    public void SetData(GuildRankList _data, GuildRankPopup _parent)
    {
        cData = _data;
        cParent = _parent;

        txtName.text = cData.strGuildName;
        txtPower.text = cData.u8Power.ToString();
        txtRank.text = cData.u8Rank.ToString();
        txtScore.text = GuildInfoMgr.Instance.GetGuildTier(cData.u2Score).ToString();
    }

    public void OnClickDetails()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestGuildRankInfo((Byte)GuildInfoMgr.GUILD_RANK_TYPE.RankDetail, cData.u8GuildSN, AckRankDetail);
    }

    private void AckRankDetail(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_SET_CREW, err), Server.ServerMgr.Instance.CallClear);
            return;
        }
        else
        {
            cParent.ShowDetail();
        }
    }
}
