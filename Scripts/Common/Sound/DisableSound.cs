using UnityEngine;
using System.Collections;

public class DisableSound : MonoBehaviour {

	public AudioClip audioClip;

	public void OnDisable()
	{
		SoundManager.Instance.PlayEff (audioClip);
	}
}
