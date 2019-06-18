using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class UI_CharPower : MonoBehaviour
{
    public Text _powerValue;
    StringBuilder tempStringbuilder;

    public void SetData(Hero _hero)
    {
        DebugMgr.LogError(_hero);
        if(tempStringbuilder == null)
            tempStringbuilder = new StringBuilder();
        tempStringbuilder.Remove(0, tempStringbuilder.Length);
        tempStringbuilder.Append(TextManager.Instance.GetText("mark_power"));
        float ret = 0;
		ret += (_hero.cFinalStatus.GetStat(1)) * 0.04f;
		ret += (_hero.cFinalStatus.GetStat(2)) * 2.1f;
		ret += (_hero.cFinalStatus.GetStat(3)) * 2.1f;
		ret += (_hero.cFinalStatus.GetStat(4)) * 1.8f;
		ret += (_hero.cFinalStatus.GetStat(5)) * 1.8f;
		ret += (_hero.cFinalStatus.GetStat(6)) * 1.6f;
		ret += (_hero.cFinalStatus.GetStat(7)) * 1.6f;
		ret /= 7f;
        tempStringbuilder.Append(" ").Append(((UInt32)ret).ToString());
        _powerValue.text = tempStringbuilder.ToString();
    }

    public void SetData(Hero _hero, UInt32[] _stat)
    {
        //DebugMgr.LogError(_hero);
        if(tempStringbuilder == null)
            tempStringbuilder = new StringBuilder();
        tempStringbuilder.Remove(0, tempStringbuilder.Length);
        tempStringbuilder.Append(TextManager.Instance.GetText("mark_power"));
        float ret = 0;
		ret += (_hero.cFinalStatus.GetStat(1) + _stat[0]) * 0.04f;
		ret += (_hero.cFinalStatus.GetStat(2) + _stat[1]) * 2.1f;
		ret += (_hero.cFinalStatus.GetStat(3) + _stat[2]) * 2.1f;
		ret += (_hero.cFinalStatus.GetStat(4) + _stat[3]) * 1.8f;
		ret += (_hero.cFinalStatus.GetStat(5) + _stat[4]) * 1.8f;
		ret += (_hero.cFinalStatus.GetStat(6) + _stat[5]) * 1.6f;
		ret += (_hero.cFinalStatus.GetStat(7) + _stat[6]) * 1.6f;
		ret /= 7f;
        tempStringbuilder.Append(" ").Append(((UInt32)ret).ToString());
        _powerValue.text = tempStringbuilder.ToString();
    }
}
