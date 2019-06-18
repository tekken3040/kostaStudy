using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UI_League : Singleton<UI_League>
{
    public LeagueMatch _leagueMatch;
    public LeagueMatchResult _leagueMatchResult;
    public LeagueMatchStart _leagueMatchStart;
    public LeagueReward _leagueReward;
    public LeagueInfomation _leagueInfomation;
    public LeagueGroup _leagueGroup;
    public LeagueGroups _leagueGroups;
    public LeagueLegendRank _leagueLegendRank;

    public void SetLeagueMatchData(LeagueMatch _match) { _leagueMatch = _match; }
    public void SetLeagueMatchResult(LeagueMatchResult _result) { _leagueMatchResult = _result; }
    public void SetLeagueMatchStart(LeagueMatchStart _start) { _leagueMatchStart = _start; }
    public void SetLeagueReward(LeagueReward _reward) { _leagueReward = _reward; }
    public void SetLeagueInfomation(LeagueInfomation _infomation) { _leagueInfomation = _infomation; }
    public void SetLeagueGroup(LeagueGroup _group) { _leagueGroup = _group; }
    public void SetLeagueGroups(LeagueGroups _groups) { _leagueGroups = _groups; }
    public void SetLeagueLegend(LeagueLegendRank _legend) { _leagueLegendRank = _legend; }

    public string sEnemyName;
    public LeagueCrew EnemyCrew;

    public LeagueMatchList cLeagueMatchList;
    public Byte u1SelectLeagueCharIndex;
	public Byte u1Prom; //0:없음 1:승격 2:강등
	public UInt32 u1BeforeRank = 0;
    public LeagueMatchList.ListSlotData RevengeCrew;
    public GameObject MyLeagueCrewSlot;
    public Byte u1SelectEnemyCrewRevenge = 0;
    public UInt64 u8SelectEnemyCrewSN = 0;

    void Awake()
    {
        cLeagueMatchList = new LeagueMatchList();
        RevengeCrew = new LeagueMatchList.ListSlotData();
    }

    public void CreateEnemyCrew()
    {
		sEnemyName = _leagueMatch.strLegionName;
		EnemyCrew = new LeagueCrew();
		List<LearnedSkill> lstLearnInfo = null;
		LearnedSkill temp;
		Hero[] tmpHeroes = new Hero[_leagueMatch.u1CharCount];
        
        for(int i=0; i<_leagueMatch.u1CharCount; i++)
        {
            tmpHeroes[i] = new Hero(_leagueMatch.lstLeagueCharInfo[i].u1CharIndex, _leagueMatch.lstLeagueCharInfo[i].u2ClassID, _leagueMatch.lstLeagueCharInfo[i].strCharName, 
                                                                             _leagueMatch.lstLeagueCharInfo[i].u1Shape[0], _leagueMatch.lstLeagueCharInfo[i].u1Shape[1], _leagueMatch.lstLeagueCharInfo[i].u1Shape[2]);
            UInt32[] heroStats = new UInt32[Server.ConstDef.CharStatPointType];
            heroStats[0] = _leagueMatch.lstLeagueCharInfo[i].u2HPPoint;
            heroStats[1] = _leagueMatch.lstLeagueCharInfo[i].u2StrPoint;
            heroStats[2] = _leagueMatch.lstLeagueCharInfo[i].u2IntPoint;
            heroStats[3] = _leagueMatch.lstLeagueCharInfo[i].u2DefPoint;
            heroStats[4] = _leagueMatch.lstLeagueCharInfo[i].u2ResPoint;
            heroStats[5] = _leagueMatch.lstLeagueCharInfo[i].u2AgiPoint;
            heroStats[6] = _leagueMatch.lstLeagueCharInfo[i].u2CriPoint;
            tmpHeroes[i].GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
            tmpHeroes[i].GetComponent<LevelComponent>().Set(Convert.ToByte(_leagueMatch.lstLeagueCharInfo[i].u2Level), 0);
            for(int j=0; j<Hero.MAX_EQUIP_OF_CHAR; j++)
            {
				UInt16 slot = _leagueMatch.lstLeagueCharInfo[i].u2EquipItemSlot[j];
                tmpHeroes[i].acEquips[j] = new EquipmentItem(_leagueMatch.dicLeagueCharEquipInfo[slot].u2ItemID);
				tmpHeroes[i].acEquips[j].GetComponent<StatusComponent>().LoadStatus(_leagueMatch.dicLeagueCharEquipInfo[slot].u4Stat, 
                tmpHeroes[i].acEquips[j].GetEquipmentInfo().acStatAddInfo[0].u1StatType, 
                tmpHeroes[i].acEquips[j].GetEquipmentInfo().acStatAddInfo[1].u1StatType,
                tmpHeroes[i].acEquips[j].GetEquipmentInfo().acStatAddInfo[2].u1StatType, 0);
                tmpHeroes[i].acEquips[j].u1SmithingLevel = _leagueMatch.dicLeagueCharEquipInfo[slot].u1SmithingLevel;
                tmpHeroes[i].acEquips[j].u1Completeness = _leagueMatch.dicLeagueCharEquipInfo[slot].u1Completeness;
				tmpHeroes[i].acEquips[j].GetComponent<LevelComponent>().Set(Convert.ToByte(_leagueMatch.dicLeagueCharEquipInfo[slot].u2Level), 0);
				tmpHeroes[i].acEquips[j].skillSlots = _leagueMatch.dicLeagueCharEquipInfo[slot].u1SkillSlot;

                tmpHeroes[i].GetComponent<StatusComponent>().Wear(tmpHeroes[i].acEquips[j].cStatus);
            }
            lstLearnInfo = new List<LearnedSkill>();
            
            for(int j=0; j<_leagueMatch.u2CharSkillCount; j++)
            {
                if(_leagueMatch.lstLeagueCharSkillInfo[j].u1CharIndex != _leagueMatch.lstLeagueCharInfo[i].u1CharIndex)
                    continue;
                temp = new LearnedSkill();
                temp.u1SlotNum = _leagueMatch.lstLeagueCharSkillInfo[j].u1SkillSlot;
                temp.u2Level = Convert.ToUInt16(_leagueMatch.lstLeagueCharSkillInfo[j].u2Level);
                temp.u1UseIndex = _leagueMatch.lstLeagueCharSkillInfo[j].u1SelectSlot;
                lstLearnInfo.Add(temp);
                
            }
			
            tmpHeroes[i].GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);
            Character tempChar;
             
            EnemyCrew.Assign(tmpHeroes[i], _leagueMatch.lstLeagueCharInfo[i].u1CrewPos-1, out tempChar);
        }
	}
}

public class LeagueMatch
{
    public string strLegionName;
    public UInt32 u4Point;
    public UInt16 u2Win;
    public UInt16 u2Draw;
    public UInt16 u2Lose;
    public Byte u1CharCount;
    public List<LeagueCharInfo> lstLeagueCharInfo;
    public UInt16 u2EquipCount;
    public Dictionary<UInt16,LeagueCharEquipInfo> dicLeagueCharEquipInfo;
    public UInt16 u2CharSkillCount;
    public List<LeagueCharSkillInfo> lstLeagueCharSkillInfo;
}

public class LeagueCharInfo
{
    public Byte u1CharIndex;
    public string strCharName;
    public UInt16 u2ClassID;
    public UInt16 u2Level;
    public Byte u1CrewPos;
    public Byte[] u1Shape; //3
    public UInt16 u2HPPoint;
    public UInt16 u2StrPoint;
    public UInt16 u2IntPoint;
    public UInt16 u2DefPoint;
    public UInt16 u2ResPoint;
    public UInt16 u2AgiPoint;
    public UInt16 u2CriPoint;
    public UInt16[] u2EquipItemSlot; //10
    /*public UInt16 u2EquipCount;
    public List<LeagueCharEquipInfo> lstLeagueCharEquipInfo;
    public UInt16 u2CharSkillCount;
    public List<LeagueCharSkillInfo> lstLeagueCharSkillInfo;*/
}

public class LeagueCharEquipInfo
{
    public UInt16 u2Slot;
    public UInt16 u2ItemID;
    public string strItemName;
    public Byte u1SmithingLevel;
    public UInt16 u2ModelID;
    public UInt16 u2Level;
    public Byte u1Completeness;
    public UInt32[] u4Stat; //3
    public Byte[] u1SkillSlot; //3
    public UInt16[] u2SkillPoint; //3
    public UInt16[] u2StatPoint; //3
}

public class LeagueCharSkillInfo
{
    public Byte u1CharIndex;
    public Byte u1SkillSlot;
    public UInt16 u2Level;
    public Byte u1SelectSlot;
}

public class LeagueMatchResult
{
    public Byte u1Rank;
    public UInt16 u2LegendRank;
}

public class LeagueMatchStart
{
    public UInt16 u2FieldID;
    public Byte u1DayOrNight;
    public Byte u1SkyBox;
    public UInt16 u2Key;
    public UInt16 u2KeyLeftTime;
}

public class LeagueReward
{
    public Byte u1LastDivision;
    public Byte u1DivisionRank;
    public Byte u1DivRwdCount;
    public struct DivRwdItem
    {
        public Byte u1DivRwdType;
        public UInt16 u2DivRwdID;
        public UInt32 u4DivRwdNumber;
    }
    public List<DivRwdItem> lstDivRwdItem;
    public Byte u1PromotionCount;
    public struct PromotionRwditem
    {
        public Byte u1Division;
        public Byte u1ProRwdCount;
        public List<DivRwdItem> lstProRwdItem;
    }
    public List<PromotionRwditem> lstProRwdItem;
}

public class LeagueInfomation
{
    public Byte u1CrewCount;
    public List<LeagueCrewInfo> lstLeagueCrewInfo;
}

public class LeagueCrewInfo
{
    public Byte u1CrewIndex;
    public Byte u1DivisionIndex;
    public UInt16 u2GroupNo;
    public Byte u1State;
    public Byte u1Rank;
    public Byte u1Legend;
    public UInt16 u2LegendRank;
    public Byte u1Reward;
    public UInt16 u2Point;
    public UInt16 u2Win;
    public UInt16 u2Lose;
    public Byte u1LeftMatchKey;
    public Byte u1LeagueDispatch;
    public Int64 u8DispatchTime;
    public DateTime dtDispatchTime;
}

public class LeagueGroup
{
    public UInt16 u1State;
    public Byte u1CrewCount;
    public List<LeagueCrewListInfo> lstLeagueCrewListInfo;
}

public class LeagueCrewListInfo
{
    public string strLegionName;
    public Byte u1CrewIndex;
    public UInt16 u2Win;
    public UInt16 u2Lose;
    public UInt16 u2Point;
    public Byte u1Legend;
}

public class LeagueGroups
{
    public Byte u1GroupCount;
    public List<LeagueGroupsInfo> lstLeagueGroupInfo;
}

public class LeagueGroupsInfo
{
    public UInt16 u2GroupID;
    public Byte u1CrewCount;
    public List<LeagueGroupsCrewInfo> lstLeagueGroupsCrewInfo;
}

public class LeagueGroupsCrewInfo
{
    public string strLegionName;
    public Byte u1CrewIndex;
    public UInt16 u2Win;
    public UInt16 u2Lose;
    public UInt16 u2Point;
    public Byte u1Legend;
}

public class LeagueLegendRank
{
    public Int64 u8CloseTime;
    public TimeSpan tsLeftTime;
    public Byte u1Count;
    public RankInfo[] sRankInfo;
    public struct RankInfo
    {
        public UInt32 u4Rank;
        public String strLegionName;
        public UInt32 u4Point;
        public UInt16 u2Win;
        public UInt16 u2Draw;
        public UInt16 u2Lose;
    }
}

public class LeagueMatchList
{
    public Byte u1DivisionIndex;
    public Byte u1LastCheckDicisionIndex;
	public Byte u1PrevDivisionIndex;
	public UInt32 u4PrevMyRank;
    public UInt32 u4MyPoint;
    public Byte u1Reward;
    public UInt32 u4MyRank;
    public Byte u1Count;
    public struct ListSlotData
    {
        public UInt32 u4Rank;
        public UInt64 u8UserSN;
        public UInt16 u2ClassID;
        public String strLegionName;
        public UInt32 u4Point;
        public Int64 u8Time;
        public DateTime dtTime;
        public Byte u1Revenge;
        public String strRevengeMessage;
    }
    public ListSlotData[] sListSlotData;

	public Byte GetPrveDivisionIndex 
	{
		get
		{
			if(u1PrevDivisionIndex == 6 && u4PrevMyRank == 1)
				++u1PrevDivisionIndex;

			return u1PrevDivisionIndex;
		}
	}
}