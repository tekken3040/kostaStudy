using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class RankListWeeklyCrafting : MonoBehaviour
{
    public Text _myRank;
    public Text _crewName;
    public Text _craftValue;
    RankListInfo _rankListInfo;
    StringBuilder tempStringBuilder;

    public void Awake()
    {
        tempStringBuilder = new StringBuilder();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_rank")).Append(" - ").Append(TextManager.Instance.GetText("mark_rank"));
        _myRank.text = tempStringBuilder.ToString();
        _crewName.text = " - ";
        _craftValue.text = " - ";
    }

    public void SetData(RankListInfo _listInfo)
    {
        tempStringBuilder = new StringBuilder();
        _rankListInfo = new RankListInfo();
        _rankListInfo = _listInfo;

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_rank")).Append(_rankListInfo.u8MyRank.ToString()).Append(TextManager.Instance.GetText("mark_rank"));
        _myRank.text = tempStringBuilder.ToString();
        
        _crewName.text = _rankListInfo.strLegionName;
        _craftValue.text = _rankListInfo.u4MakingCount.ToString();
    }
}
