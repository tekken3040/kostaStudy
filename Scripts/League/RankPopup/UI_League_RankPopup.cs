using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;

public class UI_League_RankPopup : MonoBehaviour
{
    [SerializeField] Text txtSeasonEndText;
    [SerializeField] Text txtSeasonEndLeftTime;

    [SerializeField] GameObject objSlotList;
    [SerializeField] GameObject objTopGradient;
    [SerializeField] GameObject objBottomGradient;
    [SerializeField] GameObject objNoLegend;

    [SerializeField] Image imgDivision;
    [SerializeField] Image imgTextDivision;
    [SerializeField] Image imgTextDivision2;
    [SerializeField] Text txtRankPoint;

    //private List<GameObject> _slots;
    StringBuilder tempStringBuilder;
    void Awake()
    {
        //_slots = new List<GameObject>();
        tempStringBuilder = new StringBuilder();
    }
    
    void OnEnable()
    {
        //imgDivision.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_06.division_"+Legion.Instance.GetDivision);
        //imgDivision.SetNativeSize();
        //if(Legion.Instance.GetDivision > 5)
        //{
        //    imgTextDivision.gameObject.SetActive(true);
        //    imgTextDivision2.gameObject.SetActive(false);
        //    imgTextDivision.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_06.division_t_"+Legion.Instance.GetDivision);
        //    imgTextDivision.SetNativeSize();
        //}
        //else
        //{
        //    imgTextDivision.gameObject.SetActive(false);
        //    imgTextDivision2.gameObject.SetActive(true);
        //    imgTextDivision2.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_06.division_t_"+Legion.Instance.GetDivision);
        //    imgTextDivision2.SetNativeSize();
        //}
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        if(UI_League.Instance.cLeagueMatchList.u1DivisionIndex > 5)
        {
		    tempStringBuilder.Append(UI_League.Instance.cLeagueMatchList.u4MyRank).Append(" ").Append(TextManager.Instance.GetText("mark_league_rank"));
            txtRankPoint.text = tempStringBuilder.ToString();
        }
        else
            txtRankPoint.text = "";
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_league_ranking_season_time"));
        txtSeasonEndText.text = string.Format(tempStringBuilder.ToString(), Legion.Instance.u2SeasonNum);
        txtSeasonEndLeftTime.text = string.Format(TextManager.Instance.GetText("mark_league_ranking_season_time2"), UI_League.Instance._leagueLegendRank.tsLeftTime.Days, UI_League.Instance._leagueLegendRank.tsLeftTime.Hours);
        if(UI_League.Instance._leagueLegendRank.u1Count == 0)
            objNoLegend.SetActive(true);
        else
            objNoLegend.SetActive(false);
        for(int i=0; i<UI_League.Instance._leagueLegendRank.u1Count; i++)
        {
            //if(objSlotList.transform.GetChild(i) == )
            //{
                GameObject legendSlot = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/League/LegendRankSlot_.prefab", typeof(GameObject))) as GameObject;
                legendSlot.transform.SetParent(objSlotList.transform);
                legendSlot.transform.localPosition = Vector2.zero;
                legendSlot.transform.localScale = Vector2.one;
                legendSlot.GetComponent<UI_League_RankListSlot>().SetData(UI_League.Instance._leagueLegendRank.sRankInfo[i]);
            //}
            //else
            //{
            //    objSlotList.transform.GetChild(i).GetComponent<UI_League_RankListSlot>().SetData(UI_League.Instance._leagueLegendRank.sRankInfo[i]);
            //}
        }
        //_slots = new List<GameObject>();
        //for(int i=0; i<objSlotList.transform.GetChildCount(); i++)
        //{
        //    _slots.Add(objSlotList.transform.GetChild(i).gameObject);
        //    if(UI_League.Instance._leagueLegendRank.u1Count == 0)
        //    {
        //        _slots[i].SetActive(false);
        //    }
        //    else
        //    {
        //        //_slots[i].GetComponent<UI_League_RankListSlot>().SetData(UI_League.Instance._leagueLegendRank.sRankInfo[i]);
        //        _slots[i].SetActive(true);
        //    }
        //}
    }

    void OnDisable()
    {
        for(int i=0; i<objSlotList.transform.GetChildCount(); i++)
        {
            Destroy(objSlotList.transform.GetChild(i).gameObject);
        }
    }

    public void LoadData()
    {
        if(UI_League.Instance._leagueLegendRank.u1Count == 0)
            return;
        DebugMgr.LogError("Top : " + objSlotList.transform.GetChild(1).position.y +" : "+ objTopGradient.transform.position.y);
        DebugMgr.LogError("Bottom : " + objSlotList.transform.GetChild(13).position.y +" : "+ objBottomGradient.transform.position.y);
        if(objSlotList.transform.GetChild(1).position.y >= objTopGradient.transform.position.y+2)
        {
            if((int.Parse(objSlotList.transform.GetChild(14).name)+1) > UI_League.Instance._leagueLegendRank.u1Count)
                return;
            
            //objSlotList.transform.GetChild(0).name = (int.Parse(objSlotList.transform.GetChild(14).name)+1).ToString();
            //objSlotList.transform.GetChild(0).GetComponent<UI_League_RankListSlot>().SetData(UI_League.Instance._leagueLegendRank.sRankInfo[int.Parse(objSlotList.transform.GetChild(0).name)]);
            
            objSlotList.transform.GetChild(0).SetAsLastSibling();
        }
        else if(objSlotList.transform.GetChild(13).position.y < objBottomGradient.transform.position.y-2)
        {
            if((int.Parse(objSlotList.transform.GetChild(0).name)-1) < 0)
                return;

            //objSlotList.transform.GetChild(14).name = (int.Parse(objSlotList.transform.GetChild(0).name)-1).ToString();
            //objSlotList.transform.GetChild(14).GetComponent<UI_League_RankListSlot>().SetData(UI_League.Instance._leagueLegendRank.sRankInfo[int.Parse(objSlotList.transform.GetChild(0).name)]);
            
            objSlotList.transform.GetChild(14).SetAsFirstSibling();
        }
    }
}
