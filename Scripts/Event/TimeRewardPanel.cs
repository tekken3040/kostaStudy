using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;

public class TimeRewardPanel : MonoBehaviour
{
    public GameObject RewardPopup;
    public Image _popupTitleImg;
    public Text RewardPopupTitle;
    public Text RewardTime;
    public TimeRewardSlot[] TimeRewardSlots;
    EventReward _eventReward;
    //LobbyScene _scene;

    public void OnEnable()
    {
        EventInfoMgr.Instance.OnLastReward = ObscuredPrefs.GetBool("LastReward");
        Init();
    }

    public void OnClickPopupOK()
    {
        Init();
        RewardPopup.SetActive(false);
    }

    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(this.gameObject);
        Destroy(this.gameObject);
    }

    public void Init()
    {
        _popupTitleImg.sprite = AtlasMgr.Instance.GetSprite( string.Format("Sprites/{0}.timereward_title", TextManager.Instance.GetImagePath()) );
        _popupTitleImg.SetNativeSize();

        int timeIDCount = EventInfoMgr.Instance.lstTimeEventID.Count;
        for (int i = 0; i < timeIDCount; i++)
        {
            if(EventInfoMgr.Instance.dicEventReward.ContainsKey(EventInfoMgr.Instance.lstTimeEventID[i]))
            {
                _eventReward = EventInfoMgr.Instance.dicEventReward[EventInfoMgr.Instance.lstTimeEventID[i]];
                break;
            }
        }

        bool bCheck = false;
        bool bRecv = false;
        for (int i=0; i< TimeRewardSlots.Length; i++)
        {
            //if(_eventReward.u1RewardIndex == 0)
            //    bCheck = false;
            //else if(_eventReward.u1RewardIndex >= (i+1))
            if (_eventReward.u1RewardIndex >= (i + 1))
                bCheck = true;
            else
                bCheck = false;

            if(Legion.Instance.tsConnectTime.TotalSeconds == 0 && _eventReward.u1RewardIndex == i)
                bRecv = true;
            else
                bRecv = false;

            TimeRewardSlots[i].SetData(i, TimeRewardSlots.Length, _eventReward, bCheck, bRecv , this);
        }
        
        if (_eventReward.u1RewardIndex != TimeRewardSlots.Length)
        {
            StartTimeReward();
            ObscuredPrefs.SetBool("LastReward", false);
            EventInfoMgr.Instance.OnLastReward = ObscuredPrefs.GetBool("LastReward");
        }
        else
        {
            SetTimeRewardEnd();
        }
    }

    public void StartTimeReward()
    {
        StartCoroutine("CheckRewardTime");
    }

    public void StopTimeReward()
    {
        StopCoroutine("CheckRewardTime");
    }

    public void SetTimeRewardEnd()
    {
		RewardTime.text = TextManager.Instance.GetText("mark_today_tiem_event_clear");
    }

    private IEnumerator CheckRewardTime()
    {
        while (true)
        {
            if (Legion.Instance.tsConnectTime.TotalSeconds == 0)
            {
                RewardTime.text = TextManager.Instance.GetText("mark_time_waiting");
                yield break;
            }
            RewardTime.text = Legion.Instance.tsConnectTime.Duration().ToString();
            yield return StartCoroutine(Utillity.WaitForRealSeconds(1f));
        }
    }
}
