using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SocketInfoMgr : Singleton<SocketInfoMgr>
{
	private Dictionary<UInt16, SocketInfo> dicData;

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
		SocketInfo info = new SocketInfo();
		info.SetInfo(cols);
		dicData.Add(info.u2ID, info);
	}

	public SocketInfo GetInfo(UInt16 id)
	{
		SocketInfo ret;
		dicData.TryGetValue(id, out ret);
		return ret;
	}

	public void Init()
	{
		dicData = new Dictionary<UInt16, SocketInfo>();
		DataMgr.Instance.LoadTable(this.AddInfo, "Socket");
	}
}

