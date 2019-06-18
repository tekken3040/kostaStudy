using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public enum CONDITION_TYPE{
	ImMovable = 1,
	CantAttack,
	CantSpell,
	Invincible,
	Absorb,
	Shield,
	MoveSlow,
	DefPer,
	RegPer,
	EvaPer
}

public class Condition
{
	public ConditionInfo cInfo;
	BattleCharacter owner;
	BattleCharacter cCaster;
	float fTotalTime;
	float fRemainTime;
	float fTickTime;
	float fDelayTime;
	public float fMoveSpdPer;
	int iShieldGage;
	public float fDefPer;
	public float fRegPer;
	public float fEvaPer;
	Status cAdded;
	GameObject EffObj;
	List<CONDITION_TYPE> lstExcept = new List<CONDITION_TYPE>();
	UI_ConditionIcon cIcon;
	int iPerDmg = 0;
	Byte u1Element = 0;
	bool bFin = false;

	public Condition(ConditionInfo info, BattleCharacter cChar, Byte u1BasicElement, float fBonus, BattleCharacter caster)
	{
		cInfo = info;
		owner = cChar;
		cCaster = caster;
		int stat = 0; 

		if (u1BasicElement == 1)
			stat = cCaster.cBattleStatus.GetStat (2);
		else
			stat = cCaster.cBattleStatus.GetStat (3);
		
		iPerDmg = (int)(stat * ((float)cInfo.s4DurationDamage)/100f);

		fTotalTime = ((float)cInfo.u4DurationTime / 1000f)+fBonus;
		fRemainTime = fTotalTime;
		fTickTime = (float)cInfo.u4DamageDurationTime / 1000f;
		fDelayTime = (float)cInfo.u4TimeForStop / 1000f;
		cAdded = cChar.cBattleStatus.AddPercent(info.cPercent);

		if (owner.cObject == null || !owner.cObject.activeSelf)
			return;

		Vector3 pos = owner.cObject.transform.position;

		if (cInfo.cCreatePos != null)
			pos = owner.GetSocketTrans (cInfo.cCreatePos.sSocBone).position;

		if (cChar.bHero && GraphicOption.condition_hero [Legion.Instance.graphicGrade]) {
			if (owner.iTeamIdx == 0)
				EffObj = VFXMgr.Instance.GetVFX ("/Condition/" + cInfo.sIcon + "_C", pos, Quaternion.identity);
			else
				EffObj = VFXMgr.Instance.GetVFX ("/Condition/" + cInfo.sIcon + "_M", pos, Quaternion.identity);

			ParticleSystem[] particles = EffObj.GetComponentsInChildren<ParticleSystem> ();
			foreach (ParticleSystem part in particles) {
				part.loop = true;
			}
			EffObj.transform.parent = owner.cObject.transform;
		} else if (!cChar.bHero && GraphicOption.condition_mon [Legion.Instance.graphicGrade]) {
			if (owner.iTeamIdx == 0)
				EffObj = VFXMgr.Instance.GetVFX ("/Condition/" + cInfo.sIcon + "_C", pos, Quaternion.identity);
			else
				EffObj = VFXMgr.Instance.GetVFX ("/Condition/" + cInfo.sIcon + "_M", pos, Quaternion.identity);

			ParticleSystem[] particles = EffObj.GetComponentsInChildren<ParticleSystem> ();
			foreach (ParticleSystem part in particles) {
				part.loop = true;
			}
			EffObj.transform.parent = owner.cObject.transform;
		}

		//cInfo.u1EndType
		//AddExcept(false);

		//DebugMgr.Log("cAdded.u2Strength = "+cAdded.u2Strength+"cAdded.u2Intelligence = "+cAdded.u2Intelligence);

		//DebugMgr.Log(owner.cCharacter.sName+"'s AddCondtion "+cInfo.sName);
		if (cInfo.sCircle == "buff") {
			if(owner.iTeamIdx == 0) owner.cBattle.battleUIMgr.DuplicateDmg (owner.cObject.transform.position, TextManager.Instance.GetText (cInfo.sName) + "\n0", BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_HERO, 28);
		} else {
			if(owner.iTeamIdx == 0) owner.cBattle.battleUIMgr.DuplicateDmg (owner.cObject.transform.position, TextManager.Instance.GetText (cInfo.sName) + "\n0", BattleUIMgr.DAMAGE_TEXT_TYPE.DAMAGED_HERO, 28);
		}

		if (info.u4DurationTime > 0) {
			if (owner.iTeamIdx == 0)
				cIcon = owner.cBattle.battleUIMgr.AddCondition (owner, cInfo);
			else
				cIcon = owner.cBattle.battleUIMgr.AddConditionMonster (owner, this);
		}
	}

	public void SetIcon(UI_ConditionIcon tIcon){
		if (cInfo.u4DurationTime > 0)
			cIcon = tIcon;
	}

	public void AddExcept(bool bDelay)
	{
		if(bDelay){
			if(cInfo.u1Movable == 3){
				lstExcept.Add(CONDITION_TYPE.ImMovable);
				if(owner.eCharacterState == CHAR_STATE.Move){
					ActionCut();
				}

				if(!cInfo.bBaseAttack){
					lstExcept.Add(CONDITION_TYPE.CantAttack);
					if(owner.eCharacterState == CHAR_STATE.AttackPre){
						ActionCut();
					}
				}
				if (!cInfo.bSkill) {
					lstExcept.Add (CONDITION_TYPE.CantSpell);
					if(owner.bSkill) ActionCut();
				}
			}
		}else{
			if(cInfo.bConditionClear){
				owner.cCondis.Clear();
			}

			if(cInfo.bChangable){
				Change();
			}

			if(cInfo.u1Movable == 2){
				lstExcept.Add(CONDITION_TYPE.ImMovable);
				if(owner.eCharacterState == CHAR_STATE.Move){
					ActionCut();
				}
			}

			if(cInfo.u1Movable != 3){
				if(!cInfo.bBaseAttack){
					lstExcept.Add(CONDITION_TYPE.CantAttack);
					if(owner.eCharacterState == CHAR_STATE.Attacking){
						if(!owner.bSkill) ActionCut();
					}
				}
				if (!cInfo.bSkill) {
					lstExcept.Add (CONDITION_TYPE.CantSpell);
					if(owner.bSkill) ActionCut();
				}
			}
			if(cInfo.bUnbeatable) lstExcept.Add(CONDITION_TYPE.Invincible);
			if(cInfo.bAbsorption) lstExcept.Add(CONDITION_TYPE.Absorb);
			if(cInfo.fShieldPer > 0){
				lstExcept.Add(CONDITION_TYPE.Shield);
				iShieldGage = (int)((owner.cBattleStatus.GetStat(1)+owner.cBattleStatus.GetStat(4)+owner.cBattleStatus.GetStat(5))*cInfo.fShieldPer/100);
				DebugMgr.LogError (iShieldGage);
			}

			if(cInfo.fAddDefPer != 0){
				lstExcept.Add(CONDITION_TYPE.DefPer);
				fDefPer = cInfo.fAddDefPer;
			}

			if(cInfo.fAddRegPer != 0){
				lstExcept.Add(CONDITION_TYPE.RegPer);
				fRegPer = cInfo.fAddRegPer;
			}

			if(cInfo.fAddEvaPer != 0){
				lstExcept.Add(CONDITION_TYPE.EvaPer);
				fEvaPer = cInfo.fAddEvaPer;
			}

			if(cInfo.u2MoveSpeedPercent != 0){
				lstExcept.Add(CONDITION_TYPE.MoveSlow);
				fMoveSpdPer = (float)(100f+cInfo.u2MoveSpeedPercent)/100f;
				owner.ChangeMoveSpd();
			}
		}
	}

	void ActionCut(){
		if(owner.cCondis.CheckCondition(CONDITION_TYPE.Invincible))
			return;
		
		owner.SetIdle(false);
	}

	public void Change(){
		//float hpPer = (float)owner.u4HP/(float)owner.cBattleStatus.u4HP;
        float hpPer = (float)owner.u4HP/(float)owner.cBattleStatus.GetStat(1);

		Monster cMonster = new Monster(cInfo.u2ChangingClassID, "Change");
		cMonster.GetComponent<LevelComponent>().Set(1,0);
		owner.ModelEnable(false);
		cMonster.attachModel(owner.cObject);
		cMonster.attachAnimator(owner.cObject);
		DebugMgr.Log(hpPer);
		owner.SetChangeCharacter(cMonster, hpPer);
	}

	public void ChangeEnd(){
		//float hpPer = (float)owner.u4HP/(float)owner.cBattleStatus.u4HP;
        float hpPer = (float)owner.u4HP/(float)owner.cBattleStatus.GetStat(1);
		DebugMgr.Log(hpPer);
		owner.SetOriginalCharacter(hpPer);
	}

	public int DecShieldGage(int val){
		if (iShieldGage < 0)
			return 0;

		iShieldGage -= val;
		if(iShieldGage <= 0) fRemainTime = 0.0f;

		return iShieldGage;
	}

	public bool CheckExcept(CONDITION_TYPE type){
		if(lstExcept.FindIndex(cs => cs == type) > -1) return true;

		return false;
	}

	public bool Update()
	{
		fRemainTime -= Time.deltaTime;

		if (fTickTime > 0.0f) {
			fTickTime -= Time.deltaTime;
			if(fTickTime <= 0){
				fTickTime = (float)cInfo.u4DamageDurationTime / 1000f;

				float tfEneDef = owner.GetDefPer(u1Element, cCaster.cCharacter.cLevel.u2Level);
				int tiTotalDmg = 0;

				if (tfEneDef >= 100f)
					tiTotalDmg = 0;
				else
					tiTotalDmg = (int)((iPerDmg*(100f-tfEneDef))/100f);

				owner.GetDamage(tiTotalDmg, false, 0, TextManager.Instance.GetText(cInfo.sName), cCaster);
			}
		}

		if (fDelayTime > 0.0f) {
			fDelayTime -= Time.deltaTime;
			if (fDelayTime <= 0f) {
				AddExcept(true);
			}
		}

		if(cIcon != null) cIcon.UpdateCoolTime(fRemainTime/fTotalTime);

		if (fRemainTime <= 0f)
		{
			if (!bFin) {
				Finish ();
				return true;
			}
		}
		return false;
	}

	public void Finish()
	{
		//DebugMgr.Log(owner.cCharacter.sName+"'s SubCondtion "+cInfo.sName);
		owner.cBattleStatus.Sub(cAdded);
		fMoveSpdPer = 1.0f;
		owner.ChangeMoveSpd();
		if(EffObj != null){
			ParticleSystem[] particles = EffObj.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem part in particles) {
				part.enableEmission = false;
			}
			EffObj.AddComponent<DestroyTime> ().SetTime (0.3f);
		}

		if(!owner.isDead && owner.bChange){
			ChangeEnd();
		}
		if(cIcon != null){
			cIcon.DestroyMe();
			cIcon = null;
		}
		fDelayTime = 0f;
		fTickTime = 0f;

		bFin = true;
	}
}

public class ConditionComponent : ZComponent
{
	public List<Condition> lstConditions;
	List<Condition> lstTemp;
	BattleCharacter cOwner = null;

	GameObject TextObj;

	Dictionary<UInt16, GameObject> TextList;

	public override void init(System.Object param)
	{
		lstConditions = new List<Condition>();
		lstTemp = new List<Condition>();

		TextList = new Dictionary<UInt16, GameObject> ();
		TextObj = AssetMgr.Instance.AssetLoad("Prefabs/UI/Battle/CondiText.prefab", typeof(GameObject)) as GameObject;
	}

	public Condition Spell(ConditionInfo info, Byte u1BasicEle, float fBonus, BattleCharacter caster)
	{
		// if special Condition, make sub class of Condition
		// else do bellow
		if(cOwner == null) cOwner = (BattleCharacter)owner;

		if(cOwner.CheckEmun()) return null;

		int idx = lstConditions.FindIndex (cs => cs.cInfo.u2Group == info.u2Group && cs.cInfo.u1GroupLevel >= info.u1GroupLevel);
		if(idx > -1){
			if (info.fShieldPer > 0)
				return null;
			
			lstConditions[idx].Finish();
			lstConditions.RemoveAt(idx);

			DeleteTxt (info.u2ID);
		}

		Condition cCon = new Condition(info, cOwner, u1BasicEle, fBonus, caster);

		CreateTxt (info, fBonus);

		lstConditions.Add(cCon);
		cCon.AddExcept(false);
		return cCon;
		// return condition to do special thing outside
	}

	void CreateTxt(ConditionInfo info, float fBonus){
		if (cOwner.bHero)
			return;

		BattleUIMgr.DAMAGE_TEXT_TYPE txtType = BattleUIMgr.DAMAGE_TEXT_TYPE.DAMAGED_HERO;

		if (cOwner.iTeamIdx == 0) {
			if(info.sCircle == "buff") txtType = BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_HERO;
			else txtType = BattleUIMgr.DAMAGE_TEXT_TYPE.DAMAGED_HERO;
		}else if (cOwner.iTeamIdx == 1) {
			if(info.sCircle == "buff") txtType = BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_MONSTER;
			else txtType = BattleUIMgr.DAMAGE_TEXT_TYPE.DAMAGED_MONSTER;
		}

		GameObject txt = GameObject.Instantiate(TextObj) as GameObject;
		FadeText txtScript = txt.GetComponent<FadeText> ();
		Vector3 tmpPos =  cOwner.iTeamIdx == 0 ? cOwner.GetBodyPos():cOwner.GetHeadPos(1f, 0.3f);
		Vector3 textPos = cOwner.cBattle.battleUIMgr.cUICam.ViewportToWorldPoint(cOwner.cBattle.battleUIMgr.cMainCam.WorldToViewportPoint(tmpPos));

		txt.transform.SetParent(cOwner.cBattle.battleUIMgr.objDmgTextParent);
		txt.transform.localScale = Vector3.one;

		txt.transform.position = textPos;
		if (TextList.Count > 0) {
			txt.transform.localPosition = txt.transform.localPosition + Vector3.up*TextList.Count*35f;
		}
		txt.name = "CondiTxt"+info.u2ID;
		txtScript.SetText(TextManager.Instance.GetText (info.sName), txtType, Color.black, false, ((float)info.u4DurationTime / 1000f)+fBonus, 28);

		if(!TextList.ContainsKey (info.u2ID)) TextList.Add (info.u2ID, txt);
	}

	void DeleteTxt(UInt16 u2ID){
		if (cOwner.bHero)
			return;
		
		if (!TextList.ContainsKey (u2ID))
			return;

		GameObject.Destroy (TextList [u2ID]);
		TextList.Remove (u2ID);
	}

	void MoveTxt(UInt16 u2ID, int idx){
		if (cOwner.bHero)
			return;
		
		if (!TextList.ContainsKey (u2ID))
			return;
		
		Vector3 tmpPos = cOwner.GetBodyPos();
		Vector3 textPos = cOwner.cBattle.battleUIMgr.cUICam.ViewportToWorldPoint(cOwner.cBattle.battleUIMgr.cMainCam.WorldToViewportPoint(tmpPos));

		TextList[u2ID].transform.position = textPos;
		if (idx > 0) {
			TextList[u2ID].transform.localPosition = TextList[u2ID].transform.localPosition + Vector3.up*idx*35f;
		}
	}

//	public Condition Spell(UInt16 id)
//	{
//		ConditionInfo info = ConditionInfoMgr.Instance.GetInfo(id);
//		// if special Condition, make sub class of Condition
//		// else do bellow
//		Condition cCon = new Condition(info, (BattleCharacter)owner, 0f);
//		int idx = lstConditions.FindIndex (cs => cs.cInfo.u2ID == id);
//		if(idx > -1){
//			lstConditions[idx].Finish();
//			lstConditions.RemoveAt(idx);
//		}
//		lstConditions.Add(cCon);
//		cCon.AddExcept(false);
//		return cCon;
//		// return condition to do special thing outside
//	}

	public bool CheckCondition(CONDITION_TYPE type){
		foreach (Condition cCon in lstConditions)
		{
			if(cCon.CheckExcept(type)) return true;
		}
		
		return false;
	}

	public void CheckDamageEnd(){
		if(lstConditions.Count == 0) return;

		for (int i=0; i<lstConditions.Count; i++)
		{
			if(lstConditions[i].cInfo.u1EndType == 2){
				lstConditions[i].Finish();
				DeleteTxt (lstConditions[i].cInfo.u2ID);
				lstConditions.RemoveAt(i);
				i--;
			}
		}
	}

	public int DecShield(int dmg){
		int shield = 0;

		foreach (Condition cCon in lstConditions)
		{
			if(cCon.CheckExcept(CONDITION_TYPE.Shield)){
				shield += cCon.DecShieldGage(dmg);
				//if(shield >= 0) return shield;
			}
		}
		
		return shield;
	}

	public float GetMoveSpdPer(){
		float spd = 1.0f;
		
		foreach (Condition cCon in lstConditions)
		{
			if(cCon.CheckExcept(CONDITION_TYPE.MoveSlow)){
				spd *= cCon.fMoveSpdPer;
			}
		}
		
		return spd;
	}

	public float GetDefPer(){
		float per = 0f;

		foreach (Condition cCon in lstConditions)
		{
			if(cCon.CheckExcept(CONDITION_TYPE.DefPer)){
				per += cCon.fDefPer;
			}
		}

		return per;
	}

	public float GetRegPer(){
		float per = 0f;

		foreach (Condition cCon in lstConditions)
		{
			if(cCon.CheckExcept(CONDITION_TYPE.RegPer)){
				per += cCon.fRegPer;
			}
		}

		return per;
	}

	public float GetEvaPer(){
		float per = 0f;

		foreach (Condition cCon in lstConditions)
		{
			if(cCon.CheckExcept(CONDITION_TYPE.EvaPer)){
				per += cCon.fEvaPer;
			}
		}

		return per;
	}

	public void Dispel()
	{
		for (int i=0; i<lstConditions.Count; i++)
		{
			DeleteTxt (lstConditions[i].cInfo.u2ID);
			lstConditions[i].Finish();
		}
		lstConditions.Clear();
	}

	public void Clear()
	{
		for (int i=0; i<lstConditions.Count; i++)
		{
			if (lstConditions [i].cInfo.sCircle != "buff") {
				lstConditions [i].Finish ();
				DeleteTxt (lstConditions [i].cInfo.u2ID);
				lstConditions.RemoveAt(i);
				i--;
			}
		}
	}


	public void Update()
	{
		lstTemp.Clear();
		int idx = 0;
		for (int i=0; i<lstConditions.Count; i++)
		{
			if (lstConditions [i].Update ())
				lstTemp.Add (lstConditions [i]);
			else {
				if(lstConditions.Count > i) MoveTxt (lstConditions [i].cInfo.u2ID, idx++);
			}
		}
		if (lstTemp.Count > 0)
		{
			for (int i=0; i<lstTemp.Count; i++)
			{
				DeleteTxt (lstTemp[i].cInfo.u2ID);
				lstConditions.Remove(lstTemp[i]);
			}
		}
	}
}
