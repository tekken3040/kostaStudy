using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public class SoundManager : Singleton<SoundManager>  {
	bool bMuteEff = false;
	bool bMuteBGM = false;
	bool bFading = false;
//	float fVolumeEff = 1.0f;
//	float fVolumeBGM = 1.0f;

	string sBeforeBGMPath;
	string sCurrentBGMPath;

	AudioSource cEffPlayer;
	AudioSource cBGMPlayer;

	AudioListener cListener;
	AudioListener cBattleListener;

	float fCurVolume = 0f;
	float fLastVolume = 0f;
	float fFadeTime = 0f;

	string lastBGMPath;

	public void Awake(){
		cEffPlayer = gameObject.AddComponent<AudioSource>();
        cEffPlayer.volume = ObscuredPrefs.GetFloat("VolumeEFFECT", 1f);
        
		cBGMPlayer = gameObject.AddComponent<AudioSource>();
		cBGMPlayer.volume = ObscuredPrefs.GetFloat("VolumeBGM", 1f);
		cListener = gameObject.AddComponent<AudioListener>();

        if(cEffPlayer.volume == 0f)
        {
            SetMuteEff(true);
        }
	}

	// public void SetEffList(string folder){
	// 	Object[] loadData = AssetMgr.Instance.AssetLoadAll("Sound/"+folder);
	// 	foreach (AudioClip clip in loadData) {
	// 		EffList.Add(clip.name, clip);
	// 		clip.LoadAudioData();
	// 	}
	// }

	public void SetMuteEff(bool bMute){
		cEffPlayer.mute = bMute;
		bMuteEff = bMute;
	}

	public void PlayEff (AudioClip clip, bool bloop = false) {
		if(bMuteEff) return;

		if (bloop)
		{
			GameObject tempAS = new GameObject ("tempAS");
			AudioSource asScript = tempAS.AddComponent<AudioSource> ();
			asScript.loop = true;
			asScript.clip = clip;
			asScript.volume = cEffPlayer.volume;
			asScript.Play ();
		} 
		else 
		{
			cEffPlayer.PlayOneShot (clip);
		}
	}

	public void PlayEff(string path, bool bloop = false){
		if(bMuteEff) return;

		AudioClip audioClip = Resources.Load(path, typeof(AudioClip)) as AudioClip;
		if (audioClip != null) {
			if (bloop) 
			{
				GameObject tempAS = new GameObject ("tempAS");
				AudioSource asScript = tempAS.AddComponent<AudioSource> ();
				asScript.loop = true;
				asScript.clip = audioClip;
				asScript.volume = cEffPlayer.volume;
				asScript.Play ();
			} 
			else 
			{
				cEffPlayer.PlayOneShot (audioClip);
			}
		}
	}

	public void PlayRandomEff(AudioClip[] clips){
		if(bMuteEff) return;

		int rand = Random.Range(0, clips.Length);
		
		cEffPlayer.clip = clips [rand];
		cEffPlayer.Play();
	}

	public void SetMuteBGM(bool bMute){
		cBGMPlayer.mute = bMute;
		bMuteBGM = bMute;
	}

	public IEnumerator FadeBGM (float fromVol, float toVol, float time){
		fCurVolume = fromVol;
		fLastVolume = toVol;
		fFadeTime = time;
		yield return StartCoroutine(OnFade());
		bFading = false;
	}

	IEnumerator OnFade(){
		while(cBGMPlayer.volume != fLastVolume){

			cBGMPlayer.volume += (fLastVolume-fCurVolume) * Time.deltaTime / fFadeTime;

			if(fCurVolume < fLastVolume){
				if(fLastVolume < cBGMPlayer.volume) cBGMPlayer.volume = fLastVolume;
			}else{
				if(fLastVolume > cBGMPlayer.volume) cBGMPlayer.volume = fLastVolume;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	public void PlayLoadBGM(string path) {

		if (bMuteBGM)
			return;

		if (lastBGMPath == path)
			return;

		lastBGMPath = path;
		StartCoroutine (ChangeBGM (path));
	}    
    public void PlayLoadBGM(AudioClip clip)
    {
		if (bMuteBGM)
			return;

		if (cBGMPlayer.clip == clip)
			return;

		lastBGMPath = clip.name;

		cBGMPlayer.clip = clip;
		cBGMPlayer.loop = true;
		cBGMPlayer.Play();
    }

	public IEnumerator ChangeBGM(string path)
	{
		float vol = cBGMPlayer.volume;
		if (cBGMPlayer.isPlaying) {
			bFading = true;
			sBeforeBGMPath = sCurrentBGMPath;
			StartCoroutine (FadeBGM (cBGMPlayer.volume, 0f, 0.2f));
		}
		
		while (bFading) {
			yield return null;
		}
        
		StartCoroutine (FadeBGM (0f, vol, 0.2f));
	
		AudioClip audioClip = Resources.Load(path, typeof(AudioClip)) as AudioClip;
		cBGMPlayer.clip = audioClip;
		cBGMPlayer.loop = true;
		cBGMPlayer.Play();

		sCurrentBGMPath = path;
	}    

	public void RevertBGM()
	{
		PlayLoadBGM (sBeforeBGMPath);
	}

	public void OnBattleListner(AudioListener battleListener)
	{
		cListener.enabled = false;
		cBattleListener = battleListener;
	}

	public void OffBattleListner()
	{
		if(cBattleListener == null) return;
		cBattleListener.enabled = false;
		cListener.enabled = true;
	}

    public AudioSource GetcBGMPlayer()
    {
        return cBGMPlayer;
    }

    public AudioSource GetcEffPlayer()
    {
        return cEffPlayer;
    }
}
