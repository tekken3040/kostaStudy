using UnityEngine;
using System.Collections;

public class EquipmentObject : MonoBehaviour {
	EquipmentItem _cEquipItem;
	public void SetData(EquipmentItem equipItem)
	{
		_cEquipItem = equipItem;
		ModelInfo modelInfo = null;
		if(equipItem.u2ModelID != 0)
		{
//			DebugMgr.Log("Equipment Object1 : " + equipItem.u2SlotNum + ", " + equipItem.u2ModelID);
			modelInfo = ModelInfoMgr.Instance.GetInfo(equipItem.u2ModelID);
		}
		else
		{
//			DebugMgr.Log("Equipment Object2 : " + equipItem.u2SlotNum + ", " + equipItem.u2ModelID);
			modelInfo = equipItem.GetEquipmentInfo().cModel;
		}

		for(int modelIdx=0; modelIdx<modelInfo.acModels.Length; modelIdx++)
		{
			if(modelInfo.acModels[modelIdx].bIsViewModel)
			{
//				DebugMgr.Log("Add Model : " + modelInfo.sModelPathname);
				GameObject modelObj = ModelInfoMgr.Instance.GetModelObject(modelInfo.sModelPathname+"/",modelInfo.acModels[modelIdx]);

				modelObj.transform.parent = equipItem.cObject.transform;
				modelObj.transform.localScale = modelInfo.acModels[modelIdx].cViewScale;
				modelObj.transform.localPosition = modelInfo.acModels[modelIdx].cViewPosition;
				modelObj.transform.localRotation = modelInfo.acModels[modelIdx].cViewRotation;
			}
		}
	}

//	public void SetData(ModelInfo modelInfo)
//	{
//		for(int modelIdx=0; modelIdx<modelInfo.acModels.Length; modelIdx++)
//		{
//			if(modelInfo.acModels[modelIdx].bIsViewModel)
//			{
//				DebugMgr.Log("Add Model : " + modelInfo.sModelPathname);
//				GameObject modelObj = ModelInfoMgr.Instance.GetModelObject(modelInfo.sModelPathname+"/",modelInfo.acModels[modelIdx]);
//
//				modelObj.transform.parent = transform;
//				modelObj.transform.localScale = modelInfo.acModels[modelIdx].cViewScale;
//				modelObj.transform.localPosition = modelInfo.acModels[modelIdx].cViewPosition;
//				modelObj.transform.localRotation = modelInfo.acModels[modelIdx].cViewRotation;
//			}
//		}
//	}

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
		Transform[] transforms = GetComponentsInChildren<Transform>();
//		DebugMgr.Log("smr length : " + transforms.Length);
		for(int i=0; i<transforms.Length; i++)
		{
			transforms[i].gameObject.layer = _iLayer;
		}
//		SkinnedMeshRenderer[] smrList = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
////		DebugMgr.Log("smr length : " + smrList.Length);
//		for(int i=0; i<smrList.Length; i++)
//		{
//			//			DebugMgr.Log("HeroObject Smr : " + smrList[i].transform.name);
//			smrList[i].gameObject.layer = _iLayer;
//		}
//		
//		MeshRenderer[] mrList = transform.GetComponentsInChildren<MeshRenderer>();
//		for(int i=0; i<mrList.Length; i++)
//		{
//			//			DebugMgr.Log("HeroObject Mr : " + mrList[i].transform.name);
//			mrList[i].gameObject.layer = _iLayer;
//		}
	}

	void Update()
	{
		if(gameObject.layer != _iLayer)
		{
			Transform[] transforms = GetComponentsInChildren<Transform>();
			for(int i=0; i<transforms.Length; i++)
			{
				transforms[i].gameObject.layer = _iLayer;
			}
		}
	}
}
