using System;
using System.Collections;

public class ConsumableItem : Item
{
	public UInt16 u2Count=0;

	public ConsumableItem()
	{
	}

	public ConsumableItem(UInt16 id)
	{
		cItemInfo = ItemInfoMgr.Instance.GetConsumableItemInfo(id);
	}
	public ConsumableItem(UInt16 id, UInt16 count)
	{
		cItemInfo = ItemInfoMgr.Instance.GetConsumableItemInfo(id);
		u2Count = count;
	}
	public ConsumableItemInfo GetItemInfo()
	{
		return ItemInfoMgr.Instance.GetConsumableItemInfo(base.cItemInfo.u2ID);
	}

}
