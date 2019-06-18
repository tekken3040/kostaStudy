using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
public class Inventory {
	public Dictionary<UInt16, Item> dicInventory;
	public List<Item> lstSortedInventory; // slot_num, item
	public List<EquipmentItem> lstSortedEquipment;
    public List<Item> lstSortedAllInventory; // slot_num, allitem
	public Dictionary<UInt16, UInt16> dicItemKey; // item id(not include equipment), slot_num
	
    //상점에 등록한 장비 슬롯 리스트
    public Dictionary<UInt16, EquipmentItem> lstInShop;
    public bool initInventory = false;

	public Inventory()
	{
		dicInventory = new Dictionary<UInt16, Item>();
		// lstSortedInventory = new List<Item>();
		dicItemKey = new Dictionary<UInt16, UInt16>();
	}

	public void Sort()
	{
		lstSortedInventory = dicInventory.Values.ToList<Item>();
        
        for(int i=0; i<lstSortedInventory.Count; i++)
        {
            if(lstSortedInventory[i].cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT)
			{
				EquipmentItem item = (EquipmentItem)lstSortedInventory[i];
				//EquipmentInfo info = item.GetEquipmentInfo();                    
                item.GetComponent<StatusComponent>().CountingStatPointEquip(item.cLevel.u2Level);
			}
        }
        
		lstSortedInventory.Sort();
	}

    public void AllInvenSort()
    {
        //List<Item> itemList = dicInventory.Values.ToList<Item>().FindAll( (x) => x.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT);
        lstSortedAllInventory = new List<Item>();
        List<Item> itemList = dicInventory.Values.ToList<Item>().FindAll( (x) => x.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.MATERIAL);
        itemList.Sort
        (
            delegate(Item x, Item y) 
            {
                int compare = 0;
                
                compare = ((MaterialItem)y).GetMaterialItemInfo().u2Grade.CompareTo(((MaterialItem)x).GetMaterialItemInfo().u2Grade);

                return compare;
            }
        );
        for(int i=0; i<itemList.Count; i++)
			lstSortedAllInventory.Add((MaterialItem)itemList[i]);

        itemList.Clear();
        itemList = dicInventory.Values.ToList<Item>().FindAll( (x) => x.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.CONSUMABLE);
        itemList.Sort
        (
            delegate(Item x, Item y) 
            {
                int compare = 0;

                compare = ((ConsumableItem)y).GetItemInfo().u2Grade.CompareTo(((ConsumableItem)x).GetItemInfo().u2Grade);

                return compare;
            }
        );
        for(int i=0; i<itemList.Count; i++)
			lstSortedAllInventory.Add((ConsumableItem)itemList[i]);

        itemList.Clear();
        itemList = dicInventory.Values.ToList<Item>().FindAll( (x) => x.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.RUNE);
        
        for(int i=0; i<itemList.Count; i++)
			lstSortedAllInventory.Add((EquipmentItem)itemList[i]);

        EquipSort();
        //Sort();
        
        //for(int i=0; i<lstSortedInventory.Count; i++)
        //{
        //    if(lstSortedInventory[i].cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT)
		//	{
		//		lstSortedInventory.Remove(lstSortedInventory[i]);
		//	}
        //}
		
        //for(int i=0; i<lstSortedInventory.Count; i++)
        //{
        //    if(!lstSortedAllInventory.Contains(lstSortedInventory[i]))
        //        lstSortedAllInventory.Add(lstSortedInventory[i]);
        //}


        for(int i=0; i<lstSortedEquipment.Count; i++)
            lstSortedAllInventory.Add((Item)lstSortedEquipment[i]);
    }

	public void EquipSort()
	{		
		List<Item> itemList = dicInventory.Values.ToList<Item>().FindAll( (x) => x.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT);
		lstSortedEquipment = new List<EquipmentItem>();
		
		for(int i=0; i<itemList.Count; i++)
		{
			lstSortedEquipment.Add((EquipmentItem)itemList[i]);
		}
		
		lstSortedEquipment.Sort
        (
            delegate(EquipmentItem x, EquipmentItem y) 
            {
		    	
		    	//전투력 비교
		    	//int compare = y.u2Power.CompareTo(x.u2Power);
		    	int compare = 0;
		    	//착용 여부 비교
		    	//if(compare == 0)
		    	//{
		    	//	if(x.attached.hero == null && y.attached.hero != null)
		    	//		compare = 1;
		    	//	else if(x.attached.hero != null && y.attached.hero == null)
		    	//		compare = -1;
		    	//	else
		    	//		compare = 0;
		    	//}
                
                //티어
                if(compare == 0)
                    compare = y.u1SmithingLevel.CompareTo(x.u1SmithingLevel);
                //레벨
                if(compare == 0)
                    compare = y.cLevel.u2Level.CompareTo(x.cLevel.u2Level);
                //장비 종류
                if(compare == 0)
                {
		    		compare = y.cItemInfo.eOrder.CompareTo(x.cItemInfo.eOrder);
                }
                
                return compare;
            }
        
        );
	}

	public List<EquipmentItem> GetNewEquip()
	{
		List<EquipmentItem> ret = new List<EquipmentItem>();
		foreach(EquipmentItem equipItem in lstSortedEquipment)
		{
			if(equipItem.isNew)
			{
				ret.Add(equipItem);
			}
		}
		return ret;
	}

	public UInt16 GetKey(Item item)
	{
		int idx = lstSortedInventory.IndexOf(item);
		return lstSortedInventory[idx].u2SlotNum;
	}

//	public ushort AddEquipment(UInt16 slotNum, UInt16 itemID, Byte[] slots, UInt32[] stats, string itemName = "", string createrName = "", UInt16 modelID = 0, Byte smithingLevel = 1)
//	{
//		if(modelID == 1) DebugMgr.LogError("? modelID : " + itemID);
//		if(itemID < 10000 || itemID >= 30000)
//		{
//			DebugMgr.Log("장비 ID가 아닙니다.");
//			return 0;
//		}
//		
//		if (slotNum == 0 || dicInventory.ContainsKey(slotNum))
//		{
//			slotNum = FindEmptySlot();
//			if (slotNum == 0) return 0;
//		}
//		
//		EquipmentItem item = new EquipmentItem(itemID);
//		item.u2SlotNum = slotNum;
//		item.registedInShop = 0;
//		item.itemName = itemName;
//		item.createrName = createrName;
//		item.u2ModelID = modelID;
//		item.GetComponent<StatusComponent>().LoadStatusEquipment(stats, item.GetEquipmentInfo().acStatAddInfo, 0);
//		
//		item.GetComponent<LevelComponent>().Set(1, 0);
//		item.skillSlots = slots;
//
//		item.u1SmithingLevel = smithingLevel;
//		dicInventory.Add(item.u2SlotNum, item);
//		return slotNum;
//	}

	public ushort AddEquipment(
		UInt16 slotNum,
		Byte inShop,
		UInt16 itemID,
		UInt16 u2Level,
		//UInt32 u4Exp,
		UInt64 u8Exp,
		Byte[] skillSlots,
		UInt32[] stats,
		Byte u1ResetCount,
		string itemName = "",
		string createrName = "",
		UInt16 modelID = 0,
		bool isNew = true,
		Byte smithingLevel = 1,
        UInt16 u2UnsetStatPoint = 0,
        UInt16 u2StatPointExp = 0,
        Byte u1Completeness = 1
	)
	{
		if(itemID < 10000 || itemID >= 30000)
		{
			DebugMgr.Log("장비 ID가 아닙니다.");
			return 0;
		}

		if (slotNum == 0 || dicInventory.ContainsKey(slotNum))
		{
			slotNum = FindEmptySlot();
			if (slotNum == 0) return 0;
		}

		EquipmentItem item = new EquipmentItem(itemID);
		item.u2SlotNum = slotNum;
		item.registedInShop = inShop;
		item.itemName = itemName;
		item.createrName = createrName;
		item.u2ModelID = modelID;
//		item.GetComponent<StatusComponent>().LoadStatusEquipment(stats, item.GetEquipmentInfo().acStatAddInfo, 0);
		item.GetComponent<StatusComponent>().LoadStatusEquipment(stats,
		                                                item.GetEquipmentInfo().acStatAddInfo,
		                                                u1ResetCount);

		item.skillSlots = skillSlots;
        item.isNew = isNew;
		item.u1SmithingLevel = smithingLevel;
        item.u2UnsetStatPoint = u2UnsetStatPoint;
        item.u2StatPointExp = u2StatPointExp;
        item.u1Completeness = u1Completeness;
        item.GetComponent<LevelComponent>().Set(u2Level, u8Exp);
		DebugMgr.Log(itemID + "/" + u2Level);

//        item.GetComponent<StatusComponent>().LoadStatus(stats, 1, 2, u1ResetCount);
		dicInventory.Add(item.u2SlotNum, item);
        item.GetComponent<StatusComponent>().STATPOINT_EXP = item.u2StatPointExp;
        item.GetComponent<StatusComponent>().UNSET_STATPOINT = item.u2UnsetStatPoint;
		DebugMgr.Log("AddEquip SmithLevel : " + item.u1SmithingLevel);
		// DebugMgr.Log("Add Equipment Item : " + dicInventory[item.u2SlotNum].cItemInfo.u2ID);
		return slotNum;
	}

	public UInt16 ContainID(UInt16 itemID)
	{
		UInt16 slotNum;
		if (dicItemKey.TryGetValue(itemID, out slotNum))
		{
			return slotNum;
		}
		return 0;
	}

	public bool IsInvenFull(){
		if (dicInventory.Count >= LegionInfoMgr.Instance.GetMaxInvenSize())
			return true;

		return false;
	}

	public UInt16 FindEmptySlot()
	{
		for (UInt16 i = 1; i <= LegionInfoMgr.Instance.GetMaxInvenSize()+1; i++)
		{
			if (!dicInventory.ContainsKey(i))
			{
                //상점에 등록한 슬롯은 제외
                if(lstInShop.ContainsKey(i))
                {
                    continue;
                }
                
				return i;
			}
		}
		return 0;
	}

	//public bool Add(UInt16 itemID, Byte u2Level, Byte u1Star, UInt16 count)
	//{
	//	UInt16 divID = (UInt16)(itemID / 1000);
	//	if(divID == 2 || divID == 3)
	//	{
	//		return AddItem(0, itemID, count);
	//	}
	//	else
	//	{
	//		while (count-- > 0)
	//		{
	//			if (!AddEquipment(0, itemID, u2Level, 0, u1Star)) return false;
	//		}
	//		return true;
	//	}
	//}

	public ushort AddItem(UInt16 slotNum, UInt16 itemID, UInt32 count)
	{
		DebugMgr.Log(itemID);

		if (slotNum == 0)
		{
			slotNum = ContainID(itemID);
			if (slotNum == 0)
			{
				slotNum = FindEmptySlot();
				if (slotNum == 0) return 0;
			}
		}
        
        DebugMgr.Log(slotNum);

		UInt16 divID = (UInt16)(itemID / 1000);
		if(divID == 2 || divID == 3) // Material
		{
			if (dicInventory.ContainsKey(slotNum))
			{
				MaterialItem item = (MaterialItem)dicInventory[slotNum];
				if (item.cItemInfo.u2ID != itemID) return 0;
				item.u2Count += (ushort)count;
                if(item.u2Count > 999)
                    item.u2Count = 999;
//				DebugMgr.Log(string.Format("Add Material Item : {0},  Value : {1} ", dicInventory[item.u2SlotNum].cItemInfo.u2ID, item.u2Count));
			}
			else
			{
				MaterialItem item = new MaterialItem(itemID, (ushort)count);
				item.u2SlotNum = slotNum;
				dicInventory.Add(item.u2SlotNum, item);
				if (!dicItemKey.ContainsKey(itemID)) dicItemKey.Add(itemID, slotNum);
                if(initInventory)
                    item.isNew = true; 
//				DebugMgr.Log(item.u2SlotNum);
//				DebugMgr.Log(dicInventory[item.u2SlotNum]);

//				DebugMgr.Log("Add Material Item : " + dicInventory[item.u2SlotNum].cItemInfo.u2ID);
			}
			if(initInventory){
				Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.CollectItem, itemID, (Byte)ItemInfoMgr.Instance.GetItemGrade(itemID), 0, 0, count);
			}
		}
		else if(divID == 58) // Consumable
		{
			if (dicInventory.ContainsKey(slotNum))
			{
//				DebugMgr.Log(slotNum);
//				DebugMgr.Log(dicInventory[slotNum]);

				ConsumableItem item = (ConsumableItem)dicInventory[slotNum];
				if (item.cItemInfo.u2ID != itemID) return 0;
				item.u2Count += (ushort)count;
                if(item.u2Count > 999)
                    item.u2Count = 999;
//				DebugMgr.Log(string.Format("Add Consumable Item : {0},  Value : {1} ", dicInventory[item.u2SlotNum].cItemInfo.u2ID, item.u2Count));
			}
			else
			{
				ConsumableItem item = new ConsumableItem(itemID, (ushort)count);
				item.u2SlotNum = slotNum;
				dicInventory.Add(item.u2SlotNum, item);
				if (!dicItemKey.ContainsKey(itemID)) dicItemKey.Add(itemID, slotNum);
                if(initInventory)
                    item.isNew = true;
//				DebugMgr.Log("Add Consumable Item : " + itemID);
//				DebugMgr.Log("Add Consumable Item : " + dicInventory[item.u2SlotNum].cItemInfo.u2ID);
                
			}
		}
		return slotNum;
	}
    
	public void RemoveEquip(UInt16 slotNum)
	{
        dicItemKey.Remove(dicInventory[slotNum].cItemInfo.u2ID);
		dicInventory.Remove(slotNum);
	}

	public void RemoveItem(UInt16 itemID)
	{
		dicInventory.Remove(dicItemKey[itemID]);
		dicItemKey.Remove(itemID);
	}

	public Dictionary<Byte, Byte> GetEquipSkillPoint(Hero tHero){
		Dictionary<Byte, Byte> tempDic = new Dictionary<Byte, Byte>();

		for (int i=0; i<tHero.acEquips.Length; i++) {
			if (tHero.acEquips [i] != null && tHero.acEquips [i].skillSlots != null) {
				for (int j = 0; j < tHero.acEquips [i].skillSlots.Length; j++) {
					if (tHero.acEquips [i].skillSlots [j] > 0) {
						if (tempDic.ContainsKey (tHero.acEquips [i].skillSlots [j])) {
							tempDic [tHero.acEquips [i].skillSlots [j]] += (Byte)tHero.acEquips [i].GetPoints () [j];
						} else {
							tempDic.Add (tHero.acEquips [i].skillSlots [j], (Byte)tHero.acEquips [i].GetPoints () [j]);
						}
					}
				}
			}
		}

		return tempDic;
	}
}

public class Runeventory {
	Dictionary<UInt16, RuneItem> dicRuneventory;
	List<RuneItem> lstSortedRuneventory = null;

	public Runeventory()
	{
		dicRuneventory = new Dictionary<UInt16, RuneItem>();
	}

	public void Clear(){
		dicRuneventory.Clear ();
		lstSortedRuneventory.Clear ();
	}

	public void AddItem(UInt16 u2ID, UInt32 u4Count){
		if (ItemInfoMgr.Instance.CheckIsRune (u2ID)) {
			if(!dicRuneventory.ContainsKey(u2ID)){
				RuneItem temp = new RuneItem (u2ID, u4Count);
				dicRuneventory.Add(u2ID, temp);
			}else{
				dicRuneventory[u2ID].AddCount(u4Count);
			}
		}
	}

	public void SubItem(UInt16 u2ID, UInt32 u4Count)
	{
		if (ItemInfoMgr.Instance.CheckIsRune (u2ID)) {
			if(!dicRuneventory.ContainsKey(u2ID)){
				DebugMgr.LogError("None");
			}
			else
			{
				if(dicRuneventory[u2ID].GetCount(true) < u4Count)
				{
					DebugMgr.LogError("NotEnough");
				}
				else
				{
					dicRuneventory[u2ID].SubCount(u4Count);
					if(dicRuneventory[u2ID].GetCount(true) == 0)
						dicRuneventory.Remove(u2ID);
				}
			}
		}
	}

	public RuneItem GetMyRune(UInt16 u2ID){
		if (dicRuneventory.ContainsKey (u2ID)) {
			return dicRuneventory[u2ID];
		}
		return null;
	}

	public List<RuneItem> GetRuneList(){
		lstSortedRuneventory = dicRuneventory.Values.ToList<RuneItem>();

		return lstSortedRuneventory;
	}

	public void SortByID(){
		lstSortedRuneventory = dicRuneventory.Values.ToList<RuneItem>();
		lstSortedRuneventory.Sort( delegate(RuneItem x, RuneItem y) { return y.cInfo.u2ID.CompareTo(x.cInfo.u2ID); } );
	}

	public void SortByLevel(){
		lstSortedRuneventory = dicRuneventory.Values.ToList<RuneItem>();
		lstSortedRuneventory.Sort( delegate(RuneItem x, RuneItem y) { return y.cInfo.u2Level.CompareTo(x.cInfo.u2Level); } );
	}
}
