using UnityEngine;
using System.IO;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;

public class DataMgr : Singleton<DataMgr>
{
	readonly public bool bDevelopment = false;
	readonly private bool bLoadByWeb = false;
	readonly private string serverAddress = "http://216.177.209.68/"; // 테스트 서버
	private float timeout = 20f;

	private bool isReLogin = false;
		
	public delegate void ReceiveInfo(string[] cols);
	public delegate void EndLoading();

	public enum LOAD_STEP
	{
		NONE, INFO, USER_DATA, RELOGIN
	}

	private LOAD_STEP loadStep = LOAD_STEP.NONE;
	public LOAD_STEP LoadStep
	{
		get { return loadStep; }
	}

	public void Init()
	{
		loadStep = LOAD_STEP.NONE;
	}

	public void SetLoadStep(LOAD_STEP step)
	{
		loadStep = step;
	}
	
	public void LoadData(EndLoading loadedAsync, EndLoading end_method)
	{

		#if __SERVER
		#else
		if (bLoadByWeb)
		{
			StartCoroutine(LoadSuccess(end_method));
		}
		else
		#endif
		{
			TextManager.Instance.Init(); loadedAsync();
			LegionInfoMgr.Instance.Init(); loadedAsync();
			SocketInfoMgr.Instance.Init(); loadedAsync();
			ItemInfoMgr.Instance.Init(); loadedAsync();
			ModelInfoMgr.Instance.Init(); loadedAsync();
			EquipmentInfoMgr.Instance.Init(); loadedAsync();
			ConditionInfoMgr.Instance.Init(); loadedAsync();
			ClassInfoMgr.Instance.Init(); loadedAsync();
			SkillInfoMgr.Instance.Init(); loadedAsync();
			StageInfoMgr.Instance.Init(); loadedAsync();
			AtlasMgr.Instance.Init(); loadedAsync();
			ShopInfoMgr.Instance.Init(); loadedAsync();
			LeagueInfoMgr.Instance.Init(); loadedAsync();
			ForgeInfoMgr.Instance.Init(); loadedAsync();
            QuestInfoMgr.Instance.Init(); loadedAsync();
			TutorialInfoMgr.Instance.Init(); loadedAsync();
            SocialInfo.Instance.Init(); loadedAsync();
            RankInfoMgr.Instance.Init(); loadedAsync();
            EventInfoMgr.Instance.Init(); loadedAsync();
            GuildInfoMgr.Instance.InitGuildData(); loadedAsync();
			loadStep = LOAD_STEP.INFO;
			end_method();
		}
	}
	
	IEnumerator LoadSuccess( EndLoading end_method)
	{
		LegionInfoMgr.Instance.Init();
		while (!LegionInfoMgr.Instance.LoadedInfo) { yield return null; }
		SocketInfoMgr.Instance.Init();
		while (!SocketInfoMgr.Instance.LoadedInfo) { yield return null; }
		ModelInfoMgr.Instance.Init();
		while (!ModelInfoMgr.Instance.LoadedInfo) { yield return null; }
		ConditionInfoMgr.Instance.Init();
		while (!ConditionInfoMgr.Instance.LoadedInfo) { yield return null; }
		ClassInfoMgr.Instance.Init();
		while (!ClassInfoMgr.Instance.LoadedInfo) { yield return null; }
		SkillInfoMgr.Instance.Init();
		while (!SkillInfoMgr.Instance.LoadedInfo) { yield return null; }
		EquipmentInfoMgr.Instance.Init();
		while (!EquipmentInfoMgr.Instance.LoadedInfo) { yield return null; }
		ItemInfoMgr.Instance.Init();
		while (!ItemInfoMgr.Instance.LoadedInfo) { yield return null; }
		StageInfoMgr.Instance.Init();
		while (!StageInfoMgr.Instance.LoadedInfo) { yield return null; }
		TextManager.Instance.Init();
		while(!TextManager.Instance.LoadedInfo) {yield return null;}
		ShopInfoMgr.Instance.Init();
		while(!ShopInfoMgr.Instance.LoadedInfo) {yield return null;}
        LeagueInfoMgr.Instance.Init();
        while(!LeagueInfoMgr.Instance.LoadedInfo) {yield return null; }
		ForgeInfoMgr.Instance.Init();
		while(!ForgeInfoMgr.Instance.LoadedInfo) {yield return null;}
        QuestInfoMgr.Instance.Init();
        while(!QuestInfoMgr.Instance.LoadedInfo) {yield return null;}
		TutorialInfoMgr.Instance.Init();
		while(!TutorialInfoMgr.Instance.LoadedInfo) {yield return null;}
        SocialInfo.Instance.Init();
        while(!SocialInfo.Instance.LoadedInfo) {yield return null;}
        RankInfoMgr.Instance.Init();
        while(!RankInfoMgr.Instance.LoadedInfo) {yield return null;}
        EventInfoMgr.Instance.Init();
        while(!EventInfoMgr.Instance.LoadedInfo) {yield return null;}
        GuildInfoMgr.Instance.InitGuildData();
        while(!GuildInfoMgr.Instance.LoadedInfo) {yield return null;}
		loadStep = LOAD_STEP.INFO;
		end_method();
	}
	
	public void LoadTable(ReceiveInfo set_method, string filename)
	{
//		DebugMgr.Log("----- LoadTable PATH");
		#if __SERVER
		string strPath = Application.dataPath + "/TextScripts/" + filename + ".txt";
		FileStream fs;
		fs = new FileStream(strPath, FileMode.Open);
		StreamReader streamR = new StreamReader(fs);
		DebugMgr.Log("----- FileStream End" + strPath);
		ParseFile(streamR.ReadToEnd(), set_method);
		streamR.Close();
		#else
		if (bLoadByWeb)
		{
			StartCoroutine(ReceiveFromWWW("/TextScripts/" + filename + ".txt", set_method));
		}
		else
		{
            string strPath;
            TextAsset scriptFile = new TextAsset();
			if(filename == "Common" || filename == "Error" || filename == "ErrorText")
            {
                strPath = "TextScripts/GameText/" + filename;
			    scriptFile = Resources.Load(strPath, typeof(TextAsset)) as TextAsset;
            }

            else
            {
                strPath = "TextScripts/" + filename;
			    scriptFile = (TextAsset)AssetMgr.Instance.AssetLoad(strPath + ".txt", typeof(TextAsset), true);
            }
			
			ParseFile(scriptFile.ToString(), set_method);
		}
		#endif
	}
	
	public void ParseFile(string strData, ReceiveInfo set_method)
	{
//		DebugMgr.Log("----- STR DATA : " + strData);
		string[] parseRow = strData.Split('\n');
		int startRow = 0;
		
		for (int i = 0; i < parseRow.Length; i++)
		{
			if (parseRow[i].Contains("*/"))
			{
				startRow = i + 1;
				break;
			}
		}

		string[] strTemp;
		for (int readRow = startRow; readRow < parseRow.Length; readRow++)
		{
			if (parseRow [readRow].Trim () == "")
				return;

			strTemp = parseRow[readRow].Split('\r');
            if( strTemp.Length == 2 )
				set_method(parseRow[readRow].Split('\r')[0].Split('\t'));
			else
				set_method(parseRow[readRow].Split('\t'));

		}
		
		set_method(null);	// call for end
	}
	
	public IEnumerator ReceiveFromWWW(string filename, ReceiveInfo set_method)
	{
		DebugMgr.Log("Func:ReceiveInfo, START " + filename);
		WWW hs_post = new WWW(serverAddress + filename);
		
		float sum = 0f;
		
		while (!hs_post.isDone && string.IsNullOrEmpty(hs_post.error) && sum < timeout)
		{
			sum += Time.deltaTime;
			
			//			DebugMgr.Log(sum);
			// 타임아웃 처리.
			if (sum > timeout)
			{
				// ErrorToTitle("-10000");
				// DebugMgr.Log("TimeOut => "+hs_post.text.Trim());
				yield break;
			}
			yield return 0;
		}
		
		//DebugMgr.Log(hs_post.text.Trim());
		if (hs_post.error != null)		// 에러.
		{
			DebugMgr.Log(hs_post.error.ToString());
		}
		else // 데이터 수신 완료.
		{
			DebugMgr.Log("Page : " + filename + " Result : " + hs_post.text.Trim());
			ParseFile(hs_post.text.Trim(), set_method);
//			if(Application.loadedLevelName == "ControllerScene") GameObject.FindGameObjectWithTag("UIMgr").GetComponent<BattleUIMgr>().Log(filename);
		}
		
		yield return null;
	}

	private ReceiveInfo setUserInfo;
	public void ReadUserInfo(ReceiveInfo set_method, EndLoading end_method)
	{
		if (loadStep != LOAD_STEP.INFO) return;

		#if __SERVER
		#else
		if (bLoadByWeb)
		{
			StartCoroutine(LoadUserInfoSuccess(set_method, end_method));
		}
		else
		#endif
		{
			setUserInfo = set_method;
			LoadTable(OnLoadUserInfo, "Test");
			end_method();
		}
	}
	IEnumerator LoadUserInfoSuccess(ReceiveInfo set_method, EndLoading end_method)
	{
		setUserInfo = set_method;
		LoadTable(OnLoadUserInfo, "Test");
		while (loadStep != LOAD_STEP.USER_DATA) { yield return null; }
		end_method();
	}
	public void OnLoadUserInfo(string[] cols)
	{
		if (cols == null)
		{
			loadStep = LOAD_STEP.USER_DATA;
		}
		else
		{
			setUserInfo(cols);
		}
	}

	private EndLoading EndMethodOfLoadAllStep;
	private bool bSetUserData;
	public void LoadAllStep(EndLoading loadedAsync, EndLoading end_method, bool _bSetUserData)
	{
		EndMethodOfLoadAllStep = end_method;
		bSetUserData = _bSetUserData;
		switch (DataMgr.Instance.LoadStep)
		{// Run by Battle Asset
			case DataMgr.LOAD_STEP.NONE:
				DataMgr.Instance.LoadData(loadedAsync, OnLoadInfo);
				break;
			case DataMgr.LOAD_STEP.INFO:
				OnLoadInfo();
				break;
			case DataMgr.LOAD_STEP.USER_DATA:
				OnLoadUserData();
				break;
			case DataMgr.LOAD_STEP.RELOGIN:
				OnLoadInfo ();
				break;
		}
	}

	public void OnLoadInfo()
	{
		// 유저 정보 셋팅 시작 부분
		if (bSetUserData)
		{
			Legion.Instance.SetUserData();
		}
		loadStep = LOAD_STEP.USER_DATA;
		OnLoadUserData();
	}

	public void OnLoadUserData()
	{
		EndMethodOfLoadAllStep();
	}

	public void ReLoadUserData(object[] param)
	{
		if(isReLogin)
			return;

		isReLogin = true;
		PopupManager.Instance.ShowLoadingPopup (1);
		if(Legion.Instance.cInventory.dicInventory !=null)
			Legion.Instance.InitUserData ();

        // 채팅 해제 하기
        // #CHATTING
        PopupManager.Instance.ChattingDisconnect();
        // 소셜 관련 정보 초기화
        SocialInfo.Instance.ClearSocialInfo();

		if(Server.ServerMgr.sessionID == "" || Server.ServerMgr.sessionID == null)
		{
			//DebugMgr.LogError("SessionID Check Fail");
            // 세션아이디가 없다면 타이틀로 보낸다
            isReLogin = false;
			UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
            return;
		}

        //Server.ServerMgr.Instance.Login((byte)99, Server.ServerMgr.id, Server.ServerMgr.sessionID, "", EndReLogin);
        Server.ServerMgr.Instance.GetLoginInfo(Legion.Instance.u2LastLoginServer, EndReLogin);
	}

	public void EndReLogin(Server.ERROR_ID err)
	{
		if(err == Server.ERROR_ID.LOGIN_BLOCK || err == Server.ERROR_ID.LOGIN_INSPECTION ||
			err == Server.ERROR_ID.LOGIN_PW_FAIL || err == Server.ERROR_ID.LOGIN_ID_FAIL)
		{
			//DebugMgr.LogError("EndReLogin fail " + err.ToString());
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),  TextManager.Instance.GetError(Server.MSGs.AUTH_LOGIN, err), Server.ServerMgr.Instance.ApplicationShutdown);
			return;
		}

		if(err != Server.ERROR_ID.NONE)
		{
			//DebugMgr.LogError(" EndReLogin fail " + err.ToString());
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),  TextManager.Instance.GetError(Server.MSGs.AUTH_LOGIN, err), Server.ServerMgr.Instance.ApplicationShutdown);
			return;
		}
		else
		{	
			//DebugMgr.Log("EndReLogin Ok");
			Server.ServerMgr.Instance.GetLoginMoreInfo(EndReLoginModoInfo);
		}
	}

	public void EndReLoginModoInfo(Server.ERROR_ID err)
	{
		isReLogin = false;
		PopupManager.Instance.CloseLoadingPopup();
		if(err != Server.ERROR_ID.NONE)
		{
			//DebugMgr.LogError("EndReLoginModoInfo() fail " + err.ToString());
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),  TextManager.Instance.GetError(Server.MSGs.AUTH_USERINFO, err), Server.ServerMgr.Instance.ApplicationShutdown);
		}
		else
		{
			//DebugMgr.Log("ReLogin Ok Lobby!!!");
			// 무조건 로비씬으로 보낸다
			FadeEffectMgr.Instance.QuickChangeScene("LobbyScene");
			/*
			BaseScene baseScene = Scene.GetCurrent();
			if(baseScene == null || baseScene.name != "LobbyScenee")
			{
				
				return;
			}
			if(baseScene.name == "LobbyScene")
			{
				((LobbyScene)baseScene).InitCharacter();
			}
			*/
		}
	}
}