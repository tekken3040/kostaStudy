using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class RankSlotWeeklyForest : MonoBehaviour
{
    public Sprite[] _upDownIcon;
    public Sprite[] _bgSprite;
    public Color[] _upDownColor;
    public Sprite[] _elementIcon;
    public Image _rankChangeIcon;
    public Text _rankChange;
    public Text _rank;
    public Text _crewName;
    public Text _difficulty;
    public Image _element;
    public Text _lastStage;
    public Text _clearTime;
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

        switch(_rankListDetail.u1Difficulty)
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

        switch(StageInfoMgr.Instance.dicStageData[(UInt16)_rankListDetail.u4Value].u1ForestElement)
        {
            case 2:
                _element.sprite = _elementIcon[0];
                break;

            case 3:
                _element.sprite = _elementIcon[1];
                break;

            case 4:
                _element.sprite = _elementIcon[2];
                break;
        }

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(StageInfoMgr.Instance.dicStageData[(UInt16)_rankListDetail.u4Value].chapterInfo.u1Number.ToString()).Append(" - ");
        tempStringBuilder.Append(StageInfoMgr.Instance.dicStageData[(UInt16)_rankListDetail.u4Value].u1StageNum.ToString());
        _lastStage.text = tempStringBuilder.ToString();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(_rankListDetail.dtClearTime.Month.ToString()).Append("/").Append(_rankListDetail.dtClearTime.Day.ToString()).Append(" ").Append(
            _rankListDetail.dtClearTime.ToString("HH:mm"));
        _clearTime.text = tempStringBuilder.ToString();
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
