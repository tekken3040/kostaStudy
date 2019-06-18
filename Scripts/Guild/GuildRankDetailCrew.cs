using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;

public class GuildRankDetailCrew : MonoBehaviour
{
    public Text txtGuildName;
    public Text txtGuildPower;

    public GameObject detailPopup;

    public GameObject[] charSlot;

    StringBuilder tempStringBuilder;
    Crew[] cCrews;

    private void OnEnable()
    {
        tempStringBuilder = new StringBuilder();
        PopupManager.Instance.AddPopup(this.gameObject, OnClickClose);
        Init();
    }

    public void Init()
    {
        txtGuildName.text = GuildInfoMgr.Instance.cGuildRankInfo.strName;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_power")).Append(" ").Append(GuildInfoMgr.Instance.cGuildRankInfo.u8GuildPower.ToString());
        txtGuildPower.text = tempStringBuilder.ToString();

        cCrews = new Crew[GuildInfoMgr.MAX_GUILD_ENTRY];

        for(int i=0; i<GuildInfoMgr.MAX_GUILD_ENTRY; i++)
            cCrews[i] = new Crew();

        for(int i=0; i<GuildInfoMgr.Instance.cGuildRankInfo.u1DeckCount; i++)
        {
            for(int j=0; j<GuildInfoMgr.Instance.cGuildRankInfo.lstGuildCrew[i].u1CharCount; j++)
                SetCrewData(GuildInfoMgr.Instance.cGuildRankInfo.lstGuildCrew[i].lstMatchCrew[j], GuildInfoMgr.Instance.cGuildRankInfo.lstGuildCrew[i].u1CrewIndex);
        }

        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            if(cCrews[0].acLocation[i] != null)
            {
                charSlot[i].GetComponent<UI_CharCursorElement>().SetData((Hero)cCrews[0].acLocation[i]);
                charSlot[i].SetActive(true);
            }
            else
                charSlot[i].SetActive(false);

            if(cCrews[1].acLocation[i] != null)
            {
                charSlot[i+3].GetComponent<UI_CharCursorElement>().SetData((Hero)cCrews[1].acLocation[i]);
                charSlot[i+3].SetActive(true);
            }
            else
                charSlot[i+3].SetActive(false);

            if(cCrews[2].acLocation[i] != null)
            {
                charSlot[i+6].GetComponent<UI_CharCursorElement>().SetData((Hero)cCrews[2].acLocation[i]);
                charSlot[i+6].SetActive(true);
            }
            else
                charSlot[i+6].SetActive(false);
        }
    }

    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(this.gameObject);
        this.gameObject.SetActive(false);
    }

    public void OnClickCloseDetail()
    {
        PopupManager.Instance.RemovePopup(detailPopup);
        detailPopup.SetActive(false);
    }

    public void OnClickDetail(int idx)
    {
        PopupManager.Instance.AddPopup(detailPopup, OnClickCloseDetail);
        detailPopup.SetActive(true);
        detailPopup.GetComponent<CharInfoPopup>().SetData(charSlot[idx].GetComponent<UI_CharCursorElement>().cHero);
    }

    public void SetCrewData(GuildMatchCrewChar charData, int crewIdx)
    {
        Hero tempHero = new Hero(charData.u1CharIndex, charData.u2ClassID, charData.strCharName, charData.u1Shape[0], charData.u1Shape[1], charData.u1Shape[2]);
        List<LearnedSkill> lstLearnInfo = null;
		LearnedSkill temp;

        UInt32[] heroStats = new UInt32[Server.ConstDef.CharStatPointType];
        heroStats[0] = charData.u2Stats[0];
        heroStats[1] = charData.u2Stats[1];
        heroStats[2] = charData.u2Stats[2];
        heroStats[3] = charData.u2Stats[3];
        heroStats[4] = charData.u2Stats[4];
        heroStats[5] = charData.u2Stats[5];
        heroStats[6] = charData.u2Stats[6];

        tempHero.GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
        tempHero.GetComponent<LevelComponent>().Set(Convert.ToByte(charData.u2Level), 0);

        for(int i=0; i<Hero.MAX_EQUIP_OF_CHAR; i++)
        {
            tempHero.acEquips[i] = new EquipmentItem(charData.lstCrewEquipment[i].u2ItemID);
            tempHero.acEquips[i].GetComponent<StatusComponent>().LoadStatus(charData.lstCrewEquipment[i].u4Stats,
            tempHero.acEquips[i].GetEquipmentInfo().acStatAddInfo[0].u1StatType,
            tempHero.acEquips[i].GetEquipmentInfo().acStatAddInfo[1].u1StatType,
            tempHero.acEquips[i].GetEquipmentInfo().acStatAddInfo[2].u1StatType, 0);
            tempHero.acEquips[i].u1SmithingLevel = charData.lstCrewEquipment[i].u1SmithingLevel;
            tempHero.acEquips[i].u1Completeness = charData.lstCrewEquipment[i].u1Completeness;
            tempHero.acEquips[i].GetComponent<LevelComponent>().Set(Convert.ToByte(charData.lstCrewEquipment[i].u2Level), 0);
            tempHero.acEquips[i].skillSlots = charData.lstCrewEquipment[i].u1SkillSlot;
            tempHero.GetComponent<StatusComponent>().Wear(tempHero.acEquips[i].cStatus);
        }
        lstLearnInfo = new List<LearnedSkill>();

        for(int i=0; i<charData.u2SkillCount; i++)
        {
            temp = new LearnedSkill();
            temp.u1SlotNum = charData.lstCrewSkill[i].u1SkillSlot;
            temp.u2Level = charData.lstCrewSkill[i].u2Level;
            temp.u1UseIndex = charData.lstCrewSkill[i].u1SelectSlot;
            lstLearnInfo.Add(temp);
        }

        tempHero.GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);

        cCrews[crewIdx].acLocation[charData.u1CrewPos] = tempHero;
    }
}
