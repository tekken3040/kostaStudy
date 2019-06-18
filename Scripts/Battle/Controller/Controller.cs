using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour {

	public UIController cUIController;
	public AIController cAIController;
	ReplayController cReplayController;
	public Battle cBattle;
	
	bool bPlay = false;
	public bool bAuto = false;
	public bool bReserve = false;
	float fPlayTime = 0.0f;

	//tempVar
	float fPathFindRange = 0.45f;
	//float fPathFindAngle = 45f;
	float fWayAngle = 90f;
	float fWayDist = 0.05f;
	int tempCount = 0;
	
	bool bSetUI = false;

	List<Missile> lstMissiles = new List<Missile>();

	public bool bDirection = false;
	float fSkillDirectionTime = 1.0f;
	BattleCharacter cSkillOwner;
	GameObject cSkillCam;

	bool bJoyStick = false;

	// Use this for initialization
	void Awake () {
		cUIController = gameObject.AddComponent<UIController>();
		cUIController.cCont = this;
		cAIController = gameObject.AddComponent<AIController>();
		cAIController.cCont = this;
		cSkillCam = GameObject.FindGameObjectWithTag("SkillCam");
	}

	public bool GetJoystick(){
		return bJoyStick;
	}

	public void SetUI(){
		bSetUI = true;
		cBattle.battleUIMgr.SetJoyStick (bJoyStick);
	}

	public void StartBattle(){
		bPlay = true;
	}

	public void EndBattle(){
		bPlay = false;
	}

	public bool isPlay(){
		return bPlay;
	}

	public void ChangeAuto(bool bVal){
		if(bVal){
			bAuto = true;
			if (Legion.Instance.SelectedStage.u2ID != Tutorial.TUTORIAL_STAGE_ID) {
				PlayerPrefs.SetInt ("bAuto", 1);
			}
		}else{
			bAuto = false;
			if (Legion.Instance.SelectedStage.u2ID != Tutorial.TUTORIAL_STAGE_ID) {
				PlayerPrefs.SetInt ("bAuto", 0);
			}
		}
		Legion.Instance.bAuto = bAuto;
	}

	void Update () {
		UpdateValues ();
		UpdateInput ();
		UpdateBattleChar ();
	}

	void FixedUpdate(){
		//JumpBattleChar ();
	}

	void UpdateValues () {
		fPlayTime += Time.deltaTime;
		if (bDirection) {
			fSkillDirectionTime -= Time.deltaTime;
			if (fSkillDirectionTime < 0) {
				fSkillDirectionTime = 0f;
				EndSkillDirection ();
			}
		}
	}

	void UpdateInput () {
		if (bPlay) {
			cAIController.UpdateSkillAI(bAuto, bReserve);
		}
	}

	public void PauseGame(){
		cBattle.eBattleState = Battle.BATTLE_STATE.Pause;
		bDirection = true;
		bPlay = false;

//		Time.timeScale = 0.0f;
		
		foreach (Missile comp in lstMissiles) {
			comp.PlayVFX(false);
			comp.enabled = false;
		}
		
		for(int i=0; i<2; i++){
			for(int j=0; j<cBattle.acCrews[i].acCharacters.Length; j++){
				cBattle.acCrews[i].acCharacters[j].Pause();
			}
		}
	}

	public void ResumeGame(){
		cBattle.eBattleState = Battle.BATTLE_STATE.Battle;
		bDirection = false;
		bPlay = true;

//		if(bAuto){
//			Time.timeScale = 2.0f;
//		}else{
//			Time.timeScale = 1.0f;
//		}
		
		foreach (Missile comp in lstMissiles) {
			comp.enabled = true;
			comp.PlayVFX(true);
		}
		
		for(int i=0; i<2; i++){
			for(int j=0; j<cBattle.acCrews[i].acCharacters.Length; j++){
				bool bOwner = false;
				if(cBattle.acCrews[i].acCharacters[j] == cSkillOwner) bOwner = true;
		
				cBattle.acCrews[i].acCharacters[j].Resume(bOwner);
			}
		}
	}

	public void SkillDirection(BattleCharacter owner){
//		bDirection = true;
		//cBattle.cCameraMove2.SetSkillCam("TestSkillCam");
//		fSkillDirectionTime = 1f;
//		cSkillOwner = owner;
//
//		foreach (Missile comp in lstMissiles) {
//			comp.PlayVFX(false);
//			comp.enabled = false;
//		}
//
//		for(int i=0; i<2; i++){
//			for(int j=0; j<cBattle.acCrews[i].acCharacters.Length; j++){
//				BattleCharacter tBattleChar = cBattle.acCrews[i].acCharacters[j];
//				if(owner == tBattleChar){
//					tBattleChar.PlaySkillPre();
//
//					SkinnedMeshRenderer[] srenderers = tBattleChar.cObject.GetComponentsInChildren<SkinnedMeshRenderer>();
//					foreach(SkinnedMeshRenderer obj in srenderers){
//						obj.gameObject.layer = 8;
//					}
//					MeshRenderer[] renderers = tBattleChar.cObject.GetComponentsInChildren<MeshRenderer>();
//					foreach(MeshRenderer obj in renderers){
//						obj.gameObject.layer = 8;
//					}
//				}else{
//					tBattleChar.Pause();
//				}
//			}
//		}

//		if (owner.cSkills.useMana == 100) {
//			cBattle.battleUIMgr.PlayUltDir(owner.cCharacter.cClass.sName);
//		}else{
//			cBattle.battleUIMgr.PlaySkillDir(owner.cSkills.cCurSkillInfo.sName);
//		}
	}

	public void EndSkillDirection(){
//		bDirection = false;
		
//		foreach (Missile comp in lstMissiles) {
//			comp.enabled = true;
//			comp.PlayVFX(true);
//		}
//
//		for(int i=0; i<2; i++){
//			for(int j=0; j<cBattle.acCrews[i].acCharacters.Length; j++){
//				bool bOwner = false;
//				if(cBattle.acCrews[i].acCharacters[j] == cSkillOwner) bOwner = true;
//
//				cBattle.acCrews[i].acCharacters[j].Resume(bOwner);
//			}
//		}
//
//		SkinnedMeshRenderer[] srenderers = cSkillOwner.cObject.GetComponentsInChildren<SkinnedMeshRenderer>();
//		foreach(SkinnedMeshRenderer obj in srenderers){
//			obj.gameObject.layer = 0;
//		}
//		MeshRenderer[] renderers = cSkillOwner.cObject.GetComponentsInChildren<MeshRenderer>();
//		foreach(MeshRenderer obj in renderers){
//			obj.gameObject.layer = 0;
//		}
//
//		cSkillOwner = null;
//		cBattle.battleUIScript.StopDir();
	}
	
	public bool UseSkill (int teamIdx, int charIdx, int skillIdx) {

		if(!bPlay) return false;
		if(!cBattle.acCrews[teamIdx].acCharacters[charIdx].CheckCanSpell(skillIdx)) return false;

		if(teamIdx == 0){
			return cBattle.acCrews[0].acCharacters[charIdx].aeSkill[skillIdx](cBattle.acCrews[0], cBattle.acCrews[1]);
		}else if(teamIdx == 1){
			return cBattle.acCrews[1].acCharacters[charIdx].aeSkill[skillIdx](cBattle.acCrews[1], cBattle.acCrews[0]);
		}

		return false;
	}

	public bool UseSkill (BattleCharacter owner, int skillIdx) {

		if(!bPlay) return false;
		if(!owner.CheckCanSpell(skillIdx)) return false;

		if(owner.iTeamIdx == 0){
			return owner.aeSkill[skillIdx](cBattle.acCrews[0], cBattle.acCrews[1]);
		}else if(owner.iTeamIdx == 1){
			return owner.aeSkill[skillIdx](cBattle.acCrews[1], cBattle.acCrews[0]);
		}

		return false;
	}

	public bool UseSkillCompulsion (int teamIdx, int charIdx, int skillIdx) {

		if(cBattle.acCrews[teamIdx].acCharacters[charIdx].isDead) return false;
		if(cBattle.acCrews[teamIdx].acCharacters[charIdx].aeSkill.Length <= skillIdx){ return false; }
		if(cBattle.acCrews[teamIdx].acCharacters[charIdx].aeSkill[skillIdx] == null){ return false; }
		if(cBattle.acCrews[teamIdx].acCharacters[charIdx].bJump) return false;

		if(teamIdx == 0){
			return cBattle.acCrews[0].acCharacters[charIdx].aeSkill[skillIdx](cBattle.acCrews[0], cBattle.acCrews[1]);
		}else if(teamIdx == 1){
			return cBattle.acCrews[1].acCharacters[charIdx].aeSkill[skillIdx](cBattle.acCrews[1], cBattle.acCrews[0]);
		}
		
		return false;
	}

	void JumpBattleChar(){
		if(bPlay){
			for(int i=0; i<2; i++){
				for(int j=0; j<cBattle.acCrews[i].acCharacters.Length; j++){
					BattleCharacter tBattleChar = cBattle.acCrews[i].acCharacters[j];

					if(tBattleChar.isDead) continue;

					if(tBattleChar.bJump) tBattleChar.JumpCharacter();
				}
			}
		}
	}

	void UpdateBattleChar () {
		if(bPlay){
			for(int i=0; i<2; i++){
				for(int j=0; j<cBattle.acCrews[i].acCharacters.Length; j++){
					BattleCharacter tBattleChar = cBattle.acCrews[i].acCharacters[j];

					if (tBattleChar == null)
						continue;

					if (cBattle.acCrews [i == 0 ? 1 : 0] == null) {
						continue;
					}

					bool bDebug = false;
					
					//if(i+j == 0) bDebug = true;

					tBattleChar.UpdateDamageColor ();

					if(tBattleChar.isDead) continue;
					if(tBattleChar.eCharacterState == CHAR_STATE.Pause) continue;

					tBattleChar.cSkills.Update();
					tBattleChar.cCondis.Update();

					if(tBattleChar.bSkill){
						tBattleChar.CheckSkillEnd();
						continue;
					}

					//if(tBattleChar.CheckJoystick()) continue;

					if(i==0){
						if (tBattleChar.cTarget == null) {
							FindTarget(tBattleChar, cBattle.acCrews[ i == 0 ? 1:0 ], bDebug);
							continue;
						}else if(tBattleChar.cTarget.cObject == null){
							continue;
						}else if(!tBattleChar.cTarget.cObject.activeSelf){
							FindTarget(tBattleChar, cBattle.acCrews[ i == 0 ? 1:0 ], bDebug);
							continue;
						}
						else if(tBattleChar.cTarget.isDead && (tBattleChar.eCharacterState != CHAR_STATE.Attacking && tBattleChar.eCharacterState != CHAR_STATE.AttackWait))
						{
							tBattleChar.SetAttackWait();
							continue;
						}
					}else{
						if(tBattleChar.cTarget == null || tBattleChar.cTarget.isDead){
							FindTarget(tBattleChar, cBattle.acCrews[ i == 0 ? 1:0 ], bDebug);
							continue;
						}
					}

					if(bDebug) DebugMgr.Log(tBattleChar.eCharacterState);

					switch(tBattleChar.eCharacterState){
					case CHAR_STATE.Idle:
						FindTarget(tBattleChar, cBattle.acCrews[ i == 0 ? 1:0 ], bDebug);
						break;

					case CHAR_STATE.Move:
						if (tBattleChar.fWaitTime > 0f) {
							if (bDebug)
								DebugMgr.Log (tBattleChar.fWaitTime + "->");
							tBattleChar.fWaitTime -= Time.deltaTime;
							if (tBattleChar.fWaitTime < 0)
								tBattleChar.fWaitTime = 0;
						}


						if (tBattleChar.CheckPinch ())
							return;

						if(!MoveToTarget(tBattleChar, cBattle.acCrews[ i == 0 ? 1:0 ])){
							if(bDebug) DebugMgr.Log("MoveToTarget False");
							if(tBattleChar.bWait){
								if(bDebug) DebugMgr.Log("bWait True");
								tBattleChar.bWait = false;
								tBattleChar.nav.enabled = true;
							}else{
								if(bDebug) DebugMgr.Log(tBattleChar.cTarget.cObject.name);
								if(tBattleChar.cCondis.CheckCondition(CONDITION_TYPE.ImMovable)) tBattleChar.nav.speed = 0f;
								else tBattleChar.nav.speed = (tBattleChar.lastMoveSpd/1000f)*3f;

								if(bDebug) DebugMgr.Log(tBattleChar.nav.enabled);
								if(tBattleChar.nav.enabled) tBattleChar.nav.SetDestination (tBattleChar.cTarget.cObject.transform.position);
								else tBattleChar.EnableMove(false);
							}
						}else{
							if(bDebug) DebugMgr.Log("MoveToTarget True");
							if (tBattleChar.fWaitTime <= 0f) tBattleChar.EnableMove(false);
							else tBattleChar.EnableMove(true);
						}
						break; 

					case CHAR_STATE.AttackPre:
						if(tBattleChar.cCondis.CheckCondition(CONDITION_TYPE.CantAttack)) continue;

						CheckAttackPre(tBattleChar);
						break;

					case CHAR_STATE.Attack:
//						Attack(tBattleChar, cBattle.acCrews[ i == 0 ? 1:0 ]);
						break;

					case CHAR_STATE.Attacking:
						tBattleChar.Attacking();
//						CheckAttacking(tBattleChar);
						break;

					case CHAR_STATE.AttackWait:
						CheckAttackWait(tBattleChar);
						break;

					case CHAR_STATE.Damage:
						CheckDamageEnd(tBattleChar);
						break;

//					case CHAR_STATE.Rush:
//						Rush(tBattleChar);
//						CheckAttackPre(tBattleChar);
//						break;
//					case CHAR_STATE.KnockBack:
//						Rush(tBattleChar);
//						break;

					case CHAR_STATE.Death:
						//Destroy(tBattleChar.cObject);
						break;

					case CHAR_STATE.Pause:
						//Destroy(tBattleChar.cObject);
						break;

					case CHAR_STATE.MovePortal:
						//Destroy(tBattleChar.cObject);
						break;
					}
				}
			}

			CheckBattleEnd();
		}
	}

	void CheckBattleEnd(){
		int CharCnt = cBattle.acCrews[0].acCharacters.Length;
		for(int i=0; i<cBattle.acCrews[0].acCharacters.Length; i++){
			if(cBattle.acCrews[0].acCharacters[i].isDead){
				CharCnt--;
			}
		}

		if (cBattle.eGameStyle == GameStyle.League) {
			int EnemyCnt = cBattle.acCrews[1].acCharacters.Length;
			for(int i=0; i<cBattle.acCrews[1].acCharacters.Length; i++){
				if(cBattle.acCrews[1].acCharacters[i].isDead){
					EnemyCnt--;
				}
			}

			if(EnemyCnt == 0){
				bPlay = false;
				cBattle.OnBattleEnd(Battle.BATTLE_RESULT_TYPE.KillAll);
				
				for(int i=0; i<cBattle.acCrews[0].acCharacters.Length; i++){
					if(!cBattle.acCrews[0].acCharacters[i].isDead){
						cBattle.acCrews[0].acCharacters[i].PlayWin();
					}
				}
				return;
			}
		} else if (cBattle.eGameStyle == GameStyle.Guild) {
			int EnemyCnt = cBattle.acCrews[1].acCharacters.Length;
			for(int i=0; i<cBattle.acCrews[1].acCharacters.Length; i++){
				if(cBattle.acCrews[1].acCharacters[i].isDead){
					EnemyCnt--;
				}
			}

			if(EnemyCnt == 0){
				bool bEnd = false; 
				if (cBattle.u1EnemyCrewIndex == 2) {
					bEnd = true;
				} else {
					int nextEnemy = 0;
					for (int i = cBattle.u1EnemyCrewIndex + 1; i < GuildInfoMgr.Instance.cEnemyCrews.Length; i++) {
						if (GuildInfoMgr.Instance.cEnemyCrews [i].u1Count > 0) {
							nextEnemy = i;
						}
					}

					if (nextEnemy == 0)
						bEnd = true;
				}
				
				cBattle.battleUIMgr.SetGuildCrewIcon (1, cBattle.u1EnemyCrewIndex, 0);

				if(bEnd) {
					bPlay = false;
					cBattle.OnBattleEnd (Battle.BATTLE_RESULT_TYPE.KillAll);

					for (int i = 0; i < cBattle.acCrews [0].acCharacters.Length; i++) {
						if (!cBattle.acCrews [0].acCharacters [i].isDead) {
							cBattle.acCrews [0].acCharacters [i].PlayWin ();
						}
					}
					return;
				} else {
					bPlay = false;
					cBattle.eBattleState = Battle.BATTLE_STATE.Load;
					cBattle.u1Round++;
					cBattle.u1EnemyCrewIndex++;
					if(GuildInfoMgr.Instance.cEnemyCrews [cBattle.u1EnemyCrewIndex].u1Count == 0) cBattle.u1EnemyCrewIndex++;
					cBattle.SetNewGuildRound (1);
					cBattle.battleUIMgr.SetGuildCrewIcon (1, cBattle.u1EnemyCrewIndex, 1);
				}
			}
		}
		
		if(CharCnt == 0){
			if (cBattle.eGameStyle == GameStyle.Guild) {
				bool bEnd = false; 
				if (cBattle.u1UserCrewIndex == 2) {
					bEnd = true;
				} else {
					int nextUser = 0;
					for (int i = cBattle.u1UserCrewIndex + 1; i < GuildInfoMgr.Instance.cUserCrews.Length; i++) {
						if (GuildInfoMgr.Instance.cUserCrews [i].u1Count > 0) {
							nextUser = i;
						}
					}

					if (nextUser == 0)
						bEnd = true;
				}

				cBattle.battleUIMgr.SetGuildCrewIcon (0, cBattle.u1UserCrewIndex, 0);

				if (bEnd) {
					bPlay = false;
					cBattle.OnBattleEnd (Battle.BATTLE_RESULT_TYPE.AllKilled);

					for (int i = 0; i < cBattle.acCrews [1].acCharacters.Length; i++) {
						if (!cBattle.acCrews [1].acCharacters [i].isDead) {
							cBattle.acCrews [1].acCharacters [i].PlayWin ();
						}
					}
				} else {
					bPlay = false;
					cBattle.eBattleState = Battle.BATTLE_STATE.Load;
					cBattle.u1Round++;
					cBattle.u1UserCrewIndex++;
					if(GuildInfoMgr.Instance.cUserCrews [cBattle.u1UserCrewIndex].u1Count == 0) cBattle.u1UserCrewIndex++;
					cBattle.SetNewGuildRound (0);
					cBattle.battleUIMgr.SetGuildCrewIcon (0, cBattle.u1UserCrewIndex, 1);
				}
			} else {
				bPlay = false;
				cBattle.OnBattleEnd (Battle.BATTLE_RESULT_TYPE.AllKilled);

				for (int i = 0; i < cBattle.acCrews [1].acCharacters.Length; i++) {
					if (!cBattle.acCrews [1].acCharacters [i].isDead) {
						cBattle.acCrews [1].acCharacters [i].PlayWin ();
					}
				}
			}
		}
	}

	public bool FindTarget(BattleCharacter owner, BattleCrew enemys, bool bDebug) {
		if( !owner.CheckJoystickBlock() ) return false;

		if( owner.cTarget != null && (owner.iTeamIdx != owner.cTarget.iTeamIdx) ){
			if(owner.cTarget.isDead){
				if(owner.eCharacterState == CHAR_STATE.Idle || owner.eCharacterState == CHAR_STATE.AttackWait){
					owner.cTarget = null;
				}else if(owner.eCharacterState == CHAR_STATE.Attacking &&
				         (owner.subAnimator.GetCurrentAnimatorStateInfo(0).IsName("Run") || owner.subAnimator.GetCurrentAnimatorStateInfo(0).IsName("Stand")) ){
					owner.cTarget = null;
				}
				return false;
			}
		}

		float dist = 0;
		int idx = -1;

		for(int i=0; i<enemys.acCharacters.Length; i++){
			if(!enemys.acCharacters[i].isDead){
				if(enemys.acCharacters[i].cObject == null) continue;
				float tempDist = Vector3.Magnitude(owner.cObject.transform.position - enemys.acCharacters[i].cObject.transform.position);
				Vector3 targetDir = enemys.acCharacters[i].cObject.transform.position - owner.cObject.transform.position;
				int tempIdx = -1;

				if(!owner.bHero && tempDist > owner.cCharacter.cClass.fViewDistance) continue;
				else{
					if (!owner.bHero) {
						if(owner.FindDelay < 1.0f){
							owner.FindDelay += Time.deltaTime;
							return false;
						}
					}
				}

				if (owner.bHero && cBattle.eGameStyle != GameStyle.League) {
					if(tempDist < dist || dist == 0){
						dist = tempDist;
						owner.cTarget = enemys.acCharacters[i];
					}
				}else{
					for(int j=0; j<owner.cCharacter.cClass.cPriority.asTargetID.Length; j++){
						if(owner.cCharacter.cClass.cPriority.asTargetID[j] == enemys.acCharacters[i].cCharacter.cClass.u2ID){
							tempIdx = j;
							break;
						}
					}

					if(idx == -1 || tempIdx < idx){
						idx = tempIdx;
						if(tempDist < dist || dist == 0){
							dist = tempDist;
							owner.cTarget = enemys.acCharacters[i];
						}
					}
				}
			}
		}

		if(owner.cTarget != null){
			if (!owner.bHero) owner.FindDelay = 0.0f;

			float rad = owner.totalRad + owner.cTarget.totalRad;

			owner.AttackDecision(dist - rad);
			if (!MoveToTarget(owner, enemys))
			{
				owner.SetMove();
				return true;
			}
		}

		return false;
	}

	public GameObject CreateMissileObj(Vector3 pos, string path){
		GameObject tMissile = AssetMgr.Instance.AssetLoad(path + ".prefab", typeof(GameObject)) as GameObject;
		GameObject missileObj = (GameObject)Instantiate(tMissile);
		lstMissiles.Add(missileObj.GetComponent<Missile>());
		missileObj.transform.position = pos;
		return missileObj;
	}

	public bool MoveToTarget(BattleCharacter owner, BattleCrew enemys) {
		if (owner.cTarget == null)
			return true;

		Transform ownerTrans = owner.cObject.transform;
		Transform targetTrans = owner.cTarget.cObject.transform;
		float tempDist = Vector3.Magnitude(ownerTrans.position - targetTrans.position);
		Vector3 targetDir = targetTrans.position - ownerTrans.position;
//		bool angleIn = false;
//		float addDist = 0f;
		ClassInfo ownerClass = owner.cCharacter.cClass;

		float rad = owner.totalRad + owner.cTarget.totalRad;

		if (owner.fSkillUseDist > 0) {
			if (tempDist > owner.fSkillUseDist + rad) {
				return false;
			} else {
				if (UseSkill (owner, owner.u2SavedSkillIdx)) {
					return true;
				} else {
					owner.fSkillUseDist = 0;
					return false;
				}
			}
		}

		if (owner.cCurAtk.u1Type == 1) {
			if(tempDist <= ownerClass.fBasicAttackDist+rad){
				owner.SetAttack();
				return true;
			}
		}else if (owner.cCurAtk.u1Type == 2) {
			if(owner.cCurAtk.cAttackSet.u1AttackType == 1){
				if(owner.CheckTargetInDist(ownerClass.fBasicAttackDist)){
					owner.SetAttack();
					return true;
				}
			}else if(owner.cCurAtk.cAttackSet.u1AttackType == 2){
				if(owner.CheckTargetInDist(ownerClass.fRushAttackDist)){
					owner.SetAttack();
					return true;
				}
			}else if(owner.cCurAtk.cAttackSet.u1AttackType == 3){
				if(owner.CheckTargetInDist(ownerClass.fJumpAttackDist)){
//					DebugMgr.LogError("JumpAttack");
					owner.SetAttack();
					return true;
				}
			}else if(owner.cCurAtk.cAttackSet.u1AttackType == 4){
				if(owner.CheckTargetInDist(ownerClass.fRushAttackDist)){
					owner.SetAttack();
					return true;
				}
			}else if(owner.cCurAtk.cAttackSet.u1AttackType == 5){
					if(owner.CheckTargetInDist(ownerClass.fBasicAttackDist)){
					owner.SetAttack();
					return true;
				}
			}
		}
		
		//comment
//		if(Vector3.Angle(ownerTrans.forward, targetDir) <= owner.cCurAtkModel.u2AttackAngle/2){
//			if(owner.cCurAtkModel.u2AttackRange+addDist > tempDist){
//
//				if(!owner.bSkill && owner.fWaitTime > 0f){
//					owner.SetAttackWait();
//				}else{
//					ownerTrans.LookAt(targetTrans.position);
//					SaveTargets(owner, enemys);
//					owner.ActiveAttack();
//				}
//
//				return true;
//			}
//		}

		return false;
	}



	public void DeleteMissile(Missile component){
		lstMissiles.Remove(component);
	}

	void CheckAttackPre(BattleCharacter owner){
		owner.fCreatTime -= Time.deltaTime;
		owner.fLastStateTime -= Time.deltaTime;

		if (owner.fCreatTime <= 0.0f) {
			owner.eCharacterState = CHAR_STATE.Attack;
		}
	}
	
	void CheckAttackWait(BattleCharacter owner) {
		owner.fWaitTime -= Time.deltaTime;
		//owner.LookAtTargetSlow();
		if(owner.fWaitTime <= 0f){
			owner.EndAttackWait();
		}
	}

	void CheckDamageEnd(BattleCharacter owner) {
		owner.fLastStateTime -= Time.deltaTime;
		owner.fWaitTime -= Time.deltaTime;
		
		if(owner.fLastStateTime <= 0.0f){
			owner.SetIdle(false);
		}
	}

	//DevelopmentFunc
	public void DebugCharPos(){
		if(!bSetUI) return;
		if(Time.timeScale == 1.0f) return;

		string total = "";
		for (int i=0; i<2; i++) {
			for (int j=0; j<cBattle.acCrews[i].acCharacters.Length; j++) {
				BattleCharacter tBattleChar = cBattle.acCrews [i].acCharacters [j];
				total += " / "+tBattleChar.cCharacter.sName+":"+tBattleChar.cObject.transform.position.ToString();
			}
		}
		total = total.Remove(0,3);

//		GameObject.FindGameObjectWithTag("UIMgr").GetComponent<BattleUIMgr>().Log(total);
	}

}
