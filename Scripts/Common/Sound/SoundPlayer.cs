using UnityEngine;
using System.Collections;

public class SoundPlayer : MonoBehaviour {

	public AudioClip audioClip;

	public void OnEnable()
	{
		SoundManager.Instance.PlayEff (audioClip);
	}
}
