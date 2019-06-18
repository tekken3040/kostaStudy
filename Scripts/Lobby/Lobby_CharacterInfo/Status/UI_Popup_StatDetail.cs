using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class UI_Popup_StatDetail : MonoBehaviour
{
    private StringBuilder tempStringBuilder;
    private Hero cHero;

    [SerializeField] GameObject[] objStat;
    [SerializeField] Text[] txtPercentage;
    private Text[][] txtStat;
    private Status skillResult;
    private Status heroStatus;
    private UInt32[] addStatus;

    private void OnEnable()
    {
        tempStringBuilder = new StringBuilder();
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);
    }

    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(gameObject);
        Destroy(this.gameObject);
    }
    
    public void SetData(Hero _hero, UInt32[] addStatus)
    {
        cHero = _hero;
        this.addStatus = addStatus;
        heroStatus = new Status();
        txtStat = new Text[Server.ConstDef.CharStatPointType][];
        for (int i = 0; i < Server.ConstDef.CharStatPointType; i++)
            txtStat[i] = new Text[4];
        skillResult = cHero.GetComponent<SkillComponent>().GetPassiveStatus();
        for (int i = 0; i < Server.ConstDef.CharStatPointType; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                txtStat[i][j] = objStat[i].transform.GetChild(j).GetChild(0).GetComponent<Text>();

            }
            //objStat[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = (cHero.cFinalStatus.GetStat((Byte)(i+1)) + skillResult.GetStat((Byte)(i+1))).ToString();
            //objStat[i].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = cHero.cStatus.GetStat((Byte)(i+1)).ToString();
            //objStat[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = cHero.GetComponent<StatusComponent>().EquipBase.GetStat((Byte)(i+1)).ToString();
            //objStat[i].transform.GetChild(3).GetChild(0).GetComponent<Text>().text = skillResult.GetStat((Byte)(i+1)).ToString();
            //txtStat[i][0].text = cHero.cFinalStatus.GetStat((Byte)(i+1)).ToString();
            txtStat[i][1].text = (cHero.cStatus.GetStat((Byte)(i + 1)) + addStatus[i]).ToString();
            txtStat[i][2].text = (cHero.GetComponent<StatusComponent>().EquipBase.GetStat((Byte)(i + 1))).ToString();
            txtStat[i][3].text = skillResult.GetStat((Byte)(i + 1)).ToString();
            txtStat[i][0].text = (cHero.cFinalStatus.GetStat((Byte)(i + 1)) + skillResult.GetStat((Byte)(i + 1)) + addStatus[i]).ToString();
        }
        heroStatus.Add(cHero.cFinalStatus);
        heroStatus.Add(skillResult);

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(GetDefPer(1).ToString(".##")).Append("%");
        txtPercentage[0].text = tempStringBuilder.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(GetDefPer(2).ToString(".##")).Append("%");
        txtPercentage[1].text = tempStringBuilder.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(GetCriPer().ToString(".##")).Append("%");
        txtPercentage[2].text = tempStringBuilder.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(GetEvaPer().ToString(".##")).Append("%");
        txtPercentage[3].text = tempStringBuilder.ToString();
    }

    public float GetDefPer(int defType)
	{
		//최소 방어율 + (방어스탯 / (방어스탯 + (레벨상수 * 레벨) ) * 100	
		float totalDef = 0;
        float def = 0;
        switch (defType) {
		case 1:
            def = (float)(heroStatus.GetStat(4) + addStatus[3]);
            totalDef = (float)cHero.cClass.cDefPerRange.fMin + (def / (def + (LegionInfoMgr.Instance.u2LevelDamageVal * cHero.cLevel.u2Level))) * 100f;

			if (totalDef > (float)cHero.cClass.cDefPerRange.fMax)
				totalDef = (float)cHero.cClass.cDefPerRange.fMax;

			if (totalDef < 0f)
				totalDef = 0f;
			break;
		case 2:
            def = (float)(heroStatus.GetStat(5) + addStatus[4]);
            totalDef = (float)cHero.cClass.cDefPerRange.fMin + (def / (def + (LegionInfoMgr.Instance.u2LevelDamageVal * cHero.cLevel.u2Level))) * 100f;

			if (totalDef > (float)cHero.cClass.cDefPerRange.fMax)
				totalDef = (float)cHero.cClass.cDefPerRange.fMax;

			if (totalDef < 0f)
				totalDef = 0f;
			break;
		}

		return totalDef;
	}

    public float GetEvaPer()
	{
		float totalEva = (float)((heroStatus.GetStat (6) + addStatus[5]) * cHero.cClass.cEvasionFactor.Random + cHero.cClass.cEvasionRange.fMin);

		if(totalEva > cHero.cClass.cEvasionRange.fMax)
            totalEva = (float)cHero.cClass.cEvasionRange.fMax;

		return totalEva;
	}

    public float GetCriPer()
    {
        int totalCriPer = (int)((heroStatus.GetStat(7) + addStatus[6]) * cHero.cClass.cCriticalFactor.Random
		                      + cHero.cClass.cCriticalRange.fMin);

        if(totalCriPer > (int)cHero.cClass.cCriticalRange.fMax)
            totalCriPer = (int)cHero.cClass.cCriticalRange.fMax;

        return totalCriPer;
    }
}
