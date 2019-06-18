using UnityEngine;
using System;
using System.Text;
using System.Collections;

public enum GoodsType {
	NONE = 0,
	GOLD = 1,
	CASH = 2,
	KEY = 3,
	LEAGUE_KEY = 4,
	FRIENDSHIP_POINT = 5,
	LEVEL = 6,
    PURCHASE = 7,
	VIP_LEVEL = 8,
    ODIN_POINT = 9,
	EQUIP = 10,
	MATERIAL = 11,
	CONSUME = 12,
	SCROLL = 13,
	EQUIP_COUPON = 14,
	MATERIAL_COUPON = 15,
	TRAINING_ROOM = 16,
	EQUIP_TRAINING = 17,
    STAGE_CLEAR = 18,
	SCROLL_SET = 19,
	CHARACTER_PACKAGE = 20,
	CHARACTER_AVAILABLE= 21,
    EQUIP_GOODS = 22,    // 장비 상품
    EVENT_ITEM = 23,
    //뒤로추가하지말것
    RANDOM_REWARD = 255
}

public class Goods {
	public Byte u1Type;
	public UInt16 u2ID;
	public UInt32 u4Count;

    public Goods()
    {
    }

	public bool isEquip(){
		if(u1Type == (Byte)GoodsType.EQUIP) return true;

		return false;
	}

	public Goods(string[] cols, ref UInt16 idx)
	{
		u1Type = Convert.ToByte(cols[idx++]);
		u2ID = Convert.ToUInt16(cols[idx++]);
		u4Count = Convert.ToUInt32 (cols [idx++]);
	}

	public Goods(string[] cols, ref int idx)
	{
		u1Type = Convert.ToByte(cols[idx++]);
		u2ID = Convert.ToUInt16(cols[idx++]);
		u4Count = Convert.ToUInt32 (cols [idx++]);
	}

	public Goods(Byte type, UInt16 id, UInt32 cnt)
	{
		u1Type = type;
		u2ID = id;
		u4Count = cnt;
	}

	public bool IsCoupon(){
		if (u1Type == (byte)GoodsType.EQUIP_COUPON || u1Type == (byte)GoodsType.MATERIAL_COUPON) {
			return true;
		}

		return false;
	}

	public string GetGoodsString(){
        StringBuilder tempString = new StringBuilder();
		if (u1Type != 0) {
			if (u1Type == (byte)GoodsType.CONSUME) {
                tempString.Append(TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (u2ID).sName)).Append(" ").Append(u4Count);
			} else {
                tempString.Append(Legion.Instance.GetConsumeString(u1Type)).Append(" ").Append(u4Count);
			}
		}

		return tempString.ToString();
	}
	
//	public UInt16 Set(string[] cols, ref UInt16 idx)
//	{
//		u1Type = Convert.ToByte(cols[idx++]);
//		u2ID = Convert.ToUInt16(cols[idx++]);
//		u4Count = Convert.ToUInt32 (cols [idx++]);
//		
//		return u2ID;
//	}
//
//	public UInt16 Set(string[] cols, ref int idx)
//	{
//		u1Type = Convert.ToByte(cols[idx++]);
//		u2ID = Convert.ToUInt16(cols[idx++]);
//		u4Count = Convert.ToUInt32 (cols [idx++]);
//		
//		return u2ID;
//	}
//
//	public void Set(Byte type, UInt16 id, UInt32 cnt)
//	{
//		u1Type = type;
//		u2ID = id;
//		u4Count = cnt;
//	}
}
