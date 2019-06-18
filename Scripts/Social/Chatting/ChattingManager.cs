using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

using ExitGames.Client.Photon.Chat;
using ExitGames.Client.Photon;
using LitJson;

#region Message Class
public class BaseMessage
{
    public enum ChattingType
    {
        None = 0,
        UserChat,       // 일반
        GuidChat,       // 길드
        EquipChat,      // 장비
        NoticeChat,     // 공지
        SystemChat,     // 시스템
        Max,
    };
    private const UInt16 MAX_SUB_CHATTING_LENGTH = 15;  // 서브 채팅 메시지 최대 길이

    protected ChattingType eChattingType;
    protected string strMessage;

    public ChattingType EChattingType { get { return eChattingType; } }
    public string Message { get { return strMessage; } }

    public virtual bool SetChatMessage(string msg)
    {
        if(string.IsNullOrEmpty(msg))
        {
            return false;
        }
        strMessage = msg;
        return true;
    }

    public virtual bool SetChatMessage(string sendNane, string msg)
    {
        return false;
    }
    
    // 서브 채팅창에 입력될 값으로 변환하여 반환
    public virtual string GetSubChatMsg()
    {
        if (Message.Length > MAX_SUB_CHATTING_LENGTH)
        {
            return string.Format("{0}...", Message.Substring(0, MAX_SUB_CHATTING_LENGTH));
        }

        return Message;
    }
}
// 시스템 메시지
public class SystemChatMessage : BaseMessage
{
    public SystemChatMessage()
    {
        eChattingType = ChattingType.SystemChat;
    }
}
// 유저 채팅 메시지
public class UserChatMessage : BaseMessage
{
    public UserChatMessage()
    {
        eChattingType = ChattingType.UserChat;
    }

    public override bool SetChatMessage(string sendNane, string msg)
    {
        if(string.IsNullOrEmpty(msg))
        {
            return false;
        }
        strMessage = string.Format(TextManager.Instance.GetText("chat_text_color_user"), sendNane, msg);

        return true;
    }
}
// 공지 채팅 메시지
public class NoticeChatMessage : BaseMessage
{
    public NoticeChatMessage()
    {
        eChattingType = ChattingType.NoticeChat;
    }
    
    public override bool SetChatMessage(string msgData)
    {
        if (string.IsNullOrEmpty(msgData))
        {
            return false;
        }

        JsonData msg = JsonMapper.ToObject(msgData);
        switch (TextManager.Instance.eLanguage)
        {
            case TextManager.LANGUAGE_TYPE.KOREAN:
                strMessage = msg["kr_message"].ToString();
                break;
            case TextManager.LANGUAGE_TYPE.ENGLISH:
                strMessage = msg["en_message"].ToString();
                break;
            case TextManager.LANGUAGE_TYPE.JAPANESE:
                strMessage = msg["jp_message"].ToString();
                break;
            default:
                return false;
        }

        if (string.IsNullOrEmpty(strMessage))
            return false;

        return true;
    }
}
// 장비 채팅 메시지
public class EquipChatMessage : BaseMessage
{
    protected ChatEquipInfo cChatEquipInfo;
    public ChatEquipInfo ChattingEquipInfo { get { return cChatEquipInfo; } }

    public EquipChatMessage()
    {
        eChattingType = ChattingType.EquipChat;
    }

    public bool IsEquipMessage()
    {
        if(cChatEquipInfo == null)
        {
            return false;
        }

        return true;
    }
    
    public override bool SetChatMessage(string msg)
    {
        cChatEquipInfo = new ChatEquipInfo(JsonMapper.ToObject(msg));
        EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(cChatEquipInfo.equipID);
        if (equipInfo == null)
        {
            return false;
        }

        string tier = TextManager.Instance.GetText(string.Format("forge_level_{0}", cChatEquipInfo.smithyLV));
        int lastIdx = tier.LastIndexOf("<");
        string equipMsg = String.Format("{0} ★{1} {2}</color>",
            lastIdx < 0 ? tier : tier.Substring(0, lastIdx),
            cChatEquipInfo.starCount,
            TextManager.Instance.GetText(equipInfo.sName));

        strMessage = string.Format(TextManager.Instance.GetText("chat_text_color_equip"), cChatEquipInfo.creatorName, equipMsg);
        return true;
    }

    public override string GetSubChatMsg()
    {        
        return strMessage;
    }
}
// 길드 채팅 메시지
public class GuidChatMessage : BaseMessage
{
    public GuidChatMessage()
    {
        eChattingType = ChattingType.GuidChat;
    }

    public override bool SetChatMessage(string sendNane, string msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            return false;
        }
        strMessage = string.Format(TextManager.Instance.GetText("chat_text_color_user"), sendNane, msg);

        return true;
    }
}
// 채팅에 장비 정보
public class ChatEquipInfo
{
    public UInt16 equipID;
    public string creatorName;
    public Byte smithyLV;
    public Byte starCount;
    public UInt16[] statusPonits;
    public Byte[] skillSlot;

    public ChatEquipInfo(JsonData equipInfo)
    {
        equipID = UInt16.Parse(equipInfo["itemid"].ToString()); // 아이템 아이디
        creatorName = equipInfo["creater"].ToString();          // 만든 사람 닉네임
        smithyLV = Byte.Parse(equipInfo["smithinglevel"].ToString());   // 제작 레벨 (티어)
        starCount = Byte.Parse(equipInfo["completeness"].ToString());   // 완성도 ( 성)
        statusPonits = new ushort[] {
                UInt16.Parse(equipInfo["stat1"].ToString()),        // 스텟 포인트 1
                UInt16.Parse(equipInfo["stat2"].ToString()),        // 스텟 포인트 2
                UInt16.Parse(equipInfo["stat3"].ToString())         // 스텟 포인트 3
            };
        skillSlot = new byte[] {
                Byte.Parse(equipInfo["skill1slot"].ToString()),     // 스킬 정보
                Byte.Parse(equipInfo["skill2slot"].ToString()),     // 스킬 정보
                Byte.Parse(equipInfo["skill3slot"].ToString())      // 스킬 정보
            };
    }
}
#endregion

public class ChattingManager : MonoBehaviour, IChatClientListener
{
    public enum ChattingTabType
    {
        NormalTab,     // 일반
        GuidTab,       // 길드
        NoticeTab,     // 공지
        Max,
    }

    private UInt16 MAX_LINE_COUNT;                      // 채팅 메인 라인
    private const Byte MAX_MESSAGE_LENGTH = 100;        // 메시지 최대 길이

    public const string MSG_TYPE = "type";             // 메시지 타임 Key
    public const string NICK_NAME = "nickname";        // 유저 닉네임 key
    public const string MSG = "msg";                    // 채팅 메시지 key
    
    public const string USER_MSG = "user";             // 채팅 타입 [일반]
    public const string EQUIP_MSG = "eq";              // 채팅 타입 [장비]
    public const string NOTICE_MSG = "gm";             // 채팅 타입 [공지]
    public const string GUID_MSG = "guid";             // 채팅 타입 [길드]

    //메시지 컬러
    private Color[] MESSAGE_COlOR = {
        Color.white,                    // [일반]화이트
        new Color32(255, 255, 0, 255),  // [?]옐로우
        new Color32(255, 165, 0, 255),  // [시스템]오렌지
        new Color32(0,255,255,255)      // [길드]하늘색
    };

    // 포톤 채팅 변수
    private ChatClient chatClient;                  // 채팅 클라이언트
    private ChatChannel cCurrentChannel;            // 현재 채팅 채널
    private bool isConnectChatting;                 // 채팅 서버와의 연결 여부
    private bool isReConnect;

    // 서브 채팅창 관련 변수 및 컴포넌트
    //public Text subChatText;                        // 서브 채팅창
    //private RectTransform trSubChatBG;              // 서브 채팅창 BG RectTransform
    //private float fBaseSubChatSizeX;                // 서브 채팅창 BG 사이즈

    private SubChatting cSubChatting;

    // 메인 채팅창 관련 변수 및 컴포넌트
    public GameObject mainChatWindow;               // 메인 채팅 창
    public Text textOpenChannelName;                // 채팅 채널 이름
    public Button[] chatTabBtnList;                 // 채팅 탭 버튼 리스트
    public Sprite[] tabBtnSprite;                   // 탭 버튼 이미지 0 : OFF / 1 : ON
    public RectTransform mainChatTextParent;        // 메인 채팅 텍스트 부모
    public InputField chatInputField;               // 채팅 입력
    
    private int selectedTabBtnIdx;                  // 선택되어 있는 탭 버튼의 인덱스

    private Dictionary<ChattingTabType, List<BaseMessage>> dicMessageContainer;

    // 채널 관련 팝업 변수 및 컴포넌트
    public GameObject objChannelPopup;          // 채널 팝업
    public InputField inputChannel;             // 채널 입력
    public Text txtChannelPopupMsg;             // 채널 팝업 메시지
    public Text txtChannelErrorMsg;             // 채널 팝업 에러 메시지
    private bool isChangingChannel;             // 채널 변경 중 여부
    private UInt16 u2CurChannelIdx;             // 현재 채널 인덱스
    private string strGuildChannelID;           // 길드 채팅 채널 ID

    // 공지 관련 변수 및 컴포넌트
    public Toggle toggleEquipNotice;            // 제작 알림
    private bool isEquipNotice;                 // 제작 알림 여부
    public GameObject objNoticeWindow;          // 공지 창
    public Text txtNoticeMessage;               // 공지 메시지
    private RectTransform trNoticeMsg;          // 공지 메시지 TR

    private Queue<BaseMessage> noticeMessage;   // 공지 메시지
    private float noticeWindowWidth;            // 공지 창 가로 크기
    private bool isMsgMoving;                   // 공지 메시지 움직임 여부

    // 장비 팝업
    public ChatEquipPopup cEquipPopup;          // 장비 팝업

    private bool chattingActive = false; //[임시 변수] 채팅 연결 여부를 셋팅하여 false시 채팅 서버 연결 및 기능을 제한한다
    public bool ChattingActive { get { return chattingActive; } }

    private void Awake()
    {
        isEquipNotice = ObscuredPrefs.GetBool("isEquipNotice", true);
        if (ChattingActive)
        {
            MAX_LINE_COUNT = (UInt16)mainChatTextParent.childCount;
            trNoticeMsg = txtNoticeMessage.GetComponent<RectTransform>();
            noticeWindowWidth = objNoticeWindow.GetComponent<RectTransform>().sizeDelta.x;
            Init();
        }
    }
    private void Start()
    {
        if (ChattingActive)
        {
            chatClient = new ChatClient(this);
            chatClient.ChatRegion = "ASIA";
            chatClient.MessageLimit = 1;    // 채팅 클라이언트에 메시지를 저장량을 제한한다
                                            // Set your favourite region. "EU", "US", and "ASIA" are currently supported.
            //ChattingServersConnect();     // 타이틀씬에서 접속시 Asset 받기전에는 시스템 메시지 Text를 찾지 못해서 키값이 바로 노출 됨
        }
    }
    private void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }
    private void OnApplicationQuit()
    {
        if (chatClient != null)
        {
            OnDisconnect();
        }
    }

    // 초기화
    private void Init()
    {
        if(dicMessageContainer == null)
        {
            dicMessageContainer = new Dictionary<ChattingTabType, List<BaseMessage>>();
        }
        // 채팅 리스트 초기화
        for (ChattingTabType i = 0; i < ChattingTabType.Max; ++i)
        {
            if(dicMessageContainer.ContainsKey(i))
            {
                dicMessageContainer[i].Clear();
            }
            else
            {
                dicMessageContainer.Add(i, new List<BaseMessage>());
            }
        }

        if (noticeMessage == null)
            noticeMessage = new Queue<BaseMessage>();
        else
            noticeMessage.Clear();

        strGuildChannelID = null;
        selectedTabBtnIdx = -1;
        u2CurChannelIdx = 0;

        // 채팅 연결 여부 초기화
        isConnectChatting = false;
        isChangingChannel = false;
        isReConnect = false;

        ClickTabBtn((int)ChattingTabType.NormalTab);// 일반

        CloseMainChattingWindow();
        CloseChangeChannelPopup();
        CloseNoticeWindow();
    }
    // 채팅서버 연결 끊기
    public void OnDisconnect()
    {
        if(isConnectChatting)
        {
            chatClient.Disconnect();
            Init();
        }
    }
    // 채팅 서버 접속
    private void ChattingServersConnect()
    {
        // 채팅 서버가 연결이 되어 있지 않다면 연결
        if(!isConnectChatting)
        { 
            chatClient.Connect(ChatSettings.Instance.AppId, "1.0", new AuthenticationValues(ObscuredPrefs.GetString("guestID")));
        }
    }
    
    // 메시지 보내기
    public void SendUserChatting(string msg)
    {
        if (cCurrentChannel == null || !isConnectChatting)
        {
            ChattingServersConnect();
            return;
        }

        if (chatInputField.text.Length > 0)
        {
            string message = null;
            if(chatInputField.text.Length > MAX_MESSAGE_LENGTH)
            {
                message = chatInputField.text.Substring(0, MAX_MESSAGE_LENGTH);
            }
            else
            {
                message = chatInputField.text;
            }
            switch ((ChattingTabType)selectedTabBtnIdx)
            {
                // 유저 채팅 메시지 보내기
                case ChattingTabType.NormalTab:
                    {
                        JsonData chatMsg = new JsonData();
                        chatMsg[MSG_TYPE] = USER_MSG;
                        chatMsg[NICK_NAME] = Legion.Instance.sName;
                        chatMsg[MSG] = message;

                        chatClient.PublishMessage(cCurrentChannel.Name, chatMsg.ToJson());
                    }
                    break;
                // 길드 채팅 메시지 보내기
                case ChattingTabType.GuidTab:
                    {
                        if (!string.IsNullOrEmpty(strGuildChannelID))
                        {
                            JsonData chatMsg = new JsonData();
                            chatMsg[MSG_TYPE] = GUID_MSG;
                            chatMsg[NICK_NAME] = Legion.Instance.sName;
                            chatMsg[MSG] = message;

                            chatClient.PublishMessage(strGuildChannelID, chatMsg.ToJson());
                        }
                    }
                    break;
            }
            chatInputField.text = "";
        }
    }
    // 받은 메시지 셋팅
    private void SetReceiveMessage(string messageData)
    {
        JsonData msgData = JsonMapper.ToObject(messageData);
        BaseMessage msg = null;
        // 메시지 타입에 맞도록 생성
        switch (msgData[MSG_TYPE].ToString())
        {
            case USER_MSG:
                msg = new UserChatMessage();
                break;
            case GUID_MSG:
                msg = new GuidChatMessage();
                break;
            case NOTICE_MSG:
                msg = new NoticeChatMessage();
                break;
            case EQUIP_MSG:
                // 장비메시지 보지 않기라면 아예 셋팅조차 하지 않는다
                if (!isEquipNotice)
                {
                    return;
                }

                msg = new EquipChatMessage();
                break;
        }

        // 메시지가 셋팅 되어있다면
        if ( msg != null)
        {
            switch(msg.EChattingType)
            {
                case BaseMessage.ChattingType.UserChat:
                case BaseMessage.ChattingType.GuidChat:
                    {
                        // 유저 채팅 이거나 길드 채팅일 경우 닉네임도 같이 셋팅해야 함
                        if (!msg.SetChatMessage(msgData[NICK_NAME].ToString(), msgData[MSG].ToString()))
                            return;
                    }
                    break;
                case BaseMessage.ChattingType.EquipChat:
                case BaseMessage.ChattingType.NoticeChat:
                    {
                        // 별도의 셋팅이 있음
                        if (!msg.SetChatMessage(msgData[MSG].ToJson()))
                            return;
                    }
                    break;
                default:
                    {
                        if (!msg.SetChatMessage(msgData[MSG].ToString()))
                            return;
                    }
                    break;
            }
            
            if (!string.IsNullOrEmpty(msg.Message))
            {
                AddChattingMessage(msg);
            }
        }
    }
    // 시스템 메시지 추가
    private void AddSystemMessage(string msg)
    {
        if (string.IsNullOrEmpty(msg))
            return;

        SystemChatMessage chatMsg = new SystemChatMessage();
        chatMsg.SetChatMessage(msg);

        AddChattingMessage(chatMsg);
    }
    // 채팅 메시지 추가 및 출력
    private void AddChattingMessage(BaseMessage msg)
    {
        SaveChatMessage(ChattingTabType.NormalTab, msg);
        
        // 메시지 타입에 따라 셋팅한다
        switch (msg.EChattingType)
        {
            case BaseMessage.ChattingType.UserChat:
            case BaseMessage.ChattingType.SystemChat:
                // 일반 메시지 탭 일때에만 메시지 창에 출력한다
                if (IsOpenMainChattingWindow() && selectedTabBtnIdx == (int)ChattingTabType.NormalTab)
                    MainChatWindowOutput(msg);
                break;
            //#Guild
            case BaseMessage.ChattingType.GuidChat:
                SaveChatMessage(ChattingTabType.GuidTab, msg);
                // 일반 메시지 탭이 공지 상태태가 아니라면 출력한다
                if (selectedTabBtnIdx != (int)ChattingTabType.NoticeTab)
                    MainChatWindowOutput(msg);
                break;
            default:
                AddNoticeMessage(msg);
                SaveChatMessage(ChattingTabType.NoticeTab, msg);
                if(IsOpenMainChattingWindow())
                    MainChatWindowOutput(msg);
                break;
        }
        
        // 서브 채팅창이 오픈되어 있다면
        if (IsOpenSubChatwindow())
            SubChatWindowOutput(msg);
    }
    // 채팅 타입에 따른 텍스트 옵션 변경
    private void SetTextOption(BaseMessage.ChattingType chatType, ref Text outputText)
    {
        if(outputText == null)
        {
            return;
        }
        int color = 0;
        switch (chatType)
        {
            case BaseMessage.ChattingType.UserChat:
                outputText.supportRichText = false;
                break;
            case BaseMessage.ChattingType.GuidChat:
                outputText.supportRichText = false;
                color = 3;
                break;
            case BaseMessage.ChattingType.NoticeChat:
                outputText.supportRichText = false;
                color = 1;
                break;
            case BaseMessage.ChattingType.SystemChat:
                outputText.supportRichText = false;
                color = 2;
                break;
            case BaseMessage.ChattingType.EquipChat:
                outputText.supportRichText = true;
                break;
        }

        outputText.color = MESSAGE_COlOR[color];
    }
    // 채팅 메시지 저장
    private void SaveChatMessage(ChattingTabType chatType, BaseMessage msg)
    {
        if (!dicMessageContainer.ContainsKey(chatType))
        {
            dicMessageContainer.Add(chatType, new List<BaseMessage>());
        }

        dicMessageContainer[chatType].Add(msg);
        if (GetChatMessageCount(chatType) > MAX_LINE_COUNT)
        {
            dicMessageContainer[chatType].RemoveAt(0);
        }
    }
    // 메시지 겟수 얻기
    private int GetChatMessageCount(ChattingTabType chatType)
    {
        if(!dicMessageContainer.ContainsKey(chatType))
        {
            return 0;
        }

        return dicMessageContainer[chatType].Count;
    }

    public void OnClickEquipNoticeToggle(bool isOn)
    {
        isEquipNotice = toggleEquipNotice.isOn;

        ObscuredPrefs.SetBool("isEquipNotice", isEquipNotice);
    }
    // 장비 메시지 클릭 이벤트
    public void OnClickEquipMsg(EquipChatMessage chatMsg)
    {
        if (!chatMsg.IsEquipMessage())
        {
            return;
        }

        cEquipPopup.gameObject.SetActive(true);
        cEquipPopup.SetEquipPopup(chatMsg.ChattingEquipInfo);

        PopupManager.Instance.AddPopup(cEquipPopup.gameObject, cEquipPopup.OnClickClose);
    }

#region Guild Chatting Funtion
    //#Guild
    public void GuildChatInit()
    {
        strGuildChannelID = null;
        // 길드 채팅 리스트 초기화
        if (dicMessageContainer.ContainsKey(ChattingTabType.GuidTab))
        {
            dicMessageContainer[ChattingTabType.GuidTab].Clear();
        }
        else
        {
            dicMessageContainer.Add(ChattingTabType.GuidTab, new List<BaseMessage>());
        }
        //#Guild
        if (IsOpenMainChattingWindow())
        {
            ClickTabBtn((int)ChattingTabType.NormalTab);
            // 길드에 가입 여부에 따라 길드 탭 표시를 한다
            if (GuildInfoMgr.Instance.u8GuildSN == 0)
            {
                chatTabBtnList[(int)ChattingTabType.GuidTab].gameObject.SetActive(false);
            }
            else
            {
                chatTabBtnList[(int)ChattingTabType.GuidTab].gameObject.SetActive(true);
            }
        }
    }
    // 길드 채팅 연결
    public void RequestGuildChannel()
    {
        // 길드가 없으니 연결할 필요가 없음
        if (GuildInfoMgr.Instance.u8GuildSN == 0 || !string.IsNullOrEmpty(strGuildChannelID))
        {
            return;
        }
        
        strGuildChannelID = string.Format("guild_{0}_{1}", Legion.Instance.u2LastLoginServer.ToString("D4"), GuildInfoMgr.Instance.u8GuildSN);
        if (chatClient.Subscribe(new String[] { strGuildChannelID }))
        {
            DebugMgr.LogError(string.Format("{0} 길드 채널 연결 요청 시도 성공", strGuildChannelID));
        }
    }
    // 길드 채팅 연결 끊기
    public void GuildChannelDisconnected()
    {
        if (string.IsNullOrEmpty(strGuildChannelID))
        {
            return;
        }

        ChatChannel chatChannel = null;
        if (chatClient.TryGetChannel(strGuildChannelID, out chatChannel))
        {
            // 채널 해제
            chatClient.Unsubscribe(new string[] { strGuildChannelID });
        }
    }
#endregion

#region Sub Chatting Windown Function
    // 서브 채팅창 셋팅
    public void SetSubChattingUI(SubChatting subChatting, ChattingTabType tabType)
    {
        if(chatClient != null)
        {
            cSubChatting = subChatting;
            cSubChatting.btnOpenMainChat.onClick.AddListener(() => OpenMainChattingWindow(tabType));

            if(!isConnectChatting)
            {
                ChattingServersConnect();
                return;
            }
            
            if (dicMessageContainer.ContainsKey(ChattingTabType.NormalTab))
            {
                int count = dicMessageContainer[ChattingTabType.NormalTab].Count;
                for (int i = cSubChatting.TxtChildCount; i > 0; --i)
                {
                    if (count >= i)
                    {
                        SubChatWindowOutput(dicMessageContainer[ChattingTabType.NormalTab][count - i]);
                    }
                }
            }
        }
    }
    // 서브 채팅창 오픈 여부
    public bool IsOpenSubChatwindow()
    {
        if (cSubChatting == null)
        {
            return false;
        }

        return cSubChatting.gameObject.activeSelf;
    }
    // 서브 채팅창 오픈
    public void OpenSubChatWindow()
    {
        if (cSubChatting == null)
        {
            return;
        }

        cSubChatting.gameObject.SetActive(true);
    }
    // 서브 채팅창 닫기
    public void CloseSubChatWindow()
    {
        if (cSubChatting == null)
        {
            return;
        }

        cSubChatting.gameObject.SetActive(false);
    }
    // 서브 채팅창 해제
    public void ReleaseSubChttting()
    {
        if(cSubChatting != null)
        {
            cSubChatting.btnOpenMainChat.onClick.RemoveAllListeners();
            cSubChatting = null;
        }

        //if (subChatText != null)
        //{
        //    subChatText.GetComponent<Button>().onClick.RemoveAllListeners();
        //    subChatText = null;
        //    trSubChatBG = null;
        //}
    }
    // 서브 채팅창 메시지 출력
    public void SubChatWindowOutput(BaseMessage msg)
    {
        if(msg == null || cSubChatting == null)
        {
            return;
        }

        cSubChatting.AddSubChattingMsg(msg);
        SetTextOption(msg.EChattingType, ref cSubChatting.txtChileText);

        //SetTextOption(msg.EChattingType, ref subChatText);
        //subChatText.text = msg.GetSubChatMsg();
        //
        //// 서브 채팅창 사이즈 조절
        //if (fBaseSubChatSizeX < subChatText.preferredWidth + 20)
        //    trSubChatBG.sizeDelta = new Vector2(subChatText.preferredWidth + 20, trSubChatBG.sizeDelta.y);
        //else
        //    trSubChatBG.sizeDelta = new Vector2(fBaseSubChatSizeX, trSubChatBG.sizeDelta.y);
    }
#endregion

#region Main Chatting Windown Function
    // 메인 채팅창 오픈 여부
    public bool IsOpenMainChattingWindow()
    {
        if (mainChatWindow == null)
        {
            return false;
        }

        return mainChatWindow.activeSelf;
    }
    // 메인 채팅창 오픈
    public void OpenMainChattingWindow(ChattingTabType tabType)
    {
        // 메인채팅 창이 없거나 오픈했다면
        if (IsOpenMainChattingWindow())
        {
            return;
        }

        // 크루를 만들지 않았다면 메인채팅창을 오픈 할 수 없도록 한다
        if (Legion.Instance.sName == "" || Legion.Instance.sName == null)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("chat_text_inactivation"), null);
            return;
        }

        //#Guild
        // 길드에 가입 여부에 따라 길드 탭 표시를 한다
        if(GuildInfoMgr.Instance.u8GuildSN == 0)
        {
            chatTabBtnList[(int)ChattingTabType.GuidTab].gameObject.SetActive(false);
            ClickTabBtn((int)ChattingTabType.NormalTab);
        }
        else
        {
            chatTabBtnList[(int)ChattingTabType.GuidTab].gameObject.SetActive(true);
            RequestGuildChannel();
            ClickTabBtn((int)tabType);
        }

        PopupManager.Instance.AddPopup(mainChatWindow, CloseMainChattingWindow);
        
        //ClickTabBtn((int)ChattingTabType.NormalTab);
        textOpenChannelName.text = string.Format("{0} {1}", TextManager.Instance.GetText("chat_channel"), u2CurChannelIdx.ToString());

        toggleEquipNotice.isOn = isEquipNotice;

        mainChatWindow.SetActive(true);
        SetMainChatWindonw();
    }
    // 메인 채팅창 닫기
    public void CloseMainChattingWindow()
    {
        // 메인 채팅창이 활성화 되어 있지 않다면
        if (!mainChatWindow.activeSelf)
        {
            return;
        }

        PopupManager.Instance.RemovePopup(mainChatWindow);
        mainChatWindow.SetActive(false);
    }
    // Tab 버튼 클릭
    public void ClickTabBtn(int btnIdx)
    {
        if (selectedTabBtnIdx == btnIdx)
        {
            return;
        }
        if (selectedTabBtnIdx >= 0)
        {
            chatTabBtnList[selectedTabBtnIdx].image.sprite = tabBtnSprite[0];
        }

        chatInputField.text = "";
        if (btnIdx == (int)ChattingTabType.NoticeTab)
        {
            chatInputField.gameObject.SetActive(false);
        }
        else
        {
            chatInputField.gameObject.SetActive(true);
        }
        chatTabBtnList[btnIdx].image.sprite = tabBtnSprite[1];
        // 기존 탭 종료 이벤트
        selectedTabBtnIdx = btnIdx;
        // 오픈 이벤트
        SetMainChatWindonw();
    }
    // 메인 채팅창 셋팅
    public void SetMainChatWindonw()
    {
        // 메인 채팅창이 비활성화 상태라면 채팅을 작동하지 않는다
        if (!mainChatWindow.activeSelf)
        {
            return;
        }

        // 비활성화
        for (int i = 0; i < MAX_LINE_COUNT; ++i)
        {
            mainChatTextParent.GetChild(i).gameObject.SetActive(false);
        }

        // 채팅의 갯수를 확인한다
        int count = GetChatMessageCount((ChattingTabType)selectedTabBtnIdx);
        if(count > 0)
        {
            List<BaseMessage> msgList = dicMessageContainer[(ChattingTabType)selectedTabBtnIdx];
            for (int i = 0; i < count; ++i)
            {
                MainChatWindowOutput(msgList[i]);
            }
        }
        mainChatTextParent.anchoredPosition3D = Vector3.zero;
    }
    // 메인 채팅창 출력
    public void MainChatWindowOutput(BaseMessage chatMsg)
    {
        GameObject obj = mainChatTextParent.GetChild(0).gameObject;
        if (!obj.activeSelf)
        {
            obj.SetActive(true);
        }

        Button btn = obj.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        if (chatMsg.EChattingType == BaseMessage.ChattingType.EquipChat)
        {
            btn.onClick.AddListener(() => OnClickEquipMsg(chatMsg as EquipChatMessage));
            btn.interactable = true;
        }
        else
        {
            btn.interactable = false;
        }

        Text mainChatText = obj.GetComponent<Text>();

        SetTextOption(chatMsg.EChattingType, ref mainChatText);
        mainChatText.text = chatMsg.Message;

        obj.transform.SetAsLastSibling();
    }
#endregion

#region Channel Change Funtion
    // 채널 변경 팝업 열기
    public void OpneChangeChannelPopup()
    {
        if (objChannelPopup == null)
        {
            return;
        }

        inputChannel.text = "";
        txtChannelErrorMsg.gameObject.SetActive(false);

        txtChannelPopupMsg.text = string.Format(TextManager.Instance.GetText("chat_enter_channel"), SocialInfo.Instance.dicSocialInfo[1].u2ChatChaneelMax);
        objChannelPopup.SetActive(true);
        PopupManager.Instance.AddPopup(objChannelPopup, CloseChangeChannelPopup);
    }
    // 채널 변경 팝업 닫기
    public void CloseChangeChannelPopup()
    {
        // 채널 변경 팝업이 활성화 되어 있지 않다면
        if (!objChannelPopup.activeSelf)
        {
            return;
        }

        PopupManager.Instance.RemovePopup(objChannelPopup);
        objChannelPopup.SetActive(false);
    }
    // 채팅 채널 변경 클릭 이벤트
    public void OnClickChangeChatChannel()
    {
        // 채널 변경중 이거나 이동 채널을 입력하지 않은 상황에서
        if (isChangingChannel || inputChannel.text.Length <= 0)
        {
            return;
        }

        UInt16 changeChannelIdx = Convert.ToUInt16(inputChannel.text);
        // 같은 채널 선택시 팝업을 종료 할 뿐
        if (u2CurChannelIdx == changeChannelIdx)
        {
            CloseChangeChannelPopup();
            return;
        }
        else if (changeChannelIdx > SocialInfo.Instance.dicSocialInfo[1].u2ChatChaneelMax)
        {
            txtChannelErrorMsg.gameObject.SetActive(true);
            txtChannelErrorMsg.text = TextManager.Instance.GetText("chat_text_channel_over");
            return;
        }

        isReConnect = false;
        RequestOpenChannelChanage(changeChannelIdx);
    }
    // 채팅 서버 변경 요청
    private void RequestOpenChannelChanage(UInt16 channelIdx)
    {
        // 이미 변경중이라면
        if (isChangingChannel)
        {
            return;
        }

        string changeChannelID = string.Format(("{0}_{1}"), Legion.Instance.u2LastLoginServer.ToString("D4"), channelIdx.ToString("D4"));
        if (chatClient.Subscribe(new String[] { changeChannelID ,}))
        {
            u2CurChannelIdx = channelIdx;
            isChangingChannel = true;
            DebugMgr.LogError(string.Format("{0} 채널 연결 요청 시도 성공", changeChannelID));
        }
    }
#endregion

#region Notice Message Function
    // 공지 메시지 추가
    private void AddNoticeMessage(BaseMessage msg)
    {
        if (noticeMessage == null)
            noticeMessage = new Queue<BaseMessage>();

        noticeMessage.Enqueue(msg);
        if (objNoticeWindow.activeSelf == false)
        {
            OpenNoticeWindow();
        }
    }
    // 공지 안내창 오픈
    public void OpenNoticeWindow()
    {
        // 공지 내용이 없다면 창을 오픈 하지 않는다
        if (noticeMessage.Count <= 0)
        {
            objNoticeWindow.SetActive(false);
            return;
        }

        objNoticeWindow.SetActive(true);
        if (!isMsgMoving)
        {
            SetNoticeMessage();
            StartCoroutine(MoveNoticeMsg());
        }
    }
    // 공지 안내창 닫음
    public void CloseNoticeWindow()
    {
        if (!objNoticeWindow.activeSelf)
        {
            return;
        }

        isMsgMoving = false;
        StopCoroutine(MoveNoticeMsg());
        objNoticeWindow.SetActive(false);
    }
    // 공지 메시지 셋팅
    private void SetNoticeMessage()
    {
        BaseMessage msg = noticeMessage.Dequeue();

        SetTextOption(msg.EChattingType, ref txtNoticeMessage);
        txtNoticeMessage.text = msg.Message;
    }
    // 공지 메시지 움직임
    private IEnumerator MoveNoticeMsg()
    {
        int turnCount = 0;
        UInt16 maxRepeatCount = SocialInfo.Instance.dicSocialInfo[1].u2NoticeRepeatCount;
        isMsgMoving = true;
        trNoticeMsg.anchoredPosition3D = new Vector3(noticeWindowWidth, trNoticeMsg.anchoredPosition3D.y, 0);
        Vector3 pos = trNoticeMsg.anchoredPosition3D;
        while (true)
        {
            if ((trNoticeMsg.anchoredPosition3D.x * -1) <= trNoticeMsg.sizeDelta.x)
            {
                pos.x = (50.0f * Time.deltaTime);
                trNoticeMsg.anchoredPosition3D -= pos;
            }
            else
            {
                if (++turnCount < maxRepeatCount)
                {
                    pos = trNoticeMsg.anchoredPosition3D = new Vector3(noticeWindowWidth, trNoticeMsg.anchoredPosition3D.y, 0);
                }
                else
                {
                    if (noticeMessage.Count > 0)
                    {
                        turnCount = 0;
                        SetNoticeMessage();
                        pos = trNoticeMsg.anchoredPosition3D = new Vector3(noticeWindowWidth, trNoticeMsg.anchoredPosition3D.y, 0);
                    }
                    else
                    {
                        CloseNoticeWindow();
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }
#endregion

#region 포톤 콜빽 함수
    // 에러가 낫을 경우
    public void DebugReturn(DebugLevel level, string message)
    {
        DebugMgr.LogError(string.Format("debugLv : {0}, message : {1}", level, message));
    }
    // 채팅 서버와 연결을 해제 했을때
    public void OnDisconnected()
    {
        isChangingChannel = false;
        isConnectChatting = false;
        strGuildChannelID = null;
        cCurrentChannel = null;
        if (isReConnect)
        {
            ChattingServersConnect();
        }
    }
    // 채팅 서버에 연결 성공 했을때
    public void OnConnected()
    {
        isConnectChatting = true;
        RequestOpenChannelChanage((UInt16)ObscuredPrefs.GetUInt("ChattingServerIdx", 1));
        //#Guild
        // 길드 채팅 서버에 연결한다
        RequestGuildChannel();
    }
    // 채널 메시지 받을때
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        // 현재 채널과 값은 이름이라면
        if (cCurrentChannel.Name.Equals(channelName) || strGuildChannelID.Equals(channelName)) // || (길드에 가입 여부 && 길드 챗과 같은 채널의 이름인지)
        {
            for (int i = 0; i < senders.Length; ++i)
            {
                SetReceiveMessage(messages[i].ToString());
            }
        }
    }
    // 채팅 채널의 접속 햇을때
    public void OnSubscribed(string[] channels, bool[] results)
    {
        for(int i =0; i < channels.Length; ++i)
        {
            //#Guild
            // 길드 채팅이라면 넘긴다
            if (strGuildChannelID != null && strGuildChannelID.Equals(channels[i]))
            {
                continue;
            }

            // 기존 채팅 채널이 존재 하면 채널 연결 해제
            if (cCurrentChannel != null)
            {
                // 채널 해제
                chatClient.Unsubscribe(new string[] { cCurrentChannel.Name } );
            }
            
            ObscuredPrefs.SetUInt("ChattingServerIdx", u2CurChannelIdx);
            // 현재 채널 ID 저장
            textOpenChannelName.text = string.Format("{0} {1}", TextManager.Instance.GetText("chat_channel"), u2CurChannelIdx.ToString());
            CloseChangeChannelPopup();
            isChangingChannel = false;

            chatClient.TryGetChannel(channels[i], false, out cCurrentChannel);
            // 채널 변경 시스템 메시지를 추가한다
            if(!isReConnect)
            {
                AddSystemMessage(string.Format(TextManager.Instance.GetText("chat_text_color_channel"), u2CurChannelIdx));
            }
            isReConnect = true;
        }
    }
    // 채팅 채널의 접속을 끊을때
    public void OnUnsubscribed(string[] channels)
    {
        for (int i = 0; i < channels.Length; ++i)
        {
            if(GuildInfoMgr.Instance.u8GuildSN == 0 && channels[i].CompareTo(strGuildChannelID) == 0)
            {
                GuildChatInit();
            }
            DebugMgr.LogError(string.Format("채팅 채널 해제 ID = {0}", channels[i]));
        }
    }
    public void OnChatStateChange(ChatState state)
    {
        DebugMgr.LogError(state);
        //if(state == ChatState.Disconnected)
        //{
        //    isReConnect = true;
        //    isConnectChatting = false;
        //    ChattingServersConnect();
        //}
    }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        DebugMgr.LogError(string.Format("OnStatusUpdate : user = {0}, status = {1}, gotMessage = {2}, message = {3} ", user, status, gotMessage, message));
    }
    // 개인 메시지를 받을때
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //if(string.IsNullOrEmpty(strGuildChannelID))
        //{
        //    return;
        //}
        //
        //JsonData data = JsonMapper.ToObject(message.ToString());
        //if (data[MSG_TYPE].ToString().CompareTo("gb") == 0 &&
        //    ObscuredPrefs.GetString("guestID").CompareTo(data[MSG].ToString()) == 0)
        //{
        //    GuildInfoMgr.Instance.InitUserData();
        //    GuildChannelDisconnected();
        //    GuildScene guildScene = Scene.GetCurrent() as GuildScene;
        //    if(guildScene != null)
        //    {
        //        AssetMgr.Instance.SceneLoad("GuildScene");
        //    }
        //}
    }
#endregion
}
