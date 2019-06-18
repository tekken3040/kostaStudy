using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class ScreenFadeEffect : MonoBehaviour {
	private enum FADE_STATE
	{
		NONE,
		IN,
		OUT,
	}
	private FADE_STATE fadeMode;

	public Image _Image;
	float _currentTime;
	float _time;

	public void FadeIn(float time)
	{
//		gameObject.SetActive(true);
//		_Image.color = new Color(0f, 0f, 0f, 1f);
		bFading = true;
		_time = time;
		_currentTime = time;
		fadeMode = FADE_STATE.IN;
	}

	public void FadeOut(float time)
	{
		gameObject.SetActive(true);
//		_Image.color = new Color(0f, 0f, 0f, 0f);
		_time = time;
		_currentTime = 0;
		fadeMode = FADE_STATE.OUT;
	}

	public bool bFading = false;

	// Use this for initialization
	void Start () {
		_Image = GetComponent<Image>();
		GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
	}
	// Update is called once per frame
	void FixedUpdate () {
		switch(fadeMode)
		{
		case FADE_STATE.NONE:
			return;
		case FADE_STATE.IN:
//			DebugMgr.Log("Fadein : " + _currentTime/_time);
//            PopupManager.Instance.showLoading = true;
			_Image.color = new Color(0f, 0f, 0f, _currentTime/_time);
			_currentTime -= Time.fixedDeltaTime;
			if(_currentTime <= 0)
			{
				_Image.color = new Color(0f, 0f, 0f, 0f);
				fadeMode = FADE_STATE.NONE;
				gameObject.SetActive(false);
				bFading = false;
                PopupManager.Instance.showLoading = false;
			}
			break;
		case FADE_STATE.OUT:
//			DebugMgr.Log("Fadeout : " + _currentTime/_time);
            PopupManager.Instance.showLoading = true;
			_Image.color = new Color(0f, 0f, 0f, _currentTime/_time);
			_currentTime += Time.fixedDeltaTime;
			if(_currentTime >= _time)
			{
				_Image.color = new Color(0f, 0f, 0f, 1f);
				fadeMode = FADE_STATE.NONE;
                PopupManager.Instance.showLoading = false;
			}
			break;
		}
	}

	public void ChangeSceneWithFade(string sceneName){
		gameObject.SetActive(true);
		StartCoroutine(ChangeScene(sceneName));
	}

	IEnumerator ChangeScene(string sceneName){
//		bool tip = false;
//
//		if (SceneManager.GetActiveScene ().name == "LobbyScene" && sceneName != "LobbyScene") {
//			tip = true;
//		}else if (SceneManager.GetActiveScene ().name != "LobbyScene" && sceneName == "LobbyScene") {
//			tip = true;
//		}

		bFading = true;

		_Image.color = new Color(0f, 0f, 0f, 0f);
		FadeOut(FadeEffectMgr.GLOBAL_FADE_TIME);
		yield return new WaitForSeconds (FadeEffectMgr.GLOBAL_FADE_TIME);

		_Image.color = new Color(0f, 0f, 0f, 1f);

//		if (tip) FadeEffectMgr.Instance.SetTip ();

		AssetMgr.Instance.SceneLoad(sceneName);

		yield return new WaitForSeconds (0.1f);

		while (!AssetMgr.Instance.asyncOperation.isDone) {
			_Image.color = new Color(0f, 0f, 0f, 1f);
			yield return new WaitForEndOfFrame ();
		}

		_Image.color = new Color(0f, 0f, 0f, 1f);
//		if (tip) FadeEffectMgr.Instance.DisableTip ();

		FadeIn (FadeEffectMgr.GLOBAL_FADE_TIME);

		yield return new WaitForSeconds (FadeEffectMgr.GLOBAL_FADE_TIME);
		bFading = false;

		//popupName
	}

//	IEnumerator fadeInEffect()
//	{
//		float alphaDec = 1/FadeTimeMS;
//		float alpha = 1;
//		for(int i=0; i<FadeTimeMS; i++)
//		{
//			yield return new WaitForSeconds(0.0001f);
//			// DebugMgr.Log("Fade : " + alpha);
//			alpha -= alphaDec;
//			_Image.color = new Color(0f, 0f, 0f, alpha);
//		}
//		gameObject.SetActive(false);
//	}
//
//
//	IEnumerator fadeOutEffect()
//	{
//		float alphaDec = 1/FadeTimeMS;
//		float alpha = 0;
//		for(int i=0; i<FadeTimeMS; i++)
//		{
//			yield return new WaitForSeconds(0.0001f);
//			// DebugMgr.Log("Fade : " + alpha);
//			alpha += alphaDec;
//			_Image.color = new Color(0f, 0f, 0f, alpha);
//		}
//		gameObject.SetActive(false);
//	}


	

}
