using UnityEngine;
using System;
using System.Collections.Generic;

public class GuildInfo
{
    public Byte u1MaxMember;
    public Byte u1ListCount;
    public Byte u1MaxJoinList;
    public Byte u1StartDay;
    public Byte u1StartHour;
    public Byte u1EndDay;
    public Byte u1EndHour;
    public Byte u1MaxKey;
    public Byte u1UseKey;
    public Byte u1ChargeTime;
    public Goods gCreateGuildGoods;
	public Byte u1CheckHour;
    public UInt16 u2RankUpPoint;
    public Goods gWinOdinGoods;
    public Goods gDefeatOdinGoods;

    public void Set(string[] cols)
	{
        UInt16 idx = 0;
        u1MaxMember = Convert.ToByte(cols[idx++]);
        u1ListCount = Convert.ToByte(cols[idx++]);
        u1MaxJoinList = Convert.ToByte(cols[idx++]);
        u1StartDay = Convert.ToByte(cols[idx++]);
        u1StartHour = Convert.ToByte(cols[idx++]);
        u1EndDay = Convert.ToByte(cols[idx++]);
        u1EndHour = Convert.ToByte(cols[idx++]);
        u1MaxKey = Convert.ToByte(cols[idx++]);
        u1UseKey = Convert.ToByte(cols[idx++]);
        u1ChargeTime = Convert.ToByte(cols[idx++]);
        gCreateGuildGoods = new Goods(cols, ref idx);
		u1CheckHour = Convert.ToByte(cols[idx++]);
        u2RankUpPoint = Convert.ToUInt16(cols[idx++]);
        gWinOdinGoods = new Goods(cols, ref idx);
        gDefeatOdinGoods = new Goods(cols, ref idx);
    }
}

public class GuildLoginInfo
{
    public Byte u1MemberCount;
    public Goods[] gReward;

    public Byte u1RewardTypeCnt = 4;

    public void Set(String[] cols)
    {
        UInt16 idx = 0;

        u1MemberCount = Convert.ToByte(cols[idx++]);
        gReward = new Goods[u1RewardTypeCnt];
        for(int i=0; i<u1RewardTypeCnt; i++)
            gReward[i] = new Goods(cols, ref idx);
    }
}

public class GuildLeague
{
    public UInt16 u2LeagueID;
    public Byte u1RankType;
    public Byte u1RankMin;
    public Byte u1RankMax;
    public UInt16 u2MatchScoreRange;
    public Byte u1MatchGuildCount;
    public Byte u1TopRankCount;
    public UInt16 u2RankUpPoint;
    public Goods[] gReward;

    public Byte u1RewardTypeCnt = 4;

    public UInt16 Set(String[] cols)
    {
        UInt16 idx = 0;

        u2LeagueID = Convert.ToUInt16(cols[idx++]);
        u1RankType = Convert.ToByte(cols[idx++]);
        u1RankMin = Convert.ToByte(cols[idx++]);
        u1RankMax = Convert.ToByte(cols[idx++]);
        idx++;
        u2MatchScoreRange = Convert.ToUInt16(cols[idx++]);
        u1MatchGuildCount = Convert.ToByte(cols[idx++]);
        u1TopRankCount = Convert.ToByte(cols[idx++]);
        u2RankUpPoint = Convert.ToUInt16(cols[idx++]);

        gReward = new Goods[u1RewardTypeCnt];
        for(int i=0; i<u1RewardTypeCnt; i++)
            gReward[i] = new Goods(cols, ref idx);

        return u2LeagueID;
    }
}

public class GuildList
{
    public UInt64 u8GuildSN;
    public String strGuildName;
    public Byte u1MemberCount;
    public UInt16 u2Score;
	public Byte u1Public;
    public Byte u1Request;
}

public class GuildMemberInfo
{
    public String strGuildName;
    public Byte u1Public;
    public UInt64 u8GuildPower;
    public UInt64 u8Rank;
    public UInt16 u2Score;
    public UInt64 u8LastRank;
    public UInt16 u2LastScore;
    public Byte u1DailyCheckCount;
    public Byte u1LastDailyCheckCount;
    public Byte u1GuildKey;
    public UInt16 u2GuildKeyLeftTime;
    public Byte u1RewardFlag;
    public DateTime dtRewardBeginTime;
	public DateTime dtRewardEndTime;
    public Byte u1MemberCount;
    public List<GuildMember> lstGuildMember;
    public UInt64 u8GuildSN;
	public GuildMember GetMyInfo()
    {
		return lstGuildMember.Find (cs => cs.strLegionName == Legion.Instance.sName);
	}
}

public class GuildMember
{
    public UInt64 u8UserSN;
    public String strLegionName;
    public UInt64 u8Power;
    public UInt16[] u2ClassID;
    public UInt16[] u2Level;
    public Byte[] u1Element;
    public UInt16 u2LeagueCount;
    public Byte u1Option;
    public Int64 u8LastLogin;
	public DateTime dtJoinDate;
	public bool bMember;
}

public struct GuildMatchList
{
    public UInt64 u8GuildSN;
    public String strGuildName;
    public UInt64 u8Power;
    public UInt16 u2Score;
}

public struct GuildJoinList
{
    public UInt64 u8UserSN;
	public String strLegionName;
	public UInt64 u8Power;
}

public struct GuildMatchData
{
    public Byte u1UserDeckCnt;
    public List<GuildMatchCrew> lstGuildCrew;
    public String strMatchingGuildName;
	public UInt16 u2Score;
    public UInt64 u8MatchingGuildPower;
    public Byte u1MatchingDeckCount;
    public List<GuildMatchCrew> lstEnemyCrew;
}

public struct GuildMatchCrew
{
    public UInt64 u8UserSN;
    public Byte u1CrewIndex;
    public Byte u1CharCount;
    public List<GuildMatchCrewChar> lstMatchCrew;
}

public struct GuildMatchCrewChar
{
    public Byte u1CharIndex;
    public String strCharName;
    public UInt16 u2ClassID;
    public UInt16 u2Level;
    public Byte u1CrewPos;
    public Byte[] u1Shape;
    public UInt16[] u2Stats;
    public List<GuildMatchCrewEquipment> lstCrewEquipment;
    public UInt16 u2SkillCount;
    public List<GuildMatchCrewSkill> lstCrewSkill;
}

public struct GuildMatchCrewEquipment
{
    public UInt16 u2EquipmentSlot;
    public UInt16 u2ItemID;
    public String strItemName;
    public Byte u1SmithingLevel;
    public UInt16 u2ModelId;
    public UInt16 u2Level;
    public Byte u1Completeness;
    public UInt32[] u4Stats;
    public Byte[] u1SkillSlot;
    public UInt16[] u2SkillPoint;
    public UInt16[] u2StatPoint;
}

public struct GuildMatchCrewSkill
{
    public Byte u1SkillSlot;
    public UInt16 u2Level;
    public Byte u1SelectSlot;
}

public struct GuildMatchUp
{
    public Hero[] cHero;
}

public struct GuildRankList
{
    public UInt64 u8GuildSN;
    public String strGuildName;
    public UInt64 u8Rank;
    public UInt16 u2Score;
    public UInt64 u8Power;
}

public struct GuildRankInfo
{
    public String strName;
    public UInt64 u8GuildPower;
    public Byte u1DeckCount;
    public List<GuildMatchCrew> lstGuildCrew;
}