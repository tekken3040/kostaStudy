using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class MatchPopup_EnemyCrewInfo : MonoBehaviour
{
    [SerializeField] Text txtCrewName;
    [SerializeField] Text txtCrewPower;
    [SerializeField] Text txtPoint;
    [SerializeField] Text txtWinLose;

    [SerializeField] Text[] txtCharPower;
    [SerializeField] UI_League_Slot[] _slotChar;

    StringBuilder tempStringBuilder;

    void Awake()
    {
        tempStringBuilder = new StringBuilder();
    }

    void OnEnable()
    {
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);
        Init();
    }

    void Init()
    {
        txtCrewName.text = UI_League.Instance.sEnemyName;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(TextManager.Instance.GetText("mark_league_total_power")).Append(" ").Append(UI_League.Instance.EnemyCrew.u4Power);
        txtCrewPower.text = tempStringBuilder.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(UI_League.Instance._leagueMatch.u4Point).Append(" ").Append(TextManager.Instance.GetText("mark_league_point"));
        txtPoint.text = tempStringBuilder.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        UInt16 _win = UI_League.Instance._leagueMatch.u2Win;
        UInt16 _draw = UI_League.Instance._leagueMatch.u2Draw;
        UInt16 _lose = UI_League.Instance._leagueMatch.u2Lose;
        float rate = (float)(((float)_win/(float)(_win+_draw+_lose))*100);
		tempStringBuilder.Append(UI_League.Instance._leagueMatch.u2Win).Append(TextManager.Instance.GetText("mark_league_win")).Append(" ");
		tempStringBuilder.Append(UI_League.Instance._leagueMatch.u2Draw).Append(TextManager.Instance.GetText("mark_league_draw")).Append(" ");
		tempStringBuilder.Append(UI_League.Instance._leagueMatch.u2Lose).Append(TextManager.Instance.GetText("mark_league_lose")).Append(" (").Append((Byte)rate).Append("%)");
        txtWinLose.text = tempStringBuilder.ToString();

        for(int i=0; i<LeagueCrew.MAX_CHAR_IN_CREW; i++)
        {
            if(UI_League.Instance.EnemyCrew.acLocation[i] != null)
            {
                txtCharPower[i].text = UI_League.Instance.EnemyCrew.acLocation[i].cFinalStatus.u4Power.ToString();
                Byte tempIndex = (Byte)UI_League.Instance.EnemyCrew.acLocation[i].iIndexInCrew;
                _slotChar[i].SetData((Byte)i, false, true);
                _slotChar[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                txtCharPower[i].text = "";
                _slotChar[i].SetData(0, true, true);
                _slotChar[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(gameObject);
        gameObject.SetActive(false);
    }


}
