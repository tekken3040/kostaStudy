using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
#if UNITY_ANDROID
using GooglePlayGames;
#elif UNITY_IOS
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.OurUtils;
using GooglePlayGames.IOS;
#endif

public class AccountManager : Singleton<AccountManager> {

	ILoginResult loginResult;
	Facebook.Unity.AccessToken token;
	string email;

	void Awake()
	{
		#if UNITY_ANDROID
		PlayGamesPlatform.Activate();
		#elif UNITY_IOS
//		GLinkNaverId.sharedInstance ().init (GLinkConfig.NaverLoginClientId, GLinkConfig.NaverLoginClientSecret);
		#endif
		NaverOAuthAdapter.OnLoginSuccess += OnLoginSuccess;
		NaverOAuthAdapter.OnLoginFailed += OnLoginFailed;
		NaverOAuthAdapter.OnDeleteTokenResult += OnDeleteTokenResult;
		NaverOAuthAdapter.OnRequestApiResult += OnRequestApiResult;
	}

	void OnDestroy () {
		NaverOAuthAdapter.OnLoginSuccess -= OnLoginSuccess;
		NaverOAuthAdapter.OnLoginFailed -= OnLoginFailed;
		NaverOAuthAdapter.OnDeleteTokenResult -= OnDeleteTokenResult;
		NaverOAuthAdapter.OnRequestApiResult -= OnRequestApiResult;
	}

	public void FBEventLog(string eventName) {
		string[] splitStr = eventName.Split('_');
		if (splitStr.Length > 1) {
			string back = splitStr[splitStr.Length - 1];
			string front = "";
			for (int i = 0; i < splitStr.Length - 1; i++) {
				front += splitStr[i];
			}

			#if UNITY_ANDROID
			if (front == "Landing") {
				IgaworksUnityAOS.IgaworksUnityPluginAOS.LiveOps.setTargetingData("Landing", System.Convert.ToInt16(back));
				IgaworksUnityAOS.IgaworksUnityPluginAOS.Adbrix.firstTimeExperience(eventName);
			} else if (front == "SLanding") {
				IgaworksUnityAOS.IgaworksUnityPluginAOS.LiveOps.setTargetingData("SLanding", System.Convert.ToInt16(back));
				IgaworksUnityAOS.IgaworksUnityPluginAOS.Adbrix.firstTimeExperience(eventName);
			}
			#elif UNITY_IOS
			if (front == "Landing") {
				LiveOpsPluginIOS.LiveOpsSetTargetingNumberData(System.Convert.ToInt16(back), "Landing");
				AdBrixPluginIOS.FirstTimeExperience(eventName);
			} else if (front == "SLanding") {
				LiveOpsPluginIOS.LiveOpsSetTargetingNumberData(System.Convert.ToInt16(back), "SLanding");
				AdBrixPluginIOS.FirstTimeExperience(eventName);
			}
			#endif
		}

		if (!FB.IsInitialized) {
			DebugMgr.LogError ("FB.IsInitialized False EN = "+eventName);
			return;
		}

		//		Dictionary<string, object> tutParams = new Dictionary<string, object>();
		//		tutParams[AppEventParameterName.ContentID] = contentID;
		//		tutParams[AppEventParameterName.Success] = "1";

		if (eventName == "Error_3")
			eventName = "Error_CON";

		FB.LogAppEvent(
			eventName,
			1,
			null
		);
	}

	public void FBEventLogWithParam(string eventName, string contentID) {
		if (!FB.IsInitialized) {
			//DebugMgr.LogError ("FB.IsInitialized False");
			return;
		}

		Dictionary<string, object> tParams = new Dictionary<string, object>();
		tParams[AppEventParameterName.ContentID] = contentID;

		FB.LogAppEvent(
			eventName,
			1,
			tParams
		);
	}

	public void FBLogPurchase(string sPrice) {
		if (!FB.IsInitialized) {
			//DebugMgr.LogError ("FB.IsInitialized False");
			return;
		}

		float fPrice = (float)Convert.ToDouble(sPrice);

		FB.LogPurchase(
			fPrice,
			"USD",
			null
		);
	}

	public void FBInit() {
		//DebugMgr.LogError ("IsInitialized"+FB.IsInitialized);

		if (FB.IsLoggedIn) {
			token = AccessToken.CurrentAccessToken;
			GetEmailWithJoinResponse();
			return;
		}

		if (!FB.IsInitialized) {
			FB.Init(this.FBInitComplete, this.OnHideUnity);
		} else {
			FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.InitResult);
		}
	}

	void FBInitComplete() {
		if (FB.IsLoggedIn) {
			token = AccessToken.CurrentAccessToken;
			if (token == null) DebugMgr.LogError("token null");
			else DebugMgr.Log(token.TokenString);

			GetEmailWithJoinResponse();
		} else {
			FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.InitResult);
		}
	}

	void GetEmailWithJoinResponse() {
		FB.API("/me?fields=email", HttpMethod.GET, graphResult =>
			{
				if (string.IsNullOrEmpty(graphResult.Error) == false)
				{
					DebugMgr.Log("could not get email address");
					return;
				}

				IDictionary dict = Facebook.MiniJSON.Json.Deserialize(graphResult.RawResult) as IDictionary;
				if (dict.Contains("email")) email = dict["email"].ToString();
				else email = "";
				Server.ServerMgr.Instance.Join(2, token.TokenString, (Byte)TextManager.Instance.eLanguage, EndFBJoin);
			});
	}

	void GetEmailWithConnectResponse() {
		FB.API("/me?fields=email", HttpMethod.GET, graphResult =>
			{
				if (string.IsNullOrEmpty(graphResult.Error) == false)
				{
					DebugMgr.Log("could not get email address");
					return;
				}

				IDictionary dict = Facebook.MiniJSON.Json.Deserialize(graphResult.RawResult) as IDictionary;
				if (dict.Contains("email")) email = dict["email"].ToString();
				else email = "";
				Server.ServerMgr.Instance.OptionAccount(2, loginResult.AccessToken.TokenString, email, ResultFBConnect);
			});
	}

	protected void InitResult(IResult result)
	{
		if (result == null)
		{
			DebugMgr.Log("Null Response\n");
			return;
		}

		// Some platforms return the empty string instead of null.
		if (!string.IsNullOrEmpty(result.Error))
		{
			PopupManager.Instance.CloseLoadingPopup();
			DebugMgr.Log("Error Response:\n" + result.Error);
		}
		else if (result.Cancelled)
		{
			PopupManager.Instance.CloseLoadingPopup();
			DebugMgr.Log("Cancelled Response:\n" + result.RawResult);
		}
		else if (!string.IsNullOrEmpty(result.RawResult))
		{
			loginResult = (ILoginResult)result;
			token = loginResult.AccessToken;
			GetEmailWithJoinResponse();
			DebugMgr.Log("Success Response:\n" + result.RawResult);
		}
		else
		{
			PopupManager.Instance.CloseLoadingPopup();
			DebugMgr.Log("Empty Response\n");
		}

		DebugMgr.Log("result : " + result.ToString());
	}

	void EndFBJoin(Server.ERROR_ID err) {

		if (err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),
				TextManager.Instance.GetError(Server.MSGs.AUTH_JOIN, err) + TextManager.Instance.GetText("accounts_regist_fail"),
				null);
			return;
		}

		else if (err == Server.ERROR_ID.NONE)
		{
			TitleScene titleScene = Scene.GetCurrent() as TitleScene;
			if (titleScene != null) {
				titleScene.EndJoinAndStartLogin(2, GetFBAccessToken(), email);
			}
		}
	}

	public void FBConnect() {
		DebugMgr.LogError("ConnectInitialized" + FB.IsInitialized);
		if (!FB.IsInitialized) {
			FB.Init(this.FBConnectComplete, this.OnHideUnity);
		} else {
			PopupManager.Instance.ShowLoadingPopup(1);
			FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.FBConnectResult);
		}
	}

	void FBConnectComplete() {
		PopupManager.Instance.ShowLoadingPopup(1);
		FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.FBConnectResult);
	}

	void OnHideUnity(bool isGameShown)
	{
		DebugMgr.Log("Is game shown: " + isGameShown);
	}

	public string GetFBAccessToken() {
		if (FB.IsLoggedIn) {
			return token.TokenString;
		}
		return "";
	}

	protected void FBConnectResult(IResult result)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if (result == null)
		{
			DebugMgr.Log("Null Response\n");
			return;
		}

		// Some platforms return the empty string instead of null.
		if (!string.IsNullOrEmpty(result.Error))
		{
			DebugMgr.Log("Error Response:\n" + result.Error);
		}
		else if (result.Cancelled)
		{
			DebugMgr.Log("Cancelled Response:\n" + result.RawResult);
		}
		else if (!string.IsNullOrEmpty(result.RawResult))
		{
			loginResult = (ILoginResult)result;
			token = loginResult.AccessToken;
			GetEmailWithConnectResponse();
			DebugMgr.Log("Success Response:\n" + result.RawResult);
		}
		else
		{
			DebugMgr.Log("Empty Response\n");
		}

		DebugMgr.Log(result.ToString());
	}

	void ResultFBConnect(Server.ERROR_ID err) {
		PopupManager.Instance.CloseLoadingPopup();
		if (err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),
				TextManager.Instance.GetError(Server.MSGs.OPTION_ACCOUNT, err),
				Server.ServerMgr.Instance.CallClear);
			return;
		}

		else if (err == Server.ERROR_ID.NONE)
		{
			PlayerPrefs.SetInt("SavedPublish", 2);
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("accounts_resist_success"),
				TextManager.Instance.GetText("accounts_resist_success_desc"),
				null);
		}
	}

	void FBLogout() {
		if (FB.IsLoggedIn) {
			FB.LogOut();
		}
	}

	public void Logout() {

		int pub = PlayerPrefs.GetInt("SavedPublish");

		if (pub == 1) {

		} else if (pub == 2) {
			FBLogout();
		} else if (pub == 3) {
			#if (UNITY_ANDROID || (UNITY_IOS && !NO_GPGS))
			GPLogout();
			#endif
		}
		else if (pub == 4)
		{
			#if UNITY_ANDROID
			NaverLogout();
			#elif UNITY_IOS

			#endif
		}
	}

	#if UNITY_ANDROID
	bool bGPLogin = false;
	bool bJoin = false;

	public void GPInit() {
		bGPLogin = false;
		Legion.Instance.osLogin = false;
		bJoin = true;

		DebugMgr.LogError ("GPInit = "+GooglePlayConnection.State.ToString());

		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED) {
			OnPlayerConnected();
		} else {
			GooglePlayConnection.ActionPlayerConnected += OnPlayerConnected;
			GooglePlayConnection.Instance.Connect();
		}
	}

	private void OnPlayerConnected() {
		DebugMgr.LogError ("OnPlayerConnected");
		bGPLogin = true;
		Legion.Instance.osLogin = true;

		if (bGPLogin) {
			GooglePlayManager.ActionOAuthTokenLoaded += ActionOAuthTokenLoaded;
			GooglePlayManager.Instance.LoadToken();
		} else {
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_login_fail"),
			TextManager.Instance.GetText("popup_desc_login_fail"),
			null);
		}

		GooglePlayConnection.ActionPlayerConnected -= OnPlayerConnected;
	}

	private void ActionOAuthTokenLoaded(string token) {
		if (string.IsNullOrEmpty (token)) {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("title_notice"), TextManager.Instance.GetText ("popup_google_permission_agree"), Server.ServerMgr.Instance.ApplicationShutdown);
			GooglePlayManager.ActionOAuthTokenLoaded -= ActionOAuthTokenLoaded;
			PopupManager.Instance.CloseLoadingPopup();
			return;
		}
		email = GooglePlayManager.Instance.currentAccount;
		//email = GooglePlayConnection.Player.PlayerId;
		if (bJoin) Server.ServerMgr.Instance.Join(3, token, (Byte)TextManager.Instance.eLanguage, EndGPJoin);
		else Server.ServerMgr.Instance.OptionAccount(3, token, email, ResultGPConnect);

		GooglePlayManager.ActionOAuthTokenLoaded -= ActionOAuthTokenLoaded;
	}

	private void OnPlayerDisconnected() {
		DebugMgr.LogError ("OnPlayerDisconnected");
		bGPLogin = false;
		Legion.Instance.osLogin = false;
		GooglePlayConnection.ActionPlayerDisconnected -= OnPlayerDisconnected;
	}

	void EndGPJoin(Server.ERROR_ID err) {
		if (err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),
			TextManager.Instance.GetError(Server.MSGs.AUTH_JOIN, err) + TextManager.Instance.GetText("accounts_regist_fail"),
			Server.ServerMgr.Instance.ApplicationShutdown);
			PopupManager.Instance.CloseLoadingPopup();
			return;
		}

		else if (err == Server.ERROR_ID.NONE)
		{
			TitleScene titleScene = Scene.GetCurrent() as TitleScene;
			if (titleScene != null) {
				titleScene.EndJoinAndStartLogin(3, GetGPAccessToken(), email);
			}
		}
	}

	public string GetGPAccessToken() {
		if (bGPLogin) {
			return GooglePlayManager.Instance.loadedAuthToken;
		}
		return "";
	}


	public void GPConnect() {
		bGPLogin = false;
		Legion.Instance.osLogin = false;
		bJoin = false;

		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED) {
				OnPlayerConnected();
			} else {
				GooglePlayConnection.ActionPlayerConnected += OnPlayerConnected;
				GooglePlayConnection.Instance.Connect();
			}
		}

	public void GPLogout() {
		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
		{
			DebugMgr.LogError ("GPLogout");
			GooglePlayConnection.ActionPlayerDisconnected += OnPlayerDisconnected;
			GooglePlayConnection.Instance.Disconnect();
		}
	}

	void ResultGPConnect(Server.ERROR_ID err) {
		PopupManager.Instance.CloseLoadingPopup();

		if (err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),
			TextManager.Instance.GetError(Server.MSGs.OPTION_ACCOUNT, err),
			Server.ServerMgr.Instance.CallClear);
			return;
		}
		else if (err == Server.ERROR_ID.NONE)
		{
			PlayerPrefs.SetInt("SavedPublish", 3);
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("accounts_resist_success"),
			TextManager.Instance.GetText("accounts_resist_success_desc"),
			null);
		}
	}

	#elif (UNITY_IOS && !NO_GPGS)
	bool bGPLogin = false;

	public void GPInit(){
		bGPLogin = false;

		if (!GooglePlayGames.PlayGamesPlatform.Instance.IsAuthenticated()) {
			PopupManager.Instance.ShowLoadingPopup(1);
			GooglePlayGames.PlayGamesPlatform.Instance.Authenticate (GPInitResult);
		}
	}

	void GPInitResult(bool result){
		bGPLogin = result;
		if (bGPLogin) {
	#if UNITY_IOS
			email = "";
	#elif UNITY_ANDROID
			email = ((GooglePlayGames.PlayGamesLocalUser)Social.localUser).Email;
	#endif
			Server.ServerMgr.Instance.Join (3, GetGPAccessToken(), (Byte)TextManager.Instance.eLanguage, EndGPJoin);
		} else {
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup("로그인 실패",
				"계정 로그인에 실패했습니다.",
				null);
		}
	}

	void EndGPJoin(Server.ERROR_ID err){
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),
				TextManager.Instance.GetError(Server.MSGs.AUTH_JOIN, err)+TextManager.Instance.GetText("accounts_regist_fail"),
				Server.ServerMgr.Instance.ApplicationShutdown);
			PopupManager.Instance.CloseLoadingPopup();
			return;
		}

		else if (err == Server.ERROR_ID.NONE)
		{
			TitleScene titleScene = Scene.GetCurrent() as TitleScene;
			if (titleScene != null) {
				titleScene.EndJoinAndStartLogin (3, GetGPAccessToken(), email);
			}
		}
	}

	public string GetGPAccessToken(){
		if (bGPLogin) {
			return GooglePlayGames.PlayGamesPlatform.Instance.GetAccessToken ();
		}
		return "";
	}


	public void GPConnect(){
		bGPLogin = false;

		if (!GooglePlayGames.PlayGamesPlatform.Instance.IsAuthenticated ()) {
			PopupManager.Instance.ShowLoadingPopup (1);
			GooglePlayGames.PlayGamesPlatform.Instance.Authenticate (GPConnectResult);
		}
	}

	void GPConnectResult(bool result){
		bGPLogin = result;
		if (bGPLogin) {
	#if UNITY_IOS
			email = "";
	#elif UNITY_ANDROID
			email = ((GooglePlayGames.PlayGamesLocalUser)Social.localUser).Email;
	#endif
			Server.ServerMgr.Instance.OptionAccount (3, GetGPAccessToken(), email, ResultGPConnect);
		} else {
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup("로그인 실패",
				"계정 로그인에 실패했습니다.",
				null);
		}
	}

	public void GPLogout(){
		if (GooglePlayGames.PlayGamesPlatform.Instance.IsAuthenticated ()) {
			GooglePlayGames.PlayGamesPlatform.Instance.SignOut ();
			bGPLogin = false;
		}
	}

	void ResultGPConnect(Server.ERROR_ID err){
		PopupManager.Instance.CloseLoadingPopup();
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),
				TextManager.Instance.GetError(Server.MSGs.OPTION_ACCOUNT, err),
				Server.ServerMgr.Instance.CallClear);
			return;
		}

		else if (err == Server.ERROR_ID.NONE)
		{
			PlayerPrefs.SetInt ("SavedPublish", 3);
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("accounts_resist_success"),
				TextManager.Instance.GetText("accounts_resist_success_desc"),
				null);
		}
	}
	#endif

	//2017.01.12 GENERAL
	//naver login

	private NaverOAuthAdapter adapter;
	string naverToken = null;

	bool bLogin = false;

	public NaverOAuthAdapter GetAdapter
	{
		get
		{
			return adapter;
		}
	}

	public void InitNaverLogin()
	{
		bLogin = true;
		NaverLogin ();
	}

	void NaverLogin()
	{
		if (adapter != null)
		{
			if (adapter.CheckToken ()) {
				adapter.Login ();
			} else {
				adapter.RefreshToken ();
			}
		}

		else
		{
			adapter = NaverOAuthAdapter.getInstance();
			adapter.Initialize(GLinkConfig.NaverLoginClientId, GLinkConfig.NaverLoginClientSecret, "GuardiansWar", "GCNaverLogin");
			if (adapter.CheckToken ()) {
				adapter.Login ();
			} else {
				adapter.RefreshToken ();
			}
		}
	}

	public void NaverConnect()
	{
		bLogin = false;
		NaverLogin ();
	}

	public void NaverLogout()
	{
		if (adapter != null)
		{
			adapter.Logout();
			naverToken = "";
		}
	}

	public void NaverTokenDelete()
	{
		if (adapter != null)
		{
			adapter.LogoutAndDeleteToken();
		}
	}

	void OnLoginSuccess(object sender, NALoginEventArgs e)
	{
		DebugMgr.Log("OnLoginSuccess = "+e.accessToken);
		PopupManager.Instance.ShowLoadingPopup(1);
		naverToken = e.accessToken;
		if (bLogin) {
			Server.ServerMgr.Instance.Join (4, naverToken, (Byte)TextManager.Instance.eLanguage, EndNaverJoin);
		} else {
			Server.ServerMgr.Instance.OptionAccount (4, naverToken, "", ResultNaverConnect);
		}
	}

	void OnLoginFailed(object sender, NAErrorEventArgs e)
	{
		PopupManager.Instance.CloseLoadingPopup();
		//translate
		PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_login_fail"),TextManager.Instance.GetText("popup_desc_login_fail"), null);
		NaverTokenDelete();
		DebugMgr.Log("OnLoginFailed = "+e.message.ToString());

		TitleScene titleScene = Scene.GetCurrent() as TitleScene;
		if (titleScene != null)
		{
			titleScene.loginPopup.SetActive (true);
		}
	}

	void OnDeleteTokenResult (object sender, NAFlagEventArgs e) {
		DebugMgr.Log("OnDeleteTokenResult = "+e.value);
	}

	void OnRequestApiResult (object sender, NAMessageEventArgs e) {
		DebugMgr.Log("OnRequestApiResult = "+e.value);
	}

	void EndNaverJoin(Server.ERROR_ID err)
	{

		if (err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),
				TextManager.Instance.GetError(Server.MSGs.AUTH_JOIN, err) + TextManager.Instance.GetText("accounts_regist_fail"),
				null);
			return;
		}

		else if (err == Server.ERROR_ID.NONE)
		{
			TitleScene titleScene = Scene.GetCurrent() as TitleScene;
			if (titleScene != null)
			{
				titleScene.EndJoinAndStartLogin(4, naverToken, "");
			}
		}
	}

	void ResultNaverConnect(Server.ERROR_ID err){
		PopupManager.Instance.CloseLoadingPopup();
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),
				TextManager.Instance.GetError(Server.MSGs.OPTION_ACCOUNT, err),
				Server.ServerMgr.Instance.CallClear);
			return;
		}

		else if (err == Server.ERROR_ID.NONE)
		{
			PlayerPrefs.SetInt ("SavedPublish", 4);
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("accounts_resist_success"),
				TextManager.Instance.GetText("accounts_resist_success_desc"),
				null);
		}
	}

}
