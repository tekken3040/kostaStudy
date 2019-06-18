using UnityEngine;
using System;
using System.Collections;

public class LeagueCrew {

	public const Byte MAX_CHAR_IN_CREW = 5;

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
	public Byte u1Count
    {
        get
        {
            Byte count = 0;
            for(int i=0; i<acLocation.Length; i++)
            {
                if(acLocation[i] != null)
                    count++;
            }

            return count;
        }
    }

	public Character[] acLocation;
	public Byte u1Emblem;
    
	public LeagueCrew()
	{
		acLocation = new Character[MAX_CHAR_IN_CREW];
		acLocationBackup = new Character[MAX_CHAR_IN_CREW];
	}

	public bool Assign(Character hero, int u1X, out Character changedhero)
	{
		if (u1X >= MAX_CHAR_IN_CREW)
		{
			changedhero = null;
			return false;
		}
		changedhero = acLocation[u1X];
        if(changedhero != null)
            changedhero.bAssignedLeagueCrew = false;
		acLocation[u1X] = hero;
        acLocation[u1X].bAssignedLeagueCrew = true;
		hero.iIndexInCrew = u1X;
		DebugMgr.Log("AssignedCrwe : " + u1X+1);
		//if(changedhero == null)
		//	u1Count++;

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
                acLocation[i].bAssignedLeagueCrew = false;
                acLocation[i].iIndexInCrew = -1;
				acLocation[i] = null;
				//u1Count--;
				return i;
			}
		}
		return 255;
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
    public void UndoChanging()
	{
		for (int i = 0; i < MAX_CHAR_IN_CREW; i++)
            acLocation[i] = acLocationBackup[i];
	}
}
