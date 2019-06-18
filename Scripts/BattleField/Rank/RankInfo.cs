using UnityEngine;
using System;
using System.Collections;

public class RankInfo
{
    public Byte u1RankType;
    public bool bOpen;
    public Byte u1RegistType;
    public UInt16 u2RegistID;
    public UInt16 u2RegistCount;
    public Byte u1ApeearType;
    public UInt16 u2AppearID;
    public UInt16 u2AppearCount;
    public UInt16 u2RegistMemberCountForOpen;
    public UInt16[] u2RankBase = new UInt16[8];
    public Byte[] u1RewardType1 = new Byte[8];
    public UInt16[] u2RewardID1 = new UInt16[8];
    public UInt16[] u2RewardCount1 = new UInt16[8];
    public Byte[] u1RewardType2 = new Byte[8];
    public UInt16[] u2RewardID2 = new UInt16[8];
    public UInt16[] u2RewardCount2 = new UInt16[8];

    public UInt16 Set(string[] cols)
	{
        UInt16 idx = 0;
        u1RankType = Convert.ToByte(cols[idx++]);
        bOpen = cols[idx] == "T" || cols[idx] == "t";
        idx++;
        u1RegistType = Convert.ToByte(cols[idx++]);
        u2RegistID = Convert.ToUInt16(cols[idx++]);
        u2RegistCount = Convert.ToUInt16(cols[idx++]);
        u1ApeearType = Convert.ToByte(cols[idx++]);
        u2AppearID = Convert.ToUInt16(cols[idx++]);
        u2AppearCount = Convert.ToUInt16(cols[idx++]);
        u2RegistMemberCountForOpen = Convert.ToUInt16(cols[idx++]);
        for(int i=0; i<u2RankBase.Length; i++)
        {
            u2RankBase[i] = Convert.ToUInt16(cols[idx++]);
            u1RewardType1[i] = Convert.ToByte(cols[idx++]);
            u2RewardID1[i] = Convert.ToUInt16(cols[idx++]);
            u2RewardCount1[i] = Convert.ToUInt16(cols[idx++]);
            u1RewardType2[i] = Convert.ToByte(cols[idx++]);
            u2RewardID2[i] = Convert.ToUInt16(cols[idx++]);
            u2RewardCount2[i] = Convert.ToUInt16(cols[idx++]);
        }

        return u1RankType;
    }
}

public class RankSaveInfo
{
    public string strCrewName;
    public UInt64 u8Rank;

    public UInt64 Set(string[] cols)
	{
        UInt16 idx = 0;
        strCrewName = Convert.ToString(cols[idx++]);
        u8Rank = Convert.ToUInt64(cols[idx++]);

        return u8Rank;
    }
}