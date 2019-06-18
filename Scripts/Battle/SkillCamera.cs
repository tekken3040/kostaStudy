using UnityEngine;
using System.Collections;


public class SkillCamera : MonoBehaviour {
	public Transform target;
	Transform mainCam;

	public void SetMainCam(Transform tr){
		mainCam = tr;
	}

	void Update(){
		if(target == null) return;
		Vector3 pos = new Vector3(target.position.x, LegionInfoMgr.Instance.fModelHeight, target.position.z);
		//transform.LookAt(pos);
	}

	public void SetTarget(Transform tTarget){
		target = tTarget;
	}

	public void DirectionEnd(){
		mainCam.SendMessage("DisableSkillCam");
	}

	public void InitParam(){
		target = null;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}
}
