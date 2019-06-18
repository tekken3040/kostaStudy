using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EventNoticeButtonList : BaseEventPopup
{
    public Button[] _aBtnList;
    public Text[] _aBtnTextList;

    public void SetBtnInfo(NoticeEventInfo[] info)
    {
        if (info == null)
            return;

        // 셋팅할 정보가 더 많으면 에러를 뱉는다
        if (_aBtnList.Length < info.Length)
        {
            //DebugMgr.LogError("정보 갯수와 버튼의 갯수가 맞지 않습니다");
            return;
        }

        // 정보 갯수가 부족해도 셋팅한다
        for(int i = 0; i < _aBtnList.Length; ++i)
        {
            _aBtnList[i].gameObject.SetActive(true);
            if (info[i].u2EventID > 0 && info[i].strLinkURL != "")
            {
                string linkURL = info[i].strLinkURL;
                _aBtnTextList[i].text = TextManager.Instance.GetText("popupbtntxt_" + info[i].u2EventID);
                _aBtnList[i].onClick.RemoveAllListeners();
                _aBtnList[i].onClick.AddListener(() => OpenLinkURL(linkURL));
            }
            else if (EventInfoMgr.Instance.dicEventPackage.ContainsKey(info[i].u2EventID) == true)
            {
                EventPackageInfo packageInfo = EventInfoMgr.Instance.dicEventPackage[info[i].u2EventID];
                _aBtnTextList[i].text = GetEventPackagePriceString(packageInfo);

                if (EventInfoMgr.Instance.CheckBuyPossible(info[i].u2EventID) == 1)
                {
                    _aBtnList[i].interactable = true;
                    _aBtnList[i].onClick.RemoveAllListeners();
                    _aBtnList[i].onClick.AddListener(() => OnClickBuyBtn(packageInfo));
                }
                else
                {
                    _aBtnList[i].interactable = false;
                }
            }
            else
            {
                EventReward eventInfo;
                if (EventInfoMgr.Instance.dicEventReward.TryGetValue(info[i].u2EventID, out eventInfo) == true)
                {
                    _aBtnTextList[i].text = GetEventBtnText((EVENT_TYPE)eventInfo.eventType);
                    _aBtnList[i].onClick.RemoveAllListeners();
                    _aBtnList[i].onClick.AddListener(() => GoToEventWindiow(eventInfo));
                }
                else
                {
                    _aBtnTextList[i].text = "";
                    _aBtnList[i].gameObject.SetActive(false);
                }
            }
        }
    }

    protected override void BuyRefiesh()
    {
        base.BuyRefiesh();
        if (_cBuyPackageInfo != null)
            _aBtnList[_cBuyPackageInfo.u1EventGroupOrder - 1].interactable = false;

        AssetMgr.Instance.InitDownloadList();
    }

    protected void OpenLinkURL(string linkURL)
    {
        Application.OpenURL(linkURL);
    }

    protected void GoToEventWindiow(EventReward eventInfo)
    {
        LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
        if (lobbyScene == null)
            return;

        lobbyScene.OpenEventWindown((EVENT_TYPE)eventInfo.eventType);
    }

    protected string GetEventBtnText(EVENT_TYPE eventType)
    {
        string text = "";
        switch(eventType)
        {
            case EVENT_TYPE.DICE:
                text = string.Format("{0} {1}",TextManager.Instance.GetText("event_title_marble"), TextManager.Instance.GetText("popup_btn_location_shortcut"));
                break;
            case EVENT_TYPE.FIRSTPAYMENT:           // 첫결재 이벤트					
            case EVENT_TYPE.ADDITIONALREWARD:       // 추가지급 이벤트
            case EVENT_TYPE.ADDITIONALREWARD_ITEM:
                text = TextManager.Instance.GetText("btn_notice_popup_shortcut_buy");
                break;
        }
        return text;
    }
}
