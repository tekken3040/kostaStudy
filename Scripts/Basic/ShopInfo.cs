using UnityEngine;
using System.Collections;
using System;

//일반상점, 장비상점, 암시장 정보
public class ShopInfo{

	public UInt16 u2ID;
	public Byte u1Type;

	public Goods cRenewGoods;

	public UInt16 u2AddValue;
	public UInt16 u2MaxValue;
	public Byte[] u1ArrResetTime;
    public UInt32 u4KeepTime;
	
	public Goods cPassGoods;

	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment

		u1Type = Convert.ToByte(cols[idx++]);
		cRenewGoods = new Goods(cols, ref idx);
		u2AddValue = Convert.ToUInt16(cols[idx++]);
		u2MaxValue = Convert.ToUInt16(cols[idx++]);
		u1ArrResetTime = new Byte[6];
		u1ArrResetTime[0] = Convert.ToByte(cols[idx++]);
		u1ArrResetTime[1] = Convert.ToByte(cols[idx++]);
		u1ArrResetTime[2] = Convert.ToByte(cols[idx++]);
		u1ArrResetTime[3] = Convert.ToByte(cols[idx++]);
		u1ArrResetTime[4] = Convert.ToByte(cols[idx++]);
		u1ArrResetTime[5] = Convert.ToByte(cols[idx++]);
		idx++;
		u4KeepTime = Convert.ToUInt32(cols[idx++]);
		cPassGoods = new Goods(cols, ref idx);
		idx++;
		idx++;
		if(u1Type == 2)
			ShopInfoMgr.Instance.maxEquipRegist = Convert.ToInt16(cols[idx++]);

		return u2ID;
	}
}
