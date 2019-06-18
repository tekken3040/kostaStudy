using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ItemInfoMgr : Singleton<ItemInfoMgr>
{
	private Dictionary<UInt16, ConsumableItemInfo> dicConsumableItemData;
	private Dictionary<UInt16, MaterialItemInfo> dicMaterialItemData;
	public Dictionary<UInt16, RuneInfo> dicRuneData;

	private bool loadedInfo=false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}
	
	public void AddConsumableItemInfo(string[] cols)
	{
		if (cols == null)
		{
			return;
		}
		ConsumableItemInfo info = new ConsumableItemInfo();
		dicConsumableItemData.Add(info.SetInfo(cols), info);
//		DebugMgr.Log(string.Format("add dicConsumableItemData : {0} {1}", dicConsumableItemData.ContainsKey(info.u2ID), dicConsumableItemData.ContainsValue(info)));
	}
	public Dictionary<UInt16, ConsumableItemInfo> GetConsumableItemInfo()
    {
        return dicConsumableItemData;
    }
	public ConsumableItemInfo GetConsumableItemInfo(UInt16 id)
	{
		ConsumableItemInfo ret;
		dicConsumableItemData.TryGetValue(id, out ret);
		return ret;
	}

	public List<ConsumableItemInfo> GetConsumableList()
	{
		List<ConsumableItemInfo> ret = new List<ConsumableItemInfo>();
		ret.Clear();
		foreach(KeyValuePair<UInt16, ConsumableItemInfo> pair in dicConsumableItemData)
		{
			ret.Add(pair.Value);
		}

		return ret;
	}

	public void AddMaterialItemInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		MaterialItemInfo info = new MaterialItemInfo();
		dicMaterialItemData.Add(info.SetInfo(cols), info);
//		DebugMgr.Log(string.Format("add dicMaterialItemData : {0} {1}", dicMaterialItemData.ContainsKey(info.u2ID), dicMaterialItemData.ContainsValue(info)));
	}

	public MaterialItemInfo GetMaterialItemInfo(UInt16 id)
	{
		MaterialItemInfo ret;
		dicMaterialItemData.TryGetValue(id, out ret);
		return ret;
	}

	public List<MaterialItemInfo> GetMaterialList()
	{
		List<MaterialItemInfo> ret = new List<MaterialItemInfo>();
		ret.Clear();
		foreach(KeyValuePair<UInt16, MaterialItemInfo> pair in dicMaterialItemData)
		{
			ret.Add(pair.Value);
		}

		return ret;
	}

	public void AddRuneInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		RuneInfo info = new RuneInfo();
		dicRuneData.Add(info.SetInfo(cols), info);
	}

	public RuneInfo GetRuneInfo(UInt16 id)
	{
		RuneInfo ret;
		dicRuneData.TryGetValue(id, out ret);
		return ret;
	}

	public bool CheckIsRune(UInt16 id)
	{
		return dicRuneData.ContainsKey(id);
	}

	public ItemInfo.ITEM_ORDER GetItemType(UInt16 id)
	{
		ItemInfo.ITEM_ORDER ret = ItemInfo.ITEM_ORDER.ALL;
		if( 10000 <= id && id < 30000 )
		{
			ret = ItemInfo.ITEM_ORDER.EQUIPMENT;
		}
		else if( 2000 <= id && id < 4000 )
		{
			ret = ItemInfo.ITEM_ORDER.MATERIAL;
		}
		else if( 58000 <= id && id < 58200 )
		{
			ret = ItemInfo.ITEM_ORDER.CONSUMABLE;
		}
		else if( 4650 <= id && id < 4750)
		{
			ret = ItemInfo.ITEM_ORDER.RUNE;
		}
		else if( 57000 <= id && id < 58000)
		{
			ret = ItemInfo.ITEM_ORDER.DESIGN;
		}
		else if( 60000 <= id && id < 61000)
		{
			ret = ItemInfo.ITEM_ORDER.EVENT_GOODS;
		}
		return ret;
	}
	
	public UInt16 GetItemGrade(UInt16 id)
	{
		switch(GetItemType(id))
		{
			case ItemInfo.ITEM_ORDER.MATERIAL:
				return dicMaterialItemData[id].u2Grade;
			
			case ItemInfo.ITEM_ORDER.CONSUMABLE:
				return dicConsumableItemData[id].u2Grade;
				
			//case ItemInfo.ITEM_ORDER.EQUIPMENT:
			//	return EquipmentInfoMgr.Instance.GetInfo(id).		
		}
		
		return 4571;
	}

	public string GetRuneDescription(RuneInfo runeInfo, bool multiLine)
	{
		string statInfo = "";
		
		if(runeInfo.u1Type == 5)
			statInfo = (runeInfo.u2EffVal / 1000f).ToString("0.#");
		else
			statInfo = runeInfo.u2EffVal.ToString() + "%";

		string desc = string.Format(TextManager.Instance.GetText(runeInfo.sDescription), statInfo);

		if(multiLine)
			desc.Replace("\\n", "\n");
		else
			desc.Replace("\\n", "");

		return desc;
	}

	public void Init()
	{
		dicConsumableItemData = new Dictionary<UInt16, ConsumableItemInfo>();
		DataMgr.Instance.LoadTable(this.AddConsumableItemInfo, "ConsumableItem");
//		DebugMgr.Log("dic count : " + dicConsumableItemData.Count);
		dicMaterialItemData = new Dictionary<UInt16, MaterialItemInfo>();
		DataMgr.Instance.LoadTable(this.AddMaterialItemInfo, "MaterialItem");

		dicRuneData = new Dictionary<UInt16, RuneInfo>();
		DataMgr.Instance.LoadTable(this.AddRuneInfo, "Rune");
	}
}

