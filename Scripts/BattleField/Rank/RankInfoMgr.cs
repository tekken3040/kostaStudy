using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class RankInfoMgr : Singleton<RankInfoMgr>
{
    public Dictionary<UInt16, RankInfo> dicRankData;
    public Dictionary<UInt16, RankSaveCategory> dicRankSaveCategoryData;
    public Dictionary<UInt64, RankSaveInfo> dicUserPowerData;
    public Dictionary<UInt64, RankSaveInfo> dicCrewPowerData;
    public Dictionary<UInt64, RankSaveInfo> dicTotalGoldData;
    public Dictionary<UInt64, RankSaveInfo> dicWeeklyCashData;
    public Dictionary<UInt64, RankSaveInfo> dicWeeklyCraftData;
    public Dictionary<UInt64, RankSaveInfo> dicWeeklyCampaignData;
    public Dictionary<UInt64, RankSaveInfo> dicWeeklyForestData;
    public Dictionary<UInt64, RankSaveInfo> dicWeeklyTowerData;

    //랭크 리스트
    public Dictionary<UInt16, RankListInfo> dicRankListData;
    public Byte u1RankListCount;

    //랭크 세부 정보
    public Dictionary<UInt16, RankListDetail> dicRankListDetailData;
    public Byte u1RankListDetailCount;
        
    //내 크루 순위 정보
    public Dictionary<UInt16, MyCrewRank> dicMyCrewRankData;
    public Byte u1MyCount;

    //랭크 보상
    public Dictionary<UInt16, RankReward> dicRankRewardData;
    public Byte u1RankRewardCount;


    public Byte u1RankType;
    StreamReader _sr;
    StreamWriter _sw;

    private bool loadedInfo = false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}

    public void Awake()
    {
        dicRankListData = new Dictionary<UInt16, RankListInfo>();
        dicRankListDetailData = new Dictionary<UInt16, RankListDetail>();
        dicRankRewardData = new Dictionary<UInt16, RankReward>();
        dicMyCrewRankData = new Dictionary<UInt16, MyCrewRank>();

        u1RankListCount = 0;
        u1RankListDetailCount = 0;
        u1RankRewardCount = 0;
        u1MyCount = 0;
    }

    public void ClearData()
    {
        dicRankListData.Clear();
        dicRankListDetailData.Clear();
        //dicRankRewardData.Clear();
        dicMyCrewRankData.Clear();
        u1RankListCount = 0;
        u1RankListDetailCount = 0;
        //u1RankRewardCount = 0;
        u1MyCount = 0;
    }

    public void ClearDetailData()
    {
        dicRankListDetailData.Clear();
        u1RankListDetailCount = 0;
    }

    public void Init()
	{
		dicRankData = new Dictionary<UInt16, RankInfo>();
        dicRankSaveCategoryData = new Dictionary<UInt16, RankSaveCategory>();
        dicUserPowerData = new Dictionary<UInt64, RankSaveInfo>();
        dicCrewPowerData = new Dictionary<UInt64, RankSaveInfo>();
        dicTotalGoldData = new Dictionary<UInt64, RankSaveInfo>();
        dicWeeklyCashData = new Dictionary<UInt64, RankSaveInfo>();
        dicWeeklyCraftData = new Dictionary<UInt64, RankSaveInfo>();
        dicWeeklyCampaignData = new Dictionary<UInt64, RankSaveInfo>();
        dicWeeklyForestData = new Dictionary<UInt64, RankSaveInfo>();
        dicWeeklyTowerData = new Dictionary<UInt64, RankSaveInfo>();
		DataMgr.Instance.LoadTable(this.AddInfo, "Rank");
        for(int i=0; i<8; i++)
        {
            LoadRankData(i+1);
            RankSaveCategory _category = new RankSaveCategory();
            _category.dicRankSaveData = GetRankDictionary(i);
            dicRankSaveCategoryData.Add((UInt16)(i+1), _category);
        }
	}
    public void LoadRankData(int rankType)
    {
#if UNITY_EDITOR
        //_sr = new StreamReader("Assets/Resources_Bundle/TextScripts/RankSaves/RankType"+(rankType)+".txt");
        _sw = new StreamWriter(Application.dataPath + "/RankType"+(rankType)+".txt");
        _sw.Flush();
        _sw.Close();
        _sr = new StreamReader(Application.dataPath + "/RankType"+(rankType)+".txt");
#else
        _sw = new StreamWriter(Application.persistentDataPath + "/RankType"+(rankType+1)+".txt");
        _sw.Flush();
        _sw.Close();
        _sr = new StreamReader(Application.persistentDataPath + "/RankType"+(rankType+1)+".txt");
#endif		
        string[] parseRow;
        if(_sr.Read().ToString().Trim() != "-1")
            parseRow = _sr.ReadToEnd().Split('\n');
            //parseRow = _sr.Read().ToString().Split('\n');
        else
            parseRow = new string[0];
        //string[] parseRow = _sr.ReadLine().ToString().Split('\n');
        _sr.Close();
        int startRow = 0;

		for (int readRow = startRow; readRow < parseRow.Length; readRow++)
		{
			if (parseRow [readRow].Trim () == "")
				return;
            switch(rankType)
            {
                case 1:
                    SetRankSaveData1(parseRow[readRow].Split('\t'));
                    break;
                case 2:
                    SetRankSaveData2(parseRow[readRow].Split('\t'));
                    break;
                case 3:
                    SetRankSaveData3(parseRow[readRow].Split('\t'));
                    break;
                case 4:
                    SetRankSaveData4(parseRow[readRow].Split('\t'));
                    break;
                case 5:
                    SetRankSaveData5(parseRow[readRow].Split('\t'));
                    break;
                case 6:
                    SetRankSaveData6(parseRow[readRow].Split('\t'));
                    break;
                case 7:
                    SetRankSaveData7(parseRow[readRow].Split('\t'));
                    break;
                case 8:
                    SetRankSaveData8(parseRow[readRow].Split('\t'));
                    break;
            }
		}
    }
    public void RefreashRankData()
    {
        dicRankSaveCategoryData.Clear();
        for(int i=0; i<8; i++)
        {
            RankSaveCategory _category = new RankSaveCategory();
            _category.dicRankSaveData = GetRankDictionary(i);
            dicRankSaveCategoryData.Add((UInt16)(i+1), _category);
        }
    }
    public Dictionary<UInt64, RankSaveInfo> GetRankDictionary(int _type)
    {
        switch(_type)
        {
            case 0:
                return dicUserPowerData;
            case 1:
                return dicCrewPowerData;
            case 2:
                return dicTotalGoldData;
            case 3:
                return dicWeeklyCashData;
            case 4:
                return dicWeeklyCraftData;
            case 5:
                return dicWeeklyCampaignData;
            case 6:
                return dicWeeklyForestData;
            case 7:
                return dicWeeklyTowerData;
        }
        return null;
    }

    public void AddInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		RankInfo info = new RankInfo();
		info.Set(cols);
		dicRankData.Add(info.u1RankType, info);
	}

    public void SetRankSaveData1(string[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        RankSaveInfo info = new RankSaveInfo();
        info.Set(cols);
        dicUserPowerData.Add(info.u8Rank, info);
    }
    public void SetRankSaveData2(string[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        RankSaveInfo info = new RankSaveInfo();
        info.Set(cols);
        dicCrewPowerData.Add(info.u8Rank, info);
    }
    public void SetRankSaveData3(string[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        RankSaveInfo info = new RankSaveInfo();
        info.Set(cols);
        dicTotalGoldData.Add(info.u8Rank, info);
    }
    public void SetRankSaveData4(string[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        RankSaveInfo info = new RankSaveInfo();
        info.Set(cols);
        dicWeeklyCashData.Add(info.u8Rank, info);
    }
    public void SetRankSaveData5(string[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        RankSaveInfo info = new RankSaveInfo();
        info.Set(cols);
        dicWeeklyCraftData.Add(info.u8Rank, info);
    }
    public void SetRankSaveData6(string[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        RankSaveInfo info = new RankSaveInfo();
        info.Set(cols);
        dicWeeklyCampaignData.Add(info.u8Rank, info);
    }
    public void SetRankSaveData7(string[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        RankSaveInfo info = new RankSaveInfo();
        info.Set(cols);
        dicWeeklyForestData.Add(info.u8Rank, info);
    }
    public void SetRankSaveData8(string[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        RankSaveInfo info = new RankSaveInfo();
        info.Set(cols);
        dicWeeklyTowerData.Add(info.u8Rank, info);
    }
}

public class RankListInfo
{
    public Byte u1RankType;
    public string strLegionName;
    public string strCharName;
    public Byte u1CrewIndex;
    public UInt16[] u2CharClassID;
    public UInt32 u4Value;
    public UInt32 u4MakingCount;
    public Byte u1Difficulty;
    public Int64 u8ClearTime;
    public DateTime dtClearTime;
    public UInt64 u8MyRank;
}

public class RankListDetail
{
    public Byte u1RankType;
    public UInt32 u4Rank;
    public string strLegionName;
    public string strCharName;
    public Byte u1CrewIndex;
    public UInt16[] u2CharClassID;
    public UInt32 u4Value;
    public UInt32 u4MakingCount;
    public Byte u1Difficulty;
    public Int64 u8ClearTime;
    public DateTime dtClearTime;
    public UInt64 u8MyRank;
    public UInt32 u4MyValue;
    public UInt32 u4MyMakingCount;
    public Byte u1MyDifficulty;
    public Int64 u8MyClearTime;
    public DateTime dtMyClearTime;
}

public struct MyCrewRank
{
    public Byte u1CrewIndex;
    public UInt64 u8MyRank;
    public UInt32 u4MyPower;
}

public struct RankReward
{
    public Byte u1RankType;
    public UInt32 u4Rank;
    public Byte u1RewardIndex;
}

public struct RankSaveCategory
{
    public Dictionary<UInt64, RankSaveInfo> dicRankSaveData;
}

public struct RankSave
{
    public string strCrewName;
    public UInt64 u8Rank;
}