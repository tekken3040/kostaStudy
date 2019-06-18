using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeEffectMgr : Singleton<FadeEffectMgr>
{
	public static readonly float GLOBAL_FADE_TIME = 0.2f;

	protected FadeEffectMgr(){}
	public GameObject _Canvas;
	public GameObject _objFade;
	public GameObject _objTip;
	ScreenFadeEffect _fadeScript;
	UI_LoadingTip _tipScript;

	private void checkFadeObj()
	{
		if(_Canvas.transform.FindChild("Fade") == null)
		{
			GameObject resFade = Resources.Load("Prefabs/UI/Common/Fade", typeof(GameObject)) as GameObject;
			_objFade = Instantiate(resFade) as GameObject;
			RectTransform tr = _objFade.GetComponent<RectTransform>();
			tr.SetParent(_Canvas.transform);
			tr.localPosition = Vector2.zero;
			tr.localScale = Vector2.one;
			tr.sizeDelta = Vector2.zero;
			tr.name = "Fade";
			_fadeScript = _objFade.GetComponent<ScreenFadeEffect>();
			// 2017. 01. 11 jy
			// 부모가 DontDestroyOnLoad 로드 상태라면 자식도 자동으로 적용되며
			// 최상위 부모가 아닌 이상 에러만 경고만 뜨므로 주석
			//DontDestroyOnLoad(_objFade);
		}
	}

	private void checkTipObj()
	{
		if(_Canvas.transform.FindChild("Tip") == null)
		{
			GameObject resTip = AssetMgr.Instance.AssetLoad("Prefabs/UI/Common/Tip.prefab", typeof(GameObject)) as GameObject;
			_objTip = Instantiate(resTip) as GameObject;
			RectTransform tr = _objTip.GetComponent<RectTransform>();
			tr.SetParent(_Canvas.transform);
			tr.localPosition = Vector2.zero;
			tr.localScale = Vector2.one;
			tr.sizeDelta = Vector2.zero;
			tr.name = "Tip";
			_tipScript = _objTip.GetComponent<UI_LoadingTip>();
			// 2017. 01. 11 jy
			// 부모가 DontDestroyOnLoad 로드 상태라면 자식도 자동으로 적용되며
			// 최상위 부모가 아닌 이상 에러만 경고만 뜨므로 주석
			//DontDestroyOnLoad(_objTip);
		}
	}

	public void SetTip()
	{
		checkTipObj ();
		_objTip.SetActive (true);
		_tipScript.SetTip ();
	}

	public void DisableTip()
	{
		_objTip.SetActive (false);
	}

	public void FadeIn()
	{
		checkCanvas();
		checkFadeObj();
		if (_fadeScript.bFading)
			return;
		_objFade.SetActive(true);
		_fadeScript.FadeIn(GLOBAL_FADE_TIME);
	}

	public void FadeOut()
	{
		checkCanvas();
		checkFadeObj();
		if (_fadeScript.bFading)
			return;
		_objFade.SetActive(true);
		_fadeScript.FadeOut(GLOBAL_FADE_TIME);
	}

	public void FadeIn(float time)
	{
//		DebugMgr.Log("FadeIn");
		checkCanvas();
		checkFadeObj();
		if (_fadeScript.bFading)
			return;
		_objFade.SetActive(true);
		_fadeScript.FadeIn(time);
	}

	public void FadeOut(float time)
	{
//		DebugMgr.Log("FadeOut");
		checkCanvas();
		checkFadeObj();
		if (_fadeScript.bFading)
			return;
		_objFade.SetActive(true);
		_fadeScript.FadeOut(time);
	}

	private void checkCanvas()
	{
		if (GameObject.Find ("UI_Effect_Canvas") == null) {
			GameObject resCanvas = Resources.Load("Prefabs/UI/Common/UI_Effect_Canvas", typeof(GameObject)) as GameObject;
			_Canvas = Instantiate (resCanvas) as GameObject;
			_Canvas.transform.SetAsLastSibling ();
			_Canvas.transform.name = "UI_Effect_Canvas";
			DontDestroyOnLoad (_Canvas);
		}
	}

	public void QuickChangeScene(MENU screen, int popup = 0, object[] param = null){
		string sceneName = "";      

		switch (screen) {
        case MENU.TITLE:
            sceneName = "TitleScene";
            break;
		
        case MENU.MAIN:
			sceneName = "LobbyScene";
            GameManager.Instance.ReservePopupMain((POPUP_MAIN)popup, param);                       
			break;
            
        case MENU.INVENTORY:
            sceneName = "LobbyScene";
            GameManager.Instance.ReservePopupInventory((POPUP_INVENTORY)popup, param);    
            break;
        
        case MENU.FORGE:
			sceneName = "ForgeScene";
            GameManager.Instance.ReservePopupForge((POPUP_FORGE)popup, param);
			break;
        
		case MENU.HERO_GUILD:
			sceneName = "QuestScene";
            GameManager.Instance.ReservePopupHeroGuild((POPUP_HERO_GUILD)popup, param);
			break;

		case MENU.QUEST:
			sceneName = "LobbyScene";
			GameManager.Instance.ReservePopupQuest((POPUP_QUEST)popup, param);
			break;
            
        case MENU.SHOP:
			sceneName = "LobbyScene";
            GameManager.Instance.ReservePopupShop((POPUP_SHOP)popup, param);
			break;                     
        
        case MENU.LEAGUE:
			if (AssetMgr.Instance.CheckDivisionDownload(6, 0))
				return;

             sceneName = "ALeagueScene";
             break;
        
        case MENU.CAMPAIGN:
			sceneName = "SelectStageScene";
            GameManager.Instance.ReservePopupCampaign((POPUP_CAMPAIGN)popup, param);
			break;        
        
        case MENU.CREW:
			sceneName = "LobbyScene";
			GameManager.Instance.ReservePopupCrew((POPUP_CREW)popup, param);
			break;
            
        case MENU.CREATE_CHARACTER:
			sceneName = "CreateCharacterScene";
			break;     
            
		case MENU.CHARACTER_INFO:
			sceneName = "LobbyScene";
			GameManager.Instance.ReservePopupCharacterInfo((POPUP_CHARACTER_INFO)popup, param);
			break; 
            
        case MENU.SOCIAL:
            sceneName = "LobbyScene";
			GameManager.Instance.ReservePopupSocial((POPUP_SOCIAL)popup, param);
			break;                       
        
        case MENU.BATTLE:
			break;	

		case MENU.BOSS_RUSH:
			sceneName = "BossRushScene";
			GameManager.Instance.ReservePopupSocial((POPUP_SOCIAL)popup, param);
			break;
		}

		if(sceneName == "") return;

		if(Application.loadedLevelName == sceneName && !_fadeScript.bFading)
            StartCoroutine(Scene.GetCurrent().CheckReservedPopup());
        else
		  _fadeScript.ChangeSceneWithFade(sceneName);
	}

	public void QuickChangeScene(string sceneName){
		_fadeScript.ChangeSceneWithFade(sceneName);
	}
}