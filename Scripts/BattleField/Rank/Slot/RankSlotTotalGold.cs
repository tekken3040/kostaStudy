using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class RankSlotTotalGold : MonoBehaviour
{
    public Sprite[] _upDownIcon;
    public Sprite[] _bgSprite;
    public Color[] _upDownColor;
    public Image _rankChangeIcon;
    public Text _rankChange;
    public Text _rank;
    public Text _crewName;
    public Text _goldValue;
    public GameObject _notice;
    RankListDetail _rankListDetail;
    StringBuilder tempStringBuilder;

    public void SetData(RankListDetail _detail)
    {
        _rankListDetail = new RankListDetail();
        tempStringBuilder = new StringBuilder();
        _rankListDetail = _detail;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(_rankListDetail.u4Rank.ToString()).Append(" ").Append(TextManager.Instance.GetText("mark_rank"));
        _rank.text = tempStringBuilder.ToString();
        _crewName.text = _rankListDetail.strLegionName;
        _goldValue.text = _rankListDetail.u4Value.ToString();
        if(this.transform.GetSiblingIndex()%2 == 0)
            this.GetComponent<Image>().sprite = _bgSprite[0];
        else
            this.GetComponent<Image>().sprite = _bgSprite[1];
        _notice.SetActive(false);
        if(RankInfoMgr.Instance.dicRankSaveCategoryData[(UInt16)_rankListDetail.u1RankType].dicRankSaveData.Count == 0)
        {
            _rankChange.gameObject.SetActive(false);
            _rankChangeIcon.gameObject.SetActive(false);
            return;
        }
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
                    _rankChange.gameObject.SetActive(true);
                    _rankChangeIcon.gameObject.SetActive(true);
                    _notice.SetActive(false);
                    break;
                }
                else if(tempRank > 0)
                {
                    _rankChange.text = tempRank.ToString();
                    _rankChange.color = _upDownColor[0];
                    _rankChangeIcon.sprite = _upDownIcon[0];
                    _rankChange.gameObject.SetActive(true);
                    _rankChangeIcon.gameObject.SetActive(true);
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
}
