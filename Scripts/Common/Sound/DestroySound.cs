using UnityEngine;
using System.Collections;

public class DestroySound : MonoBehaviour {

	public AudioClip audioClip;

	public void OnDestroy()
	{
        if(audioClip != null)
		    SoundManager.Instance.PlayEff (audioClip);
	}
}
