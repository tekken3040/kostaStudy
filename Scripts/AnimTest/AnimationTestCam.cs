using UnityEngine;
using System;
using System.Collections;


public class AnimationTestCam : MonoBehaviour {
	
	public Camera cam;
	Transform camTransform;

	Byte u1ForcusCharIndex = 0;
	bool bMoveToFocus = false;
	float fMoveFocusTime = 0.0f;
	
	Vector3 originalPosBeforeVibration;
	//	ShakeStyle eVibrationStyle;
	//	float fVibrationMaxRange;
	float[] fVibrationDuration = new float[2];
	float[] fVibrationTick = new float[2];
	float[] fVibrationRemainTick = new float[2];
	Vector3[] vibrationVector = new Vector3[2];
	int[] iVibUp = new int[2]{1,1};
	
	float fTimeSum = 0f;
	
	bool bHaveToMove = false;

	Vector3 velocitySmooth;
	float fMoveDelay = 0.3f;
	float fDelayTime = 0f;
	Vector3 floorPoint;
	float viewDist;
	Vector3 lastViewPoint;
	
	float camDist = 2.0f;
	
	BattleCharacter cChar;
	
	Vector3 forward;
	Quaternion lastForward;
	
	bool bFixed = false;
	float fFixedTime = 0.0f;
	
	bool bStart = false;

	Byte u1Env = 1;

	void Awake(){
		camTransform = cam.transform;
		
		viewDist = Mathf.Sqrt( Mathf.Pow(LegionInfoMgr.Instance.fCameraDistance,2) + Mathf.Pow(LegionInfoMgr.Instance.fCameraHeight-LegionInfoMgr.Instance.fModelHeight,2) );
	}

	public void SetChar(BattleCharacter tChar){
		cChar = tChar;
	}

	public void SetDefault(){

	}

	public void UpdateView(){
		if(bFixed) return;

		for (int i=0; i<fVibrationDuration.Length; i++) {
			if (fVibrationDuration[i] > 0f) {
				fVibrationRemainTick[i] += Time.deltaTime;
				if (fVibrationRemainTick[i] >= fVibrationTick[i]) {
					iVibUp[i] *= -1;
					camTransform.position = originalPosBeforeVibration + vibrationVector[i] * (UnityEngine.Random.Range (0.1f, 1.0f) * iVibUp[i]);
					fVibrationRemainTick[i] -= fVibrationTick[i];
				}
				fVibrationDuration[i] -= Time.deltaTime;
				if (fVibrationDuration[i] <= 0f)
					fVibrationDuration[i] = 0f;
			}
		}
		
		Vector3 CharPos = cChar.cObject.transform.position;
		
		floorPoint = new Vector3(CharPos.x, LegionInfoMgr.Instance.fModelHeight, CharPos.z);
		
		//DebugMgr.Log(bMoveToFocus+"/"+bHaveToMove+"/"+floorPoint+"/"+lastViewPoint);
		
		if(!bMoveToFocus){
			if(!bHaveToMove){
				if(fDelayTime < fMoveDelay){
					fDelayTime += Time.deltaTime;
					return;
				}
				if(Quaternion.Angle(cChar.cObject.transform.rotation, lastForward) > LegionInfoMgr.Instance.fCameraRotateDistance){
					bHaveToMove = true;
					lastForward = cChar.cObject.transform.rotation;
					//lastFloorPoint = floorPoint;
					//						lastFloorPoint = floorPoint;
					//						lastForward = cChar.cObject.transform.forward;
				}else if(Vector3.Magnitude(camTransform.position - CharPos) > viewDist+LegionInfoMgr.Instance.fCameraMoveDistance){
					bHaveToMove = true;
					lastForward = cChar.cObject.transform.rotation;
					//lastFloorPoint = floorPoint;
					//						lastForward = cChar.cObject.transform.forward;
				}else{
					return;
				}
			}
		}
		
		forward = cChar.cObject.transform.forward;
		
		Vector3 pos = CharPos - forward * LegionInfoMgr.Instance.fCameraDistance + Vector3.up * LegionInfoMgr.Instance.fCameraHeight;
		
		WallCollision (floorPoint, ref pos);
		
		camTransform.position = Vector3.SmoothDamp(camTransform.position, pos, ref velocitySmooth, LegionInfoMgr.Instance.fCameraMoveSpd/2f);
		
		Quaternion targetRotation = Quaternion.LookRotation(floorPoint - camTransform.position);
		
		camTransform.rotation = Quaternion.Slerp(camTransform.rotation, targetRotation, Time.deltaTime*20f);
		//camTransform.LookAt (floorPoint);
		
		if(bHaveToMove){
			if(Mathf.Abs(camTransform.rotation.eulerAngles.y - cChar.cObject.transform.rotation.eulerAngles.y) < 3f && Vector3.Magnitude(camTransform.position - CharPos) <= viewDist+0.1f){
				//DebugMgr.LogError("AngleIn");
				bHaveToMove = false;
				bMoveToFocus = false;
				fDelayTime = 0f;
				lastForward = camTransform.rotation;
				lastViewPoint = floorPoint;
			}
		}
	}
	
	public void SetShake(CameraMove2.ShakeStyle eStyle, float fVibrationPower, float fVibrationTime)
	{
		if (cam == null) return;

		int array_idx = 0;

		switch(eStyle){
		case CameraMove2.ShakeStyle.TopAndBottom:
			array_idx = 0;
			if(vibrationVector[array_idx].y > fVibrationPower) return;
			vibrationVector[array_idx] = new Vector3(0, fVibrationPower, 0);  break;
		case CameraMove2.ShakeStyle.LeftAndRight:
			array_idx = 1;
			if(vibrationVector[array_idx].x > fVibrationPower) return;
			vibrationVector[array_idx] = new Vector3(fVibrationPower, 0, 0);  break;
		}

		
		fVibrationDuration[array_idx] = fVibrationTime;
		fVibrationTick[array_idx] = 0.05f;
		fVibrationRemainTick[array_idx] = 0;

		originalPosBeforeVibration = camTransform.position;
	}
	
	void WallCollision(Vector3 from, ref Vector3 target){
		Debug.DrawLine (from, target, Color.cyan);
		
		RaycastHit wallHit = new RaycastHit();
		if (Physics.Linecast (from, target, out wallHit)) {
			if(wallHit.collider.tag == "Wall"){
				Debug.DrawRay(wallHit.point, Vector3.left, Color.red);
				target = new Vector3(wallHit.point.x, target.y, wallHit.point.z);
			}
		}
	}
	
}
