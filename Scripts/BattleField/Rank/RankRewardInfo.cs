using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class RankRewardInfo : MonoBehaviour
{
    public Text _title;
    public Text _rankInfoTitle;
    public Text _rankInfoDesc;
    public GameObject RankTab;
    public GameObject scrollGroup;
    public GameObject _prefSlot;
    public GameObject _rankBoard;
    public GameObject[] _tabs;
    StringBuilder tempStringBuilder;

    public void Awake()
    {
        tempStringBuilder = new StringBuilder();
    }

    public void OnClickClose()
    {
        for(int i=0; i<scrollGroup.transform.childCount; i++)
            GameObject.Destroy(scrollGroup.transform.GetChild(i).gameObject);
        RankTab.GetComponent<RankTab>().OnClickClose();
        this.gameObject.SetActive(false);
    }

    public void OnClickRankBoard()
    {
        _rankBoard.SetActive(true);
        for(int i=0; i<scrollGroup.transform.childCount; i++)
            GameObject.Destroy(scrollGroup.transform.GetChild(i).gameObject);
        this.gameObject.SetActive(false);
    }

    public void SetData(Byte _type)
    {
        for(int i=0; i<_tabs.Length; i++)
            _tabs[i].SetActive(false);
        _tabs[_type].SetActive(true);
        InitInfo(_type);
        for(int i=0; i<RankInfoMgr.Instance.dicRankData.Count; i++)
        {
            GameObject _obj = Instantiate(_prefSlot);
            _obj.transform.SetParent(scrollGroup.transform);
            _obj.transform.localPosition = Vector3.zero;
            _obj.transform.localScale = Vector3.one;
            _obj.name = _prefSlot.name;
            _obj.GetComponent<RankRewardInfoSlot>().SetData(_type, i);
        }
    }

    public void InitInfo(Byte _type)
    {
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_rank2")).Append(" ");
		RankListDetail myRankData = RankInfoMgr.Instance.dicRankListDetailData[(UInt16)(RankInfoMgr.Instance.dicRankListDetailData.Count-1)];
		if(myRankData.u8MyRank == 0)
            tempStringBuilder.Append("- ");
        else
            tempStringBuilder.Append(RankInfoMgr.Instance.dicRankListData[(UInt16)_type].u8MyRank);
        if(_type != 1)
            _tabs[_type].transform.GetChild(0).GetComponent<Text>().text = tempStringBuilder.ToString();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        switch(_type)
        {
            case 0:
                _title.text = TextManager.Instance.GetText("mark_user_power");
				tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_power")).Append(" ").Append(myRankData.u4MyValue);
                _tabs[_type].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
                _rankInfoTitle.text = TextManager.Instance.GetText("info_user_power");
                _rankInfoDesc.text = TextManager.Instance.GetText("desc_user_power");
                break;

            case 1:
                _title.text = TextManager.Instance.GetText("mark_crew_power");
                _rankInfoTitle.text = TextManager.Instance.GetText("info_crew_power");
                _rankInfoDesc.text = TextManager.Instance.GetText("desc_crew_power");
                break;

            case 2:
                _title.text = TextManager.Instance.GetText("mark_total_gold");
				tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_gold")).Append(" ").Append(myRankData.u4MyValue);
                _tabs[_type].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
                _rankInfoTitle.text = TextManager.Instance.GetText("info_total_have_gold");
                _rankInfoDesc.text = TextManager.Instance.GetText("desc_total_have_gold");
                break;

            case 3:
                _title.text = TextManager.Instance.GetText("mark_weekly_used_cash");
				tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_cash")).Append(" ").Append(myRankData.u4MyValue);
                _tabs[_type].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
                _rankInfoTitle.text = TextManager.Instance.GetText("info_weekly_used_cash");
                _rankInfoDesc.text = TextManager.Instance.GetText("desc_weekly_used_cash");
                break;

            case 4:
                _title.text = TextManager.Instance.GetText("mark_weekly_crafting");
				tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_craft_point")).Append(" ").Append(myRankData.u4MyValue);
                _tabs[_type].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
				tempStringBuilder.Append("(").Append(myRankData.u4MyMakingCount);
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_craft_count")).Append(")");
                _tabs[_type].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                _rankInfoTitle.text = TextManager.Instance.GetText("info_weekly_crafting");
                _rankInfoDesc.text = TextManager.Instance.GetText("desc_weekly_crafting");
                break;

            case 5:
                _title.text = TextManager.Instance.GetText("mark_weekly_campaign_clear");
				if(myRankData.u8MyRank == 0)
                {
                    tempStringBuilder.Append(" - ");
                    _tabs[_type].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
                    _tabs[_type].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                }
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_last_stage")).Append(" ");
				switch(myRankData.u1MyDifficulty)
                {
                    case 1:
						tempStringBuilder.Append("[").Append(TextManager.Instance.GetText("btn_diffi_easy")).Append("]");
                        break;
                    case 2:
						tempStringBuilder.Append("[").Append(TextManager.Instance.GetText("btn_diffi_normal")).Append("]");
                        break;
                    case 3:
						tempStringBuilder.Append("[").Append(TextManager.Instance.GetText("btn_diffi_hell")).Append("]");
                        break;
                }
					
				if(StageInfoMgr.Instance.dicStageData.ContainsKey((UInt16)myRankData.u4MyValue))
				{
					StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[(UInt16)myRankData.u4MyValue];
					tempStringBuilder.Append(stageInfo.actInfo.u1Number.ToString());
	                tempStringBuilder.Append(TextManager.Instance.GetText("mark_act")).Append(" ");
					tempStringBuilder.Append(stageInfo.chapterInfo.u1Number.ToString()).Append(" - ");
					tempStringBuilder.Append(stageInfo.u1StageNum.ToString());
	                _tabs[_type].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();

	                tempStringBuilder.Remove(0, tempStringBuilder.Length);
	                tempStringBuilder.Append("(");
					tempStringBuilder.Append(myRankData.dtMyClearTime.Month.ToString()).Append("/")
							.Append(myRankData.dtMyClearTime.Day.ToString()).Append(" ")
							.Append(myRankData.dtMyClearTime.ToString("HH:mm"));
	                tempStringBuilder.Append(")");
	                _tabs[_type].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
				}
				else
				{
					_tabs[_type].transform.GetChild(1).GetComponent<Text>().text = " - ";
					_tabs[_type].transform.GetChild(2).GetComponent<Text>().text = " - ";
				}
				_rankInfoTitle.text = TextManager.Instance.GetText("info_weekly_campaign_clear");
				_rankInfoDesc.text = TextManager.Instance.GetText("desc_weekly_campaign_clear");
                break;

            case 6:
                _title.text = TextManager.Instance.GetText("mark_weekly_forest_clear");
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_last_stage")).Append(" ");
				switch(myRankData.u1MyDifficulty)
                {
                    case 1:
						tempStringBuilder.Append("[").Append(TextManager.Instance.GetText("btn_diffi_easy")).Append("]");
                        break;

                    case 2:
						tempStringBuilder.Append("[").Append(TextManager.Instance.GetText("btn_diffi_normal")).Append("]");
                        break;

                    case 3:
						tempStringBuilder.Append("[").Append(TextManager.Instance.GetText("btn_diffi_hell")).Append("]");
                        break;
                }
                _tabs[_type].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
				if(StageInfoMgr.Instance.dicStageData.ContainsKey((UInt16)myRankData.u4MyValue))
				{
					StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[(UInt16)myRankData.u4MyValue];
					tempStringBuilder.Append(TextManager.Instance.GetText("line_element_"+ (stageInfo.u1ForestElement - 1).ToString())).Append(" ");
					/*
	                switch(StageInfoMgr.Instance.dicStageData[(UInt16)RankInfoMgr.Instance.dicRankListDetailData[(UInt16)(RankInfoMgr.Instance.dicRankListDetailData.Count-1)].u4MyValue].u1ForestElement)
	                {
	                    case 2:
	                        tempStringBuilder.Append(TextManager.Instance.GetText("line_element_1")).Append(" ");
	                        break;

	                    case 3:
	                        tempStringBuilder.Append(TextManager.Instance.GetText("line_element_2")).Append(" ");
	                        break;

	                    case 4:
	                        tempStringBuilder.Append(TextManager.Instance.GetText("line_element_3")).Append(" ");
	                        break;
	                }
	                */
					tempStringBuilder.Append(stageInfo.chapterInfo.u1Number.ToString()).Append("-");
					tempStringBuilder.Append(stageInfo.u1StageNum.ToString());
	                tempStringBuilder.Append("(");
					tempStringBuilder.Append(myRankData.dtMyClearTime.Month.ToString()).Append("/")
						.Append(myRankData.dtMyClearTime.Day.ToString()).Append(" ")
						.Append(myRankData.dtMyClearTime.ToString("HH:mm"));
	                tempStringBuilder.Append(")");
	                _tabs[_type].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
				}
				else
				{
					_tabs[_type].transform.GetChild(2).GetComponent<Text>().text = " - ";
				}
                _rankInfoTitle.text = TextManager.Instance.GetText("info_weekly_forest_clear");
                _rankInfoDesc.text = TextManager.Instance.GetText("desc_weekly_forest_clear");
                break;

            case 7:
                _title.text = TextManager.Instance.GetText("mark_weekly_tower_clear");
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_last_stage")).Append(" ");
				switch(myRankData.u1MyDifficulty)
                {
                    case 1:
						tempStringBuilder.Append("[").Append(TextManager.Instance.GetText("btn_diffi_easy")).Append("]");
                        break;
                    case 2:
						tempStringBuilder.Append("[").Append(TextManager.Instance.GetText("btn_diffi_normal")).Append("]");
                        break;
                    case 3:
						tempStringBuilder.Append("[").Append(TextManager.Instance.GetText("btn_diffi_hell")).Append("]");
                        break;
                }
				_tabs[_type].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
				tempStringBuilder.Remove(0, tempStringBuilder.Length);
				if(StageInfoMgr.Instance.dicStageData.ContainsKey((UInt16)myRankData.u4MyValue))
				{
					StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[(UInt16)myRankData.u4MyValue];
					tempStringBuilder.Append(TextManager.Instance.GetText(stageInfo.chapterInfo.strName)).Append(" ");
					tempStringBuilder.Append(stageInfo.u1StageNum.ToString());
					tempStringBuilder.Append("(");
					tempStringBuilder.Append(myRankData.dtMyClearTime.Month.ToString()).Append("/")
						.Append(myRankData.dtMyClearTime.Day.ToString()).Append(" ")
						.Append(myRankData.dtMyClearTime.ToString("HH:mm"));
					tempStringBuilder.Append(")");
					_tabs[_type].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
				}
				else
				{
					_tabs[_type].transform.GetChild(2).GetComponent<Text>().text = " - ";
				}
                _rankInfoTitle.text = TextManager.Instance.GetText("info_weekly_tower_clear");
                _rankInfoDesc.text = TextManager.Instance.GetText("desc_weekly_tower_clear");
				break;
        }
    }
}
