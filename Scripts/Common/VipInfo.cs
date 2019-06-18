using UnityEngine;
using System.Collections;
using System;

public class VipInfo
{
	public const int MAX_ONCE_REWARD = 3;
	public const int MAX_DAILY_REWARD = 4;
	public const int MAX_BUY_BONUS = 2;
    public const int MAX_EVENT_GOODS = 4;

	public Byte u1Level;
	public Goods cUnlockGoods;

	public UInt16 u2AddMaxEnergy;
	public UInt16 u2AddMaxFSPoint;
	public UInt16 u2BonusFSPoint;

	public Goods[] acOnceReward;
	//public Goods[] acDailyReward;
	public Goods[] acBuyBonus;

	public Byte u1AddSkillBuyPt;
	public Byte u1AddStatusBuyPt;
	public Byte u1AddEquipBuyPt;

    public Byte u1LvUpAddSkillPt;
	public Byte u1LvUpAddStatusPt;
	public Byte u1LvUpAddEquipPt;

	public Byte u1ReduceCharTrPer;
	public Byte u1ReduceEquipTrPer;
	public Byte u1ReduceDispatchPer;

    public byte u1VIPGachaPer;
	public bool bVisitAddBonus;
	public bool bVisitBetterBonus;

    public bool bOpenBlackMarket;
	public bool bOpenEventGoodsCondition;
	public bool bOpenEventGoodsAlways;
	public bool bOpenForest;

    public bool bVIPGacha;
	public UInt16 u2VIPGachaID;

    public Byte u1AddDiscount;
    public UInt16[] u2EventGoods;

	public UInt16 m_nShowGoodInfoID;
	public UInt16 u2OpenClassID;
	public Byte Set(string[] cols)
	{
		UInt16 idx = 0;
		u1Level = Convert.ToByte(cols[idx++]);
		cUnlockGoods = new Goods(cols, ref idx);
		
		u2AddMaxEnergy = Convert.ToUInt16(cols[idx++]);
		u2AddMaxFSPoint = Convert.ToUInt16(cols[idx++]);
		u2BonusFSPoint = Convert.ToUInt16(cols[idx++]);

		acOnceReward = new Goods[MAX_ONCE_REWARD];
		for (int i=0; i<MAX_ONCE_REWARD; i++)
        {
			acOnceReward[i] = new Goods(cols, ref idx);
		}

		//acDailyReward = new Goods[MAX_DAILY_REWARD];
		//for (int i=0; i<MAX_DAILY_REWARD; i++) {
		//	acDailyReward[i] = new Goods(cols, ref idx);
		//}

		acBuyBonus = new Goods[MAX_BUY_BONUS];
		for (int i=0; i<MAX_BUY_BONUS; i++)
        {
            acBuyBonus[i] = new Goods();
			acBuyBonus[i].u1Type = Convert.ToByte(cols[idx++]);
            acBuyBonus[i].u4Count = Convert.ToUInt32(cols [idx++]);
		}
		
		u1AddSkillBuyPt = Convert.ToByte(cols[idx++]);
		u1AddStatusBuyPt = Convert.ToByte(cols[idx++]);
		u1AddEquipBuyPt = Convert.ToByte(cols[idx++]);

		u1LvUpAddSkillPt = Convert.ToByte(cols[idx++]);
		u1LvUpAddStatusPt = Convert.ToByte(cols[idx++]);
		u1LvUpAddEquipPt = Convert.ToByte(cols[idx++]);
		
		u1ReduceCharTrPer = Convert.ToByte(cols[idx++]);
		u1ReduceEquipTrPer = Convert.ToByte(cols[idx++]);
		u1ReduceDispatchPer = Convert.ToByte(cols[idx++]);
		
		bVisitAddBonus = cols[idx] == "T" || cols[idx] == "t";
        idx++;
		bVisitBetterBonus = cols[idx] == "T" || cols[idx] == "t";
        idx++;
		//bPlayTimeBonus = cols[idx] == "T" || cols[idx] == "t";
		//bPlayTimeBetterBonus = cols[idx] == "T" || cols[idx] == "t";
		
		bOpenBlackMarket = cols[idx] == "T" || cols[idx] == "t";
        idx++;
		bOpenEventGoodsCondition = cols[idx] == "T" || cols[idx] == "t";
        idx++;
		bOpenEventGoodsAlways = cols[idx] == "T" || cols[idx] == "t";
        idx++;
		bOpenForest = cols[idx] == "T" || cols[idx] == "t";
        idx++;
		
		bVIPGacha = cols[idx] == "T" || cols[idx] == "t";
        idx++;
		u2VIPGachaID = Convert.ToUInt16(cols[idx++]);
        u1VIPGachaPer = Convert.ToByte(cols[idx++]);
        u2EventGoods = new UInt16[MAX_EVENT_GOODS];
        for(int i=0; i<MAX_EVENT_GOODS; i++)
        {
            u2EventGoods[i] = Convert.ToUInt16(cols[idx++]);
        }
		
		m_nShowGoodInfoID = Convert.ToUInt16(cols[idx++]);
		u2OpenClassID = Convert.ToUInt16(cols[idx++]);
		return u1Level;
	}

    public Byte setDefault(byte vipLevel )
    {
        
        u1Level = 0;
        cUnlockGoods = new Goods(9, 0 , 0); //vip point ,  , open point

        u2AddMaxEnergy = 0;
        u2AddMaxFSPoint = 0;
        u2BonusFSPoint = 0;

        acOnceReward = new Goods[MAX_ONCE_REWARD];
        acOnceReward[0] = new Goods(1,0,0);
        acOnceReward[1] = new Goods(3,0,0);
        acOnceReward[2] = new Goods(4,0,0);


        acBuyBonus = new Goods[MAX_BUY_BONUS];
        acBuyBonus[0] = new Goods();
        acBuyBonus[0].u1Type = 1;
        acBuyBonus[0].u4Count = 0;
        acBuyBonus[1] = new Goods();
        acBuyBonus[1].u1Type = 2;
        acBuyBonus[1].u4Count = 0;

        u1AddSkillBuyPt = 0;
        u1AddStatusBuyPt = 0;
        u1AddEquipBuyPt = 0;

        u1LvUpAddSkillPt = 0;
        u1LvUpAddStatusPt = 0;
        u1LvUpAddEquipPt = 0;

        u1ReduceCharTrPer = 0;
        u1ReduceEquipTrPer = 0;
        u1ReduceDispatchPer = 0;

        bVisitAddBonus = false;
        bVisitBetterBonus = false;
        //bPlayTimeBonus = cols[idx] == "T" || cols[idx] == "t";
        //bPlayTimeBetterBonus = cols[idx] == "T" || cols[idx] == "t";

        bOpenBlackMarket = false;
        bOpenEventGoodsCondition = false;
        bOpenEventGoodsAlways = false;
        bOpenForest = false;

        bVIPGacha = false;
        u2VIPGachaID = 0;
        u1VIPGachaPer = 0;
        u2EventGoods = new UInt16[MAX_EVENT_GOODS];
        for(int i=0; i<MAX_EVENT_GOODS; i++)
        {
            u2EventGoods[i] = 0;
        }
		u2OpenClassID = 0;

        return u1Level;
    }
}
