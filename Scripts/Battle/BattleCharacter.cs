using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum CHAR_STATE
{
	Idle,
	Move,
	AttackPre,
	Attack,
	Attacking,
	AttackWait,
	Damage,
	Skill,
	Cast,
	Death,
	Rush,
	KnockBack,
	Pause,
	Win,
	MovePortal
}

public class BattleCharacter : ZObject
{
	private string sRootBoneName = "Bip01";
	private string sCenterBoneName = "bip01-spine1";

	public Battle cBattle;
	public Character cCharacter;
	public int iTeamIdx;
	public int iCharIdx;
	public BattleCrew cOwnCrew;
	public Status cBattleStatus;
	public UInt32 u4HP;
	public int iTotalDmg;
	
	public CHAR_STATE eCharacterState;
	private CHAR_STATE eBeforeState;
	public BattleCharacter cTarget;
	public BattleCharacter cAttacker;
	public List<BattleCharacter> lTargets = new List<BattleCharacter>();
	public float fAtkSpd;
	public float fAnimSpd;
	public float fLastStateTime;
	public float fCreatTime;
	public float fWaitTime;
	public float fSkillUseDist;
	public int u2SavedSkillIdx = 0;
	public int u2LastSkillIdx = 0;
	public int u2AtkIdx = -1;
	public bool bNextCheck = false;

	public ConditionComponent cCondis;
	public BattleSkillComponent cSkills;
	public AttackModelInfo cCurAtkModel;
	public float cCurAtkFactor;
	public Dictionary<string, Transform> boneData;
	public Dictionary<string, Transform> changeBoneData;

	public Vector3 v3Center;
	public Vector3 v3Bip;

	public SkinnedMeshRenderer mainMat;

	bool bAnimColCreate = false;

	private GameObject _charObject;
	public GameObject cObject
	{
		get
		{
			return _charObject;
		}

		set
		{
			_charObject = value;

			fDiameter = cCharacter.cClass.fDiameter;
			totalDiameter = cCharacter.cClass.fDiameter*cCharacter.cClass.fScale;
			totalRad = totalDiameter / 2f;

			SetSocketData();

			audio = cObject.GetComponent<AudioSource>();
            audio.volume = SoundManager.Instance.GetcEffPlayer().volume;
			//DebugMgr.Log(cCharacter.sName + "animator : " + subAnimator.Length);
			subAnimator = _charObject.transform.FindChild("Animator").GetComponentInChildren<Animator>();
			subAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

			if (_charObject.transform.FindChild ("Animator").GetComponentInChildren<AnimEvent> () == null) {
				subAnimator.gameObject.AddComponent<AnimEvent> ().InitHead (this);
			} else {
				subAnimator.gameObject.GetComponent<AnimEvent> ().InitHead (this);
			}

			if(Bip01 == null){
				Bip01 = subAnimator.transform.GetChild(0);
			}

			nav = _charObject.GetComponent<NavMeshAgent>();
			obs = _charObject.GetComponent<NavMeshObstacle>();
//			rig = _charObject.transform.FindChild("Collider").GetComponent<Rigidbody>();
			rig = _charObject.GetComponent<Rigidbody>();
//			m_col = _charObject.transform.FindChild("Collider").GetComponent<MeshCollider>();

			mats = cObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			mats2 = cObject.GetComponentsInChildren<MeshRenderer>();

			_charObject.transform.localScale = Vector3.one * cCharacter.cClass.fScale;

//			m_col.transform.localScale = new Vector3(fDiameter, 0.2f, fDiameter);
//			m_col.transform.localPosition = Vector3.up * 0.1f;

			obs.radius = fDiameter*0.2f;
			obs.height = fDiameter*0.6f;
			obs.center = new Vector3(0f, fDiameter*0.4f, 0f);

			InitDissolveColor ();
		}
	}

	public float fDiameter;
	public float totalDiameter;
	public float totalRad;

	public Animator subAnimator;
	public Animator changeAnimator;

	// temp var for phase
	public Vector3 lastPos;
	public float moveTime = 0.15f;
	public float baseMoveSpd;
	public float moveSpd;
	public float lastMoveSpd{
		get { return baseMoveSpd*cCondis.GetMoveSpdPer(); }
	}

	public int moveRandDir = 0;
	public BattleCharacter collisionChar;
	public UInt16 collisionCount = 0;

	public bool bArrived;
	public bool bAttack;
	public bool bSkill;
	public bool bEffect;
	public bool bChange = false;
	public bool isDead = false;

	//Passive
	public bool bIgnoreDef = false;
	public UInt16 u2AddDamage = 0;
	public bool bAttackCrit = false;
	public bool bSkillCrit = false;
	
	private bool bFirstAtk = false;
	private bool bFirstDmg = false;

	private float fSpdScale = 1.0f;

	public DropBox[] acDropBoxs = null;

	public float fKnockbackDist;
	public Vector3 fKnockbackDir;

	private Character originalChar;

	public NavMeshAgent nav;
	public NavMeshObstacle obs;
	public bool bWait;

	public Rigidbody rig;
//	MeshCollider m_col;
	AudioSource audio;

	public AnimCollider a_col{
		get { 
			if (cAnimCol == null) {
				boneData.Clear ();
				bAnimColCreate = false;
				SetSocketData ();
			}
			return cAnimCol; 
		}
		set { cAnimCol = value; }
	}
	private AnimCollider cAnimCol;
	bool bAnimColInit = false;
	private Vector3 lastMovePos;
	private Vector3 beforeBipPos;
	private Vector3 animBeforePos;
	private Vector3 animChangePos;
	private Vector3 animMovePos;

	public ClassInfo.AttackInfo cCurAtk;

	public bool bTargetDead = false;

	SkinnedMeshRenderer[] mats;
	MeshRenderer[] mats2;
	public float matColorChangeTime = 0.0f;
	public float matDissolveValue = 1.0f;
	public float curMatColor = 0.0f;

	float fNextAttackPer = 0.0f;
	public Transform Bip01;
	Transform CeneterBone;

	public bool bJump = false;
	float fJumpSpd = 0.1f;
	bool bLand = false;
	float fLandSpd = 0.1f;

	public bool bAvoid = false;

	public BattleSkillAI[] acAI;

	public Byte u1Element;
	public bool bHero = false;
	public bool bSupport = false;

	public float FindDelay = 0.0f;

	public int iChainBonus;

	bool bJoyStick = false;
	public float fFirstAngle = 0;

	Vector3 lastTargetPos;
	bool bMovePause;
	 
	public BattleCharacter(Character cChar, Battle cParent, BattleCrew crew, bool bSup = false)
    {
		cBattle = cParent;
		cOwnCrew = crew;
		bSupport = bSup;
		cCondis = AddComponent<ConditionComponent>(null);
		cSkills = AddComponent<BattleSkillComponent>(cChar);	// call after SetBaseCharater()

		SetBaseCharacter(cChar);
		boneData = new Dictionary<string, Transform>();
		changeBoneData = new Dictionary<string, Transform>();

		if(cBattle != null) bJoyStick = cBattle.cCont.GetJoystick();
	}

    public void SetBaseCharacter(Character cChar)
    {
        cCharacter = cChar;
		originalChar = cCharacter;
		cBattleStatus = cChar.cFinalStatus;
		//u4HP = cBattleStatus.u4HP;
        u4HP = Convert.ToUInt32(cBattleStatus.GetStat(1));

		if (cCharacter is Hero) {
			u1Element = ((Hero)cCharacter).GetHeroElement();
			bHero = true;
		} else {
			u1Element = cCharacter.cClass.u1Element;
		}

		acAI = new BattleSkillAI[cCharacter.cClass.cAIInfo.Length];
		for (int i=0; i<acAI.Length; i++) {
			acAI[i] = new BattleSkillAI();
			if(cCharacter.cClass.cAIInfo[i] != null) acAI[i].cInfo = cCharacter.cClass.cAIInfo[i];
		}

		// u2MP = cBattleStatus.u2MP;
		baseMoveSpd = (float)cCharacter.cClass.u2Speed;
        
		InitVFX();
	}

	public void SetChangeCharacter(Character cChar, float hpPer)
	{
		SetChangeAnimator(true);

		cCharacter = cChar;
		//DebugMgr.Log(cChar.cFinalStatus.u4HP);
        DebugMgr.Log(cChar.cFinalStatus.GetStat(1));
		cBattleStatus = cChar.cFinalStatus;
		//u4HP = (ushort)((float)cBattleStatus.u4HP*hpPer);
        u4HP = (ushort)((float)cBattleStatus.GetStat(1)*hpPer);
		DebugMgr.Log(u4HP);
		baseMoveSpd = (float)cCharacter.cClass.u2Speed;
		
		InitVFX();
		bSkill = false;
		SetIdle(false);
		SetChangeSocketData();
	}

	public void SetOriginalCharacter(float hpPer){
		ModelEnable(true);
		SetChangeAnimator(false);

		cCharacter = originalChar;
		cBattleStatus = cCharacter.cFinalStatus;
		//u4HP = (ushort)((float)cBattleStatus.u4HP*hpPer);
        u4HP = (ushort)((float)cBattleStatus.GetStat(1)*hpPer);
		DebugMgr.Log(u4HP);
		baseMoveSpd = (float)cCharacter.cClass.u2Speed;

		SetIdle(false);
		changeBoneData.Clear();
	} 
	
	public void SetSocketData(){
		//ModelInfo[] models = ClassInfoMgr.Instance.GetInfo(cCharacter.cClass.u2ID).u2BasicEquips;
		Transform[] bones = cObject.transform.GetChild(0).GetChild(0).GetComponentsInChildren<Transform> ();
		foreach (Transform tr in bones) {
			if(!boneData.ContainsKey(tr.name)){
				if(sRootBoneName == tr.name){
					Bip01 = tr;
					v3Center = tr.transform.position - cObject.transform.position;
					v3Bip = tr.transform.localPosition;
				}else if(sCenterBoneName == tr.name){
					AddAnimCollider(tr);
				}
				boneData.Add (tr.name, tr);
			}
		}

		if (CeneterBone == null) {
			AddAnimCollider(cObject.transform.GetChild (0).GetChild (0).GetChild (0));
		}
	}

	void AddAnimCollider(Transform parent){
		if (bAnimColCreate)
			return;

		GameObject cbpObj = new GameObject("AnimColParent");
		GameObject cbObj = cBattle.GetAnimColWithCreate ();
		cbpObj.transform.SetParent(parent);
		cbpObj.transform.rotation = Quaternion.identity;

		cbpObj.transform.localScale = Vector3.one;
		cbpObj.transform.localPosition = Vector3.zero;

		cbObj.transform.SetParent(cbpObj.transform);
		cbObj.transform.rotation = Quaternion.identity;

		CeneterBone = cbObj.transform;

		CeneterBone.localScale = Vector3.one;
		CeneterBone.localPosition = Vector3.zero;

		a_col = CeneterBone.GetComponent<AnimCollider>();
		a_col.SetOwner(this);
		bAnimColCreate = true;
	}

	public void SetChangeSocketData(){
		//ModelInfo[] models = ClassInfoMgr.Instance.GetInfo(cCharacter.cClass.u2ID).u2BasicEquips;
		
		Transform[] bones = cObject.transform.GetChild(0).GetChild(1).GetComponentsInChildren<Transform> ();
		foreach (Transform tr in bones) {
			if(!changeBoneData.ContainsKey(tr.name))
				changeBoneData.Add (tr.name, tr);
		}
	}
	
	void InitVFX(){
//		PreloadInfo cPreVFX = ClassInfoMgr.Instance.GetPreloadInfo(cCharacter.cClass.u2ID);
//
//		for (int i=0; i< cPreVFX.AssetPath.Length; i++) {
//			if(cPreVFX.AssetPath[i] == "0"){
//				break;
//			}else{
//				VFXMgr.Instance.AddItem(cPreVFX.AssetPath[i]);
//			}
//		}
//
//		VFXMgr.Instance.AddItem(cCharacter.cClass.sDeathEffect);
//		VFXMgr.Instance.AddItem(cCharacter.cClass.sCreateEffect);

		for (int i=0; i<cSkills.lstcSkills.Count; i++) {
			if (cSkills.lstcSkills [i].cInfo.cBuff != null) {
				VFXMgr.Instance.AddItem ("/Condition/" + cSkills.lstcSkills [i].cInfo.cBuff.sIcon + "_C");
				VFXMgr.Instance.AddItem ("/Condition/" + cSkills.lstcSkills [i].cInfo.cBuff.sIcon + "_M");
			}
			if (cSkills.lstcSkills [i].cInfo.cDebuff != null) {
				VFXMgr.Instance.AddItem ("/Condition/" + cSkills.lstcSkills [i].cInfo.cDebuff.sIcon + "_C");
				VFXMgr.Instance.AddItem ("/Condition/" + cSkills.lstcSkills [i].cInfo.cDebuff.sIcon + "_M");
			}
		}
	}

	public void PlayAudioClip(AudioClip clip){
		audio.PlayOneShot(clip);
	}
	
	public BattleCrew cCrew{
		get { return cOwnCrew; }
	}

	public BattleCrew cEnemy{
		get { return cBattle.acCrews [1-iTeamIdx]; }
	}

	public void LandCharacter(){
		if (!bLand) {
			SubAnimationPlay ("Appear_Start");
			bLand = true;
		}

		float groundY = CheckGround();

		if (cObject.transform.position.y > groundY) {
			fLandSpd += (9.8f*Time.deltaTime)*Time.deltaTime;
			float finalY = cObject.transform.position.y - fLandSpd;
			if(finalY <= groundY){
				finalY = groundY;
				SubAnimationCrossFade("Appear_End", 0.2f);
				InitAnimPos ();
			}
			cObject.transform.position = new Vector3(cObject.transform.position.x, finalY, cObject.transform.position.z);
		}
	}

	public void JumpStart(){
		fJumpSpd = -0.08f;
		bJump = true;
	}

	public void JumpCharacter(){
		float groundY = CheckGround();
		if (cObject.transform.position.y > groundY) {
			fJumpSpd += (9.8f*Time.deltaTime)*Time.deltaTime;
			float finalY = cObject.transform.position.y - fJumpSpd;
			if(finalY <= groundY){
				finalY = groundY;
				bJump = false;
			}
			cObject.transform.position = new Vector3(cObject.transform.position.x, finalY, cObject.transform.position.z);
		}
	}
	
	public Vector3 GetCreatePos(){
		if(cCurAtkModel.cMissileCreatePos == null) return cObject.transform.position;

		Vector3 pos = cObject.transform.position;

		// DebugMgr.Log(cCharacter.sName+" Create Missile At Socket "+cCurAtkModel.cMissileCreatePos.sSocBone);
		if(bChange){
			if(changeBoneData.ContainsKey(cCurAtkModel.cMissileCreatePos.sSocBone)){
				pos = changeBoneData[cCurAtkModel.cMissileCreatePos.sSocBone].position;
			}
		}else{
			if(boneData.ContainsKey(cCurAtkModel.cMissileCreatePos.sSocBone)){
				pos = boneData[cCurAtkModel.cMissileCreatePos.sSocBone].position;
			}
		}

		return pos;
		//return new Vector3(0, 0.3f, 0f);
	}

	public Transform GetCreateSock(){
		if(cCurAtkModel == null || cCurAtkModel.cMissileCreatePos == null){
			return cObject.transform;
		}
		
		if(bChange){
			if(!changeBoneData.ContainsKey(cCurAtkModel.cMissileCreatePos.sSocBone)){
				return cObject.transform;
			}
		}else{
			if(!boneData.ContainsKey(cCurAtkModel.cMissileCreatePos.sSocBone)){
				return cObject.transform;
			}
		}
		
		if(bChange){
			if(changeBoneData.ContainsKey(cCurAtkModel.cMissileCreatePos.sSocBone)){
				return changeBoneData[cCurAtkModel.cMissileCreatePos.sSocBone];
			}
		}else{
			if(boneData.ContainsKey(cCurAtkModel.cMissileCreatePos.sSocBone)){
				return boneData[cCurAtkModel.cMissileCreatePos.sSocBone];
			}
		}
		
		return boneData[cCurAtkModel.cMissileCreatePos.sSocBone];
	}

	public Transform GetSocketTrans(string sockName){
		if (bChange) {
			if (changeBoneData.ContainsKey (sockName)) {
				return changeBoneData[sockName];
			}
		}else{
			if (boneData.ContainsKey (sockName)) {

				return boneData[sockName];
			}
		}

//		DebugMgr.LogError("Have Not Socket = "+sockName);

		return cObject.transform;
	}

	public Vector3 GetHeadPos(float noneY, float upY){
		if (boneData.ContainsKey ("Bip01-Head")) {

			return new Vector3(boneData["Bip01-Head"].position.x, boneData["Bip01-Head"].position.y+upY, boneData["Bip01-Head"].position.z);
		}

		return cObject.transform.position+new Vector3(0f,noneY,0f);
	}

	public Vector3 GetBodyPos(){
		if (boneData.ContainsKey (sCenterBoneName)) {

			return boneData[sCenterBoneName].position;
		}

		//		DebugMgr.LogError("Have Not Socket = "+sockName);

		return cObject.transform.position+new Vector3(0f,0.5f,0f);
	}
	
	public void Resurrection(string tText, ushort HpPer){
		isDead = false;
		GetHealPer(HpPer, 0, tText);
		VFXMgr.Instance.AddItem("/Condition/revival");
        VFXMgr.Instance.GetVFX("/Condition/revival", GetBodyPos(), Quaternion.identity);

        if (iTeamIdx == 0){
			if (GetHPPer () > 0.25f) {
				cBattle.battleUIMgr.SetRebirth (iCharIdx);
			} else {
				cBattle.battleUIMgr.SetWarning (iCharIdx);
			}
		}
		SetIdle(false);
		a_col.SetEnable(true);
	}

	public bool CheckCanSpell(int skillIdx){
		if(isDead) return false;
		if(aeSkill.Length <= skillIdx){ return false; }
		if(aeSkill[skillIdx] == null) return false;
		if(bSkill){ return false; }
		if(bJump) return false;
		if(cCondis.CheckCondition(CONDITION_TYPE.CantSpell)) return false;
//		if(cCrew.bSaveSkill) return false;
//		if(cCrew.fLegionCoolTime > 0f) return false; 

		return true;
	}

	public void SaveTargets() {
		
		lTargets = new List<BattleCharacter> ();
		
		bool bOur = false;
		bool bEnemy = true;

		if(cSkills.cCurSkillInfo != null){
			bOur = cSkills.cCurSkillInfo.CheckAllySkill();
			bEnemy = cSkills.cCurSkillInfo.CheckEnemySkill();
		}
		
		switch(cCurAtkModel.u1BasicAttackType){
		case 1:
			//CreateTestMissile(owner, owner.cTarget);
			break;
		case 2:
			if(bOur) SaveTargetByRange(cOwnCrew, 1, true);
			if(bEnemy) SaveTargetByRange(cEnemy, 1, true);
			break;
		case 3:
			if(bOur) SaveTargetByRange(cOwnCrew, 2, true);
			if(bEnemy) SaveTargetByRange(cEnemy, 2, true);
			break;
		case 4:
			if(bOur) SaveTargetByRange(cOwnCrew, 3, false);
			if(bEnemy) SaveTargetByRange(cEnemy, 3, false);
			break;
		case 5: case 6:
			//CreateRangeMissile(owner, owner.cTarget, enemys);
			break;
		}
	}

	public bool CheckTargetInDist(float dist){
		if(cTarget == null) return false;
		if (cObject == null || cTarget.cObject == null)
			return false;

		Transform ownerTrans = cObject.transform;
		Transform targetTrans = cTarget.cObject.transform;
		float tempDist = Vector3.Magnitude(ownerTrans.position - targetTrans.position);
		
		float rad = totalRad + cTarget.totalRad;

		if (tempDist <= dist + rad) {
			return true;
		}
			
		return false;
	}

	public bool CheckTargetInDist(BattleCharacter tTarget, float dist){
		if(tTarget == null) return false;
		if (cObject == null || cTarget.cObject == null)
			return false;
		
		Transform ownerTrans = cObject.transform;
		Transform targetTrans = tTarget.cObject.transform;
		float tempDist = Vector3.Magnitude(ownerTrans.position - targetTrans.position);
		
		float rad = totalRad + tTarget.totalRad;
		
		if (tempDist <= dist + rad) {
			return true;
		}

		return false;
	}

	public void AttackDecision(float tDist){

		if (cCharacter.cClass.u1AttackDistance == 1) {
			if (bHero){
				if(tDist < cCharacter.cClass.fAttackDecisionDist){
					GetRandomAttack(cCharacter.cClass.acAttacks);
				}else{
					GetRandomAttack(cCharacter.cClass.acRushAttacks);
				}
			}else{
				GetRandomAttack(cCharacter.cClass.acAttacks);
			}
		}else{
			if (bHero){
				if(tDist > cCharacter.cClass.fAttackDecisionDist){
					GetRandomAttack(cCharacter.cClass.acAttacks);
				}else{
					GetRandomAttack(cCharacter.cClass.acRushAttacks);
				}
			}else{
				GetRandomAttack(cCharacter.cClass.acAttacks);
			}
		}

//		DebugMgr.LogError (cObject.name + "/" + cCurAtk.cAttackSet.u2ID);
	}

	void GetRandomAttack(ClassInfo.AttackInfo[] attacks){
		int totalPer = 0;
		for (int i=0; i<attacks.Length; i++) {
			totalPer += (int)attacks[i].u1Percent;
		}

		if(totalPer == 0){
			GetRandomAttack(cCharacter.cClass.acAttacks);
			return;
		}

		int RandomPer = UnityEngine.Random.Range (0, totalPer);
		totalPer = 0;

		for (int i=0; i<attacks.Length; i++) {
			totalPer += (int)attacks[i].u1Percent;
			if(RandomPer < totalPer){
				cCurAtk = attacks[i];
				return;
			}
		}
	}

	public void Attack(ushort atkID, float fNextPer, bool bAttach) {
		if (cBattle.eGameStyle != GameStyle.AnimTest) {
			if (cBattle.eBattleState != Battle.BATTLE_STATE.Battle)
				return;
		}

		if(cCondis.CheckCondition(CONDITION_TYPE.CantAttack)) return;

		if(eCharacterState == CHAR_STATE.Damage) return;

		if (atkID == 0) {
			DebugMgr.LogError(cObject.name+" AttackID Zero");
			return;
		}

		ClassInfo calClass = cCharacter.cClass;
		float tfAtkSpd = 0;
		
		//tfAtkSpd = (float)calClass.cAttackSpeedRange.fMin + (cBattleStatus.u2Agility * calClass.cAttackSpeedFactor.Random);
        tfAtkSpd = (float)calClass.cAttackSpeedRange.fMin + (cBattleStatus.GetStat(6) * calClass.cAttackSpeedFactor.Random);
		
		if(tfAtkSpd < calClass.cAttackSpeedRange.fMin){
			tfAtkSpd = (float)calClass.cAttackSpeedRange.fMin;
		}else if(tfAtkSpd > calClass.cAttackSpeedRange.fMax){
			tfAtkSpd = (float)calClass.cAttackSpeedRange.fMax;
		}
		
		fAtkSpd = tfAtkSpd/1000f;

		cCurAtkModel = ClassInfoMgr.Instance.GetAttackInfo(atkID);

		if (cCurAtkModel == null) {
			DebugMgr.LogError("Class"+calClass.u2ID+" AttackID "+atkID+" is Not Exist");
			return;
		}

		VFXMgr.Instance.AddItem(cCurAtkModel.sMissileModelName);
		VFXMgr.Instance.AddItem(cCurAtkModel.sMissileCrushModelName);
		VFXMgr.Instance.AddItem(cCurAtkModel.sMissileHitModelName);
		cCurAtkFactor = cCurAtkModel.u2DamagePercent;
		cCurAtkFactor += u2AddDamage;
		u2AddDamage = 0;

		if(cTarget != null){
			SaveTargets();
			if(CheckTargetInDist(cCurAtkModel.u2AttackRange)){
				if (cCurAtkModel.u1BasicAttack == 1) {
					switch(cCurAtkModel.u1BasicAttackType){
					case 1: case 2: case 3: case 4:
						CreateMissile();
						break;
					case 5: case 6:
						CreateRangeMissile(0);
						break;
					}
				}else if(cCurAtkModel.u1BasicAttack == 4){
					CreateRangeMissile(0);
				}else if(cCurAtkModel.u1BasicAttack == 5){
					CreateMultiRangeMissile();
				}else{
					switch(cCurAtkModel.u1BasicAttackType){
					case 1:
						CreateMissile();
						break;
					case 2: case 3: case 4:
						for(int i=0; i<lTargets.Count; i++){
							CreateMissile();
						}
						break;
					case 5: case 6:
						CreateRangeMissile(0);
						break;
					}
				}
			}
		}
		bAttack = true;
		bIgnoreDef = false;

		bSkillCrit = false;
		bAttackCrit = false;

		fNextAttackPer = fNextPer;
		if (fNextPer != 0) {
			bNextCheck = true;
		} else {
			bNextCheck = false;
		}
		bEffect = true;

		if (cBattle.eGameStyle != GameStyle.AnimTest) {
			if (cCharacter.cClass.u1MonsterType == 2) {
				cBattle.battleUIMgr.HideWarning ();
			}
		}
	}

	public void Attacking(){
//		if(cTarget.cObject != null){
//			Quaternion curRot = cObject.transform.rotation;
//			Quaternion nextRot = Quaternion.LookRotation(cTarget.cObject.transform.position - cObject.transform.position);
//			cObject.transform.rotation = Quaternion.RotateTowards(curRot, nextRot, 200f * Time.deltaTime);
//		}
		if(bJump){
			if(!a_col.bStay) cObject.transform.position += cObject.transform.forward*3f*Time.deltaTime;
		}

		if(!bSkill){
			if (bAttack) {
				if (bNextCheck) {
					if (fNextAttackPer > 0) {
						int rand = UnityEngine.Random.Range (0, 100);

						if (fNextAttackPer < rand) {
							SetAttackWait ();
							//SubAnimationPlay ("Stand");
							SubAnimationCrossFade ("Stand", 0.4f);
						} else {
							bAttack = false;
							bNextCheck = false;
						}
					}
				}
			} else {
				if (subAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Stand")) {
					SetAttackWait ();
				}
			}
		}

	}
	
	int[] CalculateDamage() {
		ClassInfo calClass = cCharacter.cClass;
		
		float fDamage = 0;
		int bCrit = 0;
        int tiCritPer = (int)(cBattleStatus.GetStat(7) * calClass.cCriticalFactor.Random
		                      + calClass.cCriticalRange.fMin);

		int iElement = calClass.u1BasicAttackElement;
		
		if(tiCritPer > (int)calClass.cCriticalRange.fMax) tiCritPer = (int)calClass.cCriticalRange.fMax;
		
		if(tiCritPer > UnityEngine.Random.Range(0,100)) bCrit = 1;

		float runeBonus = 0f;

		if(bSkill){
			if(bSkillCrit) bCrit = 1;
			iElement = cSkills.cCurSkillInfo.u1BasicElement;
		}else{
			if(bAttackCrit) bCrit = 1;
		}
		
		if(iElement == 1){
            fDamage = (cBattleStatus.GetStat(2) + cBattleStatus.GetStat(6) * calClass.cPhysicalAttackAgility.Random) * calClass.cPhysicalAttack.Random;
			if(bCrit == 1) fDamage += cBattleStatus.GetStat(7);

			runeBonus = GetRuneVal(RuneType.AD, 1)-cEnemy.GetRuneVal(RuneType.AD, 2);
		}else{
            fDamage = (cBattleStatus.GetStat(3) + cBattleStatus.GetStat(6) * calClass.cMagicAttackAgility.Random) * calClass.cMagicAttack.Random;
			if(bCrit == 1) fDamage += cBattleStatus.GetStat(7);

			runeBonus = GetRuneVal(RuneType.AP, 1)-cEnemy.GetRuneVal(RuneType.AP, 2);
		}

		if (bCrit == 1) {
			if(bSkill){
				if(SkillSkillCrit != null) SkillSkillCrit(cCrew, cEnemy);
			}else{
				if(SkillAttackCrit != null) SkillAttackCrit(cCrew, cEnemy);
			}
		}

		fDamage *= (cCurAtkFactor+runeBonus)/100f;

		if(fDamage < 0f) fDamage = 0f;
		
		return new int[3]{bCrit, (int)fDamage, iElement};
	}

	public float GetEvaPer()
	{
		float totalEva = (float)(cBattleStatus.GetStat (6) * cCharacter.cClass.cEvasionFactor.Random + cCharacter.cClass.cEvasionRange.fMin);

		if(totalEva > cCharacter.cClass.cEvasionRange.fMax) totalEva = (float)cCharacter.cClass.cEvasionRange.fMax;

		if (cCondis.CheckCondition (CONDITION_TYPE.EvaPer))
			totalEva += cCondis.GetEvaPer ();

		return totalEva;
	}

	public float GetDefPer(int atkEle, UInt16 u2EnemyLevel)
	{
		//최소 방어율 + (방어스탯 / (방어스탯 + (레벨상수 * 레벨) ) * 100	
		float totalDef = 0;
		float tiEneDefBonus = 0f;

		switch (atkEle) {
		case 1:
			totalDef = (float)cCharacter.cClass.cDefPerRange.fMin + ((float)cBattleStatus.GetStat (4) / (float)(cBattleStatus.GetStat (4) + (LegionInfoMgr.Instance.u2LevelDamageVal * u2EnemyLevel))) * 100f;
			//tiEneDefBonus = GetRuneVal(RuneType.DEF, 1)-cAttacker.GetRuneVal(RuneType.DEF, 2);

			totalDef += totalDef*(tiEneDefBonus/100f);

			if (totalDef > (float)cCharacter.cClass.cDefPerRange.fMax)
				totalDef = (float)cCharacter.cClass.cDefPerRange.fMax;

			if (cCondis.CheckCondition (CONDITION_TYPE.DefPer))
				totalDef += cCondis.GetDefPer ();

			if (totalDef < 0f)
				totalDef = 0f;
			break;
		case 2:
			totalDef = (float)cCharacter.cClass.cDefPerRange.fMin + ((float)cBattleStatus.GetStat (5) / (float)(cBattleStatus.GetStat (5) + (LegionInfoMgr.Instance.u2LevelDamageVal * u2EnemyLevel))) * 100f;
			//tiEneDefBonus = GetRuneVal(RuneType.REG, 1)-cAttacker.GetRuneVal(RuneType.REG, 2);

			totalDef += totalDef*(tiEneDefBonus/100f);

			if (totalDef > (float)cCharacter.cClass.cDefPerRange.fMax)
				totalDef = (float)cCharacter.cClass.cDefPerRange.fMax;

			if (cCondis.CheckCondition (CONDITION_TYPE.RegPer))
				totalDef += cCondis.GetRegPer ();

			if (totalDef < 0f)
				totalDef = 0f;
			break;
		}

		return totalDef;
	}

	public float GetRuneVal(RuneType eType, Byte u1Increase){
		return cCrew.GetRuneVal(eType, u1Increase);
	}
	
	//shape - 1: Sector(Dispersion), 2: Round, 3: Sector(NonDispersion)
	public void SaveTargetByRange(BattleCrew crew, int shape, bool all) {
		Dictionary<int, float> dTargets = new Dictionary<int, float>();
		//DebugMgr.Log(crew.acCharacters.Length);
		for(int i=0; i<crew.acCharacters.Length; i++){
			//DebugMgr.Log(i +"/"+ crew.acCharacters[i].cCharacter.cClass.sName +"/"+ crew.acCharacters[i].cObject.name +"/"+ crew.acCharacters[i].isDead);
			if(!crew.acCharacters[i].isDead){
				Transform ownerTrans = cObject.transform;
				Transform targetTrans = crew.acCharacters[i].cObject.transform;
				
				float tempDist = Vector3.Magnitude(ownerTrans.position - targetTrans.position);
				Vector3 targetDir = targetTrans.position - ownerTrans.position;
				
				if(Vector3.Angle(ownerTrans.forward, targetDir) > cCurAtkModel.u2AttackAngle/2){
					continue;
				}
				
				if(shape == 2){
					float subDist = Vector3.Magnitude(cTarget.cObject.transform.position - targetTrans.position);
					
					if(CheckTargetInDist(crew.acCharacters[i], cCurAtkModel.u2AttackRange)){
						dTargets.Add(i, subDist);
					}
				}else{
					if(CheckTargetInDist(crew.acCharacters[i], cCurAtkModel.u2AttackRange)){
						dTargets.Add(i, tempDist);
					}
				}
			}
		}
		
		if(all){
			foreach(int t in dTargets.Keys){
				//CreateTestMissile(owner, enemys.acCharacters[t]);
				lTargets.Add(crew.acCharacters[t]);
			}
		}else{
			dTargets.OrderBy(cs => cs.Value);
			int[] acTargets = new int[dTargets.Count];
			int addIdx = 0;
			foreach(int t in dTargets.Keys){
				acTargets[addIdx] = t;
				addIdx++;
			}
			
			int maxAtkCnt = cCurAtkModel.u1MultiTargetAttackNumber;
			
			if(shape == 3){
				for(int i=0; i<maxAtkCnt; i++){
					if(i < acTargets.Length){lTargets.Add(crew.acCharacters[acTargets[i]]);}
				}
			}else{
				int idx = 0;
				for(int i=0; i<acTargets.Length; i++){
					if(idx+1 > maxAtkCnt) idx=0;
					lTargets.Add(crew.acCharacters[acTargets[i]]);
					idx++;
				}
			}
		}
	}
	
	//testCode
	void CreateMissile(){
		cBattle.cCont.CreateMissileObj(GetCreatePos(), "Prefabs/Models/Missile").GetComponent<Missile>().SetInfo(this, cTarget, cEnemy, CalculateDamage(), Missile.ATTACK_TYPE.Single, bIgnoreDef);
	}
	
	void CreateRangeMissile(float angle){
		Missile ms = cBattle.cCont.CreateMissileObj (GetCreatePos (), "Prefabs/Models/Missile").GetComponent<Missile> ();
		ms.SetInfo(this, cTarget, cEnemy, CalculateDamage(), Missile.ATTACK_TYPE.Range, bIgnoreDef);
		ms.SetAngle(angle);
	}

	void CreateMultiRangeMissile(){
		float startAngle = 0;
		if(cCurAtkModel.u1MultiTargetAttackNumber > 1) startAngle = -((cCurAtkModel.u2AttackAngle/2)*(cCurAtkModel.u1MultiTargetAttackNumber-1));
		float turmAngle = cCurAtkModel.u2AttackAngle;

		for(int i=0; i<cCurAtkModel.u1MultiTargetAttackNumber; i++){
			CreateRangeMissile(startAngle+turmAngle*i);
		}
	}

	public void SetDamageColor(float val){
		if (cObject == null)
			return;

		foreach (SkinnedMeshRenderer mat in mats) {
			for(int i=0; i<mat.materials.Length; i++){
				if(mat.materials[i].HasProperty("_Attack_Eff")){
					mat.materials[i].SetFloat("_Attack_Eff",val);
				}
			}
		}
		foreach (MeshRenderer mat in mats2) {
			for(int i=0; i<mat.materials.Length; i++){
				if(mat.materials[i].HasProperty("_Attack_Eff")){
					mat.materials[i].SetFloat("_Attack_Eff",val);
				}
			}
		}
		curMatColor = val;
	}

	void InitDissolveColor(){
		foreach (SkinnedMeshRenderer mat in mats) {
			for(int i=0; i<mat.materials.Length; i++){
				if(mat.materials[i].HasProperty("_DissolveAmount")){
					mat.materials[i].SetFloat("_DissolveAmount",0);
				}
			}
		}
		foreach (MeshRenderer mat in mats2) {
			for(int i=0; i<mat.materials.Length; i++){
				if(mat.materials[i].HasProperty("_DissolveAmount")){
					mat.materials[i].SetFloat("_DissolveAmount",0);
				}
			}
		}
	}

		
	public bool GetDamage(int dmg, bool bCritcal, int chain, string condName, BattleCharacter tAttacker, int basicAtkEle = 0){
		if(cBattle.eGameStyle == GameStyle.AnimTest) return false;

		if(eCharacterState == CHAR_STATE.Win){
			return false;
		}
			
		if (bAvoid) {
			if(iTeamIdx == 0) cBattle.battleUIMgr.DuplicateDmg (cObject.transform.position, TextManager.Instance.GetText("battle_direction_eva"), BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_HERO);
			else if(iTeamIdx == 1) cBattle.battleUIMgr.DuplicateDmg (cObject.transform.position, TextManager.Instance.GetText("battle_direction_eva"), BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_MONSTER);
			if(SkillAvoid != null) SkillAvoid(cCrew, cEnemy);
			return false;
		}

		SetDamageColor(2.0f);
		matColorChangeTime = 0.05f;

		cCondis.CheckDamageEnd();

		if(cCondis.CheckCondition(CONDITION_TYPE.Invincible)){
			DebugMgr.Log ("Invincible " + cCharacter.sName);
			return false;
		}

		cAttacker = tAttacker;

		if(!bFirstDmg){
			if(SkillFirstDamage != null) SkillFirstDamage(cCrew, cEnemy);
			bFirstDmg = true;
		}

		if(SkillDamage != null) SkillDamage(cCrew, cEnemy);

		int shieldDamage = 0;

		if(cCondis.CheckCondition(CONDITION_TYPE.Shield)){
			int shield = cCondis.DecShield(dmg);
			if(shield >= 0) shieldDamage = dmg;
			else shieldDamage = dmg + shield;
		}

		if(dmg == 0){
			if(iTeamIdx == 0) cBattle.battleUIMgr.DuplicateDmg(GetBodyPos(), TextManager.Instance.GetText("battle_direction_guard"), BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_HERO);
			else if(iTeamIdx == 1) cBattle.battleUIMgr.DuplicateDmg(GetHeadPos(1f, 0.3f), TextManager.Instance.GetText("battle_direction_guard"), BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_MONSTER);

			if(SkillGuard != null) SkillGuard(cCrew, cEnemy);

		}else if(dmg > 0){
			if(cCondis.CheckCondition(CONDITION_TYPE.Absorb)){
				GetAbsorb(dmg);
				return true;
			}

			string DmgString = "";
			if(chain > 0) DmgString = TextManager.Instance.GetText("battle_direciton_chain") + "\n";
			if(condName != "") DmgString = condName+"\n";

			if (basicAtkEle == 2) DmgString += TextManager.Instance.GetText ("mag")+" ";

			DmgString += dmg.ToString();

			if(cBattle.eGameStyle != GameStyle.AnimTest)
			{
				if (iTeamIdx == 0) {
					if (bCritcal) {
						cBattle.battleUIMgr.DuplicateDmg (GetBodyPos(), DmgString, BattleUIMgr.DAMAGE_TEXT_TYPE.DAMAGED_HERO, Color.red); //TextManager.Instance.GetText("battle_direction_critical") + "\n"+
					} else {
						cBattle.battleUIMgr.DuplicateDmg (GetBodyPos(), DmgString, BattleUIMgr.DAMAGE_TEXT_TYPE.DAMAGED_HERO);
					}
				} else {
					if (chain < 0) {
						cBattle.battleUIMgr.DuplicateDmg (GetHeadPos(1f, 0.3f), DmgString, BattleUIMgr.DAMAGE_TEXT_TYPE.COMBO_DMG);
					} else if (chain > 0) {
						cBattle.battleUIMgr.DuplicateDmg (GetHeadPos(1f, 0.3f), DmgString, BattleUIMgr.DAMAGE_TEXT_TYPE.CHAIN_DMG);
					} else if (bCritcal) {
						cBattle.battleUIMgr.DuplicateDmg (GetHeadPos(1f, 0.3f), DmgString, BattleUIMgr.DAMAGE_TEXT_TYPE.DAMAGED_MONSTER, Color.red);
					} else {
						cBattle.battleUIMgr.DuplicateDmg (GetHeadPos(1f, 0.3f), DmgString, BattleUIMgr.DAMAGE_TEXT_TYPE.DAMAGED_MONSTER);
					}
				}
			}
		}

		dmg -= shieldDamage;

		if (u4HP < dmg) {
			u4HP = 0;
			if(SkillNonHP != null){
				if(!SkillNonHP(cCrew, cEnemy)) SetDeath();
			}else{
				SetDeath();
			}
		}else{
			iTotalDmg += dmg;
			u4HP -= (ushort)(dmg);
			if(iTeamIdx == 0){
				if(GetHPPer() <= 0.25f) cBattle.battleUIMgr.SetWarning(iCharIdx);
			}
		}

		return true;
	}

	public void UpdateDamageColor()
	{
		if(matColorChangeTime > 0){
			matColorChangeTime -= Time.fixedDeltaTime;
			if(matColorChangeTime <= 0){
				SetDamageColor(0);
			}
		}
	}

	void GetAbsorb(int AbsorbPoint){
		GetHealPoint(AbsorbPoint);
	}

	void GetHealPoint(int healPoint){
		float before = GetHPPer ();

		u4HP = (uint)(u4HP + healPoint);
		//if(u4HP > cBattleStatus.u4HP) u4HP = cBattleStatus.u4HP;
        if(u4HP > cBattleStatus.GetStat(1)) u4HP = Convert.ToUInt32(cBattleStatus.GetStat(1));

		if(iTeamIdx == 0) cBattle.battleUIMgr.DuplicateDmg(GetBodyPos(), healPoint.ToString("#"), BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_HERO);
		else if(iTeamIdx == 1) cBattle.battleUIMgr.DuplicateDmg(GetBodyPos(), healPoint.ToString("#"), BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_MONSTER);

		if(iTeamIdx == 0){
			if (before <= 0.25f && GetHPPer() > 0.25f) {
				cBattle.battleUIMgr.SetDefault(iCharIdx);
			}
		}
	}

	public void GetHeal(int healPoint, int chain){
		if(cBattle.eGameStyle == GameStyle.AnimTest) return;

		float before = GetHPPer ();
		
		u4HP = (UInt32)(u4HP+healPoint);
        if(u4HP > cBattleStatus.GetStat(1)) u4HP = Convert.ToUInt32(cBattleStatus.GetStat(1));

		if(iTeamIdx == 0){
			if (before <= 0.25f && GetHPPer() > 0.25f) {
				cBattle.battleUIMgr.SetDefault(iCharIdx);
			}
		}
		string HealString = "";
		if(chain > 0) HealString = TextManager.Instance.GetText("battle_direction_healchain");
		HealString += TextManager.Instance.GetText("battle_direction_heal")+"\n" + healPoint.ToString("#");

		if (chain > 0)
			cBattle.battleUIMgr.DuplicateDmg (GetBodyPos(), HealString, BattleUIMgr.DAMAGE_TEXT_TYPE.CHAIN_HEAL);
		else {
			if(iTeamIdx == 0) cBattle.battleUIMgr.DuplicateDmg (GetBodyPos(), HealString, BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_HERO);
			else if(iTeamIdx == 1) cBattle.battleUIMgr.DuplicateDmg (GetBodyPos(), HealString, BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_MONSTER);
		}
	}

	public void GetHealPer(float healPer, int chain, string skillName = ""){
		if(cBattle.eGameStyle == GameStyle.AnimTest) return;

		float before = GetHPPer ();
		float healPoint = (cBattleStatus.GetStat(1)*healPer)/100f;
		
		u4HP = (UInt32)(u4HP+healPoint);

		if(u4HP > cBattleStatus.GetStat(1)) u4HP = Convert.ToUInt32(cBattleStatus.GetStat(1));

		if(iTeamIdx == 0){
			if (before <= 0.25f && GetHPPer() > 0.25f) {
				cBattle.battleUIMgr.SetDefault(iCharIdx);
			}
		}
		string HealString = "";
		if (skillName != "")
			HealString = TextManager.Instance.GetText (skillName) + "\n" + healPoint.ToString ("#");
		else {
			if (chain > 0) HealString = TextManager.Instance.GetText ("battle_direction_healchain");
			HealString += TextManager.Instance.GetText ("battle_direction_heal") + "\n" + healPoint.ToString ("#");
		}

		if (chain > 0)
			cBattle.battleUIMgr.DuplicateDmg (GetBodyPos(), HealString, BattleUIMgr.DAMAGE_TEXT_TYPE.CHAIN_HEAL);
		else {
			if (iTeamIdx == 0) {
				if (skillName != "") cBattle.battleUIMgr.DuplicateDmg (GetBodyPos (), HealString, BattleUIMgr.DAMAGE_TEXT_TYPE.REVIVE_HERO);
				else cBattle.battleUIMgr.DuplicateDmg (GetBodyPos (), HealString, BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_HERO);
			}else if(iTeamIdx == 1) cBattle.battleUIMgr.DuplicateDmg (GetBodyPos(), HealString, BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_MONSTER);
		}
	}

	public float GetHPPer(){
		//return (float)u4HP/(float)cBattleStatus.u4HP;
        return (float)u4HP/(float)cBattleStatus.GetStat(1);
	}

	public void Pause(){
		if(cObject == null) return;
		//SetAnimationSpeed(0.0f);
		SubAnimationEnable(false);

		if(eCharacterState == CHAR_STATE.Move){
			if(nav.enabled && !bWait){
				lastTargetPos = nav.pathEndPosition;
				bMovePause = true;
				nav.enabled = false;
			}
		}else if (eCharacterState == CHAR_STATE.MovePortal) {
			lastTargetPos = nav.pathEndPosition;
			bMovePause = true;
			nav.enabled = false;
		}

		eBeforeState = eCharacterState;
		eCharacterState = CHAR_STATE.Pause;

		a_col.SetEnable(false);
		rig.Sleep();
	}

	public void Resume(bool bOwner){
		if(cObject == null) return;
		//SetAnimationSpeed(fAnimSpd);
		eCharacterState = eBeforeState;

		if (bMovePause) {
			nav.enabled = true;
			nav.SetDestination (lastTargetPos);
			obs.enabled = false;
		} else {
			nav.enabled = false;
			obs.enabled = true;
		}
		bMovePause = false;

		if(isDead) a_col.SetEnable(false);
		else a_col.SetEnable(true);
		SubAnimationEnable (true);
		rig.WakeUp();
	}

	public void SetSpeedScale(float spd){
		fSpdScale = spd;
		SetAnimationSpeed(fSpdScale);
	}

	public void CheckAttackCancel(Vector3 dir){
		if(bAvoid){
			if(iTeamIdx == 0) cBattle.battleUIMgr.DuplicateDmg(GetBodyPos(), TextManager.Instance.GetText("battle_direction_eva"), BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_HERO);
			else if(iTeamIdx == 1) cBattle.battleUIMgr.DuplicateDmg(GetHeadPos(1f, 0.3f), TextManager.Instance.GetText("battle_direction_eva"), BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_MONSTER);
			return;
		}
		if(bHero && bSkill) return;

		AttackCancel(dir ,0.5f, 50f);
	}
	
	public void CheckAttackCancel(Vector3 dir, float dist){
		if(bAvoid){
			if(iTeamIdx == 0) cBattle.battleUIMgr.DuplicateDmg(GetBodyPos(), TextManager.Instance.GetText("battle_direction_eva"), BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_HERO);
			else if(iTeamIdx == 1) cBattle.battleUIMgr.DuplicateDmg(GetHeadPos(1f, 0.3f), TextManager.Instance.GetText("battle_direction_eva"), BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_MONSTER);
			return;
		}
		if(cCharacter.cClass.bKnockbackIgnore) return;

		if(bHero && bSkill) return;
		
		AttackCancel(dir ,0.5f, dist*100f);
	}
	
	void AttackCancel(Vector3 dir, float time, float power){
		u2AtkIdx = -1;
		eCharacterState = CHAR_STATE.Damage;

		Missile[] temp = cObject.GetComponentsInChildren<Missile>();
		foreach (Missile target in temp) {
			target.DeleteObject();
		}
		
//		DebugMgr.LogError(power);
		
		//cAnimator.Play("Damage");
		if (bHero) {
			if(bChange) SubAnimationCrossFade ("Damage", 0.2f);
			else{
				if (Vector3.Angle (cObject.transform.forward, dir) > 135f) {
					SubAnimationCrossFade ("Damage_Front", 0.2f);
				} else if (Vector3.Angle (cObject.transform.forward, dir) < 45f) {
					SubAnimationCrossFade ("Damage_Back", 0.2f);
				} else {
					Vector3 left = Quaternion.Euler (new Vector3 (0, 90, 0)) * cObject.transform.forward;
					if (Vector3.Angle (left, dir) < 45f) {
						SubAnimationCrossFade ("Damage_Right", 0.2f);
					} else {
						SubAnimationCrossFade ("Damage_Left", 0.2f);
					}
				}
			}
		} else {
			if(cCharacter.cClass.u1MonsterType == 2){
				SubAnimationCrossFade ("Stand", 0.2f);
				cBattle.battleUIMgr.HideWarning();
			}else SubAnimationCrossFade ("Damage", 0.2f);
		}
		if(nav.enabled) nav.Stop();
		SetAttackWait ();
		fLastStateTime = time;
		cSkills.cCurSkillInfo = null;
		bSkill = false;
		//rig.AddForce(Vector3.up*power);
		if(cCharacter.cClass.bKnockbackIgnore) return;

		rig.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
		rig.AddForce(Vector3.Normalize(dir)*power);
	}

	public void SetIdle(bool bInit){
		SetAnimationSpeed(1.0f);
		eCharacterState = CHAR_STATE.Idle;
		//cAnimator.Play("Stand");
		if (bInit) {
			SubAnimationPlay ("Stand");
			if(!nav.enabled) obs.enabled = true;
			Bip01.localPosition = Vector3.zero;
			InitAnimPos ();
		} else {
			SubAnimationCrossFade ("Stand", 0.1f);
			nav.enabled = false;
			obs.enabled = true;
		}

		if (bHero) {
			if (iTeamIdx == 0 && bSkill)
				cBattle.battleUIMgr.ShowHeroSkillBreak (iCharIdx);
		} else {
			if (cCharacter.cClass.u1MonsterType == 2) {
				cBattle.battleUIMgr.HideWarning ();
				if (bSkill)
					cBattle.battleUIMgr.ShowBossSkillBreak (GetBodyPos ());
			}
		}

		a_col.bStay = false;
		a_col.bWall = false;
		bWait = true;
		fWaitTime = 0;
		iChainBonus = 0;
		bSkill = false;

		rig.constraints = RigidbodyConstraints.FreezeAll;
	}

	public void InitState()
	{
		SetAnimationSpeed(1.0f);
		eCharacterState = CHAR_STATE.Idle;

		if(!nav.enabled) obs.enabled = true;

		a_col.bStay = false;
		a_col.bWall = false;
		bWait = true;
		fWaitTime = 0;
		iChainBonus = 0;
		fLandSpd = 0.1f;
		bLand = false;
		bSkill = false;

		cTarget = null;

		cCondis.Dispel ();
		cSkills.InitCoolTime ();

		rig.constraints = RigidbodyConstraints.FreezeAll;
	}

	public void MoveToTarget(BattleCharacter tTarget){
		if(bSkill) return;
		if(eCharacterState == CHAR_STATE.Attacking) return;
		cTarget = tTarget;
		SetIdle(false);
	}

	public void SetMove(){
		if(cCondis.CheckCondition(CONDITION_TYPE.ImMovable)) return;

		float moveSpd = ((float)lastMoveSpd)/1000f;

		if(moveSpd <= 0) return;
		SetAnimationSpeed(moveSpd);
		SubAnimationCrossFade("Run",0.2f);

		u2AtkIdx = 0;
		eCharacterState = CHAR_STATE.Move;
		EnableMove(true);

		rig.constraints = RigidbodyConstraints.FreezeAll;
//		rig.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
	}

	public void EnableMove(bool bEnable){
		if(nav.enabled) nav.Stop();

		if (bEnable) {
			bWait = true;
			obs.enabled = false;
		} else {
			nav.enabled = false;
			obs.enabled = true;
		}
		a_col.SetRun(bEnable);
	}
	
	public void ChangeMoveSpd(){
		//DebugMgr.Log("ChangeMoveSpd"+lastMoveSpd);
		if (eCharacterState == CHAR_STATE.Move) {
			SetAnimationSpeed(lastMoveSpd/1000f);
		}
	}

	public void CheckSkillEnd(){
		if (bAttack) {
			if (subAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Stand")) {
				SetAttackWait ();
				return;
			}
		}
	}

	public void LookAtTarget(){
		if(cTarget.cObject != null){
			Vector3 pos = new Vector3(cTarget.cObject.transform.position.x, cObject.transform.position.y, cTarget.cObject.transform.position.z);
			cObject.transform.LookAt(pos);
		}
	}

	public void LookAtTargetSlow(){
		if(cTarget.cObject != null){
			Quaternion targetRotation = Quaternion.LookRotation(cTarget.cObject.transform.position - cObject.transform.position);

			cObject.transform.rotation = Quaternion.Slerp(cObject.transform.rotation, targetRotation, 30f * Time.deltaTime);
		}
	}

	public void UseSkill(int idx){
		if(isDead) return;

		SaveCurrentBipPos();
		SubAnimatorBip01Fix(false);

		LookAtTarget();

		if(SkillActive != null) SkillActive(cCrew, cEnemy);

		bAttack = false;
		bAvoid = false;
		bSkill = true;
		bEffect = false;
		fSkillUseDist = 0;
		u2SavedSkillIdx = 0;
		EnableMove(false);
		SetAnimationSpeed(1.0f);
		SubAnimationPlay(cSkills.lstcSelectedActiveSkill[idx].cInfo.cAttackModel.sStartAnim);

//		if (col.bFocus) {
//			cBattle.cCameraMove2.PlaySkillCamAnim(cSkills.lstcSelectedActiveSkill [idx].cInfo.sDirectionName);
//		}

		eCharacterState = CHAR_STATE.Attacking;

		SetCrewCool();
		if (iTeamIdx == 0) {
			string sname = TextManager.Instance.GetText (cSkills.cCurSkillInfo.sName);
			string sdesc = cSkills.cCurSkill.GetSkillDesc();//TextManager.Instance.GetText (cSkills.cCurSkill.GetSkillDesc());
			cBattle.battleUIMgr.RemoveChainState ();
			if(iTeamIdx == 0) cBattle.cCont.cAIController.CheckNextSkill (iCharIdx, idx);
			cBattle.battleUIMgr.PlaySkillUI ();
		}

		int ChainType = 0;

		if (bHero) {
			iChainBonus = 0;
			if(cSkills.cCurSkillInfo.u2LinkCondition > 0){
				if(cCrew.u2LastBuff > 0 && cCrew.u2LastBuff == cSkills.cCurSkillInfo.u2LinkCondition){
					iChainBonus = (int)cSkills.cCurSkillInfo.u1LinkBonus;
					ChainType = 1;
				}else if(cCrew.u2LastDebuff > 0 && cCrew.u2LastDebuff == cSkills.cCurSkillInfo.u2LinkCondition){
					iChainBonus = (int)cSkills.cCurSkillInfo.u1LinkBonus;
					ChainType = 2;
				}
			}

			if(cSkills.cCurSkillInfo.cBuff != null){
				cCrew.u2LastBuff = cSkills.cCurSkillInfo.cBuff.u2ID;
				FindChainSkill(cCrew.u2LastBuff);
			}else cCrew.u2LastBuff = 0;

			if(cSkills.cCurSkillInfo.cDebuff != null){
				cCrew.u2LastDebuff = cSkills.cCurSkillInfo.cDebuff.u2ID;
				FindChainSkill(cCrew.u2LastDebuff);
			}else cCrew.u2LastDebuff = 0;

			if(cSkills.cCurSkillInfo.bComboBomb) ChainType = 3;
		}else{
			if (cCharacter.cClass.u1MonsterType == 2) {
				cBattle.battleUIMgr.ShowWarning(cObject.transform);
			}
		}

		Byte SkillEle = cSkills.cCurSkillInfo.u1Element == 5 ? u1Element : cSkills.cCurSkillInfo.u1Element;

		if (ChainType == 3) {
			if (cCrew.u1ComboCount > 2){
				CombomBomb ();
			}else{
				if(cCrew.u1ComboElement == SkillEle && SkillEle > 1){
					if(cCrew.u1ComboCount < LegionInfoMgr.Instance.MaxComboCount) cCrew.u1ComboCount++;
				}else{
					cCrew.u1ComboCount = 0;
				}
			}
		}else if(ChainType > 0){
			if(cCrew.u1ComboCount > 2 && cSkills.cCurSkillInfo.u2DamagePercent > 0){
				CombomBomb();
			}else{
				if(iTeamIdx == 0){
					if(ChainType == 1) cBattle.battleUIMgr.ShowSkillChain(true);
					else if(ChainType == 2) cBattle.battleUIMgr.ShowSkillChain(false);
				}
			}
		} else {
			if(cCrew.u1ComboElement == SkillEle && SkillEle > 1){
				if(cCrew.u1ComboCount < LegionInfoMgr.Instance.MaxComboCount) cCrew.u1ComboCount++;
			}else{
				cCrew.u1ComboCount = 0;
			}
		}

		cCrew.u1ComboElement = SkillEle;
		if(iTeamIdx == 0) cBattle.battleUIMgr.ShowCombo (cCrew.u1ComboElement, cCrew.u1ComboCount);

		u2LastSkillIdx++;
		if(u2LastSkillIdx >= cSkills.lstcSelectedActiveSkill.Count) u2LastSkillIdx = 0;

//		if(iTeamIdx == 0) cBattle.battleUIMgr.SetCoolDown(false);
	}

	public void FindChainSkill(UInt16 CondID){
		if (bSupport) return;
		if (iTeamIdx != 0) return;
		if(CondID == 0) return;
		for (int i=0; i<cCrew.acCharacters.Length; i++) {
			if(cCrew.acCharacters[i] == null) continue;
			if(cCrew.acCharacters[i].isDead) continue;

			List<BattleSkill> skills = cCrew.acCharacters [i].cSkills.lstcSelectedActiveSkill.FindAll (cs => cs.cInfo.u2LinkCondition == CondID);
			if (skills.Count > 0) {
				for(int j=0; j<skills.Count; j++)
				cBattle.battleUIMgr.SetChainState (i, skills[j].u1ActiveSkillIndex); 
			}
		}
	}

	void CombomBomb(){
		if(iTeamIdx == 0) cBattle.battleUIMgr.ShowComboBomb();

		cCrew.u2LastBuff = 0;
		cCrew.u1ComboCount = 0;

		float fDamage = 0;
		//float runeBonus = 0;

		ClassInfo calClass = cCharacter.cClass;

		if(calClass.u1BasicAttackElement == 1){
			//fDamage = (cBattleStatus.u2Strength + cBattleStatus.u2Agility * calClass.cPhysicalAttackAgility.Random) * calClass.cPhysicalAttack.Random;
            fDamage = (cBattleStatus.GetStat(2) + cBattleStatus.GetStat(6) * calClass.cPhysicalAttackAgility.Random) * calClass.cPhysicalAttack.Random;
			//runeBonus = GetRuneVal(RuneType.AD, 1)-cEnemy.GetRuneVal(RuneType.AD, 2);
		}else{
			//fDamage = (cBattleStatus.u2Intelligence + cBattleStatus.u2Agility * calClass.cMagicAttackAgility.Random) * calClass.cMagicAttack.Random;
            fDamage = (cBattleStatus.GetStat(3) + cBattleStatus.GetStat(6) * calClass.cMagicAttackAgility.Random) * calClass.cMagicAttack.Random;
			//runeBonus = GetRuneVal(RuneType.AP, 1)-cEnemy.GetRuneVal(RuneType.AP, 2);
		}

		int resultdmg = ((int)fDamage*(100+(cCrew.u1ComboCount*LegionInfoMgr.Instance.MaxComboAddPerDamge)))/100;

		for(int i=0; i<cEnemy.acCharacters.Length; i++){
			//DebugMgr.Log(i +"/"+ crew.acCharacters[i].cCharacter.cClass.sName +"/"+ crew.acCharacters[i].cObject.name +"/"+ crew.acCharacters[i].isDead);
			if(!cEnemy.acCharacters[i].isDead){
				cEnemy.acCharacters[i].GetDamage(resultdmg, false, -1, TextManager.Instance.GetText("battle_direction_combo"), this);
			}
		}
	}

	void SetCrewCool(){
//		for (int i=0; i<cOwnCrew.acCharacters.Length; i++) {
//			if(!cOwnCrew.acCharacters[i].isDead){
//				cOwnCrew.acCharacters[i].cSkills.SetCrewCool();
//			}
//		}
		cSkills.SetCrewCool();
	}

	public void SetCharState(CHAR_STATE eState){
		eCharacterState = eState;

		switch (eCharacterState) {
		case CHAR_STATE.Idle:
			break;
		case CHAR_STATE.Move:
			break;
		case CHAR_STATE.Attack:
			break;
		case CHAR_STATE.Attacking:
			break;
		case CHAR_STATE.AttackWait:
			break;
		case CHAR_STATE.Damage:
			break;
		case CHAR_STATE.Death:
			break;
		case CHAR_STATE.Win:
			break;
		}
	}

	public void SetAttack(){
		if(cCondis.CheckCondition(CONDITION_TYPE.CantAttack)) return;

		LookAtTarget();

		if(cTarget.cObject != null){
			if(a_col.bFocus) cBattle.battleUIMgr.SetMonsterHp(cTarget);
		}

		if (fWaitTime > 0f) {
			eCharacterState = CHAR_STATE.AttackWait;
			EnableMove(false);
			SubAnimationCrossFade("Stand",0.2f);
			return;
		}

		if(!bFirstAtk){
			if(SkillFirstAttack != null) SkillFirstAttack(cCrew, cEnemy);
			bFirstAtk = true;
		}

		if(SkillBasicAttack != null) SkillBasicAttack(cCrew, cEnemy);

//		if(iTeamIdx == 0 && cCurAtk.cAttackSet.u1AttackType == 2) DebugMgr.LogError("Avoid");

		SetAnimationSpeed(1.0f);
		//SubAnimationCrossFade(cCurAtk.cAttackSet.sStartAnim, 0.2f);
		SubAnimationPlay(cCurAtk.cAttackSet.sStartAnim);
		if(cCurAtk.cAttackSet.bStay) a_col.bWall = true;
//		else rig.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
		beforeBipPos = cObject.transform.position;
		eCharacterState = CHAR_STATE.Attacking;
		bAttack = false;
		bSkill = false;
		fNextAttackPer = 100;

		if(cCurAtk.cAttackSet.u1AttackType == 3) JumpStart();
		
		else VFXMgr.Instance.GetVFX("/Common/ExclamationMake", GetSocketTrans("Bip01-Head").position+Vector3.up*0.5f, Quaternion.identity);
	}

	public void SetMoveByBtn(float angle){
		if(isDead) return;

		if(eCharacterState != CHAR_STATE.Idle && eCharacterState != CHAR_STATE.Move && eCharacterState != CHAR_STATE.AttackWait) return;

//		if (angle > 45 && angle < 315) {
//			Quaternion curRot = cObject.transform.rotation;
//			Vector3 euler = cObject.transform.rotation.eulerAngles;
//			Quaternion nextRot = Quaternion.Euler(euler + new Vector3(0,angle,0));
//			cObject.transform.rotation = Quaternion.RotateTowards (curRot, nextRot, 200f * Time.deltaTime);
//		}
		angle += fFirstAngle;
		cObject.transform.rotation = Quaternion.Euler(new Vector3(0,angle,0));

		eCharacterState = CHAR_STATE.Move;
		if (nav.enabled) {
			nav.Stop ();
			nav.enabled = false;
			obs.enabled = false;
		}
		SubAnimationPlay("Run");
		cObject.transform.position += (cObject.transform.forward * (lastMoveSpd/1000f) * Time.deltaTime * 3f);
	}

	public void SetStopByBtn(){
		if(isDead) return;
		if(eCharacterState == CHAR_STATE.Move) SetIdle(false);
		else DebugMgr.LogError(eCharacterState);
	}

	public void SetAttackByBtn(){
		if(isDead) return;

		if(cCondis.CheckCondition(CONDITION_TYPE.CantAttack)) return;

		cCurAtk = cCharacter.cClass.acAttacks[0];
		if(cTarget == null) cTarget = this;
		SetAnimationSpeed(1.0f);
		SubAnimationPlay(cCurAtk.cAttackSet.sStartAnim);
		if(cCurAtk.cAttackSet.bStay) a_col.bWall = true;
		beforeBipPos = cObject.transform.position;

		eCharacterState = CHAR_STATE.Attacking;
		bAttack = false;
		bSkill = false;
		fNextAttackPer = 0;
	}

	public void SetAttackWait(){
		if (eCharacterState == CHAR_STATE.AttackWait)
			return;
		
		if(bSkill){
//			if(iTeamIdx == 0) cBattle.battleUIMgr.SetCoolDown(true);
			fWaitTime = UnityEngine.Random.Range(cSkills.cCurSkillInfo.cAttackModel.fDelay, cSkills.cCurSkillInfo.cAttackModel.fMaxDelay)/1000f;
			bSkill = false;
			bEffect = true;
			cSkills.cCurSkillInfo = null;

			cTarget = null;
		}else{
			if(cCurAtk.cAttackSet == null) fWaitTime = 0;
			else fWaitTime = UnityEngine.Random.Range(cCurAtk.cAttackSet.fDelay, cCurAtk.cAttackSet.fMaxDelay)/1000f;

			if(cTarget != null && cTarget.isDead) cTarget = null;
		}
		eCharacterState = CHAR_STATE.AttackWait;
		bAttack = false;
		bAvoid = false;
		bNextCheck = false;
		iChainBonus = 0;
		bWait = true;
		a_col.bWall = false;

		rig.constraints = RigidbodyConstraints.FreezeAll;
	}

	public void EndAttackWait(){
		EnableMove(false);
		fWaitTime = 0;
		eCharacterState = CHAR_STATE.Idle;
		if (cTarget != null) {
			if(cTarget.isDead) cTarget = null;
		}
//		if(cTarget == null) SetIdle();
//		else SetMove();
	}

	public void SetFocus(bool bFoc){
		a_col.SetFocus(bFoc);

		if(bJoyStick){
			if (bFoc) {
				if(eCharacterState == CHAR_STATE.Move){
					SetIdle(false);
				}
				nav.enabled = false;
			}
		}
	}

	public bool CheckJoystick(){
		if(!bJoyStick) return false;

		if(eCharacterState == CHAR_STATE.AttackWait || eCharacterState == CHAR_STATE.Damage) return false;

		return a_col.bFocus;
	}

	public bool CheckJoystickBlock(){
		if(bJoyStick && a_col.bFocus) return false;
		
		return true;
	}

	public void CheckChain(){
		FindChainSkill (cCrew.u2LastBuff);
		FindChainSkill (cCrew.u2LastDebuff);
	}

	public bool CheckEmun(){
		if (cBattle.TutorialCheckType == 1) {
			if (cCharacter.cClass.u1MonsterType == 2) return true;
		}

		return false;
	}

	public void SetDeath(){
		if (cBattle.TutorialCheckType == 1) {
			if (cCharacter.cClass.u1MonsterType == 2) return;
		}

		cCondis.Dispel();

		SetAnimationSpeed(1.0f);
		SubAnimationCrossFade("Die",0.1f);
		isDead = true;
		u4HP = 0;
		nav.enabled = false;
		obs.enabled = false;
		SetDamageColor(0);
		if (iTeamIdx == 0) {
			if(a_col.bFocus){
				cBattle.battleUIMgr.OnSelectCharaceter (CheckLiveAlly ());
				if(bSkill) cBattle.cCameraMove2.DisableDirectionCam();
			} 
			cBattle.cCont.cAIController.SetUserQueue ();
		}else{
			if(bHero) cBattle.cCont.cAIController.SetEnemyUserQueue ();
		}

		if(bHero) cBattle.battleUIMgr.SetDeath(iTeamIdx, iCharIdx);
			
//		m_col.enabled = false;
		eCharacterState = CHAR_STATE.Death;

		if(acDropBoxs != null){
			DropItem();
		}

		if (cBattle.eBattleState == Battle.BATTLE_STATE.Battle) {
			if (SkillDeath != null)
				SkillDeath (cCrew, cEnemy);

			for(int i=0; i<cCrew.acCharacters.Length; i++){
				if(!cCrew.acCharacters[i].isDead){
					if(cCrew.acCharacters[i].SkillTeamDeath != null)
						cCrew.acCharacters[i].SkillTeamDeath(cCrew, cEnemy);
				}
			}
		}

		if (cCharacter.cClass.u1MonsterType == 2) {
			cBattle.battleUIMgr.HideWarning ();
			cBattle.ClearStage ();
		}

		if (iTeamIdx == 0) {
			if (CheckLiveAlly () == -1) {
				cBattle.cCameraMove2.SetDefeatCam (this);
			}
		}
	}

	public int CheckLiveAlly(){
		for (int i=0; i<cCrew.acCharacters.Length; i++) {
			
			if(!cCrew.acCharacters[i].isDead){
				return i;
			}
		}

		return -1;
	}

	public float GetHitHeight(){
		return Bip01.localPosition.z;
	}

	public void StartRemoveMonster(){
		if(bHero) return;
		if(cBattle == null) return;
		cBattle.SpawnNext(this);
		cObject.AddComponent<DissolveEffect> ().SetMats (cCharacter.cClass.u2ID, mats, mats2);
//		ObjMgr.Instance.UnUseMonster(cCharacter.cClass.u2ID, cObject);
	}

	void DropItem(){
		cBattle.DropItemBox(acDropBoxs, cObject.transform.position);
	}

	public void RunToPortal(){
		//EnableMove (false);
		cTarget = null;
		nav.enabled = true;
		nav.SetDestination(GameObject.FindGameObjectWithTag("Portal").transform.position+UnityEngine.Random.insideUnitSphere);

		eCharacterState = CHAR_STATE.MovePortal;
		float moveSpd = ((float)lastMoveSpd)/1000f;
		nav.speed = moveSpd * 3f;
		SetAnimationSpeed (moveSpd);
		SubAnimationCrossFade("Run",0.1f);

		if(bSkill){
//			if(iTeamIdx == 0) cBattle.battleUIMgr.SetCoolDown(true);
			fWaitTime = UnityEngine.Random.Range(cSkills.cCurSkillInfo.cAttackModel.fDelay, cSkills.cCurSkillInfo.cAttackModel.fMaxDelay)/1000f;
			bSkill = false;
			bEffect = true;
			cSkills.cCurSkillInfo = null;
		}else{
			if(bJoyStick) fWaitTime = 0f;
			else fWaitTime = UnityEngine.Random.Range(cCurAtk.cAttackSet.fDelay, cCurAtk.cAttackSet.fMaxDelay)/1000f;
		}
		bAttack = false;
	}

	public void WaitBoss(){
		nav.enabled = false;
		a_col.SetEnable(false);
		bAnimColInit = false;
		SetIdle(false);
	}

	public void PlayLevelup(){
		cCondis.Dispel();
		EnableMove (false);
		cTarget = null;
		eCharacterState = CHAR_STATE.Win;
		isDead = false;
		SetDamageColor(0f);
		SetAnimationSpeed (1.0f);
		SubAnimationPlay("Levelup");
		nav.enabled = false;
		obs.enabled = false;
		a_col.SetEnable(false);
//		m_col.enabled = false;
	}

	public void PlayWin(){
		cCondis.Dispel();

		EnableMove (false);
		cTarget = null;
		eCharacterState = CHAR_STATE.Win;
		SetDamageColor(0f);
		SetAnimationSpeed (1.0f);
		isDead = false;
		if(cBattle.TutorialCheckType != 1) SubAnimationCrossFade("Win",0.2f);
		nav.enabled = false;
		obs.enabled = false;
		a_col.SetEnable(false);
//		m_col.enabled = false;
	}

	public void QuitEvent(){
		cCondis.Dispel();
	}

	public void PlayResult(){
		SubAnimationPlay("Win");
	}

	public void PlayDirection(){
		SubAnimationPlay("Direction1");
	}

	public void SetEnableAnimator(bool bEnable){
		subAnimator.enabled = bEnable;
	}
		
	void InitAnimPos(){
		animChangePos = Vector3.zero;
		animMovePos = Vector3.zero;
		animBeforePos = Bip01.position;
		v3Bip = Bip01.localPosition;
	}

	public void SaveAnimationBipPos(){
		if(cBattle == null) return;
		if(cBattle.eBattleState != Battle.BATTLE_STATE.Battle) return;

		animBeforePos = Bip01.position;

//		if(iTeamIdx == 0) DebugMgr.LogWarning(cObject.name+" animBeforePos"+animBeforePos.x+","+animBeforePos.z);
		//if(iTeamIdx == 0) DebugMgr.Log("SaveAnimationBipPos = "+animBeforePos);
		bAnimColInit = true;
	}

	public void TestSaveAnimationBipPos(){
		animBeforePos = Bip01.position;
		
		//		if(iTeamIdx == 0) DebugMgr.LogWarning(cObject.name+" animBeforePos"+animBeforePos.x+","+animBeforePos.z);
		//if(iTeamIdx == 0) DebugMgr.Log("SaveAnimationBipPos = "+animBeforePos);
		bAnimColInit = true;
	}

	public void CheckAnimationBipPos(){
		if(!bAnimColInit) return;

		Vector3 before = animChangePos;

//		animChangePos = animBeforePos - Bip01.localPosition;
//		animMovePos = Bip01.rotation*(animChangePos - before);
		animChangePos = Bip01.position - animBeforePos;
		animMovePos = (animChangePos - before);
//		if(iTeamIdx == 0) DebugMgr.LogWarning(cObject.name+" animMovePos"+animMovePos.x+","+animMovePos.z);
	}

	public bool CheckPinch(){
		if(nav.enabled) {
			if (nav.desiredVelocity == Vector3.zero) {
				if (moveTime >= 0f) {
					moveTime -= Time.fixedDeltaTime;
					if (moveTime < 0f) {
						moveTime = 0.15f;
						Vector3 beforeDestPos = nav.destination;
						nav.ResetPath ();
						if (cTarget != null) nav.SetDestination (beforeDestPos);
						return true;
					}
				}
			} else {
				moveTime = 0.15f;
			}
		}
			
		return false;
	}

	public void SaveCurrentBipPos(){
		beforeBipPos = cObject.transform.position;
		//if(iTeamIdx == 0 && cCharacter.iIndexInCrew == 1) DebugMgr.LogWarning(cObject.name+" beforeBipPos"+beforeBipPos.x+","+beforeBipPos.z);
	}

	public void SubAnimatorBip01Fix(bool bStay){
		if(!bAnimColInit) return;

		if(!bJump) cObject.transform.position = new Vector3(cObject.transform.position.x, CheckGround(), cObject.transform.position.z);

		//if(eCharacterState == CHAR_STATE.Move || eCharacterState == CHAR_STATE.Idle) return;
		if(eCharacterState == CHAR_STATE.Move || eCharacterState == CHAR_STATE.MovePortal) return;

		if (bStay){
//			if(iTeamIdx == 0 && cCharacter.iIndexInCrew == 0) DebugMgr.Log (cObject.name + " beforeBipPos" + beforeBipPos.x + "," + beforeBipPos.z);

			if(!bJump) cObject.transform.position = beforeBipPos;
			else cObject.transform.position = new Vector3(beforeBipPos.x, cObject.transform.position.y, beforeBipPos.z);
		} else {
//			Vector3 lastPos = animMovePos;
//
//			if(subAnimator.IsInTransition(0)) lastPos = Vector3.zero;

			lastPos = Vector3.zero;
//
//			if(iTeamIdx == 0 && cCharacter.iIndexInCrew == 0) DebugMgr.Log(cObject.name+" lastPos"+lastPos.x+","+lastPos.z);

			cObject.transform.position += new Vector3 (lastPos.x, 0f, lastPos.z);
		}
			
		if(subAnimator.transform.FindChild("Bip01") != null){
			subAnimator.transform.FindChild("Bip01").localPosition = new Vector3 (v3Bip.x,
			                                                                      v3Bip.y,
			                                                                      subAnimator.transform.FindChild("Bip01").localPosition.z);
		}else{
			subAnimator.transform.GetChild(0).localPosition = new Vector3 (0,
			                                                               0,
			                                                               subAnimator.transform.GetChild (0).localPosition.z);
		}
	}

	public float CheckGround(){
		if (cObject == null)
			return 0;
		
		Vector3 from = cObject.transform.position + Vector3.up * 0.3f;
		Vector3 target = cObject.transform.position + Vector3.down*50f;
		
		RaycastHit GroundHit = new RaycastHit();
		if (Physics.Linecast (from, target, out GroundHit)) {
			if (GroundHit.collider != null) {
				if (GroundHit.collider.tag == "Terrain") {
					return GroundHit.point.y;
				}
			}
		}

		return 0;
	}

	public void SubAnimationPlay(string animName)
	{
		if(isDead) return;

		if (bChange) {
			changeAnimator.Play(animName);
		}
			
		subAnimator.Play(animName);
	}
	
	public void SubAnimationCrossFade(string animName, float time)
	{
		if(isDead) return;

		if (bChange) {
			changeAnimator.CrossFade(animName, time, 0, 0.0f);
		}

//		DebugMgr.Log (cCharacter.cClass.sName + "/" + animName);

		subAnimator.CrossFade(animName, time, 0, 0.0f);
	}
	
	private float GetAnimLength(string animName){
		if(subAnimator.runtimeAnimatorController.animationClips != null) {
			for(int i = 0; i < subAnimator.runtimeAnimatorController.animationClips.Length; i++) {
				if(subAnimator.runtimeAnimatorController.animationClips[i].name == animName) {
					return subAnimator.runtimeAnimatorController.animationClips[i].length;
				}
			}
		}
		
		return 0f;
	}

	private void SetAnimationSpeed(float spd)
	{
		if (bChange) {
			changeAnimator.speed = spd;
		}
			
		subAnimator.speed = spd;
	}

	private void SubAnimationEnable(bool enable)
	{
		if (bChange) {
			changeAnimator.enabled = enable;
		}
			
		subAnimator.enabled = enable;
	}

	public void ModelEnable(bool enable)
	{
		subAnimator.gameObject.SetActive(enable);
	}

	public void SetChangeAnimator(bool change){
		bChange = change;
		if(change){
			changeAnimator = _charObject.transform.FindChild("Animator").GetComponentInChildren<Animator>();
		}else{
			changeAnimator.gameObject.SetActive(false);

			changeAnimator = null;
		}
	}

	public void CheckPassive(Byte u1ActSituation){
		switch (u1ActSituation)
		{
		case 1 : 	break;
		case 2 :	if(SkillPhaseStart != null) SkillPhaseStart(cCrew, cEnemy);	break;
		case 3 :	if(SkillFirstAttack != null) SkillFirstAttack(cCrew, cEnemy);	break;
		case 4 :	if(SkillDeath != null) SkillDeath(cCrew, cEnemy);	break;
		case 5 :	if(SkillActive != null) SkillActive(cCrew, cEnemy);	break;
		case 6 :	if(SkillTime != null) SkillTime(cCrew, cEnemy);	break;
		case 7 :	if(SkillFirstDamage != null) SkillFirstDamage(cCrew, cEnemy);	break;
		case 8 :	if(SkillNonHP != null) SkillNonHP(cCrew, cEnemy);	break;
		case 9 :	if(SkillTeamDeath != null) SkillTeamDeath(cCrew, cEnemy);	break;
		case 10 : 	if(SkillBasicAttack != null) SkillBasicAttack(cCrew, cEnemy);	break;
		case 11 : 	if(SkillAttackCrit != null) SkillAttackCrit(cCrew, cEnemy); 	break;
		case 12 : 	if(SkillSkillCrit != null) SkillSkillCrit(cCrew, cEnemy);	break;
		case 13 : 	if(SkillDamage != null) SkillDamage(cCrew, cEnemy);	break;
		case 14 : 	if(SkillLastAttack != null) SkillLastAttack(cCrew, cEnemy);	break;
		case 15 : 	if(SkillMonsterGroupDeath != null) SkillMonsterGroupDeath(cCrew, cEnemy); break;
		case 16 : 	if(SkillGuard != null) SkillGuard(cCrew, cEnemy); break;
		case 17 : 	if(SkillAvoid != null) SkillAvoid(cCrew, cEnemy); break;
		}
	}
	
	public delegate bool SpellSkill(BattleCrew ours, BattleCrew enemy);
	public SpellSkill[] aeSkill = new SpellSkill[6];
	public event SpellSkill SkillPhaseStart;
	public event SpellSkill SkillFirstAttack;
	public event SpellSkill SkillDeath;
	public event SpellSkill SkillActive;
	public event SpellSkill SkillTime;
	public event SpellSkill SkillFirstDamage;
	public event SpellSkill SkillNonHP;
	public event SpellSkill SkillTeamDeath;
	public event SpellSkill SkillBasicAttack;
	public event SpellSkill SkillAttackCrit;
	public event SpellSkill SkillSkillCrit;
	public event SpellSkill SkillDamage;
	public event SpellSkill SkillLastAttack;
	public event SpellSkill SkillMonsterGroupDeath;
	public event SpellSkill SkillGuard;
	public event SpellSkill SkillAvoid;
	public void RegistSkillCall(Byte u1ActSituation, Byte u1ActiveSkillIndex, SpellSkill spell)
	{
		switch (u1ActSituation)
		{
		case 1 :	aeSkill[u1ActiveSkillIndex] = spell;// TO DO : Link spell to UI call
			break;
		case 2 :	SkillPhaseStart += spell;	break;
		case 3 :	SkillFirstAttack += spell;	break;
		case 4 :	SkillDeath += spell;	break;
		case 5 :	SkillActive += spell;	break;
		case 6 :	SkillTime += spell;	break;
		case 7 :	SkillFirstDamage += spell;	break;
		case 8 :	SkillNonHP += spell;	break;
		case 9 :	SkillTeamDeath += spell;	break;
		case 10 : 	SkillBasicAttack += spell;	break;
		case 11 : 	SkillAttackCrit += spell; 	break;
		case 12 : 	SkillSkillCrit += spell;	break;
		case 13 : 	SkillDamage += spell;	break;
		case 14 : 	SkillLastAttack += spell;	break;
		case 15 : 	SkillMonsterGroupDeath += spell;	break;
		case 16 : 	SkillGuard += spell;	break;
		case 17 : 	SkillAvoid += spell;	break;
		}
	}

	public void EventToCharacters(Battle.BROADCAST_EVENT_TYPE eType, BattleCrew cOurCrew, BattleCrew cEnemyCrew)
	{
		switch (eType)
		{
		case Battle.BROADCAST_EVENT_TYPE.Load :
			cSkills.CheckSkill(cOurCrew, cEnemyCrew);
			break;
		case Battle.BROADCAST_EVENT_TYPE.StartPhase :
			bFirstAtk = false;
			bFirstDmg = false;
			if (SkillPhaseStart != null) SkillPhaseStart(cOurCrew, cEnemyCrew);
			break;
		case Battle.BROADCAST_EVENT_TYPE.GroupRespawn :
			if (SkillMonsterGroupDeath != null) SkillMonsterGroupDeath(cOurCrew, cEnemyCrew);
			break;
		}
	}

	//public FieldInfo.Pos GetPosInCrew(FieldInfo cBattleField)
	//{
	//	FieldInfo.Pos ret = new FieldInfo.Pos();
	//	ret.X = cBattleField.fGapHorizontal / 2f;
	//	if (u1LocationX == 1) ret.X *= -1f;
	//	switch (cBattle.acCrews[iTeamIdx].au1LineCount[u1LocationX])
	//	{
	//	case 1:
	//		ret.Y = 0;
	//		break;
	//	case 2:
	//		ret.Y = cBattleField.fGapVertical / 2f;
	//		if (u1LocationIndex == 0) ret.Y *= -1f;
	//		break;
	//	case 3:
	//		ret.Y = 0;
	//		if (u1LocationIndex == 0) ret.Y = cBattleField.fGapVertical * -1f;
	//		else if (u1LocationIndex == 2) ret.Y = cBattleField.fGapVertical;
	//		break;
	//	}
	//	ret.Add(cBattleField.cCrew0Center);
	//	return ret;
	//}

	//public float GetPosInCrewX(FieldInfo cBattleField)
	//{
	//	return (u1LocationX == 0 ? cBattleField.fGapHorizontal / 2f : cBattleField.fGapHorizontal / 2f * -1f) + cBattleField.cCrew0Center.X;
	//}
	//public float GetPosInCrewY(FieldInfo cBattleField)
	//{
	//	float y = 0;
	//	switch (cBattle.acCrews[iTeamIdx].au1LineCount[u1LocationX])
	//	{
	//		case 2:
	//			y = (u1LocationIndex == 0 ? cBattleField.fGapVertical / 2f * -1f : cBattleField.fGapVertical / 2f);
	//			break;
	//		case 3:
	//			if (u1LocationIndex == 0) y = cBattleField.fGapVertical * -1f;
	//			else if (u1LocationIndex == 2) y = cBattleField.fGapVertical;
	//			break;
	//	}
	//	return y + cBattleField.cCrew0Center.Y;
	//}
}