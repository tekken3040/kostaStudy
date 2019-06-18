using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using AssetBundles;
using CodeStage.AntiCheat.ObscuredTypes;

public class UI_BattleLoading2 : MonoBehaviour {
	public Image loadingGauge;
	public Text perText;
	AsyncOperation battleSceneAsync;

	AsyncOperation battleFieldAsync;
	bool battleFieldAsyncFinish;
    bool lobbySceneAsyncFinish;
	// Use this for initialization
	void Start () {
		StartCoroutine(loadScene());
		DontDestroyOnLoad(this.gameObject);
	}

	IEnumerator loadScene()
	{
		FadeEffectMgr.Instance.FadeIn();
//		Resources.UnloadUnusedAssets ();
//		GC.Collect();
		yield return new WaitForEndOfFrame();
//		Resources.UnloadUnusedAssets ();

		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);

        if(Legion.Instance.eGameStyle == GameStyle.ReloadLobby)
        {
            lobbySceneAsyncFinish = true;
        }
        else
        {
			if (Legion.Instance.SelectedStage == null) {
				StartCoroutine(ChangeSceneLobby());

				yield break;
			}

            Legion.Instance.eGameStyle = GameStyle.Stage;
		    AssetMgr.Instance.BattleSceneLoadAsync(Legion.Instance.SelectedStage.acPhases[0].getField().sFieldModelName, false);
        }
		
        battleFieldAsync = AssetMgr.Instance.asyncOperation;
		battleFieldAsync.allowSceneActivation = false;
	}

	IEnumerator ChangeSceneLobby()
	{
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);

		FadeEffectMgr.Instance.FadeOut();

		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		AssetMgr.Instance.SceneLoad("LobbyScene");
	}

	float fillAmount;
	//int fieldIdx=0;
	// Update is called once per frame
	void Update () {

		// null 체크(LoadLevel 실행전에 update가 돌면서 null 에러가 뜸).
		if(battleFieldAsync == null)
			return;

		fillAmount = battleFieldAsync.progress * 0.6f;

		if (battleFieldAsyncFinish == false)
        {
			if (battleFieldAsync.progress == 1)
            {
				if (lobbySceneAsyncFinish) {
					AssetMgr.Instance.SceneLoad ("LobbyScene", false);
					Legion.Instance.eGameStyle = GameStyle.None;
				}else
                    AssetMgr.Instance.BattleSceneLoadAsync ("ControllerScene", true);
				//AssetMgr.Instance.BattleSceneLoadAsync ("ControllerScene", true);
				battleSceneAsync = AssetMgr.Instance.asyncOperation;
				battleSceneAsync.allowSceneActivation = false;
				battleFieldAsyncFinish = true;
                CheckOptions();
			}
            else if (battleFieldAsync.progress >= 0.9f)
				battleFieldAsync.allowSceneActivation = true;
		}


		if (battleSceneAsync != null)
        {
			fillAmount += battleSceneAsync.progress * 0.4f;
			if(battleSceneAsync.progress >= 0.9f) battleSceneAsync.allowSceneActivation = true;
		}

		loadingGauge.fillAmount = fillAmount;
		perText.text = (fillAmount*100).ToString("0")+"%";

		if(fillAmount == 1)
		{
			StartCoroutine(DestroyCanvas());
		}
	}

	IEnumerator DestroyCanvas()
	{
		yield return new WaitForSeconds(0.15f);
		Destroy(gameObject);
	}

    public void CheckOptions()
    {
		Light[] _lightDirectional = GameObject.FindObjectsOfType<Light> ();

		if (_lightDirectional != null) {
			for (int i = 0; i < _lightDirectional.Length; i++) {
				if (_lightDirectional[i].type == LightType.Directional) {
					if (GraphicOption.shadow [Legion.Instance.graphicGrade]) {
						_lightDirectional[i].shadows = LightShadows.Hard;
					} else {
						_lightDirectional[i].shadows = LightShadows.None;
					}
				}
			}
		}
    }
}
