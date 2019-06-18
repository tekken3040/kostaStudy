using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;

public class TimeRewardSlot : MonoBehaviour
{
    public Text RewardValue;
    public Text TimeText;
    public Button RecvBtn;
    public GameObject RewardPopup;
    public GameObject BlackImage;
    public GameObject UiEffect;
	public Text BtnText;
    int _slotNum;
    int _slotNumMax;
    
    EventReward eventReward;
    LobbyScene _scene;
    TimeRewardPanel timePanel;

    public void SetData(int slotNum, int slotNumMax, EventReward _reward, bool _check, bool _highLight, TimeRewardPanel _timePanel)
    {
        StringBuilder tempStringBuilder = new StringBuilder();

        _slotNum = slotNum;
        _slotNumMax = slotNumMax;
        timePanel = _timePanel;
        eventReward = _reward;
        _scene = Scene.GetCurrent() as LobbyScene;
        RecvBtn.interactable = _highLight;
        UiEffect.SetActive(_highLight);
        BlackImage.SetActive(_check);

        if (_slotNum == EventInfoMgr.Instance.dicEventReward[eventReward.u2EventID].u1RewardIndex)
            StartCoroutine("CheckConnectTime");
        else
        {
            if (_check == true)
                BtnText.text = TextManager.Instance.GetText("btn_tiem_reward_done"); // 보상 완료
            else
                BtnText.text = TextManager.Instance.GetText("btn_time_reward_wating"); // 대기 중
        }

        TimeEvent timeEvent = EventInfoMgr.Instance.dicTimeEvent[eventReward.u2EventID];
        // 2016. 10. 17 jy
        // 이미지는 고정해 달라고 요청 받음
        switch (timeEvent.u1RewardItemType[_slotNum])
        {
            case (Byte)GoodsType.GOLD:
            case (Byte)GoodsType.CASH:
            case (Byte)GoodsType.KEY:
            case (Byte)GoodsType.LEAGUE_KEY:
            case (Byte)GoodsType.EQUIP_COUPON:
            case (Byte)GoodsType.MATERIAL_COUPON:
                tempStringBuilder.Append(Legion.Instance.GetConsumeString(timeEvent.u1RewardItemType[_slotNum])).Append(" X").Append(timeEvent.u4RewardItemCount[_slotNum].ToString());           
                break;
            case (Byte)GoodsType.FRIENDSHIP_POINT:
                //RewardIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_friendship");
                tempStringBuilder.Append(Legion.Instance.GetConsumeString(timeEvent.u1RewardItemType[_slotNum])).Append("\n").Append(timeEvent.u4RewardItemCount[_slotNum].ToString());
                break;
            case (Byte)GoodsType.MATERIAL:
            case (Byte)GoodsType.CONSUME:
                tempStringBuilder.Append(timeEvent.u4RewardItemCount[_slotNum].ToString());
                break;
        }
        RewardValue.text = tempStringBuilder.ToString();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        UInt16 tempTime = (UInt16)((timeEvent.u4TimePeriod * (_slotNum + 1)) / 60);
        tempStringBuilder.Append(tempTime.ToString()).Append(TextManager.Instance.GetText("mark_time_hour"));

        TimeText.text = tempStringBuilder.ToString();
    }

    public void OnClickRecvReward()
    {
		if(!Legion.Instance.CheckEmptyInven())
		{
			return;
		}

        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestEventGoodsReward(eventReward.u2EventID, RecvReward);
    }

    public void RecvReward(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.EVENT_REWARD, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            StopCheckConnectTime();

			BlackImage.SetActive(true);
			UiEffect.SetActive(false);
			RecvBtn.interactable = false;
	
			BtnText.text = TextManager.Instance.GetText("btn_tiem_reward_done");	// 보상 완료

            RewardPopup.SetActive(true);
            timePanel.StopTimeReward();
            Legion.Instance.StopConnectTimeCount();

            // 2016. 10. 27 jy 
            // 타임보상 5번째를 받게되면 오류가 발생함 다음 보상이 있는지 체크 조건 변경
            if (_slotNumMax != _slotNum + 1)
            {
                Legion.Instance.tsConnectTime = TimeSpan.FromSeconds(EventInfoMgr.Instance.dicTimeEvent[eventReward.u2EventID].u4TimePeriod * 60);
                Legion.Instance.ConnectTimeCount();
            }
            else
            {
                timePanel.SetTimeRewardEnd();

                ObscuredPrefs.SetBool("LastReward", true);
                EventInfoMgr.Instance.OnLastReward = ObscuredPrefs.GetBool("LastReward");
            }

            if (_scene != null)
                _scene._eventPanel.CheckAlarm();

        }
    }

    private IEnumerator CheckConnectTime()
    {
		BtnText.text = TextManager.Instance.GetText("btn_time_reward_ing");	// 진행 중
        while (true)
        {
            if (Legion.Instance.tsConnectTime.TotalSeconds == 0)
            {
                UiEffect.SetActive(true);
                RecvBtn.interactable = true;
                BlackImage.SetActive(false);
				BtnText.text = TextManager.Instance.GetText("btn_time_reward_get");	// 보상 받기
                yield break;
            }
            yield return StartCoroutine(Utillity.WaitForRealSeconds(1f));

        }
    }

    public void StopCheckConnectTime()
    {
        StopCoroutine("CheckConnectTime");
    }
}
