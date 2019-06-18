using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class CameraMove2 : MonoBehaviour {

	public enum ShakeStyle{
		TopAndBottom,
		LeftAndRight
	}

	public Camera cam;
	public Camera nightCam;
	public Transform camTransform;
	private Transform camParent;
	public Battle cBattle;

	Byte u1ForcusCharIndex = 0;
	bool bMoveToFocus = false;
	float fMoveFocusTime = 0.0f;
	
	Vector3 originalPosBeforeVibration;
//	ShakeStyle eVibrationStyle;
//	float fVibrationMaxRange;
	float fVibrationDuration;
	float fVibrationTick;
	float fVibrationRemainTick;
	Vector3 vibrationVector;
	int iVibUp = 1;
	
	bool bHaveToMove = false;

	Vector3 velocitySmooth;
	//float fMoveDelay = 0.3f;
	float fDelayTime = 0f;
	Vector3 floorPoint;
	float viewDist;
	float basicViewDist;
	Vector3 lastViewPoint;
	
	public BattleCharacter cChar;
	BattleCharacter cBoss;
	
	Vector3 forward;

	Transform skillCamParent;
	GameObject skillCam;

	public bool bFixed = true;
	float fFixedTime = 0.0f;

	bool bStart = false;
	public bool bCrew = true;

	SkillCamera skillCamScript;
	Animator skillCamAnimator;

	Byte u1Env = 1;

	Vector3 firstForward;
	Vector3 userRot;
	Vector3 beforeViewPoint;

	Vector3 firstTouchPoint;
	Vector3 lastTouchPoint;
	Vector3 v3Rotation;

	Vector3 v3MoveXY;

	bool _bPointerOverUIObject = false;
	float pinchPrevDistance;
	Vector2 touchPrevPosition;

	float fZoomValue = 1.0f;

	private Camera _cam { get { return u1Env == 1 ? cam : nightCam; } }

	float firstDist = 1.3f;
	float firstHeight = 0.5f;

	void Awake(){
		camTransform = _cam.transform;
		camParent = camTransform.parent;

		Input.multiTouchEnabled = true;

		skillCam = GameObject.FindGameObjectWithTag("SkillCam");
		skillCamScript = skillCam.GetComponent<SkillCamera>();
		skillCamAnimator = skillCam.GetComponent<Animator>();
		skillCamScript.SetMainCam(transform);
		skillCamParent = skillCam.transform.parent;
		skillCam.SetActive(false);

		basicViewDist = Mathf.Sqrt( Mathf.Pow(LegionInfoMgr.Instance.fBasicCameraDistance,2) + Mathf.Pow(LegionInfoMgr.Instance.fBasicCameraHeight,2) );
		viewDist = Mathf.Sqrt( Mathf.Pow(LegionInfoMgr.Instance.fCameraDistance,2) + Mathf.Pow(LegionInfoMgr.Instance.fCameraHeight,2) );
	}

	void OnDestroy(){
		Input.multiTouchEnabled = false;
	}

	public void SetStartCamera(){
		bCrew = true;
		SetCameraFix(true);

		cChar = cBattle.acCrews [0].acCharacters [0];

		if (cChar.isDead) {
			int idx = cChar.CheckLiveAlly ();
			cBattle.battleUIMgr.SetHeroDeselect (idx);
			SetFocus ((Byte)idx);
		} else {
			cBattle.battleUIMgr.SetHeroDeselect (0);
		}
	

		firstDist = 1.3f;
		firstHeight = 0.5f;


//		Vector3 CharPos = cChar.cObject.transform.position;
		Vector3 CharPos = GetAveragePos();
		forward = cChar.cObject.transform.forward;
		firstForward = forward;

//		floorPoint = new Vector3(CharPos.x, cChar.CheckGround()+LegionInfoMgr.Instance.fModelHeight, CharPos.z);
//		Vector3 pos = floorPoint + firstForward * LegionInfoMgr.Instance.fCameraDistance + Vector3.up * 0.5f;
		floorPoint = new Vector3(CharPos.x, cChar.CheckGround()+0.9f, CharPos.z);
		Vector3 pos = floorPoint + firstForward * firstDist + Vector3.up * firstHeight;
		//v3Rotation.y = -180f;

		camParent.position = floorPoint;

		camParent.LookAt (pos);
		
		camTransform.position = pos;

		camTransform.LookAt (floorPoint);

		v3Rotation = camParent.rotation.eulerAngles;

//		camParent.rotation = Quaternion.Euler (v3Rotation);

		StartCoroutine(StartDirection());
	}

	public void SetEnviroment(Byte u1Type){
		if (u1Type == 1) {
			cam.enabled = true;
			nightCam.enabled = false;
		} else if (u1Type == 2) {
			cam.enabled = false;
			nightCam.enabled = true;
		}

		u1Env = u1Type;
	}

//	public bool PlaySkillCamAnim(string animName){
//		if(skillCamAnimator.runtimeAnimatorController.animationClips != null) {
//			for (int i = 0; i < skillCamAnimator.runtimeAnimatorController.animationClips.Length; i++) {
//				DebugMgr.Log(skillCamAnimator.runtimeAnimatorController.animationClips [i].name +"/"+animName);
//				if (skillCamAnimator.runtimeAnimatorController.animationClips [i].name == animName) {
//					SetSkillCam (animName);
//					return true;
//				}
//			}
//		}
//
//		return false;
//	}

	IEnumerator StartDirection(){
		float time = 0f;

		Vector3 CharPos = GetAveragePos();
		floorPoint = new Vector3 (CharPos.x, cChar.CheckGround()+firstHeight, CharPos.z);

		camParent.localScale = Vector3.one;
		camParent.position = floorPoint;

		while (time < 1.5f) {
			camTransform.LookAt (camParent.position);
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		float step = 0;
		float rate = 1.2f;
		float smoothStep = 0f;
		float lastStep = 0f;

		float dist = firstDist;
		float height = firstHeight;

		float fRot = camParent.rotation.eulerAngles.y;

		while (time < 2.4f) {
			step += Time.deltaTime * rate;
			smoothStep = Mathf.SmoothStep(fRot,fRot-180f,step);

			v3Rotation.y = smoothStep;

			camParent.rotation = Quaternion.Euler (v3Rotation);
			camTransform.LookAt (camParent.position);

			lastStep = smoothStep;

			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		step = 0;
		floorPoint = new Vector3 (CharPos.x, cChar.CheckGround()+LegionInfoMgr.Instance.fBasicCamaraViewPtHeight, CharPos.z);

		while (dist < LegionInfoMgr.Instance.fBasicCameraDistance) {
			step += Time.deltaTime * rate;

			camParent.position = Vector3.Slerp (camParent.position, floorPoint, 0.05f);

			dist = Mathf.SmoothStep(firstDist,LegionInfoMgr.Instance.fBasicCameraDistance,step);
			height = Mathf.SmoothStep(firstHeight,LegionInfoMgr.Instance.fBasicCameraHeight,step);

			Vector3 camPos = camParent.position - firstForward * dist + Vector3.up * height;

			WallCollision (camParent.position, ref camPos);

			camTransform.position = camPos;
			camTransform.LookAt (camParent.position);
			yield return new WaitForEndOfFrame ();
		}

		SetCameraFix (false);
	}

	public float SetBossCamera(){
		SetCameraFix(true);
		userRot = Vector3.zero;
		Vector3 CharPos = cChar.cObject.transform.position;
		floorPoint = new Vector3(CharPos.x, CharPos.y+LegionInfoMgr.Instance.fModelHeight, CharPos.z);
		camParent.position = floorPoint;

		lastViewPoint = floorPoint;

		for(int i=0; i<cBattle.acCrews[1].acCharacters.Length; i++){
			ClassInfo info = cBattle.acCrews[1].acCharacters[i].cCharacter.cClass;
			if(info.u1MonsterType == 2){
				cBoss = cBattle.acCrews[1].acCharacters[i];
				string dirName = info.sDirectionCam;

				if (info.sDirectionCam == "0")
					dirName = "FortGolemDirection";

				SetDirectionCam(dirName);

				cBoss.PlayDirection();
				cBoss.SetEnableAnimator (false);

				return GetAnimLength(dirName);
			}
		}

		return 6f;
	}

	public void EnableSkillAnimator(){
		skillCamAnimator.enabled = true;
		cBoss.SetEnableAnimator (true);
	}

	public void SetDefeatCam(BattleCharacter btChar){
		if (Legion.Instance.cTutorial.au1Step [0] != Server.ConstDef.LastTutorialStep)
			return;
		
		bFixed = true;

		skillCam.SetActive(true);

		skillCamParent.position = btChar.GetBodyPos();
		skillCamParent.rotation = Quaternion.identity;

		skillCamScript.enabled = false;
		skillCamAnimator.Play("Defeat");
		_cam.enabled = false;

		if(cBattle.eGameStyle == GameStyle.Stage) StartCoroutine (UpdateDefeatLookAt (btChar));
	}

	IEnumerator UpdateDefeatLookAt(BattleCharacter btChar){
		while (skillCam.activeSelf) {
			yield return new WaitForEndOfFrame ();
			skillCamParent.position = btChar.GetBodyPos();
		}
	}

	public void SetResultCam(){
		if (!_cam.enabled) {
			DisableDirectionCam ();
		}

		bFixed = true;
		cChar = cBattle.acCrews[0].acCharacters[0];

		Vector3 CharPos = cChar.cObject.transform.position;
		floorPoint = new Vector3(CharPos.x, CharPos.y+0.5f, CharPos.z);

		Vector3 left = Quaternion.Euler (new Vector3 (0, -90, 0)) * cChar.cObject.transform.forward;

		forward = cChar.cObject.transform.forward;

		Vector3 pos = floorPoint+ forward * 2.5f + left*0.3f + Vector3.up * 0.4f;

		camTransform.position = pos;

		camTransform.LookAt (floorPoint+(left*0.8f));

		_cam.fieldOfView = 50 * Legion.Instance.ratio;
	}

	public void SetCameraFix(bool bVal){
		bFixed = bVal;
	}

	public bool ChangeCameraType(){
		if (bCrew) {
			userRot = Vector3.zero;
			bMoveToFocus = false;
			bHaveToMove = false;
			Vector3 CharPos = cChar.cObject.transform.position;
			floorPoint = new Vector3 (CharPos.x, cChar.CheckGround()+LegionInfoMgr.Instance.fModelHeight, CharPos.z);
			Vector3 camPos = floorPoint - cChar.cObject.transform.forward * LegionInfoMgr.Instance.fCameraDistance + Vector3.up * LegionInfoMgr.Instance.fCameraHeight;
			camParent.position = floorPoint;
			camParent.LookAt (camPos);

			camTransform.position = camPos;

			camTransform.LookAt (floorPoint);
			//v3Rotation = camParent.rotation.eulerAngles;
			bCrew = false;
		} else {
			userRot = Vector3.zero;
			bMoveToFocus = false;
			bHaveToMove = false;
			Vector3 CharPos = GetAveragePos();
			floorPoint = new Vector3 (CharPos.x, cChar.CheckGround()+LegionInfoMgr.Instance.fBasicCamaraViewPtHeight, CharPos.z);
			camParent.position = floorPoint;
			camTransform.position = floorPoint - firstForward*LegionInfoMgr.Instance.fBasicCameraDistance + Vector3.up*LegionInfoMgr.Instance.fBasicCameraHeight;
			camTransform.LookAt (floorPoint);
			bCrew = true;
		}

		return bCrew;
	}

	public void SetFocus(Byte u1CharIndex)
	{
		if (!_cam.enabled) {
			DisableDirectionCam ();
		}

		u1ForcusCharIndex = (Byte)(u1CharIndex);
		cChar = cBattle.acCrews[0].acCharacters[u1ForcusCharIndex];
	}

	public void SetDirectionCam(string anim, bool bEnable = false){
		if(anim == "") return;

		skillCam.SetActive(true);

		skillCamParent.position = cBoss.cObject.transform.position;
		skillCamParent.rotation = cBoss.cObject.transform.rotation;

		skillCamScript.SetTarget(cBoss.cObject.transform);
		skillCamAnimator.Play(anim);
		skillCamAnimator.enabled = bEnable;
		_cam.enabled = false;
	}

//	public void SetSkillCam(string anim){
//		return;
//		if(anim == "") return;
//
//		skillCam.SetActive(true);
//		skillCamScript.SetTarget(cChar.cObject.transform);
//		skillCamAnimator.Play(anim);
//		_cam.enabled = false;
//	}

	public void DisableDirectionCam(){
		if(_cam.enabled) return;

		if(cBattle.TutorialCheckType == 1 && cBattle.TutorialCheckStep >= (int)TutorialSubPart.BOSS_SKILL) return; 

		skillCamScript.InitParam();
		skillCam.SetActive(false);
		SetCameraFix (false);
		_cam.enabled = true;
	}
    //현재 UI가 터치 되었는지 판별
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
 
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    private bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = screenPosition;
 
        GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    //터치된 오브젝트 가져오기
    List<RaycastResult> currentSelectUIObject;
    private List<RaycastResult> GetPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        currentSelectUIObject = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, currentSelectUIObject);
        return currentSelectUIObject;
    }

	void CheckTouch(){
#if UNITY_EDITOR
		if (Input.GetMouseButtonDown (0)) {
			firstTouchPoint = Input.mousePosition;
			lastTouchPoint = firstTouchPoint;

            if(EventSystem.current.IsPointerOverGameObject()){
				//if(EventSystem.current.gameObject.layer == LayerMask.NameToLayer("UI"))
                if(GetPointerOverUIObject().Count > 0)
                if(GetPointerOverUIObject()[0].gameObject.layer == LayerMask.NameToLayer("UI"))
				{
					_bPointerOverUIObject = true;
					v3MoveXY = Vector3.zero;
					return;
				}
			}
		} else if (Input.GetMouseButton (0)) {
			if(_bPointerOverUIObject) return;

			//if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()){
			//	if(UnityEngine.EventSystems.EventSystem.current.gameObject.layer == LayerMask.NameToLayer("UI"))
			//	{
			//		_bPointerOverUIObject = true;
			//		v3MoveXY = Vector3.zero;
			//		return;
			//	}
			//}

			v3MoveXY.x = Input.mousePosition.x - lastTouchPoint.x;
			v3MoveXY.y = Input.mousePosition.y - lastTouchPoint.y;
			lastTouchPoint = Input.mousePosition;
		} else if (Input.GetMouseButtonUp (0)) {
			v3MoveXY = Vector3.zero;
			_bPointerOverUIObject = false;
		}

		if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
		{
			fZoomValue -= 0.03f;
			if(fZoomValue < LegionInfoMgr.Instance.fCameraDistance/LegionInfoMgr.Instance.fMaxCameraDistance)
				fZoomValue = LegionInfoMgr.Instance.fCameraDistance/LegionInfoMgr.Instance.fMaxCameraDistance;

			PlayerPrefs.SetFloat ("CamZoom", fZoomValue);
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
		{
			fZoomValue += 0.03f;
			if(fZoomValue > LegionInfoMgr.Instance.fCameraDistance/LegionInfoMgr.Instance.fMinCameraDistance)
				fZoomValue = LegionInfoMgr.Instance.fCameraDistance/LegionInfoMgr.Instance.fMinCameraDistance;

			PlayerPrefs.SetFloat ("CamZoom", fZoomValue);
		}

#elif UNITY_ANDROID || UNITY_IOS
		if (Input.touchCount == 2)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				pinchPrevDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()){
					//if(UnityEngine.EventSystems.EventSystem.current.gameObject.layer == LayerMask.NameToLayer("UI"))
                    if(GetPointerOverUIObject().Count > 0)
                    if(GetPointerOverUIObject()[0].gameObject.layer == LayerMask.NameToLayer("UI"))
					{
						_bPointerOverUIObject = true;
						v3MoveXY = Vector3.zero;
						return;
					}
				}
			}

			if(Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
			{
				if(_bPointerOverUIObject) return;

				//if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()){
				//	if(UnityEngine.EventSystems.EventSystem.current.gameObject.layer == LayerMask.NameToLayer("UI"))
				//	{
				//		_bPointerOverUIObject = true;
				//		v3MoveXY = Vector3.zero;
				//		return;
				//	}
				//}

				float pinchCurrentDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
				float pinchDeltaDistance = pinchCurrentDistance - pinchPrevDistance;
				fZoomValue -= pinchDeltaDistance*0.01f;
				if(fZoomValue < LegionInfoMgr.Instance.fCameraDistance/LegionInfoMgr.Instance.fMaxCameraDistance){
					fZoomValue = LegionInfoMgr.Instance.fCameraDistance/LegionInfoMgr.Instance.fMaxCameraDistance;
				}else if(fZoomValue > LegionInfoMgr.Instance.fCameraDistance/LegionInfoMgr.Instance.fMinCameraDistance){
					fZoomValue = LegionInfoMgr.Instance.fCameraDistance/LegionInfoMgr.Instance.fMinCameraDistance;
				}
				PlayerPrefs.SetFloat ("CamZoom", fZoomValue);
				pinchPrevDistance  = pinchCurrentDistance;
			}
		}
		else if (Input.touchCount == 1)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				touchPrevPosition = Input.GetTouch(0).position;
                if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()){
					//if(UnityEngine.EventSystems.EventSystem.current.gameObject.layer == LayerMask.NameToLayer("UI"))
                    if(GetPointerOverUIObject().Count > 0)
                    if(GetPointerOverUIObject()[0].gameObject.layer == LayerMask.NameToLayer("UI"))
					{
						_bPointerOverUIObject = true;
						v3MoveXY = Vector3.zero;
						return;
					}
				}
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				if(_bPointerOverUIObject) return;

				//if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()){
				//	if(UnityEngine.EventSystems.EventSystem.current.gameObject.layer == LayerMask.NameToLayer("UI"))
				//	{
				//		_bPointerOverUIObject = true;
				//		v3MoveXY = Vector3.zero;
				//		return;
				//	}
				//}
				// Get movement of the finger since last frame
				Vector2 touchDeltaPosition = (Input.GetTouch(0).position - touchPrevPosition);
				v3MoveXY.x = touchDeltaPosition.x;
				v3MoveXY.y = touchDeltaPosition.y;
				touchPrevPosition = Input.GetTouch(0).position;
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				v3MoveXY = Vector3.zero;
				_bPointerOverUIObject = false;
			}	
		}
#endif
	}
	
	void LateUpdate(){
		if(bFixed)
			return;

		if (cBattle == null)
			return;

		if (cBattle.acCrews [0] == null)
			return;

		if (cChar == null) {
			for (int i = 0; i < cBattle.acCrews [0].acCharacters.Length; i++) {
				if (cBattle.acCrews [0].acCharacters[i] != null) {
					cBattle.battleUIMgr.OnSelectCharaceter (cBattle.acCrews [0].acCharacters[i].CheckLiveAlly ());
					return;
				}
			}
			return;
		}

		if (cChar.cObject == null) {
			return;
		}

		if (fVibrationDuration > 0f)
		{
			fVibrationRemainTick += Time.deltaTime;
			if (fVibrationRemainTick >= fVibrationTick)
			{
				iVibUp *= -1;
				camParent.position = originalPosBeforeVibration + vibrationVector * (UnityEngine.Random.Range(0.1f, 1.0f)*iVibUp);
				fVibrationRemainTick -= fVibrationTick;
			}
			fVibrationDuration -= Time.deltaTime;
			if(fVibrationDuration <= 0f) camParent.position = originalPosBeforeVibration;
		}

		float fCrewZoomPer = 1.0f;
			
		if (bCrew) {
			Vector3 avg = GetAveragePos ();
			float maxDist = 0f;

			for(int i=0; i<cBattle.acCrews[0].acCharacters.Length; i++){
				if (!cBattle.acCrews [0].acCharacters [i].isDead && cBattle.acCrews [0].acCharacters [i].cObject != null) {
					float dist = Vector3.Magnitude (cBattle.acCrews [0].acCharacters [i].cObject.transform.position - avg);
					if (dist > maxDist) maxDist = dist;
				}
			}

			if (LegionInfoMgr.Instance.fMinCharctersSpace > maxDist) {
				float per = maxDist / LegionInfoMgr.Instance.fMinCharctersSpace;
				if (per < (100f - LegionInfoMgr.Instance.fMinCharSpaceZoomPer) / 100f) {
					per = (100f - LegionInfoMgr.Instance.fMinCharSpaceZoomPer) / 100f;
				}

				if(per < fCrewZoomPer) fCrewZoomPer = per;
			}else if (LegionInfoMgr.Instance.fMaxCharctersSpace < maxDist) {
				float per = maxDist / LegionInfoMgr.Instance.fMaxCharctersSpace;
				if (per > (100f + LegionInfoMgr.Instance.fMaxCharSpaceZoomPer) / 100f) {
					per = (100f + LegionInfoMgr.Instance.fMaxCharSpaceZoomPer) / 100f;
				}

				if(per > fCrewZoomPer) fCrewZoomPer = per;
			}

			Vector3 CharPos = cChar.cObject.transform.position;
			floorPoint = new Vector3 (avg.x, CharPos.y+LegionInfoMgr.Instance.fBasicCamaraViewPtHeight, avg.z);
		}else{
			Vector3 CharPos = cChar.cObject.transform.position;
			floorPoint = new Vector3 (CharPos.x, CharPos.y + LegionInfoMgr.Instance.fModelHeight, CharPos.z);
		}

		CheckTouch ();

		bool bRotate = false;

		if (v3MoveXY != Vector3.zero) {
			userRot.x += v3MoveXY.y * Time.fixedDeltaTime * 30f;
			userRot.y += v3MoveXY.x * Time.fixedDeltaTime * 30f;
			userRot.z = 0;
			//좌우
			if (LegionInfoMgr.Instance.fMaxCameraRotationLR < 360f) {
				if (userRot.y >  LegionInfoMgr.Instance.fMaxCameraRotationLR / 2f)
					userRot.y = LegionInfoMgr.Instance.fMaxCameraRotationLR / 2f;
				else if (userRot.y < -LegionInfoMgr.Instance.fMaxCameraRotationLR / 2f)
					userRot.y = -LegionInfoMgr.Instance.fMaxCameraRotationLR / 2f;
			}

			//상하
			if (userRot.x > LegionInfoMgr.Instance.fMaxCameraRotationTop)
				userRot.x = LegionInfoMgr.Instance.fMaxCameraRotationTop;
			else if (userRot.x < -LegionInfoMgr.Instance.fMaxCameraRotationBottom)
				userRot.x = -LegionInfoMgr.Instance.fMaxCameraRotationBottom;

			//bHaveToMove = true;
			bRotate = true;
		}


		if (bCrew) {
			Vector3 dir = (-(cChar.cObject.transform.forward) * LegionInfoMgr.Instance.fBasicCameraDistance * fCrewZoomPer)
			              + (Vector3.up * LegionInfoMgr.Instance.fBasicCameraHeight * fCrewZoomPer);
			
			Vector3 camPos = floorPoint + Quaternion.Euler(userRot)*dir;
			camParent.position = floorPoint;
			camPos = Vector3.Slerp (camTransform.position, camPos, LegionInfoMgr.Instance.fCameraMoveSpd * 0.2f);
			//camParent.LookAt (Vector3.Slerp (camTransform.position, camPos, LegionInfoMgr.Instance.fCameraMoveSpd * 0.2f));
			//camParent.Rotate(userRot);
			WallCollision (floorPoint, ref camPos);
			camTransform.position = camPos;
			camTransform.LookAt (floorPoint);
			bMoveToFocus = false;
			bHaveToMove = false;
			return;
		} else {
			Vector3 dir = -(cChar.cObject.transform.forward) * LegionInfoMgr.Instance.fCameraDistance + Vector3.up * LegionInfoMgr.Instance.fCameraHeight;

			Vector3 camPos = floorPoint + Quaternion.Euler(userRot)*dir;
			camParent.LookAt (Vector3.Slerp (camTransform.position, camPos, LegionInfoMgr.Instance.fCameraMoveSpd * 0.2f));
			//camParent.Rotate(userRot);
		}

		camParent.localScale = Vector3.one * fCrewZoomPer;

		if (!bMoveToFocus) {
			if (!bHaveToMove) {
				if (Vector3.Magnitude (lastViewPoint - floorPoint) > LegionInfoMgr.Instance.fCameraMoveDistance / 2f) {
					bHaveToMove = true;
					lastViewPoint = Vector3.Slerp (camParent.position, floorPoint, LegionInfoMgr.Instance.fCameraMoveSpd * 0.2f);
				} else {
					return;
				}
			} else {
				lastViewPoint = floorPoint;
			}
		} else {
			lastViewPoint = Vector3.Slerp (camParent.position, floorPoint, LegionInfoMgr.Instance.fCameraMoveSpd * 0.2f);
		}

		WallCollision (camParent.position, ref lastViewPoint);

		if(bHaveToMove){
			camParent.position = lastViewPoint;
			camTransform.LookAt (lastViewPoint);

			if (bMoveToFocus){
				float dist = Vector3.Magnitude (floorPoint - lastViewPoint);
				if (dist <= 0.1f) {
					bHaveToMove = false;
					bMoveToFocus = false;
					fDelayTime = 0f;
					lastViewPoint = floorPoint;
				}
			}
		}
	}

	Vector3 GetAveragePos(){
		if (cBattle.acCrews [0] == null)
			return Vector3.zero;

		Vector3 avg = Vector3.zero;
		float maxDist = 0;
		float count = 0f;
		for(int i=0; i<cBattle.acCrews[0].acCharacters.Length; i++){
			if (cBattle.acCrews [0].acCharacters [i] != null && !cBattle.acCrews [0].acCharacters [i].isDead) {
				if (cBattle.acCrews [0].acCharacters [i].cObject == null)
					continue;
				
				avg += cBattle.acCrews [0].acCharacters [i].cObject.transform.position;
				count++;
			}
		}
		return avg / count;
	}

	public void SetShake(ShakeStyle eStyle, float fVibrationPower, float fVibrationTime)
	{
		if (_cam == null) return;
		originalPosBeforeVibration = camParent.position;

//		eVibrationStyle = eStyle;
//		fVibrationMaxRange = fVibrationPower;

		fVibrationDuration = fVibrationTime;
		fVibrationTick = 0.05f;
		fVibrationRemainTick = 0;
		switch(eStyle){
		case ShakeStyle.TopAndBottom: vibrationVector = new Vector3(0, fVibrationPower, 0); break;
		case ShakeStyle.LeftAndRight: vibrationVector = new Vector3(fVibrationPower, 0, 0); break;
		}
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

	private float GetAnimLength(string animName){
		return ClassInfoMgr.Instance.GetAnimLength (animName);
//		if(skillCamAnimator.runtimeAnimatorController.animationClips != null) {
//			for(int i = 0; i < skillCamAnimator.runtimeAnimatorController.animationClips.Length; i++) {
//				if(skillCamAnimator.runtimeAnimatorController.animationClips[i].name == animName) {
//					return skillCamAnimator.runtimeAnimatorController.animationClips[i].length;
//				}
//			}
//		}
//
//		return 2f;
	}
}
