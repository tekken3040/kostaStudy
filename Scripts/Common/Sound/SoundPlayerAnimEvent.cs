using UnityEngine;
using System.Collections;

public class SoundPlayerAnimEvent : MonoBehaviour {

//	AudioSource audios;

	public void PlaySound(AudioClip audioClip)
	{
		//audios.PlayOneShot(audioClip);
		SoundManager.Instance.PlayEff (audioClip);
	}

	public void PlayRandomSound(string sAudioClips) // split ;
	{
		string[] paths = sAudioClips.Split(';');
		string selectedAudioClip = paths [Random.Range (0, paths.Length)];
		//audios.PlayOneShot(audioClip);
		SoundManager.Instance.PlayEff (selectedAudioClip);
	}

	public void PlaySoundLoop(AudioClip audioClip)
	{
		//audios.PlayOneShot(audioClip);
		SoundManager.Instance.PlayEff (audioClip, true);
	}
//
//	public void SetAS(AudioSource audioSource){
//		audios = audioSource;
//	}
}
