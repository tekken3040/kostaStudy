using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;

public class TitleScene : BaseScene {
	private Dictionary<string, Hashtable> versionTable;
//	private string _webURL;
	private string _appURL;
	private string _fileURL;

	private int _serverBundleVersion;

    //readonly string Download_URL = "fil
    readonly string COMPRESS_EXT = ".lzma";
    const double MB_SIZE = 1048576;
    
	[SerializeField] Button _btnTap;
    public GameObject _cheatDetector;
    
    //Download Popup
    public GameObject downloadPopup;
    public Text popupSizeText;
    
    //Download Progress
    public GameObject progressObject;
    public RectTransform totalProgressBar;
	public Text downloadMsgText;
    public Text currentSizeText;
	public Text currentPerText;
    public bool downloadStart = false;

	public OKPopup msgPopup;

	//Login Popup
	public GameObject loginPopup;

	public Text VersionText;

	public const string rootURL = "https://gcrewpublish.blob.core.windows.net/gcrewpatch/";

    string _device;

	public GameObject termsPopup;
	public Toggle perAgree;
	public Toggle useAgree;
	public Toggle pushAgree;

	DateTime updateLimit;
	public NoticePopup _NoticePopup;
	public Image _imgDownloadBG;
	public RectTransform _rtDownloadFadeBG;

    [SerializeField] GameObject objServerGroup;

	void Awake () {
        //Caching.CleanCache ();
        //ObscuredPrefs.DeleteAll ();
		versionTable = new Dictionary<string, Hashtable>();
		Application.runInBackground = true;        
        Input.multiTouchEnabled = false;
//        if(ObscuredPrefs.GetBool("FrameToggle", true))
//		    Application.targetFrameRate = 60;
//        else
//            Application.targetFrameRate = 24;

		Legion.Instance.graphicGrade = ObscuredPrefs.GetInt("Graphic", 1);

		DebugMgr.Log("Sys Language" + Application.systemLanguage.ToString());
		FadeEffectMgr.Instance.FadeIn();		
		//SoundManager.Instance.PlayLoadBGM (Resources.Load("Sound/BGM/BGM_Title", typeof(AudioClip)) as AudioClip);
        Legion.Instance.CancelAllLocalNotifications();
		AssetMgr.Instance.Init ();
		TextManager.Instance.InitCommonText ();
        //this.gameObject.AddComponent<IapManager>().Init();

#if UNITY_EDITOR
		AssetMgr.Instance.useAssetBundle = false;
#else
        AssetMgr.Instance.useAssetBundle = true;
#endif
//      AssetMgr.Instance.useAssetBundle = true;

#if UNITY_ONESTORE
		_device = "onestore";
		//GoogleCloudMessageService.Instance.Init ();
#elif UNITY_ANDROID
        _device = "android";
#elif UNITY_IOS
		_device = "ios";
#else
        _device = "ios";
#endif
	}

	IEnumerator Start()
	{
        Legion.Instance.googleAnalytics.LogEvent(new EventHitBuilder().SetEventCategory("Title").SetEventAction("Enter").SetEventLabel("TitleEnter"));
		Legion.Instance.ratio = ((Screen.height / (Screen.width / 16.0f)) / 9.0f);
		Legion.Instance.ScreenRatio = GetComponent<RectTransform>().sizeDelta;
		_btnTap.enabled = false;

		//yield return new WaitForSeconds(1f);
        //StartCoroutine(PlayStartAni());
        yield return new WaitForSeconds(1f);

        //IapManager.Instance.Init();
		SetVersion ();

		if (PlayerPrefs.GetInt ("SavedPublish", 0) == 0) {
			termsPopup.SetActive (true);
		}else{
			StartCoroutine ("CheckVersion");
		}
	}

	void SetVersion(){
		VersionText.text = Server.ServerMgr.clientVersionString;

		#if dev
		VersionText.text += " D";
		#elif qa
		VersionText.text += " T";
		#endif
		if (AssetMgr.Instance.useAssetBundle) {
			if (ObscuredPrefs.GetInt ("assetVersion", 0) > 0) {
				VersionText.text += " a" + ObscuredPrefs.GetInt ("assetVersion", 0);
			}
		}

		VersionText.text += " s"+Server.ServerMgr.Instance.serverVersion;
	}
    
    public void Login()
    {
		if(PlayerPrefs.GetInt ("Crash", 0) == 1){
		AccountManager.Instance.FBEventLog ("Crashed");
		}
		PlayerPrefs.SetInt ("Crash", 0);

		DataMgr.Instance.LoadAllStep(addLoadStepCount, InitLogin, !Server.ServerMgr.bConnectToServer);
		ObscuredPrefs.SetInt("CrewToChar", 0);        
    }

	IEnumerator CheckVersion()
	{
		TimeSpan sec = new TimeSpan (DateTime.Now.Ticks);
		int update_idx = 0;

		WWW hs_post = new WWW(rootURL+"version_config_"+_device+".ini?time="+sec.TotalSeconds.ToString("#"));

		yield return hs_post;

		if (string.IsNullOrEmpty (hs_post.error)) {
			INIParse (hs_post.text.Trim ());

			update_idx = CheckVersion (GetVersionInfo ("version").Split ('^'));
			_appURL = GetVersionInfo ("app_url");

			if (update_idx == 1){
				#if UNITY_EDITOR
					PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_version_update"), TextManager.Instance.GetText("popup_update_compulsion"), GoToLink);
				#elif UNITY_IOS
					IOSMessage msg = IOSMessage.Create(TextManager.Instance.GetText("popup_title_version_update"), TextManager.Instance.GetText("popup_update_compulsion"));
					msg.OnComplete += GoToLink;
				#elif UNITY_ANDROID
					AndroidMessage msg = AndroidMessage.Create(TextManager.Instance.GetText("popup_title_version_update"), TextManager.Instance.GetText("popup_update_compulsion"));
					msg.ActionComplete += GoToLinkAndroidYesNo;
				#endif
					yield break;
				}else if (update_idx == 2){
					string update_desc = string.Format(TextManager.Instance.GetText("popup_desc_version_update"), updateLimit.Year, updateLimit.Month, updateLimit.Day);
				#if UNITY_EDITOR
					PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_version_update"), update_desc, null);
				#elif UNITY_IOS
					IOSDialog msg = IOSDialog.Create(TextManager.Instance.GetText("popup_title_version_update"), update_desc);
					msg.OnComplete += GoToLinkIOSYesNo;
				#elif UNITY_ANDROID
					AndroidDialog msg = AndroidDialog.Create(TextManager.Instance.GetText("popup_title_version_update"), update_desc);
					msg.ActionComplete += GoToLinkAndroidYesNo;
				#endif
			}

            PopupManager.Instance.SetCouponBtnActive(GetVersionInfo(string.Format("{0}_{1}","coupon", Server.ServerMgr.version)));
			PopupManager.Instance.SetReviewURL (GetVersionInfo ("review_url"));
			PopupManager.Instance.SetImageURL (GetVersionInfo ("image_url"));
			if (Application.systemLanguage == SystemLanguage.Korean) {
				_fileURL = GetVersionInfo ("file_url");
			} else {
				_fileURL = GetVersionInfo ("file_url_global");
			}
		}else{
			string title = TextManager.Instance.GetErrorText("mark_connect_fail", "", false);
			string msg = TextManager.Instance.GetErrorText("mark_load_file_info_fail", "", false);
			PopupManager.Instance.ShowOKPopup(title, msg, RetryVersionCheck);
//			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), hs_post.error, GoToLink);
			yield break;
		}

		hs_post.Dispose ();
		hs_post = null;

		if(update_idx == 0) StartCoroutine("CheckDownload");
	}

	public void RetryVersionCheck(object[] param)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable) 
		{
			string title = TextManager.Instance.GetErrorText("mark_connect_fail", "", false);
			string msg = TextManager.Instance.GetErrorText("mark_please_connect_internet", "", false);
			PopupManager.Instance.ShowOKPopup(title, msg, RetryVersionCheck);
			return;
		}
		StopCoroutine("CheckVersion");
		StartCoroutine("CheckVersion");
	}

	void GoToLink()
	{
		Application.OpenURL (_appURL);
		#if UNITY_EDITOR
		PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_version_update"), TextManager.Instance.GetText("popup_update_compulsion"), GoToLink);
		#elif UNITY_IOS
		IOSMessage msg = IOSMessage.Create(TextManager.Instance.GetText("popup_title_version_update"), TextManager.Instance.GetText("popup_update_compulsion"));
		msg.OnComplete += GoToLink;
		#elif UNITY_ANDROID
		AndroidMessage msg = AndroidMessage.Create(TextManager.Instance.GetText("popup_title_version_update"), TextManager.Instance.GetText("popup_update_compulsion"));
		msg.ActionComplete += GoToLinkAndroidYesNo;
		#endif
	}

	void GoToLinkWithDate()
	{
		Application.OpenURL (_appURL);
		string update_desc = string.Format(TextManager.Instance.GetText("popup_desc_version_update"), updateLimit.Year, updateLimit.Month, updateLimit.Day);
		#if UNITY_EDITOR
		PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_version_update"), update_desc, null);
		#elif UNITY_IOS
		IOSDialog msg = IOSDialog.Create(TextManager.Instance.GetText("popup_title_version_update"), update_desc);
		msg.OnComplete += GoToLinkIOSYesNo;
		#elif UNITY_ANDROID
		AndroidDialog msg = AndroidDialog.Create(TextManager.Instance.GetText("popup_title_version_update"), update_desc);
		msg.ActionComplete += GoToLinkAndroidYesNo;
		#endif
	}

	void GoToLinkIOSYesNo(IOSDialogResult result) 
	{
		switch(result) {
		case IOSDialogResult.YES: 
			GoToLinkWithDate ();
			break;
		case IOSDialogResult.NO: 
			StartCoroutine("CheckDownload");
			break;
		}
	}

	void GoToLinkAndroidYesNo(AndroidDialogResult result) 
	{
		switch(result) {
		case AndroidDialogResult.YES: 
			GoToLinkWithDate ();
			break;
		case AndroidDialogResult.NO: 
			StartCoroutine("CheckDownload");
			break;
		case AndroidDialogResult.CLOSED:
			GoToLink ();
			break;
		}
	}

	public void GoToLink(object[] param)
	{
		Application.OpenURL (_appURL);
	}

	void INIParse(string ini)
	{
		Hashtable sections = new Hashtable ();
		StringReader reader = new StringReader(ini);

		int offset;
		string key = "";
		string section = "";
		string value = "";

		while(true)
		{			
			string readline = reader.ReadLine();

			if(readline == null)
				break;
			offset = readline.IndexOf("[");
			if (offset == 0) {
				if (key != "" && sections.Count > 0) {
					versionTable.Add(key,sections);
				}
				key = readline.Substring (1, readline.IndexOf("]")-1).Trim ();
//				DebugMgr.LogError (key);
				sections = new Hashtable ();
				readline = reader.ReadLine();
			}
				
			offset = readline.IndexOf ("=");
			if (offset > 0) {
				section = readline.Substring (0, offset).Trim ();
				value = readline.Substring (offset + 1).Trim (new char[]{ ' ', '"', ';' });

				if (!sections.ContainsKey (section)) {
					sections.Add (section, value);
				} else {
					string v = GetVersionInfo (section);
					sections.Remove (section);
					sections.Add (section, v + "^" + value);
				}
			}
		}

		reader.Close();
	}

	int CheckVersion(string[] v_array)
	{
		for(int i=0; i<v_array.Length; i++)
		{
			string[] split = v_array [i].Split ('/');

			if (split.Length == 1) {
				if (split [0] == Server.ServerMgr.version)
					return 0;
			} else {
				if (split [0] == Server.ServerMgr.version)
				{
				
					string[] period = split [1].Split ('~');

					int result1 = DateTime.Compare (GetDateByYMDString (period [0], 0, 0, 0), DateTime.UtcNow);
					updateLimit = GetDateByYMDString (period [1], 23, 59, 59);
					int result2 = DateTime.Compare (updateLimit, DateTime.UtcNow);

					if (result1 > 0) {
						return 0;
					}else if (result2 > 0) {
						return 2;
					}
				}
			}
		}
		return 1;
	}

	DateTime GetDateByYMDString(string date, int hour, int min, int sec)
	{
		string[] split = date.Split ('-');

		return new DateTime(Convert.ToInt32(split[0]), Convert.ToInt32(split[1]), Convert.ToInt32(split[2]),hour,min,sec);
	}

	string GetVersionInfo(string section)
	{
		if (section == "file_url_global" && !versionTable [Server.ServerMgr.Instance.GetBuildTag ()].ContainsKey ("file_url_global")) {
			section = "file_url";
		}
		return (string)versionTable[Server.ServerMgr.Instance.GetBuildTag()][section];
	}
    
    IEnumerator CheckDownload()
    {
        PopupManager.Instance.ShowLoadingPopup(1);

		string url = _fileURL;
		AssetBundleData data = new AssetBundleData();

        if(AssetMgr.Instance.useAssetBundle)
        {
		#if UNITY_ONESTORE
		url += "OneStore_Bundle_"+Server.ServerMgr.version+"/";
        #elif UNITY_ANDROID
		url += "Android_Bundle_"+Server.ServerMgr.version+"/";
        #elif UNITY_IOS
		url += "iOS_Bundle_"+Server.ServerMgr.version+"/";
		#else
		url += "iOS_Bundle_"+Server.ServerMgr.version+"/";
        #endif
        }
        else
        {
			Server.ServerMgr.Instance.SetServerAddress (GetVersionInfo ("server_url_"+Server.ServerMgr.version));
            Login();    
            yield break;
        }               

        //string url = "file://" + System.IO.Path.GetFullPath(".") + path;

		TimeSpan sec = new TimeSpan (DateTime.Now.Ticks);

		Server.ServerMgr.Instance.fileServerUrl = url;
                
		WWW www = new WWW(url + "Assets.txt?time="+sec.TotalSeconds.ToString("#"));
        
//        DebugMgr.Log(www.url);
        
        yield return www;
        
        if(string.IsNullOrEmpty(www.error))
        {
//			DebugMgr.LogError (www.text);
			//PlayerPrefs.SetString ("bundleData", www.text);
            data = JsonUtility.FromJson<AssetBundleData>(www.text);
            AssetMgr.Instance.assetBundleData = data;    
        }
        else
        {
//            DebugMgr.Log(www.error);
			string title = TextManager.Instance.GetErrorText("mark_connect_fail", "", false);
			string msg = TextManager.Instance.GetErrorText("mark_load_file_info_fail", "", false);
            PopupManager.Instance.ShowOKPopup(title, msg, Retry);
            yield break;
        }
        
        www.Dispose();
		www = null;

		WWW wwwVer = new WWW(url + "version.txt?time="+sec.TotalSeconds.ToString("#"));

		yield return wwwVer;

		if(string.IsNullOrEmpty(wwwVer.error))
		{
			_serverBundleVersion = Convert.ToInt32(wwwVer.text);
		}
		else
		{
			string title = TextManager.Instance.GetErrorText("mark_connect_fail", "", false);
			string msg = TextManager.Instance.GetErrorText("mark_load_file_info_fail", "", false);
			PopupManager.Instance.ShowOKPopup(title, msg, Retry);
			yield break;
		}

		wwwVer.Dispose();
		wwwVer = null;
        
        PopupManager.Instance.CloseLoadingPopup();
        LeanTween.alpha(GetComponent<TitleAnimation>().TouchArea.GetComponent<RectTransform>(), 0, 0.3f);
        List<AssetBundleData.ArrayEntry> lstDownload = new List<AssetBundleData.ArrayEntry>();
        
        float gagueSize = totalProgressBar.sizeDelta.x;
        
        long downloadSize = 0;
        long decompressSize = 0;
        long curSize = 0;        
        long totalSize = 0;
        
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        for(int i=0; i<data.bundleData.Length; i++)
        {
			if (AssetMgr.Instance.isDivisionBundle (data.bundleData [i].bundleName)) {
//				DebugMgr.LogError (data.bundleData [i].bundleName);
				continue;
			}

            string hash = ObscuredPrefs.GetString(data.bundleData[i].bundleName, "");
            
//			DebugMgr.LogError (data.bundleData [i].bundleName+"/"+data.bundleData[i].compressSize);

            //파일이 없거나 해쉬값이 다르면 다운로드 목록에 추가
            if(string.IsNullOrEmpty(hash) || hash.CompareTo(data.bundleData[i].hash) != 0)
            {
                lstDownload.Add(data.bundleData[i]);
                downloadSize += data.bundleData[i].compressSize;
                decompressSize += data.bundleData[i].size;
				//DebugMgr.Log(data.bundleData[i].bundleName);
            }                          
        }
        
		//DebugMgr.Log(downloadSize);
		//DebugMgr.Log(lstDownload.Count+"/"+data.bundleData.Length);

//		AssetMgr.Instance.CheckDivisionDownload (1, 1);

        if(lstDownload.Count > 0)
        {
            downloadPopup.SetActive(true);
			if (ObscuredPrefs.GetBool ("FirstDownload", true) == true) {
				AccountManager.Instance.FBEventLog ("Landing_01");
				ObscuredPrefs.SetBool ("FirstDownload", false);
			}
            popupSizeText.text = string.Format("{0:F2}MB", downloadSize / MB_SIZE); 
            
            while(!downloadStart)
            {
                yield return null;
            }
            
            downloadPopup.SetActive(false);
            
            progressObject.SetActive(true);

			downloadMsgText.text = TextManager.Instance.GetText ("mark_data_download_progress");
            
            currentSizeText.text = string.Format("({0:F2}MB / {1:F2}MB)", 0, downloadSize / MB_SIZE);
            
            for(int i=0; i<lstDownload.Count; i++)
            {
				if (lstDownload.Count == 0)
				continue;

                WWW download = new WWW(url + lstDownload[i].bundleName + COMPRESS_EXT);            
                
//                DebugMgr.Log(download.url);
                
                curSize = 0;

				downloadMsgText.text = TextManager.Instance.GetText ("mark_data_download_progress");

				yield return new WaitForEndOfFrame();
                
                while(!download.isDone)
                {
                    if(!string.IsNullOrEmpty(download.error))
                        break;
                    
                    curSize = (long)(lstDownload[i].compressSize * download.progress);                                    
                    long tempTotal = totalSize + curSize;                    
                    totalProgressBar.sizeDelta = new Vector2(gagueSize * (float)(tempTotal / (double)downloadSize), totalProgressBar.sizeDelta.y);
					currentPerText.text = ((float)(tempTotal / (double)downloadSize)*100).ToString("0") + "%";
                    currentSizeText.text = string.Format("({0:F2}MB / {1:F2}MB)", tempTotal / MB_SIZE, downloadSize / MB_SIZE);
                    yield return null;
                }
                
                if(!string.IsNullOrEmpty(download.error))
                {
					//DebugMgr.LogError(url+lstDownload[i].bundleName+" / "+download.error);
					string title = TextManager.Instance.GetErrorText("mark_connect_fail", "", false);
					string msg = TextManager.Instance.GetErrorText("mark_load_file_info_fail", "", false);
                    PopupManager.Instance.ShowOKPopup(title, msg, Retry);                    
                    yield break;
                }
                
                curSize = download.bytesDownloaded;
                
                totalSize += curSize;
                
                totalProgressBar.sizeDelta = new Vector2(gagueSize * (float)(totalSize / (double)downloadSize), totalProgressBar.sizeDelta.y);
				currentPerText.text = ((float)(totalSize / (double)downloadSize)*100).ToString("0") + "%";
                currentSizeText.text = string.Format("({0:F2}MB / {1:F2}MB)", totalSize / MB_SIZE, downloadSize / MB_SIZE);

				downloadMsgText.text = TextManager.Instance.GetText ("download_file_unlock");

				yield return new WaitForSeconds(Time.fixedDeltaTime);
                
                string outPath = Application.persistentDataPath + "/" + lstDownload[i].bundleName;
                
                if(!LZMACompress.DecompressFileLZMA(download.bytes, outPath))
                {
					DebugMgr.LogError(lstDownload[i].bundleName+" decompress error !");
                    string title = TextManager.Instance.GetErrorText("mark_connect_fail", "", false);
					string msg = TextManager.Instance.GetErrorText("mark_load_file_info_fail", "", false);
                    PopupManager.Instance.ShowOKPopup(title, msg, Retry);
                    yield break;                    
                }
                
                ObscuredPrefs.SetString(lstDownload[i].bundleName, lstDownload[i].hash);
				lstDownload.RemoveAt (0);
				i--;

//				if (download.assetBundle != null)
//					download.assetBundle.Unload (false);
                download.Dispose();
				download = null;
            }
			

            totalProgressBar.sizeDelta = new Vector2(gagueSize * 1f, totalProgressBar.sizeDelta.y);
			currentPerText.text = "100%";
        }

		ObscuredPrefs.SetInt ("assetVersion", _serverBundleVersion);
		SetVersion ();
		Server.ServerMgr.Instance.SetServerAddress (GetVersionInfo ("server_url_"+Server.ServerMgr.version+"_"+_serverBundleVersion));

		//GC.Collect ();
        
        progressObject.SetActive(false);
        
//      Screen.sleepTimeout = SleepTimeout.SystemSetting;
		EndDownloadBG();
        //DebugMgr.Log("Download Done");
        AssetMgr.Instance.InitAssetBundle();
        Login();        
    }    
    
    public void Retry(object[] param)
    {
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			string title = TextManager.Instance.GetErrorText("mark_connect_fail", "", false);
			string msg = TextManager.Instance.GetErrorText("mark_please_connect_internet", "", false);
			PopupManager.Instance.ShowOKPopup(title, msg, Retry);
			return;
		}
		StopCoroutine("CheckDownload");
        StartCoroutine("CheckDownload");
    }
    
    public void OnClickDownload()
    {
        downloadStart = true;
		StartCoroutine("DownloadBG");
    }

	private void EndDownloadBG()
	{
		StopCoroutine("DownloadBG");
		_imgDownloadBG.gameObject.SetActive(false);
		_rtDownloadFadeBG.GetComponent<Image>().color = Color.black;
	}

	private IEnumerator DownloadBG()
	{
		_imgDownloadBG.gameObject.SetActive(true);
		int count = 2;
		while(true)
		{
			yield return new WaitForSeconds(5.0f);

			LeanTween.alpha(_rtDownloadFadeBG, 1, 0.5f);
			yield return new WaitForSeconds(0.5f);

			_imgDownloadBG.sprite = Resources.Load<Sprite>("Sprites/Title/download_"+ count.ToString("00"));

			LeanTween.alpha(_rtDownloadFadeBG, 0, 0.5f);

			++count;
			if(count > 11 )
				count = 1;
		}
	}
    
	//bool dataLoading=false;
	void addLoadStepCount()
	{
	}

	UInt16 SeqNo = 0;

	public void SelectPublish(int pub){
		switch (pub) {
		case 1:
		// 게스트 로그인 클릭시 경고 팝업을 띄운다
			PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_guest_login"),
												TextManager.Instance.GetText("popup_desc_guest_login"),
												GuestLogin, null,
												ShowLoginWindow, null);
			break;
		case 2:
			PopupManager.Instance.ShowLoadingPopup (1);
			AccountManager.Instance.FBInit ();
			break;
		case 3:
			PopupManager.Instance.ShowLoadingPopup (1);
			#if !UNITY_EDITOR
            AccountManager.Instance.GPInit ();
			#endif
            break;
		case 4:
			//PopupManager.Instance.ShowLoadingPopup (1);
			#if !UNITY_EDITOR
			AccountManager.Instance.InitNaverLogin ();
			#endif
			break;
        }
	}

	//게스트 로그인 & 게스트 회원가입
	public void InitLogin()
	{
		int iPublish = PlayerPrefs.GetInt ("SavedPublish", 0);
		switch (iPublish) {
		case 0:
			PopupManager.Instance.CloseLoadingPopup();
			ShowLoginWindow ();
			break;
		case 1:
			PopupManager.Instance.ShowLoadingPopup (1);
			if (ObscuredPrefs.GetString ("guestID", "") != "") {
				Server.ServerMgr.id = ObscuredPrefs.GetString ("guestID", "");
                EndJoinAndStartLogin (1, SystemInfo.deviceUniqueIdentifier, "");
			} else {
				SeqNo = Server.ServerMgr.Instance.Join (1, SystemInfo.deviceUniqueIdentifier, (Byte)TextManager.Instance.eLanguage , EndJoin);
			}
			break;
		case 2:
			PopupManager.Instance.ShowLoadingPopup (1);
			AccountManager.Instance.FBInit ();
			break;
		case 3:
			PopupManager.Instance.ShowLoadingPopup (1);
			#if !UNITY_EDITOR
            AccountManager.Instance.GPInit ();
			#endif
            break;
		case 4:
			//PopupManager.Instance.ShowLoadingPopup (1);
			#if !UNITY_EDITOR
			AccountManager.Instance.InitNaverLogin ();
			#endif
			break;
        }
	}

	void ShowLoginWindow(object[] param = null)
	{
		loginPopup.SetActive (true);
		if (Application.systemLanguage != SystemLanguage.Korean) {
		loginPopup.transform.FindChild("Popup").FindChild("Lists").FindChild("NaverButton").gameObject.SetActive(false);
		}

        if(TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.ENGLISH)
        {
		loginPopup.transform.FindChild("Popup").FindChild("Lists").FindChild("GPButton").FindChild("GPButtonEng").gameObject.SetActive(true);
        }
	}

	void HideLoginWindow()
	{
		loginPopup.SetActive (false);
	}

	void GuestLogin(object[] obj = null)
	{
		PopupManager.Instance.ShowLoadingPopup (1);
		if (ObscuredPrefs.GetString ("guestID", "") != "") {
			Server.ServerMgr.id = ObscuredPrefs.GetString ("guestID", "");
			EndJoinAndStartLogin (1, SystemInfo.deviceUniqueIdentifier, "");
		} else {
			SeqNo = Server.ServerMgr.Instance.Join (1, SystemInfo.deviceUniqueIdentifier, (Byte)TextManager.Instance.eLanguage, EndJoin);
		}
	}

	public void EndJoinAndStartLogin(int platform, string token, string email)
	{
		PlayerPrefs.SetInt ("SavedPublish", platform);
		HideLoginWindow ();
		if(platform == 1) ObscuredPrefs.SetString ("guestID", Server.ServerMgr.id);

		Server.ServerMgr.Instance.Login((Byte)platform, Server.ServerMgr.id, token, email, EndLogin);
	}

	public void EndJoin(Server.ERROR_ID err)
	{
		//에러검사
		if(err != Server.ERROR_ID.NONE)
		{
			//DebugMgr.Log(err.ToString());
			//DebugMgr.Log("disconnect");
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.AUTH_JOIN, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else
		{
			EndJoinAndStartLogin (1, SystemInfo.deviceUniqueIdentifier, "");
			//DebugMgr.Log("guest relogin");
		}
	}

	public void EndLogin(Server.ERROR_ID err)
	{
		//DebugMgr.Log("EndMethod");
		//DebugMgr.Log ("에러코드 : " + err.ToString());
		//에러검사
		//err = Server.ERROR_ID.PREV_REQUEST_NOT_COMPLETE;

		if (err == Server.ERROR_ID.LOGIN_PW_FAIL || err == Server.ERROR_ID.LOGIN_ID_FAIL) 
		{
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.AUTH_LOGIN, err), DeleteGuestID);

			ShowLoginWindow ();
			return;
		}

		if(err == Server.ERROR_ID.LOGIN_INSPECTION)
		{
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),  TextManager.Instance.GetError(Server.MSGs.AUTH_LOGIN, err), Server.ServerMgr.Instance.CallClear);
			ShowLoginWindow ();
			return;
		}

		if(err == Server.ERROR_ID.LOGIN_BLOCK)
		{
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),  TextManager.Instance.GetError(Server.MSGs.AUTH_LOGIN, err), Server.ServerMgr.Instance.CallClear);
			ShowLoginWindow ();
			return;
		}

		if(err != Server.ERROR_ID.NONE)
		{
			DebugMgr.Log(err.ToString());
			DebugMgr.Log("disconnect");
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),  TextManager.Instance.GetError(Server.MSGs.AUTH_LOGIN, err), Server.ServerMgr.Instance.CallClear);
			ShowLoginWindow ();
			return;
		}
		else
		{	
			// 2016. 10. 07 jy 
			// 타이틀 공지사항을 위하여 순서를 변경
			PopupManager.Instance.CloseLoadingPopup();
			//_btnTap.enabled = true;
			HideLoginWindow ();
            //GetComponent<TitleAnimation>().PressAnimation();
            // 2017. 01. 19 jy
            // 이벤트 팝업 정보를 받기 위하여 호출 한다
			Server.ServerMgr.Instance.RequestGetNotice(CheckRequestNoitice);
			//StartCoroutine(PlayStartAni());
		}
	}
	
	public void CheckRequestNoitice(Server.ERROR_ID err)
	{
		if(err == Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();

#if UNITY_EDITOR
            // PC 테스트용 한글이 아니고 공지가 있다면 팝업을 띄우도록 한다
            //if (TextManager.Instance.eLanguage != TextManager.LANGUAGE_TYPE.KOREAN && SocialInfo.Instance.u1NoticeCount > 0)
            if (SocialInfo.Instance.u1NoticeCount > 0)
#else
                
            //if(Application.systemLanguage.ToString().ToUpper() != "KOREAN" && SocialInfo.Instance.u1NoticeCount > 0 )
            if(SocialInfo.Instance.u1NoticeCount > 0 )
#endif
            {
                OpenNoiticePopup();
			}
			else
			    StartCoroutine(PlayStartAni());
		}
	}

    IEnumerator PlayStartAni()
    {
        yield return new WaitForSeconds(0.1f);
        if(GetComponent<TitleAnimation>().TouchArea.GetComponent<Image>().color.a == 1)
            LeanTween.alpha(GetComponent<TitleAnimation>().TouchArea.GetComponent<RectTransform>(), 0, 0.3f);
        //yield return new WaitForSeconds(0.5f);
        if(Legion.Instance.u2LastLoginServer == 0)
        {
            objServerGroup.SetActive(true);
        }
        else
        {
            StartCoroutine(PlayStartAni2());
        }
    }

    public void OnSelectedServer()
    {
        if(!bPlayAniState)
            StartCoroutine(PlayStartAni2());
    }
    bool bPlayAniState = false;
    IEnumerator PlayStartAni2()
    {
        objServerGroup.SetActive(true);
        _btnTap.enabled = true;
        yield return new WaitForSeconds(0.5f);
        bPlayAniState = true;
        GetComponent<Animator>().enabled = true;
		GetComponent<Animator>().Play ("Start");
    }

	public void OnClickScreen()
	{
        _btnTap.enabled = false;
		//StartCoroutine(GoToLobby());
        RequestLoginInfo();
	}

    void RequestLoginInfo()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.GetLoginInfo(Legion.Instance.u2LastLoginServer, RecieveLoginInfo);
    }

    public void RecieveLoginInfo(Server.ERROR_ID err)
	{
        PopupManager.Instance.CloseLoadingPopup();
		if (err != Server.ERROR_ID.NONE) 
		{
            _btnTap.enabled = true;
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
			return;
		}

		else if(err == Server.ERROR_ID.NONE) 
		{
			objServerGroup.SetActive(false);
            StartCoroutine(GoToLobby());
        }
    }

	IEnumerator GoToLobby()
	{
		while (PopupManager.Instance.objDownloadPopup.gameObject.activeSelf) {
			yield return new WaitForSeconds(0.1f);
		}
		//GetComponent<Animator>().Play ("Pressed");
        GetComponent<TitleAnimation>().PressAnimation();
		yield return new WaitForSeconds(1f);
		//로그인 정보 더 받기
		Server.ServerMgr.Instance.GetLoginMoreInfo(EndLogInMoreInfo);
	}

	public void EndLogInMoreInfo(Server.ERROR_ID err)
	{
		if (err != Server.ERROR_ID.NONE) 
		{
            _btnTap.enabled = true;
		}

		else if(err == Server.ERROR_ID.NONE) 
		{
			if(Legion.Instance.cTutorial.au1Step[0] != 0){
				if(Legion.Instance.acHeros.Count != 0)
				{
					//StartCoroutine(ChangeScene("CreateCharacterScene"));
	                StartCoroutine(ChangeScene("LobbyScene"));
				}
				//생성된 캐릭터 없음
				else
				{
					StartCoroutine(ChangeScene("CreateCharacterScene"));
				}
			} else {
				Legion.Instance.CreateTutorialCrew();
				Legion.Instance.u2SelectStageID = Tutorial.TUTORIAL_STAGE_ID;

				AssetMgr.Instance.SceneLoad("Battle");

				Legion.Instance.cReward = new Reward(Legion.Instance.SelectedStage, 1);
			}
		}

		//게스트 아이디 저장
		ObscuredPrefs.SetString("guestID", Server.ServerMgr.id);
		//DebugMgr.Log("id " + Server.ServerMgr.id);
//		DebugMgr.Log(PlayerPrefs.GetString("guestID"));
	}

	private IEnumerator ChangeScene(string sceneName)
	{
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        if(sceneName == "LobbyScene")
        {
            //AssetMgr.Instance.SceneLoad("LobbyScene_background_01", false);
		    AssetMgr.Instance.SceneLoad(sceneName, true);
        }
        else
            AssetMgr.Instance.SceneLoad(sceneName, false);
//		GetComponent<Animator> ().runtimeAnimatorController = null;
//		Resources.UnloadUnusedAssets ();
	}

	public override void RefreshAlram()
	{
		
	}

	public override IEnumerator CheckReservedPopup()
	{
		yield break;
	}

	public void ShowPersonalTerms()
	{
		//Privacy Policy
		Application.OpenURL ("https://goo.gl/WpaokM");
	}

	public void ShowUseTerms()
	{
		Application.OpenURL ("https://goo.gl/XaZ9dJ");
	}

	public void CheckAllTerms()
	{
		if (perAgree.isOn && useAgree.isOn) {
			termsPopup.SetActive (false);
			Legion.Instance.termspush = true;

            // 푸쉬 활성화를 했을 경우에만 세팅한다
            if(pushAgree.isOn == true)
                ObscuredPrefs.SetBool("IsRequestPushSetting", true);
            else
                Legion.Instance.SetPushAtiveList(false);
            //Legion.Instance.pushActive = true;
            //ObscuredPrefs.SetBool ("pushActive", pushAgree.isOn);
            StartCoroutine(CheckVersion());
		}
	}

	public void DeleteGuestID(object[] param)
	{
		Server.ServerMgr.Instance.CallClear(param);
		ObscuredPrefs.DeleteKey ("guestID");
	}

	// 공지 사항 팝업 띄우기 관련 함수
	private void OpenNoiticePopup()
	{
		_NoticePopup.gameObject.SetActive(true);
	}

	public void CloseNoiticePopup()
	{
		_NoticePopup.gameObject.SetActive(false);
		StartCoroutine(PlayStartAni());
	}
}
