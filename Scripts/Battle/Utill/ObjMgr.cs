using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ObjMgr : Singleton<ObjMgr> {
	
	Dictionary<string, GameObject>  lstObjs = new Dictionary<string, GameObject>();
	List<ushort> lstIDs = new List<ushort>();
	GameObject monsterPref;
	GameObject pChar{
		get 
		{ 
			if (monsterPref == null) {
				monsterPref = AssetMgr.Instance.AssetLoad ("Prefabs/Models/Character.prefab", typeof(GameObject)) as GameObject;
			}
			return monsterPref; 
		}
		set { monsterPref = value; }
	}

	public class MonsterPool{
		public bool bUse;
		public GameObject cObj;
		public List<ModelInfo> cModel;

		public MonsterPool(bool bVal, GameObject cTemp, List<ModelInfo> cInfo){
			bUse = bVal;
			cObj = cTemp;
			cModel = cInfo;
		}
	}

	Dictionary<ushort, List<MonsterPool>> lstMonsterPool = new Dictionary<ushort, List<MonsterPool>>();
//	Dictionary<ModelInfo, Dictionary<string, Transform>>[] lstHeroObjs;
//	Dictionary<int, GameObject>[] lstHeroObjs;
	Dictionary<Hero, GameObject> lstHeroObjs;
	
	void Awake(){
//		lstHeroObjs = new Dictionary<ModelInfo, Dictionary<string, Transform>>[5];
//		for (int i = 0; i < lstHeroObjs.Length; i++) {
//			lstHeroObjs [i] = new Dictionary<ModelInfo, Dictionary<string, Transform>> ();
//		}
//		lstHeroObjs = new Dictionary<int, GameObject>[5];
//		for (int i = 0; i < lstHeroObjs.Length; i++) {
//			lstHeroObjs [i] = new Dictionary<int, GameObject>();
//		}
		lstHeroObjs = new Dictionary<Hero, GameObject>();
		pChar = AssetMgr.Instance.AssetLoad("Prefabs/Models/Character.prefab", typeof(GameObject)) as GameObject;
	}

	public void AddHeroModel(Hero hero, GameObject modelObjs){
		if (Legion.Instance.SelectedCrew == null)
			return;
		
		if (Legion.Instance.SelectedCrew.u1Index < 1) {
			return;
		}
		
		if (lstHeroObjs.ContainsKey (hero))
			return;
		
		DontDestroyOnLoad (modelObjs);
		lstHeroObjs.Add (hero, modelObjs);
	}
	
	public bool CheckHeroModel(Hero hero){
		if (Legion.Instance.SelectedCrew == null)
			return false;
		
		if (Legion.Instance.SelectedCrew.u1Index < 1)
			return false;
	
		if (lstHeroObjs.ContainsKey (hero)) {
			return true;
		}
	
		return false;
	}
	
	public GameObject GetHero(Hero hero){
		if(!lstHeroObjs.ContainsKey(hero)) return null;
	
		return lstHeroObjs[hero];
	}
	
	public void RemoveHeroModelPool(){
		if (Legion.Instance.SelectedCrew == null)
			return;
		
		Hero[] tempList = new Hero[lstHeroObjs.Count];
		lstHeroObjs.Keys.CopyTo (tempList, 0);
		for (int i = 0; i < tempList.Length; i++) {
			if (tempList [i].u1AssignedCrew != Legion.Instance.SelectedCrew.u1Index) {
				Destroy (lstHeroObjs [tempList [i]]);
				lstHeroObjs.Remove (tempList [i]);
			} else {
				tempList [i].DestroyModelObject ();
			}
		}
//		foreach (KeyValuePair<Hero, GameObject> val in lstHeroObjs) {
//			if (val.Key.u1AssignedCrew != Legion.Instance.SelectedCrew.u1Index) {
//				Destroy (val.Value);
//				lstHeroObjs.Remove (val.Key);
//			}
//		}
	}

	public void RemoveAll(){
		Hero[] tempList = new Hero[lstHeroObjs.Count];
		lstHeroObjs.Keys.CopyTo (tempList, 0);
		for (int i = 0; i < tempList.Length; i++) {
			Destroy (lstHeroObjs [tempList [i]]);
		}
		lstHeroObjs.Clear ();

		RemoveMonsterPool ();
	}

	public void SaveObjectAtThis()
	{
		foreach (KeyValuePair<Hero, GameObject> val in lstHeroObjs) {
			val.Value.transform.SetParent (this.transform);
			val.Value.SetActive (false);
			//val.Value.transform.localPosition = Vector3.zero;

			SkinnedMeshRenderer[] mats = val.Value.GetComponentsInChildren<SkinnedMeshRenderer>();
			MeshRenderer[] mats2 = val.Value.GetComponentsInChildren<MeshRenderer>();
			AnimCollider cols = val.Value.GetComponentInChildren<AnimCollider>();

			if (cols != null)
				Destroy (cols.transform.parent.gameObject);

			foreach (SkinnedMeshRenderer mat in mats) {
				for(int i=0; i<mat.materials.Length; i++){
					if(mat.materials[i].HasProperty("_Attack_Eff")){
						mat.materials[i].SetFloat("_Attack_Eff",0);
					}
				}
			}
			foreach (MeshRenderer mat in mats2) {
				for(int i=0; i<mat.materials.Length; i++){
					if(mat.materials[i].HasProperty("_Attack_Eff")){
						mat.materials[i].SetFloat("_Attack_Eff",0);
					}
				}
			}

		}
	}

//	public void InitMyHero(){
//		for (int i=0; i<Legion.Instance.cBestCrew.u1Count; i++) {
//			Hero temp = (Hero)Legion.Instance.cBestCrew.acLocation[i];
//
//			GameObject cObject = Instantiate(pChar) as GameObject;
//
////			temp.attachEquipment(cObject);
//
//			lstHeroObjs.Add(temp.u1Index, cObject);
//		}
//	}


//crew&hero index with rootobj
//	public void AddHeroModel(Byte crewIdx, int heroidx, GameObject modelObjs){
//		if (crewIdx == 0)
//			return;
//		
//		if (Legion.Instance.SelectedCrew.u1Index < 1) {
//			return;
//		}
//		
//		if (lstHeroObjs[crewIdx-1].ContainsKey (heroidx))
//			return;
//		
//		DontDestroyOnLoad (modelObjs);
//		lstHeroObjs[crewIdx-1].Add (heroidx, modelObjs);
//	}
//
//	public bool CheckHeroModel(Byte crewIdx, int heroidx){
//		if (crewIdx == 0)
//			return false;
//		
//		if (Legion.Instance.SelectedCrew.u1Index < 1)
//			return false;
//	
//		if (lstHeroObjs[crewIdx-1].ContainsKey (heroidx)) {
//			return true;
//		}
//	
//		return false;
//	}
//
//	public GameObject GetHero(Byte crewIdx, int heroidx){
//		if (crewIdx == 0)
//			return null;
//
//		if(!lstHeroObjs[crewIdx-1].ContainsKey(heroidx)) return null;
//
//		return lstHeroObjs[crewIdx-1][heroidx];
//	}
//
//	public void RemoveHeroModelPool(){
//		for (int i = 0; i < Legion.Instance.SelectedCrew.u1Index; i++) {
//			if (i != (Legion.Instance.SelectedCrew.u1Index - 1)) {
//				foreach (GameObject val in lstHeroObjs [i].Values) {
//					Destroy (val);
//				}
//				lstHeroObjs [i].Clear ();
//			}
//		}
//	}


//crew with model object
//	public void AddHeroModel(int crewIdx, ModelInfo info, Dictionary<string, Transform> modelObjs){
//		if (Legion.Instance.SelectedCrew.u1Index < 1)
//			return;
//		
//		if (lstHeroObjs[crewIdx-1].ContainsKey (info))
//			return;
//		
//		DontDestoryObjects (modelObjs);
//		lstHeroObjs[crewIdx-1].Add (info, modelObjs);
//	}
//
//	void DontDestoryObjects(Dictionary<string, Transform> modelObjs){
//		foreach (Transform tr in modelObjs.Values) {
//			DontDestroyOnLoad (tr.gameObject);
//		}
//	}
//
//	void DestoryObjects(Dictionary<string, Transform> modelObjs){
//		foreach (Transform tr in modelObjs.Values) {
//			Destroy (tr.gameObject);
//		}
//	}
//
//	public bool CheckHeroModel(ModelInfo info){
//		if (Legion.Instance.SelectedCrew.u1Index < 1)
//			return false;
//
//		if (lstHeroObjs[Legion.Instance.SelectedCrew.u1Index-1].ContainsKey (info)) {
//			return true;
//		}
//
//		return false;
//	}

//	public Dictionary<string, Transform> GetHeroModel(ModelInfo info){
//		if (Legion.Instance.SelectedCrew.u1Index < 1)
//			return null;
//
//		if (lstHeroObjs[Legion.Instance.SelectedCrew.u1Index-1].ContainsKey (info)) {
//			return lstHeroObjs[Legion.Instance.SelectedCrew.u1Index-1][info];
//		}
//
//		return null;
//	}

//	public void RemoveHeroModelPool(){
//		for (int i = 0; i < Legion.Instance.SelectedCrew.u1Index; i++) {
//			if (i != (Legion.Instance.SelectedCrew.u1Index - 1)) {
//				foreach (Dictionary<string, Transform> val in lstHeroObjs [i].Values) {
//					DestoryObjects (val);
//				}
//				lstHeroObjs [i].Clear ();
//			}
//		}
//	}

	public void AddMonsterPool(ushort ID, int count){
//		DebugMgr.Log(ID+":"+count);
		if (pChar == null) {
			DebugMgr.LogError ("null pChar");
		}

		//AddItem(ID);

		if(lstMonsterPool.ContainsKey(ID)) return;
		List<MonsterPool> tPool = new List<MonsterPool>();

		for(int i=0; i<count; i++){
			GameObject cObject = Instantiate(pChar) as GameObject;
			//cObject.transform.Rotate(new Vector3(270, 0, 0));

			Monster cMonster = new Monster(ID, ID.ToString()+"_"+i);
			cMonster.attachModel(cObject);
			cMonster.attachAnimator(cObject);
			List<ModelInfo> cModel = cMonster.GetAttachModel();

			cObject.SetActive(false);

			tPool.Add(new MonsterPool(false, cObject, cModel));
		}
		lstMonsterPool.Add(ID, tPool);
	}

	public GameObject AddMonsterPoolWithGetObj(ushort ID){
		//AddItem(ID);

		GameObject cObject = Instantiate(pChar) as GameObject;
		//cObject.transform.Rotate(new Vector3(270, 0, 0));

		Monster cMonster = new Monster(ID, ID.ToString()+"_"+(lstMonsterPool[ID].Count+1));
		cMonster.attachModel(cObject);
		cMonster.attachAnimator(cObject);
		List<ModelInfo> cModel = cMonster.GetAttachModel();

		lstMonsterPool[ID].Add(new MonsterPool(true, cObject, cModel));

		//DebugMgr.LogError("AddMonster "+ID);

		return cObject;
	}

	public void UnUseMonster(ushort ID, GameObject tObj){
		tObj.SetActive(false);
		lstMonsterPool[ID].Find(cs => cs.cObj == tObj).bUse = false;
	}

	public GameObject GetMonsterAtPool(ushort ID){

		//DebugMgr.Log(ID);
		MonsterPool Result = lstMonsterPool[ID].Find(cs => cs.bUse == false);
		if (Result == null) {
			return AddMonsterPoolWithGetObj (ID);
		}
		Result.bUse = true;
		Result.cObj.SetActive(true);

		return Result.cObj;
	}

	public void RemoveMonsterPool(){
		foreach (ushort obj in lstMonsterPool.Keys) {
			lstIDs.Remove (obj);
		}

		string path = "Prefabs/Models/Monster/";

		foreach (KeyValuePair<ushort, List<MonsterPool>> obj in lstMonsterPool) {
			for(int i=0; i<obj.Value.Count; i++){
				Destroy(obj.Value[i].cObj);
				obj.Value [i].cObj = null;

//				if (AssetMgr.Instance.useAssetBundle) {
//					foreach (ModelInfo cMInfo in obj.Value[i].cModel) {
//						for (int j = 0; j < cMInfo.acModels.Length; j++) {
//							if (cMInfo.acModels [j].sModelFilename.Equals ("0"))
//								continue;
//							ModelInfo.EQUIP_MODEL equipModelInfo = cMInfo.acModels [j];
//							string modelPath = path + equipModelInfo.sModelFilename;
//							AssetMgr.Instance.UnloadAssetBundle (modelPath + ".prefab");
//						}
//					}
//				}

			}
			obj.Value.Clear();
		}

		foreach (ushort obj in lstMonsterPool.Keys) {
			lstIDs.Remove (obj);
		}

		lstMonsterPool.Clear();
	}
	
//	public void RemoveItem(ushort ID){
//		if(lstIDs.FindIndex(cs => cs == ID) < 0) return;
//
////		ModelInfo[] models = new ModelInfo[ClassInfoMgr.Instance.GetInfo(ID).u2BasicEquips.Length];
//		EquipmentItem[] models = ClassInfoMgr.Instance.GetInfo(ID).u2BasicEquips;
////			ClassInfoMgr.Instance.GetInfo(ID).u2BasicEquips;
//		for(int i=0; i<models.Length; i++)
//		{
//			if(models[i] == null || models[i].u2ID == 0)
//				continue;
//			
//			string path = "Models/"+models[i].sModelPathname+"/"+models[i].acModels[0].sModelFilename;
//			
//			if(lstObjs.ContainsKey(path)) continue;
//			lstObjs.Remove(path);
//		}
//
//		lstIDs.Remove(ID);
//	}
	
	public void AddItem(ushort ID){
		if(ID == 0) return;

		if(lstIDs.FindIndex(cs => cs == ID) > -1) return;

		lstIDs.Add(ID);

//		ModelInfo[] models = ClassInfoMgr.Instance.GetInfo(ID).u2BasicEquips;
		EquipmentItem[] items = ClassInfoMgr.Instance.GetInfo(ID).u2BasicEquips;
		int equipCnt = items.Length-2;
		for(int i=0; i<equipCnt; i++)
		{
			if(items[i] == null || items[i].GetEquipmentInfo().cModel.u2ID == 0)
				continue;

			for(int j=0; j<items[i].GetEquipmentInfo().cModel.acModels.Length; j++){
				string path = "Prefabs/Models/";
				if(ID > 100) path += "Monster/";
				else path += "Hero/";
				path += items[i].GetEquipmentInfo().cModel.sModelPathname+"/"+items[i].GetEquipmentInfo().cModel.acModels[j].sModelFilename;

				if(lstObjs.ContainsKey(path)) continue;
				if(items[i].GetEquipmentInfo().cModel.acModels[j].sModelFilename == "0") continue;
				
				GameObject tempModel = AssetMgr.Instance.AssetLoad(path + ".prefab", typeof(GameObject)) as GameObject;
				lstObjs.Add(path, tempModel);
			}
		}
	}
	
	public GameObject GetObject(string path){
//		DebugMgr.Log("GetObject Path : " + path);
		GameObject subModelRes = Resources.Load(path, typeof(GameObject)) as GameObject;
		//lstObjs.Add(path, subModelRes);

		if(subModelRes == null)
			DebugMgr.LogError("SubModel Path Error : " + path);

		GameObject subModelObj = GameObject.Instantiate (subModelRes) as GameObject;
		return subModelObj;
//		if (!lstObjs.ContainsKey (path)) {
//			GameObject subModelRes = AssetMgr.Instance.AssetLoad(path + ".prefab", typeof(GameObject)) as GameObject;
//			lstObjs.Add(path, subModelRes);
//            
//            if(subModelRes == null)
//                DebugMgr.LogError("SubModel Path Error : " + path);
//                
//			GameObject subModelObj = GameObject.Instantiate (subModelRes) as GameObject;
//			return subModelObj;
//		}
//
//		GameObject temp = Instantiate(lstObjs[path]) as GameObject;
//        
//        if(temp == null)
//            DebugMgr.LogError("Model Path Error : " + path);
//        
//		return temp;
	}
    
    public bool start = false;
    public IEnumerator CoGetObject(System.Action<UnityEngine.Object> onCallback, string path){
        
        while(start)
            yield return null;
            
        start = true;    

		ResourceRequest res = Resources.LoadAsync(path, typeof(GameObject));

		yield return res;

		if(!res.isDone)
			yield return null;

		GameObject subModelRes = res.asset as GameObject;

		//yield return StartCoroutine(AssetMgr.Instance.AssetLoadAsync((x) => subModelRes = x as GameObject, path + ".prefab", typeof(GameObject)));

		//lstObjs.Add(path, subModelRes);

		if(subModelRes == null)
			DebugMgr.LogError("SubModel Path Error : " + path);

		GameObject subModelObj = GameObject.Instantiate (subModelRes) as GameObject;

		if(onCallback != null)
			onCallback(subModelObj);
        
//		DebugMgr.Log("GetObject Path : " + path);
//		if (!lstObjs.ContainsKey (path)) {
//            
//            GameObject subModelRes = null;
//
//            yield return StartCoroutine(AssetMgr.Instance.AssetLoadAsync((x) => subModelRes = x as GameObject, path + ".prefab", typeof(GameObject)));
//            			
//			lstObjs.Add(path, subModelRes);
//            
//            if(subModelRes == null)
//                DebugMgr.LogError("SubModel Path Error : " + path);
//                
//			GameObject subModelObj = GameObject.Instantiate (subModelRes) as GameObject;
//			
//            if(onCallback != null)
//                onCallback(subModelObj);
//		}

//		GameObject temp = Instantiate(lstObjs[path]) as GameObject;
//        
//        if(temp == null)
//            DebugMgr.LogError("Model Path Error : " + path);
//        
//		if(onCallback != null)
//            onCallback(temp);
            
        start = false;
	}

	public void DestroyObj(GameObject obj){
		Destroy(obj);
	}
}
