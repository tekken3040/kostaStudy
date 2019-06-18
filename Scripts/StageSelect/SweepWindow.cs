using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

//소탕 결과를 보여준다
public class SweepWindow : MonoBehaviour {

	public ChapterTab chapterTab;
	public Text title;
	public Text chapterName;
	public Text stageName;
	
	public Text crewName;
	public Image crewFlag;
	public Image crewFlagNumber;
	//Done
	public GameObject resultRewardObj;
	public GameObject[] resultRewards;
	public Image[] resultRewardItem;
	public Image[] resultRewardGrade;
	public Text[] resultRewardCount;
	public RewardButton[] rewardButton;
	
	public Text goldValue;
    public Text ticketValue;
	public Animator animator;
	
	StageInfo stageInfo;
	Crew crew;
    TimeSpan tsLeftTime;
	
	public void SetInfo(UInt16 stageID, int crewIdx)
	{
		animator.Play("Popup_Show");
		
		stageInfo = StageInfoMgr.Instance.dicStageData[stageID];
        
        //크루 정보
		crew = Legion.Instance.acCrews[crewIdx];
		crewFlagNumber.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.num_s_" + (crewIdx + 1));
		crewFlagNumber.SetNativeSize();
		crewName.text = TextManager.Instance.GetText("mark_direction_crew") + (crewIdx + 1);
		title.text = TextManager.Instance.GetText("popup_title_clean");
		if(chapterName != null)
			chapterName.text = TextManager.Instance.GetText(stageInfo.chapterInfo.strName);
		else
			DebugMgr.LogError("actName == null");
		stageName.text = TextManager.Instance.GetText(stageInfo.sName);
        
        //소탕권 표시
		if(Legion.Instance.cInventory.dicItemKey.ContainsKey(Server.ConstDef.TICKET_SWEEP))
		{
			ticketValue.text = ((ConsumableItem)Legion.Instance.cInventory.dicInventory[Legion.Instance.cInventory.dicItemKey[Server.ConstDef.TICKET_SWEEP]]).u2Count.ToString();
		}
		else
		{	
			ticketValue.text = "0";
		}             
        
		SetResultRewards();
        
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);
	}	
	
    public void CheckUnlockBoostEvent()
    {
        foreach (KeyValuePair<UInt16, EventReward> item in EventInfoMgr.Instance.dicEventReward)
        {
            if(item.Value.eventType == (Byte)EVENT_TYPE.BUFF_EXP)
            {
                tsLeftTime = item.Value.dtEventEnd - Legion.Instance.ServerTime;
                UInt16 _eventID = item.Key;
                if(tsLeftTime.TotalSeconds <= 0)
                {
                    EventInfoMgr.Instance.u4ExpBoostPer = 0;
                    EventInfoMgr.Instance.dicEventReward.Remove(_eventID);
                    break;
                }
                EventInfoMgr.Instance.u4ExpBoostPer = item.Value.recordValue;
                break;
            }
        }

        foreach (KeyValuePair<UInt16, EventReward> item in EventInfoMgr.Instance.dicEventReward)
        {
            if(item.Value.eventType == (Byte)EVENT_TYPE.BUFF_GOLD)
            {
                tsLeftTime = item.Value.dtEventEnd - Legion.Instance.ServerTime;
                UInt16 _eventID = item.Key;
                if(tsLeftTime.TotalSeconds <= 0)
                {
                    EventInfoMgr.Instance.u4GoldBoostPer = 0;
                    EventInfoMgr.Instance.dicEventReward.Remove(_eventID);
                    break;
                }
                EventInfoMgr.Instance.u4GoldBoostPer = item.Value.recordValue;
                break;
            }
        }
    }

	private void SetResultRewards()
	{
		RewardItem[] rewardItem = Legion.Instance.cReward.GetReward();
		CheckUnlockBoostEvent();
        Dictionary<UInt16, Goods> dicRewardInfo = new Dictionary<UInt16, Goods>();
        for(int i = 0; i < rewardItem.Length; i++)
        {
            if(dicRewardInfo.ContainsKey(rewardItem[i].cRewards.u2ID))
                dicRewardInfo[rewardItem[i].cRewards.u2ID].u4Count += rewardItem[i].cRewards.u4Count;
            else
                dicRewardInfo.Add(rewardItem[i].cRewards.u2ID, rewardItem[i].cRewards);
        }

        int count = 0;
        foreach (Goods goods in dicRewardInfo.Values)
        {
            if(count >= resultRewards.Length)
                continue;

            switch((GoodsType)goods.u1Type)
            {
                case GoodsType.MATERIAL:
                    resultRewardItem[count].sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(goods.u2ID).u2IconID);
                    resultRewardGrade[count].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(goods.u2ID));
                    break;
                case GoodsType.EVENT_ITEM:
                    resultRewardItem[count].sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + EventInfoMgr.Instance.dicMarbleGoods[goods.u2ID].u2IconID);
                    break;
                default:
                    DebugMgr.LogError("보상 아이템 타입이 이상함 TYPE = " + goods.u1Type + "\n 아이템 ID = " + goods.u2ID);
                    continue;
            }
            
            resultRewardGrade[count].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(goods.u2ID));
            resultRewardCount[count].text = goods.u4Count.ToString();
            rewardButton[count].SetButton(goods.u1Type, goods.u2ID);
            count++;
        }

        for (int i=0; i<rewardButton.Length; i++)
		{
			if(i < count)
				rewardButton[i].gameObject.SetActive(true);
			else
				rewardButton[i].gameObject.SetActive(false);
		}
		
		byte selectedDifficult = Legion.Instance.SelectedDifficult;
        if(StageInfoMgr.Instance.u4TotalGold == 0)
		    goldValue.text = stageInfo.acGetGoods[selectedDifficult-1].u4Count.ToString();
        else
            goldValue.text = (StageInfoMgr.Instance.u4TotalGold - StageInfoMgr.Instance.u4PrevTotalGold).ToString();

        if(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1].u1Type == (Byte)GoodsType.GOLD)
        {
            if(StageInfoMgr.Instance.u4TotalGold != 0)
                Legion.Instance.Gold = StageInfoMgr.Instance.u4TotalGold;
            else
                Legion.Instance.AddGoods(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1]);
        }
        else
            Legion.Instance.AddGoods(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1]);
        //Legion.Instance.AddGoods(stageInfo.acGetGoods[selectedDifficult-1]);

        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.Sweep);
        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.CollectItem);
        //Legion.Instance.cQuest.CheckEndDirection (AchievementTypeData.CollectItem);
    }
	
	public void OnClickRepeat()
	{
		if(!Legion.Instance.CheckEmptyInven())
		{
			return;
		}

		if(chapterTab != null)
		{
			gameObject.SetActive(false);
			chapterTab.OnClickSweep();
		}	
	}
	
	public void OnClickClose()
	{
        PopupManager.Instance.RemovePopup(gameObject);
		gameObject.SetActive(false);
		//Legion.Instance.cQuest.CheckEndDirection (AchievementTypeData.Sweep);
	}
}
