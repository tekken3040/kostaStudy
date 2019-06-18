using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LeagueInfoMgr : Singleton<LeagueInfoMgr>
{
    public Dictionary<UInt16, LeagueInfo> dicLeagueData;

    private bool loadedInfo = false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}

    public void Init()
	{
		dicLeagueData = new Dictionary<UInt16, LeagueInfo>();
		DataMgr.Instance.LoadTable(this.AddInfo, "League");
	}

    public void AddInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		LeagueInfo info = new LeagueInfo();
		info.Set(cols);
		dicLeagueData.Add(info.u2LeagueID, info);
	}

    public LeagueInfo GetLeagueInfo(UInt16 _key)
    {
        LeagueInfo ret;

        dicLeagueData.TryGetValue(_key, out ret);

        return ret;
    }
}
