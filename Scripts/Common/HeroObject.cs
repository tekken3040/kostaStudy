using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public class HeroObject : MonoBehaviour {

	Hero cHero;
	Dictionary<string, Transform> _dicCostumeTransforms; // 모델ID, 해당모델의Transform 저장.
	Dictionary<string, Transform> _dicBoneTransforms;
	Transform _animatorTr;
	Animator _headAnimator;
	SkinnedMeshRenderer _headRenderer;

	HairColorInfo _curHairColor;
    
    public bool initData = false;
	void Awake()
	{
		_dicCostumeTransforms = new Dictionary<string, Transform>();
		_dicBoneTransforms = new Dictionary<string, Transform>();
		_animatorTr = transform.FindChild("Animator");
		Quaternion rot = new Quaternion();
		rot.eulerAngles = new Vector3(270f, 0f, 0f);
		_animatorTr.localRotation = rot;
	}

	public void SetData(Hero hero, bool sync = false)
	{
        initData = false;
		cHero = hero;
		//InitCostume();
        
        if(sync)
            StartCoroutine(CoInitCostume());
        else    
            InitCostume();
	}

	public void InitCostume()
	{
//		DebugMgr.Log(cHero.u1SelectedFace-1);
//		DebugMgr.Log(cHero.u1SelectedHair-1);
//		DebugMgr.Log(cHero.u1SelectedHairColor-1);

		// 얼굴 모델 생성.
		ModelInfo faceModelInfo = cHero.cClass.lstFaceInfo[cHero.u1SelectedFace-1].cModelInfo;
		CreateObject(faceModelInfo);

		_headAnimator = GetFace ().GetComponent<Animator> ();
		_headRenderer = _headAnimator.GetComponentInChildren<SkinnedMeshRenderer> ();
		// 얼굴 모델을 기준으로 Bone Transforms 초기화.
		SetBones(GetFace());

		// 헤어 모델 생성.
		ModelInfo hairModelInfo = cHero.cClass.lstHairInfo[cHero.u1SelectedHair-1].cModelInfo;
		CreateObject(hairModelInfo);

		// 헤어 컬러 적용.
		HairColorInfo hairColorInfo = cHero.cClass.lstHairColor[cHero.u1SelectedHairColor-1];
		SetHairColor(hairColorInfo);

		// 장비 모델 생성.
		for(int equipIdx=0; equipIdx<ClassInfo.MAX_BASEEQUIP_OF_MODEL; equipIdx++)
		{
			if(cHero.acEquips[equipIdx] == null) continue;
			EquipmentInfo equipInfo = cHero.acEquips[equipIdx].GetEquipmentInfo();
			if(equipInfo.u1PosID == EquipmentInfo.EQUIPMENT_POS.HELMET && equipInfo.bRemoveHair)
			{
				for(int modelIdx=0; modelIdx<hairModelInfo.acModels.Length; modelIdx++)
				{
					Transform hairModelTr = null;
					_dicCostumeTransforms.TryGetValue(hairModelInfo.acModels[modelIdx].sModelFilename, out hairModelTr);
					hairModelTr.gameObject.SetActive(false);
				}
			}
			if(cHero.acEquips[equipIdx].u2ModelID != 0)
			{
				CreateObject(ModelInfoMgr.Instance.GetInfo(cHero.acEquips[equipIdx].u2ModelID));
			}
			else
			{
				CreateObject(equipInfo.cModel);
			}

//			DebugMgr.Log("InitCostume AddEQuip : " + cHero.acEquips[equipIdx].cItemInfo.sName);
		}
			

		// 그림자 크기 조절.
        OnOffHelmet(ObscuredPrefs.GetBool("HelmetToggle", true));
        initData = true;
	}

	public IEnumerator CoInitCostume()
	{
//		DebugMgr.Log(cHero.u1SelectedFace-1);
//		DebugMgr.Log(cHero.u1SelectedHair-1);
//		DebugMgr.Log(cHero.u1SelectedHairColor-1);

		// 얼굴 모델 생성.
		ModelInfo faceModelInfo = cHero.cClass.lstFaceInfo[cHero.u1SelectedFace-1].cModelInfo;
		yield return StartCoroutine(CoCreateObject(faceModelInfo));

		_headAnimator = GetFace ().GetComponent<Animator> ();
		_headRenderer = _headAnimator.GetComponentInChildren<SkinnedMeshRenderer> ();

		// 얼굴 모델을 기준으로 Bone Transforms 초기화.
		SetBones(GetFace());

		// 헤어 모델 생성.
		ModelInfo hairModelInfo = cHero.cClass.lstHairInfo[cHero.u1SelectedHair-1].cModelInfo;
		yield return StartCoroutine(CoCreateObject(hairModelInfo));

		// 헤어 컬러 적용.
		HairColorInfo hairColorInfo = cHero.cClass.lstHairColor[cHero.u1SelectedHairColor-1];
		SetHairColor(hairColorInfo);

		// 장비 모델 생성.
		for(int equipIdx=0; equipIdx<ClassInfo.MAX_BASEEQUIP_OF_MODEL; equipIdx++)
		{
			if(cHero.acEquips[equipIdx] == null) continue;
			EquipmentInfo equipInfo = cHero.acEquips[equipIdx].GetEquipmentInfo();
			if(equipInfo.u1PosID == EquipmentInfo.EQUIPMENT_POS.HELMET && equipInfo.bRemoveHair)
			{
				for(int modelIdx=0; modelIdx<hairModelInfo.acModels.Length; modelIdx++)
				{
					Transform hairModelTr = null;
					_dicCostumeTransforms.TryGetValue(hairModelInfo.acModels[modelIdx].sModelFilename, out hairModelTr);
					hairModelTr.gameObject.SetActive(false);
				}
			}
			if(cHero.acEquips[equipIdx].u2ModelID != 0)
			{
				yield return StartCoroutine(CoCreateObject(ModelInfoMgr.Instance.GetInfo(cHero.acEquips[equipIdx].u2ModelID)));
			}
			else
			{
				yield return StartCoroutine(CoCreateObject(equipInfo.cModel));
			}

//			DebugMgr.Log("InitCostume AddEQuip : " + cHero.acEquips[equipIdx].cItemInfo.sName);
		}
			

		// 그림자 크기 조절.
        OnOffHelmet(ObscuredPrefs.GetBool("HelmetToggle", true));
        initData = true;
	}

	void SetBones(Transform baseObject)
	{
		Transform[] boneTransforms = baseObject.GetChild(0).GetComponentsInChildren<Transform>();
		for(int i=0; i<boneTransforms.Length; i++)
		{
			_dicBoneTransforms.Add(boneTransforms[i].name, boneTransforms[i]);
		}
	}
	
	void ClearBones()
	{
		_dicBoneTransforms.Clear();
	}

	public Transform GetBones(string boneName)
	{
		if(_dicBoneTransforms.ContainsKey(boneName) == false)
			return null;

		return _dicBoneTransforms[boneName];
	}

	public void CreateObject(ModelInfo modelInfo)
	{
		Dictionary<string, Transform> dicObjects = ModelInfoMgr.Instance.GetModelObjects(modelInfo, cHero.cClass.u2ID);
		int modelIdx=0;
		foreach(KeyValuePair<string, Transform> element in dicObjects)
		{
			if(_dicCostumeTransforms.ContainsKey(element.Key)){
				_dicCostumeTransforms.Add(element.Key+"L", element.Value);
			}else{
				_dicCostumeTransforms.Add(element.Key, element.Value);
			}

			if(modelInfo.acModels[modelIdx].cSocketInfo != null)
			{
                //DebugMgr.LogError(modelInfo.u2ID);
				element.Value.parent = _dicBoneTransforms[modelInfo.acModels[modelIdx].cSocketInfo.sSocBone];

				element.Value.localPosition = modelInfo.acModels[modelIdx].cSocketInfo.fSocLoc;
				element.Value.localRotation = Quaternion.Euler(modelInfo.acModels[modelIdx].cSocketInfo.fSocRot);
				element.Value.localScale = Vector3.one;
			}
			else
			{
				element.Value.parent = _animatorTr;

				element.Value.localPosition = Vector3.zero;
				Quaternion rot = new Quaternion();
				rot.eulerAngles = Vector3.zero;
				element.Value.localRotation = rot;
				element.Value.localScale = Vector3.one;
				AddParts (element.Value.gameObject);
			}


			modelIdx++;
		}
	}
    
	public IEnumerator CoCreateObject(ModelInfo modelInfo)
	{
        Dictionary<string, Transform> dicObjects = null;
        yield return StartCoroutine(ModelInfoMgr.Instance.CoGetModelObjects((x) => dicObjects = x, modelInfo, cHero.cClass.u2ID));
        
		int modelIdx=0;
		foreach(KeyValuePair<string, Transform> element in dicObjects)
		{
			if(_dicCostumeTransforms.ContainsKey(element.Key)){
				_dicCostumeTransforms.Add(element.Key+"L", element.Value);
			}else{
				_dicCostumeTransforms.Add(element.Key, element.Value);
			}
			
			if(modelInfo.acModels[modelIdx].cSocketInfo != null)
			{
                //DebugMgr.LogError(modelInfo.u2ID);
				element.Value.parent = _dicBoneTransforms[modelInfo.acModels[modelIdx].cSocketInfo.sSocBone];
				element.Value.localPosition = modelInfo.acModels[modelIdx].cSocketInfo.fSocLoc;
				element.Value.localRotation = Quaternion.Euler(modelInfo.acModels[modelIdx].cSocketInfo.fSocRot);
				element.Value.localScale = Vector3.one;
			}
			else
			{
				element.Value.parent = _animatorTr;
				element.Value.localPosition = Vector3.zero;
				Quaternion rot = new Quaternion();
				rot.eulerAngles = Vector3.zero;
				element.Value.localRotation = rot;
				element.Value.localScale = Vector3.one;
				AddParts (element.Value.gameObject);
			}

			
			modelIdx++;
		}
	}    

	public Transform GetFace()
	{
		string faceFileName = cHero.cClass.lstFaceInfo[cHero.u1SelectedFace-1].cModelInfo.acModels[0].sModelFilename;
		return _dicCostumeTransforms[faceFileName];
	}

	public void ChangeFace(ModelInfo oldFaceModelInfo, ModelInfo newFaceModelInfo)
	{
		// 얼굴본에 장착되어있는 무기 해제.
		BackupSocketObjects();

		// 기존 얼굴 모델 제거.
		RemoveObjects(oldFaceModelInfo);
			
		// 새로운 얼굴 모델 추가 후 메인 본 새로 적용.
		CreateObject(newFaceModelInfo);
		SetHairColor (_curHairColor);

		_headRenderer.enabled = false;

		// 해제되었던 무기 얼굴본에 다시 장착.
		RestoreSocketObjects();
	}


	public Dictionary<string, Transform> GetHair()
	{
		Dictionary<string, Transform> ret = new Dictionary<string, Transform>();
		for(int modelIdx=0; modelIdx<cHero.cClass.lstHairInfo[cHero.u1SelectedHair-1].cModelInfo.u1ModelCount; modelIdx++)
		{
			string modelName = cHero.cClass.lstHairInfo[cHero.u1SelectedHair-1].cModelInfo.acModels[modelIdx].sModelFilename;
			ret.Add(modelName, _dicCostumeTransforms[modelName]);
		}
		return ret;
	}

	public void ChangeHair(ModelInfo oldHairModelInfo, ModelInfo newHairModelInfo)
	{
		//if(oldHairModelInfo.u2ID == newHairModelInfo.u2ID) return;
		// 기존 장비 삭제.
		RemoveObjects(oldHairModelInfo);

		// 새로운 헤어 착용.
		CreateObject(newHairModelInfo);
	}

	public void SetHairColor(HairColorInfo hairColorInfo)
	{
		_curHairColor = hairColorInfo; 
		Dictionary<string, Transform> dicHairModels = GetHair();
		foreach(KeyValuePair<string, Transform> element in dicHairModels)
		{
			// 헤어 컬러 선택.
			SkinnedMeshRenderer smr = element.Value.GetComponentInChildren<SkinnedMeshRenderer>();
//			DebugMgr.Log(smr.transform.name);
			for(int i=0; i<smr.materials.Length; i++)
			{
				Color32 color = new Color32(hairColorInfo.R[i], hairColorInfo.G[i], hairColorInfo.B[i], 255);
				float colorStr = hairColorInfo.Str[i];

				smr.materials[i].SetColor("_Hair_Color", color);
				smr.materials[i].SetFloat("_Hair_Str", colorStr);
			}
		}

		SkinnedMeshRenderer smr2 = GetFace().GetChild(1).GetComponentInChildren<SkinnedMeshRenderer>();
//		DebugMgr.Log(smr2.transform.name);
		for(int i=0; i<smr2.materials.Length; i++)
		{
			Color32 color = new Color32(hairColorInfo.R[2], hairColorInfo.G[2], hairColorInfo.B[2], 255);
			float colorStr = hairColorInfo.Str[2];
			
			smr2.materials[i].SetColor("_Hair_Color", color);
			smr2.materials[i].SetFloat("_Hair_Str", colorStr);
		}
	}

	void BackupSocketObjects()
	{
		// 소켓에 붙어있는 장비들을 얼굴 본에서 떼어냄
		for(int equipIdx=0; equipIdx<cHero.acEquips.Length; equipIdx++)
		{
			if(cHero.acEquips[equipIdx] == null) continue;
			if(cHero.acEquips[equipIdx].GetEquipmentInfo().cModel == null) continue;
			for(int modelIdx=0; modelIdx<cHero.acEquips[equipIdx].GetEquipmentInfo().cModel.u1ModelCount; modelIdx++)
			{
				string modelName = cHero.acEquips[equipIdx].GetEquipmentInfo().cModel.acModels[modelIdx].sModelFilename;
				Transform modelTr = null;
				_dicCostumeTransforms.TryGetValue(modelName, out modelTr);

				if(modelTr != null)
					modelTr.parent = transform;
			}
		}
	}

	void RestoreSocketObjects()
	{
		// 장비 본에 부착
		for(int equipIdx=0; equipIdx<cHero.acEquips.Length; equipIdx++)
		{
			if(cHero.acEquips[equipIdx] == null) continue;
			if(cHero.acEquips[equipIdx].GetEquipmentInfo().cModel == null) continue;
			for(int modelIdx=0; modelIdx<cHero.acEquips[equipIdx].GetEquipmentInfo().cModel.u1ModelCount; modelIdx++)
			{
				if(cHero.acEquips[equipIdx].GetEquipmentInfo().cModel.acModels[modelIdx].cSocketInfo != null)
				{
					SocketInfo sock = cHero.acEquips [equipIdx].GetEquipmentInfo ().cModel.acModels [modelIdx].cSocketInfo;
					ModelInfo.EQUIP_MODEL modelInfo = cHero.acEquips[equipIdx].GetEquipmentInfo().cModel.acModels[modelIdx];
					Transform modelTr = null;
					Transform boneTr = null;
					_dicCostumeTransforms.TryGetValue(modelInfo.sModelFilename, out modelTr);
					_dicBoneTransforms.TryGetValue(modelInfo.cSocketInfo.sSocBone, out boneTr);

					if(modelTr != null)
					{
						modelTr.parent = boneTr;
						modelTr.localPosition = sock.fSocLoc;
						modelTr.localRotation = Quaternion.Euler(sock.fSocRot);
					}
				}
			}
		}
	}

	public void SetEquip(EquipmentItem newEquip)
	{
		Dictionary<string, Transform> dicFace = null;
		ModelInfo modelInfo = null;
		if(newEquip.u2ModelID != 0)
		{
			modelInfo = ModelInfoMgr.Instance.GetInfo(newEquip.u2ModelID);
		}
		else
		{
			modelInfo = newEquip.GetEquipmentInfo().cModel;
		}
		dicFace = ModelInfoMgr.Instance.GetModelObjects(modelInfo, cHero.cClass.u2ID);

		foreach(KeyValuePair<string, Transform> element in dicFace)
		{
			_dicCostumeTransforms.Add(element.Key, element.Value);
		}

		for(int modelIdx=0; modelIdx<modelInfo.u1ModelCount; modelIdx++)
		{
			ModelInfo.EQUIP_MODEL equipModel = modelInfo.acModels[modelIdx];
			Transform modelTr = null;
			_dicCostumeTransforms.TryGetValue(equipModel.sModelFilename, out modelTr);

			if(modelInfo.acModels[modelIdx].cSocketInfo != null)
			{
				Transform boneTr = null;
				_dicBoneTransforms.TryGetValue(equipModel.cSocketInfo.sSocBone, out boneTr);
				modelTr.parent = boneTr;

				modelTr.localPosition = modelInfo.acModels[modelIdx].cSocketInfo.fSocLoc;
				modelTr.localRotation = Quaternion.Euler(modelInfo.acModels[modelIdx].cSocketInfo.fSocRot);
				modelTr.localScale = Vector3.one;
			}
			else
			{
				modelTr.parent = _animatorTr;
				modelTr.localPosition = Vector3.zero;
				Quaternion rot = new Quaternion();
				rot.eulerAngles = Vector3.zero;
				modelTr.localRotation = rot;
				modelTr.localScale = Vector3.one;
				AddParts (modelTr.gameObject);
			}
		}
        OnOffHelmet(ObscuredPrefs.GetBool("HelmetToggle", true));
	}

	void SetSingleAnimator()
	{
		foreach(Transform tr in _dicCostumeTransforms.Values)
			AddParts(tr.gameObject);
	}

	public void AddParts(GameObject parts)
	{
		if (_headAnimator == null)
			return;

		SkinnedMeshRenderer[] smrs = parts.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach(SkinnedMeshRenderer smr in smrs)
		{
			ProcessBone(smr); 
		}

		parts.SetActive(true);
	}


	void ProcessBone(SkinnedMeshRenderer render)
	{
		//equipment = new GameObject(render.gameObject.name);
		//equipment.transform.SetParent(root);

		SkinnedMeshRenderer newRender = render;

		Transform[] bones = new Transform[render.bones.Length];

		for (int i = 0; i < bones.Length; i++) {
			Transform bone = FindChildByName (render.bones [i].name, _headAnimator.transform);
			if (bone != null) {
				bones [i] = bone;
			} else {
//				DebugMgr.LogError (render.name + " => " + render.bones [i].name);
			}
		}

		newRender.bones = bones;
		newRender.sharedMesh = render.sharedMesh;
		newRender.materials = render.sharedMaterials;
	}

	Transform FindChildByName(string name, Transform trans)
	{
		Transform returnObj;

		if(trans.name == name)
			return trans;

		foreach (Transform child in trans)
		{
			returnObj = FindChildByName(name, child);

			if(returnObj)
				return returnObj;
		}

		return null;
	}


	void RemoveObjects(ModelInfo oldEquip)
	{
		for(Byte modelIdx=0; modelIdx<oldEquip.u1ModelCount; modelIdx++)
		{
			string modelName = oldEquip.acModels[modelIdx].sModelFilename;
			Transform modelTr = null;
			_dicCostumeTransforms.TryGetValue(modelName, out modelTr);
			_dicCostumeTransforms.Remove(modelName);
			if (_headAnimator.transform != modelTr) {
				Destroy (modelTr.gameObject);
			}
		}
	}

	public void ChangeEquip(EquipmentItem oldEquip, EquipmentItem newEquip)
	{
		// 장비 id가 같으면 아무런 작업을 하지 않음.
		//if(oldEquip.GetEquipmentInfo().u2ID == newEquip.GetEquipmentInfo().u2ID) return;
        if(oldEquip.u2SlotNum == newEquip.u2SlotNum) return;

		// 기존 장비 삭제.
		ModelInfo oldModelInfo = null;
		if(oldEquip.u2ModelID != 0)
		{
			oldModelInfo = ModelInfoMgr.Instance.GetInfo(oldEquip.u2ModelID);
		}
		else
		{
			oldModelInfo = oldEquip.GetEquipmentInfo().cModel;
		}
		RemoveObjects(oldModelInfo);

		// 새 장비 장착.
		SetEquip(newEquip);

		// 레이어 설정
		SetLayer(_iLayer);
	}

	public void ChangeEquipModel(UInt16 oldModelID, EquipmentItem equipItem)
	{
		// 기존 장비 삭제.
		RemoveObjects(ModelInfoMgr.Instance.GetInfo(oldModelID));

		// 새 장비 장착.
		SetEquip(equipItem);
		
		// 레이어 설정
		SetLayer(_iLayer);
	}

	public void OnOffHelmet(bool onHelmet)
	{
		ModelInfo hairModelInfo = cHero.cClass.lstHairInfo[cHero.u1SelectedHair-1].cModelInfo;

		for(int equipIdx=0; equipIdx<ClassInfo.MAX_BASEEQUIP_OF_MODEL; equipIdx++)
		{
			if(cHero.acEquips[equipIdx] == null)
				continue;

			EquipmentItem equipItem = cHero.acEquips[equipIdx];
//			EquipmentInfo equipInfo = equipItem.GetEquipmentInfo();
			if(equipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.HELMET)
			{
				ModelInfo helmetModel = null;
				if(ModelInfoMgr.Instance.GetInfo(equipItem.u2ModelID) != null)
					helmetModel = ModelInfoMgr.Instance.GetInfo(equipItem.u2ModelID);
				else
					helmetModel = equipItem.GetEquipmentInfo().cModel;
				
				// 헬멧 on/off 여부에 따라 헬멧관련한 모든 모델 오브젝트 on/off
				for(int modelIdx=0; modelIdx<helmetModel.acModels.Length; modelIdx++)
				{
					Transform helmetModelTr = null;
					_dicCostumeTransforms.TryGetValue(helmetModel.acModels[modelIdx].sModelFilename, out helmetModelTr);
					helmetModelTr.gameObject.SetActive(onHelmet);
				}
					
				bool hairOn = (onHelmet && equipItem.GetEquipmentInfo().bRemoveHair) ? false : true;

				for(int modelIdx=0; modelIdx<hairModelInfo.acModels.Length; modelIdx++)
				{
					Transform hairModelTr = null;
					_dicCostumeTransforms.TryGetValue(hairModelInfo.acModels[modelIdx].sModelFilename, out hairModelTr);
					hairModelTr.gameObject.SetActive(hairOn);
				}
			}
		}
        SetLayer(LayerMask.NameToLayer("BGMainMap"));
	}	

	public ModelInfo GetModelInfo(EquipmentInfo.EQUIPMENT_POS posID)
	{
		for(int equipIdx=0; equipIdx<ClassInfo.MAX_BASEEQUIP_OF_MODEL; equipIdx++)
		{
			if(cHero.acEquips[equipIdx] == null)
				continue;
			
			EquipmentInfo equipInfo = (EquipmentInfo)cHero.acEquips[equipIdx].cItemInfo;
			if(equipInfo.u1PosID == posID)
			{
				return equipInfo.cModel;
			}
		}

		return null;
	}

	int _iLayer = 0;
	public void SetLayer(int layer)
	{
		_iLayer = layer;
//		DebugMgr.Log("HeroObject SetLayer");
		Invoke("SetLayerBeforeDelay", Time.deltaTime);

	}

	public int GetLayer()
	{
		return _iLayer;
	}

	void SetLayerBeforeDelay()
	{
		SkinnedMeshRenderer[] smrList = transform.FindChild("Animator").GetComponentsInChildren<SkinnedMeshRenderer>();
//				DebugMgr.Log("smr length : " + smrList.Length);
		for(int i=0; i<smrList.Length; i++)
		{
			//			DebugMgr.Log("HeroObject Smr : " + smrList[i].transform.name);
			smrList[i].gameObject.layer = _iLayer;
		}
		
		MeshRenderer[] mrList = transform.FindChild("Animator").GetComponentsInChildren<MeshRenderer>();
		for(int i=0; i<mrList.Length; i++)
		{
			//			DebugMgr.Log("HeroObject Mr : " + mrList[i].transform.name);
			mrList[i].gameObject.layer = _iLayer;
		}
	}

	Transform save_parent;
	Vector3 save_position;
	Quaternion save_rotation;
	Vector3 save_scale;

	Quaternion save_model_rotation;
	int save_layer;
	bool bUseBattle;
	public void SaveTransform()
	{
		save_parent = transform.parent;
		save_position = transform.parent.localPosition;
		save_rotation = transform.parent.rotation;
		save_scale = transform.parent.localScale;
		save_model_rotation = transform.localRotation;
		save_layer = _iLayer;
	}

	public void LoadTransform()
	{
		if (save_parent == null)
			return;

		transform.parent = save_parent;
		transform.parent.localPosition = save_position;
		transform.parent.rotation = save_rotation;
		transform.parent.localScale = save_scale;
		transform.localPosition = Vector3.zero;
		transform.localRotation = save_model_rotation;
		_iLayer = save_layer;
        SetLayer(_iLayer);
	}

	public void SetAnimations_UI()
	{
		_headAnimator.runtimeAnimatorController = AssetMgr.Instance.AssetLoad ("Animations/UI/Common/Anim_UI_Common_" + cHero.cClass.u2ID + "_Stand.controller",
			typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;

		if (_headAnimator.gameObject.GetComponent<AnimEvent> () != null) {
			Destroy (_headAnimator.gameObject.GetComponent<AnimEvent> ());
		}

		if (_headAnimator.gameObject.GetComponent<SoundPlayerAnimEvent> () == null) {
			_headAnimator.gameObject.AddComponent<SoundPlayerAnimEvent> ();
		}
		_headAnimator.Play ("UI_Class_Default");

		bUseBattle = false;
		_headAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
	}
    
	public void PlayAnim(string state)
    {
		_headAnimator.CrossFade(state, 0.1f);     
    }
    
	void PlayAnimBeforeDelay()
	{

	}

    public bool IsPlaying(string state)
    {
		_headAnimator.gameObject.SetActive(false);

		if(_headAnimator.GetCurrentAnimatorStateInfo(0).IsName(state))
			return true;
		else
			return false;

		return false;
    }

	public void SetAnimations_Battle()
	{
		_headAnimator.runtimeAnimatorController = Resources.Load("Animators/Hero/Anim_" + cHero.cClass.u2ID, 
											typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;

		if (_headAnimator.gameObject.GetComponent<SoundPlayerAnimEvent> () != null) {
			Destroy (_headAnimator.gameObject.GetComponent<SoundPlayerAnimEvent> ());
		}

		_headAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

		bUseBattle = true;
	}

	public bool SetDefaultAnimBattleEnd(){
		bool btemp = bUseBattle;
		if (bUseBattle) {
			bUseBattle = false;
			_headAnimator.enabled = true;
		}

		return btemp;
	}

	void OnDestroy()
	{
		
	}
}
