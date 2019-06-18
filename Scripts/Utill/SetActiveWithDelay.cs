using UnityEngine;
using System.Collections;

public class SetActiveWithDelay : MonoBehaviour {
	public float delay;
	public GameObject obj;
	public bool isActive;

	void FixedUpdate () {
		delay -= Time.fixedDeltaTime;
		if (delay <= 0) {
			obj.SetActive(isActive);
			this.enabled = false;
		}
	}
}
