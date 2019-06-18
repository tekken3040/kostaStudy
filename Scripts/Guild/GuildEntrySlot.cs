using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class GuildEntrySlot : MonoBehaviour
{
    [SerializeField] Text txtCrewPower;    
	[SerializeField] Text txtCrewEmpty;

    [SerializeField] GameObject objCrewEmpty;
    [SerializeField] GameObject objCrewPower;

    [SerializeField] Text[] txtCharLevel;
    [SerializeField] Image[] imgClassIcon;
    [SerializeField] Image[] imgElementIcon;
    [SerializeField] GameObject[] objCharSlot;

    StringBuilder tempStringBuilder;
    GuildMember _member;
    bool _master = false;

    private void OnEnable()
    {
        tempStringBuilder = new StringBuilder();

        objCrewEmpty.SetActive(true);
        objCrewPower.SetActive(false);
        for(int i=0; i<objCharSlot.Length; i++)
            objCharSlot[i].SetActive(false);
    }

    public void SetData(GuildMember member, bool bMaster)
    {
        _member = member;
        _master = bMaster;
        for(int i=0; i<member.u2ClassID.Length; i++)
        {
            if(member.u2ClassID[i] == 0)
                objCharSlot[i].SetActive(false);
            else
            {
                objCharSlot[i].SetActive(true);
                txtCharLevel[i].text = _member.u2Level[i].ToString();
                imgClassIcon[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + _member.u2ClassID[i]);
                imgClassIcon[i].SetNativeSize();
                imgElementIcon[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + _member.u1Element[i]);
            }
        }

        if(_member.u8Power == 0)
        {
            objCrewPower.SetActive(false);
			if(_master)
                objCrewEmpty.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = TextManager.Instance.GetText("txt_guild_captain_set");
            else
                objCrewEmpty.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = TextManager.Instance.GetText("txt_guild_set");
            //if(_master && GuildInfoMgr.Instance.bGuildMaster)
            //    objCrewEmpty.SetActive(true);
            //else if(_master && !GuildInfoMgr.Instance.bGuildMaster)
            //    objCrewEmpty.SetActive(false);
            //else
            //    objCrewEmpty.SetActive(true);
        }
        else
        {
            objCrewEmpty.SetActive(false);
            objCrewPower.SetActive(true);
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            if(_member.strLegionName == Legion.Instance.sName)
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_power")).Append(" ").Append(Legion.Instance.acCrews[GuildInfoMgr.Instance.u1GuildCrewIndex-1].u4Power);
            else
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_power")).Append(" ").Append(_member.u8Power);
            txtCrewPower.text = tempStringBuilder.ToString();
        }
    }

    public void SetEmpty(bool bMaster)
    {
        //if(bMaster && GuildInfoMgr.Instance.bGuildMaster)
        //    objCrewEmpty.SetActive(true);
        //else if(bMaster && !GuildInfoMgr.Instance.bGuildMaster)
        //    objCrewEmpty.SetActive(false);
        //else
        //    objCrewEmpty.SetActive(true);
        objCrewEmpty.SetActive(true);
		if(bMaster)
			txtCrewEmpty.text = TextManager.Instance.GetText("txt_guild_captain_set");
        else
			txtCrewEmpty.text = TextManager.Instance.GetText("txt_guild_set");
        objCrewPower.SetActive(false);
        for(int i=0; i<objCharSlot.Length; i++)
            objCharSlot[i].SetActive(false);
    }
}
