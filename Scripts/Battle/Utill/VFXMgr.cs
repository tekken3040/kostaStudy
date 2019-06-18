using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class VFXMgr : Singleton<VFXMgr> {

	Dictionary<string, GameObject>  lstVFXs = new Dictionary<string, GameObject>();

	bool bInit = false;

	public void Init(){
		AddItem("/Common/Eff_Common_Spawn");
		AddItem("/Common/Eff_Common_ItemBox");
		AddItem("/Common/ExclamationMake");
		AddItem("/Common/Hit");
	}

	public void RemoveAll(){
		lstVFXs = new Dictionary<string, GameObject> ();
	}

	public void RemoveItem(string path){
		lstVFXs.Remove(path);
	}

	public void AddItem(string path){
		if(path == "0" || path == "") return;

		if(lstVFXs.ContainsKey(path)) return;

		GameObject tempVFX = AssetMgr.Instance.AssetLoad("Prefabs/Effects" + path + ".prefab", typeof(GameObject)) as GameObject;
		lstVFXs.Add(path, tempVFX);
	}

	public GameObject GetVFX(string path, Vector3 pos, Quaternion rot){
		if (!lstVFXs.ContainsKey (path)) {
			DebugMgr.LogError (path + " is Not Set");
			GameObject binObj = new GameObject ();
			return binObj;
		}

		if (lstVFXs [path] == null) {
			GameObject binObj = new GameObject ();
			return binObj;
			//lstVFXs [path] = AssetMgr.Instance.AssetLoad("Prefabs/Effects" + path + ".prefab", typeof(GameObject)) as GameObject;
		}

		GameObject temp = Instantiate(lstVFXs[path], pos, rot) as GameObject;
		return temp;
	}

//	public void AddItem(string path){
//		return;
//
//		if(path == "0" || path == "") return;
//
//		if(lstVFXs.ContainsKey(path)) return;
//
//		GameObject tempVFX = AssetMgr.Instance.AssetLoad("Prefabs/Effects" + path + ".prefab", typeof(GameObject)) as GameObject;
//		lstVFXs.Add(path, tempVFX);
//	}
//
//	public GameObject GetVFX(string path, Vector3 pos, Quaternion rot){
//		GameObject tempVFX = AssetMgr.Instance.AssetLoad("Prefabs/Effects" + path + ".prefab", typeof(GameObject)) as GameObject;
//		if (tempVFX == null) {
//			GameObject binObj = new GameObject ();
//			return binObj;
//		}
//		GameObject temp = Instantiate(tempVFX, pos, rot) as GameObject;
//		return temp;
//	}
}
