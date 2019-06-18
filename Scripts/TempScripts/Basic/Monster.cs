
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class Monster : Character
{
	Animator _headAnimator;

    public Monster(UInt16 classID, string name)
        : base(classID, name)
    {
    }

	public void attachModel(GameObject cObject)
	{
		EquipmentItem[] models = ClassInfoMgr.Instance.GetInfo(cClass.u2ID).u2BasicEquips;
		for(int i=0; i<models.Length; i++)
		{
			if(models[i] == null || models[i].GetEquipmentInfo().u2ID == 0)
				continue;
			//DebugMgr.Log("Monster Model Idx : " + i);
			List<GameObject> tempObjs = ModelInfoMgr.Instance.attachModel(cObject, models[i].GetEquipmentInfo().cModel);

			foreach (GameObject obj in tempObjs) {
				AddParts (obj);
				if(_headAnimator == null) _headAnimator = obj.GetComponent<Animator> ();
			}
		}

#if __SERVER
#else
//		cObject.transform.GetChild(0).GetComponent<SkinnedMeshCombiner>().Combine();
		//cObject.transform.Rotate(new Vector3(270f, 0f, 0f));
#endif
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
				DebugMgr.LogError (render.name + " => " + render.bones [i].name);
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

	public List<ModelInfo> GetAttachModel()
	{
		List<ModelInfo> tList = new List<ModelInfo> ();
		EquipmentItem[] models = ClassInfoMgr.Instance.GetInfo(cClass.u2ID).u2BasicEquips;
		for(int i=0; i<models.Length; i++)
		{
			if(models[i] == null || models[i].GetEquipmentInfo().u2ID == 0)
				continue;
			tList.Add(models[i].GetEquipmentInfo().cModel);
		}

		return tList;
	}
}