using UnityEngine;
using System.Collections;

public class DestroyTime : MonoBehaviour {
	public float destTime = 1.0f;
	// Use this for initialization
	void OnEnable () {
		Destroy(gameObject, destTime);
		//Invoke("ActiveFalse", destTime);
	}

	public void SetTime (float dTime) {
		destTime = dTime;
	}

//	void ActiveFalse()
//	{
//		//if(gameObject != null) gameObject.SetActive (false);
//	}
}
