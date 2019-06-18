using UnityEngine;
using System.Collections;

public class ShadowBillboard : MonoBehaviour {

	Transform shadowTrans;
	public Vector3 pos = new Vector3(360f, 180f, 0f);

	// Use this for initialization
	void Start () {
		shadowTrans = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		shadowTrans.LookAt(pos);
	}
}
