using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SocialInfo : Singleton<SocialInfo>
{
    //친구 관련
    public Dictionary<UInt16, SocialDictionary> dicSocialInfo;
    public Dictionary<UInt16, FriendCount> dicFriendCount;
    public Dictionary<UInt16, FriendRequestCount> dicFriendRequestCount;
    public Dictionary<UInt16, FriendInviteCount> dicFriendInviteCount;
    public Dictionary<UInt16, FriendRecommend> dicFriendRecommend;

    public Byte u1FriendCount;
    public Byte u1FriendRequestCount;
    public Byte u1FriendInviteCount;
    public Byte u1FriendRecommendCount;
    
    //우편 관련
    public Dictionary<UInt16, MailList> dicMailList;
    public Dictionary<UInt16, MailGetItem> dicMailGetItem;

    public UInt16 u2MailCount;
    public UInt16 u2ItemCount;

    //공지 관련
    public Dictionary<UInt16, SocialNotice> dicNotice;
    //public Dictionary<UInt16, SocialNoticeEvent> dicNoticeEvent;
    public List<SocialNoticeEvent> lstNoticeEventList;

    public Byte u1NoticeCount;
    public Byte u1EventPopupCount;

    private bool loadedInfo = false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}

    public void Awake()
    {
        dicFriendCount = new Dictionary<ushort, FriendCount>();
        dicFriendRequestCount = new Dictionary<ushort, FriendRequestCount>();
        dicFriendInviteCount = new Dictionary<ushort, FriendInviteCount>();
        dicFriendRecommend = new Dictionary<ushort, FriendRecommend>();

        dicMailList = new Dictionary<ushort, MailList>();
        dicMailGetItem = new Dictionary<ushort, MailGetItem>();

        dicNotice = new Dictionary<ushort, SocialNotice>();
        //dicNoticeEvent = new Dictionary<ushort, SocialNoticeEvent>();
        lstNoticeEventList = new List<SocialNoticeEvent>();
    }

    public void Init()
    {
        dicSocialInfo = new Dictionary<ushort, SocialDictionary>();
        DataMgr.Instance.LoadTable(this.AddInfo, "Social");
    }

    public void AddInfo(string[] cols)
    {
        if (cols == null)
		{
			loadedInfo = true;
			return;
		}
        SocialDictionary socialDic = new SocialDictionary();
        socialDic.Set(cols);
        dicSocialInfo.Add(1, socialDic);
    }

    public void ClearSocialInfo()
    {
        dicFriendCount.Clear();
        dicFriendRequestCount.Clear();
        dicFriendInviteCount.Clear();
        dicFriendRecommend.Clear();
        dicMailList.Clear();
        dicMailGetItem.Clear();
        dicNotice.Clear();
        //dicNoticeEvent.Clear();
        lstNoticeEventList.Clear();
        u1FriendCount = 0;
        u1FriendRequestCount = 0;
        u1FriendInviteCount = 0;
        u1FriendRecommendCount = 0;
        u2MailCount = 0;
        u2ItemCount = 0;
        u1NoticeCount = 0;
        u1EventPopupCount = 0;
    }

    // 불필요 이벤트 팝업 걸러네기 
    public void SFluousEventNoticeFilter()
    {
        for (int i = 0; i < u1EventPopupCount;)
        {
            int btnCount = lstNoticeEventList[i].u1ButtonCount;
            // 버튼의 갯수를 확인하여 버튼이 없는 팝업은 확인하지 않는다
            if (btnCount <= 0)
            {
                ++i;
                continue;
            }

            bool isbuyCheck = false;    // 구매 가능한지 확인할 변수
            // 이벤트를 확인하여 구매 가능한 물품인지 확인한다
            // [이미 구매를 하였거나 기간이 되지 않는 상품]
            for (int j = 0; j < btnCount; ++j)
            {
                UInt16 eventID = lstNoticeEventList[i].arrsNoticeEventInfo[j].u2EventID;
                if(eventID > 0 && lstNoticeEventList[i].arrsNoticeEventInfo[j].strLinkURL != "")
                {
                    isbuyCheck = true;
                }
                else if (EventInfoMgr.Instance.dicEventPackage.ContainsKey(eventID))
                {
                    if (EventInfoMgr.Instance.CheckBuyPossible(eventID) == 1)
                        isbuyCheck = true;
                }
                else
                {
                    if (EventInfoMgr.Instance.dicEventReward.ContainsKey(eventID) == true)
                        isbuyCheck = true;
                }
            }

            // 구매 가능한 물품이 아니라면 이벤트 팝업에서 삭제한다
            if (isbuyCheck == false)
            {
                lstNoticeEventList.RemoveAt(i);
                --u1EventPopupCount;
            }
            else
                ++i;
        }

        if (lstNoticeEventList.Count > 0)
        {
            // 정렬
            lstNoticeEventList.Sort(delegate (SocialNoticeEvent cX, SocialNoticeEvent cY)
            {
                if (cX.u1PopupNo > cY.u1PopupNo) return 1;
                else if (cX.u1PopupNo < cY.u1PopupNo) return -1;
                return 0;
            });
            Legion.Instance.AddLoginPopupStep(Legion.LoginPopupStep.EVENT_NOTICE);
        }
    }
}

public class SocialDictionary
{
    public UInt16 u2FriendMax;
    private UInt16 u2FriendMaxPoint;
    private UInt16 u2FriendPointAtSending;
    public UInt16 FRIEND_POINT_SENDING
    {
        get{ return (UInt16)(u2FriendPointAtSending + LegionInfoMgr.Instance.GetCurrentVIPInfo().u2BonusFSPoint); }
    }
    public UInt16 u2FriendInviteInPage;
    public UInt16 u2FriendInviteMax;
    public UInt16 u2FriendRequestInPage;
    public UInt16 u2FriendRecommandInPage;
    public UInt16 u2MailNormalMax;
    public UInt16 u2MainCoinMax;
    public UInt16 u2MailGachaMax;
    public UInt16 u2MailAlarmMax;
    public UInt16 u2MailKeepDay;

    public UInt16 u2ChatChannelMaxUser; // 채팅 채널 최대 인원수
    public UInt16 u2NoticeRepeatCount;  // 공지 반복 표시 횟수
    public UInt16 u2ChatChaneelMax;     // 채팅 채널 갯수

    public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
        u2FriendMax = Convert.ToUInt16(cols[idx++]);
        u2FriendMaxPoint = Convert.ToUInt16(cols[idx++]);
        u2FriendPointAtSending = Convert.ToUInt16(cols[idx++]);
        u2FriendInviteInPage = Convert.ToUInt16(cols[idx++]);
        u2FriendInviteMax = Convert.ToUInt16(cols[idx++]);
        u2FriendRequestInPage = Convert.ToUInt16(cols[idx++]);
        u2FriendRecommandInPage = Convert.ToUInt16(cols[idx++]);
        u2MailNormalMax = Convert.ToUInt16(cols[idx++]);
        u2MainCoinMax = Convert.ToUInt16(cols[idx++]);
        u2MailGachaMax = Convert.ToUInt16(cols[idx++]);
        u2MailAlarmMax = Convert.ToUInt16(cols[idx++]);
        u2MailKeepDay = Convert.ToUInt16(cols[idx++]);
        idx++;

        u2ChatChannelMaxUser = Convert.ToUInt16(cols[idx++]);
        idx++;
        idx++;
        u2NoticeRepeatCount = Convert.ToUInt16(cols[idx++]);
        u2ChatChaneelMax = Convert.ToUInt16(cols[idx++]);

        return 1;
    }

    public UInt16 MAX_FRIENDPOINT
    {
        get{ return (UInt16)(u2FriendMaxPoint + LegionInfoMgr.Instance.GetVipInfo(Legion.Instance.u1VIPLevel).u2AddMaxFSPoint); }
    }

}

public class FriendCount
{
    public UInt64 u8UserSN;
    public string strLegionName;
    public UInt16 u2MainCharClassID;
    public Byte u1MainCharElement;
    public Int64 u8LastLoginTime;
    public DateTime dtLastLoginTime;
    public UInt16 u2Level;
    public Byte u1Point;
    public Byte u1SendPoint;
    public Byte u1RecvPoint;
    public Byte u1New;
    public bool bDeleted;
}

public class FriendRequestCount
{
    public UInt64 u8UserSN;
    public string strLegionName;
    public UInt16 u2MainCharClassID;
    public Byte u1MainCharElement;
    public Int64 u8LastLoginTime;
    public DateTime dtLastLoginTime;
    public UInt16 u2Level;
    public bool bDeleted;
}

public class FriendInviteCount
{
    public UInt64 u8UserSN;
    public string strLegionName;
    public UInt16 u2MainCharClassID;
    public Byte u1MainCharElement;
    public Int64 u8LastLoginTime;
    public DateTime dtLastLoginTime;
    public UInt16 u2Level;
    public Byte u1New;
    public bool bAccept;
    public bool bDeleted;
}

public class FriendRecommend
{
    public UInt64 u8UserSN;
    public string strLegionName;
    public UInt16 u2MainCharClassID;
    public Byte u1MainCharElement;
    public Int64 u8LastLoginTime;
    public DateTime dtLastLoginTime;
    public UInt16 u2Level;
}

public class MailList
{
    public UInt16 u2MailSN;
    public UInt16 u2MailTitleCode;
    public string strMainTitle;
    public Int64 u8ExpireTime;
    public DateTime dtExpireTime;
    public Byte u1New;
    public Byte u1MailType;

    public Byte u1ItemType;
    public UInt16 u2ItemID;
    public UInt32 u4Count;
    public Byte u1Grade;
    public UInt16 u2Level;

    public UInt16 u2Slot;
    public UInt16 u2MailContentCode;
    public string strMailContent;
    public bool bCheckedMail;
}

public class MailGetItem
{
    public UInt16 u2MailSN;
    public Byte u1ItemType;
    public UInt16 u2ItemID;
    public UInt32 u4Count;

    public Byte u1SmithingLevel;
    public UInt16 u2ModelID;
    public UInt16 u2Level;
    public Byte u1Completeness;
    public UInt32[] u4Stat;
    public Byte u1SkillCount;
    public Byte[] u1SkillSlot;
}

public class SocialNotice
{
    public UInt16 u2NoticeSN;
    public string strTitle;
    public Int64 u8ShowDay;
    public DateTime dtShowDay;
    public string strContent;
    public string strImageURL;
    public Byte u1ShowTime;
    public string strLinkURL;
}

/// <summary>
/// 2016.12. 22 jy
/// 이벤트 팝업 정보 
/// ID 및 URL 주소
/// </summary>
public struct NoticeEventInfo
{
	public UInt16 u2EventID;
	public string strLinkURL;
}

public class SocialNoticeEvent
{
    public UInt16 u2EventPopupSN;
	//public string strListImageURL;	// 2016. 12. 22 서버 통합되면 삭제
    public string strImageURL;
    public Byte u1ShowTime;
    //public string strLinkURL;		// 2016. 12. 22 서버 통합되면 삭제
    public Byte u1SizeType;
	public Byte u1PopupNo;
	public Byte u1ButtonCount;
	public NoticeEventInfo[] arrsNoticeEventInfo;
}