using System;
using System.Collections.Generic;
using System.Text;

public class Crew
{
	public const Byte MAX_CHAR_IN_CREW = 3;

	public Byte u1Index;
	public bool[] abLocks;
	public bool IsLock
	{
		get{ return abLocks[0]; }
	}
	public void UnLock()
	{
		abLocks[0] = false;
	}
	public UInt32 u4Power
	{
		get{
			UInt32 power = 0;
			for (Byte i=0; i<acLocation.Length; i++) {
				if(acLocation[i] != null) power += ((Hero)acLocation[i]).u4Power;
			}

			return power;
		}
	}
	public Byte u1Count;
    public Character[] acLocation;
	public Byte u1Emblem;
	public RuneComponent cRuneComponent;

    public Crew()
	{
		acLocation = new Character[MAX_CHAR_IN_CREW];
		abLocks = new bool[MAX_CHAR_IN_CREW];
		for (int i = 0; i < MAX_CHAR_IN_CREW; i++) abLocks[i] = true;
		acLocationBackup = new Character[MAX_CHAR_IN_CREW];
		cRuneComponent = new RuneComponent(this);
	}
	public Crew(Byte _u1Count)
	{
		u1Count = _u1Count;
		acLocation = new Character[_u1Count];
		abLocks = new bool[MAX_CHAR_IN_CREW];
		for (int i = 0; i < MAX_CHAR_IN_CREW; i++) abLocks[i] = true;
		cRuneComponent = new RuneComponent(this);
	}
	public bool Fill(Character hero, int u1X)
	{
		if (u1X >= MAX_CHAR_IN_CREW)
		{
			return false;
		}
        if(acLocation[u1X] == null) u1Count++;
		acLocation[u1X] = hero;
		hero.iIndexInCrew = u1X;
		hero.u1AssignedCrew = u1Index;
		//hero.u1AssignedCrew = Convert.ToByte(u1X+1);
		DebugMgr.Log("AssignedCrew : " + u1X+1);
		
		return true;
	}
	public bool Change(Character hero, Character prevHero)
	{
		for(Byte i=0; i<acLocation.Length; i++)
		{
			if(acLocation[i] == prevHero)
			{
                if(hero != null)
                {
				    acLocation[i] = hero;
                    hero.iIndexInCrew = i;
	        	    hero.u1AssignedCrew = u1Index;
                }
                return true;
			}
            else if(acLocation[i] == hero)
            {
				if(hero != null)
                {
				    acLocation[i] = hero;
                    hero.iIndexInCrew = i;
	        	    hero.u1AssignedCrew = u1Index;
                    prevHero.u1AssignedCrew = 0;
                }
                else
                {
                    prevHero.u1AssignedCrew = 0;
                }
                return true;
            }
		}
        prevHero.u1AssignedCrew = 0;
		return false;
	}
	public bool Assign(Character hero, int u1X, out Character changedhero)
	{
		if (u1X >= MAX_CHAR_IN_CREW)
		{
			changedhero = null;
			return false;
		}
		changedhero = acLocation[u1X];
		acLocation[u1X] = hero;
		hero.iIndexInCrew = u1X;
        //AssignCrew!
		//hero.u1AssignedCrew = u1Index;
		//hero.u1AssignedCrew = Convert.ToByte(u1X+1);
		DebugMgr.Log("AssignedCrwe : " + u1X+1);
		if(changedhero == null)
			u1Count++;

		return true;
	}
	public bool Assign(Character monster, int u1Index)
	{
		if (u1Index >= u1Count)
		{
			return false;
		}
		acLocation[u1Index] = monster;

		return true;
	}

	public Byte Resign(Character character)
	{
		DebugMgr.Log("resign length : " + character.cClass.u2ID);
		for(Byte i=0; i<acLocation.Length; i++)
		{
			if(acLocation[i] == character)
			{
				DebugMgr.Log("Resinged Hero");
                //ResignCrew!
				//character.u1AssignedCrew = 0;
				acLocation[i] = null;
				u1Count--;
				return i;
			}
		}
		return 255;
	}

	public string Name
	{
		get
		{
			return string.Format("{0}   CREW{1}", Legion.Instance.sName, u1Index);
//			switch (u1Index)
//			{
//			case 1:
//				return string.Format("{0}'s first crew", Legion.Instance.sName, u1Index); 
//			case 2:
//				return string.Format("{0}'s second crew", Legion.Instance.sName, u1Index); 
//			case 3:
//				return string.Format("{0}'s third crew", Legion.Instance.sName, u1Index); 
//			default:
//				return string.Format("{0}'s {1}th crew", Legion.Instance.sName, u1Index); 
//			}
		}
	}
	public Character First
	{
		get
		{
			for (int i = 0; i < MAX_CHAR_IN_CREW; i++)
				if (acLocation[i] != null) return acLocation[i];
			return null;
		}
	}
	public void AddExp(UInt32 u4Exp)
	{
		for (int i = 0; i < MAX_CHAR_IN_CREW; i++)
			if (acLocation[i] != null) acLocation[i].GetComponent<LevelComponent>().AddExp(u4Exp);
	}

	public Character[] acLocationBackup;

	public void StartChanging()
	{
		for (int i = 0; i < MAX_CHAR_IN_CREW; i++) 
			acLocationBackup[i] = acLocation[i];
	}
	public bool bDirty
	{
		get
		{
			for (int i = 0; i < MAX_CHAR_IN_CREW; i++)
				if (acLocation[i] != acLocationBackup[i]) 
					return true;
			return false;
		}
	}
	public UInt16 CallServer(Server.OnResponse callback)
	{
		int count = 0;
		for (int i = 0; i < MAX_CHAR_IN_CREW; i++) 
			if(acLocation [i] != acLocationBackup [i]) 
				count++;

		Hero[] heros = new Hero[count];
		Byte[] poses = new Byte[count];
		count = 0;
		for (int i = 0; i < MAX_CHAR_IN_CREW; i++)
		{
			if(acLocation [i] != acLocationBackup [i])
			{
				heros[count] = (Hero)acLocation [i];
				poses[count] = (Byte)i;
				count++;
			}
		}
		//원본
		//return Server.ServerMgr.Instance.ChangeCrewInfo(this, heros, poses, (sName != nameBackup ? sName : string.Empty), (byte)Server.ConstDef.HeroInCrew, callback);
		return Server.ServerMgr.Instance.ChangeCrewInfo(this, heros, poses, callback);
	}
	public void UndoChanging()
	{
		for (int i = 0; i < MAX_CHAR_IN_CREW; i++) acLocation[i] = acLocationBackup[i];
	}
	public void DoChanging()
	{
		bool[] pullback = new bool[MAX_CHAR_IN_CREW];
		for (int i = 0; i < MAX_CHAR_IN_CREW; i++)
		{
			if (acLocation[i] != acLocationBackup[i])
			{
				if (acLocationBackup[i] != null && !pullback[i])
                {
                    //Legion.Instance.acCrews[acLocation[i].u1AssignedCrew - 1].Change(acLocationBackup[i], acLocation[i]);
                    Legion.Instance.acCrews[acLocationBackup[i].u1AssignedCrew - 1].Change(acLocation[i], acLocationBackup[i]);
                    //Legion.Instance.acCrews[acLocationBackup[i].u1AssignedCrew - 1].Change(acLocationBackup[i], acLocation[i]);
                }

				if (acLocation[i] != null)
				{
					if (acLocation[i].u1AssignedCrew != u1Index)
					{
						acLocation[i].u1AssignedCrew = u1Index;
					}
					else
					{
						for (int j = i + 1; j < MAX_CHAR_IN_CREW; j++)
						{
							if (acLocation[i] == acLocationBackup[j]) pullback[j] = true;
						}
					}
				}
			}
		}
	}

    StageInfo cDispatchStage;
    public StageInfo DispatchStage
    {
        get { return cDispatchStage; }
    }
    Byte u1Difficulty;
    public Byte StageDifficulty
    {
        get { return u1Difficulty; }
    }
    DateTime dtDispatchTime;
    public DateTime DispatchTime
    {
        get { return dtDispatchTime; }
    }
    public void Dispatch(StageInfo _cDispatchStage, Byte _u1Difficulty, DateTime _dtDispatchTime)
    {
        cDispatchStage = _cDispatchStage;
        u1Difficulty = _u1Difficulty;
        dtDispatchTime = _dtDispatchTime;
		// 파견 보상
		cDispatchRewaerd = new Reward(cDispatchStage, u1Difficulty);
    }
    public void ClearDispatch()
    {
        cDispatchStage = null;
        u1Difficulty = 0;
		dtDispatchTime = Legion.Instance.ServerTime;

		cDispatchRewaerd = null;
    }
	public void FinishDispatch()
	{
		dtDispatchTime = Legion.Instance.ServerTime;
	}

	public void SetRunes(UInt16[] acIDs){
		cRuneComponent.SetRunes(acIDs);
	}
    
	Reward cDispatchRewaerd; // 파견 보상 정보
	public Reward DispatchRewaerd
	{
		get{ return cDispatchRewaerd; }
	}

    //리그 정보
//    Byte u1Division;
//    public Byte GetDivision
//    {
//        get{ return u1Division; }
//    }
//    public void SetDivision(Byte _division)
//    {
//        u1Division = _division;
//    }
//
//    Byte u1LegendState;
//    public Byte GetLegend
//    {
//        get { return u1LegendState; }
//    }
//    public void SetLegendState(Byte _legend)
//    {
//        u1LegendState = _legend;
//    }
//
//    //리그 파견관련
//    Byte u1DispatchLeague;
//    public Byte DispatchLeague
//    {
//        get {return u1DispatchLeague;}
//    }
//
//    DateTime dtDispatchLeagueTime;
//    public DateTime DispatchLeagueTime
//    {
//        get { return dtDispatchLeagueTime; }
//    }
//
//    public void LeagueDispatch(DateTime _dtDispatchLeagueTime)
//    {
//        u1DispatchLeague = 1;
//        dtDispatchLeagueTime = _dtDispatchLeagueTime;
//    }
//
//    public void SetLeagueDispatchTime(DateTime _dtDispatchLeagueTime)
//    {
//        dtDispatchLeagueTime = _dtDispatchLeagueTime;
//    }
//
//    public void LeagueDispatchClear()
//    {
//        u1DispatchLeague = 0;
//    }
//
//    public void LeagueDispatchFinish()
//    {
//		dtDispatchLeagueTime = Legion.Instance.ServerTime;
//    }
//
//    public void LeagueDispatch(Byte _dispatch)
//    {
//        u1DispatchLeague = _dispatch;
//    }
}