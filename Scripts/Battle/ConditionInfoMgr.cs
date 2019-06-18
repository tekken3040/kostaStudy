using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ConditionInfoMgr : Singleton<ConditionInfoMgr>
{
	private Dictionary<UInt16, ConditionInfo> dicData;

	private bool loadedInfo=false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}

	public void AddInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		ConditionInfo info = new ConditionInfo();
		info.SetInfo(cols);
		dicData.Add(info.u2ID, info);
	}

	public ConditionInfo GetInfo(UInt16 id)
	{
		ConditionInfo ret;
		dicData.TryGetValue(id, out ret);
		return ret;
	}

	public void Init()
	{
		dicData = new Dictionary<UInt16, ConditionInfo>();
		DataMgr.Instance.LoadTable(this.AddInfo, "Condition");
	}
}

