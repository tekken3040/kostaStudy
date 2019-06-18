using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class RankListWeeklyCampaign : MonoBehaviour
{
    public Text _myRank;
    public Text _crewName;
    public Text _difficulty;
    public Text _lastStage;
    RankListInfo _rankListInfo;
    StringBuilder tempStringBuilder;

    public void Awake()
    {
        tempStringBuilder = new StringBuilder();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_rank")).Append(" - ").Append(TextManager.Instance.GetText("mark_rank"));
        _myRank.text = tempStringBuilder.ToString();
        _crewName.text = " - ";
        _difficulty.text = " - ";
        _lastStage.text = " - ";
    }

    public void SetData(RankListInfo _listInfo)
    {
        //tempStringBuilder = new StringBuilder();
        _rankListInfo = new RankListInfo();
        _rankListInfo = _listInfo;

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        if(_listInfo == null)
        {
            tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_rank")).Append(" - ").Append(TextManager.Instance.GetText("mark_rank"));
            _myRank.text = tempStringBuilder.ToString();
            _crewName.text = " - ";
            _difficulty.text = " - ";
            _lastStage.text = " - ";
        }
        else
        {
            tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_rank")).Append(_rankListInfo.u8MyRank.ToString()).Append(TextManager.Instance.GetText("mark_rank"));
            _myRank.text = tempStringBuilder.ToString();
            
            _crewName.text = _rankListInfo.strLegionName;
            switch(_rankListInfo.u1Difficulty)
            {
                case 1:
                    _difficulty.text = TextManager.Instance.GetText("btn_diffi_easy");
                    break;

                case 2:
                    _difficulty.text = TextManager.Instance.GetText("btn_diffi_normal");
                    break;

                case 3:
                    _difficulty.text = TextManager.Instance.GetText("btn_diffi_hell");
                    break;
            }

            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(StageInfoMgr.Instance.dicStageData[(UInt16)_rankListInfo.u4Value].actInfo.u1Number.ToString());
            tempStringBuilder.Append(TextManager.Instance.GetText("mark_act")).Append(" ");
            tempStringBuilder.Append(StageInfoMgr.Instance.dicStageData[(UInt16)_rankListInfo.u4Value].chapterInfo.u1Number.ToString()).Append(" - ");
            tempStringBuilder.Append(StageInfoMgr.Instance.dicStageData[(UInt16)_rankListInfo.u4Value].u1StageNum.ToString());
            _lastStage.text = tempStringBuilder.ToString();
        }
    }
}
