using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Server
{
	public delegate void OnResponse(ERROR_ID err);

	public class ServerMgr : Singleton<ServerMgr>
	{
        
#if dev
		private string _buildTag = "dev";
#elif qa
		private string _buildTag = "qa";
#elif lv
		private string _buildTag = "lv";
#elif sales
		private string _buildTag = "sales";
#elif live
		private string _buildTag = "live";
#else
		private string _buildTag = "dev";
#endif

        private string serverAddress = "";

		public void SetServerAddress(string addr){
            serverAddress = addr;
            //23333포트로 접속하기위한 임시 주소
            //serverAddress = "http://61.75.62.106:23333/APIService/Message";
            //26333포트로 접속하기위한 임시 주소
            //serverAddress = "http://61.75.62.106:26333/APIService/Message";
        }
    
		//Port 23333 // 폰 서버
		//Port 25333 // 외부 세일즈 서버
		//Port 26333 // 내부 레벨링 서버

		readonly private float timeout = 15f;
		public static bool bConnectToServer = true;
		readonly static public string appTag = "NYX";
		private UInt16 appVer = 32; // 서버 버전

#if UNITY_EDITOR
		readonly static public Byte platform = 3;
#elif UNITY_ONESTORE
		readonly static public Byte platform = 4;
#elif UNITY_ANDROID        
		readonly static public Byte platform = 2;
#elif UNITY_IOS
		readonly static public Byte platform = 1;
#endif   
		readonly static public Byte headVersion = 1;
		readonly static public Byte majorVersion = 0;
		readonly static public Byte minorVersion = 32; // 클라 버전
		static public string version { get {return headVersion.ToString("0")+"."+majorVersion.ToString("0")+"."+minorVersion.ToString("0"); }}
		static public string clientVersionString { get {return "v "+headVersion.ToString("0")+"."+majorVersion.ToString("0")+"."+minorVersion.ToString("0"); }}
		public string serverVersion { get {return appVer.ToString(); }}

		private string _fileServerUrl;

		public string fileServerUrl{
			get{ return _fileServerUrl; }
			set{ _fileServerUrl = value; }
		}

		public class RequestJob
		{
			public byte[] packet;
			public UInt16 seqNo;
			public System.Object obj1;
			public System.Object obj2;
			public OnResponse callback;
			public RequestJob(UInt16 seqno, byte[] buf, long length, System.Object _obj1, System.Object _obj2, OnResponse _callback)
			{
				seqNo = seqno;
				packet = new Byte[length];
				Array.Copy(buf, 0, packet, 0, length);
				obj1 = _obj1;
				obj2 = _obj2;
				callback = _callback;
			}
		}

		public static string id = string.Empty;
		public static string sessionID = string.Empty;

		byte[] outpacket;
		MemoryStream msBufW;
		BinaryWriter bwOut;
		UInt16 u2SeqNo = 1;

		public ERROR_ID error { get; private set; }
		public string errorString { get; private set; }
		public bool bProcessing { get; private set; }
		Queue<RequestJob> qRequest = new Queue<RequestJob>();
		Dictionary<UInt16, RequestJob> dicRequest = new Dictionary<UInt16, RequestJob>();

		public void Awake ()
		{            
			outpacket = new byte[ConstDef.ClientPacketMaxSize];
			msBufW = new MemoryStream(outpacket);
			bwOut = new BinaryWriter(msBufW, Encoding.Unicode);

			error = ERROR_ID.NONE;
			bProcessing = false;
		}

		public string GetBuildTag(){
			return _buildTag;
		}

		void RetryConnect(object[] param)
		{
			PopupManager.Instance.ShowLoadingPopup (1);
			Retry ();
			bProcessing = false;
		}

		public void ApplicationShutdown(object[] param)
		{
			Application.Quit ();
		}

		// Update is called once per frame
		public void Update ()
		{
			//if (Application.internetReachability == NetworkReachablility.NotReachable)
			//{
			//	// popup network disconnect
			//	return;
			//}

			if (error == ERROR_ID.NETWORK_ANSWER_DELAYED)
			{
				// popup network disconnect
				return;
			}

			if (bProcessing)
			{
				// popup network waiting
				return;
			}

			if (qRequest.Count > 0 && !bProcessing && error == ERROR_ID.NONE)
			{
				bProcessing = true;
#if __SERVER
				RequestToServer(qRequest.Peek());
#else
				StartCoroutine(RequestToServer(qRequest.Peek()));
#endif
			}
		}

#if __SERVER
		void RequestToServer(RequestJob job)
#else
		IEnumerator RequestToServer(RequestJob job)
#endif
		{
			if (bConnectToServer)
			{
				//internet notconnect
				if(Application.internetReachability == NetworkReachability.NotReachable){
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_network_error"),
														TextManager.Instance.GetErrorText("NETWORK_NOT_AVAILABLE", "", false), 
														Server.ServerMgr.Instance.ApplicationShutdown);
					yield break;
				}
				
				WWWForm from = new WWWForm();
				var headers = from.headers;
				headers["Content-Type"] = "application/json";
				REQ_ENCRYTO_DATA reqData = Message.EncryotRequestData(id, job.packet, job.packet.Length);
				string jsonString = LitJson.JsonMapper.ToJson(reqData);
				byte[] jsonBinary = Encoding.UTF8.GetBytes(jsonString);
                
                //DebugMgr.Log(jsonBinary.Length);
                
				WWW hs_post = new WWW(serverAddress, jsonBinary, headers);
				//DebugMgr.Log("Send Message to Server");

				float sum = 0f;

				while (!hs_post.isDone && string.IsNullOrEmpty(hs_post.error) && sum < timeout)
				{
					sum += Time.deltaTime;
					if (sum > timeout)
					{
						DebugMgr.Log("time out " + sum);
						error = ERROR_ID.NETWORK_ANSWER_DELAYED;
						//job.callback(error);
						PopupManager.Instance.CloseLoadingPopup();
						PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)error).ToString()), RetryConnect);
						yield break;
					}
					yield return hs_post;
				}
				
				if (hs_post.error != null)
				{
					errorString = hs_post.error.ToString();
					DebugMgr.LogError(errorString);
					//error = ERROR_ID.NETWORK_ANSWER_FAILED;
					//job.callback(error);
					PopupManager.Instance.CloseLoadingPopup();
					PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)ERROR_ID.NETWORK_ANSWER_FAILED).ToString()), RetryConnect);
					yield break;
				}
				else
				{
					RES_ENCRYTO_DATA resData = LitJson.JsonMapper.ToObject<RES_ENCRYTO_DATA>(hs_post.text);
					if ((ERROR_ID)resData.Result == ERROR_ID.PREV_REQUEST_NOT_COMPLETE)
					{
						yield return new WaitForSeconds(timeout);
						PopupManager.Instance.CloseLoadingPopup();
						PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)ERROR_ID.PREV_REQUEST_NOT_COMPLETE).ToString()), RetryConnect);
						yield break;
					}
					else if ((ERROR_ID)resData.Result != ERROR_ID.NONE && (ERROR_ID)resData.Result < ERROR_ID.NO_DATA_PACKET)
					{
						job.callback((ERROR_ID)resData.Result);
						error = (ERROR_ID)resData.Result;
						bProcessing = false;
						yield break;
					}
					UInt16 seqNo = 0;
					ERROR_ID err = Message.ParseMessage(resData, out seqNo, job);
					if (err != ERROR_ID.NONE)
					{
						if (err < ERROR_ID.NO_DATA_PACKET)
						{
							job.callback(err);
							error = err;
                            //error = ERROR_ID.NONE;
							bProcessing = false;
							yield break;
						}
						else
                        {
							error = err;
                            //error = ERROR_ID.NONE;
                        }
					}
					else
					{
						dicRequest.Remove(seqNo);
						qRequest.Dequeue();
					}
					bProcessing = false;
				}
				
				yield return null;
			}
			else
			{
				REQ_ENCRYTO_DATA reqData = Message.EncryotRequestData(id, job.packet, job.packet.Length);
				RES_ENCRYTO_DATA resData = new RES_ENCRYTO_DATA
				{
					Result = 0,
					Data = reqData.Data,
				};
				UInt16 seqNo = 0;
				ERROR_ID err = Message.ParseMessage(resData, out seqNo, job);
				if (err != ERROR_ID.NONE)
				{
					error = err;
				}
				else
				{
					dicRequest.Remove(seqNo);
					qRequest.Dequeue();
				}
				bProcessing = false;
				
				yield return null;
			}
		}

		void AddJob(OnResponse callback, System.Object obj1 = null, System.Object obj2 = null)
		{
			RequestJob job = new RequestJob(u2SeqNo, outpacket, bwOut.BaseStream.Position, obj1, obj2, callback);
			qRequest.Enqueue(job);
			dicRequest.Add(u2SeqNo, job);
			u2SeqNo++;
		}

		public bool IsExist(UInt16 seqNo)
		{
			return ServerMgr.Instance.dicRequest.ContainsKey(seqNo);
		}

		//public RequestJob GetJob(UInt16 seqNo)
		//{
		//	RequestJob job = null;
		//	ServerMgr.Instance.dicRequest.TryGetValue(seqNo, out job);
		//	return job;
		//}

		public void Retry()
		{
			error = ERROR_ID.NONE;
		}
        public void CallClear(object[] param)
        {
            ClearFirstJobError();
        }
		public void ClearFirstJobError()
		{
			error = ERROR_ID.NONE;
            if(qRequest.Count == 0)
                return;
			RequestJob job = qRequest.Peek();
			dicRequest.Remove(job.seqNo);
			qRequest.Dequeue();
            bProcessing = false;
		}

		public UInt16 Join(Byte publishingType, string phoneSN, Byte language, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.AUTH_JOIN, seqno);
			if (bConnectToServer)
			{
				bwOut.Write(appTag);
				bwOut.Write(appVer);
				bwOut.Write(platform);
				bwOut.Write(publishingType);
				bwOut.Write(phoneSN);
                bwOut.Write(language);
            }
			else
			{
				bwOut.Write("GeustID");
				bwOut.Write("SessionID");
			}
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

		public UInt16 Login(Byte publishingType, string guestID, string phoneSN, string email, OnResponse callback)
		{
			id = guestID;

			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.AUTH_LOGIN, seqno);
			bwOut.Write(appTag);
			bwOut.Write(appVer);
			bwOut.Write(platform);
			bwOut.Write(publishingType);
			if(publishingType == 1) bwOut.Write(guestID);
			bwOut.Write(phoneSN);
			if (publishingType == 2 || publishingType == 3) { 
				if (string.IsNullOrEmpty (email)) 
					email = "none"; 

				bwOut.Write (email); //google, facebook 계정일때만 이메일 전송 
			}
		#if UNITY_EDITOR
			bwOut.Write("PC");
			bwOut.Write("None");
		#else
			bwOut.Write(SystemInfo.deviceModel);
			bwOut.Write(SystemInfo.operatingSystem.Split('/')[0]);
		#endif
			if (System.Globalization.CultureInfo.CurrentCulture != null) {
				Debug.LogError (System.Globalization.CultureInfo.CurrentCulture.Name);
				bwOut.Write (System.Globalization.CultureInfo.CurrentCulture.Name);
			} else {
				bwOut.Write ("");
			}
                                        
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

        public UInt16 GetLoginInfo(UInt16 u2ServerID, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.AUTH_USERINFO, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u2ServerID);
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

		public UInt16 GetLoginMoreInfo(OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.AUTH_USERSTATE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

        public UInt16 Logout(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.AUTH_LOGOUT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 Quit(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.AUTH_QUIT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

		public UInt16 SetLegionName(string name, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.LEGION_SET_NAME, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(name);
			ServerMgr.Instance.AddJob(callback, name);
			return seqno;
		}

		public UInt16 OpenCrew(Crew crew, string name, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.CREW_OPEN, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(crew.u1Index);
			ServerMgr.Instance.AddJob(callback, crew, name);
			return seqno;
		}

		public UInt16 OpenSlotOfCrew(Crew crew, Byte u1Pos, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.CREW_OPENSLOT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(crew.u1Index);
			bwOut.Write(u1Pos);
			ServerMgr.Instance.AddJob(callback, crew, u1Pos);
			return seqno;
		}

		public UInt16 ChangeCrewInfo(Crew crew, Hero[] heroes, Byte[] u1Poses, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.CREW_CHANGE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(crew.u1Index);
			bwOut.Write((Byte)heroes.Length);
			for (int i = 0; i < heroes.Length; i++)
			{
				bwOut.Write(u1Poses[i]);
                if (heroes[i] == null)
					bwOut.Write((byte)0);
				else
					bwOut.Write(heroes[i].u1Index);
			}
			ServerMgr.Instance.AddJob(callback, crew);
			return seqno;
		}

		public UInt16 SelectCrew(Crew crew, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.CREW_SELECT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(crew.u1Index);
			ServerMgr.Instance.AddJob(callback, crew);
			return seqno;
		}

		public UInt16 CreateCharacter(string name, UInt16 classID, Byte u1SelectedHair, Byte u1SelectedHairColor, Byte u1SelectedFace, Byte u1TutorialType, Byte u1TutorialStep, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			Hero hero = new Hero(0, classID, name, u1SelectedHair, u1SelectedHairColor, u1SelectedFace);
			UInt32[] stats = new UInt32[ConstDef.CharStatPointType];
			hero.GetComponent<StatusComponent>().LoadStatus(stats, 0);
			hero.GetComponent<LevelComponent>().Set((Byte)ConstDef.BaseHeroLevel, 0);
			List<LearnedSkill> initSkills = hero.GetComponent<SkillComponent>().GetInitSkill ();
			hero.GetComponent<SkillComponent>().LoadSkill(Server.ConstDef.DefaultSkillSelectSlot, initSkills, 0, 0, 0);

			PacketHeader.Set(bwOut, MSGs.CHAR_CREATE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(name);
			bwOut.Write(classID);
			Byte[] shape = new Byte[ConstDef.LengthOfShape];
			shape[0] = u1SelectedHair;
			shape[1] = u1SelectedHairColor;
			shape[2] = u1SelectedFace;
			bwOut.Write(shape);
			bwOut.Write(u1TutorialType);
			if(u1TutorialType != 255) bwOut.Write(u1TutorialStep);
			ServerMgr.Instance.AddJob(callback, hero);
			return seqno;
		}

		public UInt16 CreateCharacterForTutorialReward(string name, UInt16 classID, Byte u1SelectedHair, Byte u1SelectedHairColor, Byte u1SelectedFace, Byte u1TutorialType, Byte u1TutorialStep, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.CHAR_CREATE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(name);
			bwOut.Write(classID);
			Byte[] shape = new Byte[ConstDef.LengthOfShape];
			shape[0] = u1SelectedHair;
			shape[1] = u1SelectedHairColor;
			shape[2] = u1SelectedFace;
			bwOut.Write(shape);
			bwOut.Write(u1TutorialType);
			if(u1TutorialType != 255) bwOut.Write(u1TutorialStep);
			ServerMgr.Instance.AddJob(callback, null);
			return seqno;
		}

		public UInt16 RetireCharacter(Hero hero, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.CHAR_RETIRE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(hero.u1Index);
			ServerMgr.Instance.AddJob(callback, hero);
			return seqno;
		}

		public UInt16 ResetCharacterStatus(Hero hero, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.CHAR_STAT_RESET, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(hero.u1Index);
			ServerMgr.Instance.AddJob(callback, hero);
			return seqno;
		}

		public UInt16 PointCharacterStatus(Hero[] hero, UInt16[][] stat, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.CHAR_STAT_POINT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write((Byte)hero.Length);
			for (int i = 0; i < hero.Length; i++)
            {
				bwOut.Write (hero[i].u1Index);
				for(int j=0; j<ConstDef.CharStatPointType; j++)
                {
					bwOut.Write (stat[i][j]);
				}
			}
			ServerMgr.Instance.AddJob(callback, hero, stat);
			return seqno;
		}

        public UInt16 ToggleCharacterStatus(Byte OnOff, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.CHAR_STAT_POINT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(OnOff);
			
			ServerMgr.Instance.AddJob(callback, OnOff);
			return seqno;
		}

        public UInt16 BuyCharacterStatusPoint(Hero hero, Byte point, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.CHAR_BUY_STAT_POINT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(hero.u1Index);
            bwOut.Write(point);
			
			ServerMgr.Instance.AddJob(callback, hero, point);
			return seqno;
		}

		public UInt16 WearHero(Hero hero, UInt16[] slots, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.CHAR_EQUIP_CHANGE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(hero.u1Index);
			bwOut.Write((Byte)slots.Length);
			for (Byte i = 0; i < slots.Length; i++)
				bwOut.Write(slots[i]);
			ServerMgr.Instance.AddJob(callback, hero, slots);
			return seqno;
		}

		public UInt16 OpenSkillSelectSlot(Hero hero, Byte u1SelectSlot, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.CHAR_SKILL_OPEN, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(hero.u1Index);
			bwOut.Write(u1SelectSlot);
			ServerMgr.Instance.AddJob(callback, hero, u1SelectSlot);
			return seqno;
		}

		public UInt16 ResetSkill(Hero hero, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.CHAR_SKILL_RESET, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(hero.u1Index);
			ServerMgr.Instance.AddJob(callback, hero);
			return seqno;
		}

		public UInt16 ChangeSkill(Byte[] hero, List<LearnedSkill>[] lstLearnInfo, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.CHAR_SKILL_CHANGE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write ((byte)hero.Length);
			for (int i = 0; i < hero.Length; i++) {
				bwOut.Write (hero[i]);
				bwOut.Write ((Byte)lstLearnInfo[i].Count);
				foreach (LearnedSkill temp in lstLearnInfo[i]) {
					bwOut.Write (temp.u1SlotNum);
					bwOut.Write ((UInt16)temp.u2Level);
					bwOut.Write (temp.u1UseIndex);
				}
			}
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

		public UInt16 BuyCharacterSkillPoint(Hero hero, Byte point, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.CHAR_BUY_SKILL_POINT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(hero.u1Index);
			bwOut.Write(point);
			
			ServerMgr.Instance.AddJob(callback, hero, point);
			return seqno;
		}

		public UInt16 StartStage(Crew crew, StageInfo stage, Byte u1Difficulty, OnResponse callback, Boolean isRepeat = false)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.STAGE_START, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(stage.u2ID);
			bwOut.Write(u1Difficulty);
			bwOut.Write(crew.u1Index);
			bwOut.Write((Byte)(isRepeat == true ? 1 : 0));	// 반복전투 여부
			ServerMgr.Instance.AddJob(callback, crew, new StageHandler.Stage(stage, u1Difficulty));
			return seqno;
		}

		public UInt16 ClearStage(Crew crew, Byte u1Result, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.STAGE_RESULT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(u1Result);
			ServerMgr.Instance.AddJob(callback, crew, u1Result);
			return seqno;
		}

		public UInt16 DispatchStage(Crew crew, StageInfo stage, Byte u1Difficulty, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.STAGE_DISPATCH, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(stage.u2ID);
			bwOut.Write(u1Difficulty);
			bwOut.Write(crew.u1Index);
			ServerMgr.Instance.AddJob(callback, crew, new StageHandler.Stage(stage, u1Difficulty));
			return seqno;
		}

		public UInt16 CancelDispatchStage(Crew crew, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.STAGE_DISPATCH_CANCEL, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(crew.u1Index);
			ServerMgr.Instance.AddJob(callback, crew);
			return seqno;
		}

		public UInt16 GetDispatchResult(Crew crew, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.STAGE_DISPATCH_RESULT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(crew.u1Index);
			ServerMgr.Instance.AddJob(callback, crew);
			return seqno;
		}

		public UInt16 FinishDispatchStage(Crew crew, UInt32 doneCostCount, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.STAGE_DISPATCH_FINISH, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(crew.u1Index);
            bwOut.Write(doneCostCount);

            ServerMgr.Instance.AddJob(callback, crew);
			return seqno;
		}

		public UInt16 SweepStage(StageInfo stage, Byte u1Difficulty, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.STAGE_SWEEP, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(stage.u2ID);
			bwOut.Write(u1Difficulty);
			ServerMgr.Instance.AddJob(callback, new StageHandler.Stage(stage, u1Difficulty));
			return seqno;
		}

		public UInt16 BuyTicket_Forest(UInt16 stageID, OnResponse callBack)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.MODE_BUY_FOREST_TICKET, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(stageID);
			bwOut.Write(Legion.Instance.SelectedDifficult);
			ServerMgr.Instance.AddJob(callBack, stageID);
			return seqno;
		}

		public UInt16 ResetEquipmentStatus(EquipmentItem item, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.EQUIP_STAT_RESET, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(item.u2SlotNum);
			ServerMgr.Instance.AddJob(callback, item);
			return seqno;
		}

		public UInt16 PointEquipmentStatus(EquipmentItem item, UInt16[] stat, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.EQUIP_STAT_POINT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(item.u2SlotNum);
			foreach (UInt16 pt in stat)
			{
				bwOut.Write(pt);
                DebugMgr.Log(pt);
			}
			ServerMgr.Instance.AddJob(callback, item, stat);
			return seqno;
		}

		public UInt16 BuyEquipmentStatPoint(EquipmentItem item, byte count, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.EQUIP_BUY_STAT_POINT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(item.u2SlotNum);
			bwOut.Write(count);
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}
        
        public UInt16 EquipCheckSlot(UInt16[] slots, OnResponse callback)
        {
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.EQUIP_CHECK_SLOT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write((UInt16)slots.Length);
            
            for(int i=0; i<slots.Length; i++)
            {
                bwOut.Write((UInt16)slots[i]);
            }            
            
			ServerMgr.Instance.AddJob(callback);
			return seqno;            
        }

		public UInt16 InvenUseItem(UInt16 Slot, UInt16 count, Byte CharIndex, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.ITEM_USE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(Slot);
			bwOut.Write(count);
			bwOut.Write(CharIndex);
			ServerMgr.Instance.AddJob(callback, CharIndex, Slot);
			return seqno;
		}

		public UInt16 InvenSellItem(UInt16 slot, UInt16 count, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
            
            DebugMgr.Log(slot);
			
			PacketHeader.Set(bwOut, MSGs.ITEM_SELL, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(slot);
			bwOut.Write(count);
			ServerMgr.Instance.AddJob(callback, slot, count);
			return seqno;
		}

		public UInt16 RequsetShopList(Byte shoptype, Byte renew, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.SHOP_LIST, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(shoptype);
			bwOut.Write(renew);
            bwOut.Write(0);
			ServerMgr.Instance.AddJob(callback, false);
			return seqno;
		}
		
		public UInt16 RequsetBlackShopList(Byte shoptype, Byte renew, Byte useCacsh, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.SHOP_LIST, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(shoptype);
			bwOut.Write(renew);
			bwOut.Write(useCacsh);
			ServerMgr.Instance.AddJob(callback, true);
			return seqno;
		}

		public UInt16 ShopBuyItem(Byte shoptype, Byte slot, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.SHOP_BUY, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(shoptype);
			bwOut.Write(slot);
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

		public UInt16 ShopResister(Item item, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.SHOP_REGISTER, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(item.u2SlotNum);
            ServerMgr.Instance.AddJob(callback);
            return seqno;
		}
		
		public UInt16 ShopFixShop(UInt16 fixID, Byte repeatDiscount, Byte u1TutorialType, Byte u1TutorialStep, string receipt, string Txid, OnResponse callback)
		{

			UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.SHOP_FIXSHOP, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(fixID);
            bwOut.Write(repeatDiscount);
			bwOut.Write(u1TutorialType);
			if(u1TutorialType != 255) bwOut.Write(u1TutorialStep);
            bwOut.Write(receipt);
            bwOut.Write(Txid);
			Debug.LogError (Txid);
            ServerMgr.Instance.AddJob(callback, fixID);
            return seqno;
		} 

        public UInt16 RequestLeagueMatch(UInt64 u8UserSN, Byte u1Revenge, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.LEAGUE_MATCH, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u8UserSN);
            bwOut.Write(u1Revenge);

            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }

        public UInt16 RequestLeagueMatchStart(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.LEAGUE_START_MATCH, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			
            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }

        public UInt16 RequestLeagueMatchResult(Byte Result, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.LEAGUE_MATCH_RESULT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(Result);
            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }

		public UInt16 RequestLeagueRevengeMessage(string msg ,OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;

			PacketHeader.Set(bwOut, MSGs.LEAGUE_REVENGEMESSAGE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(msg);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

        public UInt16 RequestLeagueSetCrew(Byte[] _index, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.LEAGUE_SET_CREW, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            for(int i=0; i<_index.Length; i++)
			    bwOut.Write(_index[i]);
            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }

		public UInt16 RequestLeagueReawrd(OnResponse callback)
		{
            UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.LEAGUE_REWARD, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }

        public UInt16 RequestLeagueGroup(Byte CrewIndex, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			
			//PacketHeader.Set(bwOut, MSGs.LEAGUE_GROUP, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(CrewIndex);

            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }

        public UInt16 RequestLeagueGroups(Byte Division, Byte State, UInt16 GroupNo, Byte UpDown, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			
			//PacketHeader.Set(bwOut, MSGs.LEAGUE_GROUPS, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(Division);
            bwOut.Write(State);
            bwOut.Write(GroupNo);
            bwOut.Write(UpDown);

            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }

        public UInt16 RequestLeagueLegend(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.LEAGUE_LEGENDRANK, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }

        public UInt16 RequestLeagueMatchList(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.LEAGUE_GET_MATCHLIST, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }

        public UInt16 RequestLeagueBuyKey(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.LEAGUE_BUY_KEY, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }

        public UInt16 RequestLeagueDivisionCheck(Byte u1Division, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.LEAGUE_CHECK, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u1Division);

            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }

		public UInt16 ForgeRune(UInt16 runeID, UInt16 count, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
		
			PacketHeader.Set(bwOut, MSGs.FORGE_RUNE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(runeID);
			bwOut.Write(count);
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

		public UInt16 SmithingEquipmentItem(UI_Panel_Forge_Smithing_Detail smithingInfo, Byte u1TutorialType, Byte u1TutorialStep, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.FORGE_SMITH, seqno);

			UInt16 equipID = smithingInfo._cEquipInfo.u2ID;
			Byte smithLevel = smithingInfo._cForgeInfo.u1Level;
			Byte selectSkillCount = smithingInfo._u1SelectedSkillCount;
			Byte[] skillSlots = smithingInfo._au1SelectedSkill;
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(equipID);
			bwOut.Write(smithLevel);
			bwOut.Write(selectSkillCount);
            for(int i=0; i<selectSkillCount; i++)
			{
				bwOut.Write(skillSlots[i]);
			}
			bwOut.Write(u1TutorialType);
			if(u1TutorialType != 255) bwOut.Write(u1TutorialStep);
			ServerMgr.Instance.AddJob(callback, smithingInfo);
			return seqno;
		}

		public UInt16 CheckDesign(Byte checkType, UInt16 count, UInt16[] equipID, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			
			PacketHeader.Set(bwOut, MSGs.FORGE_CHECK_DESIGN, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(checkType);
			bwOut.Write(count);
			for(int i=0; i<count; i++)
			{
				bwOut.Write(equipID[i]);
			}
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

		public UInt16 ChangeEquipName(EquipmentItem equipItem, string name, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FORGE_NAME, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(name);

			ServerMgr.Instance.AddJob(callback, equipItem, name);
			return seqno;
		}

		public UInt16 EquipmentFusion(EquipmentItem baseEquipItem, UInt16[] materialInvenNums, OnResponse callback) 
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FORGE_FUSION, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(baseEquipItem.u2SlotNum);
			DebugMgr.Log("Fusion Base Slot Num : " + baseEquipItem.u2SlotNum);
			bwOut.Write(Convert.ToUInt16(materialInvenNums.Length));
			foreach(UInt16 slotNum in materialInvenNums)
			{
				DebugMgr.Log("Fusion Mat SlotNum : " + slotNum);
				bwOut.Write(slotNum);
			}
			
			ServerMgr.Instance.AddJob(callback, baseEquipItem, materialInvenNums);
			return seqno;
		}

		public UInt16 ChangeLook(UInt16 invenSlotNum, UInt16 selectModelID, OnResponse callback) 
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FORGE_CHANGE_LOOK, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(invenSlotNum);
			bwOut.Write(selectModelID);
			ServerMgr.Instance.AddJob(callback, invenSlotNum, selectModelID);
			return seqno;
		}

		public UInt16 UpgradeForge(OnResponse callback) 
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FORGE_UPGRADE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}
		
		public UInt16 StageRepeatReward(UInt16 chapterID, Byte difficult, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.STAGE_REPEATREWARD, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(chapterID);
			bwOut.Write(difficult);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}
		
		public UInt16 StageStarReward(UInt16 chapterID, Byte difficult, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.STAGE_STARREWARD, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(chapterID);
			bwOut.Write(difficult);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}		

		public UInt16 RequestQuestAccept(UInt16 QuestID, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.QUEST_ACCEPT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(QuestID);
			
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

		public UInt16 RequestQuestCancel(OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.QUEST_CANCEL, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

		public UInt16 RequestQuestComplete(OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.QUEST_COMPLETE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

		public UInt16 RequestQuestAchievementReward(UInt16[] AchievementID, Byte u1TutorialType, Byte u1TutorialStep, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.QUEST_ACHIEVEMENT_REWARD, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write((UInt16)AchievementID.Length);
            for(int i=0; i<AchievementID.Length; i++)
			    bwOut.Write(AchievementID[i]);
			bwOut.Write(u1TutorialType);
			if(u1TutorialType != 255) bwOut.Write(u1TutorialStep);
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}

		public UInt16 RequestTutorial(Byte Type, Byte Step, OnResponse callback)
		{
			DebugMgr.LogError (Type+" : "+Legion.Instance.cTutorial.au1Step [Type]+"->"+Step);
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.LEGION_TUTORIAL, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(Type);
			bwOut.Write(Step);
			
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}
        
		public UInt16 CharBeginTraining(Byte roomNum, Byte[] charIndexArr, Byte[] seatNumArr, Byte timeType,  OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.CHAR_BEGIN_TRAINING, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(roomNum);
            bwOut.Write(timeType);
			bwOut.Write((Byte)charIndexArr.Length);
            for(int i=0; i<charIndexArr.Length; i++)
            {
                bwOut.Write(charIndexArr[i]);
                bwOut.Write(seatNumArr[i]);
                
            }
			
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}        

		public UInt16 CharEndTraining(Byte roomNum, TrainingInfo info, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.CHAR_END_TRAINING, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(roomNum);
			
			ServerMgr.Instance.AddJob(callback, info);
			return seqno;
		}  
        
		public UInt16 CharCancelTraining(Byte roomNum, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.CHAR_CANCEL_TRAINING, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(roomNum);
			
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}  
        
		public UInt16 CharFinishTraining(Byte roomNum, UInt32 coastCount, TrainingInfo info, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.CHAR_FINISH_TRAINING, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(roomNum);
            bwOut.Write(coastCount);
            
			ServerMgr.Instance.AddJob(callback, info);
			return seqno;
		}
        
        public UInt16 CharOpenTrainingSeat(Byte roomNum, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.CHAR_OPEN_TRAINING_SEAT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(roomNum);
            
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}
        
		public UInt16 EquipBeginTraining(Byte roomNum, UInt16[] eqiupSlotArr, Byte[] seatNumArr, Byte timeType,  OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.EQUIP_BEGIN_TRAINING, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(roomNum);
            bwOut.Write(timeType);
			bwOut.Write((Byte)eqiupSlotArr.Length);
            for(int i=0; i<eqiupSlotArr.Length; i++)
            {
                bwOut.Write(eqiupSlotArr[i]);
                bwOut.Write(seatNumArr[i]);                
            }
			
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}        

		public UInt16 EquipEndTraining(Byte roomNum, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.EQUIP_END_TRAINING, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(roomNum);
			
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}  
        
		public UInt16 EquipCancelTraining(Byte roomNum, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.EQUIP_CANCEL_TRAINING, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(roomNum);
			
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}  
        
		public UInt16 EquipFinishTraining(Byte roomNum, UInt32 coastCount, TrainingInfo info, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.EQUIP_FINISH_TRAINING, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(roomNum);
            bwOut.Write(coastCount);
            
			ServerMgr.Instance.AddJob(callback, info);
			return seqno;
		}
        
        public UInt16 EquipOpenTrainingSeat(Byte roomNum, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.EQUIP_OPEN_TRAINING_SEAT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(roomNum);
            
			ServerMgr.Instance.AddJob(callback);
			return seqno;
		}        
        
        public UInt16 LegionMark(Byte type, OnResponse callback)
        {
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.LEGION_MARK, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(type);
            
			ServerMgr.Instance.AddJob(callback, type);
			return seqno;            
        }            
        
        public UInt16 OptionPush(string deviceID, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.OPTION_PUSH, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(deviceID);
            
			ServerMgr.Instance.AddJob(callback);
			return seqno; 
        }  

		public UInt16 OptionAccount(Byte PublishingType, string Token, string email, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.OPTION_ACCOUNT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(PublishingType);
			bwOut.Write(Token);
			bwOut.Write(email);

			ServerMgr.Instance.AddJob(callback);
			return seqno; 
		}
		
        public UInt16 OptionSet(Byte u1OptionSet, Byte lenguage, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.OPTION_SET, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u1OptionSet);
            bwOut.Write(lenguage);
            
			ServerMgr.Instance.AddJob(callback);
			return seqno; 
        } 

		public UInt16 CheatMsg(string cheatMSG, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.ETC_COMMAND, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(cheatMSG);
			ServerMgr.Instance.AddJob(callback);
			return seqno; 
		}     
        
        public UInt16 RequestFriendNewCheck(UInt64[] friendSN, UInt64[] requestSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FRIEND_CHECK_NEW, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(Convert.ToByte(friendSN.Length));
			for(int i=0; i<friendSN.Length; i++)
                bwOut.Write(friendSN[i]);
            bwOut.Write(Convert.ToByte(requestSN.Length));
            for(int i=0; i<requestSN.Length; i++)
                bwOut.Write(requestSN[i]);

			ServerMgr.Instance.AddJob(callback);
			return seqno; 
        }

        public UInt16 RequestFriendList(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FRIEND_LIST, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			
			ServerMgr.Instance.AddJob(callback);
			return seqno; 
        }

        public UInt16 RequestFriendInvite(UInt64[] u8UserSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FRIEND_INVITE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(Convert.ToByte(u8UserSN.Length));
            for(int i=0; i<u8UserSN.Length; i++)
			    bwOut.Write(u8UserSN[i]);
			ServerMgr.Instance.AddJob(callback);
			return seqno; 
        }

        public UInt16 RequestFriendInviteCancel(UInt64[] u8UserSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FRIEND_CANCEL_INVITE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(Convert.ToByte(u8UserSN.Length));
			for(int i=0; i<u8UserSN.Length; i++)
                bwOut.Write(u8UserSN[i]);
			ServerMgr.Instance.AddJob(callback);
			return seqno; 
        }

        public UInt16 RequestFriendInviteConfirm(Byte u1Response, UInt64[] u8UserSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FRIEND_CONFIRM_INVITE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u1Response);
            bwOut.Write(Convert.ToByte(u8UserSN.Length));
            for(int i=0; i<u8UserSN.Length; i++)
			    bwOut.Write(u8UserSN[i]);
			ServerMgr.Instance.AddJob(callback);
			return seqno; 
        }

        public UInt16 RequestFriendRecommend(string strFriendName, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FRIEND_RECOMMEND, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(strFriendName);

			ServerMgr.Instance.AddJob(callback);
			return seqno; 
        }

        public UInt16 RequestFriendDrop(UInt64[] u8UserSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FRIEND_DROP, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(Convert.ToByte(u8UserSN.Length));
            for(int i=0; i<u8UserSN.Length; i++)
                bwOut.Write(u8UserSN[i]);
            
			ServerMgr.Instance.AddJob(callback);
			return seqno; 
        }

        public UInt16 RequestSendFriendPoint(UInt64[] u8UserSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FRIEND_SEND_POINT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(Convert.ToByte(u8UserSN.Length));
            for(int i=0; i<u8UserSN.Length; i++)
                bwOut.Write(u8UserSN[i]);
            
			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestReciveFriendPoint(UInt64[] u8UserSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.FRIEND_RECV_POINT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(Convert.ToByte(u8UserSN.Length));
            for(int i=0; i<u8UserSN.Length; i++)
                bwOut.Write(u8UserSN[i]);
            
			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestMailList(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.MAIL_LIST, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

            ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGetItemInMail(UInt16[] _mailSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.MAIL_GET, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(Convert.ToUInt16(_mailSN.Length));
            for(int i=0; i<_mailSN.Length; i++)
                bwOut.Write(Convert.ToUInt16(_mailSN[i]));

            ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestReadMail(UInt16[] _mailSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.MAIL_GET, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(_mailSN.Length);
            for(int i=0; i<_mailSN.Length; i++)
                bwOut.Write(_mailSN[i]);

            ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGetNotice(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.SOCIAL_GET_NOTICE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

            ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestCoupon(string _coupon, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.SOCIAL_COUPON, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(_coupon);

            ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestRankInfo(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.RANK_INFO, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

            ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestRankList(Byte u1RankType, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.RANK_LIST, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u1RankType);

            ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestRankReward(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.RANK_REWARD, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

            ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestEventGoodsBuy(UInt16 u2EventID, string strRecipeString, string strTxid, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.EVENT_BUY, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u2EventID);
            bwOut.Write(strRecipeString);
            bwOut.Write(strTxid);

            ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

		public UInt16 RequestEventGoodsReward(UInt16 u2EventID, OnResponse callback, Byte u1RewardIndex = 0)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.EVENT_REWARD, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u2EventID);
			bwOut.Write(u1RewardIndex);

            ServerMgr.Instance.AddJob(callback, u2EventID);
			return seqno;
        }

		public UInt16 RequestEventItemUse(UInt16 u2ItemID, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.EVENT_ITEMUSE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(u2ItemID);

			ServerMgr.Instance.AddJob(callback, u2ItemID);
			return seqno;
		}

		public UInt16 RequestAdReward(Byte u1Pos, OnResponse callback)
		{
			UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.AD_REWARD, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
			bwOut.Write(u1Pos);

			ServerMgr.Instance.AddJob(callback, u1Pos);
			return seqno;
		}

        public UInt16 RequestEventReload(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.EVENT_RELOAD, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestOxAnswer(Byte u1QuestionNum, Byte u1Answer, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.EVENT_OX, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u1QuestionNum);
            bwOut.Write(u1Answer);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestCreateGuild(String strGuildName, Byte u1JoinOption, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_CREATE, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(strGuildName);
            bwOut.Write(u1JoinOption);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGuildInfo(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_INFO, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGuildMark(Byte u1Type, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_MARK, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u1Type);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGuildSetMember(Byte u1Type, UInt64 u8UserSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_SET_MEMBER, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u1Type);
            bwOut.Write(u8UserSN);

			ServerMgr.Instance.AddJob(callback, u1Type, u8UserSN);
			return seqno;
        }

        public UInt16 RequestGuildSetCrew(Byte u1Type, UInt64 u8UserSN1, UInt64 u8UserSN2, UInt64 u8UserSN3, Byte u1CrewIndex, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_SET_CREW, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u1Type);
            //길드장 추천 덱 설정 및 유저 덱 설정
            if(u1Type == 1 || u1Type == 2)
            {
                bwOut.Write(u8UserSN1);
                bwOut.Write(u8UserSN2);
                bwOut.Write(u8UserSN3);
            }
            //유저 대표 크루 설정
            else
            {
                bwOut.Write(u1CrewIndex);
            }
			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGuildSearch(String strGuildName, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_SEARCH, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(strGuildName);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGuildJoinList(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_REQUEST_JOIN_LIST, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGuildJoin(UInt64 guildSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_REQUEST_JOIN, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(guildSN);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGuildMatchInfo(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_MATCH_INFO, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGuildMatch(UInt64 u8MatchingSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_MATCH, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u8MatchingSN);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGuildStartMatch(OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_START_MATCH, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGuildMatchResult(Byte u1Result, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_MATCH_RESULT, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u1Result);

			ServerMgr.Instance.AddJob(callback);
			return seqno;
        }

        public UInt16 RequestGuildRankInfo(Byte u1Type, UInt64 u8GuildSN, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
			PacketHeader.Set(bwOut, MSGs.GUILD_RANK_INFO, seqno);
			bwOut.Write(appTag);
			bwOut.Write(sessionID);
            bwOut.Write(u1Type);
            if(u1Type == 2)
                bwOut.Write(u8GuildSN);

			ServerMgr.Instance.AddJob(callback, u1Type);
			return seqno;
        }

        //#ODIN [오딘 미션 보상 요청]
        public UInt16 RequestMissionReward(Byte rewardType, UInt16 missionID, Byte oidnLevel, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
            PacketHeader.Set(bwOut, MSGs.MISSION_REWARD, seqno);
            bwOut.Write(appTag);
            bwOut.Write(sessionID);
            bwOut.Write(rewardType);
            // rewardType #보상 타입 1.임무보상, 2.단계 보상
            if (rewardType == 1)
            {
                bwOut.Write(missionID);
            }
            else if(rewardType == 2)
            {
                bwOut.Write(oidnLevel);
            }

            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }
        //#ODIN [오딘 미션 미루기 요청]
        public UInt16 RequestMissionPushback(UInt16 missionID, OnResponse callback)
        {
            UInt16 seqno = u2SeqNo;
            PacketHeader.Set(bwOut, MSGs.MISSION_PUSHBACK, seqno);
            bwOut.Write(appTag);
            bwOut.Write(sessionID);
            bwOut.Write(missionID);

            ServerMgr.Instance.AddJob(callback);
            return seqno;
        }
    }
}
