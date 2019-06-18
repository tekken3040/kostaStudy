using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ModelInfoMgr : Singleton<ModelInfoMgr>
{
	private Dictionary<UInt16, ModelInfo> dicData;

	private bool loadedInfo=false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}
	
	public void AddInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		ModelInfo info = new ModelInfo();
		dicData.Add(info.SetInfo(cols), info);
	}

	public ModelInfo GetInfo(UInt16 id)
	{
		ModelInfo ret;
		dicData.TryGetValue(id, out ret);
		return ret;
	}

	public void SetData(UInt16 id, UInt16 classID, EquipmentInfo.EQUIPMENT_POS posID) 
	{
		ModelInfo modelInfo;
		dicData.TryGetValue(id, out modelInfo);
		modelInfo.cModelData.SetData(classID, posID);
	}

	public void AddViewModelInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		int idx=0;
		UInt16 itemID = Convert.ToUInt16(cols[idx++]);
		idx++; // comment
		ModelInfo modelInfo = null;
		dicData.TryGetValue(itemID, out modelInfo);
		if(modelInfo != null)
		{
			for(int i=0; i<modelInfo.acModels.Length; i++)
			{
				if(Convert.ToByte(cols[idx]) == 1)
				{
					modelInfo.acModels[i].SetViewerModel(cols, ref idx);
				}
				else
				{
					idx += 8;
				}
			}
		}
	}

	public void Init()
	{
		dicData = new Dictionary<UInt16, ModelInfo>();
		DataMgr.Instance.LoadTable(this.AddInfo, "Model");
		DataMgr.Instance.LoadTable(this.AddViewModelInfo, "EquipmentViewModel");
	}

	public Dictionary<string, Transform> GetModelObjects(ModelInfo modelInfo, UInt16 classID = 0)
	{
		if(modelInfo == null){ DebugMgr.Log("model info null"); }
		string path = "Prefabs/Models/";
		if(modelInfo.u2ID > 49000) path += "Monster/";
		else path += "Hero/";
		path += modelInfo.sModelPathname + "/";
		//		DebugMgr.Log(path);
		
		Dictionary<string, Transform> modelObjs = new Dictionary<string, Transform>();
		
		for(int i=0; i<modelInfo.acModels.Length; i++)
		{
			//			DebugMgr.Log(modelInfo.acModels[i].sModelFilename);
			if(modelInfo.acModels[i].sModelFilename.Equals("0")) continue;
			ModelInfo.EQUIP_MODEL equipModelInfo = modelInfo.acModels[i];
			string modelPath = path + equipModelInfo.sModelFilename;
			
			//			GameObject modelObj = getSubModel(modelPath);
			GameObject modelObj = ObjMgr.Instance.GetObject(modelPath);

			if(modelObj.GetComponentInChildren<SkinnedMeshRenderer>() != null)
			{
				modelObj.GetComponentInChildren<SkinnedMeshRenderer>().useLightProbes = false;
				modelObj.GetComponentInChildren<SkinnedMeshRenderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			}

			modelObjs.Add(modelInfo.acModels[i].sModelFilename, modelObj.transform);
		}

		return modelObjs;
	}
    
	public IEnumerator CoGetModelObjects(System.Action<Dictionary<string, Transform>> onCallback, ModelInfo modelInfo, UInt16 classID = 0)
	{
		if(modelInfo == null){ DebugMgr.Log("model info null"); }
		string path = "Prefabs/Models/";
		if(modelInfo.u2ID > 49000) path += "Monster/";
		else path += "Hero/";
		path += modelInfo.sModelPathname + "/";
		//		DebugMgr.Log(path);
		
		Dictionary<string, Transform> modelObjs = new Dictionary<string, Transform>();
		
		for(int i=0; i<modelInfo.acModels.Length; i++)
		{
			//			DebugMgr.Log(modelInfo.acModels[i].sModelFilename);
			if(modelInfo.acModels[i].sModelFilename.Equals("0")) continue;
			ModelInfo.EQUIP_MODEL equipModelInfo = modelInfo.acModels[i];
			string modelPath = path + equipModelInfo.sModelFilename;
			
			//			GameObject modelObj = getSubModel(modelPath);
            
            GameObject modelObj = null;
            yield return StartCoroutine(ObjMgr.Instance.CoGetObject((x) => modelObj = x as GameObject, modelPath));
            
			//GameObject modelObj = ObjMgr.Instance.GetObject(modelPath);

			if(modelObj.GetComponentInChildren<SkinnedMeshRenderer>() != null)
			{
				modelObj.GetComponentInChildren<SkinnedMeshRenderer>().useLightProbes = false;
				modelObj.GetComponentInChildren<SkinnedMeshRenderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			}

			modelObjs.Add(modelInfo.acModels[i].sModelFilename, modelObj.transform);
		}

		if(onCallback != null)
            onCallback(modelObjs);
	}    

	public GameObject GetModelObject(string path, ModelInfo.EQUIP_MODEL model)
	{
		GameObject ret=null;
//		ModelInfo.EQUIP_MODEL equipModelInfo = modelInfo.acModels[i];
		string modelPath = path + model.sModelFilename;
		
		//			GameObject modelObj = getSubModel(modelPath);
		//ret = ObjMgr.Instance.GetObject(modelPath);
		ret = Instantiate(Resources.Load("Prefabs/Models/Hero/"+modelPath, typeof(GameObject)) )as GameObject;
		
		if(ret.GetComponentInChildren<SkinnedMeshRenderer>() != null)
		{
			ret.GetComponentInChildren<SkinnedMeshRenderer>().useLightProbes = false;
			ret.GetComponentInChildren<SkinnedMeshRenderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
		}

		return ret;
	}

	public List<GameObject> attachModel(GameObject cObject, ModelInfo modelInfo)
	{
		if(modelInfo == null) DebugMgr.Log("model info null");
		string path = "Prefabs/Models/";
		if(modelInfo.u2ID > 49000) path += "Monster/";
		else path += "Hero/";
		path += modelInfo.sModelPathname + "/";
//		DebugMgr.Log(path);

		List<GameObject> modelObjs = new List<GameObject>();

		for(int i=0; i<modelInfo.acModels.Length; i++)
		{
//			DebugMgr.Log(modelInfo.acModels[i].sModelFilename);
			if(modelInfo.acModels[i].sModelFilename.Equals("0")) continue;
			ModelInfo.EQUIP_MODEL equipModelInfo = modelInfo.acModels[i];
			string modelPath = path + equipModelInfo.sModelFilename;

//			GameObject modelObj = getSubModel(modelPath);
			GameObject modelObj = GameObject.Instantiate (AssetMgr.Instance.AssetLoad(modelPath + ".prefab", typeof(GameObject)) as GameObject);//ObjMgr.Instance.GetObject(modelPath);

			Transform bone = cObject.transform.GetChild(0);
			SocketInfo sockInfo = new SocketInfo();
			if(equipModelInfo.cSocketInfo != null)
			{
//				DebugMgr.Log(equipModelInfo.sModelFilename + "with Socket");
				UInt16 socketID = equipModelInfo.cSocketInfo.u2ID;
				sockInfo = SocketInfoMgr.Instance.GetInfo(socketID);
				string boneName = sockInfo.sSocBone;
//				DebugMgr.Log(socketID + "/ " + boneName);
				Transform[] childTransform = cObject.transform.GetChild(0).GetComponentsInChildren<Transform>();
				foreach(Transform tr in childTransform)
				{
					if(tr.name == boneName) {
						bone = tr;
//						DebugMgr.Log("Finded Bone Object" + bone.name);
						break;
					}
				}
			}
			//
			if(modelObj.GetComponentInChildren<SkinnedMeshRenderer>() != null)
			{
				modelObj.GetComponentInChildren<SkinnedMeshRenderer>().useLightProbes = false;
				modelObj.GetComponentInChildren<SkinnedMeshRenderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			}
//			DebugMgr.Log("LastBoneName : " + bone.name);
			modelObj.transform.parent = bone;
			modelObj.transform.localPosition = sockInfo.fSocLoc;
			modelObj.transform.localRotation = Quaternion.Euler(sockInfo.fSocRot);
			modelObj.transform.name = modelInfo.u2ID.ToString();
			modelObj.transform.localScale = Vector3.one;
			modelObjs.Add(modelObj);
			// string textureFileName = modelInfo.acModels[i].sTextureFilename;
			//drawTexture(modelObj, path, textureFileName);
		}
		cObject.transform.GetChild(0).localRotation = Quaternion.Euler(new Vector3(270,0,0));

		return modelObjs;
	}

//	public Avatar GetAvatar(UInt16 modelID)
//	{
//		Avatar modelAvatar;
//		ModelInfo modelInfo;
//		dicData.TryGetValue(modelID, out modelInfo);
//
//		string modelPath = "Animators/";
//		if(modelID > 49000) modelPath += "Monster/";
//		else modelPath += "Hero/";
//
//		modelPath += modelInfo.sModelPathname + "/" + modelInfo.acModels[0].sModelFilename;
//		GameObject modelRes = AssetMgr.Instance.AssetLoad(modelPath + ".prefab", typeof(GameObject)) as GameObject;
//		modelAvatar = modelRes.GetComponent<Animator>().avatar;
//		return modelAvatar;
//	}

//	public GameObject GetModel(UInt16 id)
//	{
//		ModelInfo modelInfo;
//		dicData.TryGetValue(id, out modelInfo);
//
//		GameObject objModel = new GameObject();
////		GameObject equipObj = new GameObject(modelINfo.u2ID.ToString());
//		// DebugMgr.Log("sModelPathname : " + modelInfo.sModelPathname);
//		string path = "Prefabs/Models/";
//		if(id > 49000) path += "Monster/";
//		else path += "Hero/";
//		path += modelInfo.sModelPathname + "/";
//		// DebugMgr.Log("ModelPath : " + path);
//		for(int i=0; i<ModelInfo.MAX_MODEL_OF_EQUIP; i++)
//		{
//			if(modelInfo.acModels[i].sModelFilename.Equals("0")) continue;
//			string subModelPath = path + modelInfo.acModels[i].sModelFilename;
//			GameObject subModel = getSubModel(subModelPath);
//			subModel.transform.parent = objModel.transform;
//			
//			string textureFileName = modelInfo.acModels[i].sTextureFilename;
//			//drawTexture(subModel, path, textureFileName);
//		}
//		return objModel;
//	}
	
	private GameObject getSubModel(string path)
	{
//		DebugMgr.Log ("subModel : " + path);
		GameObject subModelRes = AssetMgr.Instance.AssetLoad(path + ".prefab", typeof(GameObject)) as GameObject;
		GameObject subModelObj = GameObject.Instantiate( subModelRes ) as GameObject;
		return subModelObj;
	}
	
	private void drawTexture(GameObject modelObj, string basicPath, string texFileName)
	{
#if __SERVER
#else
		if(texFileName.Contains("/"))
		{
			basicPath = "Models";

		}
		if(modelObj.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>() != null)
		{
			SkinnedMeshRenderer modelSMRenderer = modelObj.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
			// DebugMgr.Log("materials : " + modelObj.transform.name + "  Material Count : " + modelSMRenderer.materials.Length);
			
			if(modelSMRenderer.materials.Length == 1)
			{
				// DebugMgr.Log("Texture PATH : " + basicPath + texFileName);
//				modelSMRenderer.material.shader = Shader.Find("Standard");
				modelSMRenderer.material.mainTexture = AssetMgr.Instance.AssetLoad(basicPath + texFileName, typeof(Texture2D)) as Texture2D;
			} else {
				string[] fileNameParse = texFileName.Split('+');
				for(int matIdx=0; matIdx<modelSMRenderer.materials.Length; matIdx++)
				{
					// DebugMgr.Log("Multi Texture PATH : " + basicPath + fileNameParse[matIdx]);
//					DebugMgr.Log(modelSMRenderer.materials.Length+":"+fileNameParse.Length);
//					modelSMRenderer.material.shader = Shader.Find("Standard");
					modelSMRenderer.materials[matIdx].mainTexture = AssetMgr.Instance.AssetLoad(basicPath + fileNameParse[matIdx], typeof(Texture2D)) as Texture2D;
				}
			}
		} else {
			MeshRenderer MRenderer = modelObj.transform.GetChild(1).GetComponent<MeshRenderer>();
			// DebugMgr.Log("materials : " + modelObj.transform.name + "  Material Count : " + MRenderer.materials.Length);
			
			if(MRenderer.materials.Length == 1)
			{
				MRenderer.material.mainTexture = AssetMgr.Instance.AssetLoad(basicPath + texFileName, typeof(Texture2D)) as Texture2D;
			} else {
				string[] fileNameParse = texFileName.Split('+');
				for(int matIdx=0; matIdx<MRenderer.materials.Length; matIdx++)
				{
//					DebugMgr.Log(MRenderer.materials.Length+":"+fileNameParse.Length);
					DebugMgr.Log("mesh renderer : " + modelObj.name);
					MRenderer.materials[matIdx].mainTexture = AssetMgr.Instance.AssetLoad(basicPath + fileNameParse[matIdx], typeof(Texture2D)) as Texture2D;
				}
			}
		}
#endif
	}
}

