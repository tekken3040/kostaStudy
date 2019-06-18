using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class RankTab : MonoBehaviour
{
    public GameObject _userPower;
    public GameObject _crewPower;
    public GameObject _totalGold;
    public GameObject _totalCash;
    public GameObject _weeklyCrafting;
    public GameObject _weeklyCampaign;
    public GameObject _weeklySearchForest;
    public GameObject _weeklyTower;
    public GameObject scrollList;
    public GameObject myCrewScrollList;
    public GameObject infoGroup;
    public GameObject categoryGroup;
    public GameObject _rankBoard;
    public GameObject _rewardInfo;
    public Text _title;
    public Text _title2;
    public Toggle _myCrewRank;
    public ScrollRect scrollGroup;
    public GameObject[] _rankInfo;
    public GameObject[] _scrollCategory;
    public GameObject[] _prefObjects;
    StringBuilder tempStringBuilder;
    StreamWriter _sw;
    StreamReader _sr;

    public void Awake()
    {
        tempStringBuilder = new StringBuilder();
    }

    public void OnEnable()
    {
        RankInfoMgr.Instance.RefreashRankData();
        RankInfoMgr.Instance.ClearData();
        _myCrewRank.isOn = false;
        RequestRankInfo();
        Legion.Instance.u1RankRewad = 0;
    }

    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(_rankBoard);
        PopupManager.Instance.RemovePopup(_rewardInfo);
        _myCrewRank.isOn = false;
        SetNewRankData(rankType);
        _rankBoard.SetActive(false);
        _rewardInfo.SetActive(false);
        RankInfoMgr.Instance.ClearDetailData();
        for(int i=0; i<scrollList.transform.childCount; i++)
            Destroy(scrollList.transform.GetChild(i).gameObject);
        for(int i=0; i<myCrewScrollList.transform.childCount; i++)
            Destroy(myCrewScrollList.transform.GetChild(i).gameObject);
        scrollList.transform.localPosition = Vector3.zero;
        myCrewScrollList.transform.localPosition = Vector3.zero;
    }

    public void SetNewRankData(int _type)
    {
        if(RankInfoMgr.Instance.dicRankSaveCategoryData.ContainsKey((UInt16)(_type+1)))
            RankInfoMgr.Instance.dicRankSaveCategoryData.Remove((UInt16)(_type+1));
        RankSaveCategory _save = new RankSaveCategory();
        _save.dicRankSaveData = new Dictionary<UInt64, RankSaveInfo>();
        for(int i=0; i<RankInfoMgr.Instance.u1RankListDetailCount; i++)
        {
            RankSaveInfo _info = new RankSaveInfo();
            _info.strCrewName = RankInfoMgr.Instance.dicRankListDetailData[(UInt16)i].strLegionName;
            _info.u8Rank = RankInfoMgr.Instance.dicRankListDetailData[(UInt16)i].u4Rank;
            _save.dicRankSaveData.Add(_info.u8Rank, _info);
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            if (i == RankInfoMgr.Instance.u1RankListDetailCount-1)
                _sw.Write(tempStringBuilder.Append(_info.strCrewName).Append("\t").Append(_info.u8Rank).Append("\t").ToString());
            else
                _sw.WriteLine(tempStringBuilder.Append(_info.strCrewName).Append("\t").Append(_info.u8Rank.ToString()).Append("\t").ToString());
        }
        _sw.Flush();
        _sw.Close();
        RankInfoMgr.Instance.dicRankSaveCategoryData.Add((UInt16)(_type+1), _save);
    }

    public void RequestRankInfo()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestRankInfo(RecvRankInfo);
    }

    public void RecvRankInfo(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.RANK_INFO, err), Server.ServerMgr.Instance.CallClear);
		}
        else
        {
            for(int i=0; i<8; i++)
            {
                if(RankInfoMgr.Instance.dicRankListData.ContainsKey((UInt16)i))
                switch(RankInfoMgr.Instance.dicRankListData[(UInt16)i].u1RankType)
                {
                    case 1:
                        _userPower.GetComponent<RankListUserPower>().SetData(RankInfoMgr.Instance.dicRankListData[(UInt16)i]);
                        break;

                    case 2:
                        _crewPower.GetComponent<RankListCrewPower>().SetData(RankInfoMgr.Instance.dicRankListData[(UInt16)i]);
                        break;

                    case 3:
                        _totalGold.GetComponent<RankListTotalGold>().SetData(RankInfoMgr.Instance.dicRankListData[(UInt16)i]);
                        break;

                    case 4:
                        _totalCash.GetComponent<RankListTotalCash>().SetData(RankInfoMgr.Instance.dicRankListData[(UInt16)i]);
                        break;

                    case 5:
                        _weeklyCrafting.GetComponent<RankListWeeklyCrafting>().SetData(RankInfoMgr.Instance.dicRankListData[(UInt16)i]);
                        break;

                    case 6:
                        _weeklyCampaign.GetComponent<RankListWeeklyCampaign>().SetData(RankInfoMgr.Instance.dicRankListData[(UInt16)i]);
                        break;

                    case 7:
                        _weeklySearchForest.GetComponent<RankListWeeklySearchForest>().SetData(RankInfoMgr.Instance.dicRankListData[(UInt16)i]);
                        break;

                    case 8:
                        _weeklyTower.GetComponent<RankListWeeklyTower>().SetData(RankInfoMgr.Instance.dicRankListData[(UInt16)i]);
                        break;
                }
            }
        }
    }
    int rankType;
    public void OnClickList(int _index)
    {
        if(!RankInfoMgr.Instance.dicRankListData.ContainsKey((UInt16)_index))
        {
			// 이호영 영어
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_battlefield_rank"), TextManager.Instance.GetText("rank_ready_new_season"), null);
            return;
        }
        for(int i=0; i<infoGroup.transform.childCount; i++)
            infoGroup.transform.GetChild(i).gameObject.SetActive(false);
        for(int i=0; i<categoryGroup.transform.childCount; i++)
            categoryGroup.transform.GetChild(i).gameObject.SetActive(false);
        _rankInfo[_index].SetActive(true);
        _scrollCategory[_index].SetActive(true);
        rankType = _index;
        RankInfoMgr.Instance.u1RankType = (Byte)(_index+1);
        RankInfoMgr.Instance.ClearDetailData();
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestRankList((Byte)(_index+1), RecvRankDetailInfo);
    }

    public void RecvRankDetailInfo(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.RANK_LIST, err), Server.ServerMgr.Instance.CallClear);
		}
        else
        {
#if UNITY_EDITOR
            //_sw = new StreamWriter("Assets/Resources_Bundle/TextScripts/RankSaves/RankType"+(rankType+1)+".txt");
            _sw = new StreamWriter(Application.dataPath + "/RankType"+(rankType+1)+".txt");
#else
            _sw = new StreamWriter(Application.persistentDataPath + "/RankType"+(rankType+1)+".txt");
#endif	
            PopupManager.Instance.AddPopup(_rankBoard, OnClickClose);
            _rankBoard.SetActive(true);
            for(int i=0; i<infoGroup.transform.childCount; i++)
                infoGroup.transform.GetChild(i).gameObject.SetActive(false);
            for(int i=0; i<categoryGroup.transform.childCount; i++)
                categoryGroup.transform.GetChild(i).gameObject.SetActive(false);
            _rankInfo[rankType].SetActive(true);
            _scrollCategory[rankType].SetActive(true);
            InitInfo();
            for(int i=0; i<RankInfoMgr.Instance.u1RankListDetailCount; i++)
            {
                InitSlots((UInt16)i);
            }
            if(rankType == 1)
                for(int i=0; i<RankInfoMgr.Instance.u1MyCount; i++)
                    InitMyCrewSlot((UInt16)i);
        }
    }

    public void OnClickMyCrewRank()
    {
        if(_myCrewRank.isOn)
        {
            _myCrewRank.transform.GetChild(0).GetComponent<Text>().text = TextManager.Instance.GetText("btn_crew_rank");
            //for(int i=0; i<RankInfoMgr.Instance.u1RankListDetailCount; i++)
            //{
            //    scrollList.transform.GetChild(i).gameObject.SetActive(false);
            //}
            //for(int i=0; i<RankInfoMgr.Instance.u1MyCount; i++)
            //    scrollList.transform.GetChild((int)RankInfoMgr.Instance.dicMyCrewRankData[(UInt16)i].u8MyRank-1).gameObject.SetActive(true);
            scrollList.SetActive(false);
            myCrewScrollList.SetActive(true);
            scrollGroup.content = myCrewScrollList.GetComponent<RectTransform>();
        }
        else
        {
            _myCrewRank.transform.GetChild(0).GetComponent<Text>().text = TextManager.Instance.GetText("btn_my_crew_rank");
            //for(int i=0; i<RankInfoMgr.Instance.u1RankListDetailCount; i++)
            //{
            //    scrollList.transform.GetChild(i).gameObject.SetActive(true);
            //}
            scrollList.SetActive(true);
            myCrewScrollList.SetActive(false);
            scrollGroup.content = scrollList.GetComponent<RectTransform>();
            //for(int i=RankInfoMgr.Instance.u1RankListDetailCount; i<scrollList.transform.GetChildCount(); i++)
            //{
            //    scrollList.transform.GetChild(i).gameObject.SetActive(false);
            //}
        }
    }

    public void OnClickInfoList()
    {
        PopupManager.Instance.RemovePopup(_rankBoard);
        PopupManager.Instance.AddPopup(_rewardInfo, OnClickClose);
        _rewardInfo.SetActive(true);
        _rewardInfo.GetComponent<RankRewardInfo>().SetData((Byte)rankType);
        _rankBoard.SetActive(false);
    }

    public void InitInfo()
    {
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_rank2")).Append(" ");
		RankListDetail myRankDetail = RankInfoMgr.Instance.dicRankListDetailData[(UInt16)(RankInfoMgr.Instance.dicRankListDetailData.Count-1)];
		if(myRankDetail.u8MyRank == 0)
            tempStringBuilder.Append("- ");
        else
            tempStringBuilder.Append(RankInfoMgr.Instance.dicRankListData[(UInt16)rankType].u8MyRank);
        if(rankType != 1)
            _rankInfo[rankType].transform.GetChild(0).GetComponent<Text>().text = tempStringBuilder.ToString();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        switch(rankType)
        {
            case 0:
                _title.text = TextManager.Instance.GetText("mark_user_power");
                _title2.text = TextManager.Instance.GetText("mark_user_power");
				tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_power")).Append(" ").Append(myRankDetail.u4MyValue);
                _rankInfo[rankType].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case 1:
                _title.text = TextManager.Instance.GetText("mark_crew_power");
                _title2.text = TextManager.Instance.GetText("mark_crew_power");
                break;

            case 2:
                _title.text = TextManager.Instance.GetText("mark_total_gold");
                _title2.text = TextManager.Instance.GetText("mark_total_gold");
				tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_gold")).Append(" ").Append(myRankDetail.u4MyValue);
                _rankInfo[rankType].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case 3:
                _title.text = TextManager.Instance.GetText("mark_weekly_used_cash");
                _title2.text = TextManager.Instance.GetText("mark_weekly_used_cash");
				tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_cash")).Append(" ").Append(myRankDetail.u4MyValue);
                _rankInfo[rankType].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case 4:
                _title.text = TextManager.Instance.GetText("mark_weekly_crafting");
                _title2.text = TextManager.Instance.GetText("mark_weekly_crafting");
				tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_craft_point")).Append(" ").Append(myRankDetail.u4MyValue);
                _rankInfo[rankType].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
				tempStringBuilder.Append("(").Append(myRankDetail.u4MyMakingCount);
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_craft_count")).Append(")");
                _rankInfo[rankType].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case 5:
			{
                _title.text = TextManager.Instance.GetText("mark_weekly_campaign_clear");
                _title2.text = TextManager.Instance.GetText("mark_weekly_campaign_clear");
				if(myRankDetail.u8MyRank == 0)
                {
                    tempStringBuilder.Append(" - ");
                    _rankInfo[rankType].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
                    _rankInfo[rankType].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                    break;
                }
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_last_stage")).Append(" ");
				switch(myRankDetail.u1MyDifficulty)
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

				if(StageInfoMgr.Instance.dicStageData.ContainsKey((UInt16)myRankDetail.u4MyValue))
				{
					StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[(UInt16)myRankDetail.u4MyValue];
					tempStringBuilder.Append(stageInfo.actInfo.u1Number.ToString());
	                tempStringBuilder.Append(TextManager.Instance.GetText("mark_act")).Append(" ");
					tempStringBuilder.Append(stageInfo.chapterInfo.u1Number.ToString()).Append(" - ");
					tempStringBuilder.Append(stageInfo.u1StageNum.ToString());
	                _rankInfo[rankType].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();

	                tempStringBuilder.Remove(0, tempStringBuilder.Length);
	                tempStringBuilder.Append("(");
					tempStringBuilder.Append(myRankDetail.dtMyClearTime.Month.ToString()).Append("/")
								.Append(myRankDetail.dtMyClearTime.Day.ToString()).Append(" ")
								.Append(myRankDetail.dtMyClearTime.ToString("HH:mm"));
	                tempStringBuilder.Append(")");
	                _rankInfo[rankType].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
				}
				else
				{
					_rankInfo[rankType].transform.GetChild(1).GetComponent<Text>().text = " - ";
					_rankInfo[rankType].transform.GetChild(2).GetComponent<Text>().text = " - ";
				}
			}
                break;

            case 6:
                _title.text = TextManager.Instance.GetText("mark_weekly_forest_clear");
                _title2.text = TextManager.Instance.GetText("mark_weekly_forest_clear");
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_last_stage")).Append(" ");
				switch(myRankDetail.u1MyDifficulty)
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
                _rankInfo[rankType].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
				if(StageInfoMgr.Instance.dicStageData.ContainsKey((UInt16)myRankDetail.u4MyValue))
				{
					StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[(UInt16)myRankDetail.u4MyValue];
					tempStringBuilder.Append(TextManager.Instance.GetText("line_element_"+ (stageInfo.u1ForestElement -1).ToString() )).Append(" ");
					tempStringBuilder.Append(stageInfo.chapterInfo.u1Number.ToString()).Append("-");
					tempStringBuilder.Append(stageInfo.u1StageNum.ToString());
	                tempStringBuilder.Append("(");
					tempStringBuilder.Append(myRankDetail.dtMyClearTime.Month.ToString()).Append("/")
						.Append(myRankDetail.dtMyClearTime.Day.ToString()).Append(" ")
						.Append(myRankDetail.dtMyClearTime.ToString("HH:mm"));
	                tempStringBuilder.Append(")");
	                _rankInfo[rankType].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
				}
				else
				{
					_rankInfo[rankType].transform.GetChild(2).GetComponent<Text>().text = " - ";
				}
                break;
            case 7:
                _title.text = TextManager.Instance.GetText("mark_weekly_tower_clear");
                _title2.text = TextManager.Instance.GetText("mark_weekly_tower_clear");
				tempStringBuilder.Append(TextManager.Instance.GetText("mark_my_last_stage")).Append(" ");
				switch(myRankDetail.u1MyDifficulty)
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
				_rankInfo[rankType].transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();

                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                if(StageInfoMgr.Instance.dicStageData.ContainsKey((UInt16)myRankDetail.u4MyValue))
				{
					StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[(UInt16)myRankDetail.u4MyValue];
					tempStringBuilder.Append(TextManager.Instance.GetText(stageInfo.chapterInfo.strName)).Append(" ");
					tempStringBuilder.Append(stageInfo.u1StageNum.ToString());//myRankDetail.u4MyValue.ToString());
	                tempStringBuilder.Append("(");
					tempStringBuilder.Append(myRankDetail.dtMyClearTime.Month.ToString()).Append("/")
						.Append(myRankDetail.dtMyClearTime.Day.ToString()).Append(" ")
						.Append(myRankDetail.dtMyClearTime.ToString("HH:mm"));
	                tempStringBuilder.Append(")");
	                _rankInfo[rankType].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
				}
				else
				{
					_rankInfo[rankType].transform.GetChild(2).GetComponent<Text>().text = " - ";
				}
                break;
        }
    }

    public void InitSlots(UInt16 num)
    {
        GameObject _obj = Instantiate(_prefObjects[rankType]);
        _obj.transform.SetParent(scrollList.transform);
        _obj.transform.localPosition = Vector3.zero;
        _obj.transform.localScale = Vector3.one;
        _obj.name = _prefObjects[rankType].name;
        switch(rankType)
        {
            case 0:
                _obj.GetComponent<RankSlotUserPower>().SetData(RankInfoMgr.Instance.dicRankListDetailData[num]);
                break;

            case 1:
                if(!_myCrewRank.isOn)
                    _obj.GetComponent<RankSlotCrewPower>().SetData(RankInfoMgr.Instance.dicRankListDetailData[num]);
                //else
                //    _obj.GetComponent<RankSlotCrewPower>().SetDataMyCrew(RankInfoMgr.Instance.dicMyCrewRankData[num]);
                break;

            case 2:
                _obj.GetComponent<RankSlotTotalGold>().SetData(RankInfoMgr.Instance.dicRankListDetailData[num]);
                break;

            case 3:
                _obj.GetComponent<RankSlotWeeklyCash>().SetData(RankInfoMgr.Instance.dicRankListDetailData[num]);
                break;

            case 4:
                _obj.GetComponent<RankSlotWeeklyCrafting>().SetData(RankInfoMgr.Instance.dicRankListDetailData[num]);
                break;

            case 5:
                _obj.GetComponent<RankSlotWeeklyCampaign>().SetData(RankInfoMgr.Instance.dicRankListDetailData[num]);
                break;

            case 6:
                _obj.GetComponent<RankSlotWeeklyForest>().SetData(RankInfoMgr.Instance.dicRankListDetailData[num]);
                break;

            case 7:
                _obj.GetComponent<RankSlotWeeklyTower>().SetData(RankInfoMgr.Instance.dicRankListDetailData[num]);
                break;
        }
    }

    public void InitMyCrewSlot(UInt16 num)
    {
        GameObject _obj = Instantiate(_prefObjects[rankType]);
        _obj.transform.SetParent(myCrewScrollList.transform);
        _obj.transform.localPosition = Vector3.zero;
        _obj.transform.localScale = Vector3.one;
        _obj.name = _prefObjects[rankType].name;

        _obj.GetComponent<RankSlotCrewPower>().SetDataMyCrew(RankInfoMgr.Instance.dicMyCrewRankData[num]);
    }
}
