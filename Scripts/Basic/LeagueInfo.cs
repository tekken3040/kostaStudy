using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class LeagueInfo
{
    public enum LeagueResultType
    {
        WIN = 0,
        DRAW = 1,
        LOSE = 2,
        MAX
    }
	public const int MAX_GRADE_COUNT = 7;
	public const int MAX_REWARD_CNT = 5;
 
    public UInt16 u2LeagueID;
    public Byte u1RunningWeek;
    public Byte u1HighRankerView;
    public Byte u1LowRankerView;
    public Byte u1LegendRankerView;
    public Byte u1RematchTime;
    public UInt16 u2FieldID;
    public Byte u1DayOrNight;
    public Byte u1SkyBox;
    public UInt16 u2PromotionPoint;
    public UInt16 u2DemotionPoint;
    public UInt16 u2ResetPoint;
	public float fResetRate;

    public Goods cWinReward;
	public Goods cPlayGoods;

    public int u2RevengeWinPoint;
	public int u2RevengeDrawPoint;
    public int u2RevengeLosePoint;

    public Byte u1PromotionRewardType;
    public UInt16 u2PromotionRewardID;
	public UInt16 u2PromotionRewardCount;

    public Goods[] arrResultOidnPoint;        // 결과 오딘 포인트

    public Byte[][] u1RewardType;
    public UInt16[][] u2RewardID;
	public UInt16[][] u2RewardCount;

    public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2LeagueID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment

		u1RunningWeek = Convert.ToByte(cols[idx++]);
        u1HighRankerView = Convert.ToByte(cols[idx++]);
        u1LowRankerView = Convert.ToByte(cols[idx++]);
        u1LegendRankerView = Convert.ToByte(cols[idx++]);
        u1RematchTime = Convert.ToByte(cols[idx++]);
        u2FieldID = Convert.ToUInt16(cols[idx++]);
        u1DayOrNight = Convert.ToByte(cols[idx++]);
        u1SkyBox = Convert.ToByte(cols[idx++]);
        u2PromotionPoint = Convert.ToUInt16(cols[idx++]);
        u2DemotionPoint = Convert.ToUInt16(cols[idx++]);
        u2ResetPoint = Convert.ToUInt16(cols[idx++]);
		fResetRate = (float)Convert.ToDouble(cols[idx++]);

        cWinReward = new Goods(cols, ref idx);
	    cPlayGoods = new Goods(cols, ref idx);

        u2RevengeWinPoint = Convert.ToInt16(cols[idx++]);
		u2RevengeDrawPoint = Convert.ToInt16(cols[idx++]);
        u2RevengeLosePoint = Convert.ToInt16(cols[idx++]);

        u1PromotionRewardType = Convert.ToByte(cols[idx++]);
        u2PromotionRewardID = Convert.ToUInt16(cols[idx++]);
        u2PromotionRewardCount = Convert.ToUInt16(cols[idx++]);

        //#ODIN [리그 결과 오딘 포인트 셋팅]
        arrResultOidnPoint = new Goods[(int)LeagueResultType.MAX];
        for(int i = 0; i < arrResultOidnPoint.Length; ++i)
        {
            //arrResultOidnPoint[i] = new Goods(9, 0, (UInt32)(20 - (i * 5)));
            arrResultOidnPoint[i] = new Goods(cols, ref idx);
        }

		u1RewardType = new Byte[MAX_GRADE_COUNT][];
		u2RewardID = new UInt16[MAX_GRADE_COUNT][];
		u2RewardCount= new UInt16[MAX_GRADE_COUNT][];
		for(int a=0; a<MAX_GRADE_COUNT; a++)
		{
			u1RewardType[a] = new Byte[MAX_REWARD_CNT];
			u2RewardID[a] = new UInt16[MAX_REWARD_CNT];
			u2RewardCount[a] = new UInt16[MAX_REWARD_CNT];
	        for(int i=0; i<MAX_REWARD_CNT; i++)
	        {
				u1RewardType[a][i] = Convert.ToByte(cols[idx++]);
				u2RewardID[a][i] = Convert.ToUInt16(cols[idx++]);
				u2RewardCount[a][i] = Convert.ToUInt16(cols[idx++]);
	        }
		}

		return u2LeagueID;
	}

	public FieldInfo getField()
	{
		return StageInfoMgr.Instance.GetFieldInfo(u2FieldID);
	}

	public bool IsRewardMaterial(UInt16 _id)
	{
		for(int a=0; a<MAX_GRADE_COUNT; a++)
		{
			for(int i=0; i<MAX_REWARD_CNT; i++)
			{
				if (u2RewardID [a] [i] == _id)
					return true;
			}
		}

		return false;
	}
}
