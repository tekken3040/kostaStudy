using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EquipmentItem : Item {
	public StatusComponent statusComponent;
	private LevelComponent levelComponent;
	public Byte[] skillSlots;	
	public byte registedInShop;
	public string itemName;
	public string createrName;
	public UInt16 u2ModelID;
	public Byte u1SmithingLevel;
    public UInt16 u2UnsetStatPoint;
    public UInt16 u2StatPointExp;
    public Byte u1Completeness;

	public Status cStatus
	{
        get { return statusComponent.STATUS; }
	}

	public Status cFinalStatus
	{
        get { return statusComponent.FINAL_STATUS; }
	}

	public Level cLevel
	{
		get { return levelComponent.cLevel; }
	}

	public UInt32 u2Power
	{
		get
		{
			float ret = 0;
			//ret += cFinalStatus.u4HP * 0.04f;
			//ret += cFinalStatus.u2Strength * 2.1f;
			//ret += cFinalStatus.u2Intelligence * 1.8f;
			//ret += cFinalStatus.u2Defence * 1.8f;
			//ret += cFinalStatus.u2Resistance * 1.8f;
			//ret += cFinalStatus.u2Agility * 1.6f;
			//ret += cFinalStatus.u2Critical * 1.6f;
            ret += cFinalStatus.GetStat(1) * 0.04f;
			ret += cFinalStatus.GetStat(2) * 2.1f;
			ret += cFinalStatus.GetStat(3) * 1.8f;
			ret += cFinalStatus.GetStat(4) * 1.8f;
			ret += cFinalStatus.GetStat(5) * 1.8f;
			ret += cFinalStatus.GetStat(6) * 1.6f;
			ret += cFinalStatus.GetStat(7) * 1.6f;
			ret /= 7f;
			return (UInt32)ret;
		} 
	}

	public Byte u1StatPoint
	{
        get { return statusComponent.STAT_POINT; }
	}

	public UInt32[] GetPoints()
	{
		return statusComponent.points;
	}

//	public UInt16 GetSkillUpPoint(Byte skillIdx)
//	{
//		return Convert.ToUInt16( (int)GetEquipmentInfo().cSkillUpgrade[skillIdx].u1BasePoint + (int)cLevel.u2Level );
//	}

	public struct AttachInfo
	{
		public Hero hero;
		public EquipmentInfo.EQUIPMENT_POS attachSlotNum;
	}
	public AttachInfo attached;
	public void AttachHero(Hero attachedHero, EquipmentInfo.EQUIPMENT_POS attachSlotNum)
	{
		attached.hero = attachedHero;
		attached.attachSlotNum = attachSlotNum;
	}
	public void Detach()
	{
		attached.hero = null;
		attached.attachSlotNum = 0;
	}
	void init()
	{
		levelComponent = AddComponent<LevelComponent> (null);
		levelComponent.Set(1,0);
//		DebugMgr.Log("ID : " + cItemInfo.u2ID);
		statusComponent = AddComponent<StatusComponent>(GetEquipmentInfo().cStatus);
	}
	public EquipmentItem()
	{
	}
    public EquipmentItem(EquipmentItem _equipItem)
	{
        cItemInfo = _equipItem.cItemInfo;
        u2StatPointExp = _equipItem.u2StatPointExp;
        u2UnsetStatPoint = _equipItem.u2UnsetStatPoint;
        init();
	}
	public EquipmentItem(UInt16 id)
	{
//		 DebugMgr.Log("EquipmentItem ID : " + id.ToString());
		cItemInfo = EquipmentInfoMgr.Instance.GetInfo(id);
		if(cItemInfo == null)
			DebugMgr.Log(id);
		init();
	}
	public EquipmentInfo GetEquipmentInfo()
	{
		return EquipmentInfoMgr.Instance.GetInfo(base.cItemInfo.u2ID);
	}
    
	public void CopyStatus(EquipmentItem destEquipItem)
	{
        destEquipItem.statusComponent.STATUS = statusComponent.STATUS;
        destEquipItem.statusComponent.FINAL_STATUS = statusComponent.FINAL_STATUS;
		destEquipItem.statusComponent.cInfo = statusComponent.cInfo;

		destEquipItem.statusComponent.EquipBase = statusComponent.EquipBase;
		destEquipItem.statusComponent.points = statusComponent.points;

		destEquipItem.statusComponent.au1StatType = statusComponent.au1StatType;

        destEquipItem.u1Completeness = u1Completeness;
        destEquipItem.u1SmithingLevel = u1SmithingLevel;
		destEquipItem.GetComponent<LevelComponent>().Set(cLevel.u2Level, cLevel.u8Exp);

        
        destEquipItem.statusComponent.STAT_POINT = statusComponent.STAT_POINT;
		destEquipItem.statusComponent.ResetCount = statusComponent.ResetCount;
		destEquipItem.statusComponent.BuyPoint = statusComponent.BuyPoint;
        destEquipItem.statusComponent.VIP_STATPOINT = statusComponent.VIP_STATPOINT;
		destEquipItem.statusComponent.USE_POINT = statusComponent.USE_POINT;
        destEquipItem.statusComponent.UNSET_STATPOINT = statusComponent.UNSET_STATPOINT;
        destEquipItem.statusComponent.STATPOINT_EXP = statusComponent.STATPOINT_EXP;
        
		destEquipItem.GetComponent<LevelComponent>().bDummy = true;
	}

	public GameObject cObject;
	public void InitViewModelObject()
	{
		if(cObject == null)
		{
			cObject = GameObject.Instantiate( AssetMgr.Instance.AssetLoad("Prefabs/Models/Equipment.prefab", typeof(GameObject)) ) as GameObject;
		}
		else
		{
			GameObject.DestroyObject(cObject);
			cObject = GameObject.Instantiate( AssetMgr.Instance.AssetLoad("Prefabs/Models/Equipment.prefab", typeof(GameObject)) ) as GameObject;
		}

		cObject.GetComponent<EquipmentObject>().SetData(this);
		Animator[] animArray = cObject.GetComponentsInChildren<Animator>();
		foreach(Animator anim in animArray)
		{
			anim.runtimeAnimatorController = null;
		}

	}

//
//					public void CreateObject(ModelInfo modelInfo)
//					{
//					Dictionary<string, Transform> dicObjects = ModelInfoMgr.Instance.GetModelObjects(modelInfo);
//					int modelIdx=0;
//					foreach(KeyValuePair<string, Transform> element in dicObjects)
//					{
//						if(_dicCostumeTransforms.ContainsKey(element.Key)){
//							_dicCostumeTransforms.Add(element.Key+"L", element.Value);
//						}else{
//							_dicCostumeTransforms.Add(element.Key, element.Value);
//						}
//						
//						if(modelInfo.acModels[modelIdx].cSocketInfo != null)
//						{
//							
//							element.Value.parent = _dicBoneTransforms[modelInfo.acModels[modelIdx].cSocketInfo.sSocBone];
//							element.Value.localPosition = modelInfo.acModels[modelIdx].cSocketInfo.fSocLoc;
//							element.Value.localRotation = Quaternion.Euler(modelInfo.acModels[modelIdx].cSocketInfo.fSocRot);
//							element.Value.localScale = Vector3.one;
//						}
//						else
//						{
//							element.Value.parent = _animatorTr;
//							element.Value.localPosition = Vector3.zero;
//							//element.Value.localRotation = ;
//							element.Value.localScale = Vector3.one;
//						}
//						
//						modelIdx++;
//					}
//				}

	public static Color32[] equipElementColors = {new Color32(255, 255, 255, 255), // 무속성
													new Color32(255, 255, 255, 255), // 변화속성
													new Color32(214, 65, 107, 255), // 불								
												new Color32(57, 255, 255, 255), // 물
		new Color32(125, 255, 149, 255)}; //바람


	public static string[] equipElementColorCode = {"ffffffff", "ffffffff", "d6415bff", "39ffffff", "7dff95ff"};

	public static Color32[] equipElementColors2 = {new Color32(255, 255, 255, 255), // 무속성
		new Color32(255, 255, 255, 255), // 변화속성
		new Color32(143, 0, 25, 255), // 불								
		new Color32(0, 128, 191, 255), // 물
		new Color32(28, 124, 46, 255)}; //바람
	
	public static string[] equipElementColorCode2 = {"ffffffff", "ffffffff", "8E0018FF", "0080BFFF", "1C7B2DFF"};
}
