using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class GuildMatchPopup : MonoBehaviour
{
    [SerializeField] Text txtMyGuildName;
    [SerializeField] Text txtEnemyGuildName;
    [SerializeField] Text txtMyGuildPower;
    [SerializeField] Text txtEnemyGuildPower;

    [SerializeField] GameObject[] myTeam;
    [SerializeField] GameObject[] enemyTeam;

    StringBuilder tempStringBuilder;

    private void OnEnable()
    {
        PopupManager.Instance.AddPopup(this.gameObject, OnClickBack);
        InitData();
    }

    private void InitData()
    {
        tempStringBuilder = new StringBuilder();

        txtMyGuildName.text = GuildInfoMgr.Instance.cGuildMemberInfo.strGuildName;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(TextManager.Instance.GetText("mark_power")).Append(" ").Append(GuildInfoMgr.Instance.GetUserDeckPower().ToString());
        txtMyGuildPower.text = tempStringBuilder.ToString();

        txtEnemyGuildName.text = GuildInfoMgr.Instance.cGuildMatchData.strMatchingGuildName;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_power")).Append(" ").Append(GuildInfoMgr.Instance.cGuildMatchData.u8MatchingGuildPower.ToString());
        txtEnemyGuildPower.text = tempStringBuilder.ToString();
        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            if(GuildInfoMgr.Instance.cUserCrews[0].acLocation[i] != null)
            {
                myTeam[i].GetComponent<UI_CharCursorElement>().SetData((Hero)GuildInfoMgr.Instance.cUserCrews[0].acLocation[i]);
                myTeam[i].SetActive(true);
            }
            else
                myTeam[i].SetActive(false);
            if(GuildInfoMgr.Instance.cUserCrews[1].acLocation[i] != null)
            {
                myTeam[i+3].GetComponent<UI_CharCursorElement>().SetData((Hero)GuildInfoMgr.Instance.cUserCrews[1].acLocation[i]);
                myTeam[i+3].SetActive(true);
            }
            else
                myTeam[i+3].SetActive(false);
            if(GuildInfoMgr.Instance.cUserCrews[2].acLocation[i] != null)
            {
                myTeam[i+6].GetComponent<UI_CharCursorElement>().SetData((Hero)GuildInfoMgr.Instance.cUserCrews[2].acLocation[i]);
                myTeam[i+6].SetActive(true);
            }
            else
                myTeam[i+6].SetActive(false);

            if(GuildInfoMgr.Instance.cEnemyCrews[0].acLocation[i] != null)
            {
                enemyTeam[i].GetComponent<UI_CharCursorElement>().SetData((Hero)GuildInfoMgr.Instance.cEnemyCrews[0].acLocation[i]);
                enemyTeam[i].SetActive(true);
            }
            else
                enemyTeam[i].SetActive(false);
            if(GuildInfoMgr.Instance.cEnemyCrews[1].acLocation[i] != null)
            {
                enemyTeam[i+3].GetComponent<UI_CharCursorElement>().SetData((Hero)GuildInfoMgr.Instance.cEnemyCrews[1].acLocation[i]);
                enemyTeam[i+3].SetActive(true);
            }
            else
                enemyTeam[i+3].SetActive(false);
            if(GuildInfoMgr.Instance.cEnemyCrews[2].acLocation[i] != null)
            {
                enemyTeam[i+6].GetComponent<UI_CharCursorElement>().SetData((Hero)GuildInfoMgr.Instance.cEnemyCrews[2].acLocation[i]);
                enemyTeam[i+6].SetActive(true);
            }
            else
                enemyTeam[i+6].SetActive(false);
        }
    }

    public void OnClickStart()
    {
		if (GuildInfoMgr.Instance.cGuildMemberInfo.dtRewardEndTime <= Legion.Instance.ServerTime) {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_guild_calculate"), TextManager.Instance.GetText ("popup_desc_guild_calculate"), null);
			return;
		} 

        if(GuildInfoMgr.Instance.GuildKey < 1)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_guild_battle_reject"), TextManager.Instance.GetText("popup_desc_guild_battle_reject"), null);
            return;
        }
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequestGuildStartMatch (ResultStartMatch);
    }

	private void ResultStartMatch(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			if(err == Server.ERROR_ID.GUILD_REQUEST_CONFLICT || err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET || err == Server.ERROR_ID.GUILD_DATA_DENIED)
			{
				GuildInfoMgr.Instance.CheckGuildError(err);
				return;
			}

			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_START_MATCH, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
            GuildInfoMgr.Instance.GuildKey -= GuildInfoMgr.Instance.cGuildInfo.u1UseKey;
			StartCoroutine(ChangeScene());
		}
	}

	private IEnumerator ChangeScene()
	{
		FadeEffectMgr.Instance.FadeOut(1f);
		yield return new WaitForSeconds(1f);
		AssetMgr.Instance.SceneLoad("GuildBattle");
	}

    public void OnClickBack()
    {
        PopupManager.Instance.RemovePopup(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
