using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class GuildBattleInfoSlot : MonoBehaviour
{
    public Text txtCrewName;
    public Text txtBattleCnt;
    public Text txtLeftTime;

    public Image imgMe;
	public Image imgMe2;
	public Image imgMaster;

    GuildMember cData;
    StringBuilder tempStringBuider;
    Int64 timeTicks;

    public void SetData(GuildMember _data)
    {
        tempStringBuider = new StringBuilder();
        cData = _data;

        if (Legion.Instance.sName == cData.strLegionName)
        {
			imgMe.enabled = true;
			imgMe2.enabled = true;
        }
        else
        {
            imgMe.enabled = false;
			imgMe2.enabled = false;
        }
        if ((cData.u1Option & 0x10) != 0)
			imgMaster.enabled = true;
		else
			imgMaster.enabled = false;

        tempStringBuider.Append(String.Format(TextManager.Instance.GetText("btn_guild_battle_count"), cData.u2LeagueCount));
        txtBattleCnt.text = tempStringBuider.ToString();

        DateTime dtTime = DateTime.FromBinary(cData.u8LastLogin);
        timeTicks = Legion.Instance.ServerTime.Ticks - dtTime.Ticks;
        TimeSpan timespan = new TimeSpan(timeTicks);
        if(timespan.Days > 0)
            txtLeftTime.text = string.Format(TextManager.Instance.GetText("mark_time_left_day"), timespan.Days);
        else
            txtLeftTime.text = string.Format(TextManager.Instance.GetText("mark_time_left_hour"), timespan.Hours,timespan.Minutes );
        txtCrewName.text = cData.strLegionName;
    }
}
