using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.Collections.Generic;
using System.Collections;
using System;

public class AdvertiseBtn : MonoBehaviour {

	Button btn;
	Text text;
	Text count;
	Image icon;

	public enum AdvScrType{
		Stage = 1,
		Shop = 2,
		VIP = 3
	}

	public AdvScrType screen = AdvScrType.Stage; 

	void Awake () {
		btn = gameObject.GetComponent<Button> ();
		text = transform.FindChild ("Text").GetComponent<Text> ();
		count = transform.FindChild ("Count").GetComponent<Text> ();
		icon = transform.FindChild ("Icon").GetComponent<Image> ();

		//UM_AdManager.Init();
	}

	void Start(){
		if ((int)screen == 0 || (int)screen > 3) {
			return;
		}
		SetInfo ();
	}

	void SetInfo(){
		if (Legion.Instance.adLeftTime[(int)screen - 1] > 0) {
			count.text = (Legion.Instance.adLeftTime [(int)screen - 1]/60)+":"+(Legion.Instance.adLeftTime [(int)screen - 1]%60).ToString("00");
		} else {
			count.text = (EventInfoMgr.Instance.dicAdReward [(Byte)screen].u1ViewCount - Legion.Instance.adRemainCount [(int)screen - 1]) + "/" + EventInfoMgr.Instance.dicAdReward [(Byte)screen].u1ViewCount;
		}
	}

	public void OnEnable(){
		CheckEnd ();
	}

	public void OnClickBtn(){
		LoadAd ();
	}

	void LoadAd() {
		if (Legion.Instance.adRemainCount [(int)screen - 1] <= 0)
			return;

		// 2016. 10. 25 jy 
		// 재화 오버 확인
		if(Legion.Instance.CheckGoodsLimitExcessx(EventInfoMgr.Instance.dicAdReward [(Byte)screen].cReward.u1Type) == true)
		{
			Legion.Instance.ShowGoodsOverMessage(EventInfoMgr.Instance.dicAdReward [(Byte)screen].cReward.u1Type);
			return;
		}

		PopupManager.Instance.ShowLoadingPopup (1);
//		//subscribing to the Interstitial events
//		UM_AdManager.OnInterstitialLoaded += HandleOnInterstitialLoaded;
//		UM_AdManager.OnInterstitialLoadFail += HandleOnInterstitialLoadFail;
//		UM_AdManager.OnInterstitialClosed += HandleOnInterstitialClosed;
//
//		//pre-loading Interstitial content
//		UM_AdManager.LoadInterstitialAd();
		switch((int)screen){
		case 1:
			ShowAdColoy ();
			break;
		case 2:
			ShowAdColoy ();
			break;
		case 3:
			ShowAdColoy ();
			break;
		}
	}

	void ShowAdColoy(){
		#if UNITY_ANDROID
		if(AdColony.IsVideoAvailable("vz0907fb8e95b947d89d")){
			if(!AdColony.ShowVideoAd ("vz0907fb8e95b947d89d")){
				ShowVungle();
			}
		}else{
			ShowVungle();
		}
		#elif UNITY_IOS
		if(AdColony.IsVideoAvailable("vze65cf270b2e64e9895")){
			if(!AdColony.ShowVideoAd ("vze65cf270b2e64e9895")){
				ShowVungle();
			}
		}else{
			ShowVungle();
		}
		#endif
		AdColony.OnVideoFinishedWithInfo += AdColonyFinished;
	}

	void ShowVungle(){
		#if UNITY_ANDROID
		ShowUnityAds();
		#elif UNITY_IOS
		if(Vungle.isAdvertAvailable()){
			Dictionary<string, object> options = new Dictionary<string, object> ();
			options ["incentivized"] = true;
			Vungle.playAdWithOptions (options);
			Vungle.onAdFinishedEvent += VungleFinished;
		}else{
			ShowUnityAds();
		}
		#endif

	}

	void ShowUnityAds(){
		if (Advertisement.IsReady ()) {
			ShowOptions opt = new ShowOptions ();
			opt.resultCallback = UnityAdsFinished;
			Advertisement.Show (opt);
		} else {
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("popup_ad_empty"), null);

		}
	}

	void UnityAdsFinished(ShowResult res)
	{
		if (res == ShowResult.Finished || res == ShowResult.Skipped) {
			Server.ServerMgr.Instance.RequestAdReward ((Byte)screen, rewardResult);
		} else {
			PopupManager.Instance.CloseLoadingPopup();
		}
	}

	#if UNITY_IOS
	void VungleFinished(AdFinishedEventArgs arg)
	{
		if (arg.IsCompletedView) {
			Server.ServerMgr.Instance.RequestAdReward ((Byte)screen, rewardResult);
		} else {
			PopupManager.Instance.CloseLoadingPopup();
		}
		Vungle.onAdFinishedEvent -= VungleFinished;
	}
	#endif
	void AdColonyFinished(AdColonyAd ad_shown)
	{
		if (ad_shown.shown) {
			Server.ServerMgr.Instance.RequestAdReward ((Byte)screen, rewardResult);
		} else {
			PopupManager.Instance.CloseLoadingPopup();
		}
		AdColony.OnVideoFinishedWithInfo -= AdColonyFinished;
	}

//	void HandleOnInterstitialClosed () {
//		Server.ServerMgr.Instance.RequestAdReward ((Byte)screen, rewardResult);
//		UM_AdManager.ResetActions();
//	}

	// 보상 결과 처리
	private void rewardResult(Server.ERROR_ID err)
	{
		//DebugMgr.Log("rewardResult " + err);

		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EVENT_REWARD, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			if(screen == AdvScrType.VIP)
			{
				UI_VIPInfoPopUp vipInfo = transform.parent.GetComponent<UI_VIPInfoPopUp>();
				if(vipInfo != null)
					vipInfo.RefleshPlayerVIPInfo();
			}
			count.text = (EventInfoMgr.Instance.dicAdReward [(Byte)screen].u1ViewCount-Legion.Instance.adRemainCount [(int)screen - 1]) + "/" + EventInfoMgr.Instance.dicAdReward [(Byte)screen].u1ViewCount;
		}

		CheckEnd ();
	}

	void CheckEnd(){
		if (Legion.Instance.adRemainCount [(int)screen - 1] <= 0) {
			btn.interactable = false;
			text.color = Color.gray;
			icon.color = Color.gray;
		}else if (Legion.Instance.adLeftTime [(int)screen - 1] > 0) {
			btn.interactable = false;
			text.color = Color.gray;
			icon.color = Color.gray;
		}
	}

	void Update(){
		SetInfo ();
	}

//	void HandleOnInterstitialLoadFail () {
//		PopupManager.Instance.CloseLoadingPopup ();
//		DebugMgr.Log ("Interstitial is failed to load");
//		UM_AdManager.ResetActions();
//	}
//
//	void HandleOnInterstitialLoaded () {
//		DebugMgr.Log ("Interstitial ad content ready");
//
//		//Content was loaded, we can now show the Interstitial ad.
//		UM_AdManager.ShowInterstitialAd();
//	}
}
