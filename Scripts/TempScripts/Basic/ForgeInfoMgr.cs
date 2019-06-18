using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ForgeInfoMgr : Singleton<ForgeInfoMgr>
{
	Dictionary<UInt16, ForgeInfo> dicInfo;
    public Dictionary<Byte, ForgeInfo.UnlockCategoryInfo> dicUnlockInfo;

	private bool loadedInfo=false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}

	Byte forgeLevel=1;
	public void AddInfo(string[] cols)
	{
		if (cols == null)
		{
			return;
		}
		ForgeInfo info = new ForgeInfo();
		dicInfo.Add(info.SetInfo(cols, forgeLevel++), info);
		//		DebugMgr.Log(string.Format("add dicConsumableItemData : {0} {1}", dicConsumableItemData.ContainsKey(info.u2ID), dicConsumableItemData.ContainsValue(info)));
	}

    public void AddUnlockCategoryInfo(string[] cols)
    {
        if (cols == null)
		{
			return;
		}
        ForgeInfo.UnlockCategoryInfo info = new ForgeInfo.UnlockCategoryInfo();
        dicUnlockInfo.Add(info.Set(cols), info);
    }
//	public void AddMaterialInfo(string[] cols)
//	{
//		if(cols == null)
//		{
//			return;
//		}
//		ForgeInfo forgeInfo;
//		UInt16 forgeID = Convert.ToUInt16(cols[0]);
//		dicInfo.TryGetValue(forgeID, out forgeInfo);
//
//		forgeInfo.cSmithingInfo.AddMaterial(cols);
//	}

	public ForgeInfo GetInfo(UInt16 id)
	{
		ForgeInfo ret;
		dicInfo.TryGetValue(id, out ret);
		return ret;
	}

	public UInt16[] GetIDs()
	{
		UInt16[] ret = new UInt16[dicInfo.Keys.Count];
		dicInfo.Keys.CopyTo(ret, 0);
		return ret;
	}

	public List<ForgeInfo> GetList()
	{
		List<ForgeInfo> ret = new List<ForgeInfo>();
		ret.Clear();
		UInt16[] IDs = GetIDs();
		for(int i=0; i<IDs.Length; i++)
		{
			ForgeInfo forgeInfo = dicInfo[IDs[i]];
			ret.Add(forgeInfo);
		}
		return ret;
	}

	public void Init()
	{
		dicInfo = new Dictionary<UInt16, ForgeInfo>();
        dicUnlockInfo = new Dictionary<Byte, ForgeInfo.UnlockCategoryInfo>();
		DataMgr.Instance.LoadTable(this.AddInfo, "Smithing");
        DataMgr.Instance.LoadTable(this.AddUnlockCategoryInfo, "SmithingOpen");
//		DataMgr.Instance.LoadTable(this.AddMaterialInfo, "EquipSmith");
	}
}
