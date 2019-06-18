using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class RankSlotCrewPower : MonoBehaviour
{
    public Sprite[] _upDownIcon;
    public Sprite[] _bgSprite;
    public Color[] _upDownColor;
    public Image _rankChangeIcon;
    public Text _rankChange;
    public Text _rank;
    public Text _crewName;
    public GameObject[] _charIcon;
    public Text _crewPower;
    public GameObject _notice;
    RankListDetail _rankListDetail;
    MyCrewRank _rankListDitailMyCrew;
    StringBuilder tempStringBuilder;

    public void SetData(RankListDetail _detail)
    {
        _rankListDetail = new RankListDetail();
        tempStringBuilder = new StringBuilder();
        _rankListDetail = _detail;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(_rankListDetail.u4Rank.ToString()).Append(" ").Append(TextManager.Instance.GetText("mark_rank"));
        _rank.text = tempStringBuilder.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(_rankListDetail.strLegionName).Append(" ").Append(TextManager.Instance.GetText("mark_crew" + _rankListDetail.u1CrewIndex.ToString()));
        _crewName.text = tempStringBuilder.ToString();
        _notice.SetActive(false);
        if(RankInfoMgr.Instance.dicRankSaveCategoryData[(UInt16)_rankListDetail.u1RankType].dicRankSaveData.Count == 0)
        {
            _rankChange.gameObject.SetActive(false);
            _rankChangeIcon.gameObject.SetActive(false);
            //return;
        }
        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            if(_rankListDetail.u2CharClassID[i] != 0)
            {
                _charIcon[i].SetActive(true);
                _charIcon[i].transform.GetChild(1).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + _rankListDetail.u2CharClassID[i].ToString());
            }
            else
                _charIcon[i].SetActive(false);
        }
        _crewPower.text = _rankListDetail.u4Value.ToString();
        if(this.transform.GetSiblingIndex()%2 == 0)
            this.GetComponent<Image>().sprite = _bgSprite[0];
        else
            this.GetComponent<Image>().sprite = _bgSprite[1];
        for(int i=0; i<RankInfoMgr.Instance.dicRankSaveCategoryData[(UInt16)_rankListDetail.u1RankType].dicRankSaveData.Count; i++)
        {
            if(RankInfoMgr.Instance.dicRankSaveCategoryData[(UInt16)_rankListDetail.u1RankType].dicRankSaveData[(UInt64)(i+1)].strCrewName 
                == _rankListDetail.strLegionName)
            {
                int tempRank = (int)(RankInfoMgr.Instance.dicRankSaveCategoryData[(UInt16)_rankListDetail.u1RankType].dicRankSaveData[(UInt64)(i+1)].u8Rank - _rankListDetail.u4Rank);
                if(tempRank < 0)
                {
                    _rankChange.text = (tempRank*-1).ToString();
                    _rankChange.color = _upDownColor[1];
                    _rankChangeIcon.sprite = _upDownIcon[1];
                    _notice.SetActive(false);
                    break;
                }
                else if(tempRank > 0)
                {
                    _rankChange.text = tempRank.ToString();
                    _rankChange.color = _upDownColor[0];
                    _rankChangeIcon.sprite = _upDownIcon[0];
                    _notice.SetActive(false);
                    break;
                }
                else
                {
                    _rankChange.gameObject.SetActive(false);
                    _rankChangeIcon.gameObject.SetActive(false);
                    _notice.SetActive(false);
                    break;
                }
            }
            else
            {
                _rankChange.gameObject.SetActive(false);
                _rankChangeIcon.gameObject.SetActive(false);
                _notice.SetActive(true);
            }
        }
    }

    public void SetDataMyCrew(MyCrewRank _detail)
    {
        _rankListDitailMyCrew = new MyCrewRank();
        tempStringBuilder = new StringBuilder();
        _rankListDitailMyCrew = _detail;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(_rankListDitailMyCrew.u8MyRank.ToString()).Append(" ").Append(TextManager.Instance.GetText("mark_rank"));
        _rank.text = tempStringBuilder.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(Legion.Instance.sName).Append(" ").Append(TextManager.Instance.GetText("mark_crew" + _rankListDitailMyCrew.u1CrewIndex.ToString()));
        _crewName.text = tempStringBuilder.ToString();
        _notice.SetActive(false);
        _rankChange.gameObject.SetActive(false);
        _rankChangeIcon.gameObject.SetActive(false);

        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            if(Legion.Instance.acCrews[_rankListDitailMyCrew.u1CrewIndex-1].acLocation[i] != null)
            {
                _charIcon[i].SetActive(true);
                _charIcon[i].transform.GetChild(1).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + Legion.Instance.acCrews[_rankListDitailMyCrew.u1CrewIndex-1].acLocation[i].cClass.u2ID.ToString());
            }
            else
                _charIcon[i].SetActive(false);
        }
        _crewPower.text = _rankListDitailMyCrew.u4MyPower.ToString();
        if(this.transform.GetSiblingIndex()%2 == 0)
            this.GetComponent<Image>().sprite = _bgSprite[0];
        else
            this.GetComponent<Image>().sprite = _bgSprite[1];
    }
}
