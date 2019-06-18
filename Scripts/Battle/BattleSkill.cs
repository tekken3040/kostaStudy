using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class BattleSkill
{
	public BattleCharacter cOwner;
	public BattleSkillComponent cComponent;
	public SkillInfo cInfo;
	public UInt16 u2Level;
	public Byte u1ActiveSkillIndex = 0;
	public bool bSet = false;
	public bool bDone = false;
	public UInt16 u2Cnt = 0;
	public float fRemainTime = 0;
	//private float fTickTime = 0;
	private float fMaxCoolTime = 0;
	private float fCoolTime = 0;

	private bool bCrewCool = false;

	private bool bCoolOn = true;

	private List<BattleCharacter> lTargets = new List<BattleCharacter>();

	public BattleSkill(SkillNode.Skill cSkill, UInt16 _u2Level)
	{
		cInfo = cSkill.cInfo;
		u2Level = _u2Level;
		fMaxCoolTime = cInfo.fSkillCool;
	}

	public string GetSkillDesc(){
		return cInfo.GetSkillDescToLevel (u2Level);
	}

	public void SetCoolTimeByCrew(){
		if(fCoolTime > LegionInfoMgr.Instance.fGroupCool) return;
		bCoolOn = false;
		bCrewCool = true;
		fCoolTime = LegionInfoMgr.Instance.fGroupCool;
//		if(cOwner.cBattle.TutorialCheckType == 1) fCoolTime = 0;
//		else fCoolTime = LegionInfoMgr.Instance.fGroupCool;
	}

	public void InitCoolTime(){
		fCoolTime = 0f;
	}

	public int GetCoolTime(){
		return Mathf.CeilToInt(fCoolTime);
	}

	public float GetCoolTimePer(){
		float per = 0f;
		if(!bCrewCool) per = fCoolTime / fMaxCoolTime;
		else per = fCoolTime / LegionInfoMgr.Instance.fGroupCool;

		return per;
	}

	public float GetSkillUseRange(){
		return cInfo.cAttackModel.cAttack.u2AttackRange;
	}

	public virtual bool Spell(BattleCrew ours, BattleCrew enemy)
	{
		if (cOwner.isDead && cInfo.u2ResurrectionHPPercent == 0)
			return false;
		//DebugMgr.Log(cComponent.fRemainCoolTime);

		if(fCoolTime > 0f) return false;
		if(cComponent.fRemainCoolTime != 0f) return false;
		
		if(cInfo.u2ResurrectionHPPercent > 0 && u2Cnt > 0){
			return false;
		}
		
//		if(cOwner.u2MP < SkillInfo.USE_MANA_BY_INDEX[u1ActiveSkillIndex]){
//			//DebugMgr.LogError(cOwner.u2MP+" < "+u1ActiveSkillIndex*SkillInfoMgr.Instance.UseMana);
//			return false;
//		}

		//DebugMgr.LogError(u1ActiveSkillIndex+" : "+cOwner.u2MP+" >= "+SkillInfo.USE_MANA_BY_INDEX[u1ActiveSkillIndex]);

		if (cInfo.u4DurationTime > 0)
		{
			fRemainTime = (float)cInfo.u4DurationTime / 1000f;
		}
		else
		{
			bDone = true;
		}

		if(cInfo.u1ActWay == 1){
			//cOwner.cCurAtkModel = cInfo.cAttackModel;

			cComponent.cCurSkill = this;
			cComponent.cCurSkillInfo = cInfo;
			//cComponent.useMana = SkillInfo.USE_MANA_BY_INDEX[u1ActiveSkillIndex];

			cOwner.cTarget = FindTarget(ours, enemy);

			cOwner.UseSkill(u1ActiveSkillIndex);

//			cOwner.u2MP -= (ushort)SkillInfo.USE_MANA_BY_INDEX[u1ActiveSkillIndex];
			if(cInfo.u1Target == SKILL_TARGET_TYPE.Self || cInfo.u1Target == SKILL_TARGET_TYPE.Self_And_Enemy){
				Effect(cOwner);
			}
			bCrewCool = false;
			bCoolOn = false;

			float coolTimeBonus = cOwner.GetRuneVal(RuneType.COOL, 2) - (float)cOwner.cEnemy.GetRuneVal(RuneType.COOL, 1);

			fMaxCoolTime = cInfo.fSkillCool + coolTimeBonus/1000f;
			fCoolTime = fMaxCoolTime;

		}else{
			int rand = UnityEngine.Random.Range(0,100);
			if(cInfo.u1ActSituationPercentApply == 2 || cInfo.u2ActSituationPercent + cInfo.fActPerLvUpBonus * (u2Level - 1) > rand){
				if(cInfo.u4DurationTime > 0) bSet = true;

				lTargets.Clear();

				if (cInfo.u1ActSituation == 13 && cInfo.u1Target == SKILL_TARGET_TYPE.Enemy) {
					lTargets.Add (cOwner.cAttacker);
				}else if (cInfo.u1Target == SKILL_TARGET_TYPE.Ally) {
					for (int i = 0; i < ours.acCharacters.Length; i++) {
						if (!ours.acCharacters [i].isDead) {
							lTargets.Add (ours.acCharacters [i]);
						}
					}
				} else if (cInfo.u1Target == SKILL_TARGET_TYPE.Ally_And_Enemy) {
					for (int i = 0; i < ours.acCharacters.Length; i++) {
						if (!ours.acCharacters [i].isDead) {
							lTargets.Add (ours.acCharacters [i]);
						}
					}
					for (int i = 0; i < enemy.acCharacters.Length; i++) {
						if (!enemy.acCharacters [i].isDead) {
							lTargets.Add (enemy.acCharacters [i]);
						}
					}
				} else {
					lTargets.Add (FindTarget (ours, enemy));
				}

				foreach(BattleCharacter tTarget in lTargets){
					if (tTarget == null)
						continue;
					
					if(tTarget.isDead){
						if(cInfo.u2ResurrectionHPPercent > 0){
							u2Cnt++;
							float bonusPer = cInfo.u2ResurrectionHPPercent + cInfo.fPercentLvUpBonus * (u2Level - 1);

							tTarget.Resurrection(cInfo.sName, (ushort)bonusPer);
						}
					}

					if(cInfo.bIgnoreDef) tTarget.bIgnoreDef = true;
					if(cInfo.bAddDamage){
						float bonusPer = cInfo.fPercentLvUpBonus * (u2Level - 1);
						tTarget.u2AddDamage = (ushort)(cInfo.u2DamagePercent + Convert.ToUInt16(bonusPer));
					}
					if(cInfo.bAttackCrit) tTarget.bAttackCrit = true;
					if(cInfo.bSkillCrit) tTarget.bSkillCrit = true;

					if(cInfo.u2HealPercent > 0){
						float bonusPer = cInfo.u2HealPercent + cInfo.fPercentLvUpBonus * (u2Level - 1);
						tTarget.GetHealPer(bonusPer, 0);
					}

					int hp = tTarget.cBattleStatus.GetStat (1);

					tTarget.cBattleStatus.AddPercent(cInfo.cPassive);
					tTarget.cBattleStatus.AddPercent(cInfo.cPassiveUp, (double)u2Level);
//					tTarget.cBattleStatus.Add(cInfo.cPassive);
//					tTarget.cBattleStatus.Add(cInfo.cPassiveUp, (double)u2Level);

					int bonusHp = tTarget.cBattleStatus.GetStat (1) - hp;
					if(bonusHp > 0) tTarget.u4HP += (uint)bonusHp;

					Effect(tTarget);
					//DebugMgr.Log(cOwner.cCharacter.sName+"'s PassiveTarget = "+tTarget.cCharacter.sName);
				}
			}
		}
		return true;
	}

	public void Effect(BattleCharacter tTarget)
	{
		if (tTarget.isDead)
			return;

		if(cInfo.u1ActSituationPercentApply == 2){
			float rand = UnityEngine.Random.Range(0f,100f);
			
			float bonusPer = cInfo.fActPerLvUpBonus*(u2Level-1);

			if(rand >= cInfo.u2ActSituationPercent+bonusPer){
				return;
			}
		}

		float bonusTime = cInfo.fCondTimeLvUpBonus*(u2Level-1);
		
		if (tTarget.iTeamIdx == cOwner.iTeamIdx) {
			if(cInfo.cBuff != null)
				tTarget.GetComponent<ConditionComponent>().Spell(cInfo.cBuff, cInfo.u1BasicElement, bonusTime, cOwner);
		}else{
			if(cInfo.cDebuff != null){
				tTarget.GetComponent<ConditionComponent>().Spell(cInfo.cDebuff, cInfo.u1BasicElement, bonusTime, cOwner);
			}
		}
	}

	public virtual bool Check(BattleCrew ours, BattleCrew enemy)
	{
		if (cInfo.u1ActWay == 1)
		{
			cOwner.RegistSkillCall(1, u1ActiveSkillIndex, Spell);
		}
		else if (cInfo.u1ActWay == 2)
		{
			if (cInfo.u1ActSituation == 1)
			{
				Spell(ours, enemy);
			}
			else
			{
				cOwner.RegistSkillCall(cInfo.u1ActSituation, 0, Spell);
			}
		}
		return true;
	}

	public virtual void Update()
	{
		if(!bCoolOn){
			fCoolTime -= Time.deltaTime;
			if (fCoolTime <= 0f)
			{
				bCoolOn = true;
				fCoolTime = 0f;
			}
		}

		if (bSet)
		{
			if (fRemainTime > 0f)
			{
				fRemainTime -= Time.deltaTime;
				if (fRemainTime <= 0f)
				{
					fRemainTime = 0f;
					bDone = true;
					bSet = false;

					for(int i=0; i<lTargets.Count; i++){
						lTargets[i].cBattleStatus.Sub(cInfo.cPassive);
					}
				}
			}
		}
	}

	public BattleCharacter FindTarget(BattleCrew ours, BattleCrew enemy) {
		ushort aggro = 0;
		int idx = 0;
		
		switch(cInfo.u1Target){
		case SKILL_TARGET_TYPE.Self: case SKILL_TARGET_TYPE.Ally: case SKILL_TARGET_TYPE.Ally_And_Enemy:
			return cOwner;
			//Dead Function Need
		case SKILL_TARGET_TYPE.Ally_Dead:
			return GetTargetByDead(ours);
		case SKILL_TARGET_TYPE.Enemy:
			return GetTargetByRange(enemy);
		case SKILL_TARGET_TYPE.Ally_Low_HP:
			return GetTargetByLowHP(ours);
		case SKILL_TARGET_TYPE.Self_And_Enemy:
			return GetTargetByRange(enemy);
		}

		return cOwner;
	}

	public bool CheckRange() {
		if(cOwner == cOwner.cTarget) return true;

		Transform ownerTrans = cOwner.cObject.transform;
		Transform targetTrans = cOwner.cTarget.cObject.transform;
		float tempDist = Vector3.Magnitude(ownerTrans.position - targetTrans.position);
		Vector3 targetDir = targetTrans.position - ownerTrans.position;
		bool angleIn = false;

		if(Vector3.Angle(ownerTrans.forward, targetDir) <= cOwner.cCurAtkModel.u2AttackAngle/2){
			if(cOwner.cCurAtkModel.u2AttackRange+cInfo.fRush > tempDist){
				return true;
			}
		}
		
		return false;
	}
	
	BattleCharacter GetTargetByLowHP(BattleCrew crew)
	{
		BattleCharacter tempTarget = null;
		float hp = 1;
		
		for(int i=0; i<crew.acCharacters.Length; i++)
		{
			if(!crew.acCharacters[i].isDead){
				if(crew.acCharacters[i].GetHPPer() <= hp){
					hp = crew.acCharacters[i].GetHPPer();
					tempTarget = crew.acCharacters[i];
				}
			}
		}
		
		if(tempTarget == null) return cOwner;
		
		return tempTarget;
	}

	BattleCharacter GetTargetByDead(BattleCrew crew)
	{
		BattleCharacter tempTarget = null;
		ushort hp = 0;
		
		for(int i=0; i<crew.acCharacters.Length; i++)
		{
			if(crew.acCharacters[i].isDead){
				return tempTarget;
			}
		}
		
		return cOwner;
	}
	
	BattleCharacter GetTargetByRange(BattleCrew crew)
	{
		float dist = 0;
		int idx = 0;
		
		for(int i=0; i<crew.acCharacters.Length; i++){
			if(!crew.acCharacters[i].isDead){
				float tempDist = Vector3.Magnitude(cOwner.cObject.transform.position - crew.acCharacters[i].cObject.transform.position);
				Vector3 targetDir = crew.acCharacters[i].cObject.transform.position - cOwner.cObject.transform.position;
				
				if(tempDist < dist || dist == 0){
					dist = tempDist;
					idx = i;
				}
			}
		}

		return crew.acCharacters[idx];
	}
}

public class BattleSkillComponent : ZComponent
{ 
	public int useMana = 0;
	public float fRemainCoolTime = 0;
	public BattleSkill cCurSkill;
	public SkillInfo cCurSkillInfo;
	public List<BattleSkill> lstcSkills;

	public List<BattleSkill> lstcSelectedActiveSkill;

	private void setSelectedSkills()
	{
		lstcSelectedActiveSkill = new List<BattleSkill>();
		for(int i=0; i<lstcSkills.Count; i++)
		{
			if(lstcSkills[i].cInfo.u1ActWay == 1)
			{
				lstcSelectedActiveSkill.Add(lstcSkills[i]);
			}
		}
	}

	public override void init(System.Object param)
	{
		Character cChar = (Character)param;
		cChar.GetComponent<SkillComponent>().GetBattleSkill(out lstcSkills);
		foreach (BattleSkill cSkill in lstcSkills)
		{
			cSkill.cOwner = (BattleCharacter)owner;
			cSkill.cComponent = this;
		}
		setSelectedSkills();
	}

	public void CheckSkill(BattleCrew ours, BattleCrew enemy)
	{
		foreach (BattleSkill cSkill in lstcSkills)
		{
			cSkill.Check(ours, enemy);
		}
	}

	public void SetCrewCool(){
		foreach (BattleSkill cSkill in lstcSelectedActiveSkill)
		{
			cSkill.SetCoolTimeByCrew();
		}
	}

	public void InitCoolTime(){
		foreach (BattleSkill cSkill in lstcSelectedActiveSkill)
		{
			cSkill.InitCoolTime();
		}
	}

	public void Update()
	{
		if (fRemainCoolTime > 0f)
		{
			fRemainCoolTime -= Time.deltaTime;

			if (fRemainCoolTime <= 0f)
			{
				fRemainCoolTime = 0f;
			}
		}
		foreach (BattleSkill cSkill in lstcSkills)
		{
			cSkill.Update();
		}
	}

}

public class BattleSkillAI
{
	public float fCurrentTime;
	public bool bOff;

	public SkillAIInfo cInfo;

	public BattleSkillAI(){
	}
}
