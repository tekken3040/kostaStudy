using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public class GuildInfoMgr : Singleton<GuildInfoMgr>
{
	public const UInt16 GuildBattleField = 4907;

    public UInt64 u8GuildSN = 0;
	public bool bRefreshGuild = false;
    public Byte u1GuildCrewIndex = 0;
    public Byte u1GuildListCount = 0;
    public Byte u1SelectedCrewIndex = 0;
    public Byte u1JoinUserCount = 1;
    public Byte u1GuildMatchListCount = 0;
    public Byte u1GuildRankCount = 0;

    public GuildMemberInfo cGuildMemberInfo;
    public GuildMatchData cGuildMatchData;

	public GuildMatchCrew cGuildDetailData;
	public Crew cGuildDetailCrew;

    public GuildInfo cGuildInfo;
    public GuildMember MyGuildInfo;

	public List<GuildLoginInfo> cGuildLoginInfo;
    public Dictionary<UInt16, GuildLeague> dicGuildLeague;
    public List<GuildRankList> lstGuildRank;
    public List<UInt16> lstRewardIds;

    public GuildRankInfo cGuildRankInfo;

    //public GuildMatchUp[] cUserCrews;
    //public GuildMatchUp[] cEnemyCrews;
    public Crew[] cUserCrews;
    public Crew[] cEnemyCrews;
    public Dictionary<Byte, GuildList> dicGuildList;
    public List<GuildMatchList> lstGuildMatchList;
    public Dictionary<Byte, GuildMember> dicUserEntry;
    public Dictionary<Byte, GuildMember> dicMasterEntry;
    public Dictionary<Byte, GuildMember> dicUserEntryBackUp;

    public bool bDirty = false;
    public bool bGuildMaster = false;

    public DateTime dtGuildKeyChargeTime;
    public static Byte MAX_GUILD_ENTRY = 3;

    private bool loadedInfo = false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}

    public enum SET_CREW_TYPE
    {
        GuildMaster = 1,
        UserCustom = 2,
        UserSelect =3
    }

    public enum GUILD_MARK_TYPE
    {
        None = 0,
        Login = 1,
        LoginReward = 2,
        BattleReward = 3,
        Secession = 4,
        Public = 5,
        Private = 6,
        DeleteGuild = 7,
    }

    public enum GUILD_RANK_TYPE
    {
        None = 0,
        RankList = 1,
        RankDetail = 2,
    }
    
    public TimeSpan tsGuildKeyTime;
    private ObscuredUShort u2GuildKey;
    public UInt16 GuildKey
    {
        set
        {
            u2GuildKey.RandomizeCryptoKey();
            u2GuildKey = value;
			//RefreshGoodInfo ();
        }
        get
        {
            u2GuildKey.RandomizeCryptoKey();
            return u2GuildKey;
        }
    }
    private IEnumerator CheckTime()
    {
		yield return StartCoroutine(Utillity.WaitForRealSeconds(1f));

        while (true)
        {            
            if(GuildKey < cGuildInfo.u1MaxKey)
            {
                TimeSpan timeSpan = dtGuildKeyChargeTime - Legion.Instance.ServerTime;
                tsGuildKeyTime = timeSpan;
                if(timeSpan.Ticks <= 0)
                {
                    GuildKey+=1;
                    dtGuildKeyChargeTime = Legion.Instance.ServerTime.AddSeconds(TimeSpan.FromHours(cGuildInfo.u1ChargeTime).TotalSeconds);
                }
            }
                                          
            yield return StartCoroutine(Utillity.WaitForRealSeconds(1f));
        }
    }

	public void InitUserData()
	{
		u8GuildSN = 0;
		u1GuildListCount = 0;
		u1SelectedCrewIndex = 0;
		u1JoinUserCount = 1;
		u1GuildMatchListCount = 0;
        u1GuildRankCount = 0;
        GuildKey = 0;

		cGuildMemberInfo = new GuildMemberInfo();
		cGuildMatchData = new GuildMatchData();
        cGuildRankInfo = new GuildRankInfo();
        MyGuildInfo = new GuildMember();
        lstRewardIds = new List<UInt16>();
		cUserCrews = new Crew[MAX_GUILD_ENTRY];
		cEnemyCrews = new Crew[MAX_GUILD_ENTRY];
		lstGuildMatchList = new List<GuildMatchList>();
        lstGuildRank = new List<GuildRankList>();
		dicUserEntry = new Dictionary<Byte, GuildMember>();
		dicMasterEntry = new Dictionary<Byte, GuildMember>();
		dicUserEntryBackUp = new Dictionary<Byte, GuildMember>();
	}

    public void InitGuildData()
    {
		Inits ();
        DataMgr.Instance.LoadTable(this.AddGuildInfo, "Guild");
        DataMgr.Instance.LoadTable(this.AddGuildLoginInfo, "GuildLoginCheck");
        DataMgr.Instance.LoadTable(this.AddGuildLeague, "GuildLeague");
    }
    
    public void Inits()
    {
        //u1GuildCrewIndex = Legion.Instance.cBestCrew.u1Index;
        dicGuildList = new Dictionary<Byte, GuildList>();
        cGuildMemberInfo = new GuildMemberInfo();
        cGuildMatchData = new GuildMatchData();
        cGuildInfo = new GuildInfo();
        cGuildRankInfo = new GuildRankInfo();
		cGuildLoginInfo = new List<GuildLoginInfo> ();
        MyGuildInfo = new GuildMember();
        dicGuildLeague = new Dictionary<UInt16, GuildLeague>();
        lstRewardIds = new List<UInt16>();
        cUserCrews = new Crew[MAX_GUILD_ENTRY];
        cEnemyCrews = new Crew[MAX_GUILD_ENTRY];
        lstGuildMatchList = new List<GuildMatchList>();
        lstGuildRank = new List<GuildRankList>();
        dicUserEntry = new Dictionary<Byte, GuildMember>();
        dicMasterEntry = new Dictionary<Byte, GuildMember>();
        dicUserEntryBackUp = new Dictionary<Byte, GuildMember>();
        StartCoroutine(CheckTime());
    }

    public void CreateMatchCrews()
    {
        cUserCrews = new Crew[MAX_GUILD_ENTRY];
        cEnemyCrews = new Crew[MAX_GUILD_ENTRY];
        int idx = 0;
        for(int i=0; i<MAX_GUILD_ENTRY; i++)
        {
            cUserCrews[i] = new Crew();
            cEnemyCrews[i] = new Crew();
        }
        for(int i=0; i<cGuildMatchData.lstGuildCrew.Count; i++)
        {
            for(int j=0; j<cGuildMatchData.lstGuildCrew[i].lstMatchCrew.Count; j++)
                SetCrewData(cGuildMatchData.lstGuildCrew[i].lstMatchCrew[j], cGuildMatchData.lstGuildCrew[i].u1CrewIndex, true);
        }
        for(int i=0; i<cGuildMatchData.lstEnemyCrew.Count; i++)
        {
            for(int j=0; j<cGuildMatchData.lstEnemyCrew[i].lstMatchCrew.Count; j++)
                SetCrewData(cGuildMatchData.lstEnemyCrew[i].lstMatchCrew[j], cGuildMatchData.lstEnemyCrew[i].u1CrewIndex, false);
        }

        for(int i=0; i<MAX_GUILD_ENTRY; i++)
        {
            if(dicUserEntry.ContainsKey((Byte)(i+1)))
                if(dicUserEntry[(Byte)(i+1)].strLegionName == Legion.Instance.sName)
                    idx = i;
        }

        cUserCrews[idx] = Legion.Instance.acCrews[u1GuildCrewIndex-1];
    }

	public ulong GetUserDeckPower()
	{
		ulong power = 0;
		for(int i=0; i<cUserCrews.Length; i++)
		{
			power += cUserCrews [i].u4Power;
		}

		return power;
	}

	public void SetCrewData(GuildMatchCrewChar charData, int crewIdx, bool bFriendly = false, bool bDetail = false)
    {
        Hero tempHero = new Hero(charData.u1CharIndex, charData.u2ClassID, charData.strCharName, charData.u1Shape[0], charData.u1Shape[1], charData.u1Shape[2]);
        List<LearnedSkill> lstLearnInfo = null;
		LearnedSkill temp;

        UInt32[] heroStats = new UInt32[Server.ConstDef.CharStatPointType];
        heroStats[0] = charData.u2Stats[0];
        heroStats[1] = charData.u2Stats[1];
        heroStats[2] = charData.u2Stats[2];
        heroStats[3] = charData.u2Stats[3];
        heroStats[4] = charData.u2Stats[4];
        heroStats[5] = charData.u2Stats[5];
        heroStats[6] = charData.u2Stats[6];

        tempHero.GetComponent<StatusComponent>().LoadStatus(heroStats, 0);
        tempHero.GetComponent<LevelComponent>().Set(Convert.ToByte(charData.u2Level), 0);

        for(int i=0; i<Hero.MAX_EQUIP_OF_CHAR; i++)
        {
            tempHero.acEquips[i] = new EquipmentItem(charData.lstCrewEquipment[i].u2ItemID);
            tempHero.acEquips[i].GetComponent<StatusComponent>().LoadStatus(charData.lstCrewEquipment[i].u4Stats,
            tempHero.acEquips[i].GetEquipmentInfo().acStatAddInfo[0].u1StatType,
            tempHero.acEquips[i].GetEquipmentInfo().acStatAddInfo[1].u1StatType,
            tempHero.acEquips[i].GetEquipmentInfo().acStatAddInfo[2].u1StatType, 0);
            tempHero.acEquips[i].u1SmithingLevel = charData.lstCrewEquipment[i].u1SmithingLevel;
            tempHero.acEquips[i].u1Completeness = charData.lstCrewEquipment[i].u1Completeness;
            tempHero.acEquips[i].GetComponent<LevelComponent>().Set(Convert.ToByte(charData.lstCrewEquipment[i].u2Level), 0);
            tempHero.acEquips[i].skillSlots = charData.lstCrewEquipment[i].u1SkillSlot;
            tempHero.GetComponent<StatusComponent>().Wear(tempHero.acEquips[i].cStatus);
        }
        lstLearnInfo = new List<LearnedSkill>();

        for(int i=0; i<charData.u2SkillCount; i++)
        {
            temp = new LearnedSkill();
            temp.u1SlotNum = charData.lstCrewSkill[i].u1SkillSlot;
            temp.u2Level = charData.lstCrewSkill[i].u2Level;
            temp.u1UseIndex = charData.lstCrewSkill[i].u1SelectSlot;
            lstLearnInfo.Add(temp);
        }

        tempHero.GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, lstLearnInfo, 0, 0, 0);

		if (bDetail) {
			cGuildDetailCrew.Fill(tempHero, charData.u1CrewPos);
		} else {
			if (bFriendly)
				cUserCrews [crewIdx].Fill(tempHero, charData.u1CrewPos);
			else
				cEnemyCrews [crewIdx].Fill(tempHero, charData.u1CrewPos);
		}
    }

    public void AddGuildInfo(String[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        GuildInfo info = new GuildInfo();
        info.Set(cols);
        cGuildInfo = info;
    }

    public void AddGuildLoginInfo(String[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        GuildLoginInfo info = new GuildLoginInfo();
        info.Set(cols);
		cGuildLoginInfo.Add (info);
    }

    public void AddGuildLeague(String[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        GuildLeague info = new GuildLeague();
        info.Set(cols);
        lstRewardIds.Add(info.u2LeagueID);
		dicGuildLeague.Add(info.u2LeagueID, info);
    }

    public void CheckGuildError(Server.ERROR_ID err)
    {
		if (err == Server.ERROR_ID.FRIEND_REQUEST_DELETED)
        {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetText ("err_guild_already_exit_member"), ReloadScene);
		}
        else if (err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET)
        {
			InitUserData ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_server_error"), TextManager.Instance.GetText ("err_guild_already_exit_guild"), ReloadScene);
		}
        else if(err == Server.ERROR_ID.GUILD_REQUEST_CONFLICT)
        {
            InitUserData ();
            GuildInfoMgr.Instance.u8GuildSN = 1;
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("err_title_guild_exit_reject"), TextManager.Instance.GetText ("err_desc_guild_exit_reject"), ReloadScene);
        }
		else if(err == Server.ERROR_ID.GUILD_DATA_DENIED)
		{
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_guild_calculate"), TextManager.Instance.GetText ("popup_desc_guild_calculate"), ReloadScene);
		}
    }

    public void ReloadScene(object[] param)
    {
		Server.ServerMgr.Instance.CallClear(null);
        GuildInfoMgr.Instance.bDirty = false;
        AssetMgr.Instance.SceneLoad("GuildScene");
    }

    public UInt16 GetGuildTier(UInt16 _score)
    {
        UInt16 _tier = 0;

        _tier = (UInt16)(_score/cGuildInfo.u2RankUpPoint);

        if(_tier == 0)
            _tier = 1;

        return _tier;
    }

    public UInt16 GetGuildBattleScore(UInt16 _myScroe, UInt16 _enemyScore)
    {
        UInt16 _winScore = 0;

        float _m = _myScroe;
        float _e = _enemyScore;

        if((_e/_m)*10 > 20)
            _winScore = 20;
        else if((_e/_m)*10 < 1)
            _winScore = 1;
        else
        {
            _winScore = (UInt16)((_e/_m)*10);
            float v = ((_e%_m)*10);
            if(v != 0)
                _winScore += 1;

            if(_winScore > 20)
                _winScore = 20;
        }

        return _winScore;
    }

    public UInt16 GetDivisionRewardID(UInt16 _Tier)
    {
        UInt16 rewardId = 0;

        if(_Tier >= 1 || _Tier <= 5)
            rewardId = lstRewardIds[8];
        else if(_Tier >= 6 || _Tier <= 10)
            rewardId = lstRewardIds[9];
        else if(_Tier >= 11 || _Tier <= 15)
            rewardId = lstRewardIds[10];
        else if(_Tier >= 16 || _Tier <= 20)
            rewardId = lstRewardIds[11];
        else if(_Tier >= 21 || _Tier <= 25)
            rewardId = lstRewardIds[12];

        return rewardId;
    }

    public UInt16 GetRankRewardID(UInt16 _Score)
    {
        UInt16 rewardId = 0;

        if(_Score == 1)
            rewardId = lstRewardIds[0];
        else if(_Score == 2)
            rewardId = lstRewardIds[1];
        else if(_Score == 3)
            rewardId = lstRewardIds[2];
        else if(_Score == 4)
            rewardId = lstRewardIds[3];
        else if(_Score == 5)
            rewardId = lstRewardIds[4];
        else if(_Score >= 6 || _Score <= 10)
            rewardId = lstRewardIds[5];
        else if(_Score >= 11 || _Score <= 50)
            rewardId = lstRewardIds[6];
        else if(_Score >= 51 || _Score <= 100)
            rewardId = lstRewardIds[7];

        return rewardId;
    }

    public Byte GetDays(String str)
    {
        Byte u1Day = 0;
        switch(str)
        {
            case "Monday":
                u1Day = 1;
                break;

            case "Tuesday":
                u1Day = 2;
                break;

            case "Wednesday":
                u1Day = 3;
                break;

            case "Thursday":
                u1Day = 4;
                break;

            case "Friday":
                u1Day = 5;
                break;

            case "Saturday":
                u1Day = 6;
                break;

            case "Sunday":
                u1Day = 7;
                break;
        }

        return u1Day;
    }
}
