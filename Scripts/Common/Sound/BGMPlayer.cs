using UnityEngine;
using System.Collections;

public class BGMPlayer : MonoBehaviour {

	public string bgmPath;
	public AudioClip bgmClip;

	void Start()
	{
		if (bgmClip != null)
			SoundManager.Instance.PlayLoadBGM (bgmClip);
		else if(bgmPath != "")
			SoundManager.Instance.PlayLoadBGM (bgmPath);
	}
}
