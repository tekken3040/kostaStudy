using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class BossRushScene : MonoBehaviour
{
    [SerializeField] GameObject StageEffect;
    [SerializeField] GameObject RewardPopup;
    [SerializeField] GameObject RewardCheckPopup;
    [SerializeField] GameObject StageInfoWindow;
    [SerializeField] Text txtProgress;
    
    [SerializeField] Vector3[] EffectPos;
    [SerializeField] Color[] rewardTxtColor;
    [SerializeField] Text[] txtToday;
    [SerializeField] Text[] txtRewardProgress;
    [SerializeField] Outline[] _txtOutline;
    [SerializeField] Toggle[] tglTopMenu;
    [SerializeField] Image[] imgRewardSlotIcon;
    [SerializeField] GameObject[] BossSlot;
    [SerializeField] RectTransform[] progressImage;

    [SerializeField]
    SubChatting _subChattingWidown; 

    List<EventDungeonStageInfo> eStageInfo;
    StringBuilder tempStringBuilder;

    Byte u1RewardIdx = 0;
    UInt32 u4RecordValue = 0;
    int selectStageIdx = 0;
    int selectRewardIdx = 0;

    private void Start()
    {
        //#CHATTING
		if(_subChattingWidown != null)
		{
            if (PopupManager.Instance.IsChattingActive())
            {
                PopupManager.Instance.SetSubChtting(_subChattingWidown);
                _subChattingWidown.gameObject.SetActive(true);
            }
            else
            {
                _subChattingWidown.gameObject.SetActive(false);
            }
        }
        //StageInfoMgr.Instance.OpenBossRush = 46;
        FadeEffectMgr.Instance.FadeIn();
        InitBossRush();
    }

    public void OnClickStage(int idx)
    {
        selectStageIdx = idx;
        EventDungeonStageInfo stageInfo = BossSlot[idx].GetComponent<BossRushSlot>().GetStageInfo;
        
        if(stageInfo != null)
        {
            PopupManager.Instance.AddPopup(StageInfoWindow, StageInfoWindow.GetComponent<BossRushStageInfoWindow>().OnClickClose);
            StageInfoWindow.SetActive(true);
            StageInfoWindow.GetComponent<BossRushStageInfoWindow>().SetData(stageInfo, idx);
        }
        else
        {
            return;
        }
    }

    public void OnClickReward(int idx)
    {
        selectRewardIdx = idx;
        if(u1RewardIdx != 0 && u1RewardIdx > selectRewardIdx)
            return;
        else
        {
            if(selectRewardIdx < progress && selectRewardIdx == u1RewardIdx)
            {
                PopupManager.Instance.ShowLoadingPopup(1);
                Server.ServerMgr.Instance.RequestEventGoodsReward(EventInfoMgr.Instance.lstBossRush[0].u2EventID, AckReward, (Byte)selectRewardIdx);
            }
            else
            {
                PopupManager.Instance.AddPopup(RewardCheckPopup, RewardCheckPopup.GetComponent<BossRushRewardCheckPopup>().OnClickClose);
                RewardCheckPopup.SetActive(true);
                RewardCheckPopup.GetComponent<BossRushRewardCheckPopup>().SetData(EventInfoMgr.Instance.lstBossRush[0], selectRewardIdx);
            }
        }
    }

    private void AckReward(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_START, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
            EventInfoMgr.Instance.u1BossRushRewardIdx = EventInfoMgr.Instance.sEventGoodsReward.u1LastRewardIndex;
            EventInfoMgr.Instance.u1BossRushProgress = (Byte)EventInfoMgr.Instance.sEventGoodsReward.u4RecordValue;
            u1RewardIdx = EventInfoMgr.Instance.u1BossRushRewardIdx;
            u4RecordValue = EventInfoMgr.Instance.u1BossRushProgress;
            imgRewardSlotIcon[selectRewardIdx].GetComponent<Button>().interactable = false;
            imgRewardSlotIcon[selectRewardIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_02.reward_get");
            imgRewardSlotIcon[selectRewardIdx].SetNativeSize();
            _txtOutline[selectRewardIdx].effectColor = rewardTxtColor[0];
            int tempPro = (int)(u4RecordValue/20f);
            if((selectRewardIdx+1) < tempPro && tempPro < imgRewardSlotIcon.Length)
                imgRewardSlotIcon[selectRewardIdx+1].sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_02.reward_open");

            for(int i=0; i<EventInfoMgr.Instance.lstBossRush[0].REWARD_STEP_IN_CNT; i++)
            {
                Legion.Instance.AddGoods(EventInfoMgr.Instance.lstBossRush[0].rewardGoods[selectRewardIdx*3 + i]);
            }
            PopupManager.Instance.AddPopup(RewardPopup, RewardPopup.GetComponent<BossRushRewardPopup>().OnClickClose);
            RewardPopup.SetActive(true);
            RewardPopup.GetComponent<BossRushRewardPopup>().SetData(EventInfoMgr.Instance.lstBossRush[0], selectRewardIdx);
        }
    }

    void InitBossRush()
    {
        tempStringBuilder = new StringBuilder();
        Legion.Instance.bAdventoStage = 0;
        eStageInfo = new List<EventDungeonStageInfo>();

        eStageInfo = EventInfoMgr.Instance.lstDungeonStage.FindAll (cs => cs.u2EventID == EventInfoMgr.Instance.lstBossRush[0].u2EventID);
        eStageInfo.Sort
        (
            delegate(EventDungeonStageInfo x, EventDungeonStageInfo y)
            {
                int compare = 0;
                compare = x.u1OpenType.CompareTo(y.u1OpenType);

                return compare;
            }
        );
        //for(int i=0; i<txtToday.Length; i++)
        //    txtToday[i].gameObject.SetActive(false);

        if(EventInfoMgr.Instance.dicEventReward.ContainsKey(EventInfoMgr.Instance.lstBossRush[0].u2EventID))
        {
            if(EventInfoMgr.Instance.dicEventReward[EventInfoMgr.Instance.lstBossRush[0].u2EventID].eventType == (Byte)EVENT_TYPE.BOSSRUSH)
            {
                //u1RewardIdx = EventInfoMgr.Instance.dicEventReward[EventInfoMgr.Instance.lstBossRush[0].u2EventID].u1RewardIndex;
                //u4RecordValue = EventInfoMgr.Instance.dicEventReward[EventInfoMgr.Instance.lstBossRush[0].u2EventID].u4RecordValue;
                u1RewardIdx = EventInfoMgr.Instance.u1BossRushRewardIdx;
                u4RecordValue = EventInfoMgr.Instance.u1BossRushProgress;
            }
        }
        else
        {
            u1RewardIdx = 0;
            u4RecordValue = 0;
        }

        int index = 0;
        int state = 0;
        for(int i=0; i<tglTopMenu.Length; i++)
        {
            state = 0;

            for(int j=0; j<tglTopMenu.Length; j++)
            {
                index = Legion.Instance.cEvent.openStageIds.FindIndex(
                    cs => cs.u2EventID == eStageInfo[i].u2EventID && cs.u2StageID == eStageInfo[i].au2StageID[j]);
                if(index >= 0)
                {
                    if(Legion.Instance.cEvent.openStageIds[index].u1Closed == 1)
                    {
                        txtToday[i].text = TextManager.Instance.GetText("event_bossrush_info_retry");
                        txtToday[i].GetComponent<Outline>().effectColor = rewardTxtColor[2];
                        state = -1;
                        break;
                    }
                    else if(Legion.Instance.cEvent.openStageIds[index].u1Closed == 2)
                    {
                        state++;
                        continue;
                    }
                    else
                        continue;
                }
                else
                {
                    continue;
                }
            }
            if(state == tglTopMenu.Length)
            {
                txtToday[i].text = TextManager.Instance.GetText("event_bossrush_stage_success");
                txtToday[i].GetComponent<Outline>().effectColor = rewardTxtColor[0];
            }
            else if(state == 0 && i>(StageInfoMgr.Instance.OpenBossRush - (int)WEEK_DAY.MONDAY))
            {
                txtToday[i].text = TextManager.Instance.GetText("event_bossrush_info_lock");
                txtToday[i].GetComponent<Outline>().effectColor = rewardTxtColor[3];
            }
            else
            {
                txtToday[i].text = TextManager.Instance.GetText("event_bossrush_info_retry");
                txtToday[i].GetComponent<Outline>().effectColor = rewardTxtColor[2];
            }
        }
        if(StageInfoMgr.Instance.OpenBossRush <= (Byte)WEEK_DAY.FRIDAY)
        {
            txtToday[StageInfoMgr.Instance.OpenBossRush - (int)WEEK_DAY.MONDAY].text = "TODAY";
            txtToday[StageInfoMgr.Instance.OpenBossRush - (int)WEEK_DAY.MONDAY].GetComponent<Outline>().effectColor = rewardTxtColor[1];
        }
        

        SetSlotData(StageInfoMgr.Instance.OpenBossRush);
        ProgressState();

        switch(StageInfoMgr.Instance.OpenBossRush)
        {
            case (Byte)WEEK_DAY.MONDAY:
                tglTopMenu[0].isOn = true;
                //txtToday[0].gameObject.SetActive(true);
                break;

            case (Byte)WEEK_DAY.TUESDAY:
                tglTopMenu[1].isOn = true;
                //txtToday[1].gameObject.SetActive(true);
                break;

            case (Byte)WEEK_DAY.WEDNESDAY:
                tglTopMenu[2].isOn = true;
                //txtToday[2].gameObject.SetActive(true);
                break;

            case (Byte)WEEK_DAY.THURSDAY:
                tglTopMenu[3].isOn = true;
                //txtToday[3].gameObject.SetActive(true);
                break;

            case (Byte)WEEK_DAY.FRIDAY:
                tglTopMenu[4].isOn = true;
                //txtToday[4].gameObject.SetActive(true);
                break;

            case (Byte)WEEK_DAY.WEEKEND:
                tglTopMenu[0].isOn = true;
                //txtToday[0].gameObject.SetActive(false);
                break;
        }
    }
    int progress = 0;
    float progress2 = 0;
    public void ProgressState()
    {
        progress = (int)(u4RecordValue/20f);
        progress2 = (float)((u4RecordValue/20f) - progress);
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(u4RecordValue).Append("%");
        txtProgress.text = tempStringBuilder.ToString();
        for(int i=0; i<txtRewardProgress.Length; i++)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(20*(i+1)).Append("%").Append(TextManager.Instance.GetText("event_bossrush_reward_complete"));
            txtRewardProgress[i].text = tempStringBuilder.ToString();
            //_txtOutline[i] = txtRewardProgress[i].GetComponent<Outline>();
        }
        for(int i=0; i<progressImage.Length; i++)
            progressImage[i].sizeDelta = new Vector2(0, progressImage[i].sizeDelta.y);
        if(progress == 0)
        {
            if(progress2 != 0)
                progressImage[0].sizeDelta = new Vector2((float)(230f*progress2), progressImage[0].sizeDelta.y);
        }
        else
        {
            for(int i=0; i<progress; i++)
                progressImage[i].sizeDelta = new Vector2((float)(230f), progressImage[progress].sizeDelta.y);
            progressImage[progress].sizeDelta = new Vector2((float)(230f*progress2), progressImage[progress].sizeDelta.y);

            for(int i=0; i<progress; i++)
            {
                imgRewardSlotIcon[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_02.reward_open");
                imgRewardSlotIcon[i].GetComponent<Button>().interactable = true;
                _txtOutline[i].effectColor = rewardTxtColor[1];
                if(i<u1RewardIdx)
                {
                    imgRewardSlotIcon[i].GetComponent<Button>().interactable = false;
                    imgRewardSlotIcon[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_02.reward_get");
                    _txtOutline[i].effectColor = rewardTxtColor[0];
                }
                else if(i > u1RewardIdx)
                {
                    imgRewardSlotIcon[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_02.reward_ready");
                }
                else
                {
                    imgRewardSlotIcon[i].GetComponent<Button>().interactable = true;
                }
                
                imgRewardSlotIcon[i].SetNativeSize();
            }
        }
    }

    public void OnClickBack()
    {
        AssetMgr.Instance.SceneLoad("LobbyScene");
    }

    public void OnClickMenu(int idx)
    {
        SetSlotData(idx);
    }

    public void SetSlotData(int idx)
    {
        if(idx == 0 || idx == (int)WEEK_DAY.WEEKEND)
            idx = (int)WEEK_DAY.MONDAY;

        for(int i=0; i<5; i++)
        {
            BossSlot[i].GetComponent<BossRushSlot>().SetData(eStageInfo[idx - (int)WEEK_DAY.MONDAY], i);
        }
        int index = 0;
        for(int i=0; i<tglTopMenu.Length; i++)
        {
            if(Legion.Instance.cEvent.openStageIds.Count == 0)
            {
                if(idx < StageInfoMgr.Instance.OpenBossRush)
                {
                    StageEffect.SetActive(false);
                }
                else if(idx > StageInfoMgr.Instance.OpenBossRush)
                {
                    StageEffect.SetActive(false);
                }
                else
                {
                    StageEffect.SetActive(true);
                    StageEffect.transform.localPosition = new Vector3(-315f, 0f, -1f);
                }
                break;
            }
            index = Legion.Instance.cEvent.openStageIds.FindIndex(
                cs => cs.u2EventID == eStageInfo[idx - (int)WEEK_DAY.MONDAY].u2EventID && 
                cs.u2StageID == eStageInfo[idx - (int)WEEK_DAY.MONDAY].au2StageID[i]);
            if(index >= 0)
            {
                if(Legion.Instance.cEvent.openStageIds[index].u1Closed == 1)
                {
                    StageEffect.SetActive(false);
                    continue;
                }
                else if(Legion.Instance.cEvent.openStageIds[index].u1Closed == 2)
                {
                    StageEffect.SetActive(false);
                    continue;
                }
                else
                {
                    StageEffect.SetActive(true);
                    StageEffect.transform.localPosition = EffectPos[i];
                    break;
                }
            }
            else
            {
                if(idx == StageInfoMgr.Instance.OpenBossRush)
                {
                    StageEffect.SetActive(true);
                    StageEffect.transform.localPosition = EffectPos[i];
                    break;
                }
                else
                {
                    StageEffect.SetActive(false);
                    continue;
                }
            }
        }
    }
}
