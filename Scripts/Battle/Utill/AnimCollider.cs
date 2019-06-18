using UnityEngine;
using System.Collections;



public class AnimCollider : MonoBehaviour {

//	CapsuleCollider col;
//	SphereCollider col;
	MeshCollider col;

	BattleCharacter owner;

	bool bFix = false;
	public bool bStay = false;
	public bool bWall = false;
	bool bOut = false;

	Vector3 stayPos;

	public bool bFocus = false;

	CameraMove2 camMove;
	AnimationTestCam testCamMove;
	bool bTestScene = false;

	Collider currentCol;

	float maxRad = 0.0f;

	float colHeight = 0.1f;

	public void SetEnable(bool enable){
		this.enabled = enable;
		col.enabled = enable;
	}

	public void SetOwner (BattleCharacter user){
		owner = user;

		col = gameObject.GetComponent<MeshCollider>();

		float rad = owner.fDiameter*0.5f;

//		col.radius = rad;
//		maxRad = rad;
//
//		col.direction = 1;
//		col.center = new Vector3(0,rad,0);
//		col.height = rad*2f;

		col.transform.localScale = new Vector3(rad, 0.1f, rad);
		transform.position = new Vector3 (transform.parent.position.x, colHeight, transform.parent.position.z);
		
		col.isTrigger = true;

		if(Legion.Instance.eGameStyle != GameStyle.AnimTest){
			if(owner.iTeamIdx == 0) camMove = user.cBattle.cCameraMove2;
		}else{
			bTestScene = true;
			testCamMove = Camera.main.GetComponent<AnimationTestCam>();
		}
	}

	public void SetFocus (bool foc){
//		camMove.DisableSkillCam();
		bFocus = foc;
	}

	public void SetRun (bool bRun){
		if(bRun) col.enabled = false;
		else col.enabled = true;
	}

//	void OnCollisionEnter(Collision collision) {
//		foreach (ContactPoint contact in collision.contacts) {
//			DebugMgr.LogError(contact.thisCollider.transform.parent.parent.parent.name + " Col "+ contact.otherCollider.transform.parent.parent.parent.name);
//		}
//	}

	void OnTriggerEnter(Collider other) {
		//DebugMgr.LogError(transform.parent.parent.parent.name + " Col "+ other.transform.parent.parent.parent.name);
//		if(!bStay){
		owner.SaveCurrentBipPos();
//			currentCol = other;
//			bStay = true;
//		}
////		bStay = true;
////		bOut = false;
//		bStay = true;
		//owner.SaveCurrentBipPos();
		if(other.gameObject.tag == "Wall") bWall = true;
	}

	void OnTriggerStay(Collider other) {
		//DebugMgr.LogError(transform.parent.parent.parent.name + " Col "+ other.transform.parent.parent.parent.name);
//		if(!bStay){
		owner.SaveCurrentBipPos();
//		currentCol = other;
		if(other.tag != "Terrain") bStay = true;
//		if (owner.iTeamIdx == 0 && owner.cObject.name == "User1") {
//			DebugMgr.LogError (owner.cObject.name+" Stay " + other.transform.root.name);
//		}
//		}
		bFix = true;
	}

	void OnTriggerExit(Collider other) {
		//DebugMgr.LogError(transform.parent.parent.parent.name + " Col "+ other.transform.parent.parent.parent.name);
//		bOut = true;
		//if(currentCol == other){
//			bStay = false;
		//}
	}

	void LateUpdate(){
		//if(owner.rig.velocity == Vector3.zero){
//			if (bOut) {
//				if(owner.CheckBipDistIn()) bStay = false;
//			}
		bool result = bStay;
		if(owner.eCharacterState == CHAR_STATE.Move) result = false;
		else if(bWall) result = true;

		owner.CheckAnimationBipPos();
		owner.SubAnimatorBip01Fix(result);
		if(bTestScene) owner.TestSaveAnimationBipPos();
		else owner.SaveAnimationBipPos();
			//owner.SaveCurrentBipPos();
		//}
//		if (!bTestScene) {
//			if (bFocus) {
//				camMove.UpdateView();
//			}
//		}
		transform.parent.rotation = Quaternion.identity;
		transform.position = new Vector3 (transform.parent.position.x, colHeight, transform.parent.position.z);

		bFix = false;
		bOut = false;
		bStay = false;
	}

	public void SetShakeCam(CameraMove2.ShakeStyle eStyle, float fVibrationRange, float fVibrationTime){
		if (bTestScene) {
			testCamMove.SetShake (eStyle, fVibrationRange, fVibrationTime);
		}else{
			if (bFocus) {
				camMove.SetShake (eStyle, fVibrationRange, fVibrationTime);
			}
		}
	}
}
