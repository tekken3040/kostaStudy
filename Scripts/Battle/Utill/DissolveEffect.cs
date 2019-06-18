using UnityEngine;
using System.Collections;
using System;

public class DissolveEffect : MonoBehaviour {

	UInt16 u2ID;
	float matDissolveValue = 0.0f;
	SkinnedMeshRenderer[] mats;
	MeshRenderer[] mats2;

	public void SetMats(UInt16 id, SkinnedMeshRenderer[] tmat, MeshRenderer[] tmat2){
		u2ID = id;
		mats = tmat;
		mats2 = tmat2;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		matDissolveValue += Time.fixedDeltaTime;
		SetRemoveColor ();
		if (matDissolveValue >= 1f) {
			ObjMgr.Instance.UnUseMonster(u2ID, gameObject);
			Destroy (this);
		}
	}

	void SetRemoveColor(){
		foreach (SkinnedMeshRenderer mat in mats) {
			for(int i=0; i<mat.materials.Length; i++){
				if(mat.materials[i].HasProperty("_DissolveAmount")){
					mat.materials[i].SetFloat("_DissolveAmount",matDissolveValue);
				}
			}
		}
		foreach (MeshRenderer mat in mats2) {
			for(int i=0; i<mat.materials.Length; i++){
				if(mat.materials[i].HasProperty("_DissolveAmount")){
					mat.materials[i].SetFloat("_DissolveAmount",matDissolveValue);
				}
			}
		}
	}
}
