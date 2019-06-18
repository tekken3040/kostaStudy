using UnityEngine;
using System.Collections;
using System;

public class AnimEvent : MonoBehaviour {
	
	BattleCharacter cOwner;
	
	bool bHead = false;
	
	public void InitHead(BattleCharacter owner){
		bHead = true;
		cOwner = owner;
	}
	
	public void InitHead(){
		bHead = true;
	}
	
	public void PlayVFX(AnimationEvent aEvent){
		if(!bHead) return;
		
		if(cOwner == null) return;
		
		SocketInfo sock = SocketInfoMgr.Instance.GetInfo ((ushort)aEvent.intParameter);
		
		if (aEvent.intParameter > 0 && sock == null) {
			//DebugMgr.LogWarning (gameObject.name + " Null Socket " + aEvent.intParameter);
			return;
		}

		if (cOwner.eCharacterState == CHAR_STATE.Attacking) {
			if (cOwner.bSkill) {
				if (!GraphicOption.attack_skill [Legion.Instance.graphicGrade])
					return;
			} else {
				if (!GraphicOption.attack_basic [Legion.Instance.graphicGrade])
					return;
			}
		}else if (cOwner.eCharacterState == CHAR_STATE.Move || cOwner.eCharacterState == CHAR_STATE.MovePortal) {
			if (!GraphicOption.run [Legion.Instance.graphicGrade])
				return;
		}
		
		string sockName = "";
		if(sock != null) sockName = sock.sSocBone;
		Transform sockTrans = cOwner.GetSocketTrans(sockName);
		if (aEvent.stringParameter != "") {
			DebugMgr.LogError(aEvent.stringParameter);
		} else {
			if (aEvent.objectReferenceParameter != null) {
				GameObject vfxObj = Instantiate (aEvent.objectReferenceParameter, sockTrans.position, sockTrans.rotation) as GameObject;
				
				if (aEvent.floatParameter > 0) {
					vfxObj.transform.parent = sockTrans;
					
					if (sock != null) {
						vfxObj.transform.localRotation = Quaternion.Euler (sock.fSocRot);
						vfxObj.transform.localPosition = sock.fSocLoc;
					} else {
						vfxObj.transform.localRotation = Quaternion.identity;
						vfxObj.transform.localPosition = Vector3.zero;
					}
				}
			} else {
				DebugMgr.LogError ("None GameObject PlayVFX");
			}
		}
	}
	
	public void Attack(AnimationEvent aEvent){
		if(!bHead) return;
		
		bool bAttach = aEvent.stringParameter == "T" || aEvent.stringParameter == "t";
		//7001
		cOwner.Attack((ushort)aEvent.intParameter, aEvent.floatParameter, bAttach);
		cOwner.bAvoid = false;
	}
	
	public void AttackRemove(string id){
		if(!bHead) return;
		
		Missile[] temp = transform.GetComponentsInChildren<Missile>();
		foreach (Missile target in temp) {
			if(target.gameObject.name == id){
				target.DeleteObject();
			}
		}
	}
	
	public void ShakeCam(AnimationEvent aEvent){
		if(!bHead) return;
		
		if (aEvent.stringParameter == "LR") {
			cOwner.a_col.SetShakeCam (CameraMove2.ShakeStyle.LeftAndRight, aEvent.floatParameter, (float)aEvent.intParameter /1000f);
		} else {
			cOwner.a_col.SetShakeCam(CameraMove2.ShakeStyle.TopAndBottom, aEvent.floatParameter, (float)aEvent.intParameter/1000f);
		}
	}
	
	public void EarthQuake(AnimationEvent aEvent){
		if(!bHead) return;
		
		if (aEvent.stringParameter == "LR") {
			if(cOwner.cBattle != null) cOwner.cBattle.cCameraMove2.SetShake (CameraMove2.ShakeStyle.LeftAndRight, aEvent.floatParameter, (float)aEvent.intParameter /1000f);
			else Camera.main.GetComponent<AnimationTestCam>().SetShake (CameraMove2.ShakeStyle.LeftAndRight, aEvent.floatParameter, (float)aEvent.intParameter /1000f);
		} else {
			if(cOwner.cBattle != null) cOwner.cBattle.cCameraMove2.SetShake (CameraMove2.ShakeStyle.TopAndBottom, aEvent.floatParameter, (float)aEvent.intParameter/1000f);
			else Camera.main.GetComponent<AnimationTestCam>().SetShake (CameraMove2.ShakeStyle.LeftAndRight, aEvent.floatParameter, (float)aEvent.intParameter /1000f);
		}
	}
	
	public void PlaySound(AudioClip clip){
		if(!bHead) return;

		if (clip == null) {
			#if UNITY_EDITOR
			//DebugMgr.LogError (cOwner.cCharacter.cClass.sName + "'s " + cOwner.subAnimator.GetCurrentAnimatorClipInfo (0) [0].clip.name+" is Not Set AudioClip");
			#endif
			return;
		}

		if (clip.name.Length > 5) {
			string haveVoice = clip.name.Remove (5);
			if (haveVoice == "Voice") {
				if (UnityEngine.Random.Range (0, 2) == 0) {
					return;
				}
			}
		}

		cOwner.PlayAudioClip(clip);
	}

	public void PlaySoundByObject(GameObject clip){
		if(!bHead) return;
		
		Instantiate (clip, cOwner.cObject.transform.position, Quaternion.identity);
	}
	
	public void JumpStart(){
		
	}
	
	public void JumpEnd(){
		
	}
	
	public void AvoidStart(){
		if(!bHead) return;
		
		cOwner.bAvoid = true;
	}
	
	public void AvoidEnd(){
		if(!bHead) return;
		
		cOwner.bAvoid = false;
	}
	
	public void AttackEnd(){
		if(!bHead) return;

		cOwner.SetAttackWait();
	}
	
	public void SetDeath(){
		if(!bHead) return;
		
		cOwner.StartRemoveMonster();
	}
}
