using UnityEngine;
using System.Collections;

public class DisableTime : MonoBehaviour {
	public float destTime = 1.0f;
	// Use this for initialization
	void Awake () {
		StartCoroutine(SetDisableAfterTime());
	}
	IEnumerator SetDisableAfterTime()
	{
		yield return new WaitForSeconds(destTime);
		gameObject.SetActive(false);
	}
}
