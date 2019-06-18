using UnityEngine;
using System.Collections;
using System;

//재화 상점 데이터
public class ShopGoodInfo {

	public UInt16 u2ID;
    public string title;
    public string gachaCount;
    public string itemLevel;
    public string itemGrade;
    public Byte u1Type;
    public UInt16 u2GroupIndex;
    public Byte u1BuyOverlap;
    public UInt16 u2BuyRestriction;
    public bool bBuyAll;
    public Goods cBuyGoods;
	public Byte u1Discount;
	public Goods cShopItem;
	public Byte u1BonusRate;
	public Byte u1Repeat;
    public UInt16 u2ProbabilityTableID;
    public Goods cVipGoods;
    public string imagePath;
    public Byte u1Grade;
    public string iOSPrice;
    public string OnestoreCord;
	public bool bSell;
	
	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment
        title = cols[idx++];
        gachaCount = cols[idx++];
        itemLevel = cols[idx++];
        itemGrade = cols[idx++];
		idx++;
        u1Type = Convert.ToByte(cols[idx++]);
        u2GroupIndex = Convert.ToUInt16(cols[idx++]);
        u1BuyOverlap = Convert.ToByte(cols[idx++]);
        u2BuyRestriction = Convert.ToUInt16(cols[idx++]);
        bBuyAll = cols[idx] == "T" || cols[idx] == "t";
        idx++;
        cBuyGoods = new Goods(cols, ref idx);
		u1Discount = Convert.ToByte(cols[idx++]);
		cShopItem = new Goods(cols, ref idx);
		u1BonusRate = Convert.ToByte(cols[idx++]);
		u1Repeat = Convert.ToByte(cols[idx++]);
        idx++;
        idx++;
        idx++;
        idx++;
        u2ProbabilityTableID = Convert.ToUInt16(cols[idx++]);
        cVipGoods = new Goods(cols, ref idx);
        imagePath = cols[idx++];
        u1Grade = Convert.ToByte(cols[idx++]);
        iOSPrice = cols[idx++];
		idx++;
        OnestoreCord = cols[idx++];
        bSell = cols[idx] == "T" || cols[idx] == "t";
		idx++;

		return u2ID;
	}

	public UInt16 GetBuyBonus(){
		UInt16 result = (UInt16)(cShopItem.u4Count * u1BonusRate / 100);
		return result;
	}
}
