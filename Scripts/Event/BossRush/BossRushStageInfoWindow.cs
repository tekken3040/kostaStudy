using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class BossRushStageInfoWindow : MonoBehaviour
{
    [SerializeField] Text txtBossName;
    [SerializeField] Image imgBossElement;
    [SerializeField] Image imgBossIcon;

    [SerializeField] Text txtRequireLv;
    [SerializeField] Image imgRequireElement;

    [SerializeField] Image imgTier;
    [SerializeField] Text txtTier;
    [SerializeField] Text txtStar;
    [SerializeField] Text txtParts;

    [SerializeField] Text txtKeyCount;

    [SerializeField] GameObject[] objReward;
    [SerializeField] Image[] imgReward;
    [SerializeField] Image[] imgRewardGrade;
    [SerializeField] Text[] txtRewardCnt;

    EventDungeonStageInfo stageInfo;
    StageInfo _stageInfo;
    StringBuilder tempStringBuilder;

    int stageIdx = 0;

    public void SetData(EventDungeonStageInfo info, int idx)
    {
        tempStringBuilder = new StringBuilder();
        stageInfo = info;
        stageIdx = idx;

        _stageInfo = StageInfoMgr.Instance.dicStageData[stageInfo.au2StageID[stageIdx]];
        
        txtBossName.text = TextManager.Instance.GetText(_stageInfo.sName);
        imgBossElement.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_icon_" + _stageInfo.GetBoss().u1Element);
        imgBossElement.SetNativeSize();
        imgBossIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/boss-rush-icon." + stageInfo.au2StageID[stageIdx]);
        txtRequireLv.text = _stageInfo.arrRecommandLevel[0].ToString();
        imgRequireElement.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_icon_" + _stageInfo.u1RecommandElement);
        imgRequireElement.SetNativeSize();

        int needPartsLevel = ForgeInfoMgr.Instance.GetInfo(_stageInfo.smithID[0]).u1Level;
        imgTier.sprite = AtlasMgr.Instance.GetSprite("Sprites/Forge/Forge_01.Forge_01_LevelIcon_" + needPartsLevel.ToString("00"));
        int needPartsCount = _stageInfo.recommendEquipPartsCountKey[0];
        //tempStringBuilder.Remove(0, tempStringBuilder.Length);
        //tempStringBuilder.Append(needPartsLevel).Append(TextManager.Instance.GetText("forge_level_2_" + needPartsLevel));
        txtTier.text = TextManager.Instance.GetText("forge_level_2_" + needPartsLevel);
        txtStar.text = _stageInfo.recommendEquipStringKey[0].Substring(11);
        txtParts.text = TextManager.Instance.GetText(needPartsCount + "part");

        txtKeyCount.text = stageInfo.acConsumeItem[0].u4Count.ToString();

        UInt16 nItemId;
        //int currentSlotIdx = objReward.Length -1;
        int currentSlotIdx = 0;
        for (int i = 0; i < StageInfo.MAX_REWARD_COUNT; ++i)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            nItemId = _stageInfo.RewardItems[0][i].cRewards.u2ID;
            if (nItemId != 0 && i >= StageInfo.CONFIRM_REWORD_COUNT)
            {
                imgReward[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_random_reward");
                imgRewardGrade[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4570");
                //imgReward[currentSlotIdx].SetNativeSize();

                txtRewardCnt[currentSlotIdx].text = "";
                currentSlotIdx++;
                break;
            }
            else if (nItemId != 0)
            {
                imgReward[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(nItemId).u2IconID);
                imgRewardGrade[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(nItemId));
                imgReward[currentSlotIdx].SetNativeSize();
                
                txtRewardCnt[currentSlotIdx].text = _stageInfo.RewardItems[0][i].cRewards.u4Count.ToString();
                currentSlotIdx++;
            }
            else
            {
                switch(_stageInfo.RewardItems[0][i].cRewards.u1Type)
                {
                    case (Byte)GoodsType.GOLD:
                        imgReward[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Gold");
                        break;
                    case (Byte)GoodsType.CASH:
                        imgReward[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Cash");
                        break;
                    case (Byte)GoodsType.FRIENDSHIP_POINT:
                        imgReward[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_friendship");
                        break;
                }
                imgRewardGrade[currentSlotIdx].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4570");
                imgReward[currentSlotIdx].SetNativeSize();
                txtRewardCnt[currentSlotIdx].text = _stageInfo.RewardItems[0][i].cRewards.u4Count.ToString();
                currentSlotIdx++;
            }
        }
    }

    public void OnClickShowItemInfo(int idx)
    {
        if(idx != 2)
            PopupManager.Instance.ShowItemInfo(_stageInfo.RewardItems[0][idx].cRewards.u1Type, _stageInfo.RewardItems[0][idx].cRewards.u2ID, (objReward[idx].transform.root.position - objReward[idx].transform.position), objReward[idx].transform.root.localScale.x);
        else
            PopupManager.Instance.ShowItemInfo((Byte)GoodsType.RANDOM_REWARD, 0, (objReward[idx].transform.root.position - objReward[idx].transform.position), objReward[idx].transform.root.localScale.x);
    }

    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(this.gameObject);
        this.gameObject.SetActive(false);
    }

    int eventIndex = 0;
    UInt16 openCost = 0;
    public void OnClickStart()
    {
        object[] param = new object[1];
        
        openCost = 0;

        int index = Legion.Instance.cEvent.openStageIds.FindIndex(cs => cs.u2EventID == stageInfo.u2EventID && cs.u2StageID == stageInfo.au2StageID[stageIdx]);
        param[0] = index;
        eventIndex = index;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        if(index >= 0)
        {
            switch(Legion.Instance.cEvent.openStageIds[index].u1Closed)
            {
                case 0:
                    //스테이지 언락
                    openCost = (UInt16)(EventInfoMgr.Instance.lstBossRush[0].u2OpenCount + (Legion.Instance.cEvent.openStageIds[index].u1PlayCount*EventInfoMgr.Instance.lstBossRush[0].u1OpenRetryUpCount));
                    if(openCost > EventInfoMgr.Instance.lstBossRush[0].u1OpenMaxCount)
                        openCost = EventInfoMgr.Instance.lstBossRush[0].u1OpenMaxCount;
                    tempStringBuilder.Append(String.Format(TextManager.Instance.GetText("event_bossrush_popup_desc"), openCost));
                    PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("event_bossrush_title"), tempStringBuilder.ToString(), RequestUnlockStage, param);
                    break;

                case 1:
                    //스테이지 언락
                    openCost = (UInt16)(EventInfoMgr.Instance.lstBossRush[0].u2OpenCount + (Legion.Instance.cEvent.openStageIds[index].u1PlayCount*EventInfoMgr.Instance.lstBossRush[0].u1OpenRetryUpCount));
                    if(openCost > EventInfoMgr.Instance.lstBossRush[0].u1OpenMaxCount)
                        openCost = EventInfoMgr.Instance.lstBossRush[0].u1OpenMaxCount;
                    tempStringBuilder.Append(String.Format(TextManager.Instance.GetText("event_bossrush_popup_desc"), openCost));
                    PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("event_bossrush_title"), tempStringBuilder.ToString(), RequestUnlockStage, param);
                    break;
            }
        }
        else
        {
            if(stageInfo.u1OpenType < StageInfoMgr.Instance.OpenBossRush)
            {
                //스테이지 언락
                openCost = (UInt16)(EventInfoMgr.Instance.lstBossRush[0].u2OpenCount);
                if(openCost > EventInfoMgr.Instance.lstBossRush[0].u1OpenMaxCount)
                    openCost = EventInfoMgr.Instance.lstBossRush[0].u1OpenMaxCount;
                tempStringBuilder.Append(String.Format(TextManager.Instance.GetText("event_bossrush_popup_desc"), openCost));
                PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("event_bossrush_title"), tempStringBuilder.ToString(), RequestUnlockStage, param);
                return;
            }
            else if(stageInfo.u1OpenType == StageInfoMgr.Instance.OpenBossRush)
            {
                if(!Legion.Instance.CheckEnoughGoods((int)GoodsType.KEY, stageInfo.acConsumeItem[0].u4Count))
                {
                    tempStringBuilder.Remove(0, tempStringBuilder.Length);
                    tempStringBuilder.Append(TextManager.Instance.GetText("mark_key")).Append(stageInfo.acConsumeItem[0].u4Count).Append(TextManager.Instance.GetText("popup_desc_nocost"));
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_nocost"), tempStringBuilder.ToString(), null);
                    return;
                }
                PopupManager.Instance.ShowLoadingPopup(1);

		        Legion.Instance.u2SelectStageID = stageInfo.au2StageID[stageIdx];
		        Legion.Instance.AUTOCONTINUE = false;
		        Legion.Instance.SelectedDifficult = 1;
		        Server.ServerMgr.Instance.StartStage (Legion.Instance.cBestCrew, StageInfoMgr.Instance.dicStageData [Legion.Instance.u2SelectStageID], Legion.Instance.SelectedDifficult, AckStartStage, false);
            }
        }
    }

    private void AckStartStage(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_START, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			if(stageInfo.acConsumeItem[0].u1Type != (Byte)GoodsType.KEY)
                Legion.Instance.SubGoods (stageInfo.acConsumeItem [0]);
            if(openCost != 0)
                Legion.Instance.SubGoods((int)GoodsType.CASH, openCost);
            Legion.Instance.SelectedBossRushStage = new EventDungeonInfo();
            if(eventIndex >= 0)
            {
                EventDungeonInfo tempInfo = Legion.Instance.cEvent.openStageIds[eventIndex];
                switch(tempInfo.u1Closed)
                {
                    case 0:
                        tempInfo.u1PlayCount++;
                        break;

                    case 1:
                        tempInfo.u1PlayCount++;
                        break;
                }
                Legion.Instance.cEvent.openStageIds.Remove(Legion.Instance.cEvent.openStageIds[eventIndex]);
                Legion.Instance.cEvent.openStageIds.Add(tempInfo);
                Legion.Instance.SelectedBossRushStage = tempInfo;
            }
            else
            {
                EventDungeonInfo tempInfo = new EventDungeonInfo();
                tempInfo.u2EventID = stageInfo.u2EventID;
                tempInfo.u2StageID = stageInfo.au2StageID[stageIdx];
                if(stageInfo.u1OpenType < StageInfoMgr.Instance.OpenBossRush)
                {
                    tempInfo.u1PlayCount++;
                    tempInfo.u1Closed = 1;
                }
                else
                {
                    tempInfo.u1PlayCount = 0;
                    tempInfo.u1Closed = 0;
                }
                Legion.Instance.SelectedBossRushStage = tempInfo;
            }
            Legion.Instance.bAdventoStage = 2;
			StartCoroutine(ChangeScene());
		}
	}

    
    public void RequestUnlockStage(object[] param)
    {
        if(!Legion.Instance.CheckEnoughGoods((int)GoodsType.CASH, openCost))
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetText("mark_cash")).Append(openCost).Append(TextManager.Instance.GetText("popup_desc_nocost"));
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_nocost"), tempStringBuilder.ToString(), null);
            return;
        }
        else if(!Legion.Instance.CheckEnoughGoods((int)GoodsType.KEY, stageInfo.acConsumeItem[0].u4Count))
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetText("mark_key")).Append(stageInfo.acConsumeItem[0].u4Count).Append(TextManager.Instance.GetText("popup_desc_nocost"));
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_nocost"), tempStringBuilder.ToString(), null);
            return;
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        Legion.Instance.u2SelectStageID = stageInfo.au2StageID[stageIdx];
		Legion.Instance.AUTOCONTINUE = false;
		Legion.Instance.SelectedDifficult = 1;
		Server.ServerMgr.Instance.StartStage (Legion.Instance.cBestCrew, StageInfoMgr.Instance.dicStageData [Legion.Instance.u2SelectStageID], Legion.Instance.SelectedDifficult, AckStartStage, false);
    }

    private IEnumerator ChangeScene()
	{
		FadeEffectMgr.Instance.FadeOut(1f);
		yield return new WaitForSeconds(1f);
		AssetMgr.Instance.SceneLoad("Battle");
	}
}
