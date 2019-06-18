using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Missile : MonoBehaviour {
	
	//	enum MissileType {
	//		Melee,
	//		RangeStraight,
	//		RangeCurve
	//	}
	
	enum TARGET_TYPE {
		Target,
		NonTarget
	}
	
	public enum ATTACK_TYPE {
		Single,
		Range
	}
	
	BattleCharacter cOwner;
	BattleCharacter cTarget;
	List<BattleCharacter> lTargets = new List<BattleCharacter>();
	BattleCrew cEnemys;
	Transform cOwnerTrans;
	Transform cTargetTrans;
	Vector3 v3Position;

	//Timer
	float fShoot;
	float fDest;
	float fTime;
	float fTick;
	float fRemain;
	bool bShot;
	bool bEffect;

	//	MissileType eMissileType;
	TARGET_TYPE eTargetType;
	ATTACK_TYPE eAttackType;
	
	Transform cTransform;

	//Damage
	bool bCritcal;
	int iDamage;
	int iAttackElement;

	//HighAngle
	float fCurGravity = 0.98f;
	float fCurSpeed;
	float fFactor = 2f;

	//MissileModel
	Transform cModelObj;
	VFX cModelVFX;

	//AttackInfo
	AttackModelInfo cAtkModel;

	//SkillInfo
	bool bSkill;
	bool bIgnore;
	BattleSkill cCurSkill;

	//HitPosition
	Vector3 hitPos = new Vector3(0f,0.4f,0f);

	List<BattleCharacter> lstHittedChar = new List<BattleCharacter>();

	Vector3 fixedDir;

	int iChainBonus;

	bool bDest = false;

	void Awake() {
		cTransform = transform;
	}
	
	// Use this for initialization
	public void SetInfo (BattleCharacter owner, BattleCharacter target, BattleCrew enemy, int[] dmgInfo, ATTACK_TYPE eType, bool igDef) {
		eAttackType = eType;
		bCritcal = dmgInfo[0] == 1 ? true:false;
		iDamage = dmgInfo[1];
		iAttackElement = dmgInfo[2];
		if(enemy != null) cEnemys = enemy;
		Init(owner, target);
		bIgnore = igDef;
	}

	public void SetAngle(float angle){
		fixedDir = Quaternion.Euler(new Vector3(0,angle,0))*cOwnerTrans.forward;
		fixedDir.y = 0.0f;
		fixedDir = Vector3.Normalize(fixedDir)*cAtkModel.u2MissileSpeed/1000f;
	}
	
	public void Init (BattleCharacter owner, BattleCharacter target) {
		cOwner = owner;
		cOwnerTrans = cOwner.cObject.transform;
		iChainBonus = cOwner.iChainBonus;

		v3Position = cOwner.cObject.transform.position+hitPos;

		cAtkModel = new AttackModelInfo ();
		cAtkModel = owner.cCurAtkModel;

		if(target != null){
			cTarget = target;
			hitPos = new Vector3(0,cTarget.GetHitHeight(),0);
			cTargetTrans = target.cObject.transform;
			v3Position = target.cObject.transform.position+hitPos;
		}

		cTransform.LookAt(v3Position);

		if(cAtkModel.u1BasicAttackType == 6){
			eTargetType = TARGET_TYPE.NonTarget;
		}else{
			eTargetType = TARGET_TYPE.Target;
		}

		if(cAtkModel.u1BasicAttack == 1 && cOwner.lTargets.Count > 0) lTargets = cOwner.lTargets;

		fTick = cAtkModel.u2AttackTick/1000f;
		fRemain = cAtkModel.u2DurationTime/1000f;

		if(cOwner.cSkills.cCurSkillInfo != null){
			bSkill = true;
			bEffect = cOwner.bEffect;
			cCurSkill = cOwner.cSkills.cCurSkill;
		}
		SetHitTime();
		CreateModel();
	}
	
	public void SetHitTime(){
		float fShootTime = cAtkModel.u2MissileShootTime;

		fShoot = (fShootTime / 1000f);
		if(cOwner.fAnimSpd > 1.0f) fShoot = fShoot/cOwner.fAnimSpd;

		float div = cOwner.fAtkSpd;
		if(div < 1.0f) div = 1.0f;

		fDest = fShoot/div;
		fFactor = cOwner.fAtkSpd*1.5f;

		switch(cAtkModel.u1BasicAttack){
		case 1:
			break;

		case 2: case 3:
			if(cAtkModel.u2MissileSpeed > 0){
				float fProjSpd = (float)cAtkModel.u2MissileSpeed;
				float fDist = Vector3.Distance(cTransform.position, v3Position);
				fDest += (1000f/fProjSpd)*fDist;
				
				fCurSpeed = (1000f/fProjSpd)*fDist/2f*fFactor;

				float modifySpd = 0.0f;
				if(fCurSpeed < 2f){
					modifySpd = 2f-fCurSpeed;
					fFactor *= 1.0f+modifySpd/fCurSpeed;
					fCurSpeed = 2f;
				}else if(fCurSpeed > 3f){
					modifySpd = fCurSpeed - 3f;
					fFactor *= 1.0f-modifySpd/fCurSpeed;
					fCurSpeed = 3f;
				}
			}
			break;

		case 5:
			fDest = cAtkModel.u2DurationTime/1000f;
			break;
		}
	}

	public void PlayVFX(bool play){
		if(cModelVFX == null) return;

		cModelVFX.SetPlay(play);
	}
	
	void CreateModel(){
		if(cAtkModel.u1BasicAttack == 1 && fShoot <= 0){
			CheckHit();
			return;
		}

		if (cAtkModel.u1BasicAttack == 4) {
			transform.parent = cOwner.GetCreateSock();
			transform.name = cAtkModel.u2ID.ToString();
		}

		if(cAtkModel.sMissileModelName == "") DebugMgr.LogError(cAtkModel.u2ID+" is have not sMissileModelName");

		if(cAtkModel.sMissileModelName != "0"){
			if (!GraphicOption.attack_missile [Legion.Instance.graphicGrade])
				return;
			//DebugMgr.Log(cAtkModel.sMissileModelName);
			cModelObj = VFXMgr.Instance.GetVFX(cAtkModel.sMissileModelName, cTransform.position, transform.rotation).transform;
			cModelObj.transform.parent = transform;
			cModelVFX = new VFX(cModelObj);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(cAtkModel.u1BasicAttack != 1){
			if (cTarget == null && !bDest) {
				DeleteObject ();
				return;
			}
		}
		
		fTime += Time.deltaTime;
		
		if(fTime >= fShoot){
			if(cAtkModel.u2MissileSpeed > 0){
				bShot = true;
			}
		}
		
		if(bShot){
			if(fixedDir != Vector3.zero){
				cTransform.position += fixedDir*Time.deltaTime;
			}else{
				float dist = 0.0f;
				Vector3 dir = cTransform.forward;

				if(fDest-fTime > 0){
					if(eTargetType == TARGET_TYPE.Target){
						Vector3 targetPos = v3Position;
						if (!cTarget.isDead) {
							targetPos = cTargetTrans.position + hitPos;
						}else{
							targetPos.y = cTransform.position.y;
						}
						cTransform.LookAt(targetPos);
						dist = Vector3.Distance(cTransform.position, targetPos);
					}else if(eTargetType == TARGET_TYPE.NonTarget){
						cTransform.LookAt(v3Position);
						dist = Vector3.Distance(cTransform.position, v3Position);
					}

					dir = (dist/(fDest-fTime))*cTransform.forward*Time.deltaTime;
				}

				cTransform.position += dir;

				if(cAtkModel.u1BasicAttack == 3){
					fCurSpeed -= Time.deltaTime*fFactor;
					fCurGravity = Time.deltaTime*fCurSpeed;
					cModelObj.position += new Vector3(0,fCurGravity,0);
					//cModelObj.localRotation = Quaternion.Euler(new Vector3(
				}
			}
		}

		if (cAtkModel.bPenetrate) {
			CheckHit();
			if(bShot){
				if(fTime >= fDest) DeleteObject();
			}
		}else{
			if(fTime >= fDest){
				fTick -= Time.deltaTime;
				if(fTick <= 0.0f){
					fTick = cAtkModel.u2AttackTick/1000f;
					CheckHit();
				}

				fRemain -= Time.deltaTime;
				if(fRemain <= 0.0f) DeleteObject();
			}
		}
	}

	void CheckHit(){
		if (cOwner == null || cTarget == null || cAtkModel == null) {
			DeleteObject();
			return;
		}

		if(cOwner.isDead || cTarget.cObject == null){
			DeleteObject();
			return;
		}

		if(cAtkModel.u1BasicAttack != 1 && cAtkModel.sMissileCrushModelName != "0"){
			bool bEff = false;
			if (bSkill) {
				bEff = GraphicOption.attack_skill [Legion.Instance.graphicGrade];
			} else {
				bEff = GraphicOption.attack_basic [Legion.Instance.graphicGrade];
			}

			if (bEff) {
				Vector3 pos = cTarget.cObject.transform.position;

				if (cAtkModel.u1HitPos == 1 || cAtkModel.u1HitPos == 3)
					pos += hitPos;

				Transform tHit = VFXMgr.Instance.GetVFX (cAtkModel.sMissileCrushModelName, pos, cTransform.rotation).transform;

				if (cAtkModel.u1HitPos == 3 || cAtkModel.u1HitPos == 4)
					tHit.SetParent (cTarget.cObject.transform);
			}
		}

		if(eAttackType == ATTACK_TYPE.Range){
			if(bSkill){
				if (cCurSkill != null) {
					if (cCurSkill.cInfo.CheckAllySkill ()) {
						CheckRange (cOwner.cCrew);
					}
					if (cCurSkill.cInfo.CheckEnemySkill ()) {
						CheckRange (cEnemys);
					}
				}
			}else{
				CheckRange(cEnemys);
			}
		}else{
			if(lTargets.Count > 0){
				for(int i=0; i<lTargets.Count; i++){
					SendHit(lTargets[i]);
				}
			}else{
				SendHit(cTarget);
			}
		}
	}

	void CheckRange(BattleCrew tCrew){
		for(int i=0; i<tCrew.acCharacters.Length; i++){
			if(lstHittedChar.FindIndex(cs => cs == tCrew.acCharacters[i]) > -1) continue;
			if(!tCrew.acCharacters[i].isDead){
				Vector3 bombPos = transform.position;
				Transform targetTrans = tCrew.acCharacters[i].cObject.transform;
				
				float tempDist = Vector3.Magnitude(bombPos - targetTrans.position);
				
				if(tempDist - tCrew.acCharacters[i].totalRad <= cAtkModel.u2ThrowRange){
					SendHit(tCrew.acCharacters[i]);
					lstHittedChar.Add(tCrew.acCharacters[i]);
				}
			}
		}
	}
	
	void SendHit(BattleCharacter tTarget){
		if (tTarget == null) {
			DeleteObject();
			return;
		}

		if(tTarget.isDead){
			DeleteObject();
			return;
		}

		if(cTarget.cTarget == null || cTarget.cTarget.isDead){
			if(cOwner.iTeamIdx != cTarget.iTeamIdx) cTarget.MoveToTarget(cOwner);
		}

		if (tTarget.iTeamIdx != cOwner.iTeamIdx) {
			//int tiEvasPer = (int)(tTarget.cBattleStatus.u2Agility * tTarget.cCharacter.cClass.cEvasionFactor.Random
			float tfEvasPer = tTarget.GetEvaPer();

			int rand = (int)Random.Range(0,100);

			if(tfEvasPer > rand){
				if(tTarget.iTeamIdx == 0) cOwner.cBattle.battleUIMgr.DuplicateDmg (tTarget.GetBodyPos(), TextManager.Instance.GetText("battle_direction_eva"), BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_HERO);
				else if(tTarget.iTeamIdx == 1) cOwner.cBattle.battleUIMgr.DuplicateDmg (tTarget.GetHeadPos(1f, 0.3f), TextManager.Instance.GetText("battle_direction_eva"), BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_MONSTER);
				DeleteObject();
				return;
			}
			
			float tfEneDef = 0;
			int tiTotalDmg = 0;

			if(!bIgnore){
				tfEneDef = tTarget.GetDefPer(iAttackElement, cOwner.cCharacter.cLevel.u2Level);
			}
				
			float bonusPer = iChainBonus;
			float per = 100;
			int Dmg = iDamage;
			if(bSkill){
				per = 0;
				if(cCurSkill.cInfo.bIgnoreDef) tfEneDef = 0f;
				per += cCurSkill.cInfo.u2DamagePercent;
				per += cCurSkill.cInfo.fPercentLvUpBonus*(cCurSkill.u2Level-1);
				if (bonusPer > 0) {
					per *= bonusPer / 100f;
				}
				Dmg = (int)((float)iDamage*(per/100f));
			}

			if (tfEneDef >= 100f)
				tiTotalDmg = 0;
			else
				tiTotalDmg = (int)((Dmg*(100f-tfEneDef))/100f);

			if(bSkill){
				if(cCurSkill.cInfo.u1Element == 5) tiTotalDmg = (int)((float)tiTotalDmg * GetElementBonus(cOwner.u1Element, tTarget.u1Element)); 
				else tiTotalDmg = (int)((float)tiTotalDmg * GetElementBonus(cCurSkill.cInfo.u1Element, tTarget.u1Element)); 
			}else{
				tiTotalDmg = (int)((float)tiTotalDmg * GetElementBonus(cOwner.u1Element, tTarget.u1Element)); 
			}

			if(per > 0){
				Vector3 dir = Vector3.zero;

				dir = tTarget.cObject.transform.position - cOwnerTrans.position;

				if(tiTotalDmg < 0) tiTotalDmg = 0;
				else{
					if(cAtkModel.u2AttackPower > tTarget.cCharacter.cClass.u2AttackReg){
						tTarget.CheckAttackCancel(dir);
					}else{
						if(cAtkModel.bKnockback) tTarget.CheckAttackCancel(dir, cAtkModel.fKnockbackDist);
					}
				}

				if(tTarget.GetDamage(tiTotalDmg, bCritcal, iChainBonus, "", cOwner, iAttackElement)){
					if(cAtkModel.sVibratePattern == "LR"){
						cOwner.a_col.SetShakeCam (CameraMove2.ShakeStyle.LeftAndRight, cAtkModel.fVibratePower, (float)cAtkModel.u2VibrateTime /1000f);
					}else if(cAtkModel.sVibratePattern == "TB"){
						cOwner.a_col.SetShakeCam (CameraMove2.ShakeStyle.TopAndBottom, cAtkModel.fVibratePower, (float)cAtkModel.u2VibrateTime /1000f);
					}
				}

				if (tTarget.isDead) {
					cOwner.CheckPassive(14);
				}
				
//				if(eTargetType == TARGET_TYPE.Target){
//					DebugMgr.Log(cOwner.cCharacter.cClass.sName+"->"+cTarget.cCharacter.sName+"("+cTarget.cCharacter.cClass.sName+") Atk="+iDamage+"|Crit="+bCritcal+"|Def="+tiEneDef+"|Dmg="+tiTotalDmg+
//				                          "|Hp="+tTarget.u2HP+"/"+tTarget.cCharacter.cFinalStatus.u2HP+"|AtkSpd="+cOwner.fAtkSpd);
//				}

				if (cAtkModel.sMissileHitModelName != "0") {
					if (GraphicOption.damage_script [Legion.Instance.graphicGrade]) {
						Vector3 pos = tTarget.cObject.transform.position;
						if (cAtkModel.u1HitPos == 1)
							pos += hitPos;
						Transform tHit = VFXMgr.Instance.GetVFX (cAtkModel.sMissileHitModelName, pos, GetRandomRot ()).transform;
						if (tHit.GetComponent<AudioSource> () != null)
							tHit.GetComponent<AudioSource> ().volume = SoundManager.Instance.GetcEffPlayer ().volume;
					}
				} else {
					if (GraphicOption.damage_basic [Legion.Instance.graphicGrade]) {
						Vector3 pos = tTarget.cObject.transform.position;
						if (cAtkModel.u1HitPos == 1)
							pos += hitPos;
						Transform tHit = VFXMgr.Instance.GetVFX ("/Common/Hit", pos, GetRandomRot ()).transform;
						if (tHit.GetComponent<AudioSource> () != null)
							tHit.GetComponent<AudioSource> ().volume = SoundManager.Instance.GetcEffPlayer ().volume;
					}
				}
			}
		}else{
			if (cAtkModel.sMissileHitModelName != "0") {
				if (GraphicOption.damage_script [Legion.Instance.graphicGrade]) {
					Vector3 pos = tTarget.cObject.transform.position;
					if (cAtkModel.u1HitPos == 1)
						pos += hitPos;
					Transform tHit = VFXMgr.Instance.GetVFX (cAtkModel.sMissileHitModelName, pos, GetRandomRot ()).transform;
					if (tHit.GetComponent<AudioSource> () != null)
						tHit.GetComponent<AudioSource> ().volume = SoundManager.Instance.GetcEffPlayer ().volume;
				}
			}
			if(cCurSkill != null){
				//DebugMgr.Log(cOwner.cCharacter.cClass.sName+" : "+cTarget.cCharacter.cClass.sName);
				if(cCurSkill.cInfo.u2HealPercent > 0){

					float bonusPer = iChainBonus;
					float per = 0;
					int Dmg = iDamage;
					if(bSkill){
						per += cCurSkill.cInfo.u2HealPercent;
						per += cCurSkill.cInfo.fPercentLvUpBonus*(cCurSkill.u2Level-1);
						if (bonusPer > 0) {
							per *= bonusPer / 100f;
						}
						Dmg = (int)((float)iDamage*(per/100f));
					}

					tTarget.GetHeal(Dmg, iChainBonus);
				}
			}
		}

		if(!bSkill){
			if(!cAtkModel.bPenetrate){
				if(fRemain == 0) DeleteObject();
			}
		}else{
			if(!tTarget.isDead) Effect(tTarget);

			if(!cAtkModel.bPenetrate){
				if(fTick <= 0){
					DeleteObject();
				}
			}
		}
	}

	Quaternion GetRandomRot(){
		return Quaternion.Euler(new Vector3(0,Random.Range(0,360),0));
	}

	float GetElementBonus(byte myEle, byte targetEle){
		if(myEle < 2 || targetEle < 2) return 1f;

		if (myEle == 2) {

			if(targetEle == 3) return 0.8f;
			else if(targetEle == 4) return 1.5f;

		}else if (myEle == 3) {

			if(targetEle == 4) return 0.8f;
			else if(targetEle == 2) return 1.5f;

		}else if (myEle == 4) {

			if(targetEle == 2) return 0.8f;
			else if(targetEle == 3) return 1.5f;
		}

		return 1f;
	}

	public void DeleteObject(){
		if (bDest)
			return;
		
		bDest = true;
		cOwner.cBattle.cCont.DeleteMissile(this);
		if(cModelObj != null){
			cModelObj.parent = null;
			cModelVFX.SetEmit(false);
			Destroy(cModelObj.gameObject, 1.0f);
		}
		Destroy(gameObject);
	}

	void Effect(BattleCharacter tTarget)
	{
		if(bEffect) return;

		if(fTick > 0.0f) bEffect = true;

		if (tTarget.isDead)
			return;

		if(cCurSkill.cInfo.u1ActSituationPercentApply == 2){
			float rand = Random.Range(0f,100f);

			float bonusPer = cCurSkill.cInfo.fActPerLvUpBonus*(cCurSkill.u2Level-1);

			if(rand >= cCurSkill.cInfo.u2ActSituationPercent+bonusPer){
				return;
			}
		}

//		if(cCurSkill.cInfo.fPush != 0){
//			Vector3 dir = tTarget.cObject.transform.position - transform.position;
//			tTarget.SetKnockBack(Vector3.Normalize(dir), cCurSkill.cInfo.fPush);
//		}

		//DebugMgr.Log("Effect");

		float bonusTime = cCurSkill.cInfo.fCondTimeLvUpBonus*(cCurSkill.u2Level-1);

		if (tTarget.iTeamIdx == cOwner.iTeamIdx) {
			if (cCurSkill.cInfo.u1Target == SKILL_TARGET_TYPE.Self || cCurSkill.cInfo.u1Target == SKILL_TARGET_TYPE.Self_And_Enemy)
				return;

			if(cCurSkill.cInfo.cBuff != null)
				tTarget.GetComponent<ConditionComponent>().Spell(cCurSkill.cInfo.cBuff, cCurSkill.cInfo.u1BasicElement, bonusTime, cOwner);
		}else{
			if(tTarget.bAvoid) return;

			if(cCurSkill.cInfo.cDebuff != null)
				tTarget.GetComponent<ConditionComponent>().Spell(cCurSkill.cInfo.cDebuff, cCurSkill.cInfo.u1BasicElement, bonusTime, cOwner);
		}
	}
}
