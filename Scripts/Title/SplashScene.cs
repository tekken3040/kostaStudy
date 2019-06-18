using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;
using IgaworksUnityAOS;
using Facebook.Unity;

public class SplashScene : MonoBehaviour
{
    public GameObject Btn_Title;
    public GameObject ImgLuno;
    public GameObject ImgPlay260;
	float vol;

    void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		IgaworksUnityPluginAOS.InitPlugin ();
#endif
		StartCoroutine("GotoTitle");
    }

	void Start()
	{
#if UNITY_ANDROID
		IgaworksUnityPluginAOS.Common.startApplication ();
		IgaworksUnityPluginAOS.Common.startSession();
#endif
		if(PlayerPrefs.GetInt ("ApplicationQuit", 1) == 0){
			PlayerPrefs.SetInt ("Crash", 1);
		}
		PlayerPrefs.SetInt ("ApplicationQuit", 0);

#if !UNITY_EDITOR
		if (FB.IsInitialized) {
			FB.ActivateApp();
		} else {
			FB.Init( () => {
				FB.ActivateApp();
			});
		}
#endif
	}

    public void OnClickTitle()
    {
        //DebugMgr.LogError("On Click Touch");
        StopCoroutine("GotoTitle");
        Btn_Title.GetComponent<Button>().interactable = false;
        this.GetComponent<ChangeScene>().LoadScene("TitleScene");
    }

    IEnumerator GotoTitle()
    {
        yield return new WaitForSeconds(1f);
        LeanTween.value(ImgLuno, 1f, 0f, 0.5f).setOnUpdate((float alpha)=> { ImgLuno.GetComponent<CanvasGroup>().alpha = alpha; });
        yield return new WaitForSeconds(0.5f);
        ImgLuno.SetActive(false);
        ImgPlay260.SetActive(true);
        LeanTween.value(ImgLuno, 0f, 1f, 0.5f).setOnUpdate((float alpha)=> { ImgPlay260.GetComponent<CanvasGroup>().alpha = alpha; });
        yield return new WaitForSeconds(2f);
        Btn_Title.GetComponent<Button>().interactable = false;
        this.GetComponent<ChangeScene>().LoadScene("TitleScene");
    }
}
