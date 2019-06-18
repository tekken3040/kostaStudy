using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {
	
	public void LoadScene(string SceneName)
	{
		//if(SceneName == "Battle"){
		//	if(!FieldInfoMgr.Instance.LoadedInfo) return;
		//}
//		DebugMgr.Log(SceneName);
		SoundManager.Instance.OffBattleListner ();
		StartCoroutine(sceneChangeEffect(SceneName));
	}

	IEnumerator sceneChangeEffect(string SceneName)
	{
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);

		AssetMgr.Instance.SceneLoad(SceneName);
	}
}
