using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class skillQueue{
	public int charIdx;
	public int skillIdx;

	public skillQueue(int ci, int si){
		charIdx = ci;
		skillIdx = si;
	}
}

public class AIController : MonoBehaviour {
	public Battle cBattle;
	public Controller cCont;

	List<skillQueue> lstAutoSkillQueue;
	List<skillQueue> lstEnemyAutoSkillQueue;

	float fAI_Tick = 1.0f;
	float fCurTime;
	float fEnemyCurTime;
	int iCurIndex;
	int iEnemyCurIndex;

	void Awake(){
		Random.seed = 860916;
	}

	void Start(){ 
		cBattle = GetComponent<Battle>();
	}

	public List<skillQueue> GetAutoSkill(){
		return lstAutoSkillQueue;
	}

	public int GetAutoIndex(){
		if (!cCont.bAuto)
			return lstAutoSkillQueue.Count;

		return iCurIndex;
	}

	public void InitUserEndEnemyQueue(){
		fCurTime = 0f;
		fEnemyCurTime = 0f;
		iCurIndex = 0;
		iEnemyCurIndex = 0;

		SetUserQueue ();
		SetEnemyUserQueue ();
	}

	public void SetUserQueue(){
		int[] aSkillCount = new int[cBattle.acCrews [0].acCharacters.Length];
		int countAll = 0;
		int charIdx = 0;
		int[] skillIdxs = new int[cBattle.acCrews [0].acCharacters.Length];

		for (int i = 0; i < cBattle.acCrews [0].acCharacters.Length; i++) {
			if (!cBattle.acCrews [0].acCharacters [i].isDead) {
				aSkillCount [i] = cBattle.acCrews [0].acCharacters [i].cSkills.lstcSelectedActiveSkill.Count;
				countAll += aSkillCount [i];
			}
		}

		lstAutoSkillQueue = new List<skillQueue> ();

		for (int i = 0; i <countAll; i++) {
			for (int j = 0; j < cBattle.acCrews [0].acCharacters.Length; j++) {
				if (aSkillCount [j] > 0) {
					lstAutoSkillQueue.Add (new skillQueue (j, skillIdxs [j]));
					skillIdxs [j]++;
					aSkillCount [j]--;
				}
			}
		}
	}

	public void SetEnemyUserQueue(){
		int[] aSkillCount = new int[cBattle.acCrews [1].acCharacters.Length];
		int countAll = 0;
		int charIdx = 0;
		int[] skillIdxs = new int[cBattle.acCrews [1].acCharacters.Length];

		for (int i = 0; i < cBattle.acCrews [1].acCharacters.Length; i++) {
			if (!cBattle.acCrews [1].acCharacters [i].isDead) {
				aSkillCount [i] = cBattle.acCrews [1].acCharacters [i].cSkills.lstcSelectedActiveSkill.Count;
				countAll += aSkillCount [i];
			}
		}

		lstEnemyAutoSkillQueue = new List<skillQueue> ();

		for (int i = 0; i <countAll; i++) {
			for (int j = 0; j < cBattle.acCrews [1].acCharacters.Length; j++) {
				if (aSkillCount [j] > 0) {
					lstEnemyAutoSkillQueue.Add (new skillQueue (j, skillIdxs [j]));
					skillIdxs [j]++;
					aSkillCount [j]--;
				}
			}
		}
	}

	public void CheckNextSkill(int charIdx, int skillIdx){
		iCurIndex = lstAutoSkillQueue.FindIndex (cs => cs.charIdx == charIdx && cs.skillIdx == skillIdx);

		if (iCurIndex + 1 >= lstAutoSkillQueue.Count)
			iCurIndex = 0;
		else
			iCurIndex++;
	}

	int[] GernerateRandomArray(int length){
		int[] arr = new int[length];

		for (int i=0; i<length; i++) arr[i] = i;


		for (int i = length - 1; i > 0; i--) {
			int r = Random.Range(0,i+1);
			int tmp = arr[i];
			arr[i] = arr[r];
			arr[r] = tmp;
		}

		return arr;
	}

	public void UpdateSkillAI(bool bAuto, bool bReserve){
		for(int i=0; i<2; i++){
			if (i == 0) {
				if (cBattle.eGameStyle == GameStyle.League || cBattle.eGameStyle == GameStyle.Guild)
				{
					if (bReserve)
					{
						continue;
					}
				} else {
					if (!bAuto || bReserve) 
					{
						continue;
					}
				}

				fCurTime += Time.deltaTime;
				if (fCurTime > fAI_Tick) 
				{
					fCurTime = 0;
				} 
				else 
				{
					continue;
				}

				if (lstAutoSkillQueue.Count == 0) 
				{
					continue;
				} 
				else 
				{
					if (iCurIndex >= lstAutoSkillQueue.Count)
						iCurIndex = 0;
				}
				
				int charIdx = lstAutoSkillQueue [iCurIndex].charIdx;
				int skillIdx = lstAutoSkillQueue [iCurIndex].skillIdx;
				BattleCharacter tBattleChar = cBattle.acCrews [i].acCharacters [charIdx];

				if (cBattle.eGameStyle == GameStyle.League || cBattle.eGameStyle == GameStyle.Guild) {
					if (!tBattleChar.bSupport) 
					{
						if (!bAuto) 
						{
							iCurIndex++;
							fCurTime = fAI_Tick;
							continue;
						}
					}
				}

				if (tBattleChar.isDead) {
					iCurIndex++;
					fCurTime = fAI_Tick;
					continue;
				}

				if(!tBattleChar.CheckTargetInDist(tBattleChar.cSkills.lstcSelectedActiveSkill[skillIdx].GetSkillUseRange()) ){
					tBattleChar.fSkillUseDist = tBattleChar.cSkills.lstcSelectedActiveSkill[skillIdx].GetSkillUseRange();
					tBattleChar.u2SavedSkillIdx = skillIdx;
				}else{
					UseSkill(i, charIdx, skillIdx);
				}
			}else if (i == 1) {
				if (cBattle.eGameStyle == GameStyle.League || cBattle.eGameStyle == GameStyle.Guild) {
					fEnemyCurTime += Time.deltaTime;
					if (fEnemyCurTime > fAI_Tick) {
						fEnemyCurTime = 0;
					} else {
						continue;
					}

					if (lstEnemyAutoSkillQueue.Count == 0) {
						continue;
					} else {
						if (iEnemyCurIndex >= lstEnemyAutoSkillQueue.Count)
							iEnemyCurIndex = 0;
					}

					int charIdx = lstEnemyAutoSkillQueue [iEnemyCurIndex].charIdx;
					int skillIdx = lstEnemyAutoSkillQueue [iEnemyCurIndex].skillIdx;
					BattleCharacter tBattleChar = cBattle.acCrews [i].acCharacters [charIdx];

					if (tBattleChar.isDead) {
						iEnemyCurIndex++;
						continue;
					}

					if (!tBattleChar.CheckTargetInDist (tBattleChar.cSkills.lstcSelectedActiveSkill [skillIdx].GetSkillUseRange ())) {
						tBattleChar.fSkillUseDist = tBattleChar.cSkills.lstcSelectedActiveSkill [skillIdx].GetSkillUseRange ();
						tBattleChar.u2SavedSkillIdx = skillIdx;
					} else {
						UseSkill (i, charIdx, skillIdx);
					}
				}else{
					int[] randOrder = GernerateRandomArray (cBattle.acCrews [i].acCharacters.Length);
					for (int j = 0; j < cBattle.acCrews [i].acCharacters.Length; j++) {
						int idx = randOrder [j];

						BattleCharacter tBattleChar = cBattle.acCrews [i].acCharacters [idx];

						if (tBattleChar.isDead)
							continue;
						for (int k = 0; k < tBattleChar.acAI.Length; k++) {
							if (tBattleChar.acAI [k].cInfo == null)
								continue;

							BattleSkillAI ai = tBattleChar.acAI [k];
							
							tBattleChar.acAI [k].fCurrentTime += Time.deltaTime;

							if (tBattleChar.acAI [k].fCurrentTime * 1000 >= ai.cInfo.u4Time) {
								tBattleChar.acAI [k].fCurrentTime = 0f;
								bool bSelected = SkillAutoSelect (i, idx, tBattleChar, ai);
							}
						}
					}
				}
			}
		}
	}

	bool SkillAutoSelect(int team_idx, int char_idx, BattleCharacter tBattleChar, BattleSkillAI ai){
		if (tBattleChar.fSkillUseDist > 0)
			return false;

		if(ai.cInfo.u1AIType == 1){
			int total = 0;
			for(int i=0; i<ai.cInfo.au2Values.Length; i++){
				total += ai.cInfo.au2Values[i];
			}

			int rand = Random.Range(0,total);
			total = 0;
			for(int i=0; i<ai.cInfo.au2Values.Length; i++){
				total += ai.cInfo.au2Values[i];
				if(total > rand){
					if(tBattleChar.aeSkill.Length <= i){ return false; }
					if(tBattleChar.aeSkill[i] == null){ return false; }

					if(!tBattleChar.CheckTargetInDist(tBattleChar.cSkills.lstcSelectedActiveSkill[i].GetSkillUseRange()) ){
						tBattleChar.fSkillUseDist = tBattleChar.cSkills.lstcSelectedActiveSkill[i].GetSkillUseRange();
						tBattleChar.u2SavedSkillIdx = i;
						return false;
					}else{
						UseSkill(team_idx, char_idx, i);
						return true;
					}
				}
			}
		}else if(ai.cInfo.u1AIType == 2){
			for(int i=0; i<ai.cInfo.au2Values.Length; i++){
				int skillIdx = tBattleChar.u2LastSkillIdx;

				if(tBattleChar.aeSkill.Length <= i){ return false; }
				if(tBattleChar.aeSkill[i] == null){ return false; }

				if(!tBattleChar.CheckTargetInDist(tBattleChar.cSkills.lstcSelectedActiveSkill[skillIdx].GetSkillUseRange()) ){
					tBattleChar.fSkillUseDist = tBattleChar.cSkills.lstcSelectedActiveSkill[skillIdx].GetSkillUseRange();
					tBattleChar.u2SavedSkillIdx = skillIdx;
					return false;
				}else{
					UseSkill(team_idx, char_idx, skillIdx);
					return true;
				}
			}
		}else if(ai.cInfo.u1AIType == 3){
			if(ai.bOff) return false;

			for(int i=0; i<ai.cInfo.au2Values.Length; i++){
				int skillIdx = CheckCondition(tBattleChar, ai);
				if(skillIdx >= 0){
					if(UseSkill(team_idx, char_idx, skillIdx)){
						if(!ai.cInfo.bLoop) ai.bOff = true;
					}
					return true;
				}
			}
		}
		return false;
	}

	int CheckCondition(BattleCharacter tBattleChar, BattleSkillAI ai){
		if (ai.cInfo.u2HPPer > 0) {
			//if((((int)tBattleChar.u4HP*100)/tBattleChar.cBattleStatus.u4HP) <= ai.cInfo.u2HPPer){
            if((((int)tBattleChar.u4HP*100)/tBattleChar.cBattleStatus.GetStat(1)) <= ai.cInfo.u2HPPer){
				return tBattleChar.cSkills.lstcSelectedActiveSkill.FindIndex(cs => cs.cInfo.u2ID == ai.cInfo.u2UseSkillID);
			}
		}else if (ai.cInfo.u2Damage > 0) {
			if(tBattleChar.iTotalDmg >= ai.cInfo.u2Damage){
				tBattleChar.iTotalDmg = 0;
				return tBattleChar.cSkills.lstcSelectedActiveSkill.FindIndex(cs => cs.cInfo.u2ID == ai.cInfo.u2UseSkillID);
			}
		}

		return -1;
	}

	bool UseSkill(int team_idx, int char_idx, int skill_idx){
		return cCont.UseSkill (team_idx, char_idx, skill_idx);
	}
}
