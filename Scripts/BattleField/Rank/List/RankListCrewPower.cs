using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class RankListCrewPower : MonoBehaviour
{
    public Text _myRank;
    public Text _crewName;
    public Text _powerValue;
    public GameObject[] _heroIcon;
    RankListInfo _rankListInfo;
    StringBuilder tempStringBuilder;

    public void Awake()
    {
        tempStringBuilder = new StringBuilder();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_rank")).Append(" - ").Append(TextManager.Instance.GetText("mark_rank"));
        _myRank.text = tempStringBuilder.ToString();
        _crewName.text = " - ";
        _powerValue.text = " - ";
        for(int i=0; i<_heroIcon.Length; i++)
        {
            _heroIcon[i].SetActive(false);
        }
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
        _powerValue.text = _rankListInfo.u4Value.ToString();
        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            if(_rankListInfo.u2CharClassID[i] != 0)
            {
                _heroIcon[i].transform.GetChild(1).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + _rankListInfo.u2CharClassID[i]);
                _heroIcon[i].SetActive(true);
            }
            else
                _heroIcon[i].SetActive(false);
        }
    }
}
