using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class ConsumableItemInfo : ItemInfo
{
	//public string sAbillityDescription;
	public UInt16 u2Grade;
	public Goods cSellGoods;
	bool bClearStage;
	public UInt32 u4Exp;
	public UInt16 SetInfo(string[] cols)
	{
		int idx=0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++; // comment
		sName = cols[idx++];
		//sAbillityDescription = cols[idx++];
		sDescription = cols[idx++];
		u2Grade = Convert.ToUInt16(cols[idx++]);
		idx++;
		idx++;
		idx++;

		cSellGoods = new Goods(cols, ref idx);
		u4Exp = Convert.ToUInt32(cols[idx++]);
//
//		if(cols[idx] == "0")
//			bClearStage = false;
//		else
//			bClearStage = true;
//
//		idx++;


		base.eOrder = ITEM_ORDER.CONSUMABLE;
		return u2ID;
	}

}
