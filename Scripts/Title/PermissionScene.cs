using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;
using UnityEngine.Networking;

public class PermissionScene : MonoBehaviour {

	public GameObject progressObject;
	public RectTransform totalProgressBar;
	public Text currentSizeText;
	public Text currentPerText;

	const double MB_SIZE = 1048576;

	bool per1 = false;
	bool per2 = false;
	bool bWait = false;

	bool bKorean = false;

	void Start () {
		if (Application.systemLanguage == SystemLanguage.Korean)
			bKorean = true;

		#if UNITY_ONESTORE && !UNITY_EDITOR
		AndroidJavaClass ajc = new AndroidJavaClass("android.os.Build$VERSION");
		int api_level = ajc.GetStatic<int>("SDK_INT");
		if (api_level > 20) {
			StartCoroutine("CheckPermission");
		}else{
			DownloadObbFile();
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass ajc = new AndroidJavaClass("android.os.Build$VERSION");
		int api_level = ajc.GetStatic<int>("SDK_INT");
		if (api_level > 20) {
			StartCoroutine("CheckPermission");
		}else{
			UnityEngine.SceneManagement.SceneManager.LoadScene("Splash");
		}
		#else
		UnityEngine.SceneManagement.SceneManager.LoadScene("Splash");
		#endif
	}
	
	IEnumerator CheckPermission()
	{
		string title = "";
		string desc = "";

		per1 = PermissionsManager.IsPermissionGranted (AN_ManifestPermission.GET_ACCOUNTS);

		if (!per1) {
			bWait = true;

			if (bKorean) {
				title = "권한 허용";
				desc = "계정 로그인을 위해 [주소록] 권한 승인이 필요합니다.\n해당 권한을 승인하지 않을 시\n원활한 게임 서비스 이용이 어려울 수도 있습니다.";
			} else {
				title = "App Permission";
				desc = "For Log-in 'Contatcs' permission is required.\nIf you don't approve this permission,\nyou may not play game.";
			}
			PermissionsManager.ActionPermissionsRequestCompleted += PermissionResult;
			AndroidMessage msg = AndroidMessage.Create(title, desc);
			msg.ActionComplete += RequestAccountPermission;
		} else {
			bWait = false;
		}

		while (bWait) {
			yield return new WaitForSeconds (0.1f);
		}

		if (!per1) {
			if (bKorean) {
				title = "권한 허용";
				desc = "앱 재실행 후 권한 허용에\n동의해주시기 바랍니다.";
			} else {
				title = "App Permission";
				desc = "Reload the app and\nagree to the permissions.";
			}
			AndroidMessage msg = AndroidMessage.Create(title, desc);
			msg.ActionComplete += AppQuit;
			yield break;
		}

		per2 = PermissionsManager.IsPermissionGranted (AN_ManifestPermission.READ_EXTERNAL_STORAGE);

		if (!per2) {
			bWait = true;

			if (bKorean) {
				title = "권한 허용";
				desc = "데이터의 저장을 위해 [기기 사진, 미디어, 파일]\n권한 승인이 필요합니다.\n해당 권한을 승인하지 않을 시\n원활한 게임 서비스 이용이 어려울 수도 있습니다.";
			} else {
				title = "App Permission";
				desc = "For saving the data  'Photos/Media/Files'\npermission is required.\nIf you don't approve this permission,\nyou may not play game.";
			}
			PermissionsManager.ActionPermissionsRequestCompleted += PermissionResult2;
			AndroidMessage msg = AndroidMessage.Create(title, desc);
			msg.ActionComplete += RequestStoragePermission;
		} else {
			bWait = false;
		}

		while (bWait) {
			yield return new WaitForSeconds (0.1f);
		}

		if (!per2) {
			if (bKorean) {
				title = "권한 허용";
				desc = "권한을 허용하지 않으면\n데이터 저장이 정상적으로 이루어지지 않습니다.";
			} else {
				title = "App Permission";
				desc = "If you don't grant permission,\nthe data will not be saved normally.";
			}
			AndroidMessage msg = AndroidMessage.Create(title, desc);
			msg.ActionComplete += AppQuit;
			yield break;
		}

		#if UNITY_ONESTORE
		DownloadObbFile();
		#else
		UnityEngine.SceneManagement.SceneManager.LoadScene("Splash");
		#endif
	}

	void PermissionResult(AN_GrantPermissionsResult result){
		bWait = false;
		if (result.RequestedPermissionsState [AN_ManifestPermission.GET_ACCOUNTS] == AN_PermissionState.PERMISSION_GRANTED)
			per1 = true;

		PermissionsManager.ActionPermissionsRequestCompleted -= PermissionResult;
	}

	void PermissionResult2(AN_GrantPermissionsResult result){
		bWait = false;
		if (result.RequestedPermissionsState [AN_ManifestPermission.READ_EXTERNAL_STORAGE] == AN_PermissionState.PERMISSION_GRANTED)
			per2 = true;

		PermissionsManager.ActionPermissionsRequestCompleted -= PermissionResult2;
	}

	void RequestAccountPermission(AndroidDialogResult param){
		PermissionsManager.Instance.RequestPermissions (AN_ManifestPermission.GET_ACCOUNTS);
	}

	void RequestStoragePermission(AndroidDialogResult param){
		PermissionsManager.Instance.RequestPermissions (AN_ManifestPermission.READ_EXTERNAL_STORAGE, AN_ManifestPermission.WRITE_EXTERNAL_STORAGE);
	}

	void AppQuit(AndroidDialogResult param){
		Application.Quit ();
	}


	string downloadUrl = "https://gcrewpublish.blob.core.windows.net/gcrewpatch/";
	string expansionFilePath = "";
	string obbFileName = "";

	private static AndroidJavaClass detectAndroidJNI;
	public static bool RunningOnAndroid()
	{
		if (detectAndroidJNI == null)
			detectAndroidJNI = new AndroidJavaClass("android.os.Build");
		return detectAndroidJNI.GetRawClass() != IntPtr.Zero;
	}

	private static AndroidJavaClass Environment;
	private const string Environment_MEDIA_MOUNTED = "mounted";

	void DownloadObbFile () {
		#if UNITY_EDITOR
		expansionFilePath = Application.persistentDataPath+"/Android/obb/com.lunosoft.guardiansone";
		obb_version = 19;
		obb_package = "com.lunosoft.guardiansone";
		obbFileName = "main."+obb_version+"."+obb_package+".obb";
		#else
		Environment = new AndroidJavaClass("android.os.Environment");

		expansionFilePath = GetExpansionFilePath ();
		obbFileName = GetObbFileName();
		#endif

		int savedVer = PlayerPrefs.GetInt ("SavedObbVer", 0);

		if (obb_version == savedVer) {
			Debug.LogError ("already downloaded "+savedVer);
			if (File.Exists (expansionFilePath + "/" + "main." + savedVer + "." + obb_package + ".obb")) {
				UnityEngine.SceneManagement.SceneManager.LoadScene ("Splash");
				return;
			} else {
				PlayerPrefs.SetInt ("SavedObbVer", 0);
			}
		} else if (savedVer > 0) {
			if (File.Exists (expansionFilePath + "/" + "main." + savedVer + "." + obb_package + ".obb")) {
				File.Delete (expansionFilePath + "/" + "main." + savedVer + "." + obb_package + ".obb");
			}
		}

		progressObject.SetActive (true);

		if (!Directory.Exists (expansionFilePath)) {
			Debug.LogError ("expansionFilePath is null");
			Directory.CreateDirectory (expansionFilePath);
		}

		AndroidMessage msg = AndroidMessage.Create("추가파일 다운로드", "실행에 필요한 파일을 다운로드 받습니다.\n(WIFI 환경 권장)");
		msg.ActionComplete += StartDownload;

	}

	void StartDownload(AndroidDialogResult param)
	{
		StartCoroutine("FileDownload");
	}



	IEnumerator FileDownload()
	{
		#if live
		WWW obbUrl = new WWW(downloadUrl + "obb_url_live.txt");
		#elif qa
		WWW obbUrl = new WWW(downloadUrl + "obb_url_qa.txt");
		#else
		WWW obbUrl = new WWW(downloadUrl + "obb_url_dev.txt");
		#endif

		while (!obbUrl.isDone)
		{
			yield return null;
		}

		string fileUrl = obbUrl.text.Trim ();

		long downloadedSize = 0;
		long fileSize = 0;
		double alreadyPer = 0;
		double remainPer = 100;
		WWW sizeCheck = new WWW(fileUrl + "size"+obb_version+".txt");

		while (!sizeCheck.isDone)
		{
			yield return null;
		}

		fileSize = int.Parse (sizeCheck.text);

		UnityWebRequest www = UnityWebRequest.Get(fileUrl + obbFileName);
		www.SetRequestHeader("Content-Type", "application/octet-stream");
		//www.SetRequestHeader("Content-Length", "64");

		FileMode fType = FileMode.Create;

		if (File.Exists (expansionFilePath + "/" + obbFileName)) {
			FileInfo curFile = new FileInfo (expansionFilePath + "/" + obbFileName);
			if (fileSize != curFile.Length) {
				downloadedSize = curFile.Length;
				alreadyPer = Math.Floor(((double)downloadedSize/(double)fileSize)*100f);
				remainPer = 100f - alreadyPer;
				fType = FileMode.Append;
				www.SetRequestHeader ("Range", "bytes=" + curFile.Length + "-");
			} else {
				PlayerPrefs.SetInt ("SavedObbVer", obb_version);
				UnityEngine.SceneManagement.SceneManager.LoadScene ("Splash");
				yield break;
			}
		}

		www.downloadHandler = new ToFileDownloadHandler(new byte[64 * 1024], expansionFilePath + "/" + obbFileName, fType);
		www.Send();

		float gagueSize = 1185;

		while (!www.isDone)
		{
			double progress = alreadyPer + remainPer * www.downloadProgress;
			if (progress > 100f)
				progress = 100f;
			
			totalProgressBar.sizeDelta = new Vector2(gagueSize * (float)(progress/100f), totalProgressBar.sizeDelta.y);
			currentPerText.text = (progress).ToString("0") + "%";
			currentSizeText.text = string.Format("({0:F2}MB / {1:F2}MB)", ((ulong)downloadedSize + www.downloadedBytes) / MB_SIZE, fileSize / MB_SIZE);
			yield return null;
		}

		if (www.isError)
		{
			Debug.Log(www.error);
			if (www.error == "Unknown Error") {
				((ToFileDownloadHandler)www.downloadHandler).Pause ();
				StopCoroutine ("FileDownload");
				StartCoroutine ("FileDownload");
				yield break;
			}
		}
		else
		{
			Debug.Log("Complete");
			PlayerPrefs.SetInt ("SavedObbVer", obb_version);

			AndroidMessage msg = AndroidMessage.Create("설치 완료", "원활한 게임 실행을 위해\n앱을 재시작 해주시기 바랍니다.");
			msg.ActionComplete += AppQuit;
		}
	}


	string GetObbFileName()
	{
		string main = String.Format("main.{0}.{1}.obb", obb_version, obb_package);

		return main;
	}


	public static string GetExpansionFilePath()
	{
		populateOBBData();

		if (Environment.CallStatic<string>("getExternalStorageState") != Environment_MEDIA_MOUNTED)
			return null;

		const string obbPath = "Android/obb";

		using (AndroidJavaObject externalStorageDirectory = Environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
		{
			string root = externalStorageDirectory.Call<string>("getPath");
			return String.Format("{0}/{1}/{2}", root, obbPath, obb_package);
		}
	}

	private static string obb_package;
	private static int obb_version = 0;
	private static void populateOBBData()
	{
		if (obb_version != 0)
			return;
		using (AndroidJavaClass unity_player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject current_activity = unity_player.GetStatic<AndroidJavaObject>("currentActivity");
			obb_package = current_activity.Call<string>("getPackageName");
			AndroidJavaObject package_info = current_activity.Call<AndroidJavaObject>("getPackageManager").Call<AndroidJavaObject>("getPackageInfo", obb_package, 0);
			obb_version = package_info.Get<int>("versionCode");
		}
	}
}
	

public class ToFileDownloadHandler : DownloadHandlerScript
{
	private int expected = -1;
	private int received = 0;
	private string filepath;
	private FileStream fileStream;
	private bool canceled = false;

	public ToFileDownloadHandler(byte[] buffer, string filepath, FileMode faType)
		: base(buffer)
	{
		this.filepath = filepath;
		fileStream = new FileStream(filepath, faType);
	}

	protected override byte[] GetData() { return null; }

	protected override bool ReceiveData(byte[] data, int dataLength)
	{
		if (data == null || data.Length < 1)
		{
			return false;
		}
		//Debug.LogError (data.Length + "/" + dataLength);

		received += dataLength;
		if (!canceled) fileStream.Write(data, 0, dataLength);
		return true;
	}

	protected override float GetProgress()
	{
		if (expected < 0) return 0;
		return (float)received / expected;
	}

	protected override void CompleteContent()
	{
		fileStream.Close();
	}

	protected override void ReceiveContentLength(int contentLength)
	{
		expected = contentLength;
	}

	public void Pause()
	{
		fileStream.Flush();
		fileStream.Close();
	}

	public void Cancel()
	{
		canceled = true;
		fileStream.Close();
		File.Delete(filepath);
	}
}
