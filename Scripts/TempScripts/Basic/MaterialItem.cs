using System;
using System.Collections;

public class MaterialItem : Item
{
	public UInt16 u2Count;
	public MaterialItem()
	{

	}
	public MaterialItem(UInt16 id)
	{
		cItemInfo = ItemInfoMgr.Instance.GetMaterialItemInfo(id);
	}
	public MaterialItem(UInt16 id, UInt16 count)
	{
		cItemInfo = ItemInfoMgr.Instance.GetMaterialItemInfo(id);
		u2Count = count;
	}
	public MaterialItemInfo GetMaterialItemInfo()
	{
		return ItemInfoMgr.Instance.GetMaterialItemInfo(base.cItemInfo.u2ID);
	}
}