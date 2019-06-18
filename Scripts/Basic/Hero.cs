using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
// ?�정 2015-05-11
public class Hero : Character
{
	public const int MAX_EQUIP_OF_CHAR = 10;

	public Byte u1Index;
    
    public Byte u1RoomNum;
    public Byte u1SeatNum;

	public Byte u1MadeByUser; // 유저 생성 시 1, 산 경우 2

	private Dictionary<string, string> socketInfo;

	public EquipmentItem[] acEquips;
	public void SetEquip(EquipmentInfo.EQUIPMENT_POS posID, EquipmentItem equipItem)
	{
		acEquips[(int)posID] = equipItem;
	}
	public void ChangeEquip(EquipmentItem newEquipItem)
	{
		EquipmentInfo.EQUIPMENT_POS posID = newEquipItem.GetEquipmentInfo().u1PosID;
		EquipmentItem oldEquipItem = new EquipmentItem ();
        //ChangeWear((Byte)posID, newEquipItem.u2SlotNum);
		//if( posID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && posID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2 && cObject != null )
		//{
		//	EquipmentItem oldEquipItem = acEquips[((Byte)posID-1)];
		//	cObject.GetComponent<HeroObject>().ChangeEquip(oldEquipItem, newEquipItem);
		//}
//		acEquips[(posID-1)] = newEquipItem;
		if(acEquips[((Byte)posID-1)] != null){
        	oldEquipItem = acEquips[((Byte)posID-1)];
		}
		ChangeWear((Byte)posID, newEquipItem.u2SlotNum);
		cObject.GetComponent<HeroObject>().ChangeEquip(oldEquipItem, newEquipItem);
	}

	public void SetEquip(EquipmentItem newEquipItem)
	{
		EquipmentInfo.EQUIPMENT_POS posID = newEquipItem.GetEquipmentInfo().u1PosID;
		acEquips [((Byte)posID - 1)] = newEquipItem;
		newEquipItem.AttachHero(this, (EquipmentInfo.EQUIPMENT_POS)posID);
	}

	public UInt16[] au2OrginalEquipSlots;

	public Byte u1SelectedHair;
	public Byte u1SelectedHairColor;

	public Byte u1SelectedFace;
	public void SetFace(Byte faceIdx)
	{
		u1SelectedFace = faceIdx;
	}
	public void ChangeFace(Byte newFaceIdx)
	{
		if(cObject != null && cObject.GetComponent<HeroObject>() != null)
		{
			ModelInfo oldModelInfo = cClass.lstFaceInfo[(Byte)(u1SelectedFace-1)].cModelInfo;
			ModelInfo newModelInfo = cClass.lstFaceInfo[newFaceIdx].cModelInfo;
			u1SelectedFace = (byte)(newFaceIdx+1);
			cObject.GetComponent<HeroObject>().ChangeFace(oldModelInfo, newModelInfo);
		}
	}

	public UInt32 u4Power
	{
		get
		{
			float ret = 0;
			ret += cFinalStatus.GetStat(1) * 0.04f;
			ret += cFinalStatus.GetStat(2) * 2.1f;
			ret += cFinalStatus.GetStat(3) * 2.1f;
			ret += cFinalStatus.GetStat(4) * 1.8f;
			ret += cFinalStatus.GetStat(5) * 1.8f;
			ret += cFinalStatus.GetStat(6) * 1.6f;
			ret += cFinalStatus.GetStat(7) * 1.6f;
			ret /= 7f;
			return (UInt32)ret;
		} 
	}

	public Byte u1PowerGrade
	{
		get
		{
			Byte powerGrade=0;
			if(u4Power <= 300) {
				powerGrade = 1;
			} else if(u4Power <= 600) {
				powerGrade = 2;
			} else if(u4Power <= 1000) {
				powerGrade = 3;
			} else if(u4Power <= 2000) {
				powerGrade = 4;
			} else if(u4Power <= 3000) {
				powerGrade = 5;
			} else if(u4Power <= 4000) {
				powerGrade = 6;
			} else if(u4Power >= 4001) {
				powerGrade = 7;
			}

			return powerGrade;
		}
	}

	public Hero():base()
	{

	}

	public Hero(Byte u2uID, UInt16 classID, string name, Byte hair, Byte hairColor, Byte face):base(classID, name)
	{
		u1Index = u2uID;
		acEquips = new EquipmentItem[MAX_EQUIP_OF_CHAR];
		au2OrginalEquipSlots = new UInt16[MAX_EQUIP_OF_CHAR];
		u1SelectedHair = hair;
		u1SelectedHairColor = hairColor;
		u1SelectedFace = face;
	}

	public Hero(Byte u2uID, UInt16 classID, string name, Byte madeByUser):base(classID, name)
	{
		u1Index = u2uID;
		u1MadeByUser = madeByUser;
		acEquips = new EquipmentItem[MAX_EQUIP_OF_CHAR];
		au2OrginalEquipSlots = new UInt16[MAX_EQUIP_OF_CHAR];
	}

	public UInt16[] au2InvenSlotNum;
	public Hero Wear(UInt16[] invenSlotNum)
	{
		au2InvenSlotNum = invenSlotNum;
//		DebugMgr.Log("invenSlotNum" + invenSlotNum.Length);
		for (int i = 0; i < invenSlotNum.Length; i++)
		{
			EquipmentItem equip = (EquipmentItem)Legion.Instance.cInventory.dicInventory[invenSlotNum[i]];
			// equip.GetComponent<LevelComponent>().Set(1, 0);
			equip.AttachHero(this, equip.GetEquipmentInfo().u1PosID);
			acEquips[(int)equip.GetEquipmentInfo().u1PosID - 1] = equip;
		//	DebugMgr.Log("Hero equip" + acEquips[((EquipmentInfo)equip.cItemInfo).u1PosID - 1].cItemInfo.u2ID);
			GetComponent<StatusComponent>().Wear(equip.cStatus);
		}
		return this;
	}

	public Hero DummyWear(UInt16[] invenSlotNum)
	{
		au2InvenSlotNum = invenSlotNum;
		//		DebugMgr.Log("invenSlotNum" + invenSlotNum.Length);
		for (int i = 0; i < invenSlotNum.Length; i++)
		{
			DebugMgr.Log("dummy inven slot num " + i + ": " + invenSlotNum[i]);
			if (invenSlotNum[i] != 0)
			{
				EquipmentItem equip = (EquipmentItem)Legion.Instance.cInventory.dicInventory[invenSlotNum[i]];
				EquipmentItem dummyEquip = new EquipmentItem(equip.GetEquipmentInfo().u2ID);
				equip.CopyStatus(dummyEquip);
				equip.GetComponent<LevelComponent>().Set(equip.cLevel.u2Level, equip.cLevel.u8Exp);
				acEquips[(int)equip.GetEquipmentInfo().u1PosID - 1] = dummyEquip;
				//	DebugMgr.Log("Hero equip" + acEquips[((EquipmentInfo)equip.cItemInfo).u1PosID - 1].cItemInfo.u2ID);
				dummyEquip.u2SlotNum = equip.u2SlotNum;
				GetComponent<StatusComponent>().Wear(dummyEquip.cFinalStatus);
			}
			else
			{
				//				DebugMgr.Log(i);
				if(ClassInfo.MAX_BASEEQUIP_OF_CLASS <= i) continue;
				if(cClass.u2CreateSceneEquips[i] == null) continue;
				EquipmentItem equip = cClass.u2CreateSceneEquips[i];
				
				acEquips[(int)equip.GetEquipmentInfo().u1PosID - 1] = equip;
				DebugMgr.Log("   acEquips" + i + " = " + equip.cItemInfo.u2ID);
			}
		}
		return this;
	}

    public Hero MakeBasicEquipAndWear(UInt16 baseStat = 0)
    {
        UInt16[] itemslots = new UInt16[cClass.u2BasicEquips.Length];
		Byte[] skillslots = new Byte[Server.ConstDef.SkillOfEquip];
   		UInt32[] stats = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType * 2];
        if (baseStat > 0)
        {
		    for(int i=0; i<stats.Length; i++)
		    {
			    stats[i] = baseStat;
		    }
        }
		for (int bIdx = 0; bIdx < cClass.u2BasicEquips.Length; bIdx++) 
		{
            EquipmentItem item = cClass.u2BasicEquips[bIdx];
			if (item != null)
			{
    			stats[Server.ConstDef.SkillOfEquip] = ((EquipmentInfo)item.cItemInfo).acStatAddInfo[0].u2BaseStatMin;
    			stats[Server.ConstDef.SkillOfEquip + 1] = ((EquipmentInfo)item.cItemInfo).acStatAddInfo[1].u2BaseStatMin;
				stats[Server.ConstDef.SkillOfEquip + 2] = ((EquipmentInfo)item.cItemInfo).acStatAddInfo[2].u2BaseStatMin;

				itemslots[bIdx] = Legion.Instance.cInventory.AddEquipment(0, 0, item.cItemInfo.u2ID, 1, 0, skillslots, stats, 0,"","",((EquipmentInfo)item.cItemInfo).u2ModelID);
			}
            else
            {
                itemslots[bIdx] = 0;
            }
		}
        Wear(itemslots);
        return this;
    }

	public void StartChangingEquip()
	{
		for(int i=0; i<acEquips.Length; i++)
		{
			au2OrginalEquipSlots[i] = acEquips[i].u2SlotNum;
		}
	}
	public void ChangeWear(UInt16 posID, UInt16 invenSlotNum)
	{
//		DebugMgr.Log("ChangeWare : " + attachSlot + " / " + invenSlotNum + " / " + ((EquipmentItem)Legion.Instance.cInventory.dicInventory[invenSlotNum]).cItemInfo.u2ID );
//		DebugMgr.Log(string.Format("Old acEquips[{0}] : {1}", (attachSlot-1), acEquips[(attachSlot-1)].cItemInfo.u2ID));
		DebugMgr.Log("ChangeWear : " + posID + " " + invenSlotNum);
		//if( acEquips[posID-1] != null && acEquips[(posID-1)].cItemInfo.u2ID != cClass.u2BasicEquips[(posID-1)].u2ID ) acEquips[(posID-1)].Detach();
		EquipmentItem equipItem = (EquipmentItem)Legion.Instance.cInventory.dicInventory[invenSlotNum];
		DebugMgr.Log("ChangeWear add status");
		if(acEquips[(posID-1)] != null)
		{
			acEquips[(posID-1)].Detach();
			GetComponent<StatusComponent>().FINAL_STATUS.Sub(acEquips[(posID-1)].cStatus);
		}
        GetComponent<StatusComponent>().FINAL_STATUS.Add(equipItem.cStatus);
		acEquips[(posID-1)] = equipItem;
		DebugMgr.Log("new equip slot : " +  acEquips[(posID-1)].u2SlotNum);
		equipItem.AttachHero(this, (EquipmentInfo.EQUIPMENT_POS)posID);

		//DebugMgr.Log(((EquipmentItem)Legion.Instance.cInventory.lstInventory[invenSlotNum]).cItemInfo.u2ID);
//		DebugMgr.Log(string.Format("ChangWear acEquips[{0}] : {1}", (attachSlot-1).ToString(), acEquips[(attachSlot-1)].cItemInfo.u2ID));
	}
	public void UndoChangingEquip()
	{
		for(UInt16 i=0; i<acEquips.Length; i++)
		{
			if (acEquips[i].u2SlotNum != au2OrginalEquipSlots[i])
			{
				ChangeWear ((UInt16)(i + 1), au2OrginalEquipSlots[i]);
			}
		}
	}

	public UInt16[] GetChangingEquip()
	{
		Byte u1Count = 0;
		for(int i=0; i<acEquips.Length; i++)
		{
			if (acEquips[i].u2SlotNum != au2OrginalEquipSlots[i])
			{
				u1Count++;
			}
		}
		if (u1Count == 0) return null;
		UInt16[] retVal = new UInt16[u1Count];
		u1Count = 0;
		for(int i=0; i<acEquips.Length; i++)
		{
			if (acEquips[i].u2SlotNum != au2OrginalEquipSlots[i])
			{
				retVal[u1Count++] = acEquips[i].u2SlotNum;
			}
		}
		return retVal;
	}

//	private GameObject _charObject;
//	public GameObject cObject
//	{
//		get
//		{
//			return _charObject;
//		}
//		
//		set
//		{
//			_charObject = value;
//			subAnimator = new Animator[_charObject.transform.GetChild(0).childCount];
//			//DebugMgr.Log(cCharacter.sName + "animator : " + subAnimator.Length);
//			subAnimator = _charObject.transform.FindChild("Animator").GetComponentsInChildren<Animator>();
//			
//			for(int i=0; i<subAnimator.Length; i++){
//				if(i == 0) subAnimator[i].gameObject.AddComponent<AnimEvent>().InitHead(this);
//				else subAnimator[i].gameObject.AddComponent<AnimEvent>();
//			}
//			
//			string path = "/Common/FootDust_Run";
//			if(cCharacter.cClass.sMoveEffect != "0") path = cCharacter.cClass.sMoveEffect;
//			VFXMgr.Instance.AddItem(path);
//			
//			Transform moveParticle = VFXMgr.Instance.GetVFX(path, _charObject.transform.position, Quaternion.identity).transform;
//			cMoveEff = new VFX( moveParticle );
//			moveParticle.parent = _charObject.transform.FindChild("Shadow");
//			cMoveEff.SetEmit(false);
//			
//			nav = _charObject.GetComponent<NavMeshAgent>();
//			obs = _charObject.GetComponent<NavMeshObstacle>();
//			rig = _charObject.GetComponent<Rigidbody>();
//			
//			col = cObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.AddComponent<AnimCollider>();
//			col.SetOwner(this);
//			//			for(int i=0; i<_charObject.transform.GetChild(0).childCount; i++)
//			//			{
//			//				subAnimator[i] = _charObject.transform.GetChild(0).GetChild(i).GetComponent<Animator>();
//			//			}
//		}
//	}

	Transform beforeParent;

	GameObject _viewObj;
	public GameObject cObject
	{
		get
		{
			if(_viewObj == null)
			{

			}
			return _viewObj;
		}

		set
		{
			_viewObj = value;
		}
	}

	public void InitModelObject(bool sync = false)
	{
		if(cObject == null)
		{
			if (ObjMgr.Instance.CheckHeroModel (this)) {
				_viewObj = ObjMgr.Instance.GetHero (this);
				_viewObj.SetActive (true);
			} else {
				_viewObj = GameObject.Instantiate (AssetMgr.Instance.AssetLoad ("Prefabs/Models/HeroBase.prefab", typeof(GameObject))) as GameObject;
			}
//            DebugMgr.Log("Init");
		}
		else
		{
			_viewObj.SetActive (true);
//            DebugMgr.Log("retun Init");
//            DebugMgr.Log(cObject.ToString());
			return;
		}
		_viewObj.transform.name = sName;
//		DebugMgr.Log("InitModelObject" + _viewObj.transform.name);
		if(_viewObj.GetComponent<HeroObject>() != null)
		{
			_viewObj.GetComponent<HeroObject>().SetData(this, sync);
		}

		if(!sync) ObjMgr.Instance.AddHeroModel (this, _viewObj);
	}

	public void DestroyModelObject()
	{
		if (ObjMgr.Instance.CheckHeroModel (this)) {
			beforeParent = _viewObj.transform.parent;
			_viewObj.transform.SetParent (ObjMgr.Instance.transform);
			if (!_viewObj.GetComponent<HeroObject> ().SetDefaultAnimBattleEnd()) {
				_viewObj.SetActive (false);
			}
		} else {
			GameObject.Destroy (_viewObj);
		}
	}

	public void SetActiveWithBeforeParent()
	{
		if (beforeParent != null) {
			_viewObj.transform.SetParent (beforeParent);
			_viewObj.SetActive (true);
		}
	}

//	public void attachFace(GameObject cObject)
//	{
//		ModelInfoMgr.Instance.attachModel(cObject, cClass.lstFaceInfo[u1SelectedFace].cModelInfo);
//	}
//
//	public void attachHair(GameObject cObject)
//	{
//		// ?�어 ?�택.
//		DebugMgr.Log(u1SelectedHair);
//		ModelInfoMgr.Instance.attachModel(cObject, cClass.lstHairInfo[u1SelectedHair].cModelInfo);
//		// ?�어 컬러 ?�택.
//		HairColorInfo hairColorInfo = cClass.lstHairColor[u1SelectedHairColor];
//		Color32 color = new Color32(hairColorInfo.R, hairColorInfo.G, hairColorInfo.B, 255);
//		int colorStr = Convert.ToInt16(hairColorInfo.Str);
//		DebugMgr.Log(cObject.transform.name);
//		//DebugMgr.Log(cObject.transform.FindChild("Animator").FindChild(cClass.lstHairInfo[u1SelectedHair].cModelInfo.acModels[0].sModelFilename).GetChild(1).name);
//		SkinnedMeshRenderer smr = cObject.transform.FindChild("Animator").FindChild(cClass.lstHairInfo[u1SelectedHair].cModelInfo.u2ID.ToString()).GetChild(1).GetComponent<SkinnedMeshRenderer>();
//		for(int i=0; i<smr.materials.Length; i++)
//		{
//			smr.materials[i].SetColor("_Hair_Color", color);
//			smr.materials[i].SetInt("_Hair_Str", colorStr);
//		}
//	}

//	public void attachEquipment(GameObject cObject)
//	{
//		this.cObject = cObject;
//		DebugMgr.Log(cClass.lstFaceInfo[u1SelectedFace].cModelInfo.u2ID);
//
//		for(int i=0; i<acEquips.Length; i++)
//		{
//			if(acEquips[i] == null) continue;
//			EquipmentInfo equipInfo = (EquipmentInfo)acEquips[i].cItemInfo;
//			if(equipInfo.u1PosID == EquipmentInfo.EQUIPMENT_POS.HELMET && equipInfo.bRemoveHair)
//			{
//				cObject.transform.FindChild("Animator").FindChild(
//						cClass.lstHairInfo[u1SelectedHair].cModelInfo.u2ID.ToString()
//				).gameObject.SetActive(false);
//			}
////			DebugMgr.LogError(cClass.sName+"Equip "+equipInfo.cModel.u2ID);
//			ModelInfoMgr.Instance.attachModel(cObject, equipInfo.cModel);
//		}
//#if __SERVER
//#else
//		// cObject.transform.GetChild(0).GetComponent<SkinnedMeshCombiner>().Combine();
//#endif
//	}

//	public override void attachAnimator(GameObject cObject)
//	{
//		string animPath = "Animations/";
//		animPath += cClass.sName + "_Animator";
//		//DebugMgr.Log("Hero Animation : " + animPath);
//#if __SERVER
//#else
//		Transform root = cObject.transform.FindChild("Animator");
//
//		RuntimeAnimatorController runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(animPath);
//
//		Avatar avatar = ModelInfoMgr.Instance.GetAvatar(cClass.lstFaceInfo[0].u2ModelID);
//
//		for(int i=0; i<root.childCount; i++)
//		{
//			Animator subAnimator = root.GetChild(i).GetComponent<Animator>();
//			subAnimator.runtimeAnimatorController = runtimeAnimatorController;
//			subAnimator.avatar = avatar;
//		}
//
//#endif
//	}

	//?�급 ?�기
	public static string GetPowerGrade(byte _powerGrade)
	{
		switch(_powerGrade)
		{
		case 2:
			return "D";
			
		case 3:
			return "C";
			
		case 4:
			return "B";
			
		case 5:
			return "A";
			
		case 6:
			return "S";
			
		case 7:
			return "SS";

		default:
			return "E";
		}
	}

	public Byte GetHeroElement()
	{
		return acEquips[(int)EquipmentInfo.EQUIPMENT_POS.WEAPON_R - 1].GetEquipmentInfo().u1Element;
	}

	public void Copy(Hero destHero)
	{
		destHero.GetComponent<StatusComponent>().LoadStatus(GetComponent<StatusComponent>().points, GetComponent<StatusComponent>().ResetCount);
		destHero.GetComponent<LevelComponent>().Set(GetComponent<LevelComponent>().cLevel.u2Level, GetComponent<LevelComponent>().cLevel.u8Exp);
		destHero.DummyWear(au2InvenSlotNum);

	}
}