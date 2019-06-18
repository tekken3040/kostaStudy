using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class RankListUserPower : MonoBehaviour
{
    public Text _myRank;
    public Text _crewName;
    public Text _heroName;
    public Text _powerValue;
    public Image _heroIcon;
    RankListInfo _rankListInfo;
    StringBuilder tempStringBuilder;

    public void Awake()
    {
        tempStringBuilder = new StringBuilder();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_rank")).Append(" - ").Append(TextManager.Instance.GetText("mark_rank"));
        _myRank.text = tempStringBuilder.ToString();
        _crewName.text = " - ";
        _heroName.text = " - ";
        _powerValue.text = " - ";
        _heroIcon.transform.parent.gameObject.SetActive(false);
    }

    public void SetData(RankListInfo _listInfo)
    {
        _heroIcon.transform.parent.gameObject.SetActive(true);
        tempStringBuilder = new StringBuilder();
        _rankListInfo = new RankListInfo();
        _rankListInfo = _listInfo;

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_rank")).Append(_rankListInfo.u8MyRank.ToString()).Append(TextManager.Instance.GetText("mark_rank"));
        _myRank.text = tempStringBuilder.ToString();
        
        _crewName.text = _rankListInfo.strLegionName;
        _heroName.text = _rankListInfo.strCharName;
        _powerValue.text = _rankListInfo.u4Value.ToString();
        _heroIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + _rankListInfo.u2CharClassID[0]);
    }
}
