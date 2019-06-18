using UnityEngine;
using System.Collections;
using System;

public class RuneInfo : ItemInfo {
	
	public Byte u1Type;
	public Byte u1IncreaseType;
	public UInt16 u2Level;
	public Byte u1Target;
	public UInt16 u2EffVal;
	public UInt16 u2MaxEffVal;
	
	public Goods cSellGoods;
	public Goods cCreateGoods;
	public Byte u1UpgradeCost;
	
	//Server
	//	public UInt16[] u2ShopID;
	//	public Byte u1ProductMin;
	//	public Byte u1ProductMax;
	//	public Byte u1ProductPer;
	
	public UInt16 SetInfo(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment
		
		u1Type = Convert.ToByte(cols[idx++]);
		u1IncreaseType = Convert.ToByte(cols[idx++]);
		u2Level = Convert.ToUInt16(cols[idx++]);
		sName = cols[idx++];
		sDescription = cols[idx++];
		u1Target = Convert.ToByte(cols[idx++]);
		u2EffVal = Convert.ToUInt16(cols[idx++]);
		u2MaxEffVal = Convert.ToUInt16(cols[idx++]);
		
		idx++;
		idx++;
		idx++;

		cSellGoods = new Goods(cols, ref idx);
		
		idx++;
		idx++;
		idx++;
		idx++;
		idx++;
		
		cCreateGoods = new Goods(cols, ref idx);
		u1UpgradeCost = Convert.ToByte(cols[idx++]);
		
		base.eOrder = ITEM_ORDER.RUNE;
		return u2ID;
	}
}
