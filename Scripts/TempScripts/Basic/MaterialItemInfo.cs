using System;
using System.Collections;
using System.Collections.Generic;
public class MaterialItemInfo : ItemInfo
{
	public struct Location
	{
		public MENU eMenuID;
		public UInt16 u2SubMenuID;
		public object[] aoMenuParam;
		public const int MAX_PARAM_COUNT = 3;
		public void Set(string[] cols, ref int idx)
		{
			eMenuID = (MENU) Enum.Parse(typeof(MENU), cols[idx++]);
			u2SubMenuID = Convert.ToUInt16 (cols [idx++]);
			aoMenuParam = new object[MAX_PARAM_COUNT];
			for (int i = 0; i < MAX_PARAM_COUNT; i++) {
				aoMenuParam [i] = Convert.ToUInt16 (cols [idx++]);
			}
		}
	}
	public UInt16 u2Grade;
	public Goods cSellGoods;
	public UInt16[] au2SellShopID;
//	public ShopInfo[] acShop;
	const int MAX_SHOP_COUNT = 2;
	public Byte u1ShopCountMin;
	public Byte u1ShopCountMax;
	public Byte u1PickProbability;
	public bool[] au1Gacha;
	const int MAX_GACHA_COUNT = 4;
	const int MAX_LOCATION_COUNT = 4;
	public Byte u1GetLocationCount;
	public Location[] acLocation;
	public UInt16 u2IconID;

	public UInt16 SetInfo(string[] cols)
	{
		int idx=0;
		base.u2ID = Convert.ToUInt16(cols[idx++]);
		idx++; // comment
		base.sName = cols[idx++];
		sDescription = cols[idx++];
		u2Grade = Convert.ToUInt16(cols[idx++]);
		idx++;
		idx++;
		idx++;
		cSellGoods = new Goods(cols, ref idx);
		au2SellShopID = new UInt16[MAX_SHOP_COUNT];
//		acShop = new ShopInfo[MAX_SHOP_COUNT];
		for(int i=0; i<MAX_SHOP_COUNT; i++)
		{
			au2SellShopID[i] = Convert.ToUInt16(Convert.ToUInt16(cols[idx++]));
//			acShop[i] = ShopInfoMgr.Instance.GetInfo(Convert.ToUInt16(cols[idx++]));
		}
		u1ShopCountMin = Convert.ToByte(cols[idx++]);
		u1ShopCountMax = Convert.ToByte(cols[idx++]);
		u1PickProbability = Convert.ToByte(cols[idx++]);
		au1Gacha = new bool[MAX_GACHA_COUNT];
		for(int i=0; i<MAX_GACHA_COUNT; i++)
		{
			if(cols[idx++] == "0")
			{
				au1Gacha[i] = false;
			}
			else
			{
				au1Gacha[i] = true;
			}
		}
		u1GetLocationCount = Convert.ToByte(cols[idx++]);
		acLocation = new Location[u1GetLocationCount];
		for(int i=0; i<MAX_LOCATION_COUNT; i++)
		{
			if (i < u1GetLocationCount) {
				acLocation [i].Set (cols, ref idx);
			} else {
				idx++;
				idx++;
				for (int j = 0; j < Location.MAX_PARAM_COUNT; j++) idx++;
			}
		}
		u2IconID = Convert.ToUInt16(cols[idx++]);
		base.eOrder = ITEM_ORDER.MATERIAL;
		return base.u2ID;
	}
	
}
