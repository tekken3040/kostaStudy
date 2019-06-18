using UnityEngine;
#if UNITY_EDITOR	
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using CodeStage.AntiCheat.ObscuredTypes;

[System.Serializable]
public class AssetBundleData
{
    [System.Serializable]
    public struct ArrayEntry
    {
        public string bundleName;
        public string hash;
        public long size;
        public long compressSize;
        public string[] includeAsset;
        public string[] dependanciesBundle;
    }
    
    public ArrayEntry[] bundleData; 
}

public class DivisionData
{
	public int pos;
	public int subID;
	public int fileType;
	public int count;
	public List<string> fileList;

	public void Set(string[] cols)
	{
		fileList = new List<string> ();

		int idx = 0;
		pos = Convert.ToInt32 (cols [idx++]);
		subID = Convert.ToInt32 (cols [idx++]);
		fileType = Convert.ToInt32 (cols [idx++]);
		count = Convert.ToInt32 (cols [idx++]);

//		string sub_asset_name = "";
//
//		if(fileType == 1) sub_asset_name = "gc_animators_hero_anim_";

		for (int i = 0; i < count; i++) {
			string front_asset_name = "gc_";

			switch (fileType) {
			case 1:
				continue;//front_asset_name += "prefabs_models_hero_";
				break;
			case 2:
				front_asset_name += "prefabs_models_monster_";
				break;
			case 3:
				front_asset_name += "animators_monster_anim_";
				break;
			case 4:
				front_asset_name += "";
				break;
			case 5:
				front_asset_name += "skyboxs_";
				break;
			}

			string keyVal = cols [idx++];
			keyVal = keyVal.Replace ('/', '_');
			keyVal = keyVal.ToLower();

			if(!fileList.Contains(front_asset_name + keyVal))
				fileList.Add(front_asset_name + keyVal);
		}

//		if (sub_asset_name != "") {
//			if(!fileList.Contains(sub_asset_name + subID))
//				fileList.Add (sub_asset_name + subID);
//		}
	}
}


public class AssetMgr : Singleton<AssetMgr> {    

	public class LoadedAssetBundle
	{
		public AssetBundle m_AssetBundle;
		public int m_ReferencedCount;

		public LoadedAssetBundle(AssetBundle assetBundle)
		{
			m_AssetBundle = assetBundle;
			m_ReferencedCount = 1;
		}
	}

	bool bUnloadAsset = true;

	public AsyncOperation asyncOperation;
	public string beforeScene;
    public bool useAssetBundle = false;
    public AssetBundleData assetBundleData;

	public List<DivisionData> divisionBundleData;

	static Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle> ();
	public Dictionary<string, string[]> resourcesBundlePath = new Dictionary<string, string[]> ();

	static Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]> ();

	List<string> m_unloadedBundles = new List<string> ();

	List<AssetBundleData.ArrayEntry> lstDownload = new List<AssetBundleData.ArrayEntry> ();

	//private string downloadURL = "file:///Users/PKH/Git/ParabullemStudio/3. Development/98.Client/COC_Unity/AssetBundles/";
	//private string downloadURL = "file:///C:/Git/ParabullemStudio/3. Development/98.Client/COC_Unity/AssetBundles/";

	readonly string bundleFolder = "Assets/Resources_Bundle/";
    readonly string resourceFolder = "Assets/Resources/";

	public bool bCanCancel = false;

	long downloadSize = 0;
	long decompressSize = 0;
	long curSize = 0;        
	long totalSize = 0;

	readonly string COMPRESS_EXT = ".lzma";
	const double MB_SIZE = 1048576;

	public void Init()
	{
		ForceUnloadAll ();

		assetBundleData = null;
		assetBundleData = new AssetBundleData();
		divisionBundleData = new List<DivisionData> ();

		resourcesBundlePath.Clear ();
		resourcesBundlePath = new Dictionary<string, string[]> ();
		m_Dependencies.Clear ();
		m_Dependencies = new Dictionary<string, string[]> ();

		m_unloadedBundles = new List<string> ();

		lstDownload = new List<AssetBundleData.ArrayEntry> ();
	}

	public void LoadDivisionData(string datas){
		if (divisionBundleData.Count > 0)
			return;

		DataMgr.Instance.ParseFile(datas, this.AddDivisionData);
	}

	public bool isDivisionBundle(string name){
		if (divisionBundleData.Count == 0)
			return false;

		string hash = ObscuredPrefs.GetString (name, "");

		if (!string.IsNullOrEmpty(hash))
			return false;

		for (int i = 0; i < divisionBundleData.Count; i++) {
			if (divisionBundleData [i].fileList.FindIndex (cs => cs == name) > -1)
				return true;
		}

		return false;
	}

	public bool CheckDivisionDownload(int pos, int id){
		if (!AssetMgr.Instance.useAssetBundle)
			return false;

		if (divisionBundleData.Count == 0)
			return false;

		List<DivisionData> divLst = new List<DivisionData> ();
		if (pos == 1 || pos == 7) {
			divLst = divisionBundleData.FindAll (cs => cs.pos == pos && cs.subID == id);
			bCanCancel = false;
		} else {
			if (id == 0)
			{
				divLst = divisionBundleData.FindAll (cs => cs.pos == pos);
			} else {
				divLst = divisionBundleData.FindAll (cs => cs.pos == pos && cs.subID <= id);
			}
			bCanCancel = true;
		}

		//DebugMgr.LogError (pos + ":" + id);

		if (divLst.Count == 0)
			return false;

		downloadSize = 0;
		decompressSize = 0;
		curSize = 0;        
		totalSize = 0;

		lstDownload = new List<AssetBundleData.ArrayEntry> ();

		for (int idx = 0; idx < divLst.Count; idx++) {
			for (int i = 0; i < divLst [idx].fileList.Count; i++) {
				string hash = ObscuredPrefs.GetString (divLst [idx].fileList [i], "");
				AssetBundleData.ArrayEntry asset = assetBundleData.bundleData.FirstOrDefault (cs => cs.bundleName == divLst [idx].fileList [i]);

				if (string.IsNullOrEmpty (hash) || hash.CompareTo (asset.hash) != 0) {
					if (lstDownload.FindIndex(cs => cs.bundleName == divLst [idx].fileList [i]) < 0) {
						lstDownload.Add (asset);
						downloadSize += asset.compressSize;
						decompressSize += asset.size;
					}
				}
			}
		}

		if (lstDownload.Count > 0) {
			PopupManager.Instance.SetDownloadPopup(string.Format("{0:F2}MB", downloadSize / MB_SIZE));
			return true;
		}

		return false;
	}

	List<int> downloadClassList = new List<int>();

	public void InitDownloadList(){
		lstDownload = new List<AssetBundleData.ArrayEntry> ();

		downloadSize = 0;
		decompressSize = 0;
		curSize = 0;        
		totalSize = 0;

		bCanCancel = false;
	}

	public void ShowDownLoadPopup(){
		if (lstDownload.Count > 0) {
			PopupManager.Instance.SetDownloadPopup (string.Format ("{0:F2}MB", downloadSize / MB_SIZE));

			downloadClassList.Clear ();
		}
	}

	public bool AddDivisionDownload(int pos, int id){
		if (!AssetMgr.Instance.useAssetBundle)
			return false;

		if (divisionBundleData.Count == 0)
			return false;

		if (pos == 1) {
			if (downloadClassList.Contains (id))
				return false;
		}

		List<DivisionData> divLst = divisionBundleData.FindAll (cs => cs.pos == pos && cs.subID == id);

		if (divLst.Count == 0)
			return false;

		for (int idx = 0; idx < divLst.Count; idx++) {
			for (int i = 0; i < divLst [idx].fileList.Count; i++) {
				string hash = ObscuredPrefs.GetString (divLst [idx].fileList [i], "");
				AssetBundleData.ArrayEntry asset = assetBundleData.bundleData.FirstOrDefault (cs => cs.bundleName == divLst [idx].fileList [i]);

				if (string.IsNullOrEmpty (hash) || hash.CompareTo (asset.hash) != 0) {
					if (lstDownload.FindIndex(cs => cs.bundleName == divLst [idx].fileList [i]) < 0) {
						lstDownload.Add (asset);
						downloadSize += asset.compressSize;
						decompressSize += asset.size;
					}
				}
			}
		}

		if (lstDownload.Count > 0) {
			if (pos == 1) downloadClassList.Add (id);
			return true;
		}

		return false;
	}

	public bool CheckDownloadedAll(){
		if (!AssetMgr.Instance.useAssetBundle)
			return false;

		if (divisionBundleData.Count == 0)
			return false;

		for (int idx = 0; idx < divisionBundleData.Count; idx++) {
			for (int i = 0; i < divisionBundleData [idx].fileList.Count; i++) {

				string hash = ObscuredPrefs.GetString (divisionBundleData [idx].fileList [i], "");
				AssetBundleData.ArrayEntry asset = assetBundleData.bundleData.FirstOrDefault (cs => cs.bundleName == divisionBundleData [idx].fileList [i]);

				if (string.IsNullOrEmpty (hash) || hash.CompareTo (asset.hash) != 0) {
					return true;
				}
			}
		}

		return false;
	}

	public bool CheckDownloadAll(){
		if (!AssetMgr.Instance.useAssetBundle)
			return false;

		if (divisionBundleData.Count == 0)
			return false;

		bCanCancel = true;

		downloadSize = 0;
		decompressSize = 0;
		curSize = 0;        
		totalSize = 0;

		lstDownload = new List<AssetBundleData.ArrayEntry> ();

		for (int idx = 0; idx < divisionBundleData.Count; idx++) {
			for (int i = 0; i < divisionBundleData [idx].fileList.Count; i++) {
				
				string hash = ObscuredPrefs.GetString (divisionBundleData [idx].fileList [i], "");
				AssetBundleData.ArrayEntry asset = assetBundleData.bundleData.FirstOrDefault (cs => cs.bundleName == divisionBundleData [idx].fileList [i]);

				if (string.IsNullOrEmpty (hash) || hash.CompareTo (asset.hash) != 0) {
					if (lstDownload.FindIndex(cs => cs.bundleName == divisionBundleData [idx].fileList [i]) < 0) {
						lstDownload.Add (asset);
						downloadSize += asset.compressSize;
						decompressSize += asset.size;
					}
				}
			}
		}

		if (lstDownload.Count > 0) {
			PopupManager.Instance.SetDownloadPopup(string.Format("{0:F2}MB", downloadSize / MB_SIZE));
			return true;
		}

		return false;
	}

	public void StartFileDownload(){
		StartCoroutine ("FileDownload");
	}

	public void StopDownload(){
		StopCoroutine ("FileDownload");
	}

	float gagueSize = 385;

	IEnumerator FileDownload(){
		if(lstDownload.Count > 0)
		{
			for(int i=0; i<lstDownload.Count; i++)
			{
				if (lstDownload.Count == 0)
					continue;

				WWW download = new WWW(Server.ServerMgr.Instance.fileServerUrl + lstDownload[i].bundleName + COMPRESS_EXT); 

//				if( string.IsNullOrEmpty(lstDownload [i].bundleName)){
//					DebugMgr.LogError (download.url);
//					continue;
//				}

				curSize = 0;

				while(!download.isDone)
				{
					if(!string.IsNullOrEmpty(download.error))
						break;

					curSize = (long)(lstDownload[i].compressSize * download.progress);                                    
					long tempTotal = totalSize + curSize;                    
					PopupManager.Instance.UpdateDownload(gagueSize * (float)(tempTotal / (double)downloadSize),
														((float)(tempTotal / (double)downloadSize)*100).ToString("0") + "%",
														string.Format("({0:F2}MB / {1:F2}MB)", tempTotal / MB_SIZE, downloadSize / MB_SIZE));
					yield return null;
				}

				if(!string.IsNullOrEmpty(download.error))
				{
					string title = TextManager.Instance.GetErrorText("mark_connect_fail", "", false);
					string msg = TextManager.Instance.GetErrorText("mark_load_file_info_fail", "", false);
					PopupManager.Instance.ShowOKPopup(title, msg, Retry);                    
					yield break;
				}

				curSize = download.bytesDownloaded;

				totalSize += curSize;

				PopupManager.Instance.UpdateDownload(gagueSize * (float)(totalSize / (double)downloadSize),
													((float)(totalSize / (double)downloadSize)*100).ToString("0") + "%",
					string.Format("({0:F2}MB / {1:F2}MB)", totalSize / MB_SIZE, downloadSize / MB_SIZE));

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

				//DebugMgr.LogError (lstDownload [i].bundleName+" End");

//				if (download.assetBundle != null)
//					download.assetBundle.Unload (false);
				download.Dispose();
				download = null;
			}
			PopupManager.Instance.UpdateDownload(gagueSize * 1f, "100%", "");
		}

		PopupManager.Instance.SetDownloadComplete();
	}

	public void Retry(object[] param)
	{
		StartCoroutine("FileDownload");
	}

	private void AddDivisionData(string[] cols)
	{
		if (cols == null) return;
		DivisionData divInfo = new DivisionData();
		divInfo.Set(cols);

		divisionBundleData.Add (divInfo);
	}

	public bool GetSceneLoadDone()
	{
		return asyncOperation.isDone;
	}

	public float GetSceneLoadProgress()
	{
		return asyncOperation.progress;
	}

	public static string GetPlatformName()
	{
		#if UNITY_EDITOR
		return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
		#else
		return GetPlatformForAssetBundles(Application.platform);
		#endif
	}

	#if UNITY_EDITOR
	private static string GetPlatformForAssetBundles(BuildTarget target)
	{
		switch(target)
		{
		case BuildTarget.Android:
			return "Android";
		case BuildTarget.iOS:
			return "iOS";
		case BuildTarget.WebGL:
			return "WebGL";
		case BuildTarget.WebPlayer:
			return "WebPlayer";
		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			return "Windows";
		case BuildTarget.StandaloneOSXIntel:
		case BuildTarget.StandaloneOSXIntel64:
		case BuildTarget.StandaloneOSXUniversal:
			return "OSX";
			// Add more build targets for your own.
			// If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
		default:
			return null;
		}
	}
	#endif

	private static string GetPlatformForAssetBundles(RuntimePlatform platform)
	{
		switch(platform)
		{
		case RuntimePlatform.Android:
			return "Android";
		case RuntimePlatform.IPhonePlayer:
			return "iOS";
		case RuntimePlatform.WebGLPlayer:
			return "WebGL";
		case RuntimePlatform.OSXWebPlayer:
		case RuntimePlatform.WindowsWebPlayer:
			return "WebPlayer";
		case RuntimePlatform.WindowsPlayer:
			return "Windows";
		case RuntimePlatform.OSXPlayer:
			return "OSX";
			// Add more build targets for your own.
			// If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
		default:
			return null;
		}
	}
    
    public void InitAssetBundle()
    {          
        if(assetBundleData == null)
            return;
        
        for (int i = 0; i < assetBundleData.bundleData.Length; i++)
        {
            if(assetBundleData.bundleData [i].dependanciesBundle.Length > 0){
                m_Dependencies.Add (assetBundleData.bundleData [i].bundleName, assetBundleData.bundleData [i].dependanciesBundle);
            }

            for (int j = 0; j < assetBundleData.bundleData [i].includeAsset.Length; j++)
            {
                string resourceNameInBundle = assetBundleData.bundleData [i].includeAsset [j];

                if (resourcesBundlePath.ContainsKey (resourceNameInBundle)) {
                    //DebugMgr.LogError (resourceNameInBundle + " : " + assetBundleData.bundleData [i].bundleName + " => " + assetBundleData.bundleData [i].includeAsset [j]);
                    continue;
                }

                resourcesBundlePath.Add (resourceNameInBundle.ToLower(), new string[2] {assetBundleData.bundleData [i].bundleName, assetBundleData.bundleData [i].includeAsset [j]});
            }
        }
        
        DebugMgr.Log("Init Bundle Info");
    }

	public void SceneLoad(string scene, bool additive = false)
	{
        if(scene == "LobbyScene")
        {
            SceneLoadAsync ("LobbyScene_background_01", false); 
            SceneLoadAsync ("LobbyScene", true); 
        }
        else
		    SceneLoadAsync (scene, additive); 
	}

	public void BattleSceneLoadAsync(string scene, bool additive = false)
	{
		//DebugMgr.Log ("Battle Scene Async - " + SceneManager.GetActiveScene().name + " to " + scene);
		StartCoroutine (CoSceneLoadAsync (scene, additive, false));
	}

	public void SceneLoadAsync(string scene, bool additive = false)
	{
		//DebugMgr.Log ("Change Scene Async - " + SceneManager.GetActiveScene().name + " to " + scene);
		StartCoroutine (CoSceneLoadAsync (scene, additive, true));
	}

	public IEnumerator CoSceneLoadAsync(string scene, bool additive = false, bool release = false)
	{
        string bundleName = "";
        string resourceFullPath = "";

		if(!additive) ObjMgr.Instance.SaveObjectAtThis ();

//		if(useAssetBundle)
//		{
//			if (release) {
//				yield return new WaitForSeconds (0.01f);
//				if (m_unloadedBundles.Count > 0) {
//					for (int a=0; a<m_unloadedBundles.Count; a++) {
//						if (m_LoadedAssetBundles [m_unloadedBundles[a]].m_AssetBundle == null) {
//							m_LoadedAssetBundles.Remove(m_unloadedBundles [a]);
//							continue;
//						}
//						Unload (m_unloadedBundles[a], false);
//					}
//				}
//				
//				yield return new WaitForSeconds (0.01f);
//			}
//		}
//
//        if(useAssetBundle)
//        {	
//            string bundlePath = "Assets/Scenes/" + scene + ".unity";
//            bundlePath = bundlePath.ToLower();
//            
//            if(!resourcesBundlePath.ContainsKey(bundlePath))
//            {
//                DebugMgr.LogError("key not found : " + bundlePath);               
//                yield break;
//            }            
//            
//            bundleName = resourcesBundlePath [bundlePath] [0];
//            resourceFullPath = resourcesBundlePath [bundlePath] [1];            
//
//            LoadDependencies (bundleName);
//
//			if (m_LoadedAssetBundles.ContainsKey (bundleName)) {
////				m_LoadedAssetBundles [bundleName].m_ReferencedCount++;
////				m_LoadedAssetBundles [bundleName].m_AssetBundle.LoadAsset (resourceFullPath);
////				DebugMgr.Log ("Count Scene " + bundleName + " = " +m_LoadedAssetBundles [bundleName].m_ReferencedCount);
//			} else {                        
//				LoadedAssetBundle bundle = new LoadedAssetBundle (AssetBundle.LoadFromFile (Application.persistentDataPath + "/" + bundleName));
//				m_LoadedAssetBundles.Add (bundleName, bundle);
////				DebugMgr.Log ("Add Scene " + bundleName);
//			}
//        }

		bool tip = false;

		if (beforeScene == "LobbyScene_background_01" && scene != "LobbyScene_background_01") {
			tip = true;
		}else if (beforeScene != "LobbyScene_background_01" && scene == "LobbyScene_background_01") {
			tip = true;
		}

		if (scene == "Battle")
			tip = false;

		if(!additive) beforeScene = scene;
        
        PopupManager.Instance.ClearPopup();
        PopupManager.Instance.showLoading = true;

		if (tip) FadeEffectMgr.Instance.SetTip ();

		SceneManager.sceneLoaded += SceneLoadFinished;

		if (!additive) {
			asyncOperation = SceneManager.LoadSceneAsync (scene);
			PopupManager.Instance.AddUserStep ("Scene " + scene);
		}else
			asyncOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

		while (!asyncOperation.isDone) {

//			DebugMgr.Log (asyncOperation.progress);

			yield return null;
		}

		if (release && !additive) AtlasMgr.Instance.UnloadAll ();
//		System.GC.Collect();
        
        PopupManager.Instance.showLoading = false;    
		if (tip) FadeEffectMgr.Instance.DisableTip ();

//		DebugMgr.LogError (scene);
//		Resources.UnloadUnusedAssets ();

//		if(additive) Unload("gc_"+beforeScene.ToLower());
//		SceneManager.UnloadScene(scene);
//		if (release) {
//			AtlasMgr.Instance.UnloadAll ();
////			System.GC.Collect ();
//		}
//		if (useAssetBundle) Unload(bundleName, false);


//		System.GC.Collect();

//		if (beforeScene == "TitleScene" || beforeScene == "Splash") {
//			////			PopupManager.Instance.RemoveManager ();
//			////			PopupManager inst = PopupManager.Instance;
//			////			Destroy (inst);
//			SceneManager.UnloadScene (beforeScene);
//		}

//		UnloadAssetBundle (bundleName);

		//for Event Sysytem Bug Fix
		#if UNITY_EDITOR
		if (FindObjectOfType<UnityEngine.EventSystems.StandaloneInputModule> () != null) {
			UnityEngine.EventSystems.StandaloneInputModule temp = FindObjectOfType<UnityEngine.EventSystems.StandaloneInputModule> ();
			temp.forceModuleActive = true;
		}
		#endif
	}

	void SceneLoadFinished(UnityEngine.SceneManagement.Scene obj1, LoadSceneMode obj2){
		LTGUI.reset();
		if (GameObject.Find ("Google Analytics") != null) {
			GameObject.Find ("Google Analytics").GetComponent<GoogleAnalytics> ().OnLevelLoaded (obj1.buildIndex);
		}

		SceneManager.sceneLoaded -= SceneLoadFinished;
	}

	public UnityEngine.Object AssetLoad(string path, System.Type type, bool bAssetBundle = false)
	{
		if (bAssetBundle) {
#if UNITY_EDITOR            
			UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(bundleFolder + path, type);
			
			if(obj == null)
				DebugMgr.LogError("Resource Load Fail : " + bundleFolder + path);
			
			return obj;
#else 
			string bundlePath = bundleFolder + path;
			bundlePath = bundlePath.ToLower();
			
			if(!resourcesBundlePath.ContainsKey(bundlePath))
			{
			    DebugMgr.LogError("key not found : " + bundlePath);               
			    return null;
			}
			
			string bundleName = resourcesBundlePath [bundlePath] [0];
			string resourceFullPath = resourcesBundlePath [bundlePath] [1];            
			
			LoadDependencies (bundleName);
			
			if (m_LoadedAssetBundles.ContainsKey (bundleName)) {
				m_LoadedAssetBundles [bundleName].m_ReferencedCount++;
			
				return m_LoadedAssetBundles [bundleName].m_AssetBundle.LoadAsset (resourceFullPath, type);
			} else {                            
				LoadedAssetBundle bundle = new LoadedAssetBundle (AssetBundle.LoadFromFile (Application.persistentDataPath + "/" + bundleName));
				m_LoadedAssetBundles.Add (bundleName, bundle);
				AddUnloadAsset(bundleName);
			
				return bundle.m_AssetBundle.LoadAsset (resourceFullPath, type);
			}
#endif
		}else{
			string[] Fullpath = path.Split ('.');
			string resultPath = "";
			if (Fullpath.Length > 2) {
				for (int i = 0; i < Fullpath.Length - 1; i++) {
					resultPath += Fullpath [i];
				}
			} else {
				resultPath = Fullpath [0];
			}
			
			UnityEngine.Object obj = Resources.Load (resultPath, type);

			if (obj == null)
				DebugMgr.LogError ("Resource Load Fail : " + resourceFolder + resultPath);

			return obj;
		}


		return null;

//        if(!useAssetBundle)
//        {
//#if UNITY_EDITOR            
//			UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(resourceFolder + path, type);
//
//            if(obj == null)
//                DebugMgr.LogError("Resource Load Fail : " + resourceFolder + path);
//
//            return obj;
//#endif            
//        }        
//        else
//        {
//            string bundlePath = bundleFolder + path;
//            bundlePath = bundlePath.ToLower();
//            
//            if(!resourcesBundlePath.ContainsKey(bundlePath))
//            {
//                DebugMgr.LogError("key not found : " + bundlePath);               
//                return null;
//            }
//            
//            string bundleName = resourcesBundlePath [bundlePath] [0];
//            string resourceFullPath = resourcesBundlePath [bundlePath] [1];            
//
////            DebugMgr.LogWarning (bundleName + "/" + resourceFullPath);
//
//            LoadDependencies (bundleName);
//
//			if (m_LoadedAssetBundles.ContainsKey (bundleName)) {
//				m_LoadedAssetBundles [bundleName].m_ReferencedCount++;
//
////				if(bUnloadAsset) UnloadAssetBundle (bundleName);
//
////				DebugMgr.Log ("Count Asset " + bundleName + " = " +m_LoadedAssetBundles [bundleName].m_ReferencedCount);
//
//				return m_LoadedAssetBundles [bundleName].m_AssetBundle.LoadAsset (resourceFullPath, type);
//			} else {                            
//				LoadedAssetBundle bundle = new LoadedAssetBundle (AssetBundle.LoadFromFile (Application.persistentDataPath + "/" + bundleName));
//				m_LoadedAssetBundles.Add (bundleName, bundle);
//				AddUnloadAsset(bundleName);
//
////				if(bUnloadAsset) UnloadAssetBundle (bundleName);
//
////				DebugMgr.Log ("Add Asset " + bundleName);
//			
//           		return bundle.m_AssetBundle.LoadAsset (resourceFullPath, type);
//			}
//        }
//        
//        return null;        
	}

	void LoadDependencies(string bundleName){
		if (!m_Dependencies.ContainsKey (bundleName))
			return;

		for (int i = 0; i < m_Dependencies [bundleName].Length; i++) {
			string dependenciName = m_Dependencies [bundleName] [i];

			if (m_LoadedAssetBundles.ContainsKey (dependenciName)) {
				m_LoadedAssetBundles [dependenciName].m_ReferencedCount++;

//				DebugMgr.LogWarning ("Dp "+dependenciName+" Cnt " + m_LoadedAssetBundles [dependenciName].m_ReferencedCount);
			} else {
				LoadedAssetBundle bundle = new LoadedAssetBundle (AssetBundle.LoadFromFile (Application.persistentDataPath + "/" + dependenciName));
				m_LoadedAssetBundles.Add (dependenciName, bundle);
				AddUnloadAsset (dependenciName);
//				DebugMgr.LogWarning ("Add Dp " + dependenciName);
			}
		}
	}

	void AddUnloadAsset(string name){
		if(!m_unloadedBundles.Contains(name))
			m_unloadedBundles.Add(name);
	}

	public void UnloadAssetBundleWithFilePath(string path){
		string bundlePath = bundleFolder + path;
		bundlePath = bundlePath.ToLower();

		if(!resourcesBundlePath.ContainsKey(bundlePath))
		{
			DebugMgr.LogError("key not found : " + bundlePath);               
			return;
		}

		string bundleName = resourcesBundlePath [bundlePath] [0];

		Unload (bundleName, false);
	}

	public void UnloadAssetBundle(string assetBundleName){        
		if (!useAssetBundle)
			return;
		
		StartCoroutine (UnloadNextFrame (assetBundleName));
	}

	IEnumerator UnloadNextFrame(string assetBundleName)
	{
		yield return new WaitForEndOfFrame();
		Unload (assetBundleName, false);
	}

	public void ForceUnloadAll(){
		List<string> curList = m_LoadedAssetBundles.Keys.ToList ();
		for(int i=0; i<curList.Count; i++) {
			Unload (curList[i], true);
		}
	}

	// Unload assetbundle and its dependencies.
	public void Unload(string assetBundleName, bool bForce)
	{
		if (!useAssetBundle)
			return;
       		
		UnloadAssetBundleInternal(assetBundleName, bForce);
		UnloadDependencies(assetBundleName, bForce);
	}

	void UnloadDependencies(string assetBundleName, bool bForce)
	{
		string[] dependencies = null;
		if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
			return;

		// Loop dependencies.
		foreach(var dependency in dependencies)
		{
			UnloadAssetBundleInternal(dependency, bForce);
		}
	}

	void UnloadAssetBundleInternal(string assetBundleName, bool bForce)
	{
		string error;
		if (!m_LoadedAssetBundles.ContainsKey(assetBundleName))
			return;

//		if(assetBundleName == "gc_fonts" ||
//			assetBundleName == "gc_shader" ||
//			assetBundleName == "gc_sprites_common") 
//			return;
//
//		if (!bForce) {
//			if (assetBundleName == "gc_prefabs_effects_common" ||
//				assetBundleName == "gc_prefabs_models_missile")
//				return;
//
//			if (assetBundleName.Length > 23) {
//				string front_name = assetBundleName.Remove (23);
//				if (front_name == "gc_prefabs_effects_hero") {
//					return;
//				}
//			}
//
//			if (assetBundleName.Length > 15) {
//				string front_name = assetBundleName.Remove (15);
//				if (front_name == "gc_sprites_item" ||
//					front_name == "gc_sprites_ques") {
//					return;
//				}
//			}
//		}
		
		LoadedAssetBundle bundle = m_LoadedAssetBundles [assetBundleName];

		if (bForce == true) {
//			DebugMgr.LogWarning ("ForceUnloaded " + assetBundleName);
			bundle.m_AssetBundle.Unload (false);

			m_LoadedAssetBundles.Remove (assetBundleName);
			if (m_unloadedBundles.Contains (assetBundleName)) {
				m_unloadedBundles.Remove (assetBundleName);
			}
		} else {
			if (--bundle.m_ReferencedCount == 0) {
				//DebugMgr.LogWarning ("Unloaded " + assetBundleName);
				bundle.m_AssetBundle.Unload (false);
				m_LoadedAssetBundles.Remove (assetBundleName);
				if (m_unloadedBundles.Contains (assetBundleName)) {
					m_unloadedBundles.Remove (assetBundleName);
				}
			} else {
//				DebugMgr.Log ("RefCnt " + assetBundleName +" = "+bundle.m_ReferencedCount);
			}
		}
	}

	public UnityEngine.Object[] AssetLoadAll(string path, System.Type type)
	{
		string[] Fullpath = path.Split ('.');

		UnityEngine.Object[] objs = Resources.LoadAll (Fullpath [0], type);

		if (objs == null)
			DebugMgr.LogError ("Resource Load Fail : " + resourceFolder + Fullpath [0]);

		return objs;
//        if(!useAssetBundle)
//        {
//#if UNITY_EDITOR            
//			UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(resourceFolder + path);            
//
//            if(objs == null)
//                DebugMgr.LogError("Resource Load All Fail : " + resourceFolder + path);
//                
//            if(objs.Length == 0)
//            {
//                string[] files = System.IO.Directory.GetFiles(Application.dataPath + "/Resources_Bundle/" + path).Where(x => !x.EndsWith(".meta")).ToArray();
//                
//				objs = new UnityEngine.Object[files.Length];
//                
//                for(int i=0; i<files.Length; i++)
//                {
////                    DebugMgr.Log("Assets" + files[i].Replace(Application.dataPath, ""));
//                    
//                    objs[i] = AssetDatabase.LoadAssetAtPath("Assets" + files[i].Replace(Application.dataPath, ""), type);
////                    DebugMgr.Log(objs[i]);
//                }
//            }
//                
//            return objs;
//#endif           
//        }        
//        else
//        {
//            string bundlePath = bundleFolder + path;
//            bundlePath = bundlePath.ToLower();
//            
//            if(!resourcesBundlePath.ContainsKey(bundlePath))
//            {
//                DebugMgr.LogError("key not found : " + bundlePath);               
//                return null;
//            }
//            
//            string bundleName = resourcesBundlePath [bundlePath] [0];
//            string resourceFullPath = resourcesBundlePath [bundlePath] [1];                        
//            
////            DebugMgr.LogWarning (bundleName);
//
//            LoadDependencies (bundleName);
//
//			if (m_LoadedAssetBundles.ContainsKey (bundleName)) {
//				m_LoadedAssetBundles [bundleName].m_ReferencedCount++;
//
////				if(bUnloadAsset) UnloadAssetBundle (bundleName);
//
////				DebugMgr.Log ("Count AssetAll " + bundleName + " = " +m_LoadedAssetBundles [bundleName].m_ReferencedCount);
//
//				UnityEngine.Object[] objs = m_LoadedAssetBundles [bundleName].m_AssetBundle.LoadAssetWithSubAssets(resourceFullPath.ToLower());
//
//               // DebugMgr.Log(resourceFullPath.ToLower());
//                //DebugMgr.Log(objs.Length);
//                
//                return objs;
//                
//			} else {                        
//				LoadedAssetBundle bundle = new LoadedAssetBundle (AssetBundle.LoadFromFile (Application.persistentDataPath + "/" + bundleName));
//				m_LoadedAssetBundles.Add (bundleName, bundle);
//				AddUnloadAsset (bundleName);
//
////				if(bUnloadAsset) UnloadAssetBundle (bundleName);
//                
//				UnityEngine.Object[] objs = m_LoadedAssetBundles [bundleName].m_AssetBundle.LoadAssetWithSubAssets(resourceFullPath.ToLower());
//
//                return objs;
//			}
//        }      
//        
//        return null;
	}    
    
	public IEnumerator AssetLoadAsync(System.Action<UnityEngine.Object> onCallback, string path, System.Type type)
	{
        if(!useAssetBundle)
        {
#if UNITY_EDITOR            
			UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(resourceFolder + path, type);

            if(obj == null)
                DebugMgr.LogError("Resource Load Fail : " + resourceFolder + path);

            if (onCallback != null) {
                onCallback(obj);
            }
            yield return null;
#endif
        }
        else             
        {
            string bundlePath = bundleFolder + path;
            bundlePath = bundlePath.ToLower();
            
            if(!resourcesBundlePath.ContainsKey(bundlePath))
            {
                DebugMgr.LogError("key not found : " + bundlePath);               
                yield break;
            }
            
            string bundleName = resourcesBundlePath [bundlePath] [0];
            string resourceFullPath = resourcesBundlePath [bundlePath] [1];

            LoadDependencies (bundleName);

//            DebugMgr.LogWarning (bundleName + "/" + resourceFullPath);
            
			if (m_LoadedAssetBundles.ContainsKey (bundleName)) {
				m_LoadedAssetBundles [bundleName].m_ReferencedCount++;
				AssetBundleRequest request = m_LoadedAssetBundles [bundleName].m_AssetBundle.LoadAssetAsync (resourceFullPath);

//				UnloadAssetBundle (bundleName);

				yield return request;

//				if(bUnloadAsset) UnloadAssetBundle (bundleName);
                
				if (onCallback != null) {
					onCallback (request.asset);
					yield break;
				}
			} else {
                
				LoadedAssetBundle bundle = new LoadedAssetBundle (AssetBundle.LoadFromFile (Application.persistentDataPath + "/" + bundleName));
				m_LoadedAssetBundles.Add (bundleName, bundle);
				AddUnloadAsset(bundleName);
				AssetBundleRequest request2 = bundle.m_AssetBundle.LoadAssetAsync (resourceFullPath);
            
//				UnloadAssetBundle (bundleName);

				yield return request2;

//				if(bUnloadAsset) UnloadAssetBundle (bundleName);

				if (onCallback != null) {
					onCallback (request2.asset);
				}
			}
        }
	}
}
