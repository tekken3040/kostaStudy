
using System;
using System.Collections.Generic;
using System.Text;

public class BattleCrew
{
	public const int MAX_CHAR_IN_CREW = 5;
	
	public Battle cBattle;
	Byte u1CrewIndex;
	public bool bSaveSkill;
	public float fLegionCoolTime;
	//public Crew cCrew;
    public BattleCharacter[] acCharacters;
	public Byte[] au1LineCount;

	public UInt16 u2LastBuff;
	public UInt16 u2LastDebuff;
	public Byte u1ComboElement;
	public Byte u1ComboCount;

	public Pos cPosInfo;

    public BattleCrew()
	{
        //cCrew = null;
        acCharacters = null;
    }

	public BattleCrew(Battle cParent)
	{
		//cCrew = null;
		cBattle = cParent;
	}

    public BattleCrew(Crew crew, Battle cParent, Byte u1Index)
    {
		cBattle = cParent;
		u1CrewIndex = u1Index;
		//cCrew = crew;
		DebugMgr.Log (crew.u1Count);
		acCharacters = new BattleCharacter[crew.u1Count];
		au1LineCount = new Byte[2];
		Byte i = 0;
		for (Byte x = 0; x < MAX_CHAR_IN_CREW; x++)
		{
			if(x >= crew.acLocation.Length) continue;
			if (crew.acLocation[x] != null)
			{
				DebugMgr.Log (i+"/"+x);
				acCharacters[i++] = new BattleCharacter(crew.acLocation[x], cBattle, this);
			}
		}
	}

	public BattleCrew(LeagueCrew crew, Battle cParent, Byte u1Index)
	{
		cBattle = cParent;
		u1CrewIndex = u1Index;
		//cCrew = crew;
		acCharacters = new BattleCharacter[crew.u1Count];
		au1LineCount = new Byte[2];
		Byte i = 0;
		for (Byte x = 0; x < MAX_CHAR_IN_CREW; x++)
		{
			if(x >= crew.acLocation.Length) continue;
			if (crew.acLocation[x] != null)
			{
				bool bSup = false;
				if (x >= 3) bSup = true;
				acCharacters[i++] = new BattleCharacter(crew.acLocation[x], cBattle, this, bSup);
			}
		}
	}

	public float GetRuneVal(RuneType eType, Byte u1Increase){
		if(cBattle.eGameStyle == GameStyle.Stage) return 0;

		//return (float)cCrew.cRuneComponent.GetTotalEffVal(eType, u1Increase);
		return 0;
	}

	public BattleCrew(FieldInfo field, Battle cParent, int iGroup)
	{
		cBattle = cParent;
		Crew cCrew = new Crew((Byte)field.acMonsterGroup[iGroup].u1MonsterCountMax);

		u1CrewIndex = 1;	// field mon crew must be set on index 1
		acCharacters = new BattleCharacter[cCrew.u1Count];
		au1LineCount = null;

		int monNum = 0;

		Dictionary<ushort, int> MaxInGroup = new Dictionary<ushort, int>();

		for (Byte i = 0; i < field.acMonsterGroup[iGroup].u1SubGroupNum; i++)
		{
			ushort monID = field.acMonsterGroup[iGroup].acMonsterInfo[i].u2MonsterID;
			if(!MaxInGroup.ContainsKey(monID)){
				MaxInGroup.Add(monID, (int)field.acMonsterGroup[iGroup].acMonsterInfo[i].u1SpawnCount);
			}else{
				MaxInGroup[monID] += (int)field.acMonsterGroup[iGroup].acMonsterInfo[i].u1SpawnCount;
			}

			for(Byte j = 0; j < field.acMonsterGroup[iGroup].acMonsterInfo[i].u1SpawnCount; j++){
				Monster mob = new Monster(field.acMonsterGroup[iGroup].acMonsterInfo[i].u2MonsterID, "Monster"+monNum);
				Level lv = field.acMonsterGroup [iGroup].acMonsterInfo [i].cLevel;
				if(Legion.Instance.SelectedDifficult > 1) lv.u2Level += field.acMonsterGroup[iGroup].u2DiffcultyAddLevel[Legion.Instance.SelectedDifficult-2];
				mob.GetComponent<LevelComponent>().Set(lv);
				mob.GetComponent<SkillComponent>().LoadAllSkill();
				cCrew.Assign(mob, monNum);
				acCharacters[monNum] = new BattleCharacter(mob, cBattle, this);
				monNum++;
				if(monNum >= field.acMonsterGroup[iGroup].u1MonsterCountMax) break;
			}

			if(monNum >= field.acMonsterGroup[iGroup].u1MonsterCountMax) break;
		}
		cPosInfo = field.acMonsterGroupPosInfo[iGroup].cCenterPos;

		foreach (ushort val in MaxInGroup.Keys) {
			cBattle.AddMaxMonsterInGroupInfo(val, MaxInGroup[val]);
		}
	}
	public void EventToCharacters(Battle.BROADCAST_EVENT_TYPE eType, BattleCrew cEnemyCrew)
	{
		foreach (BattleCharacter bc in acCharacters)
		{
			bc.EventToCharacters(eType, this, cEnemyCrew);
		}
	}
}