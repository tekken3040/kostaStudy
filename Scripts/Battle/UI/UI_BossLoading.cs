using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_BossLoading : MonoBehaviour {
	public Image loadingGauge;
	public Text perText;
	AsyncOperation battleSceneAsync;
	
	AsyncOperation battleFieldAsync;
	bool battleFieldAsyncFinish;
	// Use this for initialization
	void Start () {
		StartCoroutine(loadScene());
		DontDestroyOnLoad(this.gameObject);
	}
	
	IEnumerator loadScene()
	{
		GameObject beforeMap = GameObject.Find(Legion.Instance.SelectedStage.acPhases[0].getField().sFieldModelName);
		if(beforeMap != null) Destroy(beforeMap);
		beforeMap = null;

		FadeEffectMgr.Instance.FadeIn();

//		Resources.UnloadUnusedAssets ();

		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);

		AssetMgr.Instance.BattleSceneLoadAsync(Legion.Instance.SelectedStage.acPhases[1].getField().sFieldModelName, true);

		battleFieldAsync = AssetMgr.Instance.asyncOperation;
	}
	float fillAmount;
	//int fieldIdx=0;
	// Update is called once per frame
	void Update () {
		if (battleFieldAsync != null) {
			loadingGauge.fillAmount = battleFieldAsync.progress;
			perText.text = (battleFieldAsync.progress*100).ToString ("0") + "%";
		}
		
		if(loadingGauge.fillAmount == 1)
		{
			StartCoroutine(DestroyCanvas());
			CheckOptions();
		}

	}
	
	IEnumerator DestroyCanvas()
	{
		//GC.Collect();
		yield return new WaitForSeconds(0.5f);
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
