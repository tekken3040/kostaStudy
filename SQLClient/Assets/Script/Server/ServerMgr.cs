using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server
{
    public delegate void OnResponse(ERROR_ID err);

    public class ServerMgr : Singleton<ServerMgr>
    {
        private string serverAddress = "";

        public void SetServerAddress(string addr)
        {
            serverAddress = addr;
            serverAddress = "http://106.242.203.69:2738/";
        }

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
        static public string version { get { return headVersion.ToString("0") + "." + majorVersion.ToString("0") + "." + minorVersion.ToString("0"); } }
        static public string clientVersionString { get { return "v " + headVersion.ToString("0") + "." + majorVersion.ToString("0") + "." + minorVersion.ToString("0"); } }
        public string serverVersion { get { return appVer.ToString(); } }

        private string _fileServerUrl;

        public string fileServerUrl
        {
            get { return _fileServerUrl; }
            set { _fileServerUrl = value; }
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

        public void Awake()
        {
            outpacket = new byte[ConstDef.ClientPacketMaxSize];
            msBufW = new MemoryStream(outpacket);
            bwOut = new BinaryWriter(msBufW, Encoding.Unicode);

            error = ERROR_ID.NONE;
            bProcessing = false;
        }

        void RetryConnect(object[] param)
        {
            Retry();
            bProcessing = false;
        }

        public void ApplicationShutdown(object[] param)
        {
            Application.Quit();
        }

        // Update is called once per frame
        public void Update()
        {
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
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    yield break;
                }

                WWWForm form = new WWWForm();
                var headers = form.headers;
                headers["Content-Type"] = "application/json";
                REQ_ENCRYTO_DATA reqData = Message.EncryotRequestData(id, job.packet, job.packet.Length);
                string jsonString = LitJson.JsonMapper.ToJson(reqData);
                byte[] jsonBinary = Encoding.UTF8.GetBytes(jsonString);

                WWW hs_post = new WWW(serverAddress, jsonBinary, headers);

                float sum = 0f;

                while (!hs_post.isDone && string.IsNullOrEmpty(hs_post.error) && sum < timeout)
                {
                    sum += Time.deltaTime;
                    if (sum > timeout)
                    {
                        error = ERROR_ID.NETWORK_ANSWER_DELAYED;
                        yield break;
                    }
                    yield return hs_post;
                }

                if (hs_post.error != null)
                {
                    errorString = hs_post.error.ToString();
                    yield break;
                }
                else
                {
                    RES_ENCRYTO_DATA resData = LitJson.JsonMapper.ToObject<RES_ENCRYTO_DATA>(hs_post.text);
                    if ((ERROR_ID)resData.Result == ERROR_ID.PREV_REQUEST_NOT_COMPLETE)
                    {
                        yield return new WaitForSeconds(timeout);

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
                        Debug.Log("서버 접속");
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
            if (qRequest.Count == 0)
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
            if (publishingType == 1) bwOut.Write(guestID);
            bwOut.Write(phoneSN);
            if (publishingType == 2 || publishingType == 3)
            {
                if (string.IsNullOrEmpty(email))
                    email = "none";

                bwOut.Write(email); //google, facebook 계정일때만 이메일 전송 
            }
#if UNITY_EDITOR
            bwOut.Write("PC");
            bwOut.Write("None");
#else
			bwOut.Write(SystemInfo.deviceModel);
			bwOut.Write(SystemInfo.operatingSystem.Split('/')[0]);
#endif
            if (System.Globalization.CultureInfo.CurrentCulture != null)
            {
                Debug.LogError(System.Globalization.CultureInfo.CurrentCulture.Name);
                bwOut.Write(System.Globalization.CultureInfo.CurrentCulture.Name);
            }
            else
            {
                bwOut.Write("");
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
    }
}