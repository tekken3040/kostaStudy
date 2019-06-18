using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class GuildBattleInfoPopup : MonoBehaviour
{
    public Text txtMemberCnt;
    public Text txtGuildName;

    public Button btn_Reward;

    public GameObject objScrollList;
    public GameObject objRewardPopup;

    StringBuilder tempStringBuilder;
    GuildPanel _cGuildPanel;

    private void Awake()
    {
        _cGuildPanel = GameObject.Find("GuildPanel").GetComponent<GuildPanel>();
    }
    private void OnEnable()
    {
        PopupManager.Instance.AddPopup(this.gameObject, OnClickClose);
        tempStringBuilder = new StringBuilder();
        InitData();
    }

    private void InitData()
    {
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(GuildInfoMgr.Instance.cGuildMemberInfo.u1MemberCount).Append("/").Append(GuildInfoMgr.Instance.cGuildInfo.u1MaxMember);
        txtMemberCnt.text = tempStringBuilder.ToString();

        txtGuildName.text = GuildInfoMgr.Instance.cGuildMemberInfo.strGuildName;

        for(int i=0; i<GuildInfoMgr.Instance.cGuildMemberInfo.u1MemberCount; i++)
        {
            GameObject tempObj = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Guild/GuildBattleInfoSlot_.prefab", typeof(GameObject)) as GameObject);
            tempObj.transform.SetParent(objScrollList.transform);
            tempObj.transform.localPosition = Vector3.zero;
            tempObj.transform.localScale = Vector3.one;
            tempObj.GetComponent<GuildBattleInfoSlot>().SetData(GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i]);
        }

        if((GuildInfoMgr.Instance.cGuildMemberInfo.u1RewardFlag & 0x04) == 0)
        {
            //TimeSpan tsLeftTime = Legion.Instance.ServerTime - GuildInfoMgr.Instance.MyGuildInfo.dtJoinDate;
            TimeSpan tsLeftTime = Legion.Instance.ServerTime - GuildInfoMgr.Instance.cGuildMemberInfo.dtRewardBeginTime;
            //if(tsLeftTime.TotalDays > 1 && GuildInfoMgr.Instance.GetDays(Legion.Instance.ServerTime.DayOfWeek.ToString()) >= GuildInfoMgr.Instance.cGuildInfo.u1StartDay)
            if(tsLeftTime.TotalDays > 0)
            {
                btn_Reward.interactable = true;
            }
        }
        else
        {
            btn_Reward.interactable = false;
        }
    }

    public void OnClickGuildInfo()
    {
        _cGuildPanel.OnClickGuildInfo();
        OnClickClose();
    }

    public void OnClickReward()
    {
        TimeSpan tsLeftTime = Legion.Instance.ServerTime - GuildInfoMgr.Instance.cGuildMemberInfo.dtRewardBeginTime;
        //if(tsLeftTime.TotalDays <7)
        if(tsLeftTime.TotalDays < 1)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_guild_battle_reward_reject"), TextManager.Instance.GetText("popup_desc_guild_battle_reward_reject"), null);
            return;
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestGuildMark(3, AckGuildReward);
    }

    private void AckGuildReward(Server.ERROR_ID err)
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
			    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_INFO, err), Server.ServerMgr.Instance.CallClear);
                return;
            }
		}
		else
		{
            UInt16 tempTier = GuildInfoMgr.Instance.GetGuildTier(GuildInfoMgr.Instance.cGuildMemberInfo.u2LastScore);
            UInt16 tempDivisionId = GuildInfoMgr.Instance.GetDivisionRewardID(tempTier);
            UInt16 tempRankId = GuildInfoMgr.Instance.GetRankRewardID(tempTier);
            for(int i=0; i<GuildInfoMgr.Instance.dicGuildLeague[tempDivisionId].gReward.Length; i++)
            {
                Legion.Instance.AddGoods(GuildInfoMgr.Instance.dicGuildLeague[tempDivisionId].gReward[i]);
            }
            GuildInfoMgr.Instance.cGuildMemberInfo.u1RewardFlag += 0x04;
            objRewardPopup.SetActive(true);
            if(GuildInfoMgr.Instance.cGuildMemberInfo.u8LastRank <= 100 && GuildInfoMgr.Instance.cGuildMemberInfo.u8LastRank >= 1)
                objRewardPopup.GetComponent<GuildRewardPopup>().SetData(GuildInfoMgr.Instance.dicGuildLeague[tempDivisionId], GuildInfoMgr.Instance.dicGuildLeague[tempDivisionId].u1RankType, this, true);
            else
                objRewardPopup.GetComponent<GuildRewardPopup>().SetData(GuildInfoMgr.Instance.dicGuildLeague[tempDivisionId], GuildInfoMgr.Instance.dicGuildLeague[tempDivisionId].u1RankType, this, false);
        }
    }

    public void ShowNextReward()
    {
        UInt16 tempTier = GuildInfoMgr.Instance.GetGuildTier(GuildInfoMgr.Instance.cGuildMemberInfo.u2LastScore);
        UInt16 tempDivisionId = GuildInfoMgr.Instance.GetDivisionRewardID(tempTier);
        UInt16 tempRankId = GuildInfoMgr.Instance.GetRankRewardID(tempTier);

        if(GuildInfoMgr.Instance.cGuildMemberInfo.u8LastRank <= 100)
        {
            for(int i=0; i<GuildInfoMgr.Instance.dicGuildLeague[tempRankId].gReward.Length; i++)
            {
                Legion.Instance.AddGoods(GuildInfoMgr.Instance.dicGuildLeague[tempRankId].gReward[i]);
            }
        }
        objRewardPopup.SetActive(true);
        objRewardPopup.GetComponent<GuildRewardPopup>().SetData(GuildInfoMgr.Instance.dicGuildLeague[tempRankId], GuildInfoMgr.Instance.dicGuildLeague[tempRankId].u1RankType, this, false);
    }

    public void OnClickClose()
    {
        for(int i=0; i<objScrollList.transform.childCount; i++)
        {
            Destroy(objScrollList.transform.GetChild(i).gameObject);
        }
        PopupManager.Instance.RemovePopup(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
