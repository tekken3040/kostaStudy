using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_Popup_AutoStatAdd : MonoBehaviour
{
    public GameObject[] Status_Ability;
    public Color[] RGB_StatUpColor;
    int _statPoint;                                 //현재 스텟포인트
    int _usedStatPoint;                             //사용한 스텟포인트
    UInt32[] _status;                               //스테이터스(0 == HP, 1 == STR, 2 == INT, 3 == DEF, 4 == RES, 5 == AGI, 6 == CRI)
    UInt32[] _statusAddPoint;                       //투자한 스텟 포인트(0 == HP, 1 == STR, 2 == INT, 3 == DEF, 4 == RES, 5 == AGI, 6 == CRI)
    GameObject _panelStatus;
    public void Awake()
    {
        _panelStatus = GameObject.Find("Pref_UI_Panel_CharacterInfo_Status");
        _status = new UInt32[7];
		_statusAddPoint = new UInt32[7];
        for(int i=0; i<7; i++)
        {
            _status[i] = 0;
            _statusAddPoint[i] = 0;
        }
    }

    public void OnEnable()
    {

    }
    Hero cHero;
    public void SetRenderHeroStatus(object[] param)
    {
        cHero = ((Hero)param[0]);
		//_status[0] = (UInt32)cHero.cStatus.u4HP;
		//_status[1] = (UInt32)cHero.cStatus.u2Strength;
		//_status[2] = (UInt32)cHero.cStatus.u2Intelligence;
		//_status[3] = (UInt32)cHero.cStatus.u2Defence;
		//_status[4] = (UInt32)cHero.cStatus.u2Resistance;
		//_status[5] = (UInt32)cHero.cStatus.u2Agility;
		//_status[6] = (UInt32)cHero.cStatus.u2Critical;
        _status[0] = (UInt32)cHero.cStatus.GetStat(1);
		_status[1] = (UInt32)cHero.cStatus.GetStat(2);
		_status[2] = (UInt32)cHero.cStatus.GetStat(3);
		_status[3] = (UInt32)cHero.cStatus.GetStat(4);
		_status[4] = (UInt32)cHero.cStatus.GetStat(5);
		_status[5] = (UInt32)cHero.cStatus.GetStat(6);
		_status[6] = (UInt32)cHero.cStatus.GetStat(7);

        _statPoint = cHero.GetComponent<StatusComponent>().STAT_POINT;
        int SelectedAutoStat = 0;
        for(int i=0; i<Status_Ability.Length; i++)
        {
            Status_Ability[i].transform.GetChild(1).gameObject.SetActive(false);
            Status_Ability[i].transform.GetChild(0).GetComponent<Text>().color = RGB_StatUpColor[2];
            Status_Ability[i].GetComponent<Text>().color = RGB_StatUpColor[2];
            Status_Ability[i].transform.GetChild(0).GetComponent<Text>().text = (_status[i]).ToString();
        }
        for(int i=_statPoint; i>0;)
        {
            for(int j=0; j<ClassInfo.MAX_CHARACTER_AUTO_STATUS; j++)
            {
                SelectedAutoStat = (int)(cHero.cClass.au1AutoStat[j]);
                _statusAddPoint[SelectedAutoStat-1] += (UInt16)EquipmentInfoMgr.Instance.GetAddStatusPerPoint((byte)SelectedAutoStat);
                Status_Ability[SelectedAutoStat-1].transform.GetChild(0).GetComponent<Text>().text = (_status[SelectedAutoStat-1] + _statusAddPoint[SelectedAutoStat-1]).ToString();
                Status_Ability[SelectedAutoStat-1].transform.GetChild(1).gameObject.SetActive(true);
                Status_Ability[SelectedAutoStat-1].transform.GetChild(0).GetComponent<Text>().color = RGB_StatUpColor[1];
                Status_Ability[SelectedAutoStat-1].GetComponent<Text>().color = RGB_StatUpColor[0];

                i--;
                _statPoint--;
                _usedStatPoint++;
                if(i == 0)
                    break;
            }
        }
    }
}
